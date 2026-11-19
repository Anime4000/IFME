/*
 * encode.c - encoding jobs (port of MediaEncoding.Video, .Audio, .Muxing)
 *
 * Audio, as C#:
 *   pipe:     ffmpeg -i IN [-ss -t] -map 0:N -acodec pcm_s16le {hz} {ch} [-af]
 *             -f wav - | ENCODER {Input} {Command} {quality} {user} {Output} OUT
 *   direct:   ENCODER {Input} IN [-ss -t] -map 0:N {Command} {Codec} {quality}
 *             {hz} {ch} [-af] {user} {Output} OUT
 *   copy:     ffmpeg -c:a copy into the output's container; with "copy": true,
 *             a refused codec falls back to encoding (C#'s FastMuxAudio).
 *   Then all tracks -> output with ffmpeg -c copy, language/title per stream.
 * Audio differences: the channel choice is respected when down/up-mixing
 * (C# looked at the source only), copying works with a trim, and progress
 * comes from FFmpeg's "time=" (C# showed none for audio).
 *
 * Video:
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
 * Subtitles, attachments, cover art, tags and chapters: extras.c.
 * Not ported yet: video fast remux, auto-crop.
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "encode_int.h"

/* ---- small helpers --------------------------------------------------------- */

void enc_fail(enc *E, const char *fmt, ...)
{
	if (!E->err || !E->err_len)
		return;
	va_list ap;
	va_start(ap, fmt);
	vsnprintf(E->err, E->err_len, fmt, ap);
	va_end(ap);
}

void enc_stage(enc *E, const char *fmt, ...)
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

void enc_log(enc *E, const char *line)
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

/* Run stages in the temp folder with progress; fills E->err on failure.
 * total_frames / total_sec (0: unknown) turn encoder output into a percent. */
static ifme_task_result run(enc *E, ifme_argv *stages, size_t n, long long total_frames, double total_sec)
{
	for (size_t i = 0; i < n; i++) {
		char *cmd = ifme_argv_display(&stages[i]);
		if (cmd) {
			char *line = ifme_strdupf("%s %s", i ? "|" : "$", cmd);
			if (line)
				enc_log(E, line);
			free(line);
		}
		free(cmd);
	}

	ifme_progress_init(&E->parser, total_frames);
	E->parser.total_seconds = total_sec > 0 ? total_sec : 0;
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
		enc_fail(E, "stopped");
		result = IFME_TASK_STOPPED;
	} else if (rc != IFME_RUN_OK) {
		enc_fail(E, "%s", r.message);
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

ifme_task_result enc_run1(enc *E, ifme_argv *a, double total_sec)
{
	return run(E, a, 1, 0, total_sec);
}

/* Seconds this job covers: the trim, else the whole input. */
double enc_job_seconds(const enc *E)
{
	return E->trim_seconds > 0 ? E->trim_seconds : E->media->duration;
}

/* "-ss START -t DURATION" after the input, as C# did (accurate seeking, and
 * audio and video cut at the same points). */
void enc_push_trim(enc *E, ifme_argv *a)
{
	if (E->trim_seconds <= 0)
		return;
	const cJSON *trim = ifme_jget(E->job, "trim");
	ifme_argv_push(a, "-ss");
	ifme_argv_push(a, ifme_jstr(trim, "start") ? ifme_jstr(trim, "start") : "0");
	ifme_argv_push(a, "-t");
	ifme_argv_push(a, ifme_jstr(trim, "duration"));
}

static bool enc_is_copy(const char *plugin, const char *builtin_guid)
{
	return plugin && (ifme_strcasecmp(plugin, "copy") == 0 || ifme_strcasecmp(plugin, builtin_guid) == 0);
}

/* Extension of the output file ("mkv" when it has none). */
const char *enc_output_ext(const char *path)
{
	const char *base = ifme_basename(path);
	const char *dot = strrchr(base, '.');
	return (dot && dot[1]) ? dot + 1 : "mkv";
}

bool enc_ext_is(const char *path, const char *const *exts)
{
	const char *ext = enc_output_ext(path);
	for (; *exts; exts++)
		if (ifme_strcasecmp(ext, *exts) == 0)
			return true;
	return false;
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
		enc_fail(E, "video %zu: input has no video stream %d", idx + 1, stream);
		return IFME_TASK_ERROR;
	}

	const char *lang = (src->language && *src->language) ? src->language : "und";
	const char *plugin_query = ifme_jstr(item, "plugin");

	/* Built-in "copy" entry: keep the stream as it is. */
	if (enc_is_copy(plugin_query, VIDEO_COPY_GUID)) {
		char *out = ifme_strdupf("video%04zu_%s.mkv", idx, lang);
		ifme_argv a = { 0 };
		ifme_argv_push(&a, E->ffmpeg);
		ifme_argv_split(&a, "-hide_banner -v error -stats -i");
		ifme_argv_push(&a, E->input);
		enc_push_trim(E, &a);
		ifme_argv_push(&a, "-map");
		ifme_argv_pushf(&a, "0:%d", stream);
		ifme_argv_split(&a, "-c:v copy -y");
		ifme_argv_push(&a, out);

		enc_stage(E, "Video %zu/%zu: copying stream", idx + 1, count);
		ifme_task_result r = enc_run1(E, &a, enc_job_seconds(E));
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
		enc_fail(E, "video %zu: %s", idx + 1, why[0] ? why : "no \"plugin\" given");
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
		enc_fail(E, "video %zu: %s has modes 0..%d, job asks for %d", idx + 1, plugin_name, mode_count - 1, mode);
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
		enc_fail(E, "video %zu: %s has no %d-bit encoder (available: %s)", idx + 1, plugin_name, bits, depths);
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
	double seconds = enc_job_seconds(E);
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
	if (E->burn)
		vl += (size_t)snprintf(vf + vl, sizeof(vf) - vl, "subtitles=f=%s%s,", E->burn,
		                       E->fonts ? ":fontsdir=attachment" : "");
	if (vl > 0 && vl < sizeof(vf))
		vf[vl - 1] = '\0';

	ifme_argv cmd_ff = { 0 };
	ifme_argv_push(&cmd_ff, "-map");
	ifme_argv_pushf(&cmd_ff, "0:%d", stream);
	enc_push_trim(E, &cmd_ff);
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
			enc_stage(E, "Video %zu/%zu: %s, pass %d of %d", idx + 1, count, plugin_name, p, passes);
		else
			enc_stage(E, "Video %zu/%zu: %s", idx + 1, count, plugin_name);

		r = run(E, st, n, frames, seconds);
		ifme_argv_free(&st[0]);
		ifme_argv_free(&st[1]);
	}

	ifme_argv_free(&cmd_ff);
	ifme_argv_free(&cmd_en);
	ifme_argv_free(&common);

	char *out_path = r == IFME_TASK_DONE ? ifme_path_join(E->temp, out_file) : NULL;
	if (r == IFME_TASK_DONE && ifme_file_size(out_path) <= 0) {
		enc_fail(E, "video %zu: %s finished but wrote no %s", idx + 1, plugin_name, out_file);
		r = IFME_TASK_FAILED;
	}
	free(out_path);

	/* ---- raw stream -> container ---------------------------------------------- */
	if (r == IFME_TASK_DONE && raw_output) {
		enc_stage(E, "Video %zu/%zu: restructuring raw stream", idx + 1, count);

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

		r = enc_run1(E, &a, 0);
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

/* ---- audio (port of MediaEncoding.Audio) --------------------------------------- */

static const ifme_stream_audio *source_audio(const ifme_media *m, int stream)
{
	for (size_t i = 0; i < m->audio_count; i++)
		if (m->audio[i].id == stream)
			return &m->audio[i];
	return NULL;
}

/* C# keeps "Channels" in a SortedList and passes -ac unless the choice is
 * its first (lowest) key, normally 0 = "Auto". */
static int first_channel_key(const cJSON *channels)
{
	int lowest = 0;
	bool any = false;
	for (const cJSON *c = channels ? channels->child : NULL; c; c = c->next) {
		int k = c->string ? atoi(c->string) : 0;
		if (!any || k < lowest)
			lowest = k;
		any = true;
	}
	return lowest;
}

/* "-ar" / "-ac" unless the manifest says otherwise; a key set to null means
 * the encoder takes no such argument. */
static const char *audio_arg_name(const cJSON *ac, const char *key, const char *fallback)
{
	const cJSON *v = ifme_jget(ac, key);
	if (!v)
		return fallback;
	return cJSON_IsString(v) && v->valuestring[0] ? v->valuestring : NULL;
}

/* Media seconds a track covers: the trim, else its own file's length. */
double enc_track_seconds(const enc *E, const ifme_media *m)
{
	return E->trim_seconds > 0 ? E->trim_seconds : m->duration;
}

/* Copy the stream into the output's container (C#'s "fast remux"). */
static ifme_task_result copy_audio(enc *E, size_t idx, size_t count, const char *in,
                                   const ifme_media *med, int stream, const char *lang, char **result)
{
	char *out = ifme_strdupf("audio%04zu_%s.%s", idx, lang, enc_output_ext(E->output));
	ifme_argv a = { 0 };
	ifme_argv_push(&a, E->ffmpeg);
	ifme_argv_split(&a, "-hide_banner -v error -stats -i");
	ifme_argv_push(&a, in);
	enc_push_trim(E, &a);
	ifme_argv_push(&a, "-map");
	ifme_argv_pushf(&a, "0:%d", stream);
	ifme_argv_split(&a, "-c:a copy -y");
	ifme_argv_push(&a, out);

	enc_stage(E, "Audio %zu/%zu: copying stream", idx + 1, count);
	ifme_task_result r = enc_run1(E, &a, enc_track_seconds(E, med));
	ifme_argv_free(&a);

	/* As C#: a file of 1 KiB or less means the container refused the codec. */
	char *path = ifme_path_join(E->temp, out);
	if (r == IFME_TASK_DONE && ifme_file_size(path) <= 1024) {
		enc_fail(E, "audio %zu: the stream cannot be copied into .%s", idx + 1, enc_output_ext(E->output));
		r = IFME_TASK_FAILED;
	}
	if (r != IFME_TASK_DONE && path)
		ifme_delete_file(path);
	free(path);

	if (r == IFME_TASK_DONE)
		*result = out;
	else
		free(out);
	return r;
}

/* Encode audio item `idx` from file `in` (probed as `med`); on success
 * *result is the track file in E->temp and *lang_out its language. */
static ifme_task_result encode_audio_from(enc *E, size_t idx, size_t count, const cJSON *item,
                                          const char *in, const ifme_media *med,
                                          char **result, char **lang_out)
{
	*result = NULL;

	int stream = ifme_jint(item, "stream", med->audio_count ? med->audio[0].id : -1);
	const ifme_stream_audio *src = source_audio(med, stream);
	if (!src) {
		enc_fail(E, "audio %zu: %s has no audio stream %d", idx + 1, ifme_basename(in), stream);
		return IFME_TASK_ERROR;
	}

	const char *lang = ifme_jstr(item, "language");
	if (!lang || !*lang)
		lang = (src->language && *src->language) ? src->language : "und";
	*lang_out = ifme_strdup(lang);
	const char *plugin_query = ifme_jstr(item, "plugin");

	if (enc_is_copy(plugin_query, AUDIO_COPY_GUID))
		return copy_audio(E, idx, count, in, med, stream, lang, result);

	/* "copy": true tries the stream as it is first and encodes only if the
	 * output container refuses it. Unlike C#, this also works with a trim. */
	if (ifme_jbool(item, "copy", false)) {
		ifme_task_result r = copy_audio(E, idx, count, in, med, stream, lang, result);
		if (r == IFME_TASK_DONE || r == IFME_TASK_STOPPED)
			return r;
		char *note = ifme_strdupf("%s; encoding it instead", E->err ? E->err : "copy failed");
		if (note)
			enc_log(E, note);
		free(note);
		if (E->err && E->err_len)
			E->err[0] = '\0';
	}

	char why[512];
	int pi = ifme_plugin_find(E->ctx, IFME_PLUGIN_AUDIO, plugin_query ? plugin_query : "", why, sizeof(why));
	if (pi < 0) {
		enc_fail(E, "audio %zu: %s", idx + 1, why[0] ? why : "no \"plugin\" given");
		return IFME_TASK_ERROR;
	}

	const cJSON *manifest = ifme_plugin_json(E->ctx, (size_t)pi);
	const cJSON *ac = ifme_jget(manifest, "Audio");
	const cJSON *args = ifme_jget(ac, "Args");
	const char *plugin_name = ifme_plugin_at(E->ctx, (size_t)pi)->name;
	const char *en = ifme_plugin_at(E->ctx, (size_t)pi)->binary;

	int mode_count = cJSON_GetArraySize(ifme_jget(ac, "Mode"));
	int mode = ifme_jint(item, "mode", 0);
	if (mode < 0 || mode >= mode_count) {
		enc_fail(E, "audio %zu: %s has modes 0..%d, job asks for %d", idx + 1, plugin_name, mode_count - 1, mode);
		return IFME_TASK_ERROR;
	}
	const cJSON *m = ifme_jitem(ac, "Mode", mode);
	const char *mode_name = ifme_jstr(m, "Name") ? ifme_jstr(m, "Name") : "?";
	enc_stage(E, "Audio %zu/%zu: %s, %s", idx + 1, count, plugin_name, mode_name);

	/* ---- quality, sample rate, channels ---------------------------------------- */
	char *quality = ifme_jscalar(item, "quality");
	if (!quality || !*quality) {
		free(quality);
		quality = ifme_jscalar(m, "Default");
	}
	ifme_argv qu = { 0 };
	ifme_audio_quality_args(&qu, m, quality);
	free(quality);

	int hz = ifme_jint(item, "sample_rate", ifme_jint(ac, "SampleRateDefault", 0));
	int ch = ifme_jint(item, "channels", ifme_jint(ac, "ChannelDefault", 0));
	const char *hz_args = audio_arg_name(ac, "SampleRateArgs", "-ar");
	const char *ch_args = audio_arg_name(ac, "ChannelArgs", "-ac");

	/* Channels the encoder will actually get. C# down-mixed every source of
	 * 2+ channels and up-mixed mono based on the source alone, so a mono
	 * choice on a stereo source still became stereo; here the choice counts. */
	bool pass_ch = ch != first_channel_key(ifme_jget(ac, "Channels"));
	int effective = pass_ch && ch > 0 ? ch : src->channels;

	if (!ifme_jbool(m, "MultiChannelSupport", true) && effective > 2) {
		char *w = ifme_strdupf("%s, %s: no multi-channel support, down-mixing to stereo", plugin_name, mode_name);
		enc_log(E, w);
		free(w);
		ch = 2;
		pass_ch = true;
	}
	if (!ifme_jbool(m, "MonoSupport", true) && effective == 1) {
		char *w = ifme_strdupf("%s, %s: no mono support, up-mixing to stereo", plugin_name, mode_name);
		enc_log(E, w);
		free(w);
		ch = 2;
		pass_ch = true;
	}

	ifme_argv hzch = { 0 };
	if (hz != 0 && hz_args) {
		ifme_argv_split(&hzch, hz_args);
		ifme_argv_pushf(&hzch, "%d", hz);
	}
	if (ifme_jbool(m, "SingleChannelOnly", false)) {
		ifme_argv_split(&hzch, "-ac 1");   /* literal, as in MediaEncoding.cs */
	} else if (pass_ch && ch_args) {
		ifme_argv_split(&hzch, ch_args);
		ifme_argv_pushf(&hzch, "%d", ch);
	}

	const char *filter = ifme_jstr(item, "filter");
	const char *user_args = ifme_jstr(item, "encoder_args");
	const char *ffmpeg_args = ifme_jstr(item, "ffmpeg_args");
	const char *ext = ifme_jstr(ac, "Extension");
	char *out_file = ifme_strdupf("audio%04zu_%s.%s", idx, lang, ext && *ext ? ext : "mka");

	/* ---- command --------------------------------------------------------------- */
	ifme_argv st[2] = { { 0 }, { 0 } };
	ifme_argv *a;
	size_t n;

	if (ifme_jbool(args, "Pipe", false)) {
		/* ffmpeg decodes to 16-bit WAV on stdout, the encoder reads stdin. */
		ifme_argv *ff = &st[0];
		ifme_argv_push(ff, E->ffmpeg);
		ifme_argv_split(ff, "-hide_banner -v error -stats -i");
		ifme_argv_push(ff, in);
		enc_push_trim(E, ff);
		ifme_argv_push(ff, "-map");
		ifme_argv_pushf(ff, "0:%d", stream);
		ifme_argv_split(ff, "-acodec pcm_s16le");
		for (size_t i = 0; i < hzch.n; i++)
			ifme_argv_push(ff, hzch.v[i]);
		if (!ifme_is_disabled(filter)) {
			ifme_argv_push(ff, "-af");
			ifme_argv_push(ff, filter);
		}
		ifme_argv_split(ff, "-f wav");
		ifme_argv_split(ff, ffmpeg_args);
		ifme_argv_push(ff, "-");

		a = &st[1];
		ifme_argv_push(a, en);
		ifme_argv_split(a, ifme_jstr(args, "Input"));
		ifme_argv_split(a, ifme_jstr(args, "Command"));
		for (size_t i = 0; i < qu.n; i++)
			ifme_argv_push(a, qu.v[i]);
		n = 2;
	} else {
		a = &st[0];
		ifme_argv_push(a, en);
		ifme_argv_split(a, ifme_jstr(args, "Input"));
		ifme_argv_push(a, in);
		enc_push_trim(E, a);
		ifme_argv_push(a, "-map");
		ifme_argv_pushf(a, "0:%d", stream);
		ifme_argv_split(a, ifme_jstr(args, "Command"));
		ifme_argv_split(a, ifme_jstr(args, "Codec"));
		for (size_t i = 0; i < qu.n; i++)
			ifme_argv_push(a, qu.v[i]);
		for (size_t i = 0; i < hzch.n; i++)
			ifme_argv_push(a, hzch.v[i]);
		if (!ifme_is_disabled(filter)) {
			ifme_argv_push(a, "-af");
			ifme_argv_push(a, filter);
		}
		n = 1;
	}
	ifme_argv_split(a, user_args);
	ifme_argv_split(a, ifme_jstr(args, "Output"));
	ifme_argv_push(a, out_file);

	ifme_task_result r = run(E, st, n, 0, enc_track_seconds(E, med));
	ifme_argv_free(&st[0]);
	ifme_argv_free(&st[1]);
	ifme_argv_free(&qu);
	ifme_argv_free(&hzch);

	if (r == IFME_TASK_DONE) {
		char *path = ifme_path_join(E->temp, out_file);
		if (ifme_file_size(path) <= 0) {
			enc_fail(E, "audio %zu: %s finished but wrote no %s", idx + 1, plugin_name, out_file);
			r = IFME_TASK_FAILED;
		}
		free(path);
	}

	if (r == IFME_TASK_DONE)
		*result = out_file;
	else
		free(out_file);
	return r;
}

/* An audio entry may name its own "input" (C#: MediaQueueAudio.FilePath), e.g.
 * a dub or a commentary in a separate file; otherwise it is the job's input. */
static ifme_task_result encode_audio(enc *E, size_t idx, size_t count, const cJSON *item,
                                     char **result, char **lang_out)
{
	const char *own = ifme_jstr(item, "input");
	if (!own || !*own)
		return encode_audio_from(E, idx, count, item, E->input, E->media, result, lang_out);

	char *path = ifme_absolute_path(own);
	char why[512];
	ifme_media *m = path ? ifme_probe(E->ctx, path, NULL, why, sizeof(why)) : NULL;
	ifme_task_result r = IFME_TASK_ERROR;
	if (m)
		r = encode_audio_from(E, idx, count, item, path, m, result, lang_out);
	else
		enc_fail(E, "audio %zu: %s", idx + 1, path ? why : "out of memory");
	ifme_media_free(m);
	free(path);
	return r;
}

/* ---- muxing (port of MediaEncoding.Muxing) -------------------------------------- */

#if defined(_WIN32)
#define IFME_OS_NAME "windows"
#elif defined(__APPLE__)
#define IFME_OS_NAME "macos"
#elif defined(__linux__)
#define IFME_OS_NAME "linux"
#else
#define IFME_OS_NAME "unix"
#endif

/* Every track file -> the output container, with language/title per stream
 * and the same encoding_tool/comment tags as C#. */
static ifme_task_result mux(enc *E, const track *tracks, size_t count)
{
	enc_stage(E, "Writing %s", ifme_basename(E->output));

	ifme_argv a = { 0 };
	ifme_argv_push(&a, E->ffmpeg);
	ifme_argv_split(&a, "-strict -2 -hide_banner -v error -stats");
	for (size_t i = 0; i < count; i++) {
		ifme_argv_push(&a, "-i");
		ifme_argv_push(&a, tracks[i].file);
	}

	/* Subtitles, cover art, attachments, global tags and chapters. */
	ifme_argv extra = { 0 };
	enc_mux_extras(E, &a, &extra, count, count);

	for (size_t i = 0; i < count; i++) {
		ifme_argv_push(&a, "-map");
		ifme_argv_pushf(&a, "%zu:%c", i, tracks[i].kind);
	}
	ifme_argv_split(&a, "-c copy");
	for (size_t i = 0; i < extra.n; i++)
		ifme_argv_push(&a, extra.v[i]);
	ifme_argv_free(&extra);

	/* C#: Mp4MuxFlags (frag_keyframe, empty_moov, separate_moof). */
	static const char *const mov[] = { "mp4", "m4v", "m4a", "mov", NULL };
	const char *movflags = ifme_jstr(E->job, "movflags");
	if (movflags && *movflags && enc_ext_is(E->output, mov)) {
		ifme_argv_push(&a, "-movflags");
		ifme_argv_push(&a, movflags);
	}

	const ifme_cpu_info *cpu = &E->ctx->cpu;
	ifme_argv_push(&a, "-metadata:g");
	ifme_argv_pushf(&a, "encoding_tool=ifme%s/%s/%s/%s", ifme_version(), cpu->arch,
	                IFME_OS_NAME, cpu->hypervisor ? "vm" : "baremetal");
	ifme_argv_push(&a, "-metadata:g");
	ifme_argv_pushf(&a, "comment=%s%s%s", cpu->brand,
	                cpu->hypervisor ? "/" : "", cpu->hypervisor ? cpu->hypervisor_vendor : "");

	/* MP4-family files keep a track's name in its handler, not a title tag. */
	static const char *const iso[] = { "mp4", "m4v", "m4a", "mov", "3gp", "3g2", NULL };
	bool iso_bmff = enc_ext_is(E->output, iso);

	/* The first video and the first audio track are the default ones. Set
	 * explicitly: once any -disposition is given (subtitles, cover art),
	 * FFmpeg stops choosing defaults itself. */
	bool seen_v = false, seen_a = false;
	for (size_t i = 0; i < count; i++) {
		bool *seen = tracks[i].kind == 'v' ? &seen_v : &seen_a;
		ifme_argv_pushf(&a, "-disposition:%zu", i);
		ifme_argv_push(&a, *seen ? "0" : "default");
		*seen = true;
	}

	for (size_t i = 0; i < count; i++) {
		ifme_argv_pushf(&a, "-metadata:s:%zu", i);
		ifme_argv_pushf(&a, "language=%s", tracks[i].lang ? tracks[i].lang : "und");
		if (tracks[i].title && *tracks[i].title) {
			ifme_argv_pushf(&a, "-metadata:s:%zu", i);
			ifme_argv_pushf(&a, "%s=%s", iso_bmff ? "handler_name" : "title", tracks[i].title);
		}
	}

	ifme_argv_push(&a, "-y");
	ifme_argv_push(&a, E->output);
	ifme_task_result r = enc_run1(E, &a, enc_job_seconds(E));
	ifme_argv_free(&a);

	if (r == IFME_TASK_DONE && ifme_file_size(E->output) <= 0) {
		enc_fail(E, "no output written to %s", E->output);
		r = IFME_TASK_FAILED;
	}
	if (r != IFME_TASK_DONE)
		ifme_delete_file(E->output);   /* no half-written file */
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

	track *tracks = NULL;
	size_t track_count = 0, track_cap = 0;
	bool keep_temp = false;

	if (!ctx || !task) {
		enc_fail(&E, "no context or task");
		goto out;
	}
	if (!job) {
		enc_fail(&E, "job is not valid JSON");
		goto out;
	}
	if (E.control && atomic_load(&E.control->want_stop)) {
		enc_fail(&E, "stopped");
		r = IFME_TASK_STOPPED;
		goto out;
	}

	const char *input = ifme_jstr(job, "input");
	const char *out_rel = ifme_jstr(job, "output");
	const cJSON *videos = ifme_jget(job, "video");
	const cJSON *audios = ifme_jget(job, "audio");
	size_t video_count = 0, audio_count = 0;
	keep_temp = ifme_jbool(job, "keep_temp", false);

	/* Entries with "enable": false are skipped (C#'s per-track checkbox). */
	const cJSON *it;
	cJSON_ArrayForEach(it, videos)
		video_count += ifme_jbool(it, "enable", true);
	cJSON_ArrayForEach(it, audios)
		audio_count += ifme_jbool(it, "enable", true);

	if (!input || !*input) {
		enc_fail(&E, "job has no \"input\"");
		goto out;
	}
	if (!out_rel || !*out_rel) {
		enc_fail(&E, "job has no \"output\"");
		goto out;
	}
	if (video_count + audio_count == 0) {
		enc_fail(&E, "job has no \"video\" or \"audio\" entries");
		goto out;
	}

	E.input = ifme_absolute_path(input);
	E.output = ifme_absolute_path(out_rel);

	const cJSON *trim = ifme_jget(job, "trim");
	if (cJSON_IsObject(trim)) {
		E.trim_seconds = parse_time(ifme_jstr(trim, "duration"));
		E.trim_start = ifme_jstr(trim, "start") ? parse_time(ifme_jstr(trim, "start")) : 0;
		if (E.trim_seconds <= 0 || (ifme_jstr(trim, "start") && parse_time(ifme_jstr(trim, "start")) < 0)) {
			enc_fail(&E, "trim needs \"start\" and \"duration\" as HH:MM:SS(.mmm) or seconds");
			goto out;
		}
	}

	E.ffmpeg = ifme_find_ff_tool(ctx, "ffmpeg");
	if (!E.ffmpeg) {
		enc_fail(&E, "ffmpeg not found (PATH or Plugins/ffmpeg64)");
		goto out;
	}

	char why[512];
	E.media = ifme_probe(ctx, E.input, NULL, why, sizeof(why));
	if (!E.media) {
		enc_fail(&E, "%s", why);
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
		enc_fail(&E, "cannot create temporary folder %s", E.temp ? E.temp : "");
		goto out;
	}

	tracks = calloc(video_count + audio_count, sizeof(track));
	if (!tracks)
		goto out;
	track_cap = video_count + audio_count;

	/* Subtitles, fonts, cover art and metadata first: burning a subtitle
	 * into the video needs them (C#: Extract before Video). */
	r = enc_extract(&E);
	if (r != IFME_TASK_DONE)
		goto out;

	/* Same order as C#: video, then audio; tracks keep that order in the output. */
	size_t vi = 0;
	cJSON_ArrayForEach(it, videos) {
		if (!ifme_jbool(it, "enable", true))
			continue;
		track *t = &tracks[track_count];
		r = encode_video(&E, vi, video_count, it, &t->file);
		if (r != IFME_TASK_DONE)
			goto out;
		const ifme_stream_video *src = source_video(E.media, ifme_jint(it, "stream", first_video_stream(E.media)));
		const char *lang = ifme_jstr(it, "language");
		if (!lang || !*lang)
			lang = (src && src->language && *src->language) ? src->language : "und";
		t->kind = 'v';
		t->lang = ifme_strdup(lang);
		t->title = ifme_jstr(it, "title");
		track_count++;
		vi++;
	}

	size_t ai = 0;
	cJSON_ArrayForEach(it, audios) {
		if (!ifme_jbool(it, "enable", true))
			continue;
		track *t = &tracks[track_count];
		r = encode_audio(&E, ai, audio_count, it, &t->file, &t->lang);
		if (r != IFME_TASK_DONE)
			goto out;
		t->kind = 'a';
		t->title = ifme_jstr(it, "title");
		track_count++;
		ai++;
	}

	r = mux(&E, tracks, track_count);

out:
	if (E.temp && !keep_temp) {
		char *fonts = ifme_path_join(E.temp, "attachment");
		if (fonts) {
			ifme_clear_dir(fonts);
			ifme_rmdir(fonts);
		}
		free(fonts);
		ifme_clear_dir(E.temp);
		ifme_rmdir(E.temp);
	}
	enc_extras_free(&E);
	for (size_t i = 0; tracks && i < track_cap; i++) {
		free(tracks[i].file);
		free(tracks[i].lang);
	}
	free(tracks);
	free(E.output);
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

static bool format_has(const cJSON *manifest, const char *container)
{
	const cJSON *f;
	cJSON_ArrayForEach(f, ifme_jget(manifest, "Format")) {
		if (cJSON_IsString(f) && ifme_strcasecmp(f->valuestring, container) == 0)
			return true;
	}
	return false;
}

static const char *first_format(const cJSON *manifest, const char *fallback)
{
	const cJSON *fmt = cJSON_GetArrayItem(ifme_jget(manifest, "Format"), 0);
	return cJSON_IsString(fmt) && fmt->valuestring[0] ? fmt->valuestring : fallback;
}

#define OPUS_GUID "deadface-f00d-f00d-f00d-f00df00df00d"
#define AAC_GUID  "deadbeef-0aac-0aac-0aac-0aac0aac0aac"

/* Audio plugin for a container: FFmpeg Opus where it fits (MKV, WebM), else
 * FFmpeg AAC (MP4), else the first usable plugin listing the container. */
static int default_audio_plugin(ifme_ctx *ctx, const char *container)
{
	static const char *const preferred[] = { OPUS_GUID, AAC_GUID };
	for (size_t i = 0; i < sizeof(preferred) / sizeof(preferred[0]); i++) {
		int pi = ifme_plugin_find(ctx, IFME_PLUGIN_AUDIO, preferred[i], NULL, 0);
		if (pi >= 0 && format_has(ifme_plugin_json(ctx, (size_t)pi), container))
			return pi;
	}
	for (size_t i = 0; i < ifme_plugin_count(ctx); i++) {
		const ifme_plugin_info *p = ifme_plugin_at(ctx, i);
		if (p->kind == IFME_PLUGIN_AUDIO && p->status == IFME_PLUGIN_OK &&
		    format_has(ifme_plugin_json(ctx, i), container))
			return (int)i;
	}
	return -1;
}

static bool is_none(const char *plugin)
{
	return !plugin || !*plugin || ifme_strcasecmp(plugin, "none") == 0;
}

/* Values of a manifest list as text: numbers and strings ("128", "0.5", "g"). */
static char *join_values(const cJSON *arr)
{
	size_t cap = 256, len = 0;
	char *s = malloc(cap);
	if (!s)
		return NULL;
	s[0] = '\0';

	const cJSON *e;
	cJSON_ArrayForEach(e, arr) {
		char item[64];
		if (cJSON_IsString(e))
			snprintf(item, sizeof(item), "%s", e->valuestring);
		else if (cJSON_IsNumber(e))
			snprintf(item, sizeof(item), "%g", e->valuedouble);
		else
			continue;

		size_t need = len + strlen(item) + 3;
		if (need > cap) {
			char *grown = realloc(s, cap = need * 2);
			if (!grown) {
				free(s);
				return NULL;
			}
			s = grown;
		}
		len += (size_t)sprintf(s + len, "%s%s", len ? ", " : "", item);
	}
	return s;
}

static void add_video_entry(cJSON *videos, ifme_ctx *ctx, int pi, const ifme_stream_video *src)
{
	cJSON *v = cJSON_CreateObject();
	cJSON_AddItemToArray(videos, v);

	char fps[32];
	fps_string(source_fps(src), fps, sizeof(fps));

	cJSON_AddNumberToObject(v, "stream", src->id);
	cJSON_AddStringToObject(v, "language", src->language && *src->language ? src->language : "und");
	if (pi < 0) {
		cJSON_AddStringToObject(v, "plugin", "copy");
		return;
	}

	const cJSON *vc = ifme_jget(ifme_plugin_json(ctx, (size_t)pi), "Video");
	const cJSON *mode0 = ifme_jitem(vc, "Mode", 0);
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

static void add_audio_entry(cJSON *audios, ifme_ctx *ctx, int pi, const ifme_stream_audio *src)
{
	cJSON *a = cJSON_CreateObject();
	cJSON_AddItemToArray(audios, a);

	cJSON_AddNumberToObject(a, "stream", src->id);
	cJSON_AddStringToObject(a, "language", src->language && *src->language ? src->language : "und");
	cJSON_AddStringToObject(a, "title", src->title ? src->title : "");
	if (pi < 0) {
		cJSON_AddStringToObject(a, "plugin", "copy");
		return;
	}

	const cJSON *ac = ifme_jget(ifme_plugin_json(ctx, (size_t)pi), "Audio");
	const cJSON *mode0 = ifme_jitem(ac, "Mode", 0);
	char *quality = ifme_jscalar(mode0, "Default");

	cJSON_AddStringToObject(a, "plugin", ifme_plugin_at(ctx, (size_t)pi)->guid);
	cJSON_AddNumberToObject(a, "mode", 0);
	cJSON_AddStringToObject(a, "quality", quality ? quality : "");
	cJSON_AddNumberToObject(a, "sample_rate", ifme_jint(ac, "SampleRateDefault", 0));
	cJSON_AddNumberToObject(a, "channels", ifme_jint(ac, "ChannelDefault", 0));
	cJSON_AddBoolToObject(a, "copy", false);
	cJSON_AddStringToObject(a, "encoder_args", "");
	cJSON_AddStringToObject(a, "ffmpeg_args", "");
	cJSON_AddStringToObject(a, "filter", "");
	free(quality);

	cJSON *help = cJSON_AddObjectToObject(a, "_help");
	cJSON_AddStringToObject(help, "plugin_name", ifme_plugin_at(ctx, (size_t)pi)->name);
	char *srcinfo = ifme_strdupf("%s, %d Hz, %d channel%s", src->codec ? src->codec : "?",
	                             src->sample_rate, src->channels, src->channels == 1 ? "" : "s");
	cJSON_AddStringToObject(help, "source", srcinfo ? srcinfo : "");
	free(srcinfo);

	cJSON *modes = cJSON_AddArrayToObject(help, "modes");
	const cJSON *m;
	int k = 0;
	cJSON_ArrayForEach(m, ifme_jget(ac, "Mode")) {
		char *values = join_values(ifme_jget(m, "Quality"));
		char *def = ifme_jscalar(m, "Default");
		char *line = ifme_strdupf("%d: %s [%s], default %s", k++,
			ifme_jstr(m, "Name") ? ifme_jstr(m, "Name") : "?",
			values ? values : "", def ? def : "?");
		cJSON_AddItemToArray(modes, cJSON_CreateString(line ? line : ""));
		free(line);
		free(def);
		free(values);
	}

	/* As the C# loader: the default rate is always offered; 0 = keep the source. */
	cJSON *rates = cJSON_AddArrayToObject(help, "sample_rates");
	int def_rate = ifme_jint(ac, "SampleRateDefault", 0);
	bool has_def = false;
	cJSON_ArrayForEach(m, ifme_jget(ac, "SampleRate")) {
		if (cJSON_IsNumber(m) && m->valueint == def_rate)
			has_def = true;
	}
	if (!has_def)
		cJSON_AddItemToArray(rates, cJSON_CreateNumber(def_rate));
	cJSON_ArrayForEach(m, ifme_jget(ac, "SampleRate"))
		if (cJSON_IsNumber(m))
			cJSON_AddItemToArray(rates, cJSON_CreateNumber(m->valueint));

	cJSON *chans = cJSON_AddArrayToObject(help, "channels");
	for (const cJSON *c = ifme_jget(ac, "Channels") ? ifme_jget(ac, "Channels")->child : NULL; c; c = c->next) {
		char *line = ifme_strdupf("%s: %s", c->string ? c->string : "?",
		                          cJSON_IsString(c) ? c->valuestring : "");
		cJSON_AddItemToArray(chans, cJSON_CreateString(line ? line : ""));
		free(line);
	}
}

char *ifme_job_new(ifme_ctx *ctx, const char *input, const char *output,
                   const char *video_plugin, const char *audio_plugin,
                   char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';
	if (!ctx || !input)
		return NULL;

	/* plugin index, or -1: copy, -3: none */
	int vpi = -3, api = -3;
	bool audio_auto = audio_plugin == NULL;

	if (enc_is_copy(video_plugin, VIDEO_COPY_GUID)) {
		vpi = -1;
	} else if (!is_none(video_plugin)) {
		vpi = ifme_plugin_find(ctx, IFME_PLUGIN_VIDEO, video_plugin, err, err_len);
		if (vpi < 0)
			return NULL;
	}
	if (enc_is_copy(audio_plugin, AUDIO_COPY_GUID)) {
		api = -1;
	} else if (!audio_auto && !is_none(audio_plugin)) {
		api = ifme_plugin_find(ctx, IFME_PLUGIN_AUDIO, audio_plugin, err, err_len);
		if (api < 0)
			return NULL;
	}

	char *abs_input = ifme_absolute_path(input);
	ifme_media *media = ifme_probe(ctx, abs_input, NULL, err, err_len);
	if (!media) {
		free(abs_input);
		return NULL;
	}

	bool has_video = vpi != -3 && first_video_stream(media) >= 0;
	bool has_audio = media->audio_count > 0 && (audio_auto || api != -3);

	/* Container: the output's extension, else the video plugin's first format,
	 * else (audio only) the audio plugin's: Opus for a plain audio file. */
	const char *container = "mkv";
	if (output && *output)
		container = enc_output_ext(output);
	else if (has_video && vpi >= 0)
		container = first_format(ifme_plugin_json(ctx, (size_t)vpi), "mkv");

	if (has_audio && audio_auto) {
		api = default_audio_plugin(ctx, container);
		if (api < 0) {
			if (err && err_len)
				snprintf(err, err_len, "no usable audio plugin supports .%s "
				         "(choose one, \"copy\" or \"none\")", container);
			ifme_media_free(media);
			free(abs_input);
			return NULL;
		}
	}
	if (!(output && *output) && !has_video && has_audio && api >= 0)
		container = first_format(ifme_plugin_json(ctx, (size_t)api), "mkv");

	cJSON *job = cJSON_CreateObject();
	cJSON *videos = cJSON_CreateArray();
	cJSON *audios = cJSON_CreateArray();

	if (has_video)
		add_video_entry(videos, ctx, vpi, source_video(media, first_video_stream(media)));
	for (size_t i = 0; has_audio && i < media->audio_count; i++)
		add_audio_entry(audios, ctx, api, &media->audio[i]);

	if (cJSON_GetArraySize(videos) + cJSON_GetArraySize(audios) == 0) {
		if (err && err_len)
			snprintf(err, err_len, "%s: nothing to encode (%zu video, %zu audio streams%s)", input,
			         media->video_count, media->audio_count,
			         is_none(video_plugin) && !audio_auto && is_none(audio_plugin)
			         ? "; video and audio are both \"none\"" : "");
		goto fail;
	}

	/* Output next to the input: "<name>.ifme.<container>". */
	char *out_path = NULL;
	if (output && *output) {
		out_path = ifme_absolute_path(output);
	} else {
		char *base = ifme_strdup(abs_input);
		char *dot = base ? strrchr(base, '.') : NULL;
		const char *slash = base ? ifme_basename(base) : NULL;
		if (dot && dot > slash)
			*dot = '\0';
		out_path = base ? ifme_strdupf("%s.ifme.%s", base, container) : NULL;
		free(base);
	}

	cJSON_AddStringToObject(job, "input", abs_input);
	cJSON_AddStringToObject(job, "output", out_path ? out_path : "");
	cJSON_AddStringToObject(job, "temp_dir", "");
	cJSON_AddBoolToObject(job, "keep_temp", false);
	cJSON_AddItemToObject(job, "video", videos);
	cJSON_AddItemToObject(job, "audio", audios);
	videos = audios = NULL;
	enc_job_extras(job, media, container, has_video, vpi == -1);

	char *text = cJSON_Print(job);
	cJSON_Delete(job);
	free(out_path);
	ifme_media_free(media);
	free(abs_input);
	return text;

fail:
	cJSON_Delete(videos);
	cJSON_Delete(audios);
	cJSON_Delete(job);
	ifme_media_free(media);
	free(abs_input);
	return NULL;
}
