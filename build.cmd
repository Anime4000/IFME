@echo off
@title Build IFME and Release!
set BUILDDIR=_build
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
timeout /T 3 /NOBREAK
mkdir %BUILDDIR%
mkdir %BUILDDIR%\addons
mkdir %BUILDDIR%\lang
echo.
echo Copy IFME main file
copy installer\text_gpl2.txt %BUILDDIR%\LICENSE
copy ifme\bin\x64\Debug\lang\*.* %BUILDDIR%\lang
copy ifme\bin\x64\Debug\ifme.exe %BUILDDIR%\
copy ifme\bin\x64\Debug\ifme.hitoha.dll %BUILDDIR%\
copy ifme\bin\x64\Debug\ifme.hitoha.kawaii.dll %BUILDDIR%\
copy ifme\bin\x64\Debug\iso.gg %BUILDDIR%\
echo.
echo Copy Prerequisite
copy prerequisite\MediaInfo.dll %BUILDDIR%\MediaInfo.dll
copy prerequisite\7za.exe %BUILDDIR%\za.dll
echo.
echo Copy Addons
xcopy /i /s prerequisite\addons\* %BUILDDIR%\addons
echo.
echo.
echo If got error, please check what you missed.
echo All ok? Now can be release via Installer or Archive :)
echo.
pause