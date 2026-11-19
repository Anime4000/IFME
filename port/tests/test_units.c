/*
 * test_units.c - unit tests for core helpers (no external programs needed)
 *
 * The progress lines are real output captured from the encoders IFME ships
 * (x264, x265, VVenC, SVT-AV1, FFmpeg), plus NVEncC's documented format.
 */

#include <math.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"
#include "cJSON.h"

static int failures, checks;

#define CHECK(cond) do { \
	checks++; \
	if (!(cond)) { failures++; fprintf(stderr, "%s:%d: CHECK failed: %s\n", __FILE__, __LINE__, #cond); } \
} while (0)

#define CHECK_NEAR(a, b) CHECK(fabs((double)(a) - (double)(b)) < 0.01)

static ifme_progress feed(ifme_progress_parser *pp, const char *line, int expect_kind)
{
	ifme_progress p;
	memset(&p, 0, sizeof(p));
	int kind = ifme_progress_feed(pp, line, &p);
	if (kind != expect_kind)
		fprintf(stderr, "  line: \"%s\" kind %d, expected %d\n", line, kind, expect_kind);
	CHECK(kind == expect_kind);
	return p;
}

static void test_progress_formats(void)
{
	ifme_progress_parser pp;
	ifme_progress p;

	/* x265 / x264 with a known total; the total is learned from the line. */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "[0.6%] 1/165 frames, 5.99 fps, 2283.72 kb/s, eta 0:00:27  ", IFME_LINE_PROGRESS);
	CHECK(p.frame == 1);
	CHECK(p.total_frames == 165);
	CHECK_NEAR(p.fps, 5.99);
	CHECK_NEAR(p.kbps, 2283.72);
	CHECK(p.percent > 0.5 && p.percent < 0.7);

	/* x264 / x265 without a total. */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "1 frames: 31.25 fps, 2851.56 kb/s  ", IFME_LINE_PROGRESS);
	CHECK(p.frame == 1);
	CHECK_NEAR(p.fps, 31.25);
	CHECK_NEAR(p.kbps, 2851.56);
	CHECK(p.percent < 0);

	/* Rigaya NVEncC / QSVEncC. */
	ifme_progress_init(&pp, 1000);
	p = feed(&pp, "[12.3%] 123 frames: 456.78 fps, 1234 kb/s, remain 0:00:01, GPU 10%, VE 20%", IFME_LINE_PROGRESS);
	CHECK(p.frame == 123);
	CHECK_NEAR(p.fps, 456.78);
	CHECK_NEAR(p.kbps, 1234);

	/* VVenC (avg_ values, like C#), and its summary is not progress. */
	ifme_progress_init(&pp, 165);
	p = feed(&pp, "vvenc [info]: stats:  frame=   15 fps=  69.4 avg_fps=  69.4 bitrate=   133.30 kbps "
	              "avg_bitrate=   133.30 kbps elapsed= 00h:00m:01s    ", IFME_LINE_PROGRESS);
	CHECK(p.frame == 15);
	CHECK_NEAR(p.fps, 69.4);
	CHECK_NEAR(p.kbps, 133.30);
	feed(&pp, "vvenc [info]: stats summary: frame= 165 avg_fps= 131.0 avg_bitrate= 126.85 kbps", IFME_LINE_STATUS);

	/* FFmpeg, including bitrate=N/A at the start. */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "frame=  165 fps=0.0 q=-1.0 Lsize=    1911KiB time=00:00:10.86 bitrate=1440.8kbits/s "
	              "speed=21.7x elapsed=0:00:00.49    ", IFME_LINE_PROGRESS);
	CHECK(p.frame == 165);
	CHECK_NEAR(p.kbps, 1440.8);
	p = feed(&pp, "frame=    0 fps=0.0 q=0.0 size=       0KiB time=N/A bitrate=N/A speed=N/A", IFME_LINE_PROGRESS);
	CHECK(p.frame == 0);
	CHECK(p.kbps < 0);

	/* SVT-AV1 --progress 2, with the ANSI colours it really prints. */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "Encoding: \x1b[33m   1 Frames\x1b[0m @ \x1b[32m33.90\x1b[0m fps | \x1b[35m46.66 kb/s\x1b[0m | "
	              "Size: \x1b[31m0.03 MB\x1b[0m | Time: \x1b[36m0:00:00\x1b[0m ", IFME_LINE_PROGRESS);
	CHECK(p.frame == 1);
	CHECK_NEAR(p.fps, 33.90);
	CHECK_NEAR(p.kbps, 46.66);
	p = feed(&pp, "Encoding:   12/165 Frames @ 40.00 fps | 50.00 kb/s | Size: 0.10 MB", IFME_LINE_PROGRESS);
	CHECK(p.frame == 12 && p.total_frames == 165);

	/* SVT-AV1 legacy format (the one C# knew). */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "Encoding frame  125   1234.56 kbps   45.32 fps", IFME_LINE_PROGRESS);
	CHECK(p.frame == 125);
	CHECK_NEAR(p.fps, 45.32);
	CHECK_NEAR(p.kbps, 1234.56);

	/* Summary lines set the total and go to the log. */
	ifme_progress_init(&pp, 0);
	feed(&pp, "encoded 165 frames in 0.54s (304.43 fps), 120.32 kb/s, Avg QP:32.23", IFME_LINE_LOG);
	CHECK(pp.total_frames == 165);
	feed(&pp, "encoded 495 frames, 1289.06 fps, 174.81 kb/s", IFME_LINE_LOG);
	CHECK(pp.total_frames == 495);

	/* Ordinary log lines. */
	feed(&pp, "x265 [info]: Main profile, Level-4 (Main tier)", IFME_LINE_LOG);
	feed(&pp, "x264 [info]: profile High, level 1.2, 4:2:0, 8-bit", IFME_LINE_LOG);
}

static void test_eta(void)
{
	ifme_progress_parser pp;
	ifme_progress p;

	/* ETA appears after five fps samples, as in C#. */
	ifme_progress_init(&pp, 100);
	for (int i = 1; i <= 4; i++) {
		char line[64];
		snprintf(line, sizeof(line), "%d frames: 10.00 fps, 100 kb/s", i * 10);
		p = feed(&pp, line, IFME_LINE_PROGRESS);
		CHECK(p.eta_sec == -1);
	}
	p = feed(&pp, "50 frames: 10.00 fps, 100 kb/s", IFME_LINE_PROGRESS);
	CHECK(p.eta_sec == 5);
	CHECK_NEAR(p.percent, 50);

	/* Below 0.5 fps: C# rounded the average to 0 and never showed an ETA. */
	ifme_progress_init(&pp, 10);
	for (int i = 0; i < 5; i++)
		p = feed(&pp, "0 frames: 0.40 fps, 100 kb/s", IFME_LINE_PROGRESS);
	CHECK(p.eta_sec == 25);

	/* Percentage is capped at 100. */
	ifme_progress_init(&pp, 10);
	p = feed(&pp, "12 frames: 1.00 fps, 1 kb/s", IFME_LINE_PROGRESS);
	CHECK_NEAR(p.percent, 100);
}

/* Audio: FFmpeg reports media time, no frames. */
static void test_time_progress(void)
{
	ifme_progress_parser pp;
	ifme_progress p;

	/* Real FFmpeg 7 audio stats; ETA from speed (30 s left at 15x = 2 s). */
	ifme_progress_init(&pp, 0);
	pp.total_seconds = 60;
	p = feed(&pp, "size=     512KiB time=00:00:30.00 bitrate= 139.8kbits/s speed=  15x elapsed=0:00:02.00",
	         IFME_LINE_PROGRESS);
	CHECK(p.frame == -1);
	CHECK_NEAR(p.time_sec, 30);
	CHECK_NEAR(p.total_sec, 60);
	CHECK_NEAR(p.percent, 50);
	CHECK_NEAR(p.kbps, 139.8);
	CHECK(p.eta_sec == 2);

	/* The WAV pipe of piped encoders: no bitrate, hours, and the negative
	 * start time FFmpeg prints for some codecs. */
	p = feed(&pp, "size=N/A time=01:00:00.00 bitrate=N/A speed=60.1x", IFME_LINE_PROGRESS);
	CHECK_NEAR(p.time_sec, 3600);
	CHECK(p.kbps < 0);
	CHECK_NEAR(p.percent, 100);
	CHECK(p.eta_sec == 0);
	p = feed(&pp, "size=       0KiB time=-00:00:00.02 bitrate=N/A speed=N/A", IFME_LINE_PROGRESS);
	CHECK_NEAR(p.time_sec, 0);

	/* Unknown length: no percent, no ETA. */
	ifme_progress_init(&pp, 0);
	p = feed(&pp, "size=  64KiB time=00:00:04.00 bitrate= 131.1kbits/s speed=40x", IFME_LINE_PROGRESS);
	CHECK(p.percent < 0 && p.eta_sec == -1);

	/* FFmpeg video lines also carry the time; with no frame total, the
	 * percent comes from it (stream copies, the final mux). */
	ifme_progress_init(&pp, 0);
	pp.total_seconds = 20;
	p = feed(&pp, "frame=  150 fps=0.0 q=-1.0 size=1024KiB time=00:00:05.00 bitrate=1677.7kbits/s speed=10x",
	         IFME_LINE_PROGRESS);
	CHECK(p.frame == 150);
	CHECK_NEAR(p.time_sec, 5);
	CHECK_NEAR(p.percent, 25);

	/* A "time=" without size=/speed= is not progress. */
	ifme_progress_init(&pp, 0);
	feed(&pp, "Input #0, wav, from 'pipe:': time=00:00:01.00", IFME_LINE_LOG);
}

/* Cutting text subtitles and chapters to a trim (subtime.c). */
static void test_subtitle_cut(void)
{
	size_t kept;

	/* SubRip, cut to 3..7 s: the cue overlapping the start is clipped to 0
	 * (FFmpeg dropped it), the one past the end is clipped to the length,
	 * the one outside goes, and cues are renumbered. CRLF is kept. */
	const char *srt =
		"1\r\n00:00:01,000 --> 00:00:04,000\r\nfirst\r\n\r\n"
		"2\r\n00:00:05,000 --> 00:00:09,000\r\nsecond\r\nline two\r\n\r\n"
		"3\r\n00:00:20,000 --> 00:00:21,000\r\nlater\r\n";
	char *out = ifme_subtitle_cut(srt, strlen(srt), "srt", 3, 4, &kept);
	CHECK(kept == 2);
	CHECK(out && strcmp(out,
		"1\r\n00:00:00,000 --> 00:00:01,000\r\nfirst\r\n\r\n"
		"2\r\n00:00:02,000 --> 00:00:04,000\r\nsecond\r\nline two\r\n\r\n") == 0);
	if (out && kept != 2)
		fprintf(stderr, "srt cut:\n%s\n", out);
	free(out);

	/* ASS: only Dialogue lines change; centisecond times, commas in text. */
	const char *ass =
		"[Script Info]\nTitle: x\n\n[Events]\nFormat: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text\n"
		"Dialogue: 0,0:00:00.50,0:00:10.00,Default,,0,0,0,,Hello, world\n"
		"Comment: 0,0:00:00.00,0:00:01.00,Default,,0,0,0,,note\n"
		"Dialogue: 0,0:00:01.00,0:00:02.00,Default,,0,0,0,,gone\n"
		"Dialogue: 1,1:00:00.00,1:00:01.25,Default,,0,0,0,,hour\n";
	out = ifme_subtitle_cut(ass, strlen(ass), "ass", 3, 3600, &kept);
	CHECK(kept == 2);
	CHECK(out && strstr(out, "Dialogue: 0,0:00:00.00,0:00:07.00,Default,,0,0,0,,Hello, world\n"));
	CHECK(out && strstr(out, "Dialogue: 1,0:59:57.00,0:59:58.25,Default,,0,0,0,,hour\n"));
	CHECK(out && !strstr(out, "gone") && strstr(out, "Comment: 0,0:00:00.00") && strstr(out, "[Script Info]"));
	free(out);

	/* WebVTT: header kept, MM:SS.mmm times, cue settings kept. */
	const char *vtt = "WEBVTT\n\n00:04.000 --> 00:06.500 align:start\nhi\n";
	out = ifme_subtitle_cut(vtt, strlen(vtt), "vtt", 5, 10, &kept);
	CHECK(kept == 1);
	CHECK(out && strcmp(out, "WEBVTT\n\n00:00:00.000 --> 00:00:01.500 align:start\nhi\n\n") == 0);
	free(out);

	/* Nothing left. */
	out = ifme_subtitle_cut(srt, strlen(srt), "srt", 30, 5, &kept);
	CHECK(kept == 0);
	free(out);

	/* Chapters FFmpeg cut to nothing are dropped. */
	const char *ini = ";FFMETADATA1\ntitle=t\n[CHAPTER]\nTIMEBASE=1/1000\nSTART=0\nEND=4000\ntitle=a\n"
	                  "[CHAPTER]\nTIMEBASE=1/1000\nSTART=4000\nEND=4000\ntitle=b\n[STREAM]\nx=1\n";
	size_t dropped;
	out = ifme_chapters_drop_empty(ini, strlen(ini), &dropped);
	CHECK(dropped == 1);
	CHECK(out && strstr(out, "title=a") && !strstr(out, "title=b") && strstr(out, "[STREAM]\nx=1"));
	free(out);
}

static void test_clean_line(void)
{
	char *s = ifme_clean_line("  \x1b[33mhello\x1b[0m\tworld\b  ");
	CHECK(s && strcmp(s, "hello world") == 0);
	free(s);

	s = ifme_clean_line("        ");
	CHECK(s && *s == '\0');
	free(s);
}

/* Line splitting on \r and \n. */
static char lines_seen[8][64];
static int lines_count;

static void collect(void *user, const char *line)
{
	(void)user;
	if (lines_count < 8)
		snprintf(lines_seen[lines_count++], 64, "%s", line);
}

static void test_linebuf_split(void)
{
	ifme_linebuf lb;
	memset(&lb, 0, sizeof(lb));
	lines_count = 0;

	const char *chunk1 = "a\r\nfra";
	const char *chunk2 = "me 1\rframe 2\r\n\nlast";
	ifme_linebuf_feed(&lb, chunk1, strlen(chunk1), collect, NULL);
	ifme_linebuf_feed(&lb, chunk2, strlen(chunk2), collect, NULL);
	ifme_linebuf_flush(&lb, collect, NULL);

	CHECK(lines_count == 4);
	CHECK(strcmp(lines_seen[0], "a") == 0);
	CHECK(strcmp(lines_seen[1], "frame 1") == 0);
	CHECK(strcmp(lines_seen[2], "frame 2") == 0);
	CHECK(strcmp(lines_seen[3], "last") == 0);
}

static void test_argv_and_json(void)
{
	ifme_argv a = { 0 };
	ifme_argv_split(&a, "-vf \"scale=1:2,fps=15\"  -y 'a b' off 0");
	CHECK(a.n == 6);
	CHECK(a.n == 6 && strcmp(a.v[1], "scale=1:2,fps=15") == 0);
	CHECK(a.n == 6 && strcmp(a.v[3], "a b") == 0);
	CHECK(a.n == 6 && strcmp(a.v[4], "off") == 0);   /* real values are kept */
	CHECK(a.v[a.n] == NULL);
	ifme_argv_free(&a);

	CHECK(ifme_is_disabled(NULL));
	CHECK(ifme_is_disabled("  "));
	CHECK(ifme_is_disabled("Disable"));
	CHECK(ifme_is_disabled("0"));
	CHECK(!ifme_is_disabled("-b:a"));

	/* BOM, trailing commas and comments go; strings stay untouched. */
	char *j = ifme_json_relax("\xEF\xBB\xBF{ \"a\": [1, 2,], // note\n \"s\": \"x,]//y\", /* c */ }");
	cJSON *parsed = j ? cJSON_Parse(j) : NULL;
	CHECK(parsed != NULL);
	CHECK(cJSON_GetArraySize(cJSON_GetObjectItem(parsed, "a")) == 2);
	CHECK(cJSON_IsString(cJSON_GetObjectItem(parsed, "s")) &&
	      strcmp(cJSON_GetObjectItem(parsed, "s")->valuestring, "x,]//y") == 0);
	cJSON_Delete(parsed);
	free(j);
}

int main(void)
{
	test_progress_formats();
	test_eta();
	test_time_progress();
	test_subtitle_cut();
	test_clean_line();
	test_linebuf_split();
	test_argv_and_json();

	printf("%d checks, %d failed\n", checks, failures);
	return failures ? 1 : 0;
}
