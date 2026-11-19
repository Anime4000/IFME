/*
 * ifme_linux.c - Linux launcher for IFME (runs IFME.exe under Mono)
 *
 * Compile:
 *   gcc -O2 -Wall -o ifme_linux ifme_linux.c
 *
 * Usage:
 *   ./ifme_linux [-t] [IFME options...]
 *     -t   open IFME inside a terminal window (useful to watch the log)
 *   Every other argument is passed through to IFME.exe, e.g. --skip-avx2.
 */

#define _GNU_SOURCE
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <stdbool.h>
#include <limits.h>
#include <libgen.h>

static bool is_command_available(const char *cmd) {
	char check_cmd[256];
	snprintf(check_cmd, sizeof(check_cmd), "command -v %s > /dev/null 2>&1", cmd);
	return system(check_cmd) == 0;
}

static bool file_exists(const char *filename) {
	return access(filename, F_OK) == 0;
}

static void show_dialog(const char *title, const char *message) {
	/* Messages are fixed strings from this file, so quoting them in the shell is safe. */
	if (is_command_available("zenity")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "zenity --error --title='%s' --text='%s'", title, message);
		system(cmd);
	} else if (is_command_available("kdialog")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "kdialog --title '%s' --error '%s'", title, message);
		system(cmd);
	} else if (is_command_available("xmessage")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "xmessage '%s'", message);
		system(cmd);
	}

	fprintf(stderr, "[%s] %s\n", title, message);
}

/*
 * Change into the directory that holds this launcher. Desktop shortcuts, file managers and
 * symlinks in ~/.local/bin all start the launcher from another working directory, which
 * made the IFME.exe check below fail even on a correct install.
 */
static void chdir_to_self(void) {
	char path[PATH_MAX];
	ssize_t len = readlink("/proc/self/exe", path, sizeof(path) - 1);

	if (len <= 0)
		return;

	path[len] = '\0';

	if (chdir(dirname(path)) != 0)
		perror("chdir");
}

static int run_in_terminal(char *const mono_argv[]) {
	/* Terminals take the program and its arguments after -e / --, so no shell quoting needed. */
	const char *terms[][2] = {
		{ "x-terminal-emulator", "-e" },
		{ "gnome-terminal",      "--" },
		{ "konsole",             "-e" },
		{ "xfce4-terminal",      "-x" },
		{ "xterm",               "-e" },
	};

	for (size_t i = 0; i < sizeof(terms) / sizeof(terms[0]); i++) {
		if (!is_command_available(terms[i][0]))
			continue;

		size_t n = 0;
		while (mono_argv[n]) n++;

		/* term flag bash -c SCRIPT ifme mono IFME.exe ... NULL
		 * The script keeps the window open after IFME exits so the log can be read. */
		char **argv = calloc(n + 7, sizeof(char *));
		if (!argv)
			return 1;

		argv[0] = (char *)terms[i][0];
		argv[1] = (char *)terms[i][1];
		argv[2] = "bash";
		argv[3] = "-c";
		argv[4] = "\"$@\"; status=$?; echo; read -r -p \"IFME exited ($status). Press Enter to close...\" _";
		argv[5] = "ifme";
		memcpy(&argv[6], mono_argv, (n + 1) * sizeof(char *));

		execvp(argv[0], argv);
		perror("Failed to start terminal");
		free(argv);
		return 1;
	}

	fprintf(stderr, "No supported terminal emulator found.\n");
	return 1;
}

int main(int argc, char *argv[]) {
	bool use_terminal = false;

	chdir_to_self();

	/* mono IFME.exe <args except -t> NULL */
	char **mono_argv = calloc((size_t)argc + 2, sizeof(char *));
	if (!mono_argv)
		return 1;

	int n = 0;
	mono_argv[n++] = "mono";
	mono_argv[n++] = "IFME.exe";

	for (int i = 1; i < argc; i++) {
		if (strcmp(argv[i], "-t") == 0)
			use_terminal = true;
		else
			mono_argv[n++] = argv[i];
	}

	mono_argv[n] = NULL;

	// Check if mono is installed
	if (!is_command_available("mono")) {
		show_dialog("Mono Missing", "Mono Runtime is not installed. Please install 'mono-complete' to run IFME.");
		return 1;
	}

	// Check if IFME.exe exists
	if (!file_exists("IFME.exe")) {
		show_dialog("Missing File", "IFME.exe is not found next to this launcher. Please verify and reinstall IFME.");
		return 1;
	}

	if (use_terminal)
		return run_in_terminal(mono_argv);

	execvp("mono", mono_argv);
	perror("Failed to execute mono");
	return 1;
}
