@echo off
@title Deploy requirement
cd %~dp0

SET UNPACK=%PROGRAMFILES%\7-Zip\7z.exe
SET WGET32=%SYSTEMROOT%\System32\wget.exe

echo Creating folders
mkdir windows\32bit\plugins\
mkdir windows\64bit\plugins\
mkdir allos\extension\

echo Checking %WGET32%
IF EXIST "%WGET32%" (
	echo %WGET32% found!
) ELSE (
	echo %WGET32% not found :(
	pause
	exit 1
)

echo Checking %UNPACK%
IF EXIST "%UNPACK%" (
	echo %UNPACK% found!
) ELSE (
	echo %UNPACK% not found :(
	pause
	exit 1
)

echo Binaries
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/7za-x86.exe -O windows\32bit\7za.exe
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/7za-x64.exe -O windows\64bit\7za.exe
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/MediaInfo-x86.dll -O windows\32bit\MediaInfo.dll
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/MediaInfo-x64.dll -O windows\64bit\MediaInfo.dll

echo Downloading 32bit plugins!
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/avisynth.ifx -O windows\32bit\plugins\avisynth.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/faac.ifx -O windows\32bit\plugins\faac.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/ffmpeg.ifx -O windows\32bit\plugins\ffmpeg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/ffmsindex.ifx -O windows\32bit\plugins\ffmsindex.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/flac.ifx -O windows\32bit\plugins\flac.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/mkvtool.ifx -O windows\32bit\plugins\mkvtool.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/mp4box.ifx -O windows\32bit\plugins\mp4box.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/mp4fpsmod.ifx -O windows\32bit\plugins\mp4fpsmod.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/ogg.ifx -O windows\32bit\plugins\ogg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/opus.ifx -O windows\32bit\plugins\opus.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/32bit/x265msvc.ifx -O windows\32bit\plugins\x265msvc.ifx

echo Downloading 64bit plugins!
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/ffmpeg.ifx -O windows\64bit\plugins\ffmpeg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/ffmsindex.ifx -O windows\64bit\plugins\ffmsindex.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/flac.ifx -O windows\64bit\plugins\flac.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/mkvtool.ifx -O windows\64bit\plugins\mkvtool.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/mp4box.ifx -O windows\64bit\plugins\mp4box.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/ogg.ifx -O windows\64bit\plugins\ogg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/x265gcc.ifx -O windows\64bit\plugins\x265gcc.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/x265icc.ifx -O windows\64bit\plugins\x265icc.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/windows/64bit/x265msvc.ifx -O windows\64bit\plugins\x265msvc.ifx

echo Unpacking...
for /r "%~dp0\windows\32bit\plugins" %%i in (*.ifx) do "%UNPACK%" x "%%i" -y -o"%%~dpi"
for /r "%~dp0\windows\64bit\plugins" %%i in (*.ifx) do "%UNPACK%" x "%%i" -y -o"%%~dpi"

echo extensions
wget --no-check-certificate https://github.com/x265/HFRGen/releases/download/v0.2/hfrgen.dll -O "allos\extension\hfrgen.dll"
wget --no-check-certificate https://github.com/x265/HoloBenchmark/releases/download/v0.0.2/holobenchmark.dll -O "allos\extension\holobenchmark.dll"
wget --no-check-certificate https://github.com/x265/Nemupad/releases/download/0.0.3.1/nemupad.dll -O "allos\extension\holobenchmark.dll"

echo Delete cache
del /f /s /q *.ifx

echo Done!
pause
exit 0