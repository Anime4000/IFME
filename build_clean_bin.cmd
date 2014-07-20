@echo off
@title Clean Compiled IFME
echo This script will remove compiled binary in bin folder
echo No more "up-to-date" in bulid log in VS
pause
del /f /q ifme\bin\x64\Debug\ifme.exe
del /f /q ifme\bin\x64\Debug\ifme.hitoha.dll
del /f /q ifme\bin\x64\Debug\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha\bin\x64\Debug\ifme.hitoha.dll
del /f /q ifme.hitoha\bin\x64\Debug\ifme.hitoha.kawaii.dll
del /f /q ifme.hitoha.kawaii\bin\x64\Debug\ifme.hitoha.kawaii.dll
echo.
echo Done
pause