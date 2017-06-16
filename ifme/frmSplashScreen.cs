using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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

            // Init FFmpeg
            var arch = Properties.Settings.Default.FFmpegArch;
            FFmpegDotNet.FFmpeg.Main = Path.Combine(Get.AppRootFolder, "plugin", $"ffmpeg{arch}", "ffmpeg");
            FFmpegDotNet.FFmpeg.Probe = Path.Combine(Get.AppRootFolder, "plugin", $"ffmpeg{arch}", "ffprobe");

            // Init Folder
            if (!Directory.Exists(Get.FolderTemp))
                Directory.CreateDirectory(Get.FolderTemp);

            if (!Directory.Exists(Get.FolderSave))
                Directory.CreateDirectory(Get.FolderSave);
        }

        private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}
