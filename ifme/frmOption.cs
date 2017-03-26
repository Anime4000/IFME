using System;
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
			txtNamePostfix.Text = Properties.Settings.Default.FileNamePostfix;

			if (Properties.Settings.Default.FileNamePrefixType == 0)
				rdoNamePrefixNone.Checked = true;
			else if (Properties.Settings.Default.FileNamePrefixType == 1)
				rdoNamePrefixDateTime.Checked = true;
			else
				rdoNamePrefixCustom.Checked = true;

			if (Properties.Settings.Default.FileNamePostfixType == 0)
				rdoNamePostfixNone.Checked = true;
			else
				rdoNamePostfixCustom.Checked = true;

			// Encoding
			if (Properties.Settings.Default.FFmpegArch == 32)
				rdoFFmpeg32.Checked = true;
			else
				rdoFFmpeg64.Checked = true;

			if (AviSynth.IsInstalled)
			{
				lblAviSynthInstall.Text = "Installed!";
				lblAviSynthVersion.Text = AviSynth.InstalledVersion;
			}

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

			// Final
			Properties.Settings.Default.Save();
		}
	}
}
