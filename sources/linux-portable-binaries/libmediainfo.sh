#!/bin/sh

commonflags="-Wall -Os -fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."

export CXXFLAGS="$commonflags"
export CFLAGS="$commonflags"
export LDFLAGS="-Wl,-Bsymbolic-functions -Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed"

mkflags=-j4

set -v
sudo apt-get install zlib1g-dev

curdir="$PWD"

url=$(wget -q -O - "http://mediaarea.net/MediaInfo/Download/Source" | grep -e 'mediaarea.net/download/binary/mediainfo/' | cut -d '"' -f2)
wget -O MediaInfo.tar.bz2 $url
tar xvf MediaInfo.tar.bz2
rm -f MediaInfo.tar.bz2
cd MediaInfo_CLI_GNU_FromSource

cd ZenLib/Project/GNU/Library
sed -i 's|-O2|-Os|g' configure.ac
autoreconf -if 2>/dev/null
set -e
./configure
make $mkflags

cd "$curdir"/MediaInfo_CLI_GNU_FromSource/MediaInfoLib/Project/GNU/Library/
sed -i 's|-O2|-Os|g' configure.ac
set +e
autoreconf -if 2>/dev/null
set -e
./configure --disable-static --enable-shared
make $mkflags

chmod a-x .libs/*
cp -f .libs/libmediainfo.so.0 "$curdir"
cd "$curdir"
rm -rf MediaInfo_CLI_GNU_FromSource
strip libmediainfo.so.0

set +v
echo
echo "version: $(echo $url | cut -d '/' -f7)"

