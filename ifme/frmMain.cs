using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

using FFmpegDotNet;

namespace ifme
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); // shirnk exe size
            FormBorderStyle = FormBorderStyle.Sizable; // do accurate container size excluding window bezel
        }

        #region Form Action
        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load settings
            if (string.IsNullOrEmpty(Properties.Settings.Default.TempFolder))
                Properties.Settings.Default.TempFolder = Path.Combine(Path.GetTempPath(), "ifme");

            Properties.Settings.Default.Save();

            // Default
            FFmpeg.Main = Path.Combine(Environment.CurrentDirectory, "plugin", "ffmpeg32", "ffmpeg");
            FFmpeg.Probe = Path.Combine(Environment.CurrentDirectory, "plugin", "ffmpeg32", "ffprobe");
            
            cboVideoResolution.SelectedIndex = 0;
            cboVideoFrameRate.SelectedIndex = 0;
            cboVideoChroma.SelectedIndex = 0;

            cboVideoDiMode.SelectedIndex = 1;
            cboVideoDiField.SelectedIndex = 0;

            // Load all plugins
            Plugin.Initialize();

            // Video
            Dictionary<Guid, string> video = new Dictionary<Guid, string>();
            foreach (var item in Plugin.Video)
                video.Add(item.Key, item.Value.Name);
            
            cboVideoEncoder.DataSource = new BindingSource(video, null);
            cboVideoEncoder.DisplayMember = "Value";
            cboVideoEncoder.ValueMember = "Key";

            // Audio
            Dictionary<Guid, string> audio = new Dictionary<Guid, string>();
            foreach (var item in Plugin.Audio)
                audio.Add(item.Key, item.Value.Name);

            cboAudioEncoder.DataSource = new BindingSource(audio, null);
            cboAudioEncoder.DisplayMember = "Value";
            cboAudioEncoder.ValueMember = "Key";

            // Subtitle Language Code
            Dictionary<string, string> lang = new Dictionary<string, string>();
            foreach (var item in File.ReadAllLines("language.code"))
                lang.Add(item.Substring(0, 3), item);

            cboSubLang.DataSource = new BindingSource(lang, null);
            cboSubLang.DisplayMember = "Value";
            cboSubLang.ValueMember = "Key";
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        #endregion

        #region Queue Button
        private void btnQueueAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog getFiles = new OpenFileDialog();
            getFiles.Filter = "All Files|*.*";
            getFiles.FilterIndex = 1;
            getFiles.Multiselect = true;

            if (getFiles.ShowDialog() == DialogResult.OK)
                foreach (var item in getFiles.FileNames)
                    QueueAdd(item);
        }
        private void btnQueueRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstQueue.SelectedItems)
                item.Remove();
        }

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
                MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnQueueStop_Click(object sender, EventArgs e)
        {

        }

        private void btnQueuePause_Click(object sender, EventArgs e)
        {

        }

        private void btnQueueStart_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Queue List Action
        private void lstQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
                QueueDisplay(lstQueue.SelectedItems[0].Index);
        }

        private void lstQueue_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstQueue_DragEnter(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
                QueueAdd(file);
        }
        #endregion

        #region Encoding Profile & Output Folder
        private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnProfileSave_Click(object sender, EventArgs e)
        {

        }

        private void btnProfileDelete_Click(object sender, EventArgs e)
        {

        }

        private void txtOutputFolder_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Tab Properties
        private void rdoMP4_CheckedChanged(object sender, EventArgs e)
        {
            // no need
        }

        private void rdoMKV_CheckedChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    if (rdoMKV.Checked)
                    {
                        (item.Tag as Queue).MkvOut = true;
                    }
                    else
                    {
                        (item.Tag as Queue).MkvOut = false;
                    }
                }
            }
        }
        #endregion

        #region Tab Video
        private void cboVideoResolution_TextChanged(object sender, EventArgs e)
        {
            // prevent user enter invalid value
            Regex regex = new Regex(@"(^\d{3,5}x\d{3,5}$)");
            MatchCollection matches = regex.Matches(cboVideoResolution.Text);

            if (matches.Count == 0)
            {
                if (lstQueue.SelectedItems.Count > 0)
                {
                    var qi = (lstQueue.SelectedItems[0].Tag as Queue).Properties;

                    if (qi.Video.Count > 0)
                    {
                        cboVideoResolution.Text = $"{qi.Video[0].Width}x{qi.Video[0].Height}";
                    }
                }
            }

            // apply
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        var res = cboVideoResolution.Text.Split('x');

                        if (res.Length > 1)
                        {
                            int.TryParse(res[0], out video.Width);
                            int.TryParse(res[1], out video.Height);
                        }
                    }
                }
            }
        }

        private void cboVideoFrameRate_TextChanged(object sender, EventArgs e)
        {
            // prevent user enter invalid value
            Regex regex = new Regex(@"(^\d+$)|(^\d+.\d+$)");
            MatchCollection matches = regex.Matches(cboVideoFrameRate.Text);

            if (matches.Count == 0)
            {
                if (lstQueue.SelectedItems.Count > 0)
                {
                    var qi = (lstQueue.SelectedItems[0].Tag as Queue).Properties;

                    if (qi.Video.Count > 0)
                    {
                        cboVideoFrameRate.Text = $"{qi.Video[0].FrameRate:N3}";
                    }
                }
            }

            // apply
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        float.TryParse(cboVideoFrameRate.Text, out video.FrameRate);
                    }
                }
            }
        }

        private void cboVideoBitDepth_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        int.TryParse(cboVideoBitDepth.Text, out video.BitDepth);
                    }
                }
            }
        }

        private void cboVideoChroma_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        int.TryParse(cboVideoChroma.Text, out video.Chroma);
                    }
                }
            }
        }

        private void chkVideoDeinterlace_CheckedChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.Deinterlace = chkVideoDeinterlace.Checked;
                    }
                }
            }
        }

        private void cboVideoDiMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.DeinterlaceMode = cboVideoDiMode.SelectedIndex;
                    }
                }
            }
        }

        private void cboVideoDiField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.DeinterlaceField = cboVideoDiField.SelectedIndex;
                    }
                }
            }
        }

        private void cboVideoEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            PluginVideo test;
            var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

            if (Plugin.Video.TryGetValue(key, out test))
            {
                cboVideoBitDepth.Items.Clear();
                cboVideoBitDepth.Items.AddRange(test.App.BitDepth);
                cboVideoBitDepth.SelectedIndex = 0;

                cboVideoPreset.Items.Clear();
                cboVideoPreset.Items.AddRange(test.App.Preset);
                cboVideoPreset.SelectedItem = test.App.PresetDefault;

                cboVideoTune.Items.Clear();
                cboVideoTune.Items.AddRange(test.App.Tune);
                cboVideoTune.SelectedItem = test.App.TuneDefault;

                cboVideoEncodingType.Items.Clear();
                for (int i = 0; i < test.App.Mode; i++)
                    cboVideoEncodingType.Items.Add(test.Mode[i].Name);
                cboVideoEncodingType.SelectedIndex = 0; // look on cboVideoEncodingType_SelectedIndexChanged
            }
        }

        private void cboVideoEncoder_Leave(object sender, EventArgs e)
        {
            var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.Encoder = key;
                    }
                }
            }
        }

        private void cboVideoPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.EncoderPreset = cboVideoPreset.Text;
                    }
                }
            }
        }

        private void cboVideoTune_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.EncoderTune = cboVideoTune.Text;
                    }
                }
            }
        }

        private void cboVideoEncodingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int m = cboVideoEncodingType.SelectedIndex;
            if (m >= 0)
            {
                PluginVideo test;
                var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

                if (Plugin.Video.TryGetValue(key, out test))
                {
                    nudVideoRateFactor.DecimalPlaces = test.Mode[m].DecimalPlaces;
                    nudVideoRateFactor.Increment = test.Mode[m].Step;
                    nudVideoRateFactor.Minimum = test.Mode[m].ValueMin;
                    nudVideoRateFactor.Maximum = test.Mode[m].ValueMax;
                    nudVideoRateFactor.Value = test.Mode[m].ValueDefault;
                    nudVideoMultipass.Enabled = test.Mode[m].IsMultipass;
                    nudVideoMultipass.Value = 1;
                }
            }
        }

        private void cboVideoEncodingType_Leave(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.EncoderRateControl = cboVideoEncodingType.SelectedIndex;
                    }
                }
            }
        }

        private void nudVideoRateFactor_ValueChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.EncoderRateValue = nudVideoRateFactor.Value;
                    }
                }
            }
        }

        private void nudVideoMultipass_ValueChanged(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lstQueue.SelectedItems)
                {
                    var qi = item.Tag as Queue;

                    foreach (var video in qi.Video)
                    {
                        video.EncoderMultiPass = Convert.ToInt32(nudVideoMultipass.Value);                      
                    }
                }
            }
        }

        private void btnVideoArgEdit_Click(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                var q = lstQueue.Items[0].Tag as Queue;

                if (q.Video.Count > 0)
                {
                    var f = new frmInputBox(btnVideoArgEdit.Text.Replace("&", ""), q.Video[0].EncoderArgs);

                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        q.Video[0].EncoderArgs = f.ReturnValue;
                    }
                }
            }
        }
        #endregion

        #region Tab Audio
        private void btnAudioAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnAudioRemove_Click(object sender, EventArgs e)
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
            if (lstQueue.SelectedItems.Count > 0)
            {
                if (lstAudio.SelectedItems.Count > 0)
                {
                    int ai = lstAudio.SelectedItems[0].Index;
                    var ap = (lstQueue.SelectedItems[0].Tag as Queue).Audio[ai];

                    cboAudioEncoder.SelectedValue = ap.Encoder;
                    cboAudioMode.SelectedIndex = ap.EncoderMode;
                    cboAudioQuality.SelectedItem = $"{ap.EncoderValue}";
                    cboAudioFreq.SelectedItem = $"{ap.EncoderSampleRate}";
                    cboAudioChannel.SelectedItem = $"{ap.EncoderChannel}";
                }
            }
        }

        private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            PluginAudio test;
            var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

            if (Plugin.Audio.TryGetValue(key, out test))
            {
                cboAudioMode.Items.Clear();
                for (int i = 0; i < test.App.Mode; i++)
                    cboAudioMode.Items.Add(test.Mode[i].Name);
                cboAudioMode.SelectedIndex = 0;

                cboAudioFreq.Items.Clear();
                cboAudioFreq.Items.AddRange(test.App.SampleRate);
                cboAudioFreq.SelectedItem = $"{test.App.SampleRateDefault}";

                cboAudioChannel.Items.Clear();
                cboAudioChannel.Items.AddRange(test.App.Channel);
                cboAudioChannel.SelectedItem = $"{test.App.ChannelDefault}";
            }
        }

        private void cboAudioEncoder_Leave(object sender, EventArgs e)
        {
            var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
            if (lstQueue.SelectedItems.Count > 0)
            {
                if (lstAudio.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lstAudio.SelectedItems)
                    {
                        (lstQueue.SelectedItems[0].Tag as Queue).Audio[item.Index].Encoder = key;
                    }
                }
            }
        }

        private void cboAudioMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            int m = cboAudioMode.SelectedIndex;
            if (m >= 0)
            {
                PluginAudio test;
                var key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

                if (Plugin.Audio.TryGetValue(key, out test))
                {
                    lblAudioQuality.Text = $"{test.Mode[m].Name}:";

                    cboAudioQuality.Items.Clear();
                    cboAudioQuality.Items.AddRange(test.Mode[m].Quality);
                    cboAudioQuality.SelectedItem = test.Mode[m].QualityDefault;
                }
            }
        }

        private void cboAudioMode_Leave(object sender, EventArgs e)
        {
            var m = cboAudioMode.SelectedIndex;
            if (lstQueue.SelectedItems.Count > 0)
            {
                if (lstAudio.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lstAudio.SelectedItems)
                    {
                        (lstQueue.SelectedItems[0].Tag as Queue).Audio[item.Index].EncoderMode = m;
                    }
                }
            }
        }

        private void cboAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAudioFreq_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboAudioChannel_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAudioEditArg_Click(object sender, EventArgs e)
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                var q = lstQueue.SelectedItems[0].Tag as Queue;

                if (lstAudio.SelectedItems.Count > 0)
                {
                    var i = lstAudio.SelectedItems[0].Index;

                    if (q.Audio.Count > 0)
                    {
                        var f = new frmInputBox(btnAudioEditArg.Text.Replace("&", ""), q.Audio[i].EncoderArgs);

                        if (f.ShowDialog() == DialogResult.OK)
                        {
                            q.Audio[i].EncoderArgs = f.ReturnValue;
                        }
                    }
                }
            }
        }
        #endregion

        #region Tab Subtitle
        private void lstSub_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstSub_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files)
                SubtitleAdd(item);

            QueueRefresh();
        }

        private void btnSubAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog getFiles = new OpenFileDialog();
            getFiles.Filter = "Supported Subtitle|*.ass;*.ssa;*.srt|"
                + "SubStation Alpha|*.ass;*.ssa|"
                + "SubRip|*.srt|"
                + "All Files|*.*";
            getFiles.FilterIndex = 1;
            getFiles.Multiselect = true;

            if (getFiles.ShowDialog() == DialogResult.OK)
                foreach (var item in getFiles.FileNames)
                    SubtitleAdd(item);

            QueueRefresh();
        }

        private void btnSubRemove_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstSub.SelectedItems)
            {
                (lstQueue.SelectedItems[0].Tag as Queue).Subtitle.RemoveAt(item.Index);
                item.Remove();
            }
        }

        private void btnSubMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnSubMoveDown_Click(object sender, EventArgs e)
        {

        }

        private void lstSub_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSub.SelectedItems.Count > 0)
            {
                cboSubLang.SelectedValue = lstSub.SelectedItems[0].SubItems[1].Text;
            }
        }

        private void cboSubLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstSub.SelectedItems)
            {
                // modify queue data
                (lstQueue.SelectedItems[0].Tag as Queue).Subtitle[item.Index].Lang = $"{cboSubLang.SelectedValue}";

                // modify display data
                item.SubItems[1].Text = $"{cboSubLang.SelectedValue}";
            }
        }
        #endregion

        #region Tab Attchment
        private void lstAttach_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstAttach_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var item in files)
                SubtitleAdd(item);

            QueueRefresh();
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
                    AttachmentAdd(item);

            QueueRefresh();
        }

        private void btnAttachDel_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstAttach.SelectedItems)
            {
                (lstQueue.SelectedItems[0].Tag as Queue).Attachment.RemoveAt(item.Index);
                item.Remove();
            }
        }

        private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        #endregion

        #region Background Worker
        private void bwEncoding_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void bwEncoding_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        #endregion

        #region Add Video to Queue
        private void QueueAdd(string filePath)
        {
            var qi = new Queue();
            var mi = new FFmpeg.Stream(filePath);

            qi.Properties = mi;

            if (mi.Video.Count > 0 || mi.Audio.Count > 0)
            {
                qi.Enable = true;
                qi.MkvOut = true;

                foreach (var item in mi.Video)
                {
                    qi.Video.Add(new QueueVideo
                    {
                        File = filePath,
                        Id = item.Id,
                        Lang = item.Language,
                        Format = Get.CodecFormat(item.Codec),
                        Width = item.Width,
                        Height = item.Height,
                        FrameRate = item.FrameRate,
                        BitDepth = item.BitDepth,
                        Chroma = item.Chroma,

                        Deinterlace = false,
                        DeinterlaceField = 0,
                        DeinterlaceMode = 0,

                        Encoder = new Guid("deadbeef-0265-0265-0265-026502650265"),
                        EncoderPreset = "medium",
                        EncoderTune = "psnr",
                        EncoderRateControl = 0,
                        EncoderRateValue = 26,
                        EncoderMultiPass = 1,
                        EncoderArgs = "--pme --pmode",
                    });
                }

                foreach (var item in mi.Audio)
                {
                    qi.Audio.Add(new QueueAudio
                    {
                        File = filePath,
                        Id = item.Id,
                        Lang = item.Language,
                        Format = Get.CodecFormat(item.Codec),

                        BitDepth = item.BitDepth, // use for decoding, hidden from GUI

                        Encoder = new Guid("deadbeef-faac-faac-faac-faacfaacfaac"),
                        EncoderMode = 0,
                        EncoderValue = "128",
                        EncoderSampleRate = item.SampleRate,
                        EncoderChannel = item.Channel,
                        EncoderArgs = "",
                    });
                }

                foreach (var item in mi.Subtitle)
                {
                    qi.Subtitle.Add(new QueueSubtitle
                    {
                        File = filePath,
                        Id = item.Id,
                        Lang = item.Language,
                        Format = Get.CodecFormat(item.Codec),
                    });
                }

                // add to queue
                ListViewItem lst = new ListViewItem(new[] {
                    Path.GetFileName(filePath),
                    TimeSpan.FromSeconds(mi.Duration).ToString("hh\\:mm\\:ss"),
                    "MKV",
                    "Ready",
                });
                lst.Tag = qi;
                lst.Checked = true;

                lstQueue.Items.Add(lst);
            }
        }

        private void QueueDisplay(int index)
        {
            var qi = (Queue)lstQueue.Items[index].Tag;

            // Properties - Source Info
            txtSourceInfo.Text = Queue.Info(qi);

            // Properties - Output
            rdoMKV.Checked = qi.MkvOut;
            rdoMP4.Checked = !qi.MkvOut;

            // Video
            if (qi.Video.Count >= 1)
            {
                var item = qi.Video[0];

                // Quality
                cboVideoResolution.Text = $"{item.Width}x{item.Height}";
                cboVideoFrameRate.Text = $"{item.FrameRate:N3}";
                cboVideoBitDepth.Text = $"{item.BitDepth}";
                cboVideoChroma.Text = $"{item.Chroma}";

                // Deinterlace
                chkVideoDeinterlace.Checked = item.Deinterlace;
                cboVideoDiMode.SelectedIndex = item.DeinterlaceMode;
                cboVideoDiField.SelectedIndex = item.DeinterlaceField;

                // Encoder
                cboVideoEncoder.SelectedValue = item.Encoder; // Guid key
                cboVideoPreset.SelectedItem = item.EncoderPreset;
                cboVideoTune.SelectedItem = item.EncoderTune;
                cboVideoEncodingType.SelectedIndex = item.EncoderRateControl;
                nudVideoRateFactor.Value = item.EncoderRateValue;
                nudVideoMultipass.Value = item.EncoderMultiPass;
            }

            // Audio
            lstAudio.Items.Clear();
            if (qi.Audio.Count >= 1)
            {
                foreach (var item in qi.Audio)
                {
                    lstAudio.Items.Add(new ListViewItem(new[] {
                        $"{item.Id:D2}, {item.Lang} @ {Path.GetFileName(item.File)}",
                    }));
                }
            }

            // Subtitle
            lstSub.Items.Clear();
            if (qi.Subtitle.Count >= 1)
            {
                foreach (var item in qi.Subtitle)
                {
                    lstSub.Items.Add(new ListViewItem(new[] {
                        $"{item.Id}",
                        item.Lang,
                        Path.GetFileName(item.File),
                    }));
                }
            }

            // Attachment
            lstAttach.Items.Clear();
            if (qi.Attachment.Count >= 1)
            {
                foreach (var item in qi.Attachment)
                {
                    lstAttach.Items.Add(new ListViewItem(new[]
                    {
                        Path.GetFileName(item.File),
                        item.Mime
                    }));
                }
            }
        }

        private void QueueRefresh()
        {
            if (lstQueue.SelectedItems.Count > 0)
            {
                QueueDisplay(lstQueue.SelectedItems[0].Index);
            }
        }

        private void QueueUnselect()
        {

        }
        #endregion

        #region Add Subtitle to Queue
        private void SubtitleAdd(string filePath)
        {
            foreach (ListViewItem item in lstQueue.SelectedItems)
            {
                (item.Tag as Queue).Subtitle.Add(new QueueSubtitle() { Id = -1, File = filePath, Lang = "und", Format = Get.FileExtension(filePath) });
            }
        }
        #endregion

        #region Add Attachment to Queue
        private void AttachmentAdd(string filePath)
        {
            foreach (ListViewItem item in lstQueue.SelectedItems)
            {
                (item.Tag as Queue).Attachment.Add(new QueueAttachment() { File = filePath, Mime = Get.AttachmentValid(filePath) });
            }
        }
        #endregion
    }
}
