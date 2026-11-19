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

	job_text = ifme_job_new(ctx, o->input, o->video ? o->video : "x265", err, sizeof(err));
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

	cJSON *v = cJSON_GetArrayItem(cJSON_GetObjectItem(job, "video"), 0);

	set_string(job, "output", o->output);
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
