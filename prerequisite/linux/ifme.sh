#!/bin/bash
appname="Internet Friendly Media Encoder"
appfile="ifme.exe"
clear
echo " "
echo "Make sure you have \"mono-complete\" and \"libboost-all-dev\" installed"
echo "for debian based (ubuntu, linux mint), simply run:"
echo "                    sudo apt-get install mono-complete libboost-all-dev"
echo " "
sleep 1
echo "If you facing some permission error, I suggest to re-install"
echo "because it may deleted IFME files and folders :)"
echo " "
sleep 2
echo "[info] Setup temporary library path at $(pwd)"
unset LD_LIBRARY_PATH & export LD_LIBRARY_PATH=$(pwd):$LD_LIBRARY_PATH
echo "[info] Launching $appname"
mono --nollvm --gc=boehm $appfile
echo "[info] Reset temporary library path"
unset LD_LIBRARY_PATH
