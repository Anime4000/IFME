@echo off
@title Make IFME

cd /d "%~dp0"

set NETCOMPILER="%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
set ISCC="%PROGRAMFILES(X86)%\Inno Setup 5\iscc.exe"

echo Checking Visual Studio
IF NOT EXIST %NETCOMPILER% (
	echo Need Visual Studio 2017
	goto FAIL
)

echo Checking Inno Setup
IF NOT EXIST %ISCC% (
	echo Need Inno Setup 5
	goto FAIL
)

echo Little housekeeping
IF EXIST "%~dp0\build" (
	rmdir /s /q "%~dp0\build"
)

echo Compiling...
call %NETCOMPILER% /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=Release
timeout /t 5 >nul

echo Copying main files
mkdir build
robocopy ifme\bin\Release\branding build\branding /E
robocopy ifme\bin\Release\lang build\lang /E
robocopy ifme\bin\Release\preset build\preset /E
copy ifme\bin\Release\avisynth.json build\
copy ifme\bin\Release\FFmpegDotNet.dll build\
copy ifme\bin\Release\format.json build\
copy ifme\bin\Release\ifme.exe build\
copy ifme\bin\Release\language.json build\
copy ifme\bin\Release\mime.json build\
copy ifme\bin\Release\Newtonsoft.Json.dll build\
copy ifme\bin\Release\targetfmt.json build\

copy sources\ffmpeg64_32layer.cmd build\
copy sources\ffmpeg64_32layer.sh build\
copy sources\ifme.desktop build\
copy sources\ifme.sh build\

copy prerequisite\FontReg32.exe build\
copy prerequisite\FontReg64.exe build\

copy changelog.txt build\
copy license.txt build\
copy patents.txt build\

echo Creating Installer
%ISCC% installer.iss

echo Making build folder working
robocopy prerequisite\64bit\plugin build\plugin /E

echo Complete!
goto OKAY

:OKAY
timeout /t 5
exit 0

:FAIL
pause
exit 1