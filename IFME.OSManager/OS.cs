using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME.OSManager
{
    public class OS
    {
        public static bool IsWindows
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT;
            }
        }

        public static bool IsUnix
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.Unix;
            }
        }

        public static bool IsMacOSX
        {
            get
            {
                return Environment.OSVersion.Platform == PlatformID.MacOSX;
            }
        }

        public static bool IsLinux
        {
            get
            {
                return (IsUnix) || (IsMacOSX) || (Environment.OSVersion.Platform == (PlatformID)128);
            }
        }

        public static bool Is64bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        public static string Null
        {
            get
            {
                if (IsWindows)
                    return "nul";
                else
                    return "/dev/null";
            }
        }

        public static string Console
        {
            get
            {
                if (IsWindows)
                    return "cmd";
                else
                    return "bash";
            }
        }

        public static string ConsoleCmd
        {
            get
            {
                if (IsWindows)
                    return "/c";

                return "-c";
            }
        }

        public static string ConsoleEnv(string EnvironmentVariable)
        {
            if (IsWindows)
                return $"/c %{EnvironmentVariable}%";

            return $"-c 'eval ${EnvironmentVariable}'";
        }
    }
}
