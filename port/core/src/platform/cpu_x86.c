/*
 * cpu_x86.c - CPU model and SIMD support on x86 / x86-64
 *
 * Ported from Native/libcpufeature.c. Always compiled; the compiler's own
 * target macros decide whether it is active, so cross-compiles and macOS
 * universal builds pick the right file per slice. Requires GCC or Clang.
 */

#if defined(__x86_64__) || defined(__i386__)

#include <cpuid.h>
#include <stdint.h>
#include <string.h>

#include "../internal.h"

static uint64_t read_xcr0(void)
{
	uint32_t eax, edx;
	__asm__ __volatile__("xgetbv" : "=a"(eax), "=d"(edx) : "c"(0));
	return ((uint64_t)edx << 32) | eax;
}

static void copy_trimmed(char *dst, size_t cap, const char *src)
{
	while (*src == ' ')
		src++;

	size_t n = strlen(src);
	while (n > 0 && src[n - 1] == ' ')
		n--;
	if (n >= cap)
		n = cap - 1;

	memcpy(dst, src, n);
	dst[n] = '\0';
}

void ifme_platform_cpu(ifme_cpu_info *info)
{
	unsigned int eax, ebx, ecx, edx;

	memset(info, 0, sizeof(*info));
	info->is_x86 = true;
#if defined(__x86_64__)
	strcpy(info->arch, "x86_64");
#else
	strcpy(info->arch, "x86");
#endif

	/* Brand string: CPUID 0x80000002..0x80000004 */
	if (__get_cpuid_max(0x80000000, NULL) >= 0x80000004) {
		unsigned int data[12];
		char raw[49];

		__cpuid(0x80000002, data[0], data[1], data[2], data[3]);
		__cpuid(0x80000003, data[4], data[5], data[6], data[7]);
		__cpuid(0x80000004, data[8], data[9], data[10], data[11]);
		memcpy(raw, data, 48);
		raw[48] = '\0';
		copy_trimmed(info->brand, sizeof(info->brand), raw);
	} else {
		strcpy(info->brand, "unknown");
	}

	unsigned int max_leaf = __get_cpuid_max(0, NULL);
	if (max_leaf < 1)
		return;

	__cpuid(1, eax, ebx, ecx, edx);

	info->sse42 = (ecx & (1u << 20)) != 0;
	info->fma3 = (ecx & (1u << 12)) != 0;
	info->hypervisor = (ecx & (1u << 31)) != 0;

	/* AVX needs the CPU bit *and* the OS saving YMM state (OSXSAVE + XCR0). */
	bool osxsave = (ecx & (1u << 27)) != 0;
	uint64_t xcr0 = osxsave ? read_xcr0() : 0;
	bool os_ymm = (xcr0 & 0x6) == 0x6;           /* XMM | YMM            */
	bool os_zmm = (xcr0 & 0xE6) == 0xE6;         /* + opmask, ZMM hi/lo  */

	info->avx = (ecx & (1u << 28)) != 0 && os_ymm;
	info->fma3 = info->fma3 && os_ymm;

	if (max_leaf >= 7) {
		__cpuid_count(7, 0, eax, ebx, ecx, edx);
		info->avx2 = info->avx && (ebx & (1u << 5)) != 0;
		info->avx512f = os_zmm && (ebx & (1u << 16)) != 0;
	}

	if (info->hypervisor) {
		char vendor[13];

		__cpuid(0x40000000, eax, ebx, ecx, edx);
		memcpy(&vendor[0], &ebx, 4);
		memcpy(&vendor[4], &ecx, 4);
		memcpy(&vendor[8], &edx, 4);
		vendor[12] = '\0';
		copy_trimmed(info->hypervisor_vendor, sizeof(info->hypervisor_vendor), vendor);
	}
}

#endif /* x86 */
