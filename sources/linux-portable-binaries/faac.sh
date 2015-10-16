#!/bin/sh

commonflags="-O2 -fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."

export CXXFLAGS="$commonflags"
export CFLAGS="$commonflags"

mkflags=-j4

version=1.28+cvs20150510
revision=1

set -v
sudo apt-get install quilt

curdir="$PWD"
mkdir faac-build
cd faac-build

set -e

# CVS sucks, so we're downloading a source package from Debian instead
url="http://http.debian.net/debian/pool/non-free/f/faac"
wget "$url/faac_${version}.orig.tar.xz"
wget "$url/faac_${version}-${revision}.debian.tar.xz"
tar xf faac_${version}.orig.tar.xz
cd faac-${version}
tar xf ../faac_${version}-${revision}.debian.tar.xz
# apply patches
QUILT_PATCHES=debian/patches quilt push -a

./bootstrap
./configure --disable-shared
make $mkflags

cd frontend
# re-link binary
g++ -s  -Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed  -o "$curdir/faac"  main.o input.o ../libfaac/.libs/libfaac.a ../common/mp4v2/libmp4v2.a  -lm

cd "$curdir"
rm -rf faac-build

set +v
echo
echo "version: ${version}-${revision}"

