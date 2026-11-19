/*
 * cpu_other.c - non-x86 targets (Apple Silicon, ARM Linux, ...)
 *
 * There is no AVX here; encoders use NEON/SVE instead, so the x86 flags
 * stay false and callers must not refuse to start because of them.
 * Compiled everywhere, active only when the target is not x86.
 */

#if !defined(__x86_64__) && !defined(__i386__)

#include <string.h>

#include "../internal.h"

#if defined(__APPLE__)
#include <sys/sysctl.h>
#endif

void ifme_platform_cpu(ifme_cpu_info *info)
{
	memset(info, 0, sizeof(*info));
	info->is_x86 = false;

#if defined(__aarch64__) || defined(_M_ARM64)
	strcpy(info->arch, "arm64");
#elif defined(__arm__)
	strcpy(info->arch, "arm");
#elif defined(__riscv)
	strcpy(info->arch, "riscv");
#else
	strcpy(info->arch, "unknown");
#endif

	strcpy(info->brand, "unknown");

#if defined(__APPLE__)
	size_t len = sizeof(info->brand);
	if (sysctlbyname("machdep.cpu.brand_string", info->brand, &len, NULL, 0) != 0)
		strcpy(info->brand, "unknown");
#endif
}

#endif /* not x86 */
