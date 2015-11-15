# IFME make file
CC=gcc
MONOCC=xbuild
MONOMK=mkbundle
MODE=Release
DIR=build

all: only
	mv $(DIR) ifme5
	tar -cvJf ifme5-amd64_linux.tar.xz ifme5
	mv ifme5 $(DIR)

only: clean compile copy copylib copymono
	$(CC) "sources/ifme-gnome.c" -o "$(DIR)/ifme-gnome"
	$(CC) "sources/ifme-xterm.c" -o "$(DIR)/ifme-xterm"
	cp "sources/ifme.sh" "$(DIR)/ifme"
	$(MONOMK) -z --deps --static -o "$(DIR)/ifme-bin" "$(DIR)/ifme.exe" "$(DIR)/INIFileParser.dll" "$(DIR)/MediaInfoDotNet.dll"
	rm -f "$(DIR)/ifme.exe"
	rm -f "$(DIR)/INIFileParser.dll"
	rm -f "$(DIR)/MediaInfoDotNet.dll"
	rm -f "$(DIR)/MediaInfoDotNet.dll.config"
	find "./$(DIR)" -type d -exec chmod 775 {} \;
	find "./$(DIR)" -type f -exec chmod 664 {} \;
	find "./$(DIR)" -name "*.sh" -exec chmod +x {} \;
	find "./$(DIR)" -type f -exec /bin/sh -c "file {} | grep -q executable && chmod +x {}" \;

compile:
	$(MONOCC) /nologo /verbosity:normal ifme.sln /target:Build /property:Configuration=$(MODE)
	
copy:
	mkdir "$(DIR)/benchmark"
	mkdir "$(DIR)/extension"
	cp -r "ifme/lang" "$(DIR)/"
	cp -r "ifme/profile" "$(DIR)/"
	cp -r "ifme/sounds" "$(DIR)/"
	cp "ifme/addons_linux32.repo" "$(DIR)/"
	cp "ifme/addons_linux64.repo" "$(DIR)/"
	cp "ifme/addons_windows32.repo" "$(DIR)/"
	cp "ifme/addons_windows32.repo" "$(DIR)/"
	cp "ifme/avisynthsource.code" "$(DIR)/"
	cp "ifme/format.ini" "$(DIR)/"
	cp "ifme/iso.code" "$(DIR)/"
	cp "sources/metauser.if" "$(DIR)/"
	cp "changelog.txt" "$(DIR)/"
	cp "license.txt" "$(DIR)/"
	cp "patents.txt" "$(DIR)/"
	cp -r "prerequisite/linux/64bit/plugins" "$(DIR)/"
	cp -r "prerequisite/allos/extension" "$(DIR)/"
	
copylib:
	cp "/usr/lib/p7zip/7za" "$(DIR)/"
	cp "prerequisite/linux/64bit/libmediainfo.so.0" "$(DIR)/"
	
copymono:
	cp "ifme/bin/$(MODE)/ifme.exe" "$(DIR)/"
	cp "ifme/bin/$(MODE)/INIFileParser.dll" "$(DIR)/"
	cp "ifme/bin/$(MODE)/MediaInfoDotNet.dll" "$(DIR)/"
	cp "sources/MediaInfoDotNet.dll.config" "$(DIR)/"

clean:
	rm -f ifme5-x64_linux.tar.xz
	rm -rf "$(DIR)"
	mkdir "$(DIR)"