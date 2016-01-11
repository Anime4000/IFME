using System;
using System.IO;
using System.Threading;

using static ifme.Properties.Settings;

namespace ifme
{
	public class StartUp
	{
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
			if (!Program.ApplyUpdate)
			{
				// Plugins
				Plugin.Update();
				Plugin.Load();

				// Extension
				Extension.Update();
				Extension.Load();

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
				Console.WriteLine("AviSynth detected!");
				Console.ResetColor();
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("AviSynth not detected!");
				Console.ResetColor();
			}

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

			// For fun
			Console.WriteLine("\nEstablishing battlefield control, standby!");
			Thread.Sleep(1000);
		}
	}
}
