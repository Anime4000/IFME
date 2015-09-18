#!/bin/sh
ORIDIR="`pwd`"
CompileMode="Debug"
BUILDDIR="build"
echo "Currently you need already compiled version,"
echo "compiling on Linux for now not possible, so,"
echo "this script will make it standalone program"
echo "without Mono required to install"
echo " "
echo "Before proceed, you need:"
echo "sudo apt-get install mono-complete p7zip-full mediainfo"

read -rp "Press any ENTER to continue..." key

echo "Remove windows builds"
rm -rf "$BUILDDIR"

echo "Make new folder"
mkdir "$BUILDDIR"

echo "Copying stuff"
cp -r "ifme/bin/$CompileMode/extension" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/lang" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/profile" "$BUILDDIR/"
cp -r "ifme/bin/$CompileMode/sounds" "$BUILDDIR/"
mkdir "$BUILDDIR/plugins"
mkdir "$BUILDDIR/benchmark"
cp "/usr/lib/p7zip/7za" "$BUILDDIR/"
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
cp "/usr/lib/x86_64-linux-gnu/libmediainfo.so.0.0.0" "$BUILDDIR/"
cp -a "/usr/lib/x86_64-linux-gnu/libmediainfo.so.0" "$BUILDDIR/"

echo "Remove stuff"
rm -rf "$BUILDDIR/extension/AvsPmod"
rm -rf "$BUILDDIR/extension/AvsPmodBridge.dll"

echo "Copying compiled"
cp "ifme/bin/$CompileMode/ifme.exe" "$BUILDDIR/"
cp "ifme/bin/$CompileMode/INIFileParser.dll" "$BUILDDIR/"
cp "ifme/bin/$CompileMode/MediaInfoDotNet.dll" "$BUILDDIR/"
cp "MediaInfoDotNet.dll.config" "$BUILDDIR/"

echo "Building..."
cd $BUILDDIR
mkbundle --deps --static -o ifme ifme.exe INIFileParser.dll MediaInfoDotNet.dll
gcc "../ifme-gnome.c" -o "ifme-gnome"
gcc "../ifme-xterm.c" -o "ifme-xterm"

echo "Remove bytecode"
rm -f "ifme.exe"
rm -f "INIFileParser.dll"
rm -f "MediaInfoDotNet.dll"
rm -f "MediaInfoDotNet.dll.config"

cd $ORIDIR

echo "Next... Making packaging (.tar.xz). Get ready first"
read -rp "Press any ENTER to continue..." key

echo "Fix directory permission"
find "$ORIDIR/$BUILDDIR" -type d -exec chmod 775 {} +

echo "Packaging..."
mv $BUILDDIR ifme5
tar -cvJf ifme5.tar.xz ifme5
mv ifme5 $BUILDDIR

echo "Done!"
sleep 3