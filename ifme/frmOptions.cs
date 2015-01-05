using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ifme.hitoha;
using IniParser;
using IniParser.Model;

namespace ifme.hitoha
{
	public partial class frmOptions : Form
	{
		public frmOptions()
		{
			InitializeComponent();
			this.Icon = Properties.Resources.ifme_flat;

			// Fix linux drawing
			if (OS.IsLinux)
			{
				if (this.Width < 800)
					this.Width = 800;
				if (this.Height < 600)
					this.Height = 600;
			}
		}

		private void LoadLang()
		{
			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Path.Combine(Language.Folder, Language.Default + ".ini"));

			Control ctrl = this;
			do
			{
				ctrl = this.GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is RadioButton ||
						ctrl is GroupBox)
						if (ctrl.Text.Contains('{'))
							if (data[Language.Section.Ops][ctrl.Name].Contains('|'))
								ctrl.Text = String.Format(ctrl.Text, data[Language.Section.Ops][ctrl.Name].Split('|'));
							else
								ctrl.Text = String.Format(ctrl.Text, data[Language.Section.Ops][ctrl.Name]);

			} while (ctrl != null);

			this.Text = String.Format(this.Text, data[Language.Section.Ops]["TheTitle"]);
			lstAddons.Columns[0].Text = data[Language.Section.Ops]["colID"];
			lstAddons.Columns[1].Text = data[Language.Section.Ops]["colName"];
			lstAddons.Columns[2].Text = data[Language.Section.Ops]["colVer"];
			lstAddons.Columns[3].Text = data[Language.Section.Ops]["colDev"];
			lstAddons.Columns[4].Text = data[Language.Section.Ops]["colProvider"];

			if (OS.IsLinux)
				tabPerformance.Text += " (require root)";
		}

		private void GetUserLang()
		{
			cboLang.Items.Clear();

			// List langauge in drop down
			for (int i = 0; i < Language.Installed.Data.GetLength(0); i++)
			{
				if (Language.Installed.Data[i, 0] != null)
				{
					foreach (var x in System.IO.File.ReadAllLines(Globals.Files.ISO))
					{
						if (Language.Installed.Data[i, 0] == x.Substring(0, 3))
						{
							string c = x.Substring(5, x.Length - 6);
							cboLang.Items.Add(c);
						}
					}
				}
			}

			// Get user default language
			for (int i = 0; i < Language.Installed.Data.GetLength(0); i++)
			{
				if (Language.Installed.Data[i, 0] != null)
				{
					if (Properties.Settings.Default.DefaultLang == Language.Installed.Data[i, 0])
					{
						cboLang.Text = Language.Installed.Data[i, 1];
					}
				}
			}
		}

		private void GetAddonsList()
		{
			// Clear
			lstAddons.Items.Clear();

			Addons.Installed.Get();

			// Display installed addons
			for (int i = 0; i < Addons.Installed.Data.GetLength(0); i++)
			{
				if (Addons.Installed.Data[i, 0] != null)
				{
					ListViewItem lst = new ListViewItem(i.ToString());
					lst.SubItems.Add(Addons.Installed.Data[i, 2]);
					lst.SubItems.Add(Addons.Installed.Data[i, 4].Contains("//") ? "No Data" : Addons.Installed.Data[i, 4]);
					lst.SubItems.Add(Addons.Installed.Data[i, 3].Contains("//") ? "No Data" : Addons.Installed.Data[i, 3]);
					lst.SubItems.Add(Addons.Installed.Data[i, 7]);
					lstAddons.Items.Add(lst);
				}
			}
		}

		private void frmOptions_Load(object sender, EventArgs e)
		{
			// Display Language
			LoadLang();
			GetUserLang();
			GetAddonsList();

			// Load stuff
			txtTempDir.Text = Properties.Settings.Default.TemporaryFolder;
			rdoUseMkv.Checked = Properties.Settings.Default.UseMkv;
			rdoUseMp4.Checked = !Properties.Settings.Default.UseMkv;
			chkUpdate.Checked = Properties.Settings.Default.UpdateAlways;
			chkLogSave.Checked = Properties.Settings.Default.LogAutoSave;

			// Load CPU stuff
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				if (i >= 2)
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
				else
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
			}

			cboPerf.SelectedIndex = Properties.Settings.Default.Nice;

			if (cboLang.SelectedIndex == -1)
				cboLang.SelectedIndex = 0;
		}

		private void cboLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cboLang.SelectedIndex;
			lblLangWho.Text = Language.Installed.Data[i, 2]
				+ "\n" + Language.Installed.Data[i, 3]
				+ "\n" + Language.Installed.Data[i, 4];
		}

		private void btnTempFindFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog GetFolder = new FolderBrowserDialog();

			GetFolder.Description = Language.IMessage.OpenFolder;
			GetFolder.ShowNewFolderButton = true;
			GetFolder.RootFolder = Environment.SpecialFolder.MyComputer;

			if (!String.IsNullOrEmpty(txtTempDir.Text))
			{
				GetFolder.SelectedPath = txtTempDir.Text;
			}

			if (GetFolder.ShowDialog() == DialogResult.OK)
			{
				if (Directory.EnumerateFileSystemEntries(GetFolder.SelectedPath).Any())
				{
					MessageBox.Show(Language.IMessage.NotEmptyFolder, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				else
				{
					txtTempDir.Text = GetFolder.SelectedPath;
				}
			}
		}

		private void btnCPUBoost_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				clbCPU.SetItemChecked(i, false);
			}

			for (int i = Environment.ProcessorCount - 1; i >= Environment.ProcessorCount / 2; --i)
			{
				clbCPU.SetItemChecked(i, true);
			}
		}

		private void btnAddonInstall_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFile = new OpenFileDialog();

			GetFile.Title = Language.IMessage.OpenFile;
			GetFile.Filter = "IFME Addon File|*.ifz;*.aruuie;*.danny";
			GetFile.FilterIndex = 1;

			GetFile.RestoreDirectory = true;

			if (GetFile.ShowDialog() == DialogResult.OK)
			{
				try
				{
					Addons.Extract(GetFile.FileName, Addons.Folder);
					MessageBox.Show(Language.IMessage.InstallMsg, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

					// Get new addon into action
					Addons.Installed.Get();
					GetAddonsList();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message);
					return;
				}
			}
		}

		private void btnLangAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFile = new OpenFileDialog();

			GetFile.Title = Language.IMessage.OpenFile; ;
			GetFile.Filter = "Supported IFME UI Language|*.ifl;*.ooi;*.ini|"
				+ "IFME UI Language|*.ifl;*.ooi|"
				+ "Configuration settings|*.ini";
			GetFile.FilterIndex = 1;
			GetFile.Multiselect = true;

			GetFile.RestoreDirectory = true;

			try
			{
				var L = Language.Installed.Data;
				var I = Language.Section.Info;
				if (GetFile.ShowDialog() == DialogResult.OK)
				{
					foreach (var item in GetFile.FileNames)
					{
						var parser = new FileIniDataParser();
						IniData data = parser.ReadFile(item);

						string ID = data[I]["iso"];

						System.IO.File.Copy(item, Path.Combine(Language.Folder, ID + ".ini"), false);

						for (int i = 0; i < L.GetLength(0); i++)
						{
							if (L[i, 0] == null)		/* Insert new installed language */
							{
								L[i, 0] = System.IO.Path.GetFileNameWithoutExtension(item);
								L[i, 1] = data[I]["iso"] + " (Just installed, choose and restart to take effect)";
								L[i, 2] = data[I]["Name"];
								L[i, 3] = data[I]["Version"];
								L[i, 4] = data[I]["Contact"];

								cboLang.Items.Add(L[i, 1]);

								break;					/* Stop right here! */
							}
						}
					}
					MessageBox.Show(Language.IMessage.InstallMsg, "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void btnAddonRemove_Click(object sender, EventArgs e)
		{
			int i = lstAddons.SelectedItems[0].Index;

			if (lstAddons.Items[i].SubItems[4].Text.Contains("Build-in"))
			{
				MessageBox.Show(Language.IMessage.RemoveMsgErr, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			else
			{
				var msgbox = MessageBox.Show(Language.IMessage.RemoveMsg, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (msgbox == DialogResult.Yes)
				{
					string[] temp = Addons.Installed.Data[i, 0].Split('|');
					for (int h = 0; h < lstAddons.SelectedItems.Count; h++)
					{
						lstAddons.Items.Remove(lstAddons.SelectedItems[h]);
					}
					System.IO.Directory.Delete(temp[1], true);

					// remove selected row of array by bring next array to current and nulled last array
					for (int j = i; j < Addons.Installed.Data.GetLength(0) - 1; j++)
					{
						for (int k = 0; k < Addons.Installed.Data.GetLength(1); k++)
						{
							if (Addons.Installed.Data[j + 1, k] != null)
								Addons.Installed.Data[j, k] = Addons.Installed.Data[j + 1, k];
							else
								Addons.Installed.Data[j, k] = null;
						}
					}
				}
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			string temp = Properties.Settings.Default.DefaultLang;

			// Save stuff
			Properties.Settings.Default.DefaultLang = Language.Installed.Data[cboLang.SelectedIndex, 0];
			Properties.Settings.Default.TemporaryFolder = txtTempDir.Text;
			Properties.Settings.Default.UseMkv = rdoUseMkv.Checked;
			Properties.Settings.Default.UpdateAlways = chkUpdate.Checked;
			Properties.Settings.Default.LogAutoSave = chkLogSave.Checked;

			// Save CPU affinity
			string aff = "";
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = clbCPU.GetItemChecked(i);
				aff += clbCPU.GetItemChecked(i).ToString() + ",";
			}
			aff = aff.Remove(aff.Length - 1);
			Properties.Settings.Default.CPUAffinity = aff;
			Properties.Settings.Default.Nice = cboPerf.SelectedIndex;

			// Save settings
			Properties.Settings.Default.Save();

			if (temp != Properties.Settings.Default.DefaultLang)
			{
				Options.RestartNow = true;
			}

			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnResetSettings_Click(object sender, EventArgs e)
		{
			var msg = MessageBox.Show(Language.IMessage.ResetSettingsAsk, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (msg == DialogResult.Yes)
			{
				Options.ResetDefault = true;
				Options.RestartNow = true;
				this.Close();
			}
		}
	}

	class Options
	{
		public static bool RestartNow = false;
		public static bool ResetDefault = false;
	}
}