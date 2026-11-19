/*
 * proc_win32.c - run a pipeline of programs without cmd.exe
 *
 *  - Each stage gets an argument list, quoted for the Microsoft C runtime
 *    parser, so file names with spaces, quotes or `%` arrive intact.
 *  - Every process goes into one Job Object: killing the job kills the whole
 *    pipeline, and JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE ends the encoders even
 *    if IFME itself crashes.
 *  - PROC_THREAD_ATTRIBUTE_HANDLE_LIST limits which handles a child inherits.
 *    Without it the encoder would also inherit the pipe's write end and never
 *    see end-of-file from ffmpeg.
 */

#if defined(_WIN32)

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "../internal.h"

/* Quote one argument following the CommandLineToArgvW / MSVCRT rules. */
static size_t quote_arg(const char *arg, char *out)
{
	size_t o = 0;

	if (*arg && !strpbrk(arg, " \t\n\v\"")) {
		size_t l = strlen(arg);
		if (out)
			memcpy(out, arg, l);
		return l;
	}

	if (out) out[o] = '"';
	o++;

	for (const char *p = arg;; p++) {
		size_t backslashes = 0;
		while (*p == '\\') {
			backslashes++;
			p++;
		}

		if (*p == '\0') {
			/* Double trailing backslashes so the closing quote stays a quote. */
			for (size_t i = 0; i < backslashes * 2; i++, o++)
				if (out) out[o] = '\\';
			break;
		} else if (*p == '"') {
			for (size_t i = 0; i < backslashes * 2 + 1; i++, o++)
				if (out) out[o] = '\\';
			if (out) out[o] = '"';
			o++;
		} else {
			for (size_t i = 0; i < backslashes; i++, o++)
				if (out) out[o] = '\\';
			if (out) out[o] = *p;
			o++;
		}
	}

	if (out) out[o] = '"';
	o++;
	return o;
}

static wchar_t *build_command_line(const ifme_argv *a)
{
	size_t total = 1;
	for (size_t i = 0; i < a->n; i++)
		total += quote_arg(a->v[i], NULL) + 1;

	char *line = malloc(total);
	if (!line)
		return NULL;

	size_t pos = 0;
	for (size_t i = 0; i < a->n; i++) {
		if (i)
			line[pos++] = ' ';
		pos += quote_arg(a->v[i], line + pos);
	}
	line[pos] = '\0';

	wchar_t *w = ifme_utf8_to_wide(line);
	free(line);
	return w;
}

static void set_error(char *err, size_t len, const char *what, const char *prog, DWORD code)
{
	if (!err || !len)
		return;

	wchar_t msgw[256] = L"";
	FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS, NULL, code,
	               0, msgw, 256, NULL);
	char *msg = ifme_wide_to_utf8(msgw);

	if (msg) {
		size_t l = strlen(msg);
		while (l > 0 && (msg[l - 1] == '\r' || msg[l - 1] == '\n' || msg[l - 1] == '.'))
			msg[--l] = '\0';
	}

	snprintf(err, len, "%s %s: %s (error %lu)", what, prog ? prog : "", msg ? msg : "", (unsigned long)code);
	free(msg);
}

static bool spawn(const ifme_argv *stage, const wchar_t *wcwd, HANDLE in, HANDLE out,
                  HANDLE errh, HANDLE job, HANDLE *proc_out, DWORD *last_error)
{
	wchar_t *cmd = build_command_line(stage);
	if (!cmd) {
		*last_error = ERROR_NOT_ENOUGH_MEMORY;
		return false;
	}

	/* Hand the child exactly these handles (de-duplicated: the list must not
	 * repeat a handle). */
	HANDLE list[3];
	size_t n = 0;
	HANDLE cand[3] = { in, out, errh };
	for (size_t i = 0; i < 3; i++) {
		bool dup = false;
		for (size_t j = 0; j < n; j++)
			dup = dup || list[j] == cand[i];
		if (!dup)
			list[n++] = cand[i];
	}

	SIZE_T attr_size = 0;
	InitializeProcThreadAttributeList(NULL, 1, 0, &attr_size);
	LPPROC_THREAD_ATTRIBUTE_LIST attrs = malloc(attr_size);

	STARTUPINFOEXW si;
	PROCESS_INFORMATION pi;
	memset(&si, 0, sizeof(si));
	memset(&pi, 0, sizeof(pi));
	si.StartupInfo.cb = sizeof(si);
	si.StartupInfo.dwFlags = STARTF_USESTDHANDLES;
	si.StartupInfo.hStdInput = in;
	si.StartupInfo.hStdOutput = out;
	si.StartupInfo.hStdError = errh;

	bool ok = attrs &&
		InitializeProcThreadAttributeList(attrs, 1, 0, &attr_size) &&
		UpdateProcThreadAttribute(attrs, 0, PROC_THREAD_ATTRIBUTE_HANDLE_LIST,
		                          list, n * sizeof(HANDLE), NULL, NULL);

	if (ok) {
		si.lpAttributeList = attrs;
		ok = CreateProcessW(NULL, cmd, NULL, NULL, TRUE,
		                    CREATE_SUSPENDED | CREATE_NO_WINDOW |
		                    EXTENDED_STARTUPINFO_PRESENT | CREATE_UNICODE_ENVIRONMENT,
		                    NULL, wcwd, &si.StartupInfo, &pi);
	}

	*last_error = ok ? 0 : GetLastError();

	if (attrs) {
		DeleteProcThreadAttributeList(attrs);
		free(attrs);
	}
	free(cmd);

	if (!ok)
		return false;

	/* Join the job before the process runs a single instruction. */
	if (!AssignProcessToJobObject(job, pi.hProcess)) {
		*last_error = GetLastError();
		TerminateProcess(pi.hProcess, 1);
		CloseHandle(pi.hThread);
		CloseHandle(pi.hProcess);
		return false;
	}

	ResumeThread(pi.hThread);
	CloseHandle(pi.hThread);
	*proc_out = pi.hProcess;
	return true;
}

/* Read whatever is waiting in an anonymous pipe without blocking. Anonymous
 * pipes cannot be poll()ed on Windows, but PeekNamedPipe works on them.
 * Closes the handle and sets it to NULL at end-of-file. */
static bool drain(HANDLE *h, ifme_capture *c)
{
	char buf[65536];
	bool got = false;

	while (*h) {
		DWORD avail = 0, n = 0;

		if (!PeekNamedPipe(*h, NULL, 0, NULL, &avail, NULL)) {
			CloseHandle(*h);           /* ERROR_BROKEN_PIPE: writer gone */
			*h = NULL;
			break;
		}
		if (avail == 0)
			break;

		if (!ReadFile(*h, buf, avail < sizeof(buf) ? avail : (DWORD)sizeof(buf), &n, NULL) || n == 0) {
			CloseHandle(*h);
			*h = NULL;
			break;
		}

		ifme_capture_append(c, buf, n);
		got = true;
	}
	return got;
}

static bool make_pipe(HANDLE *read_end, HANDLE *write_end, SECURITY_ATTRIBUTES *sa)
{
	if (!CreatePipe(read_end, write_end, sa, 0))
		return false;

	/* Only the write end is meant for children. */
	SetHandleInformation(*read_end, HANDLE_FLAG_INHERIT, 0);
	return true;
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

	SECURITY_ATTRIBUTES sa = { sizeof(sa), NULL, TRUE };   /* inheritable */
	ifme_capture cap_out = { 0 }, cap_err = { 0 };
	cap_out.limit = cap_err.limit = opts->capture_limit;

	HANDLE job = CreateJobObjectW(NULL, NULL);
	JOBOBJECT_EXTENDED_LIMIT_INFORMATION limits;
	memset(&limits, 0, sizeof(limits));
	limits.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;
	if (job)
		SetInformationJobObject(job, JobObjectExtendedLimitInformation, &limits, sizeof(limits));

	HANDLE null_in = CreateFileW(L"NUL", GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE,
	                             &sa, OPEN_EXISTING, 0, NULL);
	HANDLE null_out = CreateFileW(L"NUL", GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE,
	                              &sa, OPEN_EXISTING, 0, NULL);

	HANDLE out_read = NULL, out_write = null_out;
	HANDLE err_read = NULL, err_write = null_out;
	wchar_t *wcwd = (opts->cwd && *opts->cwd) ? ifme_utf8_to_wide(opts->cwd) : NULL;
	HANDLE *procs = calloc(count, sizeof(HANDLE));
	size_t started = 0;

	if (!job || null_in == INVALID_HANDLE_VALUE || null_out == INVALID_HANDLE_VALUE || !procs) {
		set_error(res->message, sizeof(res->message), "cannot prepare", "pipeline", GetLastError());
		goto done;
	}

	if (opts->capture_stdout && !make_pipe(&out_read, &out_write, &sa)) {
		set_error(res->message, sizeof(res->message), "cannot create", "stdout pipe", GetLastError());
		goto done;
	}

	if (opts->capture_stderr) {
		if (!make_pipe(&err_read, &err_write, &sa)) {
			set_error(res->message, sizeof(res->message), "cannot create", "stderr pipe", GetLastError());
			goto done;
		}
	} else if (opts->stderr_path) {
		wchar_t *wlog = ifme_utf8_to_wide(opts->stderr_path);
		HANDLE log = wlog ? CreateFileW(wlog, FILE_APPEND_DATA,
		                                FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
		                                &sa, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL)
		                  : INVALID_HANDLE_VALUE;
		free(wlog);
		if (log != INVALID_HANDLE_VALUE)
			err_write = log;
	}

	HANDLE prev_read = null_in;

	for (size_t i = 0; i < count; i++) {
		HANDLE next_read = NULL, out = (i + 1 == count) ? out_write : NULL;

		if (i + 1 < count && !CreatePipe(&next_read, &out, &sa, 0)) {
			set_error(res->message, sizeof(res->message), "cannot create pipe for", stages[i].v[0], GetLastError());
			if (prev_read != null_in)
				CloseHandle(prev_read);
			goto kill;
		}

		DWORD le = 0;
		bool ok = spawn(&stages[i], wcwd, prev_read, out, err_write, job, &procs[i], &le);

		/* Our copies of the inter-stage pipe ends are no longer needed. */
		if (prev_read != null_in)
			CloseHandle(prev_read);
		if (i + 1 < count)
			CloseHandle(out);

		if (!ok) {
			set_error(res->message, sizeof(res->message), "cannot start", stages[i].v[0], le);
			if (next_read)
				CloseHandle(next_read);
			goto kill;
		}

		started++;
		prev_read = next_read;
	}

	/* Every child has its copy now; keeping ours would hide end-of-file. */
	if (out_write != null_out) {
		CloseHandle(out_write);
		out_write = null_out;
	}
	if (err_write != null_out) {
		CloseHandle(err_write);
		err_write = null_out;
	}

	uint64_t deadline = opts->timeout_ms > 0 ? ifme_now_ms() + (uint64_t)opts->timeout_ms : 0;
	bool timed_out = false, stragglers_killed = false;

	for (;;) {
		bool got = drain(&out_read, &cap_out);
		got = drain(&err_read, &cap_err) || got;

		bool exited = WaitForMultipleObjects((DWORD)count, procs, TRUE, 0) == WAIT_OBJECT_0;

		if (exited) {
			if (!out_read && !err_read)
				break;

			/* A grandchild may still hold our pipe open; the stages are done,
			 * so end it to get end-of-file. Buffered data stays readable. */
			if (!stragglers_killed) {
				TerminateJobObject(job, 0);
				stragglers_killed = true;
			}
		}

		if (deadline && !timed_out && ifme_now_ms() >= deadline) {
			TerminateJobObject(job, 1);
			timed_out = true;
			snprintf(res->message, sizeof(res->message), "timed out after %d s", opts->timeout_ms / 1000);
		}

		if (!got) {
			if (out_read || err_read)
				Sleep(5);
			else
				WaitForMultipleObjects((DWORD)count, procs, TRUE, 100);
		}
	}

	res->status = timed_out ? IFME_RUN_TIMEOUT : IFME_RUN_OK;

	DWORD code = 0;
	if (GetExitCodeProcess(procs[count - 1], &code))
		res->exit_code = (int)code;
	goto done;

kill:
	if (job)
		TerminateJobObject(job, 1);

done:
	if (out_read)
		CloseHandle(out_read);
	if (err_read)
		CloseHandle(err_read);
	if (out_write && out_write != null_out)
		CloseHandle(out_write);
	if (err_write && err_write != null_out)
		CloseHandle(err_write);

	ifme_capture_finish(&cap_out, &res->out, &res->out_len);
	ifme_capture_finish(&cap_err, &res->err, &res->err_len);

	for (size_t i = 0; procs && i < started; i++)
		CloseHandle(procs[i]);
	free(procs);
	free(wcwd);
	if (null_in != INVALID_HANDLE_VALUE)
		CloseHandle(null_in);
	if (null_out != INVALID_HANDLE_VALUE)
		CloseHandle(null_out);
	if (job)
		CloseHandle(job);
	return res->status;
}

#endif /* _WIN32 */
