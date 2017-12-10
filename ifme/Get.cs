using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;

using Newtonsoft.Json;

enum MediaType
{
    Video,
    Audio,
    Subtitle,
    Attachment,
    VideoAudio
}

namespace ifme
{
    internal static class Get
    {
        internal static bool IsReady { get; set; } = false;

        internal static Dictionary<string, string> LanguageCode
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(AppRootDir, "language.json")));
            }
        }

        internal static Dictionary<string, string> MimeList
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(AppRootDir, "mime.json")));
            }
        }

        internal static Dictionary<string, string> TargetFormat
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(AppRootDir, "targetfmt.json")));
            }
        }

        internal static SortedSet<string> MimeTypeList
        {
            get
            {
                var temp = new SortedSet<string>();

                foreach (var item in MimeList)
                {
                    try { temp.Add(item.Value); } catch { }
                }

                return temp;
            }
        }

        internal static string AppPath
        {
            get
            {
                return Assembly.GetExecutingAssembly().Location;
            }
        }

        internal static string AppRootDir
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        internal static Icon AppIcon
        {
            get
            {
                return Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            }
        }

        internal static string AppName
        {
            get
            {
                return Branding.Title();
            }
        }

        internal static string AppNameLong
        {
            get
            {
                return $"{AppName} v{Application.ProductVersion} ('{Properties.Resources.AppCodeName}')";
            }
        }

        internal static string AppNameLongAdmin
        {
            get
            {
                return $"{AppNameLong} {(Elevated.IsAdmin ? "[Administrator]" : "")}";
            }
        }

        internal static string AppNameProject(string filePath)
        {
            return $"{Path.GetFileName(filePath)} - {AppNameLongAdmin}";
        }

        internal static string AppNameLib
		{
			get
			{
				return $"{Branding.TitleShort()} v{Application.ProductVersion} {(OS.Is64bit ? "amd64" : "i686")} {(OS.IsWindows ? "windows" : "unix-like")}";
			}
		}

		internal static string FolderTemp
		{
			get
			{				
				return Properties.Settings.Default.TempDir;
			}
			set
			{
				Properties.Settings.Default.TempDir = value;
				Properties.Settings.Default.Save();
			}
		}

		internal static string FolderSave
		{
			get
			{
				return Properties.Settings.Default.OutputDir;
			}
			set
			{
				Properties.Settings.Default.OutputDir = value;
				Properties.Settings.Default.Save();
			}
		}

        internal static string CodecFormat(string codecId)
		{
			var json = File.ReadAllText(Path.Combine(AppRootDir, "format.json"));
			var format = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			var formatId = string.Empty;

			if (format.TryGetValue(codecId, out formatId))
				return formatId;

			return "mkv";
		}

        internal static string FileExtension(string file)
        {
            return Path.GetExtension(file).ToLowerInvariant();
        }

		internal static string FileLang(string file)
		{
			file = Path.GetFileNameWithoutExtension(file);

			return file.Substring(file.Length - 3);
		}

		internal static string FileSizeIEC(long InBytes)
		{
			string[] IEC = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

			if (InBytes == 0)
				return $"0{IEC[0]}";

			long bytes = Math.Abs(InBytes);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return $"{(Math.Sign(InBytes) * num)}{IEC[place]}";
		}

		internal static string FileSizeDEC(long InBytes)
		{
			string[] DEC = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

			if (InBytes == 0)
				return $"0{DEC[0]}";

			long bytes = Math.Abs(InBytes);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
			double num = Math.Round(bytes / Math.Pow(1000, place), 1);
			return $"{(Math.Sign(InBytes) * num)}{DEC[place]}";
		}

		internal static string Duration(DateTime past)
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

        internal static List<string> FilesRecursive()
        {
            var files = new List<string>();
            var fbd = new FolderBrowserDialog
            {
                ShowNewFolderButton = false,
                SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var dirs = new List<string>(Directory.GetDirectories(fbd.SelectedPath));

                if (dirs.Count == 0)
                    dirs.Add(fbd.SelectedPath);

                foreach (var d in dirs)
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        files.Add(f);
                    }
                }
            }

            return files;
        }


        internal static string MimeType(string FileName)
		{
			var mime = new Dictionary<string, string>(MimeList, StringComparer.InvariantCultureIgnoreCase);
			var type = string.Empty;

			if (mime.TryGetValue(Path.GetExtension(FileName), out type))
			{
				return type;
			}

			return "application/octet-stream";
		}

        internal static string LangCheck(string lang)
        {
            var temp = string.Empty;

            if (LanguageCode.TryGetValue(lang, out temp))
            {
                return lang; // if found
            }

            return "und";
        }

        internal static bool IsValidPath(string FilePath)
		{
			try
			{
				Path.GetFileName(FilePath);

				return Path.IsPathRooted(FilePath);
			}
			catch (Exception)
			{
				return false;
			}
		}

		internal static string NewFilePath(string SaveDir, string FilePath, string Ext)
		{
			// base
			var filename = Path.GetFileNameWithoutExtension(FilePath);
			var prefix = string.Empty;
			var postfix = string.Empty;

			// prefix
			if (Properties.Settings.Default.FileNamePrefixType == 1)
				prefix = $"[{DateTime.Now:yyyyMMdd_HHmmss}] ";
			else if (Properties.Settings.Default.FileNamePrefixType == 2)
				prefix = Properties.Settings.Default.FileNamePrefix;

			// postfix
			if (Properties.Settings.Default.FileNamePostfixType == 1)
				postfix = Properties.Settings.Default.FileNamePostfix;

			// use save folder
			filename = Path.Combine(SaveDir, $"{prefix}{filename}{postfix}{Ext}");

			// check SaveDir is valid
			if (!IsValidPath(filename))
				filename = Path.Combine(Path.GetDirectoryName(FilePath), $"{prefix}{filename}{postfix}{Ext}");

			// if exist, make a duplicate
			if (File.Exists(filename))
				filename = Path.Combine(Path.GetDirectoryName(filename), $"{prefix}{filename}{postfix} NEW{Ext}");

			return filename;
		}
	}
}
