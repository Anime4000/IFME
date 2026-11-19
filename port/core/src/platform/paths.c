/*
 * paths.c - executable folder and per-user config folder, per platform
 */

#include <stdlib.h>
#include <string.h>

#include "../internal.h"

#if defined(_WIN32)

#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <initguid.h>     /* define FOLDERID_* here instead of linking libuuid */
#include <knownfolders.h>
#include <shlobj.h>

static char *utf16_to_utf8(const wchar_t *w)
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

char *ifme_platform_app_dir(void)
{
	/* Long-path aware: grow the buffer until the full path fits. */
	DWORD cap = MAX_PATH;
	wchar_t *buf = NULL;

	for (;;) {
		wchar_t *grown = realloc(buf, cap * sizeof(wchar_t));
		if (!grown) {
			free(buf);
			return NULL;
		}
		buf = grown;

		DWORD len = GetModuleFileNameW(NULL, buf, cap);
		if (len == 0) {
			free(buf);
			return NULL;
		}
		if (len < cap)
			break;
		cap *= 2;
	}

	wchar_t *slash = wcsrchr(buf, L'\\');
	if (slash)
		*slash = L'\0';

	char *out = utf16_to_utf8(buf);
	free(buf);
	return out;
}

char *ifme_platform_config_dir(void)
{
	char *base = NULL;
	PWSTR known = NULL;

	/* Ask the shell rather than trusting %APPDATA%, which can be missing from
	 * a sanitised environment (services, some launchers and build tools). */
	if (SUCCEEDED(SHGetKnownFolderPath(&FOLDERID_RoamingAppData, 0, NULL, &known)))
		base = utf16_to_utf8(known);
	CoTaskMemFree(known);

	if (!base) {
		static wchar_t appdata[32768];
		DWORD len = GetEnvironmentVariableW(L"APPDATA", appdata, 32768);
		if (len == 0 || len >= 32768)
			return NULL;
		base = utf16_to_utf8(appdata);
	}

	if (!base)
		return NULL;

	char *out = ifme_path_join(base, "IFME");
	free(base);
	return out;
}

#else /* POSIX */

#include <limits.h>
#include <unistd.h>

#if defined(__APPLE__)
#include <mach-o/dyld.h>
#endif

static char *dirname_dup(const char *path)
{
	char *copy = ifme_strdup(path);
	if (!copy)
		return NULL;

	char *slash = strrchr(copy, '/');
	if (slash == copy)
		slash[1] = '\0';   /* executable in "/" */
	else if (slash)
		*slash = '\0';
	return copy;
}

char *ifme_platform_app_dir(void)
{
#if defined(__APPLE__)
	uint32_t size = 0;
	_NSGetExecutablePath(NULL, &size);   /* query required size */

	char *raw = malloc(size);
	if (!raw || _NSGetExecutablePath(raw, &size) != 0) {
		free(raw);
		return NULL;
	}

	char *resolved = realpath(raw, NULL);   /* resolve symlinks and ".." */
	free(raw);
	if (!resolved)
		return NULL;

	char *out = dirname_dup(resolved);
	free(resolved);
	return out;
#else
	/* Linux (and FreeBSD with linprocfs). */
	char buf[PATH_MAX];
	ssize_t len = readlink("/proc/self/exe", buf, sizeof(buf) - 1);
	if (len <= 0)
		return NULL;

	buf[len] = '\0';
	return dirname_dup(buf);
#endif
}

char *ifme_platform_config_dir(void)
{
	const char *home = getenv("HOME");

#if defined(__APPLE__)
	if (!home || !*home)
		return NULL;

	char *support = ifme_path_join(home, "Library/Application Support");
	if (!support)
		return NULL;

	char *out = ifme_path_join(support, "IFME");
	free(support);
	return out;
#else
	const char *xdg = getenv("XDG_CONFIG_HOME");
	if (xdg && *xdg == '/')
		return ifme_path_join(xdg, "ifme");

	if (!home || !*home)
		return NULL;

	char *config = ifme_path_join(home, ".config");
	if (!config)
		return NULL;

	char *out = ifme_path_join(config, "ifme");
	free(config);
	return out;
#endif
}

#endif
