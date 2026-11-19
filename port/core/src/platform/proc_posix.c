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

static void sleep_ms(long ms)
{
	struct timespec ts = { ms / 1000, (ms % 1000) * 1000000L };
	while (nanosleep(&ts, &ts) == -1 && errno == EINTR)
		;
}

int ifme_run_pipeline(const ifme_argv *stages, size_t count, const char *cwd,
                      const char *log_path, int timeout_ms, int *exit_code,
                      char *err, size_t err_len)
{
	if (err && err_len)
		err[0] = '\0';
	if (exit_code)
		*exit_code = -1;
	if (count == 0)
		return IFME_RUN_START_FAILED;

	int rc = IFME_RUN_START_FAILED;
	int null_in = open_cloexec("/dev/null", O_RDONLY);
	int null_out = open_cloexec("/dev/null", O_WRONLY);
	int log_fd = log_path ? open_cloexec(log_path, O_WRONLY | O_CREAT | O_APPEND) : -1;
	int err_fd = log_fd >= 0 ? log_fd : null_out;

	pid_t *pids = calloc(count, sizeof(pid_t));
	int *status = calloc(count, sizeof(int));
	char **paths = calloc(count, sizeof(char *));
	pid_t pgid = 0;
	size_t started = 0;

	if (null_in < 0 || null_out < 0 || !pids || !status || !paths) {
		snprintf(err, err_len, "cannot prepare pipeline: %s", strerror(errno));
		goto done;
	}

	/* Resolve every program before forking: execv needs a path and PATH lookup
	 * allocates, which the child must not do. */
	for (size_t i = 0; i < count; i++) {
		const char *prog = stages[i].v[0];
		paths[i] = strchr(prog, '/') ? ifme_strdup(prog) : ifme_find_in_path(prog);
		if (!paths[i]) {
			snprintf(err, err_len, "cannot start %s: program not found", prog);
			goto done;
		}
	}

	int prev_read = null_in;

	for (size_t i = 0; i < count; i++) {
		int fds[2] = { -1, -1 };
		int out = null_out;

		if (i + 1 < count) {
			if (!make_pipe(fds)) {
				snprintf(err, err_len, "cannot create pipe: %s", strerror(errno));
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
		if (out != null_out)
			close(out);

		if (pid < 0) {
			snprintf(err, err_len, "cannot start %s: %s", stages[i].v[0], strerror(fork_errno));
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

	/* Wait for every stage, with an optional deadline. */
	uint64_t deadline = timeout_ms > 0 ? ifme_now_ms() + (uint64_t)timeout_ms : 0;
	size_t remaining = count;
	bool timed_out = false;

	while (remaining > 0) {
		for (size_t i = 0; i < count; i++) {
			if (pids[i] <= 0)
				continue;

			pid_t r = waitpid(pids[i], &status[i], WNOHANG);
			if (r == pids[i] || (r < 0 && errno == ECHILD)) {
				pids[i] = -pids[i];    /* mark reaped, keep the value */
				remaining--;
			}
		}

		if (remaining == 0)
			break;

		if (!timed_out && deadline && ifme_now_ms() >= deadline) {
			killpg(pgid, SIGKILL);
			timed_out = true;
			snprintf(err, err_len, "timed out after %d s", timeout_ms / 1000);
		}

		sleep_ms(20);
	}

	rc = timed_out ? IFME_RUN_TIMEOUT : IFME_RUN_OK;

	if (exit_code) {
		int st = status[count - 1];
		*exit_code = WIFEXITED(st) ? WEXITSTATUS(st)
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
	return rc;
}

#endif /* !_WIN32 */
