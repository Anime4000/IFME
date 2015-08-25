using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
			System.IO.Stream s = null;

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
				get { return System.Windows.Forms.Application.ProductName; }
			}

			public static string NameFull
			{
				get { return String.Format("{0} v{1} ( '{2}' ) - {3}", Name, Version, CodeName, Type); }
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
				get { return Path.Combine(Folder.AppDir, "sounds", "finish.wav"); }
			}
		}

		public class Folder
		{
			public static string AppDir
			{
				get { return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location); }
			}

			public static string Profile
			{
				get { return Path.Combine(AppDir, "profile"); }
			}

			public static string Plugins
			{
				get { return Path.Combine(AppDir, "plugins"); }
			}

			public static string Temp
			{
				get { return Path.Combine(Path.GetTempPath(), "ifme"); }
			}

			public static string Language
			{
				get { return Path.Combine(AppDir, "lang"); }
			}

			public static string Benchmark
			{
				get { return Path.Combine(Global.Folder.AppDir, "benchmark"); }
			}

			public static string Extension
			{
				get { return Path.Combine(Global.Folder.AppDir, "extension"); }
			}
		}

		public class File
		{
			public static string Benchmark4K
			{
				get { return Path.Combine(Global.Folder.Benchmark, "gsmarena_v001.mp4"); }
			}
		}
	}
}
