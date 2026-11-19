/*
 * subtime.c - cut text subtitles and chapters to a trimmed part
 *
 * FFmpeg's own trimming of subtitle streams (-ss/-t) drops every event that
 * starts before the cut, lets events run past the end (which stretches the
 * output's duration), and does not shift re-encoded SubRip at all. Text
 * subtitles are therefore extracted whole and re-timed here: shifted by the
 * start, clipped to the part, and dropped when outside it.
 */

#include <ctype.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"

/* ---- a growable string ------------------------------------------------------ */

typedef struct sbuf {
	char *s;
	size_t len, cap;
	bool oom;
} sbuf;

static void sb_add(sbuf *b, const char *p, size_t n)
{
	if (b->oom)
		return;
	if (b->len + n + 1 > b->cap) {
		size_t cap = b->cap ? b->cap : 4096;
		while (cap < b->len + n + 1)
			cap *= 2;
		char *g = realloc(b->s, cap);
		if (!g) {
			b->oom = true;
			return;
		}
		b->s = g;
		b->cap = cap;
	}
	memcpy(b->s + b->len, p, n);
	b->len += n;
	b->s[b->len] = '\0';
}

static void sb_str(sbuf *b, const char *s)
{
	sb_add(b, s, strlen(s));
}

static char *sb_done(sbuf *b)
{
	if (b->oom) {
		free(b->s);
		return NULL;
	}
	if (!b->s)
		sb_add(b, "", 0);
	return b->s;
}

/* ---- timestamps ---------------------------------------------------------------- */

/* "H:MM:SS,mmm", "HH:MM:SS.mmm", "H:MM:SS.cc" or "MM:SS.mmm" at *p. */
static bool parse_ts(const char **p, double *out)
{
	const char *s = *p;
	long parts[3];
	int n = 0;

	while (n < 3) {
		if (!isdigit((unsigned char)*s))
			return false;
		char *end;
		parts[n++] = strtol(s, &end, 10);
		s = end;
		if (*s != ':')
			break;
		s++;
	}
	if (n < 2)
		return false;

	double frac = 0, scale = 0.1;
	if (*s == '.' || *s == ',') {
		s++;
		if (!isdigit((unsigned char)*s))
			return false;
		for (; isdigit((unsigned char)*s); s++, scale /= 10)
			frac += (*s - '0') * scale;
	}

	*out = (n == 3 ? parts[0] * 3600.0 + parts[1] * 60.0 + parts[2] : parts[0] * 60.0 + parts[1]) + frac;
	*p = s;
	return true;
}

enum { TS_SRT, TS_VTT, TS_ASS };

static void fmt_ts(char *buf, size_t n, double t, int style)
{
	if (t < 0)
		t = 0;
	if (style == TS_ASS) {
		long long cs = (long long)(t * 100 + 0.5);
		snprintf(buf, n, "%lld:%02lld:%02lld.%02lld", cs / 360000, cs / 6000 % 60, cs / 100 % 60, cs % 100);
	} else {
		long long ms = (long long)(t * 1000 + 0.5);
		snprintf(buf, n, "%02lld:%02lld:%02lld%c%03lld", ms / 3600000, ms / 60000 % 60, ms / 1000 % 60,
		         style == TS_SRT ? ',' : '.', ms % 1000);
	}
}

/* Shift [*a, *b] by -start and clip to [0, dur]; false when nothing is left. */
static bool cut(double *a, double *b, double start, double dur)
{
	double s = *a - start, e = *b - start;
	if (s < 0)
		s = 0;
	if (e > dur)
		e = dur;
	if (e <= s)
		return false;
	*a = s;
	*b = e;
	return true;
}

/* ---- lines ----------------------------------------------------------------------- */

/* Next line of text[pos..len): its content without the line break. */
static bool next_line(const char *text, size_t len, size_t *pos, const char **line, size_t *n)
{
	if (*pos >= len)
		return false;
	const char *s = text + *pos;
	const char *nl = memchr(s, '\n', len - *pos);
	size_t full = nl ? (size_t)(nl - s) + 1 : len - *pos;
	*line = s;
	*n = nl ? (size_t)(nl - s) : full;
	if (*n && s[*n - 1] == '\r')
		(*n)--;
	*pos += full;
	return true;
}

static bool blank_line(const char *s, size_t n)
{
	for (size_t i = 0; i < n; i++)
		if (!isspace((unsigned char)s[i]))
			return false;
	return true;
}

/* ---- SubRip / WebVTT ----------------------------------------------------------- */

/* Blocks separated by blank lines; a block with "-->" is a cue. */
static char *cut_cues(const char *text, size_t len, bool vtt, double start, double dur, size_t *kept)
{
	const char *eol = memchr(text, '\r', len) ? "\r\n" : "\n";
	sbuf out = { 0 };
	size_t pos = 0, number = 0;
	*kept = 0;

	while (pos < len) {
		/* Collect one block. */
		const char *lines[64];
		size_t lens[64], count = 0;
		const char *l;
		size_t n;
		size_t save = pos;

		while (next_line(text, len, &pos, &l, &n) && blank_line(l, n))
			save = pos;
		if (pos == save && pos >= len)
			break;
		pos = save;
		while (next_line(text, len, &pos, &l, &n) && !blank_line(l, n))
			if (count < 64) {
				lines[count] = l;
				lens[count++] = n;
			}
		if (count == 0)
			continue;

		int timing = -1;
		for (size_t i = 0; i < count && timing < 0; i++) {
			char tmp[256];
			size_t m = lens[i] < sizeof(tmp) - 1 ? lens[i] : sizeof(tmp) - 1;
			memcpy(tmp, lines[i], m);
			tmp[m] = '\0';
			if (strstr(tmp, "-->"))
				timing = (int)i;
		}

		if (timing < 0) {
			/* WebVTT header, NOTE and STYLE blocks stay; SubRip has none. */
			for (size_t i = 0; i < count; i++) {
				sb_add(&out, lines[i], lens[i]);
				sb_str(&out, eol);
			}
			sb_str(&out, eol);
			continue;
		}

		char tl[256];
		size_t m = lens[timing] < sizeof(tl) - 1 ? lens[timing] : sizeof(tl) - 1;
		memcpy(tl, lines[timing], m);
		tl[m] = '\0';

		const char *p = tl;
		while (*p == ' ')
			p++;
		double a, b;
		if (!parse_ts(&p, &a))
			continue;
		const char *arrow = strstr(p, "-->");
		if (!arrow)
			continue;
		p = arrow + 3;
		while (*p == ' ')
			p++;
		if (!parse_ts(&p, &b) || !cut(&a, &b, start, dur))
			continue;   /* outside the part (or not a cue) */

		char ta[32], tb[32];
		fmt_ts(ta, sizeof(ta), a, vtt ? TS_VTT : TS_SRT);
		fmt_ts(tb, sizeof(tb), b, vtt ? TS_VTT : TS_SRT);
		(*kept)++;

		for (size_t i = 0; i < count; i++) {
			if ((int)i == timing) {
				sb_str(&out, ta);
				sb_str(&out, " --> ");
				sb_str(&out, tb);
				sb_str(&out, p);   /* WebVTT cue settings */
			} else if (!vtt && (int)i == timing - 1) {
				char num[32];
				snprintf(num, sizeof(num), "%zu", ++number);   /* SubRip: renumber */
				sb_str(&out, num);
			} else {
				sb_add(&out, lines[i], lens[i]);
			}
			sb_str(&out, eol);
		}
		sb_str(&out, eol);
	}
	return sb_done(&out);
}

/* ---- ASS / SSA ------------------------------------------------------------------ */

/* "Dialogue: Layer,Start,End,Style,..." lines are re-timed; all else stays. */
static char *cut_ass(const char *text, size_t len, double start, double dur, size_t *kept)
{
	const char *eol = memchr(text, '\r', len) ? "\r\n" : "\n";
	sbuf out = { 0 };
	size_t pos = 0;
	const char *l;
	size_t n;
	*kept = 0;

	while (next_line(text, len, &pos, &l, &n)) {
		if (n < 9 || strncmp(l, "Dialogue:", 9) != 0) {
			sb_add(&out, l, n);
			sb_str(&out, eol);
			continue;
		}

		const char *end = l + n;
		const char *c1 = memchr(l, ',', n);
		const char *c2 = c1 ? memchr(c1 + 1, ',', (size_t)(end - c1 - 1)) : NULL;
		const char *c3 = c2 ? memchr(c2 + 1, ',', (size_t)(end - c2 - 1)) : NULL;
		if (!c3) {
			sb_add(&out, l, n);
			sb_str(&out, eol);
			continue;
		}

		char ts[64];
		double a, b;
		size_t la = (size_t)(c2 - c1 - 1), lb = (size_t)(c3 - c2 - 1);
		const char *p;
		bool ok = la < sizeof(ts) && lb < sizeof(ts);
		if (ok) {
			memcpy(ts, c1 + 1, la);
			ts[la] = '\0';
			p = ts;
			while (*p == ' ')
				p++;
			ok = parse_ts(&p, &a);
		}
		if (ok) {
			memcpy(ts, c2 + 1, lb);
			ts[lb] = '\0';
			p = ts;
			while (*p == ' ')
				p++;
			ok = parse_ts(&p, &b);
		}
		if (!ok) {
			sb_add(&out, l, n);
			sb_str(&out, eol);
			continue;
		}
		if (!cut(&a, &b, start, dur))
			continue;

		char ta[32], tb[32];
		fmt_ts(ta, sizeof(ta), a, TS_ASS);
		fmt_ts(tb, sizeof(tb), b, TS_ASS);
		sb_add(&out, l, (size_t)(c1 - l) + 1);
		sb_str(&out, ta);
		sb_str(&out, ",");
		sb_str(&out, tb);
		sb_add(&out, c3, (size_t)(end - c3));
		sb_str(&out, eol);
		(*kept)++;
	}
	return sb_done(&out);
}

char *ifme_subtitle_cut(const char *text, size_t len, const char *format,
                        double start, double duration, size_t *kept)
{
	size_t dummy;
	if (!kept)
		kept = &dummy;
	if (format && (ifme_strcasecmp(format, "ass") == 0 || ifme_strcasecmp(format, "ssa") == 0))
		return cut_ass(text, len, start, duration, kept);
	return cut_cues(text, len, format && ifme_strcasecmp(format, "vtt") == 0, start, duration, kept);
}

/* ---- chapters ------------------------------------------------------------------- */

char *ifme_chapters_drop_empty(const char *text, size_t len, size_t *dropped)
{
	sbuf out = { 0 };
	size_t pos = 0;
	const char *l;
	size_t n;
	*dropped = 0;

	while (pos < len) {
		size_t block = pos;
		if (!next_line(text, len, &pos, &l, &n))
			break;

		if (!(n >= 9 && strncmp(l, "[CHAPTER]", 9) == 0)) {
			sb_add(&out, text + block, pos - block);
			continue;
		}

		/* A chapter runs until the next section. */
		long long s = 0, e = -1;
		while (pos < len) {
			size_t at = pos;
			if (!next_line(text, len, &pos, &l, &n))
				break;
			if (n && l[0] == '[') {
				pos = at;
				break;
			}
			if (n > 6 && strncmp(l, "START=", 6) == 0)
				s = strtoll(l + 6, NULL, 10);
			else if (n > 4 && strncmp(l, "END=", 4) == 0)
				e = strtoll(l + 4, NULL, 10);
		}
		if (e >= 0 && e <= s) {
			(*dropped)++;
			continue;
		}
		sb_add(&out, text + block, pos - block);
	}
	return sb_done(&out);
}
