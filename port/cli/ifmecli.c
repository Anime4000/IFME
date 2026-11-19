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

int main(int argc, char *argv[])
{
#ifdef _WIN32
	/* The manifest sets the process code page to UTF-8; make the console match. */
	SetConsoleOutputCP(CP_UTF8);
#endif

	enum { OPT_JSON = 1000, OPT_FRAMERATE, OPT_FRAMES };
	static const struct option long_opts[] = {
		{ "info",         no_argument,       NULL, 'i' },
		{ "exec",         no_argument,       NULL, 'x' },
		{ "frames",       required_argument, NULL, OPT_FRAMES },
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

	while ((opt = getopt_long(argc, argv, "ixpltar:vVh", long_opts, NULL)) != -1) {
		switch (opt) {
		case 'i': want_info = true; break;
		case 'x': want_exec = true; break;
		case OPT_FRAMES: frames = atoll(optarg); break;
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
