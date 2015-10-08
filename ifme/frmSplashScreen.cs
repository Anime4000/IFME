using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Net;

using static System.Console;
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

			// Profile
			Profile.Load();

			// Check IFME if got new update
			VersionCheck();

			// Plugin 
			PluginCheck(); // check repo
			Plugin.Load(); // load to memory
			if (!Program.ApplyUpdate) { PluginUpdate(); Plugin.Load(); }

			// Extension
			Extension.Load();
			Extension.CheckDefault();
			if (!Program.ApplyUpdate) { ExtensionUpdate(); Extension.Load(); }

			// Language
			if (!File.Exists(Path.Combine(Global.Folder.Language, $"{Default.Language}.ini")))
			{
				Default.Language = "en";
				WriteLine($"Language file {Default.Language}.ini not found, make sure file name and CODE are same");
			}
			Language.Display();
			WriteLine($"Loading language file: {Default.Language}.ini");

			// Detect AviSynth
			if (Plugin.IsExistAviSynth)
				WriteLine("AviSynth detected!");
			else
				WriteLine("AviSynth not detected!");

			// Format fix
			WriteLine("Loading codec fingerprint");
			Download("https://github.com/Anime4000/IFME/raw/master/ifme/format.ini", Path.Combine(Global.Folder.App, "format.ini"));

			// AviSynth filter, allow IFME to find real file
			WriteLine("Loading AviSynth filter");
			Download("https://github.com/Anime4000/IFME/raw/master/ifme/avisynthsource.code", Path.Combine(Global.Folder.App, "avisynthsource.code"));

			// Thanks to our donor
			WriteLine("Loading our donor list :) you can see via \"About IFME\"");
			Download("http://x265.github.io/supporter.txt", Path.Combine(Global.Folder.App, "metauser.if"));

			// Save all settings
			Default.Save();

			// For fun
			WriteLine("\nEstablishing battlefield control, standby!");
			System.Threading.Thread.Sleep(3000);
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}

		private void VersionCheck()
		{
#if !STEAM
			if (!string.Equals(Global.App.VersionRelease, DownloadString("https://x265.github.io/update/version.txt")))
				Global.App.NewRelease = true;
#endif
		}

		#region Plugins
		public void PluginCheck()
		{
			string repo = null;
			int counter = 0;
			int counted = 0;

			if (OS.IsWindows)
				if (OS.Is64bit)
					repo = Path.Combine(Global.Folder.App, "addons_windows64.repo");
				else
					repo = Path.Combine(Global.Folder.App, "addons_windows32.repo");
				
			if (OS.IsLinux)
				if (OS.Is64bit)
					repo = Path.Combine(Global.Folder.App, "addons_linux64.repo");
				else
					repo = Path.Combine(Global.Folder.App, "addons_linux32.repo");

			counted = File.ReadAllLines(repo).Length;

			foreach (var item in File.ReadAllLines(repo))
			{
				string content = item.Replace("\\n", "\n");
				string[] nemu = content.Split('\n');

				counter++;

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, nemu[0])))
				{
					Write($"Downloading component {counter,2:##} of {counted,2:##}: {nemu[0]}\n");
					DownloadExtract(nemu[1], Global.Folder.Plugins);
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

				if (string.Equals(item.Profile.Ver, DownloadString(item.Provider.Update)))
					continue;

				DownloadExtract(item.Provider.Download, Global.Folder.Plugins);
			}
		}
		#endregion

		void ExtensionUpdate()
		{
			foreach (var item in Extension.Items)
			{
				string version = string.Empty;
				string link = string.Empty;

                Write($"Checking for update: {item.Name}\n");

				if (string.IsNullOrEmpty(item.UrlVersion))
					continue;

				version = DownloadString(item.UrlVersion);

				if (string.Equals(item.Version, version ?? "0"))
					continue;
				
				link = string.Format(item.UrlDownload, version);

				DownloadExtract(link, Global.Folder.Extension);
			}
		}

		void Download(string url, string fileout)
		{
			string filetemp = $"{fileout}.tmp";

			try
			{
				client.DownloadFile(url, filetemp);
			}
			catch (Exception)
			{
				LogError($"Problem when trying to download: {fileout}");
			}
			finally
			{
				if (File.Exists(filetemp))
				{
					if (File.Exists(fileout))
					{
						File.Delete(fileout);
						File.Move(filetemp, fileout);
					}
					else
					{
						File.Move(filetemp, fileout);
					}
				}
			}
		}

		string DownloadString(string url)
		{
			// Due to mono broken WebClient.DownloadString, using this method
			try
			{
				client.DownloadFile(url, Path.Combine(Global.Folder.DefaultTemp, "_string.txt"));
				
				if (File.Exists(Path.Combine(Global.Folder.DefaultTemp, "_string.txt")))
					return File.ReadAllText(Path.Combine(Global.Folder.DefaultTemp, "_string.txt"));
				else
					return null;
			}
			catch (Exception)
			{
				LogError("WebClient.DownloadString() broken on current version of Mono, skipping...");
				return null;
			}
		}

		void DownloadExtract(string url, string targetFolder)
		{
			string tempFile = Path.Combine(Global.Folder.DefaultTemp, "package.ifa");

            try
			{
				finish = false;

				client.DownloadFileAsync(new Uri(url), tempFile);

				while (!finish) { /* doing nothing, just block stuff */ }

				Extract(tempFile, targetFolder);
			}
			catch (Exception)
			{
				LogError("File not found or Offline");
			}
		}

		void Extract(string archiveFile, string targetFolder)
		{
			string zipApp = Path.Combine(Global.Folder.App, "7za");

			Write($"Extracting...");

			if (File.Exists($"{(OS.IsLinux ? zipApp : $"{zipApp}.exe")}"))
				TaskManager.Run($"\"{zipApp}\" x \"{archiveFile}\" -y \"-o{targetFolder}\" > {OS.Null} 2>&1");
			else
				Write($"File {zipApp} not found...");

			Write("Done!\n");
			File.Delete(archiveFile);
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Write($"\r{((float)e.BytesReceived / e.TotalBytesToReceive):P} Completed...");
		}

		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			finish = true;
		}

		void LogError(string message)
		{
			ForegroundColor = ConsoleColor.Red;
			Write("ERROR: ");
			ResetColor();
			Write($"{message}\n");
		}
	}
}
