#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by darealshinji https://github.com/darealshinji
# Modified by Anime4000 https://github.com/Anime4000
#
# This script allow to build least static binary
# ensure more portable, originally created by darealshinji
# modify for latest version

PASSWORD="142536"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "$PASSWORD" | sudo -S apt-get install mercurial cmake cmake-curses-gui build-essential yasm -y

rm -rf x265
hg clone https://bitbucket.org/multicoreware/x265
cd "x265/build/linux"

cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON
make
mv x265 x265-08
make clean

if [ $(uname -m | grep '64') ]; then
	cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=OFF
	make
	mv x265 x265-10
	make clean

	cmake -G "Unix Makefiles" ../../source -DENABLE_SHARED=OFF -DSTATIC_LINK_CRT=ON -DENABLE_CLI=ON -DHIGH_BIT_DEPTH=ON -DMAIN12=ON
	make
	mv x265 x265-12
	make clean
fi

echo "DONE!"
