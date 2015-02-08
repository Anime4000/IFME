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
			private static string[,] _Data = new string[500, 16];
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
				Data[0, 5] = "8";
				Data[0, 6] = "0";
				Data[0, 7] = "0";
				Data[0, 8] = "28";
				Data[0, 9] = "--dither";
				Data[0, 10] = "Passthrough/Extract all audio (Mode configuration ignored)";
				Data[0, 11] = "5";
				Data[0, 12] = "2";
				Data[0, 13] = "0";
				Data[0, 14] = "0";
				Data[0, 15] = "";

				foreach (var item in Directory.GetFiles(Folder))
				{
					var parser = new FileIniDataParser();
					IniData data = parser.ReadFile(item);

					Count = ++i;

					Data[i, 0] = item;
					Data[i, 1] = data["profile"]["name"];
					Data[i, 2] = data["profile"]["author"];
					Data[i, 3] = data["profile"]["version"];
					Data[i, 4] = data["profile"]["homepage"];
					Data[i, 5] = data["video"]["preset"];
					Data[i, 6] = data["video"]["tuning"];
					Data[i, 7] = data["video"]["ratectrl"];
					Data[i, 8] = data["video"]["ratefact"];
					Data[i, 9] = data["video"]["command"];
					Data[i, 10] = data["audio"]["encoder"];
					Data[i, 11] = data["audio"]["bit"];
					Data[i, 12] = data["audio"]["freq"];
					Data[i, 13] = data["audio"]["channel"];
					Data[i, 14] = data["audio"]["mode"];
					Data[i, 15] = data["audio"]["command"];
				}

				return i;
			}
		}
	}
}
