using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace ifme.hitoha
{
	class Globals
	{
		public static class AppInfo
		{
			/// <summary>
			/// Read creation date via PE header and return Date datatype
			/// </summary><returns>Date datatype (without time)</returns>
			private static DateTime RetrieveLinkerTimestamp()
			{
				string filePath = System.Reflection.Assembly.GetCallingAssembly().Location;
				const int c_PeHeaderOffset = 60;
				const int c_LinkerTimestampOffset = 8;
				byte[] b = new byte[2048];
				System.IO.Stream s = null;

				try
				{
					s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
					s.Read(b, 0, 2048);
				}
				finally
				{
					if (s != null)
					{
						s.Close();
					}
				}

				int i = System.BitConverter.ToInt32(b, c_PeHeaderOffset);
				int secondsSince1970 = System.BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
				DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
				dt = dt.AddSeconds(secondsSince1970);
				dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
				return dt;
			}

			/// <summary>
			/// Return this program full name
			/// </summary>
			public static string Name
			{
				get { return System.Windows.Forms.Application.ProductName; }
			}

			/// <summary>
			/// Return this program acronym name
			/// </summary>
			public static string NameShort
			{
				get { return "IFME"; }
			}

			/// <summary>
			/// Return program code name
			/// </summary>
			public static string NameCode
			{
				get { return Properties.Resources.EpicWord; }
			}

			public static string NameTitle = String.Format("{0} v{1} ( '{2}' )", NameShort, Version, NameCode);

			/// <summary>
			/// Return program complied date via PE Header
			/// </summary>
			public static string BuildDate
			{
				get { return RetrieveLinkerTimestamp().Date.ToString("dd/MMM/yyyy"); }
			}

			/// <summary>
			/// Return current version in string (including dot)
			/// </summary>
			public static string Version
			{
				get { return System.Windows.Forms.Application.ProductVersion; }
			}

			public static string VersionMsg = "Unable to fetch latest version, no internet connected.";
			public static string VersionNew = null;
			public static bool VersionEqual = true;

			/// <summary>
			/// Return author name
			/// </summary>
			public static string Author
			{
				get { return System.Windows.Forms.Application.CompanyName; }
			}

			/// <summary>
			/// Return website address
			/// </summary>
			public static string WebSite
			{
				get { return "http://ifme.sf.net/"; }
			}

		#if DEBUG
			private const string _Bulid = "DEBUG";
		#else
			private const string _Bulid = "RELEASE";
		#endif
			private static string _CPU = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

			/// <summary>
			/// Return machine CPU and program build release
			/// </summary>
			public static string CPU
			{
				get
				{
					if (String.IsNullOrEmpty(_CPU))
						return String.Format("{0} {1}", OS.Name, _Bulid);
					else
						return String.Format("{0}/{1} {2}", _CPU, OS.Name, _Bulid);
				}
			}

			/// <summary>
			/// Return program name for encoding metadata
			/// </summary>
			public static string WritingApp
			{
				get { return String.Format("Encoded with IFME v{0}", Version); }
			}

			/// <summary>
			/// Return current program folder
			/// </summary>
			public static string CurrentFolder
			{
				get { return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); }
			}

			/// <summary>
			/// Return this program temporary folder
			/// </summary>
			public static string TempFolder
			{
				get { return Path.Combine(Path.GetTempPath(), "ifme"); }
			}

			static Random rnd = new Random();
			public static int CharTheme = rnd.Next(1, 100);
		}

		public static class Files
		{
			/// <summary>
			/// Return list of ISO 639-2B file
			/// </summary>
			public static string ISO
			{
				get { return Path.Combine(Globals.AppInfo.CurrentFolder, "iso.gg"); }
			}
		}
	}

	class OS
	{
		private static int p = (int)Environment.OSVersion.Platform;
		private static bool _IsWindows = false;
		private static bool _IsLinux = false;

		// Extra note:
		// Windows 7 and 8 return 2
		// Ubuntu 14.04.1 return 4

		/// <summary>
		/// Return true if this program running on Windows OS
		/// </summary>
		public static bool IsWindows
		{
			get
			{
				if (p == 2)
					_IsWindows = true;
				return _IsWindows;
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
					_IsLinux = true;
				return _IsLinux;
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
	}

	class ListQueue
	{
		public static int Current = 0;
		public static int Count = 0;
	}

	class Log
	{
		public static int Info = 0;
		public static int OK = 1;
		public static int Warn = 2;
		public static int Error = 3;
		public static string AutoSaveName = "";
	}
}
