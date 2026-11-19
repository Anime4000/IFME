/*
 * task.c - run a pipeline with live progress and pause/resume/stop
 *
 * Port of ProcessManager.cs (C#). The control functions only set atomic
 * flags; the runner applies them, so they are safe from any thread and from
 * signal / console-control handlers.
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"

struct ifme_task {
	ifme_control control;
};

ifme_task *ifme_task_create(void)
{
	ifme_task *t = calloc(1, sizeof(*t));
	if (t) {
		atomic_init(&t->control.want_pause, 0);
		atomic_init(&t->control.want_stop, 0);
		atomic_init(&t->control.paused, 0);
	}
	return t;
}

void ifme_task_destroy(ifme_task *task)
{
	free(task);
}

void ifme_task_pause(ifme_task *task)
{
	if (task)
		atomic_store(&task->control.want_pause, 1);
}

void ifme_task_resume(ifme_task *task)
{
	if (task)
		atomic_store(&task->control.want_pause, 0);
}

void ifme_task_stop(ifme_task *task)
{
	if (task)
		atomic_store(&task->control.want_stop, 1);
}

bool ifme_task_is_paused(const ifme_task *task)
{
	return task && atomic_load(&((ifme_task *)task)->control.paused) != 0;
}

typedef struct exec_state {
	const ifme_task_callbacks *cb;
	ifme_progress_parser parser;
} exec_state;

static void on_line(void *user, const char *line)
{
	exec_state *s = user;

	/* UIs get text without ANSI colours (SVT-AV1) and never blank lines
	 * (x265 erases its progress line by printing spaces). */
	char *clean = ifme_clean_line(line);
	if (!clean || !*clean) {
		free(clean);
		return;
	}

	ifme_progress p;
	int kind = ifme_progress_feed(&s->parser, clean, &p);

	if (s->cb && s->cb->on_output)
		s->cb->on_output(s->cb->user, clean, kind != IFME_LINE_LOG);
	if (kind == IFME_LINE_PROGRESS && s->cb && s->cb->on_progress)
		s->cb->on_progress(s->cb->user, &p);
	free(clean);
}

static void on_tick(void *user)
{
	exec_state *s = user;
	if (s->cb && s->cb->on_tick)
		s->cb->on_tick(s->cb->user);
}

ifme_task_result ifme_task_exec(ifme_task *task, ifme_ctx *ctx,
                                const char *const *argv, size_t argc,
                                const char *cwd, long long total_frames,
                                const ifme_task_callbacks *cb,
                                int *exit_code, char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';
	if (exit_code)
		*exit_code = -1;

	if (!task || !argv || argc == 0) {
		if (err && err_len)
			snprintf(err, err_len, "nothing to run");
		return IFME_TASK_ERROR;
	}

	/* Split on "|" into stages. */
	size_t stage_count = 1;
	for (size_t i = 0; i < argc; i++)
		stage_count += argv[i] && strcmp(argv[i], "|") == 0;

	ifme_argv *stages = calloc(stage_count, sizeof(ifme_argv));
	if (!stages)
		return IFME_TASK_ERROR;

	size_t si = 0;
	ifme_task_result result = IFME_TASK_ERROR;

	for (size_t i = 0; i < argc; i++) {
		const char *a = argv[i] ? argv[i] : "";

		if (strcmp(a, "|") == 0) {
			si++;
			continue;
		}

		/* First word of a stage: "ffmpeg"/"ffprobe" may be the bundled copy. */
		if (stages[si].n == 0 && ctx &&
		    (strcmp(a, "ffmpeg") == 0 || strcmp(a, "ffprobe") == 0)) {
			char *tool = ifme_find_ff_tool(ctx, a);
			ifme_argv_push(&stages[si], tool ? tool : a);
			free(tool);
		} else {
			ifme_argv_push(&stages[si], a);
		}
	}

	for (size_t i = 0; i < stage_count; i++) {
		if (stages[i].n == 0) {
			if (err && err_len)
				snprintf(err, err_len, "empty command around \"|\"");
			goto out;
		}
	}

	exec_state state;
	state.cb = cb;
	ifme_progress_init(&state.parser, total_frames);

	ifme_run_opts opts;
	memset(&opts, 0, sizeof(opts));
	opts.cwd = cwd;
	opts.on_line = on_line;
	opts.on_tick = on_tick;
	opts.user = &state;
	opts.control = &task->control;

	ifme_run_result run;
	int rc = ifme_run(stages, stage_count, &opts, &run);

	if (exit_code)
		*exit_code = run.exit_code;

	switch (rc) {
	case IFME_RUN_OK:
		result = run.exit_code == 0 ? IFME_TASK_DONE : IFME_TASK_FAILED;
		if (result == IFME_TASK_FAILED && err && err_len)
			snprintf(err, err_len, "exit code %d", run.exit_code);
		break;
	case IFME_RUN_STOPPED:
		result = IFME_TASK_STOPPED;
		if (err && err_len)
			snprintf(err, err_len, "stopped");
		break;
	default:
		result = IFME_TASK_ERROR;
		if (err && err_len)
			snprintf(err, err_len, "%s", run.message);
		break;
	}

	ifme_run_result_free(&run);

out:
	for (size_t i = 0; i < stage_count; i++)
		ifme_argv_free(&stages[i]);
	free(stages);
	return result;
}
