# IFME make file
LBITS := $(shell getconf LONG_BIT)

CC=gcc
MONOCC=msbuild
MONOMK=mkbundle

MODE=Release

DIR=build
DIR32=build32
DIR64=build64

REL32=release-ifme7_i686
REL64=release-ifme7_amd64

ARC32=ifme7_i686
ARC64=ifme7_amd64

all: clean compile copy fixmod
	cp -r "$(DIR)" "$(REL32)"
	mv "$(DIR)" "$(REL64)"
	tar -xvJf ./prerequisite/plugin32.tar.xz -C "$(REL32)"
	tar -xvJf ./prerequisite/plugin64.tar.xz -C "$(REL64)"
	7za a -tzip -mm=Deflate -mfb=258 -mpass=15 "$(ARC32).zip" "$(REL32)"
	7za a -tzip -mm=Deflate -mfb=258 -mpass=15 "$(ARC64).zip" "$(REL64)"
	mv "$(REL32)" "$(DIR32)"
	mv "$(REL64)" "$(DIR64)"

clean:
	rm -rf "$(DIR)"
	rm -rf "$(DIR32)"
	rm -rf "$(DIR64)"
	rm -f "$(ARC32).7z"
	rm -f "$(ARC64).7z"

compile:
	$(MONOCC) /nologo /verbosity:normal ifme.sln /target:Build /property:Configuration=$(MODE)

copy:
	mkdir "$(DIR)"
	cp "license.txt" "$(DIR)"
	cp "patents.txt" "$(DIR)"
	cp "changelog.txt" "$(DIR)"
	cp "doc/readme.txt" "$(DIR)"
	cp "prerequisite/FontReg32.exe" "$(DIR)"
	cp "prerequisite/FontReg64.exe" "$(DIR)"
	cp "sources/ifme.sh" "$(DIR)"
	cp "sources/ifme.desktop" "$(DIR)"
	cp "sources/ffmpeg64_32layer.cmd" "$(DIR)"
	cp "sources/ffmpeg64_32layer.sh" "$(DIR)"
	cp "ifme/bin/Release/ifme.exe" "$(DIR)"
	cp "ifme/bin/Release/FFmpegDotNet.dll" "$(DIR)"
	cp "ifme/bin/Release/Newtonsoft.Json.dll" "$(DIR)"
	cp "ifme/bin/Release/avisynth.json" "$(DIR)"
	cp "ifme/bin/Release/format.json" "$(DIR)"
	cp "ifme/bin/Release/language.json" "$(DIR)"
	cp "ifme/bin/Release/targetfmt.json" "$(DIR)"
	cp "ifme/bin/Release/mime.json" "$(DIR)"
	cp -r "ifme/bin/Release/branding" "$(DIR)"
	cp -r "ifme/bin/Release/lang" "$(DIR)"
	cp -r "ifme/bin/Release/preset" "$(DIR)"

fixmod:
	find "./$(DIR)" -type d -exec chmod 755 {} \;
	find "./$(DIR)" -type f -exec chmod 644 {} \;
	find "./$(DIR)" -type f -exec /bin/sh -c "file {} | grep -q executable && chmod +x {}" \;
	find "./$(DIR)" -name "*.sh" -exec chmod +x {} \;
	find "./$(DIR)" -name "*.desktop" -exec chmod +x {} \;
	find "./$(DIR)" -name "*.exe" -exec chmod -x {} \;
	find "./$(DIR)" -name "*.dll" -exec chmod -x {} \;
	