:: This script for building x265 for Internet Friendly Media Encoder
:: compiled binaries will placed on "Release" folder
@echo off
if "%VS120COMNTOOLS%" == "" (
  msg "%username%" "Visual Studio 12 not detected"
  exit 1
)

call "%VS120COMNTOOLS%\..\..\VC\vcvarsall.bat"

:: 8 bit
cmake -G "Visual Studio 12 Win64" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
if exist x265.sln (
  MSBuild /property:Configuration="Release" x265.sln
  move /y Release\x265.exe Release\x265-08.exe
)

:: 10bit
cmake -G "Visual Studio 12 Win64" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
if exist x265.sln (
  MSBuild /property:Configuration="Release" x265.sln
  move /y Release\x265.exe Release\x265-10.exe
)

:: 12bit
cmake -G "Visual Studio 12 Win64" ..\..\source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
if exist x265.sln (
  MSBuild /property:Configuration="Release" x265.sln
  move /y Release\x265.exe Release\x265-12.exe
)