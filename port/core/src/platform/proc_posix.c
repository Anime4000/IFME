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

/* Read what is ready on *fd; close it and set -1 at end-of-file. */
static bool drain_fd(int *fd, ifme_capture *c)
{
	char buf[65536];
	ssize_t n = read(*fd, buf, sizeof(buf));

	if (n > 0) {
		ifme_capture_append(c, buf, (size_t)n);
		return true;
	}
	if (n == 0 || (errno != EINTR && errno != EAGAIN)) {
		close(*fd);
		*fd = -1;
	}
	return false;
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

	ifme_capture cap_out = { 0 }, cap_err = { 0 };
	cap_out.limit = cap_err.limit = opts->capture_limit;

	int null_in = open_cloexec("/dev/null", O_RDONLY);
	int null_out = open_cloexec("/dev/null", O_WRONLY);
	int out_pipe[2] = { -1, -1 }, err_pipe[2] = { -1, -1 };
	int log_fd = -1;

	pid_t *pids = calloc(count, sizeof(pid_t));
	int *status = calloc(count, sizeof(int));
	char **paths = calloc(count, sizeof(char *));
	pid_t pgid = 0;
	size_t started = 0;

	if (null_in < 0 || null_out < 0 || !pids || !status || !paths) {
		snprintf(res->message, sizeof(res->message), "cannot prepare pipeline: %s", strerror(errno));
		goto done;
	}

	if (opts->capture_stdout && !make_pipe(out_pipe)) {
		snprintf(res->message, sizeof(res->message), "cannot create pipe: %s", strerror(errno));
		goto done;
	}
	if (opts->capture_stderr) {
		if (!make_pipe(err_pipe)) {
			snprintf(res->message, sizeof(res->message), "cannot create pipe: %s", strerror(errno));
			goto done;
		}
	} else if (opts->stderr_path) {
		log_fd = open_cloexec(opts->stderr_path, O_WRONLY | O_CREAT | O_APPEND);
	}

	const int last_out = out_pipe[1] >= 0 ? out_pipe[1] : null_out;
	const int err_fd = err_pipe[1] >= 0 ? err_pipe[1] : log_fd >= 0 ? log_fd : null_out;

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
	if (out_pipe[1] >= 0) {
		close(out_pipe[1]);
		out_pipe[1] = -1;
	}
	if (err_pipe[1] >= 0) {
		close(err_pipe[1]);
		err_pipe[1] = -1;
	}

	uint64_t deadline = opts->timeout_ms > 0 ? ifme_now_ms() + (uint64_t)opts->timeout_ms : 0;
	size_t remaining = count;
	bool timed_out = false, stragglers_killed = false;

	for (;;) {
		struct pollfd pfd[2];
		int *which[2];
		nfds_t n = 0;

		if (out_pipe[0] >= 0) {
			pfd[n].fd = out_pipe[0];
			pfd[n].events = POLLIN;
			which[n++] = &out_pipe[0];
		}
		if (err_pipe[0] >= 0) {
			pfd[n].fd = err_pipe[0];
			pfd[n].events = POLLIN;
			which[n++] = &err_pipe[0];
		}

		/* Also serves as the 20 ms sleep when nothing is captured. */
		if (poll(n ? pfd : NULL, n, 20) > 0) {
			for (nfds_t k = 0; k < n; k++) {
				if (pfd[k].revents & (POLLIN | POLLHUP | POLLERR))
					drain_fd(which[k], which[k] == &out_pipe[0] ? &cap_out : &cap_err);
			}
		}

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
			if (out_pipe[0] < 0 && err_pipe[0] < 0)
				break;

			/* A grandchild may still hold our pipe open; the stages are done,
			 * so end the group to get end-of-file. Buffered data stays readable. */
			if (!stragglers_killed) {
				killpg(pgid, SIGKILL);
				stragglers_killed = true;
			}
		}

		if (deadline && !timed_out && ifme_now_ms() >= deadline) {
			killpg(pgid, SIGKILL);
			timed_out = true;
			snprintf(res->message, sizeof(res->message), "timed out after %d s", opts->timeout_ms / 1000);
		}
	}

	res->status = timed_out ? IFME_RUN_TIMEOUT : IFME_RUN_OK;
	{
		int st = status[count - 1];
		res->exit_code = WIFEXITED(st) ? WEXITSTATUS(st)
		               : WIFSIGNALED(st) ? 128 + WTERMSIG(st) : -1;
	}
	goto done;

kill:
	if (pgid > 0)
		killpg(pgid, SIGKILL);
	for (size_t i = 0; i < started; i++)
		if (pids[i] > 0)
			waitpid(pids[i], NULL, 0);

done:
	for (int k = 0; k < 2; k++) {
		if (out_pipe[k] >= 0)
			close(out_pipe[k]);
		if (err_pipe[k] >= 0)
			close(err_pipe[k]);
	}
	ifme_capture_finish(&cap_out, &res->out, &res->out_len);
	ifme_capture_finish(&cap_err, &res->err, &res->err_len);

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
