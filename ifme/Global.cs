using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ifme
{
	public class Global
	{
		static Random rnd = new Random();
		public static int GetRandom = rnd.Next(0, 9);

		static DateTime RetrieveLinkerTimestamp()
		{
			string filePath = Assembly.GetCallingAssembly().Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;
			byte[] b = new byte[2048];
			Stream s = null;

			try
			{
				s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				s.Read(b, 0, 2048);
			}
			finally
			{
				if (s != null)
				{
					s.Close();
				}
			}

			int i = BitConverter.ToInt32(b, c_PeHeaderOffset);
			int secondsSince1970 = BitConverter.ToInt32(b, i + c_LinkerTimestampOffset);
			DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
			dt = dt.AddSeconds(secondsSince1970);
			dt = dt.AddHours(TimeZone.CurrentTimeZone.GetUtcOffset(dt).Hours);
			return dt;
		}

		public class App
		{
			public static string Name
			{
				get { return Application.ProductName; }
			}

			public static string NameFull
			{
				get { return $"{Name} {(OS.Is64bit ? "64bit" : "32bit")} v{VersionRelease} ( '{CodeName}' )"; }
			}

			public static string CodeName
			{
				get { return "Twin Angel"; }
			}

			public static string Version
			{
				get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
			}

			public static string VersionRelease
			{
				get { return Application.ProductVersion; }
			}

			public static string BuildDate
			{
				get { return RetrieveLinkerTimestamp().Date.ToString("yyyy/MMM/dd"); }
			}

			public static string Type
			{
#if DEBUG
				get { return "Experimental"; }
#elif RELEASE
				get { return "Release"; }
#endif
			}

			public static bool NewRelease = false;
		}

		public class Sounds
		{
			public static string Finish
			{
				get { return Path.Combine(Folder.App, "sounds", "finish.wav"); }
			}
		}

		public class Folder
		{
			public static string App
			{
				get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
			}

			public static string Profile
			{
				get { return Path.Combine(App, "profile"); }
			}

			public static string Plugins
			{
				get { return Path.Combine(App, "plugins"); }
			}

			public static string Temp
			{
				get { return Path.Combine(Path.GetTempPath(), "ifme"); }
			}

			public static string Language
			{
				get { return Path.Combine(App, "lang"); }
			}

			public static string Benchmark
			{
				get { return Path.Combine(App, "benchmark"); }
			}

			public static string Extension
			{
				get { return Path.Combine(App, "extension"); }
			}
		}

		public class File
		{
			public static string Benchmark4K
			{
				get { return Path.Combine(Folder.Benchmark, "gsmarena_v001.mp4"); }
			}
		}
	}
}
