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

/* Free memory returned by functions documented as "caller frees". */
void ifme_free(void *ptr);

#ifdef __cplusplus
}
#endif

#endif /* IFME_H */
