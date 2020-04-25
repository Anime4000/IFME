﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace IFME
{
	public partial class frmMain : Form
	{
		private BackgroundWorker2 bgThread = new BackgroundWorker2();

		public frmMain()
		{
			InitializeComponent();
			InitializeFonts();
			InitializeLog();
			Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
			Text = "Internet Friendly Media Encoder 20.02";
			FormBorderStyle = FormBorderStyle.Sizable;

			bgThread.DoWork += bgThread_DoWork;
			bgThread.ProgressChanged += bgThread_ProgressChanged;
			bgThread.RunWorkerCompleted += bgThread_RunWorkerCompleted;
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			new PluginsLoad();
			new ProfilesManager();

			cboVideoRes.SelectedIndex = 9;
			cboVideoFps.SelectedIndex = 5;
			cboVideoPixFmt.SelectedIndex = 0;
			cboVideoDeInterMode.SelectedIndex = 1;
			cboVideoDeInterField.SelectedIndex = 0;
			
			cboAudioEncoder.DataSource = new BindingSource(Plugins.Items.Audio.ToDictionary(p => p.Key, p => p.Value.Name), null);
			cboAudioEncoder.DisplayMember = "Value";
			cboAudioEncoder.ValueMember = "Key";
			cboAudioEncoder.SelectedValue = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac");

			cboVideoEncoder.DataSource = new BindingSource(Plugins.Items.Video.ToDictionary(p => p.Key, p => p.Value.Name), null);
			cboVideoEncoder.DisplayMember = "Value";
			cboVideoEncoder.ValueMember = "Key";
			cboVideoEncoder.SelectedValue = new Guid("deadbeef-0265-0265-0265-026502650265");

			cboVideoLang.DataSource = new BindingSource(Language.Codes, null);
			cboVideoLang.DisplayMember = "Value";
			cboVideoLang.ValueMember = "Key";
			cboVideoLang.SelectedValue = "eng";

			cboAudioLang.DataSource = new BindingSource(Language.Codes, null);
			cboAudioLang.DisplayMember = "Value";
			cboAudioLang.ValueMember = "Key";
			cboAudioLang.SelectedValue = "eng";

			cboSubLang.DataSource = new BindingSource(Language.Codes, null);
			cboSubLang.DisplayMember = "Value";
			cboSubLang.ValueMember = "Key";
			cboSubLang.SelectedValue = "eng";

			cboAttachMime.DataSource = new BindingSource(Mime.Codes, null);
			cboAttachMime.DisplayMember = "Value";
			cboAttachMime.ValueMember = "Key";
			cboAttachMime.SelectedValue = ".ttf";

			cboFormat.SelectedIndex = 3;

			cboProfile.Items.Clear();
			foreach (var item in Profiles.Items)
			{
				cboProfile.Items.Add(item.ProfileName);
			}
			cboProfile.SelectedIndex = 0;

			if (Properties.Settings.Default.OutputFolder.IsDisable())
			{
				Properties.Settings.Default.OutputFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
				Properties.Settings.Default.Save();
			}

			txtOutputPath.Text = Properties.Settings.Default.OutputFolder;
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{

		}

		private void frmMain_SizeChanged(object sender, EventArgs e)
		{
			var w = PbxBanner.Width;
			var h = PbxBanner.Height;

			var b1 = Properties.Resources.Banner_2a;
			var b2 = Properties.Resources.Banner_2b;
			var iW = b1.Width;
			var iH = b1.Height;

			var p = h / (double)iH;

			var nW = iW * p;
			var nH = iH * p;

			var bL = Images.Resize(b1, (int)nW, (int)nH);
			var bR = Images.Resize(b2, (int)nW, (int)nH);
			try
			{
				if (w <= 0 || h <= 0)
					return;

				using (var png = new Bitmap(w, h, PixelFormat.Format32bppArgb))
				{
					using (var img = Graphics.FromImage(png))
					{
						img.DrawImage(new Bitmap(bL), 0, 0);
						img.DrawImage(new Bitmap(bR), w - (int)nW, 0);
					}

					PbxBanner.BackgroundImage = new Bitmap(png);
				}
			}
			catch (Exception)
			{

			}
		}

		private void btnFileAdd_Click(object sender, EventArgs e)
		{
			foreach (var item in OpenFiles(MediaType.Video | MediaType.Audio))
				MediaFileListAdd(item);
		}

		private void btnFileAdd_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				var btnSender = (Button)sender;
				var ptLowerLeft = new Point(1, btnSender.Height);
				ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
				cmsFileAdd.Show(ptLowerLeft);
			}
		}

		private void btnFileDelete_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstFile.SelectedItems)
				item.Remove();
		}

		private void btnOptions_Click(object sender, EventArgs e)
		{

		}

		private void btnFileUp_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
			{
				ListViewItem selected = lstFile.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstFile.Items.Count;

				if (indx == 0)
				{
					lstFile.Items.Remove(selected);
					lstFile.Items.Insert(totl - 1, selected);
				}
				else
				{
					lstFile.Items.Remove(selected);
					lstFile.Items.Insert(indx - 1, selected);
				}
			}
		}

		private void btnFileDown_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
			{
				ListViewItem selected = lstFile.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstFile.Items.Count;

				if (indx == totl - 1)
				{
					lstFile.Items.Remove(selected);
					lstFile.Items.Insert(0, selected);
				}
				else
				{
					lstFile.Items.Remove(selected);
					lstFile.Items.Insert(indx + 1, selected);
				}
			}
		}

		private void btnDonate_Click(object sender, EventArgs e)
		{

		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (lstFile.Items.Count > 0)
			{
				if (bgThread.IsBusy)
				{
					if (ProcessManager.IsPause)
					{
						ProcessManager.Resume();
						btnStart.Text = Fonts.fa.pause;
					}
					else
					{
						ProcessManager.Pause();
						btnStart.Text = Fonts.fa.play;
					}
				}
				else
				{
					btnStart.Text = Fonts.fa.pause;
					tabConfig.SelectedTab = tabConfigLog;

					var data = new Dictionary<int, MediaQueue>();
					foreach (ListViewItem item in lstFile.Items)
					{
						data.Add(item.Index, item.Tag as MediaQueue);
						item.SubItems[4].Text = "Waiting . . .";
					}

					bgThread.RunWorkerAsync(data);
				}
			}
		}

		private void btnStop_Click(object sender, EventArgs e)
		{
			if (bgThread.IsBusy)
			{
				bgThread.Abort();
				bgThread.Dispose();
				ProcessManager.Stop();
			}
		}

		private void lstFile_ItemChecked(object sender, ItemCheckedEventArgs e)
		{

		}

		private void lstFile_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
			{
				var data = lstFile.SelectedItems[0].Tag as MediaQueue;

				// Format
				cboFormat.SelectedIndex = (int)data.OutputFormat;

				// Video
				lstVideo.SelectedItems.Clear();
				lstVideo.Items.Clear();
				foreach (var item in data.Video)
				{
					var res = "original res";
					var fps = "original fps";

					if (item.Quality.Width != 0 && item.Quality.Height != 0)
						res = $"{item.Quality.Width}x{item.Quality.Height}";
					
					if (item.Quality.FrameRate != 0)
						fps = $"{item.Quality.FrameRate}fps";

					lstVideo.Items.Add(new ListViewItem(new[]
					{
						$"{item.Id}",
						item.Lang,
						$"{res}, {fps}"
					})
					{
						Checked = true,
						Tag = item
					});
				}

				// Audio
				lstAudio.SelectedItems.Clear();
				lstAudio.Items.Clear();
				foreach (var item in data.Audio)
				{
					lstAudio.Items.Add(new ListViewItem(new[]
					{
						$"{item.Id}",
						item.Lang,
						$"{item.Encoder.Channel}ch x {item.Encoder.SampleRate}Hz"
					})
					{
						Checked = true,
						Tag = item
					});
				}


				// Subtitle
				lstSub.SelectedItems.Clear();
				lstSub.Items.Clear();
				foreach (var item in data.Subtitle)
				{
					lstSub.Items.Add(new ListViewItem(new[]
					{
						$"{item.Id}",
						Path.GetFileName(item.File),
						Language.FullName(item.Lang)
					})
					{
						Checked = true,
						Tag = item
					});
				}

				// Attachment
				lstAttach.SelectedItems.Clear();
				lstAttach.Items.Clear();
				foreach (var item in data.Attachment)
				{
					lstAttach.Items.Add(new ListViewItem(new[]
					{
						$"{item.Id}",
						Path.GetFileName(item.File),
						item.Mime
					})
					{
						Checked = true
					});
				}

				if (data.Video.Count > 0)
					lstVideo.Items[0].Selected = true;
				if (data.Audio.Count > 0)
					lstAudio.Items[0].Selected = true;
				if (data.Subtitle.Count > 0)
					lstSub.Items[0].Selected = true;
				if (data.Attachment.Count > 0)
					lstAttach.Items[0].Selected = true;
			}
		}

		private void lstFile_DragDrop(object sender, DragEventArgs e)
		{
			foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
				MediaFileListAdd(file);
		}

		private void lstFile_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void btnVideoAdd_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Video))
					MediaVideoListAdd(item);
		}

		private void btnVideoDel_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstVideo.SelectedItems)
				item.Remove();
		}

		private void btnVideoMoveUp_Click(object sender, EventArgs e)
		{
			if (lstVideo.SelectedItems.Count > 0)
			{
				// Copy
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video;
				for (int i = 0; i < data.Count; i++)
					lstVideo.Items[i].Tag = data[i];

				ListViewItem selected = lstVideo.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstVideo.Items.Count;

				if (indx == 0)
				{
					lstVideo.Items.Remove(selected);
					lstVideo.Items.Insert(totl - 1, selected);
				}
				else
				{
					lstVideo.Items.Remove(selected);
					lstVideo.Items.Insert(indx - 1, selected);
				}

				// Paste
				for (int i = 0; i < data.Count; i++)
					data[i] = lstVideo.Items[i].Tag as MediaQueueVideo;
			}
		}

		private void btnVideoMoveDown_Click(object sender, EventArgs e)
		{
			if (lstVideo.SelectedItems.Count > 0)
			{
				// Copy
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video;
				for (int i = 0; i < data.Count; i++)
					lstVideo.Items[i].Tag = data[i];

				ListViewItem selected = lstVideo.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstVideo.Items.Count;

				if (indx == totl - 1)
				{
					lstVideo.Items.Remove(selected);
					lstVideo.Items.Insert(0, selected);
				}
				else
				{
					lstVideo.Items.Remove(selected);
					lstVideo.Items.Insert(indx + 1, selected);
				}

				// Paste
				for (int i = 0; i < data.Count; i++)
					data[i] = lstVideo.Items[i].Tag as MediaQueueVideo;
			}
		}

		private void lstVideo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstVideo.SelectedItems.Count > 0)
			{
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[lstVideo.SelectedItems[0].Index];
				new Thread(MediaShowDataVideo).Start(data);
			}
		}

		private void lstVideo_DragDrop(object sender, DragEventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
					MediaVideoListAdd(file);
		}

		private void lstVideo_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void cboVideoLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Lang = $"{cboVideoLang.SelectedValue}";
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Lang = $"{cboVideoLang.SelectedValue}";
						}
					}
				}

				MediaShowReList();
			}
		}

		private void cboVideoEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboVideoEncoder.SelectedIndex <= -1)
				return;

			var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

			if (Plugins.Items.Video.TryGetValue(key, out var temp))
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

				nudVideoRateFactor.Minimum = video.Mode[0].Value.Min;
				nudVideoRateFactor.Maximum = video.Mode[0].Value.Max;
				nudVideoRateFactor.Value = video.Mode[0].Value.Default;
				nudVideoMultiPass.Value = 2;

				var dei = temp.Video.Args.Pipe;
				chkVideoDeInterlace.Enabled = dei;
				grpVideoInterlace.Enabled = dei;

				cboVideoPreset.Enabled = cboVideoPreset.Items.Count > 0;
				cboVideoTune.Enabled = cboVideoTune.Items.Count > 0;

				if ((sender as Control).Focused)
				{
					if (lstFile.SelectedItems.Count == 1)
					{
						foreach (ListViewItem item in lstVideo.SelectedItems)
						{
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Id = new Guid(cboVideoEncoder.SelectedValue.ToString());
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Preset = cboVideoPreset.Text;
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Tune = cboVideoTune.Text;
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Mode = cboVideoRateControl.SelectedIndex;
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Value = nudVideoRateFactor.Value;
							(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.MultiPass = (int)nudVideoMultiPass.Value;
						}
					}
					else if (lstFile.SelectedItems.Count > 1)
					{
						foreach (ListViewItem queue in lstFile.SelectedItems)
						{
							foreach (var d in (queue.Tag as MediaQueue).Video)
							{
								d.Encoder.Id = new Guid(cboVideoEncoder.SelectedValue.ToString());
								d.Encoder.Preset = cboVideoPreset.Text;
								d.Encoder.Tune = cboVideoTune.Text;
								d.Encoder.Mode = cboVideoRateControl.SelectedIndex;
								d.Encoder.Value = nudVideoRateFactor.Value;
								d.Encoder.MultiPass = (int)nudVideoMultiPass.Value;
							}
						}
					}
				}
			}
		}

		private void cboVideoPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Preset = cboVideoPreset.Text;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder.Preset = cboVideoPreset.Text;
						}
					}
				}
			}
		}

		private void cboVideoTune_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Tune = cboVideoTune.Text;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder.Tune = cboVideoTune.Text;
						}
					}
				}
			}
		}

		private void cboVideoRateControl_SelectedIndexChanged(object sender, EventArgs e)
		{
			var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
			var id = cboVideoRateControl.SelectedIndex;

			if (id >= 0)
			{
				if (Plugins.Items.Video.TryGetValue(key, out var temp))
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

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Mode = cboVideoRateControl.SelectedIndex;
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Value = nudVideoRateFactor.Value;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder.Mode = cboVideoRateControl.SelectedIndex;
							item.Encoder.Value = nudVideoRateFactor.Value;
						}
					}
				}
			}
		}

		private void nudVideoRateFactor_ValueChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Value = nudVideoRateFactor.Value;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder.Value = nudVideoRateFactor.Value;
						}
					}
				}
			}
		}

		private void nudVideoMultiPass_ValueChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.MultiPass = (int)nudVideoMultiPass.Value;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder.MultiPass = (int)nudVideoMultiPass.Value;
						}
					}
				}
			}
		}

		private void btnVideoDec_Click(object sender, EventArgs e)
		{

		}

		private void btnVideoEnc_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoRes_TextChanged(object sender, EventArgs e)
		{
			if (cboVideoRes.Text.Length > 0)
			{
				if (cboVideoRes.Text[0] == 'a')
					cboVideoRes.Text = "auto";

				if (cboVideoRes.Text[0] == '0')
					cboVideoRes.Text = "auto";
			}

			Regex regex = new Regex(@"(^\d{1,5}x\d{1,5}$)|^auto$");
			MatchCollection matches = regex.Matches(cboVideoRes.Text);

			if (matches.Count == 0)
				cboVideoRes.Text = "1280x720";

			var w = 0;
			var h = 0;
			var x = cboVideoRes.Text;
			if (x.Contains('x'))
			{
				int.TryParse(x.Split('x')[0], out w);
				int.TryParse(x.Split('x')[1], out h);
			}

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.Width = w;
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.Height = h;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Quality.Width = w;
							item.Quality.Height = h;
						}
					}
				}

				MediaShowReList();
			}
		}

		private void cboVideoFps_TextChanged(object sender, EventArgs e)
		{
			if (cboVideoFps.Text.Length > 0)
			{
				if (cboVideoFps.Text[0] == 'a')
					cboVideoFps.Text = "auto";

				if (cboVideoFps.Text[0] == '0')
					cboVideoFps.Text = "auto";
			}

			Regex regex = new Regex(@"(^\d+$)|(^\d+.\d+$)|(^auto$)");
			MatchCollection matches = regex.Matches(cboVideoFps.Text);

			if (matches.Count == 0)
				cboVideoFps.Text = "23.976";

			float fps = 23.796F;
			float.TryParse(cboVideoFps.Text, out fps);

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.FrameRate = fps;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Quality.FrameRate = fps;
						}
					}
				}

				MediaShowReList();
			}
		}

		private void cboVideoBitDepth_SelectedIndexChanged(object sender, EventArgs e)
		{
			int.TryParse((sender as Control).Text, out int x);

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.BitDepth = x;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Quality.BitDepth = x;
						}
					}
				}
			}
		}

		private void cboVideoPixFmt_SelectedIndexChanged(object sender, EventArgs e)
		{
			int.TryParse((sender as Control).Text, out int x);

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.PixelFormat = x;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Quality.PixelFormat = x;
						}
					}
				}
			}
		}

		private void chkVideoDeInterlace_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].DeInterlace.Enable = chkVideoDeInterlace.Checked;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.DeInterlace.Enable = chkVideoDeInterlace.Checked;
						}
					}
				}
			}
		}

		private void cboVideoDeInterMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].DeInterlace.Mode = cboVideoDeInterMode.SelectedIndex;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.DeInterlace.Mode = cboVideoDeInterMode.SelectedIndex;
						}
					}
				}
			}
		}

		private void cboVideoDeInterField_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstVideo.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].DeInterlace.Field = cboVideoDeInterField.SelectedIndex;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.DeInterlace.Field = cboVideoDeInterField.SelectedIndex;
						}
					}
				}
			}
		}

		private void btnAudioAdd_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Audio))
					MediaAudioListAdd(item);
		}

		private void btnAudioDel_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstAudio.SelectedItems)
				item.Remove();
		}

		private void btnAudioMoveUp_Click(object sender, EventArgs e)
		{
			if (lstAudio.SelectedItems.Count > 0)
			{
				// Copy to List
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio;
				for (int i = 0; i < data.Count; i++)
					lstAudio.Items[i].Tag = data[i];

				ListViewItem selected = lstAudio.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstAudio.Items.Count;

				if (indx == 0)
				{
					lstAudio.Items.Remove(selected);
					lstAudio.Items.Insert(totl - 1, selected);
				}
				else
				{
					lstAudio.Items.Remove(selected);
					lstAudio.Items.Insert(indx - 1, selected);
				}

				// Paste to Queue
				for (int i = 0; i < data.Count; i++)
					data[i] = lstAudio.Items[i].Tag as MediaQueueAudio;
			}
		}

		private void btnAudioMoveDown_Click(object sender, EventArgs e)
		{
			if (lstAudio.SelectedItems.Count > 0)
			{
				// Copy to List
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio;
				for (int i = 0; i < data.Count; i++)
					lstAudio.Items[i].Tag = data[i];

				ListViewItem selected = lstAudio.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstAudio.Items.Count;

				if (indx == totl - 1)
				{
					lstAudio.Items.Remove(selected);
					lstAudio.Items.Insert(0, selected);
				}
				else
				{
					lstAudio.Items.Remove(selected);
					lstAudio.Items.Insert(indx + 1, selected);
				}

				// Paste to Queue
				for (int i = 0; i < data.Count; i++)
					data[i] = lstAudio.Items[i].Tag as MediaQueueAudio;
			}
		}

		private void lstAudio_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstAudio.SelectedItems.Count > 0)
			{
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[lstAudio.SelectedItems[0].Index];
				new Thread(MediaShowDataAudio).Start(data);
			}
		}

		private void lstAudio_DragDrop(object sender, DragEventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
					MediaAudioListAdd(file);
		}

		private void lstAudio_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void cboAudioLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Lang = $"{cboAudioLang.SelectedValue}";
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Lang = $"{cboAudioLang.SelectedValue}";
						}
					}
				}

				MediaShowReList();
			}
		}

		private void chkAudioCopy_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Copy = chkAudioCopy.Checked;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Copy = chkAudioCopy.Checked;
						}
					}
				}
			}
		}

		private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboAudioEncoder.SelectedIndex <= -1)
				return;

			var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

			if (Plugins.Items.Audio.TryGetValue(key, out var temp))
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

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Id = new Guid($"{cboAudioEncoder.SelectedValue}");
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder.Id = new Guid($"{cboAudioEncoder.SelectedValue}");
						}
					}
				}
			}
		}

		private void cboAudioMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
			var id = cboAudioMode.SelectedIndex;

			if (Plugins.Items.Audio.TryGetValue(key, out var temp))
			{
				var mode = temp.Audio.Mode[id];

				cboAudioQuality.Items.Clear();
				foreach (var item in mode.Quality)
					cboAudioQuality.Items.Add(item);
				cboAudioQuality.SelectedItem = mode.Default;
			}

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Mode = cboAudioMode.SelectedIndex;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder.Mode = cboAudioMode.SelectedIndex;
						}
					}
				}
			}
		}

		private void cboAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
		{
			decimal.TryParse(cboAudioQuality.Text, out decimal q);

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Quality = q;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder.Quality = q;
						}
					}
				}
			}
		}

		private void cboAudioSampleRate_SelectedIndexChanged(object sender, EventArgs e)
		{
			int.TryParse(cboAudioSampleRate.Text, out int hz);

			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.SampleRate = hz;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder.SampleRate = hz;
						}
					}
				}

				MediaShowReList();
			}
		}

		private void cboAudioChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstAudio.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Channel = (int)cboAudioChannel.SelectedValue;
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder.Channel = (int)cboAudioChannel.SelectedValue;
						}
					}
				}

				MediaShowReList();
			}
		}

		private void btnAudioDec_Click(object sender, EventArgs e)
		{

		}

		private void btnAudioEnc_Click(object sender, EventArgs e)
		{

		}

		private void btnSubAdd_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Subtitle))
					MediaSubtitleListAdd(item);
		}

		private void btnSubDel_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstSub.SelectedItems)
				item.Remove();
		}

		private void btnSubMoveUp_Click(object sender, EventArgs e)
		{
			if (lstSub.SelectedItems.Count > 0)
			{
				// Copy to List
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle;
				for (int i = 0; i < data.Count; i++)
					lstSub.Items[i].Tag = data[i];

				ListViewItem selected = lstSub.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstSub.Items.Count;

				if (indx == 0)
				{
					lstSub.Items.Remove(selected);
					lstSub.Items.Insert(totl - 1, selected);
				}
				else
				{
					lstSub.Items.Remove(selected);
					lstSub.Items.Insert(indx - 1, selected);
				}

				// Paste to Queue
				for (int i = 0; i < data.Count; i++)
					data[i] = lstSub.Items[i].Tag as MediaQueueSubtitle;
			}
		}

		private void btnSubMoveDown_Click(object sender, EventArgs e)
		{
			if (lstSub.SelectedItems.Count > 0)
			{
				// Copy to List
				var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle;
				for (int i = 0; i < data.Count; i++)
					lstSub.Items[i].Tag = data[i];

				ListViewItem selected = lstSub.SelectedItems[0];
				int indx = selected.Index;
				int totl = lstSub.Items.Count;

				if (indx == totl - 1)
				{
					lstSub.Items.Remove(selected);
					lstSub.Items.Insert(0, selected);
				}
				else
				{
					lstSub.Items.Remove(selected);
					lstSub.Items.Insert(indx + 1, selected);
				}

				// Paste to Queue
				for (int i = 0; i < data.Count; i++)
					data[i] = lstSub.Items[i].Tag as MediaQueueSubtitle;
			}
		}

		private void chkSubHard_CheckedChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count > 0)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						(queue.Tag as MediaQueue).HardSub = chkSubHard.Checked;
					}
				}
			}
		}

		private void lstSub_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lstSub_DragDrop(object sender, DragEventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
					MediaSubtitleListAdd(file);
		}

		private void lstSub_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count == 1)
				{
					foreach (ListViewItem item in lstSub.SelectedItems)
					{
						(lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle[item.Index].Lang = $"{cboSubLang.SelectedValue}";
					}
				}
				else if (lstFile.SelectedItems.Count > 1)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						foreach (var item in (queue.Tag as MediaQueue).Subtitle)
						{
							item.Lang = $"{cboSubLang.SelectedValue}";
						}
					}
				}

				MediaShowReList();
			}
		}

		private void btnAttachAdd_Click(object sender, EventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Attachment))
					MediaAttachmentListAdd(item);
		}

		private void btnAttachDel_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstAttach.SelectedItems)
				item.Remove();
		}

		private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lstAttach_DragDrop(object sender, DragEventArgs e)
		{
			if (lstFile.SelectedItems.Count > 0)
				foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
					MediaAttachmentListAdd(file);
		}

		private void lstAttach_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void cboAttachMime_TextChanged(object sender, EventArgs e)
		{
			
		}

		private void chkAdvTrim_CheckedChanged(object sender, EventArgs e)
		{
			grpAdvTrim.Enabled = chkAdvTrim.Checked;
		}

		private void cboFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count > 0)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						(queue.Tag as MediaQueue).OutputFormat = (MediaContainer)cboFormat.SelectedIndex;
					}
				}
			}
		}

		private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
		{
			if ((sender as Control).Focused)
			{
				if (lstFile.SelectedItems.Count > 0)
				{
					foreach (ListViewItem queue in lstFile.SelectedItems)
					{
						var profile = Profiles.Items[cboProfile.SelectedIndex];

						(queue.Tag as MediaQueue).OutputFormat = profile.Container;

						foreach (var item in (queue.Tag as MediaQueue).Video)
						{
							item.Encoder = profile.Video.Encoder;
							item.Quality = profile.Video.Quality;
							item.DeInterlace = profile.Video.DeInterlace;
						}

						foreach (var item in (queue.Tag as MediaQueue).Audio)
						{
							item.Encoder = profile.Audio.Encoder;
							item.Command = profile.Audio.Command;
							item.Copy = profile.Audio.Copy;
						}
					}

					MediaShowReList();
				}
			}
		}

		private void btnOutputBrowse_Click(object sender, EventArgs e)
		{
			var fbd = new OpenFileDialog
			{
				ValidateNames = false,
				CheckFileExists = false,
				CheckPathExists = false,
				Title = "Select desire save location folder",
				FileName = "Save folder"
			};

			if (fbd.ShowDialog() == DialogResult.OK)
			{
				var fPath = Path.GetDirectoryName(fbd.FileName);
				txtOutputPath.Text = fPath;
			}
		}

		private void tsmiImportFiles_Click(object sender, EventArgs e)
		{
			btnFileAdd.PerformClick(); // using same function
		}

		private void tsmiImportFolder_Click(object sender, EventArgs e)
		{

		}

		private void tsmiImportYouTube_Click(object sender, EventArgs e)
		{

		}

		private void bgThread_DoWork(object sender, DoWorkEventArgs e)
		{
			var data = e.Argument as Dictionary<int, MediaQueue>;

			foreach (var item in data)
			{
				var id = item.Key;
				var mq = item.Value;

				if (mq.Enable)
				{
					var tt = DateTime.Now;

					lstFile.Invoke((MethodInvoker)delegate
					{
						lstFile.Items[id].SubItems[4].Text = "Encoding...";
					});

					// Create Temporary Session Folder
					var tses = Path.Combine(Path.GetTempPath(), "IFME", $"{Guid.NewGuid()}");
					Directory.CreateDirectory(tses);

					// Extract
					MediaEncoding.Extract(mq, tses);

					// Audio
					MediaEncoding.Audio(mq, tses);

					// Video
					MediaEncoding.Video(mq, tses);

					// Mux
					MediaEncoding.Muxing(mq, tses, txtOutputPath.Text);

					// Delete Temporary Session Folder

					lstFile.Invoke((MethodInvoker)delegate
					{
						lstFile.Items[id].Checked = false;
						lstFile.Items[id].SubItems[4].Text = $"Done! ({DateTime.Now.Subtract(tt):dd\\.hh\\:mm\\:ss})";
					});
				}
				else
				{
					lstFile.Invoke((MethodInvoker)delegate
					{
						lstFile.Items[id].SubItems[4].Text = "Skip...";
					});
				}
			}
		}

		private void bgThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			Text = e.ProgressPercentage.ToString();
		}

		private void bgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			btnStart.Text = Fonts.fa.play;

			ProcessManager.Clear();

			if (e.Cancelled)
			{
				Console2.WriteLine("[WARN] Operation was canceled by user");

				foreach (ListViewItem item in lstFile.Items)
				{
					item.SubItems[4].Text = "Aborted!";
				}
			}
		}
	}
}
