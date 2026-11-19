#ifndef IFME_ENCODE_CMD_H
#define IFME_ENCODE_CMD_H

#include <stdbool.h>

#include "ifme.h"

typedef struct encode_opts {
	const char *input;
	const char *output;
	const char *video;          /* plugin GUID or part of its name */
	const char *preset, *tune, *value, *size, *fps;
	const char *trim_start, *trim_duration;
	const char *encoder_args, *filter, *temp_dir;
	const char *job_file;       /* --job FILE instead of building one */
	int mode, passes, bit_depth, chroma;   /* -1 / 0: not given */
	bool keep_temp, print_job;
} encode_opts;

int cmd_encode(ifme_ctx *ctx, const encode_opts *o);

#endif
