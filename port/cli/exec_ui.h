#ifndef IFME_EXEC_UI_H
#define IFME_EXEC_UI_H

#include "ifme.h"

/* Run argv (commands separated by "|") with a live progress line and p/r/q
 * keys. Returns a process exit status for ifmecli. */
int run_exec(ifme_ctx *ctx, char **argv, int argc, long long total_frames);

#endif
