/*
 * ifmecli.c - command-line front-end for libifmecore
 *
 * License: GPL-2.0
 */

#include <getopt.h>
#include <stdio.h>
#include <stdlib.h>

#include "ifme.h"

#ifdef _WIN32
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#endif

static void print_usage(FILE *out)
{
	fprintf(out,
		"Usage: ifmecli [OPTIONS]\n"
		"\n"
		"Options:\n"
		"  -i, --info       Show system information (paths, CPU features)\n"
		"  -V, --version    Show version and exit\n"
		"  -h, --help       Show this message and exit\n");
}

static const char *yes_no(bool v)
{
	return v ? "yes" : "no";
}

static int print_info(const ifme_ctx *ctx)
{
	const ifme_cpu_info *cpu = ifme_cpu(ctx);

	printf("IFME core   %s\n", ifme_version());
	printf("App folder  %s\n", ifme_app_dir(ctx));
	printf("Config      %s\n", ifme_config_dir(ctx));
	printf("CPU         %s (%s)\n", cpu->brand, cpu->arch);

	if (cpu->is_x86) {
		printf("SIMD        SSE4.2:%s AVX:%s AVX2:%s FMA3:%s AVX-512F:%s\n",
		       yes_no(cpu->sse42), yes_no(cpu->avx), yes_no(cpu->avx2),
		       yes_no(cpu->fma3), yes_no(cpu->avx512f));
	}

	if (cpu->hypervisor)
		printf("Hypervisor  %s\n", cpu->hypervisor_vendor[0] ? cpu->hypervisor_vendor : "unknown");

	return 0;
}

int main(int argc, char *argv[])
{
#ifdef _WIN32
	/* The manifest sets the process code page to UTF-8; make the console match. */
	SetConsoleOutputCP(CP_UTF8);
#endif

	static const struct option long_opts[] = {
		{ "info",    no_argument, NULL, 'i' },
		{ "version", no_argument, NULL, 'V' },
		{ "help",    no_argument, NULL, 'h' },
		{ NULL, 0, NULL, 0 }
	};

	bool want_info = false;
	int opt;

	while ((opt = getopt_long(argc, argv, "iVh", long_opts, NULL)) != -1) {
		switch (opt) {
		case 'i':
			want_info = true;
			break;
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

	if (!want_info) {
		print_usage(stdout);
		return 0;
	}

	ifme_ctx *ctx = ifme_create();
	if (!ctx) {
		fprintf(stderr, "ifmecli: out of memory\n");
		return 1;
	}

	int rc = print_info(ctx);
	ifme_destroy(ctx);
	return rc;
}
