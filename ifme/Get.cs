using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

using Newtonsoft.Json;


namespace ifme
{
	static class Get
    {
        public static bool IsReady { get; set; } = false;

        public static Dictionary<string, string> LanguageCode
		{
			get
			{
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("language.json"));
			}
		}

		public static Dictionary<string, string> MimeType
		{
			get
			{
                var fmime = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("mime.json"));
                var nmime = new Dictionary<string, string>();

                foreach (var item in fmime)
                    nmime.Add(item.Key, $"[{item.Key}] {item.Value}");

                return nmime;
			}
		}

		public static string AppPath
		{
			get
			{
				return Assembly.GetExecutingAssembly().Location;
			}
		}

		public static string AppRootFolder
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		public static Icon AppIcon
		{
			get
			{
				return Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
			}
		}

		public static string AppName
		{
			get
			{
                return Branding.Title();
			}
		}

		public static string AppNameLong
		{
			get
			{
				return $"{AppName} v{Application.ProductVersion} ('{Properties.Resources.AppCodeName}')";
			}
		}

		public static string AppNameLib
		{
			get
			{
				return $"{Branding.TitleShort()} v{Application.ProductVersion} {(OS.Is64bit ? "amd64" : "i686")} {(OS.IsWindows ? "windows" : "unix-like")}";
			}
		}

		public static string FolderTemp
		{
			get
			{
				if (string.IsNullOrEmpty(Properties.Settings.Default.TempDir))
				{
					Properties.Settings.Default.TempDir = Path.Combine(Path.GetTempPath(), "IFME");
					Properties.Settings.Default.Save();
				}
				
				return Properties.Settings.Default.TempDir;
			}
		}

		public static string FolderSave
		{
			get
			{
				if (string.IsNullOrEmpty(Properties.Settings.Default.OutputDir))
				{
					Properties.Settings.Default.OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "IFME");
					Properties.Settings.Default.Save();
				}

				return Properties.Settings.Default.OutputDir;
			}
		}

		public static string CodecFormat(string codecId)
        {
            var json = File.ReadAllText("format.json");
            var format = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var formatId = string.Empty;

            if (format.TryGetValue(codecId, out formatId))
                return formatId;

            return "mkv";
        }

		public static string FileLang(string file)
		{
			file = Path.GetFileNameWithoutExtension(file);
			return file.Substring(file.Length - 3);
		}

		public static string FileSizeIEC(long InBytes)
		{
			string[] IEC = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

			if (InBytes == 0)
				return $"0{IEC[0]}";

			long bytes = Math.Abs(InBytes);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return $"{(Math.Sign(InBytes) * num)}{IEC[place]}";
		}

		public static string FileSizeDEC(long InBytes)
		{
			string[] DEC = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

			if (InBytes == 0)
				return $"0{DEC[0]}";

			long bytes = Math.Abs(InBytes);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
			double num = Math.Round(bytes / Math.Pow(1000, place), 1);
			return $"{(Math.Sign(InBytes) * num)}{DEC[place]}";
		}

		public static string Duration(DateTime past)
		{
			var span = DateTime.Now.Subtract(past);

			if (span.Days != 0)
				return $"{span.Days}d {span.Hours}h {span.Minutes}m {span.Seconds}s {span.Milliseconds}ms";
			else if (span.Hours != 0)
				return $"{span.Hours}h {span.Minutes}m {span.Seconds}s {span.Milliseconds}ms";
			else if (span.Minutes != 0)
				return $"{span.Minutes}m {span.Seconds}s {span.Milliseconds}ms";
			else
				return $"{span.Seconds}s {span.Milliseconds}ms";
		}

		internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, true);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
	}
}
