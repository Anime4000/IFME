using System;

namespace ifme
{
    public class OS
    {
        private static PlatformID P = Environment.OSVersion.Platform;

        /// <summary>
        /// Return true if this program running on Windows NT like Windows 2000, XP, Vista, 7 and later
        /// </summary>
        public static bool IsWindows
        {
            get
            {
                if (P == PlatformID.Win32NT)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Return true if this program running on Linux/Unix-like OS
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                if (P == PlatformID.Unix)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Return true if this program running on Mac OS (Intel)
        /// </summary>
        public static bool IsMacOS
        {
            get
            {
                if (P == PlatformID.MacOSX)
                    return true;
                else
                    return false;
            }
        }

        // Test note:
        // Windows 7 and 8 return 2
        // Ubuntu 14.04.1 return 4

        /// <summary>
        /// Return true if OS is 64bit
        /// </summary>
        public static bool Is64bit
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        /// <summary>
        /// Return general OS name
        /// </summary>
        public static string Name
        {
            get
            {
                if (P == PlatformID.Win32NT)
                    return "Windows NT";
                else if (P == PlatformID.Unix)
                    return "Linux/UNIX";
                else if (P == PlatformID.MacOSX)
                    return "Mac OS";
                return "Unknown";
            }
        }

        /// <summary>
        /// Return null device by specific OS
        /// </summary>
        public static string NULL
        {
            get
            {
                if (P == PlatformID.Win32NT)
                    return "nul";
                else
                    return "/dev/null";
            }
        }
    }
}
