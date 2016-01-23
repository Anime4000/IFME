#!/bin/bash

# Require 7z (7zip) to be installed

cd "linux/"
cd "32bit/"
7za a -tzip ffmpeg.ifx -r ffmpeg32/ -mm=lzma -mx=8

cd "../"
cd "64bit/"
7za a -tzip faac.ifx -r faac/ -mm=lzma -mx=8
7za a -tzip ffmpeg.ifx -r ffmpeg64/ -mm=lzma -mx=8
7za a -tzip ffmsindex.ifx -r ffmsindex/ -mm=lzma -mx=8
7za a -tzip mkvtoolnix.ifx -r mkvtoolnix/ -mm=lzma -mx=8
7za a -tzip mp4box.ifx -r mp4box/ -mm=lzma -mx=8
7za a -tzip mp4fpsmod.ifx -r mp4fpsmod/ -mm=lzma -mx=8
7za a -tzip x265gcc.ifx -r x265gcc/ -mm=lzma -mx=8

echo "YAY"