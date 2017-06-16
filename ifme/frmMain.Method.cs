using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ifme
{
	partial class frmMain
	{
		enum ListViewItemType
		{
			Media,
			Video,
			Audio,
			Subtitle
		}

		enum Direction
		{
			Up,
			Down
		}

		// BackgroundWorker with Abort support
		// refer to BackgroundWorkerEx.cs file
		private BackgroundWorkerEx bgThread = new BackgroundWorkerEx();

        public void InitializeUX()
		{
            // Show Splash Screen (hold) & Loading everyting!
            var ss = new frmSplashScreen();
            ss.ShowDialog();

			// Load user settings
			txtFolderOutput.Text = Properties.Settings.Default.OutputDir;

			cboVideoResolution.Text = "1920x1080";
			cboVideoFrameRate.Text = "23.976";
			cboVideoPixelFormat.SelectedIndex = 0;
			cboVideoDeinterlaceMode.SelectedIndex = 1;
			cboVideoDeinterlaceField.SelectedIndex = 0;

			// Display Language ComboBox
			cboSubLang.DataSource = new BindingSource(Get.LanguageCode, null);
			cboSubLang.DisplayMember = "Value";
			cboSubLang.ValueMember = "Key";
			cboSubLang.SelectedValue = "und";

			// Display MIME ComboBox
			cboAttachMime.DataSource = new BindingSource(Get.MimeType, null);
			cboAttachMime.DisplayMember = "Value";
			cboAttachMime.ValueMember = "Key";
			cboAttachMime.SelectedValue = ".ttf";

            // Load everyting
            ApplyLanguage();
            ApplyPlugins();
            ApplyEncodingPreset();

            // Draw
            DrawBanner();
		}

        private void ApplyLanguage()
        {
            if (OS.IsWindows)
                Font = Language.Lang.UIFontWindows;
            else
                Font = Language.Lang.UIFontLinux;
            
            cboEncodingPreset.Font = new System.Drawing.Font(Font.Name, 9);
            txtFolderOutput.Font = new System.Drawing.Font(Font.Name, 9);

            cmsNewImport.Font = Font;
            cmsEncodingPreset.Font = Font;

            var frm = Language.Lang.frmMain;
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
                        if (frm.ContainsKey(ctrl.Name))
                            ctrl.Text = frm[ctrl.Name];

            } while (ctrl != null);

            foreach (ToolStripMenuItem item in cmsNewImport.Items)
                if (Language.Lang.cmsNewImport.ContainsKey(item.Name))
                    item.Text = Language.Lang.cmsNewImport[item.Name];

            foreach (ToolStripMenuItem item in cmsEncodingPreset.Items)
                if (Language.Lang.cmsEncodingPreset.ContainsKey(item.Name))
                    item.Text = Language.Lang.cmsEncodingPreset[item.Name];

            cboVideoDeinterlaceMode.Items.Clear();
            cboVideoDeinterlaceMode.Items.AddRange(Language.Lang.ComboBoxDeInterlaceMode.ToArray());

            cboVideoDeinterlaceField.Items.Clear();
            cboVideoDeinterlaceField.Items.AddRange(Language.Lang.ComboBoxDeInterlaceField.ToArray());
        }

        private void ApplyPlugins()
        {
            var video = new Dictionary<Guid, string>();
            var audio = new Dictionary<Guid, string>();

            foreach (var item in Plugin.Items)
            {
                var value = item.Value;

                if (!string.IsNullOrEmpty(value.Video.Extension))
                    video.Add(item.Key, value.Name);

                if (!string.IsNullOrEmpty(value.Audio.Extension))
                    audio.Add(item.Key, value.Name);
            }

            cboVideoEncoder.DataSource = new BindingSource(video, null);
            cboVideoEncoder.DisplayMember = "Value";
            cboVideoEncoder.ValueMember = "Key";
            cboVideoEncoder.SelectedValue = new Guid("deadbeef-0265-0265-0265-026502650265");

            cboAudioEncoder.DataSource = new BindingSource(audio, null);
            cboAudioEncoder.DisplayMember = "Value";
            cboAudioEncoder.ValueMember = "Key";
            cboAudioEncoder.SelectedValue = new Guid("deadbeef-faac-faac-faac-faacfaacfaac");
        }

        private void ApplyEncodingPreset()
        {
            var encpre = new Dictionary<string, string>();

            foreach (var item in MediaPreset.List)
                encpre.Add(item.Key, item.Value.Name);

            cboEncodingPreset.DataSource = new BindingSource(encpre, null);
            cboEncodingPreset.DisplayMember = "Value";
            cboEncodingPreset.ValueMember = "Key";
        }

        private void CheckVersion()
        {
            if (!string.Equals(Application.ProductVersion, new Download().GetString("https://raw.githubusercontent.com/Anime4000/IFME/master/version.txt")))
            {
                var frm = new frmCheckUpdate();
                Invoke((MethodInvoker)delegate ()
                {
                    frm.Show();
                });
            }
        }

        private void DrawBanner()
        {
            pbxBanner.BackgroundImage = Branding.Banner(pbxBanner.Width, pbxBanner.Height);
        }

        private void EncodingPreset(string id, string name)
		{
			var preset = new MediaPreset();
            var target = 0;

            if (rdoFormatMp4.Checked)
                target = (int)TargetFormat.MP4;
            else if (rdoFormatMkv.Checked)
                target = (int)TargetFormat.MKV;
            else if (rdoFormatWebm.Checked)
                target = (int)TargetFormat.WEBM;
            else if (rdoFormatAudioMp3.Checked)
                target = (int)TargetFormat.MP3;
            else if (rdoFormatAudioMp4.Checked)
                target = (int)TargetFormat.M4A;
            else if (rdoFormatAudioOgg.Checked)
                target = (int)TargetFormat.OGG;
            else if (rdoFormatAudioOpus.Checked)
                target = (int)TargetFormat.OPUS;
            else if (rdoFormatAudioFlac.Checked)
                target = (int)TargetFormat.FLAC;

            preset.OutputFormat = target;

            preset.Video.Encoder = new Guid($"{cboVideoEncoder.SelectedValue}");
            preset.Video.EncoderPreset = cboVideoPreset.Text;
            preset.Video.EncoderTune = cboVideoTune.Text;
            preset.Video.EncoderMode = cboVideoRateControl.SelectedIndex;
            preset.Video.EncoderValue = nudVideoRateFactor.Value;
            preset.Video.EncoderMultiPass = (int)nudVideoMultiPass.Value;

            if (lstMedia.SelectedItems.Count > 0)
                if (lstVideo.Items.Count > 0)
                    preset.Video.EncoderCommand = (lstMedia.SelectedItems[0].Tag as MediaQueue).Video[0].EncoderCommand;

            var width = 1280;
            var height = 720;
            var fps = 23.976;
            var bpc = 8;
            var pix = 420;
            int.TryParse(cboVideoResolution.Text.Split('x')[0], out width);
            int.TryParse(cboVideoResolution.Text.Split('x')[1], out height);
            double.TryParse(cboVideoFrameRate.Text, out fps);
            int.TryParse(cboVideoBitDepth.Text, out bpc);
            int.TryParse(cboVideoPixelFormat.Text, out pix);

            preset.Video.Width = width;
            preset.Video.Height = height;
            preset.Video.FrameRate = fps;
            preset.Video.BitDepth = bpc;
            preset.Video.PixelFormat = pix;

            preset.Video.DeInterlace = chkVideoDeinterlace.Checked;
            preset.Video.DeInterlaceMode = cboVideoDeinterlaceMode.SelectedIndex;
            preset.Video.DeInterlaceField = cboVideoDeinterlaceField.SelectedIndex;

            decimal quality = 128000;
            var samplerate = 44100;
            var channel = 0;
            decimal.TryParse(cboAudioQuality.Text, out quality);
            int.TryParse(cboAudioSampleRate.Text, out samplerate);
            int.TryParse(cboAudioChannel.Text, out channel);

            preset.Audio.Encoder = new Guid($"{cboAudioEncoder.SelectedValue}");
            preset.Audio.EncoderMode = cboAudioMode.SelectedIndex;
            preset.Audio.EncoderQuality = quality;
            preset.Audio.EncoderSampleRate = samplerate;
            preset.Audio.EncoderChannel = channel;

            if (lstMedia.SelectedItems.Count > 0)
                if (lstAudio.Items.Count > 0)
                        preset.Audio.EncoderCommand = (lstMedia.SelectedItems[0].Tag as MediaQueue).Audio[0].EncoderCommand;

            if (MediaPreset.List.ContainsKey(id))
            {
                preset.Name = MediaPreset.List[id].Name;
                preset.Author = MediaPreset.List[id].Author;

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(preset, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine("preset", $"{id}.json"), json);
            }
            else
            {
                preset.Name = name;
                preset.Author = Environment.MachineName;

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(preset, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(Path.Combine("preset", $"{DateTime.Now:yyyyMMdd_HHmmss-ffff}_{id.Substring(0, 4)}.json"), json);
            }

            // reload listing
            ApplyEncodingPreset();
        }

		private string[] OpenFiles(MediaType type)
		{
			var ofd = new OpenFileDialog();

			var exts = string.Empty;
			var extsVideo = "All video types|*.mkv;*.mp4;*.m4v;*.mts;*.m2ts;*.flv;*.webm;*.ogv;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.avs|";
			var extsAudio = "All audio types|*.mp2;*.mp3;*.mp4;*.m4a;*.aac;*.ogg;*.opus;*.flac;*.wav|";
			var extsSub = "All subtitle types|*.ssa;*.ass;*.srt|";
			var extsAtt = "All font types|*.ttf;*.otf;*.woff;*.woff2;*.eot|";

			switch (type)
			{
				case MediaType.Video:
					exts = extsVideo;
					break;
				case MediaType.Audio:
					exts = extsAudio;
					break;
				case MediaType.Subtitle:
					exts = extsSub;
					break;
				case MediaType.Attachment:
					exts = extsAtt;
					break;
				default:
					exts = extsVideo + extsAudio;
					break;
			}

			exts += "All types|*.*";

			ofd.Filter = exts;
			ofd.FilterIndex = 1;
			ofd.Multiselect = true;

			if (ofd.ShowDialog() == DialogResult.OK)
				return ofd.FileNames;

			return new string[0];
		}

		private void ListViewItemMove(ListViewItemType type, Direction direction)
		{
			try
			{
				if (type == ListViewItemType.Media)
				{
					if (lstMedia.SelectedItems.Count > 0)
					{
						ListViewItem selected = lstMedia.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstMedia.Items.Count;

						if (direction == Direction.Up)
						{
							if (indx == 0)
							{
								lstMedia.Items.Remove(selected);
								lstMedia.Items.Insert(totl - 1, selected);
							}
							else
							{
								lstMedia.Items.Remove(selected);
								lstMedia.Items.Insert(indx - 1, selected);
							}
						}
						else
						{
							if (indx == totl - 1)
							{
								lstMedia.Items.Remove(selected);
								lstMedia.Items.Insert(0, selected);
							}
							else
							{
								lstMedia.Items.Remove(selected);
								lstMedia.Items.Insert(indx + 1, selected);
							}
						}
					}
				}
				else if (type == ListViewItemType.Video)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstVideo.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Video;
						for (int i = 0; i < data.Count; i++)
							lstVideo.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstVideo.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstVideo.Items.Count;

						if (direction == Direction.Up)
						{
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
						}
						else
						{
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
						}

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstVideo.Items[i].Tag as MediaQueueVideo;

						// refresh UI
						UXReloadVideo();
					}
				}
				else if (type == ListViewItemType.Audio)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstAudio.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Audio;
						for (int i = 0; i < data.Count; i++)
							lstAudio.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstAudio.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstAudio.Items.Count;

						if (direction == Direction.Up)
						{
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
						}
						else
						{
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
						}

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstAudio.Items[i].Tag as MediaQueueAudio;

						// refresh UI
						UXReloadAudio();
					}
				}
				else if (type == ListViewItemType.Subtitle)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstSub.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Subtitle;
						for (int i = 0; i < data.Count; i++)
							lstSub.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstSub.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstSub.Items.Count;

						if (direction == Direction.Up)
						{
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
						}
						else
						{
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
						}

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstSub.Items[i].Tag as MediaQueueSubtitle;

						// refresh
						UXReloadSubtitle();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void MediaSelect()
		{
			if (lstMedia.Items.Count > 0)
			{
				var index = lstMedia.Items.Count - 1;
				lstMedia.SelectedIndices.Clear();
				lstMedia.Items[index].Selected = true;
			}
		}

		private void UXReloadMedia()
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
                var selected = lstMedia.SelectedItems;

                foreach (ListViewItem item in selected)
                {
                    lstMedia.Items[item.Index].Selected = false;
                    lstMedia.Items[item.Index].Selected = true;
                }
			}
		}

		private void UXReloadVideo()
		{
			Thread.Sleep(1);
			var id = lstVideo.SelectedItems[0].Index;
			lstVideo.SelectedItems[0].Selected = false;
			lstVideo.Items[id].Selected = true;
		}

		private void UXReloadAudio()
		{
			Thread.Sleep(1);
			var id = lstAudio.SelectedItems[0].Index;
			lstAudio.SelectedItems[0].Selected = false;
			lstAudio.Items[id].Selected = true;

		}

		private void UXReloadSubtitle()
		{
			Thread.Sleep(1);
			var id = lstSub.SelectedItems[0].Index;
			lstSub.SelectedItems[0].Selected = false;
			lstSub.Items[id].Selected = true;
		}

		private void MediaAdd(string file)
		{
			var queue = new MediaQueue();
			var media = new FFmpegDotNet.FFmpeg.Stream(file);

			if (media.Video.Count == 0 && media.Audio.Count == 0)
				return;

			queue.Enable = true;
			queue.File = file;
			queue.OutputFormat = TargetFormat.MKV;

            // if input only have audio
            if (media.Video.Count == 0)
                queue.OutputFormat = TargetFormat.OGG;

            queue.MediaInfo = media;

			var vdef = new MediaDefaultVideo(MediaTypeVideo.MKV);
			var adef = new MediaDefaultAudio(MediaTypeAudio.MP4);

			foreach (var item in media.Video)
			{
				queue.Video.Add(new MediaQueueVideo
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Duration = item.Duration,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = vdef.Encoder,
					EncoderPreset = vdef.Preset,
					EncoderTune = vdef.Tune,
					EncoderMode = vdef.Mode,
					EncoderValue = vdef.Value,
					EncoderMultiPass = vdef.Pass,
					EncoderCommand = vdef.Command,

					Width = item.Width,
					Height = item.Height,
					FrameRate = (float)Math.Round(item.FrameRateAvg, 3),
					FrameRateAvg = item.FrameRateAvg,
					FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRate),
					IsVFR = !item.FrameRateConstant,
					BitDepth = item.BitDepth,
					PixelFormat = item.Chroma,

					DeInterlace = false,
					DeInterlaceMode = 1,
					DeInterlaceField = 0
				});
			}

			foreach (var item in media.Audio)
			{
				queue.Audio.Add(new MediaQueueAudio
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = adef.Encoder,
					EncoderMode = adef.Mode,
					EncoderQuality = adef.Quality,
					EncoderSampleRate = adef.SampleRate,
					EncoderChannel = adef.Channel,
					EncoderCommand = adef.Command
				});
			}

			foreach (var item in media.Subtitle)
			{
				queue.Subtitle.Add(new MediaQueueSubtitle
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),
				});
			}

			var lst = new ListViewItem(new[]
			{
					Path.GetFileName(file),
					TimeSpan.FromSeconds(media.Duration).ToString("hh\\:mm\\:ss"),
					Path.GetExtension(file).Substring(1).ToUpperInvariant(),
					"MKV",
					"Ready",
			});

			lst.Tag = queue;
			lst.Checked = true;

			lstMedia.Items.Add(lst);
		}

		private void VideoAdd(string file)
		{
			var vdef = new MediaDefaultVideo(MediaTypeVideo.MP4);

			if (rdoFormatWebm.Checked)
				vdef = new MediaDefaultVideo(MediaTypeVideo.WEBM);

			var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

			foreach (var item in new FFmpegDotNet.FFmpeg.Stream(file).Video)
			{
				media.Video.Add(new MediaQueueVideo
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Duration = item.Duration,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = vdef.Encoder,
					EncoderPreset = vdef.Preset,
					EncoderTune = vdef.Tune,
					EncoderMode = vdef.Mode,
					EncoderValue = vdef.Value,
					EncoderMultiPass = vdef.Pass,
					EncoderCommand = vdef.Command,

					Width = item.Width,
					Height = item.Height,
					FrameRate = (float)Math.Round(item.FrameRate, 3),
					FrameRateAvg = item.FrameRateAvg,
					FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRate),
					IsVFR = !item.FrameRateConstant,
					BitDepth = item.BitDepth,
					PixelFormat = item.Chroma,

					DeInterlace = false,
					DeInterlaceMode = 1,
					DeInterlaceField = 0

				});
			}
		}

		private void AudioAdd(string file)
		{
			var adef = new MediaDefaultAudio(MediaTypeAudio.MP4);

			if (rdoFormatAudioMp3.Checked)
				adef = new MediaDefaultAudio(MediaTypeAudio.MP3);
			else if (rdoFormatAudioOgg.Checked || rdoFormatWebm.Checked)
				adef = new MediaDefaultAudio(MediaTypeAudio.OGG);
			else if (rdoFormatAudioOpus.Checked)
				adef = new MediaDefaultAudio(MediaTypeAudio.OPUS);
			else if (rdoFormatAudioFlac.Checked)
				adef = new MediaDefaultAudio(MediaTypeAudio.FLAC);

			var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

			foreach (var item in new FFmpegDotNet.FFmpeg.Stream(file).Audio)
			{
				media.Audio.Add(new MediaQueueAudio
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = adef.Encoder,
					EncoderMode = adef.Mode,
					EncoderQuality = adef.Quality,
					EncoderSampleRate = adef.SampleRate,
					EncoderChannel = adef.Channel,
					EncoderCommand = adef.Command
				});
			}
		}

		private void SubtitleAdd(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;

				queue.Subtitle.Add(new MediaQueueSubtitle
				{
					Enable = true,
					File = file,
					Id = -1,
					Lang = "und",
					Format = Path.GetExtension(file).Remove(1)
				});
			}
		}

		private void AttachmentAdd(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;
				var mime = "application/octet-stream";

				Get.MimeType.TryGetValue(Path.GetExtension(file), out mime);

				queue.Attachment.Add(new MediaQueueAttachment
				{
					Enable = true,
					File = file,
					Mime = mime
				});
			}
		}

		private void MediaFormatDefault(object sender, EventArgs e)
		{
			var vdef = new MediaDefaultVideo(MediaTypeVideo.MP4);
			var adef = new MediaDefaultAudio(MediaTypeAudio.MP4);

			if (rdoFormatWebm.Checked)
			{
				vdef = new MediaDefaultVideo(MediaTypeVideo.WEBM);
				adef = new MediaDefaultAudio(MediaTypeAudio.OGG);

				pnlVideo.Enabled = true;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else if (rdoFormatAudioMp3.Checked)
			{
				adef = new MediaDefaultAudio(MediaTypeAudio.MP3);

				pnlVideo.Enabled = false;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else if (rdoFormatAudioMp4.Checked)
			{
				adef = new MediaDefaultAudio(MediaTypeAudio.MP4);

				pnlVideo.Enabled = false;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else if (rdoFormatAudioOgg.Checked)
			{
				adef = new MediaDefaultAudio(MediaTypeAudio.OGG);

				pnlVideo.Enabled = false;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else if (rdoFormatAudioOpus.Checked)
			{
				adef = new MediaDefaultAudio(MediaTypeAudio.OPUS);

				pnlVideo.Enabled = false;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else if (rdoFormatAudioFlac.Checked)
			{
				adef = new MediaDefaultAudio(MediaTypeAudio.FLAC);

				pnlVideo.Enabled = false;
				pnlSubtitle.Enabled = false;
				pnlAttachment.Enabled = false;
			}
			else
			{
				if (rdoFormatMp4.Checked)
				{
					pnlVideo.Enabled = true;
					pnlSubtitle.Enabled = false;
					pnlAttachment.Enabled = false;
				}
				else
				{
					pnlVideo.Enabled = true;
					pnlSubtitle.Enabled = true;
					pnlAttachment.Enabled = true;
				}
			}

			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var mf = q.Tag as MediaQueue;

				foreach (var v in mf.Video)
				{
					v.Encoder = vdef.Encoder;
					v.EncoderPreset = vdef.Preset;
					v.EncoderTune = vdef.Tune;
					v.EncoderMode = vdef.Mode;
					v.EncoderValue = vdef.Value;
					v.EncoderMultiPass = vdef.Pass;
					v.EncoderCommand = vdef.Command;
				}

				foreach (var a in mf.Audio)
				{
					a.Encoder = adef.Encoder;
					a.EncoderMode = adef.Mode;
					a.EncoderQuality = adef.Quality;
					a.EncoderSampleRate = adef.SampleRate;
					a.EncoderChannel = adef.Channel;
				}
			}
		}

		// Minimise code, all controls subscribe one function :)
		private void MediaApply(object sender, EventArgs e)
		{
			var ctrl = (sender as Control).Name;

			// input validation
			if (string.Equals(ctrl, cboVideoResolution.Name))
			{
				Regex regex = new Regex(@"(^\d{1,5}x\d{1,5}$)|^auto$");
				MatchCollection matches = regex.Matches(cboVideoResolution.Text);

				if (matches.Count == 0)
				{
					cboVideoResolution.Text = "1280x720";
				}
			}

			if (string.Equals(ctrl, cboVideoFrameRate.Name))
			{
				Regex regex = new Regex(@"(^\d+$)|(^\d+.\d+$)|(^auto$)");
				MatchCollection matches = regex.Matches(cboVideoFrameRate.Text);

				if (matches.Count == 0)
				{
					cboVideoFrameRate.Text = "24";
				}
			}

			// data update
			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var m = q.Tag as MediaQueue;

				if (rdoFormatMp4.Checked)
					m.OutputFormat = TargetFormat.MP4;
				else if (rdoFormatMkv.Checked)
					m.OutputFormat = TargetFormat.MKV;
				else if (rdoFormatWebm.Checked)
					m.OutputFormat = TargetFormat.WEBM;
				else if (rdoFormatAudioMp3.Checked)
					m.OutputFormat = TargetFormat.MP3;
				else if (rdoFormatAudioMp4.Checked)
					m.OutputFormat = TargetFormat.M4A;
				else if (rdoFormatAudioOgg.Checked)
					m.OutputFormat = TargetFormat.OGG;
				else if (rdoFormatAudioOpus.Checked)
					m.OutputFormat = TargetFormat.OPUS;
				else if (rdoFormatAudioFlac.Checked)
					m.OutputFormat = TargetFormat.FLAC;

				if (lstMedia.SelectedItems.Count > 1)
				{
					for (int i = 0; i < m.Video.Count; i++)
					{
						var temp = m.Video[i];
						MediaApplyVideo(ctrl, ref temp);
					}

					for (int i = 0; i < m.Audio.Count; i++)
					{
						var temp = m.Audio[i];
						MediaApplyAudio(ctrl, ref temp);
					}

					for (int i = 0; i < m.Subtitle.Count; i++)
					{
						var temp = m.Subtitle[i];
						MediaApplySubtitle(ctrl, ref temp);
					}

					for (int i = 0; i < m.Attachment.Count; i++)
					{
						var temp = m.Attachment[i];
						MediaApplyAttachment(ctrl, ref temp);
					}
				}
				else
				{
					foreach (ListViewItem i in lstVideo.SelectedItems)
					{
						var temp = m.Video[i.Index];
						MediaApplyVideo(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstAudio.SelectedItems)
					{
						var temp = m.Audio[i.Index];
						MediaApplyAudio(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstSub.SelectedItems)
					{
						var temp = m.Subtitle[i.Index];
						MediaApplySubtitle(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstAttach.SelectedItems)
					{
						var temp = m.Attachment[i.Index];
						MediaApplyAttachment(ctrl, ref temp);
					}
				}
			}

            // refresh
            UXReloadMedia();
        }

		private void MediaApplyVideo(string ctrl, ref MediaQueueVideo video)
		{
			if (string.Equals(ctrl, cboVideoEncoder.Name))
			{
				video.Encoder = new Guid($"{cboVideoEncoder.SelectedValue}");
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoPreset.Name))
			{
				video.EncoderPreset = cboVideoPreset.Text;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoTune.Name))
			{
				video.EncoderTune = cboVideoTune.Text;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoRateControl.Name))
			{
				video.EncoderMode = cboVideoRateControl.SelectedIndex;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, nudVideoRateFactor.Name))
			{
				video.EncoderValue = nudVideoRateFactor.Value;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, nudVideoMultiPass.Name))
			{
				video.EncoderMultiPass = Convert.ToInt32(nudVideoMultiPass.Value);
			}

			// Video pixel
			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoResolution.Name))
			{
				var w = 0;
				var h = 0;
				var x = cboVideoResolution.Text;
				if (x.Contains('x'))
				{
					int.TryParse(x.Split('x')[0], out w);
					int.TryParse(x.Split('x')[1], out h);
				}
				video.Width = w;
				video.Height = h;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoFrameRate.Name))
			{
				float f = 0;
				float.TryParse(cboVideoFrameRate.Text, out f);
				video.FrameRate = f;
				video.FrameCount = (int)Math.Ceiling(video.Duration * f);
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoBitDepth.Name))
			{
				var b = 8;
				int.TryParse(cboVideoBitDepth.Text, out b);
				video.BitDepth = b;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoPixelFormat.Name))
			{
				var y = 420;
				int.TryParse(cboVideoPixelFormat.Text, out y);
				video.PixelFormat = y;
			}

			// Video Interlace
			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, chkVideoDeinterlace.Name))
			{
				video.DeInterlace = chkVideoDeinterlace.Checked;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoDeinterlaceMode.Name))
			{
				video.DeInterlaceMode = cboVideoDeinterlaceMode.SelectedIndex;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoDeinterlaceField.Name))
			{
				video.DeInterlaceField = cboVideoDeinterlaceField.SelectedIndex;
			}
		}

		private void MediaApplyAudio(string ctrl, ref MediaQueueAudio audio)
		{
			if (string.Equals(ctrl, cboAudioEncoder.Name))
			{
				audio.Encoder = new Guid($"{cboAudioEncoder.SelectedValue}");
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioMode.Name))
			{
				audio.EncoderMode = cboAudioMode.SelectedIndex;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioQuality.Name))
			{
				decimal q = 0;
				decimal.TryParse(cboAudioQuality.Text, out q);
				audio.EncoderQuality = q;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioSampleRate.Name))
			{
				var hz = 0;
				int.TryParse(cboAudioSampleRate.Text, out hz);
				audio.EncoderSampleRate = hz;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioChannel.Name))
			{
				double ch = 0;
				double.TryParse(cboAudioChannel.Text, out ch);
				audio.EncoderChannel = (int)Math.Ceiling(ch); // when value 5.1 become 6, 7.1 become 8
			}
		}

		private void MediaApplySubtitle(string ctrl, ref MediaQueueSubtitle subtitle)
		{
			if (string.Equals(ctrl, cboSubLang.Name))
			{
				subtitle.Lang = $"{cboSubLang.SelectedValue}";
			}
		}

		private void MediaApplyAttachment(string ctrl, ref MediaQueueAttachment attachment)
		{
			if (string.Equals(ctrl, cboAttachMime.Name))
			{
				attachment.Mime = cboAttachMime.Text;
			}
		}

		private void MediaPopulate(MediaQueue media)
		{
			// Media Info
			try
			{
				var mf = media.MediaInfo;
				var md = string.Empty;
				var du = TimeSpan.FromSeconds(mf.Duration);

				md =
					$"File path          : {mf.FilePath}\r\n" +
					$"File size          : {Get.FileSizeDEC((long)mf.FileSize)} (base 10), {Get.FileSizeIEC((long)mf.FileSize)} (base 2)\r\n" +
					$"Bitrate            : {mf.BitRate}\r\n" +
					$"Duration           : {du.Hours:D2}:{du.Minutes:D2}:{du.Seconds:D2} (estimated)\r\n" +
					$"Format             : {mf.FormatName} ({mf.FormatNameFull})\r\n";

				if (mf.Video.Count > 0)
				{
					md += "\r\nVideo\r\n";
					foreach (var item in mf.Video)
					{
						md +=
							$"ID                 : {item.Id:00}\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Width              : {item.Width}\r\n" +
							$"Height             : {item.Height}\r\n" +
							$"Frame rate         : {item.FrameRate:00.000}fps\r\n" +
							$"Frame rate (avg)   : {item.FrameRateAvg:00.000}fps\r\n" +
							$"Bit Depth          : {item.BitDepth}bit per channel\r\n" +
							$"Chroma             : {item.Chroma}\r\n";
					}
				}

				if (mf.Audio.Count > 0)
				{
					md += "\r\nAudio\r\n";
					foreach (var item in mf.Audio)
					{
						md +=
							$"ID                 : {item.Id:00}\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Sample rate        : {item.SampleRate}Hz\r\n" +
							$"Channel            : {item.Channel}Ch\r\n";
					}
				}

				if (mf.Subtitle.Count > 0)
				{
					md += "\r\nSubtitle\r\n";
					foreach (var item in mf.Subtitle)
					{
						md +=
							$"ID                 : {item.Id:00}\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Language           : {item.Language}\r\n";
					}
				}

				txtMediaInfo.Text = md;
			}
			catch
			{
				txtMediaInfo.Text = "New file, no main data to display";
			}

			// Format choice
			var format = media.OutputFormat;

			switch (format)
			{
				case TargetFormat.MP4:
					rdoFormatMp4.Checked = true;
					break;
				case TargetFormat.MKV:
					rdoFormatMkv.Checked = true;
					break;
				case TargetFormat.WEBM:
					rdoFormatWebm.Checked = true;
					break;
				case TargetFormat.MP3:
					rdoFormatAudioMp3.Checked = true;
					break;
				case TargetFormat.M4A:
					rdoFormatAudioMp4.Checked = true;
					break;
				case TargetFormat.OGG:
					rdoFormatAudioOgg.Checked = true;
					break;
				case TargetFormat.OPUS:
					rdoFormatAudioOpus.Checked = true;
					break;
				case TargetFormat.FLAC:
					rdoFormatAudioFlac.Checked = true;
					break;
				default:
					rdoFormatMkv.Checked = true;
					break;
			}

			// Video
			lstVideo.Items.Clear();
			if (media.Video.Count > 0)
			{
				foreach (var item in media.Video)
				{
					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						$"{item.Width}x{item.Height}",
						$"{item.BitDepth} bpc",
						$"{item.FrameRate} fps"
					});
					lst.Checked = item.Enable;
					lst.Tag = item; // allow lstVideo to arrange item UP or DOWN

					lstVideo.Items.Add(lst);
				}

				lstVideo.Items[0].Selected = true;
			}

			// Audio
			lstAudio.Items.Clear();
			if (media.Audio.Count > 0)
			{
				foreach (var item in media.Audio)
				{
					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						$"{item.EncoderSampleRate}Hz",
						$"{(item.EncoderChannel == 0 ? "auto" : $"{item.EncoderChannel}")}"
					});
					lst.Checked = item.Enable;
					lst.Tag = item; // allow lstAudio to arrange item UP or DOWN

					lstAudio.Items.Add(lst);
				}

				lstAudio.Items[0].Selected = true;
			}

			// Subtitle
			lstSub.Items.Clear();
			if (media.Subtitle.Count > 0)
			{
				foreach (var item in media.Subtitle)
				{
					var langFull = string.Empty;
					Get.LanguageCode.TryGetValue(item.Lang, out langFull);

					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						Path.GetFileName(item.File),
						langFull
					});
					lst.Checked = item.Enable;
					lst.Tag = item; //allow lstSub to arrange item UP or DOWN

					lstSub.Items.Add(lst);
				}

				lstSub.Items[0].Selected = true;
			}

			// Attachment
			if (media.Attachment.Count > 0)
			{
				foreach (var item in media.Attachment)
				{
					lstAttach.Items.Add(new ListViewItem(new[]
					{
						$"{item.File}",
						$"{item.Mime}"
					}));
				}
			}
		}

		private void MediaPopulateVideo(object video)
		{
			// delay
			Thread.Sleep(1);

			// populate
			var v = video as MediaQueueVideo;

			// select encoder and wait ui thread to load
			BeginInvoke((Action)delegate () { cboVideoEncoder.SelectedValue = v.Encoder; });
			Thread.Sleep(1);

			// select mode and wait ui thread to load
			BeginInvoke((Action)delegate () { cboVideoRateControl.SelectedIndex = v.EncoderMode; });
			Thread.Sleep(1);

			// when control is loaded, begin to display
			BeginInvoke((Action)delegate ()
			{
				cboVideoPreset.SelectedItem = v.EncoderPreset;
				cboVideoTune.SelectedItem = v.EncoderTune;

				nudVideoRateFactor.Value = v.EncoderValue;
				nudVideoMultiPass.Value = v.EncoderMultiPass;

				cboVideoResolution.Text = $"{v.Width}x{v.Height}";
				cboVideoFrameRate.Text = $"{Math.Round(v.FrameRate, 3)}";
				cboVideoBitDepth.Text = $"{v.BitDepth}";
				cboVideoPixelFormat.Text = $"{v.PixelFormat}";

				chkVideoDeinterlace.Checked = v.DeInterlace;
				cboVideoDeinterlaceMode.SelectedIndex = v.DeInterlaceMode;
				cboVideoDeinterlaceField.SelectedIndex = v.DeInterlaceField;
			});
		}

		private void MediaPopulateAudio(object audio)
		{
			// delay
			Thread.Sleep(1);

			// populate
			var a = audio as MediaQueueAudio;

			// select encoder and wait ui thread to load
			BeginInvoke((Action)delegate () { cboAudioEncoder.SelectedValue = a.Encoder; });
			Thread.Sleep(1);

			// select mode and wait ui thread to load
			BeginInvoke((Action)delegate () { cboAudioMode.SelectedIndex = a.EncoderMode; });
			Thread.Sleep(1);

			// when ui is loaded, begin to display
			BeginInvoke((Action)delegate ()
			{
				cboAudioQuality.Text = $"{a.EncoderQuality}";
				cboAudioSampleRate.Text = $"{a.EncoderSampleRate}";
				cboAudioChannel.Text = $"{a.EncoderChannel}";
			});
		}

		private void MediaPopulateSubtitle(object subtitle)
		{
		
		}

		private void bgThread_DoWork(object sender, DoWorkEventArgs e)
		{
			var media = e.Argument as Dictionary<int, MediaQueue>;

			foreach (var item in media)
			{
				var id = item.Key;
				var mq = item.Value;

				if (mq.Enable)
				{
					var tt = DateTime.Now;

					lstMedia.Invoke((MethodInvoker)delegate
					{
						lstMedia.Items[id].SubItems[4].Text = "Encoding...";
					});

					new MediaEncoding(mq);

					lstMedia.Invoke((MethodInvoker)delegate
					{
						lstMedia.Items[id].Checked = false;
						lstMedia.Items[id].SubItems[4].Text = $"Done! ({Get.Duration(tt)})";
					});
				}
			}
		}

		private void bgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				ProcessManager.Stop();

				Console.Write("\n\n");
				ConsoleEx.Write(LogLevel.Warning, "ifme", "Encoding cancel by user...");

				foreach (ListViewItem item in lstMedia.Items)
					item.SubItems[4].Text = "Abort by user";
			}

			Console.Write("\n\n");

			if (Properties.Settings.Default.ShutdownType == 1)
			{
				ProcessManager.ComputerReboot();
			}
			else if (Properties.Settings.Default.ShutdownType == 2)
			{
				ProcessManager.ComputerShutdown();
			}

			btnStop.Enabled = false;
			btnPause.Enabled = false;
			btnStart.Enabled = true;
		}
	}
}
