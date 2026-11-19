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

### Plugins

The new core reads the same `Plugins/**/_plugin.a*.json` and `_plugin.v*.json`
manifests as IFME today. Point it at an existing install with `--root`:

```bash
./build/ifmecli --root "/d/Program Files/IFME" --list-plugins
./build/ifmecli --root "/d/Program Files/IFME" --self-test          # plugins with "TestRequired"
./build/ifmecli --root "/d/Program Files/IFME" --self-test --all -v # every usable plugin, full detail
./build/ifmecli --root "/d/Program Files/IFME" --list-plugins --json
./build/ifme --root "/d/Program Files/IFME"                          # GUI
```

Behaviour compared with `PluginsLoad.cs`:

- Manifests may contain comments and trailing commas (Newtonsoft accepted them).
- Encoders run **without a shell**, as argument lists, so folders with spaces,
  `$`, backticks or non-Latin characters work on every platform.
- A missing encoder program is reported at load time (`missing`).
- A self-test passes only if the encoder exits with 0 **and** writes a non-empty
  file. The audio test command follows a real encode (`MediaEncoding.cs`); the
  old test dropped exhale's preset and sent stereo to mono-only AMR-NB, and its
  exit-code-only check hid both.
- Optional `"Arch": ["x86_64", "arm64"]` in a manifest overrides `"X64"`, for
  Apple Silicon and ARM Linux builds.
- Not yet: the user's disabled-plugin list and remembering test failures
  (needs the settings file, a later milestone).

### Media information

```bash
./build/ifmecli --root "/d/Program Files/IFME" movie.mkv other.mp4     # same layout as IFME's MediaInfo
./build/ifmecli --root "/d/Program Files/IFME" --json movie.mkv        # add -v for the raw ffprobe output
./build/ifmecli -p --framerate 24 "frames/img%04d.png"                 # image sequence
```

In the GUI, drop files onto the window (or pass them on the command line).

ffprobe comes from `PATH`, else `<root>/Plugins/ffmpeg64`. Compared with
`MediaInfo.cs` the fields and defaults are the same, except:

- **Audio bit depth** is real: C# assigned `sample_fmt` (e.g. `"fltp"`) to an
  int, which always threw, so every stream showed 16 bits. C#'s rule that
  32-bit/float sources count as 24 is kept.
- **Tags are matched case-insensitively**, so Matroska's `ENCODER` (writing
  application) and other upper-case tags are found.
- A `0/0` frame rate is `0`, not NaN; a failure reports ffprobe's reason.

### Running encoders: progress, pause, resume, stop

`--exec` runs any pipeline without a shell; separate commands with a `"|"`
argument. `ffmpeg`/`ffprobe` fall back to the bundled copy.

```bash
./build/ifmecli --root "/d/Program Files/IFME" --exec --frames 495 -- \
    ffmpeg -v error -i in.mp4 -strict -1 -f yuv4mpegpipe - "|" \
    "/d/Program Files/IFME/Plugins/h265_x265/x265.exe" --y4m - -o "out file.h265"
```

```
[ 49.7%] frame 246/495  42.22 fps  920 kb/s  ETA 00:00:06   [p]ause [r]esume [q]uit
```

In a terminal: **p** pauses, **r** resumes, **q** or **Ctrl+C** stops the whole
pipeline (Windows: every thread of the Job Object is suspended / the job is
terminated; Linux/macOS: SIGSTOP / SIGCONT / SIGKILL to the process group).
Progress is read from x264, x265, VVenC, SVT-AV1, NVEncC/QSVEncC and FFmpeg.
Compared with `ProcessManager.cs`:

- SVT-AV1 progress works (the plugin's `--progress 2` output is coloured with
  ANSI codes in a format C# did not match, so C# showed nothing).
- x264/x265 report their own total, which fills in an unknown frame count.
- The ETA is no longer rounded to whole fps first, so encoders slower than
  0.5 fps (VVenC at 4K) get one.

For the GUI the same is available as `ifme_task_*` in `ifme.h`; pause, resume
and stop are safe from any thread.

### Tests

```bash
cmake -B build -G Ninja -DIFME_TEST_ROOT="/d/Program Files/IFME"
cmake --build build
ctest --test-dir build --output-on-failure
```

- `units`: progress parser on real encoder output, line splitting, argument
  templates, relaxed JSON.
- `task`: a real `ffmpeg | x265` encode is paused (no frame may advance for
  2 s), resumed, then stopped while paused (must return within 3 s); plus
  stop-before-start, a failing command and a missing program. Skipped when
  `IFME_TEST_ROOT` is not set.

## Layout

```
core/include/ifme.h          public C API (extern "C", UTF-8, opaque handles)
core/src/core.c              context lifetime, string helpers
core/src/plugins.c           manifest loading, self-test
core/src/probe.c             media information from ffprobe
core/src/progress.c          frame / fps / bitrate / ETA from encoder output
core/src/task.c              public task API: exec, pause, resume, stop
core/src/argv.c              argument lists built from manifest templates
core/src/json_relax.c        accept comments / trailing commas
core/src/platform/paths.c    executable folder, per-user config folder
core/src/platform/fs.c       files, folders, PATH lookup (UTF-16 APIs on Windows)
core/src/platform/proc_*.c   run "a | b" pipelines and capture stdout/stderr:
                             Job Objects (Windows), process groups +
                             fork/exec + poll (Linux, macOS)
core/src/platform/cpu_*.c    CPU model and SIMD flags (x86 / everything else)
core/third_party/cjson/      cJSON 1.7.18 (MIT)
cli/ifmecli.c                command-line front-end
cli/exec_ui.c                --exec: live progress line, p/r/q keys, Ctrl+C
tests/                       ctest: unit tests and a real-pipeline test
gui/                         Qt front-end
res/                         Windows manifest and resource script
```

## Roadmap

- [x] 0. Skeleton: core library, CLI and GUI build on Windows; paths and CPU detection
- [x] 1. `--list-plugins` / `--self-test`: read the existing plugin manifests (cJSON),
      run `ffmpeg | encoder` pipelines without a shell
- [x] 2. `--probe FILE`: run ffprobe, parse its JSON; GUI accepts dropped files
- [x] 3. Process layer: live progress from stderr, pause/resume/stop on demand
      (Job Objects on Windows, process groups on Linux/macOS); `--exec`; ctest
- [ ] 4. Video encode (single and multi-pass) with progress and ETA
- [ ] 5. Audio encode and muxing
- [ ] 6. Subtitles, fonts, attachments, metadata
- [ ] 7. GUI on top of the finished core
