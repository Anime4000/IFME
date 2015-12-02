#!/bin/sh

rm -rf x265
hg clone https://bitbucket.org/multicoreware/x265 x265

cd x265/build
mkdir icl64

# MSYS
cd msys
wget 