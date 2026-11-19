/*
 * internal.h - shared between core modules, not part of the public API
 */

#ifndef IFME_INTERNAL_H
#define IFME_INTERNAL_H

#include <stdarg.h>
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

/* ---- plugins.c ------------------------------------------------------------ */
void ifme_plugins_clear(ifme_ctx *ctx);

/* An FFmpeg tool ("ffmpeg", "ffprobe"): PATH first, then the bundled copy in
 * <root>/Plugins/ffmpeg64 (ffmpeg32 on 32-bit x86). Caller frees. */
char *ifme_find_ff_tool(const ifme_ctx *ctx, const char *name);

/* ---- platform/proc_*.c ---------------------------------------------------- */
enum {
	IFME_RUN_OK = 0,          /* every stage ran to completion          */
	IFME_RUN_START_FAILED = -1,
	IFME_RUN_TIMEOUT = -2     /* killed after timeout_ms                */
};

typedef struct ifme_run_opts {
	const char *cwd;          /* NULL: inherit                                   */
	int timeout_ms;           /* <= 0: no limit                                  */
	bool capture_stdout;      /* last stage's stdout -> result.out (else null)   */
	bool capture_stderr;      /* every stage's stderr -> result.err              */
	const char *stderr_path;  /* if not capturing: append stderr here (NULL: drop) */
	size_t capture_limit;     /* per buffer; 0 = 64 MiB. Excess is read and dropped */
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

#endif /* IFME_INTERNAL_H */
