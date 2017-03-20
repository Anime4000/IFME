using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace ifme
{
	public partial class frmOption : Form
	{
		public frmOption()
		{
			InitializeComponent();

			Icon = Get.AppIcon;
			FormBorderStyle = FormBorderStyle.Sizable;
		}

		private void frmOption_Load(object sender, EventArgs e)
		{
			// General
			txtTempPath.Text = Properties.Settings.Default.TempDir;
			txtNamePrefix.Text = Properties.Settings.Default.FileNamePrefix;

			if (Properties.Settings.Default.FileNamePrefixType == 0)
				rdoNamePrefixNone.Checked = true;
			else if (Properties.Settings.Default.FileNamePrefixType == 1)
				rdoNamePrefixDateTime.Checked = true;
			else
				rdoNamePrefixCustom.Checked = true;

			// Encoding
			if (Properties.Settings.Default.FFmpegArch == 32)
				rdoFFmpeg32.Checked = true;
			else
				rdoFFmpeg64.Checked = true;

			// List all plugins
			foreach (var item in Plugin.Items)
			{
				lstModule.Items.Add(new ListViewItem(new[] 
				{
					item.Value.Name,
					(item.Value.X64 ? "64bit" : "32bit"),
					item.Value.Author.Developer

				}));
			}
		}

		private void btnTempPath_Click(object sender, EventArgs e)
		{
			var GetDir = new FolderBrowserDialog();

			GetDir.Description = "Choose a empty temporary folder";
			GetDir.ShowNewFolderButton = true;
			GetDir.RootFolder = Environment.SpecialFolder.MyComputer;
			GetDir.SelectedPath = txtTempPath.Text;

			if (GetDir.ShowDialog() == DialogResult.OK)
			{
				if (GetDir.SelectedPath[0] == '\\' && GetDir.SelectedPath[1] == '\\')
				{
					MessageBox.Show("Over network not supported, please mount it as drive", "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				txtTempPath.Text = GetDir.SelectedPath;
			}
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			// Save all
			// General
			Properties.Settings.Default.TempDir = txtTempPath.Text;
			Properties.Settings.Default.FileNamePrefix = txtNamePrefix.Text;

			if (rdoNamePrefixCustom.Checked)
				Properties.Settings.Default.FileNamePrefixType = 2;
			else if (rdoNamePrefixDateTime.Checked)
				Properties.Settings.Default.FileNamePrefixType = 1;
			else
				Properties.Settings.Default.FileNamePrefixType = 0;

			// Encoding
			if (rdoFFmpeg32.Checked)
				Properties.Settings.Default.FFmpegArch = 32;
			else
				Properties.Settings.Default.FFmpegArch = 64;

			// Final
			Properties.Settings.Default.Save();
		}
	}
}
