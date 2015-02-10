using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// Asset
using ifme.hitoha;
using IniParser;
using IniParser.Model;

namespace ifme
{
	class UserPreset
	{
		public static string Folder
		{
			get { return Path.Combine(Globals.AppInfo.CurrentFolder, "preset"); }
		}

		public static int SelectedId = 0;

		public static class Installed
		{
			private static string[,] _Data = new string[500, 17];
			public static string[,] Data
			{
				get { return _Data; }
				set { _Data = value; }
			}

			private static int _Count = 0;
			public static int Count
			{
				get { return _Count; }
				set { _Count = value; }
			}

			public static int Get()
			{
				int i = 0;
				Array.Clear(Data, 0, Data.Length);

				// Default
				Data[0, 0] = "null";
				Data[0, 1] = "Default";
				Data[0, 2] = "Anime4000";
				Data[0, 3] = "0.1";
				Data[0, 4] = "https://github.com/x265";
				Data[0, 5] = "mp4";
				Data[0, 6] = "veryslow";
				Data[0, 7] = "off";
				Data[0, 8] = "0";
				Data[0, 9] = "28";
				Data[0, 10] = "--dither";
				Data[0, 11] = "Passthrough/Extract all audio (Mode configuration ignored)";
				Data[0, 12] = "128";
				Data[0, 13] = "44100";
				Data[0, 14] = "Stereo";
				Data[0, 15] = "0";
				Data[0, 16] = "";

				foreach (var item in Directory.GetFiles(Folder, "*.nemu"))
				{
					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(item);

					if (!Properties.Settings.Default.UseMkv)
						if (String.Equals(data["profile"]["format"], "mkv", StringComparison.CurrentCultureIgnoreCase))
							continue;

					Count = ++i;

					Data[i, 0] = item;
					Data[i, 1] = data["profile"]["name"];
					Data[i, 2] = data["profile"]["author"];
					Data[i, 3] = data["profile"]["version"];
					Data[i, 4] = data["profile"]["homepage"];
					Data[i, 5] = data["profile"]["format"];
					Data[i, 6] = data["video"]["preset"];
					Data[i, 7] = data["video"]["tuning"];
					Data[i, 8] = data["video"]["ratectrl"];
					Data[i, 9] = data["video"]["ratefact"];
					Data[i, 10] = data["video"]["command"];
					Data[i, 11] = data["audio"]["encoder"];
					Data[i, 12] = data["audio"]["bit"];
					Data[i, 13] = data["audio"]["freq"];
					Data[i, 14] = data["audio"]["channel"];
					Data[i, 15] = data["audio"]["mode"];
					Data[i, 16] = data["audio"]["command"];
				}

				return i;
			}
		}
	}
}
