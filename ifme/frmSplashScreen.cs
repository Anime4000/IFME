using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace ifme.hitoha
{
	public partial class frmSplashScreen : Form
	{
		WebClient client = new WebClient();
		private int wait = 2500;
		bool finish = false;

		public frmSplashScreen()
		{
			InitializeComponent();

			this.DoubleBuffered = true;
			this.Icon = Properties.Resources.ifme_green;
			this.Text = "Starting " + Globals.AppInfo.NameTitle;

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			lblVersion.Text = String.Format(lblVersion.Text, Globals.AppInfo.Version);

			if (OS.IsLinux)
			{
				BGThread.RunWorkerAsync();
			}
			else
			{
				this.Opacity = 0.0;
				tmrFadeIn.Start();
			}
		}

		private void BGThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// Make sure ifme temp folder is clean
			if (Directory.Exists(Globals.AppInfo.TempFolder))
				foreach (var item in Directory.GetFiles(Globals.AppInfo.TempFolder))
					File.Delete(item);

			// Move here, let another thread do his job
			Addons.Installed.Get();
			Language.Installed.Get();

			// Delete old old stuff
			if (File.Exists(Path.Combine(Globals.AppInfo.CurrentFolder, "za.dll")))
				File.Delete(Path.Combine(Globals.AppInfo.CurrentFolder, "za.dll"));

			// Get IFME version
			try
			{
				InvokeStatus("Checking version", Globals.AppInfo.NameTitle);
				Globals.AppInfo.VersionNew = client.DownloadString("http://ifme.sourceforge.net/update/version.txt");
				string[] GetVersion = Globals.AppInfo.VersionNew.Split('.');
				int[] GetVersionInt = new int[4];

				for (int x = 0; x < 4; x++)
				{
					GetVersionInt[x] = int.Parse(GetVersion[x]);
				}

				string[] NowVersion = Globals.AppInfo.Version.Split('.');
				int[] NowVersionInt = new int[4];

				for (int i = 0; i < 4; i++)
				{
					NowVersionInt[i] = int.Parse(NowVersion[i]);
				}

				for (int i = 0; i < 4; i++)
				{
					if (NowVersionInt[i] == GetVersionInt[i])
					{
						Globals.AppInfo.VersionEqual = true;
						Globals.AppInfo.VersionMsg = String.Format("{0} is Up-to-date!", Globals.AppInfo.NameShort);
						continue;
					}
					else if (NowVersionInt[i] < GetVersionInt[i])
					{
						Globals.AppInfo.VersionEqual = false;
						Globals.AppInfo.VersionMsg = String.Format("Version {0}.{1}.{2}.{3} is available! click About button to perform updates!", GetVersion);
						break;
					}
					else
					{
						Globals.AppInfo.VersionEqual = false;
						Globals.AppInfo.VersionMsg = String.Format("This version intend for private testing, public version are {0}.{1}.{2}.{3}", GetVersion);
						break;
					}
				}
			}
			catch (WebException ex)
			{
				InvokeStatus("Error", ex.Message);
				System.Threading.Thread.Sleep(wait);
				return;
			}

			// IFME have new version, dont proceed check addons verions
			if (!Globals.AppInfo.VersionEqual)
				return;

			// If use disable update, dont proceed
			if (!Properties.Settings.Default.UpdateAlways)
				return;

			// Get addons version and update
			for (int i = 0; i < Addons.Installed.Data.GetLength(0); i++)
			{
				// Stop when reach end of addons
				if (Addons.Installed.Data[i, 0] == null)
					break;

				// Get version
				try
				{
					InvokeStatus("Loading", Addons.Installed.Data[i, 2]);
					string GetVersion = client.DownloadString(Addons.Installed.Data[i, 8]);
					if (GetVersion == Addons.Installed.Data[i, 4] || GetVersion == null)
						continue;
				}
				catch
				{
					continue;
				}

				// Apply update
				try
				{
					InvokeStatus("Downloading updates", Addons.Installed.Data[i, 2]);
					client.DownloadFileAsync(new Uri(Addons.Installed.Data[i, 9]), Path.Combine(Addons.Folder, "addons.ifz"));
					finish = false;

					while (finish == false)
					{
						// block and do noting...
					}

					InvokeStatus("Updating", Addons.Installed.Data[i, 2]);
					System.IO.Directory.Delete(Addons.Installed.Data[i, 0], true);
					Addons.Extract(Path.Combine(Addons.Folder, "addons.ifz"), Addons.Folder);

					// Tell startup there are got addon update
					Addons.Installed.IsUpdated = true;
				}
				catch (WebException ex)
				{
					InvokeStatus("Error", ex.Message);
					System.Threading.Thread.Sleep(wait);

					if (ex.Status == WebExceptionStatus.ProtocolError)
						if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
							continue;

					if (ex.Status == WebExceptionStatus.ConnectFailure)
						break;
				}
			}

			// Re-run, get latest addons
			if (Addons.Installed.IsUpdated)
			{
				InvokeStatus("Initialising Addons!", "Please Wait...");
				Addons.Installed.Get();
			}
		}

		private void BGThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lblStatus.Text = "";

			if (OS.IsLinux)
				this.Close();
			else
				tmrFadeOut.Start();
		}

		private void InvokeStatus(string status, string thing)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lblStatus.Text = String.Format("{0}: {1}", status, thing)));
			else
				lblStatus.Text = String.Format("{0}: {1}", status, thing);
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			if (lblProgress.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lblProgress.Text = String.Format("{2:P}: {0} of {1} bytes completed!", e.BytesReceived, e.TotalBytesToReceive, ((double)e.BytesReceived / (double)e.TotalBytesToReceive))));
			else
				lblProgress.Text = String.Format("{2:P}: {0} of {1} bytes completed!", e.BytesReceived, e.TotalBytesToReceive, ((double)e.BytesReceived / (double)e.TotalBytesToReceive));
		}

		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (lblProgress.InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(() => lblProgress.Text = ""));
				finish = true;
			}
			else
			{
				lblProgress.Text = "";
				finish = true;
			}
		}

		private void tmrFadeIn_Tick(object sender, EventArgs e)
		{
			this.Opacity += 0.04;

			if (this.Opacity >= 1)
			{
				BGThread.RunWorkerAsync();
				tmrFadeIn.Stop();
			} 
		}

		private void tmrFadeOut_Tick(object sender, EventArgs e)
		{
			this.Opacity -= 0.04;

			if (this.Opacity <= 0)
			{
				this.Close();
			} 
		}
	}
}
