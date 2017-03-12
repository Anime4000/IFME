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
			InitializeUX();
        }

        private void btnMediaFileNew_Click(object sender, EventArgs e)
        {

        }

        private void btnMediaFileOpen_Click(object sender, EventArgs e)
        {
			foreach (var item in OpenFiles(MediaType.VideoAudio))
				MediaAdd(item);

			MediaSelect();
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
			ListViewItemMove(ListViewItemType.Media, Direction.Up);
		}

        private void btnMediaMoveDown_Click(object sender, EventArgs e)
        {
			ListViewItemMove(ListViewItemType.Media, Direction.Down);
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
				var tf = !(lstMedia.SelectedItems.Count > 1);
				grpVideoStream.Enabled = tf;
				grpAudioStream.Enabled = tf;

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
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Video))
					VideoAdd(item);

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

        }

        private void lstVideo_DragEnter(object sender, DragEventArgs e)
        {

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
            var temp = new Plugin();
            var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

            if (Plugin.Items.TryGetValue(key, out temp))
            {
				if ((rdoFormatMp4.Checked && temp.Format.Contains("mp4")) ||
					(rdoFormatMkv.Checked && temp.Format.Contains("mkv")) ||
					(rdoFormatWebm.Checked && temp.Format.Contains("webm")))
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

					LastCorrectVideo = cboVideoEncoder.SelectedIndex;
				}
				else
				{
					MessageBox.Show("Output format and codec are not compatible! Choose different one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					cboVideoEncoder.SelectedIndex = LastCorrectVideo;
				}
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
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Audio))
					AudioAdd(item);

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
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Subtitle))
					SubtitleAdd(item);

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
			if (lstMedia.SelectedItems.Count > 0)
				foreach (var item in OpenFiles(MediaType.Attachment))
					AttachmentAdd(item);

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
					(lstMedia.SelectedItems[0].Tag as MediaQueue).Subtitle.RemoveAt(id);
				}
			}
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
	}
}
