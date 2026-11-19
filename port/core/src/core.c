/*
 * core.c - context lifetime and small helpers
 */

#include <stdio.h>
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

	ctx->root_dir = ifme_strdup(ctx->app_dir);

	ifme_platform_cpu(&ctx->cpu);

	if (!ctx->app_dir || !ctx->config_dir || !ctx->root_dir) {
		ifme_destroy(ctx);
		return NULL;
	}

	return ctx;
}

void ifme_destroy(ifme_ctx *ctx)
{
	if (!ctx)
		return;

	ifme_plugins_set_disabled(ctx, NULL, 0);
	ifme_plugins_clear(ctx);
	free(ctx->app_dir);
	free(ctx->config_dir);
	free(ctx->root_dir);
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

bool ifme_set_root_dir(ifme_ctx *ctx, const char *dir)
{
	if (!ctx || !dir || !ifme_dir_exists(dir))
		return false;

	char *full = ifme_full_path(dir);
	if (!full)
		return false;

	ifme_plugins_clear(ctx);
	free(ctx->root_dir);
	ctx->root_dir = full;
	return true;
}

const char *ifme_root_dir(const ifme_ctx *ctx)
{
	return ctx ? ctx->root_dir : "";
}

void ifme_free(void *ptr)
{
	free(ptr);
}

/* ---- string helpers -------------------------------------------------------- */

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

char *ifme_vstrdupf(const char *fmt, va_list ap)
{
	va_list ap2;
	va_copy(ap2, ap);
	int n = vsnprintf(NULL, 0, fmt, ap2);
	va_end(ap2);

	if (n < 0)
		return NULL;

	char *out = malloc((size_t)n + 1);
	if (out)
		vsnprintf(out, (size_t)n + 1, fmt, ap);
	return out;
}

char *ifme_strdupf(const char *fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	char *out = ifme_vstrdupf(fmt, ap);
	va_end(ap);
	return out;
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

static int lower(int c)
{
	return (c >= 'A' && c <= 'Z') ? c + ('a' - 'A') : c;
}

int ifme_strcasecmp(const char *a, const char *b)
{
	for (;; a++, b++) {
		int ca = lower((unsigned char)*a), cb = lower((unsigned char)*b);
		if (ca != cb || ca == 0)
			return ca - cb;
	}
}

bool ifme_starts_with_ci(const char *s, const char *prefix)
{
	for (; *prefix; s++, prefix++)
		if (lower((unsigned char)*s) != lower((unsigned char)*prefix))
			return false;
	return true;
}

bool ifme_ends_with_ci(const char *s, const char *suffix)
{
	size_t ls = strlen(s), lx = strlen(suffix);
	return ls >= lx && ifme_strcasecmp(s + ls - lx, suffix) == 0;
}
