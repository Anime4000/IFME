#ifndef IFME_ENCODE_CMD_H
#define IFME_ENCODE_CMD_H

#include <stdbool.h>

#include "ifme.h"

typedef struct encode_opts {
	const char *input;
	const char *output;
	const char *video;          /* plugin GUID, part of its name, "copy", "none" */
	const char *audio;          /* same; NULL: a plugin that fits the container */
	const char *audio_quality, *audio_args, *audio_filter;
	int audio_mode, sample_rate, channels;  /* -1: not given */
	bool audio_copy;            /* try the source audio as it is first */
	bool video_copy;            /* try remuxing the source video first */
	const char *add_audio[8];   /* --add-audio FILE: audio tracks from other files */
	int add_audio_count;
	const char *add_subtitle[8];     /* --add-subtitle FILE (.srt, .ass, or any media) */
	int add_subtitle_count;
	const char *add_attachment[16];  /* --add-attachment FILE (fonts) */
	int add_attachment_count;
	const char *cover;          /* --cover IMAGE */
	const char *movflags;
	bool no_subtitles, no_attachments, no_cover, no_metadata, no_chapters, burn_subtitle;
	const char *preset, *tune, *value, *size, *fps;
	const char *trim_start, *trim_duration;
	const char *encoder_args, *filter, *temp_dir;
	const char *job_file;       /* --job FILE instead of building one */
	int mode, passes, bit_depth, chroma;   /* -1 / 0: not given */
	bool keep_temp, print_job;
} encode_opts;

int cmd_encode(ifme_ctx *ctx, const encode_opts *o);

#endif
