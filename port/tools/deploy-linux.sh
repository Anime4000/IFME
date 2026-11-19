#!/usr/bin/env bash
# deploy-linux.sh - make a portable copy of the Qt build on Linux, runnable on
# other distributions of the same age or newer (glibc not bundled).
#
#   deploy-linux.sh <ifme9> <ifmecli> <dest dir> <IFME folder with the .wad files> [<a Qt plugin file>]
#
# Run through:  cmake --build build --target deploy
#
# Layout:
#   ifme9, ifmecli          the programs (RPATH $ORIGIN/lib)
#   lib/                    Qt and the libraries it needs that a desktop may lack
#   lib/qt6/plugins/        Qt plugins (xcb, wayland, image formats, input methods)
#   qt.conf                 points Qt at those plugins
#   jason.wad, resources.wad
set -euo pipefail

gui=$1 cli=$2 dest=$3 from=$4 plugin_hint=${5:-}

# ---- where Qt keeps its plugins ----
plugins=
if [[ -n $plugin_hint && -f $plugin_hint ]]; then
    plugins=$(dirname "$(dirname "$plugin_hint")")
fi
for q in qtpaths6 qtpaths qmake6 qmake; do
    [[ -n $plugins && -d $plugins ]] && break
    if command -v "$q" >/dev/null 2>&1; then
        if [[ $q == qtpaths* ]]; then
            plugins=$("$q" --query QT_INSTALL_PLUGINS 2>/dev/null || true)
        else
            plugins=$("$q" -query QT_INSTALL_PLUGINS 2>/dev/null || true)
        fi
    fi
done
if [[ -z $plugins || ! -d $plugins/platforms ]]; then
    echo "deploy: cannot find the Qt plugins folder (tried qtpaths6/qmake6)" >&2
    exit 1
fi

rm -rf "$dest/lib"
mkdir -p "$dest/lib/qt6/plugins"
cp -f "$gui" "$cli" "$dest/"

# ---- Qt plugins: the platform ones and what a desktop session uses ----
copy_plugin() {   # <group> [file pattern]
    local group=$1 pattern=${2:-*.so}
    [[ -d $plugins/$group ]] || return 0
    mkdir -p "$dest/lib/qt6/plugins/$group"
    find "$plugins/$group" -maxdepth 1 -name "$pattern" -exec cp -fL {} "$dest/lib/qt6/plugins/$group/" \;
}
copy_plugin platforms 'libqxcb.so'
copy_plugin platforms 'libqwayland*.so'
copy_plugin platforms 'libqoffscreen.so'
copy_plugin xcbglintegrations
copy_plugin platforminputcontexts
copy_plugin imageformats 'libqico.so'
copy_plugin imageformats 'libqjpeg.so'
copy_plugin imageformats 'libqgif.so'
copy_plugin platformthemes 'libqxdgdesktopportal.so'
for g in wayland-shell-integration wayland-decoration-client wayland-graphics-integration-client; do
    copy_plugin "$g"
done
find "$dest/lib/qt6/plugins" -type d -empty -delete

# ---- libraries: everything they load, except what every desktop has ----
# (glibc, the compiler runtime, OpenGL/DRM, X11/xcb core, fontconfig and the
# libraries those share with the system; bundling them breaks more than it fixes.)
excluded() {
    case $1 in
        ld-linux*|libc.so*|libm.so*|libdl.so*|libpthread.so*|librt.so*|libresolv.so*|libutil.so*|\
        libanl.so*|libnsl.so*|libmvec.so*|libBrokenLocale.so*|libgcc_s.so*|libstdc++.so*|\
        libGL.so*|libGLX*.so*|libGLdispatch.so*|libOpenGL.so*|libEGL*.so*|libGLES*.so*|libdrm*.so*|libgbm.so*|\
        libX11.so*|libX11-xcb.so*|libxcb.so*|libxcb-dri*.so*|libxcb-glx.so*|libxcb-present.so*|libXau.so*|libXdmcp.so*|\
        libxshmfence.so*|libfontconfig.so*|libfreetype.so*|libharfbuzz.so*|libexpat.so*|libz.so*|\
        libdbus-1.so*|libsystemd.so*|libudev.so*|libglib-2.0.so*|libgobject-2.0.so*|libgio-2.0.so*|\
        libgmodule-2.0.so*|libgthread-2.0.so*|libffi.so*|libpcre2-8.so*|libselinux.so*|libmount.so*|libblkid.so*|\
        libuuid.so*|libcap.so*|libgcrypt.so*|libgpg-error.so*|liblzma.so*|libzstd.so*|liblz4.so*|libbsd.so*|libmd.so*|\
        libpng16.so*|libbz2.so*|libbrotli*.so*|libgraphite2.so*|libwayland-*.so*|libvulkan.so*)
            return 0 ;;
    esac
    return 1
}

while :; do
    added=0
    while IFS= read -r path; do
        name=$(basename "$path")
        excluded "$name" && continue
        if [[ ! -e $dest/lib/$name ]]; then
            cp -fL "$path" "$dest/lib/$name"
            added=1
        fi
    done < <(find "$dest" -maxdepth 1 -type f \( -name ifme9 -o -name ifmecli \) -print0 |
             cat - <(find "$dest/lib" -type f -name '*.so*' -print0) |
             xargs -0 -n1 ldd 2>/dev/null | awk '$2 == "=>" && $3 ~ /^\// { print $3 }' | sort -u)
    [[ $added == 0 ]] && break
done

# ---- make the programs find lib/ ----
# The build links them with DT_RPATH $ORIGIN/lib (inherited by Qt and its
# plugins); patchelf, when installed, makes sure of it.
for exe in "$dest/ifme9" "$dest/ifmecli"; do
    if command -v patchelf >/dev/null 2>&1; then
        patchelf --force-rpath --set-rpath '$ORIGIN/lib' "$exe"
    fi
    if ! readelf -d "$exe" | grep -q 'RPATH.*\$ORIGIN/lib'; then
        echo "deploy: $(basename "$exe") has no RPATH \$ORIGIN/lib; install patchelf or rebuild" >&2
    fi
done

cat > "$dest/qt.conf" <<'EOF'
[Paths]
Prefix = .
Plugins = lib/qt6/plugins
EOF

# ---- data the .NET build ships as .wad files (Language.json lives in jason.wad) ----
for wad in jason.wad resources.wad; do
    if [[ -f $from/$wad ]]; then
        cp -f "$from/$wad" "$dest/"
    else
        echo "deploy: $from/$wad not found, skipped" >&2
    fi
done

echo "deploy: $(find "$dest/lib" -maxdepth 1 -name '*.so*' | wc -l) libraries, $(find "$dest/lib/qt6/plugins" -name '*.so' | wc -l) plugins, $(du -sh "$dest" | cut -f1) in $dest"
