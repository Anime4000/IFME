using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmCheckUpdate : Form
	{
		public frmCheckUpdate(string LogText)
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			FormBorderStyle = FormBorderStyle.Sizable;

            rtbLog.Text = LogText;
		}

		private void frmCheckUpdate_Load(object sender, EventArgs e)
		{
			InitializeUX();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnDownload_Click(object sender, EventArgs e)
		{
			Process.Start("http://x265.github.io/download.html");
		}
	}
}
