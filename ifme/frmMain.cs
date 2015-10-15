using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Reflection;
using System.Diagnostics;
using System.Text.RegularExpressions;

using IniParser;
using IniParser.Model;
using MediaInfoDotNet;

using static ifme.Properties.Settings;

namespace ifme
{
    public partial class frmMain : Form
	{
		StringComparison IC = StringComparison.OrdinalIgnoreCase; // Just ignore case what ever it is.

		public frmMain()
		{
			InitializeComponent();

			Icon = Properties.Resources.ifme5;

			pbxRight.Parent = pbxLeft;
			pbxLeft.Image = Properties.Resources.BannerA;
			pbxRight.Image = Global.GetRandom % 2 != 0 ? Properties.Resources.BannerB : Properties.Resources.BannerC;
		}

		#region Form
		private void frmMain_Load(object sender, EventArgs e)
		{
			// Language UI
#if MAKELANG
			LangCreate();
#else
			LangApply();
#endif

			// Features & Fix
			if (OS.IsLinux)
			{
				tsmiQueuePreview.Enabled = false;
				tsmiBenchmark.Enabled = false;

				// Interface fix - Picture Tab
				grpPictureFormat.Height += 20;
				grpPictureBasic.Height += 20;
				grpPictureQuality.Height += 20;
				grpPictureYadif.Top += 6;
				grpPictureYadif.Height += 20;
				chkPictureYadif.Top += 8;
				chkPictureVideoCopy.Top += 22;

				grpAudioDrop.Top += 6;
				grpAudioDrop.Height -= 6;
			}

			tsmiQueueAviSynth.Enabled = Plugin.IsExistAviSynth;
			tsmiQueueAviSynthEdit.Enabled = Plugin.IsExistAviSynth;
			tsmiQueueAviSynthGenerate.Enabled = Plugin.IsExistAviSynth;

			// Picture
			if (OS.Is64bit)
				cboPictureBit.Items.Add("12");

			// Audio
			foreach (var item in Plugin.List)
			{
				if (item.Info.Type.ToLower() == "audio")
				{
					cboAudioEncoder.Items.Add(item.Profile.Name);
				}
			}

			// Add language list
			foreach (var item in File.ReadAllLines("iso.code"))
				cboSubLang.Items.Add(item);

			// Setting ready
			chkDestination.Checked = Default.IsDirOutput;
			txtDestination.Text = Default.DirOutput;

			// Add profile
			ProfileAdd();

			// Extension menu (runtime)
			foreach (var item in Extension.Items)
			{
				if (!string.Equals(item.Type, "AviSynth", IC))
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

			// Fun
#if !STEAM
			var TopThree = File.ReadAllLines(Path.Combine(Global.Folder.App, "metauser.if"));
			Console.WriteLine("Top #3 donor");
			Console.WriteLine("------------");
			for (int i = 1; i <= 3; i++)
				Console.WriteLine($"{i}. {TopThree[i]}");

			Console.WriteLine("\nThank You for your support!\n");
#else
			btnDonate.Visible = false;
			sptVert4.Visible = false;
#endif
		}

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(ObjectIO.FileName))
				QueueListOpen(ObjectIO.FileName);

			QueueListFile(ObjectIO.FileName);

			// Tell user there are new version can be downloaded
			if (Global.App.NewRelease)
			{
				InvokeLog("New version available, visit: https://x265.github.io/");
				tipUpdate.Show(null, pbxRight, 0); // should fix ballon position: http://stackoverflow.com/a/4646021

				tipUpdate.IsBalloon = true;
				tipUpdate.ToolTipTitle = Language.TipUpdateTitle;
				tipUpdate.Show(Language.TipUpdateMessage, pbxRight, 488, pbxRight.Height / 2, 30000);
			}
		}

		private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (lstQueue.Items.Count > 1)
			{
				var MsgBox = MessageBox.Show(Language.Quit, null, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
				if (MsgBox == DialogResult.Yes)
				{
					if (string.IsNullOrEmpty(ObjectIO.FileName))
						QueueListSaveAs();
					else
						QueueListSave();

					btnQueueStop.PerformClick();
				}
				else if (MsgBox == DialogResult.Cancel)
				{
					e.Cancel = true;
					return;
				}
			}
		}
		#endregion

		private void pbxRight_Click(object sender, EventArgs e)
		{
			Process.Start("https://x265.github.io/");
		}

		private void pbxLeft_Click(object sender, EventArgs e)
		{
			Process.Start("https://x265.github.io/");
		}

		#region Profile
		void ProfileAdd()
		{
			// Clear before adding object
			cboProfile.Items.Clear();

			// Add new object
			cboProfile.Items.Add("< new >");
			foreach (var item in Profile.List)
				cboProfile.Items.Add($"{item.Info.Platform}: {item.Info.Format.ToUpper()} {item.Info.Name}");

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
				cboPictureBit.Text = $"{p.Picture.BitDepth}";
				cboPictureYuv.Text = $"{p.Picture.Chroma}";

				cboVideoPreset.Text = p.Video.Preset;
				cboVideoTune.Text = p.Video.Tune;
				cboVideoType.SelectedIndex = p.Video.Type;
				txtVideoValue.Text = p.Video.Value;
				txtVideoCmd.Text = p.Video.Command;

				bool exist = Plugin.IsExist(p.Audio.Encoder);
                cboAudioEncoder.Text = exist ? p.Audio.Encoder : Plugin.Default.Audio.Name;
				cboAudioBit.Text = exist ? p.Audio.BitRate : Plugin.Default.Audio.BitRate;
				cboAudioFreq.Text = exist ? p.Audio.Frequency : Plugin.Default.Audio.Frequency;
				cboAudioChannel.Text = exist ? p.Audio.Channel : Plugin.Default.Audio.Channel;
				chkAudioMerge.Checked = exist ? p.Audio.Merge : Plugin.Default.Audio.Merge;
				txtAudioCmd.Text = exist ? p.Audio.Command : Plugin.Default.Audio.Command;
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
				using (var form = new frmInputBox(Language.SaveNewProfilesTitle, Language.SaveNewProfilesInfo, ""))
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

				file = Path.Combine(Global.Folder.Profile, $"{DateTime.Now:yyyyMMdd_HHmmss}.ifp");
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
		private void btnDonate_Click(object sender, EventArgs e)
		{
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U");
		}

		private void chkDestination_CheckedChanged(object sender, EventArgs e)
		{
			var x = chkDestination.Checked;
			Default.IsDirOutput = x;
			txtDestination.Enabled = x;
			btnBrowse.Enabled = x;
		}

		private void txtDestination_TextChanged(object sender, EventArgs e)
		{
			Default.DirOutput = txtDestination.Text;
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog GetDir = new FolderBrowserDialog();

			GetDir.Description = "";
			GetDir.ShowNewFolderButton = true;
			GetDir.RootFolder = Environment.SpecialFolder.MyComputer;

			if (GetDir.ShowDialog() == DialogResult.OK)
			{
				if (GetDir.SelectedPath[0] == '\\' && GetDir.SelectedPath[1] == '\\')
					InvokeLog("Over network not supported, please mount it as drive");

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
			if (GetInfo.IsPathNetwork(file))
			{
				InvokeLog($"Rejected! Please mount as \"Network Drive\" File: {file}");
				return;
			}

			string FileType = null;
			string FileOut = null;
			var Info = new Queue();

			var i = cboProfile.SelectedIndex;			// Profiles
			var p = Profile.List[i == 0 ? 0 : i - 1];	// When profiles at <new> mean auto detect

			Info.Data.File = file;
			Info.Data.SaveAsMkv = i == 0 ? true : string.Equals(p.Info.Format, "mkv", IC);

			MediaFile AVI = new MediaFile(file);

			Info.Data.IsFileMkv = string.Equals(AVI.format, "Matroska", IC);
			Info.Data.IsFileAvs = GetInfo.IsAviSynth(file);

			if (!Plugin.IsExistAviSynth)
			{
				if (Info.Data.IsFileAvs)
				{
					if (Plugin.IsForceAviSynth)
					{
						InvokeLog($"Forcing AviSynth added to queue: {file}");
					}
					else
					{
						InvokeLog($"AviSynth not installed, skipping this file: {file}");
						return;
					}
				}
			}

			// Picture section
			if (AVI.Video.Count > 0)
			{
				var Video = AVI.Video[0];
				Info.Picture.Resolution = i == 0 ? "auto" : p.Picture.Resolution;
				Info.Picture.FrameRate = i == 0 ? "auto" : p.Picture.FrameRate;
				Info.Picture.BitDepth = i == 0 ? Video.bitDepth : p.Picture.BitDepth;
				Info.Picture.Chroma = i == 0 ? 420 : p.Picture.Chroma;

				Info.Prop.Duration = Video.duration;
				Info.Prop.FrameCount = Video.frameCount;

				Info.Picture.IsCopy = false;
				Info.Picture.IsHevc = string.Equals(Video.format, "HEVC", IC);

				FileType = $"{Path.GetExtension(file).ToUpper()} ({Video.format})";
				FileOut = $".{(Info.Data.SaveAsMkv ? "MKV" : "MP4")} (H265)";

				if (string.Equals(Video.frameRateMode, "vfr", IC))
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
					Info.Picture.BitDepth = 8;
					Info.Picture.Chroma = 420;

					FileType = "AviSynth Script";
					FileOut = $".{(Info.Data.SaveAsMkv ? "MKV" : "MP4")} (H265)";
				}
				else
				{
					if (AVI.Audio.Count > 0)
					{
						var Audio = AVI.Audio[0];
						FileType = $"{Path.GetExtension(file).ToUpper()} ({Audio.format})";
						FileOut = $".{(Info.Data.SaveAsMkv ? "MKV" : "MP4")}";
                    }
					else
					{
						FileType = "Unknown";
						FileOut = $".{(Info.Data.SaveAsMkv ? "MKV" : "MP4")} (H265)";
                    }
				}
			}

			// Video section
			Info.Video.Preset = i == 0 ? "medium" : p.Video.Preset;
			Info.Video.Tune = i == 0 ? "off" : p.Video.Tune;
			Info.Video.Type = i == 0 ? 0 : p.Video.Type;
			Info.Video.Value = i == 0 ? "26" : p.Video.Value;
			Info.Video.Command = i == 0 ? "--dither" : p.Video.Command;

			// Audio section
			bool exist = Plugin.IsExist(p.Audio.Encoder);
            Info.Audio.Encoder = i == 0 || !exist ? Plugin.Default.Audio.Name : p.Audio.Encoder;
			Info.Audio.BitRate = i == 0 || !exist ? Plugin.Default.Audio.BitRate : p.Audio.BitRate;
			Info.Audio.Frequency = i == 0 || !exist ? Plugin.Default.Audio.Frequency : p.Audio.Frequency;
			Info.Audio.Channel = i == 0 || !exist ? Plugin.Default.Audio.Channel : p.Audio.Channel;
			Info.Audio.Command = i == 0 || !exist ? Plugin.Default.Audio.Command : p.Audio.Command;

			// Drop audio tracks support
			foreach (var item in GetStream.Media(file, StreamType.Audio))
				Info.DropAudioId.Add(new DropAudio { Id = item.ID, Text = $"{item.ID}, {item.Lang}, {item.OtherInfo}", Checked = false });
			
			// Add to queue list
			ListViewItem qItem = new ListViewItem(new[] {
				GetInfo.FileName(file),
				GetInfo.FileSize(file),
				FileType,
				FileOut,
				"Ready"
			});
			qItem.Tag = Info;
			qItem.Checked = true;
			lstQueue.Items.Add(qItem);

			// Print to log
			InvokeLog($"File added {Info.Data.File}");
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
				InvokeLog($"File removed {item.SubItems[0].Text}");
			}
		}
		#endregion

		#region Queue: Display item properties
		private void lstQueue_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count > 0)
			{
				QueueDisplay(lstQueue.SelectedItems[0].Index);
			}
			else
			{
				QueueUnselect();
			}
		}

		void QueueDisplay(int index)
		{
			var Info = (Queue)lstQueue.Items[index].Tag;

			// Picture
			rdoMKV.Checked = Info.Data.SaveAsMkv;
			rdoMP4.Checked = !rdoMKV.Checked;
			cboPictureRes.Text = Info.Picture.Resolution;
			cboPictureFps.Text = Info.Picture.FrameRate;
			cboPictureBit.Text = $"{Info.Picture.BitDepth}";
			cboPictureYuv.Text = $"{Info.Picture.Chroma}";
			chkPictureYadif.Checked = Info.Picture.YadifEnable;
			cboPictureYadifMode.SelectedIndex = Info.Picture.YadifMode;
			cboPictureYadifField.SelectedIndex = Info.Picture.YadifField;
			cboPictureYadifFlag.SelectedIndex = Info.Picture.YadifFlag;

			chkPictureVideoCopy.Enabled = Info.Picture.IsHevc;
			chkPictureVideoCopy.Checked = Info.Picture.IsCopy;

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

			// Audio Tracks
			chkAudioDrop.Checked = Info.DropAudioTracks;

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
			// Audio Drop
			chkAudioDrop.Checked = false;
			clbAudioTracks.Items.Clear();

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
			PluginAudioCheck();
			QueueUpdate(QueueProp.FormatMkv);
		}

		private void rdoMP4_CheckedChanged(object sender, EventArgs e)
		{
			PluginAudioCheck();
			QueueUpdate(QueueProp.FormatMp4);
		}

		void PluginAudioCheck()
		{
			if (rdoMP4.Checked)
			{
				foreach (var item in Plugin.List)
				{
					if (string.Equals(item.Info.Type, "audio", IC))
					{
						if (string.Equals(item.Profile.Name, cboAudioEncoder.Text, IC))
						{
							if (!string.Equals(item.Info.Support, "mp4", IC))
							{
								InvokeLog($"Encoder \"{cboAudioEncoder.Text}\" was not supported by MP4, choose MKV or different encoder");
								cboAudioEncoder.SelectedIndex = 1;
                            }
						}
					}
				}
			}
		}

		private void cboPictureRes_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureResolution);
		}

		private void cboPictureRes_Leave(object sender, EventArgs e)
		{
			Regex regex = new Regex("([0-9+x])|([auto])");
			MatchCollection matches = regex.Matches(cboPictureRes.Text);

			if (matches.Count != cboPictureRes.Text.Length)
			{
				cboPictureRes.Text = "auto";
				InvokeLog("Input resolution format was invalid, back to auto.");
			}

			// Prevent 32bit PC encode more then Full HD
			if (!OS.Is64bit)
				if (!string.Equals("auto", cboPictureRes.Text, IC))
					if (Convert.ToInt32(cboPictureRes.Text.Split('x')[0]) * Convert.ToInt32(cboPictureRes.Text.Split('x')[1]) > 2073600)
						cboPictureRes.Text = "1920x1080";
        }

		private void cboPictureFps_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureFrameRate);
		}

		private void cboPictureFps_Leave(object sender, EventArgs e)
		{
			Regex regex = new Regex("([0-9+.])|([auto])");
			MatchCollection matches = regex.Matches(cboPictureFps.Text);

			if (matches.Count != cboPictureFps.Text.Length)
			{
				cboPictureFps.Text = "auto";
				InvokeLog("Input frame rate format was invalid, back to auto.");
			}
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

		private void chkPictureVideoCopy_CheckedChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.PictureCopyVideo);
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

					lblVideoRateValue.Text = Language.Get[Name][lblVideoRateValue.Name];
					txtVideoValue.Text = $"{(trkVideoRate.Value = 260) / 10:0.0}";
                    break;

				case 1:
					lblVideoRateH.Visible = true;
					lblVideoRateL.Visible = true;
					trkVideoRate.Visible = true;

					trkVideoRate.Minimum = 0;
					trkVideoRate.Maximum = 51;
					trkVideoRate.TickFrequency = 1;

					lblVideoRateValue.Text = Language.Get[Name][lblVideoRateValue.Name];
					txtVideoValue.Text = Convert.ToString(trkVideoRate.Value = 26);
					break;

				default:
					lblVideoRateH.Visible = false;
					lblVideoRateL.Visible = false;
					trkVideoRate.Visible = false;

					lblVideoRateValue.Text = $"{Language.Get[Name][lblVideoRateValue.Name].Replace(":", "")} (kbps):";
					txtVideoValue.Text = "2048";
					break;
			}

			QueueUpdate(QueueProp.VideoType);
		}

		private void txtVideoValue_Leave(object sender, EventArgs e)
		{
			var i = cboVideoType.SelectedIndex;
			if (i == 0)
				if (!string.IsNullOrEmpty(txtVideoValue.Text))
					if (Convert.ToDouble(txtVideoValue.Text) >= 51.0)
						txtVideoValue.Text = "51";
					else if (Convert.ToDouble(txtVideoValue.Text) <= 0.0)
						txtVideoValue.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(Convert.ToDouble(txtVideoValue.Text) * 10.0);
				else
					trkVideoRate.Value = 0;
			else if (i == 1)
				if (!string.IsNullOrEmpty(txtVideoValue.Text))
					if (Convert.ToInt32(txtVideoValue.Text) >= 51)
						txtVideoValue.Text = "51";
					else if (Convert.ToInt32(txtVideoValue.Text) <= 0)
						txtVideoValue.Text = "0";
					else
						trkVideoRate.Value = Convert.ToInt32(txtVideoValue.Text);
				else
					trkVideoRate.Value = 0;
		}

		private void txtVideoValue_TextChanged(object sender, EventArgs e)
		{
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
				txtVideoValue.Text = $"{Convert.ToDouble(trkVideoRate.Value) * 0.1:0.0}";
			else
				txtVideoValue.Text = Convert.ToString(trkVideoRate.Value);
		}

		private void txtVideoCmd_TextChanged(object sender, EventArgs e)
		{
			QueueUpdate(QueueProp.VideoCommand);
		}
		#endregion

		#region Queue: Property update - Audio Tab
		private void chkAudioDrop_CheckedChanged(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				var qitem = lstQueue.SelectedItems[0].Tag as Queue;
				string file = qitem.Data.File;

				if (chkAudioDrop.Checked)
				{
					clbAudioTracks.Items.Clear();

					foreach (var item in qitem.DropAudioId)
						clbAudioTracks.Items.Add(item.Text, item.Checked);

					qitem.DropAudioTracks = true;
				}
				else
				{
					clbAudioTracks.Items.Clear();
                    qitem.DropAudioTracks = false;
				}
			}
			else
			{
				if (chkAudioDrop.Checked)
					MessageBox.Show(Language.OneItem);

				chkAudioDrop.Checked = false;
			}

			clbAudioTracks.Visible = chkAudioDrop.Checked;
        }

		private void clbAudioTracks_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			// Ref: http://stackoverflow.com/a/17511730
			// Due this event fire 2 times and no ItemChecked event,
			// Apply this trick or hacks

			// Copy
			CheckedListBox clb = (CheckedListBox)sender;

			// Switch off event handler
			clb.ItemCheck -= clbAudioTracks_ItemCheck;
			clb.SetItemCheckState(e.Index, e.NewValue);

			// Switch on event handler
			clb.ItemCheck += clbAudioTracks_ItemCheck;

			// Do stuff
			if (lstQueue.SelectedItems.Count == 1)
				(lstQueue.SelectedItems[0].Tag as Queue).DropAudioId[e.Index].Checked = e.NewValue == CheckState.Checked ? true : false;
		}

		private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
		{
			PluginAudioCheck();

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
					MessageBox.Show(Language.NotSupported, "");
					chkSubEnable.Checked = false;
					return;
				}
				
				if (lstQueue.SelectedItems.Count == 0 || lstQueue.SelectedItems.Count >= 2)
				{
					MessageBox.Show(Language.OneVideo, "");
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
				MessageBox.Show(Language.SelectOneVideoSubtitle);
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
					MessageBox.Show(Language.NotSupported, "");
					chkAttachEnable.Checked = false;
					return;
				}

				if (lstQueue.SelectedItems.Count == 0 || lstQueue.SelectedItems.Count >= 2)
				{
					MessageBox.Show(Language.OneVideo, "");
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
				MessageBox.Show(Language.SelectOneVideoAttch, "Error");
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
			if (lstQueue.SelectedItems.Count == 0)
			{
				tipNotify.ToolTipIcon = ToolTipIcon.Warning;
				tipNotify.ToolTipTitle = Language.TipUpdateTitle;
				tipNotify.Show(Language.OneItem, tabConfig, 0, 0, 5000);
			}

			foreach (ListViewItem item in lstQueue.SelectedItems)
			{
				var X = item.Tag as Queue;

				switch (Id)
				{
					case QueueProp.FormatMkv:
						item.SubItems[3].Text = item.SubItems[3].Text.Replace("MP4","MKV");
						X.Data.SaveAsMkv = true;
						break;

					case QueueProp.FormatMp4:
						item.SubItems[3].Text = item.SubItems[3].Text.Replace("MKV", "MP4");
						X.Data.SaveAsMkv = false;
						break;

					case QueueProp.PictureResolution:
						X.Picture.Resolution = cboPictureRes.Text;
						break;

					case QueueProp.PictureFrameRate:
						X.Picture.FrameRate = cboPictureFps.Text;
						break;

					case QueueProp.PictureBitDepth:
						X.Picture.BitDepth = Convert.ToInt32(cboPictureBit.Text);
						break;

					case QueueProp.PictureChroma:
						X.Picture.Chroma = Convert.ToInt32(cboPictureYuv.Text);
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

					case QueueProp.PictureCopyVideo:
						if (X.Picture.IsHevc)
							X.Picture.IsCopy = chkPictureVideoCopy.Checked;
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

		#region Queue Menu
		private void tsmiQueuePreview_Click(object sender, EventArgs e)
		{
			if (lstQueue.SelectedItems.Count == 1)
			{
				foreach (var item in Directory.GetFiles(Path.Combine(Properties.Settings.Default.DirTemp), "video*"))
				{
					TaskManager.Run($"\"{Plugin.FPLAY}\" \"{item}\" > {OS.Null} 2>&1");
				}
			}
			else
			{
				MessageBox.Show(Language.SelectOneVideoPreview, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
					MessageBox.Show(Language.SelectNotAviSynth);
				}
			}
			else
			{
				var msgbox = MessageBox.Show(Language.BenchmarkNoFile, "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (msgbox == DialogResult.Yes)
				{
					if (!Directory.Exists(Global.Folder.Benchmark))
						Directory.CreateDirectory(Global.Folder.Benchmark);

					if (!File.Exists(Path.Combine(Global.Folder.Benchmark, "gsmarena_v001.mp4")))
					{
						var msgbox2 = MessageBox.Show(Language.BenchmarkDownload, "", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
						if (msgbox2 == DialogResult.Yes)
						{
							using (var dl = new frmDownload("http://cdn.gsmarena.com/vv/reviewsimg/oneplus-one/camera/gsmarena_v001.mp4", Path.Combine(Global.Folder.Benchmark, "gsmarena_v001.temp")))
							{
								var result = dl.ShowDialog();
								if (result == DialogResult.OK)
								{
									File.Move(dl.SavePath, Global.Files.Benchmark4K);
									Benchmark(Global.Files.Benchmark4K);
								}
							}
						}
					}
					else
					{
						Benchmark(Global.Files.Benchmark4K);
					}
				}
			}
		}

		void Benchmark(string file)
		{
			string extsfile = Default.DefaultBenchmark;
			string typename = Path.GetFileNameWithoutExtension(extsfile);

			Assembly asm = Assembly.LoadFrom(Path.Combine("extension", extsfile));
			Type type = asm.GetType(typename + ".frmMain");
			Form form = (Form)Activator.CreateInstance(type, new object[] { file, Default.Compiler, "eng" });
			form.ShowDialog();
		}

		private void tsmiQueueNew_Click(object sender, EventArgs e)
		{
			if (lstQueue.Items.Count > 0)
			{
				var MsgBox = MessageBox.Show(Language.QueueOpenChange, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (MsgBox == DialogResult.Yes)
					tsmiQueueSave.PerformClick();
				else if (MsgBox == DialogResult.Cancel)
					return;

				lstQueue.Items.Clear();
				QueueListFile(null);
			}
		}

		private void QueueListFile(string file)
		{
			// Program Start, New, Open, Save As
			if (string.IsNullOrEmpty(file))
				Text = $"Untitled - {Global.App.NameFull}";
			else
				Text = $"{Path.GetFileName(file)} - {Global.App.NameFull}";

			ObjectIO.FileName = file;
		}

		private void tsmiQueueOpen_Click(object sender, EventArgs e)
		{
			if (lstQueue.Items.Count > 0)
			{
				var MsgBox = MessageBox.Show(Language.QueueOpenChange, "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
				if (MsgBox == DialogResult.Yes)
				{
					tsmiQueueSave.PerformClick();
				}
				else if (MsgBox == DialogResult.Cancel)
				{
					return;
				}
			}

			// Empty list and "No" button fall this code
			OpenFileDialog GetFile = new OpenFileDialog();
			GetFile.Filter = "eXtensible Markup Language|*.xml";
            GetFile.FilterIndex = 1;
			GetFile.Multiselect = false;

			if (GetFile.ShowDialog() == DialogResult.OK)
				QueueListOpen(GetFile.FileName);
		}

		private void QueueListOpen(string file)
		{
			lstQueue.Items.Clear(); // clear all listing

			InvokeLog($"Opening queue file: {file}");

			List<Queue> gg = ObjectIO.IsValidXml(file) ?
				ObjectIO.ReadFromXmlFile<List<Queue>>(file) :
				ObjectIO.ReadFromBinaryFile<List<Queue>>(file);

			// Remove file that not exist
			for (int i = 0; i < gg.Count; i++)
			{
				if (!File.Exists(gg[i].Data.File))
				{
					InvokeLog($"File not found: {gg[i].Data.File}");
					gg.RemoveAt(i);
				}
			}

			foreach (var item in gg)
			{
				if (GetInfo.IsAviSynth(item.Data.File))
					if (!Plugin.IsExistAviSynth)
						continue;

				ListViewItem qItem = new ListViewItem(new[] {
						GetInfo.FileName(item.Data.File),
						GetInfo.FileSize(item.Data.File),
						$"{Path.GetExtension(item.Data.File).ToUpper()} ({new MediaFile(item.Data.File).Video[0].format})",
						$"{(item.Data.SaveAsMkv ? ".MKV" : ".MP4")} (HEVC)",
						item.IsEnable ? "Ready" : "Done"
					});

				qItem.Tag = item;
				qItem.Checked = item.IsEnable;
				lstQueue.Items.Add(qItem);
			}

			QueueListFile(file);
		}

		private void tsmiQueueSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(ObjectIO.FileName))
			{
				tsmiQueueSaveAs.PerformClick();
			}
			else
			{
				QueueListSave();
            }
		}

		private void QueueListSave()
		{
			List<Queue> gg = new List<Queue>();
			foreach (ListViewItem item in lstQueue.Items)
			{
				(item.Tag as Queue).IsEnable = item.Checked;
				gg.Add(item.Tag as Queue);
			}

			if (ObjectIO.IsValidXml(ObjectIO.FileName))
				ObjectIO.WriteToXmlFile(ObjectIO.FileName, gg);
			else
				ObjectIO.WriteToBinaryFile(ObjectIO.FileName, gg);
		}

		private void tsmiQueueSaveAs_Click(object sender, EventArgs e)
		{
			var MsgBox = MessageBox.Show(Language.QueueSave, "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
			if (MsgBox == DialogResult.Yes)
			{
				if (lstQueue.Items.Count > 1)
				{
					QueueListSaveAs();
				}
				else
				{
					MessageBox.Show(Language.QueueSaveError);
				}
			}
		}

		private void QueueListSaveAs()
		{
			List<Queue> gg = new List<Queue>();
			foreach (ListViewItem item in lstQueue.Items)
			{
				(item.Tag as Queue).IsEnable = item.Checked;
				gg.Add(item.Tag as Queue);
			}

			SaveFileDialog SaveFile = new SaveFileDialog();
			SaveFile.Filter = "eXtensible Markup Language|*.xml";
			SaveFile.FilterIndex = 1;

			if (SaveFile.ShowDialog() == DialogResult.OK)
			{
				ObjectIO.WriteToXmlFile(SaveFile.FileName, gg);
				QueueListFile(SaveFile.FileName);
			}
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
					Form form = (Form)Activator.CreateInstance(type, new object[] { item.Data.File, Properties.Settings.Default.Language });
					form.ShowDialog();

					lstQueue.SelectedItems[0].SubItems[1].Text = GetInfo.FileSize(item.Data.File); // refresh new size
				}
				else
				{
					MessageBox.Show(Language.SelectAviSynth);
				}
			}
			else
			{
				MessageBox.Show(Language.OneItem);
			}
		}

		private void tsmiQueueAviSynthGenerate_Click(object sender, EventArgs e)
		{
			var msgbox = MessageBox.Show(Language.VideoToAviSynth);
			if (msgbox == DialogResult.OK)
			{
				if (lstQueue.SelectedItems.Count > 0)
				{
					foreach (ListViewItem items in lstQueue.SelectedItems)
					{
						var item = (items.Tag as Queue);
						if (!item.Data.IsFileAvs)
						{
							UTF8Encoding UTF8 = new UTF8Encoding(false);
							string newfile = Path.Combine(Path.GetDirectoryName(item.Data.File), Path.GetFileNameWithoutExtension(item.Data.File)) + ".avs";

							File.WriteAllText(newfile, $"{Properties.Settings.Default.AvsDecoder}(\"{item.Data.File}\")", UTF8);

							items.SubItems[0].Text = GetInfo.FileName(newfile);
							items.SubItems[1].Text = GetInfo.FileSize(newfile);
							items.SubItems[2].Text = "AviSynth Script";
							item.Data.IsFileAvs = true;
							item.Data.File = newfile;
						}
					}
				}
				else
				{
					MessageBox.Show(Language.OneItem);
				}
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
				Form form = (Form)Activator.CreateInstance(type, new object[] { queue.Data.File, Properties.Settings.Default.Language });
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
				MessageBox.Show(Language.OneItem);
			}
		}
#endregion

		#region Encoding
		private void btnQueueStart_Click(object sender, EventArgs e)
		{
			if (lstQueue.Items.Count == 0)
				return;

			// Send a new copy to another thread
			if (!bgwEncoding.IsBusy)
			{
				// Make a copy, thread safe
				List<object> gg = new List<object>();

				foreach (ListViewItem item in lstQueue.Items)
				{
					(item.Tag as Queue).IsEnable = item.Checked;
                    gg.Add(item.Tag);
				}

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

				// Remove temp file
				MediaEncoder.CleanUp();

				// Time current queue
				var SessionCurrent = DateTime.Now;

				// Log current queue
				InvokeLog("Processing: " + item.Data.File);

				// AviSynth aware
				string file = item.Data.File;
				string filereal = GetStream.AviSynthGetFile(file);

				// Extract mkv embedded subtitle, font and chapter
				InvokeQueueStatus(id, "Extracting");
				MediaEncoder.Extract(filereal, item);

				// User cancel
				if (bgwEncoding.CancellationPending)
				{
					InvokeQueueAbort(id);
					e.Cancel = true;
					return;
				}

				// Audio
				InvokeQueueStatus(id, "Processing Audio");
				MediaEncoder.Audio(filereal, item);

				// User cancel
				if (bgwEncoding.CancellationPending)
				{
					InvokeQueueAbort(id);
					e.Cancel = true;
					return;
				}

				// Video
				InvokeQueueStatus(id, "Processing Video");
				MediaEncoder.Video(file, item);

				// User cancel
				if (bgwEncoding.CancellationPending)
				{
					InvokeQueueAbort(id);
					e.Cancel = true;
					return;
				}

				// Mux
				InvokeQueueStatus(id, "Compiling");
				MediaEncoder.Mux(item);

				// User cancel
				if (bgwEncoding.CancellationPending)
				{
					InvokeQueueAbort(id);
					e.Cancel = true;
					return;
				}

				string timeDone = GetInfo.Duration(SessionCurrent);
				InvokeQueueDone(id, timeDone);
				InvokeLog($"Completed in {timeDone} for {item.Data.File}");
			}

			// Tell user all is done
			InvokeLog($"All Queue Completed in {GetInfo.Duration(Session)}");
		}

		private void bgwEncoding_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			Console.Title = "IFME console";
			btnQueueStart.Visible = true;
			btnQueuePause.Visible = false;
			ControlEnable(true);

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
				if (Default.SoundFinish)
				{
					SoundPlayer notification = new SoundPlayer(Global.Sounds.Finish);
					notification.Play();
				}
			}
		}

		void InvokeQueueStatus(int index, string s)
		{
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = s));
			else
				lstQueue.Items[index].SubItems[4].Text = s;
		}

		void InvokeQueueAbort(int index)
		{
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = "Abort!"));
			else
				lstQueue.Items[index].SubItems[4].Text = "Abort!";
		}

		void InvokeQueueDone(int index, string message)
		{
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].Checked = false));
			else
				lstQueue.Items[index].Checked = false;

			string a = $"Finished in {message}";
			if (InvokeRequired)
				BeginInvoke(new MethodInvoker(() => lstQueue.Items[index].SubItems[4].Text = a));
			else
				lstQueue.Items[index].SubItems[4].Text = a;
		}

		void InvokeLog(string message)
		{
			message = $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}] {message}";

			if (InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(() => Console.WriteLine(message)));
				BeginInvoke(new MethodInvoker(() => txtLog.AppendText(message + "\r\n")));
			}
			else
			{
				Console.WriteLine(message);
                txtLog.AppendText(message + "\r\n");
			}
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
		#endregion

		#region Language - Load and Apply
		void LangApply()
		{
			var data = Language.Get;

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

			foreach (ToolStripItem item in cmsQueueMenu.Items)
				if (item is ToolStripMenuItem)
					item.Text = data[Name][item.Name];

			foreach (ToolStripItem item in tsmiQueueAviSynth.DropDownItems)
				if (item is ToolStripMenuItem)
					item.Text = data[Name][item.Name];

			foreach (ColumnHeader item in lstQueue.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			foreach (ColumnHeader item in lstSub.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			foreach (ColumnHeader item in lstAttach.Columns)
				item.Text = data[Name][$"{item.Tag}"];

			Language.BenchmarkDownload = data[Name]["BenchmarkDownload"];
			Language.BenchmarkNoFile = data[Name]["BenchmarkNoFile"].Replace("\\n", "\n");
			Language.NotSupported = data[Name]["NotSupported"];
			Language.OneItem = data[Name]["OneItem"];
			Language.OneVideo = data[Name]["OneVideo"];
			Language.SaveNewProfilesInfo = data[Name]["SaveNewProfilesInfo"];
			Language.SaveNewProfilesTitle = data[Name]["SaveNewProfilesTitle"];
			Language.SelectAviSynth = data[Name]["SelectAviSynth"];
			Language.SelectNotAviSynth = data[Name]["SelectNotAviSynth"];
			Language.SelectOneVideoAttch = data[Name]["SelectOneVideoAttch"];
			Language.SelectOneVideoPreview = data[Name]["SelectOneVideoPreview"];
			Language.SelectOneVideoSubtitle = data[Name]["SelectOneVideoSubtitle"];
			Language.VideoToAviSynth = data[Name]["VideoToAviSynth"].Replace("\\n", "\n");
			Language.QueueSave = data.Sections[Name]["QueueSave"];
			Language.QueueSaveError = data.Sections[Name]["QueueSaveError"];
			Language.QueueOpenChange = data.Sections[Name]["QueueOpenChange"];
			Language.Quit = data.Sections[Name]["Quit"];

			Language.TipUpdateTitle = data.Sections[Name]["TipUpdateTitle"];
			Language.TipUpdateMessage = data.Sections[Name]["TipUpdateMessage"];
		}

		void LangCreate()
		{
			var parser = new FileIniDataParser();
			IniData data = new IniData();

			data.Sections.AddSection("info");
			data.Sections["info"].AddKey("Code", "en"); // file id
			data.Sections["info"].AddKey("Name", "English");
			data.Sections["info"].AddKey("Author", "Anime4000");
			data.Sections["info"].AddKey("Version", $"{Global.App.VersionRelease}");
			data.Sections["info"].AddKey("Contact", "https://github.com/Anime4000");
			data.Sections["info"].AddKey("Comment", "Please refer IETF Language Tag here: http://www.i18nguy.com/unicode/language-identifiers.html");

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

			foreach (ToolStripItem item in cmsQueueMenu.Items)
				if (item is ToolStripMenuItem)
					data.Sections[Name].AddKey(item.Name, item.Text);

			foreach (ToolStripItem item in tsmiQueueAviSynth.DropDownItems)
				if (item is ToolStripMenuItem)
					data.Sections[Name].AddKey(item.Name, item.Text);

			foreach (ColumnHeader item in lstQueue.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			foreach (ColumnHeader item in lstSub.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			foreach (ColumnHeader item in lstAttach.Columns)
				data.Sections[Name].AddKey($"{item.Tag}", item.Text);

			data.Sections.AddSection(Name);
			data.Sections[Name].AddKey("BenchmarkDownload", Language.BenchmarkDownload);
			data.Sections[Name].AddKey("BenchmarkNoFile", Language.BenchmarkNoFile);
			data.Sections[Name].AddKey("NotSupported", Language.NotSupported);
			data.Sections[Name].AddKey("OneItem", Language.OneItem);
			data.Sections[Name].AddKey("OneVideo", Language.OneVideo);
			data.Sections[Name].AddKey("SaveNewProfilesInfo", Language.SaveNewProfilesInfo);
			data.Sections[Name].AddKey("SaveNewProfilesTitle", Language.SaveNewProfilesTitle);
			data.Sections[Name].AddKey("SelectAviSynth", Language.SelectAviSynth);
			data.Sections[Name].AddKey("SelectNotAviSynth", Language.SelectNotAviSynth);
			data.Sections[Name].AddKey("SelectOneVideoAttch", Language.SelectOneVideoAttch);
			data.Sections[Name].AddKey("SelectOneVideoPreview", Language.SelectOneVideoPreview);
			data.Sections[Name].AddKey("SelectOneVideoSubtitle", Language.SelectOneVideoSubtitle);
			data.Sections[Name].AddKey("VideoToAviSynth", Language.VideoToAviSynth.Replace("\n", "\\n"));
			data.Sections[Name].AddKey("QueueSave", Language.QueueSave);
			data.Sections[Name].AddKey("QueueSaveError", Language.QueueSaveError);
			data.Sections[Name].AddKey("QueueOpenChange", Language.QueueOpenChange);
			data.Sections[Name].AddKey("Quit", Language.Quit);

			data.Sections[Name].AddKey("TipUpdateTitle", Language.TipUpdateTitle);
			data.Sections[Name].AddKey("TipUpdateMessage", Language.TipUpdateMessage);

			parser.WriteFile(Path.Combine(Global.Folder.Language, "en.ini"), data, Encoding.UTF8);		
		}
		#endregion
	}
}
