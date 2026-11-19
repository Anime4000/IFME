/*
 * ifme.h - public C interface of libifmecore
 *
 * Used by ifmecli (C) and the Qt GUI (C++). Everything that crosses this
 * boundary is plain C: UTF-8 strings, POD structs, opaque handles.
 *
 * License: GPL-2.0
 */

#ifndef IFME_H
#define IFME_H

#include <stdbool.h>
#include <stddef.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct ifme_ctx ifme_ctx;

typedef struct ifme_cpu_info {
	char brand[64];              /* e.g. "AMD Ryzen 9 3950X 16-Core Processor" */
	char arch[16];               /* "x86_64", "x86", "arm64", ...               */
	bool is_x86;                 /* false: the SIMD flags below are meaningless */
	bool sse42;
	bool avx;                    /* includes the OS (XGETBV) check              */
	bool avx2;
	bool fma3;
	bool avx512f;
	bool hypervisor;             /* running inside a virtual machine            */
	char hypervisor_vendor[16];  /* e.g. "KVMKVMKVM", "Microsoft Hv"            */
} ifme_cpu_info;

/* Library version, e.g. "0.1.0". Never NULL. */
const char *ifme_version(void);

/* Create / destroy a context. Returns NULL only when out of memory. */
ifme_ctx *ifme_create(void);
void      ifme_destroy(ifme_ctx *ctx);

/* Folder that holds the running executable (UTF-8, no trailing separator).
 * Bundled Plugins/ and resources are looked up relative to this. */
const char *ifme_app_dir(const ifme_ctx *ctx);

/* Per-user settings folder (UTF-8). Not created automatically.
 *   Windows: %APPDATA%\IFME
 *   Linux:   $XDG_CONFIG_HOME/ifme  (default ~/.config/ifme)
 *   macOS:   ~/Library/Application Support/IFME */
const char *ifme_config_dir(const ifme_ctx *ctx);

/* CPU model and instruction-set support, detected once in ifme_create(). */
const ifme_cpu_info *ifme_cpu(const ifme_ctx *ctx);

/* Folder that contains Plugins/ and Samples/ (UTF-8). Defaults to the app
 * folder; override it to use another IFME installation. Changing it unloads
 * any loaded plugins. Returns false when the folder does not exist. */
bool        ifme_set_root_dir(ifme_ctx *ctx, const char *dir);
const char *ifme_root_dir(const ifme_ctx *ctx);

/* ------------------------------------------------------------------------ */
/* Plugins                                                                  */
/* ------------------------------------------------------------------------ */

typedef enum ifme_plugin_kind {
	IFME_PLUGIN_AUDIO = 1,
	IFME_PLUGIN_VIDEO = 2
} ifme_plugin_kind;

typedef enum ifme_plugin_status {
	IFME_PLUGIN_OK = 0,          /* usable                                       */
	IFME_PLUGIN_BUILTIN,         /* "copy"/passthrough entry, no binary          */
	IFME_PLUGIN_WRONG_ARCH,      /* built for another CPU architecture           */
	IFME_PLUGIN_DUPLICATE,       /* GUID already provided by an earlier manifest */
	IFME_PLUGIN_MISSING_BINARY,  /* encoder program not found                    */
	IFME_PLUGIN_INVALID,         /* manifest could not be parsed                 */
	IFME_PLUGIN_TEST_FAILED,     /* self-test ran and failed                     */
	IFME_PLUGIN_DISABLED         /* switched off by the user                     */
} ifme_plugin_status;

typedef struct ifme_plugin_info {
	ifme_plugin_kind   kind;
	ifme_plugin_status status;
	const char *guid;            /* lower case, as written in the manifest       */
	const char *name;
	const char *version;         /* may be ""                                    */
	const char *developer;       /* may be ""                                    */
	const char *manifest;        /* full path of the _plugin.*.json file         */
	const char *binary;          /* resolved encoder program, "" when none       */
	const char *message;         /* why status is not OK, otherwise ""           */
	bool from_path;              /* binary was found through PATH               */
	bool test_required;          /* manifest asks for a self-test               */
} ifme_plugin_info;

/* Scan <root>/Plugins recursively for _plugin.a*.json / _plugin.v*.json.
 * Returns the number of usable plugins (OK or BUILTIN), or -1 when the
 * Plugins folder does not exist. Reloading replaces the previous list. */
int ifme_plugins_load(ifme_ctx *ctx);

size_t                  ifme_plugin_count(const ifme_ctx *ctx);
const ifme_plugin_info *ifme_plugin_at(const ifme_ctx *ctx, size_t index);
const char             *ifme_plugin_status_name(ifme_plugin_status status);

/* The user's disabled plugins (GUIDs; C#: Settings.PluginsDisabled). They get
 * IFME_PLUGIN_DISABLED now and after every reload: not found by name, not
 * self-tested, not offered. Replaces the previous list; NULL/0 clears it. */
void ifme_plugins_set_disabled(ifme_ctx *ctx, const char *const *guids, size_t count);

/* Every plugin as a JSON array, including each resolved manifest under
 * "manifest_json". Caller frees with ifme_free(). */
char *ifme_plugins_json(const ifme_ctx *ctx);

typedef enum ifme_test_outcome {
	IFME_TEST_PASS = 0,
	IFME_TEST_FAIL,
	IFME_TEST_SKIPPED            /* e.g. sample file or ffmpeg missing           */
} ifme_test_outcome;

typedef struct ifme_selftest_callbacks {
	void (*on_start)(void *user, const ifme_plugin_info *plugin);
	void (*on_result)(void *user, const ifme_plugin_info *plugin,
	                  ifme_test_outcome outcome, const char *detail,
	                  unsigned elapsed_ms);
	void *user;
} ifme_selftest_callbacks;

/* Encode the bundled sample with each usable plugin, like IFME's startup
 * test. Only plugins with "TestRequired" are tested unless test_all is set.
 * Failed plugins get IFME_PLUGIN_TEST_FAILED. Blocks until finished.
 * Returns the number of failures, or -1 if plugins are not loaded. */
int ifme_plugins_selftest(ifme_ctx *ctx, bool test_all,
                          const ifme_selftest_callbacks *cb);

/* ------------------------------------------------------------------------ */
/* Media information (ffprobe)                                              */
/* ------------------------------------------------------------------------ */

typedef struct ifme_stream_video {
	int id;                      /* ffprobe stream index (-map 0:id)             */
	const char *language;        /* "und" when untagged                          */
	const char *codec;           /* "h264", "hevc", ... or "unknown"             */
	const char *title;           /* may be ""                                    */
	const char *pix_fmt;         /* raw ffprobe value, may be ""                 */
	int width, height;
	int chroma;                  /* 420, 422, 444 (420 when unknown)             */
	int bit_depth;               /* 8, 10, 12, ...                               */
	double frame_rate;           /* r_frame_rate, 0 when unknown                 */
	double frame_rate_avg;       /* avg_frame_rate, 0 when unknown               */
	bool frame_rate_constant;    /* r_frame_rate == avg_frame_rate               */
	long long frame_count;       /* estimate: duration x average frame rate      */
	double duration;             /* seconds (container duration)                 */
	bool attached_pic;           /* cover art, not a real video track            */
} ifme_stream_video;

typedef struct ifme_stream_audio {
	int id;
	const char *language;
	const char *codec;
	const char *title;
	int sample_rate;             /* Hz                                           */
	int channels;
	int bit_depth;               /* 8, 16 or 24 (float/32-bit sources -> 24)     */
	double duration;
} ifme_stream_audio;

typedef struct ifme_stream_subtitle {
	int id;
	const char *language;
	const char *codec;           /* file extension style: "srt", "ass", "sup"... */
	const char *title;
} ifme_stream_subtitle;

typedef struct ifme_stream_attachment {
	int id;
	const char *filename;        /* "unknown" when untagged                      */
	const char *mime_type;       /* "application/octet-stream" when untagged     */
} ifme_stream_attachment;

typedef struct ifme_media {
	const char *path;
	const char *format_name;     /* e.g. "matroska,webm"                         */
	const char *format_long_name;
	const char *encoder;         /* writing application, may be ""               */
	unsigned long long file_size;   /* bytes                                     */
	unsigned long long bit_rate;    /* bits per second                           */
	double duration;                /* seconds                                   */

	size_t video_count, audio_count, subtitle_count, attachment_count;
	const ifme_stream_video      *video;
	const ifme_stream_audio      *audio;
	const ifme_stream_subtitle   *subtitle;
	const ifme_stream_attachment *attachment;
} ifme_media;

/* Run ffprobe on `path` (ffprobe from PATH, else <root>/Plugins/ffmpeg64).
 * frame_rate may be NULL; set it for image sequences ("img%04d.png").
 * Returns NULL on failure and writes a reason into err (if given).
 * Free the result with ifme_media_free(). */
ifme_media *ifme_probe(ifme_ctx *ctx, const char *path, const char *frame_rate,
                       char *err, size_t err_len);
void ifme_media_free(ifme_media *media);

/* The media as JSON; with include_raw the untouched ffprobe output is added
 * under "ffprobe". Caller frees with ifme_free(). */
char *ifme_media_json(const ifme_media *media, bool include_raw);

/* ------------------------------------------------------------------------ */
/* Running encoders: progress, pause, resume, stop                          */
/* ------------------------------------------------------------------------ */

typedef struct ifme_progress {
	long long frame;             /* frames done, -1 unknown                      */
	long long total_frames;      /* 0 unknown                                    */
	double percent;              /* 0..100, -1 unknown                           */
	double fps;                  /* -1 unknown                                   */
	double kbps;                 /* -1 unknown                                   */
	long long eta_sec;           /* -1 until enough samples / total unknown      */
	double time_sec;             /* media time done (FFmpeg "time="), -1 unknown */
	double total_sec;            /* media time to do, 0 unknown                  */
} ifme_progress;

typedef struct ifme_task ifme_task;

typedef struct ifme_task_callbacks {
	/* Every encoder output line. is_status: a progress-style line that a UI
	 * should show in place (status bar) rather than append to the log. */
	void (*on_output)(void *user, const char *line, bool is_status);
	/* Parsed progress (x264, x265, VVenC, SVT-AV1, NVEncC/QSVEncC, FFmpeg). */
	void (*on_progress)(void *user, const ifme_progress *progress);
	/* About every 50 ms on the thread running ifme_task_exec(). */
	void (*on_tick)(void *user);
	/* A new step of a job begins, e.g. "Video 1/1: pass 1 of 2" (ifme_encode). */
	void (*on_stage)(void *user, const char *stage);
	void *user;
} ifme_task_callbacks;

typedef enum ifme_task_result {
	IFME_TASK_DONE = 0,          /* finished, exit code 0                        */
	IFME_TASK_FAILED,            /* finished with a non-zero exit code           */
	IFME_TASK_STOPPED,           /* ended by ifme_task_stop()                    */
	IFME_TASK_ERROR              /* could not start (see err)                    */
} ifme_task_result;

/* A task controls one running pipeline. Create one per job. */
ifme_task *ifme_task_create(void);
void       ifme_task_destroy(ifme_task *task);

/* Safe from any thread, and from a signal / console-control handler. They
 * take effect within a few milliseconds; pause/resume act on every process
 * of the pipeline (Windows: all threads of the job; POSIX: SIGSTOP/SIGCONT
 * to the process group). Stop kills the whole pipeline. A stop requested
 * before ifme_task_exec() makes it return IFME_TASK_STOPPED at once. */
void ifme_task_pause(ifme_task *task);
void ifme_task_resume(ifme_task *task);
void ifme_task_stop(ifme_task *task);
bool ifme_task_is_paused(const ifme_task *task);

/* Run a pipeline and block until it ends. argv holds one or more commands
 * separated by a "|" element, e.g.
 *   { "ffmpeg", "-i", "in.mkv", "-f", "yuv4mpegpipe", "-", "|",
 *     "x265", "--y4m", "-", "-o", "out.h265" }
 * No shell is involved. "ffmpeg"/"ffprobe" fall back to the bundled copy.
 * total_frames (0 = unknown) enables percentage and ETA. */
ifme_task_result ifme_task_exec(ifme_task *task, ifme_ctx *ctx,
                                const char *const *argv, size_t argc,
                                const char *cwd, long long total_frames,
                                const ifme_task_callbacks *cb,
                                int *exit_code, char *err, size_t err_len);

/* ------------------------------------------------------------------------ */
/* Encoding jobs                                                            */
/* ------------------------------------------------------------------------ */

/* Find a usable plugin of `kind` by GUID, or by a case-insensitive part of
 * its name ("x265", "svt"). Returns its index, -1 if none matches, -2 if the
 * name matches more than one (err lists them). Loads plugins if needed. */
int ifme_plugin_find(ifme_ctx *ctx, ifme_plugin_kind kind, const char *query,
                     char *err, size_t err_len);

/* A job (JSON) for `input` with every setting filled in from the source
 * (ffprobe) and the plugins' defaults - what the GUI would show. Edit it, then
 * pass it to ifme_encode(). Each entry also carries a read-only "_help"
 * object (presets, modes with ranges, ...). Caller frees; NULL on error.
 *
 * output: NULL for "<input name>.ifme.<container>"; otherwise its extension
 * is the container (and decides the default audio plugin).
 *
 * video_plugin / audio_plugin: a GUID or a unique part of a plugin name,
 * "copy" (keep the streams as they are) or "none" (leave them out).
 * video_plugin NULL is "none". audio_plugin NULL picks a plugin that fits the
 * container: FFmpeg Opus (MKV, WebM), FFmpeg AAC (MP4), else the first
 * usable one listing it. Without an output, the container is the video
 * plugin's first format (the audio plugin's for audio-only jobs, e.g. Opus).
 * Every audio stream of the input gets an entry.
 *
 *   { "input": "...", "output": "...", "temp_dir": "", "keep_temp": false,
 *     "trim": { "start": "00:01:00", "duration": "00:00:30" },   (optional)
 *     "video": [ { "stream": 0, "language": "und", "plugin": "<guid>",
 *                  "preset": "medium", "tune": "", "mode": 0, "value": 25.5,
 *                  "passes": 1, "encoder_args": "", "width": 1920,
 *                  "height": 1080, "fps": "24000/1001", "bit_depth": 10,
 *                  "chroma": 420, "ffmpeg_args": "", "filter": "",
 *                  "deinterlace": { "mode": 0, "field": 1 } } ],  (optional)
 *     "audio": [ { "stream": 1, "language": "jpn", "title": "",
 *                  "plugin": "<guid>", "mode": 0, "quality": "128",
 *                  "sample_rate": 48000, "channels": 0, "copy": false,
 *                  "encoder_args": "", "ffmpeg_args": "", "filter": "" } ] }
 *
 * Any entry may have "enable": false. Audio "sample_rate"/"channels" 0 keep
 * the source; "copy": true tries the stream as it is first and encodes only
 * when the output container refuses it. An audio entry may add
 * "input": "other file" to take its stream from there (a separate dub).
 *
 * Everything else the output keeps (only keys that apply are written):
 *     "subtitles":   [ { "stream": 2, "language": "eng", "title": "",
 *                        "default": true, "input": "(optional) other.srt" } ],
 *     "burn_subtitle": false,      burn the first subtitle into the video
 *     "attachments": [ { "stream": 4, "filename": "font.ttf",
 *                        "mime_type": "...", "input": "(optional)" },
 *                      { "file": "C:/fonts/extra.ttf" } ],
 *     "cover":       { "stream": 5, "input": "(optional) art.jpg" },
 *     "metadata": true, "chapters": true,   the input's global tags / chapters
 *     "movflags": ""                         MP4 only, e.g. "+faststart"
 * Subtitles and attachments are only defaulted with video (as in C#).
 * MKV takes every subtitle and attachment; MP4 and WebM take text subtitles
 * (written as mov_text / WebVTT); other containers drop them with a log
 * line, so the default for those is to burn the first text subtitle in.
 * With a trim, chapters and text subtitles are cut to the same part. */
char *ifme_job_new(ifme_ctx *ctx, const char *input, const char *output,
                   const char *video_plugin, const char *audio_plugin,
                   char *err, size_t err_len);

/* Run a job and block until it ends. Progress, output lines and stages are
 * reported through cb; pause/resume/stop through `task` as with
 * ifme_task_exec(). Extracts subtitles, attachments, cover art, tags and
 * chapters, encodes the video and audio tracks, then writes everything to
 * "output" (container chosen by its extension). */
ifme_task_result ifme_encode(ifme_task *task, ifme_ctx *ctx, const char *job_json,
                             const ifme_task_callbacks *cb, char *err, size_t err_len);

/* Free memory returned by functions documented as "caller frees". */
void ifme_free(void *ptr);

#ifdef __cplusplus
}
#endif

#endif /* IFME_H */
