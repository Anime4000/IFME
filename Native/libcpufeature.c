/*
 * libcpufeature.c - CPU feature detection for IFME
 *
 * This file defines native functions to check whether the host CPU
 * supports the AVX2 or FMA3 instruction sets, and to retrieve the
 * CPU brand string (e.g., "AMD Ryzen 9 3950X").
 *
 * Platform: Linux (.so), Windows (.dll)
 * Author: Anime4000
 * License: GPL-2.0
 *
 * Compile:
 *   Linux:   gcc -fPIC -shared -o libcpufeature.so libcpufeature.c
 *   Windows: gcc -shared -o libcpufeature.dll libcpufeature.c
 */

#include <cpuid.h>
#include <stdbool.h>
#include <stddef.h>
#include <string.h>
#include <stdint.h>

// Cross-platform export macro
#ifdef _WIN32
#define DLL_EXPORT __declspec(dllexport)
#else
#define DLL_EXPORT
#endif

// CPU Brand String: CPUID.80000002H..80000004H
DLL_EXPORT const char* get_cpu_brand_string() {
	static char brand[49] = {0};
	unsigned int data[12];

	__cpuid(0x80000002, data[0], data[1], data[2], data[3]);
	__cpuid(0x80000003, data[4], data[5], data[6], data[7]);
	__cpuid(0x80000004, data[8], data[9], data[10], data[11]);

	memcpy(brand, data, sizeof(data));
	brand[48] = '\0'; // Ensure null-terminated

	return brand;
}

// Bit 0 = SSE, 1 = SSE2, ..., 5 = SSE4.2
// Bit 6 = MMX
DLL_EXPORT unsigned int get_simd_flags() {
	unsigned int eax, ebx, ecx, edx;
	unsigned int flags = 0;
	__cpuid(1, eax, ebx, ecx, edx);

	if (edx & (1 << 25)) flags |= (1 << 0); // SSE
	if (edx & (1 << 26)) flags |= (1 << 1); // SSE2
	if (ecx & (1 << 0))  flags |= (1 << 2); // SSE3
	if (ecx & (1 << 9))  flags |= (1 << 3); // SSSE3
	if (ecx & (1 << 19)) flags |= (1 << 4); // SSE4.1
	if (ecx & (1 << 20)) flags |= (1 << 5); // SSE4.2
	if (edx & (1 << 23)) flags |= (1 << 6); // MMX

	return flags;
}

DLL_EXPORT bool is_sse_supported() {
	unsigned int eax, ebx, ecx, edx;
	if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 1) return false;

	__cpuid(1, eax, ebx, ecx, edx);
	return (edx & (1 << 25)) != 0; // SSE
}

DLL_EXPORT bool is_sse2_supported() {
	unsigned int eax, ebx, ecx, edx;
	__cpuid(1, eax, ebx, ecx, edx);
	return (edx & (1 << 26)) != 0;
}

DLL_EXPORT bool is_sse3_supported() {
	unsigned int eax, ebx, ecx, edx;
	__cpuid(1, eax, ebx, ecx, edx);
	return (ecx & (1 << 0)) != 0;
}

DLL_EXPORT bool is_ssse3_supported() {
	unsigned int eax, ebx, ecx, edx;
	__cpuid(1, eax, ebx, ecx, edx);
	return (ecx & (1 << 9)) != 0;
}

DLL_EXPORT bool is_sse41_supported() {
	unsigned int eax, ebx, ecx, edx;
	__cpuid(1, eax, ebx, ecx, edx);
	return (ecx & (1 << 19)) != 0;
}

DLL_EXPORT bool is_sse42_supported() {
	unsigned int eax, ebx, ecx, edx;
	__cpuid(1, eax, ebx, ecx, edx);
	return (ecx & (1 << 20)) != 0;
}

// AVX support: ECX[28]
DLL_EXPORT bool is_avx_supported() {
	unsigned int eax, ebx, ecx, edx;

	// Step 1: CPUID function 1, ECX[28]
	if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 1)
		return false;

	__cpuid(1, eax, ebx, ecx, edx);

	bool avx_bit = (ecx & (1 << 28)) != 0;
	bool osxsave = (ecx & (1 << 27)) != 0; // Check if XSAVE/XGETBV is enabled

	if (!avx_bit || !osxsave)
		return false;

	// Step 2: Check OS supports saving YMM state via XGETBV
	uint32_t xcr0_eax, xcr0_edx;

	// Use xgetbv instruction (EAX=0)
	__asm__ __volatile__ (
		"xgetbv"
		: "=a"(xcr0_eax), "=d"(xcr0_edx)
		: "c"(0)
	);

	// Check if XMM (bit 1) and YMM (bit 2) states are enabled
	return ((xcr0_eax & 0x6) == 0x6);
}

// AVX2 support: CPUID.07H.EBX[5]
DLL_EXPORT bool is_avx2_supported() {
	unsigned int eax, ebx, ecx, edx;

	// Check if CPUID function 7 is available
	if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 7)
		return false;

	// Call CPUID with eax = 7, ecx = 0
	__cpuid_count(7, 0, eax, ebx, ecx, edx);

	// Bit 5 of EBX indicates AVX2 support
	return (ebx & (1 << 5)) != 0;
}

// FMA3 support: CPUID.01H.ECX[12]
DLL_EXPORT bool is_fma3_supported() {
	unsigned int eax, ebx, ecx, edx;

	// Check if CPUID function 1 is supported
	if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 1)
		return false;

	// Call CPUID with eax = 1
	__cpuid(1, eax, ebx, ecx, edx);
	
	// Bit 12 of ECX indicates FMA (FMA3) support
	return (ecx & (1 << 12)) != 0;
}

