using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ifme
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();

			// BackgroundWorkerEx Event
			bgThread.DoWork += bgThread_DoWork;
			bgThread.RunWorkerCompleted += bgThread_RunWorkerCompleted;

			Icon = Get.AppIcon;
            Text = Get.AppNameLongAdmin;
            FormBorderStyle = FormBorderStyle.Sizable;
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			InitializeUX();
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
            var tt = new ToolTip();
            tt.Show(null, btnAbout, 0);
            tt.IsBalloon = false;
            tt.ToolTipIcon = ToolTipIcon.Info;
            tt.ToolTipTitle = "Hello!";
            tt.SetToolTip(btnAbout, "");
			tt.Show(Language.Lang.ToolTipDonate, btnAbout, btnAbout.Width / 2, btnAbout.Height / 2, 30000);

			Get.IsReady = true;
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void pbxBanner_Resize(object sender, EventArgs e)
		{
			DrawBanner();
		}

		private void btnMediaFileOpen_Click(object sender, EventArgs e)
		{
			Button btnSender = (Button)sender;
			Point ptLowerLeft = new Point(1, btnSender.Height);
			ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
			cmsNewImport.Show(ptLowerLeft);
		}

        private void tsmiNew_Click(object sender, EventArgs e)
        {
            var frm = new frmInputBox(Language.Lang.InputBoxNewMedia.Title, Language.Lang.InputBoxNewMedia.Message, 1);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(frm.ReturnValue))
                    return;

                if (string.IsNullOrWhiteSpace(frm.ReturnValue))
                    return;

                var queue = new MediaQueue
                {
                    Enable = true,
                    FilePath = frm.ReturnValue + ".new",
                    OutputFormat = "mkv",
                };

                MediaQueueAdd(queue);
            }
        }

        private void tsmiImport_Click(object sender, EventArgs e)
		{
			foreach (var item in OpenFiles(MediaType.VideoAudio))
				AddMedia(item);

			MediaSelect();
		}

        private void tsmiImportFolder_Click(object sender, EventArgs e)
        {
            var files = Get.FilesRecursive();
            var frm = new frmProgressBar();

            if (files.Count == 0)
                return;

            frm.Show();
            frm.Text = Language.Lang.PleaseWait;
            frm.Status = Language.Lang.ReadProjectFile;

            var thread = new BackgroundWorker();

            thread.DoWork += delegate (object o, DoWorkEventArgs r)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    AddMedia(files[i]);

                    if (InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            frm.Progress = (int)(((float)(i + 1) / files.Count) * 100.0);
                            frm.Status = string.Format(Language.Lang.ProgressBarImport.Message, i + 1, files.Count, files[i]);
                            frm.Text = Language.Lang.ProgressBarImport.Title + $": {frm.Progress}%";

                            //Application.DoEvents();
                        }));
                    }
                }
            };

            thread.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs r)
            {
                frm.Close();
            };

            thread.RunWorkerAsync();
        }

        private void tsmiProjectNew_Click(object sender, EventArgs e)
        {
            if (lstMedia.Items.Count > 0)
            {
                var msg = MessageBox.Show(Language.Lang.MsgNewProject.Message, Language.Lang.MsgNewProject.Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                switch (msg)
                {
                    case DialogResult.Yes:
                        tsmiProjectSave.PerformClick();
                        goto case DialogResult.No;

                    case DialogResult.No:
                        lstMedia.Items.Clear();
                        Text = Get.AppNameProject("New Project");
                        break;

                    default:
                        break;
                }
            }
        }

        private void tsmiProjectOpen_Click(object sender, EventArgs e)
        {
            var file = OpenFileProject();

            if (!string.IsNullOrEmpty(file))
                ProjectOpen(file);
        }

        private void tsmiProjectSave_Click(object sender, EventArgs e)
        {
            if (lstMedia.Items.Count > 0)
            {
                if (File.Exists(MediaProject.ProjectFile))
                {
                    ProjectSave(MediaProject.ProjectFile);
                    return;
                }

                tsmiProjectSaveAs.PerformClick(); // reuse code
            }
        }

        private void tsmiProjectSaveAs_Click(object sender, EventArgs e)
        {
            if (lstMedia.Items.Count > 0)
            {
                var sdf = new SaveFileDialog
                {
                    Filter = "IFME project file|*.ifp",
                    FilterIndex = 1,
                };

                if (sdf.ShowDialog() == DialogResult.OK)
                {
                    ProjectSave(sdf.FileName);
                }
            }
        }

        private void btnMediaFileDel_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstMedia.SelectedItems)
				item.Remove();
		}

		private void btnOption_Click(object sender, EventArgs e)
		{
			new frmOption().ShowDialog();
		}

		private void btnAbout_Click(object sender, EventArgs e)
		{
			new frmAbout().ShowDialog();
		}

		private void btnDonate_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U");
		}

		private void btnMediaMoveUp_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Media, Direction.Up);
		}

		private void btnMediaMoveDown_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Media, Direction.Down);
		}

		private void btnDonePowerOff_Click(object sender, EventArgs e)
		{
			new frmShutdown().ShowDialog();
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
            if (lstMedia.Items.Count == 0)
                return;

            if (Properties.Settings.Default.ShutdownType == 1 || Properties.Settings.Default.ShutdownType == 2)
			{
				var msg = MessageBox.Show(Language.Lang.MsgBoxShutdown.Message, Language.Lang.MsgBoxShutdown.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (msg == DialogResult.No)
					return;
			}

			if (bgThread.IsBusy)
			{
				if (!btnPause.Enabled)
				{
					btnStart.Enabled = false;
					btnPause.Enabled = true;

					ProcessManager.Resume();
					return;
				}
			}
			else
			{
				// thread safe, make a copy
				// int = ListView Index
				// MediaQueue = ListView Tag
				var dict = new Dictionary<int, MediaQueue>();
				foreach (ListViewItem item in lstMedia.Items)
				{
                    dict.Add(item.Index, item.Tag as MediaQueue);
                    item.SubItems[4].Text = "Waiting...";
                }

                // check if all queue has enable hardsub
                if (!dict.All(x => !x.Value.HardSub))
                {
                    if (!Elevated.IsAdmin)
                    {
                        var msg = MessageBox.Show(Language.Lang.MsgHardSub.Message, Language.Lang.MsgHardSub.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                        if (msg == DialogResult.Yes)
                        {
                            var tempFile = Path.Combine(Path.GetTempPath(), $"ifme_elevated_{DateTime.Now:yyyy-MM-dd_hh-mm-ss}.pis");

                            ProjectSave(tempFile);

                            Elevated.RunAsAdmin(tempFile);

                            return;
                        }
                    }
                }

				bgThread.RunWorkerAsync(dict);

				btnStart.Enabled = false;
				btnPause.Enabled = true;
				btnStop.Enabled = true;
			}
		}

		private void btnPause_Click(object sender, EventArgs e)
		{
			if (btnPause.Enabled)
			{
				btnStart.Enabled = true;
				btnPause.Enabled = false;
				ProcessManager.Pause();
			}
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			if (bgThread.IsBusy)
			{
				bgThread.Abort();
				bgThread.Dispose();
			}
		}

		// this code quite special, multiple controls share one function
		private void ListViewItem_KeyDown(object sender, KeyEventArgs e)
		{
			var ctrl = (sender as ListView);

			if (e.KeyCode == Keys.A && e.Control)
			{
				foreach (ListViewItem item in ctrl.Items)
				{
					item.Selected = true;
				}
			}

			if (e.KeyCode == Keys.Delete)
			{
				foreach (ListViewItem item in ctrl.SelectedItems)
				{
					item.Remove();
				}
			}
		}

        private void lstMedia_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            (lstMedia.Items[e.Item.Index].Tag as MediaQueue).Enable = e.Item.Checked;
        }

        private void lstMedia_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var tf = !(lstMedia.SelectedItems.Count > 1);
				grpVideoStream.Enabled = tf;
				grpAudioStream.Enabled = tf;
				pnlSubtitle.Enabled = tf;
				pnlAttachment.Enabled = tf;

				MediaPopulate(lstMedia.SelectedItems[0].Tag as MediaQueue);
			}

			if (lstMedia.SelectedItems.Count == 0)
			{
				lstVideo.Items.Clear();
				lstAudio.Items.Clear();
				lstSub.Items.Clear();
				lstAttach.Items.Clear();
			}
		}

		private void lstMedia_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstMedia_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddMedia(file);
		}

        private void btnAdvanceCommand_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem queue in lstMedia.SelectedItems)
            {
                var media = queue.Tag as MediaQueue;

                var command = string.Empty;

                var ibtitle = string.Empty;
                var ibmsg = string.Empty;

                var type = 0; // 0 = decoder video, 1 = encoder video, 2 = decoder audio, 3 = encoder video
                var ctrl = sender as Button;

                if (string.Equals(ctrl.Name, btnVideoAdvDec.Name))
                {
                    if (media.Video.Count > 0)
                        command = media.Video[0].Quality.Command;

                    type = 0;
                    ibtitle = Language.Lang.InputBoxCommandLineFFmpeg.Title;
                    ibmsg = Language.Lang.InputBoxCommandLineFFmpeg.Message;
                }
                else if (string.Equals(ctrl.Name, btnVideoAdv.Name))
                {
                    if (media.Video.Count > 0)
                        command = media.Video[0].Encoder.Command;

                    type = 1;
                    ibtitle = Language.Lang.InputBoxCommandLine.Title;
                    ibmsg = Language.Lang.InputBoxCommandLine.Message;
                }
                else if (string.Equals(ctrl.Name, btnAudioAdvDec.Name))
                {
                    if (media.Audio.Count > 0)
                        command = media.Audio[0].Command;

                    type = 2;
                    ibtitle = Language.Lang.InputBoxCommandLineFFmpeg.Title;
                    ibmsg = Language.Lang.InputBoxCommandLineFFmpeg.Message;
                }
                else if (string.Equals(ctrl.Name, btnAudioAdv.Name))
                {
                    if (media.Audio.Count > 0)
                        command = media.Audio[0].Encoder.Command;

                    type = 3;
                    ibtitle = Language.Lang.InputBoxCommandLine.Title;
                    ibmsg = Language.Lang.InputBoxCommandLine.Message;
                }
                else
                {
                    type = 0;
                }

                // display
                var frm = new frmInputBox(ibtitle, ibmsg, command, 0);
                if (frm.ShowDialog() == DialogResult.OK)
                    command = frm.ReturnValue;

                // apply
                foreach (var video in media.Video)
                {
                    switch (type)
                    {
                        case 0:
                            video.Quality.Command = command;
                            break;
                        case 1:
                            video.Encoder.Command = command;
                            break;
                        default:
                            break;
                    }
                }

                foreach (var audio in media.Audio)
                {
                    switch (type)
                    {
                        case 2:
                            audio.Command = command;
                            break;
                        case 3:
                            audio.Encoder.Command = command;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void cboTargetFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMedia.SelectedItems.Count < 1)
                return;

            var format = ((KeyValuePair<string,string>)cboTargetFormat.SelectedItem).Key;

            foreach (ListViewItem q in lstMedia.SelectedItems)
            {
                var mf = q.Tag as MediaQueue;

                for (int i = 0; i < mf.Video.Count; i++)
                {
                    if (MediaValidator.IsOutFormatValid(format, mf.Video[i].Encoder.Id, true))
                    {

                    }
                    else
                    {
                        if (MediaValidator.GetCodecVideo(format, out var video))
                        {
                            mf.OutputFormat = format;
                            mf.Video[i].Encoder = video.Encoder;
                        }
                    }
                }

                for (int i = 0; i < mf.Audio.Count; i++)
                {
                    if (MediaValidator.IsOutFormatValid(format, mf.Audio[i].Encoder.Id, false))
                    {

                    }
                    else
                    {
                        if (MediaValidator.GetCodecAudio(format, out var audio))
                        {
                            mf.OutputFormat = format;
                            mf.Audio[i].Encoder = audio.Encoder;
                        }
                    }
                }

                MediaApply(sender, e);
            }
        }

        private void cboEncodingPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Get.IsReady)
			{
				if (lstMedia.SelectedItems.Count <= 0)
				{
					ConsoleEx.Write(LogLevel.Error, "Select one or more item before apply custom Encoding Preset.\n");
					return;
				}
			}

			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var m = q.Tag as MediaQueue;
				var p = MediaPreset.List[cboEncodingPreset.SelectedValue as string];

				m.OutputFormat = p.OutputFormat;

				foreach (var video in m.Video)
				{
                    video.Encoder = p.VideoEncoder;
					video.Quality = p.VideoQuality;
					video.DeInterlace = p.VideoDeInterlace;
				}

				foreach (var audio in m.Audio)
				{
					audio.Encoder = p.AudioEncoder;
                    audio.Command = p.AudioCommand;
				}
			}

			UXReloadMedia();
		}

		private void btnEncodingPresetSave_Click(object sender, EventArgs e)
		{
			Button btnSender = (Button)sender;
			Point ptLowerLeft = new Point(1, btnSender.Height);
			ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
			cmsEncodingPreset.Show(ptLowerLeft);
		}

		private void tsmiEncodingPresetSave_Click(object sender, EventArgs e)
		{
			EncodingPreset(cboEncodingPreset.SelectedValue as string, string.Empty);
		}

		private void tsmiEncodingPresetSaveAs_Click(object sender, EventArgs e)
		{
			var frm = new frmInputBox(Language.Lang.InputBoxEncodingPreset.Title, Language.Lang.InputBoxEncodingPreset.Message, cboEncodingPreset.Text, 4);

			if (frm.ShowDialog() == DialogResult.OK)
			{
				if (string.IsNullOrEmpty(frm.ReturnValue))
					return;

				if (string.IsNullOrWhiteSpace(frm.ReturnValue))
					return;

				EncodingPreset(frm.ReturnValue, frm.ReturnValue);
			}
		}

		private void txtFolderOutput_TextChanged(object sender, EventArgs e)
		{
            if (Get.IsValidPath(txtFolderOutput.Text))
                Get.FolderSave = txtFolderOutput.Text;
            else
                txtFolderOutput.Text = Get.FolderSave;
		}

		private void btnBrowseFolderOutput_Click(object sender, EventArgs e)
		{
			var GetDir = new FolderBrowserDialog();

			GetDir.ShowNewFolderButton = true;
			GetDir.RootFolder = Environment.SpecialFolder.MyComputer;

			if (GetDir.ShowDialog() == DialogResult.OK)
			{
				if (GetDir.SelectedPath[0] == '\\' && GetDir.SelectedPath[1] == '\\')
				{
					MessageBox.Show(Language.Lang.MsgBoxSaveFolder.Message, Language.Lang.MsgBoxSaveFolder.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				txtFolderOutput.Text = GetDir.SelectedPath;

				Properties.Settings.Default.OutputDir = GetDir.SelectedPath;
				Properties.Settings.Default.Save();
			}
		}

		private void tabMediaConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				switch ((sender as TabControl).SelectedIndex)
				{
					case 0:
						break;
					case 1:
						if (lstVideo.Items.Count > 0)
						{
							lstVideo.SelectedIndices.Clear();
							lstVideo.Items[0].Selected = true;
						}
						break;
					case 2:
						if (lstAudio.Items.Count > 0)
						{
							lstAudio.SelectedIndices.Clear();
							lstAudio.Items[0].Selected = true;
						}
						break;
					case 3:
						if (lstSub.Items.Count > 0)
						{
							lstSub.SelectedIndices.Clear();
							lstSub.Items[0].Selected = true;
						}
						break;
					case 4:
						if (lstAttach.Items.Count > 0)
						{
							lstAttach.SelectedIndices.Clear();
							lstAttach.Items[0].Selected = true;
						}
						break;
					default:
						break;
				}
			}
		}

		// Video
		private void btnVideoAdd_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Video))
					AddVideo(item);

			UXReloadMedia();
		}
		private void btnVideoDel_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstVideo.SelectedItems)
				{
					var id = item.Index;
					item.Remove();
					(lstMedia.SelectedItems[0].Tag as MediaQueue).Video.RemoveAt(id);
				}
			}
		}

		private void lstVideo_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstVideo_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddVideo(file);

			UXReloadMedia();
		}

		private void btnVideoMoveUp_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Video, Direction.Up);
		}

		private void btnVideoMoveDown_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Video, Direction.Down);
		}

		private void lstVideo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstVideo.SelectedItems.Count > 0)
				{
					var t = new Thread(MediaPopulateVideo);
					t.Start((lstMedia.SelectedItems[0].Tag as MediaQueue).Video[lstVideo.SelectedItems[0].Index]);
				}
			}
		}

		private void cboVideoEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboVideoEncoder.SelectedIndex <= -1)
				return;

			var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

			if (Plugin.Items.TryGetValue(key, out var temp))
			{
                if (MediaValidator.IsOutFormatValid((string)cboTargetFormat.SelectedValue, temp.GUID, true))
				{
					var video = temp.Video;

					cboVideoBitDepth.Items.Clear();
					foreach (var item in video.Encoder)
						cboVideoBitDepth.Items.Add(item.BitDepth);
					cboVideoBitDepth.SelectedIndex = 0;

					cboVideoPreset.Items.Clear();
					cboVideoPreset.Items.AddRange(video.Preset);
					cboVideoPreset.SelectedItem = video.PresetDefault;

					cboVideoTune.Items.Clear();
					cboVideoTune.Items.AddRange(video.Tune);
					cboVideoTune.SelectedItem = video.TuneDefault;

					cboVideoRateControl.Items.Clear();
					foreach (var item in video.Mode)
						cboVideoRateControl.Items.Add(item.Name);
					cboVideoRateControl.SelectedIndex = 0;

					var dei = temp.Video.Args.Pipe;
					chkVideoDeinterlace.Enabled = dei;
					grpVideoInterlace.Enabled = dei;

                    cboVideoPreset.Enabled = cboVideoPreset.Items.Count > 0;
                    cboVideoTune.Enabled = cboVideoTune.Items.Count > 0;
                }
				else
				{
					MessageBox.Show(Language.Lang.MsgBoxCodecIncompatible.Message, Language.Lang.MsgBoxCodecIncompatible.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (MediaValidator.GetCodecVideo((string)cboTargetFormat.SelectedValue, out var video))
                        cboVideoEncoder.SelectedValue = video.Encoder.Id;
				}

				btnVideoAdvDec.Enabled = temp.Video.Args.Pipe;
			}
		}

		private void cboVideoRateControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			var temp = new Plugin();
			var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
			var id = cboVideoRateControl.SelectedIndex;

			if (id >= 0)
			{
				if (Plugin.Items.TryGetValue(key, out temp))
				{
					var mode = temp.Video.Mode[id];

					nudVideoRateFactor.DecimalPlaces = mode.Value.DecimalPlace;
					nudVideoRateFactor.Minimum = mode.Value.Min;
					nudVideoRateFactor.Maximum = mode.Value.Max;
					nudVideoRateFactor.Value = mode.Value.Default;
					nudVideoRateFactor.Increment = mode.Value.Step;
					nudVideoMultiPass.Enabled = mode.MultiPass;
					nudVideoMultiPass.Value = 2;
				}

			}
		}

		private void cboVideoResolution_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"(^\d{1,5}x\d{1,5}$)|^auto$");
			MatchCollection matches = regex.Matches(cboVideoResolution.Text);

			if (matches.Count == 0)
			{
				cboVideoResolution.Text = "1280x720";
			}
		}

		private void cboVideoFrameRate_TextChanged(object sender, EventArgs e)
		{
			Regex regex = new Regex(@"(^\d+$)|(^\d+.\d+$)|(^auto$)");
			MatchCollection matches = regex.Matches(cboVideoFrameRate.Text);

			if (matches.Count == 0)
			{
				cboVideoFrameRate.Text = "23.976";
			}
		}

		private void chkVideoDeinterlace_CheckedChanged(object sender, EventArgs e)
		{
			var c = chkVideoDeinterlace.Checked;
			lblVideoDeinterlaceMode.Enabled = c;
			cboVideoDeinterlaceMode.Enabled = c;
			lblVideoDeinterlaceField.Enabled = c;
			cboVideoDeinterlaceField.Enabled = c;

			if (cboVideoDeinterlaceMode.SelectedIndex < 0)
				cboVideoDeinterlaceMode.SelectedIndex = 1;

			if (cboVideoDeinterlaceField.SelectedIndex < 0)
				cboVideoDeinterlaceField.SelectedIndex = 0;
		}

		// Audio
		private void btnAudioAdd_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Audio))
					AddAudio(item);

			UXReloadMedia();
		}

		private void btnAudioDel_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstAudio.SelectedItems)
				{
					var id = item.Index;
					item.Remove();
					(lstMedia.SelectedItems[0].Tag as MediaQueue).Audio.RemoveAt(id);
				}
			}
		}

		private void btnAudioMoveUp_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Audio, Direction.Up);
		}

		private void btnAudioMoveDown_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Audio, Direction.Down);
		}

		private void lstAudio_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstAudio_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddAudio(file);

			UXReloadMedia();
		}

		private void lstAudio_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstAudio.SelectedItems.Count > 0)
				{
					var t = new Thread(MediaPopulateAudio);
					t.Start((lstMedia.SelectedItems[0].Tag as MediaQueue).Audio[lstAudio.SelectedItems[0].Index]);
				}
			}
		}

		private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboAudioEncoder.SelectedIndex <= -1)
				return;

			var temp = new Plugin();
			var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

			if (Plugin.Items.TryGetValue(key, out temp))
			{
				if (MediaValidator.IsOutFormatValid((string)cboTargetFormat.SelectedValue, temp.GUID, false))
				{
					var audio = temp.Audio;

					cboAudioMode.Items.Clear();
					foreach (var item in audio.Mode)
						cboAudioMode.Items.Add(item.Name);
					cboAudioMode.SelectedIndex = 0;

					cboAudioQuality.Items.Clear();
					foreach (var item in audio.Mode[0].Quality)
						cboAudioQuality.Items.Add(item);
					cboAudioQuality.SelectedItem = audio.Mode[0].Default;

					cboAudioSampleRate.Items.Clear();
					foreach (var item in audio.SampleRate)
						cboAudioSampleRate.Items.Add(item);
					cboAudioSampleRate.SelectedItem = audio.SampleRateDefault;

                    // Channel
                    cboAudioChannel.DataSource = null;
                    cboAudioChannel.Items.Clear();

                    var ch = new Dictionary<int, string>();
                    foreach (var item in audio.Channel)
                    {
                        switch (item)
                        {
                            case 0:
                                ch.Add(0, "Auto");
                                break;
                            case 1:
                                ch.Add(1, "Mono");
                                break;
                            case 2:
                                ch.Add(2, "Stereo");
                                break;
                            case 6:
                                ch.Add(6, "5.1 Surround");
                                break;
                            case 8:
                                ch.Add(8, "7.1 Surround");
                                break;
                            default:
                                break;
                        }
                    }
                    cboAudioChannel.DisplayMember = "Value";
                    cboAudioChannel.ValueMember = "Key";
                    cboAudioChannel.DataSource = new BindingSource(ch, null);

                    cboAudioChannel.SelectedValue = audio.ChannelDefault;
				}
				else
				{
					MessageBox.Show(Language.Lang.MsgBoxCodecIncompatible.Message, Language.Lang.MsgBoxCodecIncompatible.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (MediaValidator.GetCodecAudio((string)cboTargetFormat.SelectedValue, out var audio))
					    cboAudioEncoder.SelectedValue = audio.Encoder.Id;
				}

				btnAudioAdvDec.Enabled = temp.Audio.Args.Pipe;
			}
		}

		private void cboAudioMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			var temp = new Plugin();
			var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
			var id = cboAudioMode.SelectedIndex;

			if (Plugin.Items.TryGetValue(key, out temp))
			{
				var mode = temp.Audio.Mode[id];

				cboAudioQuality.Items.Clear();
				foreach (var item in mode.Quality)
					cboAudioQuality.Items.Add(item);
				cboAudioQuality.SelectedItem = mode.Default;
			}
		}

		// Subtitle
		private void btnSubAdd_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Subtitle))
					AddSubtitle(item);

			UXReloadMedia();
		}

		private void btnSubAdd2_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Video))
					AddSubtitle2(item);

			UXReloadMedia();
		}

		private void btnSubDel_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstSub.SelectedItems)
				{
					var id = item.Index;
					item.Remove();
					(lstMedia.SelectedItems[0].Tag as MediaQueue).Subtitle.RemoveAt(id);
				}
			}
		}

		private void btnSubMoveUp_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Subtitle, Direction.Up);
		}

		private void btnSubMoveDown_Click(object sender, EventArgs e)
		{
			ListViewItemMove(ListViewItemType.Subtitle, Direction.Down);
		}

        private void chkSubHard_CheckedChanged(object sender, EventArgs e)
        {
            var ctrl = sender as CheckBox;

            if (lstSub.Items.Count == 0)
            {
                ctrl.Checked = false;
                return;
            }

            if (ctrl.Checked)
            {
                var tt = new ToolTip();

                tt.Show(null, ctrl, 0);
                tt.IsBalloon = true;
                tt.ToolTipIcon = ToolTipIcon.Warning;
                tt.ToolTipTitle = Language.Lang.Warning;
                tt.SetToolTip(ctrl, ctrl.Text);
                tt.Show(Language.Lang.ToolTipHardSub, ctrl, ctrl.Width - 10, ctrl.Height - 10, 10000);
            }
        }

        private void lstSub_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstSub_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddSubtitle(file);

			UXReloadMedia();
		}

		private void lstSub_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstSub.SelectedItems.Count > 0)
				{
					var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;
					var index = lstSub.SelectedItems[0].Index;
					cboSubLang.SelectedValue = media.Subtitle[index].Lang;
				}
			}
		}

		private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstSub.SelectedItems.Count > 0)
				{
					foreach (ListViewItem item in lstSub.SelectedItems)
					{
						item.SubItems[2].Text = cboSubLang.Text;
					}
				}
			}
		}

		// Attachment
		private void btnAttachAdd_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Attachment))
					AddAttachment(item);

			UXReloadMedia();
		}

		private void btnAttchAdd2_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Video))
					AddAttachment2(item);

			UXReloadMedia();
		}

		private void btnAttachDel_Click(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				foreach (ListViewItem item in lstAttach.SelectedItems)
				{
					var id = item.Index;
					item.Remove();
					(lstMedia.SelectedItems[0].Tag as MediaQueue).Attachment.RemoveAt(id);
				}
			}
		}

		private void lstAttach_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstAttach_DragEnter(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				AddAttachment(file);

			UXReloadMedia();
		}

		private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstAttach.SelectedItems.Count > 0)
				{
					var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;
					var index = lstAttach.SelectedItems[0].Index;
					cboAttachMime.Text = media.Attachment[index].Mime;
				}
			}
		}

		private void cboAttachMime_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				if (lstSub.SelectedItems.Count > 0)
				{
					foreach (ListViewItem item in lstAttach.SelectedItems)
					{
						item.SubItems[1].Text = cboAttachMime.Text;
					}
				}
			}
		}

		private void chkAdvTrim_CheckedChanged(object sender, EventArgs e)
		{
			grpAdvTrim.Enabled = chkAdvTrim.Checked;
		}

		private void txtAdvTrim_Leave(object sender, EventArgs e)
		{
			var ctrl = sender as MaskedTextBox;
			var time = ctrl.Text.Split(':');

			if (time.Length != 3)
				return;

			var hour = 0;
			var min = 0;
			var sec = 0;

			int.TryParse(time[0], out hour);
			int.TryParse(time[1], out min);
			int.TryParse(time[2], out sec);

			if (min >= 60)
				min = 59;

			if (sec >= 60)
				sec = 59;

			ctrl.Text = $"{hour:00}:{min:00}:{sec:00}";

			var start = new TimeSpan(0, 0, 0);
			var end = new TimeSpan(0, 0, 0);
			var du = new TimeSpan(0, 0, 0);

			try
			{
				var st = mtxAdvTimeStart.Text.Split(':');
				var et = mtxAdvTimeEnd.Text.Split(':');
				var dt = mtxAdvTimeDuration.Text.Split(':');

				int sth = 0, stm = 0, sts = 0;
				int eth = 0, etm = 0, ets = 0;
				int dth = 0, dtm = 0, dts = 0;

				int.TryParse(st[0], out sth);
				int.TryParse(st[1], out stm);
				int.TryParse(st[2], out sts);

				int.TryParse(et[0], out eth);
				int.TryParse(et[1], out etm);
				int.TryParse(et[2], out ets);

				int.TryParse(dt[0], out dth);
				int.TryParse(dt[1], out dtm);
				int.TryParse(dt[2], out dts);

				start = new TimeSpan(sth, stm, sts);
				end = new TimeSpan(eth, etm, ets);
				du = new TimeSpan(dth, dtm, dts);

				if (string.Equals(ctrl.Name, mtxAdvTimeEnd.Name) || 
					string.Equals(ctrl.Name, mtxAdvTimeStart.Name))
				{
					du = end.Subtract(start);
					mtxAdvTimeDuration.Text = $"{du.Hours:00}:{du.Minutes:00}:{du.Seconds:00}";
				}

				if (string.Equals(ctrl.Name, mtxAdvTimeDuration.Name))
				{
					end = start.Add(du);
					mtxAdvTimeEnd.Text = $"{end.Hours:00}:{end.Minutes:00}:{end.Seconds:00}";
				}
			}
			catch
			{

			}

			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var m = q.Tag as MediaQueue;

				m.Trim.Enable = chkAdvTrim.Checked;
				m.Trim.Start = $"{start.Hours:00}:{start.Minutes:00}:{start.Seconds:00}";
				m.Trim.End = $"{end.Hours:00}:{end.Minutes:00}:{end.Seconds:00}";
				m.Trim.Duration = $"{du.Hours:00}:{du.Minutes:00}:{du.Seconds:00}";
			}
		}
    }
}
