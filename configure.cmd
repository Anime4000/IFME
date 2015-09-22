@echo off
@title Configure IFME Builds

cd "%~dp0"

SET UNZIP="%PROGRAMFILES%\7-Zip\7z.exe"
SET WGET="%SYSTEMROOT%\System32\wget.exe"

echo Checking %WGET%
IF EXIST %WGET% (
	echo %WGET% found!
) ELSE (
	echo %WGET% not found :(
	pause
	exit 1
)

echo Checking %UNZIP%
IF EXIST %UNZIP% (
	echo %UNZIP% found!
) ELSE (
	echo %UNZIP% not found :(
	pause
	exit 1
)

echo Creating folders
mkdir prerequisite\windows\32bit\plugins\
mkdir prerequisite\windows\64bit\plugins\
mkdir prerequisite\allos\extension\

wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/INIFileParser.dll -O "references\INIFileParser.dll"
wget --no-check-certificate https://github.com/x265/MediaInfoDotNet/releases/download/v0.7.8/MediaInfoDotNet.dll -O "references\MediaInfoDotNet.dll"

echo Binaries
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/7za-x86.exe -O "prerequisite\windows\32bit\7za.exe"
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/7za-x64.exe -O "prerequisite\windows\64bit\7za.exe"
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/MediaInfo-x86.dll -O "prerequisite\windows\32bit\MediaInfo.dll"
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/MediaInfo-x64.dll -O "prerequisite\windows\64bit\MediaInfo.dll"

echo Extensions
wget --no-check-certificate https://github.com/x265/HFRGen/releases/download/v0.2/hfrgen.dll -O "prerequisite\allos\extension\hfrgen.dll"
wget --no-check-certificate https://github.com/x265/HoloBenchmark/releases/download/v0.0.2/holobenchmark.dll -O "prerequisite\allos\extension\holobenchmark.dll"
wget --no-check-certificate https://github.com/x265/Nemupad/releases/download/0.0.3.1/nemupad.dll -O "prerequisite\allos\extension\nemupad.dll"

echo Downloading 32bit plugins!
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/avisynth.ifx -O "prerequisite\windows\32bit\plugins\avisynth.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/faac.ifx -O "prerequisite\windows\32bit\plugins\faac.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/ffmpeg.ifx -O "prerequisite\windows\32bit\plugins\ffmpeg.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/ffmsindex.ifx -O "prerequisite\windows\32bit\plugins\ffmsindex.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/flac.ifx -O "prerequisite\windows\32bit\plugins\flac.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/mkvtool.ifx -O "prerequisite\windows\32bit\plugins\mkvtool.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/mp4box.ifx -O "prerequisite\windows\32bit\plugins\mp4box.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/mp4fpsmod.ifx -O "prerequisite\windows\32bit\plugins\mp4fpsmod.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/ogg.ifx -O "prerequisite\windows\32bit\plugins\ogg.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/opus.ifx -O "prerequisite\windows\32bit\plugins\opus.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/32bit/x265msvc.ifx -O "prerequisite\windows\32bit\plugins\x265msvc.ifx"

echo Downloading 64bit plugins!
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/ffmpeg.ifx -O "prerequisite\windows\64bit\plugins\ffmpeg.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/ffmsindex.ifx -O "prerequisite\windows\64bit\plugins\ffmsindex.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/flac.ifx -O "prerequisite\windows\64bit\plugins\flac.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/mkvtool.ifx -O "prerequisite\windows\64bit\plugins\mkvtool.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/mp4box.ifx -O "prerequisite\windows\64bit\plugins\mp4box.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/ogg.ifx -O "prerequisite\windows\64bit\plugins\ogg.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/x265gcc.ifx -O "prerequisite\windows\64bit\plugins\x265gcc.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/x265icc.ifx -O "prerequisite\windows\64bit\plugins\x265icc.ifx"
wget http://sourceforge.net/projects/ifme/files/plugins/windows/64bit/x265msvc.ifx -O "prerequisite\windows\64bit\plugins\x265msvc.ifx"

echo Unpacking
for /r "%~dp0\prerequisite\windows\32bit\plugins" %%i in (*.ifx) do "%UNPACK%" x "%%i" -y -o"%%~dpi"
for /r "%~dp0\prerequisite\windows\64bit\plugins" %%i in (*.ifx) do "%UNPACK%" x "%%i" -y -o"%%~dpi"

echo Delete cache
del /f /s /q *.ifx

echo Done, All OK!
TIMEOUT /T 3