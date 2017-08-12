using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

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
			InitializeUX();
		}

		private void cboLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboLanguage.SelectedIndex >= 0)
			{
				var id = ((KeyValuePair<string, string>)cboLanguage.SelectedItem).Key;

				if (Language.List.ContainsKey(id))
				{
					var l = Language.List[id];

					lblLanguageAuthor.Text = $"{l.AuthorName} ({l.AuthorEmail})\n{l.AuthorProfile}";
				}
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
			Properties.Settings.Default.Language = ((KeyValuePair<string, string>)cboLanguage.SelectedItem).Key;
			Properties.Settings.Default.TempDir = txtTempPath.Text;
			Properties.Settings.Default.FileNamePrefix = txtNamePrefix.Text;
			Properties.Settings.Default.FileNamePostfix = txtNamePostfix.Text;

			if (rdoNamePrefixCustom.Checked)
				Properties.Settings.Default.FileNamePrefixType = 2;
			else if (rdoNamePrefixDateTime.Checked)
				Properties.Settings.Default.FileNamePrefixType = 1;
			else
				Properties.Settings.Default.FileNamePrefixType = 0;

			if (rdoNamePostfixNone.Checked)
				Properties.Settings.Default.FileNamePostfixType = 0;
			else
				Properties.Settings.Default.FileNamePostfixType = 1;

			// Encoding
			if (rdoFFmpeg32.Checked)
				Properties.Settings.Default.FFmpegArch = 32;
			else
				Properties.Settings.Default.FFmpegArch = 64;

			Properties.Settings.Default.FrameCountOffset = (int)nudFrameCountOffset.Value;

			// Final
			Properties.Settings.Default.Save();
		}
	}
}
