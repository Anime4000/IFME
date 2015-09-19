@echo off
@title Build IFME and Release!

cd "%~dp0"

set ISCC="%PROGRAMFILES(X86)%\Inno Setup 5\iscc.exe"
set CompileMode=Debug
set BUILDDIR=build
set MSBuildVer=14.0

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

:AGAIN
set /p answer=Do you wish to download all required item before compile? (Y/n): 
if /i "%answer%" EQU "n" goto DODL
if /i "%answer%" EQU "Y" goto DODL
if /i "%answer%" EQU "n" goto SKIPDL
if /i "%answer%" EQU "N" goto SKIPDL
echo Please type Y for Yes or N for No
goto AGAIN


:DODL
cd prerequisite
deploy.cmd
cd ../references
download.cmd
cd ..
goto SKIPDL

:SKIPDL
@title DELETEING %BUILDDIR%!
echo DELETEING %BUILDDIR%!
rmdir /s /q %BUILDDIR%
mkdir "%BUILDDIR%"
timeout /t 1 >nul
echo.

@title COMPILING IFME (VISUAL STUDIO 2015)
echo COMPILING IFME (VISUAL STUDIO 2015)
start "" /B /D . /WAIT "%ProgramFiles(x86)%\MSBuild\%MSBuildVer%\Bin\amd64\MSBuild.exe" /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=%CompileMode%
timeout /t 3 >nul
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

@title COPY COMPILED
echo COPY COMPILED
copy ifme\bin\%CompileMode%\ifme.exe %BUILDDIR%
copy ifme\bin\%CompileMode%\INIFileParser.dll %BUILDDIR%
copy ifme\bin\%CompileMode%\MediaInfoDotNet.dll %BUILDDIR%
echo.

@title COPY EXTENSION
echo COPY EXTENSION
robocopy prerequisite\allos\extension %BUILDDIR% /E

@title CLEAN UP
echo CLEAN UP
del /f /q %BUILDDIR%\ifme.pdb
del /f /q %BUILDDIR%\ifme.exe.config
del /f /q %BUILDDIR%\ifme.vshost.exe
del /f /q %BUILDDIR%\ifme.vshost.exe.config
del /f /q %BUILDDIR%\ifme.vshost.exe.manifest
del /f /q %BUILDDIR%\ifme.imouto.pdb
del /f /q %BUILDDIR%\metauser.if
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