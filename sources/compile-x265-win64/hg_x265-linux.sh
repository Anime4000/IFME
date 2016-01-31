#!/bin/sh
this=`pwd`

rm -rf x265/
hg clone https://bitbucket.org/multicoreware/x265 x265/

cd x265/build/linux

cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
make
mv x265 x265-08

cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
make
mv x265 x265-10

cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
make
mv x265 x265-12

cd "$this"