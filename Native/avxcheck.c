/*
 * avxcheck.c - AVX2 CPU feature detection for IFME
 *
 * This file defines a native function to check whether the host CPU
 * supports the AVX2 instruction set (Advanced Vector Extensions 2).
 * AVX2 is required by certain encoders such as x265 and H.266.
 *
 * Platform: Linux (shared object .so), Windows (dynamic link library .dll)
 * Author: Anime4000
 * License: GPL-2.0
 *
 * Compile with:
 *     gcc -fPIC -shared -o libavxcheck.so avxcheck.c
 *     gcc -fPIC -shared -o libavxcheck.dll avxcheck.c
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
