using System;

namespace ifme
{
	class OS
	{
		private static int p = (int)Environment.OSVersion.Platform;

		/// <summary>
		/// Return true if this program running on Windows OS
		/// </summary>
		public static bool IsWindows
		{
			get
			{
				if (p == 2)
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
				if ((p == 4) || (p == 6) || (p == 128))
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
				if ((p == 4) || (p == 6) || (p == 128))
					return "Linux";
				return "Windows";
			}
		}

		/// <summary>
		/// Return null device by specific OS
		/// </summary>
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
	}
}
