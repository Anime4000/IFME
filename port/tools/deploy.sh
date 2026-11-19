#!/usr/bin/env bash
# deploy.sh - make a portable copy of the Qt build (MSYS2 UCRT64), runnable
# outside the MSYS2 shell, for a combined release beside the .NET IFME.
#
#   deploy.sh <ifme9.exe> <ifmecli.exe> <dest dir> <IFME folder with the .wad files> <Qt bin dir>
#
# The Qt bin dir (e.g. C:/msys64/ucrt64/bin) also holds every MinGW DLL.
#
# Run through:  cmake --build build --target deploy
set -euo pipefail

gui=$1 cli=$2 dest=$3 from=$4 bin=$5

mkdir -p "$dest"
cp -f "$gui" "$cli" "$dest/"

# Qt DLLs and plugins (platform, style, image formats).
"$bin/windeployqt6.exe" --release --no-translations --no-system-d3d-compiler \
    --no-opengl-sw --no-network --dir "$(cygpath -w "$dest")" "$(cygpath -w "$dest/$(basename "$gui")")" >/dev/null

# Every other DLL the programs and plugins import from that folder (the
# compiler runtime, ICU, zlib, freetype, ...), until nothing new turns up.
# DLLs not found there belong to Windows.
while :; do
    added=0
    while IFS= read -r name; do
        if [[ ! -e "$dest/$name" && -f "$bin/$name" ]]; then
            cp -f "$bin/$name" "$dest/"
            added=1
        fi
    done < <(find "$dest" \( -iname '*.exe' -o -iname '*.dll' \) -print0 | xargs -0 "$bin/objdump.exe" -p 2>/dev/null |
             awk '/DLL Name:/ { print $3 }' | sort -u)
    [[ $added == 0 ]] && break
done

# Data the .NET build ships as .wad files (Language.json lives in jason.wad).
for wad in jason.wad resources.wad; do
    if [[ -f "$from/$wad" ]]; then
        cp -f "$from/$wad" "$dest/"
    else
        echo "deploy: $from/$wad not found, skipped" >&2
    fi
done

echo "deploy: $(find "$dest" -type f | wc -l) files, $(du -sh "$dest" | cut -f1) in $dest"
