using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
			this.Icon = Properties.Resources.aruuie_ifme;
			this.Text = Globals.AppInfo.Name;
			pictBannerRight.Parent = pictBannerLeft;
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
			if (Properties.Settings.Default.TemporaryFolder == "none" || !System.IO.Directory.Exists(Globals.AppInfo.TempFolder))
			{
				System.IO.Directory.CreateDirectory(Globals.AppInfo.TempFolder);
				Properties.Settings.Default.TemporaryFolder = Globals.AppInfo.TempFolder;
				Properties.Settings.Default.Save();

				PrintLog(Log.OK, "New default temporary folder created!");
			}

			// Get installed addons, this code has been moved here since 4.0.0.4, issue are when disable update check cause addons not to load
			Addons.Installed.Get();

			// Check for updates
			if (Properties.Settings.Default.UpdateAlways)
			{
				// Check addons for any updates
				PrintLog(Log.Info, "Checking for updates");
				frmSplashScreen SS = new frmSplashScreen();
				SS.ShowDialog();
			}

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

			//CreateLang(); // Developer tool, Create new empty language
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
				proTip.ToolTipTitle = Language.IMessage.ProTipTitle;
				proTip.Show(Language.IMessage.ProTipUpdate, this.btnAbout, 50, 18, 15000);
			}
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
					QueueList.SubItems.Add(h[4] + " bit");
					QueueList.SubItems.Add(h[5]);

					lstQueue.Items.Add(QueueList);
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
				QueueList.SubItems.Add(h[4] + " bit");
				QueueList.SubItems.Add(h[5]);

				lstQueue.Items.Add(QueueList);
			}

			if (lstQueue.Items.Count != 0)
				btnStart.Enabled = true;
			else
				btnStart.Enabled = false;
		}

		private void chkQueueSaveTo_CheckedChanged(object sender, EventArgs e)
		{
			txtDestDir.Enabled = chkQueueSaveTo.Checked;
			btnQueueBrowseDest.Enabled = chkQueueSaveTo.Checked;
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

				SaveMe.FileName = "ifme_log.log";
				SaveMe.Filter = "Console Log (in plain text)|*.log";
				SaveMe.FilterIndex = 0;

				if (SaveMe.ShowDialog() == DialogResult.OK)
				{
					rtfLog.SaveFile(SaveMe.FileName, RichTextBoxStreamType.PlainText);

					rtfLog.Clear();
					PrintLog(Log.Info, "Console log has been saved and cleared!");
				}
			}
		}

		private void rtfLog_TextChanged(object sender, EventArgs e)
		{
			rtfLog.SelectionStart = rtfLog.TextLength;
			rtfLog.ScrollToCaret();
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
					UserSettingsSave();

					System.Diagnostics.Process P = new System.Diagnostics.Process();
					P.StartInfo.FileName = "cmd.exe";
					P.StartInfo.Arguments = String.Format("/c TIMEOUT /T 3 /NOBREAK & start \"\" \"{0}\\ifme.exe\"", Globals.AppInfo.CurrentFolder);
					P.StartInfo.WorkingDirectory = Globals.AppInfo.CurrentFolder;
					P.StartInfo.CreateNoWindow = true;
					P.StartInfo.UseShellExecute = false;

					P.Start();
					Application.ExitThread();

					return;
				}
			}

			cboAudioFormat.Items.Clear();
			AddAudio();
			cboAudioFormat.SelectedIndex = 0;
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
				if (chkQueueSaveTo.Checked && (txtDestDir.Text == null || txtDestDir.Text == "" || !txtDestDir.Text.Contains('\\')))
				{
					MessageBox.Show(Language.IMessage.EmptySave, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (Addons.Installed.Data[cboAudioFormat.SelectedIndex, 6] != "mp4" && !Properties.Settings.Default.UseMkv)
				{
					MessageBox.Show(Language.IMessage.WrongCodec, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}

				//if (lstQueue.Items.Count == 0)
				//{
				//	MessageBox.Show(Language.IMessage.EmptyQueue, Language.IMessage.Invalid, MessageBoxButtons.OK, MessageBoxIcon.Error);
				//	return;
				//}
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

				// Ready Data
				string[] queue = new string[lstQueue.Items.Count];
				string[,] subtitle = new string[lstSubtitle.Items.Count, 3];
				string[,] attachment = new string[lstAttachment.Items.Count, 3];

				for (int i = 0; i < queue.Length; i++)
				{
					queue[i] = lstQueue.Items[i].SubItems[5].Text;
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

			// Process exit code or return
			int PEC = 0;

			// Decoding-Encoding
			for (int x = 0; x < queue.Length; x++)
			{
				// Get file information
				MediaFile Avi = new MediaFile(queue[x]);
				var audio = Avi.Audio;
				var video = Avi.Video;

				// Set current queue, this will display during x265 encoding
				ListQueue.Current = x + 1;
				ListQueue.Count = queue.Length;

				// Set title
				FormTitle(String.Format("Queue {0} of {1}: Decoding all audio...", (x + 1).ToString(), queue.Length.ToString()));

				// Set Time
				System.DateTime CurrentQ = System.DateTime.Now;

				// Decode audio and process multiple streams
				if (!BGThread.CancellationPending)
				{
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
								arg = String.Format("-i \"{0}\" {1} -filter_complex amix=inputs={2}:duration=first:dropout_transition=0 -ar {3} -y \"{4}\\audio1.wav\"", queue[x], map, audio.Count, AudFreq, tmp);

								PEC = StartProcess(Addons.BuildIn.FFmpeg, arg);

								break;
							case 2:
								if (audio.Count == 1)
									goto default;

								for (int i = 0; i < audio.Count; i++)
								{
									PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -map 0:{1} -ar {2} -y \"{3}\\audio{4}.wav\"", queue[x], AudioMapID[i].ToString(), AudFreq, tmp, (i + 1).ToString()));
								}

								break;
							default:
								PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format("-i \"{0}\" -vn -ar {1} -y \"{2}\\audio1.wav\"", queue[x], AudFreq, tmp));
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

				// Set title
				FormTitle(String.Format("Queue {0} of {1}: Encoding all audio...", (x + 1).ToString(), queue.Length.ToString()));

				// Encode all decoded audio
				if (!BGThread.CancellationPending)
				{
					//                      Folder\app.exe
					string app = String.Format("{0}\\{1}", Addons.Installed.Data[AudFormat, 0], Addons.Installed.Data[AudFormat, 10]);
					string cmd = Addons.Installed.Data[AudFormat, 11];

					// get all wav file
					foreach (var item in System.IO.Directory.GetFiles(tmp, "*.wav"))
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
						args[1] = tmp + "\\" + System.IO.Path.GetFileNameWithoutExtension(item);
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

				// Delete wav file
				if (!BGThread.CancellationPending)
				{
					foreach (var item in System.IO.Directory.GetFiles(tmp, "*.wav"))
					{
						System.IO.File.Delete(item);
					}
				}
				else
				{
					e.Cancel = true;
					break;
				}

				// Realtime decoding-encoding
				if (!BGThread.CancellationPending)
				{
					if (video.Count >= 1)
					{
						string cmd = "-i \"{0}\" -pix_fmt yuv420p -f yuv4mpegpipe - 2> nul | \"{1}\" -p {2} -t {3} --{4} {5} --fps {6} -f {7} {9} -o \"{8}\\video.hvc\" --y4m -";
						string[] args = new string[10];
						args[0] = queue[x];

						if (video[0].bitDepth == 8)
							args[1] = Addons.BuildIn.HEVC;
						else
							args[1] = Addons.BuildIn.HEVC10;

						args[2] = VidPreset;
						args[3] = VidTune;
						args[4] = VidType;
						args[5] = VidValue;
						args[6] = video[0].frameRate.ToString();
						args[7] = video[0].frameCount.ToString();
						args[8] = tmp;
						args[9] = VidXcmd;

						PEC = StartProcess(Addons.BuildIn.FFmpeg, String.Format(cmd, args));
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

				// Set title
				FormTitle(String.Format("Queue {0} of {1}: Merge encoded files...", (x + 1).ToString(), queue.Length.ToString()));

				// Muxing
				if (!BGThread.CancellationPending)
				{
					// Ready path for destination folder
					string FileOut = null;
					if (IsDestDir)
						FileOut = DestDir + "\\" + System.IO.Path.GetFileNameWithoutExtension(queue[x]);
					else
						FileOut = System.IO.Path.GetDirectoryName(queue[x]) + "\\_encoded_" + System.IO.Path.GetFileNameWithoutExtension(queue[x]);

					// Mux by MKV or MP4
					if (Properties.Settings.Default.UseMkv)
					{
						string command = "";
						string trackorder = "";

						// Attachment
						string attach = "";
						if (IsAttachEnable)
						{
							for (int i = 0; i < attachment[i, 0].Length; i++)
							{
								string[] place = new string[3];
								place[0] = attachment[i, 0]; //file name only
								place[1] = attachment[i, 2]; //full path
								place[2] = attachment[i, 1]; //MIME
								attach += String.Format("--attachment-mime-type \"{3}\" --attachment-description \"{0}\" --attachment-name \"{0}\" --attach-file \"{1}\" ", place);
							}
							attach = attach.Remove(attach.Length - 1);
						}


						// Video
						string[] vp = new string[4];
						vp[0] = FileOut;
						vp[1] = Globals.AppInfo.WritingApp;
						vp[2] = video[0].frameRate.ToString();
						vp[3] = tmp;
						command = String.Format("-o \"{0}.mkv\"  --track-name \"0:{1}\" --forced-track 0:no --default-duration 0:{2}p -d 0 -A -S -T --no-global-tags --no-chapters ( \"{3}\\video.hvc\" ) ", vp);
						trackorder = "--track-order 0:0,";

						// Audio
						int id = 1;
						foreach (var item in System.IO.Directory.GetFiles(tmp, "audio*"))
						{
							string[] ap = new string[2];
							ap[0] = id.ToString();
							ap[1] = item;
							command += String.Format("--track-name \"0:Track {0}\" --forced-track 0:no --sync 0:-1 -a 0 -D -S -T --no-global-tags --no-chapters ( \"{1}\" ) ", ap);
							trackorder += id.ToString() + ":0,";
							id++;
						}

						// Subtitle
						if (IsSubtitleEnable)
						{
							string[] sp = new string[3];
							sp[0] = subtitle[x, 1]; //lang
							sp[1] = subtitle[x, 0]; //file name only
							sp[2] = subtitle[x, 2]; //full path
							command += String.Format("--language \"0:{0}\" --track-name \"0:{1}\" --forced-track 0:no -s 0 -D -A -T --no-global-tags --no-chapters ( \"{2}\" ) ", sp);
							trackorder += id.ToString() + ":0,";
						}

						// Remove last comma either by audio or subtitle.
						trackorder = trackorder.Remove(trackorder.Length - 1);

						// Build command for mkvmerge
						if (IsAttachEnable)
							command = command + trackorder + " " + attachment;
						else
							command = command + trackorder;

						// Send to mkvmerge
						PEC = StartProcess(Addons.BuildIn.MKV, command);
					}
					else
					{
						string command = "";
						int i = 0;
						// Video
						command = String.Format("-add \"{0}\\video.hvc#video:name={1}:fmt=HEVC:fps={2}\" ", tmp, Globals.AppInfo.WritingApp, video[0].frameRate.ToString());

						// Audio
						foreach (var item in System.IO.Directory.GetFiles(tmp, "*.mp4"))
						{
							command += String.Format("-add \"{0}#audio:name=Track {1}\" ", item, i.ToString());
							i++;
						}

						// Build command
						command += String.Format("\"{0}.mp4\"", FileOut);

						// Send to mp4box
						PEC = StartProcess(Addons.BuildIn.MP4, command);
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

				// Display total wasted time
				InvokeLog(Log.Info, "This queue take", CurrentQ);

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
			SI.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\cmd.exe";
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

			TaskManager.ProcessPerf(exe, args);

			P.WaitForExit();
			int X = P.ExitCode;
			P.Close();

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
				if (outputString.Contains("%]"))
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
				if (errorString.Contains("%]"))
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

		private void FormTitle(string s)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => this.Text = s));
			else
				this.Text = s;
		}

		private void InvokeLog(int status, string word, System.DateTime LastTime)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => PrintLog(status, String.Format("{0} {1}", word, Duration(LastTime)))));
			else
				PrintLog(status, String.Format("{0} {1}", word, Duration(LastTime)));
		}

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
			this.Text = Globals.AppInfo.Name;
			EncodingStarted(false);
		}

		public string Duration(System.DateTime pastTime)
		{
			// Calculate past time - present time and return the result.
			TimeSpan timeSpan = System.DateTime.Now.Subtract(pastTime);
			string returnTime = null;

			if (timeSpan.Days.ToString() != "0")
				returnTime = String.Format("{0}d {1}h {2}m {3}s", timeSpan.Days.ToString(), timeSpan.Hours.ToString(), timeSpan.Minutes.ToString(), timeSpan.Seconds.ToString());
			else if (timeSpan.Hours.ToString() != "0")
				returnTime = String.Format("{0}h {1}m {2}s", timeSpan.Hours.ToString(), timeSpan.Minutes.ToString(), timeSpan.Seconds.ToString());
			else if (timeSpan.Minutes.ToString() != "0")
				returnTime = String.Format("{0} min {1} sec", timeSpan.Minutes.ToString(), timeSpan.Seconds.ToString());
			else
				returnTime = timeSpan.Seconds.ToString() + " seconds";

			return returnTime;
		}

		private void EncodingStarted(bool x)
		{
			btnOptions.Enabled = !x;

			btnStart.Visible = !x;
			btnStop.Visible = x;
			btnPause.Visible = x;
			btnResume.Visible = x;

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
		}

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

			cboVideoPreset.SelectedIndex = Properties.Settings.Default.VideoPreset;
			cboVideoTune.SelectedIndex = Properties.Settings.Default.VideoTune;
			cboVideoRateCtrl.SelectedIndex = Properties.Settings.Default.VideoRateType;
			txtVideoRate.Text = Properties.Settings.Default.VideoRateValue.ToString();
			txtVideoAdvCmd.Text = Properties.Settings.Default.VideoCmd;

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

			Properties.Settings.Default.VideoPreset = cboVideoPreset.SelectedIndex;
			Properties.Settings.Default.VideoTune = cboVideoTune.SelectedIndex;
			Properties.Settings.Default.VideoRateType = cboVideoRateCtrl.SelectedIndex;
			Properties.Settings.Default.VideoRateValue = Convert.ToInt32(txtVideoRate.Text);
			Properties.Settings.Default.VideoCmd = txtVideoAdvCmd.Text;

			Properties.Settings.Default.AudioFormat = cboAudioFormat.SelectedIndex;
			Properties.Settings.Default.AudioBitRate = cboAudioBitRate.SelectedIndex;
			Properties.Settings.Default.AudioFreq = cboAudioFreq.SelectedIndex;
			Properties.Settings.Default.AudioChan = cboAudioChan.SelectedIndex;
			Properties.Settings.Default.AudioMode = cboAudioMode.SelectedIndex;

			Properties.Settings.Default.Save();
		}
		#endregion

		#region Interface Language Section (Load and Create)
		private void LoadLang()
		{
			string Path = Language.Path.Folder + "\\" + Language.Default + ".ini";

			if (!System.IO.File.Exists(Path))
				Language.Default = "eng";

			Path = Language.Path.Folder + "\\" + Language.Default + ".ini";

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Path);

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
			lstQueue.Columns[4].Text = data[Language.Section.Lst]["BitDepth"];
			lstQueue.Columns[5].Text = data[Language.Section.Lst]["Path"];
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
			System.IO.File.WriteAllText(Language.Path.FileENG, "");

			var parser = new FileIniDataParser();
			IniData data = parser.ReadFile(Language.Path.FileENG);

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

			parser.WriteFile(Language.Path.FileENG, data, System.Text.Encoding.Unicode);
		}
		#endregion

		public void PrintLog(int type, string message)
		{
			rtfLog.SelectionColor = Color.White;
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
	}
}
