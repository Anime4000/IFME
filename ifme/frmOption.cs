using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using IniParser;
using IniParser.Model;

using static ifme.Properties.Settings;

namespace ifme
{
	public partial class frmOption : Form
	{
		StringComparison IC = StringComparison.OrdinalIgnoreCase;

		public frmOption()
		{
			InitializeComponent();
			Icon = Properties.Resources.control_equalizer_blue;

			btnBrowse.Image = Properties.Resources.folder_explore;
		}

		private void frmOption_Load(object sender, EventArgs e)
		{
			// Language UI (cannot put bottom due dynamic label)
#if MAKELANG
			LangCreate();
#else
			LangApply();
#endif

			// General
			txtTempFolder.Text = Default.DirTemp;
			txtNamePrefix.Text = Default.NamePrefix;
			chkSoundDone.Checked = Default.SoundFinish;

			// FFmpeg 64bit
			chkFFmpeg64.Enabled = OS.Is64bit;
			chkFFmpeg64.Checked = Default.UseFFmpeg64;
			grpFFmpeg.Text = "&FFmpeg";
			chkFFmpeg64.Text = "FFmpeg &64bit *";

			// Load CPU stuff
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				if (i >= 2)
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
				else
					clbCPU.Items.Add("CPU " + (i + 1).ToString(), TaskManager.CPU.Affinity[i]);
			}

			cboCPUPriority.SelectedIndex = Default.Nice;

			// AviSynth
			lblAviSynthStatus.Text = Plugin.IsExistAviSynth ? Language.Installed : Language.NotInstalled;
			lblAviSynthStatus.ForeColor = Plugin.IsExistAviSynth ? Color.Green : Color.Red;

			if (Plugin.IsExistAviSynth)
			{
				if (string.Equals(CRC32.GetFile(Plugin.AviSynthFile), "0x073A3318"))
				{
					lblAviSynthStatus.Text += ", 2.6 MT (32bit, 2015.02.20)";
				}
				else if (string.Equals(CRC32.GetFile(Plugin.AviSynthFile), "0x30E0D263"))
				{
					lblAviSynthStatus.Text += ", 2.6 ST (32bit, Original)";
				}
				else
				{
					lblAviSynthStatus.Text += " (Unknown)";
				}
			}

			txtAvsDecoder.Text = Default.AvsDecoder;
			chkCopyContentMKV.Checked = Default.AvsMkvCopy;

			// Plugin
			foreach (var item in Plugin.List)
			{
				ListViewItem x = new ListViewItem(new[] {
					$"{item.Value.Profile.Name}{(OS.Is64bit ? $" {(item.Value.Profile.Arch == 32 ? "*32" : "")}" : "")}",
					item.Value.Profile.Ver,
					item.Value.Profile.Dev,
					item.Value.Provider.Name
				});

				x.Tag = item.Value.Profile.Web;

				lstPlugin.Items.Add(x);
			}

			// Extension
			foreach (var item in Extension.Items)
			{
				ListViewItem x = new ListViewItem(new[] {
					$"{item.Name} ({item.FileName})",
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
				if (((string)cboDefaultEditor.Items[i]).Contains(Default.DefaultNotepad))
				{
					cboDefaultEditor.SelectedIndex = i;
					break; // stop found default notepad
				}
			}

			for (int i = 0; i < cboDefaultBenchmark.Items.Count; i++)
			{
				if (((string)cboDefaultBenchmark.Items[i]).Contains(Default.DefaultBenchmark))
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
			if (string.Equals(Default.Compiler, "gcc", IC))
				rdoCompilerGCC.Checked = true;

			if (string.Equals(Default.Compiler, "icc", IC))
				rdoCompilerIntel.Checked = true;

			if (string.Equals(Default.Compiler, "msvc", IC))
				rdoCompilerMicrosoft.Checked = true;				

			if (!Plugin.IsExistHEVCGCC)
				rdoCompilerGCC.Enabled = false;

			if (!Plugin.IsExistHEVCICC)
				rdoCompilerIntel.Enabled = false;

			if (!Plugin.IsExistHEVCMSVC)
				rdoCompilerMicrosoft.Enabled = false;

			// Language List
			foreach (var item in Language.Lists)
			{
				cboLang.Items.Add(item.Name);

				if (string.Equals(Default.Language, item.Code))
					cboLang.Text = item.Name;
			}
		}

		private void cboLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboLang.SelectedIndex >= 0)
			{
				lblLangWho.Text = Language.Lists[cboLang.SelectedIndex].Author;
				lblLangWhoWeb.Text = Language.Lists[cboLang.SelectedIndex].Contact;
			}
		}

		private void lblLangWhoWeb_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(lblLangWhoWeb.Text))
				Process.Start(lblLangWhoWeb.Text);
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
					Default.DirTemp = GetFolder.SelectedPath;
				}
			}
		}

		private void txtNamePrefix_TextChanged(object sender, EventArgs e)
		{
			Default.NamePrefix = txtNamePrefix.Text;
		}

		private void chkSoundDone_CheckedChanged(object sender, EventArgs e)
		{
			Default.SoundFinish = chkSoundDone.Checked;
		}

		private void cboDefaultEditor_SelectedIndexChanged(object sender, EventArgs e)
		{
			string item = cboDefaultEditor.Text;
			Default.DefaultNotepad = item.Substring(item.IndexOf('(') + 1).Replace(")", "");
		}

		private void cboDefaultBenchmark_SelectedIndexChanged(object sender, EventArgs e)
		{
			string item = cboDefaultBenchmark.Text;
			Default.DefaultBenchmark = item.Substring(item.IndexOf('(') + 1).Replace(")", "");
		}

		private void txtAvsDecoder_TextChanged(object sender, EventArgs e)
		{
			Default.AvsDecoder = txtAvsDecoder.Text;
		}

		private void chkCopyContentMKV_CheckedChanged(object sender, EventArgs e)
		{
			Default.AvsMkvCopy = chkCopyContentMKV.Checked;
		}

		private void rdoCompilerGCC_CheckedChanged(object sender, EventArgs e)
		{
			Default.Compiler = "gcc";
		}

		private void rdoCompilerIntel_CheckedChanged(object sender, EventArgs e)
		{
			Default.Compiler = "icc";
		}

		private void rdoCompilerMicrosoft_CheckedChanged(object sender, EventArgs e)
		{
			Default.Compiler = "msvc";
		}

		private void tsmiPluginWeb_Click(object sender, EventArgs e)
		{
			if (lstPlugin.SelectedItems.Count == 1)
				if(!string.IsNullOrEmpty((string)lstPlugin.SelectedItems[0].Tag))
					Process.Start((string)lstPlugin.SelectedItems[0].Tag);
		}

		private void tsmiExtensionWeb_Click(object sender, EventArgs e)
		{
			if (lstExtension.SelectedItems.Count == 1)
				if (!string.IsNullOrEmpty((string)lstExtension.SelectedItems[0].Tag))
					Process.Start((string)lstExtension.SelectedItems[0].Tag);
		}

		private void tsmiProfileWeb_Click(object sender, EventArgs e)
		{
			if (lstProfile.SelectedItems.Count == 1)
				if (!string.IsNullOrEmpty((string)lstProfile.SelectedItems[0].Tag))
					Process.Start((string)lstProfile.SelectedItems[0].Tag);
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (chkReset.Checked)
			{
				Default.Reset();
				Default.Save();
				return;
			}

			// Save CPU affinity
			string aff = "";
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = clbCPU.GetItemChecked(i);
				aff += clbCPU.GetItemChecked(i).ToString() + ",";
			}
			aff = aff.Remove(aff.Length - 1);
			Default.CPUAffinity = aff;
			Default.Nice = cboCPUPriority.SelectedIndex;

			// Language
			if (cboLang.SelectedIndex >= 0)
				Default.Language = Language.Lists[cboLang.SelectedIndex].Code;
			else
				Default.Language = "en";

			// Temp Folder (again)
			Default.DirTemp = txtTempFolder.Text;

			// FFmpeg 64bit
			Default.UseFFmpeg64 = chkFFmpeg64.Checked;

			// Save
			Default.Save();

			// Test, make sure it can run
			TaskManager.Run($"\"{Plugin.HEVC08}\" --help > {OS.Null} 2>&1");
        }

		void LangApply()
		{
			var data = Language.Get;

			Text = data[Name]["title"];

			Control ctrl = this;
			do
			{
				ctrl = GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is RadioButton ||
						ctrl is GroupBox)
						if (!string.IsNullOrEmpty(ctrl.Text))
							ctrl.Text = data[Name][ctrl.Name].Replace("\\n", "\n");

			} while (ctrl != null);

			tsmiPluginWeb.Text = data[Name]["VisitWeb"];
			tsmiExtensionWeb.Text = data[Name]["VisitWeb"];
			tsmiProfileWeb.Text = data[Name]["VisitWeb"];

			foreach (ColumnHeader item in lstPlugin.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			foreach (ColumnHeader item in lstExtension.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			foreach (ColumnHeader item in lstProfile.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			Language.Installed = data[Name]["Installed"];
			Language.NotInstalled = data[Name]["NotInstalled"];
		}

		void LangCreate()
		{
			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Path.Combine(Global.Folder.Language, "en.ini"));

			data.Sections[Name].AddKey("title", Text);

			data.Sections.AddSection(Name);
			Control ctrl = this;
			do
			{
				ctrl = GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is RadioButton ||
						ctrl is GroupBox)
						if (!string.IsNullOrEmpty(ctrl.Text))
							data.Sections[Name].AddKey(ctrl.Name, ctrl.Text.Replace("\n", "\\n").Replace("\r", ""));

			} while (ctrl != null);

			data.Sections[Name].AddKey($"VisitWeb", "Visit &Website");

			foreach (ColumnHeader item in lstPlugin.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			foreach (ColumnHeader item in lstExtension.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			foreach (ColumnHeader item in lstProfile.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			data.Sections[Name].AddKey("Installed", Language.Installed);
			data.Sections[Name].AddKey("NotInstalled", Language.NotInstalled);

			parser.WriteFile(Path.Combine(Global.Folder.Language, "en.ini"), data);
		}
	}
}
