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
#include <tlhelp32.h>
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

/* Suspend or resume every thread of every process in the job. Windows has
 * no documented "suspend process" call, so walk a Toolhelp thread snapshot. */
static void job_suspend(HANDLE job, bool suspend)
{
	struct {
		JOBOBJECT_BASIC_PROCESS_ID_LIST head;
		ULONG_PTR more[255];
	} list;
	memset(&list, 0, sizeof(list));

	if (!QueryInformationJobObject(job, JobObjectBasicProcessIdList, &list, sizeof(list), NULL))
		return;

	DWORD count = list.head.NumberOfProcessIdsInList;
	HANDLE snap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
	if (snap == INVALID_HANDLE_VALUE)
		return;

	THREADENTRY32 te;
	te.dwSize = sizeof(te);

	for (BOOL ok = Thread32First(snap, &te); ok; ok = Thread32Next(snap, &te)) {
		bool ours = false;
		for (DWORD i = 0; i < count && !ours; i++)
			ours = list.head.ProcessIdList[i] == (ULONG_PTR)te.th32OwnerProcessID;
		if (!ours)
			continue;

		HANDLE th = OpenThread(THREAD_SUSPEND_RESUME, FALSE, te.th32ThreadID);
		if (th) {
			if (suspend)
				SuspendThread(th);
			else
				ResumeThread(th);
			CloseHandle(th);
		}
		te.dwSize = sizeof(te);
	}

	CloseHandle(snap);
}

typedef struct stream_reader {
	HANDLE h;                    /* NULL once at end-of-file */
	ifme_capture *capture;       /* NULL: don't keep the bytes */
	ifme_linebuf *lines;         /* NULL: don't split into lines */
	const ifme_run_opts *opts;
} stream_reader;

/* Read whatever is waiting in an anonymous pipe without blocking. Anonymous
 * pipes cannot be poll()ed on Windows, but PeekNamedPipe works on them. */
static bool drain(stream_reader *r)
{
	char buf[65536];
	bool got = false;

	while (r->h) {
		DWORD avail = 0, n = 0;

		if (!PeekNamedPipe(r->h, NULL, 0, NULL, &avail, NULL)) {
			CloseHandle(r->h);         /* ERROR_BROKEN_PIPE: writer gone */
			r->h = NULL;
			break;
		}
		if (avail == 0)
			break;

		if (!ReadFile(r->h, buf, avail < sizeof(buf) ? avail : (DWORD)sizeof(buf), &n, NULL) || n == 0) {
			CloseHandle(r->h);
			r->h = NULL;
			break;
		}

		if (r->capture)
			ifme_capture_append(r->capture, buf, n);
		if (r->lines)
			ifme_linebuf_feed(r->lines, buf, n, r->opts->on_line, r->opts->user);
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

/* Apply pause/stop requests, the timeout and the tick callback. */
static void apply_control(HANDLE job, const ifme_run_opts *opts, ifme_run_state *st,
                          char *message, size_t message_len)
{
	ifme_control *c = opts->control;
	uint64_t now = ifme_now_ms();

	if (c && !st->stopped && atomic_load(&c->want_stop)) {
		if (st->paused)
			job_suspend(job, false);   /* not required to kill, but tidy */
		TerminateJobObject(job, 1);
		st->stopped = true;
		snprintf(message, message_len, "stopped");
	}

	if (c && !st->stopped) {
		bool want = atomic_load(&c->want_pause) != 0;
		if (want != st->paused) {
			job_suspend(job, want);
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
		TerminateJobObject(job, 1);
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

	SECURITY_ATTRIBUTES sa = { sizeof(sa), NULL, TRUE };   /* inheritable */
	ifme_capture cap_out = { 0 }, cap_err = { 0 };
	cap_out.limit = cap_err.limit = opts->capture_limit;
	ifme_linebuf *lines = opts->on_line ? calloc(1, sizeof(ifme_linebuf)) : NULL;

	stream_reader out_r = { NULL, &cap_out, NULL, opts };
	stream_reader err_r = { NULL, opts->capture_stderr ? &cap_err : NULL, lines, opts };

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

	HANDLE out_write = null_out, err_write = null_out;
	wchar_t *wcwd = (opts->cwd && *opts->cwd) ? ifme_utf8_to_wide(opts->cwd) : NULL;
	HANDLE *procs = calloc(count, sizeof(HANDLE));
	size_t started = 0;
	ifme_run_state st;
	memset(&st, 0, sizeof(st));

	if (!job || null_in == INVALID_HANDLE_VALUE || null_out == INVALID_HANDLE_VALUE || !procs ||
	    (opts->on_line && !lines)) {
		set_error(res->message, sizeof(res->message), "cannot prepare", "pipeline", GetLastError());
		goto done;
	}

	if (opts->capture_stdout && !make_pipe(&out_r.h, &out_write, &sa)) {
		set_error(res->message, sizeof(res->message), "cannot create", "stdout pipe", GetLastError());
		goto done;
	}

	if (opts->capture_stderr || opts->on_line) {
		if (!make_pipe(&err_r.h, &err_write, &sa)) {
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

	st.deadline = opts->timeout_ms > 0 ? ifme_now_ms() + (uint64_t)opts->timeout_ms : 0;
	bool stragglers_killed = false;

	for (;;) {
		bool got = drain(&out_r);
		got = drain(&err_r) || got;

		apply_control(job, opts, &st, res->message, sizeof(res->message));

		bool exited = WaitForMultipleObjects((DWORD)count, procs, TRUE, 0) == WAIT_OBJECT_0;

		if (exited) {
			if (!out_r.h && !err_r.h)
				break;

			/* A grandchild may still hold our pipe open; the stages are done,
			 * so end it to get end-of-file. Buffered data stays readable. */
			if (!stragglers_killed) {
				TerminateJobObject(job, 0);
				stragglers_killed = true;
			}
		}

		if (!got) {
			if (out_r.h || err_r.h)
				Sleep(5);
			else
				WaitForMultipleObjects((DWORD)count, procs, TRUE, 20);
		}
	}

	if (lines)
		ifme_linebuf_flush(lines, opts->on_line, opts->user);

	res->status = st.stopped ? IFME_RUN_STOPPED : st.timed_out ? IFME_RUN_TIMEOUT : IFME_RUN_OK;

	DWORD code = 0;
	if (GetExitCodeProcess(procs[count - 1], &code))
		res->exit_code = (int)code;
	goto done;

kill:
	if (job)
		TerminateJobObject(job, 1);

done:
	if (opts->control)
		atomic_store(&opts->control->paused, 0);
	if (out_r.h)
		CloseHandle(out_r.h);
	if (err_r.h)
		CloseHandle(err_r.h);
	if (out_write && out_write != null_out)
		CloseHandle(out_write);
	if (err_write && err_write != null_out)
		CloseHandle(err_write);

	ifme_capture_finish(&cap_out, &res->out, &res->out_len);
	ifme_capture_finish(&cap_err, &res->err, &res->err_len);
	free(lines);

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
