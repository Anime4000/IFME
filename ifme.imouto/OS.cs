using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;

namespace ifme.imouto
{
    public class OS
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

		// Test note:
		// Windows 7 and 8 return 2
		// Ubuntu 14.04.1 return 4

		/// <summary>
		/// Return a friendly name operating system
		/// </summary>
		public static string FriendlyName
		{
			get
			{
				if (IsWindows)
				{
					string result = string.Empty;
					ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem");
					foreach (ManagementObject os in searcher.Get())
					{
						result = os["Caption"].ToString();
						break;
					}

					return String.Format("{0}({1})", result, Environment.OSVersion.Version);
				}
				else
				{
					return String.Format("{0}", Environment.OSVersion);
				}
			}
		}
    }
}
