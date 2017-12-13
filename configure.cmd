@echo off
@title Configure and Generating stuff

cd /d "%~dp0"

set WINRAR="%PROGRAMFILES%\WINRAR\WinRAR.exe"
set SEVENZIP="%PROGRAMFILES%\7-Zip\7z.exe"
set PLUGIN32=https://sourceforge.net/projects/ifme/files/plugin/plugin-basic_2017-12-12_i686.tar.xz
set PLUGIN64=https://sourceforge.net/projects/ifme/files/plugin/plugin-basic_2017-12-12_amd64.tar.xz

set UNZIPCMD1=do not edit
set UNZIPCMD2=do not edit

echo Checking archive utility 7-zip or WinRAR
IF EXIST %SEVENZIP% goto USE7ZIP
IF EXIST %WINRAR% goto USEWINRAR

echo Archive utility not detected!
goto FAIL

:USE7ZIP
echo %SEVENZIP% found!
set UNZIPCMD1=%SEVENZIP% x
set UNZIPCMD2=-y -o
goto STAGE1

:USEWINRAR
echo %WINRAR% found!
set UNZIPCMD1=%WINRAR% x -ibck
set UNZIPCMD2=
goto STAGE1

:STAGE1
echo Creating folders
mkdir prerequisite

echo Download prerequisite files
IF NOT EXIST "%~dp0\prerequisite\FontReg32.exe" (
	powershell -command "& { (new-object System.Net.WebClient).DownloadFile('https://sourceforge.net/projects/ifme/files/stuff/FontReg32.exe','prerequisite\FontReg32.exe') }"
)

IF NOT EXIST "%~dp0\prerequisite\FontReg64.exe" (
	powershell -command "& { (new-object System.Net.WebClient).DownloadFile('https://sourceforge.net/projects/ifme/files/stuff/FontReg64.exe','prerequisite\FontReg64.exe') }"
)

echo Download plugin
IF NOT EXIST "%~dp0\prerequisite\plugin32.tar.xz" (
	powershell -command "& { (new-object System.Net.WebClient).DownloadFile('%PLUGIN32%','prerequisite\plugin32.tar.xz') }"
)

IF NOT EXIST "%~dp0\prerequisite\plugin64.tar.xz" (
	powershell -command "& { (new-object System.Net.WebClient).DownloadFile('%PLUGIN32%','prerequisite\plugin64.tar.xz') }"
)

echo Complete, now you can make
goto DONE

:DONE
timeout /t 5
exit 0

:FAIL
pause
exit 1