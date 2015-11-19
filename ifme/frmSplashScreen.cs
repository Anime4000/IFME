using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Threading;

using static ifme.Properties.Settings;

enum DownloadType
{
	Plugin,
	Extension
}

namespace ifme
{
	public partial class frmSplashScreen : Form
	{
		public frmSplashScreen()
		{
			InitializeComponent();

			Icon = Properties.Resources.ifme5;
			BackgroundImage = Global.GetRandom % 2 != 0 ? Properties.Resources.SplashScreen5CA : Properties.Resources.SplashScreen5CB;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			bgwThread.RunWorkerAsync();
		}

		private void bgwThread_DoWork(object sender, DoWorkEventArgs e)
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

#if !STEAM
			Console.Write("Checking version...");
			string version = new Download().GetString("https://x265.github.io/update/version.txt");
			Global.App.NewRelease = string.IsNullOrEmpty(version) ? false : string.Equals(Global.App.VersionRelease, version) ? false : true;
			Console.WriteLine($"{(Global.App.NewRelease ? "New version is available to download!" : "This is latest version!")}");
#endif

			// Profile
			Profile.Load();

			// Plugin 
			Plugin.Repo(); // check repo
			Plugin.Load(); // load to memory
			if (!Program.ApplyUpdate) { Plugin.Update(); Plugin.Load(); }

			// Extension
			Extension.Load();
			Extension.CheckDefault();
			if (!Program.ApplyUpdate) { Extension.Update(); Extension.Load(); }

			// Language
			if (!File.Exists(Path.Combine(Global.Folder.Language, $"{Default.Language}.ini")))
			{
				Default.Language = "en";
				Console.WriteLine($"\nLanguage file {Default.Language}.ini not found, make sure file name and CODE are same");
			}
			Language.Display();
			Console.WriteLine($"\nLoading language file: {Default.Language}.ini");

			// Detect AviSynth
			if (Plugin.IsExistAviSynth)
				Console.WriteLine("AviSynth detected!");
			else
				Console.WriteLine("AviSynth not detected!");

			// Fetch latest file
			if (!Program.ApplyUpdate)
			{
				// Format fix
				Console.WriteLine("Fetch codec fingerprint");
				new Download().GetFile("https://github.com/Anime4000/IFME/raw/master/ifme/format.ini", "format.ini");

				// AviSynth filter, allow IFME to find real file
				Console.WriteLine("Fetch AviSynth filter");
				new Download().GetFile("https://github.com/Anime4000/IFME/raw/master/ifme/avisynthsource.code", "avisynthsource.code");

				// Thanks to our donor
				Console.WriteLine("Fetch our donor list :) you can see via \"About IFME\"");
				new Download().GetFile("http://x265.github.io/supporter.txt", "metauser.if");
			}

			// Save all settings
			Default.Save();

			// For fun
			Console.WriteLine("\nEstablishing battlefield control, standby!");
			Thread.Sleep(3000);
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}
	}
}
