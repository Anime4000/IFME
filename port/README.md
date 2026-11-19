# IFME native port

Work-in-progress rewrite of IFME without Mono/.NET:

| Part | Language | Folder |
|---|---|---|
| `libifmecore`: plugins, probing, encoding pipeline, process control | C11 | `core/` |
| `ifmecli`: command-line front-end | C11 | `cli/` |
| `ifme`: desktop GUI | C++17 + Qt 6 Widgets | `gui/` |

Both front-ends use the same C interface, [`core/include/ifme.h`](core/include/ifme.h).
Encoders stay external programs described by the existing `Plugins/*.json`
manifests; they are spawned, never linked.

## Building

The commands are the same on every platform; only the packages differ.

```bash
cmake -B build -G Ninja
cmake --build build
```

Add `-DIFME_BUILD_GUI=OFF` to build only the core and `ifmecli` (no Qt needed).

### Windows (MSYS2, UCRT64 shell)

```bash
pacman -S --needed mingw-w64-ucrt-x86_64-toolchain mingw-w64-ucrt-x86_64-cmake mingw-w64-ucrt-x86_64-ninja mingw-w64-ucrt-x86_64-pkgconf mingw-w64-ucrt-x86_64-qt6-base mingw-w64-ucrt-x86_64-qt6-tools
```

Run the programs from the UCRT64 shell (it puts the Qt DLLs on `PATH`), or
bundle them for distribution with `windeployqt build/ifme.exe`.

Every executable embeds [`res/ifme.manifest`](res/ifme.manifest), which makes
the process code page UTF-8 (Windows 10 1903+). Plain `char *` APIs (`fopen`,
`CreateProcessA`, `argv`) therefore accept file names in any language.

### Ubuntu / Debian

```bash
sudo apt install build-essential cmake ninja-build qt6-base-dev
```

### macOS

```bash
xcode-select --install
brew install cmake ninja qt
```

## Try it

```bash
./build/ifmecli --info
```

```
IFME core   0.1.0
App folder  D:\...\port\build
Config      C:\Users\<you>\AppData\Roaming\IFME
CPU         AMD Ryzen 9 3950X 16-Core Processor (x86_64)
SIMD        SSE4.2:yes AVX:yes AVX2:yes FMA3:yes AVX-512F:no
```

## Layout

```
core/include/ifme.h          public C API (extern "C", UTF-8, opaque handles)
core/src/core.c              context lifetime, helpers
core/src/platform/paths.c    executable folder, per-user config folder
core/src/platform/cpu_*.c    CPU model and SIMD flags (x86 / everything else)
cli/ifmecli.c                command-line front-end
gui/                         Qt front-end
res/                         Windows manifest and resource script
```

## Roadmap

- [x] 0. Skeleton: core library, CLI and GUI build on Windows; paths and CPU detection
- [ ] 1. `--list-plugins` / `--self-test`: read the existing plugin manifests (cJSON)
- [ ] 2. `--probe FILE`: run ffprobe, parse its JSON
- [ ] 3. Process layer: spawn without a shell, pipes (`ffmpeg | encoder`), pause/resume/stop
      (Job Objects on Windows, process groups on Linux/macOS)
- [ ] 4. Video encode (single and multi-pass) with progress and ETA
- [ ] 5. Audio encode and muxing
- [ ] 6. Subtitles, fonts, attachments, metadata
- [ ] 7. GUI on top of the finished core
