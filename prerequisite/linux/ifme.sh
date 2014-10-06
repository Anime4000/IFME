#!/bin/bash

# Basic
appname="Internet Friendly Media Encoder"
appfile="ifme.exe"

# Required shared file
rsf="mono-complete mediainfo libavformat-dev libswscale-dev libboost-all-dev"

# Command
clear
echo -e "\033[37m"
echo -e "Make sure you have \033[35m$rsf \033[37minstalled."
echo -e "For debian based (ubuntu, linux mint), simply type:"
echo -e "\033[33msudo apt-get install \033[35m$rsf\033[37m"
echo -e "\033[37m"
echo -e "\033[37m"
sleep 1
echo -e "If you facing some permission error, I suggest to \033[31mre-install\033[37m"
echo -e "because it may deleted IFME files and folders :)"
echo -e "\033[37m"
sleep 2
echo -e "[info] Setup temporary library path at \033[32m$(pwd)\033[37m"
unset LD_LIBRARY_PATH & export LD_LIBRARY_PATH=$(pwd):$LD_LIBRARY_PATH
echo -e "[info] Launching \033[31m$appname\033[37m"
mono --nollvm --gc=boehm $appfile
echo -e "[info] Reset temporary library path"
unset LD_LIBRARY_PATH