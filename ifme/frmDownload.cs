using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmDownload : Form
	{
		WebClient client = new WebClient();

		Stopwatch stopwatch;
		long ctime = 0, ptime = 0, current = 0, previous = 0;

		public string Url;
		public string SavePath;

		public frmDownload(string url, string savepath)
		{
			InitializeComponent();
			Icon = Properties.Resources.drive_network;

			Url = url;
			SavePath = savepath;

			client.DownloadProgressChanged += client_DownloadProgressChanged;
			client.DownloadFileCompleted += client_DownloadFileCompleted;
		}

		private void frmDownload_Load(object sender, EventArgs e)
		{
			lblFile.Text = $"URL: {Url}";
            lblSave.Text = $"Save: {SavePath}";
		}

		private void frmDownload_Shown(object sender, EventArgs e)
		{
			stopwatch = new Stopwatch();
			client.DownloadFileAsync(new Uri(Url), SavePath);
			stopwatch.Start();
		}

		private void frmDownload_FormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				client.CancelAsync();
			}
			catch
			{
				// wut
			}
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

			// correction
			speed = speed > 0 ? speed : 0;

			InvokeStatus($"{FileSize(e.BytesReceived)} of {FileSize(e.TotalBytesToReceive)} ({e.BytesReceived / e.TotalBytesToReceive:P}) @ {speed} KB/s");
			InvokeProgress(e.ProgressPercentage);
		}

		void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		void InvokeStatus(string s)
		{
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lblStatus.Text = s));
			else
				lblStatus.Text = s;
		}

		void InvokeProgress(int i)
		{
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => pbDownload.Value = i));
			else
				pbDownload.Value = i;
		}

		string FileSize(long value)
		{
			long byteCount = value;

			string[] IEC = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
			if (byteCount == 0)
				return "0" + IEC[0];

			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);

            return $"{(Math.Sign(byteCount) * num):0.00} {IEC[place]}";
        }
	}
}