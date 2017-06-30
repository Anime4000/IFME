#!/bin/bash

PASSWORD="142536"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

echo "$PASSWORD" | sudo -S apt install --no-install-recommends rake libbz2-dev liblzo2-dev \
zlib1g-dev libmagic-dev libflac-dev libogg-dev libvorbis-dev libboost-dev \
libboost-filesystem-dev libboost-regex-dev libboost-date-time-dev \
libboost-system-dev pkg-config po4a docbook-xsl xsltproc -y

git clone https://github.com/mbunkus/mkvtoolnix
cd mkvtoolnix
git checkout $(git describe --abbrev=0 --tags)
git submodule init
git submodule update
./autogen.sh
./configure --without-gettext --disable-qt --enable-static
sed -i 's|^BOOST_SYSTEM_LIB = -lboost_system|BOOST_SYSTEM_LIB = -lboost_system -lpthread -pthread|g' build-config
sed -i 's|^MANPAGES_TRANSLATIONS = .*|MANPAGES_TRANSLATIONS =|g' build-config
sleep 5
rake -j4
strip src/*

cd "$DIR"