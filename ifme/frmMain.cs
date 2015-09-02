using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Reflection;

using ifme.imouto;

using MediaInfoDotNet;
using IniParser;
using IniParser.Model;

namespace ifme
{
	public partial class frmMain : Form
	{
		StringComparison IC = StringComparison.OrdinalIgnoreCase; // Just ignore case what ever it is.

		public frmMain()
		{
			InitializeComponent();

			this.Icon = Properties.Resources.ifme5;
			this.Text = Global.App.NameFull;

			pbxRight.Parent = pbxLeft;
			pbxLeft.Image = imouto.Properties.Resources.BannerA;
			pbxRight.Image = Global.GetRandom % 2 != 0 ? imouto.Properties.Resources.BannerB : imouto.Properties.Resources.BannerC;

			btnQueueAdd.Image = imouto.Properties.Resources.film_add;
			btnQueueRemove.Image = imouto.Properties.Resources.film_delete;

			btnQueueMoveUp.Image = imouto.Properties.Resources.arw_up;
			btnQueueMoveDown.Image = imouto.Properties.Resources.arw_dn;

			btnQueueStart.Image = imouto.Properties.Resources.control_play_blue;
			btnQueueStop.Image = imouto.Properties.Resources.control_stop_blue;
			btnQueuePause.Image = imouto.Properties.Resources.control_pause_blue;

			btnSubAdd.Image = imouto.Properties.Resources.page_add;
			btnSubRemove.Image = imouto.Properties.Resources.page_delete;

			btnAttachAdd.Image = imouto.Properties.Resources.page_add;
			btnAttachRemove.Image = imouto.Properties.Resources.page_delete;

			btnProfileSave.Image = imouto.Properties.Resources.disk;
			btnBrowse.Image = imouto.Properties.Resources.folder_explore;

			btnConfig.Image = imouto.Properties.Resources.wrench;
			btnAbout.Image = imouto.Properties.Resources.information;
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			// Add language list
			foreach (var item in File.ReadAllLines("iso.code"))
				cboSubLang.Items.Add(item);

			// Setting ready
			txtDestination.Text = Properties.Settings.Default.DirOutput;

			// Add profile
			ProfileAdd();

			// Extension menu (runtime)
			foreach (var item in Extension.Items)
			{
				if (!String.Equals(item.Type, "AviSynth", IC))
					continue;

				ToolStripMenuItem tsmi = new ToolStripMenuItem();
				tsmi.Text = item.Name;
				tsmi.Tag = item.FileName;
				tsmi.Name = Path.GetFileNameWithoutExtension(item.FileName);
				tsmi.Click += new EventHandler(tsmi_Click);
				tsmiQueueAviSynth.DropDownItems.Add(tsmi); // or cmsQueueMenu.Items.Add(tsmi); not sub menu
			}

			// Default
			rdoMKV.Checked = true;
			cboPictureRes.SelectedIndex = 8;
			cboPictureFps.SelectedIndex = 5;
			cboPictureBit.SelectedIndex = 0;
			cboPictureYuv.SelectedIndex = 0;
			cboPictureYadifMode.SelectedIndex = 0;
			cboPictureYadifField.SelectedIndex = 0;
			cboPictureYadifFlag.SelectedIndex = 0;
			cboVideoPreset.SelectedIndex = 5;
			cboVideoTune.SelectedIndex = 0;
			cboVideoType.SelectedIndex = 0;
			cboAudioEncoder.SelectedIndex = 1;
			cboAudioBit.SelectedIndex = 1;
			cboAudioFreq.SelectedIndex = 0;
			cboAudioChannel.SelectedIndex = 0;
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (Global.App.NewRelease)
			{
				this.Text += " (please update)";
			}
		}

		#region Profile
		void ProfileAdd()
		{
			// Clear before adding object
			cboProfile.Items.Clear();

			// Add new object
			cboProfile.Items.Add("< new >");
			foreach (var item in Profile.List)
				cboProfile.Items.Add(String.Format("{0}: {1} {2}", item.Info.Platform, item.Info.Format.ToUpper(), item.Info.Name));

			cboProfile.SelectedIndex = 0;
		}

		private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
		{
			var i = cboProfile.SelectedIndex;
			if (i == 0)
			{
				// Here should load last saved
			}
			else
			{
				--i;
				var p = Profile.List[i];

				rdoMKV.Checked = p.Info.Format.ToLower() == "mkv" ? true : false;
				rdoMP4.Checked = p.Info.Format.ToLower() == "mp4" ? true : false;
				cboPictureRes.Text = p.Picture.Resolution;
				cboPictureFps.Text = p.Picture.FrameRate;
				cboPictureBit.Text = p.Picture.BitDepth;
				cboPictureYuv.Text = p.Picture.Chroma;

				cboVideoPreset.Text = p.Video.Preset;
				cboVideoTune.Text = p.Video.Tune;
				cboVideoType.SelectedIndex = p.Video.Type;
				txtVideoValue.Text = p.Video.Value;
				txtVideoCmd.Text = p.Video.Command;

				cboAudioEncoder.Text = p.Audio.Encoder;
				cboAudioBit.Text = p.Audio.BitRate;
				cboAudioFreq.Text = p.Audio.Frequency;
				cboAudioChannel.Text = p.Audio.Channel;
				chkAudioMerge.Checked = p.Audio.Merge;
				txtAudioCmd.Text = p.Audio.Command;
			}
		}

		private void btnProfileSave_Click(object sender, EventArgs e)
		{
			if (cboProfile.SelectedIndex == -1) // Error free
				return;

			var i = cboProfile.SelectedIndex;
			var p = Profile.List[i == 0 ? 0 : i - 1];

			string file;
			string platform;
			string name;
			string author;
			string web;

			if (i == 0)
			{
				using (var form = new frmInputBox("Save new profile", "&Please enter a new profile name", ""))
				{
					var result = form.ShowDialog();
					if (result == DialogResult.OK)
					{
						name = form.ReturnValue; // return
					}
					else
					{
						return;
					}
				}

				file = Path.Combine(Global.Folder.Profile, String.Format("{0:yyyyMMdd_HHmmss}.ifp", DateTime.Now));
				platform = "User";
				// return
				author = Environment.UserName;
				web = "";
			}
			else
			{
				file = p.File;
				platform = p.Info.Platform;
				name = p.Info.Name;
				author = p.Info.Author;
				web = p.Info.Web;
			}

			var parser = new FileIniDataParser();
			IniData data = new IniData();

			data.Sections.AddSection("info");
			data["info"].AddKey("format", rdoMKV.Checked ? "mkv" : "mp4");
			data["info"].AddKey("platform", platform);
			data["info"].AddKey("name", name);
			data["info"].AddKey("author", author);
			data["info"].AddKey("web", web);

			data.Sections.AddSection("picture");
			data["picture"].AddKey("resolution", cboPictureRes.Text);
			data["picture"].AddKey("framerate", cboPictureFps.Text);
			data["picture"].AddKey("bitdepth", cboPictureBit.Text);
			data["picture"].AddKey("chroma", cboPictureYuv.Text);

			data.Sections.AddSection("video");
			data["video"].AddKey("preset", cboVideoPreset.Text);
			data["video"].AddKey("tune", cboVideoTune.Text);
			data["video"].AddKey("type", cboVideoType.SelectedIndex.ToString());
			data["video"].AddKey("value", txtVideoValue.Text);
			data["video"].AddKey("cmd", txtVideoCmd.Text);

			data.Sections.AddSection("audio");
			data["audio"].AddKey("encoder", cboAudioEncoder.Text);
			data["audio"].AddKey("bitrate", cboAudioBit.Text);
			data["audio"].AddKey("frequency", cboAudioFreq.Text);
			data["audio"].AddKey("channel", cboAudioChannel.Text);
			data["audio"].AddKey("compile", chkAudioMerge.Checked ? "true" : "false");
			data["audio"].AddKey("cmd", txtAudioCmd.Text);

			parser.WriteFile(file, data, Encoding.UTF8);
			Profile.Load(); //reload list
			ProfileAdd();
		}
		#endregion

		#region Browse, Config & About button
		private void btnBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog GetDir = new FolderBrowserDialog();

			GetDir.Description = "";
			GetDir.ShowNewFolderButton = true;
			GetDir.RootFolder = Environment.SpecialFolder.MyComputer;

			if (GetDir.ShowDialog() == DialogResult.OK)
			{
				txtDestination.Text = GetDir.SelectedPath;
			}
		}

		private void btnConfig_Click(object sender, EventArgs e)
		{
			frmOption fo = new frmOption();
			fo.ShowDialog();
		}

		private void btnAbout_Click(object sender, EventArgs e)
		{
			Form frm = new frmAbout();
			frm.ShowDialog();
		}
		#endregion

		#region Queue: Add files
		private void btnQueueAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
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
				+ "MPEG-1/DVD/VCD|*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob|"
				+ "AviSynth Script|*.avs|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (GetFiles.ShowDialog() == DialogResult.OK)
				foreach (var item in GetFiles.FileNames)
					QueueAdd(item);
		}

		private void lstQueue_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstQueue_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var file in files)
				QueueAdd(file);
		}

		void QueueAdd(string file)
		{
			string FileType;
			var Info = new Queue();

			Info.Data.File = file;
			Info.Data.SaveAsMkv = true;

			MediaFile AVI = new MediaFile(file);

			Info.Data.IsFileMkv = String.Equals(AVI.format, "Matroska", IC);
			Info.Data.IsFileAvs = GetInfo.IsAviSynth(file);

			if (AVI.Video.Count > 0)
			{
				var Video = AVI.Video[0];
				Info.Picture.Resolution = String.Format("{0}x{1}", Video.width, Video.height);
				Info.Picture.FrameRate = String.Format("{0}", Video.frameRateGet);
				Info.Picture.BitDepth = String.Format("{0}", Video.bitDepth);
				Info.Picture.Chroma = "420";

				Info.Prop.Duration = Video.duration;
				Info.Prop.FrameCount = Video.frameCount;

				FileType = String.Format("{0} ({1})", Path.GetExtension(file).ToUpper(), Video.format);

				if (String.Equals(Video.frameRateMode, "vfr", IC))
					Info.Prop.IsVFR = true;

				if (Video.isInterlace)
				{
					Info.Picture.YadifEnable = true;
					Info.Picture.YadifMode = 0;
					Info.Picture.YadifField = (Video.isTopFieldFirst ? 0 : 1);
					Info.Picture.YadifFlag = 0;
				}
			}
			else
			{
				if (Info.Data.IsFileAvs)
				{
					Info.Picture.Resolution = "auto";
					Info.Picture.FrameRate = "auto";
					Info.Picture.BitDepth = "8";
					Info.Picture.Chroma = "420";

					FileType = "AviSynth Script";
				}
				else
				{
					FileType = "Unknown";
				}
			}

			Info.Video.Preset = "medium";
			Info.Video.Tune = "off";
			Info.Video.Type = 0;
			Info.Video.Value = "26";
			Info.Video.Command = "--dither";

			Info.Audio.Encoder = "Passthrough (Extract all audio)";
			Info.Audio.BitRate = "128";
			Info.Audio.Frequency = "auto";
			Info.Audio.Channel = "stereo";
			Info.Audio.Command = "";

			// Add to queue list
			ListViewItem qItem = new ListViewItem(new[] {
				GetInfo.FileName(file),
				GetInfo.FileSize(file),
				FileType,
				".MKV (HEVC)",
				"Ready"
			});
			qItem.Tag = Info;
			qItem.Checked = true;
			lstQueue.Items.Add(qItem);

			// Print to log
			InvokeLog(String.Format("File added {0}", Info.Data.File));
		}
		#endregion

		#region Queue: CheckBox, this event fired once when add new item and tick event
		private void lstQueue_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			(lstQueue.Items[e.Index].Tag as Queue).IsEnable = e.CurrentValue == CheckState.Unchecked ? true : false; // reverse event
		}
		#endregion

		#region Queue: Move item up, down and remove
		private void btnQueueMoveUp_Click(object sender, EventArgs e)
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

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnQueueMoveDown_Click(object sender, EventArgs e)
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

				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btnQueueRemove_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstQueue.SelectedItems)
			{
				item.Remove();
				InvokeLog(String.Format("File removed {0}", item.SubItems[0].Text));
			}
		}
		#endregion

		#region Queue: Display item properties
		private void lstQueue_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
				QueueDisplay(lstQueue.SelectedItems[0].Index);
			else
				QueueUnselect();
		}

		void QueueDisplay(int index)
		{
			var Info = (Queue)lstQueue.Items[index].Tag;

			// Picture
			rdoMKV.Checked = Info.Data.SaveAsMkv;
			rdoMP4.Checked = !rdoMKV.Checked;
			cboPictureRes.Text = Info.Picture.Resolution;
			cboPictureFps.Text = Info.Picture.FrameRate;
			cboPictureBit.Text = Info.Picture.BitDepth;
			cboPictureYuv.Text = Info.Picture.Chroma;
			chkPictureYadif.Checked = Info.Picture.YadifEnable;
			cboPictureYadifMode.SelectedIndex = Info.Picture.YadifMode;
			cboPictureYadifField.SelectedIndex = Info.Picture.YadifField;
			cboPictureYadifFlag.SelectedIndex = Info.Picture.YadifFlag;

			// Video
			cboVideoPreset.Text = Info.Video.Preset;
			cboVideoTune.Text = Info.Video.Tune;
			cboVideoType.SelectedIndex = Info.Video.Type;
			txtVideoValue.Text = Info.Video.Value;
			txtVideoCmd.Text = Info.Video.Command;

			// Audio
			cboAudioEncoder.Text = Info.Audio.Encoder;
			cboAudioBit.Text = Info.Audio.BitRate;
			cboAudioFreq.Text = Info.Audio.Frequency;
			cboAudioChannel.Text = Info.Audio.Channel;
			chkAudioMerge.Checked = Info.Audio.Merge;
			txtAudioCmd.Text = Info.Audio.Command;

			// Subtitles
			lstSub.Items.Clear();
			chkSubEnable.Checked = Info.SubtitleEnable;
			if (Info.Subtitle != null)
				foreach (var item in Info.Subtitle)
					lstSub.Items.Add(new ListViewItem(new[] { GetInfo.FileName(item.File), item.Lang }));
			
			// Attachments
			lstAttach.Items.Clear();
			chkAttachEnable.Checked = Info.AttachEnable;
			if (Info.Attach != null)
				foreach (var item in Info.Attach)
					lstAttach.Items.Add(new ListViewItem(new[] { GetInfo.FileName(item.File), item.MIME }));
			
			// AviSynth
			var x = Info.Data.IsFileAvs;
			grpPictureBasic.Enabled = !x;
			grpPictureQuality.Enabled = !x;
			chkPictureYadif.Enabled = !x;

		}

		void QueueUnselect()
		{
			// Subtitles
			chkSubEnable.Checked = false;
			lstSub.Items.Clear();

			// Attachments
			chkAttachEnable.Checked = false;
			lstAttach.Items.Clear();
		}
		#endregion

		#region Queue: Property update
		#region Queue: Property update - Picture Tab
		private void rdoMKV_CheckedChanged(object sender, EventArgs e)
		{
			PluginAudioReload();
			QueueUpdate(QueueProp.FormatMkv);
		}

		private void rdoMP4_CheckedChanged(object sender, EventArgs e)
		{
			PluginAudioReload();
			QueueUpdate(QueueProp.FormatMp4);
		}

		void PluginAudioReload()
		{
			cboAudioEncoder.Items.Clear();

			foreach (var item in Plugin.List)
			{
				if (item.Info.Type.ToLower() == "audio")
				{
					if (rdoMP4.Checked)
					{
						if (item.Info.Support.ToLower() == "mp4")
						{
							cboAudioEncoder.Items.Add(item.Profile.Name);
						}
					}
					else
					{
						cboAudioEncoder.Items.Add(item.Profile.Name);
					}
				}
			}
		}

		private void cboPictureRes_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureResolution);
		}

		private void cboPictureFps_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureFrameRate);
		}

		private void cboPictureBit_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureBitDepth);
		}

		private void cboPictureYuv_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureChroma);
		}

		private void chkPictureYadif_CheckedChanged(object sender, EventArgs e)
		{
			grpPictureYadif.Enabled = chkPictureYadif.Checked;
			QueueUpdate(QueueProp.PictureYadifEnable);
		}

		private void cboPictureYadifMode_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureYadifMode);

			if (cboPictureYadifMode.SelectedIndex == 1)
			{
				cboPictureFps.SelectedIndex = 0;
				cboPictureFps.Enabled = false;
			}
			else
			{
				cboPictureFps.Enabled = true;
			}
		}

		private void cboPictureYadifField_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureYadifField);
		}

		private void cboPictureYadifFlag_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureYadifFlag);
		}
		#endregion

		#region Queue: Property update - Video Tab
		private void cboVideoPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.VideoPreset);
		}

		private void cboVideoTune_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.VideoTune);
		}

		private void cboVideoType_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (cboVideoType.SelectedIndex)
			{
				case 0:
					lblVideoRateH.Visible = true;
					lblVideoRateL.Visible = true;
					trkVideoRate.Visible = true;

					trkVideoRate.Minimum = 0;
					trkVideoRate.Maximum = 510;
					trkVideoRate.TickFrequency = 10;

					lblVideoRateValue.Text = "Ratefactor:";
					txtVideoValue.Text = String.Format("{0:0.0}", (trkVideoRate.Value = 260) / 10);
					break;

				case 1:
					lblVideoRateH.Visible = true;
					lblVideoRateL.Visible = true;
					trkVideoRate.Visible = true;

					trkVideoRate.Minimum = 0;
					trkVideoRate.Maximum = 51;
					trkVideoRate.TickFrequency = 1;

					lblVideoRateValue.Text = "Ratefactor:";
					txtVideoValue.Text = Convert.ToString(trkVideoRate.Value = 26);
					break;

				default:
					lblVideoRateH.Visible = false;
					lblVideoRateL.Visible = false;
					trkVideoRate.Visible = false;

					lblVideoRateValue.Text = "Bit-rate (kbps):";
					txtVideoValue.Text = "2048";
					break;
			}

			QueueUpdate(QueueProp.VideoType);
		}

		private void txtVideoValue_TextChanged(object sender, EventArgs e)
		{
			var i = cboVideoType.SelectedIndex;
			if (i == 0)
				if (!String.IsNullOrEmpty(txtVideoValue.Text))
					if (Convert.ToDouble(txtVideoValue.Text) >= 51.0)
						txtVideoValue.Text = "51";
					else if (Convert.ToDouble(txtVideoValue.Text) <= 0.0)
						txtVideoValue.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(Convert.ToDouble(txtVideoValue.Text) * (double)10.0);
				else
					trkVideoRate.Value = 0;
			else if (i == 1)
				if (!String.IsNullOrEmpty(txtVideoValue.Text))
					if (Convert.ToInt32(txtVideoValue.Text) >= 51)
						txtVideoValue.Text = "51";
					else if (Convert.ToInt32(txtVideoValue.Text) <= 0)
						txtVideoValue.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(txtVideoValue.Text);
				else
					trkVideoRate.Value = 0;

			QueueUpdate(QueueProp.VideoValue);
		}

		private void txtVideoValue_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
				e.Handled = true;

			// only allow one decimal point
			if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
				e.Handled = true;
		}

		private void trkVideoRate_ValueChanged(object sender, EventArgs e)
		{
			if (cboVideoType.SelectedIndex == 0)
				txtVideoValue.Text = String.Format("{0:0.0}", Convert.ToDouble(trkVideoRate.Value) * 0.1);
			else
				txtVideoValue.Text = Convert.ToString(trkVideoRate.Value);
		}

		private void txtVideoCmd_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.VideoCommand);
		}
		#endregion

		#region Queue: Property update - Audio Tab
		private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (var item in Plugin.List)
			{
				if (item.Info.Type.ToLower() == "audio")
				{
					if (item.Profile.Name == cboAudioEncoder.Text)
					{
						cboAudioBit.Items.Clear();
						cboAudioBit.Items.AddRange(item.App.Quality);
						cboAudioBit.Text = item.App.Default;
						cboAudioFreq.SelectedIndex = 0;
						cboAudioChannel.SelectedIndex = 0;
						txtAudioCmd.Text = item.Arg.Advance;

						QueueUpdate(QueueProp.AudioEncoder);

						return;
					}
				}
			}
		}

		private void cboAudioBit_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.AudioBitRate);
		}

		private void cboAudioFreq_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.AudioFreq);
		}

		private void cboAudioChannel_SelectedIndexChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.AudioChannel);
		}

		private void chkAudioMerge_CheckedChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.AudioMerge);
		}

		private void txtAudioCmd_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.AudioCommand);
		}
		#endregion

		#region Queue: Property update - Subtitles Tab
		private void tabSubtitles_Leave(object sender, EventArgs e)
		{
			if (lstSub.Items.Count == 0)
				chkSubEnable.Checked = false;
		}

		private void chkSubEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (chkSubEnable.Checked)
			{
				if (rdoMP4.Checked)
				{
					MessageBox.Show("MP4コンテナは特殊な字幕をサポートしていません", "Error");
					chkSubEnable.Checked = false;
					return;
				}
				
				if (lstQueue.SelectedItems.Count == 0 || lstQueue.SelectedItems.Count >= 2)
				{
					MessageBox.Show("Please select one video", "Error");
					chkSubEnable.Checked = false;
					return;
				}
			}

			var x = chkSubEnable.Checked;

			btnSubAdd.Visible = x;
			btnSubRemove.Visible = x;
			lstSub.Visible = x;
			lblSubLang.Visible = x;
			cboSubLang.Visible = x;
			lblSubNote.Visible = !x;

			if (lstQueue.SelectedItems.Count == 1)
				(lstQueue.SelectedItems[0].Tag as Queue).SubtitleEnable = x;
		}

		private void btnSubAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Filter = "Supported Subtitle|*.ass;*.ssa;*.srt|"
				+ "SubStation Alpha|*.ass;*.ssa|"
				+ "SubRip|*.srt|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (GetFiles.ShowDialog() == DialogResult.OK)
				foreach (var item in GetFiles.FileNames)
					SubAdd(item);
		}

		private void lstSub_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstSub_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var item in files)
				SubAdd(item);
		}

		void SubAdd(string file)
		{
			if (!GetInfo.SubtitleValid(file))
				return;

			foreach (ListViewItem item in lstQueue.SelectedItems)
				(item.Tag as Queue).Subtitle.Add(new Subtitle() { File = file, Lang = "und (Undetermined)" });

			lstSub.Items.Add(new ListViewItem(new[] { GetInfo.FileName(file), "und (Undetermined)" }));
		}

		private void btnSubRemove_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count > 1)
			{
				MessageBox.Show("Please select one video before removing subtitles");
				return;
			}

			foreach (ListViewItem subs in lstSub.SelectedItems) 
			{
				(lstQueue.SelectedItems[0].Tag as Queue).Subtitle.RemoveAt(subs.Index);
				subs.Remove();
			}
		}

		private void lstSub_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
				if (lstSub.SelectedItems.Count > 0)
					cboSubLang.Text = lstSub.SelectedItems[0].SubItems[1].Text;
		}

		private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
		{
			foreach (ListViewItem subs in lstSub.SelectedItems)
			{
				subs.SubItems[1].Text = cboSubLang.Text;
				(lstQueue.SelectedItems[0].Tag as Queue).Subtitle[subs.Index].Lang = cboSubLang.Text;
			}
		}
		#endregion

		#region Queue: Property update - Attachments Tab
		private void tabAttachments_Leave(object sender, EventArgs e)
		{
			if (lstAttach.Items.Count == 0)
				chkAttachEnable.Checked = false;
		}

		private void chkAttachEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (chkAttachEnable.Checked)
			{
				if (rdoMP4.Checked)
				{
					MessageBox.Show("MP4コンテナはアタッチメントサポートしていません", "Error");
					chkAttachEnable.Checked = false;
					return;
				}

				if (lstQueue.SelectedItems.Count == 0 || lstQueue.SelectedItems.Count >= 2)
				{
					MessageBox.Show("Please select one video", "Error");
					chkAttachEnable.Checked = false;
					return;
				}
			}

			var x = chkAttachEnable.Checked;

			btnAttachAdd.Visible = x;
			btnAttachRemove.Visible = x;
			lstAttach.Visible = x;
			lblAttachDescription.Visible = x;
			txtAttachDescription.Visible = x;
			lblAttachNote.Visible = !x;

			if (lstQueue.SelectedItems.Count == 1)
				(lstQueue.SelectedItems[0].Tag as Queue).AttachEnable = x;
		}

		private void btnAttachAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Filter = "Known font files|*.ttf;*.otf;*.woff|"
				+ "TrueType Font|*.ttf|"
				+ "OpenType Font|*.otf|"
				+ "Web Open Font Format|*.woff|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (GetFiles.ShowDialog() == DialogResult.OK)
				foreach (var item in GetFiles.FileNames)
					AttachAdd(item);
		}

		private void lstAttach_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void lstAttach_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			foreach (var item in files)
				AttachAdd(item);
		}

		void AttachAdd(string file)
		{
			foreach (ListViewItem item in lstQueue.SelectedItems)
				(item.Tag as Queue).Attach.Add(new Attachment() { File = file, MIME = GetInfo.AttachmentValid(file), Comment = "No" });

			lstAttach.Items.Add(new ListViewItem(new[] { GetInfo.FileName(file), GetInfo.AttachmentValid(file), "No" }));
		}

		private void btnAttachRemove_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count > 1)
			{
				MessageBox.Show("Please select one video before removing attachment", "Error");
				return;
			}

			foreach (ListViewItem item in lstAttach.SelectedItems)
			{
				(lstQueue.SelectedItems[0].Tag as Queue).Attach.RemoveAt(item.Index);
				item.Remove();
			}
		}

		private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
				if (lstAttach.SelectedItems.Count > 0)
					txtAttachDescription.Text = lstAttach.SelectedItems[0].SubItems[2].Text;
		}

		private void txtAttachDescription_TextChanged(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstAttach.SelectedItems)
			{
				item.SubItems[2].Text = txtAttachDescription.Text;
				(lstQueue.SelectedItems[0].Tag as Queue).Attach[item.Index].Comment = txtAttachDescription.Text;
			}
		}
		#endregion

		void QueueUpdate(QueueProp Id)
		{
			foreach (ListViewItem item in lstQueue.SelectedItems)
			{
				var X = item.Tag as Queue;

				switch (Id)
				{
					case QueueProp.FormatMkv:
						cboAudioEncoder.Text = "Passthrough (Extract all audio)";
						item.SubItems[3].Text = ".MKV (HEVC)";
						X.Data.SaveAsMkv = true;
						break;

					case QueueProp.FormatMp4:
						cboAudioEncoder.Text = "Passthrough (Extract all audio)";
						item.SubItems[3].Text = ".MP4 (HEVC)";
						X.Data.SaveAsMkv = false;
						break;

					case QueueProp.PictureResolution:
						X.Picture.Resolution = cboPictureRes.Text;
						break;

					case QueueProp.PictureFrameRate:
						X.Picture.FrameRate = cboPictureFps.Text;
						break;

					case QueueProp.PictureBitDepth:
						X.Picture.BitDepth = cboPictureBit.Text;
						break;

					case QueueProp.PictureChroma:
						X.Picture.Chroma = cboPictureYuv.Text;
						break;

					case QueueProp.PictureYadifEnable:
						X.Picture.YadifEnable = chkPictureYadif.Checked;
						break;

					case QueueProp.PictureYadifMode:
						X.Picture.YadifMode = cboPictureYadifMode.SelectedIndex;
						break;

					case QueueProp.PictureYadifField:
						X.Picture.YadifField = cboPictureYadifField.SelectedIndex;
						break;

					case QueueProp.PictureYadifFlag:
						X.Picture.YadifFlag = cboPictureYadifFlag.SelectedIndex;
						break;

					case QueueProp.VideoPreset:
						X.Video.Preset = cboVideoPreset.Text;
						break;

					case QueueProp.VideoTune:
						X.Video.Tune = cboVideoTune.Text;
						break;

					case QueueProp.VideoType:
						X.Video.Type = cboVideoType.SelectedIndex;
						break;

					case QueueProp.VideoValue:
						X.Video.Value = txtVideoValue.Text;
						break;

					case QueueProp.VideoCommand:
						X.Video.Command = txtVideoCmd.Text;
						break;

					case QueueProp.AudioEncoder:
						X.Audio.Encoder = cboAudioEncoder.Text;
						break;

					case QueueProp.AudioBitRate:
						X.Audio.BitRate = cboAudioBit.Text;
						break;

					case QueueProp.AudioFreq:
						X.Audio.Frequency = cboAudioFreq.Text;
						break;

					case QueueProp.AudioChannel:
						X.Audio.Channel = cboAudioChannel.Text;
						break;

					case QueueProp.AudioMerge:
						X.Audio.Merge = chkAudioMerge.Checked;
						break;

					case QueueProp.AudioCommand:
						X.Audio.Command = txtAudioCmd.Text;
						break;

					default:
						break;
				}
			}
		}
		#endregion

		private void btnQueueStart_Click(object sender, EventArgs e)
		{
			// Send a new copy to another thread
			if (!bgwEncoding.IsBusy)
			{
				// Make a copy, thread safe
				List<object> gg = new List<object>();

				foreach (ListViewItem item in lstQueue.Items)
					gg.Add(item.Tag);

				// View log
				tabConfig.SelectedIndex = 5;

				// Start
				bgwEncoding.RunWorkerAsync(gg);
				btnQueueStart.Visible = false;
				btnQueuePause.Visible = true;

				ControlEnable(false);
			}
			else
			{
				TaskManager.Resume();
				btnQueueStart.Visible = false;
				btnQueuePause.Visible = true;
			}
		}

		private void btnQueueStop_Click(object sender, EventArgs e)
		{
			TaskManager.Kill();
			bgwEncoding.CancelAsync();

			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("Encoding has been cancelled...");
			Console.ResetColor();
		}

		private void btnQueuePause_Click(object sender, EventArgs e)
		{
			TaskManager.Pause();
			btnQueueStart.Visible = true;
			btnQueuePause.Visible = false;
		}

		private void bgwEncoding_DoWork(object sender, DoWorkEventArgs e)
		{
			// NULL
			var NULL = OS.Null;

			// Temp Dir
			string tempdir = Properties.Settings.Default.DirTemp;

			// Time entire queue
			DateTime Session = DateTime.Now;

			// Log current session
			InvokeLog("Encoding has been started!");

			// Encoding process
			int id = -1;
			List<object> argList = e.Argument as List<object>;
			foreach (Queue item in argList)
			{
				id++;

				// Only checked list get encoded
				if (!item.IsEnable)
				{
					id++;
					continue;
				}

				// Time current queue
				DateTime SessionCurrent = DateTime.Now;

				// Log current queue
				InvokeLog("Processing: " + item.Data.File);

				// Remove temp file
				foreach (var files in Directory.GetFiles(tempdir))
					File.Delete(files);

				// Naming
				string prefix = String.IsNullOrEmpty(Properties.Settings.Default.NamePrefix) ? "" : Properties.Settings.Default.NamePrefix + " ";
				string fileout = Path.Combine(Properties.Settings.Default.DirOutput, prefix + Path.GetFileNameWithoutExtension(item.Data.File));

				// AviSynth aware
				string file = item.Data.File;
				string filereal = GetStream.AviSynthGetFile(file);

				// Extract mkv embedded subtitle, font and chapter
				InvokeQueueStatus(id, "Extracting");
				if (item.Data.IsFileMkv || (item.Data.IsFileAvs && Properties.Settings.Default.AvsMkvCopy))
				{
					int sc = 0;
					foreach (var subs in GetStream.Media(filereal, StreamType.Subtitle))
						TaskManager.Run(String.Format("\"{0}\" -i \"{1}\" -map {2} -y sub{3:0000}_{4}.{5}", Plugin.LIBAV, filereal, subs.ID, sc++, subs.Lang, subs.Format));
					

					foreach (var font in GetStream.MediaMkv(filereal, StreamType.Attachment))
						TaskManager.Run(String.Format("\"{0}\" attachments \"{1}\" {2}:\"{3}\"", Plugin.MKVEX, filereal, font.ID, font.File));

					TaskManager.Run(String.Format("\"{0}\" chapters \"{1}\" > chapters.xml", Plugin.MKVEX, filereal));

					if (bgwEncoding.CancellationPending)
					{
						InvokeQueueAbort(id);
						e.Cancel = true;
						return;
					}
				}

				// Audio
				InvokeQueueStatus(id, "Processing audio");
				if (String.Equals(item.Audio.Encoder, "No Audio", IC))
				{
					// Do noting
				}
				else if (String.Equals(item.Audio.Encoder, "Passthrough (Extract all audio)", IC))
				{
					int counter = 0;
					foreach (var audio in GetStream.Media(filereal, StreamType.Audio))
					{
						TaskManager.Run(String.Format("\"{0}\" -i \"{1}\" -map {2} -acodec copy -y audio{3:0000}_{4}.{5}", Plugin.LIBAV, filereal, audio.ID, counter++, audio.Lang, audio.Format));

						if (bgwEncoding.CancellationPending)
						{
							InvokeQueueAbort(id);
							e.Cancel = true;
							return;
						}
					}
				}
				else
				{
					string frequency;
					if (String.Equals(item.Audio.Frequency, "auto", IC))
						frequency = "";
					else
						frequency = "-ar " + item.Audio.Frequency;

					string channel;
					if (String.Equals(item.Audio.Channel, "auto", IC))
						channel = "";
					else if (String.Equals(item.Audio.Channel, "mono", IC))
						channel = "-ac 1";
					else
						channel = "-ac 2";

					int counter = 0;
					foreach (var codec in Plugin.List)
					{
						if (String.Equals(codec.Profile.Name, item.Audio.Encoder, IC))
						{
							foreach (var audio in GetStream.Media(filereal, StreamType.Audio))
							{
								string outfile = String.Format("audio{0:0000}_{1}", counter++, audio.Lang);
								TaskManager.Run(String.Format("\"{0}\" -i \"{1}\" -map {2} {3} {4} -y {5}.wav", Plugin.LIBAV, filereal, audio.ID, frequency, channel, outfile));
								TaskManager.Run(String.Format("\"{0}\" {1} {2} {3} {4} {5}.wav {6} {5}.{7}", codec.App.Bin, codec.Arg.Bitrate, item.Audio.BitRate, item.Audio.Command, codec.Arg.Input, outfile, codec.Arg.Output, codec.App.Ext));
								File.Delete(Path.Combine(Global.Folder.Temp, outfile + ".wav"));

								if (bgwEncoding.CancellationPending)
								{
									InvokeQueueAbort(id);
									e.Cancel = true;
									return;
								}
							}
						}
					}
				}

				// Video
				InvokeQueueStatus(id, "Processing video");
				foreach (var video in GetStream.Media(file, StreamType.Video))
				{
					string resolution = String.Equals(item.Picture.Resolution, "auto", IC) ? null : String.Format("-s {0}", item.Picture.Resolution);
					string framerate = String.Equals(item.Picture.FrameRate, "auto", IC) ? null : String.Format("-r {0}", item.Picture.FrameRate);
					int bitdepth = Convert.ToInt32(item.Picture.BitDepth);
					string chroma = String.Format("yuv{0}p{1}", item.Picture.Chroma, bitdepth == 10 ? "10le" : null);
					string yadif = item.Picture.YadifEnable ? String.Format("-vf \"yadif={0}:{1}:{2}\"", item.Picture.YadifMode, item.Picture.YadifField, item.Picture.YadifFlag) : null;
					int framecount = item.Prop.FrameCount;
					string vsync = "cfr";

					if (String.IsNullOrEmpty(framerate))
					{
						if (item.Prop.IsVFR)
						{
							TaskManager.Run(String.Format("\"{0}\" -f -c \"{1}\" timecode", Plugin.FFMS2, file));
							vsync = "vfr";
						}
					}
					else // when fps is set, generate new framecount
					{
						framecount = (int)Math.Ceiling(((float)item.Prop.Duration / 1000.0) * Convert.ToDouble(item.Picture.FrameRate));
					}

					if (item.Picture.YadifEnable)
					{
						if (item.Picture.YadifMode == 1)
						{
							framecount *= 2; // make each fields as new frame
						}
					}

					if (item.Data.IsFileAvs) // get new framecount for avisynth
						framecount = GetStream.AviSynthFrameCount(file);
					
					// x265 settings
					string preset = String.Format("-p {0}", item.Video.Preset);
					string tune = String.Equals(item.Video.Tune, "off", IC) ? null : String.Format("--tune {0}", item.Video.Tune);
					int type = item.Video.Type;
					int pass;
					string ratef;
					string value = item.Video.Value;
					string command = item.Video.Command;

					if (type == 0)
						ratef = String.Format("--crf {0}", value);
					else if (type == 1)
						ratef = String.Format("--qp {0}", value);
					else
						ratef = String.Format("--bitrate {0}", value);
					

					string HEVC = bitdepth == 8 ? Plugin.HEVCL : Plugin.HEVCH;
					string decoder = item.Data.IsFileAvs ? String.Format("\"{0}\" video \"{1}\"", Plugin.AVS4P, file) : String.Format("\"{0}\" -i \"{1}\" -vsync {2} -f yuv4mpegpipe -pix_fmt {3} -strict -1 {4} {5} {6} -", Plugin.LIBAV, file, vsync, chroma, resolution, framerate, yadif);
					string encoder = String.Format("\"{0}\" --y4m - {1} {2} {3} -o video0000_{4}.hevc", HEVC, preset, ratef, command, video.Lang);

					// Encoding start
					if (type-- >= 3) // multi pass
					{
						for (int i = 0; i < type; i++)
						{
							if (i == 0)
								pass = 1;
							else if (i == type)
								pass = 2;
							else
								pass = 3;

							if (i == 1) // get actual frame count
								framecount = GetStream.FrameCount(Path.Combine(tempdir, String.Format("video0000_{0}.hevc", video.Lang)));

							Console.WriteLine("Pass {0} of {1}", i + 1, type + 1); // human read no index
							TaskManager.Run(String.Format("{0} 2> {1} | {2} -f {3} --pass {4}", decoder, NULL, encoder, framecount, pass));

							if (bgwEncoding.CancellationPending)
							{
								InvokeQueueAbort(id);
								e.Cancel = true;
								return;
							}
						}
					}
					else
					{
						TaskManager.Run(String.Format("{0} 2> {1} | {2} -f {3}", decoder, NULL, encoder, framecount));

						if (bgwEncoding.CancellationPending)
						{
							InvokeQueueAbort(id);
							e.Cancel = true;
							return;
						}
					}

					break;
				}

				// Mux
				InvokeQueueStatus(id, "Compiling");
				if (item.Data.SaveAsMkv)
				{
					fileout += ".mkv";

					string tags = String.Format(imouto.Properties.Resources.Tags, Global.App.NameFull, "Nemu System 5.1.1");
					string cmdvideo = null;
					string cmdaudio = null;
					string cmdsubs = null;
					string cmdattach = null;
					string cmdchapter = null;

					foreach (var tc in Directory.GetFiles(tempdir, "timecode_*"))
					{
						cmdvideo += String.Format("--timecodes 0:\"{0}\" ", tc); break;
					}

					foreach (var video in Directory.GetFiles(tempdir, "video*"))
					{
						cmdvideo += String.Format("--language 0:{0} \"{1}\" ", GetInfo.FileLang(video), video);
					}

					foreach (var audio in Directory.GetFiles(tempdir, "audio*"))
					{
						cmdaudio += String.Format("--language 0:{0} \"{1}\" ", GetInfo.FileLang(audio), audio);
					}

					if (item.SubtitleEnable)
					{
						foreach (var subs in item.Subtitle)
						{
							cmdsubs += String.Format("--compression 0:zlib --sub-charset 0:UTF-8 --language 0:{0} \"{1}\" ", subs.Lang, subs.File);
						}
					}
					else
					{
						foreach (var subs in Directory.GetFiles(tempdir, "sub*"))
						{
							cmdsubs += String.Format("--compression 0:zlib --sub-charset 0:UTF-8 --language 0:{0} \"{1}\" ", GetInfo.FileLang(subs), subs);
						}
					}

					if (item.AttachEnable)
					{
						foreach (var attach in item.Attach)
						{
							cmdattach += String.Format("--attachment-mime-type {0} --attachment-description {1} --attach-file \"{2}\" ", attach.MIME, attach.Comment, attach.File);
						}
					}
					else
					{
						foreach (var attach in Directory.GetFiles(tempdir, "font*.*f"))
						{
							cmdattach += String.Format("--attachment-description No --attach-file \"{0}\" ", attach);
						}
					}

					if (File.Exists(Path.Combine(tempdir, "chapters.xml")))
					{
						FileInfo ChapLen = new FileInfo(Path.Combine(tempdir, "chapters.xml"));
						if (ChapLen.Length > 256)
							cmdchapter = String.Format("--chapters \"{0}\"", Path.Combine(tempdir, "chapters.xml"));
					}

					File.WriteAllText(Path.Combine(tempdir, "tags.xml"), tags);
					TaskManager.Run(String.Format("\"{0}\" -o \"{1}\" -t 0:tags.xml --disable-track-statistics-tags {2} {3} {4} {5} {6}", Plugin.MKVME, fileout, cmdvideo, cmdaudio, cmdsubs, cmdattach, cmdchapter));
				}
				else
				{
					fileout += ".mp4";

					string timecode = null;
					string cmdvideo = null;
					string cmdaudio = null;

					foreach (var tc in Directory.GetFiles(tempdir, "timecode_*"))
					{
						timecode = tc; break;
					}

					foreach (var video in Directory.GetFiles(tempdir, "video*"))
					{
						cmdvideo += String.Format("-add \"{0}#video:name=IFME:lang={1}:fmt=HEVC\" ", video, GetInfo.FileLang(video));
					}

					foreach (var audio in Directory.GetFiles(tempdir, "audio*"))
					{
						cmdaudio += String.Format("-add \"{0}#audio:name=IFME:lang={1}\" ", audio, GetInfo.FileLang(audio));
					}

					if (String.IsNullOrEmpty(timecode))
					{
						TaskManager.Run(String.Format("\"{0}\" {1} {2} -itags tool=\"{3}\" -new \"{4}\"", Plugin.MP4BX, cmdvideo, cmdaudio, Global.App.NameFull, fileout));
					}
					else
					{
						TaskManager.Run(String.Format("\"{0}\" {1} {2} -itags tool=\"{3}\" -new _temp.mp4", Plugin.MP4BX, cmdvideo, cmdaudio, Global.App.NameFull));
						TaskManager.Run(String.Format("\"{0}\" -t \"{1}\" _temp.mp4 -o \"{2}\"", Plugin.MP4FP, timecode, fileout));
					}
				}

				if (bgwEncoding.CancellationPending)
				{
					InvokeQueueAbort(id);
					e.Cancel = true;
					return;
				}

				string timeDone = GetInfo.Duration(SessionCurrent);
				InvokeQueueDone(id, timeDone);
				InvokeLog(String.Format("Completed in {0} for {1}", timeDone, item.Data.File));
			}

			InvokeLog(String.Format("All Queue Completed in {0}", GetInfo.Duration(Session)));
		}

		private void bgwEncoding_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Console.Title = "IFME console";
			btnQueueStart.Visible = true;
			btnQueuePause.Visible = false;

			if (e.Error != null)
			{
				InvokeLog("Error was found, sorry could finsih it.");
			}
			else if (e.Cancelled)
			{
				InvokeLog("Queue has been canceled by user.");
			}
			else
			{
				if (Properties.Settings.Default.SoundFinish)
				{
					SoundPlayer notification = new SoundPlayer(Global.Sounds.Finish);
					notification.Play();
				}
			}

			ControlEnable(true);
		}

		void InvokeQueueStatus(int index, string s)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = s));
			else
				lstQueue.Items[index].SubItems[4].Text = s;
		}

		void InvokeQueueAbort(int index)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = "Abort!"));
			else
				lstQueue.Items[index].SubItems[4].Text = "Abort!";
		}

		void InvokeQueueDone(int index, string message)
		{
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].Checked = false));
			else
				lstQueue.Items[index].Checked = false;

			string a = String.Format("Finished in {0}", message);
			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = a));
			else
				lstQueue.Items[index].SubItems[4].Text = a;
		}

		void InvokeLog(string message)
		{
			message = String.Format("[{0:yyyy/MMM/dd HH:mm:ss}]: {1}\r\n", DateTime.Now, message);

			if (this.InvokeRequired)
				BeginInvoke(new MethodInvoker(() => txtLog.AppendText(message)));
			else
				txtLog.AppendText(message);
		}

		void ControlEnable(bool x)
		{
			foreach (ListViewItem item in lstQueue.SelectedItems)
				item.Selected = false;

			btnQueueAdd.Enabled = x;
			btnQueueRemove.Enabled = x;
			btnQueueMoveUp.Enabled = x;
			btnQueueMoveDown.Enabled = x;

			grpPictureFormat.Enabled = x;
			grpPictureBasic.Enabled = x;
			grpPictureQuality.Enabled = x;
			chkPictureYadif.Enabled = x;
			grpPictureYadif.Enabled = x;

			grpVideoBasic.Enabled = x;
			grpVideoRateCtrl.Enabled = x;
			txtVideoCmd.Enabled = x;

			grpAudioBasic.Enabled = x;
			txtAudioCmd.Enabled = x;

			chkSubEnable.Enabled = x;
			chkAttachEnable.Enabled = x;

			cboProfile.Enabled = x;
			btnProfileSave.Enabled = x;

			txtDestination.Enabled = x;
			btnBrowse.Enabled = x;

			btnConfig.Enabled = x;
		}

		#region Queue Menu
		private void tsmiQueuePreview_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				foreach (var item in Directory.GetFiles(Path.Combine(Properties.Settings.Default.DirTemp), "video*"))
				{
					TaskManager.Run(String.Format("\"{0}\" \"{1}\" > {2} 2>&1", Plugin.FPLAY, item, OS.Null));
				}
			}
			else
			{
				MessageBox.Show("Please select only one video for preview", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void tsmiBenchmark_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				if (!(lstQueue.SelectedItems[0].Tag as Queue).Data.IsFileAvs)
				{
					Benchmark((lstQueue.SelectedItems[0].Tag as Queue).Data.File);
				}
				else
				{
					MessageBox.Show("Please select non AviSynth media");
				}
			}
			else
			{
				var msgbox = MessageBox.Show("No video file selected, use RAW 4K video as benchmark?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (msgbox == DialogResult.Yes)
				{
					if (!Directory.Exists(Global.Folder.Benchmark))
						Directory.CreateDirectory(Global.Folder.Benchmark);

					if (!File.Exists(Path.Combine(Global.Folder.Benchmark, "gsmarena_v001.mp4")))
					{
						var msgbox2 = MessageBox.Show("File not exist, let us download before start?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
						if (msgbox2 == DialogResult.Yes)
						{
							using (var dl = new frmDownload("http://cdn.gsmarena.com/vv/reviewsimg/oneplus-one/camera/gsmarena_v001.mp4", Path.Combine(Global.Folder.Benchmark, "gsmarena_v001.temp")))
							{
								var result = dl.ShowDialog();
								if (result == DialogResult.OK)
								{
									File.Move(dl.SavePath, Global.File.Benchmark4K);
									Benchmark(Global.File.Benchmark4K);
								}
							}
						}
					}
					else
					{
						Benchmark(Global.File.Benchmark4K);
					}
				}
			}
		}

		void Benchmark(string file)
		{
			string extsfile = Properties.Settings.Default.DefaultBenchmark;
			string typename = Path.GetFileNameWithoutExtension(extsfile);

			Assembly asm = Assembly.LoadFrom(Path.Combine("extension", extsfile));
			Type type = asm.GetType(typename + ".frmMain");
			Form form = (Form)Activator.CreateInstance(type, new object[] { file, Properties.Settings.Default.Compiler, "eng" });
			form.ShowDialog();
		}

		private void tsmiQueueSelectAll_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstQueue.Items)
			{
				item.Selected = true;
			}
		}

		private void tsmiQueueSelectNone_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstQueue.Items)
			{
				item.Selected = false;
			}
		}

		private void tsmiQueueSelectInvert_Click(object sender, EventArgs e)
		{
			foreach (ListViewItem item in lstQueue.Items)
			{
				item.Selected = !item.Selected;
			}
		}

		private void tsmiQueueAviSynthEdit_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				var item = (lstQueue.SelectedItems[0].Tag as Queue);
				if (item.Data.IsFileAvs)
				{
					string extsfile = Properties.Settings.Default.DefaultNotepad;
					string typename = Path.GetFileNameWithoutExtension(extsfile); // get namespace

					Assembly asm = Assembly.LoadFrom(Path.Combine("extension", extsfile));
					Type type = asm.GetType(typename + ".frmMain");
					Form form = (Form)Activator.CreateInstance(type, new object[] { item.Data.File, "und" });
					form.ShowDialog();

					lstQueue.SelectedItems[0].SubItems[1].Text = GetInfo.FileSize(item.Data.File); // refresh new size
				}
				else
				{
					MessageBox.Show("This not an AviSynth script");
				}
			}
			else
			{
				MessageBox.Show("Please select one");
			}
		}

		private void tsmiQueueAviSynthGenerate_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				var item = (lstQueue.SelectedItems[0].Tag as Queue);
				if (!item.Data.IsFileAvs)
				{
					var msgbox = MessageBox.Show("Do you want to change this file into an AviSynth script\n(used for bypass FFmpeg decoder)");
					if (msgbox == DialogResult.OK)
					{
						UTF8Encoding UTF8 = new UTF8Encoding(false);
						string newfile = Path.Combine(Path.GetDirectoryName(item.Data.File), Path.GetFileNameWithoutExtension(item.Data.File)) + ".avs";

						File.WriteAllText(newfile, String.Format("DirectShowSource(\"{0}\")", item.Data.File), UTF8);

						lstQueue.SelectedItems[0].SubItems[0].Text = GetInfo.FileName(newfile);
						lstQueue.SelectedItems[0].SubItems[1].Text = GetInfo.FileSize(newfile);
						lstQueue.SelectedItems[0].SubItems[2].Text = "AviSynth Script";
						item.Data.IsFileAvs = true;
						item.Data.File = newfile;
					}
				}
				else
				{
					MessageBox.Show("Please select non AviSynth Script");
				}
			}
			else
			{
				MessageBox.Show("Please select one");
			}
		}

		// Runtime Menu
		void tsmi_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				ToolStripMenuItem menu = sender as ToolStripMenuItem;
				var queue = lstQueue.SelectedItems[0].Tag as Queue;

				string extsfile = menu.Tag as string;
				string typename = menu.Name;

				Assembly asm = Assembly.LoadFrom(Path.Combine("extension", extsfile));
				Type type = asm.GetType(typename + ".frmMain");
				Form form = (Form)Activator.CreateInstance(type, new object[] { queue.Data.File, "eng" });
				var result = form.ShowDialog();

				if (result == DialogResult.OK)
				{
					var filenew = (string)form.GetType().GetField("_fileavs").GetValue(form);

					lstQueue.SelectedItems[0].SubItems[0].Text = GetInfo.FileName(filenew);
					lstQueue.SelectedItems[0].SubItems[1].Text = GetInfo.FileSize(filenew);
					lstQueue.SelectedItems[0].SubItems[2].Text = "AviSynth Script";
					queue.Data.IsFileAvs = true;
					queue.Data.File = filenew;
				}
			}
			else
			{
				MessageBox.Show("Please select one");
			}
		}
		#endregion

		void LangCreate()
		{
			var parser = new FileIniDataParser();
			IniData data = new IniData();

			data.Sections.AddSection("INFO");
			data.Sections["INFO"].AddKey("iso", "eng"); // file id
			data.Sections["INFO"].AddKey("Name", "Anime4000");
			data.Sections["INFO"].AddKey("Version", "0.1");
			data.Sections["INFO"].AddKey("Contact", "fb.com/anime4000");

			data.Sections.AddSection("frmMain");
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
							data.Sections["frmMain"].AddKey(ctrl.Name, ctrl.Text.Replace("\n", "\\n"));

			} while (ctrl != null);

			parser.WriteFile(Path.Combine("lang", "eng.core.ini"), data, Encoding.UTF8);
		}
	}
}
