using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// Asset
using ifme.hitoha;
using IniParser;
using IniParser.Model;

namespace ifme.hitoha
{
	class Addons
	{
		public static void Extract(string Archive, string Output)
		{
			System.Diagnostics.Process P = new System.Diagnostics.Process();

			P.StartInfo.FileName = "unpack.exe";
			P.StartInfo.Arguments = String.Format("x -y -o\"{0}\" \"{1}\"", Output, Archive);
			P.StartInfo.WorkingDirectory = Path.GetFullPath(".");
			P.StartInfo.UseShellExecute = false;
			P.StartInfo.CreateNoWindow = true;
			P.StartInfo.RedirectStandardOutput = true;
			P.StartInfo.RedirectStandardError = true;

			P.Start();
			P.WaitForExit();
			P.Close();
		}

		public static string Folder
		{
			get { return Path.GetFullPath("addons"); }
		}

		public static string IniFile
		{
			get { return "addon.ini"; }
		}

		public static class BuildIn
		{
			public static string FFmpeg = Path.Combine(Folder, "ffmpeg", "ffmpeg.exe");
			public static string FFms = Path.Combine(Folder, "ffmsindex", "ffmsindex.exe");
			public static string HEVC = Path.Combine(Folder, "x265", "x265.exe");
			public static string HEVCHI = Path.Combine(Folder, "x265", "x265hi.exe");
			public static string MKV = Path.Combine(Folder, "mkvmerge", "mkvmerge.exe");
			public static string MKE = Path.Combine(Folder, "mkvmerge", "mkvextract.exe");
			public static string MP4 = Path.Combine(Folder, "mp4box", "mp4box.exe");
			public static string MP4FPS = Path.Combine(Folder, "mp4fpsmod", "mp4fpsmod.exe");
		}

		public static class Installed
		{
			private static string[,] _Data = new string[500, 15];
			public static int Count = 0;
			public static bool IsUpdated = false;

			public static string[,] Data
			{
				get { return _Data; }
				set { _Data = value; }
			}

			public static int Get()
			{
				int i = 0;
				foreach (var item in Directory.GetDirectories(Folder))
				{
					if (!System.IO.File.Exists(Path.Combine(item, IniFile)))
						continue;

					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(Path.Combine(item, IniFile));

					if (data["addon"]["type"] != "audio")
						continue;

					Data[i, 0] = item;
					Data[i, 1] = data["addon"]["type"];
					Data[i, 2] = data["profile"]["name"];
					Data[i, 3] = data["profile"]["dev"];
					Data[i, 4] = data["profile"]["version"];
					Data[i, 5] = data["profile"]["homepage"];
					Data[i, 6] = data["profile"]["container"];
					Data[i, 7] = data["provider"]["name"];
					Data[i, 8] = data["provider"]["update"];
					Data[i, 9] = data["provider"]["download"];
					Data[i, 10] = data["data"]["app"];
					Data[i, 11] = data["data"]["cmd"].Replace('|', '"');
					Data[i, 12] = data["data"]["adv"];
					Data[i, 13] = data["data"]["quality"];
					Data[i, 14] = data["data"]["default"];

					i++;
				}
				return GetBuildin(i);
			}

			public static int GetCount()
			{
				return System.IO.Directory.GetDirectories(Folder).Count();
			}

			public static int GetBuildin(int x)
			{
				int k = x;
				foreach (var item in System.IO.Directory.GetDirectories(Folder))
				{
					// GetFileName works with folder, just dont get "/"
					string name = System.IO.Path.GetFileName(item);

					if (!File.Exists(Path.Combine(item, name + ".aai")))
						continue;

					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(Path.Combine(item, name + ".aai"));

					if (data["addon"]["type"] == "audio")
						continue;

					Data[k, 0] = item;
					Data[k, 1] = data["addon"]["type"];
					Data[k, 2] = data["profile"]["name"];
					Data[k, 3] = data["profile"]["dev"];
					Data[k, 4] = data["profile"]["version"];
					Data[k, 5] = data["profile"]["homepage"];
					Data[k, 6] = data["profile"]["container"];
					Data[k, 7] = data["provider"]["name"];
					Data[k, 8] = data["provider"]["update"];
					Data[k, 9] = data["provider"]["download"];
					Data[k, 10] = data["data"]["app"];
					Data[k, 11] = data["data"]["cmd"];
					Data[k, 12] = data["data"]["adv"];
					Data[k, 13] = data["data"]["quality"];
					Data[k, 14] = data["data"]["default"];

					k++;
				}

				return k;
			}
		}
	}
}
