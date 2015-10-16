#!/bin/sh

commonflags="-Os -fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."

export CXXFLAGS="$commonflags"
export CFLAGS="$commonflags"
export LDFLAGS="-s -Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed"

mkflags=-j4

set -v

sudo apt-get install ruby1.9.3 libmagic-dev zlib1g-dev \
	libboost-filesystem-dev libboost-system-dev \
	libboost-regex-dev libboost-date-time-dev

mkdir mkvtoolnix
cd mkvtoolnix
git clone --depth 800 https://github.com/mbunkus/mkvtoolnix
cd mkvtoolnix

# checkout latest release tag
version=$(git describe --abbrev=0 --tags)
git checkout $version
git submodule init
git submodule update

cd ..
git clone https://git.xiph.org/ogg.git
git clone https://git.xiph.org/vorbis.git
git clone https://git.xiph.org/flac.git

curdir="$PWD"

set -e

cd ogg
./autogen.sh
./configure --prefix="$curdir/libs" --disable-shared
make $mkflags
make install

cd ../vorbis
./autogen.sh
./configure --prefix="$curdir/libs" --disable-shared --with-ogg="$curdir/libs"
make $mkflags
make install

cd ../flac
./autogen.sh
./configure --prefix="$curdir/libs" --disable-shared
make $mkflags
make install

cd ../mkvtoolnix
./autogen.sh
./configure --prefix=/usr --enable-magic --without-curl --disable-qt \
	--with-extra-includes="$curdir/libs/include" \
	--with-extra-libs="$curdir/libs/lib"
# link most dependencies statically
sed -i 's|, $common_libs|, " -Wl,-Bstatic ", $common_libs, "-Wl,-Bdynamic "|g' Rakefile
./drake $mkflags apps:mkvmerge apps:mkvextract
cd src
cp mkvmerge mkvextract ../../../

cd ../../..
rm -rf mkvtoolnix

set +v
echo
echo "version: $version"

