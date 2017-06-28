# IFME make file
LBITS := $(shell getconf LONG_BIT)

CC=gcc
MONOCC=msbuild
MONOMK=mkbundle

MODE=Release

DIR=build
IFME=ifme7

all: clean compile copy fixmod
	mv "$(DIR)" "release-ifme7"
	zip -r "$(IFME).zip" "release-ifme7"
	mv "release-ifme7" "$(DIR)"

clean:
	rm -rf "$(DIR)"
	rm -f "$(IFME).zip"

compile:
	$(MONOCC) /nologo /verbosity:normal ifme.sln /target:Build /property:Configuration=$(MODE)

copy:
	mkdir "$(DIR)"
	tar -xvJf plugin.tar.xz -C "$(DIR)"
	cp "license.txt" "$(DIR)"
	cp "patents.txt" "$(DIR)"
	cp "doc/readme.txt" "$(DIR)"
	cp "sources/ifme.sh" "$(DIR)"
	cp "sources/ifme.desktop" "$(DIR)"
	cp "ifme/bin/Release/ifme.exe" "$(DIR)"
	cp "ifme/bin/Release/FFmpegDotNet.dll" "$(DIR)"
	cp "ifme/bin/Release/Newtonsoft.Json.dll" "$(DIR)"
	cp "ifme/bin/Release/avisynth.json" "$(DIR)"
	cp "ifme/bin/Release/format.json" "$(DIR)"
	cp "ifme/bin/Release/language.json" "$(DIR)"
	cp "ifme/bin/Release/mime.json" "$(DIR)"
	cp -r "ifme/bin/Release/branding" "$(DIR)"
	cp -r "ifme/bin/Release/lang" "$(DIR)"
	cp -r "ifme/bin/Release/preset" "$(DIR)"
	
	ifeq ($(LBITS),32)
		cp "ifme/bin/Release/ffmpeg64_32layer.sh" "$(DIR)"
		cp "ifme/bin/Release/ffmpeg64_32layer.cmd" "$(DIR)"
	endif

fixmod:
	find "./$(DIR)" -type d -exec chmod 755 {} \;
	find "./$(DIR)" -type f -exec chmod 644 {} \;
	find "./$(DIR)" -type f -exec /bin/sh -c "file {} | grep -q executable && chmod +x {}" \;
	find "./$(DIR)" -name "*.sh" -exec chmod +x {} \;
	find "./$(DIR)" -name "*.exe" -exec chmod -x {} \;
	find "./$(DIR)" -name "*.dll" -exec chmod -x {} \;