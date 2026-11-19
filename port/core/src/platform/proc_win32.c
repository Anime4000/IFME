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
	SECURITY_ATTRIBUTES sa = { sizeof(sa), NULL, TRUE };   /* inheritable */

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
	HANDLE log = INVALID_HANDLE_VALUE;

	if (log_path) {
		wchar_t *wlog = ifme_utf8_to_wide(log_path);
		if (wlog)
			log = CreateFileW(wlog, FILE_APPEND_DATA, FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE,
			                  &sa, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
		free(wlog);
	}

	wchar_t *wcwd = (cwd && *cwd) ? ifme_utf8_to_wide(cwd) : NULL;
	HANDLE *procs = calloc(count, sizeof(HANDLE));
	size_t started = 0;

	if (!job || null_in == INVALID_HANDLE_VALUE || null_out == INVALID_HANDLE_VALUE || !procs) {
		set_error(err, err_len, "cannot prepare", "pipeline", GetLastError());
		goto done;
	}

	HANDLE errh = log != INVALID_HANDLE_VALUE ? log : null_out;
	HANDLE prev_read = null_in;

	for (size_t i = 0; i < count; i++) {
		HANDLE next_read = NULL, out = null_out;

		if (i + 1 < count && !CreatePipe(&next_read, &out, &sa, 0)) {
			set_error(err, err_len, "cannot create pipe for", stages[i].v[0], GetLastError());
			if (prev_read != null_in)
				CloseHandle(prev_read);
			goto kill;
		}

		DWORD le = 0;
		bool ok = spawn(&stages[i], wcwd, prev_read, out, errh, job, &procs[i], &le);

		/* Our copies of the pipe ends are no longer needed. */
		if (prev_read != null_in)
			CloseHandle(prev_read);
		if (out != null_out)
			CloseHandle(out);

		if (!ok) {
			set_error(err, err_len, "cannot start", stages[i].v[0], le);
			if (next_read)
				CloseHandle(next_read);
			goto kill;
		}

		started++;
		prev_read = next_read;
	}

	DWORD wait = WaitForMultipleObjects((DWORD)count, procs, TRUE,
	                                    timeout_ms > 0 ? (DWORD)timeout_ms : INFINITE);
	if (wait == WAIT_TIMEOUT) {
		TerminateJobObject(job, 1);
		WaitForMultipleObjects((DWORD)count, procs, TRUE, 5000);
		snprintf(err, err_len, "timed out after %d s", timeout_ms / 1000);
		rc = IFME_RUN_TIMEOUT;
	} else {
		rc = IFME_RUN_OK;
	}

	DWORD code = 0;
	if (exit_code && GetExitCodeProcess(procs[count - 1], &code))
		*exit_code = (int)code;
	goto done;

kill:
	if (job)
		TerminateJobObject(job, 1);

done:
	for (size_t i = 0; procs && i < started; i++)
		CloseHandle(procs[i]);
	free(procs);
	free(wcwd);
	if (log != INVALID_HANDLE_VALUE)
		CloseHandle(log);
	if (null_in != INVALID_HANDLE_VALUE)
		CloseHandle(null_in);
	if (null_out != INVALID_HANDLE_VALUE)
		CloseHandle(null_out);
	if (job)
		CloseHandle(job);
	return rc;
}

#endif /* _WIN32 */
