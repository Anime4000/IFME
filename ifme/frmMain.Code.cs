using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ifme
{
	partial class frmMain
	{
		enum MediaType
		{
			Video,
			Audio,
			Subtitle,
			Attachment,
			VideoAudio
		}

		public void InitializeUX()
		{
			// Load default
			cboVideoResolution.Text = "1920x1080";
			cboVideoFrameRate.Text = "23.976";
			cboVideoPixelFormat.SelectedIndex = 0;
			cboVideoDeinterlaceMode.SelectedIndex = 1;
			cboVideoDeinterlaceField.SelectedIndex = 0;

			var workdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			FFmpegDotNet.FFmpeg.Main = Path.Combine(workdir, "plugin", "ffmpeg32", "ffmpeg");
			FFmpegDotNet.FFmpeg.Probe = Path.Combine(workdir, "plugin", "ffmpeg32", "ffprobe");

			// Load Language
			cboSubLang.DataSource = new BindingSource(Get.LanguageCode, null);
			cboSubLang.DisplayMember = "Value";
			cboSubLang.ValueMember = "Key";
			cboSubLang.SelectedValue = "und";

			// Load plugin
			new PluginLoad();

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

		private void MediaSelect()
		{
			if (lstMedia.Items.Count > 0)
			{
				var index = lstMedia.Items.Count - 1;
				lstMedia.SelectedIndices.Clear();
				lstMedia.Items[index].Selected = true;
			}
		}

		private void MediaRefresh()
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var index = lstMedia.SelectedItems[0].Index;
				lstMedia.SelectedItems[0].Selected = false;
				lstMedia.Items[index].Selected = true;
			}
		}

		private void MediaAdd(string file)
		{
			var queue = new MediaQueue();
			var media = new FFmpegDotNet.FFmpeg.Stream(file);

			queue.Enable = true;
			queue.OutputFormat = "mkv";

			foreach (var item in media.Video)
			{
				queue.Video.Add(new MediaQueueVideo
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = new Guid("deadbeef-0265-0265-0265-026502650265"),
					EncoderPreset = "medium",
					EncoderTune = "psnr",
					EncoderMode = 0,
					EncoderValue = 26,
					EncoderMultiPass = 2,
					EncoderCommand = "--pme --pmode",

					Width = item.Width,
					Height = item.Height,
					FrameRate = item.FrameRate,
					FrameRateAvg = item.FrameRateAvg,
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

					Encoder = new Guid("deadbeef-faac-faac-faac-faacfaacfaac"),
					EncoderMode = 0,
					EndoderQuality = 128,
					EncoderSampleRate = 44100,
					EncoderChannel = 0,
					EncoderCommand = "-w -s -c 24000"
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
			var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

			foreach (var item in new FFmpegDotNet.FFmpeg.Stream(file).Video)
			{
				media.Video.Add(new MediaQueueVideo
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = item.Language,
					Format = Get.CodecFormat(item.Codec),

					Encoder = new Guid("deadbeef-0265-0265-0265-026502650265"),
					EncoderPreset = "medium",
					EncoderTune = "psnr",
					EncoderMode = 0,
					EncoderValue = 26,
					EncoderMultiPass = 2,
					EncoderCommand = "--pme --pmode",

					Width = item.Width,
					Height = item.Height,
					FrameRate = item.FrameRate,
					FrameRateAvg = item.FrameRateAvg,
					IsVFR = !item.FrameRateConstant,
					BitDepth = item.BitDepth,
					PixelFormat = item.Chroma,

					DeInterlace = false,
					DeInterlaceMode = 1,
					DeInterlaceField = 0

				});
			}
		}

		private void AddAudio(string file)
		{
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

					Encoder = new Guid("deadbeef-eaac-eaac-eaac-eaaceaaceaac"),
					EncoderMode = 0,
					EndoderQuality = 128000,
					EncoderSampleRate = 44100,
					EncoderChannel = 0,
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
					Format = ""
				});
			}
		}

		// Minimise code, all controls subscribe one function :)
		private void MediaApply(object sender, EventArgs e)
		{
			var ctrl = (sender as Control).Name;

			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var m = q.Tag as MediaQueue;

				if (rdoFormatMp4.Checked)
					m.OutputFormat = "mp4";
				else if (rdoFormatMkv.Checked)
					m.OutputFormat = "mkv";
				else if (rdoFormatWebm.Checked)
					m.OutputFormat = "webm";
				else if (rdoFormatAudioMp3.Checked)
					m.OutputFormat = "mp3";
				else if (rdoFormatAudioMp4.Checked)
					m.OutputFormat = "m4a";
				else if (rdoFormatAudioOgg.Checked)
					m.OutputFormat = "ogg";
				else if (rdoFormatAudioOpus.Checked)
					m.OutputFormat = "opus";
				else if (rdoFormatAudioFlac.Checked)
					m.OutputFormat = "flac";

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
				}
			}
		}

		private void MediaApplyVideo(string ctrl, ref MediaQueueVideo video)
		{
			switch (ctrl)
			{
				case "cboVideoEncoder":
					video.Encoder = new Guid($"{cboVideoEncoder.SelectedValue}");
					break;
				case "cboVideoPreset":
					video.EncoderPreset = cboVideoPreset.Text;
					break;
				case "cboVideoTune":
					video.EncoderTune = cboVideoTune.Text;
					break;
				case "cboVideoRateControl":
					video.EncoderMode = cboVideoRateControl.SelectedIndex;
					break;
				case "nudVideoRateFactor":
					video.EncoderValue = nudVideoRateFactor.Value;
					break;
				case "nudVideoMultiPass":
					video.EncoderMultiPass = Convert.ToInt32(nudVideoMultiPass.Value);
					break;

				case "cboVideoResolution":
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
					break;
				case "cboVideoFrameRate":
					float f = 0;
					float.TryParse(cboVideoFrameRate.Text, out f);
					video.FrameRate = f;
					break;
				case "cboVideoBitDepth":
					var b = 8;
					int.TryParse(cboVideoBitDepth.Text, out b);
					video.BitDepth = b;
					break;
				case "cboVideoPixelFormat":
					var y = 420;
					int.TryParse(cboVideoPixelFormat.Text, out y);
					video.PixelFormat = y;
					break;

				case "chkVideoDeinterlace":
					video.DeInterlace = chkVideoDeinterlace.Checked;
					break;
				case "cboVideoDeinterlaceMode":
					video.DeInterlaceMode = cboVideoDeinterlaceMode.SelectedIndex;
					break;
				case "cboVideoDeinterlaceField":
					video.DeInterlaceField = cboVideoDeinterlaceField.SelectedIndex;
					break;

				default:
					break;
			}
		}

		private void MediaApplyAudio(string ctrl, ref MediaQueueAudio audio)
		{
			switch (ctrl)
			{
				case "cboAudioEncoder":
					audio.Encoder = new Guid($"{cboAudioEncoder.SelectedValue}");
					break;
				case "cboAudioMode":
					audio.EncoderMode = cboAudioMode.SelectedIndex;
					break;
				case "cboAudioQuality":
					decimal q = 0;
					decimal.TryParse(cboAudioQuality.Text, out q);
					audio.EndoderQuality = q;
					break;
				case "cboAudioSampleRate":
					var hz = 0;
					int.TryParse(cboAudioSampleRate.Text, out hz);
					audio.EncoderSampleRate = hz;
					break;
				case "cboAudioChannel":
					double ch = 0;
					double.TryParse(cboAudioChannel.Text, out ch);
					audio.EncoderChannel = (int)Math.Ceiling(ch); // when value 5.1 become 6, 7.1 become 8
					break;

				default:
					break;
			}
		}

		private void MediaApplySubtitle(string ctrl, ref MediaQueueSubtitle subtitle)
		{
			switch (ctrl)
			{
				case "cboSubLang":
					subtitle.Lang = $"{cboSubLang.SelectedValue}";
					break;
				default:
					break;
			}
		}

		private void MediaPopulate(MediaQueue media)
		{
			// Format choice
			var format = media.OutputFormat;

			switch (format)
			{
				case "mp4":
					rdoFormatMp4.Checked = true;
					break;
				case "mkv":
					rdoFormatMkv.Checked = true;
					break;
				case "webm":
					rdoFormatWebm.Checked = true;
					break;
				case "mp3":
					rdoFormatAudioMp3.Checked = true;
					break;
				case "m4a":
					rdoFormatAudioMp4.Checked = true;
					break;
				case "ogg":
					rdoFormatAudioOgg.Checked = true;
					break;
				case "opus":
					rdoFormatAudioOpus.Checked = true;
					break;
				case "flac":
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
						$"{item.Id}"
					});
					lst.Checked = item.Enable;

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
					var langFull = "";
					Get.LanguageCode.TryGetValue(item.Lang, out langFull);

					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						item.File,
						langFull
					});
					lst.Checked = item.Enable;

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
						$"Id: {item.File}"
					}));
				}
			}
		}

		private void MediaPopulateVideo(object video)
		{
			// delay
			Thread.Sleep(100);

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
				cboVideoFrameRate.Text = $"{v.FrameRate}";
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
			Thread.Sleep(3);

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
				cboAudioQuality.SelectedItem = $"{a.EndoderQuality}";
				cboAudioSampleRate.SelectedItem = $"{a.EncoderSampleRate}";
				cboAudioChannel.SelectedItem = $"{a.EncoderChannel}";
			});
		}
	}
}
