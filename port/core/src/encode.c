/*
 * encode.c - encoding jobs: video (port of MediaEncoding.Video)
 *
 * Same command shapes as the C# encoder:
 *   pipe:     ffmpeg -i IN -map 0:N [-ss -t] -pix_fmt .. -vf scale,fps,.. -f y4m - |
 *             ENCODER {Y4M} {Input} [frames] {res fps bits csp} {preset} {tune}
 *                     {mode} {Command} {user args} [pass] {Output} OUT
 *   direct:   ENCODER {Input} IN {..same..} {UnPipe} {user args} {Command} [pass] {Output} OUT
 *             (FFmpeg-based plugins get the -map/-pix_fmt/-vf part instead)
 *   raw output (x264, x265, VVC, NVEncC...) -> MP4Box into the plugin's
 *   container, then every video track -> the job's output file.
 *
 * Differences from C#, on purpose:
 *  - Commands are argument lists, run without a shell.
 *  - A missing N-bit encoder is an error message, not a NullReferenceException.
 *  - Scaling is only added when the size actually changes.
 *  - Frame rates such as 24000/1001 are kept exact (C# printed a rounded float).
 *  - When MP4Box is missing, ffmpeg wraps the raw stream instead.
 * Not ported yet: fast remux, auto-crop, hard subtitles, thumbnails
 * (they depend on later milestones).
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"
#include "cJSON.h"

#define VIDEO_COPY_GUID "00000000-0000-0000-0000-000000000000"
#define LOG_KEEP 8

typedef struct enc {
	ifme_ctx *ctx;
	ifme_control *control;
	const ifme_task_callbacks *cb;
	char *ffmpeg;
	char *input;               /* absolute */
	char *temp;
	const cJSON *job;
	ifme_media *media;
	double trim_seconds;       /* 0: no trim */
	char *err;
	size_t err_len;

	ifme_progress_parser parser;
	char recent[LOG_KEEP][256];   /* last log lines, for error messages */
	int recent_count, recent_pos;
} enc;

/* ---- small helpers --------------------------------------------------------- */

static void fail(enc *E, const char *fmt, ...)
{
	if (!E->err || !E->err_len)
		return;
	va_list ap;
	va_start(ap, fmt);
	vsnprintf(E->err, E->err_len, fmt, ap);
	va_end(ap);
}

static void stage(enc *E, const char *fmt, ...)
{
	if (!E->cb || !E->cb->on_stage)
		return;
	va_list ap;
	va_start(ap, fmt);
	char *s = ifme_vstrdupf(fmt, ap);
	va_end(ap);
	if (s)
		E->cb->on_stage(E->cb->user, s);
	free(s);
}

static void log_line(enc *E, const char *line)
{
	if (E->cb && E->cb->on_output)
		E->cb->on_output(E->cb->user, line, false);
}

/* "HH:MM:SS.mmm", "MM:SS" or plain seconds -> seconds; < 0 if invalid. */
static double parse_time(const char *s)
{
	if (!s || !*s)
		return -1;

	double total = 0;
	const char *p = s;
	for (int parts = 0; parts < 3; parts++) {
		char *end = NULL;
		double v = strtod(p, &end);
		if (end == p)
			return -1;
		total = total * 60 + v;
		if (*end != ':')
			return *end == '\0' ? total : -1;
		p = end + 1;
	}
	return -1;
}

/* "24000/1001" or "23.976" -> 23.976...; 0 if invalid. */
static double parse_fps(const char *s)
{
	if (!s || !*s)
		return 0;
	char *end = NULL;
	double n = strtod(s, &end);
	if (end && *end == '/') {
		double d = strtod(end + 1, NULL);
		return d > 0 ? n / d : 0;
	}
	return (end && *end == '\0') ? n : 0;
}

static long long round_ll(double x)
{
	return (long long)(x < 0 ? x - 0.5 : x + 0.5);
}

/* Keep NTSC rates exact ("24000/1001"), whole rates as integers. */
static void fps_string(double fps, char *buf, size_t len)
{
	if (fps <= 0) {
		snprintf(buf, len, "0");
		return;
	}

	long long whole = round_ll(fps);
	if (fps - (double)whole < 0.0005 && (double)whole - fps < 0.0005) {
		snprintf(buf, len, "%lld", whole);
		return;
	}

	long long ntsc = round_ll(fps * 1001.0);
	double back = (double)ntsc / 1001.0;
	if (ntsc % 1000 == 0 && back - fps < 0.0001 && fps - back < 0.0001) {
		snprintf(buf, len, "%lld/1001", ntsc);
		return;
	}
	snprintf(buf, len, "%.3f", fps);
}

/* ArgsParser.Parse: "{arg} {value}" unless either is blank or value is a
 * "switched off" word. */
static void push_arg_value(ifme_argv *a, const char *arg, const char *value)
{
	static const char *const off[] = { "0", "no", "off", "non", "none", "blank", "disable", "disabled" };

	if (!arg || !*arg || !value || !*value)
		return;
	for (size_t i = 0; i < sizeof(off) / sizeof(off[0]); i++)
		if (ifme_strcasecmp(value, off[i]) == 0)
			return;

	ifme_argv_split(a, arg);
	ifme_argv_push(a, value);
}

/* Replace every "{0}" in tmpl with value. Caller frees. */
static char *format_value(const char *tmpl, const char *value)
{
	size_t vl = strlen(value), n = 1;
	for (const char *p = tmpl; *p; p++)
		n += (p[0] == '{' && p[1] == '0' && p[2] == '}') ? vl : 1;

	char *out = malloc(n);
	if (!out)
		return NULL;

	size_t o = 0;
	for (const char *p = tmpl; *p; p++) {
		if (p[0] == '{' && p[1] == '0' && p[2] == '}') {
			memcpy(out + o, value, vl);
			o += vl;
			p += 2;
		} else {
			out[o++] = *p;
		}
	}
	out[o] = '\0';
	return out;
}

static bool has_placeholder(const char *s)
{
	for (; s && *s; s++)
		if (s[0] == '{' && s[1] >= '0' && s[1] <= '9')
			return true;
	return false;
}

/* ---- running one step --------------------------------------------------------- */

static void enc_line(void *user, const char *line)
{
	enc *E = user;
	char *clean = ifme_clean_line(line);
	if (!clean || !*clean) {
		free(clean);
		return;
	}

	ifme_progress p;
	int kind = ifme_progress_feed(&E->parser, clean, &p);

	if (kind == IFME_LINE_LOG) {
		snprintf(E->recent[E->recent_pos], sizeof(E->recent[0]), "%s", clean);
		E->recent_pos = (E->recent_pos + 1) % LOG_KEEP;
		if (E->recent_count < LOG_KEEP)
			E->recent_count++;
	}

	if (E->cb && E->cb->on_output)
		E->cb->on_output(E->cb->user, clean, kind != IFME_LINE_LOG);
	if (kind == IFME_LINE_PROGRESS && E->cb && E->cb->on_progress)
		E->cb->on_progress(E->cb->user, &p);
	free(clean);
}

static void enc_tick(void *user)
{
	enc *E = user;
	if (E->cb && E->cb->on_tick)
		E->cb->on_tick(E->cb->user);
}

/* Run stages in the temp folder with progress; fills E->err on failure. */
static ifme_task_result run(enc *E, ifme_argv *stages, size_t n, long long total_frames)
{
	for (size_t i = 0; i < n; i++) {
		char *cmd = ifme_argv_display(&stages[i]);
		if (cmd) {
			char *line = ifme_strdupf("%s %s", i ? "|" : "$", cmd);
			if (line)
				log_line(E, line);
			free(line);
		}
		free(cmd);
	}

	ifme_progress_init(&E->parser, total_frames);
	E->recent_count = E->recent_pos = 0;

	ifme_run_opts opts;
	memset(&opts, 0, sizeof(opts));
	opts.cwd = E->temp;
	opts.on_line = enc_line;
	opts.on_tick = enc_tick;
	opts.user = E;
	opts.control = E->control;

	ifme_run_result r;
	int rc = ifme_run(stages, n, &opts, &r);
	ifme_task_result result;

	if (rc == IFME_RUN_STOPPED) {
		fail(E, "stopped");
		result = IFME_TASK_STOPPED;
	} else if (rc != IFME_RUN_OK) {
		fail(E, "%s", r.message);
		result = IFME_TASK_ERROR;
	} else if (r.exit_code != 0) {
		/* Name the program that failed, plus its last words. */
		size_t used = 0;
		if (E->err && E->err_len)
			used = (size_t)snprintf(E->err, E->err_len, "%s exited with code %d",
			                        ifme_basename(stages[n - 1].v[0]), r.exit_code);
		for (int k = 0; k < E->recent_count && E->err && used < E->err_len; k++) {
			int idx = (E->recent_pos - E->recent_count + k + LOG_KEEP) % LOG_KEEP;
			used += (size_t)snprintf(E->err + used, E->err_len - used, "\n  %s", E->recent[idx]);
		}
		result = IFME_TASK_FAILED;
	} else {
		result = IFME_TASK_DONE;
	}

	ifme_run_result_free(&r);
	return result;
}

static ifme_task_result run1(enc *E, ifme_argv *a, long long total)
{
	return run(E, a, 1, total);
}

/* ---- video ------------------------------------------------------------------ */

static const ifme_stream_video *source_video(const ifme_media *m, int stream)
{
	for (size_t i = 0; i < m->video_count; i++)
		if (m->video[i].id == stream)
			return &m->video[i];
	return NULL;
}

static int first_video_stream(const ifme_media *m)
{
	for (size_t i = 0; i < m->video_count; i++)
		if (!m->video[i].attached_pic)
			return m->video[i].id;
	return -1;
}

static double source_fps(const ifme_stream_video *v)
{
	if (v->frame_rate > 0)
		return v->frame_rate;
	if (v->frame_rate_avg > 0)
		return v->frame_rate_avg;
	return 24000.0 / 1001.0;   /* C#: val_fps fallback 23.976 */
}

/* Encode video item `idx`; on success *result is the track file in E->temp. */
static ifme_task_result encode_video(enc *E, size_t idx, size_t count, const cJSON *item, char **result)
{
	*result = NULL;

	int stream = ifme_jint(item, "stream", first_video_stream(E->media));
	const ifme_stream_video *src = source_video(E->media, stream);
	if (!src) {
		fail(E, "video %zu: input has no video stream %d", idx + 1, stream);
		return IFME_TASK_ERROR;
	}

	const char *lang = (src->language && *src->language) ? src->language : "und";
	const char *plugin_query = ifme_jstr(item, "plugin");

	/* Built-in "copy" entry: keep the stream as it is. */
	if (plugin_query && ifme_strcasecmp(plugin_query, VIDEO_COPY_GUID) == 0) {
		char *out = ifme_strdupf("video%04zu_%s.mkv", idx, lang);
		ifme_argv a = { 0 };
		ifme_argv_push(&a, E->ffmpeg);
		ifme_argv_split(&a, "-hide_banner -v error -i");
		ifme_argv_push(&a, E->input);
		ifme_argv_pushf(&a, "-map");
		ifme_argv_pushf(&a, "0:%d", stream);
		ifme_argv_split(&a, "-c:v copy -y");
		ifme_argv_push(&a, out);

		stage(E, "Video %zu/%zu: copying stream", idx + 1, count);
		ifme_task_result r = run1(E, &a, 0);
		ifme_argv_free(&a);
		if (r == IFME_TASK_DONE)
			*result = out;
		else
			free(out);
		return r;
	}

	char why[512];
	int pi = ifme_plugin_find(E->ctx, IFME_PLUGIN_VIDEO, plugin_query ? plugin_query : "", why, sizeof(why));
	if (pi < 0) {
		fail(E, "video %zu: %s", idx + 1, why[0] ? why : "no \"plugin\" given");
		return IFME_TASK_ERROR;
	}

	const cJSON *manifest = ifme_plugin_json(E->ctx, (size_t)pi);
	const cJSON *vc = ifme_jget(manifest, "Video");
	const cJSON *args = ifme_jget(vc, "Args");
	const char *plugin_name = ifme_plugin_at(E->ctx, (size_t)pi)->name;

	/* ---- settings (job value, else source / plugin default) ---------------- */
	int mode_count = cJSON_GetArraySize(ifme_jget(vc, "Mode"));
	int mode = ifme_jint(item, "mode", 0);
	if (mode < 0 || mode >= mode_count) {
		fail(E, "video %zu: %s has modes 0..%d, job asks for %d", idx + 1, plugin_name, mode_count - 1, mode);
		return IFME_TASK_ERROR;
	}
	const cJSON *m = ifme_jitem(vc, "Mode", mode);
	const cJSON *mv = ifme_jget(m, "Value");

	double value = ifme_jdouble(item, "value", ifme_jdouble(mv, "Default", 0));
	int decimals = ifme_jint(mv, "DecimalPlace", 0);
	char value_str[64];
	snprintf(value_str, sizeof(value_str), "%.*f", decimals < 0 ? 0 : decimals, value);

	bool multipass = ifme_jbool(m, "MultiPass", false);
	int passes = multipass ? ifme_jint(item, "passes", 2) : 1;
	if (passes < 1)
		passes = 1;

	int width = ifme_jint(item, "width", src->width);
	int height = ifme_jint(item, "height", src->height);
	int bits = ifme_jint(item, "bit_depth", 8);
	int chroma = ifme_jint(item, "chroma", 420);
	if (bits < 8)
		bits = 8;                        /* C#: val_bpc */
	if (chroma < 420)
		chroma = 420;                    /* C#: val_csp */

	char fps[32];
	char *fps_job = ifme_jscalar(item, "fps");
	double fps_value = parse_fps(fps_job);
	if (fps_value > 0)
		snprintf(fps, sizeof(fps), "%s", fps_job);
	else
		fps_string(fps_value = source_fps(src), fps, sizeof(fps));
	free(fps_job);

	const char *preset = ifme_jstr(item, "preset");
	const char *tune = ifme_jstr(item, "tune");
	const char *user_args = ifme_jstr(item, "encoder_args");
	const char *ffmpeg_args = ifme_jstr(item, "ffmpeg_args");
	const char *filter = ifme_jstr(item, "filter");
	const cJSON *deint = ifme_jget(item, "deinterlace");

	/* Encoder binary for this bit depth (C# crashed when there was none). */
	const char *en = NULL;
	char depths[64] = "";
	const cJSON *e;
	cJSON_ArrayForEach(e, ifme_jget(vc, "Encoder")) {
		size_t l = strlen(depths);
		snprintf(depths + l, sizeof(depths) - l, "%s%d", l ? ", " : "", ifme_jint(e, "BitDepth", 0));
		if (!en && ifme_jint(e, "BitDepth", 0) == bits)
			en = ifme_jstr(e, "Binary");
	}
	if (!en) {
		fail(E, "video %zu: %s has no %d-bit encoder (available: %s)", idx + 1, plugin_name, bits, depths);
		return IFME_TASK_ERROR;
	}

	const char *ext = ifme_jstr(vc, "Extension");
	const char *container = cJSON_GetArrayItem(ifme_jget(manifest, "Format"), 0) &&
	                        cJSON_IsString(cJSON_GetArrayItem(ifme_jget(manifest, "Format"), 0))
	                        ? cJSON_GetArrayItem(ifme_jget(manifest, "Format"), 0)->valuestring : "mkv";
	bool raw_output = ifme_jbool(vc, "RawOutput", false);
	bool pipe = ifme_jbool(args, "Pipe", false);

	char *raw_file = ifme_strdupf("raw-v%04zu_%s.%s", idx, lang, ext && *ext ? ext : "bin");
	char *fmt_file = ifme_strdupf("video%04zu_%s.%s", idx, lang, container);
	const char *out_file = raw_output ? raw_file : fmt_file;

	/* Frame count: (trim or full) duration x frame rate, rounded up. */
	double seconds = E->trim_seconds > 0 ? E->trim_seconds : E->media->duration;
	long long frames = (long long)(seconds * fps_value);
	if ((double)frames < seconds * fps_value)
		frames++;

	/* ---- FFmpeg side: map, trim, pixel format, filters ----------------------- */
	char yuv[32];
	snprintf(yuv, sizeof(yuv), "yuv%dp%s", chroma, bits > 8 ? (bits == 10 ? "10le" : bits == 12 ? "12le" : "16le") : "");
	if (bits > 8 && bits != 10 && bits != 12 && bits != 16)
		snprintf(yuv, sizeof(yuv), "yuv%dp%dle", chroma, bits);

	char vf[1024] = "";
	size_t vl = 0;
	if ((width != src->width || height != src->height) && (width >= 128 || height >= 128))
		vl += (size_t)snprintf(vf + vl, sizeof(vf) - vl, "scale=%d:%d:flags=lanczos,", width, height);
	vl += (size_t)snprintf(vf + vl, sizeof(vf) - vl, "fps=%s,", fps);
	if (cJSON_IsObject(deint))
		vl += (size_t)snprintf(vf + vl, sizeof(vf) - vl, "yadif=%d:%d:0,",
		                       ifme_jint(deint, "mode", 0), ifme_jint(deint, "field", 1));
	if (!ifme_is_disabled(filter))
		vl += (size_t)snprintf(vf + vl, sizeof(vf) - vl, "%s,", filter);
	if (vl > 0 && vl < sizeof(vf))
		vf[vl - 1] = '\0';

	const cJSON *trim = ifme_jget(E->job, "trim");

	ifme_argv cmd_ff = { 0 };
	ifme_argv_push(&cmd_ff, "-map");
	ifme_argv_pushf(&cmd_ff, "0:%d", stream);
	if (E->trim_seconds > 0) {
		ifme_argv_push(&cmd_ff, "-ss");
		ifme_argv_push(&cmd_ff, ifme_jstr(trim, "start") ? ifme_jstr(trim, "start") : "0");
		ifme_argv_push(&cmd_ff, "-t");
		ifme_argv_push(&cmd_ff, ifme_jstr(trim, "duration"));
	}
	ifme_argv_push(&cmd_ff, "-pix_fmt");
	ifme_argv_push(&cmd_ff, yuv);
	ifme_argv_push(&cmd_ff, "-vf");
	ifme_argv_push(&cmd_ff, vf);

	/* ---- encoder side ----------------------------------------------------------- */
	bool is_ffmpeg = ifme_starts_with_ci(ifme_basename(en), "ffmpeg") &&
	                 (ifme_basename(en)[6] == '\0' || ifme_basename(en)[6] == '.');

	ifme_argv cmd_en = { 0 };   /* resolution, fps, bit depth, colour space */
	if (is_ffmpeg) {
		for (size_t i = 0; i < cmd_ff.n; i++)
			ifme_argv_push(&cmd_en, cmd_ff.v[i]);
	} else {
		const char *res_t = ifme_jstr(args, "Resolution");
		if (res_t && *res_t && width > 128 && height > 128) {
			char w[16], h[16];
			snprintf(w, sizeof(w), "%d", width);
			snprintf(h, sizeof(h), "%d", height);
			char *t1 = format_value(res_t, w);
			char *t2 = NULL;
			if (t1) {
				/* "{1}" is the height */
				size_t hl = strlen(h), n2 = strlen(t1) + 1 + hl * 4;
				t2 = malloc(n2);
				if (t2) {
					size_t o = 0;
					for (const char *p = t1; *p && o + hl + 1 < n2; p++) {
						if (p[0] == '{' && p[1] == '1' && p[2] == '}') {
							memcpy(t2 + o, h, hl);
							o += hl;
							p += 2;
						} else {
							t2[o++] = *p;
						}
					}
					t2[o] = '\0';
				}
			}
			ifme_argv_split(&cmd_en, t2 ? t2 : t1);
			free(t1);
			free(t2);
		}

		const char *fr = ifme_jstr(args, "FrameRate");
		if (fr && *fr && fps_value > 0) {
			ifme_argv_split(&cmd_en, fr);
			ifme_argv_push(&cmd_en, fps);
		}

		const char *bi = ifme_jstr(args, "BitDepthIn"), *bo = ifme_jstr(args, "BitDepthOut");
		if (bi && *bi) {
			ifme_argv_split(&cmd_en, bi);
			ifme_argv_pushf(&cmd_en, "%d", bits);
		}
		if (bo && *bo) {
			ifme_argv_split(&cmd_en, bo);
			ifme_argv_pushf(&cmd_en, "%d", bits);
		}

		cJSON_ArrayForEach(e, ifme_jget(vc, "Chroma")) {
			const char *c = ifme_jstr(e, "Command");
			if (ifme_jint(e, "Value", 0) == chroma && c && *c) {
				/* C#: piped encoders that take a "yuv..." format get the bit
				 * depth suffix appended (e.g. yuv420p -> yuv420p10le). */
				if (pipe && strstr(c, "yuv") && bits > 8) {
					char *c2 = ifme_strdupf("%s%dle", c, bits);
					ifme_argv_split(&cmd_en, c2);
					free(c2);
				} else {
					ifme_argv_split(&cmd_en, c);
				}
			}
		}
	}

	ifme_argv common = { 0 };   /* preset, tune, rate-control mode */
	push_arg_value(&common, ifme_jstr(args, "Preset"), preset);
	push_arg_value(&common, ifme_jstr(args, "Tune"), tune);

	const char *mode_args = ifme_jstr(m, "Args");
	if (mode_args && *mode_args) {
		if (has_placeholder(mode_args)) {
			char *t = format_value(mode_args, value_str);
			ifme_argv_split(&common, t);
			free(t);
		} else {
			const char *pre = ifme_jstr(m, "Prefix"), *post = ifme_jstr(m, "Postfix");
			ifme_argv_split(&common, mode_args);
			ifme_argv_pushf(&common, "%s%s%s", pre ? pre : "", value_str, post ? post : "");
		}
	}

	bool image_seq = strchr(E->input, '%') != NULL;
	char infps[32];
	fps_string(source_fps(src), infps, sizeof(infps));

	/* ---- passes ---------------------------------------------------------------- */
	ifme_task_result r = IFME_TASK_DONE;

	for (int p = 1; p <= passes && r == IFME_TASK_DONE; p++) {
		const char *pass_arg = NULL;
		if (multipass) {
			pass_arg = ifme_jstr(args, "PassNth");
			if (p == 1)
				pass_arg = ifme_jstr(args, "PassFirst");
			if (p == passes)
				pass_arg = ifme_jstr(args, "PassLast");
		}

		ifme_argv st[2] = { { 0 }, { 0 } };
		ifme_argv *a;
		size_t n;

		if (pipe) {
			ifme_argv *ff = &st[0];
			ifme_argv_push(ff, E->ffmpeg);
			ifme_argv_split(ff, "-hide_banner -v error");
			if (image_seq) {
				ifme_argv_push(ff, "-framerate");
				ifme_argv_push(ff, infps);
			}
			ifme_argv_push(ff, "-i");
			ifme_argv_push(ff, E->input);
			for (size_t i = 0; i < cmd_ff.n; i++)
				ifme_argv_push(ff, cmd_ff.v[i]);
			ifme_argv_split(ff, "-strict -1 -f");
			ifme_argv_push(ff, (ifme_jstr(args, "Y4M") && *ifme_jstr(args, "Y4M")) ? "yuv4mpegpipe" : "rawvideo");
			ifme_argv_split(ff, ffmpeg_args);
			ifme_argv_push(ff, "-");

			a = &st[1];
			ifme_argv_push(a, en);
			ifme_argv_split(a, ifme_jstr(args, "Y4M"));
			ifme_argv_split(a, ifme_jstr(args, "Input"));
			/* C# turns the frame count off for multi-pass (keeps passes consistent). */
			if (!multipass && !ifme_is_disabled(ifme_jstr(args, "FrameCount"))) {
				ifme_argv_split(a, ifme_jstr(args, "FrameCount"));
				ifme_argv_pushf(a, "%lld", frames);
			}
			for (size_t i = 0; i < cmd_en.n; i++)
				ifme_argv_push(a, cmd_en.v[i]);
			for (size_t i = 0; i < common.n; i++)
				ifme_argv_push(a, common.v[i]);
			ifme_argv_split(a, ifme_jstr(args, "Command"));
			ifme_argv_split(a, user_args);
			n = 2;
		} else {
			a = &st[0];
			ifme_argv_push(a, en);
			if (image_seq && is_ffmpeg) {
				ifme_argv_push(a, "-framerate");
				ifme_argv_push(a, infps);
			}
			ifme_argv_split(a, ifme_jstr(args, "Input"));
			ifme_argv_push(a, E->input);
			for (size_t i = 0; i < cmd_en.n; i++)
				ifme_argv_push(a, cmd_en.v[i]);
			for (size_t i = 0; i < common.n; i++)
				ifme_argv_push(a, common.v[i]);
			ifme_argv_split(a, ifme_jstr(args, "UnPipe"));
			ifme_argv_split(a, user_args);
			ifme_argv_split(a, ifme_jstr(args, "Command"));
			n = 1;
		}

		ifme_argv_split(a, pass_arg);
		ifme_argv_split(a, ifme_jstr(args, "Output"));
		ifme_argv_push(a, out_file);

		if (passes > 1)
			stage(E, "Video %zu/%zu: %s, pass %d of %d", idx + 1, count, plugin_name, p, passes);
		else
			stage(E, "Video %zu/%zu: %s", idx + 1, count, plugin_name);

		r = run(E, st, n, frames);
		ifme_argv_free(&st[0]);
		ifme_argv_free(&st[1]);
	}

	ifme_argv_free(&cmd_ff);
	ifme_argv_free(&cmd_en);
	ifme_argv_free(&common);

	char *out_path = r == IFME_TASK_DONE ? ifme_path_join(E->temp, out_file) : NULL;
	if (r == IFME_TASK_DONE && ifme_file_size(out_path) <= 0) {
		fail(E, "video %zu: %s finished but wrote no %s", idx + 1, plugin_name, out_file);
		r = IFME_TASK_FAILED;
	}
	free(out_path);

	/* ---- raw stream -> container ---------------------------------------------- */
	if (r == IFME_TASK_DONE && raw_output) {
		stage(E, "Video %zu/%zu: restructuring raw stream", idx + 1, count);

		char *mp4box = ifme_find_tool(E->ctx, "MP4Box", "mp4box");
		bool mp4 = !ifme_strcasecmp(container, "mp4") || !ifme_strcasecmp(container, "m4v") ||
		           !ifme_strcasecmp(container, "mov");
		ifme_argv a = { 0 };

		if (mp4box && mp4) {
			/* As C#, plus the frame rate: raw streams carry no timestamps and
			 * not every encoder writes it into the bitstream. */
			ifme_argv_push(&a, mp4box);
			ifme_argv_push(&a, "-add");
			ifme_argv_pushf(&a, "%s#video:name=:fps=%.6f", raw_file, fps_value);
			ifme_argv_push(&a, "-itags");
			ifme_argv_push(&a, "tool=IFME MP4");
			ifme_argv_push(&a, "-new");
			ifme_argv_push(&a, fmt_file);
		} else {
			ifme_argv_push(&a, E->ffmpeg);
			ifme_argv_split(&a, "-hide_banner -v error -framerate");
			ifme_argv_push(&a, fps);
			ifme_argv_push(&a, "-i");
			ifme_argv_push(&a, raw_file);
			ifme_argv_split(&a, "-c copy -y");
			ifme_argv_push(&a, fmt_file);
		}
		free(mp4box);

		r = run1(E, &a, 0);
		ifme_argv_free(&a);

		char *raw_path = ifme_path_join(E->temp, raw_file);
		if (raw_path)
			ifme_delete_file(raw_path);
		free(raw_path);
	}

	if (r == IFME_TASK_DONE)
		*result = ifme_strdup(fmt_file);

	free(raw_file);
	free(fmt_file);
	return r;
}

/* ---- job ------------------------------------------------------------------------ */

static unsigned long job_counter;

ifme_task_result ifme_encode(ifme_task *task, ifme_ctx *ctx, const char *job_json,
                             const ifme_task_callbacks *cb, char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';

	enc E;
	memset(&E, 0, sizeof(E));
	E.ctx = ctx;
	E.control = ifme_task_control(task);
	E.cb = cb;
	E.err = err;
	E.err_len = err_len;

	ifme_task_result r = IFME_TASK_ERROR;
	char *strict = job_json ? ifme_json_relax(job_json) : NULL;
	cJSON *job = strict ? cJSON_Parse(strict) : NULL;
	free(strict);
	E.job = job;

	char *output = NULL;
	char **tracks = NULL;
	size_t track_count = 0;
	bool keep_temp = false;

	if (!ctx || !task) {
		fail(&E, "no context or task");
		goto out;
	}
	if (!job) {
		fail(&E, "job is not valid JSON");
		goto out;
	}
	if (E.control && atomic_load(&E.control->want_stop)) {
		fail(&E, "stopped");
		r = IFME_TASK_STOPPED;
		goto out;
	}

	const char *input = ifme_jstr(job, "input");
	const char *out_rel = ifme_jstr(job, "output");
	const cJSON *videos = ifme_jget(job, "video");
	size_t video_count = (size_t)cJSON_GetArraySize(videos);
	keep_temp = ifme_jbool(job, "keep_temp", false);

	if (!input || !*input) {
		fail(&E, "job has no \"input\"");
		goto out;
	}
	if (!out_rel || !*out_rel) {
		fail(&E, "job has no \"output\"");
		goto out;
	}
	if (!cJSON_IsArray(videos) || video_count == 0) {
		fail(&E, "job has no \"video\" entries (audio-only jobs come with milestone 5)");
		goto out;
	}

	E.input = ifme_absolute_path(input);
	output = ifme_absolute_path(out_rel);

	const cJSON *trim = ifme_jget(job, "trim");
	if (cJSON_IsObject(trim)) {
		E.trim_seconds = parse_time(ifme_jstr(trim, "duration"));
		if (E.trim_seconds <= 0 || (ifme_jstr(trim, "start") && parse_time(ifme_jstr(trim, "start")) < 0)) {
			fail(&E, "trim needs \"start\" and \"duration\" as HH:MM:SS(.mmm) or seconds");
			goto out;
		}
	}

	E.ffmpeg = ifme_find_ff_tool(ctx, "ffmpeg");
	if (!E.ffmpeg) {
		fail(&E, "ffmpeg not found (PATH or Plugins/ffmpeg64)");
		goto out;
	}

	char why[512];
	E.media = ifme_probe(ctx, E.input, NULL, why, sizeof(why));
	if (!E.media) {
		fail(&E, "%s", why);
		goto out;
	}

	/* Temp folder: job's choice, else <tmp>/ifme-<pid>-<n>. */
	const char *temp = ifme_jstr(job, "temp_dir");
	if (temp && *temp) {
		E.temp = ifme_absolute_path(temp);
	} else {
		char *tmp = ifme_temp_dir();
		char *name = ifme_strdupf("ifme-%lu-%lu", ifme_pid(), ++job_counter);
		E.temp = (tmp && name) ? ifme_path_join(tmp, name) : NULL;
		free(tmp);
		free(name);
	}
	if (!E.temp || !ifme_mkdir(E.temp)) {
		fail(&E, "cannot create temporary folder %s", E.temp ? E.temp : "");
		goto out;
	}

	tracks = calloc(video_count, sizeof(char *));
	if (!tracks)
		goto out;

	for (size_t i = 0; i < video_count; i++) {
		r = encode_video(&E, i, video_count, cJSON_GetArrayItem(videos, (int)i), &tracks[i]);
		if (r != IFME_TASK_DONE)
			goto out;
		track_count++;
	}

	/* Write every video track into the output container. */
	stage(&E, "Writing %s", ifme_basename(output));
	ifme_argv a = { 0 };
	ifme_argv_push(&a, E.ffmpeg);
	ifme_argv_split(&a, "-hide_banner -v error");
	for (size_t i = 0; i < track_count; i++) {
		ifme_argv_push(&a, "-i");
		ifme_argv_push(&a, tracks[i]);
	}
	for (size_t i = 0; i < track_count; i++) {
		ifme_argv_push(&a, "-map");
		ifme_argv_pushf(&a, "%zu:v", i);
	}
	ifme_argv_split(&a, "-c copy -metadata:g encoding_tool=IFME -y");
	ifme_argv_push(&a, output);
	r = run1(&E, &a, 0);
	ifme_argv_free(&a);

	if (r == IFME_TASK_DONE && ifme_file_size(output) <= 0) {
		fail(&E, "no output written to %s", output);
		r = IFME_TASK_FAILED;
	}

out:
	if (E.temp && !keep_temp) {
		ifme_clear_dir(E.temp);
		ifme_rmdir(E.temp);
	}
	for (size_t i = 0; tracks && i < track_count; i++)
		free(tracks[i]);
	free(tracks);
	free(output);
	free(E.temp);
	free(E.ffmpeg);
	free(E.input);
	ifme_media_free(E.media);
	cJSON_Delete(job);
	return r;
}

/* ---- job defaults --------------------------------------------------------------- */

static int supported_or(const cJSON *list, const char *key, int want, int fallback)
{
	const cJSON *e;
	int first = 0;
	cJSON_ArrayForEach(e, list) {
		int v = ifme_jint(e, key, 0);
		if (v == want)
			return want;
		if (!first)
			first = v;
	}
	for (const cJSON *x = list ? list->child : NULL; x; x = x->next)
		if (ifme_jint(x, key, 0) == fallback)
			return fallback;
	return first ? first : fallback;
}

static void add_strings(cJSON *obj, const char *name, const cJSON *arr)
{
	cJSON *out = cJSON_AddArrayToObject(obj, name);
	const cJSON *e;
	cJSON_ArrayForEach(e, arr) {
		if (cJSON_IsString(e))
			cJSON_AddItemToArray(out, cJSON_CreateString(e->valuestring));
	}
}

char *ifme_job_new(ifme_ctx *ctx, const char *input, const char *video_plugin,
                   char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';
	if (!ctx || !input)
		return NULL;

	int pi = ifme_plugin_find(ctx, IFME_PLUGIN_VIDEO, video_plugin ? video_plugin : "", err, err_len);
	bool copy = video_plugin && ifme_strcasecmp(video_plugin, VIDEO_COPY_GUID) == 0;
	if (pi < 0 && !copy)
		return NULL;

	char *abs_input = ifme_absolute_path(input);
	ifme_media *media = ifme_probe(ctx, abs_input, NULL, err, err_len);
	if (!media) {
		free(abs_input);
		return NULL;
	}

	int stream = first_video_stream(media);
	const ifme_stream_video *src = source_video(media, stream);
	if (!src) {
		if (err && err_len)
			snprintf(err, err_len, "%s has no video stream", input);
		ifme_media_free(media);
		free(abs_input);
		return NULL;
	}

	const cJSON *manifest = copy ? NULL : ifme_plugin_json(ctx, (size_t)pi);
	const cJSON *vc = ifme_jget(manifest, "Video");
	const cJSON *mode0 = ifme_jitem(vc, "Mode", 0);
	const cJSON *fmt = cJSON_GetArrayItem(ifme_jget(manifest, "Format"), 0);
	const char *container = cJSON_IsString(fmt) ? fmt->valuestring : "mkv";

	/* Output next to the input: "<name>.ifme.<container>". */
	char *output = NULL;
	{
		char *base = ifme_strdup(abs_input);
		char *dot = base ? strrchr(base, '.') : NULL;
		const char *slash = base ? ifme_basename(base) : NULL;
		if (dot && dot > slash)
			*dot = '\0';
		output = base ? ifme_strdupf("%s.ifme.%s", base, container) : NULL;
		free(base);
	}

	cJSON *job = cJSON_CreateObject();
	cJSON_AddStringToObject(job, "input", abs_input);
	cJSON_AddStringToObject(job, "output", output ? output : "");
	cJSON_AddStringToObject(job, "temp_dir", "");
	cJSON_AddBoolToObject(job, "keep_temp", false);

	cJSON *videos = cJSON_AddArrayToObject(job, "video");
	cJSON *v = cJSON_CreateObject();
	cJSON_AddItemToArray(videos, v);

	char fps[32];
	fps_string(source_fps(src), fps, sizeof(fps));

	cJSON_AddNumberToObject(v, "stream", stream);
	if (copy) {
		cJSON_AddStringToObject(v, "plugin", VIDEO_COPY_GUID);
	} else {
		const char *ps = ifme_jstr(vc, "PresetDefault"), *tu = ifme_jstr(vc, "TuneDefault");
		const cJSON *mv = ifme_jget(mode0, "Value");

		cJSON_AddStringToObject(v, "plugin", ifme_plugin_at(ctx, (size_t)pi)->guid);
		cJSON_AddStringToObject(v, "preset", ps ? ps : "");
		cJSON_AddStringToObject(v, "tune", tu ? tu : "");
		cJSON_AddNumberToObject(v, "mode", 0);
		cJSON_AddNumberToObject(v, "value", ifme_jdouble(mv, "Default", 0));
		cJSON_AddNumberToObject(v, "passes", ifme_jbool(mode0, "MultiPass", false) ? 2 : 1);
		cJSON_AddStringToObject(v, "encoder_args", "");
		cJSON_AddNumberToObject(v, "width", src->width);
		cJSON_AddNumberToObject(v, "height", src->height);
		cJSON_AddStringToObject(v, "fps", fps);
		cJSON_AddNumberToObject(v, "bit_depth",
			supported_or(ifme_jget(vc, "Encoder"), "BitDepth", src->bit_depth, 8));
		cJSON_AddNumberToObject(v, "chroma",
			supported_or(ifme_jget(vc, "Chroma"), "Value", src->chroma, 420));
		cJSON_AddStringToObject(v, "ffmpeg_args", "");
		cJSON_AddStringToObject(v, "filter", "");

		/* Read-only hints for people and UIs editing the job. */
		cJSON *help = cJSON_AddObjectToObject(v, "_help");
		cJSON_AddStringToObject(help, "plugin_name", ifme_plugin_at(ctx, (size_t)pi)->name);
		add_strings(help, "presets", ifme_jget(vc, "Preset"));
		add_strings(help, "tunes", ifme_jget(vc, "Tune"));
		cJSON *modes = cJSON_AddArrayToObject(help, "modes");
		const cJSON *m;
		int k = 0;
		cJSON_ArrayForEach(m, ifme_jget(vc, "Mode")) {
			const cJSON *mv2 = ifme_jget(m, "Value");
			int dp = ifme_jint(mv2, "DecimalPlace", 0);
			if (dp < 0 || dp > 6)
				dp = 0;
			char *line = ifme_strdupf("%d: %s (%.*f..%.*f, default %.*f)%s", k++,
				ifme_jstr(m, "Name") ? ifme_jstr(m, "Name") : "?",
				dp, ifme_jdouble(mv2, "Min", 0), dp, ifme_jdouble(mv2, "Max", 0),
				dp, ifme_jdouble(mv2, "Default", 0),
				ifme_jbool(m, "MultiPass", false) ? ", multi-pass" : "");
			cJSON_AddItemToArray(modes, cJSON_CreateString(line ? line : ""));
			free(line);
		}
		cJSON *depths = cJSON_AddArrayToObject(help, "bit_depths");
		cJSON_ArrayForEach(m, ifme_jget(vc, "Encoder"))
			cJSON_AddItemToArray(depths, cJSON_CreateNumber(ifme_jint(m, "BitDepth", 0)));
		cJSON *chromas = cJSON_AddArrayToObject(help, "chromas");
		cJSON_ArrayForEach(m, ifme_jget(vc, "Chroma"))
			cJSON_AddItemToArray(chromas, cJSON_CreateNumber(ifme_jint(m, "Value", 0)));
	}

	char *text = cJSON_Print(job);
	cJSON_Delete(job);
	free(output);
	ifme_media_free(media);
	free(abs_input);
	return text;
}
