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
			BackgroundImage = Properties.Resources.SplashScreenPlain;
		}

		private void frmSplashScreen_Load(object sender, EventArgs e)
		{
			
		}

		private void frmSplashScreen_Shown(object sender, EventArgs e)
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
