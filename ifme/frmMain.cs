using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using FFmpegDotNet;

namespace ifme
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Sizable;
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load default
            cboVideoResolution.Text = "1920x1080";
            cboVideoFrameRate.Text = "23.976";
            cboVideoPixelFormat.SelectedIndex = 0;
            cboVideoDeinterlaceMode.SelectedIndex = 1;
            cboVideoDeinterlaceField.SelectedIndex = 0;

            var workdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            FFmpeg.Main = Path.Combine(workdir, "plugin", "ffmpeg32", "ffmpeg");
            FFmpeg.Probe = Path.Combine(workdir, "plugin", "ffmpeg32", "ffprobe");

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

        private void btnMediaFileNew_Click(object sender, EventArgs e)
        {

        }

        private void btnMediaFileOpen_Click(object sender, EventArgs e)
        {
            var files = new OpenFileDialog();

            files.Filter = "All Files|*.*";
            files.FilterIndex = 1;
            files.Multiselect = true;

            if (files.ShowDialog() == DialogResult.OK)
                foreach (var item in files.FileNames)
                    MediaAdd(item);
        }

        private void btnMediaFileDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstMedia.SelectedItems)
                item.Remove();
        }

        private void btnOption_Click(object sender, EventArgs e)
        {

        }

        private void btnMediaMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnMediaMoveDown_Click(object sender, EventArgs e)
        {

        }

        private void btnDonePowerOff_Click(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void lstMedia_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMedia.SelectedItems.Count > 0)
            {
                MediaPopulate(lstMedia.SelectedItems[0].Tag as MediaQueue);
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
                MediaAdd(file);
        }

        private void cboEncodingPreset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnEncodingPresetSave_Click(object sender, EventArgs e)
        {

        }

        private void txtFolderOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowseFolderOutput_Click(object sender, EventArgs e)
        {

        }

        private void btnVideoAdd_Click(object sender, EventArgs e)
        {
            var files = new OpenFileDialog();

            files.Filter = "All Files|*.*";
            files.FilterIndex = 1;
            files.Multiselect = true;

            if (files.ShowDialog() == DialogResult.OK)
                foreach (var item in files.FileNames)
                    lstVideo_Add(item);
        }

        private void lstVideo_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void lstVideo_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void lstVideo_Add(string file)
        {
            var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

            foreach (var item in new FFmpeg.Stream(file).Video)
            {
                media.Video.Add(new MediaQueueVideo
                {
                    Enable = true,
                    File = file,
                    Id = item.Id,
                    Lang = item.Language,
                    Format = new Get().CodecFormat(item.Codec),

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

        private void btnVideoDel_Click(object sender, EventArgs e)
        {

        }

        private void btnVideoMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnVideoMoveDown_Click(object sender, EventArgs e)
        {

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
            var temp = new Plugin();
            var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

            if (Plugin.Items.TryGetValue(key, out temp))
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
			//formatting
        }

        private void cboVideoFrameRate_TextChanged(object sender, EventArgs e)
        {
			//formatting
        }

		private void chkVideoDeinterlace_CheckedChanged(object sender, EventArgs e)
		{
			grpVideoInterlace.Enabled = chkVideoDeinterlace.Checked;
		}

		private void btnAudioAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnAudioDel_Click(object sender, EventArgs e)
        {

        }

        private void btnAudioMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnAudioMoveDown_Click(object sender, EventArgs e)
        {

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
            var temp = new Plugin();
            var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

            if (Plugin.Items.TryGetValue(key, out temp))
            {
                var audio = temp.Audio;

                cboAudioMode.Items.Clear();
                foreach (var item in audio.Mode)
                    cboAudioMode.Items.Add(item.Name);
                cboAudioMode.SelectedIndex = 0;

                cboAudioSampleRate.Items.Clear();
                foreach (var item in audio.SampleRate)
                    cboAudioSampleRate.Items.Add(item);
                cboAudioSampleRate.SelectedItem = audio.SampleRateDefault;

                cboAudioChannel.Items.Clear();
                foreach (var item in audio.Channel)
                    cboAudioChannel.Items.Add(item);
                cboAudioChannel.SelectedItem = audio.ChannelDefault;            
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

        private void cboAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAudioSampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAudioChannel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAudioAdv_Click(object sender, EventArgs e)
        {

        }

        private void btnSubAdd_Click(object sender, EventArgs e)
        {
			var files = new OpenFileDialog();

			files.Filter = "Subtitle File|*.ssa;*.ass;*.srt|" +
				"SubStation Alpha|*.ssa;*.ass|" +
				"SubRip|*.srt";
			files.FilterIndex = 1;
			files.Multiselect = true;

			if (files.ShowDialog() == DialogResult.OK)
				foreach (var item in files.FileNames)
					SubtitleAdd(item);
		}

        private void btnSubDel_Click(object sender, EventArgs e)
        {

        }

        private void btnSubMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnSubMoveDown_Click(object sender, EventArgs e)
        {

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

        private void btnAttachAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnAttachDel_Click(object sender, EventArgs e)
        {

        }

        private void btnAttachMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnAttachMoveDown_Click(object sender, EventArgs e)
        {

        }

        private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAttachMime_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MediaAdd(string file)
        {
            var queue = new MediaQueue();
            var media = new FFmpeg.Stream(file);

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
                    Format = new Get().CodecFormat(item.Codec),

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
                    Format = new Get().CodecFormat(item.Codec),

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
                    Format = new Get().CodecFormat(item.Codec),
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
		private void ctrlApplyMedia(object sender, EventArgs e)
		{
			var ctrl = (sender as Control).Name;

			if (lstMedia.SelectedItems.Count > 0)
			{
				var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

				if (rdoFormatMp4.Checked)
					media.OutputFormat = "mp4";
				else if (rdoFormatMkv.Checked)
					media.OutputFormat = "mkv";
				else if (rdoFormatWebm.Checked)
					media.OutputFormat = "webm";
				else if (rdoFormatAudioMp3.Checked)
					media.OutputFormat = "mp3";
				else if (rdoFormatAudioMp4.Checked)
					media.OutputFormat = "m4a";
				else if (rdoFormatAudioOgg.Checked)
					media.OutputFormat = "ogg";
				else if (rdoFormatAudioOpus.Checked)
					media.OutputFormat = "opus";
				else if (rdoFormatAudioFlac.Checked)
					media.OutputFormat = "flac";

				foreach (ListViewItem item in lstVideo.SelectedItems)
				{
					var i = media.Video[item.Index];

					switch (ctrl)
					{
						case "cboVideoEncoder":
							i.Encoder = new Guid($"{cboVideoEncoder.SelectedValue}");
							break;
						case "cboVideoPreset":
							i.EncoderPreset = cboVideoPreset.Text;
							break;
						case "cboVideoTune":
							i.EncoderTune = cboVideoTune.Text;
							break;
						case "cboVideoRateControl":
							i.EncoderMode = cboVideoRateControl.SelectedIndex;
							break;
						case "nudVideoRateFactor":
							i.EncoderValue = nudVideoRateFactor.Value;
							break;
						case "nudVideoMultiPass":
							i.EncoderMultiPass = Convert.ToInt32(nudVideoMultiPass.Value);
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
							i.Width = w;
							i.Height = h;
							break;
						case "cboVideoFrameRate":
							float f = 0;
							float.TryParse(cboVideoFrameRate.Text, out f);
							i.FrameRate = f;
							break;
						case "cboVideoBitDepth":
							var b = 8;
							int.TryParse(cboVideoBitDepth.Text, out b);
							i.BitDepth = b;
							break;
						case "cboVideoPixelFormat":
							var y = 420;
							int.TryParse(cboVideoPixelFormat.Text, out y);
							i.PixelFormat = y;
							break;

						case "chkVideoDeinterlace":
							i.DeInterlace = chkVideoDeinterlace.Checked;
							break;
						case "cboVideoDeinterlaceMode":
							i.DeInterlaceMode = cboVideoDeinterlaceMode.SelectedIndex;
							break;
						case "cboVideoDeinterlaceField":
							i.DeInterlaceField = cboVideoDeinterlaceField.SelectedIndex;
							break;

						default:
							break;
					}
				}

				foreach (ListViewItem item in lstAudio.SelectedItems)
				{
					var i = media.Audio[item.Index];

					switch (ctrl)
					{
						case "cboAudioEncoder":
							i.Encoder = new Guid($"{cboAudioEncoder.SelectedValue}");
							break;
						case "cboAudioMode":
							i.EncoderMode = cboAudioMode.SelectedIndex;
							break;
						case "cboAudioQuality":
							decimal q = 0;
							decimal.TryParse(cboAudioQuality.Text, out q);
							i.EndoderQuality = q;
							break;
						case "cboAudioSampleRate":
							var hz = 0;
							int.TryParse(cboAudioSampleRate.Text, out hz);
							i.EncoderSampleRate = hz;
							break;
						case "cboAudioChannel":
							double ch = 0;
							double.TryParse(cboAudioChannel.Text, out ch);
							i.EncoderChannel = (int)Math.Ceiling(ch); // when value 5.1 become 6, 7.1 become 8
							break;

						default:
							break;
					}
				}

				foreach (ListViewItem item in lstSub.SelectedItems)
				{
					var i = media.Subtitle[item.Index];

					switch (ctrl)
					{
						case "cboSubLang":
							i.Lang = $"{cboSubLang.SelectedValue}";
							break;
						default:
							break;
					}
				}
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
