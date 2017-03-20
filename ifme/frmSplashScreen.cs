using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmSplashScreen : Form
	{
		private Bitmap BMP;

		public frmSplashScreen()
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			Text = Get.AppNameLong;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			var rand = Properties.Settings.Default.SplashScreenRand;
			if (rand <= 0 || rand >= 5)
				rand = 1;

			if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "glfxut.exe")))
				BMP = Properties.Resources.SplashScreen5;
			else if (rand == 1)
				BMP = Properties.Resources.SplashScreen1;
			else if (rand == 2)
				BMP = Properties.Resources.SplashScreen2;
			else if (rand == 3)
				BMP = Properties.Resources.SplashScreen3;
			else if (rand == 4)
				BMP = Properties.Resources.SplashScreen4;
			else
				BMP = Properties.Resources.SplashScreen1;

			Properties.Settings.Default.SplashScreenRand = ++rand;
			Properties.Settings.Default.Save();
		}

		private void frmSplashScreen_Shown(object sender, EventArgs e)
		{
			new PluginLoad();

			Close();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			var gfx = e.Graphics;
			gfx.DrawImage(BMP, new Rectangle(0, 0, 854, 480));
		}
	}
}
