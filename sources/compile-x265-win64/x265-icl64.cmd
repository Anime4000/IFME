@echo off
::
:: run this batch file to create an Intel C++ 2016 NMake makefile for this project.
:: See the cmake documentation for other generator targets
::

if "%VS120COMNTOOLS%" == "" (
  msg "%username%" "Visual Studio 12 not detected"
  exit 1
)

if "%ICPP_COMPILER16%" == "" (
  msg "%username%" "Intel C++ 2016 not detected"
  pause
  exit 1
)

call "%ICPP_COMPILER16%\bin\compilervars.bat" intel64

set CC=icl /Qstd=c++11 /Qoption,cpp,--no_user_defined_literals
set CXX=icl /Qstd=c++11 /Qoption,cpp,--no_user_defined_literals

:: 8bit
cmake -G "NMake Makefiles" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
nmake
move /y x265.exe x265-08.exe

:: 10bit
cmake -G "NMake Makefiles" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
nmake
move /y x265.exe x265-10.exe

:: 12bit
cmake -G "NMake Makefiles" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
nmake
move /y x265.exe x265-12.exe

pause