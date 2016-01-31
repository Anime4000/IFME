/*
	IFME launcher for Linux Desktop
	This allow run a mono program with
	terminal open just like Windows Console
*/

#include <stdlib.h>

int main() {
	system("chmod +x ./ifme");
	system("xterm -geometry 120x33 -e './ifme --gui'");
	return 0;
}
