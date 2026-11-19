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

/* ---- platform/proc_*.c ---------------------------------------------------- */
enum {
	IFME_RUN_OK = 0,          /* every stage ran to completion          */
	IFME_RUN_START_FAILED = -1,
	IFME_RUN_TIMEOUT = -2     /* killed after timeout_ms                */
};

/* Run stages[0] | stages[1] | ... without a shell. stdin of the first stage
 * and stdout of the last are the null device; stderr of every stage is
 * appended to log_path (or discarded when NULL). *exit_code receives the
 * last stage's status (128+signal when killed by a signal). */
int ifme_run_pipeline(const ifme_argv *stages, size_t count, const char *cwd,
                      const char *log_path, int timeout_ms, int *exit_code,
                      char *err, size_t err_len);

#endif /* IFME_INTERNAL_H */
