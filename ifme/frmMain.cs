using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
// Asset
using ifme.framework;
using IniParser;
using IniParser.Model;
using MediaInfoDotNet;

namespace ifme.hitoha
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();

			// Dynamic
			if (Globals.AppInfo.CharTheme % 2 != 0)
				pictBannerRight.Image = Properties.Resources.BannerBRight; // Odd, Ifumii
			else
				pictBannerRight.Image = Properties.Resources.BannerCRight; // Even, Hotaru

			// Form Init.
			this.Size = Properties.Settings.Default.FormSize;
			this.Icon = Properties.Resources.ifme_flat;
			pictBannerRight.Parent = pictBannerMain;

			// Fix WinForms in Linux (Mono) drawing and positioning
			if (OS.IsLinux)
			{
				rtfLog.Font = new Font("Monospace", 8, FontStyle.Regular);
				pictBannerMain.Height -= 5;
				pictBannerMain.Width += 9;
				pictBannerRight.Left += 116;

				panel1.Width += 16;
				panel1.Height += 11;
				panel1.Left += 35;
				panel1.Top += 51;

				// In UNIX, shutdown require root
				chkDoneOffMachine.Visible = false;
			}

			// Enhanced screen, some text (languages) not fit
			if (Properties.Settings.Default.FormSize.Width < 800)
				this.Width = 800;
			if (Properties.Settings.Default.FormSize.Height < 600)
				this.Height = 600;
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (BGThread.IsBusy)
			{
				e.Cancel = true;
				return;
			}

			var msgbox = MessageBox.Show(Language.IMessage.Quit, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (msgbox == DialogResult.No)
			{
				e.Cancel = true;
				return;
			}
			
			UserSettingsSave();
			
			if (OS.IsLinux)
				Console.Write("[info] Your settings has been saved, {0} exit safely\n", Globals.AppInfo.NameShort);
		}

		public void StartUpLog()
		{
			// Header
			rtfLog.SelectionColor = Color.Yellow;
			rtfLog.SelectedText = String.Format("{0} - by {1} ({2})\nVersion: {3} compiled on {4} ({5})\n\n", Globals.AppInfo.Name, Globals.AppInfo.Author, Globals.AppInfo.WebSite, Globals.AppInfo.Version, Globals.AppInfo.BuildDate, Globals.AppInfo.CPU);
			rtfLog.SelectionColor = Color.Red;
			rtfLog.SelectedText = "Warning: This program still in beta, unexpected behaviour may occur.\n";
			rtfLog.SelectionColor = Color.Aqua;

			// Linux cannot send terminal text to program, tell them!
			if (OS.IsWindows)
				rtfLog.SelectedText = "Save this log? Click here and press CTRL+S (console will be clear after save)\n\n";
			else
				rtfLog.SelectedText = "All encoding will redirect to terminal, make sure start application via termianl\n\n";
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// Startup
			StartUpLog();

			// Message StartUp
			if (Properties.Settings.Default.LogAutoSave)
				PrintLog(Log.Info, "Log will save each session!");

			// Migrate previous settings
			if (Properties.Settings.Default.UpdateSettings)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.UpdateSettings = false;
				Properties.Settings.Default.Save();

				PrintLog(Log.OK, "Previous settings has been upgraded!");
			}

			// Temp Folder
			if (String.IsNullOrEmpty(Properties.Settings.Default.TemporaryFolder))
			{
				if (!Directory.Exists(Globals.AppInfo.TempFolder))
					System.IO.Directory.CreateDirectory(Globals.AppInfo.TempFolder);

				Properties.Settings.Default.TemporaryFolder = Globals.AppInfo.TempFolder;
				Properties.Settings.Default.Save();

				PrintLog(Log.OK, "New default temporary folder created!");
			}

			// Check for updates and load addons
			PrintLog(Log.Info, "Checking for updates");
			frmSplashScreen SS = new frmSplashScreen();
			SS.ShowDialog();

			// Display that IFME has new version
			PrintLog(Log.Info, Globals.AppInfo.VersionMsg);

			// Update form title
			this.Text = Globals.AppInfo.NameTitle;

			// Load list of ISO language file to control
			try
			{
				foreach (var item in System.IO.File.ReadAllLines(Globals.Files.ISO))
				{
					cboSubLang.Items.Add(item);
				}
			}
			catch (Exception ex)
			{
				PrintLog(Log.Error, ex.Message);
			}
			
			// Get Lang
			try
			{
				PrintLog(Log.OK, Language.Installed.GetCount() + " Installed language detected!");
			}
			catch (Exception ex)
			{
				PrintLog(Log.Error, ex.Message);
			}

			// Count how many addons has been load
			try
			{
				PrintLog(Log.OK, Addons.Installed.GetCount() + " Addons has been loaded!");
			}
			catch (Exception ex)
			{
				PrintLog(Log.Error, ex.Message);
			}

			// Tell user current langauge
			PrintLog(Log.Info, String.Format("Current MUI: {0} by {1} ({2})", Language.Installed.Data[Language.GetCurrent(), 1], Language.Installed.Data[Language.GetCurrent(), 2], Language.Installed.Data[Language.GetCurrent(), 4]));

			// Detect AviSynth
			if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "avisynth.dll")))
			{
				PrintLog(Log.Error, "AviSynth is not detected, AviSynth support has been disabled!");
				Addons.Installed.AviSynth = false;
			}
			else
			{
				PrintLog(Log.OK, "AviSynth detected! Make sure you have proper AviSynth configuration, enjoy!");
				Addons.Installed.AviSynth = true;
			}
			
			//CreateLang(); // Developer tool, Scan and Create new empty language
			LoadLang(); // Load language, GUI must use {0} as place-holder

			// After addons has been load, now display it on UI
			AddAudio();

			// Then add user preset (must after load language)
			AddUserPreset(true);

			// Load Settings
			UserSettingsLoad();

			// Display console
			tabEncoding.SelectedTab = tabStatus;

			// Display News
			if (String.IsNullOrEmpty(Globals.AppInfo.News))
				PrintLog(Log.Error, "Unable to fetch latest news... so sad :(");
			else
				PrintLog(Log.Info, "Displaying latest news, read below.\n---\n" + Globals.AppInfo.News + "\n---");
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!Globals.AppInfo.VersionEqual)
			{
				btnAbout.Text = "&Update!";
				proTip.ToolTipTitle = Language.IMessage.ProTipTitle;
				proTip.Show(Language.IMessage.ProTipUpdate, this.btnAbout, 50, 18, 15000);
			}

			adjListBoxColumn();
		}

		private void frmMain_SizeChanged(object sender, EventArgs e)
		{
			adjListBoxColumn();
		}

		private void adjListBoxColumn()
		{
			// List View for Queue, fast way
			// Get size
			var lst = lstQueue.Columns;

			// We want auto size on last column
			lst[6].Width = (lstQueue.Width - 4) - (lst[0].Width + lst[1].Width + lst[2].Width + lst[3].Width + lst[4].Width + lst[5].Width);
		}

		private void chkDoneOffMachine_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.Shutdown = chkDoneOffMachine.Checked;
		}

		private void lstQueue_DoubleClick(object sender, EventArgs e)
		{
			// resue code
			btnEdit.PerformClick();
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 0)
				return;

			if (String.Equals(lstQueue.SelectedItems[0].SubItems[1].Text, ".avs", StringComparison.InvariantCultureIgnoreCase))
				return;

			// Allow user to change video resolution!
			string res = lstQueue.SelectedItems[0].SubItems[3].Text;
			using (var from = new frmProperties(res))
			{
				var result = from.ShowDialog();
				if (result == DialogResult.OK)
				{
					string val = from.NewScreenRes;
					lstQueue.SelectedItems[0].SubItems[3].Text = val;
				}
			}
		}

		private void btnPreview_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 0)
				return;

			// This data will submited, palying with code block
			Globals.Preview.Enable = true;
			Globals.Preview.Selected = lstQueue.SelectedItems[0].Index;
			Globals.Preview.File = lstQueue.SelectedItems[0].SubItems[0].Text;
			Globals.Preview.Duration = Properties.Settings.Default.PreviewDuration;
			btnStart.PerformClick(); // <- LOL
		}

		private void btnQueueEditScript_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 0)
				return;

			string file = lstQueue.SelectedItems[0].SubItems[6].Text;
			var from = new frmAviSynthEditor(file);
			from.ShowDialog();
		}

		private void btnQueueGenerate_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 0)
				return;

			string file = lstQueue.SelectedItems[0].SubItems[6].Text;
			using(var form = new frmAviSynthHFR(file))
			{
				var result = form.ShowDialog();
				if (result == DialogResult.OK)
				{
					lstQueue.SelectedItems[0].SubItems[6].Text = form.script;
					lstQueue.SelectedItems[0].SubItems[1].Text = ".avs";
				}
			}
		}

		private void lstQueue_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count > 0)
			{
				btnEdit.Enabled = true;
				btnPreview.Enabled = true;
				if (".avs" == lstQueue.SelectedItems[0].SubItems[1].Text)
				{
					btnQueueEditScript.Enabled = true;
					btnQueueGenerate.Enabled = false;
				}
				else
				{
					btnQueueEditScript.Enabled = false;
					btnQueueGenerate.Enabled = true;
				}
			}
			else
			{
				btnEdit.Enabled = false;
				btnPreview.Enabled = false;
				btnQueueEditScript.Enabled = false;
				btnQueueGenerate.Enabled = false;
			}
		}

		private void btnQueueAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Title = Language.IMessage.OpenFile;
			GetFiles.Filter = "Supported video files|*.mkv;*.mp4;*.m4v;*.mts;*.m2ts;*.flv;*.webm;*.ogv;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.avs|"
				+ "HTML5 video files|*.ogv;*.webm;*.mp4|"
				+ "WebM|*.webm|"
				+ "Theora|*.ogv|"
				+ "Matroska|*.mkv|"
				+ "MPEG-4|*.mp4;*.m4v|"
				+ "Flash Video|*.flv|"
				+ "Windows Media Video|*.wmv|"
				+ "Audio Video Interleaved|*.avi;*.divx|"
				+ "MPEG-2 Transport Stream|*.mts;*.m2ts|"
				+ "MPEG-1/DVD|*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob|"
				+ "AviSynth Script|*.avs|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (Properties.Settings.Default.LastOpenQueueLocation == "")
				GetFiles.RestoreDirectory = true;
			else
				GetFiles.InitialDirectory = Properties.Settings.Default.LastOpenQueueLocation;

			if (GetFiles.ShowDialog() == DialogResult.OK)
			{
				foreach (var item in GetFiles.FileNames)
				{
					string[] h = GetMetaData.MediaData(item);

					if (h[0] == null)	// try and catach, if media not supported or missing DLL, leaving array with null
						PrintLog(Log.Error, String.Format("File \"{0}\" not loaded:\n\t{1}", item, h[1]));

					if (h[2] == null)	// make sure required data
						continue;

					ListViewItem QueueList = new ListViewItem(h[0]);
					QueueList.SubItems.Add(h[1]);
					QueueList.SubItems.Add(h[2]);
					QueueList.SubItems.Add(h[3]);
					QueueList.SubItems.Add(h[4]);
					QueueList.SubItems.Add(h[5]);
					QueueList.SubItems.Add(h[6]);

					lstQueue.Items.Add(QueueList);

					PrintLog(Log.Info, String.Format("File \"{0}\" added via Open File (button). Format: {2} ({1}). RES: {3}. FPS: {4}. BPP: {5}", h));
				}
				Properties.Settings.Default.LastOpenQueueLocation = System.IO.Path.GetDirectoryName(GetFiles.FileName);
			}

			if (lstQueue.Items.Count != 0)
				btnStart.Enabled = true;
			else
				btnStart.Enabled = false;
		}

		private void btnQueueRemove_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstQueue.SelectedItems)
				item.Remove();

			if (lstQueue.Items.Count != 0)
				btnStart.Enabled = true;
			else
				btnStart.Enabled = false;
		}

		private void btnQueueClear_Click(object sender, EventArgs e)
		{
			lstQueue.Items.Clear();
			btnStart.Enabled = false;
			btnEdit.Enabled = false;
			btnPreview.Enabled = false;
			btnQueueEditScript.Enabled = false;
			btnQueueGenerate.Enabled = false;
		}

		private void btnQueueUp_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstQueue.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstQueue.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstQueue.Items.Count;

					if (indx == 0)
					{
						lstQueue.Items.Remove(selected);
						lstQueue.Items.Insert(totl - 1, selected);
					}
					else
					{
						lstQueue.Items.Remove(selected);
						lstQueue.Items.Insert(indx - 1, selected);
					}
				}
				else
				{
					MessageBox.Show(Language.IMessage.MoveItem, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnQueueDown_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstQueue.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstQueue.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstQueue.Items.Count;

					if (indx == totl - 1)
					{
						lstQueue.Items.Remove(selected);
						lstQueue.Items.Insert(0, selected);
					}
					else
					{
						lstQueue.Items.Remove(selected);
						lstQueue.Items.Insert(indx + 1, selected);
					}
				}
				else
				{
					MessageBox.Show(Language.IMessage.MoveItem, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void lstQueue_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstQueue_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (string file in files)
			{
				string[] h = GetMetaData.MediaData(file);

				if (h[0] == null)	// try and catach, if media not supported or missing DLL, leaving array with null
					PrintLog(Log.Error, String.Format("File \"{0}\" not loaded:\n\t{1}", file, h[1]));

				if (h[2] == null)	// make sure required data
					continue;

				ListViewItem QueueList = new ListViewItem(h[0]);
				QueueList.SubItems.Add(h[1]);
				QueueList.SubItems.Add(h[2]);
				QueueList.SubItems.Add(h[3]);
				QueueList.SubItems.Add(h[4]);
				QueueList.SubItems.Add(h[5]);
				QueueList.SubItems.Add(h[6]);

				lstQueue.Items.Add(QueueList);

				PrintLog(Log.Info, String.Format("File \"{0}\" added via Drag n Drop. Format: {2} ({1}). RES: {3}. FPS: {4}. BPP: {5}", h));
			}

			if (lstQueue.Items.Count != 0)
				btnStart.Enabled = true;
			else
				btnStart.Enabled = false;
		}

		private void btnQueueBrowseDest_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog GetDir = new FolderBrowserDialog();

			GetDir.Description = Language.IMessage.OpenFolder;
			GetDir.ShowNewFolderButton = true;
			GetDir.RootFolder = Environment.SpecialFolder.MyComputer;

			if (txtDestDir.Text != "")
			{
				GetDir.SelectedPath = txtDestDir.Text;
			}

			if (GetDir.ShowDialog() == DialogResult.OK)
			{
				txtDestDir.Text = GetDir.SelectedPath;
			}
		}

		private void chkQueueSaveTo_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.OutputDirEnable = chkQueueSaveTo.Checked;
		}

		private void txtDestDir_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.OutputDirPath = txtDestDir.Text;
		}

		private void cboUserPreList_SelectedIndexChanged(object sender, EventArgs e)
		{
			UserPreset.SelectedId = cboUserPreList.SelectedIndex;
			Properties.Settings.Default.UserPreset = UserPreset.SelectedId;
			int i = UserPreset.SelectedId;

			lblUserPreData.Text = String.Format("{0}\n{1}\n{2}", UserPreset.Installed.Data[i, 2], UserPreset.Installed.Data[i, 3], UserPreset.Installed.Data[i, 4]);

			cboVideoPreset.Text = UserPreset.Installed.Data[i, 6];
			cboVideoTune.Text = UserPreset.Installed.Data[i, 7];
			cboVideoRateCtrl.SelectedIndex = Convert.ToInt32(UserPreset.Installed.Data[i, 8]);
			txtVideoRate.Text = UserPreset.Installed.Data[i, 9];
			txtVideoAdvCmd.Text = UserPreset.Installed.Data[i, 10];

			cboAudioFormat.Text = UserPreset.Installed.Data[i, 11];
			cboAudioBitRate.Text = UserPreset.Installed.Data[i, 12];
			cboAudioFreq.Text = UserPreset.Installed.Data[i, 13];
			cboAudioChan.Text = UserPreset.Installed.Data[i, 14];
			cboAudioMode.SelectedIndex = Convert.ToInt32(UserPreset.Installed.Data[i, 15]);
			txtAudioCmd.Text = UserPreset.Installed.Data[i, 16];
		}

		private void lblUserPreData_Click(object sender, EventArgs e)
		{
			int i = UserPreset.SelectedId;
			Process.Start(UserPreset.Installed.Data[i, 4]);
		}

		private void btnUserPreSave_Click(object sender, EventArgs e)
		{
			int i = UserPreset.SelectedId;

			if (i == 0 || i == 1)
				return;

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(UserPreset.Installed.Data[i, 0]);

			data["profile"]["name"] = cboUserPreList.Text;
			data["profile"]["format"] = Properties.Settings.Default.UseMkv ? "mkv" : "mp4";

			data["video"]["preset"] = cboVideoPreset.Text;
			data["video"]["tuning"] = cboVideoTune.Text;
			data["video"]["ratectrl"] = cboVideoRateCtrl.SelectedIndex.ToString();
			data["video"]["ratefact"] = txtVideoRate.Text;
			data["video"]["command"] = txtVideoAdvCmd.Text;

			data["audio"]["encoder"] = cboAudioFormat.Text;
			data["audio"]["bit"] = cboAudioBitRate.Text;
			data["audio"]["freq"] = cboAudioFreq.Text;
			data["audio"]["channel"] = cboAudioChan.Text;
			data["audio"]["mode"] = cboAudioMode.SelectedIndex.ToString();
			data["audio"]["command"] = txtAudioCmd.Text;

			parser.WriteFile(UserPreset.Installed.Data[i, 0], data, Encoding.UTF8);

			AddUserPreset(false);
			cboUserPreList.SelectedIndex = UserPreset.SelectedId;
		}

		private void btnUserPreAdd_Click(object sender, EventArgs e)
		{
			string FileName = String.Format("{0:yyyyMMdd_HHmmss}.nemu", DateTime.Now);
			string FilePath = Path.Combine(UserPreset.Folder, FileName);
			File.WriteAllText(FilePath, Properties.Resources.TemplateUserPreset);

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(FilePath);

			data["profile"]["name"] = cboUserPreList.Text;
			data["profile"]["author"] = Environment.UserName;
			data["profile"]["version"] = String.Format("{0:yyyy.MM.dd_HHmmss}", DateTime.Now);
			data["profile"]["homepage"] = "";
			data["profile"]["format"] = Properties.Settings.Default.UseMkv ? "mkv" : "mp4";

			data["video"]["preset"] = cboVideoPreset.Text;
			data["video"]["tuning"] = cboVideoTune.Text;
			data["video"]["ratectrl"] = cboVideoRateCtrl.SelectedIndex.ToString();
			data["video"]["ratefact"] = txtVideoRate.Text;
			data["video"]["command"] = txtVideoAdvCmd.Text;

			data["audio"]["encoder"] = cboAudioFormat.Text;
			data["audio"]["bit"] = cboAudioBitRate.Text;
			data["audio"]["freq"] = cboAudioFreq.Text;
			data["audio"]["channel"] = cboAudioChan.Text;
			data["audio"]["mode"] = cboAudioMode.SelectedIndex.ToString();
			data["audio"]["command"] = txtAudioCmd.Text;

			parser.WriteFile(FilePath, data, Encoding.UTF8);

			AddUserPreset(false);
			cboUserPreList.SelectedIndex = UserPreset.SelectedId;
		}

		private void btnUserPreDelete_Click(object sender, EventArgs e)
		{
			int i = UserPreset.SelectedId;
			File.Delete(UserPreset.Installed.Data[i, 0]);

			AddUserPreset(false);
			cboUserPreList.SelectedIndex = 0;
		}

		private void cboVideoRateCtrl_SelectedIndexChanged(object sender, EventArgs e)
		{
			var i = cboVideoRateCtrl.SelectedIndex;
			if (i == 0)
			{
				//txtVideoRate.ReadOnly = true;
				lblVideoRateFH.Visible = true;
				lblVideoRateFL.Visible = true;
				trkVideoRate.Visible = true;

				trkVideoRate.Minimum = 0;
				trkVideoRate.Maximum = 510;
				trkVideoRate.TickFrequency = 10;

				lblVideoRateFactor.Text = "Ratefactor:";
				txtVideoRate.Text = Convert.ToString((trkVideoRate.Value = 260) / 10);
			}
			else if (i == 1)
			{
				//txtVideoRate.ReadOnly = true;
				lblVideoRateFH.Visible = true;
				lblVideoRateFL.Visible = true;
				trkVideoRate.Visible = true;

				trkVideoRate.Minimum = 0;
				trkVideoRate.Maximum = 51;
				trkVideoRate.TickFrequency = 1;

				lblVideoRateFactor.Text = "Ratefactor:";
				txtVideoRate.Text = Convert.ToString(trkVideoRate.Value = 26);
			}
			else
			{
				txtVideoRate.ReadOnly = !true;
				lblVideoRateFH.Visible = !true;
				lblVideoRateFL.Visible = !true;
				trkVideoRate.Visible = !true;

				lblVideoRateFactor.Text = "Bit-rate (kbps):";
				txtVideoRate.Text = "2048";
			}
		}

		private void trkVideoRate_ValueChanged(object sender, EventArgs e)
		{
			var i = cboVideoRateCtrl.SelectedIndex;
			if (i == 0)
				txtVideoRate.Text = Convert.ToString(Convert.ToDouble(trkVideoRate.Value) * 0.1);
			else
				txtVideoRate.Text = Convert.ToString(trkVideoRate.Value);
		}

		private void txtVideoRate_TextChanged(object sender, EventArgs e)
		{
			var i = cboVideoRateCtrl.SelectedIndex;

			if (i == 0)
				if (!String.IsNullOrEmpty(txtVideoRate.Text))
					if (Convert.ToDouble(txtVideoRate.Text) > 51.0)
						txtVideoRate.Text = "51";
					else if (Convert.ToDouble(txtVideoRate.Text) < 0.0)
						txtVideoRate.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(Convert.ToDouble(txtVideoRate.Text) * (double)10.0);
				else
					trkVideoRate.Value = 0;
			else if (i == 1)
				if (!String.IsNullOrEmpty(txtVideoRate.Text))
					if (Convert.ToInt32(txtVideoRate.Text) > 51)
						txtVideoRate.Text = "51";
					else if (Convert.ToInt32(txtVideoRate.Text) < 0)
						txtVideoRate.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(txtVideoRate.Text);
				else
					trkVideoRate.Value = 0;
		}

		private void txtVideoRate_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
			{
				e.Handled = true;
			}

			// only allow one decimal point
			if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
			{
				e.Handled = true;
			}
		}

		private void cboVideoPreset_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoPreset = cboVideoPreset.Text;
		}

		private void cboVideoTune_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoTune = cboVideoTune.Text;
		}

		private void cboVideoRateCtrl_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoRateType = cboVideoRateCtrl.SelectedIndex;
		}

		private void txtVideoAdvCmd_TextChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoCmd = txtVideoAdvCmd.Text;
		}

		private void cboAudioFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cboAudioFormat.SelectedIndex;
			cboAudioMode.Enabled = i == 0 ? false : true;
			txtAudioCmd.Enabled = i == 0 ? false : true;

			lblAudioFInfo.Text = Addons.Installed.Data[i, 3] + "\n"
				+ Addons.Installed.Data[i, 4] + "\n"
				+ Addons.Installed.Data[i, 5];

			txtAudioCmd.Text = Addons.Installed.Data[i, 12];

			cboAudioBitRate.Items.Clear();
			cboAudioBitRate.Items.AddRange(Addons.Installed.Data[i, 13].Split(','));
			cboAudioBitRate.Text = Addons.Installed.Data[i, 14];
		}

		private void cboAudioFormat_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioFormat = cboAudioFormat.Text;
		}

		private void cboAudioBitRate_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioBitRate = cboAudioBitRate.SelectedIndex;
		}

		private void cboAudioFreq_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioFreq = cboAudioFreq.Text;
		}

		private void cboAudioChan_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioChan = cboAudioFreq.Text;
		}

		private void cboAudioMode_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioMode = cboAudioMode.SelectedIndex;
		}

		private void chkSubEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (!Properties.Settings.Default.UseMkv && chkSubEnable.Checked)
			{
				MessageBox.Show(Language.IMessage.MKVOnly, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				chkSubEnable.Checked = false;
				return;
			}

			foreach (Control ctrl in tabSubtitle.Controls)
			{
				if (!(ctrl is CheckBox))
					ctrl.Visible = chkSubEnable.Checked;
			}
			lblSubtitleNotice.Visible = !chkSubEnable.Checked;
			chkAttachEnable.Enabled = chkSubEnable.Checked;

			if (!chkSubEnable.Checked)
				chkAttachEnable.CheckState = CheckState.Unchecked;
		}

		private void lstSubtitle_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstSubtitle_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var item in files)
			{
				// Get valid files
				if (!GetMetaData.SubtitleValid(item))
					return;

				// Get valid file
				string[] d = GetMetaData.SubtitleData(item);

				ListViewItem SubList = new ListViewItem(d[0]);
				SubList.SubItems.Add(d[1]);
				SubList.SubItems.Add(d[2]);
				SubList.SubItems.Add(d[3]);
				lstSubtitle.Items.Add(SubList);
			}
		}

		private void btnSubAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Title = Language.IMessage.OpenFile;
			GetFiles.Filter = "Supported Subtitle|*.ass;*.ssa;*.srt|"
				+ "SubStation Alpha|*.ass;*.ssa|"
				+ "SubRip|*.srt|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (Properties.Settings.Default.LastOpenSubtitleLocation == "")
				GetFiles.RestoreDirectory = true;
			else
				GetFiles.InitialDirectory = Properties.Settings.Default.LastOpenSubtitleLocation;

			if (GetFiles.ShowDialog() == DialogResult.OK)
			{
				foreach (var item in GetFiles.FileNames)
				{
					if (!GetMetaData.SubtitleValid(item))
						return;

					string[] d = GetMetaData.SubtitleData(item);

					ListViewItem SubList = new ListViewItem(d[0]);
					SubList.SubItems.Add(d[1]);
					SubList.SubItems.Add(d[2]);
					SubList.SubItems.Add(d[3]);
					lstSubtitle.Items.Add(SubList);
				}
				Properties.Settings.Default.LastOpenSubtitleLocation = System.IO.Path.GetDirectoryName(GetFiles.FileName);
			}
		}

		private void btnSubRemove_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < lstSubtitle.SelectedItems.Count; i++)
			{
				lstSubtitle.Items.Remove(lstSubtitle.SelectedItems[i]);
			}
		}

		private void btnSubClear_Click(object sender, EventArgs e)
		{
			lstSubtitle.Items.Clear();
		}

		private void btnSubUp_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstSubtitle.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstSubtitle.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstSubtitle.Items.Count;

					if (indx == 0)
					{
						lstSubtitle.Items.Remove(selected);
						lstSubtitle.Items.Insert(totl - 1, selected);
					}
					else
					{
						lstSubtitle.Items.Remove(selected);
						lstSubtitle.Items.Insert(indx - 1, selected);
					}
				}
				else
				{
					MessageBox.Show(Language.IMessage.MoveItem, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnSubDown_Click(object sender, EventArgs e)
		{
			try
			{
				if (lstSubtitle.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstSubtitle.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstSubtitle.Items.Count;

					if (indx == totl - 1)
					{
						lstSubtitle.Items.Remove(selected);
						lstSubtitle.Items.Insert(0, selected);
					}
					else
					{
						lstSubtitle.Items.Remove(selected);
						lstSubtitle.Items.Insert(indx + 1, selected);
					}
				}
				else
				{
					MessageBox.Show(Language.IMessage.MoveItem, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void lstSubtitle_SelectedIndexChanged(object sender, EventArgs e)
		{
			for (int i = 0; i < lstSubtitle.SelectedItems.Count; i++)	// This for loop really need to get SelectedItem (no "s")
			{
				cboSubLang.Text = lstSubtitle.SelectedItems[i].SubItems[2].Text;
			}
		}

		private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstSubtitle.SelectedIndices.Count <= 0)
			{
				return;
			}

			int i = lstSubtitle.SelectedIndices[0];
			if (i >= 0)
			{
				if (cboSubLang.Text.Contains("---"))
					lstSubtitle.Items[i].SubItems[2].Text = "und (Undetermined)";
				else
					lstSubtitle.Items[i].SubItems[2].Text = cboSubLang.Text;
			}
		}

		private void chkAttachEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (!chkSubEnable.Checked)
				chkAttachEnable.Checked = false;

			foreach (Control ctrl in tabAttachment.Controls)
			{
				if (!(ctrl is CheckBox))
					ctrl.Visible = chkAttachEnable.Checked;
			}
			lblAttachNotice.Visible = !chkAttachEnable.Checked;
		}

		private void lstAttachment_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstAttachment_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var item in files)
			{
				// Check if drag n drop file is valid
				if (!GetMetaData.AttachmentValid(item))
					return;
				
				// Process only valid file
				string[] d = GetMetaData.AttachmentData(item);

				ListViewItem SubList = new ListViewItem(d[0]);
				SubList.SubItems.Add(d[1]);
				SubList.SubItems.Add(d[2]);
				SubList.SubItems.Add(d[3]);
				lstAttachment.Items.Add(SubList);
			}
		}

		private void btnAttachAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Title = Language.IMessage.OpenFile;
			GetFiles.Filter = "Known font files|*.ttf;*.otf;*.woff|"
				+ "TrueType Font|*.ttf|"
				+ "OpenType Font|*.otf|"
				+ "Web Open Font Format|*.woff|"
				+ "All Files|*.*";

			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (Properties.Settings.Default.LastOpenAttachLocation == "C:\\")
				GetFiles.RestoreDirectory = true;
			else
				GetFiles.InitialDirectory = Properties.Settings.Default.LastOpenAttachLocation;

			if (GetFiles.ShowDialog() == DialogResult.OK)
			{
				foreach (var item in GetFiles.FileNames)
				{
					// Valid the file
					if (!GetMetaData.AttachmentValid(item))
						return;

					// Get
					string[] d = GetMetaData.AttachmentData(item);

					ListViewItem SubList = new ListViewItem(d[0]);
					SubList.SubItems.Add(d[1]);
					SubList.SubItems.Add(d[2]);
					SubList.SubItems.Add(d[3]);
					lstAttachment.Items.Add(SubList);
				}
				Properties.Settings.Default.LastOpenAttachLocation = System.IO.Path.GetDirectoryName(GetFiles.FileName);
			}
		}

		private void btnAttachRemove_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < lstAttachment.SelectedItems.Count; i++)
			{
				lstAttachment.Items.Remove(lstAttachment.SelectedItems[i]);
			}
		}

		private void btnAttachClear_Click(object sender, EventArgs e)
		{
			lstAttachment.Items.Clear();
		}

		private void rtfLog_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData == (Keys.Control | Keys.S)))
			{
				SaveFileDialog SaveMe = new SaveFileDialog();

				SaveMe.FileName = "ifme_log.rtf";
				SaveMe.Filter = "Poor Text Format|*.rtf|Console Log (in plain text)|*.log";
				SaveMe.FilterIndex = 0;

				if (SaveMe.ShowDialog() == DialogResult.OK)
				{
					rtfLog.SelectionStart = 0;
					rtfLog.SelectionLength = rtfLog.TextLength;
					rtfLog.SelectionBackColor = Color.Black;

					if (SaveMe.FileName.Contains(".rtf"))
						rtfLog.SaveFile(SaveMe.FileName, RichTextBoxStreamType.RichText);
					else
						rtfLog.SaveFile(SaveMe.FileName, RichTextBoxStreamType.PlainText);

					rtfLog.Clear();
					PrintLog(Log.Info, "Console log has been saved and cleared!");
				}
			}
		}

		private void btnOptions_Click(object sender, EventArgs e)
		{
			frmOptions frm = new frmOptions();
			frm.ShowDialog(); // This blocking mode, when Form close, going to next line of code

			if (Options.RestartNow)
			{
				var resbox = MessageBox.Show(Language.IMessage.Restart, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (resbox == DialogResult.Yes)
				{
					string cmd = null, arg = null, app = null;

					if (Options.ResetDefault)
						Properties.Settings.Default.Reset();

					if (OS.IsWindows)
					{
						app = Path.Combine(Globals.AppInfo.CurrentFolder, "ifme.exe");
						cmd = "cmd.exe";
						arg = String.Format("/c TIMEOUT /T 3 /NOBREAK & START \"\" \"{0}\"", app);
					}
					else
					{
						app = Path.Combine(Globals.AppInfo.CurrentFolder, "ifme.sh");
						cmd = "/bin/bash";
						arg = String.Format("-c 'sleep 3 & \"{0}\"'", app);
					}

					System.Diagnostics.Process P = new System.Diagnostics.Process();
					P.StartInfo.FileName = cmd;
					P.StartInfo.Arguments = arg;
					P.StartInfo.WorkingDirectory = Globals.AppInfo.CurrentFolder;
					P.StartInfo.CreateNoWindow = true;
					P.StartInfo.UseShellExecute = false;

					P.Start();
					Application.ExitThread();

					return;
				}
			}

			// Reload audio encoder
			AddAudio();

			// Reload user preset
			AddUserPreset(false);

			// If user choose MP4, uncheck Subtitle
			if (!Properties.Settings.Default.UseMkv)
				chkSubEnable.Checked = false;
		}

		private void btnAbout_Click(object sender, EventArgs e)
		{
			frmAbout frm = new frmAbout();
			frm.ShowDialog();
		}

		private void btnResume_Click(object sender, EventArgs e)
		{
			if (!BGThread.IsBusy)
				return;

			if (btnResume.Text.Equals(Language.IControl.btnResume))
			{
				btnResume.Text = Language.IControl.btnPause;
				
				if (OS.IsLinux)
				{
					TaskManager.ModLinux.ResumeProcess(TaskManager.ImageName.Id);
				}
				else
				{
					Process[] App = Process.GetProcessesByName(TaskManager.ImageName.Current);
					TaskManager.Mod.ResumeProcess(App[0]);
				}
			}
			else
			{
				btnResume.Text = Language.IControl.btnResume;
				
				if (OS.IsLinux)
				{
					TaskManager.ModLinux.SuspendProcess(TaskManager.ImageName.Id);
				}
				else
				{
					Process[] App = Process.GetProcessesByName(TaskManager.ImageName.Current);
					TaskManager.Mod.SuspendProcess(App[0]);
				}
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (BGThread.IsBusy)
			{
				var msgbox = MessageBox.Show(Language.IMessage.Halt, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
				if (msgbox == DialogResult.Yes)
				{
					PrintLog(Log.Info, "Stopping...");
					TaskManager.CPU.Kill(TaskManager.ImageName.Current);
					BGThread.CancelAsync();
				}
			}

			if (!BGThread.IsBusy)
			{
				// Validation
				if (chkQueueSaveTo.Checked)
				{
					try
					{
						string dir = System.IO.Path.GetFullPath(txtDestDir.Text);

						if (!System.IO.Directory.Exists(dir))
							System.IO.Directory.CreateDirectory(dir);
					}
					catch (Exception ex)
					{
						MessageBox.Show(Language.IMessage.EmptySave + "\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}

				if (Addons.Installed.Data[cboAudioFormat.SelectedIndex, 6] != "mp4" && !Properties.Settings.Default.UseMkv)
				{
					MessageBox.Show(Language.IMessage.WrongCodec, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				if (lstQueue.Items.Count == 0)
				{
					MessageBox.Show(Language.IMessage.EmptyQueue, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (chkSubEnable.Checked && lstSubtitle.Items.Count == 0)
				{
					MessageBox.Show(Language.IMessage.EmptySubtitle, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (chkAttachEnable.Checked && lstAttachment.Items.Count == 0)
				{
					MessageBox.Show(Language.IMessage.EmptyAttachment, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (lstQueue.Items.Count != lstSubtitle.Items.Count && chkSubEnable.Checked)
				{
					MessageBox.Show(Language.IMessage.NotEqual, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				// Below when all if filter are passed, proceed to data collecting and start encoding thread
				// Temp folder check
				if (!System.IO.Directory.Exists(Properties.Settings.Default.TemporaryFolder))
				{
					System.IO.Directory.CreateDirectory(Properties.Settings.Default.TemporaryFolder);
					PrintLog(Log.Info, "New temporary folder created!");
				}
				else
				{
					int i = 0;
					foreach (var item in Directory.GetFiles(Properties.Settings.Default.TemporaryFolder))
						i++;

					if (i >= 1)
						PrintLog(Log.Warn, "Temporary folder is not empty! Files inside it will get deleted!");
				}

				// Ready Data
				string[] queue = new string[lstQueue.Items.Count];
				string[] screen = new string[lstQueue.Items.Count];
				string[,] subtitle = new string[lstSubtitle.Items.Count, 3];
				string[,] attachment = new string[lstAttachment.Items.Count, 3];

				for (int i = 0; i < queue.Length; i++)
				{
					string q = lstQueue.Items[i].SubItems[6].Text;
					queue[i] = q;
					string s = lstQueue.Items[i].SubItems[3].Text;
					screen[i] = s.Remove(s.Length - 1);
				}

				for (int s = 0; s < subtitle.GetLength(0); s++)
				{
					subtitle[s, 0] = lstSubtitle.Items[s].SubItems[0].Text;					//File Name
					subtitle[s, 1] = lstSubtitle.Items[s].SubItems[2].Text.Substring(0, 3); //Language
					subtitle[s, 2] = lstSubtitle.Items[s].SubItems[3].Text;					//Path
				}

				for (int x = 0; x < attachment.GetLength(0); x++)
				{
					attachment[x, 0] = lstAttachment.Items[x].SubItems[0].Text; //File Name
					attachment[x, 1] = lstAttachment.Items[x].SubItems[2].Text; //MIME
					attachment[x, 2] = lstAttachment.Items[x].SubItems[3].Text; //Path
				}

				// Since adding multipass, new var is needed!
				string onepass = cboVideoRateCtrl.SelectedIndex == 0 ? "crf" : cboVideoRateCtrl.SelectedIndex == 1 ? "qp" : "bitrate";
				string twopass = cboVideoRateCtrl.Text.Substring(11, 1);
				string encType = cboVideoRateCtrl.SelectedIndex <= 2 ? onepass : twopass;

				// All data become one object, and send to Threading
				List<object> something = new List<object>
				{
					queue,
					subtitle,
					attachment,

					chkQueueSaveTo.Checked,
					txtDestDir.Text,

					cboVideoPreset.Text,
					cboVideoTune.Text,
					encType,
					txtVideoRate.Text,
					txtVideoAdvCmd.Text,

					cboAudioFormat.SelectedIndex,
					cboAudioMode.SelectedIndex,
					cboAudioChan.SelectedIndex,
					cboAudioBitRate.Text,
					cboAudioFreq.Text,

					chkSubEnable.Checked,
					chkAttachEnable.Checked,
					txtAttachDesc.Text,
				
					Properties.Settings.Default.TemporaryFolder,

					// Future addition
					screen
				};

				tabEncoding.SelectedIndex = 6;
				EncodingStarted(true);
				PrintLog(Log.Info, "Encoding started...");
				BGThread.RunWorkerAsync(something);
			}
		}

		#region Encoding thread
		private void BGThread_DoWork(object sender, DoWorkEventArgs e)
		{
			List<object> argsList = e.Argument as List<object>;

			string[] queue = (string[])argsList[0];
			string[,] subtitle = (string[,])argsList[1];
			string[,] attachment = (string[,])argsList[2];

			// Get all UI parameter.
			// Queue output
			bool IsDestDir = (bool)argsList[3];
			string DestDir = (string)argsList[4];
			// Video configuration
			string VidPreset = (string)argsList[5];
			string VidTune = (string)argsList[6];
			string VidType = (string)argsList[7];
			string VidValue = (string)argsList[8];
			string VidXcmd = (string)argsList[9];
			// Audio configuration
			int AudFormat = (int)argsList[10];
			int AudMode = (int)argsList[11];
			int AudChan = (int)argsList[12];
			string AudBitR = (string)argsList[13];
			string AudFreq = (string)argsList[14];
			// Subtitle and attachment
			bool IsSubtitleEnable = (bool)argsList[15];
			bool IsAttachEnable = (bool)argsList[16];
			string AttachDesc = (string)argsList[17];
			// Temp
			string tmp = (string)argsList[18];
			// Future addition
			string[] screen = (string[])argsList[19];

			// Multipass encoding
			int pass = new[] { "2", "3", "4", "5", "6", "7", "8", "9" }.Contains(VidType) ? int.Parse(VidType) : 0;
			VidType = pass >= 2 ? "bitrate" : VidType;

			// Process exit code or return
			int PEC = 0;

			// Decoding-Encoding
			for (int x = 0; x < queue.Length; x++)
			{
				// Determine is Avisynth or not
				var AviSynth = queue[x];
				if (String.Equals(System.IO.Path.GetExtension(AviSynth), ".avs", StringComparison.InvariantCultureIgnoreCase))
					queue[x] = GetMetaData.AviSynthReader(queue[x]); // get original file inside script

				// Get file information
				MediaFile Avi = new MediaFile(queue[x]);
				var audio = Avi.Audio;
				var video = Avi.Video;
				var stext = Avi.Text; // subtitle

				// Set current queue, this will display during x265 encoding
				ListQueue.Current = x + 1;
				ListQueue.Count = queue.Length;

				// Set Time
				System.DateTime CurrentQ = System.DateTime.Now;

				// Change string to int/bool for easy code
				bool IsInterlaced = video[0].scanType.Equals("Interlaced", StringComparison.CurrentCultureIgnoreCase) ? true : false;
				bool IsTopFF = video[0].scanOrder.Equals("Bottom Field First", StringComparison.CurrentCultureIgnoreCase) ? false : true;

				// Preview Block - Set current position.
				if (Globals.Preview.Enable)
				{
					x = Globals.Preview.Selected;
					goto PreviewBegin; 
				}

				// Print current file to encode
				InvokeLog(Log.Info, String.Format("Processing {0}", Path.GetFileName(queue[x])));

				// Extract timecodes (current video will converted to mkv and get timecodes)
				if (!BGThread.CancellationPending)
				{
					// Only progressive and VFR video can be extract timecodes
					if (String.Equals(video[0].frameRateMode, "VFR"))
					{
						// Tell user
						FormTitle(String.Format("Queue {0} of {1}: Indexing source video", x + 1, queue.Length));
						InvokeLog(Log.Info, "Indexing source for future reference. Please Wait...");

						// Get source timecode, this gurantee video and audio in sync (https://github.com/FFMS/ffms2/issues/165)
						// FFmpeg mkvtimestamp_v2 give wrong timecodes, DTS or duplicate frame issue, using FFms Index to provide timecodes
						//StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -copyts -vsync 0 -an -f mkvtimestamp_v2 \"{1}\\timecodes.txt\" -y", queue[x], tmp));
						StartProcess(Addons.BuildIn.FFms, String.Format("-f -c \"{0}\" \"{1}\"", queue[x], Path.Combine(tmp, "timecodes")));

						// Move FFms timecodes track id to timecodes.txt
						// Delete if exist
						if (File.Exists(Path.Combine(tmp, "timecodes.txt")))
							File.Delete(Path.Combine(tmp, "timecodes.txt"));

						// Check index Id
						int id = video[0].Id == 0 ? 0 : video[0].Id - 1;

						// Move while rename
						File.Move(Path.Combine(tmp, String.Format("timecodes_track0{0}.tc.txt", id)), Path.Combine(tmp, "timecodes.txt"));
					}
				}

				// Extract MKV
				if (!BGThread.CancellationPending)
				{
					// Make sure only MKV can proceed
					if (Avi.format.Equals("Matroska", StringComparison.CurrentCultureIgnoreCase))
					{
						// Proceed when Subtitle and Attachment is disabled also MKV option is selected
						if (Properties.Settings.Default.UseMkv && !IsSubtitleEnable)
						{
							// Set title
							FormTitle(String.Format("Queue {0} of {1}: Extracting MKV Stream", x + 1, queue.Length));
							InvokeLog(Log.Info, "If this part got error, don't worry about it :)");

							// Extract metadata. Go to "MKV extarcted content" below (use CTRL+F)
							StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -vn -an -map 0 -y \"{1}\"", queue[x], Path.Combine(tmp, "archive.mkv")));
						}
					}
				}

				// Decode audio and process multiple streams
				if (!BGThread.CancellationPending)
				{
					if (audio.Count >= 1)
					{
						// Set title
						FormTitle(String.Format("Queue {0} of {1}: Decoding all audio...", x + 1, queue.Length));
						InvokeLog(Log.Info, "Now decoding audio~ Please Wait...");

						// Capture MediaInfo Audio ID and assigned to FFmpeg Map ID
						string[] AudioMapID = Stuff.MediaMap(queue[x], "Audio");

						// Accepting automatic audio freq.
						string Freq;
						if (String.Equals(AudFreq, "Automatic"))
							Freq = "";
						else
							Freq = "-ar " + AudFreq;

						// Decode Audio
						switch (AudMode)
						{
							case 0:
								if (AudFormat == 0)
									goto case 3;
								else
									goto default;

							case 1:
								if (AudFormat == 0)
									goto case 3;

								if (audio.Count == 1)
									goto default;

								string arg = null;
								string map = null;
								for (int i = 0; i < audio.Count; i++)
									map += String.Format("-map {0} ", AudioMapID[i]);

								map = map.Remove(map.Length - 1);
								
								arg = String.Format("-i \"{0}\" {1} -filter_complex amix=inputs={2}:duration=first:dropout_transition=0 {3} -y \"{4}\"", queue[x], map, audio.Count, Freq, Path.Combine(tmp, "audio1.wav"));
								PEC = StartProcess(Addons.BuildIn.FFmpeg, arg);

								break;

							case 2:
								if (AudFormat == 0)
									goto case 3;

								if (audio.Count == 1)
									goto default;

								for (int i = 0; i < audio.Count; i++)
									PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -map {1} {2} -y \"{3}\"", queue[x], AudioMapID[i], Freq, Path.Combine(tmp, String.Format("audio{0}.wav", i + 1))));

								break;

							case 3:
								var modecopy = "-i \"{0}\" -vn -map {1} -acodec copy -y \"{2}\"";
								var modeconv = "-i \"{0}\" -vn -map {1} -strict experimental -c:a aac -b:v {2}k -y \"{3}\"";

								for (int i = 0; i < audio.Count; i++)
									if (Properties.Settings.Default.UseMkv)
										PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format(modecopy, queue[x], AudioMapID[i], Path.Combine(tmp, String.Format("audio{0}.mka", i + 1))));
									else
										if (String.Equals(audio[i].format.ToLower(), "alac") || 
											String.Equals(audio[i].format.ToLower(), "ac-3") || 
											String.Equals(audio[i].format.ToLower(), "aac") || 
											audio[i].format.ToLower().Contains("mpeg"))
											PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format(modecopy, queue[x], AudioMapID[i], Path.Combine(tmp, String.Format("audio{0}.mp4", i + 1))));
										else
											PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format(modeconv, queue[x], AudioMapID[i], AudBitR, Path.Combine(tmp, String.Format("audio{0}.mp4", i + 1))));

								break;

							default:
								PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -vn {1} -y \"{2}\"", queue[x], Freq, Path.Combine(tmp, "audio1.wav")));
								break;
						}
					}

					if (PEC == 1)
					{
						e.Cancel = true;
						break;
					}
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Encode all decoded audio
				if (!BGThread.CancellationPending)
				{
					// Make sure not pass-through mode
					if (audio.Count >= 1 && AudFormat != 0)
					{
						// Set title
						FormTitle(String.Format("Queue {0} of {1}: Encoding all audio...", x + 1, queue.Length));
						InvokeLog(Log.Info, "Now encoding audio~ Please Wait...");

						// Split ID and Folder path
						string getpath = Addons.Installed.Data[AudFormat, 0];

						// Merge
						string app = Path.Combine(getpath, Addons.Installed.Data[AudFormat, 10]);
						string cmd = Addons.Installed.Data[AudFormat, 11];

						// get all wav file
						foreach (var filein in Directory.GetFiles(tmp, "*.wav"))
						{
							/* example of ogg
							 * {3} -b {0} "{2}" -o "{1}.ogg"
							 * example of opus
							 * --bitrate {0} {3} "{2}" "{1}.opus"
							 * 0 = extra command-line
							 * 1 = bit rate/level
							 * 2 = output file
							 * 3 = input file
							 */
							string xargs = Addons.Installed.Data[AudFormat, 12];
							string fileout = Path.Combine(tmp, Path.GetFileNameWithoutExtension(filein));
							string[] args = new string[4];

							args[0] = AudBitR;
							args[1] = fileout;
							args[2] = filein;
							args[3] = xargs;

							PEC = StartProcess(app, String.Format(cmd, args));
						}

						if (PEC == 1)
						{
							e.Cancel = true;
							break;
						}
					}
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Delete all wav file
				foreach (var item in Directory.GetFiles(tmp, "*.wav"))
					File.Delete(item);

				// Tell user
				FormTitle(String.Format("Queue {0} of {1}: Encoding video...", x + 1, queue.Length));

				// Preview Block - Jump here
			PreviewBegin:
				if (Globals.Preview.Enable)
					InvokeLog(Log.Info, String.Format("Creating preview for: {0}", Path.GetFileName(queue[x])));
				else
					InvokeLog(Log.Info, String.Format("Encoding {0}", Path.GetFileName(queue[x])));

				// Realtime decoding-encoding
				if (!BGThread.CancellationPending)
				{
					if (video.Count >= 1)
					{
						string EXE = Addons.BuildIn.FFmpeg;
						string[] args = new string[11];
						string cmd = null;
						string yuv = "yuv420p"; // future use, allowing converting YUV

						bool IsVFR = String.Equals(video[0].frameRateMode, "VFR") ? true : false;
						ulong FrameCount = video[0].frameCount;
						float FpsOri = video[0].frameRateOri;
						float FPS = video[0].frameRate;

						string vsync = IsVFR ? "passthrough" : FpsOri == 0 ? "cfr" : String.Format("cfr -r {0}", FPS);

						// FFmpeg part
						args[0] = String.Format("-i \"{0}\"", queue[x]);
						args[1] = String.Format("-pix_fmt {0}", yuv);
						args[2] = String.Format("-f yuv4mpegpipe -s {0} -vsync {1} -", screen[x], vsync);

						// x265 part
						args[3] = String.Format("-p {0}", VidPreset);

						if (!VidTune.Equals("off"))
							args[4] = String.Format("-t {0}", VidTune);

						args[5] = String.Format("--{0} {1}", VidType, VidValue);
						args[6] = FrameCount == 0 ? "" : String.Format("-f \"{0}\"", FrameCount);
						args[7] = String.Format("-o \"{0}\"", Path.Combine(tmp, "video.hevc"));
						args[8] = VidXcmd;

						// Preview Block - Modify total frame to be process
						if (Globals.Preview.Enable)
							args[6] = String.Format("-f \"{0}\"", (int)((float)Globals.Preview.Duration * FPS));

						// Due limitation of x265, cannot change 10 bit to 8 bit or vice versa
						if (video[0].bitDepth > 8)
							args[9] = Addons.BuildIn.HEVCHI;
						else
							args[9] = Addons.BuildIn.HEVCLO;
						
						// Due x265 limitation of interlaced video, do deinterlaced by keep both field
						if (IsInterlaced)
						{
							// Ready data and default
							var xp = VidPreset.ToLower();
							var qp = Int32.Parse(VidValue);
							int fi = 0;
							int mo = 0;

							// Apply deinterlaced filter after
							if (!IsTopFF)
								fi = 1;

							// Deinterlace by following x265 preset 
							if (xp == "faster" || xp == "fast" || xp == "medium")
								mo = 1;
							else if (xp == "slow" || xp == "slower" || xp == "veryslow")
								mo = 2;
							else if (xp == "placebo")
								mo = 3;
							else
								mo = 0;

							// Make sure value not more then 51 if user choose bitrate
							if (qp > 51)
								qp = 10;

							// Split field into seperate frame
							args[2] += String.Format(" -vf \"yadif=1:{0}:0, mcdeint={1}:{0}:{2}, pp=lb\"", fi, mo, qp);

							// Since split field to each frame, total frame become double
							// Preview Block - Modify total frame to be process
							if (!Globals.Preview.Enable)
								args[6] = String.Format("-f \"{0}\"", (FrameCount * 2));
							else
								args[6] = String.Format("-f \"{0}\"", (int)((float)Globals.Preview.Duration * (FPS * 2)));
						}

						// AviSynth Stuff (get's override)
						if (!String.IsNullOrEmpty(AviSynth))
						{
							EXE = Addons.BuildIn.AVI2PIPE;
							args[0] = "video";
							args[1] = String.Format("\"{0}\"", AviSynth);
							args[2] = "";
							args[6] = "";

							InvokeLog(Log.Warn, String.Format("Could not detect how many frame in AviSynth Script ({0}).", Path.GetFileName(AviSynth)));
						}

						// Add space
						for (int i = 0; i < args.GetLength(0); i++)
						{
							if (i == 2 || i == 8 || i == 9 || i == 10)
								continue;

							if (args[i] != null)
								args[i] = args[i] + " ";
						}

						// Specify null device for stdout and stdin
						if (OS.IsLinux)
							args[10] = "/dev/null";
						else
							args[10] = "nul";

						cmd = String.Format("{0}{1}{2} 2> {10} | \"{9}\" {3}{4}{5}{6}{7}{8} --y4m -", args);

						// Multi Pass
						if (pass >= 2)
						{
							for (int i = 1; i <= pass; i++)
							{
								// Tell user current pass
								InvokeLog(Log.Info, String.Format("Processing images, pass {0} of {1}", i, pass));

								// Proceed to encode
								if (i == 1)
									PEC = StartProcess(EXE, cmd + " --pass 1"); // create stats
								else if (i == pass)
									PEC = StartProcess(EXE, cmd + " --pass 2"); // override, used for last pass
								else
									PEC = StartProcess(EXE, cmd + " --pass 3"); // not override stats, use for 'n'th pass

								// Break encoding when user press stop
								if (PEC == 1) { e.Cancel = true; break; }
							}
						}
						else
						{
							InvokeLog(Log.Info, String.Format("Processing images. Preset: {0}. Tune: {1}. Rate control: {2} {3}", VidPreset, VidTune, VidType.ToUpper(), VidValue));
							PEC = StartProcess(EXE, cmd);
						}
					}

					if (PEC == 1) { e.Cancel = true; break; }
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Muxing
				if (!BGThread.CancellationPending)
				{
					// Tell user
					FormTitle(String.Format("Queue {0} of {1}: Merge encoded files...", x + 1, queue.Length));
					InvokeLog(Log.Info, "Now merge some stuff and synchronise video and audio~ Almost there!");

					// Ready path for destination folder
					string FileOut = null;
					if (IsDestDir)
						FileOut = Globals.Preview.Enable ? Path.Combine(DestDir, "[preview] " + Path.GetFileNameWithoutExtension(queue[x])) : Path.Combine(DestDir, Path.GetFileNameWithoutExtension(queue[x]));
					else
						FileOut = Globals.Preview.Enable ? Path.Combine(Path.GetDirectoryName(queue[x]), "[preview] " + Path.GetFileNameWithoutExtension(queue[x])) : Path.Combine(Path.GetDirectoryName(queue[x]), "[encoded] " + Path.GetFileNameWithoutExtension(queue[x]));
					
					// Generate one, save log after conversion finished
					if (Properties.Settings.Default.LogAutoSave)
						Log.AutoSaveName = FileOut + ".log";

					// Mux by MKV or MP4
					if (Properties.Settings.Default.UseMkv)
					{
						// MKV most powerful container format!
						string command = null;
						string trackorder = null;

						// Attachment
						string attach = null;
						if (IsAttachEnable)
						{
							for (int i = 0; i < attachment.GetLength(0); i++)
							{
								string[] place = new string[4];
								place[0] = attachment[i, 0]; //file name only
								place[1] = attachment[i, 2]; //full path
								place[2] = attachment[i, 1]; //MIME
								place[3] = AttachDesc;
								attach += String.Format("--attachment-mime-type \"{2}\" --attachment-description \"{3}\" --attachment-name \"{0}\" --attach-file \"{1}\" ", place);
							}
						}

						// Video
						string[] vp = new string[4];
						vp[0] = FileOut;
						vp[1] = Globals.AppInfo.WritingApp;
						vp[2] = Path.Combine(tmp, "video.hevc");
						vp[3] = null;

						if (File.Exists(Path.Combine(tmp, "timecodes.txt")))
							vp[3] = String.Format("--timecodes 0:\"{0}\" ", Path.Combine(tmp, "timecodes.txt"));

						command = String.Format("-o \"{0}.mkv\" --track-name \"0:{1}\" --forced-track 0:no {3}-d 0 -A -S -T --no-global-tags --no-chapters \"(\" \"{2}\" \")\" ", vp);
						trackorder = "--track-order 0:0";
						
						// Audio
						int id = 1;
						foreach (var item in System.IO.Directory.GetFiles(tmp, "audio*"))
						{
							string[] ap = new string[2];
							ap[0] = id.ToString();
							ap[1] = item;
							command += String.Format("--track-name \"0:Track {0}\" --forced-track 0:no -a 0 -D -S -T --no-global-tags --no-chapters \"(\" \"{1}\" \")\" ", ap);
							trackorder = trackorder + "," + id.ToString() + ":0";
							id++;
						}
						
						// Subtitle
						if (IsSubtitleEnable)
						{
							string[] sp = new string[3];
							sp[0] = subtitle[x, 1]; //lang
							sp[1] = subtitle[x, 0]; //file name only (description?)
							sp[2] = subtitle[x, 2]; //full path
							command += String.Format("--language \"0:{0}\" --track-name \"0:{1}\" --forced-track 0:no -s 0 -D -A -T --no-global-tags --no-chapters \"(\" \"{2}\" \")\" ", sp);
							trackorder = trackorder + "," + id.ToString() + ":0";
						}
						
						// MKV extarcted content
						if (File.Exists(Path.Combine(tmp, "archive.mkv")))
						{
							FileInfo ChapLen = new FileInfo(Path.Combine(tmp, "archive.mkv"));
							if (ChapLen.Length > 1024)
								if (!IsSubtitleEnable)
									command += String.Format("\"(\" \"{0}\" \")\" ", Path.Combine(tmp, "archive.mkv"));
						}

						
						// Build command for mkvmerge
						command = "--disable-track-statistics-tags " + command + attach + trackorder;
						
						// Send to mkvmerge
						PEC = StartProcess(Addons.BuildIn.MKV, command);
						if (PEC == 1)
						{
							PEC = 0; // mark mkvtoolnix warning as 'ok' condition
							InvokeLog(Log.Warn, "mkvtoolnix return a warning, mostly sync issue for older codec.");
						}
							

						// Preview Block - Set file
						if (Globals.Preview.Enable)
							Globals.Preview.File = FileOut + ".mkv";

						// Try modify MKV metadata
						try
						{
							Stuff.WriteMkvTagWApp(FileOut + ".mkv");
						}
						catch
						{
							InvokeLog(Log.Error, "Cannot modify MKV metadata, noting critical =P");
						}
					}
					else
					{
						// MP4, not awesome :(
						string command = "";
						int i = 0;
						
						// Video
						command = String.Format("-add \"{0}#video:name={1}:fmt=HEVC\"", Path.Combine(tmp, "video.hevc"), Globals.AppInfo.WritingApp);
						
						// Audio
						foreach (var item in System.IO.Directory.GetFiles(tmp, "audio*"))
							command += String.Format(" -add \"{0}#audio:name=Track {1}\"", item, i++);

						// Send to mp4box
						PEC = StartProcess(Addons.BuildIn.MP4, String.Format("{0} \"{1}\"", command, Path.Combine(tmp, "mod.mp4")));

						// If timecodes exist, apply.
						if (File.Exists(Path.Combine(tmp, "timecodes.txt")))
							PEC = StartProcess(Addons.BuildIn.MP4FPS, String.Format("-t \"{0}\" \"{1}\" -o \"{2}.mp4\"", Path.Combine(tmp, "timecodes.txt"), Path.Combine(tmp, "mod.mp4"), FileOut));
						else
							File.Copy(Path.Combine(tmp, "mod.mp4"), FileOut + ".mp4", true);

						// Preview Block - Set file
						if (Globals.Preview.Enable)
							Globals.Preview.File = FileOut + ".mp4";
					}

					// Delete all temp file for next queue
					foreach (var item in Directory.GetFiles(tmp))
						File.Delete(item);

					if (PEC == 1)
					{
						e.Cancel = true;
						break;
					}
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Display total wasted time
				InvokeLogDuration(Log.Info, "This session takes", CurrentQ);

				// Save log if required
				if (Properties.Settings.Default.LogAutoSave)
				{
					try
					{
						if (this.InvokeRequired)
						{
							BeginInvoke(new MethodInvoker(() => rtfLog.SaveFile(Log.AutoSaveName, RichTextBoxStreamType.PlainText)));
							BeginInvoke(new MethodInvoker(() => rtfLog.Clear()));
							BeginInvoke(new MethodInvoker(() => StartUpLog()));
						}
						else
						{
							rtfLog.SaveFile(Log.AutoSaveName, RichTextBoxStreamType.PlainText);
							rtfLog.Clear();
							StartUpLog();
						}
						InvokeLog(Log.Info, "Log Saved: " + Log.AutoSaveName);
					}
					catch
					{
						InvokeLog(Log.Error, "Log cannot save, unable to write to disk, sorry!");
					}
				}

				// Preview Block - Finish and Break loop,
				if (Globals.Preview.Enable)
					break;

				// One file finished
				if (this.InvokeRequired)
					BeginInvoke(new MethodInvoker(() => lstQueue.Items.RemoveAt(0)));
				else
					lstQueue.Items.RemoveAt(0);
			}
			e.Result = PEC;
		}
		#endregion

		#region Run console program and display all console line
		private int StartProcess(string exe, string args)
		{
			// This allow longer command to pass
			Environment.SetEnvironmentVariable("IFMECMD", String.Format("\"{0}\" {1}", exe, args), EnvironmentVariableTarget.Process);

			Process P = new Process();
			var SI = P.StartInfo;

			if (OS.IsWindows)
			{
				SI.FileName = "cmd";
				SI.Arguments = "/c %IFMECMD%";
			}
			else
			{
				SI.FileName = "bash";
				SI.Arguments = "-c '$IFMECMD'";
			}

			SI.WorkingDirectory = Properties.Settings.Default.TemporaryFolder;
			SI.CreateNoWindow = true;
			SI.UseShellExecute = false;
			SI.RedirectStandardOutput = OS.IsWindows;
			SI.RedirectStandardError = OS.IsWindows;

			P.OutputDataReceived += consoleOutputHandler;
			P.ErrorDataReceived += consoleErrorHandler;

			P.Start();

			if (OS.IsWindows)
			{
				P.BeginOutputReadLine();
				P.BeginErrorReadLine();
			}

			TaskManager.SetPerformance(exe, args);

			P.WaitForExit();
			int X = P.ExitCode;
			P.Close();

			if (X == 1)
				InvokeLog(Log.Warn, String.Format("Sorry, it seem having a problem, IFME sending command: {0} {1}", exe, args));

			return X;
		}

		// Standard Output
		private delegate void consoleOutputDelegate(string outputString);
		private void consoleOutput(string outputString)
		{
			// Invoke the sub to allow cross thread calls to pass safely
			if (this.InvokeRequired)
			{
				consoleOutputDelegate del = new consoleOutputDelegate(consoleOutput);
				object[] args = { outputString };
				this.Invoke(del, args);
			}
			else
			{
				// Now we can update your textbox with the data passed from the asynchronous thread
				if (outputString.Contains(" frames, ") || outputString.Contains(" frames: "))
					this.Text = String.Format("Queue {0} of {1}: {2}", ListQueue.Current, ListQueue.Count, outputString);
				else
					rtfLog.AppendText(string.Concat(outputString, Environment.NewLine));
			}
		}

		// Catch the Standard Output
		public void consoleOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
		{
			if (!string.IsNullOrEmpty(outLine.Data))
			{
				// If the Current output line is not empty then pass it back to your main thread (Form1)
				consoleOutput(outLine.Data);
			}
		}

		// Error Output
		private delegate void consoleErrorDelegate(string errorString);
		private void consoleError(string errorString)
		{
			// Invoke the sub to allow cross thread calls to pass safely
			if (this.InvokeRequired)
			{
				consoleErrorDelegate del = new consoleErrorDelegate(consoleError);
				object[] args = { errorString };
				this.Invoke(del, args);
			}
			else
			{
				// Now we can update your textbox with the data passed from the asynchronous thread
				if (errorString.Contains(" frames, ") || errorString.Contains(" frames: "))
					this.Text = String.Format("Queue {0} of {1}: {2}", ListQueue.Current, ListQueue.Count, errorString);
				else
					rtfLog.AppendText(string.Concat(errorString, Environment.NewLine));
			}
		}

		// Catch the Error Output
		private void consoleErrorHandler(object sendingProcess, DataReceivedEventArgs errLine)
		{
			if (!string.IsNullOrEmpty(errLine.Data))
			{
				// If the Current error line is not empty then pass it back to your main thread (Form1)
				consoleError(errLine.Data);
			}
		}
		#endregion

		#region BGThread Invoke/Delegate UI (use for access winforms controls from different thread)
		private void FormTitle(string s)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => this.Text = s));
			else
				this.Text = s;
		}

		private void InvokeLog(int status, string word)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => PrintLog(status, String.Format("{0}", word))));
			else
				PrintLog(status, String.Format("{0}", word));
		}

		private void InvokeLogDuration(int status, string word, System.DateTime LastTime)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => PrintLog(status, String.Format("{0} {1}", word, Duration(LastTime)))));
			else
				PrintLog(status, String.Format("{0} {1}", word, Duration(LastTime)));
		}
		#endregion

		#region BGThread_RunWorkerCompleted (When thread is finished)
		private void BGThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
				PrintLog(Log.Error, "Encoding did not run perfectly, Deal with it!"); 
			else if (e.Cancelled)
				PrintLog(Log.Warn, "Encoding canceled! Nope!"); 
			else
				PrintLog(Log.OK, "Encoding completed! Oh yeah!");

			// Delete all temp file when complete
			foreach (var item in Directory.GetFiles(Properties.Settings.Default.TemporaryFolder))
				File.Delete(item);

			// Button
			if (lstQueue.SelectedItems.Count == 0)
			{
				btnStart.Enabled = false;
				btnEdit.Enabled = false;
				btnPreview.Enabled = false;
				btnQueueEditScript.Enabled = false;
				btnQueueGenerate.Enabled = false;
			}

			// UI stuff
			int a = cboAudioFormat.SelectedIndex;
			int b = cboAudioFormat.Items.Count - 1;
			cboAudioFormat.SelectedIndex = 0;
			cboAudioFormat.SelectedIndex = b;
			cboAudioFormat.SelectedIndex = a;

			// Reset
			EncodingStarted(false);
			this.Text = Globals.AppInfo.NameTitle;

			// Preview Block - Play the files
			if (Globals.Preview.Enable)
			{
				Globals.Preview.Enable = false;
				if (File.Exists(Globals.Preview.File))
				{
					PrintLog(Log.Warn, "Make sure you have player that able to play HEVC codec.");
					PrintLog(Log.Warn, "Preview file will not delete automatically.");
					PrintLog(Log.Info, "Opening " + Globals.Preview.File);
					Process.Start(Globals.Preview.File);
				}
				else
				{
					PrintLog(Log.Warn, "Preview cancel by user or file not found.");
				}
			}

			// Shutdown on when encoding job completed
			if (OS.IsWindows)
				if (chkDoneOffMachine.Checked)
					if (!e.Cancelled || e.Error != null)
						Process.Start("shutdown", "/s /f /t 3 /c \"Queue encoding complete!\"");
		}
		#endregion

		#region When encoding running, disable control or enable when finish
		private void EncodingStarted(bool x)
		{
			btnOptions.Enabled = !x;
			btnAbout.Enabled = !x;
			btnResume.Enabled = x;

			// Hybrid button
			btnResume.Text = Language.IControl.btnPause;
			btnStart.Text = x ? Language.IControl.btnStop : Language.IControl.btnStart;

			// Button, if quete not empty, dont disable
			if (lstQueue.Items.Count == 0)
				btnStart.Enabled = x;

			// Also not forget about Attchment Enable, must disable if subtitle not checked
			chkAttachEnable.Enabled = chkSubEnable.Checked;

			// Disable control during encoding, code simplified!
			TabControl.TabPageCollection pages = tabEncoding.TabPages;
			foreach (TabPage page in pages)
			{
				if (String.Equals(page.Name, "tabStatus"))
					continue;

				foreach (Control ctl in page.Controls)
					ctl.Enabled = !x;
			}

			// Never disable
			chkDoneOffMachine.Enabled = true;
		}
		#endregion

		#region Return total time
		public string Duration(System.DateTime pastTime)
		{
			// Calculate past time - present time and return the result.
			TimeSpan timeSpan = System.DateTime.Now.Subtract(pastTime);
			string returnTime = null;

			if (timeSpan.Days != 0)
				returnTime = String.Format("{0}d {1}h {2}m {3}s", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			else if (timeSpan.Hours != 0)
				returnTime = String.Format("{0}h {1}m {2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			else if (timeSpan.Minutes != 0)
				returnTime = String.Format("{0} min {1} sec", timeSpan.Minutes, timeSpan.Seconds);
			else
				returnTime = String.Format("{0} seconds!", timeSpan.Seconds);

			return returnTime;
		}
		#endregion

		#region Add and filter all installed audio addons to Combo Box
		private void AddAudio()
		{
			cboAudioFormat.Items.Clear();
			Addons.Installed.Get();

			for (int i = 0; i < Addons.Installed.Data.Length; i++)
			{
				if (Addons.Installed.Data[i, 0] == null)
					break;

				if (Addons.Installed.Data[i, 1] == "audio")
						cboAudioFormat.Items.Add(Addons.Installed.Data[i, 2]);
			}

			cboAudioFormat.SelectedIndex = 0;
		}
		#endregion

		#region Add user preset list into combo box
		private void AddUserPreset(bool IsStartUp)
		{
			cboUserPreList.Items.Clear();
			UserPreset.Installed.Get();

			for (int i = 0; i < UserPreset.Installed.Data.Length; i++)
			{
				if (UserPreset.Installed.Data[i, 0] == null)
					break;

				cboUserPreList.Items.Add(UserPreset.Installed.Data[i, 1]);
			}

			if (IsStartUp)
			{
				try
				{
					cboUserPreList.SelectedIndex = Properties.Settings.Default.UserPreset;
				}
				catch
				{
					cboUserPreList.SelectedIndex = 0;
				}
			}
		}
		#endregion

		#region User Settings Section, Load and Save
		private void UserSettingsLoad()
		{
			// CPU Affinity, Load previous, if none, set default all CPU
			if (Properties.Settings.Default.CPUAffinity == "none" || Properties.Settings.Default.CPUAffinity == null)
			{
				Properties.Settings.Default.CPUAffinity = TaskManager.CPU.DefaultAll(true);
				Properties.Settings.Default.Save();
			}

			string[] aff = Properties.Settings.Default.CPUAffinity.Split(',');
			for (int i = 0; i < Environment.ProcessorCount; i++)
			{
				TaskManager.CPU.Affinity[i] = Convert.ToBoolean(aff[i]);
			}

			// Form default
			if (Properties.Settings.Default.FormFullScreen)
				this.WindowState = FormWindowState.Maximized;

			// Output Dir
			if (Properties.Settings.Default.OutputDirPath == "")
				Properties.Settings.Default.OutputDirPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

			txtDestDir.Text = Properties.Settings.Default.OutputDirPath;
			chkQueueSaveTo.Checked = Properties.Settings.Default.OutputDirEnable;
			chkDoneOffMachine.Checked = Properties.Settings.Default.Shutdown;
		}

		private void UserSettingsSave()
		{
			if (this.WindowState == FormWindowState.Maximized)
			{
				Properties.Settings.Default.FormFullScreen = true;
			}
			else
			{
				Properties.Settings.Default.FormFullScreen = false;
				Properties.Settings.Default.FormSize = this.Size;
			}

			// This one cannot put in event of Text Change
			Properties.Settings.Default.VideoRateValue = txtVideoRate.Text;

			// Save
			Properties.Settings.Default.Save();
		}
		#endregion

		#region Interface Language Section (Load and Create)
		private void LoadLang()
		{
			string file = Path.Combine(Language.Folder, Language.Default + ".ini");

			if (!System.IO.File.Exists(file))
				Language.Default = "eng";

			file = Path.Combine(Language.Folder, Language.Default + ".ini");

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(file);

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
						if (ctrl.Text.Contains('{'))
							if (data[Language.Section.Root][ctrl.Name].Contains('|'))
								ctrl.Text = String.Format(ctrl.Text, data[Language.Section.Root][ctrl.Name].Split('|'));
							else
								ctrl.Text = String.Format(ctrl.Text, data[Language.Section.Root][ctrl.Name]);

			} while (ctrl != null);

			// Audio mode
			cboAudioMode.Items.AddRange(data[Language.Section.Amtd]["WhichMode"].Split('|'));
			// Queue
			lstQueue.Columns[0].Text = data[Language.Section.Lst]["FileName"];
			lstQueue.Columns[1].Text = data[Language.Section.Lst]["Ext"];
			lstQueue.Columns[2].Text = data[Language.Section.Lst]["Codec"];
			lstQueue.Columns[3].Text = data[Language.Section.Lst]["Res"];
			/* FPS was here, 4 */
			lstQueue.Columns[5].Text = data[Language.Section.Lst]["BitDepth"];
			lstQueue.Columns[6].Text = data[Language.Section.Lst]["Path"];
			// Sub
			lstSubtitle.Columns[0].Text = data[Language.Section.Lst]["FileName"];
			lstSubtitle.Columns[1].Text = data[Language.Section.Lst]["Ext"];
			lstSubtitle.Columns[2].Text = data[Language.Section.Lst]["Lang"];
			lstSubtitle.Columns[3].Text = data[Language.Section.Lst]["Path"];
			// Attach
			lstAttachment.Columns[0].Text = data[Language.Section.Lst]["FileName"];
			lstAttachment.Columns[1].Text = data[Language.Section.Lst]["Ext"];
			lstAttachment.Columns[2].Text = data[Language.Section.Lst]["Mime"];
			lstAttachment.Columns[3].Text = data[Language.Section.Lst]["Path"];
			// Message
			Language.IMessage.OpenFile = data[Language.Section.Msg]["OpenFile"];
			Language.IMessage.OpenFolder = data[Language.Section.Msg]["OpenFolder"];
			Language.IMessage.Invalid = data[Language.Section.Msg]["Invalid"];
			Language.IMessage.MoveItem = data[Language.Section.Msg]["MoveItem"];
			Language.IMessage.WrongCodec = data[Language.Section.Msg]["WrongCodec"];
			Language.IMessage.EmptySave = data[Language.Section.Msg]["EmptySave"];
			Language.IMessage.EmptyQueue = data[Language.Section.Msg]["EmptyQueue"];
			Language.IMessage.EmptySubtitle = data[Language.Section.Msg]["EmptySubtitle"];
			Language.IMessage.EmptyAttachment = data[Language.Section.Msg]["EmptyAttachment"];
			Language.IMessage.NotEqual = data[Language.Section.Msg]["NotEqual"];
			Language.IMessage.Quit = data[Language.Section.Msg]["Quit"];
			Language.IMessage.Halt = data[Language.Section.Msg]["Halt"];
			Language.IMessage.InstallMsg = data[Language.Section.Msg]["InstallMsg"];
			Language.IMessage.RemoveMsg = data[Language.Section.Msg]["RemoveMsg"];
			Language.IMessage.RemoveMsgErr = data[Language.Section.Msg]["RemoveMsgErr"];
			Language.IMessage.Restart = data[Language.Section.Msg]["Restart"];
			Language.IMessage.ResetSettingsAsk = data[Language.Section.Msg]["ResetSettingsAsk"];
			Language.IMessage.ResetSettingsOK = data[Language.Section.Msg]["ResetSettingsOK"];
			Language.IMessage.MKVOnly = data[Language.Section.Msg]["MKVOnly"];
			Language.IMessage.NotEmptyFolder = data[Language.Section.Msg]["NotEmptyFolder"];
			// ToolTip
			Language.IMessage.ProTipTitle = data[Language.Section.Pro]["Title"];
			Language.IMessage.ProTipUpdate = data[Language.Section.Pro]["TellUpdate"];
			// Hybrid button
			Language.IControl.btnStart = data[Language.Section.Root][btnStart.Name];
			Language.IControl.btnStop = data[Language.Section.Root]["btnStop"];
			Language.IControl.btnResume = data[Language.Section.Root][btnResume.Name];
			Language.IControl.btnPause = data[Language.Section.Root]["btnPause"];
			btnStart.Text = Language.IControl.btnStart;
			btnResume.Text = Language.IControl.btnPause;
		}

		// Developer Use, Capture all valid control for multi language support
		private void CreateLang()
		{
			System.IO.File.WriteAllText(Language.FileEng, "");

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Language.FileEng);

			data.Sections.AddSection(Language.Section.Info);
			data.Sections[Language.Section.Info].AddKey("iso", "eng"); // file id
			data.Sections[Language.Section.Info].AddKey("Name", "Anime4000");
			data.Sections[Language.Section.Info].AddKey("Version", "0.1");
			data.Sections[Language.Section.Info].AddKey("Contact", "fb.com/anime4000");

			data.Sections.AddSection(Language.Section.Root);
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
						if (ctrl.Text.Contains('{'))
							data.Sections[Language.Section.Root].AddKey(ctrl.Name, "");

			} while (ctrl != null);

			parser.WriteFile(Language.FileEng, data, System.Text.Encoding.UTF8);
		}
		#endregion

		#region Console and Log Printing
		public void PrintLog(int type, string message)
		{
			rtfLog.SelectionColor = Color.LightGray;
			rtfLog.SelectedText = "[";

			switch (type)
			{
				case 0:
					rtfLog.SelectionColor = Color.Cyan;
					//rtfLog.SelectedText = "info";
					rtfLog.SelectedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
					break;
				case 1:
					rtfLog.SelectionColor = Color.LightGreen;
					//rtfLog.SelectedText = " ok ";
					rtfLog.SelectedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
					break;
				case 2:
					rtfLog.SelectionColor = Color.Gold;
					//rtfLog.SelectedText = "warn";
					rtfLog.SelectedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
					break;
				default:
					rtfLog.SelectionColor = Color.Red;
					//rtfLog.SelectedText = "erro";
					rtfLog.SelectedText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
					tabEncoding.SelectedTab = tabStatus;
					break;
			}

			rtfLog.SelectionColor = Color.LightGray;
			rtfLog.SelectedText = "] " + message + "\n";
		}
		#endregion

		private void pictDonate_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://goo.gl/Y71xsQ");
		}

		private void btnAdvanceHelp_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("http://x265.readthedocs.org/en/default/cli.html");
		}
	}
}