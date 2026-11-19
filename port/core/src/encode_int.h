/*
 * encode_int.h - state shared by the encoding job runner (encode.c) and the
 * subtitle / attachment / cover / metadata steps (extras.c). Internal only.
 */

#ifndef IFME_ENCODE_INT_H
#define IFME_ENCODE_INT_H

#include "internal.h"
#include "cJSON.h"

#define VIDEO_COPY_GUID "00000000-0000-0000-0000-000000000000"
#define AUDIO_COPY_GUID "aaaaaaaa-0000-0000-0000-000000000000"
#define LOG_KEEP 8

/* One stream file in the temp folder, written to the output by the mux. */
typedef struct track {
	char *file;          /* relative to E->temp */
	char kind;           /* 'v' video, 'a' audio, 's' subtitle, 'c' cover art */
	char *lang;
	const char *title;   /* from the job, may be NULL */
	bool is_default;     /* audio, subtitles: the "default" flag */
	bool forced;         /* subtitles: the "forced" flag */
} track;

/* A file in <temp>/attachment, attached to Matroska outputs. */
typedef struct attachment {
	char *name;          /* file name inside <temp>/attachment */
	char *mime;
} attachment;

typedef struct enc {
	ifme_ctx *ctx;
	ifme_control *control;
	const ifme_task_callbacks *cb;
	char *ffmpeg;
	char *input;               /* absolute */
	char *output;              /* absolute */
	char *temp;
	const cJSON *job;
	ifme_media *media;
	double trim_seconds;       /* 0: no trim */
	double trim_start;         /* seconds; with trim_seconds > 0 */
	char *err;
	size_t err_len;

	ifme_progress_parser parser;
	char recent[LOG_KEEP][256];   /* last log lines, for error messages */
	int recent_count, recent_pos;

	/* extras.c */
	track *subs;               /* subtitle tracks, then the cover (kind 'c') */
	size_t sub_count;
	attachment *att;
	size_t att_count;
	char *burn;                /* subtitle file to burn into the video, or NULL */
	bool fonts;                /* <temp>/attachment exists (fontsdir for burning) */
	bool ini;                  /* metadata.ini (global tags + chapters) exists */
} enc;

/* encode.c */
void enc_fail(enc *E, const char *fmt, ...);
void enc_stage(enc *E, const char *fmt, ...);
void enc_log(enc *E, const char *line);
ifme_task_result enc_run1(enc *E, ifme_argv *a, double total_sec);
void enc_push_trim(enc *E, ifme_argv *a);
const char *enc_output_ext(const char *path);
double enc_job_seconds(const enc *E);
double enc_track_seconds(const enc *E, const ifme_media *m);
bool enc_ext_is(const char *path, const char *const *exts);   /* NULL-terminated list */

/* extras.c */
ifme_task_result enc_extract(enc *E);   /* before video/audio: subs, fonts, cover, metadata */
void enc_extras_free(enc *E);
/* Mux arguments for E->subs / E->att / metadata.ini; `first_input` and
 * `first_stream` are the indexes the extras start at. */
void enc_mux_extras(enc *E, ifme_argv *inputs, ifme_argv *options,
                    size_t first_input, size_t first_stream);
/* Job defaults: "subtitles", "burn_subtitle", "attachments", "cover",
 * "metadata", "chapters", "movflags". */
void enc_job_extras(cJSON *job, const ifme_media *media, const char *container,
                    bool has_video, bool video_copy);

#endif
