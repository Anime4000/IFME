@echo off
@title Build IFME and Release!
set CompileMode=Release
set BUILDDIR=_build
set MSBuildVer=12.0
cls
echo.
echo This script allowing publish IFME after compile. Using %CompileMode% build.
echo                   1. ifme.exe
echo                   2. ifme.hitoha.dll
echo                   3. ifme.hitoha.kawaii.dll
echo.
echo Please download these file and put on "prerequisite" folder!
echo                   1. addons\* (all addons)
echo                   2. MediaInfo.dll (64bit)
echo                   3. 7za.exe (64bit)
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
del /f /q ifme\bin\x64\%CompileMode%\ifme.exe
del /f /q ifme\bin\x64\%CompileMode%\ifme.hitoha.dll
del /f /q ifme\bin\x64\%CompileMode%\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha\bin\x64\%CompileMode%\ifme.hitoha.dll
del /f /q ifme.hitoha\bin\x64\%CompileMode%\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha.kawaii\bin\x64\%CompileMode%\ifme.hitoha.kawaii.dll
echo.

echo COMPILING IFME (VISUAL STUDIO 2012/2013)
start "" /B /D . /WAIT "%ProgramFiles(x86)%\MSBuild\%MSBuildVer%\Bin\amd64\MSBuild.exe" /nologo /verbosity:normal ifme.sln /t:Build /p:Configuration=%CompileMode%
echo.

echo COPY IFME MAIN FILE
copy installer\text_addon_license.txt %BUILDDIR%\LICENSE_ADDONS.TXT
copy installer\text_gpl2.txt %BUILDDIR%\LICENSE.TXT
copy ifme\lang\*.* %BUILDDIR%\lang
copy ifme\bin\x64\%CompileMode%\ifme.exe %BUILDDIR%\
copy ifme\bin\x64\%CompileMode%\ifme.hitoha.dll %BUILDDIR%\
copy ifme\bin\x64\%CompileMode%\ifme.hitoha.kawaii.dll %BUILDDIR%\
copy ifme\bin\x64\%CompileMode%\iso.gg %BUILDDIR%\
echo.

echo COPY PREREQUISITE
copy prerequisite\MediaInfo.dll %BUILDDIR%\MediaInfo.dll
copy prerequisite\7za.exe %BUILDDIR%\unpack.exe
echo.

echo COPY ADDONS
xcopy /i /s prerequisite\addons\* %BUILDDIR%\addons
echo.

echo CLEAN UP
del /f /s /q %BUILDDIR%\*.ifz

echo.
echo.
echo If got error, please check what you missed.
echo All ok? Now can be release via Installer or Archive :)
echo.
pause