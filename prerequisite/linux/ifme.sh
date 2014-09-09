#!/bin/bash
clear
echo " "
echo "Make sure you have \"mono-runtime\" and \"libboost-all-dev\" installed"
echo "for debian based (ubuntu, linux mint), simply run:"
echo "            sudo apt-get install mono-runtime libboost-all-dev"
echo " "
sleep 1
echo "If you facing some permission error, I suggest to re-install"
echo "because it may deleted IFME files and folders :)"
echo " "
sleep 1
echo "Setup temporary library path at $(pwd)"
unset LD_LIBRARY_PATH
export LD_LIBRARY_PATH=$(pwd):$LD_LIBRARY_PATH
mono --debug ifme.exe && unset LD_LIBRARY_PATH
