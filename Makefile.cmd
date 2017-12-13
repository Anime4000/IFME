@echo off
@title Make IFME

cd /d "%~dp0"

set NETCOMPILER="%PROGRAMFILES(X86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe"
set ISCC="%PROGRAMFILES(X86)%\Inno Setup 5\iscc.exe"

set WINRAR="%PROGRAMFILES%\WINRAR\WinRAR.exe"
set SEVENZIP="%PROGRAMFILES%\7-Zip\7z.exe"

set UNZIPCMD1=do not edit
set UNZIPCMD2=do not edit

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

echo Checking Archive Utility (WinRAR)
IF EXIST %WINRAR% (
	set UNZIPCMD1=%WINRAR% x -ibck
	set UNZIPCMD2=
) ELSE IF EXIST %SEVENZIP% (
	set UNZIPCMD1=%SEVENZIP% x
	set UNZIPCMD2=-y -o
) ELSE (
	echo Need 7-zip or WinRAR
	goto FAIL
)

echo Little housekeeping
IF EXIST "%~dp0\build" (
	rmdir /s /q "build"
)

IF EXIST "%~dp0\prerequisite\32bit" (
	rmdir /s /q "prerequisite\32bit"
)

IF EXIST "%~dp0\prerequisite\64bit" (
	rmdir /s /q "prerequisite\64bit"
)

echo Compiling...
call %NETCOMPILER% /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=Release
timeout /t 5 >nul

echo Copying main files
mkdir build
mkdir prerequisite\32bit
mkdir prerequisite\64bit

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

echo Unpacking prerequisite files
%UNZIPCMD1% prerequisite\plugin32.tar.xz %UNZIPCMD2%"prerequisite\32bit"
%UNZIPCMD1% prerequisite\plugin64.tar.xz %UNZIPCMD2%"prerequisite\64bit"

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