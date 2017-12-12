:: Internet Friendly Media Encoder, 2017.
:: Created by Anime4000 @ https://github.com/Anime4000

@echo off
@title Fake FFmpeg 64-bit (32-bit Compatiblity Layer)

reg Query "HKLM\Hardware\Description\System\CentralProcessor\0" | find /i "x86" > NUL && set OS=32BIT || set OS=64BIT

if %OS%==32BIT goto 32BIT
if %OS%==64BIT goto 64BIT

:32BIT
echo.
echo =================================================
echo ^| This script will make fake 64-bit of FFmpeg   ^|
echo ^| by creating symlink back to 32-bit of FFmpeg. ^|
echo ^| Allow 64-bit plugin into 32-bit OS            ^|
echo =================================================
echo.

net session >nul 2>&1
if %errorLevel% == 0 (
	echo SUCCESS: Administrative permissions confirmed.
	goto BEGIN
) else (
	echo FAILURE: Current permissions inadequate. Please run as admin.
	goto FAIL1
)

:BEGIN
echo.
cd /d "%~dp0\plugin\ffmpeg64"
echo Current folder is %cd%

mklink "ffmpeg" "..\ffmpeg32\ffmpeg"
mklink "ffmpeg.exe" "..\ffmpeg32\ffmpeg.exe"
mklink "ffprobe" "..\ffmpeg32\ffprobe"
mklink "ffprobe.exe" "..\ffmpeg32\ffprobe.exe"
ren "_plugin.json.off" "_plugin.json"

echo.
echo DONE...
echo.

pause
exit 0

:FAIL1
pause
exit 1

:64BIT
echo This script only for 32-bit OS,
echo You have 64-bit OS, which is awesome :)
echo.
echo Poor 32-bit users, they should use 64-bit
echo.
pause
exit 99