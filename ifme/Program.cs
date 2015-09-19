using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using static System.Console;
using static ifme.Properties.Settings;

namespace ifme
{
	class Program
	{
		public static bool ApplyUpdate = false;

		[STAThread]
		static int Main(string[] args)
		{
			// Essential Stuff
			Title = $"{Global.App.Name} Console";
			Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

			// Load settings
			SettingsLoad();

			// Upgrade settings
			SettingsUpgrade();

			// Command
			if (Command(args) == 0)
				return 0;

			// Make WinForms much pretty
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Splash Screen, loading and update
			SplashScreen();

			// Main Form
			MainForm();

			// Save settings and exit
			Default.Save();
			return 0;
		}

		static void Head()
		{
			ForegroundColor = ConsoleColor.Yellow;
			WriteLine(Global.App.NameFull);
			WriteLine($"Compiled release: {Global.App.Version}-{(OS.Is64bit ? "x64" : "x86")}-{Global.App.Type}\n");
			ResetColor();
		}

		static void DisplayHelp()
		{
			Head();
			WriteLine("Usage: ifme [OPTION...] [GUI|CLI]");
			WriteLine();
			WriteLine("Mandatory arguments to long options are mandatory for short options too.");
			WriteLine();
			WriteLine("Option:");
			WriteLine("  -h, --help                   show this help");
			WriteLine("  -r, --reset                  reset IFME configuration");
			WriteLine("  -g, --gui                    open IFME GUI (linux only)");
			WriteLine();
			WriteLine("GUI:");
			WriteLine("      --open file.xml          open IFME queue file via GUI");
			WriteLine("  -s                           skip all update checking (faster loading)");
			WriteLine();
			WriteLine("CLI:");
			WriteLine("  -i, --input file.xml         load IFME queue file via CLI");
			WriteLine("  -f                           start encoding immediately! (skip confirmation)");
			WriteLine();
			WriteLine("Option GUI & CLI are cannot combine together, CLI will implies GUI.");
			WriteLine();
			WriteLine("Report bugs to: <https://github.com/Anime4000/IFME/issues>");
			WriteLine("IFME home page: <https://x265.github.io/>");
			WriteLine("IFME fb page  : <https://fb.com/internetfriendlymediaencoder/>");
		}

		static int Command(string[] args)
		{
			string Input = string.Empty;
			bool IsHelp = false;
			bool IsForce = false;
			bool IsXterm = false;
			bool IsReset = false;

			if (args.Length > 0)
			{
				for (int i = 0; i < args.Length; i++)
				{
					for (int n = 0; n < args[i].Length; n++)
					{
						if (args[i][0] != '-')
							break;

						if (args[i][1] == '-')
							break;

						if (args[i][n] == 'h')
							IsHelp = true;

						if (args[i][n] == 'r')
							IsReset = true;

						if (args[i][n] == 'g')
							IsXterm = true;

						if (args[i][n] == 's')
							ApplyUpdate = true;

						if (args[i][n] == 'f')
							IsForce = true;

						if (args[i][n] == 'i')
							if (i < args.Length)
								Input = args[++i];
					}

					if (args[i] == "--help")
						IsHelp = true;

					if (args[i] == "--reset")
						IsReset = true;

					if (args[i] == "--gui")
						IsXterm = true;

					if (args[i] == "--input")
						if (i < args.Length)
							Input = args[++i];

					if (args[i] == "--open")
						if (i < args.Length)
							ObjectIO.FileName = args[++i];
				}
			}

			if (IsHelp)
			{
				DisplayHelp();
				return 0;
			}

			if (IsReset)
			{
				Default.Reset();
				Default.Save();

				WriteLine("Settings has been reset!");
			}

			if (!string.IsNullOrEmpty(ObjectIO.FileName))
				return 1; // Proceed with GUI

			if (!string.IsNullOrEmpty(Input))
			{
				EncodingStart(Input, IsForce);
				return 0; // Proceed with CLI (Quit after)
			}
			
			if (OS.IsLinux)
			{
				if (!IsXterm)
				{
					MessageBox.Show("Please use \"ifme-xterm\" to run, this intend for CLI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					DisplayHelp();
					return 0;
				}
			}

			return 1; // Proceed with GUI
		}

		static void SettingsLoad()
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

		static void SettingsUpgrade()
		{
			if (!string.Equals(Default.Version, Global.App.VersionRelease))
			{
				Default.Upgrade();
				Default.Version = Global.App.VersionRelease;

				if (string.IsNullOrEmpty(Default.Language))
					Default.Language = "en";

				if (OS.IsLinux)
					Default.Compiler = "gcc";
				else
					Default.Compiler = "msvc";
			}
		}

		static void SplashScreen()
		{
			Application.Run(new frmSplashScreen());
		}

		static void MainForm()
		{
			Title = $"{Global.App.Name} Console";

			Clear();
			Head();

			ForegroundColor = ConsoleColor.Green;
			WriteLine(" ________________________________________");
			Write("/ ");

			ForegroundColor = ConsoleColor.Cyan;
			Write("  All encoding activity and progress  ");

			ForegroundColor = ConsoleColor.Green;
			Write(" \\\n");
			Write("\\ ");

			ForegroundColor = ConsoleColor.Cyan;
			Write("         will be display here         ");

			ForegroundColor = ConsoleColor.Green;
			Write(" /\n");
			WriteLine(" ----------------------------------------");

			ResetColor();
			WriteLine(@"        \   ^__^");
			WriteLine(@"         \  (oo)\_______");
			WriteLine(@"            (__)\       )\/\");
			WriteLine(@"                ||----w |");
			WriteLine(@"                ||     ||");

			WriteLine();

			Application.Run(new frmMain());
		}

		static int EncodingStart(string queueFile, bool force)
		{
			Head();

			WriteLine($"Current location: {Global.Folder.App}");

			WriteLine("Loading plugins... please wait...");
			Plugin.Load();

			WriteLine("Reading queue file...");
			List<Queue> argList = ObjectIO.IsValidXml(queueFile) ?
				 ObjectIO.ReadFromXmlFile<List<Queue>>(queueFile) : 
				 ObjectIO.ReadFromBinaryFile<List<Queue>>(queueFile);

			WriteLine($"There are {argList.Count} video's in the queue file");
			if (force)
			{
				for (int i = 5; i > 0; i--)
				{
					Write($"IFME will start in {i}\r");
					Thread.Sleep(1000);
				}
			}
			else
			{
				WriteLine("List video's that will process\n");
				for (int i = 0; i < argList.Count; i++)
					WriteLine($"{i + 1,3:000}: {Path.GetFileName(argList[i].Data.File)}");

				Write("\nPress any key to begin...");
				ReadKey();
			}

			// Time entire queue
			DateTime Session = DateTime.Now;

			// Encoding process
			int id = -1;
			foreach (Queue item in argList)
			{
				id++;

				// Only checked list get encoded
				if (!item.IsEnable)
				{
					id++;
					continue;
				}

				// Remove temp file
				MediaEncoder.CleanUp();

				// AviSynth aware
				string file = item.Data.File;
				string filereal = GetStream.AviSynthGetFile(file);

				// Extract mkv embedded subtitle, font and chapter
				MediaEncoder.Extract(filereal, item);

				// Audio
				MediaEncoder.Audio(filereal, item);

				// Tell User
				WriteLine($"File: {Path.GetFileName(item.Data.File)}");
				WriteLine($"Current queue {id + 1:000} of {argList.Count}");

				// Video
				MediaEncoder.Video(file, item);

				// Mux
				MediaEncoder.Mux(item);
			}

			WriteLine(GetInfo.Duration(Session));

			return 0;
		}
	}
}
