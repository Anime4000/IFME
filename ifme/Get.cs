using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using Newtonsoft.Json;


namespace ifme
{
    public static class Get
    {
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
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("mime.json"));
			}
		}

		public static string AppRootFolder
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}

		public static string AppName
		{
			get
			{
				return Properties.Resources.AppTitle;
			}
		}

		public static string AppNameLong
		{
			get
			{
				return $"{Properties.Resources.AppTitle} v{Application.ProductVersion} ( '{Properties.Resources.AppCodeName}' )";
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
				file.CopyTo(temppath, false);
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

		internal static bool IsOneOf<T>(this T value, params T[] items)
		{
			for (int i = 0; i < items.Length; ++i)
			{
				if (items[i].Equals(value))
				{
					return true;
				}
			}

			return false;
		}
	}
}
