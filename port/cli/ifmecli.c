/*
 * ifmecli.c - command-line front-end for libifmecore
 *
 * License: GPL-2.0
 */

#include <getopt.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "ifme.h"
#include "exec_ui.h"
#include "encode_cmd.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

static bool verbose = false;

static void print_usage(FILE *out)
{
	fprintf(out,
		"Usage: ifmecli [OPTIONS] [FILE...]\n"
		"       ifmecli [OPTIONS] --exec [--frames N] -- COMMAND [ARGS] [\"|\" COMMAND [ARGS]]...\n"
		"       ifmecli [OPTIONS] --encode INPUT [-o OUTPUT] [--video PLUGIN] [ENCODE OPTIONS]\n"
		"       ifmecli [OPTIONS] --job JOB.json\n"
		"\n"
		"Encoding (defaults come from the source and the plugins):\n"
		"  -e, --encode INPUT   Encode INPUT (keys while running: p pause, r resume, q stop)\n"
		"  -o, --output FILE    Output file; its extension picks the container\n"
		"                       (default: INPUT name + .ifme.<plugin container>)\n"
		"      --video PLUGIN   Video plugin: GUID or part of its name (default: x265),\n"
		"                       \"copy\" (keep the stream) or \"none\" (audio only)\n"
		"      --preset NAME    --tune NAME     encoder preset / tune\n"
		"      --mode N         Rate-control mode index (see --print-job \"_help\")\n"
		"      --value V        CRF / QP / bitrate for that mode\n"
		"      --passes N       Passes for multi-pass modes (default 2)\n"
		"      --size WxH       --fps RATE (e.g. 24000/1001)\n"
		"      --bit-depth N    --chroma 420|422|444\n"
		"      --start T        --duration T    encode only a part (HH:MM:SS or seconds)\n"
		"      --encoder-args S Extra encoder arguments; --filter S extra ffmpeg -vf filter\n"
		"      --video-copy     Remux video as it is when the container takes it, else encode\n"
		"      --audio PLUGIN   Audio plugin for every audio track, \"copy\" or \"none\"\n"
		"                       (default: Opus for MKV/WebM, AAC for MP4)\n"
		"      --audio-mode N   --audio-quality Q   mode index and quality (see _help)\n"
		"      --sample-rate HZ --channels N    0 keeps the source\n"
		"      --audio-copy     Copy audio as it is when the container takes it, else encode\n"
		"      --audio-args S   --audio-filter S    extra encoder arguments / ffmpeg -af\n"
		"      --add-audio FILE Also take the audio tracks of FILE (repeatable, up to 8)\n"
		"      --add-subtitle FILE    Also take the subtitles of FILE (.srt, .ass, or a video)\n"
		"      --add-attachment FILE  Attach FILE, e.g. a font (MKV)\n"
		"      --burn-subtitle  Burn the first subtitle into the video (text formats)\n"
		"      --cover IMAGE    Cover art (MKV, MP4, M4A, MP3, FLAC); default: the input's\n"
		"      --no-subtitles   --no-attachments   --no-cover   leave those out\n"
		"      --no-metadata    --no-chapters      drop the input's tags / chapters\n"
		"      --movflags F     MP4 -movflags, e.g. +faststart or +frag_keyframe+empty_moov\n"
		"      --temp-dir DIR   --keep-temp     where intermediate files go / keep them\n"
		"      --print-job      Print the job as JSON instead of running it\n"
		"      --job FILE       Run a job file (as printed by --print-job, then edited)\n"
		"\n"
		"Options:\n"
		"  -i, --info           Show system information (paths, CPU features)\n"
		"  -x, --exec           Run COMMAND (a pipeline: separate commands with a \"|\"\n"
		"                       argument) with live progress, no shell involved.\n"
		"                       Keys: p pause, r resume, q / Ctrl+C stop\n"
		"      --frames N       With --exec: total frames, for percentage and ETA\n"
		"  -p, --probe          Show media information for each FILE (ffprobe)\n"
		"      --framerate N    With --probe: frame rate for image sequences\n"
		"                       (e.g. ifmecli -p --framerate 24 img%%04d.png)\n"
		"  -l, --list-plugins   List encoder plugins and their status\n"
		"  -t, --self-test      Encode the sample files with each plugin that asks\n"
		"                       for a test (\"TestRequired\")\n"
		"  -a, --all            With --self-test: test every usable plugin\n"
		"      --json           Print JSON instead of text (--probe, --list-plugins);\n"
		"                       with --verbose, --probe adds the raw ffprobe output\n"
		"  -r, --root DIR       IFME folder holding Plugins/ and Samples/\n"
		"                       (default: the folder of this program)\n"
		"  -v, --verbose        More detail (paths, full failure output)\n"
		"  -V, --version        Show version and exit\n"
		"  -h, --help           Show this message and exit\n");
}

static const char *yes_no(bool v)
{
	return v ? "yes" : "no";
}

static void print_info(const ifme_ctx *ctx)
{
	const ifme_cpu_info *cpu = ifme_cpu(ctx);

	printf("IFME core   %s\n", ifme_version());
	printf("App folder  %s\n", ifme_app_dir(ctx));
	printf("Root        %s\n", ifme_root_dir(ctx));
	printf("Config      %s\n", ifme_config_dir(ctx));
	printf("CPU         %s (%s)\n", cpu->brand, cpu->arch);

	if (cpu->is_x86) {
		printf("SIMD        SSE4.2:%s AVX:%s AVX2:%s FMA3:%s AVX-512F:%s\n",
		       yes_no(cpu->sse42), yes_no(cpu->avx), yes_no(cpu->avx2),
		       yes_no(cpu->fma3), yes_no(cpu->avx512f));
	}

	if (cpu->hypervisor)
		printf("Hypervisor  %s\n", cpu->hypervisor_vendor[0] ? cpu->hypervisor_vendor : "unknown");
}

static bool load_plugins(ifme_ctx *ctx)
{
	if (ifme_plugins_load(ctx) < 0) {
		fprintf(stderr, "ifmecli: no Plugins folder in %s (use --root DIR)\n", ifme_root_dir(ctx));
		return false;
	}
	return true;
}

/* Print `text` indented, one line at a time. */
static void print_indented(const char *text, const char *indent)
{
	const char *p = text;
	while (*p) {
		const char *nl = strchr(p, '\n');
		size_t len = nl ? (size_t)(nl - p) : strlen(p);
		printf("%s%.*s\n", indent, (int)len, p);
		p += len + (nl ? 1 : 0);
	}
}

static void print_plugin_table(const ifme_ctx *ctx)
{
	size_t n = ifme_plugin_count(ctx);
	int usable = 0;

	for (int pass = 0; pass < 2; pass++) {
		ifme_plugin_kind kind = pass == 0 ? IFME_PLUGIN_AUDIO : IFME_PLUGIN_VIDEO;
		printf("%s plugins\n", kind == IFME_PLUGIN_AUDIO ? "Audio" : "Video");

		for (size_t i = 0; i < n; i++) {
			const ifme_plugin_info *p = ifme_plugin_at(ctx, i);
			if (p->kind != kind)
				continue;

			bool ok = p->status == IFME_PLUGIN_OK || p->status == IFME_PLUGIN_BUILTIN;
			usable += ok;

			printf("  %-11s %s%s%s%s\n", ifme_plugin_status_name(p->status), p->name,
			       *p->version ? " " : "", p->version, p->from_path ? "  [PATH]" : "");

			if (!ok && *p->message && !verbose) {
				/* First line only; --verbose shows everything. */
				const char *nl = strchr(p->message, '\n');
				printf("              %.*s\n", nl ? (int)(nl - p->message) : (int)strlen(p->message), p->message);
			}

			if (verbose) {
				printf("              guid     %s\n", p->guid);
				printf("              manifest %s\n", p->manifest);
				if (*p->binary)
					printf("              binary   %s\n", p->binary);
				if (*p->message)
					print_indented(p->message, "              ");
			}
		}
		printf("\n");
	}

	printf("%d of %zu plugins usable\n", usable, n);
}

/* Same layout as MediaInfo.Print() in the C# version. */
#define ROW "%-28s: "

static void print_media(const ifme_media *m)
{
	printf("General\n");
	printf(ROW "%s\n", "Complete name", m->path);
	printf(ROW "%s (%s)\n", "Format", m->format_name, m->format_long_name);
	printf(ROW "%llu bytes\n", "File size", m->file_size);
	printf(ROW "%.3f seconds\n", "Duration", m->duration);
	printf(ROW "%llu bps\n", "Overall bit rate", m->bit_rate);
	printf(ROW "%s\n", "Writing Application", m->encoder);

	for (size_t i = 0; i < m->video_count; i++) {
		const ifme_stream_video *v = &m->video[i];
		printf("\nVideo #%zu%s\n", i, v->attached_pic ? " (cover art)" : "");
		printf(ROW "%d\n", "ID", v->id);
		printf(ROW "%s\n", "Format", v->codec);
		printf(ROW "%d\n", "Width", v->width);
		printf(ROW "%d\n", "Height", v->height);
		printf(ROW "%s\n", "Frame rate mode", v->frame_rate_constant ? "Constant" : "Variable");
		printf(ROW "%.3f fps (%.3f fps)\n", "Frame rate", v->frame_rate, v->frame_rate_avg);
		printf(ROW "%lld (estimated)\n", "Frame count", v->frame_count);
		printf(ROW "%d (%s)\n", "Chroma subsampling", v->chroma, v->pix_fmt);
		printf(ROW "%d bits\n", "Bit depth", v->bit_depth);
		printf(ROW "%s\n", "Language", v->language);
		printf(ROW "%s\n", "Title", v->title);
	}

	for (size_t i = 0; i < m->audio_count; i++) {
		const ifme_stream_audio *a = &m->audio[i];
		printf("\nAudio #%zu\n", i);
		printf(ROW "%d\n", "ID", a->id);
		printf(ROW "%s\n", "Format", a->codec);
		printf(ROW "%d\n", "Channel(s)", a->channels);
		printf(ROW "%d Hz\n", "Sampling rate", a->sample_rate);
		printf(ROW "%d bits\n", "Bit Depth", a->bit_depth);
		printf(ROW "%s\n", "Language", a->language);
		printf(ROW "%s\n", "Title", a->title);
	}

	for (size_t i = 0; i < m->subtitle_count; i++) {
		const ifme_stream_subtitle *s = &m->subtitle[i];
		printf("\nSubtitles #%zu\n", i);
		printf(ROW "%d\n", "ID", s->id);
		printf(ROW "%s\n", "Format", s->codec);
		printf(ROW "%s\n", "Language", s->language);
		printf(ROW "%s\n", "Title", s->title);
	}

	for (size_t i = 0; i < m->attachment_count; i++) {
		const ifme_stream_attachment *t = &m->attachment[i];
		printf("\nAttachments #%zu\n", i);
		printf(ROW "%d\n", "ID", t->id);
		printf(ROW "%s\n", "Name", t->filename);
		printf(ROW "%s\n", "MIME", t->mime_type);
	}
}

/* Probe every file; returns the number of failures. */
static int probe_files(ifme_ctx *ctx, char **files, int count, const char *frame_rate, bool json)
{
	int failures = 0;

	if (json && count > 1)
		printf("[\n");

	for (int i = 0; i < count; i++) {
		char err[512];
		ifme_media *m = ifme_probe(ctx, files[i], frame_rate, err, sizeof(err));

		if (!m) {
			fprintf(stderr, "ifmecli: %s: %s\n", files[i], err);
			failures++;
			continue;
		}

		if (json) {
			char *text = ifme_media_json(m, verbose);
			if (text)
				printf("%s%s\n", text, (count > 1 && i + 1 < count) ? "," : "");
			ifme_free(text);
		} else {
			if (i > 0)
				printf("\n\n");
			print_media(m);
		}

		ifme_media_free(m);
	}

	if (json && count > 1)
		printf("]\n");
	return failures;
}

static void on_test_start(void *user, const ifme_plugin_info *p)
{
	(void)user;
	printf("  %-48s ", p->name);
	fflush(stdout);
}

static void on_test_result(void *user, const ifme_plugin_info *p, ifme_test_outcome outcome,
                           const char *detail, unsigned elapsed_ms)
{
	int *counts = user;
	(void)p;

	const char *label = outcome == IFME_TEST_PASS ? "PASS" : outcome == IFME_TEST_FAIL ? "FAIL" : "SKIP";
	counts[outcome]++;
	printf("%s  %5.1f s", label, elapsed_ms / 1000.0);

	if (outcome == IFME_TEST_PASS) {
		printf("  (%s)\n", detail);
		return;
	}

	printf("\n");
	if (verbose || outcome == IFME_TEST_SKIPPED) {
		print_indented(detail, "      ");
	} else {
		/* Reason and the encoder's last line; --verbose adds the commands. */
		const char *nl = strchr(detail, '\n');
		printf("      %.*s\n", nl ? (int)(nl - detail) : (int)strlen(detail), detail);
	}
}

#define ARRAY_LEN(a) ((int)(sizeof(a) / sizeof((a)[0])))

/* A repeatable --add-* option. */
static bool add_file(const char **list, int *count, int max, const char *what, const char *file)
{
	if (*count >= max) {
		fprintf(stderr, "ifmecli: at most %d --add-%s files\n", max, what);
		return false;
	}
	list[(*count)++] = file;
	return true;
}

int main(int argc, char *argv[])
{
#ifdef _WIN32
	/* The manifest sets the process code page to UTF-8; make the console match. */
	SetConsoleOutputCP(CP_UTF8);
#endif

	enum {
		OPT_JSON = 1000, OPT_FRAMERATE, OPT_FRAMES, OPT_VIDEO, OPT_PRESET, OPT_TUNE, OPT_MODE,
		OPT_VALUE, OPT_PASSES, OPT_SIZE, OPT_FPS, OPT_BITS, OPT_CHROMA, OPT_START, OPT_DURATION,
		OPT_ENC_ARGS, OPT_FILTER, OPT_TEMP, OPT_KEEP_TEMP, OPT_PRINT_JOB, OPT_JOB,
		OPT_AUDIO, OPT_AUDIO_MODE, OPT_AUDIO_QUALITY, OPT_SAMPLE_RATE, OPT_CHANNELS,
		OPT_AUDIO_COPY, OPT_VIDEO_COPY, OPT_AUDIO_ARGS, OPT_AUDIO_FILTER, OPT_ADD_AUDIO,
		OPT_ADD_SUB, OPT_ADD_ATT, OPT_COVER, OPT_MOVFLAGS, OPT_NO_SUBS, OPT_NO_ATT, OPT_NO_COVER,
		OPT_NO_META, OPT_NO_CHAPTERS, OPT_BURN
	};
	static const struct option long_opts[] = {
		{ "info",         no_argument,       NULL, 'i' },
		{ "exec",         no_argument,       NULL, 'x' },
		{ "frames",       required_argument, NULL, OPT_FRAMES },
		{ "encode",       required_argument, NULL, 'e' },
		{ "output",       required_argument, NULL, 'o' },
		{ "video",        required_argument, NULL, OPT_VIDEO },
		{ "preset",       required_argument, NULL, OPT_PRESET },
		{ "tune",         required_argument, NULL, OPT_TUNE },
		{ "mode",         required_argument, NULL, OPT_MODE },
		{ "value",        required_argument, NULL, OPT_VALUE },
		{ "passes",       required_argument, NULL, OPT_PASSES },
		{ "size",         required_argument, NULL, OPT_SIZE },
		{ "fps",          required_argument, NULL, OPT_FPS },
		{ "bit-depth",    required_argument, NULL, OPT_BITS },
		{ "chroma",       required_argument, NULL, OPT_CHROMA },
		{ "start",        required_argument, NULL, OPT_START },
		{ "duration",     required_argument, NULL, OPT_DURATION },
		{ "encoder-args", required_argument, NULL, OPT_ENC_ARGS },
		{ "filter",       required_argument, NULL, OPT_FILTER },
		{ "audio",        required_argument, NULL, OPT_AUDIO },
		{ "audio-mode",   required_argument, NULL, OPT_AUDIO_MODE },
		{ "audio-quality",required_argument, NULL, OPT_AUDIO_QUALITY },
		{ "sample-rate",  required_argument, NULL, OPT_SAMPLE_RATE },
		{ "channels",     required_argument, NULL, OPT_CHANNELS },
		{ "audio-copy",   no_argument,       NULL, OPT_AUDIO_COPY },
		{ "video-copy",   no_argument,       NULL, OPT_VIDEO_COPY },
		{ "audio-args",   required_argument, NULL, OPT_AUDIO_ARGS },
		{ "audio-filter", required_argument, NULL, OPT_AUDIO_FILTER },
		{ "add-audio",    required_argument, NULL, OPT_ADD_AUDIO },
		{ "add-subtitle", required_argument, NULL, OPT_ADD_SUB },
		{ "add-attachment", required_argument, NULL, OPT_ADD_ATT },
		{ "cover",        required_argument, NULL, OPT_COVER },
		{ "movflags",     required_argument, NULL, OPT_MOVFLAGS },
		{ "no-subtitles", no_argument,       NULL, OPT_NO_SUBS },
		{ "no-attachments", no_argument,     NULL, OPT_NO_ATT },
		{ "no-cover",     no_argument,       NULL, OPT_NO_COVER },
		{ "no-metadata",  no_argument,       NULL, OPT_NO_META },
		{ "no-chapters",  no_argument,       NULL, OPT_NO_CHAPTERS },
		{ "burn-subtitle", no_argument,      NULL, OPT_BURN },
		{ "temp-dir",     required_argument, NULL, OPT_TEMP },
		{ "keep-temp",    no_argument,       NULL, OPT_KEEP_TEMP },
		{ "print-job",    no_argument,       NULL, OPT_PRINT_JOB },
		{ "job",          required_argument, NULL, OPT_JOB },
		{ "probe",        no_argument,       NULL, 'p' },
		{ "framerate",    required_argument, NULL, OPT_FRAMERATE },
		{ "list-plugins", no_argument,       NULL, 'l' },
		{ "self-test",    no_argument,       NULL, 't' },
		{ "all",          no_argument,       NULL, 'a' },
		{ "json",         no_argument,       NULL, OPT_JSON },
		{ "root",         required_argument, NULL, 'r' },
		{ "verbose",      no_argument,       NULL, 'v' },
		{ "version",      no_argument,       NULL, 'V' },
		{ "help",         no_argument,       NULL, 'h' },
		{ NULL, 0, NULL, 0 }
	};

	bool want_info = false, want_list = false, want_test = false, want_probe = false;
	bool want_exec = false, test_all = false, json = false;
	const char *root = NULL, *frame_rate = NULL;
	long long frames = 0;
	int opt;

	encode_opts eo;
	memset(&eo, 0, sizeof(eo));
	eo.mode = -1;
	eo.audio_mode = eo.sample_rate = eo.channels = -1;
	bool want_encode = false;

	while ((opt = getopt_long(argc, argv, "ixe:o:pltar:vVh", long_opts, NULL)) != -1) {
		switch (opt) {
		case 'i': want_info = true; break;
		case 'x': want_exec = true; break;
		case OPT_FRAMES: frames = atoll(optarg); break;
		case 'e': want_encode = true; eo.input = optarg; break;
		case 'o': eo.output = optarg; break;
		case OPT_VIDEO: eo.video = optarg; break;
		case OPT_PRESET: eo.preset = optarg; break;
		case OPT_TUNE: eo.tune = optarg; break;
		case OPT_MODE: eo.mode = atoi(optarg); break;
		case OPT_VALUE: eo.value = optarg; break;
		case OPT_PASSES: eo.passes = atoi(optarg); break;
		case OPT_SIZE: eo.size = optarg; break;
		case OPT_FPS: eo.fps = optarg; break;
		case OPT_BITS: eo.bit_depth = atoi(optarg); break;
		case OPT_CHROMA: eo.chroma = atoi(optarg); break;
		case OPT_START: eo.trim_start = optarg; break;
		case OPT_DURATION: eo.trim_duration = optarg; break;
		case OPT_ENC_ARGS: eo.encoder_args = optarg; break;
		case OPT_FILTER: eo.filter = optarg; break;
		case OPT_AUDIO: eo.audio = optarg; break;
		case OPT_AUDIO_MODE: eo.audio_mode = atoi(optarg); break;
		case OPT_AUDIO_QUALITY: eo.audio_quality = optarg; break;
		case OPT_SAMPLE_RATE: eo.sample_rate = atoi(optarg); break;
		case OPT_CHANNELS: eo.channels = atoi(optarg); break;
		case OPT_AUDIO_COPY: eo.audio_copy = true; break;
		case OPT_VIDEO_COPY: eo.video_copy = true; break;
		case OPT_AUDIO_ARGS: eo.audio_args = optarg; break;
		case OPT_AUDIO_FILTER: eo.audio_filter = optarg; break;
		case OPT_ADD_AUDIO:
			if (!add_file(eo.add_audio, &eo.add_audio_count, ARRAY_LEN(eo.add_audio), "audio", optarg))
				return 2;
			break;
		case OPT_ADD_SUB:
			if (!add_file(eo.add_subtitle, &eo.add_subtitle_count, ARRAY_LEN(eo.add_subtitle), "subtitle", optarg))
				return 2;
			break;
		case OPT_ADD_ATT:
			if (!add_file(eo.add_attachment, &eo.add_attachment_count, ARRAY_LEN(eo.add_attachment),
			              "attachment", optarg))
				return 2;
			break;
		case OPT_COVER: eo.cover = optarg; break;
		case OPT_MOVFLAGS: eo.movflags = optarg; break;
		case OPT_NO_SUBS: eo.no_subtitles = true; break;
		case OPT_NO_ATT: eo.no_attachments = true; break;
		case OPT_NO_COVER: eo.no_cover = true; break;
		case OPT_NO_META: eo.no_metadata = true; break;
		case OPT_NO_CHAPTERS: eo.no_chapters = true; break;
		case OPT_BURN: eo.burn_subtitle = true; break;
		case OPT_TEMP: eo.temp_dir = optarg; break;
		case OPT_KEEP_TEMP: eo.keep_temp = true; break;
		case OPT_PRINT_JOB: eo.print_job = true; break;
		case OPT_JOB: want_encode = true; eo.job_file = optarg; break;
		case 'p': want_probe = true; break;
		case OPT_FRAMERATE: frame_rate = optarg; break;
		case 'l': want_list = true; break;
		case 't': want_test = true; break;
		case 'a': test_all = true; break;
		case OPT_JSON: json = true; break;
		case 'r': root = optarg; break;
		case 'v': verbose = true; break;
		case 'V':
			printf("ifmecli %s\n", ifme_version());
			return 0;
		case 'h':
			print_usage(stdout);
			return 0;
		default:
			print_usage(stderr);
			return 2;
		}
	}

	if (want_encode) {
		ifme_ctx *ctx = ifme_create();
		if (!ctx || (root && !ifme_set_root_dir(ctx, root))) {
			fprintf(stderr, "ifmecli: %s\n", ctx ? "root folder not found" : "out of memory");
			ifme_destroy(ctx);
			return 2;
		}

		int status = cmd_encode(ctx, &eo);
		ifme_destroy(ctx);
		return status;
	}

	if (want_exec) {
		if (optind >= argc) {
			fprintf(stderr, "ifmecli: --exec needs a command after \"--\"\n");
			return 2;
		}

		ifme_ctx *ctx = ifme_create();
		if (!ctx || (root && !ifme_set_root_dir(ctx, root))) {
			fprintf(stderr, "ifmecli: %s\n", ctx ? "root folder not found" : "out of memory");
			ifme_destroy(ctx);
			return 2;
		}

		int status = run_exec(ctx, argv + optind, argc - optind, frames);
		ifme_destroy(ctx);
		return status;
	}

	/* Bare files mean --probe. */
	if (optind < argc && !want_list && !want_test && !want_info)
		want_probe = true;

	if (want_probe && optind >= argc) {
		fprintf(stderr, "ifmecli: --probe needs at least one FILE\n");
		return 2;
	}

	if (!want_info && !want_list && !want_test && !want_probe) {
		print_usage(stdout);
		return 0;
	}

	ifme_ctx *ctx = ifme_create();
	if (!ctx) {
		fprintf(stderr, "ifmecli: out of memory\n");
		return 1;
	}

	int rc = 0;

	if (root && !ifme_set_root_dir(ctx, root)) {
		fprintf(stderr, "ifmecli: folder not found: %s\n", root);
		rc = 2;
		goto out;
	}

	if (want_info) {
		print_info(ctx);
		if (want_list || want_test || want_probe)
			printf("\n");
	}

	if (want_probe) {
		if (probe_files(ctx, argv + optind, argc - optind, frame_rate, json) > 0)
			rc = 1;
		if (want_list || want_test)
			printf("\n");
	}

	if (want_list || want_test) {
		if (!load_plugins(ctx)) {
			rc = 1;
			goto out;
		}
	}

	if (want_test) {
		int counts[3] = { 0, 0, 0 };
		ifme_selftest_callbacks cb = { on_test_start, on_test_result, counts };

		printf("Self-test (%s)\n", test_all ? "all usable plugins" : "plugins with TestRequired");
		int failed = ifme_plugins_selftest(ctx, test_all, &cb);

		if (failed < 0) {
			fprintf(stderr, "ifmecli: cannot run the self-test (temporary folder?)\n");
			rc = 1;
			goto out;
		}

		printf("\n%d passed, %d failed, %d skipped\n\n",
		       counts[IFME_TEST_PASS], counts[IFME_TEST_FAIL], counts[IFME_TEST_SKIPPED]);
		if (failed > 0)
			rc = 1;
	}

	if (want_list) {
		if (json) {
			char *text = ifme_plugins_json(ctx);
			if (text)
				printf("%s\n", text);
			ifme_free(text);
		} else {
			print_plugin_table(ctx);
		}
	}

out:
	ifme_destroy(ctx);
	return rc;
}
