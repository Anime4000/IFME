using System;
using System.IO;
using System.Threading;

using static ifme.Properties.Settings;

namespace ifme
{
	public class StartUp
	{
		public static bool SkipUpdate { get; set; } = false;

		public static void SettingsLoad()
		{
			// Output folder
			if (string.IsNullOrEmpty(Default.DirOutput))
				Default.DirOutput = Global.Folder.DefaultSave;

			if (!Directory.Exists(Default.DirOutput))
				Directory.CreateDirectory(Default.DirOutput);

			// Temporary folder
			if (string.IsNullOrEmpty(Default.DirTemp))
				Default.DirTemp = Global.Folder.DefaultTemp;

			if (!Directory.Exists(Default.DirTemp))
				Directory.CreateDirectory(Default.DirTemp);

			// CPU Affinity, Load previous, if none, set default all CPU
			if (string.IsNullOrEmpty(Default.CPUAffinity))
			{
				Default.CPUAffinity = TaskManager.CPU.DefaultAll(true);
				Default.Save();
			}

			string[] aff = Default.CPUAffinity.Split(',');
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = Convert.ToBoolean(aff[i]);
			}
		}

		public static void SettingsUpgrade()
		{
			if (!string.Equals(Default.Version, Global.App.VersionRelease))
			{
				Default.Upgrade();
				Default.Version = Global.App.VersionRelease;

				if (string.IsNullOrEmpty(Default.Language))
					Default.Language = "en";

				// Compiler
				if (string.IsNullOrEmpty(Default.Compiler))
				{
					if (OS.IsWindows)
					{
						if (OS.Is64bit)
						{
							Default.Compiler = "gcc";
						}
						else
						{
							Default.Compiler = "msvc";
						}
					}
					else
					{
						Default.Compiler = "gcc";
					}
				}
			}
		}

		public static void RunSetting()
		{
			// Check x265, just incase user remove folder
			if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, $"x265{Default.Compiler}")))
			{
				if (OS.IsWindows)
				{
					if (OS.Is64bit)
					{
						Default.Compiler = "gcc";
					}
					else
					{
						Default.Compiler = "msvc";
					}
				}
				else
				{
					Default.Compiler = "gcc";
				}
			}

			// Check x265 compiler binary
			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265gcc")))
				Plugin.IsExistHEVCGCC = true;

			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265icc")))
				Plugin.IsExistHEVCICC = true;

			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265msvc")))
				Plugin.IsExistHEVCMSVC = true;

			// Check FFmpeg 64bit, make sure not enable in 32bit OS
			if (!OS.Is64bit)
				if (Default.UseFFmpeg64)
					Default.UseFFmpeg64 = false;
        }

		public static void RunLoad()
		{
			// Profile
			Profile.Load();

			// Plugin 
			Plugin.Repo(); // check repo
			Plugin.Load(); // load to memory

			// Extension
			Extension.Load();
			Extension.CheckDefault();
		}

		public static void RunUpdate()
		{
			RunLoad();

			// Fetch latest file
			if (!SkipUpdate)
			{
#if !STEAM
				// Update check
				Console.WriteLine("Checking new version, please wait...");
				string version = new Download().GetString("https://x265.github.io/update/version.txt");
				Global.App.NewRelease = string.IsNullOrEmpty(version) ? false : string.Equals(Global.App.VersionRelease, version) ? false : true;
#endif
				// Plugins
				Plugin.Update();

				// Extension
				Extension.Update();

				// Format fix
				Console.WriteLine("\nFetch codec fingerprint from GitHub Repo");
				new Download().GetFile("https://github.com/Anime4000/IFME/raw/master/ifme/format.ini", "format.ini");

				// AviSynth filter, allow IFME to find real file
				Console.WriteLine("Fetch AviSynth filter from GitHub Repo");
				new Download().GetFile("https://github.com/Anime4000/IFME/raw/master/ifme/avisynthsource.code", "avisynthsource.code");

				// Thanks to our donor
				Console.WriteLine("Fetch our donor list :) you can see via \"About IFME\"");
				new Download().GetFile("http://x265.github.io/supporter.txt", "metauser.if");
			}
			else
			{
				// Tell
				Console.WriteLine("\nSkipping update checks!");
			}
		}

		public static void RunFinal()
		{
			// Detect AviSynth
			if (Plugin.IsExistAviSynth)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("AviSynth was found, enabled!");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("AviSynth not found, disabled!");
			}

			// Tell FFmpeg default
			if (Default.UseFFmpeg64)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Using 64-bit FFmpeg & AviSynth");
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Using 32-bit FFmpeg & AviSynth");
			}

			Console.ResetColor();

			// Language
			if (!File.Exists(Path.Combine(Global.Folder.Language, $"{Default.Language}.ini")))
			{
				Default.Language = "en";
				Console.WriteLine($"Language file {Default.Language}.ini not found, make sure file name and CODE are same");
			}
			Language.Display();
			Console.WriteLine($"Loading language file: {Default.Language}.ini");

			// Save all settings
			Default.Save();
		}
	}
}
