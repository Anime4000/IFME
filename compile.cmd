@echo off
@title Build IFME and Release!

cd "%~dp0"

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
echo Make sure you have 7zip installed on your computer.
pause >nul

echo.
echo.
echo.

echo DELETEING %BUILDDIR%!
rmdir /s /q %BUILDDIR%
mkdir "%BUILDDIR%"
timeout /t 1 >nul
echo.

echo COMPILING IFME (VISUAL STUDIO 2015)
start "" /B /D . /WAIT "%ProgramFiles(x86)%\MSBuild\%MSBuildVer%\Bin\amd64\MSBuild.exe" /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=%CompileMode%
timeout /t 3 >nul
echo.

echo COPY IFME MAIN FILE
mkdir %BUILDDIR%\benchmark
robocopy ifme\bin\%CompileMode%\extension %BUILDDIR%\extension /E
robocopy ifme\bin\%CompileMode%\lang %BUILDDIR%\lang /E
robocopy ifme\bin\%CompileMode%\profile %BUILDDIR%\profile /E
robocopy ifme\bin\%CompileMode%\sounds %BUILDDIR%\sounds /E
copy ifme\bin\%CompileMode%\addons_linux32.repo %BUILDDIR%
copy ifme\bin\%CompileMode%\addons_linux64.repo %BUILDDIR%
copy ifme\bin\%CompileMode%\addons_windows32.repo %BUILDDIR%
copy ifme\bin\%CompileMode%\addons_windows64.repo %BUILDDIR%
copy ifme\bin\%CompileMode%\avisynthsource.code %BUILDDIR%
copy ifme\bin\%CompileMode%\format.ini %BUILDDIR%
copy ifme\bin\%CompileMode%\ifme.exe %BUILDDIR%
copy ifme\bin\%CompileMode%\INIFileParser.dll %BUILDDIR%
copy ifme\bin\%CompileMode%\iso.code %BUILDDIR%
copy ifme\bin\%CompileMode%\MediaInfo.dll %BUILDDIR%
copy ifme\bin\%CompileMode%\MediaInfoDotNet.dll %BUILDDIR%
echo.

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

echo COPY DOCUMENTS
copy changelog.txt %BUILDDIR%
copy license.txt %BUILDDIR%
copy patents.txt %BUILDDIR%
echo.

echo CLEAN UP
del /f /s /q %BUILDDIR%\*.ifz

echo Next... Making packaging (.7z). Get ready first
timeout /t 3 >nul



echo PACKAGING
ren %BUILDDIR% ifme5
"%PROGRAMFILES%\7-Zip\7z.exe" a -r -t7z -mx=9 -ms=on ifme5-x64.7z ifme5
ren ifme5 %BUILDDIR%

echo.
echo.
echo.
echo If got error, please check what you missed.
echo All ok? Now can be release via Installer or Archive :)
timeout /t 10