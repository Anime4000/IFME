/*
 * test_task.c - pause / resume / stop on a real ffmpeg | x265 pipeline
 *
 * Usage: test_task IFME_ROOT   (folder with Plugins/ffmpeg64 and Plugins/h265_x265)
 * Exits 77 (CTest "skipped") when the root or the encoders are missing.
 *
 * A worker thread runs ifme_task_exec(); this thread controls it:
 *   1. wait for progress, pause, and check that no frame is encoded for 2 s
 *   2. resume and check that encoding continues
 *   3. stop and check the call returns IFME_TASK_STOPPED within 3 s
 * Plus: stop-before-exec, a failing command, a missing program.
 */

#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>

#include "ifme.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
static void sleep_ms(int ms) { Sleep((DWORD)ms); }
static const char *X265 = "Plugins/h265_x265/x265.exe";
#else
#include <unistd.h>
static void sleep_ms(int ms) { usleep((useconds_t)ms * 1000); }
static const char *X265 = "Plugins/h265_x265/x265";
#endif

static int failures;
#define CHECK(cond) do { if (!(cond)) { failures++; \
	fprintf(stderr, "%s:%d: CHECK failed: %s\n", __FILE__, __LINE__, #cond); } } while (0)

static atomic_llong frame = -1;
static atomic_int progress_events;

static void on_progress(void *user, const ifme_progress *p)
{
	(void)user;
	atomic_store(&frame, p->frame);
	atomic_fetch_add(&progress_events, 1);
}

typedef struct job {
	ifme_ctx *ctx;
	ifme_task *task;
	const char *const *argv;
	size_t argc;
	ifme_task_result result;
	char err[256];
	int exit_code;
} job;

static void *worker(void *arg)
{
	job *j = arg;
	ifme_task_callbacks cb = { .on_progress = on_progress };
	j->result = ifme_task_exec(j->task, j->ctx, j->argv, j->argc, NULL, 0, &cb,
	                           &j->exit_code, j->err, sizeof(j->err));
	return NULL;
}

static double now_s(void)
{
	struct timespec ts;
	clock_gettime(CLOCK_MONOTONIC, &ts);
	return (double)ts.tv_sec + (double)ts.tv_nsec / 1e9;
}

static bool wait_for(bool (*cond)(long long), long long arg, double seconds)
{
	double end = now_s() + seconds;
	while (now_s() < end) {
		if (cond(arg))
			return true;
		sleep_ms(20);
	}
	return cond(arg);
}

static bool frame_at_least(long long n) { return atomic_load(&frame) >= n; }
static bool frame_above(long long n) { return atomic_load(&frame) > n; }

static char *join(const char *a, const char *b)
{
	size_t n = strlen(a) + strlen(b) + 2;
	char *s = malloc(n);
	snprintf(s, n, "%s/%s", a, b);
	return s;
}

static FILE *open_check(const char *path)
{
	return fopen(path, "rb");
}

int main(int argc, char **argv)
{
	if (argc < 2 || !argv[1][0]) {
		printf("skipped: no IFME root given (configure with -DIFME_TEST_ROOT=...)\n");
		return 77;
	}

	const char *root = argv[1];
	char *x265 = join(root, X265);
	char *sample = join(root, "Samples/wonderland_online_dungeon.mp4");

	FILE *f1 = open_check(x265), *f2 = open_check(sample);
	if (!f1 || !f2) {
		printf("skipped: %s or %s missing\n", x265, sample);
		return 77;
	}
	fclose(f1);
	fclose(f2);

	ifme_ctx *ctx = ifme_create();
	CHECK(ctx && ifme_set_root_dir(ctx, root));

	/* ---- 1-3: pause, resume, stop on a long-running encode ---------------- */
	const char *enc[] = {
		"ffmpeg", "-hide_banner", "-v", "error", "-stream_loop", "20", "-i", sample,
		"-strict", "-1", "-f", "yuv4mpegpipe", "-",
		"|",
		x265, "--y4m", "-", "--preset", "slow", "-o",
#ifdef _WIN32
		"NUL"
#else
		"/dev/null"
#endif
	};

	job j = { ctx, ifme_task_create(), enc, sizeof(enc) / sizeof(enc[0]), IFME_TASK_ERROR, "", -1 };
	pthread_t th;
	pthread_create(&th, NULL, worker, &j);

	CHECK(wait_for(frame_at_least, 20, 60));
	printf("encoding: frame %lld\n", (long long)atomic_load(&frame));

	ifme_task_pause(j.task);
	sleep_ms(300);   /* let the runner apply it and drain what was already written */
	CHECK(ifme_task_is_paused(j.task));
	long long paused_at = atomic_load(&frame);
	sleep_ms(2000);
	long long after_pause = atomic_load(&frame);
	printf("paused:   frame %lld -> %lld over 2 s\n", paused_at, after_pause);
	CHECK(after_pause == paused_at);

	ifme_task_resume(j.task);
	CHECK(wait_for(frame_above, after_pause, 20));
	CHECK(!ifme_task_is_paused(j.task));
	printf("resumed:  frame %lld\n", (long long)atomic_load(&frame));

	/* Stop while paused: must still end promptly. */
	ifme_task_pause(j.task);
	sleep_ms(200);
	double t0 = now_s();
	ifme_task_stop(j.task);
	pthread_join(th, NULL);
	double took = now_s() - t0;
	printf("stopped:  returned after %.2f s (%s)\n", took, j.err);
	CHECK(j.result == IFME_TASK_STOPPED);
	CHECK(took < 3.0);
	ifme_task_destroy(j.task);

	/* ---- stop before exec --------------------------------------------------- */
	ifme_task *t = ifme_task_create();
	ifme_task_stop(t);
	const char *quick[] = { x265, "--version" };
	char err[256];
	CHECK(ifme_task_exec(t, ctx, quick, 2, NULL, 0, NULL, NULL, err, sizeof(err)) == IFME_TASK_STOPPED);
	ifme_task_destroy(t);

	/* ---- failing command and missing program ------------------------------- */
	t = ifme_task_create();
	const char *bad[] = { x265, "--no-such-option" };
	int code = 0;
	CHECK(ifme_task_exec(t, ctx, bad, 2, NULL, 0, NULL, &code, err, sizeof(err)) == IFME_TASK_FAILED);
	CHECK(code != 0);
	printf("failing:  exit code %d\n", code);

	const char *missing[] = { "definitely-not-a-program-ifme" };
	ifme_task_result r = ifme_task_exec(t, ctx, missing, 1, NULL, 0, NULL, &code, err, sizeof(err));
	printf("missing:  result %d (%s)\n", (int)r, err);
	CHECK(r == IFME_TASK_ERROR);
	ifme_task_destroy(t);

	ifme_destroy(ctx);
	free(x265);
	free(sample);

	printf("%s\n", failures ? "FAILED" : "OK");
	return failures ? 1 : 0;
}
