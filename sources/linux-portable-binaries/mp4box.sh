#!/bin/sh

export CFLAGS="-Os -fstack-protector --param=ssp-buffer-size=4 -fno-strict-aliasing -ffunction-sections -fdata-sections -D_FORTIFY_SOURCE=2 -I."
export LDFLAGS="-Wl,-z,relro -Wl,-z,defs -Wl,--gc-sections -Wl,--as-needed"

mkflags=-j4

set -v
sudo apt-get install zlib1g-dev

git clone --depth 1500 https://github.com/gpac/gpac
cd gpac
# checkout latest release tag
version=$(git describe --abbrev=0 --tags)
git checkout $version

set -e

./configure --verbose \
	--enable-static-bin --static-modules --static-mp4box --disable-pic \
	--disable-jack --disable-pulseaudio --disable-ssl --disable-x11 \
	--use-js=no --use-ft=no --use-jpeg=no --use-png=no --use-faad=no --use-mad=no --use-xvid=no \
	--use-ffmpeg=no --use-ogg=no --use-vorbis=no --use-theora=no --use-openjpeg=no --use-a52=no \
	--extra-cflags="$CFLAGS" \
	--extra-ldflags="$LDFLAGS" \
	--extra-libs="-lz -lm"
# `--static-mp4box --disable-pic' doesn't seem to do the trick
sed -i 's|-fPIC -DPIC||g' config.mak
echo 'MP4BOX_STATIC=yes' >> config.mak

# libgpac_static.a (ignore the error messages)
make lib $mkflags || true
# mp4box
make -C applications/mp4box $mkflags

strip bin/gcc/MP4Box
cp bin/gcc/MP4Box ..

cd ..
rm -rf gpac

set +v
echo
echo "version: $version"

