/*
 * probe.c - media information from ffprobe
 *
 * Port of IFME.FFmpeg/MediaInfo.cs + ReadFile.cs. Same defaults and rules,
 * with these corrections:
 *  - Audio bit depth: C# assigned sample_fmt (a string such as "fltp") to an
 *    int, which always threw, so every stream reported 16. Here it comes from
 *    bits_per_raw_sample or sample_fmt; C#'s ">= 32 means 24" rule is kept.
 *  - A 0/0 frame rate is 0 rather than NaN.
 *  - ffprobe runs without a shell and with "-v error", so a failure reports
 *    ffprobe's own reason instead of an empty result.
 */

#include <ctype.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"
#include "cJSON.h"

#define PROBE_TIMEOUT_MS (60 * 1000)

typedef struct media_impl {
	ifme_media pub;              /* must stay first: ifme_media* == media_impl* */
	cJSON *raw;                  /* untouched ffprobe output                    */
	char **strings;              /* every string the public struct points to    */
	size_t string_count, string_cap;
	ifme_stream_video *video;
	ifme_stream_audio *audio;
	ifme_stream_subtitle *subtitle;
	ifme_stream_attachment *attachment;
} media_impl;

/* ---- helpers --------------------------------------------------------------- */

/* Keep a copy of s alive for the lifetime of the media object. */
static const char *keep(media_impl *m, const char *s)
{
	if (!s)
		s = "";

	if (m->string_count == m->string_cap) {
		size_t cap = m->string_cap ? m->string_cap * 2 : 32;
		char **grown = realloc(m->strings, cap * sizeof(char *));
		if (!grown)
			return "";
		m->strings = grown;
		m->string_cap = cap;
	}

	char *copy = ifme_strdup(s);
	if (!copy)
		return "";
	m->strings[m->string_count++] = copy;
	return copy;
}

static const cJSON *jget(const cJSON *o, const char *key)
{
	return o ? cJSON_GetObjectItemCaseSensitive(o, key) : NULL;
}

static const char *jstr(const cJSON *o, const char *key)
{
	const cJSON *i = jget(o, key);
	return cJSON_IsString(i) ? i->valuestring : NULL;
}

/* ffprobe writes many numbers as strings ("48000", "5.120000"). */
static bool jnumber(const cJSON *o, const char *key, double *out)
{
	const cJSON *i = jget(o, key);

	if (cJSON_IsNumber(i)) {
		*out = i->valuedouble;
		return true;
	}
	if (cJSON_IsString(i) && *i->valuestring) {
		char *end = NULL;
		double d = strtod(i->valuestring, &end);
		if (end && *end == '\0') {
			*out = d;
			return true;
		}
	}
	return false;
}

static int jint(const cJSON *o, const char *key, int def)
{
	double d;
	return jnumber(o, key, &d) ? (int)d : def;
}

/* Tag lookup: ffprobe keeps tag case as written by the muxer ("language",
 * "LANGUAGE", "Title"...), so compare case-insensitively. Empty -> def. */
static const char *tag(const cJSON *stream, const char *key, const char *def)
{
	const cJSON *tags = jget(stream, "tags");
	const cJSON *i = tags ? cJSON_GetObjectItem(tags, key) : NULL;
	return (cJSON_IsString(i) && *i->valuestring) ? i->valuestring : def;
}

/* "30000/1001" -> 29.97; "0/0" or garbage -> 0. */
static double rate(const char *s)
{
	if (!s)
		return 0;

	char *end = NULL;
	double n = strtod(s, &end);
	if (!end || *end != '/')
		return 0;

	double d = strtod(end + 1, NULL);
	return d != 0 ? n / d : 0;
}

/* C#: Regex "yuv(\d+)" -> chroma, default 420. */
static int pix_chroma(const char *pf)
{
	for (const char *p = pf ? strstr(pf, "yuv") : NULL; p; p = strstr(p + 1, "yuv"))
		if (isdigit((unsigned char)p[3]))
			return atoi(p + 3);
	return 420;
}

/* C#: Regex "yuv\d+p(\d+)" -> bit depth, else 0 (caller falls back to 8). */
static int pix_bits(const char *pf)
{
	for (const char *p = pf ? strstr(pf, "yuv") : NULL; p; p = strstr(p + 1, "yuv")) {
		const char *q = p + 3;
		if (!isdigit((unsigned char)*q))
			continue;
		while (isdigit((unsigned char)*q))
			q++;
		if (*q == 'p' && isdigit((unsigned char)q[1]))
			return atoi(q + 1);
	}
	return 0;
}

static int audio_bits(const cJSON *stream)
{
	int bits = jint(stream, "bits_per_raw_sample", 0);

	if (bits <= 0) {
		const char *fmt = jstr(stream, "sample_fmt");
		if (!fmt)
			bits = 0;
		else if (!strncmp(fmt, "u8", 2))
			bits = 8;
		else if (!strncmp(fmt, "s16", 3))
			bits = 16;
		else if (!strncmp(fmt, "s32", 3) || !strncmp(fmt, "flt", 3))
			bits = 32;
		else if (!strncmp(fmt, "s64", 3) || !strncmp(fmt, "dbl", 3))
			bits = 64;
	}

	/* Same normalisation as MediaInfo.cs: unknown -> 16, 32-bit/float -> 24. */
	if (bits == 0)
		return 16;
	if (bits >= 32)
		return 24;
	return bits;
}

/* FormatId.cs: subtitle codec name -> file extension. */
static const char *subtitle_ext(const char *codec)
{
	if (!strcmp(codec, "subrip"))
		return "srt";
	if (!strcmp(codec, "hdmv_pgs_subtitle"))
		return "sup";
	return codec;
}

/* First non-empty line of ffprobe's stderr, for error messages. */
static void first_line(const char *text, char *out, size_t len)
{
	out[0] = '\0';
	if (!text || !len)
		return;

	while (*text == '\r' || *text == '\n' || *text == ' ')
		text++;

	size_t n = strcspn(text, "\r\n");
	if (n >= len)
		n = len - 1;
	memcpy(out, text, n);
	out[n] = '\0';
}

/* ---- parsing --------------------------------------------------------------- */

static bool grow(void **arr, size_t count, size_t size)
{
	void *grown = realloc(*arr, (count + 1) * size);
	if (!grown)
		return false;
	*arr = grown;
	return true;
}

static void parse(media_impl *m, const char *path)
{
	ifme_media *pub = &m->pub;
	const cJSON *format = jget(m->raw, "format");
	double d;

	pub->path = keep(m, path);
	pub->format_name = keep(m, jstr(format, "format_name") ? jstr(format, "format_name") : "NEW");
	pub->format_long_name = keep(m, jstr(format, "format_long_name") ? jstr(format, "format_long_name") : "");
	pub->encoder = keep(m, tag(format, "encoder", ""));
	pub->file_size = jnumber(format, "size", &d) && d > 0 ? (unsigned long long)d : 0;
	pub->bit_rate = jnumber(format, "bit_rate", &d) && d > 0 ? (unsigned long long)d : 0;
	pub->duration = jnumber(format, "duration", &d) ? d : 0;

	const cJSON *stream;
	cJSON_ArrayForEach(stream, jget(m->raw, "streams")) {
		const char *type = jstr(stream, "codec_type");
		if (!type)
			continue;

		int id = jint(stream, "index", 0);
		const char *lang = tag(stream, "language", "und");
		const char *codec = jstr(stream, "codec_name");
		const char *title = tag(stream, "title", "");
		if (!codec || !*codec)
			codec = "unknown";

		if (!ifme_strcasecmp(type, "video")) {
			if (!grow((void **)&m->video, pub->video_count, sizeof(*m->video)))
				continue;

			ifme_stream_video *v = &m->video[pub->video_count++];
			const char *pf = jstr(stream, "pix_fmt");
			const cJSON *disp = jget(stream, "disposition");

			memset(v, 0, sizeof(*v));
			v->id = id;
			v->language = keep(m, lang);
			v->codec = keep(m, codec);
			v->title = keep(m, title);
			v->pix_fmt = keep(m, pf ? pf : "");
			v->width = jint(stream, "width", 0);
			v->height = jint(stream, "height", 0);
			v->chroma = pix_chroma(pf);
			v->bit_depth = jint(stream, "bits_per_raw_sample", 0);
			if (v->bit_depth <= 0)
				v->bit_depth = pix_bits(pf);
			if (v->bit_depth <= 0)
				v->bit_depth = 8;
			v->frame_rate = rate(jstr(stream, "r_frame_rate"));
			v->frame_rate_avg = rate(jstr(stream, "avg_frame_rate"));
			double diff = v->frame_rate - v->frame_rate_avg;
			v->frame_rate_constant = diff > -0.001 && diff < 0.001;
			v->duration = pub->duration;   /* C# used the container duration */
			v->frame_count = (long long)(pub->duration *
				(v->frame_rate_avg > 0 ? v->frame_rate_avg : v->frame_rate));
			v->attached_pic = jint(disp, "attached_pic", 0) != 0;
		} else if (!ifme_strcasecmp(type, "audio")) {
			if (!grow((void **)&m->audio, pub->audio_count, sizeof(*m->audio)))
				continue;

			ifme_stream_audio *a = &m->audio[pub->audio_count++];
			memset(a, 0, sizeof(*a));
			a->id = id;
			a->language = keep(m, lang);
			a->codec = keep(m, codec);
			a->title = keep(m, title);
			a->sample_rate = jint(stream, "sample_rate", 44100);
			a->channels = jint(stream, "channels", 2);
			a->bit_depth = audio_bits(stream);
			a->duration = pub->duration;
		} else if (!ifme_strcasecmp(type, "subtitle")) {
			if (!grow((void **)&m->subtitle, pub->subtitle_count, sizeof(*m->subtitle)))
				continue;

			ifme_stream_subtitle *s = &m->subtitle[pub->subtitle_count++];
			s->id = id;
			s->language = keep(m, lang);
			s->codec = keep(m, subtitle_ext(codec));
			s->title = keep(m, title);
		} else if (!ifme_strcasecmp(type, "attachment")) {
			if (!grow((void **)&m->attachment, pub->attachment_count, sizeof(*m->attachment)))
				continue;

			ifme_stream_attachment *t = &m->attachment[pub->attachment_count++];
			t->id = id;
			t->filename = keep(m, tag(stream, "filename", "unknown"));
			t->mime_type = keep(m, tag(stream, "mimetype", "application/octet-stream"));
		}
	}

	pub->video = m->video;
	pub->audio = m->audio;
	pub->subtitle = m->subtitle;
	pub->attachment = m->attachment;
}

/* ---- public API -------------------------------------------------------------- */

static void set_err(char *err, size_t len, const char *fmt, const char *arg)
{
	if (err && len)
		snprintf(err, len, fmt, arg ? arg : "");
}

ifme_media *ifme_probe(ifme_ctx *ctx, const char *path, const char *frame_rate,
                       char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';
	if (!ctx || !path || !*path) {
		set_err(err, err_len, "no file given%s", NULL);
		return NULL;
	}

	/* "%" marks an image-sequence pattern, which is not a real file. */
	if (!ifme_file_exists(path) && !strchr(path, '%')) {
		set_err(err, err_len, "file not found: %s", path);
		return NULL;
	}

	char *ffprobe = ifme_find_ff_tool(ctx, "ffprobe");
	if (!ffprobe) {
		set_err(err, err_len, "ffprobe not found (PATH or Plugins/ffmpeg64)%s", NULL);
		return NULL;
	}

	ifme_argv cmd = { 0 };
	ifme_argv_push(&cmd, ffprobe);
	ifme_argv_split(&cmd, "-hide_banner -v error -print_format json -show_format -show_streams");
	if (frame_rate && *frame_rate) {
		ifme_argv_push(&cmd, "-framerate");
		ifme_argv_push(&cmd, frame_rate);
	}
	/* A name starting with "-" would be read as an option. */
	if (path[0] == '-')
		ifme_argv_pushf(&cmd, "file:%s", path);
	else
		ifme_argv_push(&cmd, path);
	free(ffprobe);

	ifme_run_opts opts = { 0 };
	opts.timeout_ms = PROBE_TIMEOUT_MS;
	opts.capture_stdout = true;
	opts.capture_stderr = true;

	ifme_run_result run;
	int rc = ifme_run(&cmd, 1, &opts, &run);
	ifme_argv_free(&cmd);

	media_impl *m = NULL;

	if (rc != IFME_RUN_OK) {
		set_err(err, err_len, "ffprobe: %s", run.message);
	} else if (run.exit_code != 0) {
		char line[400];
		first_line(run.err, line, sizeof(line));
		if (err && err_len)
			snprintf(err, err_len, "ffprobe failed (exit code %d)%s%s", run.exit_code,
			         line[0] ? ": " : "", line);
	} else {
		m = calloc(1, sizeof(*m));
		if (m)
			m->raw = cJSON_Parse(run.out ? run.out : "");

		if (!m || !m->raw) {
			set_err(err, err_len, m ? "ffprobe printed something that is not JSON%s"
			                        : "out of memory%s", NULL);
			free(m);
			m = NULL;
		} else {
			parse(m, path);
		}
	}

	ifme_run_result_free(&run);
	return m ? &m->pub : NULL;
}

void ifme_media_free(ifme_media *media)
{
	if (!media)
		return;

	media_impl *m = (media_impl *)media;
	for (size_t i = 0; i < m->string_count; i++)
		free(m->strings[i]);
	free(m->strings);
	free(m->video);
	free(m->audio);
	free(m->subtitle);
	free(m->attachment);
	cJSON_Delete(m->raw);
	free(m);
}

char *ifme_media_json(const ifme_media *media, bool include_raw)
{
	if (!media)
		return NULL;

	const media_impl *m = (const media_impl *)media;
	cJSON *o = cJSON_CreateObject();
	if (!o)
		return NULL;

	cJSON_AddStringToObject(o, "path", media->path);
	cJSON_AddStringToObject(o, "format_name", media->format_name);
	cJSON_AddStringToObject(o, "format_long_name", media->format_long_name);
	cJSON_AddStringToObject(o, "encoder", media->encoder);
	cJSON_AddNumberToObject(o, "file_size", (double)media->file_size);
	cJSON_AddNumberToObject(o, "bit_rate", (double)media->bit_rate);
	cJSON_AddNumberToObject(o, "duration", media->duration);

	cJSON *arr = cJSON_AddArrayToObject(o, "video");
	for (size_t i = 0; arr && i < media->video_count; i++) {
		const ifme_stream_video *v = &media->video[i];
		cJSON *s = cJSON_CreateObject();
		cJSON_AddNumberToObject(s, "id", v->id);
		cJSON_AddStringToObject(s, "language", v->language);
		cJSON_AddStringToObject(s, "codec", v->codec);
		cJSON_AddStringToObject(s, "title", v->title);
		cJSON_AddStringToObject(s, "pix_fmt", v->pix_fmt);
		cJSON_AddNumberToObject(s, "width", v->width);
		cJSON_AddNumberToObject(s, "height", v->height);
		cJSON_AddNumberToObject(s, "chroma", v->chroma);
		cJSON_AddNumberToObject(s, "bit_depth", v->bit_depth);
		cJSON_AddNumberToObject(s, "frame_rate", v->frame_rate);
		cJSON_AddNumberToObject(s, "frame_rate_avg", v->frame_rate_avg);
		cJSON_AddBoolToObject(s, "frame_rate_constant", v->frame_rate_constant);
		cJSON_AddNumberToObject(s, "frame_count", (double)v->frame_count);
		cJSON_AddNumberToObject(s, "duration", v->duration);
		cJSON_AddBoolToObject(s, "attached_pic", v->attached_pic);
		cJSON_AddItemToArray(arr, s);
	}

	arr = cJSON_AddArrayToObject(o, "audio");
	for (size_t i = 0; arr && i < media->audio_count; i++) {
		const ifme_stream_audio *a = &media->audio[i];
		cJSON *s = cJSON_CreateObject();
		cJSON_AddNumberToObject(s, "id", a->id);
		cJSON_AddStringToObject(s, "language", a->language);
		cJSON_AddStringToObject(s, "codec", a->codec);
		cJSON_AddStringToObject(s, "title", a->title);
		cJSON_AddNumberToObject(s, "sample_rate", a->sample_rate);
		cJSON_AddNumberToObject(s, "channels", a->channels);
		cJSON_AddNumberToObject(s, "bit_depth", a->bit_depth);
		cJSON_AddNumberToObject(s, "duration", a->duration);
		cJSON_AddItemToArray(arr, s);
	}

	arr = cJSON_AddArrayToObject(o, "subtitle");
	for (size_t i = 0; arr && i < media->subtitle_count; i++) {
		const ifme_stream_subtitle *t = &media->subtitle[i];
		cJSON *s = cJSON_CreateObject();
		cJSON_AddNumberToObject(s, "id", t->id);
		cJSON_AddStringToObject(s, "language", t->language);
		cJSON_AddStringToObject(s, "codec", t->codec);
		cJSON_AddStringToObject(s, "title", t->title);
		cJSON_AddItemToArray(arr, s);
	}

	arr = cJSON_AddArrayToObject(o, "attachment");
	for (size_t i = 0; arr && i < media->attachment_count; i++) {
		const ifme_stream_attachment *t = &media->attachment[i];
		cJSON *s = cJSON_CreateObject();
		cJSON_AddNumberToObject(s, "id", t->id);
		cJSON_AddStringToObject(s, "filename", t->filename);
		cJSON_AddStringToObject(s, "mime_type", t->mime_type);
		cJSON_AddItemToArray(arr, s);
	}

	if (include_raw && m->raw)
		cJSON_AddItemToObject(o, "ffprobe", cJSON_Duplicate(m->raw, true));

	char *out = cJSON_Print(o);
	cJSON_Delete(o);
	return out;
}
