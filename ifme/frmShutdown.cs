using System;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmShutdown : Form
	{
		public frmShutdown()
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		private void frmShutdown_Load(object sender, EventArgs e)
		{
			InitializeUX();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.ShutdownType = cboShutdown.SelectedIndex;
			Properties.Settings.Default.Save();
		}
	}
}
