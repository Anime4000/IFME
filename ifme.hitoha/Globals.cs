using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme.hitoha
{
	class Globals
	{
		public static class AppInfo
		{
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

			public static string Name
			{
				get { return System.Windows.Forms.Application.ProductName; }
			}

			public static string NameShort
			{
				get { return "IFME"; }
			}

			public static string NameCode
			{
				get { return "Himari-chan desu"; }
			}

			public static string BuildDate
			{
				get { return RetrieveLinkerTimestamp().Date.ToString("d"); }
			}

			public static bool VersionEqual = true;
			public static string VersionNew = "";
			public static string Version
			{
				get { return System.Windows.Forms.Application.ProductVersion; }
			}

			public static string Author
			{
				get { return System.Windows.Forms.Application.CompanyName; }
			}

			public static string WebSite
			{
				get { return "http://ifme.sf.net/"; }
			}

			public static string CPU
			{
				get { return System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"); }
			}

			public static string WritingApp
			{
				get { return String.Format("Encoded with IFME v{0}", Version); }
			}

			public static string CurrentFolder
			{
				get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location); }
			}

			public static string TempFolder
			{
				get { return CurrentFolder + "\\ztemp"; }
			}
		}

		public static class Files
		{
			public static string ISO
			{
				get { return AppInfo.CurrentFolder + "\\iso.gg"; }
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
	}
}
