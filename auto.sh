#!/bin/bash

echo "IFME automated linux builds"
echo "Please run ./configure first before execute this script, CTRL+C to exit"
echo "Use screen to run in the background: screen -mdSU auto bash auto.sh"
sleep 5

while true
do
    git fetch origin > auto.log 2>&1

    if [[ -s auto.log ]] ; then
		git pull
	
        echo "[$(date +"%Y:%m:%d %H:%M:%S")] New changes detected, building..."

        make
        
        rm -rf "../builds/"
        mkdir "../builds/"

        mv -f ifme5-x64_linux.tar.xz ../builds/ifme5-amd64_linux-build-$(date +"%Y%m%d_%H%M%S").tar.xz
    else
        echo "[$(date +"%Y:%m:%d %H:%M:%S")] No changes detected"
        sleep 60
    fi
done
