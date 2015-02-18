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
			string cmd = null, arg = null;
			System.Diagnostics.Process P = new System.Diagnostics.Process();

			if (OS.IsWindows)
			{
				cmd = "cmd.exe";
				arg = String.Format("/c .\\unpack.exe x -y -o\"{0}\" \"{1}\"", Output, Archive);
			}
			else
			{
				cmd = "bash";
				arg = String.Format("-c \"./unpack x \\\"{1}\\\" -so | tar xf - -C \\\"{0}\\\" && echo ONE ADDONS UPDATED!\"", Output, Archive);
			}

			P.StartInfo.FileName = cmd;
			P.StartInfo.Arguments = arg;
			P.StartInfo.WorkingDirectory = Globals.AppInfo.CurrentFolder;
			P.StartInfo.UseShellExecute = false;
			P.StartInfo.CreateNoWindow = true;

			P.Start();
			P.WaitForExit();
			P.Close();
		}

		public static string Folder
		{
			get { return Path.Combine(Globals.AppInfo.CurrentFolder, "addons"); }
		}

		public static string IniFile
		{
			get { return "addon.ini"; }
		}

		public static class BuildIn
		{
			public static string FFmpeg = Path.Combine(Folder, "ffmpeg", "ffmpeg");
			public static string FFms = Path.Combine(Folder, "ffmsindex", "ffmsindex");
			public static string HEVCLO = Path.Combine(Folder, "x265", "x265lo");
			public static string HEVCHI = Path.Combine(Folder, "x265", "x265hi");
			public static string MKV = Path.Combine(Folder, "mkvmerge", "mkvmerge");
			public static string MKE = Path.Combine(Folder, "mkvmerge", "mkvextract");
			public static string MP4 = Path.Combine(Folder, "mp4box", "MP4Box");
			public static string MP4FPS = Path.Combine(Folder, "mp4fpsmod", "mp4fpsmod");
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
				int i = 1;
				Array.Clear(Data, 0, Data.Length);

				// Passthrough
				Data[0, 0] = "0|null";
				Data[0, 1] = "audio";
				Data[0, 2] = "Passthrough/Extract all audio (Mode configuration ignored)";
				Data[0, 3] = "// If MP4 output format is selected,";
				Data[0, 4] = "// unsupported codec will converted to AAC.";
				Data[0, 5] = "// Just-in-case, choose best quality as you like.";
				Data[0, 6] = "mp4";
				Data[0, 7] = "Build-in";
				Data[0, 8] = "http://example.com/example.txt";
				Data[0, 9] = "http://example.com/example.ifz";
				Data[0, 10] = "../ffmpeg/ffmpeg";
				Data[0, 11] = "";
				Data[0, 12] = "";
				Data[0, 13] = "45,64,80,96,112,128,160,192,224,256,320,499";
				Data[0, 14] = "128";

				// Get Audio addons first, pref. issue making Index here same with ComboBox Index
				foreach (var item in Directory.GetDirectories(Folder))
				{
					if (!System.IO.File.Exists(Path.Combine(item, IniFile)))
						continue;

					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(Path.Combine(item, IniFile));

					if (!Properties.Settings.Default.UseMkv)
						if (String.Equals(data["profile"]["container"], "mkv", StringComparison.CurrentCultureIgnoreCase))
							continue;

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

				// Then get other addons
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
					string name = System.IO.Path.GetFileName(item) + ".ini";

					if (!File.Exists(Path.Combine(item, name)))
						continue;

					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(Path.Combine(item, name));

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
