using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;

using IniParser;
using IniParser.Model;

using ifme.imouto;

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

		Stopwatch stopwatch;
		long ctime = 0, ptime = 0, current = 0, previous = 0;

		public frmSplashScreen()
		{
			InitializeComponent();
			this.Icon = Properties.Resources.ifme5;
			this.BackgroundImage = Properties.Resources.SplashScreenB;

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			Console.Title = "Nemu Bootstrap";
			Console.WriteLine(@"_____   __                      ___            ________            _____ ");
			Console.WriteLine(@"___  | / /___________ _______  __( )_______    ___  __ )_____________  /_");
			Console.WriteLine(@"__   |/ /_  _ \_  __ `__ \  / / /|/__  ___/    __  __  |  __ \  __ \  __/");
			Console.WriteLine(@"_  /|  / /  __/  / / / / / /_/ /   _(__  )     _  /_/ // /_/ / /_/ / /_  ");
			Console.WriteLine(@"/_/ |_/  \___//_/ /_/ /_/\__,_/    /____/      /_____/ \____/\____/\__/  ");
			Console.WriteLine();
			Console.WriteLine(@"                                                           nemuserver.net");
			Console.WriteLine();

			bgwThread.RunWorkerAsync();
		}

		private void bgwThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// CPU Affinity, Load previous, if none, set default all CPU
			if (String.IsNullOrEmpty(Properties.Settings.Default.CPUAffinity))
			{
				Properties.Settings.Default.CPUAffinity = TaskManager.CPU.DefaultAll(true);
				Properties.Settings.Default.Save();
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
			}
			else
			{
				Plugin.AviSynthInstalled = false;
			}

			// Thanks to our donor
			try
			{
				File.WriteAllText("metauser.if", client.DownloadString("http://x265.github.io/supporter.txt"), Encoding.UTF8);
			}
			catch (Exception)
			{
				Console.Write("Sorry, cannot load something :( it seem no Internet");
			}

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

			// App Version
#if NONSTEAM
			if (!String.Equals(Global.App.VersionRelease, client.DownloadString("https://x265.github.io/update/version_ifme5.txt")))
				Global.App.NewRelease = true;
#endif

			// For fun
			Console.WriteLine("\n\nAll done! Initialising...");
			System.Threading.Thread.Sleep(3000);
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.Close();
		}

		#region Settings
		void SettingLoad()
		{
			// Settings
			if (String.IsNullOrEmpty(Properties.Settings.Default.DirOutput))
				Properties.Settings.Default.DirOutput = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "IFME");

			if (!Directory.Exists(Properties.Settings.Default.DirOutput))
				Directory.CreateDirectory(Properties.Settings.Default.DirOutput);

			if (String.IsNullOrEmpty(Properties.Settings.Default.DirTemp))
				Properties.Settings.Default.DirTemp = Global.Folder.Temp;

			if (!Directory.Exists(Properties.Settings.Default.DirTemp))
				Directory.CreateDirectory(Properties.Settings.Default.DirTemp);
		}
		#endregion

		#region Plugins
		public void PluginCheck()
		{
			// Check folder
			if (OS.IsWindows)
			{
				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "ffmpeg")))
				{
					Console.WriteLine("Downloading component  1 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/ffmpeg.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "avisynth")))
				{
					Console.WriteLine("Downloading component  2 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/avisynth.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "ffmsindex")))
				{
					Console.WriteLine("Downloading component  3 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/ffmsindex.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "mp4fpsmod")))
				{
					Console.WriteLine("Downloading component  4 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/mp4fpsmod.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "mkvtool")))
				{
					Console.WriteLine("Downloading component  5 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/mkvtool.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "mp4box")))
				{
					Console.WriteLine("Downloading component  6 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/mp4box.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265gcc")))
				{
					Console.WriteLine("Downloading component  7 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/x265gcc.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265icc")))
				{
					Console.WriteLine("Downloading component  8 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/x265icc.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "x265msvc")))
				{
					Console.WriteLine("Downloading component  9 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/x265msvc.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "flac")))
				{
					Console.WriteLine("Downloading component 10 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/flac.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "ogg")))
				{
					Console.WriteLine("Downloading component 11 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/ogg.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "opus")))
				{
					Console.WriteLine("Downloading component 12 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/opus.ifx");
				}

				if (!Directory.Exists(Path.Combine(Global.Folder.Plugins, "faac")))
				{
					Console.WriteLine("Downloading component 13 of 13");
					Download("http://master.dl.sourceforge.net/project/ifme/plugins/ifme5/windows/faac.ifx");
				}
			}
			else
			{
				// coming soon
			}
		}

		void PluginUpdate()
		{
			foreach (var item in Plugin.List)
			{
				Console.Write("\nChecking for update: {0}", item.Profile.Name);

				if (String.IsNullOrEmpty(item.Provider.Update))
					continue;

				if (String.Equals(item.Profile.Ver, client.DownloadString(item.Provider.Update)))
					continue;

				Download(item.Provider.Download, Global.Folder.Plugins, "update.ifx");
			}
		}
		#endregion

		void ExtensionUpdate()
		{
			foreach (var item in Extension.Items)
			{
				Console.Write("\nChecking for update: {0}", item.Name);

				if (String.IsNullOrEmpty(item.UrlVersion))
					continue;

				string version = client.DownloadString(item.UrlVersion);

				if (String.Equals(item.Version, version))
					continue;

				string link = String.Format(item.UrlDownload, version);

				Download(link, Global.Folder.Extension, "zombie.ife");
			}
		}

		void Download(string url)
		{
			Download(url, Global.Folder.Plugins, "imouto.ifx");
		}

		void Download(string url, string folder, string file)
		{
			Console.Write("\n");

			try
			{
				finish = false;

				stopwatch = new Stopwatch();
				client.DownloadFileAsync(new Uri(url), Path.Combine(folder, file));
				stopwatch.Start();

				while (finish == false) { /* doing nothing, just block */ }

				Extract(folder, file);
			}
			catch
			{
				Console.WriteLine("File not found or Offline");
			}
		}

		void Extract(string dir, string file)
		{
			string unzip = Path.Combine(Global.Folder.AppDir, "7za");
			string fullpath = Path.Combine(dir, file);

			Console.Write("\nExtracting... ");
			TaskManager.Run(String.Format("\"{0}\" x \"{1}\" -y \"-o{2}\" > {3} 2>&1", unzip, fullpath, dir, OS.Null));
			
			Console.Write("Done!\n");
			File.Delete(fullpath);
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			// Info: http://www.doyatte.com/how-to-get-the-download-speed-while-using-downloaddataasync/
			// get the elapsed time in milliseconds
			ctime = stopwatch.ElapsedMilliseconds;
			// get the received bytes at the particular instant
			current = e.BytesReceived;
			// calculate the speed the bytes were downloaded and assign it to a Textlabel (speedLabel in this instance)
			int speed = ((int)(((current - previous) / (double)1024) / ((ctime - ptime) / (double)1000)));

			previous = current;
			ptime = ctime;

			Console.Write("\r{0:P} Completed... ({1} KB/s)\t", ((double)e.BytesReceived / (double)e.TotalBytesToReceive), speed > 0 ? speed : 0);
		}

		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			finish = true;
		}
	}
}
