using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace ifme
{
	public partial class frmSplashScreen : Form
	{
		public frmSplashScreen()
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Get.AppNameLong;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{

		}
		private void frmSplashScreen_Shown(object sender, EventArgs e)
		{
			bgwThread.RunWorkerAsync();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			var gfx = e.Graphics;
			gfx.DrawImage(Branding.SplashScreen(), new Rectangle(0, 0, 854, 480));
		}

		private void bgwThread_DoWork(object sender, DoWorkEventArgs e)
		{
			// Init
			Language.Load();
			Plugin.Load();
			MediaPreset.Load();
			AviSynth.Load();

			// Init FFmpeg
			var arch = Properties.Settings.Default.FFmpegArch;
			FFmpegDotNet.FFmpeg.Main = Path.Combine(Get.AppRootDir, "plugin", $"ffmpeg{arch}", "ffmpeg");
			FFmpegDotNet.FFmpeg.Probe = Path.Combine(Get.AppRootDir, "plugin", $"ffmpeg{arch}", "ffprobe");

			// Init Folder
			if (Get.IsValidPath(Get.FolderTemp))
			{
				if (!Directory.Exists(Get.FolderTemp))
					Directory.CreateDirectory(Get.FolderTemp);
			}
			else
			{
				Get.FolderTemp = Path.Combine(Path.GetTempPath(), "IFME");

				if (!Directory.Exists(Get.FolderTemp))
					Directory.CreateDirectory(Get.FolderTemp);
			}
			
			if (Get.IsValidPath(Get.FolderSave))
			{
				if (!Directory.Exists(Get.FolderSave))
					Directory.CreateDirectory(Get.FolderSave);
			}
			else
			{
				var path = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

				// windows xp
				if (path.IsDisable())
					path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

				Get.FolderSave = Path.Combine(path, "Encoded");

				if (!Directory.Exists(Get.FolderSave))
					Directory.CreateDirectory(Get.FolderSave);
			}
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}
	}
}
