/*
 * internal.h - shared between core modules, not part of the public API
 */

#ifndef IFME_INTERNAL_H
#define IFME_INTERNAL_H

#include "ifme.h"

struct ifme_ctx {
	char *app_dir;
	char *config_dir;
	ifme_cpu_info cpu;
};

/* platform/paths.c - both return malloc'd UTF-8 strings, or NULL on failure */
char *ifme_platform_app_dir(void);
char *ifme_platform_config_dir(void);

/* platform/cpu_x86.c or platform/cpu_other.c */
void ifme_platform_cpu(ifme_cpu_info *info);

/* small helpers (core.c) */
char *ifme_strdup(const char *s);
char *ifme_path_join(const char *a, const char *b);

#endif /* IFME_INTERNAL_H */
