# IFME native port

Work-in-progress rewrite of IFME without Mono/.NET:

| Part | Language | Folder |
|---|---|---|
| `libifmecore`: plugins, probing, encoding pipeline, process control | C11 | `core/` |
| `ifmecli`: command-line front-end | C11 | `cli/` |
| `ifme9`: desktop GUI (IFME 9 alpha) | C++17 + Qt 6 Widgets | `gui/` |

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
make a portable copy that runs anywhere:

```bash
cmake --build build --target deploy
```

This fills `../IFME/bin/Qt` (next to the .NET `Release` folder) with
`ifme9.exe`, `ifmecli.exe`, the Qt and MinGW DLLs (`windeployqt6` plus every
DLL they import from UCRT64) and the `.wad` files from `../IFME/bin/Debug`.
For a combined release put its contents beside `IFME.exe`, `Plugins/` and
`Samples/`. Run from `bin/Qt` itself, it uses `bin/Debug`'s `Plugins/`.

On Linux the same target (`tools/deploy-linux.sh`) puts `ifme9`, `ifmecli`,
Qt and the libraries a desktop may lack in `lib/`, its plugins in
`lib/qt6/plugins/`, a `qt.conf` and the `.wad` files in `../IFME/bin/Qt`.
glibc, OpenGL, X11/xcb core and fontconfig come from the system, so the copy
runs on distributions as new as the build machine or newer. The programs are
linked with RPATH `$ORIGIN/lib`; `patchelf`, when installed, re-applies it.
The version is `9.0.0-alpha` (the .NET release is 8.x).

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
IFME core   9.0.0-alpha
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
./build/ifme9 --root "/d/Program Files/IFME"                          # GUI
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
- The user's disabled plugins (`ifme_plugins_set_disabled`) are not offered,
  found by name or tested; the GUI keeps the list and, as C#, adds plugins
  that fail the start-up test to it.

### Media information

```bash
./build/ifmecli --root "/d/Program Files/IFME" movie.mkv other.mp4     # same layout as IFME's MediaInfo
./build/ifmecli --root "/d/Program Files/IFME" --json movie.mkv        # add -v for the raw ffprobe output
./build/ifmecli -p --framerate 24 "frames/img%04d.png"                 # image sequence
```

In the GUI, the Media Info tab shows the same for the selected file.

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

### Encoding

```bash
# defaults from the source and the plugins (x265 unless --video says otherwise;
# audio: Opus for MKV/WebM, AAC for MP4, every audio track of the input)
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mkv

# two-pass 600 kb/s, 10-bit, 1280x720, only 00:01:00-00:01:30
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mp4 \
    --video x265 --mode 2 --value 600 --bit-depth 10 --size 1280x720 --start 60 --duration 30

# save the job, edit it, run it later (the "_help" block lists presets/modes)
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv --video svt --print-job > job.json
./build/ifmecli -r "/d/Program Files/IFME" --job job.json
```

```bash
# audio: another plugin and quality, 48 kHz stereo, copy when the container takes it
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mp4 \
    --audio "Nero" --audio-mode 1 --audio-quality 160 --sample-rate 48000 --channels 2
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mkv --audio-copy

# just change the container: remux video and audio as they are, encode only
# what the new format refuses (C#'s Smart Stream Copy / fast remux)
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mp4 --video-copy --audio-copy

# keep the video, re-encode only the audio; or audio only
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mkv --video copy --audio opus
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv --video none     # -> movie.ifme.opus

# add the audio tracks of other files (a dub, a commentary)
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mp4 -o out.mkv --add-audio dub.m4a

# subtitles, fonts, cover art: the input's are kept by default; add or burn more
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mp4 -o out.mkv     --add-subtitle signs.ass --add-attachment SignFont.ttf --cover poster.jpg
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mp4 --burn-subtitle
./build/ifmecli -r "/d/Program Files/IFME" --encode movie.mkv -o out.mp4     --no-chapters --no-metadata --movflags +faststart
```

`--video` / `--audio` take a GUID, any unique part of a plugin name ("x264",
"Fraunhofer", "H.265 Rigaya", "exhale"), `copy` or `none`. While encoding:
p pause, r resume, q / Ctrl+C stop (temp files are removed, no partial output
is left). In a job file, every audio entry has its own plugin, mode, quality,
sample rate, channels, language and title, and may name its own `"input"`;
any entry can be switched off with `"enable": false`.

This ports `MediaEncoding.Video()`, `.Audio()` and `.Muxing()`: the same
piped (`ffmpeg | encoder`) and direct command shapes, multi-pass, `{0}` mode
templates, raw streams wrapped by MP4Box; audio through a 16-bit WAV pipe or
directly, with the manifest's down-/up-mix rules (`MultiChannelSupport`,
`MonoSupport`, `SingleChannelOnly`); `.Extract()`: subtitles, attachments
(fonts), cover art, global tags and chapters (`metadata.ini`), and burning a
subtitle into the picture; then every track written to the output by ffmpeg
with language, title and default flag per stream and C#'s `encoding_tool` /
`comment` tags. Checked on Windows with x264 (10-bit 4:2:2), x265 (CRF,
2-pass), SVT-AV1, VVenC, VP9 and NVEncC for video, and FFmpeg AAC / Opus,
Nero AAC and exhale (piped), AMR-NB (mono only) for audio, plus stream copies
into MKV, MP4 and WebM; SRT/ASS subtitles, a font and a cover into MKV, MP4
(`mov_text`), WebM (WebVTT); ASS and SRT burned into MP4 / TS; cover art
into MP3, FLAC and M4A. Not yet ported: auto-crop.

Where things go: MKV takes every subtitle (pictures too) and attachment, with
cover art as a `cover.jpg` attachment; MP4 and WebM take text subtitles;
cover art also goes into MP4, M4A, MP3 and FLAC. A container that cannot hold
something gets a log line instead of a failed mux, and for containers without
soft subtitles (TS, AVI, ...) the default is to burn the first text subtitle
in, as C# did.

Differences from C#:

- Frame rates such as 24000/1001 stay exact; a missing N-bit encoder is an
  error message instead of a crash; scaling is only added when the size
  changes; MP4Box gets the frame rate explicitly, and ffmpeg wraps raw streams
  when MP4Box is absent.
- Audio progress is shown (FFmpeg's `time=` against the length); C# had none.
- Down-/up-mixing follows the chosen channel count; C# looked only at the
  source, so mono picked for a stereo source on an eSBR mode became stereo.
- `--audio-copy` (C#'s "fast remux") also works on trimmed jobs, and copying
  a video stream now honours the trim. `--video-copy` (the Video tab's fast
  remux) is skipped on trimmed jobs, as C#: a copied stream can only be cut
  at key frames, so those are encoded.
- Track titles are written where MP4 players read them (`handler_name`).
- Trimmed jobs get their chapters and text subtitles cut to the same part:
  events are shifted, clipped and dropped as needed (C# kept the chapters of
  the whole file; FFmpeg alone drops subtitle lines that start before the cut
  and lets lines past the end stretch the file). Chapters the cut empties are
  removed.
- The input's global tags (title, artist, ...) really reach the output; C#
  mapped them from the first encoded track by mistake, so only chapters came
  through. Chapters only come from the input, never by accident from a track
  file (neroAacEnc writes one).
- Attachment names from the source are reduced to a plain file name; in C#
  a name such as `../../x.ttf` was written outside the temp folder. Every
  attachment is kept (C# skipped the one with stream index 0).
- Other text subtitle formats (e.g. `mov_text`) become SRT so they fit MKV;
  a picture subtitle asked to be burned in is an error, not silently ignored.
- The first video and audio track are flagged default explicitly (setting a
  subtitle's default flag otherwise left the audio unflagged).

### The GUI

```bash
./build/ifme9 --root "/d/Program Files/IFME"        # remembered in Preferences
./build/ifme9 movie.mkv other.mp4                    # add files (or folders)
./build/ifme9 -f mp4 -o out --start --quit-when-done movie.mkv   # batch, exit code 0 if all worked
```

The same window as IFME today, on the finished core:

- **Splash screen** (IFME's `SplashScreen14` artwork) while the plugins load
  and, as C#, the plugins that ask for it (GPU encoders) are tested; the ones
  that fail are disabled from then on (Preferences can turn the test off,
  the Plugins window turns plugins back on).
- **Banner** on top (the right image fades over the left one), the toolbar
  under it, the queue (name, in ► out format,
  duration, size, status, progress bar), and the tabs **Media Info**,
  **Video**, **Audio**, **Subtitle**, **Attachment**, **Advanced** (trim,
  cover art, tags, chapters, MP4 flags) and **Logging**. Drop files or
  folders onto the window.
- Languages are picked by name from `Language.json` in `jason.wad`, as C#
  does (a built-in copy of `../IFME/Language.json` when there is no wad):
  a Language box on the Video, Audio and Subtitle tabs, or double-click a
  track's Language cell.
- Lists select whole rows and keep the selection visible. Track changes
  (language, encoder settings) go to the selected tracks, or all tracks
  when none is selected, in every file selected in the queue; with several
  files, selected tracks are matched by position.
- Settings come from the plugin manifests (presets, tunes, rate-control modes
  with their ranges, bit depths, chroma; audio modes, qualities, sample rates,
  channels). Encoder-wide settings apply to every selected file; preset, mode
  and quality only to files using the same plugin; track edits (on/off,
  language, title, default) to the file shown. Changing the format swaps
  encoders the new format cannot hold, as C# did; new files get the encoders
  chosen last.
- Extra audio, subtitle and font files can be added per file; cover art can
  be kept, dropped or replaced.
- **Start / Pause / Stop** run the queue on a worker thread with live
  progress and ETA; the log shows every command and encoder line.
- **About** (black, as frmAbout) with the banner and the four characters;
  the splash has the acrylic blur of C# on Windows 10 1803 and later; IFME's
  icon on the window and in `ifme9.exe` / `ifmecli.exe`.
- Settings live in `ifme-qt.ini` in IFME's config folder.

The artwork is used as is from `../IFME/Resources` (one copy in the repository).
Not yet: profiles, translations (i18n), file name prefix/postfix, the C#
"shut down when done" option, a macOS `.icns` app icon and a Linux
`.desktop` file.

### Tests

```bash
cmake -B build -G Ninja -DIFME_TEST_ROOT="/d/Program Files/IFME"
cmake --build build
ctest --test-dir build --output-on-failure
```

- `units`: progress parser on real encoder output (frames and FFmpeg's
  audio `time=`), cutting SRT/ASS/WebVTT and chapters to a trim, line
  splitting, argument templates, relaxed JSON.
- `task`: a real `ffmpeg | x265` encode is paused (no frame may advance for
  2 s), resumed, then stopped while paused (must return within 3 s); plus
  stop-before-start, a failing command and a missing program. Skipped when
  `IFME_TEST_ROOT` is not set.
- `encode`: job defaults, a short trimmed/downscaled x265 encode to a file
  with a Japanese name (checked with ffprobe), stop during an encode (temp
  folder removed, no output left), the default audio plugin per container,
  an x265 + Opus encode taking its audio from a separate file (language,
  title, length checked), AAC refused by WebM falling back to Opus, external
  SRT/ASS subtitles with an attachment and cover art into MKV (the SRT read
  back to check the cut), a subtitle burned into a .ts, and clear errors for
  bad jobs.

## Layout

```
core/include/ifme.h          public C API (extern "C", UTF-8, opaque handles)
core/src/core.c              context lifetime, string helpers
core/src/plugins.c           manifest loading, self-test
core/src/probe.c             media information from ffprobe
core/src/progress.c          frame / fps / bitrate / ETA from encoder output
core/src/task.c              public task API: exec, pause, resume, stop
core/src/encode.c            encoding jobs: defaults (ifme_job_new), video and
                             audio encode, muxing into the output
core/src/extras.c            subtitles, attachments, cover art, tags, chapters,
                             burning subtitles
core/src/subtime.c           cut text subtitles and chapters to a trim
core/src/jsonutil.c          lenient JSON accessors (Newtonsoft-compatible)
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
cli/exec_ui.c                live progress line, p/r/q keys, Ctrl+C (--exec, --encode)
cli/encode_cmd.c             --encode / --job / --print-job
tests/                       ctest: unit tests and a real-pipeline test
gui/main.cpp                 command line, splash, then the main window
gui/mainwindow.cpp           queue, format / output folder, start / pause / stop
gui/settingstabs.cpp         Video, Audio, Subtitle, Attachment, Advanced tabs
gui/encodeworker.cpp         runs ifme_encode() on a worker thread
gui/splash.cpp, aboutdialog.cpp, dialogs.cpp, widgets.cpp
                             splash, About, Plugins / Preferences, banner
gui/app.cpp                  core context, plugin list, settings, JSON helpers
gui/resources.qrc            IFME's icon and artwork (from ../IFME/Resources)
res/                         Windows manifest, resource scripts (icon)
```

## Roadmap

- [x] 0. Skeleton: core library, CLI and GUI build on Windows; paths and CPU detection
- [x] 1. `--list-plugins` / `--self-test`: read the existing plugin manifests (cJSON),
      run `ffmpeg | encoder` pipelines without a shell
- [x] 2. `--probe FILE`: run ffprobe, parse its JSON; GUI accepts dropped files
- [x] 3. Process layer: live progress from stderr, pause/resume/stop on demand
      (Job Objects on Windows, process groups on Linux/macOS); `--exec`; ctest
- [x] 4. Video encode (single and multi-pass) with progress and ETA; job JSON
- [x] 5. Audio encode (piped, direct, copy with fallback, extra files) and muxing
- [x] 6. Subtitles (soft and burned), fonts/attachments, cover art, tags, chapters
- [x] 7. GUI on top of the finished core: splash, queue, settings tabs, encoding, plugins, About
