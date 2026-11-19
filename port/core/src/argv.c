/*
 * argv.c - argument vectors built from plugin manifest templates
 *
 * Commands are kept as argument lists all the way to the OS, so file names
 * are never re-parsed by a shell (no quoting bugs with spaces, $, `, ...).
 */

#include <stdlib.h>
#include <string.h>

#include "internal.h"

void ifme_argv_free(ifme_argv *a)
{
	for (size_t i = 0; i < a->n; i++)
		free(a->v[i]);
	free(a->v);
	a->v = NULL;
	a->n = a->cap = 0;
}

static bool push_owned(ifme_argv *a, char *s)
{
	if (!s)
		return false;

	if (a->n + 2 > a->cap) {
		size_t cap = a->cap ? a->cap * 2 : 16;
		char **grown = realloc(a->v, cap * sizeof(char *));
		if (!grown) {
			free(s);
			return false;
		}
		a->v = grown;
		a->cap = cap;
	}

	a->v[a->n++] = s;
	a->v[a->n] = NULL;
	return true;
}

bool ifme_argv_push(ifme_argv *a, const char *s)
{
	return push_owned(a, ifme_strdup(s ? s : ""));
}

bool ifme_argv_pushf(ifme_argv *a, const char *fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	char *s = ifme_vstrdupf(fmt, ap);
	va_end(ap);
	return push_owned(a, s);
}

bool ifme_is_disabled(const char *s)
{
	/* Exactly the list in Extensions.IsDisable (C#). */
	static const char *const off[] = {
		"disabled", "disable", "blank", "none", "null", "nan", "non", "off", "no", "0"
	};

	if (!s)
		return true;

	while (*s == ' ' || *s == '\t')
		s++;
	if (!*s)
		return true;

	for (size_t i = 0; i < sizeof(off) / sizeof(off[0]); i++)
		if (ifme_strcasecmp(s, off[i]) == 0)
			return true;
	return false;
}

bool ifme_argv_split(ifme_argv *a, const char *tmpl)
{
	/* Only NULL/blank means "nothing". Words like "off" or "0" are real values
	 * here (x265 --tune off); callers apply ifme_is_disabled() where C# did. */
	if (!tmpl)
		return true;

	size_t len = strlen(tmpl);
	char *tok = malloc(len + 1);
	if (!tok)
		return false;

	const char *p = tmpl;
	while (*p) {
		while (*p == ' ' || *p == '\t' || *p == '\r' || *p == '\n')
			p++;
		if (!*p)
			break;

		size_t n = 0;
		char quote = 0;

		for (; *p; p++) {
			if (quote) {
				if (*p == quote)
					quote = 0;
				else
					tok[n++] = *p;
			} else if (*p == '"' || *p == '\'') {
				quote = *p;
			} else if (*p == ' ' || *p == '\t' || *p == '\r' || *p == '\n') {
				break;
			} else {
				tok[n++] = *p;
			}
		}

		tok[n] = '\0';
		if (!ifme_argv_push(a, tok)) {
			free(tok);
			return false;
		}
	}

	free(tok);
	return true;
}

char *ifme_argv_display(const ifme_argv *a)
{
	size_t total = 1;
	for (size_t i = 0; i < a->n; i++)
		total += strlen(a->v[i]) + 3;

	char *out = malloc(total);
	if (!out)
		return NULL;

	size_t pos = 0;
	for (size_t i = 0; i < a->n; i++) {
		const char *s = a->v[i];
		bool quote = !*s || strpbrk(s, " \t") != NULL;

		if (i)
			out[pos++] = ' ';
		if (quote)
			out[pos++] = '"';
		size_t l = strlen(s);
		memcpy(out + pos, s, l);
		pos += l;
		if (quote)
			out[pos++] = '"';
	}
	out[pos] = '\0';
	return out;
}
