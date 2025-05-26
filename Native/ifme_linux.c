#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <stdbool.h>

bool is_command_available(const char *cmd) {
	char check_cmd[256];
	snprintf(check_cmd, sizeof(check_cmd), "command -v %s > /dev/null 2>&1", cmd);
	return system(check_cmd) == 0;
}

bool file_exists(const char *filename) {
	return access(filename, F_OK) == 0;
}

void show_dialog(const char *title, const char *message) {
	if (is_command_available("zenity")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "zenity --error --title='%s' --text='%s'", title, message);
		system(cmd);
	} else if (is_command_available("xmessage")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "xmessage '%s'", message);
		system(cmd);
	} else {
		fprintf(stderr, "[%s] %s\n", title, message);
	}
}

void run_in_terminal(const char *command) {
	if (is_command_available("x-terminal-emulator")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "x-terminal-emulator -e '%s'", command);
		system(cmd);
	} else if (is_command_available("gnome-terminal")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "gnome-terminal -- bash -c '%s; exec bash'", command);
		system(cmd);
	} else if (is_command_available("konsole")) {
		char cmd[512];
		snprintf(cmd, sizeof(cmd), "konsole -e bash -c '%s; exec bash'", command);
		system(cmd);
	} else {
		fprintf(stderr, "No supported terminal emulator found.\n");
	}
}

int main(int argc, char *argv[]) {
	bool use_terminal = false;

	// Check for -t argument
	for (int i = 1; i < argc; i++) {
		if (strcmp(argv[i], "-t") == 0) {
			use_terminal = true;
		}
	}

	// Check if mono is installed
	if (!is_command_available("mono")) {
		show_dialog("Mono Missing", "Mono Runtime is not installed. Please install 'mono-complete' to run IFME.");
		return 1;
	}

	// Check if IFME.exe exists
	if (!file_exists("IFME.exe")) {
		show_dialog("Missing File", "IFME.exe is not found in the current directory. Please verify and reinstall IFME.");
		return 1;
	}

	// Build the command string
	const char *exec_cmd = "mono IFME.exe";

	// Run command
	if (use_terminal) {
		run_in_terminal(exec_cmd);
	} else {
		execlp("mono", "mono", "IFME.exe", (char *)NULL);
		perror("Failed to execute mono");
		return 1;
	}

	return 0;
}