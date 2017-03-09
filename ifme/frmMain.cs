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
			try
			{
				if (lstMedia.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstMedia.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstMedia.Items.Count;

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
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void btnMediaMoveDown_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstMedia.SelectedItems.Count > 0)
				{
					ListViewItem selected = lstMedia.SelectedItems[0];
					int indx = selected.Index;
					int totl = lstMedia.Items.Count;

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
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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
			foreach (var item in OpenFiles(MediaType.Video))
				VideoAdd(item);

			MediaRefresh();
		}

        private void lstVideo_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void lstVideo_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void btnVideoDel_Click(object sender, EventArgs e)
        {

        }

        private void btnVideoMoveUp_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstVideo.SelectedItems.Count > 0)
				{
					var v = (lstMedia.SelectedItems[0].Tag as MediaQueue).Video;

					for (int i = 0; i < v.Count; i++)
						lstVideo.Items[i].Tag = v[i];

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

					for (int i = 0; i < v.Count; i++)
						v[i] = lstVideo.Items[i].Tag as MediaQueueVideo;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void btnVideoMoveDown_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstVideo.SelectedItems.Count > 0)
				{
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
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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
			foreach (var item in OpenFiles(MediaType.Audio))
				AddAudio(item);
        }

        private void btnAudioDel_Click(object sender, EventArgs e)
        {

        }

        private void btnAudioMoveUp_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstAudio.SelectedItems.Count > 0)
				{
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
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void btnAudioMoveDown_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstAudio.SelectedItems.Count > 0)
				{
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
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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
			foreach (var item in OpenFiles(MediaType.Subtitle))
				SubtitleAdd(item);
		}

        private void btnSubDel_Click(object sender, EventArgs e)
        {

        }

        private void btnSubMoveUp_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstSub.SelectedItems.Count > 0)
				{
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
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void btnSubMoveDown_Click(object sender, EventArgs e)
        {
			try
			{
				if (lstSub.SelectedItems.Count > 0)
				{
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
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
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

        private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAttachMime_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

		private void tabMediaConfig_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void tabGeneral_Click(object sender, EventArgs e)
		{

		}

		private void pnlGeneral_Paint(object sender, PaintEventArgs e)
		{

		}

		private void grpTargetFormat_Enter(object sender, EventArgs e)
		{

		}

		private void tabVideo_Click(object sender, EventArgs e)
		{

		}

		private void pnlVideo_Paint(object sender, PaintEventArgs e)
		{

		}

		private void grpVideoStream_Enter(object sender, EventArgs e)
		{

		}

		private void grpVideoCodec_Enter(object sender, EventArgs e)
		{

		}

		private void nudVideoMultiPass_ValueChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoMultiPass_Click(object sender, EventArgs e)
		{

		}

		private void nudVideoRateFactor_ValueChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoRateFactor_Click(object sender, EventArgs e)
		{

		}

		private void lblVideoRateControl_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoTune_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoTune_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoPreset_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoPreset_Click(object sender, EventArgs e)
		{

		}

		private void lblVideoEncoder_Click(object sender, EventArgs e)
		{

		}

		private void grpVideoInterlace_Enter(object sender, EventArgs e)
		{

		}

		private void cboVideoDeinterlaceField_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoDeinterlaceField_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoDeinterlaceMode_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoDeinterlaceMode_Click(object sender, EventArgs e)
		{

		}

		private void grpVideoPicture_Enter(object sender, EventArgs e)
		{

		}

		private void cboVideoPixelFormat_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblPixelFormat_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoBitDepth_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoBitDepth_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoFrameRate_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoFrameRate_Click(object sender, EventArgs e)
		{

		}

		private void cboVideoResolution_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblVideoResolution_Click(object sender, EventArgs e)
		{

		}

		private void tabAudio_Click(object sender, EventArgs e)
		{

		}

		private void pnlAudio_Paint(object sender, PaintEventArgs e)
		{

		}

		private void grpAudioCodec_Enter(object sender, EventArgs e)
		{

		}

		private void lblAudioMode_Click(object sender, EventArgs e)
		{

		}

		private void lblAudioChannel_Click(object sender, EventArgs e)
		{

		}

		private void lblAudioSampleRate_Click(object sender, EventArgs e)
		{

		}

		private void lblAudioQuality_Click(object sender, EventArgs e)
		{

		}

		private void lblAudioEncoder_Click(object sender, EventArgs e)
		{

		}

		private void grpAudioStream_Enter(object sender, EventArgs e)
		{

		}

		private void tabSubtitle_Click(object sender, EventArgs e)
		{

		}

		private void pnlSubtitle_Paint(object sender, PaintEventArgs e)
		{

		}

		private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void lblSubLang_Click(object sender, EventArgs e)
		{

		}

		private void tabAttachment_Click(object sender, EventArgs e)
		{

		}

		private void pnlAttachment_Paint(object sender, PaintEventArgs e)
		{

		}

		private void lblAttachMime_Click(object sender, EventArgs e)
		{

		}

		private void lblOutputFolder_Click(object sender, EventArgs e)
		{

		}

		private void lblEncodingPreset_Click(object sender, EventArgs e)
		{

		}

		private void lblSplit1_Click(object sender, EventArgs e)
		{

		}

		private void lblSplit2_Click(object sender, EventArgs e)
		{

		}

		private void pnlBanner_Paint(object sender, PaintEventArgs e)
		{

		}

		private void pbxBannerA_Click(object sender, EventArgs e)
		{

		}

		private void pbxBannerB_Click(object sender, EventArgs e)
		{

		}
	}
}
