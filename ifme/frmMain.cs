using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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
            cboAudioEncoder.SelectedValue = new Guid("deadbeef-eaac-eaac-eaac-eaaceaaceaac");
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

        private void rdoFormatMp4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatMkv_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatWebm_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatAudioMp3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatAudioMp4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatAudioOgg_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatAudioOpus_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdoFormatAudioFlac_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnVideoAdd_Click(object sender, EventArgs e)
        {

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

        private void cboVideoPreset_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoTune_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        private void nudVideoRateFactor_ValueChanged(object sender, EventArgs e)
        {

        }

        private void nudVideoMultiPass_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnVideoAdv_Click(object sender, EventArgs e)
        {
            
        }

        private void cboVideoResolution_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoFrameRate_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoBitDepth_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoPixelFormat_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chkVideoDeinterlace_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoDeinterlaceMode_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboVideoDeinterlaceField_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        }

        private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
        {

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

        }
    }
}
