#!/bin/sh

# This script for building x265 for Internet Friendly Media Encoder
# compiled binaries will place here

# 8bit
cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
make
mv x265.exe x265-08.exe

# 10bit
cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
make
mv x265.exe x265-10.exe

# 12bit
cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
make
mv x265.exe x265-12.exe