/*
 * ifme.h - public C interface of libifmecore
 *
 * Used by ifmecli (C) and the Qt GUI (C++). Everything that crosses this
 * boundary is plain C: UTF-8 strings, POD structs, opaque handles.
 *
 * License: GPL-2.0
 */

#ifndef IFME_H
#define IFME_H

#include <stdbool.h>
#include <stddef.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct ifme_ctx ifme_ctx;

typedef struct ifme_cpu_info {
	char brand[64];              /* e.g. "AMD Ryzen 9 3950X 16-Core Processor" */
	char arch[16];               /* "x86_64", "x86", "arm64", ...               */
	bool is_x86;                 /* false: the SIMD flags below are meaningless */
	bool sse42;
	bool avx;                    /* includes the OS (XGETBV) check              */
	bool avx2;
	bool fma3;
	bool avx512f;
	bool hypervisor;             /* running inside a virtual machine            */
	char hypervisor_vendor[16];  /* e.g. "KVMKVMKVM", "Microsoft Hv"            */
} ifme_cpu_info;

/* Library version, e.g. "0.1.0". Never NULL. */
const char *ifme_version(void);

/* Create / destroy a context. Returns NULL only when out of memory. */
ifme_ctx *ifme_create(void);
void      ifme_destroy(ifme_ctx *ctx);

/* Folder that holds the running executable (UTF-8, no trailing separator).
 * Bundled Plugins/ and resources are looked up relative to this. */
const char *ifme_app_dir(const ifme_ctx *ctx);

/* Per-user settings folder (UTF-8). Not created automatically.
 *   Windows: %APPDATA%\IFME
 *   Linux:   $XDG_CONFIG_HOME/ifme  (default ~/.config/ifme)
 *   macOS:   ~/Library/Application Support/IFME */
const char *ifme_config_dir(const ifme_ctx *ctx);

/* CPU model and instruction-set support, detected once in ifme_create(). */
const ifme_cpu_info *ifme_cpu(const ifme_ctx *ctx);

/* Folder that contains Plugins/ and Samples/ (UTF-8). Defaults to the app
 * folder; override it to use another IFME installation. Changing it unloads
 * any loaded plugins. Returns false when the folder does not exist. */
bool        ifme_set_root_dir(ifme_ctx *ctx, const char *dir);
const char *ifme_root_dir(const ifme_ctx *ctx);

/* ------------------------------------------------------------------------ */
/* Plugins                                                                  */
/* ------------------------------------------------------------------------ */

typedef enum ifme_plugin_kind {
	IFME_PLUGIN_AUDIO = 1,
	IFME_PLUGIN_VIDEO = 2
} ifme_plugin_kind;

typedef enum ifme_plugin_status {
	IFME_PLUGIN_OK = 0,          /* usable                                       */
	IFME_PLUGIN_BUILTIN,         /* "copy"/passthrough entry, no binary          */
	IFME_PLUGIN_WRONG_ARCH,      /* built for another CPU architecture           */
	IFME_PLUGIN_DUPLICATE,       /* GUID already provided by an earlier manifest */
	IFME_PLUGIN_MISSING_BINARY,  /* encoder program not found                    */
	IFME_PLUGIN_INVALID,         /* manifest could not be parsed                 */
	IFME_PLUGIN_TEST_FAILED      /* self-test ran and failed                     */
} ifme_plugin_status;

typedef struct ifme_plugin_info {
	ifme_plugin_kind   kind;
	ifme_plugin_status status;
	const char *guid;            /* lower case, as written in the manifest       */
	const char *name;
	const char *version;         /* may be ""                                    */
	const char *developer;       /* may be ""                                    */
	const char *manifest;        /* full path of the _plugin.*.json file         */
	const char *binary;          /* resolved encoder program, "" when none       */
	const char *message;         /* why status is not OK, otherwise ""           */
	bool from_path;              /* binary was found through PATH               */
	bool test_required;          /* manifest asks for a self-test               */
} ifme_plugin_info;

/* Scan <root>/Plugins recursively for _plugin.a*.json / _plugin.v*.json.
 * Returns the number of usable plugins (OK or BUILTIN), or -1 when the
 * Plugins folder does not exist. Reloading replaces the previous list. */
int ifme_plugins_load(ifme_ctx *ctx);

size_t                  ifme_plugin_count(const ifme_ctx *ctx);
const ifme_plugin_info *ifme_plugin_at(const ifme_ctx *ctx, size_t index);
const char             *ifme_plugin_status_name(ifme_plugin_status status);

/* Every plugin as a JSON array, including each resolved manifest under
 * "manifest_json". Caller frees with ifme_free(). */
char *ifme_plugins_json(const ifme_ctx *ctx);

typedef enum ifme_test_outcome {
	IFME_TEST_PASS = 0,
	IFME_TEST_FAIL,
	IFME_TEST_SKIPPED            /* e.g. sample file or ffmpeg missing           */
} ifme_test_outcome;

typedef struct ifme_selftest_callbacks {
	void (*on_start)(void *user, const ifme_plugin_info *plugin);
	void (*on_result)(void *user, const ifme_plugin_info *plugin,
	                  ifme_test_outcome outcome, const char *detail,
	                  unsigned elapsed_ms);
	void *user;
} ifme_selftest_callbacks;

/* Encode the bundled sample with each usable plugin, like IFME's startup
 * test. Only plugins with "TestRequired" are tested unless test_all is set.
 * Failed plugins get IFME_PLUGIN_TEST_FAILED. Blocks until finished.
 * Returns the number of failures, or -1 if plugins are not loaded. */
int ifme_plugins_selftest(ifme_ctx *ctx, bool test_all,
                          const ifme_selftest_callbacks *cb);

/* Free memory returned by functions documented as "caller frees". */
void ifme_free(void *ptr);

#ifdef __cplusplus
}
#endif

#endif /* IFME_H */
