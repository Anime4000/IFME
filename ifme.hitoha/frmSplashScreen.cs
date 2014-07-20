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
using System.IO.Compression;

namespace ifme.hitoha
{
	public partial class frmSplashScreen : Form
	{
		WebClient client = new WebClient();
		private int wait = 2500;
		bool finish = false;

		public frmSplashScreen()
		{
			this.DoubleBuffered = true;
			this.Icon = Properties.Resources.ifme_green;

			InitializeComponent();

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			lblVersion.Text = String.Format(lblVersion.Text, Globals.AppInfo.Version);
			lblVersion.Parent = pictSS;
			lblStatus.Parent = pictSS;
			lblProgress.Parent = pictSS;
			this.Opacity = 0.0;

			tmrFadeIn.Start();
		}

		private void BGThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// Get addons version and update
			for (int i = 0; i < Addons.Installed.Data.GetLength(0); i++)
			{
				// Stop when reach end of addons
				if (Addons.Installed.Data[i, 0] == null)
					break;

				try
				{
					// Get version
					InvokeStatus("Checking", Addons.Installed.Data[i, 2]);
					string GetVersion = client.DownloadString(Addons.Installed.Data[i, 8]);
					if (GetVersion == Addons.Installed.Data[i, 4] || GetVersion == null)
						continue;
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.ProtocolError)
						if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
							continue;

					if (ex.Status == WebExceptionStatus.ConnectFailure)
						break;
				}

				try
				{
					InvokeStatus("Downloading updates", Addons.Installed.Data[i, 2]);
					client.DownloadFileAsync(new Uri(Addons.Installed.Data[i, 9]), Addons.Path.Folder + "\\addons.ifz");
					finish = false;

					while (finish == false)
					{
						// block and do noting...
					}

					InvokeStatus("Updating", Addons.Installed.Data[i, 2]);
					System.IO.Directory.Delete(Addons.Installed.Data[i, 0], true);
					ZipFile.ExtractToDirectory(Addons.Path.Folder + "\\addons.ifz", Addons.Path.Folder);
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

			// Get IFME version
			try
			{
				string GetVersion = client.DownloadString("http://ifme.sourceforge.net/update/version.txt");
				Globals.AppInfo.VersionNew = GetVersion;

				if (GetVersion == Globals.AppInfo.Version)
					Globals.AppInfo.VersionEqual = true; 
				else
					Globals.AppInfo.VersionEqual = false;
			}
			catch (WebException ex)
			{
				InvokeStatus("Error", ex.Message);
				System.Threading.Thread.Sleep(wait);
			}
		}

		private void BGThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			lblStatus.Text = "";
			tmrFadeOut.Start();
		}

		private void InvokeStatus(string status, string thing)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lblStatus.Text = String.Format("Status: {0}\n    ► {1}", status, thing)));
			else
				lblStatus.Text = String.Format("Status: {0}\n    ► {1}", status, thing);
		}

		void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			if (lblProgress.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lblProgress.Text = String.Format("    ► {2}%: {0} of {1} bytes completed!", e.BytesReceived, e.TotalBytesToReceive, Math.Round(((double)e.BytesReceived / (double)e.TotalBytesToReceive) * 100.0, 0))));
			else
				lblProgress.Text = String.Format("    ► {2}%: {0} of {1} bytes completed!", e.BytesReceived, e.TotalBytesToReceive, Math.Round(((double)e.BytesReceived / (double)e.TotalBytesToReceive) * 100.0, 0));
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
