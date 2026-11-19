/*
 * json_relax.c - accept the JSON that Newtonsoft.Json accepted
 *
 * Existing plugin manifests contain trailing commas (e.g. `"ts",` before `]`)
 * and may contain comments. cJSON is strict, so normalise first.
 */

#include <stdlib.h>
#include <string.h>

#include "internal.h"

/* Skip whitespace and comments; returns the next significant character. */
static const char *skip_insignificant(const char *p)
{
	for (;;) {
		while (*p == ' ' || *p == '\t' || *p == '\r' || *p == '\n')
			p++;

		if (p[0] == '/' && p[1] == '/') {
			while (*p && *p != '\n')
				p++;
		} else if (p[0] == '/' && p[1] == '*') {
			p += 2;
			while (*p && !(p[0] == '*' && p[1] == '/'))
				p++;
			if (*p)
				p += 2;
		} else {
			return p;
		}
	}
}

char *ifme_json_relax(const char *in)
{
	if (!in)
		return NULL;

	char *out = malloc(strlen(in) + 1);
	if (!out)
		return NULL;

	const char *p = in;
	size_t o = 0;

	if ((unsigned char)p[0] == 0xEF && (unsigned char)p[1] == 0xBB && (unsigned char)p[2] == 0xBF)
		p += 3;

	while (*p) {
		if (*p == '"') {
			/* Copy a string literal verbatim, honouring escapes. */
			out[o++] = *p++;
			while (*p && *p != '"') {
				if (*p == '\\' && p[1])
					out[o++] = *p++;
				out[o++] = *p++;
			}
			if (*p)
				out[o++] = *p++;
		} else if (p[0] == '/' && (p[1] == '/' || p[1] == '*')) {
			p = skip_insignificant(p);
		} else if (*p == ',') {
			const char *next = skip_insignificant(p + 1);
			if (*next == ']' || *next == '}')
				p++;            /* trailing comma: drop it */
			else
				out[o++] = *p++;
		} else {
			out[o++] = *p++;
		}
	}

	out[o] = '\0';
	return out;
}
