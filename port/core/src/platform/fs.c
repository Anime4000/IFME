/*
 * fs.c - file system helpers. All paths are UTF-8; Windows uses the wide
 * (UTF-16) APIs internally so the library works even in a process that
 * lacks the UTF-8 code page manifest.
 */

#if !defined(_WIN32) && !defined(_GNU_SOURCE)
#define _GNU_SOURCE
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "../internal.h"

#ifdef _WIN32
#define PATH_LIST_SEP ';'
#else
#define PATH_LIST_SEP ':'
#endif

/* ========================================================================== */
#if defined(_WIN32)

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

wchar_t *ifme_utf8_to_wide(const char *s)
{
	int n = MultiByteToWideChar(CP_UTF8, 0, s, -1, NULL, 0);
	if (n <= 0)
		return NULL;

	wchar_t *w = malloc((size_t)n * sizeof(wchar_t));
	if (w && MultiByteToWideChar(CP_UTF8, 0, s, -1, w, n) <= 0) {
		free(w);
		return NULL;
	}
	return w;
}

char *ifme_wide_to_utf8(const wchar_t *w)
{
	int n = WideCharToMultiByte(CP_UTF8, 0, w, -1, NULL, 0, NULL, NULL);
	if (n <= 0)
		return NULL;

	char *s = malloc((size_t)n);
	if (s && WideCharToMultiByte(CP_UTF8, 0, w, -1, s, n, NULL, NULL) <= 0) {
		free(s);
		return NULL;
	}
	return s;
}

static DWORD attributes(const char *path)
{
	wchar_t *w = ifme_utf8_to_wide(path);
	if (!w)
		return INVALID_FILE_ATTRIBUTES;

	DWORD a = GetFileAttributesW(w);
	free(w);
	return a;
}

bool ifme_file_exists(const char *path)
{
	DWORD a = attributes(path);
	return a != INVALID_FILE_ATTRIBUTES && !(a & FILE_ATTRIBUTE_DIRECTORY);
}

bool ifme_dir_exists(const char *path)
{
	DWORD a = attributes(path);
	return a != INVALID_FILE_ATTRIBUTES && (a & FILE_ATTRIBUTE_DIRECTORY);
}

long long ifme_file_size(const char *path)
{
	WIN32_FILE_ATTRIBUTE_DATA data;
	wchar_t *w = ifme_utf8_to_wide(path);
	if (!w)
		return -1;

	BOOL ok = GetFileAttributesExW(w, GetFileExInfoStandard, &data);
	free(w);
	if (!ok || (data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
		return -1;

	return ((long long)data.nFileSizeHigh << 32) | data.nFileSizeLow;
}

bool ifme_path_is_absolute(const char *p)
{
	if (!p || !*p)
		return false;
	if ((p[0] == '\\' || p[0] == '/') && (p[1] == '\\' || p[1] == '/'))
		return true;   /* UNC */
	return ((p[0] | 0x20) >= 'a' && (p[0] | 0x20) <= 'z') && p[1] == ':' &&
	       (p[2] == '\\' || p[2] == '/');
}

char *ifme_full_path(const char *path)
{
	wchar_t *w = ifme_utf8_to_wide(path);
	if (!w)
		return NULL;

	DWORD n = GetFullPathNameW(w, 0, NULL, NULL);
	wchar_t *full = n ? malloc(n * sizeof(wchar_t)) : NULL;
	char *out = NULL;

	if (full && GetFullPathNameW(w, n, full, NULL) > 0)
		out = ifme_wide_to_utf8(full);

	free(full);
	free(w);
	return out ? out : ifme_strdup(path);
}

char *ifme_resolve_program(const char *path)
{
	if (ifme_file_exists(path))
		return ifme_strdup(path);

	if (!ifme_ends_with_ci(path, ".exe")) {
		char *exe = ifme_strdupf("%s.exe", path);
		if (exe && ifme_file_exists(exe))
			return exe;
		free(exe);
	}
	return NULL;
}

char *ifme_temp_dir(void)
{
	wchar_t buf[MAX_PATH + 2];
	DWORD n = GetTempPathW(MAX_PATH + 2, buf);
	if (n == 0 || n > MAX_PATH + 1)
		return ifme_strdup(".");

	if (n > 1 && buf[n - 1] == L'\\')
		buf[n - 1] = L'\0';
	return ifme_wide_to_utf8(buf);
}

bool ifme_mkdir(const char *path)
{
	wchar_t *w = ifme_utf8_to_wide(path);
	if (!w)
		return false;

	BOOL ok = CreateDirectoryW(w, NULL) || GetLastError() == ERROR_ALREADY_EXISTS;
	free(w);
	return ok;
}

void ifme_clear_dir(const char *path)
{
	char *pattern = ifme_path_join(path, "*");
	wchar_t *w = pattern ? ifme_utf8_to_wide(pattern) : NULL;
	free(pattern);
	if (!w)
		return;

	WIN32_FIND_DATAW fd;
	HANDLE h = FindFirstFileW(w, &fd);
	free(w);
	if (h == INVALID_HANDLE_VALUE)
		return;

	do {
		if (fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
			continue;

		char *name = ifme_wide_to_utf8(fd.cFileName);
		char *full = name ? ifme_path_join(path, name) : NULL;
		wchar_t *wf = full ? ifme_utf8_to_wide(full) : NULL;
		if (wf)
			DeleteFileW(wf);
		free(wf);
		free(full);
		free(name);
	} while (FindNextFileW(h, &fd));

	FindClose(h);
}

void ifme_rmdir(const char *path)
{
	wchar_t *w = ifme_utf8_to_wide(path);
	if (w)
		RemoveDirectoryW(w);
	free(w);
}

unsigned long ifme_pid(void)
{
	return GetCurrentProcessId();
}

uint64_t ifme_now_ms(void)
{
	return GetTickCount64();
}

void ifme_walk_files(const char *root, ifme_file_cb cb, void *user)
{
	char *pattern = ifme_path_join(root, "*");
	wchar_t *w = pattern ? ifme_utf8_to_wide(pattern) : NULL;
	free(pattern);
	if (!w)
		return;

	WIN32_FIND_DATAW fd;
	HANDLE h = FindFirstFileW(w, &fd);
	free(w);
	if (h == INVALID_HANDLE_VALUE)
		return;

	do {
		if (wcscmp(fd.cFileName, L".") == 0 || wcscmp(fd.cFileName, L"..") == 0)
			continue;

		char *name = ifme_wide_to_utf8(fd.cFileName);
		char *full = name ? ifme_path_join(root, name) : NULL;

		if (full) {
			if (fd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) {
				/* Don't follow junctions/symlinks: they can form loops. */
				if (!(fd.dwFileAttributes & FILE_ATTRIBUTE_REPARSE_POINT))
					ifme_walk_files(full, cb, user);
			} else {
				cb(user, full);
			}
		}

		free(full);
		free(name);
	} while (FindNextFileW(h, &fd));

	FindClose(h);
}

/* ========================================================================== */
#else /* POSIX */

#include <dirent.h>
#include <errno.h>
#include <limits.h>
#include <sys/stat.h>
#include <time.h>
#include <unistd.h>

bool ifme_file_exists(const char *path)
{
	struct stat st;
	return stat(path, &st) == 0 && S_ISREG(st.st_mode);
}

bool ifme_dir_exists(const char *path)
{
	struct stat st;
	return stat(path, &st) == 0 && S_ISDIR(st.st_mode);
}

long long ifme_file_size(const char *path)
{
	struct stat st;
	if (stat(path, &st) != 0 || !S_ISREG(st.st_mode))
		return -1;
	return (long long)st.st_size;
}

bool ifme_path_is_absolute(const char *p)
{
	return p && p[0] == '/';
}

char *ifme_full_path(const char *path)
{
	char *r = realpath(path, NULL);
	return r ? r : ifme_strdup(path);
}

char *ifme_resolve_program(const char *path)
{
	if (ifme_file_exists(path) && access(path, X_OK) == 0)
		return ifme_strdup(path);
	return NULL;
}

char *ifme_temp_dir(void)
{
	const char *t = getenv("TMPDIR");
	return ifme_strdup(t && *t ? t : "/tmp");
}

bool ifme_mkdir(const char *path)
{
	return mkdir(path, 0700) == 0 || errno == EEXIST;
}

void ifme_clear_dir(const char *path)
{
	DIR *d = opendir(path);
	if (!d)
		return;

	struct dirent *e;
	while ((e = readdir(d)) != NULL) {
		char *full = ifme_path_join(path, e->d_name);
		if (full && ifme_file_exists(full))
			unlink(full);
		free(full);
	}
	closedir(d);
}

void ifme_rmdir(const char *path)
{
	rmdir(path);
}

unsigned long ifme_pid(void)
{
	return (unsigned long)getpid();
}

uint64_t ifme_now_ms(void)
{
	struct timespec ts;
	clock_gettime(CLOCK_MONOTONIC, &ts);
	return (uint64_t)ts.tv_sec * 1000u + (uint64_t)ts.tv_nsec / 1000000u;
}

void ifme_walk_files(const char *root, ifme_file_cb cb, void *user)
{
	DIR *d = opendir(root);
	if (!d)
		return;

	struct dirent *e;
	while ((e = readdir(d)) != NULL) {
		if (strcmp(e->d_name, ".") == 0 || strcmp(e->d_name, "..") == 0)
			continue;

		char *full = ifme_path_join(root, e->d_name);
		struct stat st;

		/* lstat: don't follow symlinked folders (loops); symlinked files are fine. */
		if (full && lstat(full, &st) == 0) {
			if (S_ISDIR(st.st_mode))
				ifme_walk_files(full, cb, user);
			else if (S_ISREG(st.st_mode) || (S_ISLNK(st.st_mode) && ifme_file_exists(full)))
				cb(user, full);
		}
		free(full);
	}
	closedir(d);
}

#endif

/* ========================================================================== */
/* Portable                                                                   */

char *ifme_read_file(const char *path, size_t *len)
{
#ifdef _WIN32
	wchar_t *w = ifme_utf8_to_wide(path);
	FILE *f = w ? _wfopen(w, L"rb") : NULL;
	free(w);
#else
	FILE *f = fopen(path, "rb");
#endif
	if (!f)
		return NULL;

	char *buf = NULL;
	size_t size = 0, cap = 0;

	for (;;) {
		if (size + 4096 + 1 > cap) {
			cap = cap ? cap * 2 : 8192;
			char *grown = realloc(buf, cap);
			if (!grown) {
				free(buf);
				fclose(f);
				return NULL;
			}
			buf = grown;
		}

		size_t got = fread(buf + size, 1, cap - size - 1, f);
		size += got;
		if (got == 0)
			break;
	}

	fclose(f);
	buf[size] = '\0';
	if (len)
		*len = size;
	return buf;
}

char *ifme_find_in_path(const char *name)
{
	if (!name || !*name || strchr(name, '/') || strchr(name, '\\'))
		return NULL;

#ifdef _WIN32
	/* getenv() would convert through the ANSI code page and mangle non-ASCII
	 * folders when the host process has no UTF-8 manifest. */
	char *list = NULL;
	DWORD need = GetEnvironmentVariableW(L"PATH", NULL, 0);
	if (need > 0) {
		wchar_t *w = malloc(need * sizeof(wchar_t));
		if (w && GetEnvironmentVariableW(L"PATH", w, need) > 0)
			list = ifme_wide_to_utf8(w);
		free(w);
	}
#else
	const char *env = getenv("PATH");
	char *list = env ? ifme_strdup(env) : NULL;
#endif
	if (!list)
		return NULL;

	char *found = NULL;
	char *entry = list;

	while (entry && !found) {
		char *next = strchr(entry, PATH_LIST_SEP);
		if (next)
			*next++ = '\0';

		/* Trim spaces and the quotes Windows users put around entries. An empty
		 * entry would mean "current folder", which is not what PATH is for here. */
		while (*entry == ' ' || *entry == '"')
			entry++;
		size_t l = strlen(entry);
		while (l > 0 && (entry[l - 1] == ' ' || entry[l - 1] == '"'))
			entry[--l] = '\0';

		if (l > 0) {
			char *candidate = ifme_path_join(entry, name);
			if (candidate) {
				found = ifme_resolve_program(candidate);
				free(candidate);
			}
		}

		entry = next;
	}

	free(list);
	return found;
}
