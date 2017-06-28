#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by darealshinji https://github.com/darealshinji
# Modified by Anime4000 https://github.com/Anime4000
#
# This script allow to build least static binary
# ensure more portable, originally created by darealshinji
# modify for latest version

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# https://svn.boost.org/trac10/ticket/12132?replyto=1
if [ $(uname -m | grep '64') ]; then
	BIT="x86_64"
else
	BIT="i386"
fi

echo "OS is $BIT"

sudo apt install build-essential software-properties-common python2.7 git libssl-dev autoconf clang ruby rake libtool libtool-bin zlib1g-dev libxslt-dev xsltproc docbook-xsl liblzo2-dev libbz2-dev po4a libicu-dev -y

echo "Checking Ogg Dev"
if [ ! -f "/usr/local/include/ogg/ogg.h" ]; then
	echo "Ogg Dev not found, building..."
	git clone https://git.xiph.org/ogg.git
	cd ogg
	./autogen.sh
	./configure --disable-shared
	make
	sudo make install
	cd "$DIR"
fi

echo "Checking Vorbis dev"
if [ ! -f "/usr/local/include/vorbis/codec.h" ]; then
	echo "Vorbis dev not found, building..."
	git clone https://git.xiph.org/vorbis.git
	cd vorbis
	./autogen.sh
	./configure --disable-shared --with-ogg="$LIB"
	make
	sudo make install
	cd "$DIR"
fi

echo "Checking FLAC dev"
if [ ! -f "/usr/local/include/FLAC/format.h" ]; then
	echo "FLAC dev not found, building..."
	git clone https://git.xiph.org/flac.git
	cd flac
	./autogen.sh
	./configure --disable-shared
	make
	sudo make install
	cd "$DIR"
fi

git clone https://github.com/threatstack/libmagic
cd libmagic
./configure --enable-static
make
sudo make install
cd "$DIR"

wget https://dl.bintray.com/boostorg/release/1.63.0/source/boost_1_63_0.tar.gz
tar -xvf boost_1_63_0.tar.gz
cd boost_1_63_0
./bootstrap.sh --with-icu=/usr/include/$BIT-linux-gnu --with-libraries=date_time,regex,filesystem,system,math
sudo ./bjam toolset=gcc link=static threading=single install
cd "$DIR"

git clone https://github.com/mbunkus/mkvtoolnix
cd mkvtoolnix
git checkout $(git describe --abbrev=0 --tags)
git submodule init
git submodule update
./autogen.sh
./configure --enable-magic --without-curl --disable-qt
sed -i 's|, $common_libs|, " -Wl,-Bstatic ", $common_libs, "-Wl,-Bdynamic "|g' Rakefile
rake
cd "$DIR"
