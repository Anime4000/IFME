#!/bin/bash
clear
# variable
cm="Release"
bd="_build"
msbulid="/usr/bin/xbuild"
echo " "
echo "This script allowing publish IFME after compile. Using $cm build."
echo "                  1. ifme.exe"
echo "                  2. ifme.framework.dll"
echo " "
echo "Please download these file and put on \"prerequisite\" folder!"
echo "                  1. addons/* (all addons)"
echo "                  2. libmediainfo.so.0 (64bit)"
echo "                  3. libzen.so.0 (64bit)"
echo "                  4. libgpac.so.3 (64bit)"
echo "                  5. libmozjs185.so.1.0 (any?)"
echo "                  6. 7za (64bit)"
echo " "
echo " "
echo "Visit IFME GitHub project for more info and links."
echo "Make sure you have latest \"xbuild\", if not press CTRL+C"
echo " "
echo "Starting in 10 seconds..."
sleep 10
echo " "
echo " "
echo " "

echo "Cleaning old build folder"
rm -r -f ./$bd
mkdir ./$bd
mkdir ./$bd/addons
mkdir ./$bd/lang
mkdir ./$bd/preset

echo "Remove previous compiled"
rm -r -f ./ifme/bin/x64/$cm
mkdir ./ifme/bin/x64/$cm

echo "Compiling..."
$msbulid /nologo /verbosity:normal ifme.sln /target:Build /property:Configuration=$cm

echo "Copy main files"
cp ./ifme/bin/x64/$cm/iso.gg ./$bd/iso.gg
cp ./ifme/bin/x64/$cm/ifme.exe ./$bd/ifme.exe
cp ./ifme/bin/x64/$cm/ifme.exe.config ./$bd/ifme.exe.config
cp ./ifme/bin/x64/$cm/ifme.framework.dll ./$bd/ifme.framework.dll
echo "<?xml version=\"1.0\" encoding=\"utf-8\"?>" > ./$bd/ifme.framework.dll.config
echo "<configuration>" >> ./$bd/ifme.framework.dll.config
echo "     <dllmap dll=\"MediaInfo.dll\" target=\"libmediainfo.so.0\"/>" >> ./$bd/ifme.framework.dll.config
echo "</configuration>" >> ./$bd/ifme.framework.dll.config
cp -avr ./ifme/bin/x64/$cm/lang/ ./$bd/
cp -avr ./ifme/bin/x64/$cm/preset/ ./$bd/
cp ./installer/text_addon_license.txt ./$bd/LICENSE_ADDONS
cp ./installer/text_gpl2.txt ./$bd/LICENSE

echo "Copy prerequisite"
cp ./prerequisite/linux/7za ./$bd/unpack
cp ./prerequisite/linux/ifme.sh ./$bd/ifme.sh
cp -avr ./prerequisite/linux/addons/ ./$bd/
cp -avr ./prerequisite/linux/lib* ./$bd/

echo "Adjust permission"
chmod +x ./$bd/ifme.sh
chmod -x ./$bd/ifme.exe
chmod -x ./$bd/ifme.framework.dll
chmod +x ./$bd/unpack

echo " "
echo " "
echo "Make sure binary file inside addons folder has \"execute\" permission."
echo "Build completed"
