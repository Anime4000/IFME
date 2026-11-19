/*
 * jsonutil.c - lenient JSON accessors shared by the core
 *
 * Manifests and jobs were written for Newtonsoft.Json, which matches keys
 * case-insensitively and converts "42" to 42; these helpers do the same.
 */

#include <stdlib.h>
#include <string.h>

#include "internal.h"
#include "cJSON.h"

const cJSON *ifme_jget(const cJSON *o, const char *key)
{
	return (o && key) ? cJSON_GetObjectItem(o, key) : NULL;
}

const char *ifme_jstr(const cJSON *o, const char *key)
{
	const cJSON *i = ifme_jget(o, key);
	return cJSON_IsString(i) ? i->valuestring : NULL;
}

bool ifme_jnum(const cJSON *o, const char *key, double *out)
{
	const cJSON *i = ifme_jget(o, key);

	if (cJSON_IsNumber(i)) {
		*out = i->valuedouble;
		return true;
	}
	if (cJSON_IsString(i) && *i->valuestring) {
		char *end = NULL;
		double d = strtod(i->valuestring, &end);
		if (end && *end == '\0') {
			*out = d;
			return true;
		}
	}
	return false;
}

int ifme_jint(const cJSON *o, const char *key, int def)
{
	double d;
	return ifme_jnum(o, key, &d) ? (int)d : def;
}

double ifme_jdouble(const cJSON *o, const char *key, double def)
{
	double d;
	return ifme_jnum(o, key, &d) ? d : def;
}

bool ifme_jbool(const cJSON *o, const char *key, bool def)
{
	const cJSON *i = ifme_jget(o, key);

	if (cJSON_IsBool(i))
		return cJSON_IsTrue(i);
	if (cJSON_IsNumber(i))
		return i->valuedouble != 0;
	if (cJSON_IsString(i))
		return ifme_strcasecmp(i->valuestring, "true") == 0;
	return def;
}

char *ifme_jscalar(const cJSON *o, const char *key)
{
	const cJSON *i = ifme_jget(o, key);

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

const cJSON *ifme_jitem(const cJSON *o, const char *key, int index)
{
	const cJSON *a = ifme_jget(o, key);
	return cJSON_IsArray(a) ? cJSON_GetArrayItem(a, index) : NULL;
}
