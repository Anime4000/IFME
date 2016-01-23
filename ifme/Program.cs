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
		[STAThread]
		static int Main(string[] args)
		{
			// Console Title, Intro
			ConsoleHeader();

			// Make WinForms much pretty
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			// Modify current working directory, portable!
			Environment.CurrentDirectory = Global.Folder.Root;

			// Never comma under decimal/floating points
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

			// Load settings
			StartUp.SettingsLoad();

			// Upgrade settings
			StartUp.SettingsUpgrade();

			// Command
			if (Command(args) == 0)
				return 0; // finish with console mode

			// Splash Screen, loading and update
			SplashScreen();

			// Main Form
			MainForm();

			// Clean temp file
			MediaEncoder.CleanUp();

			// Save settings and exit
			Default.Save();
			return 0;
		}

		static void ConsoleHeader()
		{
			Title = $"{Global.App.Name} Console";

			ForegroundColor = ConsoleColor.Yellow;
			WriteLine(Global.App.NameFull);
			WriteLine("Release: " + Global.App.VersionCompiled);
			WriteLine();
			ResetColor();
		}

		static void DisplayHelp()
		{
			WriteLine("Usage: ifme [OPTION...] -i INPUT");
			WriteLine();
			WriteLine("Mandatory arguments to long options are mandatory for short options too.");
			WriteLine();
			WriteLine("Option:");
			WriteLine("  -h, --help                   show this help");
			WriteLine("  -r, --reset                  reset IFME configuration");
			WriteLine("  -g, --gui                    open IFME GUI (linux only)");
			WriteLine("  -c, --cli                    open IFME CLI (impiles -g)");
			WriteLine("  -i, --input file.xml         load IFME queue file via CLI");
			WriteLine("  -s                           skip all update checking (faster loading)");
			WriteLine("  -f                           start encoding immediately! (skip confirmation)");
			WriteLine();
			WriteLine("Option GUI & CLI are cannot combine together, CLI will implies GUI.");
			WriteLine();
			WriteLine("IFME home page: <https://x265.github.io/>");
			WriteLine("Report bugs to: <https://github.com/Anime4000/IFME/issues>");
			WriteLine("Facebook page : <https://fb.com/internetfriendlymediaencoder/>");
		}

		static int Command(string[] args)
		{
			bool IsHelp = false;
			bool IsForce = false;
			bool IsReset = false;
			bool IsCLI = false;
			bool IsGUI = false;

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
							IsGUI = true;

						if (args[i][n] == 'c')
							IsCLI = true;

						if (args[i][n] == 's')
							StartUp.SkipUpdate = true;

						if (args[i][n] == 'f')
							IsForce = true;

						if (args[i][n] == 'i')
							if (i < args.Length)
								ObjectIO.FileName = args[++i];
					}

					if (args[i] == "--help")
						IsHelp = true;

					if (args[i] == "--reset")
						IsReset = true;

					if (args[i] == "--gui")
						IsGUI = true;

					if (args[i] == "--cli")
						IsCLI = true;

					if (args[i] == "--input")
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

			if (IsCLI)
			{
				EncodingStart(ObjectIO.FileName, IsForce);
				return 0;
			}
			else
			{
				if (OS.IsLinux)
				{
					if (!IsGUI)
					{
						MessageBox.Show("Under linux, please use \"ifme-xterm\" to run, this intend for CLI.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						DisplayHelp();
						return 0;
					}
				}
			}

			return 1; // Proceed with GUI
		}

		static void SplashScreen()
		{
			Application.Run(new frmSplashScreen());
		}

		static void MainForm()
		{
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

#if DEBUG
			ForegroundColor = ConsoleColor.Red;
			Write("WARNING! ");
			ForegroundColor = ConsoleColor.White;
			Write("Experimental build may contain bug, report if found, use with caution.\n");
			ResetColor();
#endif

			Application.Run(new frmMain());
		}

		static int EncodingStart(string queueFile, bool force)
		{
			WriteLine("Loading plugins... Please Wait...");
			Plugin.Load();

			WriteLine("Reading queue file...");
			List<Queue> argList = ObjectIO.IsValidXml(queueFile) ?
				 ObjectIO.ReadFromXmlFile<List<Queue>>(queueFile) : 
				 ObjectIO.ReadFromBinaryFile<List<Queue>>(queueFile);

			WriteLine("Removing file not exist");
			for (int i = 0; i < argList.Count; i++)
			{
				if (!File.Exists(argList[i].FilePath))
				{
					WriteLine($"File Not Found: {argList[i].FilePath}");
					argList.RemoveAt(i);
				}
			}

			WriteLine($"There are {argList.Count} video's in the queue file\n");
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
				WriteLine("No. Status File");
				WriteLine("--- ------ ----");

				for (int i = 0; i < argList.Count; i++)
				{
					WriteLine($"{i + 1,3:000} {(argList[i].IsEnable ? "Queued" : " Skip ")} {Path.GetFileName(argList[i].FilePath)}");
				}

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

				// Current media
				string file = item.FilePath;

				// Extract mkv embedded subtitle, font and chapter
				MediaEncoder.Extract(item);

				// Audio
				MediaEncoder.Audio(item);

				// Tell User
				WriteLine($"File: {Path.GetFileName(item.FilePath)}");
				WriteLine($"Current queue {id + 1} of {argList.Count}");

				// Video
				MediaEncoder.Video(item);

				// Mux
				MediaEncoder.Mux(item);

				// Save
				WriteLine("Saving current queue... Please Wait...");
				item.IsEnable = false;
				ObjectIO.WriteToXmlFile(queueFile, argList);
            }

			// Tell user
			WriteLine(Get.Duration(Session));

			return 0;
		}
	}
}
