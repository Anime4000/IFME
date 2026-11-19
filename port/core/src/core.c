/*
 * core.c - context lifetime and small helpers
 */

#include <stdlib.h>
#include <string.h>

#include "internal.h"

#ifndef IFME_VERSION
#define IFME_VERSION "0.0.0"
#endif

#ifdef _WIN32
#define IFME_SEP '\\'
#else
#define IFME_SEP '/'
#endif

const char *ifme_version(void)
{
	return IFME_VERSION;
}

ifme_ctx *ifme_create(void)
{
	ifme_ctx *ctx = calloc(1, sizeof(*ctx));
	if (!ctx)
		return NULL;

	ctx->app_dir = ifme_platform_app_dir();
	if (!ctx->app_dir)
		ctx->app_dir = ifme_strdup(".");

	ctx->config_dir = ifme_platform_config_dir();
	if (!ctx->config_dir)
		ctx->config_dir = ifme_strdup(ctx->app_dir);

	ifme_platform_cpu(&ctx->cpu);

	if (!ctx->app_dir || !ctx->config_dir) {
		ifme_destroy(ctx);
		return NULL;
	}

	return ctx;
}

void ifme_destroy(ifme_ctx *ctx)
{
	if (!ctx)
		return;

	free(ctx->app_dir);
	free(ctx->config_dir);
	free(ctx);
}

const char *ifme_app_dir(const ifme_ctx *ctx)
{
	return ctx ? ctx->app_dir : "";
}

const char *ifme_config_dir(const ifme_ctx *ctx)
{
	return ctx ? ctx->config_dir : "";
}

const ifme_cpu_info *ifme_cpu(const ifme_ctx *ctx)
{
	return ctx ? &ctx->cpu : NULL;
}

void ifme_free(void *ptr)
{
	free(ptr);
}

char *ifme_strdup(const char *s)
{
	if (!s)
		return NULL;

	size_t n = strlen(s) + 1;
	char *copy = malloc(n);
	if (copy)
		memcpy(copy, s, n);
	return copy;
}

char *ifme_path_join(const char *a, const char *b)
{
	size_t la = strlen(a), lb = strlen(b);
	char *out = malloc(la + lb + 2);
	if (!out)
		return NULL;

	memcpy(out, a, la);
	size_t pos = la;
	if (la > 0 && a[la - 1] != '/' && a[la - 1] != '\\')
		out[pos++] = IFME_SEP;
	memcpy(out + pos, b, lb + 1);
	return out;
}
