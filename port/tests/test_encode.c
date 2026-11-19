/*
 * test_encode.c - encoding jobs end to end
 *
 * Usage: test_encode IFME_ROOT WORK_DIR
 * Exits 77 (CTest "skipped") when the root, x265 or the sample is missing.
 *
 *   1. ifme_job_new() fills defaults from the source and the x265 plugin
 *   2. a short x265 encode (trimmed, downscaled) produces the right file
 *   3. stopping a running encode returns STOPPED, removes the temp folder
 *      and leaves no output behind
 *   4. bad jobs give clear errors (unknown plugin, mode, bit depth, input)
 */

#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>

#include "ifme.h"
#include "cJSON.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
static void sleep_ms(int ms) { Sleep((DWORD)ms); }
#else
#include <unistd.h>
static void sleep_ms(int ms) { usleep((useconds_t)ms * 1000); }
#endif

static int failures;
#define CHECK(cond) do { if (!(cond)) { failures++; \
	fprintf(stderr, "%s:%d: CHECK failed: %s\n", __FILE__, __LINE__, #cond); } } while (0)

static bool exists(const char *path)
{
	struct stat st;
	return stat(path, &st) == 0;
}

static char *join(const char *a, const char *b)
{
	size_t n = strlen(a) + strlen(b) + 2;
	char *s = malloc(n);
	snprintf(s, n, "%s/%s", a, b);
	return s;
}

/* Edit a job: set a string or number field at top level or in video[0]. */
static char *edit_job(const char *job, const char *where, const char *key, const char *str, double num)
{
	cJSON *j = cJSON_Parse(job);
	cJSON *target = strcmp(where, "video") == 0
		? cJSON_GetArrayItem(cJSON_GetObjectItem(j, "video"), 0) : j;
	cJSON_DeleteItemFromObjectCaseSensitive(target, key);
	if (str)
		cJSON_AddStringToObject(target, key, str);
	else
		cJSON_AddNumberToObject(target, key, num);
	char *out = cJSON_Print(j);
	cJSON_Delete(j);
	return out;
}

static char *add_trim(const char *job, const char *start, const char *duration)
{
	cJSON *j = cJSON_Parse(job);
	cJSON *t = cJSON_AddObjectToObject(j, "trim");
	cJSON_AddStringToObject(t, "start", start);
	cJSON_AddStringToObject(t, "duration", duration);
	char *out = cJSON_Print(j);
	cJSON_Delete(j);
	return out;
}

static atomic_llong frame = -1;
static atomic_int stages;

static void on_progress(void *user, const ifme_progress *p)
{
	(void)user;
	atomic_store(&frame, p->frame);
}

static void on_stage(void *user, const char *stage)
{
	(void)user;
	atomic_fetch_add(&stages, 1);
	printf("  stage: %s\n", stage);
}

typedef struct job_run {
	ifme_ctx *ctx;
	ifme_task *task;
	const char *json;
	ifme_task_result result;
	char err[1024];
} job_run;

static void *worker(void *arg)
{
	job_run *r = arg;
	ifme_task_callbacks cb = { .on_progress = on_progress, .on_stage = on_stage };
	r->result = ifme_encode(r->task, r->ctx, r->json, &cb, r->err, sizeof(r->err));
	return NULL;
}

static ifme_task_result encode_now(ifme_ctx *ctx, const char *json, char *err, size_t len)
{
	ifme_task *t = ifme_task_create();
	ifme_task_callbacks cb = { .on_progress = on_progress, .on_stage = on_stage };
	ifme_task_result r = ifme_encode(t, ctx, json, &cb, err, len);
	ifme_task_destroy(t);
	return r;
}

int main(int argc, char **argv)
{
	if (argc < 3 || !argv[1][0]) {
		printf("skipped: no IFME root given (configure with -DIFME_TEST_ROOT=...)\n");
		return 77;
	}

	const char *root = argv[1], *work = argv[2];
	char *sample = join(root, "Samples/wonderland_online_dungeon.mp4");
	if (!exists(sample)) {
		printf("skipped: %s missing\n", sample);
		return 77;
	}

	ifme_ctx *ctx = ifme_create();
	CHECK(ctx && ifme_set_root_dir(ctx, root));

	char err[1024];
	if (ifme_plugin_find(ctx, IFME_PLUGIN_VIDEO, "x265", err, sizeof(err)) < 0) {
		printf("skipped: %s\n", err);
		return 77;
	}

	/* ---- 1. defaults ---------------------------------------------------------- */
	char *job = ifme_job_new(ctx, sample, "x265", err, sizeof(err));
	CHECK(job != NULL);
	if (!job) {
		fprintf(stderr, "%s\n", err);
		return 1;
	}
	cJSON *j = cJSON_Parse(job);
	cJSON *v = cJSON_GetArrayItem(cJSON_GetObjectItem(j, "video"), 0);
	CHECK(strcmp(cJSON_GetObjectItem(v, "plugin")->valuestring, "deadbeef-0265-0265-0265-026502650265") == 0);
	CHECK(cJSON_GetObjectItem(v, "width")->valueint == 1280);
	CHECK(cJSON_GetObjectItem(v, "height")->valueint == 800);
	CHECK(strcmp(cJSON_GetObjectItem(v, "fps")->valuestring, "15") == 0);
	CHECK(strcmp(cJSON_GetObjectItem(v, "preset")->valuestring, "medium") == 0);
	CHECK(cJSON_GetObjectItem(v, "_help") != NULL);
	cJSON_Delete(j);

	/* ---- 2. a short encode ----------------------------------------------------- */
	char *out = join(work, "test encode 出力.mkv");
	remove(out);
	char *j2 = edit_job(job, "", "output", out, 0);
	char *j3 = edit_job(j2, "video", "width", NULL, 320);
	char *j4 = edit_job(j3, "video", "height", NULL, 200);
	char *j5 = edit_job(j4, "video", "preset", "ultrafast", 0);
	char *short_job = add_trim(j5, "1", "3");

	CHECK(encode_now(ctx, short_job, err, sizeof(err)) == IFME_TASK_DONE);
	if (err[0])
		printf("  error: %s\n", err);
	CHECK(exists(out));

	ifme_media *m = ifme_probe(ctx, out, NULL, err, sizeof(err));
	CHECK(m != NULL);
	if (m) {
		CHECK(m->video_count == 1);
		CHECK(m->video_count == 1 && strcmp(m->video[0].codec, "hevc") == 0);
		CHECK(m->video_count == 1 && m->video[0].width == 320 && m->video[0].height == 200);
		CHECK(m->duration > 2.9 && m->duration < 3.1);
		printf("encoded:  %s %dx%d %.3f s\n", m->video_count ? m->video[0].codec : "?",
		       m->video_count ? m->video[0].width : 0, m->video_count ? m->video[0].height : 0, m->duration);
		ifme_media_free(m);
	}

	/* ---- 3. stop while encoding ------------------------------------------------- */
	char *temp = join(work, "ifme-test-temp");
	char *out2 = join(work, "stopped.mkv");
	remove(out2);
	char *s1 = edit_job(job, "", "output", out2, 0);
	char *s2 = edit_job(s1, "", "temp_dir", temp, 0);
	char *slow = edit_job(s2, "video", "preset", "veryslow", 0);

	job_run r = { ctx, ifme_task_create(), slow, IFME_TASK_ERROR, "" };
	atomic_store(&frame, -1);
	pthread_t th;
	pthread_create(&th, NULL, worker, &r);

	for (int i = 0; i < 600 && atomic_load(&frame) < 5; i++)
		sleep_ms(50);
	CHECK(atomic_load(&frame) >= 5);
	CHECK(exists(temp));
	ifme_task_stop(r.task);
	pthread_join(th, NULL);

	printf("stopped:  result %d (%s), temp %s, output %s\n", (int)r.result, r.err,
	       exists(temp) ? "LEFT BEHIND" : "removed", exists(out2) ? "EXISTS" : "none");
	CHECK(r.result == IFME_TASK_STOPPED);
	CHECK(!exists(temp));
	CHECK(!exists(out2));
	ifme_task_destroy(r.task);

	/* ---- 4. bad jobs -------------------------------------------------------------- */
	char *b1 = edit_job(job, "video", "plugin", "no-such-encoder", 0);
	CHECK(encode_now(ctx, b1, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad plugin:    %s\n", err);
	CHECK(strstr(err, "no-such-encoder") != NULL);

	char *b2 = edit_job(job, "video", "mode", NULL, 9);
	CHECK(encode_now(ctx, b2, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad mode:      %s\n", err);

	char *b3 = edit_job(job, "video", "bit_depth", NULL, 16);
	CHECK(encode_now(ctx, b3, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad bit depth: %s\n", err);
	CHECK(strstr(err, "8, 10, 12") != NULL);

	char *b4 = edit_job(job, "", "input", "Z:/missing/file.mkv", 0);
	CHECK(encode_now(ctx, b4, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad input:     %s\n", err);

	CHECK(encode_now(ctx, "{ not json", err, sizeof(err)) == IFME_TASK_ERROR);

	remove(out);
	cJSON_free(b1); cJSON_free(b2); cJSON_free(b3); cJSON_free(b4);
	cJSON_free(s1); cJSON_free(s2); cJSON_free(slow);
	cJSON_free(j2); cJSON_free(j3); cJSON_free(j4); cJSON_free(j5); cJSON_free(short_job);
	ifme_free(job);
	free(out); free(out2); free(temp); free(sample);
	ifme_destroy(ctx);

	printf("%s\n", failures ? "FAILED" : "OK");
	return failures ? 1 : 0;
}
