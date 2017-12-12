#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by darealshinji https://github.com/darealshinji
# Modified by Anime4000 https://github.com/Anime4000
#
# This script allow to build least static binary
# ensure more portable, originally created by darealshinji
# modify for latest version

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

cd "$DIR"
rm -rf x265
hg clone --insecure http://bitbucket.org/multicoreware/x265
cp -vrf "$DIR/x265/build/msys" "$DIR/x265/build/msys32"
cp -vrf "$DIR/x265/build/msys" "$DIR/x265/build/msys64"

cd "$DIR/x265/build/msys32"
cmake -G "MSYS Makefiles" ../../source -DWINXP_SUPPORT=ON -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
make
mv x265.exe "$DIR/x265-08_xp86.exe"


cd "$DIR/x265/build/msys64"
cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
make
mv x265.exe "$DIR/x265-08.exe"
make clean

cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
make
mv x265.exe "$DIR/x265-10.exe"
make clean

cmake -G "MSYS Makefiles" -DCMAKE_TOOLCHAIN_FILE=toolchain-x86_64-w64-mingw32.cmake ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
make
mv x265.exe "$DIR/x265-12.exe"
make clean

echo "DONE!"