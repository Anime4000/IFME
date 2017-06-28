#!/bin/bash

# Internet Friendly Media Encoder, 2017.
# Created by darealshinji https://github.com/darealshinji
# Modified by Anime4000 https://github.com/Anime4000
#
# This script allow to build least static binary
# ensure more portable, originally created by darealshinji
# modify for latest version

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

sudo apt install build-essential software-properties-common python2.7 git libssl-dev autoconf clang ruby rake libtool libtool-bin zlib1g-dev libxslt-dev xsltproc docbook-xsl liblzo2-dev libbz2-dev po4a libicu-dev -y

wget https://github.com/gpac/gpac/archive/v0.7.0.tar.gz
tar -xvf v0.7.0.tar.gz
cd gpac-0.7.0
./configure --enable-static-bin --static-mp4box --static-modules
make
cd "$DIR"

mv "gpac-0.7.0/bin/gcc/MP4Box" "gpac-0.7.0/bin/gcc/mp4box"
