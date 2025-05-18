using System;
using System.Runtime.InteropServices;

[Flags]
public enum SIMDFlags : uint
{
    None = 0,
    MMX = 1 << 0,
    SSE = 1 << 1,
    SSE2 = 1 << 2,
    SSE3 = 1 << 3,
    SSSE3 = 1 << 4,
    SSE41 = 1 << 5,
    SSE42 = 1 << 6
}

namespace IFME.OSManager
{
    public class CPU
    {
        public static string GetCpuBrandString
        {
            get
            {
                return Marshal.PtrToStringAnsi(get_cpu_brand_string()).Trim();
            }
        }

        public static SIMDFlags GetSIMDFlags
        {
            get
            {
                return (SIMDFlags)get_simd_flags();
            }
        }

        public static bool HasAVX
        {
            get
            {
                return is_avx_supported();
            }
        }

        public static bool HasAVX2
        {
            get
            {
                return is_avx2_supported();
            }
        }

        public static bool HasFMA3
        {
            get
            {
                return is_fma3_supported();
            }
        }

        [DllImport("libcpufeature", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr get_cpu_brand_string();

        [DllImport("libcpufeature", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint get_simd_flags();

        [DllImport("libcpufeature", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool is_avx_supported();

        [DllImport("libcpufeature", CallingConvention = CallingConvention.StdCall)]
        private static extern bool is_avx2_supported();

        [DllImport("libcpufeature", CallingConvention = CallingConvention.StdCall)]
        private static extern bool is_fma3_supported();
    }
}