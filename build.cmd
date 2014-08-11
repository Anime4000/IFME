@echo off
@title Build IFME and Release!
set BUILDDIR=_build
set MSBuildVer=12.0
cls
echo.
echo This script allowing publish IFME after compile. Using Debug build.
echo because "Release" got some issue with x265...
echo                   1. ifme.exe
echo                   2. ifme.hitoha.dll
echo                   3. ifme.hitoha.kawaii.dll
echo.
echo Please download these file and put on "prerequisite" folder!
echo                   1. addons\* (all addons)
echo                   2. MediaInfo.dll (64bit)
echo                   3. 7za.exe (rename to za.dll)
echo.
echo.
echo IFME will use dummy addon, you need actual addon, get from webpage
echo.

echo Press any key to start making (existing folder will be removed!)...
pause >nul

echo.
echo.
echo.

echo DELETEING %BUILDDIR%!
rmdir /s /q %BUILDDIR%
mkdir %BUILDDIR%
mkdir %BUILDDIR%\addons
mkdir %BUILDDIR%\lang
echo.

echo CLEAN PREVIOUS BUILD
del /f /q ifme\bin\x64\Debug\ifme.exe
del /f /q ifme\bin\x64\Debug\ifme.hitoha.dll
del /f /q ifme\bin\x64\Debug\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha\bin\x64\Debug\ifme.hitoha.dll
del /f /q ifme.hitoha\bin\x64\Debug\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha.kawaii\bin\x64\Debug\ifme.hitoha.kawaii.dll
echo.

echo COMPILING IFME (VISUAL STUDIO 2012/2013)
start "" /B /D . /WAIT "%ProgramFiles(x86)%\MSBuild\%MSBuildVer%\Bin\amd64\MSBuild.exe" /nologo /verbosity:normal ifme.sln
echo.

echo COPY IFME MAIN FILE
copy installer\text_gpl2.txt %BUILDDIR%\LICENSE
copy ifme\bin\x64\Debug\lang\*.* %BUILDDIR%\lang
copy ifme\bin\x64\Debug\ifme.exe %BUILDDIR%\
copy ifme\bin\x64\Debug\ifme.hitoha.dll %BUILDDIR%\
copy ifme\bin\x64\Debug\ifme.hitoha.kawaii.dll %BUILDDIR%\
copy ifme\bin\x64\Debug\iso.gg %BUILDDIR%\
echo.

echo COPY PREREQUISITE
copy prerequisite\MediaInfo.dll %BUILDDIR%\MediaInfo.dll
copy prerequisite\7za.exe %BUILDDIR%\unpack.exe
echo.

echo COPY ADDONS
xcopy /i /s prerequisite\addons\* %BUILDDIR%\addons

echo.
echo.
echo If got error, please check what you missed.
echo All ok? Now can be release via Installer or Archive :)
echo.
pause