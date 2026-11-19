/*
 * proc_common.c - pieces of the process runner shared by Windows and POSIX
 */

#include <stdlib.h>
#include <string.h>

#include "../internal.h"

#define DEFAULT_CAPTURE_LIMIT ((size_t)64 * 1024 * 1024)

void ifme_capture_append(ifme_capture *c, const char *buf, size_t n)
{
	size_t limit = c->limit ? c->limit : DEFAULT_CAPTURE_LIMIT;

	/* Beyond the limit keep draining the pipe (so the child never blocks on a
	 * full pipe) but drop the bytes. */
	if (c->len >= limit)
		return;
	if (n > limit - c->len)
		n = limit - c->len;

	if (c->len + n + 1 > c->cap) {
		size_t cap = c->cap ? c->cap : 4096;
		while (cap < c->len + n + 1)
			cap *= 2;

		char *grown = realloc(c->data, cap);
		if (!grown)
			return;
		c->data = grown;
		c->cap = cap;
	}

	memcpy(c->data + c->len, buf, n);
	c->len += n;
	c->data[c->len] = '\0';
}

void ifme_capture_finish(ifme_capture *c, char **out, size_t *out_len)
{
	if (!c->data) {
		c->data = malloc(1);
		if (c->data)
			c->data[0] = '\0';
	}

	*out = c->data;
	*out_len = c->len;
	c->data = NULL;
	c->len = c->cap = 0;
}

void ifme_run_result_free(ifme_run_result *res)
{
	if (!res)
		return;

	free(res->out);
	free(res->err);
	res->out = res->err = NULL;
	res->out_len = res->err_len = 0;
}
