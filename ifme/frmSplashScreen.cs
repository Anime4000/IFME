using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

using static System.Console;

enum DownloadType
{
	Plugin,
	Extension
}

namespace ifme
{
    public partial class frmSplashScreen : Form
	{
		WebClient client = new WebClient();
		bool finish = false;

		public frmSplashScreen()
		{
			InitializeComponent();
			Icon = Properties.Resources.ifme5;
			BackgroundImage = Global.GetRandom % 2 != 0 ? Properties.Resources.SplashScreen6A : Properties.Resources.SplashScreen6B;

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			Title = "Nemu Command Centre";
			WriteLine(@"_____   __                      ___            ________            _____ ");
			WriteLine(@"___  | / /___________ _______  __( )_______    ___  __ )_____________  /_");
			WriteLine(@"__   |/ /_  _ \_  __ `__ \  / / /|/__  ___/    __  __  |  __ \  __ \  __/");
			WriteLine(@"_  /|  / /  __/  / / / / / /_/ /   _(__  )     _  /_/ // /_/ / /_/ / /_  ");
			WriteLine(@"/_/ |_/  \___//_/ /_/ /_/\__,_/    /____/      /_____/ \____/\____/\__/  ");
			WriteLine();
			WriteLine(@"                                                           nemuserver.net");
			WriteLine();

			bgwThread.RunWorkerAsync();
		}

		private void bgwThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// App Version
#if NONSTEAM
			if (!string.Equals(Global.App.VersionRelease, client.DownloadString("https://x265.github.io/update/version.txt")))
				Global.App.NewRelease = true;
#endif

			// Setting Load
			SettingLoad();

			// Plugin 
			PluginCheck(); // check repo
			Plugin.Load(); // load to memory
			PluginUpdate(); // apply update
			Plugin.Load(); // reload

			// Profile
			Profile.Load();

			// Extension
			Extension.Load();
			Extension.CheckDefault();
			ExtensionUpdate();
			Extension.Load(); // reload

			// Check x265 compiler binary
			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265gcc")))				
				Plugin.IsExistHEVCGCC = true;

			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265icc")))				
				Plugin.IsExistHEVCICC = true;

			if (Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265msvc")))
				Plugin.IsExistHEVCMSVC = true;

			// Language
			if (!File.Exists(Path.Combine(Global.Folder.Language, $"{Properties.Settings.Default.Language}.ini")))
			{
				Properties.Settings.Default.Language = "en";
				WriteLine($"Language file {Properties.Settings.Default.Language}.ini not found, make sure file name and CODE are same");
			}
			else
			{
				Language.Display();
				WriteLine($"Loading language file: {Properties.Settings.Default.Language}.ini");
			}

			// CPU Affinity, Load previous, if none, set default all CPU
			if (string.IsNullOrEmpty(Properties.Settings.Default.CPUAffinity))
			{
				Properties.Settings.Default.CPUAffinity = TaskManager.CPU.DefaultAll(true);
				Properties.Settings.Default.Save();

				WriteLine("Applying CPU settings...");
			}

			string[] aff = Properties.Settings.Default.CPUAffinity.Split(',');
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = Convert.ToBoolean(aff[i]);
			}

			// Detect AviSynth
			if (File.Exists(Plugin.AviSynthFile))
			{
				Plugin.AviSynthInstalled = true;
				WriteLine("AviSynth detected!");
			}
			else
			{
				Plugin.AviSynthInstalled = false;
				WriteLine("AviSynth not detected!");
			}

			// Thanks to our donor
			try
			{
				WriteLine("Loading our donor list :) you can see via \"About IFME\"");
				File.WriteAllText("metauser.if", client.DownloadString("http://x265.github.io/supporter.txt"), Encoding.UTF8);
			}
			catch (Exception)
			{
				WriteLine("Sorry, cannot load something :( it seem no Internet");
			}

			// Upgrade settings
			if (!string.Equals(Properties.Settings.Default.Version, Global.App.VersionRelease))
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.Version = Global.App.VersionRelease;

				if (OS.IsLinux)
					Properties.Settings.Default.Compiler = "gcc";
				else
					Properties.Settings.Default.Compiler = "msvc";

				WriteLine("Settings has been upgraded!");
			}

			// Save all settings
			Properties.Settings.Default.Save();

			// For fun
			WriteLine("\nEstablishing battlefield control, standby!");
			System.Threading.Thread.Sleep(3000);
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		#region Settings
		void SettingLoad()
		{
			// Settings
			if (string.IsNullOrEmpty(Properties.Settings.Default.DirOutput))
				Properties.Settings.Default.DirOutput = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "IFME");

			if (!Directory.Exists(Properties.Settings.Default.DirOutput))
				Directory.CreateDirectory(Properties.Settings.Default.DirOutput);

			if (string.IsNullOrEmpty(Properties.Settings.Default.DirTemp))
				Properties.Settings.Default.DirTemp = Global.Folder.Temp;

			if (!Directory.Exists(Properties.Settings.Default.DirTemp))
				Directory.CreateDirectory(Properties.Settings.Default.DirTemp);
		}
		#endregion

		#region Plugins
		public void PluginCheck()
		{
			string repo = null;
			int counter = 0;
			int counted = 0;

			if (OS.IsWindows)
				if (OS.Is64bit)
					repo = Path.Combine(Global.Folder.AppDir, "addons_windows64.repo");
				else
					repo = Path.Combine(Global.Folder.AppDir, "addons_windows32.repo");
				
			if (OS.IsLinux)
				if (OS.Is64bit)
					repo = Path.Combine(Global.Folder.AppDir, "addons_linux64.repo");
				else
					repo = Path.Combine(Global.Folder.AppDir, "addons_linux32.repo");

			counted = File.ReadAllLines(repo).Length;

			foreach (var item in File.ReadAllLines(repo))
			{
				string content = item.Replace("\\n", "\n");
				string[] nemu = content.Split('\n');

				counter++;

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, nemu[0])))
				{
					Write($"Downloading component {counter,2:##} of {counted,2:##}: {nemu[0]}\n");
					Download(nemu[1]);
				}
			}
		}

		void PluginUpdate()
		{
			foreach (var item in Plugin.List)
			{
				Write($"Checking for update: {item.Profile.Name}\n");

				if (string.IsNullOrEmpty(item.Provider.Update))
					continue;

				if (string.Equals(item.Profile.Ver, client.DownloadString(item.Provider.Update)))
					continue;

				Download(item.Provider.Download, Global.Folder.Plugins, "update.ifx");
			}
		}
		#endregion

		void ExtensionUpdate()
		{
			foreach (var item in Extension.Items)
			{
				Write($"Checking for update: {item.Name}\n");

				if (string.IsNullOrEmpty(item.UrlVersion))
					continue;

				string version = client.DownloadString(item.UrlVersion);

				if (string.Equals(item.Version, version))
					continue;

				string link = string.Format(item.UrlDownload, version);

				Download(link, Global.Folder.Extension, "zombie.ife");
			}
		}

		void Download(string url)
		{
			Download(url, Global.Folder.Plugins, "imouto.ifx");
		}

		void Download(string url, string folder, string file)
		{
			try
			{
				finish = false;

				client.DownloadFileAsync(new Uri(url), Path.Combine(folder, file));

				while (finish == false) { /* doing nothing, just block */ }

				Extract(folder, file);
			}
			catch
			{
				WriteLine("File not found or Offline");
			}
		}

		void Extract(string dir, string file)
		{
			string unzip = Path.Combine(Global.Folder.AppDir, "7za");
			string zipfile = Path.Combine(dir, file);

			Write("Extracting... ");
			TaskManager.Run($"\"{unzip}\" x \"{zipfile}\" -y \"-o{dir}\" > {OS.Null} 2>&1");
			
			Write("Done!\n");
			File.Delete(zipfile);
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Write($"\r{((float)e.BytesReceived / e.TotalBytesToReceive):P} Completed...");
		}

		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			finish = true;
		}
	}
}
