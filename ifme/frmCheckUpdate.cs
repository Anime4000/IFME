using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmCheckUpdate : Form
	{
		public frmCheckUpdate()
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			FormBorderStyle = FormBorderStyle.Sizable;
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
