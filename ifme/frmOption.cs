using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ifme.imouto;

namespace ifme
{
	public partial class frmOption : Form
	{
		StringComparison IC = StringComparison.OrdinalIgnoreCase;

		public frmOption()
		{
			InitializeComponent();
			Icon = imouto.Properties.Resources.control_equalizer_blue;

			btnBrowse.Image = imouto.Properties.Resources.folder_explore;
		}

		private void frmOption_Load(object sender, EventArgs e)
		{
			// General
			txtTempFolder.Text = Properties.Settings.Default.DirTemp;
			txtNamePrefix.Text = Properties.Settings.Default.NamePrefix;
			chkSoundDone.Checked = Properties.Settings.Default.SoundFinish;

			// Load CPU stuff
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				if (i >= 2)
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
				else
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
			}

			cboCPUPriority.SelectedIndex = Properties.Settings.Default.Nice;

			// AviSynth
			lblAviSynthStatus.Text = Plugin.AviSynthInstalled ? "Installed" : "Not found";
			lblAviSynthStatus.ForeColor = Plugin.AviSynthInstalled ? Color.Green : Color.Red;

			if (Plugin.AviSynthInstalled)
			{
				if (string.Equals(imouto.CRC32.GetFile(Plugin.AviSynthFile), "0x 73A3318"))
				{
					lblAviSynthStatus.Text += ", 2.6 MT (2015.02.20)";
				}
				else
				{
					lblAviSynthStatus.Text += " (Unknown)";
				}
			}

			chkCopyContentMKV.Checked = Properties.Settings.Default.AvsMkvCopy;

			// Plugin
			foreach (var item in Plugin.List)
			{
				ListViewItem x = new ListViewItem(new[] { 
					item.Profile.Name,
					item.Profile.Ver,
					item.Profile.Dev,
					item.Provider.Name
				});

				x.Tag = item.Profile.Web;

				lstPlugin.Items.Add(x);
			}

			// Extension
			foreach (var item in Extension.Items)
			{
				ListViewItem x = new ListViewItem(new[] {
					string.Format("{0} ({1})", item.Name, item.FileName),
					item.Type,
					item.Version,
					item.Developer
				});

				x.Tag = item.UrlWeb;

				lstExtension.Items.Add(x);

				// List all default extension
				if (string.Equals(item.Type, "notepad", IC))
					cboDefaultEditor.Items.Add($"{item.Name} ({item.FileName})");
				else if (string.Equals(item.Type, "benchmark", IC))
					cboDefaultBenchmark.Items.Add($"{item.Name} ({item.FileName})");
			}

			for (int i = 0; i < cboDefaultEditor.Items.Count; i++)
			{
				if (((string)cboDefaultEditor.Items[i]).Contains(Properties.Settings.Default.DefaultNotepad))
				{
					cboDefaultEditor.SelectedIndex = i;
					break; // stop found default notepad
				}
			}

			for (int i = 0; i < cboDefaultBenchmark.Items.Count; i++)
			{
				if (((string)cboDefaultBenchmark.Items[i]).Contains(Properties.Settings.Default.DefaultBenchmark))
				{
					cboDefaultBenchmark.SelectedIndex = i;
					break; // stop found default notepad
				}
			}

			// Profile
			foreach (var item in Profile.List)
			{
				ListViewItem x = new ListViewItem(new[] {
					item.Info.Name,
					item.Info.Format,
					item.Info.Platform,
					item.Info.Author
				});

				x.Tag = item.Info.Web;

				lstProfile.Items.Add(x);
			}

			// Compiler
			if (string.Equals(Properties.Settings.Default.Compiler, "icc", IC))
				rdoCompilerIntel.Checked = true;
			else if (string.Equals(Properties.Settings.Default.Compiler, "msvc", IC))
				rdoCompilerMicrosoft.Checked = true;
			else
				rdoCompilerGCC.Checked = true;

			if (OS.IsLinux)
			{
				rdoCompilerIntel.Enabled = false;
				rdoCompilerMicrosoft.Enabled = false;
			}
		}

		private void frmOption_Shown(object sender, EventArgs e)
		{/*
			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Path.Combine("lang", "eng.ini"));

			data.Sections.AddSection("frmOption");
			Control ctrl = this;
			do
			{
				ctrl = this.GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is GroupBox)
						if (!String.IsNullOrEmpty(ctrl.Text))
							data.Sections["frmOption"].AddKey(ctrl.Name, ctrl.Text.Replace("\n", "\\n"));

			} while (ctrl != null);

			parser.WriteFile(Path.Combine("lang", "eng.ini"), data, Encoding.UTF8);*/
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog GetFolder = new FolderBrowserDialog();

			GetFolder.Description = "";
			GetFolder.ShowNewFolderButton = true;
			GetFolder.RootFolder = Environment.SpecialFolder.MyComputer;

			if (!string.IsNullOrEmpty(txtTempFolder.Text))
			{
				GetFolder.SelectedPath = txtTempFolder.Text;
			}

			if (GetFolder.ShowDialog() == DialogResult.OK)
			{
				if (Directory.EnumerateFileSystemEntries(GetFolder.SelectedPath).Any())
				{
					MessageBox.Show("Please choose an empty folder", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					txtTempFolder.Text = GetFolder.SelectedPath;
					Properties.Settings.Default.DirTemp = GetFolder.SelectedPath;
					Properties.Settings.Default.Save();
				}
			}
		}

		private void txtNamePrefix_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.NamePrefix = txtNamePrefix.Text;
			Properties.Settings.Default.Save();
		}

		private void chkSoundDone_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.SoundFinish = chkSoundDone.Checked;
			Properties.Settings.Default.Save();
		}

		private void cboDefaultEditor_SelectedIndexChanged(object sender, EventArgs e)
		{
			string item = cboDefaultEditor.Text;
			Properties.Settings.Default.DefaultNotepad = item.Substring(item.IndexOf('(') + 1).Replace(")", "");
			Properties.Settings.Default.Save();
		}

		private void cboDefaultBenchmark_SelectedIndexChanged(object sender, EventArgs e)
		{
			string item = cboDefaultBenchmark.Text;
			Properties.Settings.Default.DefaultBenchmark = item.Substring(item.IndexOf('(') + 1).Replace(")", "");
			Properties.Settings.Default.Save();
		}

		private void cboDefaultHfr_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void chkCopyContentMKV_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.AvsMkvCopy = chkCopyContentMKV.Checked;
			Properties.Settings.Default.Save();
		}

		private void lblHFR_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.spirton.com/interframe/");
		}

		private void rdoCompilerGCC_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.Compiler = "gcc";
		}

		private void rdoCompilerIntel_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.Compiler = "icc";
		}

		private void rdoCompilerMicrosoft_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.Compiler = "msvc";
		}

		private void tsmiPluginWeb_Click(object sender, EventArgs e)
		{
			if (lstPlugin.SelectedItems.Count == 1)
			{
				Process.Start((string)lstPlugin.SelectedItems[0].Tag);
			}
		}

		private void tsmiExtensionWeb_Click(object sender, EventArgs e)
		{
			if (lstExtension.SelectedItems.Count == 1)
			{
				Process.Start((string)lstExtension.SelectedItems[0].Tag);
			}
		}

		private void tsmiProfileWeb_Click(object sender, EventArgs e)
		{
			if (lstProfile.SelectedItems.Count == 1)
			{
				Process.Start((string)lstProfile.SelectedItems[0].Tag);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			// Save CPU affinity
			string aff = "";
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = clbCPU.GetItemChecked(i);
				aff += clbCPU.GetItemChecked(i).ToString() + ",";
			}
			aff = aff.Remove(aff.Length - 1);
			Properties.Settings.Default.CPUAffinity = aff;
			Properties.Settings.Default.Nice = cboCPUPriority.SelectedIndex;

			// Save
			Properties.Settings.Default.Save();

			// Compiler
			Plugin.HEVCL = Path.Combine(Global.Folder.Plugins, "x265" + Properties.Settings.Default.Compiler, "x265lo");
			Plugin.HEVCH = Path.Combine(Global.Folder.Plugins, "x265" + Properties.Settings.Default.Compiler, "x265hi");
		}
	}
}
