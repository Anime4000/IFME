@echo off

cd "%~dp0"

IF /I "%1"=="" goto HELP
IF /I "%1"=="debug" goto DEBUG
IF /I "%1"=="release" goto RELEASE

:HELP
echo Valid command are:
echo %~n0 debug
echo %~n0 release
pause
exit

:DEBUG
SET CompileMode=Debug
goto MAKEIT

:RELEASE
SET CompileMode=Release
goto MAKEIT

:MAKEIT
@title Build IFME - %CompileMode%
SET UNZIP="%PROGRAMFILES%\7-Zip\7z.exe"
SET WGET="%SYSTEMROOT%\System32\wget.exe"
SET ISCC="%PROGRAMFILES(X86)%\Inno Setup 5\iscc.exe"
SET BUILDDIR=build
SET MSBuildVer=14.0

:: Compiler of choice
:: Visual Studio 2015 or Mono 4.0

SET NETCOMPILER="%ProgramFiles(x86)%\MSBuild\%MSBuildVer%\Bin\amd64\MSBuild.exe"
:: SET NETCOMPILER="%ProgramFiles(x86)%\Mono\bin\xbuild.exe"

cls

echo.
echo.
echo.
echo This script allowing publish IFME after compile. Using %CompileMode% build.
echo Be sure ifme.exe in %CompileMode% is working perfectly including addons.
echo.
echo.
echo Don't forget to close any running Visual Studio before compile
echo Press any key to start making (existing folder will be removed!)...
echo.
echo.
echo Make sure you have wget & 7zip installed on your computer.
echo.
echo.
echo.
pause

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

timeout /t 3 >nul

@title DELETEING %BUILDDIR%!
echo DELETEING %BUILDDIR%!
rmdir /s /q "%BUILDDIR%"
mkdir "%BUILDDIR%"
timeout /t 1 >nul
echo.

@title COMPILING IFME
echo COMPILING IFME
call %NETCOMPILER% /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=%CompileMode%
timeout /t 5 >nul
echo.

@title COPY IFME MAIN FILE
echo COPY IFME MAIN FILE
mkdir %BUILDDIR%\benchmark
mkdir %BUILDDIR%\extension
robocopy ifme\lang %BUILDDIR%\lang /E
robocopy ifme\profile %BUILDDIR%\profile /E
robocopy ifme\sounds %BUILDDIR%\sounds /E
copy ifme\addons_linux32.repo %BUILDDIR%
copy ifme\addons_linux64.repo %BUILDDIR%
copy ifme\addons_windows32.repo %BUILDDIR%
copy ifme\addons_windows64.repo %BUILDDIR%
copy ifme\avisynthsource.code %BUILDDIR%
copy ifme\format.ini %BUILDDIR%
copy ifme\iso.code %BUILDDIR%
copy sources\metauser.if %BUILDDIR%

@title COPY COMPILED
echo COPY COMPILED
copy ifme\bin\%CompileMode%\ifme.exe %BUILDDIR%
copy ifme\bin\%CompileMode%\INIFileParser.dll %BUILDDIR%
copy ifme\bin\%CompileMode%\MediaInfoDotNet.dll %BUILDDIR%
copy ifme\bin\%CompileMode%\FFmpegDotNet.dll %BUILDDIR%
echo.

@title COPY EXTENSION
echo COPY EXTENSION
robocopy prerequisite\allos\extension %BUILDDIR%\extension /E

@title CLEAN UP
echo CLEAN UP
del /f /q %BUILDDIR%\ifme.pdb
del /f /q %BUILDDIR%\ifme.exe.config
del /f /q %BUILDDIR%\ifme.vshost.exe
del /f /q %BUILDDIR%\ifme.vshost.exe.config
del /f /q %BUILDDIR%\ifme.vshost.exe.manifest
del /f /q %BUILDDIR%\ifme.imouto.pdb
timeout /t 3 >nul
echo.

@title COPY DOCUMENTS
echo COPY DOCUMENTS
copy changelog.txt %BUILDDIR%
copy license.txt %BUILDDIR%
copy patents.txt %BUILDDIR%
echo.

@title CLEAN UP
echo CLEAN UP
del /f /s /q %BUILDDIR%\*.ifx

echo.
echo.
echo.

@title Creating Installer
echo Creating Installer
%ISCC% installer.iss
echo Done!!!
timeout /t 10