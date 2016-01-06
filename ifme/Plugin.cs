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
			public string Raw;
			public string Input;
			public string Output;
			public string Bitrate;
			public string Advance;
		}

		// This 'get' always retrive latest variable data
		public static string HEVC08 { get { return Path.Combine(Global.Folder.Plugins, $"x265{Properties.Settings.Default.Compiler}", "x265-08"); } }
		public static string HEVC10 { get { return Path.Combine(Global.Folder.Plugins, $"x265{Properties.Settings.Default.Compiler}", "x265-10"); } }
		public static string HEVC12 { get { return Path.Combine(Global.Folder.Plugins, $"x265{Properties.Settings.Default.Compiler}", "x265-12"); } }

		// Run once
		public static string FFMPEG { get { return Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffmpeg"); } }
		public static string FFPROBE { get { return Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffprobe"); } }
		public static string FFPLAY { get { return Path.Combine(Global.Folder.Plugins, "ffmpeg", "ffplay"); } }
		public static string MKVEXT { get { return Path.Combine(Global.Folder.Plugins, "mkvtoolnix", "mkvextract"); } }
		public static string MKVMER { get { return Path.Combine(Global.Folder.Plugins, "mkvtoolnix", "mkvmerge"); } }
		public static string MP4BOX { get { return Path.Combine(Global.Folder.Plugins, "mp4box", "MP4Box"); } }
		public static string AVSPIPE { get { return Path.Combine(Global.Folder.Plugins, "avisynth", "avs2pipe"); } }
		public static string FFMS2 { get { return Path.Combine(Global.Folder.Plugins, "ffmsindex", "ffmsindex"); } }
		public static string MP4FPS { get { return Path.Combine(Global.Folder.Plugins, "mp4fpsmod", "mp4fpsmod"); } }

		public static string AviSynthFile { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "avisynth.dll"); } }
		public static bool IsExistAviSynth = File.Exists(AviSynthFile);
		public static bool IsForceAviSynth = false;

		public static bool IsExistHEVCGCC = false;
		public static bool IsExistHEVCICC = false;
		public static bool IsExistHEVCMSVC = false;

		public static Dictionary<Guid, Plugin> List = new Dictionary<Guid, Plugin>();

		public static void Repo()
		{
			string repo = null;
			int counter = 0;
			int counted = 0;

			if (OS.IsWindows)
				if (OS.Is64bit)
					repo = "addons_windows64.repo";
				else
					repo = "addons_windows32.repo";

			if (OS.IsLinux)
				if (OS.Is64bit)
					repo = "addons_linux64.repo";
				else
					repo = "addons_linux32.repo";

			counted = File.ReadAllLines(repo).Length;

			foreach (var item in File.ReadAllLines(repo))
			{
				string content = item.Replace("\\n", "\n");
				string[] nemu = content.Split('\n');

				counter++;

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, nemu[0])))
				{
					Console.Write($"\nDownloading component {counter,2:##} of {counted,2:##}: {nemu[0]}");
					new Download().GetFileExtract(nemu[1], Global.Folder.Plugins);
				}
			}
		}

		public static void Update()
		{
			foreach (var item in List)
			{
				var obj = item.Value;
				Console.Write($"\nChecking for update: {obj.Profile.Name}");

				if (string.IsNullOrEmpty(obj.Provider.Update))
					continue;

				var version = new Download().GetString(obj.Provider.Update);

				if (string.IsNullOrEmpty(version))
					continue;

				if (string.Equals(obj.Profile.Ver, version))
					continue;

				new Download().GetFileExtract(obj.Provider.Download, Global.Folder.Plugins);
			}
		}

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
					p.Arg.Raw = data["arg"]["raw"];
					p.Arg.Input = data["arg"]["input"];
					p.Arg.Output = data["arg"]["output"];
					p.Arg.Bitrate = data["arg"]["bitrate"];
					p.Arg.Advance = data["arg"]["advance"];
				}

				List.Add(new Guid(data["info"]["guid"]), p);
			}
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
			a.Provider.Update = string.Empty;
			a.Provider.Download = string.Empty;
			a.App.Bin = string.Empty;
			a.App.Quality = new[] { "0" };
			a.App.Default = "0";
			a.Arg.Raw = string.Empty;
			a.Arg.Input = string.Empty;
			a.Arg.Output = string.Empty;
			a.Arg.Bitrate = string.Empty;
			a.Arg.Advance = string.Empty;

			List.Add(new Guid("00000000-0000-0000-0000-000000000000"), a);

			var b = new Plugin();
			b.Info.Type = "audio";
			b.Info.Support = "mp4";
			b.Profile.Name = "Passthrough (Extract all audio)";
			b.Profile.Dev = "Anime4000";
			b.Profile.Ver = Global.App.Version;
			b.Profile.Web = "https://x265.github.io/";
			b.Provider.Name = "IFME";
			b.Provider.Update = string.Empty;
			b.Provider.Download = string.Empty;
			b.App.Bin = string.Empty;
			b.App.Quality = new[] { "128", "192", "256", "384", "512", "768", "1024" };
			b.App.Default = "256";
			b.Arg.Raw = string.Empty;
			b.Arg.Input = string.Empty;
			b.Arg.Output = string.Empty;
			b.Arg.Bitrate = string.Empty;
			b.Arg.Advance = string.Empty;

			List.Add(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), b);
        }
	}
}
