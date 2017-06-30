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

# check nasm
(nasm -v) < /dev/null > /dev/null 2>&1 || {
	wget http://www.nasm.us/pub/nasm/releasebuilds/2.14rc0/nasm-2.14rc0.tar.xz
	tar -xvf nasm-2.14rc0.tar.xz
	cd nasm-2.14rc0
	./configure
	make
	sudo make install
	cd "$DIR"
}

git clone http://git.videolan.org/git/x264.git
cd x264
./configure --enable-static --bit-depth=8
make
mv x264 x264-08
./configure --enable-static --bit-depth=10
make
mv x264 x264-10
cd "$DIR"
