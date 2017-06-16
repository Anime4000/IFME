#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by Anime4000 @ https://github.com/Anime4000
#
# Purpose of this script allow IFME to be launch
# with Terminal Emulator, by default all message
# will be output through Terminal stdout.
#
# IFME heavily depend on mono-runtime, it's encourage
# user to use latest version @ http://www.mono-project.com/download/#download-lin

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

ERROR_TTY="No terminal attached, IFME require to display log here!"
ERROR_MONO="You must have mono-runtime installed to run IFME.\nDownload the appropriate package for your distribution."

OKAY_TTY="Terminal attached, IFME will post encoding process here."

function runtty {
	if hash gnome-terminal 2>/dev/null; then
		gnome-terminal -e 'bash ifme.sh'
		exit 0
	fi

	if hash konsole 2>/dev/null; then
		konsole -e 'bash ifme.sh'
		exit 0
	fi

	if hash xfce4-terminal 2>/dev/null; then
		xfce4-terminal -e 'bash ifme.sh'
		exit 0
	fi
}

function checktty {
	tty -s
	if [ "0" == "$?" ]; then
		echo $OKAY_TTY
		sleep 1
		clear
	else
		echo $ERROR_TTY
		runtty
		exit 1
	fi
}

function checkmono {
	(mono --version) < /dev/null > /dev/null 2>&1 || {
		echo
		echo $ERROR_MONO
		echo
		zenity --error --title "ERROR!" --text "$ERROR_MONO"
		sleep 5
		exit 1
	}
}

function ifme {
	cd "$DIR"
	mono --jitmap ifme.exe
	exit 0
}

checktty
checkmono
ifme
