/*
 * test_encode.c - encoding jobs end to end
 *
 * Usage: test_encode IFME_ROOT WORK_DIR
 * Exits 77 (CTest "skipped") when the root, x265 or the sample is missing.
 *
 *   1. ifme_job_new() fills defaults from the source and the x265 plugin
 *   2. a short x265 encode (trimmed, downscaled) produces the right file
 *   3. stopping a running encode returns STOPPED, removes the temp folder
 *      and leaves no output behind
 *   4. audio: the default plugin follows the container, an A/V encode with
 *      the audio from a separate file (language, title, trim), and "copy"
 *      falling back to encoding when the container refuses the codec
 *   5. subtitles from external SRT/ASS files cut to the trim (read back),
 *      an attachment, cover art from a video frame, and burning a subtitle
 *      into a .ts (plus the error when the video is only copied)
 *   6. bad jobs give clear errors (unknown plugin, mode, bit depth, input)
 */

#include <pthread.h>
#include <stdatomic.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>

#include "ifme.h"
#include "cJSON.h"

#define OPUS_GUID "deadface-f00d-f00d-f00d-f00df00df00d"
#define AAC_GUID  "deadbeef-0aac-0aac-0aac-0aac0aac0aac"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
static void sleep_ms(int ms) { Sleep((DWORD)ms); }
#else
#include <unistd.h>
static void sleep_ms(int ms) { usleep((useconds_t)ms * 1000); }
#endif

static int failures;
#define CHECK(cond) do { if (!(cond)) { failures++; \
	fprintf(stderr, "%s:%d: CHECK failed: %s\n", __FILE__, __LINE__, #cond); } } while (0)

static bool exists(const char *path)
{
	struct stat st;
	return stat(path, &st) == 0;
}

static char *join(const char *a, const char *b)
{
	size_t n = strlen(a) + strlen(b) + 2;
	char *s = malloc(n);
	snprintf(s, n, "%s/%s", a, b);
	return s;
}

/* Edit a job: set a string or number field at top level or in video[0]. */
static char *edit_job(const char *job, const char *where, const char *key, const char *str, double num)
{
	cJSON *j = cJSON_Parse(job);
	cJSON *target = strcmp(where, "video") == 0
		? cJSON_GetArrayItem(cJSON_GetObjectItem(j, "video"), 0) : j;
	cJSON_DeleteItemFromObjectCaseSensitive(target, key);
	if (str)
		cJSON_AddStringToObject(target, key, str);
	else
		cJSON_AddNumberToObject(target, key, num);
	char *out = cJSON_Print(j);
	cJSON_Delete(j);
	return out;
}

static char *add_trim(const char *job, const char *start, const char *duration)
{
	cJSON *j = cJSON_Parse(job);
	cJSON *t = cJSON_AddObjectToObject(j, "trim");
	cJSON_AddStringToObject(t, "start", start);
	cJSON_AddStringToObject(t, "duration", duration);
	char *out = cJSON_Print(j);
	cJSON_Delete(j);
	return out;
}

static atomic_llong frame = -1;
static atomic_int stages;

static void on_progress(void *user, const ifme_progress *p)
{
	(void)user;
	atomic_store(&frame, p->frame);
}

static void on_stage(void *user, const char *stage)
{
	(void)user;
	atomic_fetch_add(&stages, 1);
	printf("  stage: %s\n", stage);
}

typedef struct job_run {
	ifme_ctx *ctx;
	ifme_task *task;
	const char *json;
	ifme_task_result result;
	char err[1024];
} job_run;

static void *worker(void *arg)
{
	job_run *r = arg;
	ifme_task_callbacks cb = { .on_progress = on_progress, .on_stage = on_stage };
	r->result = ifme_encode(r->task, r->ctx, r->json, &cb, r->err, sizeof(r->err));
	return NULL;
}

static ifme_task_result encode_now(ifme_ctx *ctx, const char *json, char *err, size_t len)
{
	ifme_task *t = ifme_task_create();
	ifme_task_callbacks cb = { .on_progress = on_progress, .on_stage = on_stage };
	ifme_task_result r = ifme_encode(t, ctx, json, &cb, err, len);
	ifme_task_destroy(t);
	return r;
}

int main(int argc, char **argv)
{
	if (argc < 3 || !argv[1][0]) {
		printf("skipped: no IFME root given (configure with -DIFME_TEST_ROOT=...)\n");
		return 77;
	}

	const char *root = argv[1], *work = argv[2];
	char *sample = join(root, "Samples/wonderland_online_dungeon.mp4");
	if (!exists(sample)) {
		printf("skipped: %s missing\n", sample);
		return 77;
	}

	ifme_ctx *ctx = ifme_create();
	CHECK(ctx && ifme_set_root_dir(ctx, root));

	char err[1024];
	if (ifme_plugin_find(ctx, IFME_PLUGIN_VIDEO, "x265", err, sizeof(err)) < 0) {
		printf("skipped: %s\n", err);
		return 77;
	}

	/* ---- 1. defaults ---------------------------------------------------------- */
	char *job = ifme_job_new(ctx, sample, NULL, "x265", NULL, err, sizeof(err));
	CHECK(job != NULL);
	if (!job) {
		fprintf(stderr, "%s\n", err);
		return 1;
	}
	cJSON *j = cJSON_Parse(job);
	cJSON *v = cJSON_GetArrayItem(cJSON_GetObjectItem(j, "video"), 0);
	CHECK(strcmp(cJSON_GetObjectItem(v, "plugin")->valuestring, "deadbeef-0265-0265-0265-026502650265") == 0);
	CHECK(cJSON_GetObjectItem(v, "width")->valueint == 1280);
	CHECK(cJSON_GetObjectItem(v, "height")->valueint == 800);
	CHECK(strcmp(cJSON_GetObjectItem(v, "fps")->valuestring, "15") == 0);
	CHECK(strcmp(cJSON_GetObjectItem(v, "preset")->valuestring, "medium") == 0);
	CHECK(cJSON_GetObjectItem(v, "_help") != NULL);
	cJSON_Delete(j);

	/* ---- 2. a short encode ----------------------------------------------------- */
	char *out = join(work, "test encode 出力.mkv");
	remove(out);
	char *j2 = edit_job(job, "", "output", out, 0);
	char *j3 = edit_job(j2, "video", "width", NULL, 320);
	char *j4 = edit_job(j3, "video", "height", NULL, 200);
	char *j5 = edit_job(j4, "video", "preset", "ultrafast", 0);
	char *short_job = add_trim(j5, "1", "3");

	CHECK(encode_now(ctx, short_job, err, sizeof(err)) == IFME_TASK_DONE);
	if (err[0])
		printf("  error: %s\n", err);
	CHECK(exists(out));

	ifme_media *m = ifme_probe(ctx, out, NULL, err, sizeof(err));
	CHECK(m != NULL);
	if (m) {
		CHECK(m->video_count == 1);
		CHECK(m->video_count == 1 && strcmp(m->video[0].codec, "hevc") == 0);
		CHECK(m->video_count == 1 && m->video[0].width == 320 && m->video[0].height == 200);
		CHECK(m->duration > 2.9 && m->duration < 3.1);
		printf("encoded:  %s %dx%d %.3f s\n", m->video_count ? m->video[0].codec : "?",
		       m->video_count ? m->video[0].width : 0, m->video_count ? m->video[0].height : 0, m->duration);
		ifme_media_free(m);
	}

	/* ---- 3. stop while encoding ------------------------------------------------- */
	char *temp = join(work, "ifme-test-temp");
	char *out2 = join(work, "stopped.mkv");
	remove(out2);
	char *s1 = edit_job(job, "", "output", out2, 0);
	char *s2 = edit_job(s1, "", "temp_dir", temp, 0);
	char *slow = edit_job(s2, "video", "preset", "veryslow", 0);

	job_run r = { ctx, ifme_task_create(), slow, IFME_TASK_ERROR, "" };
	atomic_store(&frame, -1);
	pthread_t th;
	pthread_create(&th, NULL, worker, &r);

	for (int i = 0; i < 600 && atomic_load(&frame) < 5; i++)
		sleep_ms(50);
	CHECK(atomic_load(&frame) >= 5);
	CHECK(exists(temp));
	ifme_task_stop(r.task);
	pthread_join(th, NULL);

	printf("stopped:  result %d (%s), temp %s, output %s\n", (int)r.result, r.err,
	       exists(temp) ? "LEFT BEHIND" : "removed", exists(out2) ? "EXISTS" : "none");
	CHECK(r.result == IFME_TASK_STOPPED);
	CHECK(!exists(temp));
	CHECK(!exists(out2));
	ifme_task_destroy(r.task);

	/* ---- 4. audio ------------------------------------------------------------------- */
	char *m4a = join(root, "Samples/wonderland_online_dungeon.m4a");
	char *av_out = join(work, "test av 音声.mkv");
	char *webm_out = join(work, "test fallback.webm");
	char *bad_audio = NULL;
	if (!exists(m4a) || ifme_plugin_find(ctx, IFME_PLUGIN_AUDIO, OPUS_GUID, err, sizeof(err)) < 0) {
		printf("audio:    skipped (%s)\n", exists(m4a) ? err : "no m4a sample");
	} else {
		/* Default plugin follows the container: Opus for MKV/WebM and plain
		 * audio files, AAC for MP4. */
		char *ja = ifme_job_new(ctx, m4a, NULL, "x265", NULL, err, sizeof(err));
		CHECK(ja != NULL);
		cJSON *jj = ja ? cJSON_Parse(ja) : NULL;
		const cJSON *a0 = cJSON_GetArrayItem(cJSON_GetObjectItem(jj, "audio"), 0);
		const cJSON *jout = cJSON_GetObjectItem(jj, "output");
		CHECK(cJSON_GetArraySize(cJSON_GetObjectItem(jj, "video")) == 0);
		CHECK(a0 && strcmp(cJSON_GetObjectItem(a0, "plugin")->valuestring, OPUS_GUID) == 0);
		CHECK(cJSON_IsString(jout) && strstr(jout->valuestring, ".ifme.opus") != NULL);
		cJSON_Delete(jj);
		ifme_free(ja);

		char *jm = ifme_job_new(ctx, m4a, "out.mp4", "none", NULL, err, sizeof(err));
		jj = jm ? cJSON_Parse(jm) : NULL;
		a0 = cJSON_GetArrayItem(cJSON_GetObjectItem(jj, "audio"), 0);
		CHECK(a0 && strcmp(cJSON_GetObjectItem(a0, "plugin")->valuestring, AAC_GUID) == 0);
		cJSON_Delete(jj);
		ifme_free(jm);

		/* Video from the sample, audio from the separate .m4a ("input"),
		 * tagged Japanese with a title; trimmed and downscaled. */
		remove(av_out);
		char *jv = ifme_job_new(ctx, sample, av_out, "x265", NULL, err, sizeof(err));
		char *jx = ifme_job_new(ctx, m4a, av_out, "none", NULL, err, sizeof(err));
		CHECK(jv && jx);
		cJSON *job_av = jv ? cJSON_Parse(jv) : NULL;
		cJSON *job_x = jx ? cJSON_Parse(jx) : NULL;
		cJSON *vv = cJSON_GetArrayItem(cJSON_GetObjectItem(job_av, "video"), 0);
		cJSON_ReplaceItemInObjectCaseSensitive(vv, "width", cJSON_CreateNumber(320));
		cJSON_ReplaceItemInObjectCaseSensitive(vv, "height", cJSON_CreateNumber(200));
		cJSON_ReplaceItemInObjectCaseSensitive(vv, "preset", cJSON_CreateString("ultrafast"));
		cJSON *ax = cJSON_DetachItemFromArray(cJSON_GetObjectItem(job_x, "audio"), 0);
		cJSON_ReplaceItemInObjectCaseSensitive(ax, "language", cJSON_CreateString("jpn"));
		cJSON_ReplaceItemInObjectCaseSensitive(ax, "title", cJSON_CreateString("BGM テスト"));
		cJSON_AddStringToObject(ax, "input", m4a);
		cJSON_AddItemToArray(cJSON_GetObjectItem(job_av, "audio"), ax);
		cJSON *trim = cJSON_AddObjectToObject(job_av, "trim");
		cJSON_AddStringToObject(trim, "start", "1");
		cJSON_AddStringToObject(trim, "duration", "3");
		char *av_job = cJSON_Print(job_av);

		CHECK(encode_now(ctx, av_job, err, sizeof(err)) == IFME_TASK_DONE);
		if (err[0])
			printf("  error: %s\n", err);
		m = ifme_probe(ctx, av_out, NULL, err, sizeof(err));
		CHECK(m && m->video_count == 1 && m->audio_count == 1);
		if (m && m->audio_count == 1) {
			const ifme_stream_audio *as = &m->audio[0];
			CHECK(strcmp(as->codec, "opus") == 0);
			CHECK(as->sample_rate == 48000 && as->channels == 2);
			CHECK(strcmp(as->language, "jpn") == 0);
			CHECK(strcmp(as->title, "BGM テスト") == 0);
			CHECK(m->duration > 2.9 && m->duration < 3.2);
			printf("a/v:      %s + %s %d Hz %d ch [%s] \"%s\" %.3f s\n", m->video[0].codec, as->codec,
			       as->sample_rate, as->channels, as->language, as->title, m->duration);
		}
		ifme_media_free(m);

		/* "copy": true into WebM: AAC is refused, so it is encoded to Opus. */
		remove(webm_out);
		char *jw = ifme_job_new(ctx, m4a, webm_out, "none", NULL, err, sizeof(err));
		cJSON *job_w = jw ? cJSON_Parse(jw) : NULL;
		cJSON *aw = cJSON_GetArrayItem(cJSON_GetObjectItem(job_w, "audio"), 0);
		cJSON_ReplaceItemInObjectCaseSensitive(aw, "copy", cJSON_CreateTrue());
		char *webm_job = cJSON_Print(job_w);
		CHECK(encode_now(ctx, webm_job, err, sizeof(err)) == IFME_TASK_DONE);
		m = ifme_probe(ctx, webm_out, NULL, err, sizeof(err));
		CHECK(m && m->audio_count == 1 && strcmp(m->audio[0].codec, "opus") == 0);
		printf("fallback: aac -> %s in .webm\n", m && m->audio_count ? m->audio[0].codec : "?");
		ifme_media_free(m);

		/* Bad audio mode. */
		cJSON_ReplaceItemInObjectCaseSensitive(aw, "mode", cJSON_CreateNumber(7));
		bad_audio = cJSON_Print(job_w);
		CHECK(encode_now(ctx, bad_audio, err, sizeof(err)) == IFME_TASK_ERROR);
		printf("bad audio:     %s\n", err);
		CHECK(strstr(err, "audio 1") != NULL);

		cJSON_free(av_job);
		cJSON_free(webm_job);
		cJSON_Delete(job_av);
		cJSON_Delete(job_x);
		cJSON_Delete(job_w);
		ifme_free(jv);
		ifme_free(jx);
		ifme_free(jw);
		remove(av_out);
		remove(webm_out);
	}

	/* ---- 5. subtitles, attachments, cover art, burning ---------------------------- */
	char *srt = join(work, "test sub.srt");
	char *ass = join(work, "test sub.ass");
	char *notes = join(work, "notes.txt");
	char *x_out = join(work, "test extras 字幕.mkv");
	char *back = join(work, "back.srt");
	char *ts_out = join(work, "test burn.ts");
	char *bad_burn = NULL;
	{
		FILE *f = fopen(srt, "wb");
		fputs("1\r\n00:00:00,500 --> 00:00:02,000\r\none\r\n\r\n2\r\n00:00:03,000 --> 00:00:06,000\r\ntwo\r\n", f);
		fclose(f);
		f = fopen(ass, "wb");
		fputs("[Script Info]\nScriptType: v4.00+\n\n[V4+ Styles]\n"
		      "Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, "
		      "Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, "
		      "Shadow, Alignment, MarginL, MarginR, MarginV, Encoding\n"
		      "Style: Default,Arial,20,&H00FFFFFF,&H000000FF,&H00000000,&H00000000,0,0,0,0,100,100,0,0,1,1,0,2,10,10,10,1\n\n"
		      "[Events]\nFormat: Layer, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text\n"
		      "Dialogue: 0,0:00:00.00,0:00:09.00,Default,,0,0,0,,sign\n", f);
		fclose(f);
		f = fopen(notes, "wb");
		fputs("attached by test_encode\n", f);
		fclose(f);

		/* Video 1..4 s, an external SRT and ASS, a text attachment, and the
		 * first frame of the sample as cover art (converted to PNG). */
		remove(x_out);
		char *jt = ifme_job_new(ctx, sample, x_out, "x265", "none", err, sizeof(err));
		cJSON *jx = jt ? cJSON_Parse(jt) : NULL;
		cJSON *vx = cJSON_GetArrayItem(cJSON_GetObjectItem(jx, "video"), 0);
		cJSON_ReplaceItemInObjectCaseSensitive(vx, "width", cJSON_CreateNumber(320));
		cJSON_ReplaceItemInObjectCaseSensitive(vx, "height", cJSON_CreateNumber(200));
		cJSON_ReplaceItemInObjectCaseSensitive(vx, "preset", cJSON_CreateString("ultrafast"));
		cJSON *subs = cJSON_AddArrayToObject(jx, "subtitles");
		cJSON *s1 = cJSON_CreateObject();
		cJSON_AddStringToObject(s1, "input", srt);
		cJSON_AddStringToObject(s1, "language", "eng");
		cJSON_AddBoolToObject(s1, "default", true);
		cJSON_AddItemToArray(subs, s1);
		cJSON *s2 = cJSON_CreateObject();
		cJSON_AddStringToObject(s2, "input", ass);
		cJSON_AddStringToObject(s2, "language", "jpn");
		cJSON_AddStringToObject(s2, "title", "Signs");
		cJSON_AddItemToArray(subs, s2);
		cJSON *at = cJSON_CreateObject();
		cJSON_AddStringToObject(at, "file", notes);
		cJSON_AddItemToArray(cJSON_AddArrayToObject(jx, "attachments"), at);
		cJSON_AddStringToObject(cJSON_AddObjectToObject(jx, "cover"), "input", sample);
		cJSON *tr = cJSON_AddObjectToObject(jx, "trim");
		cJSON_AddStringToObject(tr, "start", "1");
		cJSON_AddStringToObject(tr, "duration", "3");
		char *x_job = cJSON_Print(jx);

		CHECK(encode_now(ctx, x_job, err, sizeof(err)) == IFME_TASK_DONE);
		if (err[0])
			printf("  error: %s\n", err);
		m = ifme_probe(ctx, x_out, NULL, err, sizeof(err));
		CHECK(m != NULL);
		if (m) {
			CHECK(m->subtitle_count == 2);
			CHECK(m->subtitle_count == 2 && strcmp(m->subtitle[0].language, "eng") == 0 &&
			      strcmp(m->subtitle[1].language, "jpn") == 0 && strcmp(m->subtitle[1].title, "Signs") == 0 &&
			      strcmp(m->subtitle[1].codec, "ass") == 0);
			CHECK(m->attachment_count == 1 && strcmp(m->attachment[0].filename, "notes.txt") == 0 &&
			      strcmp(m->attachment[0].mime_type, "text/plain") == 0);
			CHECK(m->video_count == 2 && m->video[1].attached_pic);   /* MKV cover = attachment */
			CHECK(m->duration > 2.9 && m->duration < 3.2);            /* subtitles do not stretch it */
			printf("extras:   %zu subtitles, %zu attachment, cover %s, %.3f s\n", m->subtitle_count,
			       m->attachment_count, m->video_count == 2 ? m->video[1].codec : "none", m->duration);
		}
		ifme_media_free(m);

		/* The SRT read back: cut to 1..4 s and shifted (FFmpeg alone dropped
		 * the cue that starts before the cut). */
		remove(back);
		const char *ex[] = { "ffmpeg", "-v", "error", "-i", x_out, "-map", "0:s:0", "-y", back };
		ifme_task *et = ifme_task_create();
		int code = -1;
		CHECK(ifme_task_exec(et, ctx, ex, sizeof(ex) / sizeof(ex[0]), NULL, 0, NULL, &code, err, sizeof(err))
		      == IFME_TASK_DONE);
		ifme_task_destroy(et);
		FILE *bf = fopen(back, "rb");
		char text[512] = "";
		if (bf) {
			text[fread(text, 1, sizeof(text) - 1, bf)] = '\0';
			fclose(bf);
		}
		CHECK(strstr(text, "00:00:00,000 --> 00:00:01,000") && strstr(text, "one"));
		CHECK(strstr(text, "00:00:02,000 --> 00:00:03,000") && strstr(text, "two"));
		if (!strstr(text, "00:00:02,000 --> 00:00:03,000"))
			printf("  srt read back:\n%s\n", text);

		/* A .ts cannot hold the subtitles: burned into the picture instead. */
		remove(ts_out);
		char *jb = ifme_job_new(ctx, sample, ts_out, "x265", "none", err, sizeof(err));
		cJSON *jbj = jb ? cJSON_Parse(jb) : NULL;
		cJSON *vb = cJSON_GetArrayItem(cJSON_GetObjectItem(jbj, "video"), 0);
		cJSON_ReplaceItemInObjectCaseSensitive(vb, "preset", cJSON_CreateString("ultrafast"));
		cJSON_AddItemToObject(jbj, "subtitles", cJSON_Duplicate(subs, true));
		cJSON_AddBoolToObject(jbj, "burn_subtitle", true);
		cJSON *tb = cJSON_AddObjectToObject(jbj, "trim");
		cJSON_AddStringToObject(tb, "duration", "2");
		char *b_job = cJSON_Print(jbj);
		CHECK(encode_now(ctx, b_job, err, sizeof(err)) == IFME_TASK_DONE);
		if (err[0])
			printf("  error: %s\n", err);
		m = ifme_probe(ctx, ts_out, NULL, err, sizeof(err));
		CHECK(m && m->video_count == 1 && m->subtitle_count == 0);
		printf("burned:   %s, %zu subtitle streams left\n", m ? "ok" : "?", m ? m->subtitle_count : 99);
		ifme_media_free(m);

		/* Burning needs a video that is encoded. */
		cJSON_ReplaceItemInObjectCaseSensitive(vb, "plugin", cJSON_CreateString("copy"));
		bad_burn = cJSON_Print(jbj);
		CHECK(encode_now(ctx, bad_burn, err, sizeof(err)) == IFME_TASK_ERROR);
		printf("bad burn:      %s\n", err);
		CHECK(strstr(err, "burn") != NULL);

		cJSON_free(x_job);
		cJSON_free(b_job);
		cJSON_Delete(jx);
		cJSON_Delete(jbj);
		ifme_free(jt);
		ifme_free(jb);
		remove(x_out);
		remove(ts_out);
		remove(back);
		remove(srt);
		remove(ass);
		remove(notes);
	}

	/* ---- 6. bad jobs -------------------------------------------------------------- */
	char *b1 = edit_job(job, "video", "plugin", "no-such-encoder", 0);
	CHECK(encode_now(ctx, b1, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad plugin:    %s\n", err);
	CHECK(strstr(err, "no-such-encoder") != NULL);

	char *b2 = edit_job(job, "video", "mode", NULL, 9);
	CHECK(encode_now(ctx, b2, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad mode:      %s\n", err);

	char *b3 = edit_job(job, "video", "bit_depth", NULL, 16);
	CHECK(encode_now(ctx, b3, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad bit depth: %s\n", err);
	CHECK(strstr(err, "8, 10, 12") != NULL);

	char *b4 = edit_job(job, "", "input", "Z:/missing/file.mkv", 0);
	CHECK(encode_now(ctx, b4, err, sizeof(err)) == IFME_TASK_ERROR);
	printf("bad input:     %s\n", err);

	CHECK(encode_now(ctx, "{ not json", err, sizeof(err)) == IFME_TASK_ERROR);

	remove(out);
	cJSON_free(b1); cJSON_free(b2); cJSON_free(b3); cJSON_free(b4); cJSON_free(bad_audio);
	free(m4a); free(av_out); free(webm_out);
	free(srt); free(ass); free(notes); free(x_out); free(back); free(ts_out); cJSON_free(bad_burn);
	cJSON_free(s1); cJSON_free(s2); cJSON_free(slow);
	cJSON_free(j2); cJSON_free(j3); cJSON_free(j4); cJSON_free(j5); cJSON_free(short_job);
	ifme_free(job);
	free(out); free(out2); free(temp); free(sample);
	ifme_destroy(ctx);

	printf("%s\n", failures ? "FAILED" : "OK");
	return failures ? 1 : 0;
}
