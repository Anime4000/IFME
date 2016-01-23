@echo off
@title Create IFME plugins package

:: Require 7z (7zip) to be installed and PATH is set

cd "windows"
cd "32bit"
7z a -t7z faac.ifx -r faac\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z ffmpeg.ifx -r ffmpeg32\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z ffmsindex.ifx -r ffmsindex\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z flac.ifx -r flac\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z mkvtoolnix.ifx -r mkvtoolnix\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z mp4box.ifx -r mp4box\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z mp4fpsmod.ifx -r mp4fpsmod\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z ogg.ifx -r ogg\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z opus.ifx -r opus\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z x265msvc.ifx -r x265msvc\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2

cd ".."
cd "64bit"
7z a -t7z ffmpeg.ifx -r ffmpeg64\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z ffmsindex.ifx -r ffmsindex\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z flac.ifx -r flac\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z mkvtoolnix.ifx -r mkvtoolnix\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z mp4box.ifx -r mp4box\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z ogg.ifx -r ogg\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z x265gcc.ifx -r x265gcc\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z x265icc.ifx -r x265icc\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2
7z a -t7z x265msvc.ifx -r x265msvc\ -mx=9 -mm=lzma2 -md=256m -ms=on -mmt=2

echo YAY