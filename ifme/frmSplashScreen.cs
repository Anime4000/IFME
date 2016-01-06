using System;
using System.ComponentModel;
using System.Windows.Forms;

enum DownloadType
{
	Plugin,
	Extension
}

namespace ifme
{
	public partial class frmSplashScreen : Form
	{
		public frmSplashScreen()
		{
			InitializeComponent();

			Icon = Properties.Resources.ifme_zenui;
			BackgroundImage = Global.GetRandom % 2 != 0 ? Properties.Resources.SplashScreen5CA : Properties.Resources.SplashScreen5CB;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			bgwThread.RunWorkerAsync();
		}

		private void bgwThread_DoWork(object sender, DoWorkEventArgs e)
		{
			StartUp.RunSetting();
			StartUp.RunUpdate();
			StartUp.RunFinal();
		}

		private void bgwThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Close();
		}
	}
}
