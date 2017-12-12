#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by Anime4000 @ https://github.com/Anime4000

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

if $(uname -m | grep '64'); then
	echo "This script only for 32-bit OS,"
	echo "You have 64-bit OS, which is awesome :)"
	echo ""
	echo "Poor 32-bit users, they should use 64-bit"
	echo ""
	exit 1
else
	echo ""
	echo "================================================="
	echo "  This script will make fake 64-bit of FFmpeg    "
	echo "  by creating symlink back to 32-bit of FFmpeg.  "
	echo "  Allow 64-bit plugin into 32-bit OS             "
	echo "================================================="
	echo ""

	cd "$DIR/plugin/ffmpeg64"
	echo "Current folder is $PWD"

	ln "..\ffmpeg32\ffmpeg" ffmpeg
	ln "..\ffmpeg32\ffmpeg.exe" ffmpeg.exe
	ln "..\ffmpeg32\ffprobe" ffprobe
	ln "..\ffmpeg32\ffprobe.exe" ffprobe.exe
	mv "_plugin.json.off" "_plugin.json"

	echo "DONE..."
	exit 0
fi
