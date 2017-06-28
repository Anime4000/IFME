#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by darealshinji https://github.com/darealshinji
# Modified by Anime4000 https://github.com/Anime4000
#
# This script allow to build least static binary
# ensure more portable, originally created by darealshinji
# modify for latest version

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

commonflags="-Os -fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."

export CXXFLAGS="$commonflags"
export CFLAGS="$commonflags"
export LDFLAGS="-s -Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed"

# https://svn.boost.org/trac10/ticket/12132?replyto=1
if [ $(uname -m | grep '64') ]; then
	BIT="x86_64"
else
	BIT="i386"
fi

echo "OS is $BIT"

sudo apt-get install build-essential software-properties-common python2.7 git libssl-dev autoconf clang ruby rake libtool libtool-bin zlib1g-dev libxslt-dev xsltproc docbook-xsl liblzo2-dev libbz2-dev libmagic-dev po4a libicu-dev gawk doxygen -y

echo "Checking Ogg Dev"
if [ ! -f "/usr/local/include/ogg/ogg.h" ]; then
	echo "Ogg Dev not found, building..."
	git clone https://github.com/xiph/ogg
	cd ogg
	git checkout v1.3.2
	./autogen.sh
	./configure --enable-static=yes --enable-shared=no
	make -j4
	sudo make install
	cd "$DIR"
fi

echo "Checking Vorbis dev"
if [ ! -f "/usr/local/include/vorbis/codec.h" ]; then
	echo "Vorbis dev not found, building..."
	git clone https://github.com/xiph/vorbis
	cd vorbis
	git checkout v1.3.5
	./autogen.sh
	./configure --enable-static=yes --enable-shared=no
	make -j4
	sudo make install
	cd "$DIR"
fi

echo "Checking FLAC dev"
if [ ! -f "/usr/local/include/FLAC/format.h" ]; then
	echo "FLAC dev not found, building..."
	git clone https://github.com/xiph/flac
	cd flac
	./autogen.sh
	./configure --enable-static=yes --enable-shared=no
	make -j4
	sudo make install
	cd "$DIR"
fi

echo "Checking libboost"
if [ ! -d "/usr/local/include/boost" ]; then
	wget https://dl.bintray.com/boostorg/release/1.64.0/source/boost_1_64_0.tar.gz
	tar -xvf boost_1_64_0.tar.gz
	cd boost_1_64_0
	./bootstrap.sh --with-icu=/usr/include/$BIT-linux-gnu --with-libraries=date_time,regex,filesystem,system,math
	sudo ./bjam toolset=gcc link=static threading=single install
	cd "$DIR"
fi

git clone https://github.com/mbunkus/mkvtoolnix
cd mkvtoolnix
git checkout $(git describe --abbrev=0 --tags)
git submodule init
git submodule update
./autogen.sh
./configure --enable-static --enable-magic --without-curl --disable-qt
sleep 3
sed -i 's|^BOOST_SYSTEM_LIB = -lboost_system|BOOST_SYSTEM_LIB = -lboost_system -lpthread -pthread|g' build-config
sed -i 's|^MANPAGES_TRANSLATIONS = .*|MANPAGES_TRANSLATIONS =|g' build-config
rake -j6
cd "$DIR"