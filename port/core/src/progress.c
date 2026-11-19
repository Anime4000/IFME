/*
 * progress.c - read frame / fps / bitrate from encoder output
 *
 * Port of ProcessManager.Proc_DataReceived (C#). Each C# regex became a
 * small matcher; no regex library needed. Changes, checked against real
 * output of the encoders IFME ships:
 *  - SVT-AV1 with "--progress 2" (what the bundled plugin uses) prints
 *    "Encoding:  N Frames @ F fps | K kb/s" wrapped in ANSI colour codes.
 *    C# matched neither the colours nor the format, so it showed nothing.
 *  - "[P%] N/T frames" lines also give the total, used when the caller had
 *    none (C# ignored it).
 *  - ETA uses the 5-sample moving average without rounding it to a whole
 *    number: C# rounded, so encoders below 0.5 fps (VVenC at 4K) never got an
 *    ETA. When an encoder reports no fps, it is derived from elapsed time.
 *  - "encoded N frames" summary lines still update the total (C#) and are
 *    also kept in the log instead of being dropped.
 */

#include <ctype.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"

char *ifme_clean_line(const char *line)
{
	size_t n = strlen(line);
	char *out = malloc(n + 1);
	if (!out)
		return NULL;

	size_t o = 0;
	for (size_t i = 0; i < n; i++) {
		unsigned char c = (unsigned char)line[i];

		if (c == 0x1b && line[i + 1] == '[') {
			/* ANSI CSI: ESC [ params... final byte 0x40-0x7e */
			i += 2;
			while (i < n && !((unsigned char)line[i] >= 0x40 && (unsigned char)line[i] <= 0x7e))
				i++;
			continue;
		}
		if (c == '\b' || c == 0x1b)
			c = ' ';
		if (c == '\t')
			c = ' ';
		out[o++] = (char)c;
	}

	while (o > 0 && out[o - 1] == ' ')
		o--;
	out[o] = '\0';

	size_t lead = strspn(out, " ");
	if (lead)
		memmove(out, out + lead, o - lead + 1);
	return out;
}

/* `key` as a whole word: "fps=" must not match inside "avg_fps=". */
static const char *find_key(const char *s, const char *key)
{
	for (const char *p = strstr(s, key); p; p = strstr(p + 1, key)) {
		if (p == s || p[-1] == ' ' || p[-1] == '\t')
			return p;
	}
	return NULL;
}

/* Number right after `key` (spaces allowed), or false. */
static bool num_after(const char *s, const char *key, double *out)
{
	const char *p = find_key(s, key);
	if (!p)
		return false;

	p += strlen(key);
	while (*p == ' ')
		p++;

	char *end = NULL;
	double d = strtod(p, &end);
	if (end == p)
		return false;
	*out = d;
	return true;
}

/* Integer ending just before `at` (skipping spaces), e.g. "  165 frames:". */
static bool int_before(const char *line, const char *at, long long *out)
{
	const char *p = at;
	while (p > line && p[-1] == ' ')
		p--;
	const char *end = p;
	while (p > line && isdigit((unsigned char)p[-1]))
		p--;
	if (p == end)
		return false;
	*out = strtoll(p, NULL, 10);
	return true;
}

typedef struct match {
	long long frame;
	long long total;     /* 0 if the line doesn't say */
	double fps;          /* -1 unknown */
	double kbps;         /* -1 unknown */
} match;

/* vvenc [info]: stats:  frame=   15 fps=  69.4 avg_fps=  69.4 bitrate= ... avg_bitrate=   133.30 kbps */
static bool match_vvenc(const char *s, match *m)
{
	double f, fps, kb;
	if (!strstr(s, "stats:") || strstr(s, "stats summary"))
		return false;
	if (!num_after(s, "frame=", &f) || !num_after(s, "avg_fps=", &fps) || !num_after(s, "avg_bitrate=", &kb))
		return false;
	m->frame = (long long)f;
	m->fps = fps;
	m->kbps = kb;
	return true;
}

/* x264 / x265 with a known total: "[0.6%] 1/165 frames, 5.99 fps, 2283.72 kb/s, eta 0:00:27" */
static bool match_x26x(const char *s, match *m)
{
	const char *p = strstr(s, "%] ");
	long long n, t;
	double fps, kb;
	if (!p || sscanf(p + 3, "%lld/%lld frames, %lf fps, %lf kb/s", &n, &t, &fps, &kb) != 4)
		return false;
	m->frame = n;
	m->total = t;
	m->fps = fps;
	m->kbps = kb;
	return true;
}

/* x264 / x265 without a total, Rigaya NVEncC/QSVEncC:
 * "165 frames: 45.32 fps, 1234.56 kb/s" (NVEncC may prefix "[12.3%]") */
static bool match_frames_colon(const char *s, match *m)
{
	const char *p = strstr(s, " frames: ");
	long long n;
	double fps, kb;
	if (!p) {
		/* line may start with the number: "1 frames: ..." */
		p = strstr(s, "frames: ");
		if (!p || p == s)
			return false;
	}
	if (!int_before(s, p, &n))
		return false;
	if (sscanf(strstr(p, "frames: ") + 8, "%lf fps, %lf kb/s", &fps, &kb) != 2)
		return false;
	m->frame = n;
	m->fps = fps;
	m->kbps = kb;
	return true;
}

/* FFmpeg: "frame=  165 fps= 45 q=28.0 size= 256KiB time=00:00:02.86 bitrate= 731.2kbits/s speed=5.71x" */
static bool match_ffmpeg(const char *s, match *m)
{
	double f, fps, kb;
	if (!num_after(s, "frame=", &f) || !num_after(s, "fps=", &fps))
		return false;
	m->frame = (long long)f;
	m->fps = fps;
	m->kbps = num_after(s, "bitrate=", &kb) ? kb : -1;   /* "bitrate=N/A" early on */
	return true;
}

/* SVT-AV1 legacy: "Encoding frame  125   1234.56 kbps   45.32 fps" */
static bool match_svt_legacy(const char *s, match *m)
{
	const char *p = strstr(s, "Encoding frame");
	long long n;
	double kb, fps;
	if (!p || sscanf(p + 14, "%lld %lf kbps %lf fps", &n, &kb, &fps) != 3)
		return false;
	m->frame = n;
	m->fps = fps;
	m->kbps = kb;
	return true;
}

/* SVT-AV1 --progress 2 (after colour codes are removed):
 * "Encoding:    1 Frames @ 33.90 fps | 46.66 kb/s | Size: 0.03 MB | Time: 0:00:00"
 * and with a known total: "Encoding:   12/165 Frames @ ..." */
static bool match_svt(const char *s, match *m)
{
	const char *p = strstr(s, "Encoding:");
	long long n, t = 0;
	double fps, kb;
	if (!p)
		return false;
	p += 9;
	if (sscanf(p, "%lld/%lld Frames @ %lf fps | %lf kb/s", &n, &t, &fps, &kb) != 4) {
		t = 0;
		if (sscanf(p, "%lld Frames @ %lf fps | %lf kb/s", &n, &fps, &kb) != 3)
			return false;
	}
	m->frame = n;
	m->total = t;
	m->fps = fps;
	m->kbps = kb;
	return true;
}

/* "encoded 165 frames in 0.54s (304.43 fps), ..." (x265) / "encoded 165 frames, ..." (x264) */
static bool match_encoded_total(const char *s, long long *total)
{
	const char *p = strstr(s, "encoded ");
	if (!p || !isdigit((unsigned char)p[8]))
		return false;
	*total = strtoll(p + 8, NULL, 10);
	return *total > 0;
}

/* Progress-ish text that belongs in a status bar, not the log (C#'s
 * classification regex, simplified). */
static bool looks_like_status(const char *s)
{
	if (strstr(s, "kb/s") || strstr(s, "kbps") || strstr(s, " fps") || strstr(s, "fps=") ||
	    strstr(s, "frame=") || strstr(s, "size="))
		return true;

	/* MP4Box: "Import: |====      | (40/100)", "ISO File Writing: |...| (99/100)" */
	const char *bar = strstr(s, "/100)");
	if (bar && strchr(s, '|') && bar[5] == '\0')
		return true;

	/* SVT-AV1 default progress: bare frame numbers. */
	for (const char *p = s; *p; p++)
		if (!isdigit((unsigned char)*p) && *p != ' ')
			return false;
	return *s != '\0';
}

void ifme_progress_init(ifme_progress_parser *pp, long long total_frames)
{
	memset(pp, 0, sizeof(*pp));
	pp->total_frames = total_frames > 0 ? total_frames : 0;
	pp->start_frame = -1;
}

int ifme_progress_feed(ifme_progress_parser *pp, const char *line, ifme_progress *out)
{
	char *s = ifme_clean_line(line);
	if (!s)
		return IFME_LINE_LOG;

	long long total;
	if (match_encoded_total(s, &total)) {
		pp->total_frames = total;
		free(s);
		return IFME_LINE_LOG;
	}

	match m = { -1, 0, -1, -1 };
	bool ok = match_vvenc(s, &m) || match_x26x(s, &m) || match_svt(s, &m) ||
	          match_svt_legacy(s, &m) || match_frames_colon(s, &m) || match_ffmpeg(s, &m);

	if (!ok) {
		int kind = looks_like_status(s) ? IFME_LINE_STATUS : IFME_LINE_LOG;
		free(s);
		return kind;
	}
	free(s);

	if (m.total > 0 && pp->total_frames <= 0)
		pp->total_frames = m.total;

	/* No fps from the encoder: derive it from frames over wall time. */
	uint64_t now = ifme_now_ms();
	if (pp->start_frame < 0) {
		pp->start_frame = m.frame;
		pp->start_ms = now;
	}
	if (m.fps < 0 && now > pp->start_ms)
		m.fps = (double)(m.frame - pp->start_frame) * 1000.0 / (double)(now - pp->start_ms);

	out->frame = m.frame;
	out->total_frames = pp->total_frames;
	out->fps = m.fps;
	out->kbps = m.kbps;
	out->percent = -1;
	out->eta_sec = -1;

	if (pp->total_frames > 0) {
		double pct = (double)m.frame * 100.0 / (double)pp->total_frames;
		out->percent = pct > 100 ? 100 : pct;
	}

	if (m.fps > 0) {
		pp->fps_history[pp->history_pos] = m.fps;
		pp->history_pos = (pp->history_pos + 1) % 5;
		if (pp->history_count < 5)
			pp->history_count++;
	}

	/* Same rule as C#: ETA once five fps samples exist. */
	if (pp->total_frames > 0 && pp->history_count == 5) {
		double avg = 0;
		for (int i = 0; i < 5; i++)
			avg += pp->fps_history[i];
		avg /= 5;

		if (avg > 0) {
			long long remaining = pp->total_frames - m.frame;
			if (remaining < 0)
				remaining = 0;
			out->eta_sec = (long long)((double)remaining / avg + 0.5);
		}
	}

	return IFME_LINE_PROGRESS;
}
