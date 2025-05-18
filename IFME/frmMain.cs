using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using IFME.OSManager;

namespace IFME
{
    public partial class frmMain : Form
    {
        private BackgroundWorker2 bgThread = new BackgroundWorker2();
        private Array Format = Enum.GetValues(typeof(MediaContainer));

        public frmMain()
        {
            new frmSplashScreen().ShowDialog(); // loading, init all inside that

            frmMainStatic = this;

            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = $"{Version.Title} {Version.Release} ( '{Version.CodeName}' )";
            FormBorderStyle = FormBorderStyle.Sizable;

            bgThread.DoWork += bgThread_DoWork;
            bgThread.ProgressChanged += bgThread_ProgressChanged;
            bgThread.RunWorkerCompleted += bgThread_RunWorkerCompleted;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboVideoRes.SelectedIndex = 9;
            cboVideoFps.SelectedIndex = 5;
            cboVideoPixFmt.SelectedIndex = 0;
            cboVideoDeInterMode.SelectedIndex = 1;
            cboVideoDeInterField.SelectedIndex = 0;

            if (Plugins.Items.Audio.Count > 0)
            {
                cboAudioEncoder.DataSource = new BindingSource(Plugins.Items.Audio.ToDictionary(p => p.Key, p => p.Value.Name), null);
                cboAudioEncoder.DisplayMember = "Value";
                cboAudioEncoder.ValueMember = "Key";
            }
            
            if (Plugins.Items.Video.Count > 0)
            {
                cboVideoEncoder.DataSource = new BindingSource(Plugins.Items.Video.ToDictionary(p => p.Key, p => p.Value.Name), null);
                cboVideoEncoder.DisplayMember = "Value";
                cboVideoEncoder.ValueMember = "Key";
            }

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

            txtOutputPath.Text = Properties.Settings.Default.FolderOutput;

#if SAVE_LANG
            i18n.Save(this, Name);
#else
            Initialize_i18n();
#endif
            InitializeFonts();

            if (Plugins.Items.Audio.Count == 0 || Plugins.Items.Video.Count == 0)
            {
                var hed = i18nUI.Dialog("NoEncoderAvailableTitle");
                var msg = i18nUI.Dialog("NoEncoderAvailableMsg1");

                MessageBox.Show(msg, hed, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            var c = 0;
            foreach (var item in Format)
            {
                cboFormat.Items.Add($"{item}{(c >= (int)MediaContainer.MP2 ? $" ({i18nUI.Status("AudioOnly")})" : "")}");
                c++;
            }
            cboFormat.SelectedIndex = 2;

            InitializeProfiles();
            InitializeLog();
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            if (!CPU.HasAVX)
            {
                MessageBox.Show("AVX instruction set not detected. A modern CPU with AVX support is required to continue. Please ensure your hardware is compatible. The program will now exit.", i18nUI.Status("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            if (!CPU.HasAVX2)
            {
                MessageBox.Show("AVX2 instruction set not detected. A modern CPU with AVX2 support is required to continue. Please ensure your hardware is compatible.", i18nUI.Status("Error"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            InitializeTab(); // need loop all table to make controls respond
        }

        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            var w = PbxBanner.Width;
            var h = PbxBanner.Height;

            var b1 = Image.FromFile(Path.Combine("Resources", "Banner_4a.png"));
            var b2 = Image.FromFile(Path.Combine("Resources", "Banner_2b.png"));
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

                if (lstFile.Width > 1000)
                {
                    var pWidthInc = (((float)lstFile.Width - 1000) / 1000);

                    colFileName.Width = (int)Math.Ceiling((250 * pWidthInc) + 250);
                    colFileType.Width = (int)(90 * pWidthInc) + 90;
                    colFileDuration.Width = (int)(72 * pWidthInc) + 72;
                    colFileSize.Width = (int)(64 * pWidthInc) + 64;
                    colFileStatus.Width = (int)(110 * pWidthInc) + 110;
                    colFileProgress.Width = (int)Math.Ceiling((390 * pWidthInc) + 390);
                }
            }
            catch (Exception)
            {

            }
        }

        private void btnFileAdd_Click(object sender, EventArgs e)
        {
            var btnSender = (Button)sender;
            var ptLowerLeft = new Point(1, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            cmsFileAdd.Show(ptLowerLeft);
        }

        private void btnFileAdd_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                btnFileAdd.PerformClick();
            }
        }

        private void btnFileDelete_Click(object sender, EventArgs e)
        {
            lstFile.SelectedIndexChanged -= lstFile_SelectedIndexChanged;

            foreach (ListViewItem item in lstFile.SelectedItems)
                item.Remove();

            if (lstFile.Items.Count == 0)
                DisplayProperties_Clear();

            lstFile.SelectedIndexChanged += lstFile_SelectedIndexChanged;
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            var currentLang = Properties.Settings.Default.UILanguage;

            new frmOptions().ShowDialog();

            if (currentLang != Properties.Settings.Default.UILanguage)
            {
                Initialize_i18n();
                InitializeFonts();
            }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
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
            ProcessManager.Donate();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (tsmiPowerOff.Checked)
            {
                var msgBox = MessageBox.Show(i18nUI.Dialog("ForcedEncodingShutdown"), i18nUI.Status("Warning"), MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (msgBox == DialogResult.Cancel)
                    return;
            }

            if (cboFormat.SelectedIndex == -1)
            {
                MessageBox.Show(i18nUI.Dialog("OutputFormatNotSet"), i18nUI.Status("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                        if (!item.Checked)
                            continue;

                        data.Add(item.Index, item.Tag as MediaQueue);
                        item.SubItems[4].Text = i18nUI.Status("Waiting");
                        item.SubItems[5].Text = string.Empty;
                    }

                    if (data.Count > 0)
                    {
                        bgThread.RunWorkerAsync(data);
                    }
                    else
                    {
                        frmMain.PrintLog(i18nUI.Log("NotingToEncode"));
                        btnStart.Text = Fonts.fa.play;
                    }
                }
            }
        }

        private void btnStart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var btnSender = (Button)sender;
                var ptLowerLeft = new Point(1, btnSender.Height);
                ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
                cmsPower.Show(ptLowerLeft);
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
            (lstFile.Items[e.Item.Index].Tag as MediaQueue).Enable = e.Item.Checked;
        }

        private void lstFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                var data = lstFile.SelectedItems[0].Tag as MediaQueue;
                var isImgSq = false;

                // Profile
                if (data.ProfileId >= 0)
                {
                    if (Profiles.Items[data.ProfileId].Container == data.OutputFormat)
                        cboProfile.SelectedIndex = data.ProfileId;
                    else
                        cboProfile.SelectedIndex = -1;
                }
                else
                {
                    cboProfile.SelectedIndex = -1;
                }

                // Format
                cboFormat.SelectedIndex = (int)data.OutputFormat;

                // MP4 Remux Test
                chkVideoMP4Compt.Checked = data.FastMuxVideo;
                chkAudioMP4Compt.Checked = data.FastMuxAudio;

                // Sub Burn
                chkSubHard.Checked = data.HardSub;
                chkSubHard.Enabled = data.OutputFormat != MediaContainer.MKV || data.OutputFormat != MediaContainer.MP4;

                // Video
                ListViewItem_RefreshVideo(data);

                // Audio
                ListViewItem_RefreshAudio(data);

                // Subtitle
                ListViewItem_RefreshSubtitle(data);

                // Attachment
                ListViewItem_RefreshAttachment(data);

                if (data.Video.Count > 0)
                    lstVideo.Items[0].Selected = true;
                if (data.Audio.Count > 0)
                    lstAudio.Items[0].Selected = true;
                if (data.Subtitle.Count > 0)
                    lstSub.Items[0].Selected = true;
                if (data.Attachment.Count > 0)
                    lstAttach.Items[0].Selected = true;

                chkAdvTrim.Checked = data.Trim.Enable;
                txtAdvTrimStart.Text = data.Trim.Start;
                txtAdvTrimEnd.Text = data.Trim.End;
                txtAdvTrimDuration.Text = data.Trim.Duration;

                chkAdvCropAuto.Checked = data.Crop.Enable;
                txtAdvCropStart.Text = data.Crop.Start;
                txtAdvCropDuration.Text = data.Crop.Duration;

                // Media Info
                txtMediaInfo.Text = FFmpeg.MediaInfo.Print(data.Info);
            }
            else if (lstFile.SelectedItems.Count == 0 || lstFile.SelectedIndices.Count == 0)
            {
                DisplayProperties_Clear();
            }
        }

        private void lstFile_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var paths = new List<string>();

            foreach (var file in files)
            {
                if (Directory.Exists(file))
                {
                    var fileDir = Directory.GetFiles(file, "*.*", SearchOption.AllDirectories);
                    paths.AddRange(fileDir);
                }
                else
                {
                    paths.Add(file);
                }
            }

            ImportFiles(paths.ToArray());
        }

        private void lstFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A && e.Control)
            {
                lstFile.MultiSelect = true; // force
                foreach (ListViewItem item in lstFile.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void tabConfigVideo_Enter(object sender, EventArgs e)
        {
            grpVideoInterlace.Enabled = chkVideoDeInterlace.Checked;
        }

        private void lstVideo_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstVideo_DragDrop(object sender, DragEventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
                    MediaVideoListAdd(file);

            ListViewItem_RefreshVideo();
        }

        private void btnVideoAdd_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Video))
                    MediaVideoListAdd(item);

            ListViewItem_RefreshVideo();
        }

        private void btnVideoDel_Click(object sender, EventArgs e)
        {
            lstVideo.SelectedIndexChanged -= lstVideo_SelectedIndexChanged;

            foreach (ListViewItem item in lstVideo.SelectedItems)
            {
                (lstFile.SelectedItems[0].Tag as MediaQueue).Video.RemoveAt(item.Index);
                item.Remove();
            }

            lstVideo.SelectedIndexChanged += lstVideo_SelectedIndexChanged;
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
            if (lstFile.Focused || (sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    DisplayProperties_Video();
                }
            }
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

                DisplayProperties_Video();
            }
        }

        private void cboVideoEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = new Guid();

            if (cboVideoEncoder.SelectedItem == null)
                return;

            try
            {
                key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
            }
            catch (Exception ex)
            {
                PrintLog(String.Format(i18nUI.Log("ContainerNotSupportVideo"), ex.Message));
                return;
            }

            if (!cboProfile.Focused || !cboFormat.Focused)
            {
                foreach (Control ctrl in tabConfigVideo.Controls)
                {
                    if (ctrl is GroupBox groupBox)
                    {
                        continue;
                    }

                    ctrl.Enabled = true;
                }
            }

            if (Plugins.Items.Video.TryGetValue(key, out var temp))
            {
                var video = temp.Video;

                cboVideoPreset.Items.Clear();
                cboVideoPreset.Items.AddRange(video.Preset);
                cboVideoPreset.SelectedItem = video.PresetDefault;

                cboVideoTune.Items.Clear();
                cboVideoTune.Items.AddRange(video.Tune);
                cboVideoTune.SelectedItem = video.TuneDefault;

                nudVideoRateFactor.Minimum = video.Mode[0].Value.Min;
                nudVideoRateFactor.Maximum = video.Mode[0].Value.Max;
                nudVideoRateFactor.Value = video.Mode[0].Value.Default;
                nudVideoMultiPass.Value = 2;

                cboVideoRateControl.Items.Clear();
                cboVideoRateControl.Items.AddRange(temp.Video.Mode.Select(x => x.Name).ToArray());
                cboVideoRateControl.SelectedIndex = 0;

                cboVideoBitDepth.Items.Clear();
                cboVideoBitDepth.Items.AddRange(temp.Video.Encoder.Select(x => x.BitDepth.ToString()).ToArray());
                if (cboVideoBitDepth.SelectedIndex == -1)
                    cboVideoBitDepth.SelectedIndex = 0;

                cboVideoPixFmt.Items.Clear();
                cboVideoPixFmt.Items.AddRange(temp.Video.Chroma.Select(x => x.Value.ToString()).ToArray());
                if (cboVideoPixFmt.SelectedIndex == -1)
                    cboVideoPixFmt.SelectedIndex = 0;

                cboVideoPreset.Enabled = cboVideoPreset.Items.Count > 0;
                cboVideoTune.Enabled = cboVideoTune.Items.Count > 0;

                var topBitDepth = Convert.ToInt32(cboVideoBitDepth.Items[cboVideoBitDepth.Items.Count - 1]);
                var topPixelFmt = Convert.ToInt32(cboVideoPixFmt.Items[cboVideoPixFmt.Items.Count - 1]);

                if ((sender as Control).Focused)
                {
                    var enc = new MediaQueueVideoEncoder
                    {
                        Id = new Guid(cboVideoEncoder.SelectedValue.ToString()),
                        Preset = cboVideoPreset.Text,
                        Tune = cboVideoTune.Text,
                        Mode = cboVideoRateControl.SelectedIndex,
                        Value = nudVideoRateFactor.Value,
                        MultiPass = (int)nudVideoMultiPass.Value
                    };

                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        if (lstFile.SelectedItems.Count == 1)
                        {
                            foreach (ListViewItem item in lstVideo.SelectedItems)
                            {
                                var d = (queue.Tag as MediaQueue).Video[item.Index];

                                if (CheckImageSeqEncoder(d, enc.Id))
                                {
                                    MessageBox.Show(i18nUI.Dialog("ImageSeqCannotCopy"), i18nUI.Status("Unsupported"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue;
                                }

                                d.Encoder = enc;

                                d.Quality.BitDepth = d.Quality.BitDepth <= topBitDepth ? d.Quality.BitDepth : topBitDepth;
                                d.Quality.PixelFormat = d.Quality.PixelFormat <= topPixelFmt ? d.Quality.PixelFormat : topPixelFmt;

                                d.Encoder.Command = string.Empty;
                            }

                            break;
                        }
                        else
                        {
                            foreach (var d in (queue.Tag as MediaQueue).Video)
                            {
                                if (CheckImageSeqEncoder(d, enc.Id))
                                {
                                    MessageBox.Show(i18nUI.Dialog("ImageSeqCannotCopy"), i18nUI.Status("Unsupported"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    continue;
                                }

                                d.Encoder = enc;

                                d.Quality.BitDepth = d.Quality.BitDepth <= topBitDepth ? d.Quality.BitDepth : topBitDepth;
                                d.Quality.PixelFormat = d.Quality.PixelFormat <= topPixelFmt ? d.Quality.PixelFormat : topPixelFmt;

                                d.Encoder.Command = string.Empty;
                            }
                        }
                    }

                    DisplayProperties_Video();
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

            if (id < 0)
                return;

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

        private void nudVideoRateFactor_Leave(object sender, EventArgs e)
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

        private void nudVideoMultiPass_Leave(object sender, EventArgs e)
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

        private void btnVideoDec_Click(object sender, EventArgs e)
        {
            var cmd1 = string.Empty;
            var cmd2 = string.Empty;

            if (lstFile.SelectedItems.Count > 0)
            {
                if (lstVideo.SelectedItems.Count > 0)
                {
                    cmd1 = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[0].Quality.Command;
                    cmd2 = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[0].Quality.CommandFilter;
                }
            }

            var ib2 = new InputBox2(i18nUI.Dialog("VideoDecoderTitle"), i18nUI.Dialog("VideoDecoderMsg1"), i18nUI.Dialog("VideoDecoderMsg2"), cmd1, cmd2);
            if (ib2.ShowDialog() == DialogResult.OK)
            {
                cmd1 = ib2.ReturnValue1;
                cmd2 = ib2.ReturnValue2;
            }

            var auto = false;
            if (!string.IsNullOrEmpty(cmd1))
            {                
                auto = cmd1.Contains("crop"); // ToDo: add more function that resolution changed
                if (auto)
                    cboVideoRes.Text = "auto";
            }

            if (lstFile.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in lstVideo.SelectedItems)
                {
                    if (auto)
                    {
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.Width = 0;
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.Height = 0;
                    }

                    (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.Command = cmd1;
                    (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.CommandFilter = cmd2;
                }
            }
            else if (lstFile.SelectedItems.Count > 1)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    foreach (var item in (queue.Tag as MediaQueue).Video)
                    {
                        if (auto)
                        {
                            item.Quality.Width = 0;
                            item.Quality.Height = 0;
                        }

                        item.Quality.Command = cmd1;
                        item.Quality.CommandFilter = cmd2;
                    }
                }
            }

            DisplayProperties_Video();
        }

        private void btnVideoEnc_Click(object sender, EventArgs e)
        {
            var cmd1 = string.Empty;

            if (lstFile.SelectedItems.Count > 0)
            {
                if (lstVideo.SelectedItems.Count > 0)
                {
                    cmd1 = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[0].Encoder.Command;
                }
            }

            var ib = new InputBox(String.Format(i18nUI.Dialog("CodecCommandLine1"), cboVideoEncoder.Text), i18nUI.Dialog("VideoEncoderMsg1"), cmd1);
            if (ib.ShowDialog() == DialogResult.OK)
            {
                cmd1 = ib.ReturnValue;
            }

            if (lstFile.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in lstVideo.SelectedItems)
                {
                    (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Encoder.Command = cmd1;
                }
            }
            else if (lstFile.SelectedItems.Count > 1)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    foreach (var item in (queue.Tag as MediaQueue).Video)
                    {
                        item.Encoder.Command =  cmd1;
                    }
                }
            }
        }

        private void cboVideoRes_TextChanged(object sender, EventArgs e)
        {
            // avoid this event to call itself recursively, add the event again at the end of every return cause
            cboVideoRes.TextChanged -= cboVideoRes_TextChanged;

            if (cboVideoRes.Text.Length > 0)
            {
                if (cboVideoRes.Text[0] == 'a' || cboVideoRes.Text[0] == '0')
                    cboVideoRes.Text = "auto";

                if (cboVideoRes.Text[0] == 'o')
                    cboVideoRes.Text = "original";
            }

            Regex regex = new Regex(@"(^-?\d{1,5}x-?\d{1,5}$)|^auto$|^original$");
            MatchCollection matches = regex.Matches(cboVideoRes.Text);

            if (matches.Count == 0 || cboVideoRes.Text.Count(c => c == '-') > 1)
            {
                cboVideoRes.Text = "auto";
            }
            else if (cboVideoRes.Text.Count(c => c == '-') == 1)
            {
                // replaces every negative number with -1, leaves -2 as it is, since those are the two cases we need
                cboVideoRes.Text = Regex.Replace(cboVideoRes.Text, @"-(\d+)", match => {
                    return (match.Value == "-2") ? match.Value : "-1";
                });
            }

            var w = 0;
            var h = 0;
            var x = cboVideoRes.Text;
            var auto = string.Equals(cboVideoRes.Text, "auto", StringComparison.OrdinalIgnoreCase);
            var original = string.Equals(cboVideoRes.Text, "original", StringComparison.OrdinalIgnoreCase);

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
                        if (original)
                        {
                            w = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Info.Width;
                            h = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Info.Height;
                        }
                        else if (auto)
                        {
                            w = 0;
                            h = 0;
                        }

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
                            if (original)
                            {
                                w = item.Info.Width;
                                h = item.Info.Height;
                            }
                            else if (auto)
                            {
                                w = 0;
                                h = 0;
                            }

                            item.Quality.Width = w;
                            item.Quality.Height = h;
                        }
                    }
                }

                DisplayProperties_Video();
            }

            cboVideoRes.TextChanged += cboVideoRes_TextChanged;
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

            if ((sender as Control).Focused)
            {
                if (matches.Count == 0)
                    cboVideoFps.Text = "0";

                float.TryParse(cboVideoFps.Text, out float fps);

                if (lstFile.SelectedItems.Count == 1)
                {
                    foreach (ListViewItem item in lstVideo.SelectedItems)
                    {
                        if (fps == 0.0)
                            fps = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Info.FrameRate;

                        (lstFile.SelectedItems[0].Tag as MediaQueue).Video[item.Index].Quality.FrameRate = fps;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Video)
                        {
                            if (fps == 0.0)
                                fps = item.Info.FrameRate;

                            item.Quality.FrameRate = fps;
                        }
                    }
                }

                DisplayProperties_Video();
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

                DisplayProperties_Video();
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

                DisplayProperties_Video();
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

            grpVideoInterlace.Enabled = chkVideoDeInterlace.Checked;
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

        private void chkVideoMP4Compt_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count == 1)
                {
                    (lstFile.SelectedItems[0].Tag as MediaQueue).FastMuxVideo = chkVideoMP4Compt.Checked;
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        (queue.Tag as MediaQueue).FastMuxVideo = chkVideoMP4Compt.Checked;
                    }
                }
            }
        }

        private void chkAudioMP4Compt_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count == 1)
                {
                    (lstFile.SelectedItems[0].Tag as MediaQueue).FastMuxAudio = chkAudioMP4Compt.Checked;
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        (queue.Tag as MediaQueue).FastMuxAudio = chkAudioMP4Compt.Checked;
                    }
                }
            }
        }

        private void lstAudio_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstAudio_DragDrop(object sender, DragEventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
                    MediaAudioListAdd(file);

            ListViewItem_RefreshAudio();
        }

        private void btnAudioAdd_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Audio))
                    MediaAudioListAdd(item);

            ListViewItem_RefreshAudio();
        }

        private void btnAudioDel_Click(object sender, EventArgs e)
        {
            lstAudio.SelectedIndexChanged -= lstAudio_SelectedIndexChanged;

            foreach (ListViewItem item in lstAudio.SelectedItems)
            {
                (lstFile.SelectedItems[0].Tag as MediaQueue).Audio.RemoveAt(item.Index);
                item.Remove();
            }

            lstAudio.SelectedIndexChanged += lstAudio_SelectedIndexChanged;
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
            if (lstFile.Focused || (sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    DisplayProperties_Audio();
                }
            }
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

                DisplayProperties_Audio();
            }
        }

        private void cboAudioEncoder_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = new Guid();

            try
            {
                if (cboAudioEncoder.SelectedItem == null)
                    cboAudioEncoder.SelectedIndex = 0;

                key = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
            }
            catch (Exception ex)
            {
                PrintLog(String.Format(i18nUI.Log("ContainerNotSupportAudio"), ex.Message));
                return;
            }
            
            if (cboAudioEncoder.SelectedIndex < 0)
                return;

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

                // Sample Rate
                cboAudioSampleRate.DataSource = null;
                cboAudioSampleRate.Items.Clear();

                var hz = new Dictionary<int, string>();
                foreach (var item in audio.SampleRate)
                {
                    if (item == 0)
                        hz.Add(0, "Auto");
                    else
                        hz.Add(item, $"{item} Hz");
                }
                cboAudioSampleRate.DisplayMember = "Value";
                cboAudioSampleRate.ValueMember = "Key";
                cboAudioSampleRate.DataSource = new BindingSource(hz, null);

                cboAudioSampleRate.SelectedValue = audio.SampleRateDefault;

                // Channel
                cboAudioChannel.DataSource = null;
                cboAudioChannel.Items.Clear();

                var ch = audio.Channels;
                cboAudioChannel.DisplayMember = "Value";
                cboAudioChannel.ValueMember = "Key";
                cboAudioChannel.DataSource = new BindingSource(ch, null);

                cboAudioChannel.SelectedValue = audio.ChannelDefault;
            }

            if ((sender as Control).Focused)
            {
                var enc = new MediaQueueAudioEncoder
                {
                    Id = new Guid($"{cboAudioEncoder.SelectedValue}"),
                    Mode = 0,
                    Quality = temp.Audio.Mode[0].Default,
                    SampleRate = temp.Audio.SampleRateDefault,
                    Channel = temp.Audio.ChannelDefault,
                    Command = string.Empty
                };

                if (lstFile.SelectedItems.Count == 1)
                {
                    foreach (ListViewItem item in lstAudio.SelectedItems)
                    {
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder = enc;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder = enc;
                        }
                    }
                }

                DisplayProperties_Audio();
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
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Quality = cboAudioQuality.Text;
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.SampleRate = (int?)(cboAudioSampleRate.SelectedValue) ?? 0;
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Channel = (int?)(cboAudioChannel.SelectedValue) ?? 0;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder.Mode = cboAudioMode.SelectedIndex;
                            item.Encoder.Quality = cboAudioQuality.Text;
                            item.Encoder.SampleRate = (int?)(cboAudioSampleRate.SelectedValue) ?? 0;
                            item.Encoder.Channel = (int?)(cboAudioChannel.SelectedValue) ?? 0;
                        }
                    }
                }

                DisplayProperties_Audio();
            }
        }

        private void cboAudioQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count == 1)
                {
                    foreach (ListViewItem item in lstAudio.SelectedItems)
                    {
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Quality = cboAudioQuality.Text;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder.Quality = cboAudioQuality.Text;
                        }
                    }
                }

                DisplayProperties_Audio();
            }
        }

        private void cboAudioSampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count == 1)
                {
                    foreach (ListViewItem item in lstAudio.SelectedItems)
                    {
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.SampleRate = (int?)(cboAudioSampleRate.SelectedValue) ?? 0;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder.SampleRate = (int?)(cboAudioSampleRate.SelectedValue) ?? 0;
                        }
                    }
                }

                DisplayProperties_Audio();
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
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Channel = (int?)(cboAudioChannel.SelectedValue) ?? 0;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder.Channel = (int?)(cboAudioChannel.SelectedValue) ?? 0;
                        }
                    }
                }

                DisplayProperties_Audio();
            }
        }

        private void btnAudioDec_Click(object sender, EventArgs e)
        {
            var cmd1 = string.Empty;
            var cmd2 = string.Empty;

            if (lstFile.SelectedItems.Count > 0)
            {
                if (lstAudio.SelectedItems.Count > 0)
                {
                    cmd1 = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[0].Command;
                    cmd2 = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[0].CommandFilter;
                }
            }

            var ib2 = new InputBox2(i18nUI.Dialog("AudioDecoderTitle"), i18nUI.Dialog("AudioDecoderMsg1"), i18nUI.Dialog("AudioDecoderMsg2"), cmd1, cmd2);
            if (ib2.ShowDialog() == DialogResult.OK)
            {
                cmd1 = ib2.ReturnValue1;
                cmd2 = ib2.ReturnValue2;
            }

            if (lstFile.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in lstAudio.SelectedItems)
                {
                    (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Command = cmd1;
                    (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].CommandFilter = cmd2;
                }
            }
            else if (lstFile.SelectedItems.Count > 1)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    foreach (var item in (queue.Tag as MediaQueue).Audio)
                    {
                        item.Command = cmd1;
                        item.CommandFilter = cmd2;
                    }
                }
            }
        }

        private void btnAudioEnc_Click(object sender, EventArgs e)
        {
            var cmd1 = string.Empty;

            if (lstFile.SelectedItems.Count > 0)
            {
                if (lstAudio.SelectedItems.Count > 0)
                {
                    cmd1 = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[0].Encoder.Command;
                }
            }

            var ib = new InputBox(String.Format(i18nUI.Dialog("CodecCommandLine1"), cboAudioEncoder.Text), i18nUI.Dialog("AudioEncoderMsg1"), cmd1);
            if (ib.ShowDialog() == DialogResult.OK)
            {
                cmd1 = ib.ReturnValue;
            }

            if (lstFile.SelectedItems.Count == 1)
            {
                foreach (ListViewItem item in lstAudio.SelectedItems)
                {
                    (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[item.Index].Encoder.Command = cmd1;
                }
            }
            else if (lstFile.SelectedItems.Count > 1)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    foreach (var item in (queue.Tag as MediaQueue).Audio)
                    {
                        item.Encoder.Command = cmd1;
                    }
                }
            }
        }

        private void tabConfigSubtitle_Enter(object sender, EventArgs e)
        {
            if (cboVideoEncoder.SelectedIndex < 0)
                return;

            var key = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;

            if (Plugins.Items.Video.TryGetValue(key, out var temp))
            {
                var video = temp.Video;

                var isFFmpeg = string.Equals(Path.GetFileNameWithoutExtension(video.Encoder[0].Binary).ToLowerInvariant(), "ffmpeg");
                var isPipe = video.Args.Pipe;

                chkSubHard.Enabled = isPipe || isFFmpeg; // check if selected video encoder is pipe or not for burning subs
            }
        }

        private void btnSubAdd_Click(object sender, EventArgs e)
        {
            var btnSender = (Button)sender;
            var ptLowerLeft = new Point(1, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            cmsFileAddSubs.Show(ptLowerLeft);
        }

        private void btnSubDel_Click(object sender, EventArgs e)
        {
            lstSub.SelectedIndexChanged -= lstSub_SelectedIndexChanged;

            foreach (ListViewItem item in lstSub.SelectedItems)
            {
                (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle.RemoveAt(item.Index);
                item.Remove();
            }

            lstSub.SelectedIndexChanged += lstSub_SelectedIndexChanged;
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
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    DisplayProperties_Subtitle();
                }
            }
        }

        private void lstSub_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstSub_DragDrop(object sender, DragEventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
                    MediaSubtitleListAdd(file);

            ListViewItem_RefreshSubtitle();
        }

        private void tsmiFileAddSubs_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Subtitle))
                    MediaSubtitleListAdd(item);

            ListViewItem_RefreshSubtitle();
        }

        private void tsmiFileAddSubsEmbed_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Video))
                    MediaSubtitleListAdd(item);

            ListViewItem_RefreshSubtitle();
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

                DisplayProperties_Subtitle();
            }
        }

        private void btnAttachAdd_Click(object sender, EventArgs e)
        {
            var btnSender = (Button)sender;
            var ptLowerLeft = new Point(1, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            cmsFileAddAttach.Show(ptLowerLeft);
        }

        private void btnAttachDel_Click(object sender, EventArgs e)
        {
            lstAttach.SelectedIndexChanged -= lstAttach_SelectedIndexChanged;

            foreach (ListViewItem item in lstAttach.SelectedItems)
            {
                (lstFile.SelectedItems[0].Tag as MediaQueue).Attachment.RemoveAt(item.Index);
                item.Remove();
            }

            lstAttach.SelectedIndexChanged += lstAttach_SelectedIndexChanged;
        }

        private void lstAttach_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    DisplayProperties_Attachment();
                }
            }
        }

        private void lstAttach_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void lstAttach_DragDrop(object sender, DragEventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var file in (string[])e.Data.GetData(DataFormats.FileDrop))
                    MediaAttachmentListAdd(file);

            ListViewItem_RefreshAttachment();
        }

        private void cboAttachMime_TextChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count == 1)
                {
                    foreach (ListViewItem item in lstAttach.SelectedItems)
                    {
                        (lstFile.SelectedItems[0].Tag as MediaQueue).Attachment[item.Index].Mime = cboAttachMime.Text;
                    }
                }
                else if (lstFile.SelectedItems.Count > 1)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        foreach (var item in (queue.Tag as MediaQueue).Attachment)
                        {
                            item.Mime = cboAttachMime.Text;
                        }
                    }
                }

                DisplayProperties_Attachment();
            }
        }

        private void tsmiFileAddAttach_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Attachment))
                    MediaAttachmentListAdd(item);

            ListViewItem_RefreshAttachment();
        }

        private void tsmiFileAddAttachEmbed_Click(object sender, EventArgs e)
        {
            if (lstFile.SelectedItems.Count > 0)
                foreach (var item in OpenFiles(MediaType.Video))
                    MediaAttachmentListAdd(item);

            ListViewItem_RefreshAttachment();
        }

        private void tabConfigAdvance_Enter(object sender, EventArgs e)
        {
            chkAdvTrim.Enabled = !(chkAudioMP4Compt.Checked || chkVideoMP4Compt.Checked); // prevent user trim when do Fast Remux
            chkAdvCropAuto.Enabled = !chkVideoMP4Compt.Checked; // prevent user crop when do Fast Remux

            grpAdvTrim.Enabled = chkAdvTrim.Checked;
            grpAdvCrop.Enabled = chkAdvCropAuto.Checked;
        }

        private void chkAdvTrim_CheckedChanged(object sender, EventArgs e)
        {
            grpAdvTrim.Enabled = chkAdvTrim.Checked;

            chkVideoMP4Compt.Enabled = !chkAdvTrim.Checked;
            chkAudioMP4Compt.Enabled = !chkAdvTrim.Checked;

            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        (queue.Tag as MediaQueue).Trim.Enable = grpAdvTrim.Enabled;
                    }
                }
            }
        }

        private void txtTrim_Validating(object sender, CancelEventArgs e)
        {
            var rTime = new Regex(@"[0-9][0-9]\:[0-6][0-9]\:[0-6][0-9]\.[0-9]{1,}");

            if ((sender as TextBox).Text.Length > 0)
            {
                if (!rTime.IsMatch((sender as TextBox).Text))
                {
                    MessageBox.Show(i18nUI.Dialog("FormatTimeCode"), i18nUI.Status("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtTrim_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || (e.KeyChar.ToString() == ":") || (e.KeyChar.ToString() == "."))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void txtTrim_Event(object sender, EventArgs e)
        {
            var ctrl = sender as TextBox;

            var bTimeStart = TimeSpan.TryParse(txtAdvTrimStart.Text, out var timeStart);
            var bTimeEnd = TimeSpan.TryParse(txtAdvTrimEnd.Text, out var timeEnd);
            var bTimeSpan = TimeSpan.TryParse(txtAdvTrimDuration.Text, out var timeSpan);

            if (bTimeStart && bTimeEnd && bTimeSpan)
            {
                if (ctrl.Focused)
                {
                    if (ctrl == txtAdvTrimStart)
                    {
                        timeSpan = timeEnd - timeStart;
                    }

                    if (ctrl == txtAdvTrimEnd)
                    {
                        timeSpan = timeEnd - timeStart;
                    }

                    if (ctrl == txtAdvTrimDuration)
                    {
                        timeEnd = timeStart + timeSpan;
                    }

                    txtAdvTrimStart.Text = $"{timeStart:hh\\:mm\\:ss\\.fff}";
                    txtAdvTrimEnd.Text = $"{timeEnd:hh\\:mm\\:ss\\.fff}";
                    txtAdvTrimDuration.Text = $"{timeSpan:hh\\:mm\\:ss\\.fff}";
                }

                foreach (ListViewItem item in lstFile.SelectedItems)
                {
                    (item.Tag as MediaQueue).Trim.Start = $"{timeStart:hh\\:mm\\:ss\\.fff}";
                    (item.Tag as MediaQueue).Trim.End = $"{timeEnd:hh\\:mm\\:ss\\.fff}";
                    (item.Tag as MediaQueue).Trim.Duration = $"{timeSpan:hh\\:mm\\:ss\\.fff}";
                }
            }
        }

        private void chkAdvCropAuto_CheckedChanged(object sender, EventArgs e)
        {
            grpAdvCrop.Enabled = chkAdvCropAuto.Checked;

            chkVideoMP4Compt.Enabled = !chkAdvCropAuto.Checked;
            lblVideoRes.Enabled = !chkAdvCropAuto.Checked;
            cboVideoRes.Enabled = !chkAdvCropAuto.Checked;

            if ((sender as Control).Focused)
            {
                if (lstFile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem item in lstFile.SelectedItems)
                    {
                        (item.Tag as MediaQueue).Crop.Enable = chkAdvCropAuto.Checked;
                    }
                }
            }
        }

        private void txtCrop_Event(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lstFile.SelectedItems)
            {
                (item.Tag as MediaQueue).Crop.Start = txtAdvCropStart.Text;
                (item.Tag as MediaQueue).Crop.Duration = txtAdvCropDuration.Text;
            }
        }

        private void cboFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            var videoId = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
            var audioId = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff");
            var videoCmd = string.Empty;
            var audioCmd = string.Empty;

            if (cboFormat.SelectedIndex > -1)
                ShowSupportedCodec(cboFormat.Text.ToLowerInvariant());

            if ((sender as Control).Focused)
            {
                cboProfile.SelectedIndex = -1;

                if (cboVideoEncoder.SelectedIndex >= 0)
                {
                    videoId = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
                    videoCmd = Plugins.Items.Video[videoId].Video.Args.Command;
                }

                if (cboAudioEncoder.SelectedIndex >= 0)
                {
                    audioId = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
                    audioCmd = Plugins.Items.Audio[audioId].Audio.Args.Command;
                }                

                if (lstFile.SelectedItems.Count > 0)
                {
                    foreach (ListViewItem queue in lstFile.SelectedItems)
                    {
                        var inExt = Path.GetExtension((queue.Tag as MediaQueue).FilePath).Substring(1).ToUpperInvariant();
                        var outExt = Enum.GetName(typeof(MediaContainer), cboFormat.SelectedIndex);

                        queue.SubItems[1].Text = $"{inExt} ► {outExt}";
                        (queue.Tag as MediaQueue).OutputFormat = (MediaContainer)cboFormat.SelectedIndex;

                        foreach (var item in (queue.Tag as MediaQueue).Video)
                        {
                            item.Encoder = new MediaQueueVideoEncoder
                            {
                                Id = videoId,
                                Preset = cboVideoPreset.Text,
                                Tune = cboVideoTune.Text,
                                Mode = cboVideoRateControl.SelectedIndex,
                                Value = nudVideoRateFactor.Value,
                                MultiPass = (int)nudVideoMultiPass.Value,
                                Command = videoCmd
                            };
                        }

                        foreach (var item in (queue.Tag as MediaQueue).Audio)
                        {
                            item.Encoder = new MediaQueueAudioEncoder
                            {
                                Id = audioId,
                                Mode = cboAudioMode.SelectedIndex,
                                Quality = cboAudioQuality.Text,
                                SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key,
                                Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key,
                                Command = audioCmd
                            };
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(cboFormat.Text))
            {
                btnStart.Enabled = false;
                cboVideoEncoder.DataSource = null;
                cboAudioEncoder.DataSource = null;
            }
            else
            {
                btnStart.Enabled = true;
            }

            tabConfigMediaInfo.Enabled = btnStart.Enabled;
            tabConfigVideo.Enabled = cboVideoEncoder.Items.Count > 0;
            tabConfigAudio.Enabled = cboAudioEncoder.Items.Count > 0;
            tabConfigSubtitle.Enabled = (MediaContainer)cboFormat.SelectedIndex < MediaContainer.WMV;
            tabConfigAttachment.Enabled = (MediaContainer)cboFormat.SelectedIndex == MediaContainer.MKV;
            tabConfigAdvance.Enabled = btnStart.Enabled;

            DisplayProperties_Video();
            DisplayProperties_Audio();
            DisplayProperties_Subtitle();
            DisplayProperties_Attachment();
        }

        private void cboProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Focused)
                SetProfileData(Profiles.Items[cboProfile.SelectedIndex]);
        }

        private void btnProfileSaveLoad_Click(object sender, EventArgs e)
        {
            int[] res = new int[] { 0, 0 };

            if (cboVideoRes.Text.Length > 0)
            {
                if (cboVideoRes.Text.Contains('x'))
                {
                    var temp = cboVideoRes.Text.Split('x');
                    res[0] = int.TryParse(temp[0], out int w) ? res[0] = w : res[0] = 0;
                    res[1] = int.TryParse(temp[1], out int h) ? res[1] = h : res[1] = 0;
                }
            }

            var input = new InputBox(i18nUI.Dialog("ProfileSaveTitle"), i18nUI.Dialog("ProfileSaveMsg1"), 4);
            if (input.ShowDialog() == DialogResult.OK)
            {
                var v = new ProfilesVideo
                {
                    Encoder = new MediaQueueVideoEncoder
                    {
                        Id = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key,
                        Preset = cboVideoPreset.Text,
                        Tune = cboVideoTune.Text,
                        Mode = cboVideoRateControl.SelectedIndex,
                        Value = nudVideoRateFactor.Value,
                        MultiPass = (int)nudVideoMultiPass.Value
                    },
                    Quality = new MediaQueueVideoQuality
                    {
                        Width = res[0],
                        Height = res[1],
                        FrameRate = float.TryParse(cboVideoFps.Text, out float fps) ? fps : 0,
                        BitDepth = int.TryParse(cboVideoBitDepth.Text, out int bpc) ? bpc : 8,
                        PixelFormat = int.TryParse(cboVideoPixFmt.Text, out int pix) ? pix : 420
                    },
                    DeInterlace = new MediaQueueVideoDeInterlace
                    {
                        Enable = chkVideoDeInterlace.Checked,
                        Mode = cboVideoDeInterMode.SelectedIndex,
                        Field = cboVideoDeInterField.SelectedIndex
                    }
                };

                var a = new ProfilesAudio
                {
                    Encoder = new MediaQueueAudioEncoder
                    {
                        Id = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key,
                        Mode = cboAudioMode.SelectedIndex,
                        Quality = cboAudioQuality.Text,
                        SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key,
                        Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key
                    }
                };
                
                ProfilesManager.Save(input.ReturnValue, (MediaContainer)cboFormat.SelectedIndex, v, a, chkVideoMP4Compt.Checked, chkAudioMP4Compt.Checked);

                // Reload new listing
                InitializeProfiles();
            }
        }

        private void btnProfileSaveLoad_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var btnSender = (Button)sender;
                var ptLowerLeft = new Point(1, btnSender.Height);
                ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
                cmsProfiles.Show(ptLowerLeft);
            }
        }

        private void btnOutputBrowse_Click(object sender, EventArgs e)
        {
            var fbd = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = false,
                Title = i18nUI.Dialog("OutputLocationTitle"),
                FileName = i18nUI.Dialog("OutputLocationFolder"),
                InitialDirectory = txtOutputPath.Text
            };

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                var fPath = Path.GetDirectoryName(fbd.FileName);
                txtOutputPath.Text = fPath;
            }
        }

        private void txtOutputPath_TextChanged(object sender, EventArgs e)
        {
            var savePath = (sender as Control).Text;

            try
            {
                var fPath = Path.GetFullPath(savePath);
                
                if (Path.IsPathRooted(fPath))
                {
                    Properties.Settings.Default.FolderOutput = fPath;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                PrintLog(ex.Message);
            }
        }

        private void tsmiImportFiles_Click(object sender, EventArgs e)
        {
            lstFile.SelectedIndices.Clear(); // clear last selected items

            var files = OpenFiles(MediaType.Video | MediaType.Audio);
            if (files.Length == 0)
                return;

            ImportFiles(files);
        }

        private void tsmiImportFolder_Click(object sender, EventArgs e)
        {
            var files = OS.FilesRecursive().ToArray();
            if (files.Length == 0)
                return;

            ImportFiles(files);
        }

        private void tsmiImportImgSeq_Click(object sender, EventArgs e)
        {
            var files = OpenFiles(MediaType.Image, true);

            if (files.Length <= 0)
                return;

            lstFile.SelectedIndices.Clear(); // clear last selected items

            using (var formImgSeq = new frmImportImageSeq(files[0]))
            {
                if (formImgSeq.ShowDialog() == DialogResult.OK)
                {
                    MediaFileListAdd(formImgSeq.FilePath, true, formImgSeq.FrameRate);
                }
            }
        }

        private void tsmiImportYouTube_Click(object sender, EventArgs e)
        {
            
        }

        private void tsmiUseAsIngestStation_Click(object sender, EventArgs e)
        {

            tsmiUseAsIngestStation.Checked = true;
        }

        private void tsmiPowerOff_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
        }

        private void tsmiProfilesSave_Click(object sender, EventArgs e)
        {
            btnProfileSaveLoad.PerformClick(); // using same function
        }

        private void tsmiProfilesRename_Click(object sender, EventArgs e)
        {
            if (cboProfile.SelectedIndex > -1)
            {
                if (cboProfile.Items.Count == Profiles.Items.Count)
                {
                    var index = cboProfile.SelectedIndex;
                    var oldName = Profiles.Items[index].ProfileName;

                    var ib = new InputBox(i18nUI.Dialog("ProfileRenameTitle"), i18nUI.Dialog("ProfileRenameMsg1"), oldName, 4);
                    if (ib.ShowDialog() == DialogResult.OK)
                    {
                        Profiles.Items[index].ProfileName = ib.ReturnValue;
                        ProfilesManager.Rename(index);
                        InitializeProfiles();
                        cboProfile.SelectedIndex = index;
                    }
                }
                else
                {
                    InitializeProfiles();
                }
            }
        }

        private void tsmiProfilesDelete_Click(object sender, EventArgs e)
        {
            if (cboProfile.SelectedIndex > -1)
            {
                if (cboProfile.Items.Count == Profiles.Items.Count)
                {
                    var msgBox = MessageBox.Show(i18nUI.Dialog("ProfileDeleteMsg1"), i18nUI.Dialog("ProfileDeleteTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (msgBox == DialogResult.Yes)
                    {
                        var index = cboProfile.SelectedIndex;
                        ProfilesManager.Delete(index);
                        InitializeProfiles();
                    }
                }
                else
                {
                    InitializeProfiles();
                }
            }
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

                    MediaEncoding.CurrentIndex = id;

                    var prefix = string.Empty;
                    var postfix = string.Empty;

                    switch (Properties.Settings.Default.PrefixMode)
                    {
                        case 1:
                            prefix = $"[{DateTime.Now:yyyy-MM-dd_HH-mm-ss}] ";
                            break;
                        case 2:
                            prefix = Properties.Settings.Default.PrefixText;
                            break;
                        default:
                            break;
                    }

                    switch (Properties.Settings.Default.PostfixMode)
                    {
                        case 1:
                            postfix = $" [{DateTime.Now:yyyy-MM-dd_HH-mm-ss}]";
                            break;
                        case 2:
                            postfix = Properties.Settings.Default.PostfixText;
                            break;
                        default:
                            break;
                    }

                    var outFileName = $"{prefix}{Path.GetFileNameWithoutExtension(mq.FilePath)}{postfix}";
                    var outFileExts = $"{mq.OutputFormat.ToString().ToLowerInvariant()}";
                    var saveFileName = $"{outFileName}.{outFileExts}";
                    var r = 1;

                    while (File.Exists(Path.Combine(txtOutputPath.Text, saveFileName)))
                    {
                        saveFileName = $"{outFileName}_{++r}.{outFileExts}";
                    }

                    // Create Temporary Session Folder
                    var tempSes = Path.Combine(Properties.Settings.Default.FolderTemporary, $"{Guid.NewGuid()}");
                    Directory.CreateDirectory(tempSes);



                    // Extract
                    MediaEncoding.Extract(mq, tempSes);

                    // Audio
                    MediaEncoding.Audio(mq, tempSes);

                    // Video
                    MediaEncoding.Video(mq, tempSes);

                    // Mux
                    var errCodeMux = MediaEncoding.Muxing(mq, tempSes, txtOutputPath.Text, saveFileName);

                    // Check FFmpeg Muxing is failed (negative) or 1 or sucess/warning (positive)
                    if (errCodeMux <= -1 || errCodeMux == 1)
                    {
                        Extensions.DirectoryCopy(tempSes, Path.Combine(txtOutputPath.Text, "[Muxing Failed]", $"{saveFileName}"), true);
                        PrintLog(i18nUI.Log("MuxingFailed"));
                        PrintLog(String.Format(i18nUI.Log("FFmpegReturnCode"), errCodeMux));
                    }
                    else
                    {
                        PrintLog(i18nUI.Log("MuxingSuccess"));
                        PrintLog(String.Format(i18nUI.Log("FFmpegReturnCode"), errCodeMux));
                    }

                    // Delete Temporary Session Folder
                    try { Directory.Delete(tempSes, true); }
                    catch (Exception ex) { PrintLog($"[ERROR] {ex.Message}"); }

                    lstFile.Invoke((MethodInvoker)delegate
                    {
                        lstFile.Items[id].Checked = false;
                        lstFile.Items[id].SubItems[4].Text = i18nUI.Status("Done");
                        lstFile.Items[id].SubItems[5].Text = String.Format(i18nUI.Status("Complete"), $"{DateTime.Now.Subtract(tt):dd\\:hh\\:mm\\:ss}");
                    });
                }
                else
                {
                    lstFile.Invoke((MethodInvoker)delegate
                    {
                        lstFile.Items[id].SubItems[4].Text = i18nUI.Status("Skip");
                        lstFile.Items[id].SubItems[5].Text = string.Empty;
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
                frmMain.PrintLog(i18nUI.Log("OperationCanceled"));

                foreach (ListViewItem item in lstFile.Items)
                {
                    item.SubItems[4].Text = i18nUI.Status("Abort");
                    item.SubItems[5].Text = "";
                }
            }

            if (!e.Cancelled && tsmiPowerOff.Checked)
            {
                frmMain.PrintLog(i18nUI.Log("OperationCompleteShutdown"));
                OS.PowerOff(3);
                return;
            }
        }
    }
}
