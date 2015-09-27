using System;
using System.Collections.Generic;
using System.IO;

using IniParser;
using IniParser.Model;

namespace ifme
{
	public class Plugin
	{
		public string IniFile;
		public info Info = new info();
		public profile Profile = new profile();
		public provider Provider = new provider();
		public app App = new app();
		public arg Arg = new arg();

		public class info
		{
			public string Type;
			public string Support;
		}

		public class profile
		{
			public int Arch;
			public string Name;
			public string Dev;
			public string Ver;
			public string Web;
		}

		public class provider
		{
			public string Name;
			public string Update;
			public string Download;
		}

		public class app
		{
			public string Bin;
			public string Ext;
			public string[] Quality;
			public string Default;
		}

		public class arg
		{
			public string Input;
			public string Output;
			public string Bitrate;
			public string Advance;
		}

		public class Default
		{
			public class Audio
			{
				public static string Name = "Passthrough (Extract all audio)";
				public static string BitRate = "256";
				public static string Frequency = "auto";
				public static string Channel = "auto";
				public static bool Merge = false;
				public static string Command = null;
			}
		}

		public static string HEVCL = Path.Combine(Global.Folder.Plugins, $"x265{Properties.Settings.Default.Compiler}", "x265lo");
		public static string HEVCH = Path.Combine(Global.Folder.Plugins, $"x265{Properties.Settings.Default.Compiler}", "x265hi");
		public static string LIBAV = Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffmpeg");
		public static string PROBE = Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffprobe");
		public static string FPLAY = Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffplay");
		public static string MKVEX = Path.Combine(Global.Folder.Plugins, "mkvtool", "mkvextract");
		public static string MKVME = Path.Combine(Global.Folder.Plugins, "mkvtool", "mkvmerge");
		public static string MP4BX = Path.Combine(Global.Folder.Plugins, "mp4box", "mp4box");
		public static string AVS4P = Path.Combine(Global.Folder.Plugins, "avisynth", "avs2pipe");
		public static string FFMS2 = Path.Combine(Global.Folder.Plugins, "ffmsindex", "ffmsindex");
		public static string MP4FP = Path.Combine(Global.Folder.Plugins, "mp4fpsmod", "mp4fpsmod");

		public static string AviSynthFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "avisynth.dll");
		public static bool IsExistAviSynth = File.Exists(AviSynthFile);
		public static bool IsForceAviSynth = false;

		public static bool IsExistHEVCGCC = false;
		public static bool IsExistHEVCICC = false;
		public static bool IsExistHEVCMSVC = false;

		public static List<Plugin> List = new List<Plugin>();

		public static void Load()
		{
			// Revoke
			List.Clear();

			// Ready build-in
			BuildIn();

			// Load plugins
			foreach (var item in Directory.GetFiles(Global.Folder.Plugins, "_*.ini", SearchOption.AllDirectories))
			{
				var parser = new FileIniDataParser();
				IniData data = parser.ReadFile(item);

				var p = new Plugin();

				p.IniFile = item;
				p.Info.Type = data["info"]["type"];
				p.Info.Support = data["info"]["support"];
				p.Profile.Arch = int.Parse(data["profile"]["arch"]);
				p.Profile.Name = data["profile"]["name"];
				p.Profile.Dev = data["profile"]["dev"];
				p.Profile.Ver = data["profile"]["ver"];
				p.Profile.Web = data["profile"]["web"];
				p.Provider.Name = data["provider"]["name"];
				p.Provider.Update = data["provider"]["update"];
				p.Provider.Download = data["provider"]["download"];

				if (string.Equals(p.Info.Type, "audio"))
				{
					p.App.Bin = Path.Combine(Path.GetDirectoryName(item), data["app"]["bin"]);
					p.App.Ext = data["app"]["ext"];
					p.App.Quality = data["app"]["quality"].Split(',');
					p.App.Default = data["app"]["default"];
					p.Arg.Input = data["arg"]["input"];
					p.Arg.Output = data["arg"]["output"];
					p.Arg.Bitrate = data["arg"]["bitrate"];
					p.Arg.Advance = data["arg"]["advance"];
				}

				List.Add(p);
			}
		}

		public static bool IsExist(string name)
		{
			foreach (var item in List)
				if (string.Equals(item.Profile.Name, name, StringComparison.OrdinalIgnoreCase))
					return true;

			return false;
		}

		public static void BuildIn()
		{
			// Audio section
			var a = new Plugin();
			a.Info.Type = "audio";
			a.Info.Support = "mp4";
			a.Profile.Name = "No Audio";
			a.Profile.Dev = "Anime4000";
			a.Profile.Ver = Global.App.Version;
			a.Profile.Web = "https://x265.github.io/";
			a.Provider.Name = "IFME";
			a.Provider.Update = null;
			a.Provider.Download = null;
			a.App.Bin = null;
			a.App.Quality = new[] { "0" };
			a.App.Default = "0";
			a.Arg.Input = null;
			a.Arg.Output = null;
			a.Arg.Bitrate = null;
			a.Arg.Advance = null;

			List.Add(a);

			var b = new Plugin();
			b.Info.Type = "audio";
			b.Info.Support = "mp4";
			b.Profile.Name = Default.Audio.Name;
			b.Profile.Dev = "Anime4000";
			b.Profile.Ver = Global.App.Version;
			b.Profile.Web = "https://x265.github.io/";
			b.Provider.Name = "IFME";
			b.Provider.Update = null;
			b.Provider.Download = null;
			b.App.Bin = null;
			b.App.Quality = new[] { "128", "192", "256", "384", "512", "768", "1024" };
			b.App.Default = Default.Audio.BitRate;
			b.Arg.Input = null;
			b.Arg.Output = null;
			b.Arg.Bitrate = null;
			b.Arg.Advance = null;

			List.Add(b);
		}
	}
}
