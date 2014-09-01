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

			// Form Init.
			this.Size = Properties.Settings.Default.FormSize;
			this.Icon = Properties.Resources.ifme_green;
			this.Text = Globals.AppInfo.NameTitle;
			pictBannerRight.Parent = pictBannerMain;
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			var msgbox = MessageBox.Show(Language.IMessage.Quit, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			if (msgbox == DialogResult.No)
				e.Cancel = true;

			UserSettingsSave();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// Startup
			rtfLog.SelectionColor = Color.Yellow;
			rtfLog.SelectedText = String.Format("{0} - compiled on {1}\nVersion: {2} ({3} build) by {4} ({5})\n\n", Globals.AppInfo.Name, Globals.AppInfo.BuildDate, Globals.AppInfo.Version, Globals.AppInfo.CPU, Globals.AppInfo.Author, Globals.AppInfo.WebSite); ;
			rtfLog.SelectionColor = Color.Red;
			rtfLog.SelectedText = "Warning: This program still in beta, unexpected event may occur. \n\n";

			// Migrate previous settings
			if (Properties.Settings.Default.UpdateSettings)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.UpdateSettings = false;
				Properties.Settings.Default.Save();

				PrintLog(Log.OK, "Previous settings has been upgraded!");
			}

			// Temp Folder
			if (String.IsNullOrEmpty(Properties.Settings.Default.TemporaryFolder) || !System.IO.Directory.Exists(Globals.AppInfo.TempFolder))
			{
				System.IO.Directory.CreateDirectory(Globals.AppInfo.TempFolder);
				Properties.Settings.Default.TemporaryFolder = Globals.AppInfo.TempFolder;
				Properties.Settings.Default.Save();

				PrintLog(Log.OK, "New default temporary folder created!");
			}

			// Check for updates
			PrintLog(Log.Info, "Checking for updates");
			frmSplashScreen SS = new frmSplashScreen();
			SS.ShowDialog();

			// Display that IFME has new version
			if (!Globals.AppInfo.VersionEqual)
				PrintLog(Log.Info, "Latest IFME version: " + Globals.AppInfo.VersionNew + "! Click About button to perform update");
			
			// ISO
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

			// Load Addons + Build-in again
			try
			{
				PrintLog(Log.OK, Addons.Installed.GetCount() + " Addons has been loaded!");
			}
			catch (Exception ex)
			{
				PrintLog(Log.Error, ex.Message);
			}

			// Console log now can be save and clear
			PrintLog(Log.Info, "Save this log? Click here and press CTRL+S (console will be clear after save)");

			// After addons has been load, now display it on UI
			AddAudio();

			//CreateLang(); // Developer tool, Scan and Create new empty language
			LoadLang(); // Load language, GUI must use {0} as place-holder

			// Load Settings
			UserSettingsLoad();

			// Display console
			tabEncoding.SelectedTab = tabStatus;
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!Globals.AppInfo.VersionEqual)
			{
				btnAbout.Text = "&Update!";
				proTip.ToolTipTitle = Language.IMessage.ProTipTitle;
				proTip.Show(Language.IMessage.ProTipUpdate, this.btnAbout, 50, 18, 15000);
			}
		}

		private void frmMain_SizeChanged(object sender, EventArgs e)
		{
			// List View for Queue, fast way
			// Get size
			var lst = lstQueue.Columns;

			// We want auto size on last column
			lst[6].Width = (lstQueue.Width - 4) - (lst[0].Width + lst[1].Width + lst[2].Width + lst[3].Width + lst[4].Width + lst[5].Width);
		}

		private void btnQueueAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Title = Language.IMessage.OpenFile;
			GetFiles.Filter = "Known Video files|*.mkv;*.mp4;*.m4v;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.emp;*.emm|"
				+ "MKV Container|*.mkv|"
				+ "MP4 Container|*.mp4;*.m4v|"
				+ "Audio Video Interleaved|*.avi;*.divx|"
				+ "Windows Media Video|*.wmv|"
				+ "Moving Picture Experts Group|*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob|"
				+ "Empire Media|*.emp;*.emm|"
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

					PrintLog(Log.Info, String.Format("File \"{0}\" added via Open File (button)\n       Format: {2} ({1}). RES: {3}. FPS: {4}. BPP: {5}", h));
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
			for (int i = 0; i < lstQueue.SelectedItems.Count; i++)
			{
				lstQueue.Items.Remove(lstQueue.SelectedItems[i]);
			}

			if (lstQueue.Items.Count != 0)
				btnStart.Enabled = true;
			else
				btnStart.Enabled = false;
		}

		private void btnQueueClear_Click(object sender, EventArgs e)
		{
			lstQueue.Items.Clear();
			btnStart.Enabled = false;
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

				PrintLog(Log.Info, String.Format("File \"{0}\" added via Drag n Drop\n       Format: {2} ({1}). RES: {3}. FPS: {4}. BPP: {5}", h));
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

		private void cboVideoRateCtrl_SelectedIndexChanged(object sender, EventArgs e)
		{
			var i = cboVideoRateCtrl.SelectedIndex;
			if (i == 0)
			{
				txtVideoRate.ReadOnly = true;
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
				txtVideoRate.ReadOnly = true;
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

		private void cboVideoPreset_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoPreset = cboVideoPreset.SelectedIndex;
		}

		private void cboVideoTune_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.VideoTune = cboVideoTune.SelectedIndex;
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
			Properties.Settings.Default.AudioFormat = cboAudioFormat.SelectedIndex;
		}

		private void cboAudioBitRate_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioBitRate = cboAudioBitRate.SelectedIndex;
		}

		private void cboAudioFreq_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioFreq = cboAudioFreq.SelectedIndex;
		}

		private void cboAudioChan_DropDownClosed(object sender, EventArgs e)
		{
			Properties.Settings.Default.AudioChan = cboAudioFreq.SelectedIndex;
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
					if (Options.ResetDefault)
						Properties.Settings.Default.Reset();

					System.Diagnostics.Process P = new System.Diagnostics.Process();
					P.StartInfo.FileName = "cmd.exe";
					P.StartInfo.Arguments = String.Format("/c TIMEOUT /T 3 /NOBREAK & start \"\" \"{0}\"", Path.Combine(Globals.AppInfo.CurrentFolder, "ifme.exe"));
					P.StartInfo.WorkingDirectory = Globals.AppInfo.CurrentFolder;
					P.StartInfo.CreateNoWindow = true;
					P.StartInfo.UseShellExecute = false;

					P.Start();
					Application.ExitThread();

					return;
				}
			}

			// Reload audio encoder
			cboAudioFormat.Items.Clear();
			AddAudio();

			// If user choose MP4, uncheck Subtitle
			if (!Properties.Settings.Default.UseMkv)
				chkSubEnable.Checked = false;
		}

		private void btnAbout_Click(object sender, EventArgs e)
		{
			frmAbout frm = new frmAbout();
			frm.ShowDialog();
		}

		private void btnPause_Click(object sender, EventArgs e)
		{
			Process[] App = Process.GetProcessesByName(TaskManager.ImageName.Current);
			TaskManager.Mod.SuspendProcess(App[0]);
			btnPause.Visible = false;
			btnResume.Visible = true;
		}

		private void btnResume_Click(object sender, EventArgs e)
		{
			Process[] App = Process.GetProcessesByName(TaskManager.ImageName.Current);
			TaskManager.Mod.ResumeProcess(App[0]);
			btnPause.Visible = true;
			btnResume.Visible = false;
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			var msgbox = MessageBox.Show(Language.IMessage.Halt, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (msgbox == DialogResult.Yes)
			{
				TaskManager.CPU.Kill(TaskManager.ImageName.Current);
				BGThread.CancelAsync();
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
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
				if (!System.IO.Directory.Exists(Globals.AppInfo.TempFolder))
					System.IO.Directory.CreateDirectory(Globals.AppInfo.TempFolder);					

				// Ready Data
				string[] queue = new string[lstQueue.Items.Count];
				string[,] subtitle = new string[lstSubtitle.Items.Count, 3];
				string[,] attachment = new string[lstAttachment.Items.Count, 3];

				for (int i = 0; i < queue.Length; i++)
				{
					queue[i] = lstQueue.Items[i].SubItems[6].Text;
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
					cboVideoRateCtrl.Text.Substring(cboVideoRateCtrl.Text.Length - 3),
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
				
					Properties.Settings.Default.TemporaryFolder
				};

				tabEncoding.SelectedIndex = 5;
				EncodingStarted(true);

				BGThread.RunWorkerAsync(something);
			}
		}

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
			// Mkv Extract Trigger, this make sure not to extract a mkv which dont have subtitle and attachment
			bool HasSubtitle = false;
			bool HasAttachment = false;

			// Process exit code or return
			int PEC = 0;

			// Decoding-Encoding
			for (int x = 0; x < queue.Length; x++)
			{
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
				bool IsInterlaced = false;
				bool IsTopFF = true;

				if (video[0].scanType.Equals("Interlaced", StringComparison.CurrentCultureIgnoreCase))
					IsInterlaced = true;

				if (video[0].scanOrder.Equals("Bottom Field First", StringComparison.CurrentCultureIgnoreCase))
					IsTopFF = false;

				// Extract timecodes (current video will converted to mkv and get timecodes)
				if (!BGThread.CancellationPending)
				{
					// Only progressive video can be extract timecodes
					if (!IsInterlaced)
					{
						// Tell user
						FormTitle(String.Format("Queue {0} of {1}: Get timecodes for synchronisation", x + 1, queue.Length));
						InvokeLog(Log.Info, "Reading source time codes for reference. Please Wait...");

						// Get source timecode, this gurantee video and audio in sync (https://github.com/FFMS/ffms2/issues/165)
						// FFmpeg mkvtimestamp_v2 give wrong timecodes, DTS or duplicate frame issue, using FFms Index to provide timecodes
						//StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -copyts -vsync 0 -an -f mkvtimestamp_v2 \"{1}\\timecodes.txt\" -y", queue[x], tmp));
						StartProcess(Addons.BuildIn.FFms, String.Format("-f -c \"{0}\" \"{1}\" > nul", queue[x], Path.Combine(tmp, "timecodes")));

						// Move FFms timecodes track id to timecodes.txt
						// Delete if exist
						if (File.Exists(Path.Combine(tmp, "timecodes.txt")))
							File.Delete(Path.Combine(tmp, "timecodes.txt"));

						// Check index Id
						int id;
						if (video[0].Id == 0)
							id = 0;
						else
							id = video[0].Id - 1;

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
							InvokeLog(Log.Info, "Currently detect and extracting MKV stream(s). Please Wait...");

							// Chapters!
							StartProcess(Addons.BuildIn.MKE, String.Format("chapters \"{0}\" > \"{1}\"", queue[x], Path.Combine(tmp, "chapters.xml")));
							
							// Print mkv stream
							StartProcess(Addons.BuildIn.MKV, String.Format("-i \"{0}\" > \"{1}\"", queue[x], Path.Combine(tmp, "meta.if")));

							// Reset list
							MkvExtractId.ClearList();

							// Attachment
							string cmdath = null;
							MkvExtractId.AttachmentDataGet(Path.Combine(tmp, "meta.if"));
							for (int q = 0; q < MkvExtractId.AttachmentData.GetLength(0); q++)
							{
								if (MkvExtractId.AttachmentData[q, 0] == null)
									break;

								int id = int.Parse(MkvExtractId.AttachmentData[q, 2]);
								string file = MkvExtractId.AttachmentData[q, 0];
								string mime = MkvExtractId.AttachmentData[q, 1];

								cmdath += String.Format("{0}:\"{1}\" ", id, Path.Combine(tmp, file));

								// Now this video has attachment in it, change to true
								if (!HasAttachment)
									HasAttachment = true;
							}

							StartProcess(Addons.BuildIn.MKE, String.Format("attachments \"{0}\" {1}", queue[x], cmdath));

							// Subtitle
							string cmdsub = null;
							for (int s = 0; s < stext.Count; s++)
							{
								int id = stext[s].Id - 1;
								string iso = stext[s].languageThree;
								string fmt = stext[s].format.ToLower();
								string file = String.Format("subtitle_id_{0}_{1}.{2}", id, iso, fmt);

								MkvExtractId.SubtitleData[s, 0] = iso;
								MkvExtractId.SubtitleData[s, 1] = file;
								MkvExtractId.SubtitleData[s, 2] = id.ToString();

								cmdsub += String.Format("{0}:\"{1}\" ", id, Path.Combine(tmp, file));

								// Now this video has subtitle in it, change to true
								if (stext.Count != 0)
									HasSubtitle = true;
							}

							StartProcess(Addons.BuildIn.MKE, String.Format("tracks \"{0}\" {1}", queue[x], cmdsub));
						}
					}
				}

				// Decode audio and process multiple streams
				if (!BGThread.CancellationPending)
				{
					// Set title
					FormTitle(String.Format("Queue {0} of {1}: Decoding all audio...", x + 1, queue.Length));
					InvokeLog(Log.Info, "Now decoding audio~ Please Wait...");

					if (audio.Count >= 1)
					{
						// Capture MediaInfo Audio ID and assigned to FFmpeg Map ID
						int[] AudioMapID = new int[100];
						for (int i = 0; i < audio.Count; i++)
						{
							AudioMapID[i] = audio[i].Id - 1; //FFmpeg uses zero based index
						}

						// Decode Audio
						switch (AudMode)
						{
							case 0:
								goto default;
							case 1:
								if (audio.Count == 1)
									goto default;

								string arg = null;
								string map = null;
								for (int i = 0; i < audio.Count; i++)
								{
									map += String.Format("-map 0:{0} ", AudioMapID[i].ToString());
								}
								map = map.Remove(map.Length - 1);
								arg = String.Format("-i \"{0}\" {1} -filter_complex amix=inputs={2}:duration=first:dropout_transition=0 -ar {3} -y \"{4}\"", queue[x], map, audio.Count, AudFreq, Path.Combine(tmp, "audio1.wav"));

								PEC = StartProcess(Addons.BuildIn.FFmpeg, arg);

								break;
							case 2:
								if (audio.Count == 1)
									goto default;

								for (int i = 0; i < audio.Count; i++)
								{
									PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -map 0:{1} -ar {2} -y \"{3}\"", queue[x], AudioMapID[i], AudFreq, Path.Combine(tmp, String.Format("audio{0}.wav", i + 1))));
								}

								break;
							default:
								PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -vn -ar {1} -y \"{2}\"", queue[x], AudFreq, Path.Combine(tmp, "audio1.wav")));
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
					// Set title
					FormTitle(String.Format("Queue {0} of {1}: Encoding all audio...", x + 1, queue.Length));
					InvokeLog(Log.Info, "Now encoding audio~ Please Wait...");

					//                      Folder\app.exe
					string app = Path.Combine(Addons.Installed.Data[AudFormat, 0], Addons.Installed.Data[AudFormat, 10]);
					string cmd = Addons.Installed.Data[AudFormat, 11];

					// get all wav file
					foreach (var item in Directory.GetFiles(tmp, "*.wav"))
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
						string[] args = new string[4];
						args[0] = AudBitR;
						args[1] = Path.Combine(tmp, Path.GetFileNameWithoutExtension(item));
						args[2] = item;
						args[3] = Addons.Installed.Data[AudFormat, 12];

						PEC = StartProcess(app, String.Format(cmd, args));
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

				// Delete all wav file
				foreach (var item in Directory.GetFiles(tmp, "*.wav"))
					File.Delete(item);

				// Realtime decoding-encoding
				if (!BGThread.CancellationPending)
				{
					// Tell user
					InvokeLog(Log.Info, "Now decoding video, can take very long time. Just be patient...");

					if (video.Count >= 1)
					{
						string cmd = "";
						string[] args = new string[10];

						// FFmpeg part
						args[0] = String.Format("-i \"{0}\"", queue[x]);
						args[1] = String.Format("-pix_fmt {0}", "yuv420p");
						args[2] = String.Format("-f {0}", "yuv4mpegpipe");

						// x265 part
						args[3] = String.Format("-p {0}", VidPreset);

						if (!VidTune.Equals("off"))
							args[4] = String.Format("-t {0}", VidTune);

						args[5] = String.Format("--{0} {1}", VidType, VidValue);
						args[6] = String.Format("-f {0}", video[0].frameCount);
						args[7] = String.Format("-o \"{0}\"", Path.Combine(tmp, "video.hevc"));
						args[8] = VidXcmd;

						if (video[0].bitDepth > 8)
							args[9] = Addons.BuildIn.HEVCHI;
						else
							args[9] = Addons.BuildIn.HEVC;
						
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
							args[6] = String.Format("-f {0}", (video[0].frameCount * 2));
						}

						// Add space
						for (int i = 0; i < args.GetLength(0); i++)
						{
							if (i == 2 || i == 8)
								continue;

							if (args[i] != null)
								args[i] = args[i] + " ";
						}

						cmd = String.Format("{0}{1}{2} - 2> nul | \"{9}\" {3}{4}{5}{6}{7}{8} --y4m -", args);

						PEC = StartProcess(Addons.BuildIn.FFmpeg, cmd);
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

				// Muxing
				if (!BGThread.CancellationPending)
				{
					// Tell user
					FormTitle(String.Format("Queue {0} of {1}: Merge encoded files...", x + 1, queue.Length));
					InvokeLog(Log.Info, "Now merge some stuff and synchronise video and audio~ Almost there!");

					// Ready path for destination folder
					string FileOut = null;
					if (IsDestDir)
						FileOut = Path.Combine(DestDir, Path.GetFileNameWithoutExtension(queue[x]));
					else
						FileOut = Path.Combine(Path.GetDirectoryName(queue[x]), "[encoded] " + Path.GetFileNameWithoutExtension(queue[x]));

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
						else if (HasAttachment)
						{
							// Change back to false, prevent error that video dont have attachment
							HasAttachment = false;
							for (int i = 0; i < MkvExtractId.AttachmentData.GetLength(0); i++)
							{
								if (MkvExtractId.AttachmentData[i, 0] == null)
									break;

								string[] place = new string[4];
								place[0] = MkvExtractId.AttachmentData[i, 0]; //file name only
								place[1] = Path.Combine(tmp, MkvExtractId.AttachmentData[i, 0]); //full path
								place[2] = MkvExtractId.AttachmentData[i, 1]; //MIME
								place[3] = "Build-in Transfer";
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

						command = String.Format("-o \"{0}.mkv\" --track-name \"0:{1}\" --forced-track 0:no {3}-d 0 -A -S -T --no-global-tags --no-chapters ( \"{2}\" ) ", vp);
						trackorder = "--track-order 0:0";
						
						// Audio
						int id = 1;
						foreach (var item in System.IO.Directory.GetFiles(tmp, "audio*"))
						{
							string[] ap = new string[2];
							ap[0] = id.ToString();
							ap[1] = item;
							command += String.Format("--track-name \"0:Track {0}\" --forced-track 0:no -a 0 -D -S -T --no-global-tags --no-chapters ( \"{1}\" ) ", ap);
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
							command += String.Format("--language \"0:{0}\" --track-name \"0:{1}\" --forced-track 0:no -s 0 -D -A -T --no-global-tags --no-chapters ( \"{2}\" ) ", sp);
							trackorder = trackorder + "," + id.ToString() + ":0";
						}
						else if (HasSubtitle)
						{
							// Turn to disable
							HasSubtitle = false;
							for (int i = 0; i < MkvExtractId.SubtitleData.GetLength(0); i++)
							{
								if (MkvExtractId.SubtitleData[i, 0] == null)
									break;
								string[] sp = new string[3];
								sp[0] = MkvExtractId.SubtitleData[i, 0]; //lang
								sp[1] = MkvExtractId.SubtitleData[i, 1].ToUpper(); //file name only (description?)
								sp[2] = Path.Combine(tmp, MkvExtractId.SubtitleData[i, 1]); //full path
								command += String.Format("--language \"0:{0}\" --track-name \"0:{1}\" --forced-track 0:no -s 0 -D -A -T --no-global-tags --no-chapters ( \"{2}\" ) ", sp);
								trackorder = trackorder + "," + id.ToString() + ":0";
								id++;
							}
						}
						
						// Chapters! proceed when file is exist and not empty
						string chapters = null;
						if (File.Exists(Path.Combine(tmp, "chapters.xml")))
						{
							FileInfo ChapLen = new FileInfo(Path.Combine(tmp, "chapters.xml"));
							if (ChapLen.Length > 20)
								chapters = String.Format("--chapters \"{0}\" ", Path.Combine(tmp, "chapters.xml"));
						}
						
						// Build command for mkvmerge
						command = command + attach + chapters + trackorder;
						
						// Send to mkvmerge
						PEC = StartProcess(Addons.BuildIn.MKV, command);
					}
					else
					{
						// MP4, not awesome :(
						string command = "";
						int i = 0;
						
						// Video
						command = String.Format("-add \"{0}#video:name={1}:fmt=HEVC\"", Path.Combine(tmp, "video.hevc"), Globals.AppInfo.WritingApp);
						
						// Audio
						foreach (var item in System.IO.Directory.GetFiles(tmp, "*.mp4"))
						{
							command += " ";
							command += String.Format("-add \"{0}#audio:name=Track {1}\"", item, i.ToString());
							i++;
						}

						// Send to mp4box
						PEC = StartProcess(Addons.BuildIn.MP4, String.Format("{0} \"{1}\"", command, Path.Combine(tmp, "mod.mp4")));

						// Modify FPS
						PEC = StartProcess(Addons.BuildIn.MP4FPS, String.Format("-t \"{0}\" \"{1}\" -o \"{2}.mp4\"", Path.Combine(tmp, "timecodes.txt"), Path.Combine(tmp, "mod.mp4"), FileOut));
					}

					if (PEC == 1)
					{
						e.Cancel = true;
						break;
					}

					// Delete all temp file
					foreach (var item in Directory.GetFiles(tmp))
					{
						File.Delete(item);
					}
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Display total wasted time
				InvokeLogDuration(Log.Info, "This queue take", CurrentQ);

				// One file finished
				if (this.InvokeRequired)
					BeginInvoke(new MethodInvoker(() => lstQueue.Items.RemoveAt(0)));
				else
					lstQueue.Items.RemoveAt(0);
			}
			e.Result = PEC;
		}

		#region Run console program and display all console line
		private int StartProcess(string exe, string args)
		{
			Process P = new Process();

			var SI = P.StartInfo;
			SI.FileName = "cmd.exe";
			SI.Arguments = String.Format("/c start \"IFME\" /D \"{2}\" /WAIT /B \"{0}\" {1}", exe, args, Globals.AppInfo.CurrentFolder);
			SI.WorkingDirectory = Globals.AppInfo.CurrentFolder;
			SI.CreateNoWindow = true;
			SI.RedirectStandardOutput = true;
			SI.RedirectStandardError = true;
			SI.UseShellExecute = false;

			P.OutputDataReceived += consoleOutputHandler;
			P.ErrorDataReceived += consoleErrorHandler;

			P.Start();

			P.BeginOutputReadLine();
			P.BeginErrorReadLine();

			// Set CPU Performance and Affinity
			TaskManager.ProcessPerf(exe, args);

			P.WaitForExit();
			int X = P.ExitCode;
			P.Close();

			// If process not exited, kill it
			TaskManager.CPU.Kill(exe);

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
				if (outputString.Contains(" frames, "))
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
				if (errorString.Contains(" frames, "))
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

		private void BGThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				PrintLog(Log.Error, "Encoding did not run perfectly, check status log!");
			}
			else if (e.Cancelled)
			{
				PrintLog(Log.Warn, "Encoding canceled :(");
			}
			else
			{
				PrintLog(Log.OK, "Encoding completed! Yay!");
			}

			// Delete everything in temp folder
			foreach (var item in System.IO.Directory.GetFiles(Properties.Settings.Default.TemporaryFolder))
			{
				System.IO.File.Delete(item);
			}

			// Reset
			EncodingStarted(false);
			MkvExtractId.ClearList();
			this.Text = Globals.AppInfo.NameTitle;
		}

		public string Duration(System.DateTime pastTime)
		{
			// Calculate past time - present time and return the result.
			TimeSpan timeSpan = System.DateTime.Now.Subtract(pastTime);
			string returnTime = null;

			if (timeSpan.Days.ToString() != "0")
				returnTime = String.Format("{0}d {1}h {2}m {3}s", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			else if (timeSpan.Hours.ToString() != "0")
				returnTime = String.Format("{0}h {1}m {2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			else if (timeSpan.Minutes.ToString() != "0")
				returnTime = String.Format("{0} min {1} sec", timeSpan.Minutes, timeSpan.Seconds);
			else
				returnTime = String.Format("{0} seconds!", timeSpan.Seconds);

			return returnTime;
		}

		#region When encoding running, disable control or enable when finish
		private void EncodingStarted(bool x)
		{
			btnOptions.Enabled = !x;
			btnAbout.Enabled = !x;

			btnStart.Visible = !x;
			btnStop.Visible = x;
			btnPause.Visible = x;
			btnResume.Visible = x;

			// Button, if quete not empty, dont disable
			if (lstQueue.Items.Count == 0)
				btnStart.Enabled = x;

			// Also not forget about Attchment Enable, must disable if subtitle not checked
			chkAttachEnable.Enabled = chkSubEnable.Checked;

			foreach (Control ctl in tabQueue.Controls)
			{
				ctl.Enabled = !x;
			}

			foreach (Control ctl in tabVideo.Controls)
			{
				ctl.Enabled = !x;
			}

			foreach (Control ctl in tabAudio.Controls)
			{
				ctl.Enabled = !x;
			}

			foreach (Control ctl in tabSubtitle.Controls)
			{
				ctl.Enabled = !x;
			}

			foreach (Control ctl in tabAttachment.Controls)
			{
				ctl.Enabled = !x;
			}
		}
		#endregion

		#region Add and filter all installed audio addons to Combo Box
		private void AddAudio()
		{
			for (int i = 0; i < Addons.Installed.Data.Length; i++)
			{
				if (Addons.Installed.Data[i, 0] == null)
					break;

				if (Addons.Installed.Data[i, 1] == "audio")
					if (Properties.Settings.Default.UseMkv)
						cboAudioFormat.Items.Add(Addons.Installed.Data[i, 2]);
					else
						if (Addons.Installed.Data[i, 6] == "mp4")
							cboAudioFormat.Items.Add(Addons.Installed.Data[i, 2]);
			}

			if (cboAudioFormat.SelectedIndex == -1)
				if (cboAudioFormat.Items.Count >= Properties.Settings.Default.AudioFormat)
					cboAudioFormat.SelectedIndex = Properties.Settings.Default.AudioFormat;
				else
					cboAudioFormat.SelectedIndex = 0;
			else
				cboAudioFormat.SelectedIndex = Properties.Settings.Default.AudioFormat;
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

			// Video
			cboVideoPreset.SelectedIndex = Properties.Settings.Default.VideoPreset;
			cboVideoTune.SelectedIndex = Properties.Settings.Default.VideoTune;
			cboVideoRateCtrl.SelectedIndex = Properties.Settings.Default.VideoRateType;
			txtVideoRate.Text = Properties.Settings.Default.VideoRateValue.ToString();
			txtVideoAdvCmd.Text = Properties.Settings.Default.VideoCmd;

			// Audio
			cboAudioFormat.SelectedIndex = Properties.Settings.Default.AudioFormat;
			cboAudioBitRate.SelectedIndex = Properties.Settings.Default.AudioBitRate;
			cboAudioFreq.SelectedIndex = Properties.Settings.Default.AudioFreq;
			cboAudioChan.SelectedIndex = Properties.Settings.Default.AudioChan;
			cboAudioMode.SelectedIndex = Properties.Settings.Default.AudioMode;
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
			Properties.Settings.Default.VideoRateValue = Convert.ToInt32(txtVideoRate.Text);

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
			// ToolTip
			Language.IMessage.ProTipTitle = data[Language.Section.Pro]["Title"];
			Language.IMessage.ProTipUpdate = data[Language.Section.Pro]["TellUpdate"];
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
					rtfLog.SelectedText = "info";
					break;
				case 1:
					rtfLog.SelectionColor = Color.LightGreen;
					rtfLog.SelectedText = " ok ";
					break;
				case 2:
					rtfLog.SelectionColor = Color.Gold;
					rtfLog.SelectedText = "warn";
					break;
				default:
					rtfLog.SelectionColor = Color.Red;
					rtfLog.SelectedText = "erro";
					tabEncoding.SelectedTab = tabStatus;
					break;
			}

			rtfLog.SelectionColor = Color.LightGray;
			rtfLog.SelectedText = "] " + message + "\n";
		}
		#endregion
	}
}
