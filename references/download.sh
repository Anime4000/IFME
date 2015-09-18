#!/bin/sh
cd "$(dirname $(readlink -f $0))"
wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/INIFileParser.dll -O "INIFileParser.dll"
wget --no-check-certificate https://github.com/x265/MediaInfoDotNet/releases/download/v0.7.8/MediaInfoDotNet.dll -O "MediaInfoDotNet.dll"
echo "Done!!!"
sleep 3