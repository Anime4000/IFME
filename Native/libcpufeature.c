/*
 * libcpufeature.c - CPU feature detection for IFME
 *
 * This file defines a native function to check whether the host CPU
 * supports the AVX2 instruction set (Advanced Vector Extensions 2).
 * or support the FMA3 instruction set (Fused Multiply-Add 3).
 *
 * Platform: Linux (shared object .so), Windows (dynamic link library .dll)
 * Author: Anime4000
 * License: GPL-2.0
 *
 * Compile with:
 *     gcc -fPIC -shared -o libcpufeature.so libcpufeature.c
 *     gcc -fPIC -shared -o libcpufeature.dll libcpufeature.c
 */

#include <cpuid.h>
#include <stdbool.h>
#include <stddef.h>

bool is_avx2_supported() {
    unsigned int eax, ebx, ecx, edx;

    // Check if CPUID function 7 is available
    if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 7)
        return false;

    // Call CPUID with eax = 7, ecx = 0
    __cpuid_count(7, 0, eax, ebx, ecx, edx);

    // Bit 5 of EBX indicates AVX2 support
    return (ebx & (1 << 5)) != 0;
}

bool is_fma3_supported() {
    unsigned int eax, ebx, ecx, edx;

    // Check if CPUID function 1 is supported
    if (!__get_cpuid_max(0, NULL) || __get_cpuid_max(0, NULL) < 1)
        return false;

    // Call CPUID with eax = 1
    __cpuid(1, eax, ebx, ecx, edx);

    // Bit 12 of ECX indicates FMA (FMA3) support
    return (ecx & (1 << 12)) != 0;
}