#!/bin/sh
cd "$(dirname $(readlink -f $0))"
echo "Creating folder"
mkdir "linux"
mkdir "linux/64bit"
mkdir "linux/64bit/plugins"
mkdir "allos/"
mkdir "allos/extension"
echo "Downloading plugins"
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/faac.ifx -O linux/64bit/plugins/faac.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/ffmpeg.ifx -O linux/64bit/plugins/ffmpeg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/ffmpeg-ogg.ifx -O linux/64bit/plugins/fmpeg-ogg.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/ffmsindex.ifx -O linux/64bit/plugins/ffmsindex.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/mkvtool.ifx -O linux/64bit/plugins/mkvtool.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/mp4box.ifx -O linux/64bit/plugins/mp4box.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/mp4fpsmod.ifx -O linux/64bit/plugins/mp4fpsmod.ifx
wget http://master.dl.sourceforge.net/project/ifme/plugins/linux/64bit/x265gcc.ifx -O linux/64bit/plugins/x265gcc.ifx
echo "Unpacking"
7za x "linux/64bit/plugins/faac.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/ffmpeg.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/fmpeg-ogg.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/ffmsindex.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/mkvtool.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/mp4box.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/mp4fpsmod.ifx" -y -o"linux/64bit/plugins/"
7za x "linux/64bit/plugins/x265gcc.ifx" -y -o"linux/64bit/plugins/"
echo "Downloading extension"
wget --no-check-certificate https://github.com/x265/HFRGen/releases/download/v0.2/hfrgen.dll -O "allos/extension/hfrgen.dll"
wget --no-check-certificate https://github.com/x265/HoloBenchmark/releases/download/v0.0.2/holobenchmark.dll -O "allos/extension/holobenchmark.dll"
wget --no-check-certificate https://github.com/x265/Nemupad/releases/download/0.0.3.1/nemupad.dll -O "allos/extension/holobenchmark.dll"
echo "Delete cache"
find . -name "*.ifx" -type f -delete
echo "Done!"
sleep 3