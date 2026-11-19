/*
 * internal.h - shared between core modules, not part of the public API
 */

#ifndef IFME_INTERNAL_H
#define IFME_INTERNAL_H

#include <stdarg.h>
#include <stdatomic.h>
#include <stdint.h>

#include "ifme.h"

struct ifme_plugin;

struct ifme_ctx {
	char *app_dir;
	char *config_dir;
	char *root_dir;              /* holds Plugins/ and Samples/; defaults to app_dir */
	ifme_cpu_info cpu;

	struct ifme_plugin **plugins;
	size_t plugin_count;
	bool plugins_loaded;
};

/* ---- platform/paths.c ---------------------------------------------------- */
/* Both return malloc'd UTF-8 strings, or NULL on failure. */
char *ifme_platform_app_dir(void);
char *ifme_platform_config_dir(void);

/* ---- platform/cpu_x86.c or platform/cpu_other.c ------------------------- */
void ifme_platform_cpu(ifme_cpu_info *info);

/* ---- core.c: string helpers --------------------------------------------- */
char *ifme_strdup(const char *s);
char *ifme_strdupf(const char *fmt, ...);
char *ifme_vstrdupf(const char *fmt, va_list ap);
char *ifme_path_join(const char *a, const char *b);
int   ifme_strcasecmp(const char *a, const char *b);   /* ASCII only */
bool  ifme_starts_with_ci(const char *s, const char *prefix);
bool  ifme_ends_with_ci(const char *s, const char *suffix);

/* ---- argv.c: argument vectors ------------------------------------------- */
typedef struct ifme_argv {
	char **v;      /* NULL-terminated when n > 0 */
	size_t n, cap;
} ifme_argv;

void  ifme_argv_free(ifme_argv *a);
bool  ifme_argv_push(ifme_argv *a, const char *s);
bool  ifme_argv_pushf(ifme_argv *a, const char *fmt, ...);
/* Split a manifest template like `-vf "scale=1:2" -y` into arguments; quotes
 * are removed. NULL or blank adds nothing. */
bool  ifme_argv_split(ifme_argv *a, const char *tmpl);
/* Human-readable command line, for logs only (never executed). Caller frees. */
char *ifme_argv_display(const ifme_argv *a);

/* IFME's "this argument is switched off" values: NULL, blank, disable(d),
 * blank, none, null, nan, non, off, no, 0 (same as Extensions.IsDisable). */
bool ifme_is_disabled(const char *s);

/* ---- json_relax.c --------------------------------------------------------- */
/* Turn Newtonsoft-tolerated JSON into strict JSON: strips a UTF-8 BOM,
 * // and block comments, and trailing commas. Caller frees. */
char *ifme_json_relax(const char *in);

/* ---- platform/fs.c -------------------------------------------------------- */
char     *ifme_read_file(const char *path, size_t *len);   /* NUL-terminated */
bool      ifme_write_file(const char *path, const void *data, size_t len);
bool      ifme_copy_file(const char *from, const char *to); /* whole file in memory */
bool      ifme_file_exists(const char *path);               /* regular file  */
bool      ifme_dir_exists(const char *path);
long long ifme_file_size(const char *path);                 /* -1 if missing */
bool      ifme_path_is_absolute(const char *path);
char     *ifme_full_path(const char *path);                 /* normalise ".." */
/* A runnable file at exactly this path; on Windows also tries ".exe". */
char     *ifme_resolve_program(const char *path);
/* Look a bare program name up in PATH. NULL if absent or name has a separator. */
char     *ifme_find_in_path(const char *name);
char     *ifme_temp_dir(void);
bool      ifme_mkdir(const char *path);
void      ifme_clear_dir(const char *path);                 /* files only     */
void      ifme_rmdir(const char *path);
unsigned long ifme_pid(void);
uint64_t  ifme_now_ms(void);                                /* monotonic      */

typedef void (*ifme_file_cb)(void *user, const char *path);
/* Recursively call cb for every regular file under root. */
void ifme_walk_files(const char *root, ifme_file_cb cb, void *user);

#ifdef _WIN32
#include <wchar.h>
wchar_t *ifme_utf8_to_wide(const char *s);
char    *ifme_wide_to_utf8(const wchar_t *w);
#endif

/* ---- subtime.c: cut text subtitles and chapters to a trimmed part ----------- */
/* Shift a text subtitle (format "srt", "vtt", "ass" or "ssa") by -start, clip
 * every event to [0, duration] and drop events outside it. *kept: events
 * left. Caller frees; NULL when out of memory. */
char *ifme_subtitle_cut(const char *text, size_t len, const char *format,
                        double start, double duration, size_t *kept);
/* An ffmetadata text without zero-length chapters (left by a trim). */
char *ifme_chapters_drop_empty(const char *text, size_t len, size_t *dropped);

/* ---- plugins.c ------------------------------------------------------------ */
struct cJSON;

void ifme_plugins_clear(ifme_ctx *ctx);

/* The resolved manifest (encoder paths are absolute) of plugin `index`. */
const struct cJSON *ifme_plugin_json(const ifme_ctx *ctx, size_t index);

/* An FFmpeg tool ("ffmpeg", "ffprobe"): PATH first, then the bundled copy in
 * <root>/Plugins/ffmpeg64 (ffmpeg32 on 32-bit x86). Caller frees. */
char *ifme_find_ff_tool(const ifme_ctx *ctx, const char *name);

/* Any tool: PATH first, then <root>/Plugins/<subdir>/<name>. Caller frees. */
char *ifme_find_tool(const ifme_ctx *ctx, const char *name, const char *subdir);

/* Audio rate-control arguments of a manifest's Mode entry for `quality`
 * (self-test and real encodes use the same rule). */
void ifme_audio_quality_args(ifme_argv *out, const struct cJSON *mode, const char *quality);

/* ---- jsonutil.c: lenient accessors (case-insensitive keys, numbers may be
 * strings, as Newtonsoft accepted) ---------------------------------------- */
const struct cJSON *ifme_jget(const struct cJSON *o, const char *key);
const char *ifme_jstr(const struct cJSON *o, const char *key);          /* NULL if not a string */
bool        ifme_jnum(const struct cJSON *o, const char *key, double *out);
int         ifme_jint(const struct cJSON *o, const char *key, int def);
double      ifme_jdouble(const struct cJSON *o, const char *key, double def);
bool        ifme_jbool(const struct cJSON *o, const char *key, bool def);
char       *ifme_jscalar(const struct cJSON *o, const char *key);       /* caller frees */
const struct cJSON *ifme_jitem(const struct cJSON *o, const char *key, int index);

/* ---- fs.c (more) ------------------------------------------------------------ */
/* Rename, or copy + delete across drives; replaces `to`. */
bool ifme_move_file(const char *from, const char *to);
bool ifme_delete_file(const char *path);
/* File name without folder; points into `path`. */
const char *ifme_basename(const char *path);
/* Absolute version of a path that may not exist yet. Caller frees. */
char *ifme_absolute_path(const char *path);

/* ---- task.c ---------------------------------------------------------------- */
struct ifme_control *ifme_task_control(ifme_task *task);

/* ---- platform/proc_*.c ---------------------------------------------------- */
enum {
	IFME_RUN_OK = 0,          /* every stage ran to completion          */
	IFME_RUN_START_FAILED = -1,
	IFME_RUN_TIMEOUT = -2,    /* killed after timeout_ms                */
	IFME_RUN_STOPPED = -3     /* killed because control->want_stop      */
};

/* Requests from any thread (or a signal handler) to a running pipeline. The
 * runner polls these every few milliseconds and does the OS work itself, so
 * callers never race with process start-up or tear-down. */
typedef struct ifme_control {
	atomic_int want_pause;    /* 1: suspend every process, 0: run          */
	atomic_int want_stop;     /* 1: kill the whole pipeline                */
	atomic_int paused;        /* runner's actual state                     */
} ifme_control;

typedef struct ifme_run_opts {
	const char *cwd;          /* NULL: inherit                                   */
	int timeout_ms;           /* <= 0: no limit (time spent paused not counted)  */
	bool capture_stdout;      /* last stage's stdout -> result.out (else null)   */
	bool capture_stderr;      /* every stage's stderr -> result.err              */
	const char *stderr_path;  /* if not capturing/streaming: append stderr here  */
	size_t capture_limit;     /* per buffer; 0 = 64 MiB. Excess is read and dropped */

	/* Streaming: every stderr line (split on \r and \n, empty lines skipped). */
	void (*on_line)(void *user, const char *line);
	void (*on_tick)(void *user);   /* about every 50 ms while running */
	void *user;
	ifme_control *control;         /* NULL: no pause/stop              */
} ifme_run_opts;

typedef struct ifme_run_result {
	int status;               /* IFME_RUN_*                                       */
	int exit_code;            /* last stage; 128+signal when killed by a signal   */
	char *out;                /* NUL-terminated, may contain NULs; NULL if none   */
	size_t out_len;
	char *err;
	size_t err_len;
	char message[512];        /* why it could not start / timed out               */
} ifme_run_result;

/* Run stages[0] | stages[1] | ... without a shell. stdin of the first stage is
 * the null device. Always fills *res; free it with ifme_run_result_free(). */
int  ifme_run(const ifme_argv *stages, size_t count, const ifme_run_opts *opts,
              ifme_run_result *res);
void ifme_run_result_free(ifme_run_result *res);

/* proc_common.c: growable capture buffer shared by both platforms */
typedef struct ifme_capture {
	char *data;
	size_t len, cap, limit;
} ifme_capture;

void ifme_capture_append(ifme_capture *c, const char *buf, size_t n);
void ifme_capture_finish(ifme_capture *c, char **out, size_t *out_len);

/* Splits a byte stream into lines on \r or \n (progress output uses \r). */
typedef struct ifme_linebuf {
	char buf[4096];
	size_t len;
} ifme_linebuf;

void ifme_linebuf_feed(ifme_linebuf *lb, const char *data, size_t n,
                       void (*cb)(void *user, const char *line), void *user);
void ifme_linebuf_flush(ifme_linebuf *lb, void (*cb)(void *user, const char *line), void *user);

/* Shared bookkeeping for the runner loops. */
typedef struct ifme_run_state {
	uint64_t deadline;        /* 0: none                                    */
	uint64_t paused_since;    /* 0: running                                 */
	uint64_t last_tick;
	bool paused;
	bool stopped;
	bool timed_out;
} ifme_run_state;

/* ---- progress.c ------------------------------------------------------------ */
typedef struct ifme_progress_parser {
	long long total_frames;     /* 0: unknown; encoders may report it        */
	double fps_history[5];      /* moving average for the ETA (as in C#)     */
	int history_count, history_pos;
	uint64_t start_ms;          /* to derive fps when an encoder omits it    */
	long long start_frame;
	double total_seconds;       /* 0: unknown; for time-only (audio) progress */
} ifme_progress_parser;

enum { IFME_LINE_LOG = 0, IFME_LINE_STATUS = 1, IFME_LINE_PROGRESS = 2 };

void ifme_progress_init(ifme_progress_parser *pp, long long total_frames);
/* Classify one output line; for IFME_LINE_PROGRESS *out is filled. */
int  ifme_progress_feed(ifme_progress_parser *pp, const char *line, ifme_progress *out);
/* Copy without ANSI colour codes and backspaces, trimmed. Caller frees. */
char *ifme_clean_line(const char *line);

#endif /* IFME_INTERNAL_H */
