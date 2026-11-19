/*
 * encode_cmd.c - ifmecli --encode / --job
 *
 * Builds a job from ifme_job_new() (source + plugin defaults), applies the
 * command-line overrides, then runs it (or prints it with --print-job, so it
 * can be edited and run later with --job FILE).
 *
 * License: GPL-2.0
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "cJSON.h"
#include "encode_cmd.h"
#include "exec_ui.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

static char *read_text_file(const char *path)
{
#ifdef _WIN32
	/* UTF-8 path -> wide, so job files in any folder open. */
	int n = MultiByteToWideChar(CP_UTF8, 0, path, -1, NULL, 0);
	wchar_t *w = n > 0 ? malloc((size_t)n * sizeof(wchar_t)) : NULL;
	FILE *f = NULL;
	if (w && MultiByteToWideChar(CP_UTF8, 0, path, -1, w, n) > 0)
		f = _wfopen(w, L"rb");
	free(w);
#else
	FILE *f = fopen(path, "rb");
#endif
	if (!f)
		return NULL;

	size_t cap = 4096, len = 0;
	char *buf = malloc(cap);
	size_t got;
	while (buf && (got = fread(buf + len, 1, cap - len - 1, f)) > 0) {
		len += got;
		if (cap - len < 2) {
			char *grown = realloc(buf, cap *= 2);
			if (!grown) {
				free(buf);
				buf = NULL;
			}
			buf = grown;
		}
	}
	fclose(f);
	if (buf)
		buf[len] = '\0';
	return buf;
}

static void set_string(cJSON *o, const char *key, const char *value)
{
	if (!value)
		return;
	cJSON_DeleteItemFromObjectCaseSensitive(o, key);
	cJSON_AddStringToObject(o, key, value);
}

static void set_number(cJSON *o, const char *key, double value)
{
	cJSON_DeleteItemFromObjectCaseSensitive(o, key);
	cJSON_AddNumberToObject(o, key, value);
}

int cmd_encode(ifme_ctx *ctx, const encode_opts *o)
{
	char err[1024] = "";
	char *job_text = NULL;

	if (o->job_file) {
		job_text = read_text_file(o->job_file);
		if (!job_text) {
			fprintf(stderr, "ifmecli: cannot read job file %s\n", o->job_file);
			return 2;
		}
		if (o->print_job) {
			printf("%s\n", job_text);
			free(job_text);
			return 0;
		}
		int rc = run_encode(ctx, job_text);
		free(job_text);
		return rc;
	}

	job_text = ifme_job_new(ctx, o->input, o->output, o->video ? o->video : "x265", o->audio,
	                        err, sizeof(err));
	if (!job_text) {
		fprintf(stderr, "ifmecli: %s\n", err[0] ? err : "cannot create job");
		return 2;
	}

	cJSON *job = cJSON_Parse(job_text);
	ifme_free(job_text);
	if (!job) {
		fprintf(stderr, "ifmecli: internal error: job is not JSON\n");
		return 1;
	}

	/* --add-audio: the other file's audio tracks, with the same defaults
	 * (same output container, so the same audio plugin), then "input". */
	cJSON *audios = cJSON_GetObjectItem(job, "audio");
	for (int i = 0; i < o->add_audio_count; i++) {
		const cJSON *out = cJSON_GetObjectItem(job, "output");
		char *extra = ifme_job_new(ctx, o->add_audio[i], cJSON_IsString(out) ? out->valuestring : NULL,
		                           "none", o->audio, err, sizeof(err));
		cJSON *ej = extra ? cJSON_Parse(extra) : NULL;
		ifme_free(extra);
		if (!ej) {
			fprintf(stderr, "ifmecli: --add-audio %s: %s\n", o->add_audio[i], err[0] ? err : "no audio");
			cJSON_Delete(job);
			return 2;
		}
		const cJSON *src_in = cJSON_GetObjectItem(ej, "input");
		cJSON *e;
		cJSON_ArrayForEach(e, cJSON_GetObjectItem(ej, "audio"))
			set_string(e, "input", cJSON_IsString(src_in) ? src_in->valuestring : o->add_audio[i]);

		cJSON *list = cJSON_DetachItemFromObjectCaseSensitive(ej, "audio");
		while (list && list->child)
			cJSON_AddItemToArray(audios, cJSON_DetachItemViaPointer(list, list->child));
		cJSON_Delete(list);
		cJSON_Delete(ej);
	}

	/* Subtitles, attachments, cover art, tags and chapters. */
	if (o->no_subtitles) {
		cJSON_DeleteItemFromObjectCaseSensitive(job, "subtitles");
		cJSON_DeleteItemFromObjectCaseSensitive(job, "burn_subtitle");
	}
	if (o->no_attachments)
		cJSON_DeleteItemFromObjectCaseSensitive(job, "attachments");
	if (o->no_cover || o->cover)
		cJSON_DeleteItemFromObjectCaseSensitive(job, "cover");
	if (o->cover)
		cJSON_AddStringToObject(cJSON_AddObjectToObject(job, "cover"), "input", o->cover);

	for (int i = 0; i < o->add_subtitle_count; i++) {
		ifme_media *m = ifme_probe(ctx, o->add_subtitle[i], NULL, err, sizeof(err));
		if (!m || m->subtitle_count == 0) {
			fprintf(stderr, "ifmecli: --add-subtitle %s: %s\n", o->add_subtitle[i],
			        m ? "no subtitle stream in it" : err);
			ifme_media_free(m);
			cJSON_Delete(job);
			return 2;
		}
		cJSON *subs = cJSON_GetObjectItem(job, "subtitles");
		if (!subs)
			subs = cJSON_AddArrayToObject(job, "subtitles");
		for (size_t k = 0; k < m->subtitle_count; k++) {
			cJSON *e = cJSON_CreateObject();
			cJSON_AddStringToObject(e, "input", m->path);
			cJSON_AddNumberToObject(e, "stream", m->subtitle[k].id);
			cJSON_AddStringToObject(e, "language", m->subtitle[k].language);
			cJSON_AddStringToObject(e, "title", m->subtitle[k].title);
			cJSON_AddBoolToObject(e, "default", cJSON_GetArraySize(subs) == 0);
			cJSON_AddItemToArray(subs, e);
		}
		ifme_media_free(m);
	}

	for (int i = 0; i < o->add_attachment_count; i++) {
		cJSON *atts = cJSON_GetObjectItem(job, "attachments");
		if (!atts)
			atts = cJSON_AddArrayToObject(job, "attachments");
		cJSON *e = cJSON_CreateObject();
		cJSON_AddStringToObject(e, "file", o->add_attachment[i]);
		cJSON_AddItemToArray(atts, e);
	}

	if (o->burn_subtitle) {
		cJSON_DeleteItemFromObjectCaseSensitive(job, "burn_subtitle");
		cJSON_AddBoolToObject(job, "burn_subtitle", true);
	}
	if (o->no_metadata) {
		cJSON_DeleteItemFromObjectCaseSensitive(job, "metadata");
		cJSON_AddBoolToObject(job, "metadata", false);
	}
	if (o->no_chapters) {
		cJSON_DeleteItemFromObjectCaseSensitive(job, "chapters");
		cJSON_AddBoolToObject(job, "chapters", false);
	}
	set_string(job, "movflags", o->movflags);

	/* Video options apply to the first video track (NULL when there is none:
	 * cJSON ignores edits of NULL). Audio options apply to every audio track. */
	cJSON *v = cJSON_GetArrayItem(cJSON_GetObjectItem(job, "video"), 0);

	cJSON *a;
	cJSON_ArrayForEach(a, cJSON_GetObjectItem(job, "audio")) {
		const cJSON *plugin = cJSON_GetObjectItem(a, "plugin");
		if (cJSON_IsString(plugin) && strcmp(plugin->valuestring, "copy") == 0)
			continue;
		if (o->audio_mode >= 0) {
			set_number(a, "mode", o->audio_mode);
			cJSON_DeleteItemFromObjectCaseSensitive(a, "quality");   /* that mode's default */
		}
		set_string(a, "quality", o->audio_quality);
		if (o->sample_rate >= 0)
			set_number(a, "sample_rate", o->sample_rate);
		if (o->channels >= 0)
			set_number(a, "channels", o->channels);
		if (o->audio_copy) {
			cJSON_DeleteItemFromObjectCaseSensitive(a, "copy");
			cJSON_AddBoolToObject(a, "copy", true);
		}
		set_string(a, "encoder_args", o->audio_args);
		set_string(a, "filter", o->audio_filter);
	}

	set_string(job, "temp_dir", o->temp_dir);
	if (o->keep_temp) {
		cJSON_DeleteItemFromObjectCaseSensitive(job, "keep_temp");
		cJSON_AddBoolToObject(job, "keep_temp", true);
	}

	if (o->trim_start || o->trim_duration) {
		cJSON *trim = cJSON_AddObjectToObject(job, "trim");
		cJSON_AddStringToObject(trim, "start", o->trim_start ? o->trim_start : "0");
		cJSON_AddStringToObject(trim, "duration", o->trim_duration ? o->trim_duration : "");
	}

	set_string(v, "preset", o->preset);
	set_string(v, "tune", o->tune);
	set_string(v, "encoder_args", o->encoder_args);
	set_string(v, "filter", o->filter);
	set_string(v, "fps", o->fps);
	if (o->mode >= 0) {
		set_number(v, "mode", o->mode);
		/* Another mode has its own value range and pass count: drop the
		 * defaults of mode 0 so the encoder uses the new mode's defaults. */
		cJSON_DeleteItemFromObjectCaseSensitive(v, "value");
		cJSON_DeleteItemFromObjectCaseSensitive(v, "passes");
	}
	if (o->value)
		set_number(v, "value", strtod(o->value, NULL));
	if (o->passes > 0)
		set_number(v, "passes", o->passes);
	if (o->bit_depth > 0)
		set_number(v, "bit_depth", o->bit_depth);
	if (o->chroma > 0)
		set_number(v, "chroma", o->chroma);
	if (o->video_copy && v) {
		cJSON_DeleteItemFromObjectCaseSensitive(v, "copy");
		cJSON_AddBoolToObject(v, "copy", true);
	}
	if (o->size) {
		int w = 0, h = 0;
		if (sscanf(o->size, "%dx%d", &w, &h) != 2 || w <= 0 || h <= 0) {
			fprintf(stderr, "ifmecli: --size wants WIDTHxHEIGHT, e.g. 1280x720\n");
			cJSON_Delete(job);
			return 2;
		}
		set_number(v, "width", w);
		set_number(v, "height", h);
	}

	char *final_json = cJSON_Print(job);
	cJSON_Delete(job);
	if (!final_json)
		return 1;

	int rc = 0;
	if (o->print_job)
		printf("%s\n", final_json);
	else
		rc = run_encode(ctx, final_json);

	cJSON_free(final_json);
	return rc;
}
