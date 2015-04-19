using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
// Asset
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

				// Last Settings
				Data[i, 0] = "null";
				Data[i, 1] = "Last saved configuration";
				Data[i, 2] = Environment.UserName;
				Data[i, 3] = Globals.AppInfo.BuildVersion;
				Data[i, 4] = "https://www.facebook.com/internetfriendlymediaencoder";
				Data[i, 5] = Properties.Settings.Default.UseMkv ? "mkv" : "mp4";
				Data[i, 6] = Properties.Settings.Default.VideoPreset;
				Data[i, 7] = Properties.Settings.Default.VideoTune;
				Data[i, 8] = String.Format("{0}", Properties.Settings.Default.VideoRateType);
				Data[i, 9] = String.Format("{0}", Properties.Settings.Default.VideoRateValue);
				Data[i, 10] = Properties.Settings.Default.VideoCmd;
				Data[i, 11] = Properties.Settings.Default.AudioFormat;
				Data[i, 12] = String.Format("{0}", Properties.Settings.Default.AudioBitRate);
				Data[i, 13] = Properties.Settings.Default.AudioFreq;
				Data[i, 14] = Properties.Settings.Default.AudioChan;
				Data[i, 15] = String.Format("{0}", Properties.Settings.Default.AudioMode);
				Data[i, 16] = Properties.Settings.Default.AudioCmd;
				i++;

				// Default
				Data[i, 0] = "null";
				Data[i, 1] = "Default";
				Data[i, 2] = Environment.UserName;
				Data[i, 3] = Globals.AppInfo.BuildVersion;
				Data[i, 4] = "https://www.facebook.com/internetfriendlymediaencoder";
				Data[i, 5] = "mp4";
				Data[i, 6] = "medium";
				Data[i, 7] = "off";
				Data[i, 8] = "0";
				Data[i, 9] = "28";
				Data[i, 10] = "--dither";
				Data[i, 11] = "Passthrough/Extract all audio (Mode configuration ignored)";
				Data[i, 12] = "128";
				Data[i, 13] = "Automatic";
				Data[i, 14] = "Automatic";
				Data[i, 15] = "0";
				Data[i, 16] = "";

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
