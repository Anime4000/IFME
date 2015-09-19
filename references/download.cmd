@echo off
cd "%~dp0"

SET WGET32=%SYSTEMROOT%\System32\wget.exe

echo Checking %WGET32%
IF EXIST "%WGET32%" (
	echo %WGET32% found!
) ELSE (
	echo %WGET32% not found :(
	pause
	exit 1
)

wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/INIFileParser.dll -O "INIFileParser.dll"
wget --no-check-certificate https://github.com/x265/MediaInfoDotNet/releases/download/v0.7.8/MediaInfoDotNet.dll -O "MediaInfoDotNet.dll"
echo Done!!
TIMEOUT /T 3