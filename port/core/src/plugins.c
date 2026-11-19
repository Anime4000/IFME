/*
 * plugins.c - load plugin manifests and self-test them
 *
 * Port of IFME/Plugins/PluginsLoad.cs. Differences on purpose:
 *  - Commands are argument lists run without a shell (see argv.c, proc_*.c).
 *  - A missing encoder program is reported at load time (MISSING_BINARY)
 *    instead of failing later during an encode.
 *  - A self-test passes only when the last program exits with 0 *and* writes
 *    a non-empty output file (C# accepted any exit code except -1, 1, 127).
 *  - The audio test command follows a real encode (see build_audio_test).
 *  - An optional "Arch": ["x86_64", "arm64", ...] array overrides "X64", so
 *    manifests can describe Apple Silicon and ARM Linux builds.
 *  - PATH is only searched for bare program names ("x265"), never for
 *    relative paths such as "../ffmpeg64/ffmpeg".
 */

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "internal.h"
#include "cJSON.h"

#define VIDEO_BUILTIN_GUID "00000000-0000-0000-0000-000000000000"
#define AUDIO_BUILTIN_GUID "aaaaaaaa-0000-0000-0000-000000000000"

#define SAMPLE_AUDIO "wonderland_online_south_island.ogg"
#define SAMPLE_VIDEO "wonderland_online_dungeon.mp4"

#define TEST_TIMEOUT_MS (5 * 60 * 1000)
#define TEST_WIDTH  320
#define TEST_HEIGHT 240
#define TEST_FPS    15
#define TEST_BITS   8
#define TEST_FRAMES 1000

typedef struct ifme_plugin {
	ifme_plugin_info info;
	cJSON *json;            /* manifest, with encoder paths resolved */
	char *guid;
	char *name;
	char *version;
	char *developer;
	char *manifest;
	char *binary;
	char *message;
} ifme_plugin;

/* ---- JSON helpers (case-insensitive keys, like Newtonsoft) ---------------- */

static const cJSON *jget(const cJSON *o, const char *key)
{
	return o ? cJSON_GetObjectItem(o, key) : NULL;
}

static const char *jstr(const cJSON *o, const char *key)
{
	const cJSON *i = jget(o, key);
	return cJSON_IsString(i) ? i->valuestring : NULL;
}

/* String, number or bool rendered as text; NULL when absent or null. */
static char *jscalar(const cJSON *o, const char *key)
{
	const cJSON *i = jget(o, key);

	if (cJSON_IsString(i))
		return ifme_strdup(i->valuestring);
	if (cJSON_IsBool(i))
		return ifme_strdup(cJSON_IsTrue(i) ? "true" : "false");
	if (cJSON_IsNumber(i)) {
		double d = i->valuedouble;
		if (d == (double)(long long)d && d > -1e15 && d < 1e15)
			return ifme_strdupf("%lld", (long long)d);
		return ifme_strdupf("%.15g", d);
	}
	return NULL;
}

static bool jbool(const cJSON *o, const char *key, bool def)
{
	const cJSON *i = jget(o, key);

	if (cJSON_IsBool(i))
		return cJSON_IsTrue(i);
	if (cJSON_IsNumber(i))
		return i->valuedouble != 0;
	if (cJSON_IsString(i))
		return ifme_strcasecmp(i->valuestring, "true") == 0;
	return def;
}

static int jint(const cJSON *o, const char *key, int def)
{
	const cJSON *i = jget(o, key);

	if (cJSON_IsNumber(i))
		return (int)i->valuedouble;
	if (cJSON_IsString(i) && *i->valuestring)
		return atoi(i->valuestring);
	return def;
}

static const cJSON *jitem(const cJSON *o, const char *key, int index)
{
	const cJSON *a = jget(o, key);
	return cJSON_IsArray(a) ? cJSON_GetArrayItem(a, index) : NULL;
}

/* ---- plugin records --------------------------------------------------------- */

static void sync_info(ifme_plugin *p)
{
	p->info.guid = p->guid ? p->guid : "";
	p->info.name = p->name ? p->name : "";
	p->info.version = p->version ? p->version : "";
	p->info.developer = p->developer ? p->developer : "";
	p->info.manifest = p->manifest ? p->manifest : "";
	p->info.binary = p->binary ? p->binary : "";
	p->info.message = p->message ? p->message : "";
}

static void set_status(ifme_plugin *p, ifme_plugin_status status, char *message)
{
	p->info.status = status;
	free(p->message);
	p->message = message;
	sync_info(p);
}

static void plugin_free(ifme_plugin *p)
{
	if (!p)
		return;

	cJSON_Delete(p->json);
	free(p->guid);
	free(p->name);
	free(p->version);
	free(p->developer);
	free(p->manifest);
	free(p->binary);
	free(p->message);
	free(p);
}

void ifme_plugins_clear(ifme_ctx *ctx)
{
	for (size_t i = 0; i < ctx->plugin_count; i++)
		plugin_free(ctx->plugins[i]);

	free(ctx->plugins);
	ctx->plugins = NULL;
	ctx->plugin_count = 0;
	ctx->plugins_loaded = false;
}

static bool plugin_append(ifme_ctx *ctx, ifme_plugin *p)
{
	ifme_plugin **grown = realloc(ctx->plugins, (ctx->plugin_count + 1) * sizeof(*grown));
	if (!grown)
		return false;

	ctx->plugins = grown;
	ctx->plugins[ctx->plugin_count++] = p;
	return true;
}

/* ---- architecture --------------------------------------------------------- */

static const char *normalise_arch(const char *a)
{
	if (!ifme_strcasecmp(a, "x86_64") || !ifme_strcasecmp(a, "amd64") || !ifme_strcasecmp(a, "x64"))
		return "x86_64";
	if (!ifme_strcasecmp(a, "x86") || !ifme_strcasecmp(a, "i386") ||
	    !ifme_strcasecmp(a, "i686") || !ifme_strcasecmp(a, "ia32"))
		return "x86";
	if (!ifme_strcasecmp(a, "arm64") || !ifme_strcasecmp(a, "aarch64"))
		return "arm64";
	return a;
}

/* Returns NULL when compatible, otherwise a malloc'd explanation. */
static char *check_arch(const cJSON *json, const char *host)
{
	const cJSON *list = jget(json, "Arch");

	if (cJSON_IsArray(list)) {
		const cJSON *e;
		cJSON_ArrayForEach(e, list) {
			if (cJSON_IsString(e) && strcmp(normalise_arch(e->valuestring), host) == 0)
				return NULL;
		}
		return ifme_strdupf("built for other architectures, this system is %s", host);
	}

	const char *want = jbool(json, "X64", false) ? "x86_64" : "x86";
	if (strcmp(want, host) == 0)
		return NULL;
	return ifme_strdupf("built for %s, this system is %s", want, host);
}

/* ---- encoder program lookup ------------------------------------------------ */

static bool has_separator(const char *s)
{
	return strchr(s, '/') || strchr(s, '\\');
}

/* Mirrors C#: absolute path, else bare name in PATH, else relative to the
 * manifest's folder. *tried receives the path that was checked (for errors). */
static char *resolve_binary(const char *manifest_dir, const char *bin, bool *from_path, char **tried)
{
	*from_path = false;
	*tried = NULL;

	if (!bin || !*bin)
		return NULL;

	if (ifme_path_is_absolute(bin)) {
		*tried = ifme_full_path(bin);
		return *tried ? ifme_resolve_program(*tried) : NULL;
	}

	if (!has_separator(bin)) {
		char *found = ifme_find_in_path(bin);
		if (found) {
			*from_path = true;
			*tried = ifme_strdup(found);
			return found;
		}
	}

	char *joined = ifme_path_join(manifest_dir, bin);
	*tried = joined ? ifme_full_path(joined) : NULL;
	free(joined);
	return *tried ? ifme_resolve_program(*tried) : NULL;
}

static char *dirname_of(const char *path)
{
	char *d = ifme_strdup(path);
	if (!d)
		return NULL;

	char *a = strrchr(d, '/'), *b = strrchr(d, '\\');
	char *cut = a > b ? a : b;
	if (cut)
		*cut = '\0';
	return d;
}

/* Resolve one JSON string member in place. Returns the resolved path (owned by
 * the caller) or NULL; *tried is set for error messages. */
static char *resolve_member(cJSON *holder, const char *key, const char *dir, bool *from_path, char **tried)
{
	cJSON *item = cJSON_GetObjectItem(holder, key);
	*from_path = false;
	*tried = NULL;

	if (!cJSON_IsString(item))
		return NULL;

	char *resolved = resolve_binary(dir, item->valuestring, from_path, tried);
	if (resolved)
		cJSON_SetValuestring(item, resolved);
	return resolved;
}

/* ---- loading ---------------------------------------------------------------- */

static ifme_plugin *find_usable(const ifme_ctx *ctx, const char *guid)
{
	for (size_t i = 0; i < ctx->plugin_count; i++) {
		ifme_plugin *q = ctx->plugins[i];
		if ((q->info.status == IFME_PLUGIN_OK || q->info.status == IFME_PLUGIN_BUILTIN) &&
		    ifme_strcasecmp(q->guid, guid) == 0)
			return q;
	}
	return NULL;
}

static void lowercase(char *s)
{
	for (; s && *s; s++)
		if (*s >= 'A' && *s <= 'Z')
			*s += 'a' - 'A';
}

static ifme_plugin *load_one(ifme_ctx *ctx, const char *path, ifme_plugin_kind kind)
{
	ifme_plugin *p = calloc(1, sizeof(*p));
	if (!p)
		return NULL;

	p->info.kind = kind;
	p->manifest = ifme_strdup(path);
	sync_info(p);

	char *text = ifme_read_file(path, NULL);
	char *strict = text ? ifme_json_relax(text) : NULL;
	free(text);

	if (!strict) {
		set_status(p, IFME_PLUGIN_INVALID, ifme_strdup("cannot read manifest"));
		return p;
	}

	p->json = cJSON_Parse(strict);
	if (!p->json) {
		const char *at = cJSON_GetErrorPtr();
		set_status(p, IFME_PLUGIN_INVALID,
		           ifme_strdupf("JSON syntax error near: %.40s", at ? at : "?"));
		free(strict);
		return p;
	}
	free(strict);

	const char *section_name = kind == IFME_PLUGIN_AUDIO ? "Audio" : "Video";
	cJSON *section = cJSON_GetObjectItem(p->json, section_name);

	p->guid = ifme_strdup(jstr(p->json, "GUID"));
	p->name = ifme_strdup(jstr(p->json, "Name"));
	p->version = jscalar(p->json, "Version");
	p->developer = ifme_strdup(jstr(jget(p->json, "Author"), "Developer"));
	p->info.test_required = jbool(p->json, "TestRequired", false);
	lowercase(p->guid);
	sync_info(p);

	if (!p->guid || !p->name || !cJSON_IsObject(section)) {
		set_status(p, IFME_PLUGIN_INVALID,
		           ifme_strdupf("manifest needs \"GUID\", \"Name\" and an \"%s\" section", section_name));
		return p;
	}

	if (!strcmp(p->guid, kind == IFME_PLUGIN_AUDIO ? AUDIO_BUILTIN_GUID : VIDEO_BUILTIN_GUID)) {
		set_status(p, IFME_PLUGIN_BUILTIN, NULL);
		return p;
	}

	char *why = check_arch(p->json, ctx->cpu.arch);
	if (why) {
		set_status(p, IFME_PLUGIN_WRONG_ARCH, why);
		return p;
	}

	ifme_plugin *first = find_usable(ctx, p->guid);
	if (first) {
		set_status(p, IFME_PLUGIN_DUPLICATE, ifme_strdupf("same GUID as %s", first->manifest));
		return p;
	}

	/* Resolve every encoder path in the manifest, like the C# loader does. */
	char *dir = dirname_of(path);
	char *primary = NULL, *primary_tried = NULL;
	bool primary_from_path = false, primary_seen = false;

	if (kind == IFME_PLUGIN_AUDIO) {
		primary = resolve_member(section, "Encoder", dir, &primary_from_path, &primary_tried);
		primary_seen = true;
	} else {
		cJSON *encoders = cJSON_GetObjectItem(section, "Encoder");
		cJSON *e;
		cJSON_ArrayForEach(e, encoders) {
			bool fp;
			char *tried;
			char *r = resolve_member(e, "Binary", dir, &fp, &tried);

			if (!primary_seen) {
				primary = r;
				primary_tried = tried;
				primary_from_path = fp;
				primary_seen = true;
			} else {
				free(r);
				free(tried);
			}
		}
	}
	free(dir);

	p->info.from_path = primary_from_path;

	if (!primary) {
		const char *bin = kind == IFME_PLUGIN_AUDIO
			? jstr(section, "Encoder")
			: jstr(jitem(section, "Encoder", 0), "Binary");
		set_status(p, IFME_PLUGIN_MISSING_BINARY,
		           primary_tried ? ifme_strdupf("encoder not found: %s", primary_tried)
		                         : ifme_strdupf("no encoder program in manifest (%s)", bin ? bin : "none"));
		free(primary_tried);
		return p;
	}

	free(primary_tried);
	p->binary = primary;
	set_status(p, IFME_PLUGIN_OK, NULL);
	return p;
}

typedef struct { char **v; size_t n, cap; } path_list;

static void collect_manifest(void *user, const char *path)
{
	path_list *list = user;
	const char *a = strrchr(path, '/'), *b = strrchr(path, '\\');
	const char *name = (a > b ? a : b);
	name = name ? name + 1 : path;

	/* _plugin.a*.json or _plugin.v*.json */
	if (!ifme_starts_with_ci(name, "_plugin.") || !ifme_ends_with_ci(name, ".json"))
		return;
	char k = name[8] | 0x20;
	if (k != 'a' && k != 'v')
		return;

	if (list->n == list->cap) {
		size_t cap = list->cap ? list->cap * 2 : 64;
		char **grown = realloc(list->v, cap * sizeof(char *));
		if (!grown)
			return;
		list->v = grown;
		list->cap = cap;
	}
	list->v[list->n++] = ifme_strdup(path);
}

static int compare_paths(const void *x, const void *y)
{
	const char *a = *(const char *const *)x, *b = *(const char *const *)y;
	int c = ifme_strcasecmp(a, b);
	return c ? c : strcmp(a, b);
}

static ifme_plugin_kind kind_of(const char *path)
{
	const char *a = strrchr(path, '/'), *b = strrchr(path, '\\');
	const char *name = (a > b ? a : b) + 1;
	return (name[8] | 0x20) == 'a' ? IFME_PLUGIN_AUDIO : IFME_PLUGIN_VIDEO;
}

int ifme_plugins_load(ifme_ctx *ctx)
{
	if (!ctx)
		return -1;

	ifme_plugins_clear(ctx);

	char *dir = ifme_path_join(ctx->root_dir, "Plugins");
	if (!dir || !ifme_dir_exists(dir)) {
		free(dir);
		return -1;
	}

	path_list list = { 0 };
	ifme_walk_files(dir, collect_manifest, &list);
	free(dir);

	qsort(list.v, list.n, sizeof(char *), compare_paths);

	/* Audio first, then video - same order as the C# loader. */
	const ifme_plugin_kind order[] = { IFME_PLUGIN_AUDIO, IFME_PLUGIN_VIDEO };
	for (size_t pass = 0; pass < 2; pass++) {
		for (size_t i = 0; i < list.n; i++) {
			if (!list.v[i] || kind_of(list.v[i]) != order[pass])
				continue;

			ifme_plugin *p = load_one(ctx, list.v[i], order[pass]);
			if (p && !plugin_append(ctx, p))
				plugin_free(p);
		}
	}

	for (size_t i = 0; i < list.n; i++)
		free(list.v[i]);
	free(list.v);

	ctx->plugins_loaded = true;

	int usable = 0;
	for (size_t i = 0; i < ctx->plugin_count; i++) {
		ifme_plugin_status s = ctx->plugins[i]->info.status;
		usable += s == IFME_PLUGIN_OK || s == IFME_PLUGIN_BUILTIN;
	}
	return usable;
}

size_t ifme_plugin_count(const ifme_ctx *ctx)
{
	return ctx ? ctx->plugin_count : 0;
}

const ifme_plugin_info *ifme_plugin_at(const ifme_ctx *ctx, size_t index)
{
	return (ctx && index < ctx->plugin_count) ? &ctx->plugins[index]->info : NULL;
}

const char *ifme_plugin_status_name(ifme_plugin_status status)
{
	switch (status) {
	case IFME_PLUGIN_OK:             return "ok";
	case IFME_PLUGIN_BUILTIN:        return "builtin";
	case IFME_PLUGIN_WRONG_ARCH:     return "wrong-arch";
	case IFME_PLUGIN_DUPLICATE:      return "duplicate";
	case IFME_PLUGIN_MISSING_BINARY: return "missing";
	case IFME_PLUGIN_INVALID:        return "invalid";
	case IFME_PLUGIN_TEST_FAILED:    return "test-failed";
	}
	return "unknown";
}

char *ifme_plugins_json(const ifme_ctx *ctx)
{
	cJSON *arr = cJSON_CreateArray();
	if (!arr || !ctx)
		return NULL;

	for (size_t i = 0; i < ctx->plugin_count; i++) {
		const ifme_plugin *p = ctx->plugins[i];
		cJSON *o = cJSON_CreateObject();
		if (!o)
			break;

		cJSON_AddStringToObject(o, "guid", p->info.guid);
		cJSON_AddStringToObject(o, "name", p->info.name);
		cJSON_AddStringToObject(o, "version", p->info.version);
		cJSON_AddStringToObject(o, "developer", p->info.developer);
		cJSON_AddStringToObject(o, "kind", p->info.kind == IFME_PLUGIN_AUDIO ? "audio" : "video");
		cJSON_AddStringToObject(o, "status", ifme_plugin_status_name(p->info.status));
		cJSON_AddStringToObject(o, "message", p->info.message);
		cJSON_AddStringToObject(o, "manifest", p->info.manifest);
		cJSON_AddStringToObject(o, "binary", p->info.binary);
		cJSON_AddBoolToObject(o, "from_path", p->info.from_path);
		cJSON_AddBoolToObject(o, "test_required", p->info.test_required);
		if (p->json)
			cJSON_AddItemToObject(o, "manifest_json", cJSON_Duplicate(p->json, true));

		cJSON_AddItemToArray(arr, o);
	}

	char *out = cJSON_Print(arr);
	cJSON_Delete(arr);
	return out;
}

/* ---- self-test ------------------------------------------------------------- */

char *ifme_find_ff_tool(const ifme_ctx *ctx, const char *name)
{
	/* Same order as MediaEncoding.FFmpeg: PATH first, then the bundled copy. */
	char *found = ifme_find_in_path(name);
	if (found)
		return found;

	const char *folder = strcmp(ctx->cpu.arch, "x86") == 0 ? "ffmpeg32" : "ffmpeg64";
	char *plugins = ifme_path_join(ctx->root_dir, "Plugins");
	char *dir = plugins ? ifme_path_join(plugins, folder) : NULL;
	char *bin = dir ? ifme_path_join(dir, name) : NULL;
	char *resolved = bin ? ifme_resolve_program(bin) : NULL;

	free(bin);
	free(dir);
	free(plugins);
	return resolved;
}

static char *sample_path(const ifme_ctx *ctx, const char *name)
{
	char *dir = ifme_path_join(ctx->root_dir, "Samples");
	char *path = dir ? ifme_path_join(dir, name) : NULL;
	free(dir);

	if (path && !ifme_file_exists(path)) {
		free(path);
		return NULL;
	}
	return path;
}

/* "{0}x{1}" style template (C# string.Format) with width and height. */
static char *format_resolution(const char *tmpl, int w, int h)
{
	size_t cap = strlen(tmpl) + 32;
	char *out = malloc(cap);
	if (!out)
		return NULL;

	size_t o = 0;
	for (const char *p = tmpl; *p && o + 12 < cap; p++) {
		if (p[0] == '{' && (p[1] == '0' || p[1] == '1') && p[2] == '}') {
			o += (size_t)snprintf(out + o, cap - o, "%d", p[1] == '0' ? w : h);
			p += 2;
		} else {
			out[o++] = *p;
		}
	}
	out[o] = '\0';
	return out;
}

static bool blank(const char *s)
{
	if (!s)
		return true;
	while (*s == ' ' || *s == '\t')
		s++;
	return *s == '\0';
}

/* Build the TestVideo() command. Returns the number of stages (1 or 2). */
static size_t build_video_test(const cJSON *section, const char *encoder, const char *ffmpeg,
                               const char *sample, const char *out, ifme_argv st[2])
{
	const cJSON *args = jget(section, "Args");
	ifme_argv *en;
	size_t n;

	if (jbool(args, "Pipe", false)) {
		ifme_argv *ff = &st[0];
		ifme_argv_push(ff, ffmpeg);
		ifme_argv_split(ff, "-hide_banner -v error -i");
		ifme_argv_push(ff, sample);
		ifme_argv_split(ff, "-strict -1 -f");
		ifme_argv_push(ff, blank(jstr(args, "Y4M")) ? "rawvideo" : "yuv4mpegpipe");
		ifme_argv_split(ff, "-pix_fmt yuv420p -vf");
		ifme_argv_pushf(ff, "scale=%d:%d:flags=lanczos,fps=%d", TEST_WIDTH, TEST_HEIGHT, TEST_FPS);
		ifme_argv_push(ff, "-");

		en = &st[1];
		ifme_argv_push(en, encoder);
		ifme_argv_split(en, jstr(args, "Input"));
		ifme_argv_split(en, jstr(args, "Y4M"));

		const char *res = jstr(args, "Resolution");
		if (!blank(res)) {
			char *r = format_resolution(res, TEST_WIDTH, TEST_HEIGHT);
			ifme_argv_split(en, r);
			free(r);
		}
		if (!blank(jstr(args, "FrameRate"))) {
			ifme_argv_split(en, jstr(args, "FrameRate"));
			ifme_argv_pushf(en, "%d", TEST_FPS);
		}
		if (!blank(jstr(args, "BitDepthIn"))) {
			ifme_argv_split(en, jstr(args, "BitDepthIn"));
			ifme_argv_pushf(en, "%d", TEST_BITS);
		}
		if (!blank(jstr(args, "BitDepthOut"))) {
			ifme_argv_split(en, jstr(args, "BitDepthOut"));
			ifme_argv_pushf(en, "%d", TEST_BITS);
		}

		ifme_argv_split(en, jstr(jitem(section, "Chroma", 0), "Command"));

		/* C# appended PresetDefault/TuneDefault even without a -preset flag,
		 * leaving a stray word on the command line; only pass the pair. */
		if (!blank(jstr(args, "Preset"))) {
			ifme_argv_split(en, jstr(args, "Preset"));
			ifme_argv_split(en, jstr(section, "PresetDefault"));
		}
		if (!blank(jstr(args, "Tune"))) {
			ifme_argv_split(en, jstr(args, "Tune"));
			ifme_argv_split(en, jstr(section, "TuneDefault"));
		}
		if (!ifme_is_disabled(jstr(args, "FrameCount"))) {
			ifme_argv_split(en, jstr(args, "FrameCount"));
			ifme_argv_pushf(en, "%d", TEST_FRAMES);
		}
		n = 2;
	} else {
		en = &st[0];
		ifme_argv_push(en, encoder);
		ifme_argv_split(en, jstr(args, "Input"));
		ifme_argv_push(en, sample);
		ifme_argv_split(en, jstr(args, "UnPipe"));
		n = 1;
	}

	ifme_argv_split(en, jstr(args, "Output"));
	ifme_argv_push(en, out);
	return n;
}

/* Build an audio test command the way a real encode does (MediaEncoding.cs),
 * using Mode[0] and its default quality. The old C# TestAudio() differed: it
 * dropped the quality value when the mode's Args was blank (exhale then got no
 * preset and wrote nothing) and ignored SingleChannelOnly (AMR-NB got stereo).
 * Returns the number of stages (1 or 2). */
static size_t build_audio_test(const cJSON *section, const char *encoder, const char *ffmpeg,
                               const char *sample, const char *out, ifme_argv st[2])
{
	const cJSON *args = jget(section, "Args");
	const cJSON *mode = jitem(section, "Mode", 0);

	/* Quality: "{Args} {QualityPrefix}{Default}{QualityPostfix}".
	 *   "Args": "-b:a"  -> flag and value        (most encoders)
	 *   "Args": ""      -> bare positional value (exhale preset "9")
	 *   "Args": null, or a disabled word like "none" -> no quality at all
	 *                     (WAV/FLAC: a stray "0" would become an output file) */
	ifme_argv quality = { 0 };
	const cJSON *mode_args = jget(mode, "Args");
	if (cJSON_IsString(mode_args) &&
	    (blank(mode_args->valuestring) || !ifme_is_disabled(mode_args->valuestring))) {
		const char *pre = jstr(mode, "QualityPrefix"), *post = jstr(mode, "QualityPostfix");
		char *def = jscalar(mode, "Default");

		ifme_argv_split(&quality, mode_args->valuestring);
		if (def && *def)
			ifme_argv_pushf(&quality, "%s%s%s", pre ? pre : "", def, post ? post : "");
		free(def);
	}

	/* Missing keys fall back to the C# defaults "-ar" / "-ac". */
	const cJSON *hz_key = jget(section, "SampleRateArgs"), *ch_key = jget(section, "ChannelArgs");
	const char *hz_args = hz_key ? jstr(section, "SampleRateArgs") : "-ar";
	const char *ch_args = ch_key ? jstr(section, "ChannelArgs") : "-ac";
	int hz = jint(section, "SampleRateDefault", 0);
	int ch = jint(section, "ChannelDefault", 0);

	if (mode && !jbool(mode, "MultiChannelSupport", true))
		ch = 2;   /* e.g. exhale eSBR: down-mix to stereo */

	bool mono_only = mode && jbool(mode, "SingleChannelOnly", false);

	ifme_argv extra = { 0 };   /* sample rate + channels */
	if (hz != 0 && hz_args) {
		ifme_argv_split(&extra, hz_args);
		ifme_argv_pushf(&extra, "%d", hz);
	}
	if (mono_only) {
		ifme_argv_split(&extra, "-ac 1");   /* literal, as in MediaEncoding.cs */
	} else if (ch != 0 && ch_args) {
		ifme_argv_split(&extra, ch_args);
		ifme_argv_pushf(&extra, "%d", ch);
	}

	ifme_argv *en;
	size_t n;

	if (jbool(args, "Pipe", false)) {
		ifme_argv *ff = &st[0];
		ifme_argv_push(ff, ffmpeg);
		ifme_argv_split(ff, "-hide_banner -v error -i");
		ifme_argv_push(ff, sample);
		ifme_argv_split(ff, "-acodec pcm_s16le");
		for (size_t i = 0; i < extra.n; i++)
			ifme_argv_push(ff, extra.v[i]);
		ifme_argv_split(ff, "-f wav -");

		en = &st[1];
		ifme_argv_push(en, encoder);
		ifme_argv_split(en, jstr(args, "Input"));
		ifme_argv_split(en, jstr(args, "Command"));
		for (size_t i = 0; i < quality.n; i++)
			ifme_argv_push(en, quality.v[i]);
		n = 2;
	} else {
		en = &st[0];
		ifme_argv_push(en, encoder);
		ifme_argv_split(en, jstr(args, "Input"));
		ifme_argv_push(en, sample);
		ifme_argv_split(en, jstr(args, "Command"));
		ifme_argv_split(en, jstr(args, "Codec"));
		for (size_t i = 0; i < quality.n; i++)
			ifme_argv_push(en, quality.v[i]);
		for (size_t i = 0; i < extra.n; i++)
			ifme_argv_push(en, extra.v[i]);
		n = 1;
	}

	ifme_argv_split(en, jstr(args, "Output"));
	ifme_argv_push(en, out);

	ifme_argv_free(&quality);
	ifme_argv_free(&extra);
	return n;
}

/* Last few non-empty lines of the encoder log (progress uses \r too). */
static char *log_tail(const char *log_path, size_t max_lines)
{
	size_t len = 0;
	char *text = ifme_read_file(log_path, &len);
	if (!text || len == 0) {
		free(text);
		return NULL;
	}

	size_t lines = 0;
	char *start = text + len;
	char *end = text + len;

	while (start > text && lines < max_lines) {
		while (end > text && (end[-1] == '\n' || end[-1] == '\r' || end[-1] == ' '))
			end--;
		start = end;
		while (start > text && start[-1] != '\n' && start[-1] != '\r')
			start--;
		if (start == end)
			break;
		lines++;
		if (lines < max_lines)
			end = start;
	}

	/* Keep the window bounded so one huge line can't flood the report. */
	size_t keep = (size_t)(text + len - start);
	if (keep > 600) {
		start = text + len - 600;
		keep = 600;
	}

	char *out = malloc(keep + 1);
	if (out) {
		size_t o = 0;
		for (size_t i = 0; i < keep; i++)
			out[o++] = start[i] == '\r' ? '\n' : start[i];
		while (o > 0 && (out[o - 1] == '\n' || out[o - 1] == ' '))
			o--;
		out[o] = '\0';
	}
	free(text);
	return out;
}

static char *with_tail(char *msg, const char *log_path)
{
	char *tail = log_tail(log_path, 4);
	if (!tail || !*tail) {
		free(tail);
		return msg;
	}

	char *joined = ifme_strdupf("%s\n%s", msg, tail);
	free(tail);
	if (!joined)
		return msg;
	free(msg);
	return joined;
}

int ifme_plugins_selftest(ifme_ctx *ctx, bool test_all, const ifme_selftest_callbacks *cb)
{
	if (!ctx || !ctx->plugins_loaded)
		return -1;

	char *ffmpeg = ifme_find_ff_tool(ctx, "ffmpeg");
	char *sample_audio = sample_path(ctx, SAMPLE_AUDIO);
	char *sample_video = sample_path(ctx, SAMPLE_VIDEO);

	char *tmp = ifme_temp_dir();
	char *name = ifme_strdupf("ifme-selftest-%lu", ifme_pid());
	char *work = (tmp && name) ? ifme_path_join(tmp, name) : NULL;
	free(tmp);
	free(name);

	if (!work || !ifme_mkdir(work)) {
		free(work);
		free(ffmpeg);
		free(sample_audio);
		free(sample_video);
		return -1;
	}

	char *log_path = ifme_path_join(work, "test.log");
	int failures = 0;

	for (size_t i = 0; i < ctx->plugin_count; i++) {
		ifme_plugin *p = ctx->plugins[i];
		if (p->info.status != IFME_PLUGIN_OK || (!test_all && !p->info.test_required))
			continue;

		if (cb && cb->on_start)
			cb->on_start(cb->user, &p->info);

		uint64_t t0 = ifme_now_ms();
		bool audio = p->info.kind == IFME_PLUGIN_AUDIO;
		const cJSON *section = jget(p->json, audio ? "Audio" : "Video");
		const char *sample = audio ? sample_audio : sample_video;
		const bool pipe = jbool(jget(section, "Args"), "Pipe", false);

		ifme_test_outcome outcome;
		char *detail = NULL;

		if (!sample) {
			outcome = IFME_TEST_SKIPPED;
			detail = ifme_strdupf("sample file missing: Samples/%s", audio ? SAMPLE_AUDIO : SAMPLE_VIDEO);
		} else if (pipe && !ffmpeg) {
			outcome = IFME_TEST_SKIPPED;
			detail = ifme_strdup("ffmpeg not found (PATH or Plugins/ffmpeg64)");
		} else {
			const char *ext = jstr(section, "Extension");
			char *out_name = ifme_strdupf("test.%s", blank(ext) ? "out" : ext);
			char *out_path = out_name ? ifme_path_join(work, out_name) : NULL;
			free(out_name);

			ifme_argv stages[2] = { { 0 }, { 0 } };
			size_t n = audio
				? build_audio_test(section, p->binary, ffmpeg, sample, out_path, stages)
				: build_video_test(section, p->binary, ffmpeg, sample, out_path, stages);

			ifme_clear_dir(work);

			ifme_run_opts ro = { 0 };
			ro.cwd = work;
			ro.timeout_ms = TEST_TIMEOUT_MS;
			ro.stderr_path = log_path;

			ifme_run_result run;
			int rc = ifme_run(stages, n, &ro, &run);
			int code = run.exit_code;
			long long size = out_path ? ifme_file_size(out_path) : -1;

			if (rc != IFME_RUN_OK) {
				outcome = IFME_TEST_FAIL;
				detail = with_tail(ifme_strdup(run.message), log_path);
			} else if (code != 0) {
				outcome = IFME_TEST_FAIL;
				detail = with_tail(ifme_strdupf("exit code %d", code), log_path);
			} else if (size <= 0) {
				outcome = IFME_TEST_FAIL;
				detail = with_tail(ifme_strdup("finished but wrote no output"), log_path);
			} else {
				outcome = IFME_TEST_PASS;
				detail = ifme_strdupf("%lld bytes", size);
			}

			if (outcome == IFME_TEST_FAIL) {
				/* Record the exact command to make failures reproducible. */
				for (size_t s = 0; s < n; s++) {
					char *cmd = ifme_argv_display(&stages[s]);
					char *more = cmd ? ifme_strdupf("%s\n%s %s", detail, s ? "|" : "$", cmd) : NULL;
					free(cmd);
					if (more) {
						free(detail);
						detail = more;
					}
				}
			}

			ifme_run_result_free(&run);
			ifme_argv_free(&stages[0]);
			ifme_argv_free(&stages[1]);
			free(out_path);
		}

		if (outcome == IFME_TEST_FAIL) {
			failures++;
			set_status(p, IFME_PLUGIN_TEST_FAILED, ifme_strdup(detail));
		}

		if (cb && cb->on_result)
			cb->on_result(cb->user, &p->info, outcome, detail ? detail : "",
			              (unsigned)(ifme_now_ms() - t0));
		free(detail);
	}

	ifme_clear_dir(work);
	ifme_rmdir(work);
	free(log_path);
	free(work);
	free(ffmpeg);
	free(sample_audio);
	free(sample_video);
	return failures;
}
