#!/bin/sh
ORIDIR="$(dirname $(readlink -f $0))"
CompileMode="Debug"
BUILDDIR="build"

cd "$ORIDIR"

echo "Currently you need already compiled version,"
echo "compiling on Linux for now not possible, so,"
echo "this script will make it standalone program"
echo "without Mono required to install"
echo " "
echo "Before you start:"
echo "      1. Mono 4.0+ <http://www.mono-project.com/download/#download-lin>"
echo "      2. sudo apt-get install p7zip-full mediainfo"
echo " "
echo "make sure you run \"deploy.sh\" at \"prerequisite\" folder"
echo " "

read -rp "Press any ENTER to continue..." key

echo "Download .NET references"
if [ -f "references/MediaInfoDotNet.dll" ]
then
	echo "File found, no need to download"
else
	wget --no-check-certificate https://github.com/x265/MediaInfoDotNet/releases/download/v0.7.8/MediaInfoDotNet.dll -O "references/MediaInfoDotNet.dll"
fi

if [ -f "references/INIFileParser.dll" ]
then
	echo "File found, no need to download"
else
	wget --no-check-certificate https://github.com/Anime4000/IFME/releases/download/v5.0-beta.8/INIFileParser.dll -O "references/INIFileParser.dll"
fi

echo "Remove windows builds"
rm -rf "$BUILDDIR"

echo "Make new folder"
mkdir "$BUILDDIR"

echo "Copying stuff"
mkdir "$BUILDDIR/benchmark"
mkdir "$BUILDDIR/extension"
cp -r "ifme/bin/$CompileMode/lang" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/profile" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/sounds" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/addons_linux32.repo" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/addons_linux64.repo" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/addons_windows32.repo" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/addons_windows32.repo" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/avisynthsource.code" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/format.ini" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/iso.code" "$BUILDDIR/"
cp -r "changelog.txt" "$BUILDDIR/"
cp -r "license.txt" "$BUILDDIR/"
cp -r "patents.txt" "$BUILDDIR/"
cp "/usr/lib/p7zip/7za" "$BUILDDIR/"
cp "/usr/lib/x86_64-linux-gnu/libmediainfo.so.0.0.0" "$BUILDDIR/"
cp -a "/usr/lib/x86_64-linux-gnu/libmediainfo.so.0" "$BUILDDIR/"

echo "Copying compiled"
cp "ifme/bin/$CompileMode/ifme.exe" "$BUILDDIR/"
cp "ifme/bin/$CompileMode/INIFileParser.dll" "$BUILDDIR/"
cp "ifme/bin/$CompileMode/MediaInfoDotNet.dll" "$BUILDDIR/"
cp "MediaInfoDotNet.dll.config" "$BUILDDIR/"

echo "Copying plugins"
cp -r "prerequisite/linux/64bit/plugins" "$BUILDDIR/"

echo "Building..."
xbuild /nologo /verbosity:normal ifme.sln /target:Build /property:Configuration=Debug

cd $BUILDDIR
mkbundle --deps --static -o ifme ifme.exe INIFileParser.dll MediaInfoDotNet.dll

gcc "../ifme-gnome.c" -o "ifme-gnome"
gcc "../ifme-xterm.c" -o "ifme-xterm"

echo "Remove bytecode"
rm -f "ifme.exe"
rm -f "INIFileParser.dll"
rm -f "MediaInfoDotNet.dll"
rm -f "MediaInfoDotNet.dll.config"

cd "$ORIDIR"

echo "Fix directory permission"
find "$ORIDIR/$BUILDDIR" -type d -exec chmod 775 {} +

echo "Packaging..."
mv $BUILDDIR ifme5
tar -cvJf ifme5-x64_linux.tar.xz ifme5
mv ifme5 $BUILDDIR

echo "Done!"
sleep 3