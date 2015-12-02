#!/bin/sh

# This script intend for automatic builds for IFME
# Enjoy!

rm -rf x265
hg clone https://bitbucket.org/multicoreware/x265 x265

cd x265/build
mkdir icl64

# MSYS
cd msys
wget https://github.com/Anime4000/IFME/raw/master/sources/compile-x265-win64/x265-msys.sh
sh x265-msys.sh

cd ..

# VS12
cd vc12-x86_64
wget https://github.com/Anime4000/IFME/raw/master/sources/compile-x265-win64/x265-vs12.cmd
cmd /c x265-vs12.cmd

cd ..

# ICL64
cd icl64
wget https://github.com/Anime4000/IFME/raw/master/sources/compile-x265-win64/x265-icl64.cmd
cmd /c x265-icl64.cmd

cd ..

# Capture
mkdir ifme
mkdir ifme/x265gcc
mkdir ifme/x265msvc
mkdir ifme/x265icc

cp msys/x265-08.exe ifme/x265gcc/
cp msys/x265-10.exe ifme/x265gcc/
cp msys/x265-12.exe ifme/x265gcc/

cp vc12-x86_64/x265-08.exe ifme/x265msvc/
cp vc12-x86_64/x265-10.exe ifme/x265msvc/
cp vc12-x86_64/x265-12.exe ifme/x265msvc/

cp icl64/x265-08.exe ifme/x265icc/
cp icl64/x265-10.exe ifme/x265icc/
cp icl64/x265-12.exe ifme/x265icc/