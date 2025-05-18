using System.Runtime.InteropServices;

class CPUFeature
{
	static bool? cached;

	public static bool HasAVX2()
	{
		if (cached.HasValue) return cached.Value;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			return is_avx2_supported_win();
		}
		else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return is_avx2_supported_linux();
		}
		return false;
	}

	public static bool HasFMA3()
	{
        if (cached.HasValue) return cached.Value;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return is_fma3_supported_win();
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return is_fma3_supported_linux();
        }
        return false;
    }

	[DllImport("libcpufeature.dll", EntryPoint = "is_avx2_supported", CallingConvention = CallingConvention.StdCall)]
	private static extern bool is_avx2_supported_win();

	[DllImport("libcpufeature.so", EntryPoint = "is_avx2_supported")]
	private static extern bool is_avx2_supported_linux();

    [DllImport("libcpufeature.dll", EntryPoint = "is_fma3_supported", CallingConvention = CallingConvention.StdCall)]
    private static extern bool is_fma3_supported_win();

    [DllImport("libcpufeature.so", EntryPoint = "is_fma3_supported")]
    private static extern bool is_fma3_supported_linux();
}
