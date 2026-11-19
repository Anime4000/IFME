/*
 * exec_ui.c - ifmecli --exec: run a pipeline with a live progress line
 *
 * Keys (when stdin is a terminal): p = pause, r = resume, q = stop.
 * Ctrl+C stops the whole pipeline cleanly on every platform.
 *
 * License: GPL-2.0
 */

#if !defined(_WIN32) && !defined(_POSIX_C_SOURCE)
#define _POSIX_C_SOURCE 200809L
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "ifme.h"
#include "exec_ui.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <conio.h>
#include <io.h>
#define isatty _isatty
#define fileno _fileno
#else
#include <signal.h>
#include <termios.h>
#include <time.h>
#include <unistd.h>
#endif

/* The task is reachable from the Ctrl+C handler, which may only touch it. */
static ifme_task *volatile g_task;

typedef struct ui {
	bool tty_out;            /* draw a status line with \r            */
	bool keys;               /* read p/r/q from the keyboard          */
	bool status_drawn;
	bool have_progress;
	ifme_progress last;
	char raw_status[160];    /* progress-like line we could not parse */
	unsigned long long last_draw, last_plain;
} ui;

static unsigned long long now_ms(void)
{
#ifdef _WIN32
	return GetTickCount64();
#else
	struct timespec ts;
	clock_gettime(CLOCK_MONOTONIC, &ts);
	return (unsigned long long)ts.tv_sec * 1000u + (unsigned long long)ts.tv_nsec / 1000000u;
#endif
}

/* ---- keyboard ------------------------------------------------------------- */

#ifdef _WIN32

static BOOL WINAPI on_console_ctrl(DWORD type)
{
	(void)type;
	if (g_task)
		ifme_task_stop(g_task);
	return TRUE;   /* handled: don't kill ifmecli itself */
}

static bool keys_begin(void)
{
	SetConsoleCtrlHandler(on_console_ctrl, TRUE);
	DWORD mode;
	return GetConsoleMode(GetStdHandle(STD_INPUT_HANDLE), &mode) != 0;
}

static int key_poll(void)
{
	return _kbhit() ? _getch() : -1;
}

static void keys_end(void)
{
	SetConsoleCtrlHandler(on_console_ctrl, FALSE);
}

#else

static struct termios g_saved_tio;
static bool g_tio_saved;

static void on_signal(int sig)
{
	(void)sig;
	if (g_task)
		ifme_task_stop(g_task);   /* only an atomic store: signal-safe */
}

static bool keys_begin(void)
{
	struct sigaction sa;
	memset(&sa, 0, sizeof(sa));
	sa.sa_handler = on_signal;
	sigemptyset(&sa.sa_mask);
	sigaction(SIGINT, &sa, NULL);
	sigaction(SIGTERM, &sa, NULL);

	if (!isatty(STDIN_FILENO) || tcgetattr(STDIN_FILENO, &g_saved_tio) != 0)
		return false;

	/* Unbuffered, no echo, non-blocking reads; keep ISIG so Ctrl+C works. */
	struct termios raw = g_saved_tio;
	raw.c_lflag &= ~(tcflag_t)(ICANON | ECHO);
	raw.c_cc[VMIN] = 0;
	raw.c_cc[VTIME] = 0;
	g_tio_saved = tcsetattr(STDIN_FILENO, TCSANOW, &raw) == 0;
	return g_tio_saved;
}

static int key_poll(void)
{
	unsigned char c;
	return read(STDIN_FILENO, &c, 1) == 1 ? c : -1;
}

static void keys_end(void)
{
	if (g_tio_saved)
		tcsetattr(STDIN_FILENO, TCSANOW, &g_saved_tio);
	g_tio_saved = false;
	signal(SIGINT, SIG_DFL);
	signal(SIGTERM, SIG_DFL);
}

#endif

/* ---- drawing -------------------------------------------------------------- */

static void format_eta(long long sec, char *buf, size_t len)
{
	if (sec < 0)
		snprintf(buf, len, "--:--:--");
	else
		snprintf(buf, len, "%02lld:%02lld:%02lld", sec / 3600, (sec / 60) % 60, sec % 60);
}

static void status_text(const ui *u, bool paused, char *buf, size_t len)
{
	const char *state = paused ? "PAUSED  " : "";

	if (!u->have_progress) {
		snprintf(buf, len, "%s%s", state, u->raw_status[0] ? u->raw_status : "starting...");
		return;
	}

	const ifme_progress *p = &u->last;
	char pct[16] = "", total[32] = "", fps[48] = "", kbps[48] = "", eta[64];

	if (p->percent >= 0)
		snprintf(pct, sizeof(pct), "[%5.1f%%] ", p->percent);
	if (p->total_frames > 0)
		snprintf(total, sizeof(total), "/%lld", p->total_frames);
	if (p->fps >= 0)
		snprintf(fps, sizeof(fps), "  %.2f fps", p->fps);
	if (p->kbps >= 0)
		snprintf(kbps, sizeof(kbps), "  %.0f kb/s", p->kbps);
	format_eta(p->eta_sec, eta, sizeof(eta));

	snprintf(buf, len, "%s%sframe %lld%s%s%s  ETA %s", state, pct, p->frame, total, fps, kbps, eta);
}

static void clear_status(ui *u)
{
	if (u->tty_out && u->status_drawn) {
		printf("\r%-79s\r", "");
		u->status_drawn = false;
	}
}

static void draw_status(ui *u, bool force)
{
	unsigned long long t = now_ms();
	char text[200];
	bool paused = g_task && ifme_task_is_paused(g_task);

	status_text(u, paused, text, sizeof(text));

	if (u->tty_out) {
		if (!force && t - u->last_draw < 100)
			return;
		u->last_draw = t;

		char line[256];
		snprintf(line, sizeof(line), "%s%s", text, u->keys ? "   [p]ause [r]esume [q]uit" : "");
		printf("\r%-79.79s", line);
		u->status_drawn = true;
	} else {
		/* Not a terminal (log file, CI): one plain line every 2 s, and only
		 * once there is something to say. */
		if (!u->have_progress && !u->raw_status[0] && !paused)
			return;
		if (!force && t - u->last_plain < 2000)
			return;
		u->last_plain = t;
		printf("%s\n", text);
	}
	fflush(stdout);
}

/* ---- callbacks ------------------------------------------------------------ */

static void on_output(void *user, const char *line, bool is_status)
{
	ui *u = user;

	if (is_status) {
		if (!u->have_progress)
			snprintf(u->raw_status, sizeof(u->raw_status), "%s", line);
		return;
	}

	clear_status(u);
	printf("%s\n", line);
	fflush(stdout);
}

static void on_progress(void *user, const ifme_progress *p)
{
	ui *u = user;
	u->last = *p;
	u->have_progress = true;
}

static void on_tick(void *user)
{
	ui *u = user;
	int c;

	while (u->keys && (c = key_poll()) >= 0) {
		switch (c) {
		case 'p': case 'P': ifme_task_pause(g_task); break;
		case 'r': case 'R': ifme_task_resume(g_task); break;
		case 'q': case 'Q': case 27: ifme_task_stop(g_task); break;
		default: break;
		}
	}

	draw_status(u, false);
}

/* Close the current status line: the last progress line an encoder prints
 * comes before its last frame, so a clean finish is shown as 100 %. */
static void finish_status(ui *u, bool done)
{
	if (done && u->have_progress && u->last.total_frames > 0) {
		u->last.frame = u->last.total_frames;
		u->last.percent = 100;
		u->last.eta_sec = 0;
	}

	if (u->have_progress || u->raw_status[0])
		draw_status(u, true);
	if (u->tty_out && u->status_drawn)
		printf("\n");
	u->status_drawn = false;
}

static void on_stage(void *user, const char *stage)
{
	ui *u = user;

	/* A new step starts, so the previous one finished. */
	finish_status(u, true);
	u->have_progress = false;
	u->raw_status[0] = '\0';

	printf("==> %s\n", stage);
	fflush(stdout);
}

/* ---- entry points ----------------------------------------------------------- */

typedef struct run_args {
	ifme_ctx *ctx;
	char **argv;
	int argc;
	long long total_frames;
	const char *job_json;
} run_args;

static int run_ui(const run_args *ra)
{
	ifme_task *task = ifme_task_create();
	if (!task) {
		fprintf(stderr, "ifmecli: out of memory\n");
		return 1;
	}

	ui u;
	memset(&u, 0, sizeof(u));
	u.tty_out = isatty(fileno(stdout)) != 0;

	g_task = task;
	u.keys = keys_begin();

	ifme_task_callbacks cb = { .on_output = on_output, .on_progress = on_progress,
	                          .on_tick = on_tick, .on_stage = on_stage, .user = &u };
	char err[1024];
	int exit_code = -1;
	unsigned long long t0 = now_ms();
	ifme_task_result r;

	if (ra->job_json)
		r = ifme_encode(task, ra->ctx, ra->job_json, &cb, err, sizeof(err));
	else
		r = ifme_task_exec(task, ra->ctx, (const char *const *)ra->argv, (size_t)ra->argc,
		                   NULL, ra->total_frames, &cb, &exit_code, err, sizeof(err));

	finish_status(&u, r == IFME_TASK_DONE);

	keys_end();
	g_task = NULL;
	ifme_task_destroy(task);

	double secs = (double)(now_ms() - t0) / 1000.0;

	switch (r) {
	case IFME_TASK_DONE:
		printf("Done in %.1f s\n", secs);
		return 0;
	case IFME_TASK_STOPPED:
		printf("Stopped after %.1f s\n", secs);
		return 130;
	case IFME_TASK_FAILED:
		fprintf(stderr, "ifmecli: failed after %.1f s: %s\n", secs, err);
		return exit_code > 0 && exit_code < 126 ? exit_code : 1;
	default:
		fprintf(stderr, "ifmecli: %s\n", err);
		return 1;
	}
}

int run_exec(ifme_ctx *ctx, char **argv, int argc, long long total_frames)
{
	run_args ra = { ctx, argv, argc, total_frames, NULL };
	return run_ui(&ra);
}

int run_encode(ifme_ctx *ctx, const char *job_json)
{
	run_args ra = { ctx, NULL, 0, 0, job_json };
	return run_ui(&ra);
}
