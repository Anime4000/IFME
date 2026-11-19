/*
 * extras.c - subtitles, fonts/attachments, cover art, global tags and
 * chapters (port of MediaEncoding.Extract and the matching parts of Muxing)
 *
 * Before the video and audio are encoded, everything else the output keeps is
 * copied into the temp folder:
 *   metadata.ini         ffmpeg -f ffmetadata: global tags and chapters
 *   attachment/NAME      fonts and other attachments (-dump_attachment)
 *   subtitleNNNN_LANG.X  each subtitle stream, -c:s copy
 *   burn.X               the subtitle to burn into the video, if any
 *   cover.jpg / .png     cover art
 * The mux then adds them to the output.
 *
 * Differences from C#, on purpose:
 *  - Trimmed jobs get their chapters and subtitles cut to the same part;
 *    C# kept the chapters of the whole file.
 *  - The global tags really come from the source (C# mapped them from the
 *    first encoded track by mistake, so only the chapters came through).
 *  - Attachment names from the source are reduced to a plain file name: a
 *    name such as "../../x.ttf" could write outside the temp folder in C#.
 *  - Text subtitles other than SRT/ASS/SSA/WebVTT (e.g. mov_text) become SRT,
 *    so they fit MKV as well as MP4; WebM gets WebVTT. Subtitles a container
 *    cannot hold are left out with a log line instead of failing the mux.
 *  - Every attachment of the source is kept (C# skipped attachment stream 0
 *    and read past the list for extra files).
 *  - A subtitle that cannot be burned (a picture format such as PGS) is an
 *    error, not silently ignored.
 */

#include <stdarg.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "encode_int.h"

static const char *const MATROSKA[] = { "mkv", "mka", "mks", NULL };
static const char *const ISO_SUBS[] = { "mp4", "m4v", "mov", NULL };            /* mov_text */
static const char *const ISO_ALL[]  = { "mp4", "m4v", "m4a", "mov", "3gp", "3g2", NULL };
static const char *const WEBM[]     = { "webm", NULL };                          /* WebVTT */
static const char *const COVER[]    = { "mkv", "mka", "mp4", "m4v", "m4a", "mov", "mp3", "flac", NULL };

static bool ext_in(const char *ext, const char *const *list)
{
	for (; ext && *list; list++)
		if (ifme_strcasecmp(ext, *list) == 0)
			return true;
	return false;
}

/* ---- the file an entry reads ------------------------------------------------- */

/* An entry's own "input" (probed), else the job's input. */
typedef struct src {
	const char *path;
	const ifme_media *media;
	char *own_path;
	ifme_media *own_media;
} src;

static bool src_open(enc *E, const cJSON *item, const char *what, size_t idx, src *s)
{
	memset(s, 0, sizeof(*s));
	const char *own = ifme_jstr(item, "input");
	if (!own || !*own) {
		s->path = E->input;
		s->media = E->media;
		return true;
	}

	char why[512] = "";
	s->own_path = ifme_absolute_path(own);
	s->own_media = s->own_path ? ifme_probe(E->ctx, s->own_path, NULL, why, sizeof(why)) : NULL;
	if (!s->own_media) {
		enc_fail(E, "%s %zu: %s", what, idx + 1, why[0] ? why : "out of memory");
		free(s->own_path);
		s->own_path = NULL;
		return false;
	}
	s->path = s->own_path;
	s->media = s->own_media;
	return true;
}

static void src_close(src *s)
{
	ifme_media_free(s->own_media);
	free(s->own_path);
}

static void logf_(enc *E, const char *fmt, ...)
{
	va_list ap;
	va_start(ap, fmt);
	char *s = ifme_vstrdupf(fmt, ap);
	va_end(ap);
	if (s)
		enc_log(E, s);
	free(s);
}

static char *temp_path(const enc *E, const char *name)
{
	return ifme_path_join(E->temp, name);
}

/* ---- subtitles ----------------------------------------------------------------- */

/* Temp file extension for a subtitle codec (as ifme_probe names it), and
 * whether FFmpeg converts it to SubRip on the way. */
static const char *sub_file_ext(const char *codec, bool *to_srt)
{
	static const char *const keep[] = { "srt", "ass", "ssa", "sup", NULL };
	static const char *const text[] = { "mov_text", "text", "subviewer", "subviewer1", "microdvd",
	                                    "jacosub", "realtext", "sami", "mpl2", "pjs", "stl",
	                                    "vplayer", NULL };
	*to_srt = false;
	if (ext_in(codec, keep))
		return codec;
	if (codec && strcmp(codec, "webvtt") == 0)
		return "vtt";
	if (ext_in(codec, text)) {
		*to_srt = true;
		return "srt";
	}
	return "mks";   /* DVD/DVB pictures and the rest: Matroska keeps them as they are */
}

static bool is_text_ext(const char *ext)
{
	static const char *const text[] = { "srt", "ass", "ssa", "vtt", NULL };
	return ext_in(ext, text);
}

static const ifme_stream_subtitle *find_sub(const ifme_media *m, int id)
{
	for (size_t i = 0; i < m->subtitle_count; i++)
		if (m->subtitle[i].id == id)
			return &m->subtitle[i];
	return NULL;
}

/* Some ASS files from MKV contain NUL characters, which end the subtitle
 * text early in libass and FFmpeg; C# replaced them with spaces. */
static void ass_strip_nul(const enc *E, const char *file)
{
	char *path = temp_path(E, file);
	size_t n = 0;
	char *d = path ? ifme_read_file(path, &n) : NULL;
	bool changed = false;
	for (size_t i = 0; d && i < n; i++) {
		if (d[i] == '\0') {
			d[i] = ' ';
			changed = true;
		}
	}
	if (changed)
		ifme_write_file(path, d, n);
	free(d);
	free(path);
}

static ifme_task_result extract_sub(enc *E, const src *s, int stream, bool to_srt, bool trim,
                                    const char *file)
{
	ifme_argv a = { 0 };
	ifme_argv_push(&a, E->ffmpeg);
	ifme_argv_split(&a, "-hide_banner -v error -stats -i");
	ifme_argv_push(&a, s->path);
	if (trim)
		enc_push_trim(E, &a);
	ifme_argv_push(&a, "-map");
	ifme_argv_pushf(&a, "0:%d", stream);
	ifme_argv_split(&a, "-map_metadata -1 -map_chapters -1 -vn -an -dn -c:s");
	ifme_argv_push(&a, to_srt ? "srt" : "copy");
	ifme_argv_push(&a, "-y");
	ifme_argv_push(&a, file);
	ifme_task_result r = enc_run1(E, &a, trim ? enc_track_seconds(E, s->media) : s->media->duration);
	ifme_argv_free(&a);
	return r;
}

/* Cut a whole text subtitle to the trim; a file with no event left is
 * emptied (and then left out). False only on a read/write error. */
static bool retime_sub(enc *E, const char *path, const char *fmt)
{
	size_t len = 0, kept = 0;
	char *text = ifme_read_file(path, &len);
	char *cut = text ? ifme_subtitle_cut(text, len, fmt, E->trim_start, E->trim_seconds, &kept) : NULL;
	bool ok = cut && ifme_write_file(path, cut, kept ? strlen(cut) : 0);
	if (!ok)
		enc_fail(E, "cannot cut subtitle %s to the trim", ifme_basename(path));
	free(cut);
	free(text);
	return ok;
}

static bool video_is_copied(const enc *E, bool *any_video)
{
	bool copied = false;
	const cJSON *it;
	*any_video = false;
	cJSON_ArrayForEach(it, ifme_jget(E->job, "video")) {
		if (!ifme_jbool(it, "enable", true))
			continue;
		*any_video = true;
		const char *p = ifme_jstr(it, "plugin");
		if (p && (ifme_strcasecmp(p, "copy") == 0 || ifme_strcasecmp(p, VIDEO_COPY_GUID) == 0))
			copied = true;
	}
	return copied;
}

static ifme_task_result do_subtitles(enc *E)
{
	const cJSON *list = ifme_jget(E->job, "subtitles");
	bool burn = ifme_jbool(E->job, "burn_subtitle", false);
	const char *ext = enc_output_ext(E->output);
	bool matroska = ext_in(ext, MATROSKA), iso = ext_in(ext, ISO_SUBS), webm = ext_in(ext, WEBM);
	bool burned = false;
	size_t idx = 0, count = 0;
	const cJSON *it;

	cJSON_ArrayForEach(it, list)
		count += ifme_jbool(it, "enable", true);
	if (count == 0) {
		if (burn)
			enc_log(E, "burn_subtitle: the job has no subtitles; nothing to burn");
		return IFME_TASK_DONE;
	}

	cJSON_ArrayForEach(it, list) {
		if (!ifme_jbool(it, "enable", true))
			continue;
		size_t n = idx++;

		src s;
		if (!src_open(E, it, "subtitle", n, &s))
			return IFME_TASK_ERROR;

		const ifme_stream_subtitle *ss = find_sub(s.media,
			ifme_jint(it, "stream", s.media->subtitle_count ? s.media->subtitle[0].id : -1));
		if (!ss) {
			enc_fail(E, "subtitle %zu: %s has no subtitle stream %d", n + 1, ifme_basename(s.path),
			         ifme_jint(it, "stream", -1));
			src_close(&s);
			return IFME_TASK_ERROR;
		}

		const char *lang = ifme_jstr(it, "language");
		if (!lang || !*lang)
			lang = ss->language && *ss->language ? ss->language : "und";
		bool to_srt;
		const char *fext = sub_file_ext(ss->codec, &to_srt);
		ifme_task_result r = IFME_TASK_DONE;

		/* The first subtitle is burned in when asked (C#: HardSub). It is cut by
		 * the video's own trim later, so it is extracted whole. */
		if (burn && !burned) {
			burned = true;
			bool any_video;
			bool copied = video_is_copied(E, &any_video);
			if (!any_video || copied) {
				enc_fail(E, "burn_subtitle: %s", any_video
				         ? "a copied video stream cannot have a subtitle burned in"
				         : "the job has no video to burn the subtitle into");
				r = IFME_TASK_ERROR;
			} else if (!is_text_ext(fext)) {
				enc_fail(E, "subtitle %zu (%s) is a picture format; only text subtitles "
				         "(SRT, ASS, SSA, WebVTT) can be burned in", n + 1, ss->codec);
				r = IFME_TASK_ERROR;
			} else {
				char *file = ifme_strdupf("burn.%s", fext);
				enc_stage(E, "Subtitle %zu: extracting to burn in", n + 1);
				r = extract_sub(E, &s, ss->id, to_srt, false, file);
				if (r == IFME_TASK_DONE) {
					ass_strip_nul(E, file);
					E->burn = file;
				} else {
					free(file);
				}
			}
			src_close(&s);
			if (r != IFME_TASK_DONE)
				return r;
			continue;
		}

		/* Which subtitles the container takes (C#: MKV all, MP4 as mov_text). */
		if (!matroska && !((iso || webm) && is_text_ext(fext))) {
			logf_(E, "subtitle %zu (%s): a .%s file cannot hold it, left out", n + 1, ss->codec, ext);
			src_close(&s);
			continue;
		}

		/* Text subtitles are extracted whole and cut here (see subtime.c);
		 * pictures (PGS, DVD) get FFmpeg's own cut. */
		char *file = ifme_strdupf("subtitle%04zu_%s.%s", n, lang, fext);
		bool retime = E->trim_seconds > 0 && is_text_ext(fext);
		enc_stage(E, "Subtitle %zu/%zu: extracting", n + 1, count);
		r = extract_sub(E, &s, ss->id, to_srt, E->trim_seconds > 0 && !retime, file);
		src_close(&s);
		if (r != IFME_TASK_DONE) {
			free(file);
			return r;
		}

		char *path = temp_path(E, file);
		if (retime && !retime_sub(E, path, fext)) {
			free(path);
			free(file);
			return IFME_TASK_ERROR;
		}
		if (ifme_file_size(path) <= 0) {
			logf_(E, "subtitle %zu: nothing in the encoded part, left out", n + 1);
			ifme_delete_file(path);
			free(path);
			free(file);
			continue;
		}
		free(path);

		if (ext_in(fext, (const char *const[]){ "ass", "ssa", NULL }))
			ass_strip_nul(E, file);

		track *t = &E->subs[E->sub_count++];
		t->file = file;
		t->kind = 's';
		t->lang = ifme_strdup(lang);
		t->title = ifme_jstr(it, "title");
		t->is_default = ifme_jbool(it, "default", false);
		t->forced = ifme_jbool(it, "forced", false);
	}
	return IFME_TASK_DONE;
}

/* ---- attachments ------------------------------------------------------------------ */

/* A plain file name: no folders (the name comes from the source file and
 * must not write outside the temp folder), no characters Windows refuses. */
static char *safe_name(const char *name, size_t idx)
{
	const char *b = name ? name : "";
	for (const char *p = b; *p; p++)
		if (*p == '/' || *p == '\\')
			b = p + 1;

	char *s = ifme_strdup(b);
	if (!s)
		return NULL;
	for (char *p = s; *p; p++) {
		unsigned char c = (unsigned char)*p;
		if (c < 32 || strchr(":*?\"<>|", c))
			*p = '_';
	}
	if (!*s || strcmp(s, ".") == 0 || strcmp(s, "..") == 0 || ifme_strcasecmp(s, "unknown") == 0) {
		free(s);
		s = ifme_strdupf("attachment%zu.bin", idx);
	}
	return s;
}

/* The MIME types mkvmerge writes (players look fonts up by these). */
static const char *mime_for(const char *name, const char *given)
{
	if (given && *given && ifme_strcasecmp(given, "application/octet-stream") != 0)
		return given;
	static const char *const map[][2] = {
		{ "ttf", "application/x-truetype-font" }, { "ttc", "application/x-truetype-font" },
		{ "otf", "application/vnd.ms-opentype" }, { "woff", "font/woff" }, { "woff2", "font/woff2" },
		{ "jpg", "image/jpeg" }, { "jpeg", "image/jpeg" }, { "png", "image/png" },
		{ "webp", "image/webp" }, { "txt", "text/plain" }, { "xml", "application/xml" },
	};
	const char *dot = name ? strrchr(name, '.') : NULL;
	for (size_t i = 0; dot && i < sizeof(map) / sizeof(map[0]); i++)
		if (ifme_strcasecmp(dot + 1, map[i][0]) == 0)
			return map[i][1];
	return "application/octet-stream";
}

static bool name_taken(const enc *E, const char *name, char *const *planned, size_t planned_n)
{
	for (size_t i = 0; i < E->att_count; i++)
		if (ifme_strcasecmp(E->att[i].name, name) == 0)
			return true;
	for (size_t i = 0; i < planned_n; i++)
		if (ifme_strcasecmp(planned[i], name) == 0)
			return true;
	return false;
}

/* The metadata dump of the job's input, plus every attachment of the job's
 * input in the same FFmpeg run (C# dumped them one run each). Attachments of
 * other files get a run of their own; "file" entries are copied. */
static ifme_task_result do_attachments_and_metadata(enc *E)
{
	const cJSON *list = ifme_jget(E->job, "attachments");
	const char *ext = enc_output_ext(E->output);
	bool keep = ext_in(ext, MATROSKA);
	bool want_meta = ifme_jbool(E->job, "metadata", true);
	bool want_ch = ifme_jbool(E->job, "chapters", true);
	size_t count = 0;
	const cJSON *it;

	cJSON_ArrayForEach(it, list)
		count += ifme_jbool(it, "enable", true);
	if (count && !keep && !E->burn)
		logf_(E, "attachments: a .%s file cannot hold them, left out", ext);
	bool use = count && (keep || E->burn);   /* fonts are also needed to burn ASS */

	E->att = use ? calloc(count, sizeof(attachment)) : NULL;
	char **main_names = use ? calloc(count, sizeof(char *)) : NULL;
	int *main_ids = use ? calloc(count, sizeof(int)) : NULL;
	char **main_mimes = use ? calloc(count, sizeof(char *)) : NULL;
	size_t main_n = 0, idx = 0;
	ifme_task_result r = IFME_TASK_DONE;
	char *dir = temp_path(E, "attachment");

	if (use && (!E->att || !main_names || !main_ids || !main_mimes || !dir || !ifme_mkdir(dir))) {
		enc_fail(E, "cannot create %s", dir ? dir : "the attachment folder");
		r = IFME_TASK_ERROR;
		goto out;
	}
	E->fonts = use;

	const cJSON *todo = use ? list : NULL;
	cJSON_ArrayForEach(it, todo) {
		if (!ifme_jbool(it, "enable", true))
			continue;
		size_t n = idx++;

		/* A file on disk (a font that is not inside any media file). */
		const char *file = ifme_jstr(it, "file");
		if (file && *file) {
			char *from = ifme_absolute_path(file);
			char *name = safe_name(ifme_jstr(it, "filename") ? ifme_jstr(it, "filename") : ifme_basename(from), n);
			if (name_taken(E, name, main_names, main_n)) {
				char *u = ifme_strdupf("%zu_%s", n, name);
				free(name);
				name = u;
			}
			char *to = name ? ifme_path_join(dir, name) : NULL;
			if (!to || !ifme_copy_file(from, to)) {
				enc_fail(E, "attachment %zu: cannot read %s", n + 1, from ? from : file);
				free(from); free(name); free(to);
				r = IFME_TASK_ERROR;
				goto out;
			}
			E->att[E->att_count].name = name;
			E->att[E->att_count].mime = ifme_strdup(mime_for(name, ifme_jstr(it, "mime_type")));
			E->att_count++;
			free(from);
			free(to);
			continue;
		}

		src s;
		if (!src_open(E, it, "attachment", n, &s)) {
			r = IFME_TASK_ERROR;
			goto out;
		}
		int id = ifme_jint(it, "stream", s.media->attachment_count ? s.media->attachment[0].id : -1);
		const ifme_stream_attachment *sa = NULL;
		for (size_t i = 0; i < s.media->attachment_count; i++)
			if (s.media->attachment[i].id == id)
				sa = &s.media->attachment[i];
		if (!sa) {
			enc_fail(E, "attachment %zu: %s has no attachment stream %d", n + 1, ifme_basename(s.path), id);
			src_close(&s);
			r = IFME_TASK_ERROR;
			goto out;
		}

		const char *want = ifme_jstr(it, "filename");
		char *name = safe_name(want && *want ? want : sa->filename, n);
		if (name_taken(E, name, main_names, main_n)) {
			char *u = ifme_strdupf("%zu_%s", n, name);
			free(name);
			name = u;
		}
		const char *mime = mime_for(name, ifme_jstr(it, "mime_type") ? ifme_jstr(it, "mime_type") : sa->mime_type);

		if (!s.own_media) {
			/* From the job's input: dumped together with the metadata below. */
			main_names[main_n] = name;
			main_ids[main_n] = id;
			main_mimes[main_n] = ifme_strdup(mime);
			main_n++;
		} else {
			ifme_argv a = { 0 };
			ifme_argv_push(&a, E->ffmpeg);
			ifme_argv_split(&a, "-hide_banner -v error");
			ifme_argv_pushf(&a, "-dump_attachment:%d", id);
			ifme_argv_pushf(&a, "attachment/%s", name);
			ifme_argv_push(&a, "-i");
			ifme_argv_push(&a, s.path);
			ifme_argv_split(&a, "-f ffmetadata -y attachment.ini");
			enc_stage(E, "Attachment %zu: %s", n + 1, name);
			r = enc_run1(E, &a, 0);
			ifme_argv_free(&a);
			char *junk = temp_path(E, "attachment.ini");
			ifme_delete_file(junk);
			free(junk);
			if (r != IFME_TASK_DONE) {
				free(name);
				src_close(&s);
				goto out;
			}
			E->att[E->att_count].name = name;
			E->att[E->att_count].mime = ifme_strdup(mime);
			E->att_count++;
		}
		src_close(&s);
	}

	/* Global tags and chapters (cut to the trim), and the input's attachments. */
	if (want_meta || want_ch || main_n) {
		ifme_argv a = { 0 };
		ifme_argv_push(&a, E->ffmpeg);
		ifme_argv_split(&a, "-hide_banner -v error");
		for (size_t i = 0; i < main_n; i++) {
			ifme_argv_pushf(&a, "-dump_attachment:%d", main_ids[i]);
			ifme_argv_pushf(&a, "attachment/%s", main_names[i]);
		}
		ifme_argv_push(&a, "-i");
		ifme_argv_push(&a, E->input);
		enc_push_trim(E, &a);
		ifme_argv_split(&a, "-f ffmetadata -y metadata.ini");
		enc_stage(E, main_n ? "Extracting metadata and %zu attachment%s" : "Extracting metadata",
		          main_n, main_n == 1 ? "" : "s");
		r = enc_run1(E, &a, 0);
		ifme_argv_free(&a);
		if (r != IFME_TASK_DONE)
			goto out;

		char *ini = temp_path(E, "metadata.ini");
		E->ini = (want_meta || want_ch) && ifme_file_size(ini) > 0;

		/* FFmpeg cuts chapters to the trim but keeps the ones it cut to
		 * nothing (START == END). */
		size_t len = 0, dropped = 0;
		char *text = E->ini && E->trim_seconds > 0 ? ifme_read_file(ini, &len) : NULL;
		char *kept = text ? ifme_chapters_drop_empty(text, len, &dropped) : NULL;
		if (kept && dropped)
			ifme_write_file(ini, kept, strlen(kept));
		free(kept);
		free(text);
		free(ini);

		for (size_t i = 0; i < main_n; i++) {
			char *rel = ifme_path_join("attachment", main_names[i]);
			char *path = rel ? temp_path(E, rel) : NULL;
			if (path && ifme_file_size(path) >= 0) {
				E->att[E->att_count].name = main_names[i];
				E->att[E->att_count].mime = main_mimes[i];
				E->att_count++;
				main_names[i] = main_mimes[i] = NULL;
			} else {
				logf_(E, "attachment %s: not written by ffmpeg, left out", main_names[i]);
			}
			free(path);
			free(rel);
		}
	}

out:
	for (size_t i = 0; i < main_n; i++) {
		free(main_names ? main_names[i] : NULL);
		free(main_mimes ? main_mimes[i] : NULL);
	}
	free(main_names);
	free(main_mimes);
	free(main_ids);
	free(dir);
	return r;
}

/* ---- cover art ---------------------------------------------------------------------- */

static ifme_task_result do_cover(enc *E)
{
	const cJSON *c = ifme_jget(E->job, "cover");
	if (!cJSON_IsObject(c) || !ifme_jbool(c, "enable", true))
		return IFME_TASK_DONE;

	const char *ext = enc_output_ext(E->output);
	if (!ext_in(ext, COVER)) {
		logf_(E, "cover art: a .%s file cannot hold it, left out", ext);
		return IFME_TASK_DONE;
	}

	src s;
	if (!src_open(E, c, "cover", 0, &s))
		return IFME_TASK_ERROR;

	/* Default: the input's attached picture, else (an image file) its picture. */
	int id = -1;
	for (size_t i = 0; i < s.media->video_count && id < 0; i++)
		if (s.media->video[i].attached_pic)
			id = s.media->video[i].id;
	if (id < 0 && s.own_media && s.media->video_count)
		id = s.media->video[0].id;
	id = ifme_jint(c, "stream", id);

	const ifme_stream_video *v = NULL;
	for (size_t i = 0; i < s.media->video_count; i++)
		if (s.media->video[i].id == id)
			v = &s.media->video[i];
	if (!v) {
		enc_fail(E, "cover: %s has no picture stream %d", ifme_basename(s.path), id);
		src_close(&s);
		return IFME_TASK_ERROR;
	}

	/* JPEG and PNG are kept as they are; anything else becomes PNG. */
	bool jpeg = strcmp(v->codec, "mjpeg") == 0, png = strcmp(v->codec, "png") == 0;
	char *file = ifme_strdup(jpeg ? "cover.jpg" : "cover.png");

	ifme_argv a = { 0 };
	ifme_argv_push(&a, E->ffmpeg);
	ifme_argv_split(&a, "-hide_banner -v error -i");
	ifme_argv_push(&a, s.path);
	ifme_argv_push(&a, "-map");
	ifme_argv_pushf(&a, "0:%d", id);
	ifme_argv_split(&a, "-frames:v 1 -c:v");
	ifme_argv_push(&a, jpeg || png ? "copy" : "png");
	ifme_argv_push(&a, "-y");
	ifme_argv_push(&a, file);
	enc_stage(E, "Cover art");
	ifme_task_result r = enc_run1(E, &a, 0);
	ifme_argv_free(&a);
	src_close(&s);

	if (r == IFME_TASK_DONE) {
		track *t = &E->subs[E->sub_count++];
		t->file = file;
		t->kind = 'c';
	} else {
		free(file);
	}
	return r;
}

/* ---- entry points ------------------------------------------------------------------ */

ifme_task_result enc_extract(enc *E)
{
	size_t n = 1;   /* the cover */
	const cJSON *it;
	cJSON_ArrayForEach(it, ifme_jget(E->job, "subtitles"))
		n++;
	E->subs = calloc(n, sizeof(track));
	if (!E->subs) {
		enc_fail(E, "out of memory");
		return IFME_TASK_ERROR;
	}

	ifme_task_result r = do_subtitles(E);
	if (r == IFME_TASK_DONE)
		r = do_attachments_and_metadata(E);
	if (r == IFME_TASK_DONE)
		r = do_cover(E);
	return r;
}

void enc_extras_free(enc *E)
{
	for (size_t i = 0; i < E->sub_count; i++) {
		free(E->subs[i].file);
		free(E->subs[i].lang);
	}
	free(E->subs);
	for (size_t i = 0; i < E->att_count; i++) {
		free(E->att[i].name);
		free(E->att[i].mime);
	}
	free(E->att);
	free(E->burn);
	E->subs = NULL;
	E->att = NULL;
	E->burn = NULL;
	E->sub_count = E->att_count = 0;
}

void enc_mux_extras(enc *E, ifme_argv *in, ifme_argv *opt, size_t first_input, size_t first_stream)
{
	const char *ext = enc_output_ext(E->output);
	bool iso = ext_in(ext, ISO_ALL), webm = ext_in(ext, WEBM), matroska = ext_in(ext, MATROSKA);
	size_t ni = first_input, ns = first_stream;
	bool any_sub = false;
	const char *mkv_cover = NULL;

	for (size_t i = 0; i < E->sub_count; i++) {
		const track *t = &E->subs[i];

		/* Matroska keeps cover art as an attachment named cover.jpg/.png
		 * (mkvmerge's convention); a mapped picture would become a video track. */
		if (t->kind == 'c' && matroska) {
			mkv_cover = t->file;
			continue;
		}

		ifme_argv_push(in, "-i");
		ifme_argv_push(in, t->file);
		ifme_argv_push(opt, "-map");
		ifme_argv_pushf(opt, "%zu:%c", ni, t->kind == 'c' ? 'v' : 's');

		if (t->kind == 'c') {
			bool jpg = strstr(t->file, ".jpg") != NULL;
			ifme_argv_pushf(opt, "-disposition:%zu", ns);
			ifme_argv_push(opt, "attached_pic");
			ifme_argv_pushf(opt, "-metadata:s:%zu", ns);
			ifme_argv_push(opt, jpg ? "filename=cover.jpg" : "filename=cover.png");
			ifme_argv_pushf(opt, "-metadata:s:%zu", ns);
			ifme_argv_push(opt, jpg ? "mimetype=image/jpeg" : "mimetype=image/png");
			ni++;
			ns++;
			continue;
		}

		any_sub = true;
		ifme_argv_pushf(opt, "-disposition:%zu", ns);
		ifme_argv_push(opt, t->is_default && t->forced ? "default+forced"
		                    : t->is_default ? "default" : t->forced ? "forced" : "0");
		ifme_argv_pushf(opt, "-metadata:s:%zu", ns);
		ifme_argv_pushf(opt, "language=%s", t->lang ? t->lang : "und");
		if (t->title && *t->title) {
			ifme_argv_pushf(opt, "-metadata:s:%zu", ns);
			ifme_argv_pushf(opt, "%s=%s", iso ? "handler_name" : "title", t->title);
		}
		ni++;
		ns++;
	}

	if (any_sub && (iso || webm)) {
		ifme_argv_push(opt, "-c:s");
		ifme_argv_push(opt, iso ? "mov_text" : "webvtt");
	}

	if (matroska) {
		for (size_t i = 0; i < E->att_count; i++) {
			ifme_argv_push(opt, "-attach");
			ifme_argv_pushf(opt, "attachment/%s", E->att[i].name);
			ifme_argv_pushf(opt, "-metadata:s:t:%zu", i);
			ifme_argv_pushf(opt, "filename=%s", E->att[i].name);
			ifme_argv_pushf(opt, "-metadata:s:t:%zu", i);
			ifme_argv_pushf(opt, "mimetype=%s", E->att[i].mime);
		}
		if (mkv_cover) {
			bool jpg = strstr(mkv_cover, ".jpg") != NULL;
			ifme_argv_push(opt, "-attach");
			ifme_argv_push(opt, mkv_cover);
			ifme_argv_pushf(opt, "-metadata:s:t:%zu", E->att_count);
			ifme_argv_push(opt, jpg ? "filename=cover.jpg" : "filename=cover.png");
			ifme_argv_pushf(opt, "-metadata:s:t:%zu", E->att_count);
			ifme_argv_push(opt, jpg ? "mimetype=image/jpeg" : "mimetype=image/png");
		}
	}

	/* Tags and chapters from metadata.ini; nothing by accident from the
	 * track files (neroAacEnc writes a chapter, copied tracks bring theirs). */
	bool want_meta = ifme_jbool(E->job, "metadata", true);
	bool want_ch = ifme_jbool(E->job, "chapters", true);
	if (E->ini) {
		ifme_argv_split(in, "-f ffmetadata -i metadata.ini");
		ifme_argv_push(opt, "-map_metadata");
		ifme_argv_pushf(opt, "%d", want_meta ? (int)ni : -1);
		ifme_argv_push(opt, "-map_chapters");
		ifme_argv_pushf(opt, "%d", want_ch ? (int)ni : -1);
	} else {
		ifme_argv_split(opt, "-map_metadata -1 -map_chapters -1");
	}
}

/* ---- job defaults ------------------------------------------------------------------- */

void enc_job_extras(cJSON *job, const ifme_media *m, const char *container,
                    bool has_video, bool video_copy)
{
	bool matroska = ext_in(container, MATROSKA), iso = ext_in(container, ISO_SUBS),
	     webm = ext_in(container, WEBM);

	/* As C#: subtitles and fonts only go with video. */
	if (has_video && m->subtitle_count) {
		/* The source's default subtitle (e.g. English of German + English);
		 * when it flags none, the first one, as C#. */
		bool flagged = false;
		for (size_t i = 0; i < m->subtitle_count; i++)
			flagged |= m->subtitle[i].is_default;
		cJSON *subs = cJSON_AddArrayToObject(job, "subtitles");
		for (size_t i = 0; i < m->subtitle_count; i++) {
			const ifme_stream_subtitle *s = &m->subtitle[i];
			cJSON *e = cJSON_CreateObject();
			cJSON_AddNumberToObject(e, "stream", s->id);
			cJSON_AddStringToObject(e, "language", s->language && *s->language ? s->language : "und");
			cJSON_AddStringToObject(e, "title", s->title ? s->title : "");
			cJSON_AddBoolToObject(e, "default", flagged ? s->is_default : i == 0);
			cJSON_AddBoolToObject(e, "forced", s->forced);
			cJSON *help = cJSON_AddObjectToObject(e, "_help");
			cJSON_AddStringToObject(help, "format", s->codec ? s->codec : "?");
			cJSON_AddItemToArray(subs, e);
		}

		/* C#: burn in when the container cannot carry subtitles; only text
		 * subtitles can be burned. */
		bool to_srt;
		bool text = is_text_ext(sub_file_ext(m->subtitle[0].codec, &to_srt));
		cJSON_AddBoolToObject(job, "burn_subtitle", !matroska && !iso && !webm && !video_copy && text);
	}

	if (has_video && m->attachment_count) {
		cJSON *atts = cJSON_AddArrayToObject(job, "attachments");
		for (size_t i = 0; i < m->attachment_count; i++) {
			const ifme_stream_attachment *a = &m->attachment[i];
			cJSON *e = cJSON_CreateObject();
			cJSON_AddNumberToObject(e, "stream", a->id);
			cJSON_AddStringToObject(e, "filename", a->filename);
			cJSON_AddStringToObject(e, "mime_type", a->mime_type);
			cJSON_AddItemToArray(atts, e);
		}
	}

	for (size_t i = 0; i < m->video_count; i++) {
		if (m->video[i].attached_pic && ext_in(container, COVER)) {
			cJSON *c = cJSON_AddObjectToObject(job, "cover");
			cJSON_AddNumberToObject(c, "stream", m->video[i].id);
			break;
		}
	}

	cJSON_AddBoolToObject(job, "metadata", true);
	cJSON_AddBoolToObject(job, "chapters", true);
	cJSON_AddStringToObject(job, "movflags", "");
}
