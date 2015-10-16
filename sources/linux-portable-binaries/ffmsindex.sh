#!/bin/sh

commonflags="-fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."
mkflags=-j4

export LDFLAGS="-Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed"

set -v
sudo apt-get install zlib1g-dev

mkdir ffms2
cd ffms2
curdir="$PWD"

set -e

git clone --depth 1 "git://source.ffmpeg.org/ffmpeg.git"
cd ffmpeg
CFLAGS="$commonflags" ./configure --prefix="$curdir/libs" \
  --disable-programs --disable-doc --disable-bzlib --disable-lzma --disable-xlib \
  --disable-d3d11va --disable-dxva2 --disable-vaapi --disable-vda --disable-vdpau \
  --disable-debug --disable-asm --disable-encoders
# -O2 should be enough
sed -i 's|-O3|-O2|g' config.mak
make $mkflags
make install
cd ..

git clone "https://github.com/FFMS/ffms2.git"
cd ffms2
# checkout latest release tag
version=$(git describe --abbrev=0 --tags)
git checkout $version
autoreconf -ivf
PKG_CONFIG_PATH="$curdir/libs/lib/pkgconfig" \
CXXFLAGS="-O2 $commonflags -Wno-missing-field-initializers" \
  ./configure --prefix="$curdir/libs" --disable-shared --disable-debug
make $mkflags

strip src/index/ffmsindex
mv -f src/index/ffmsindex ../../
cd ../../
rm -rf ffms2

set +v
echo
echo "version: ${version}"

