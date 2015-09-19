/*
	IFME launcher for Linux Desktop
	This allow run a mono program with
	terminal open just like Windows Console
*/

#include <stdio.h>

int main() {
	system("chmod +x ./ifme");
	system("xterm -e './ifme --gui'");
	return 0;
}