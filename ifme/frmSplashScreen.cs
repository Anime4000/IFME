using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmSplashScreen : Form
	{
		public frmSplashScreen()
		{
			InitializeComponent();
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{

		}

		private void frmSplashScreen_Shown(object sender, EventArgs e)
		{
			new PluginLoad();

			Close();
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			var gfx = e.Graphics;
			gfx.DrawImage(Properties.Resources.SplashScreen, new Rectangle(0, 0, 854, 480));
		}
	}
}
