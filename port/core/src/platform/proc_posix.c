/*
 * proc_posix.c - run a pipeline of programs without /bin/sh (Linux, macOS)
 *
 *  - fork + execv with an argument list: no shell, no quoting, no globbing.
 *  - All stages share one process group, so killpg() stops, continues or
 *    kills the whole pipeline at once (used later for pause/resume).
 *  - Every descriptor is opened close-on-exec; children only get the three
 *    standard streams, so the encoder sees end-of-file when ffmpeg finishes.
 *  - Between fork and exec the child only calls async-signal-safe functions,
 *    which keeps this safe inside the multithreaded Qt GUI.
 */

#if !defined(_WIN32)

#if !defined(_GNU_SOURCE)
#define _GNU_SOURCE
#endif

#include <errno.h>
#include <fcntl.h>
#include <poll.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <time.h>
#include <unistd.h>

#include "../internal.h"

static int open_cloexec(const char *path, int flags)
{
	return open(path, flags | O_CLOEXEC, 0644);
}

static bool make_pipe(int fds[2])
{
#if defined(__linux__)
	return pipe2(fds, O_CLOEXEC) == 0;
#else
	if (pipe(fds) != 0)
		return false;
	fcntl(fds[0], F_SETFD, FD_CLOEXEC);
	fcntl(fds[1], F_SETFD, FD_CLOEXEC);
	return true;
#endif
}

/* Child side: put `fd` on `target`, clearing close-on-exec even if equal. */
static void move_fd(int fd, int target)
{
	if (fd == target)
		fcntl(fd, F_SETFD, 0);
	else
		dup2(fd, target);
}

typedef struct stream_reader {
	int fd;                      /* -1 once at end-of-file */
	ifme_capture *capture;       /* NULL: don't keep the bytes */
	ifme_linebuf *lines;         /* NULL: don't split into lines */
	const ifme_run_opts *opts;
} stream_reader;

/* Read what is ready; close the descriptor at end-of-file. */
static void drain_fd(stream_reader *r)
{
	char buf[65536];
	ssize_t n = read(r->fd, buf, sizeof(buf));

	if (n > 0) {
		if (r->capture)
			ifme_capture_append(r->capture, buf, (size_t)n);
		if (r->lines)
			ifme_linebuf_feed(r->lines, buf, (size_t)n, r->opts->on_line, r->opts->user);
		return;
	}
	if (n == 0 || (errno != EINTR && errno != EAGAIN)) {
		close(r->fd);
		r->fd = -1;
	}
}

/* Apply pause/stop requests, the timeout and the tick callback. */
static void apply_control(pid_t pgid, const ifme_run_opts *opts, ifme_run_state *st,
                          char *message, size_t message_len)
{
	ifme_control *c = opts->control;
	uint64_t now = ifme_now_ms();

	if (c && !st->stopped && atomic_load(&c->want_stop)) {
		killpg(pgid, SIGKILL);       /* works on stopped processes too */
		st->stopped = true;
		snprintf(message, message_len, "stopped");
	}

	if (c && !st->stopped) {
		bool want = atomic_load(&c->want_pause) != 0;
		if (want != st->paused) {
			killpg(pgid, want ? SIGSTOP : SIGCONT);
			st->paused = want;
			atomic_store(&c->paused, want ? 1 : 0);

			if (want) {
				st->paused_since = now;
			} else {
				if (st->deadline)
					st->deadline += now - st->paused_since;   /* pause doesn't count */
				st->paused_since = 0;
			}
		}
	}

	if (st->deadline && !st->paused && !st->stopped && !st->timed_out && now >= st->deadline) {
		killpg(pgid, SIGKILL);
		st->timed_out = true;
		snprintf(message, message_len, "timed out after %d s", opts->timeout_ms / 1000);
	}

	if (opts->on_tick && now - st->last_tick >= 50) {
		st->last_tick = now;
		opts->on_tick(opts->user);
	}
}

int ifme_run(const ifme_argv *stages, size_t count, const ifme_run_opts *opts,
             ifme_run_result *res)
{
	static const ifme_run_opts defaults;
	if (!opts)
		opts = &defaults;

	memset(res, 0, sizeof(*res));
	res->status = IFME_RUN_START_FAILED;
	res->exit_code = -1;

	if (count == 0) {
		snprintf(res->message, sizeof(res->message), "nothing to run");
		return res->status;
	}

	if (opts->control && atomic_load(&opts->control->want_stop)) {
		res->status = IFME_RUN_STOPPED;
		snprintf(res->message, sizeof(res->message), "stopped");
		return res->status;
	}

	ifme_capture cap_out = { 0 }, cap_err = { 0 };
	cap_out.limit = cap_err.limit = opts->capture_limit;
	ifme_linebuf *lines = opts->on_line ? calloc(1, sizeof(ifme_linebuf)) : NULL;

	stream_reader out_r = { -1, &cap_out, NULL, opts };
	stream_reader err_r = { -1, opts->capture_stderr ? &cap_err : NULL, lines, opts };
	int out_write = -1, err_write = -1;

	int null_in = open_cloexec("/dev/null", O_RDONLY);
	int null_out = open_cloexec("/dev/null", O_WRONLY);
	int log_fd = -1;

	pid_t *pids = calloc(count, sizeof(pid_t));
	int *status = calloc(count, sizeof(int));
	char **paths = calloc(count, sizeof(char *));
	pid_t pgid = 0;
	size_t started = 0;
	ifme_run_state st;
	memset(&st, 0, sizeof(st));

	if (null_in < 0 || null_out < 0 || !pids || !status || !paths || (opts->on_line && !lines)) {
		snprintf(res->message, sizeof(res->message), "cannot prepare pipeline: %s", strerror(errno));
		goto done;
	}

	if (opts->capture_stdout) {
		int fds[2];
		if (!make_pipe(fds)) {
			snprintf(res->message, sizeof(res->message), "cannot create pipe: %s", strerror(errno));
			goto done;
		}
		out_r.fd = fds[0];
		out_write = fds[1];
	}
	if (opts->capture_stderr || opts->on_line) {
		int fds[2];
		if (!make_pipe(fds)) {
			snprintf(res->message, sizeof(res->message), "cannot create pipe: %s", strerror(errno));
			goto done;
		}
		err_r.fd = fds[0];
		err_write = fds[1];
	} else if (opts->stderr_path) {
		log_fd = open_cloexec(opts->stderr_path, O_WRONLY | O_CREAT | O_APPEND);
	}

	const int last_out = out_write >= 0 ? out_write : null_out;
	const int err_fd = err_write >= 0 ? err_write : log_fd >= 0 ? log_fd : null_out;

	/* Resolve every program before forking: execv needs a path and PATH lookup
	 * allocates, which the child must not do. */
	for (size_t i = 0; i < count; i++) {
		const char *prog = stages[i].v[0];
		paths[i] = strchr(prog, '/') ? ifme_strdup(prog) : ifme_find_in_path(prog);
		if (!paths[i]) {
			snprintf(res->message, sizeof(res->message), "cannot start %s: program not found", prog);
			goto done;
		}
	}

	const char *cwd = opts->cwd;
	int prev_read = null_in;

	for (size_t i = 0; i < count; i++) {
		int fds[2] = { -1, -1 };
		int out = last_out;

		if (i + 1 < count) {
			if (!make_pipe(fds)) {
				snprintf(res->message, sizeof(res->message), "cannot create pipe: %s", strerror(errno));
				if (prev_read != null_in)
					close(prev_read);
				goto kill;
			}
			out = fds[1];
		}

		/* Prepared before fork so the child only touches ready-made memory. */
		static const char exec_failed[] = "ifme: cannot execute ";
		const char *path = paths[i];
		size_t path_len = strlen(path);

		pid_t pid = fork();
		if (pid == 0) {
			setpgid(0, pgid);
			if (cwd && *cwd && chdir(cwd) != 0)
				_exit(126);

			move_fd(prev_read, STDIN_FILENO);
			move_fd(out, STDOUT_FILENO);
			move_fd(err_fd, STDERR_FILENO);

			signal(SIGPIPE, SIG_DFL);
			execv(path, stages[i].v);

			ssize_t ignored = write(STDERR_FILENO, exec_failed, sizeof(exec_failed) - 1);
			ignored = write(STDERR_FILENO, path, path_len);
			ignored = write(STDERR_FILENO, "\n", 1);
			(void)ignored;
			_exit(127);
		}

		int fork_errno = errno;

		if (prev_read != null_in)
			close(prev_read);
		if (i + 1 < count)
			close(out);

		if (pid < 0) {
			snprintf(res->message, sizeof(res->message), "cannot start %s: %s",
			         stages[i].v[0], strerror(fork_errno));
			if (fds[0] >= 0)
				close(fds[0]);
			goto kill;
		}

		/* Set the group in the parent too, so it holds whichever side runs first. */
		if (pgid == 0)
			pgid = pid;
		setpgid(pid, pgid);

		pids[i] = pid;
		started++;
		prev_read = fds[0];
	}

	/* Every child has its copy now; keeping ours would hide end-of-file. */
	if (out_write >= 0) {
		close(out_write);
		out_write = -1;
	}
	if (err_write >= 0) {
		close(err_write);
		err_write = -1;
	}

	st.deadline = opts->timeout_ms > 0 ? ifme_now_ms() + (uint64_t)opts->timeout_ms : 0;
	size_t remaining = count;
	bool stragglers_killed = false;

	for (;;) {
		struct pollfd pfd[2];
		stream_reader *which[2];
		nfds_t n = 0;

		if (out_r.fd >= 0) {
			pfd[n].fd = out_r.fd;
			pfd[n].events = POLLIN;
			which[n++] = &out_r;
		}
		if (err_r.fd >= 0) {
			pfd[n].fd = err_r.fd;
			pfd[n].events = POLLIN;
			which[n++] = &err_r;
		}

		/* Also serves as the 20 ms sleep when nothing is streamed. */
		if (poll(n ? pfd : NULL, n, 20) > 0) {
			for (nfds_t k = 0; k < n; k++)
				if (pfd[k].revents & (POLLIN | POLLHUP | POLLERR))
					drain_fd(which[k]);
		}

		apply_control(pgid, opts, &st, res->message, sizeof(res->message));

		for (size_t i = 0; i < count; i++) {
			if (pids[i] <= 0)
				continue;

			pid_t r = waitpid(pids[i], &status[i], WNOHANG);
			if (r == pids[i] || (r < 0 && errno == ECHILD)) {
				pids[i] = -pids[i];    /* mark reaped, keep the value */
				remaining--;
			}
		}

		if (remaining == 0) {
			if (out_r.fd < 0 && err_r.fd < 0)
				break;

			/* A grandchild may still hold our pipe open; the stages are done,
			 * so end the group to get end-of-file. Buffered data stays readable. */
			if (!stragglers_killed) {
				killpg(pgid, SIGKILL);
				stragglers_killed = true;
			}
		}
	}

	if (lines)
		ifme_linebuf_flush(lines, opts->on_line, opts->user);

	res->status = st.stopped ? IFME_RUN_STOPPED : st.timed_out ? IFME_RUN_TIMEOUT : IFME_RUN_OK;
	{
		int s = status[count - 1];
		res->exit_code = WIFEXITED(s) ? WEXITSTATUS(s)
		               : WIFSIGNALED(s) ? 128 + WTERMSIG(s) : -1;
	}
	goto done;

kill:
	if (pgid > 0)
		killpg(pgid, SIGKILL);
	for (size_t i = 0; i < started; i++)
		if (pids[i] > 0)
			waitpid(pids[i], NULL, 0);

done:
	if (opts->control)
		atomic_store(&opts->control->paused, 0);
	if (out_r.fd >= 0)
		close(out_r.fd);
	if (err_r.fd >= 0)
		close(err_r.fd);
	if (out_write >= 0)
		close(out_write);
	if (err_write >= 0)
		close(err_write);

	ifme_capture_finish(&cap_out, &res->out, &res->out_len);
	ifme_capture_finish(&cap_err, &res->err, &res->err_len);
	free(lines);

	for (size_t i = 0; paths && i < count; i++)
		free(paths[i]);
	free(paths);
	free(status);
	free(pids);
	if (log_fd >= 0)
		close(log_fd);
	if (null_in >= 0)
		close(null_in);
	if (null_out >= 0)
		close(null_out);
	return res->status;
}

#endif /* !_WIN32 */
