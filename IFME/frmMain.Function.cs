using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using IFME.OSManager;
using System.Net.Http.Headers;

namespace IFME
{
    [Flags]
    public enum MediaType
    {
        Video = 0x1,
        Audio = 0x2,
        Image = 0x4,
        Subtitle = 0x8,
        Attachment = 0x10
    }

    class VSDesignerBugFixA { }

    public partial class frmMain
    {
        private void InitializeFonts()
        {
            Fonts.Initialize();

            Control ctrl = this;
            do
            {
                ctrl = GetNextControl(ctrl, true);

                if (ctrl != null)
                {
                    if (ctrl is Button)
                    {
                        ctrl.Font = Fonts.Awesome(10f, FontStyle.Regular);
                    }
                    else
                    {
                        ctrl.Font = new Font("Tahoma", 8f);
                    }
                }
            } while (ctrl != null);

            txtMediaInfo.Font = Fonts.Uni(12f, FontStyle.Regular);
            rtfConsole.Font = Fonts.Uni(12f, FontStyle.Regular);
            cboFormat.Font = new Font("Consolas", 10f);
            cboProfile.Font = cboFormat.Font;
            txtOutputPath.Font = cboFormat.Font;

            btnFileAdd.Text = Fonts.fa.plus;
            btnFileDelete.Text = Fonts.fa.minus;
            btnOptions.Text = $"{Fonts.fa.gears}";
            btnAbout.Text = $"{Fonts.fa.info_circle}";
            btnFileUp.Text = Fonts.fa.chevron_up;
            btnFileDown.Text = Fonts.fa.chevron_down;
            btnDonate.Text = $"{Fonts.fa.money} {btnDonate.Text}";
            btnStart.Text = Fonts.fa.play;
            btnStop.Text = Fonts.fa.stop;

            btnVideoAdd.Text = btnFileAdd.Text;
            btnVideoDel.Text = btnFileDelete.Text;
            btnVideoMoveUp.Text = btnFileUp.Text;
            btnVideoMoveDown.Text = btnFileDown.Text;
            btnVideoDec.Font = new Font("Tahoma", 8f);
            btnVideoEnc.Font = new Font("Tahoma", 8f);

            btnAudioAdd.Text = btnFileAdd.Text;
            btnAudioDel.Text = btnFileDelete.Text;
            btnAudioMoveUp.Text = btnFileUp.Text;
            btnAudioMoveDown.Text = btnFileDown.Text;
            btnAudioDec.Font = new Font("Tahoma", 8f);
            btnAudioEnc.Font = new Font("Tahoma", 8f);

            btnSubAdd.Text = btnFileAdd.Text;
            btnSubDel.Text = btnFileDelete.Text;
            btnSubMoveUp.Text = btnFileUp.Text;
            btnSubMoveDown.Text = btnFileDown.Text;

            btnAttachAdd.Text = btnFileAdd.Text;
            btnAttachDel.Text = btnFileDelete.Text;

            btnProfileSaveLoad.Text = Fonts.fa.floppy_o;
            btnOutputBrowse.Text = Fonts.fa.folder;

            tabConfig.Font = Fonts.Awesome(10.5f, FontStyle.Regular);
            tabConfigMediaInfo.Text = $"{Fonts.fa.info} {tabConfigMediaInfo.Text}";
            tabConfigVideo.Text = $"{Fonts.fa.video_camera} {tabConfigVideo.Text}";
            tabConfigAudio.Text = $"{Fonts.fa.volume_up} {tabConfigAudio.Text}";
            tabConfigSubtitle.Text = $"{Fonts.fa.subscript} {tabConfigSubtitle.Text}";
            tabConfigAttachment.Text = $"{Fonts.fa.paperclip} {tabConfigAttachment.Text}";
            tabConfigAdvance.Text = $"{Fonts.fa.gear} {tabConfigAdvance.Text}";
            tabConfigLog.Text = $"{Fonts.fa.terminal} {tabConfigLog.Text}";
        }

        private void InitializeLog()
        {
            rtfConsole.Text = $"{Version.Title} {Version.Release} ( '{Version.CodeName}' )\n" +
                $"Build: {Version.Name} v{Version.Release} {Version.OSPlatform} {Version.OSArch} {Version.March} ({MArch.GetArchName[Version.March]})\r\n" +
                "\r\n" +
                $"(c) {DateTime.Now.Year} {Version.TradeMark}\r\n\r\nContributor: {Version.Contrib}\r\n" +
                "\r\n" +
                "Warning, DO NOT close this Terminal/Console, all useful info will be shown here.\r\n" +
                "\r\n";

            if (string.IsNullOrEmpty(PluginsLoad.ErrorLog))
                rtfConsole.AppendText("[INFO] No error during plugins initialising...\r\n");
            else
                rtfConsole.AppendText(PluginsLoad.ErrorLog);
        }

        private void InitializeProfiles()
        {
            new ProfilesManager();

            cboProfile.Items.Clear();
            foreach (var item in Profiles.Items)
                cboProfile.Items.Add(item.ProfileName);

            cboProfile.SelectedIndex = -1;
        }

        private void InitializeTab()
        {
            new Thread(Thread_InitializedTabs).Start();
        }

        private string[] OpenFiles(MediaType type, bool multiSelect = true)
        {
            var extsVideo = "All video types|*.mkv;*.mp4;*.m4v;*.ts;*.mts;*.m2ts;*.flv;*.webm;*.ogv;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.avs|";
            var extsAudio = "All audio types|*.mp2;*.mp3;*.mp4;*.m4a;*.aac;*.ogg;*.opus;*.flac;*.wav|";
            var extsImg = "All image types|*.bmp;*.png;*.jpg;*.jpe;*.jpeg;*.tiff;*.gif|";
            var extsSub = "All subtitle types|*.ssa;*.ass;*.srt;*.vtt;*.sub;*.idx;*.sup;*.slt;*.sst;*.ttml|";
            var extsSub2 = "MPEG subtitle types|*.srt;*.vtt;*.ttml|";
            var extsAtt = "All font types|*.ttf;*.otf;*.woff;*.woff2;*.eot|";

            string exts = string.Empty;

            if (type.HasFlag(MediaType.Video))
            {
                exts += extsVideo;
            }

            if (type.HasFlag(MediaType.Audio))
            {
                exts += extsAudio;
            }

            if (type.HasFlag(MediaType.Image))
            {
                exts += extsImg;
            }

            if (type.HasFlag(MediaType.Subtitle))
            {
                if ((MediaContainer)cboFormat.SelectedIndex == MediaContainer.MP4)
                    exts = extsSub2;
                else
                    exts += extsSub;
            }

            if (type.HasFlag(MediaType.Attachment))
            {
                exts += extsAtt;
            }

            var ofd = new OpenFileDialog
            {
                Filter = exts += "All types|*.*",
                FilterIndex = 1,
                Multiselect = multiSelect
            };

            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileNames;

            return Array.Empty<string>();
        }

        public static void EnableTab(TabPage page, bool enable)
        {
            foreach (Control ctrl in page.Controls)
                ctrl.Enabled = enable;
        }

        private void MediaFileListAdd(string path, bool isImages, string frameRate = "")
        {
            var fileData = new FFmpeg.MediaInfo(path, frameRate);
            var fileQueue = new MediaQueue()
            {
                Enable = true,

                FilePath = path,
                FileSize = fileData.FileSize,
                Duration = fileData.Duration,
                InputFormat = fileData.FormatNameFull,
                OutputFormat = (MediaContainer)cboFormat.SelectedIndex,
                Info = fileData
            };

            foreach (var item in fileData.Video)
                fileQueue.Video.Add(MediaQueueParse.Video(path, item, isImages));

            foreach (var item in fileData.Audio)
                fileQueue.Audio.Add(MediaQueueParse.Audio(path, item));	

            foreach (var item in fileData.Subtitle)
                fileQueue.Subtitle.Add(MediaQueueParse.Subtitle(path, item));
            
            foreach (var item in fileData.Attachment)
                fileQueue.Attachment.Add(MediaQueueParse.Attachment(path, item));

            // Enable HardSub (Burn Subtitle) when using incompatible container, make sure disable control
            if ((MediaContainer)cboFormat.SelectedIndex == MediaContainer.MKV)
                fileQueue.HardSub = false;
            else if ((MediaContainer)cboFormat.SelectedIndex == MediaContainer.MP4)
                fileQueue.HardSub = false;
            else
                fileQueue.HardSub = true;

            var lst = new ListViewItem(new[]
            {
                Path.GetFileName(path),
                $"{Path.GetExtension(path).Substring(1).ToUpperInvariant()} ► {Enum.GetName(typeof(MediaContainer), cboFormat.SelectedIndex)}",
                TimeSpan.FromSeconds(fileData.Duration).ToString("hh\\:mm\\:ss"),
                OS.PrintFileSize(fileData.FileSize),
                fileQueue.Enable ? "Ready" : "Done",
                ""
            })
            {
                Tag = fileQueue,
                Checked = true,
                Selected = true
            };

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    lstFile.Focus();
                    lstFile.Items.Add(lst);
                }));
            }
            else
            {
                lstFile.Focus();
                lstFile.Items.Add(lst);
            }
        }

        private void MediaVideoListAdd(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                foreach (var item in fileData.Video)
                {
                    (lst.Tag as MediaQueue).Video.Add(MediaQueueParse.Video(path, item));
                }
            }

            DisplayProperties_Video();
        }

        private void MediaAudioListAdd(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                foreach (var item in fileData.Audio)
                {
                    (lst.Tag as MediaQueue).Audio.Add(MediaQueueParse.Audio(path, item));
                }
            }

            DisplayProperties_Audio();
        }

        private void MediaSubtitleListAdd(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                var id = -1;

                if (fileData.Video.Count > 0 && fileData.Subtitle.Count > 0)
                {
                    id = -2;
                }

                (lst.Tag as MediaQueue).Subtitle.Add(new MediaQueueSubtitle
                {
                    Enable = true,
                    File = path,
                    Id = id,
                    Lang = "und",
                    Codec = string.Empty
                });
            }

            DisplayProperties_Subtitle();
        }

        private void MediaAttachmentListAdd(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                var id = -1;
                var name = Path.GetFileName(path);
                var mime = Mime.GetType(path);

                if (fileData.Video.Count > 0 && fileData.Attachment.Count > 0)
                {
                    id = -2;
                    name = $"(embed) {name}";
                    mime = "application/octet-stream";
                }

                (lst.Tag as MediaQueue).Attachment.Add(new MediaQueueAttachment
                {
                    Enable = true,
                    File = path,
                    Id = id,
                    Name = name,
                    Mime = mime
                });
            }

            DisplayProperties_Attachment();
        }

        private void DisplayProperties_Video()
        {
            if ((MediaContainer)cboFormat.SelectedIndex >= MediaContainer.MP2)
                return;

            if (lstVideo.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[lstVideo.SelectedItems[0].Index];
                    new Thread(Thread_LoadPropertiesVideo).Start(data);
                }
                catch (Exception ex)
                {
                    PrintLog($"[ERRO] DisplayProperties_Video(), {ex.Message}");
                }
            }
        }

        private void DisplayProperties_Audio()
        {
            if (lstAudio.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[lstAudio.SelectedItems[0].Index];
                    new Thread(Thread_LoadPropertiesAudio).Start(data);
                }
                catch (Exception ex)
                {
                    PrintLog($"[ERRO] ID: DisplayProperties_Audio(), {ex.Message}");
                }
            }
        }

        private void DisplayProperties_Subtitle()
        {
            if (lstSub.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle[lstSub.SelectedItems[0].Index];
                    new Thread(Thread_LoadPropertiesSubtitle).Start(data);
                }
                catch (Exception ex)
                {
                    PrintLog($"[ERRO] ID: DisplayProperties_Subtitle(), {ex.Message}");
                }
            }
        }

        private void DisplayProperties_Attachment()
        {
            if (lstAttach.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Attachment[lstAttach.SelectedItems[0].Index];
                    new Thread(Thread_LoadPropertiesAttachment).Start(data);

                }
                catch (Exception ex)
                {
                    PrintLog($"[ERRO] ID: DisplayProperties_Attachment(), {ex.Message}");
                }
            }
        }

        private void Thread_InitializedTabs()
        {
            BeginInvoke((Action)delegate ()
            {
                foreach (TabPage item in tabConfig.TabPages)
                {
                    tabConfig.SelectedTab = item;
                }

                tabConfig.SelectedTab = tabConfigMediaInfo;

                chkVideoDeInterlace.Checked = true;
                chkVideoDeInterlace.Checked = false;
                chkAdvTrim.Checked = true;
                chkAdvTrim.Checked = false;
                chkAdvCropAuto.Checked = true;
                chkAdvCropAuto.Checked = false;

                cboProfile.Focus();
                cboProfile.SelectedIndex = cboProfile.Items.Count - 1;
            });
        }

        private void Thread_LoadPropertiesVideo(object obj)
        {
            var data = obj as MediaQueueVideo;

            BeginInvoke((Action)delegate ()
            {
                cboVideoLang.SelectedValue = data.Lang;
                
                cboVideoEncoder.SelectedValue = data.Encoder.Id;
                cboVideoPreset.SelectedItem = data.Encoder.Preset;
                cboVideoTune.SelectedItem = data.Encoder.Tune;
                cboVideoRateControl.SelectedIndex = data.Encoder.Mode;

                nudVideoRateFactor.Minimum = Plugins.Items.Video[data.Encoder.Id].Video.Mode[data.Encoder.Mode].Value.Min;
                nudVideoRateFactor.Maximum = Plugins.Items.Video[data.Encoder.Id].Video.Mode[data.Encoder.Mode].Value.Max;
                nudVideoRateFactor.Value = data.Encoder.Value;
                nudVideoMultiPass.Value = data.Encoder.MultiPass;

                cboVideoRes.Text = $"{(data.Quality.Width == 0 || data.Quality.Height == 0 ? "auto" : $"{data.Quality.Width}x{data.Quality.Height}" )}";
                cboVideoFps.Text = $"{Math.Round(data.Quality.FrameRate, 3)}";
                cboVideoBitDepth.Text = $"{data.Quality.BitDepth}";
                cboVideoPixFmt.Text = $"{data.Quality.PixelFormat}";

                chkVideoDeInterlace.Checked = data.DeInterlace.Enable;
                cboVideoDeInterMode.SelectedIndex = data.DeInterlace.Mode;
                cboVideoDeInterField.SelectedIndex = data.DeInterlace.Field;

                // Fast Remux compact.
                chkVideoMP4Compt.Enabled = !data.IsImageSeq;

                foreach (ListViewItem item in lstVideo.SelectedItems)
                {
                    item.SubItems[1].Text = Language.FullName(data.Lang);
                    item.SubItems[2].Text = cboVideoRes.Text;
                    item.SubItems[3].Text = cboVideoFps.Text;
                    item.SubItems[4].Text = cboVideoBitDepth.Text;
                    item.SubItems[5].Text = cboVideoPixFmt.Text;
                }
            });
        }

        private void Thread_LoadPropertiesAudio(object obj)
        {
            var data = obj as MediaQueueAudio;

            BeginInvoke((Action)delegate ()
            {
                cboAudioLang.SelectedValue = data.Lang;
                cboAudioEncoder.SelectedValue = data.Encoder.Id;
                cboAudioMode.SelectedIndex = data.Encoder.Mode;
            });

            Thread.Sleep(1);

            BeginInvoke((Action)delegate ()
            {
                cboAudioQuality.SelectedItem = data.Encoder.Quality;
                cboAudioSampleRate.SelectedValue = data.Encoder.SampleRate;
                cboAudioChannel.SelectedValue = data.Encoder.Channel;

                foreach (ListViewItem item in lstAudio.SelectedItems)
                {
                    item.SubItems[1].Text = Language.FullName(data.Lang);
                    item.SubItems[2].Text = cboAudioQuality.Text.Equals("0") ? "Auto" : cboAudioQuality.Text;
                    item.SubItems[3].Text = cboAudioSampleRate.Text.Equals("0") ? "Auto" : cboAudioSampleRate.Text;
                    item.SubItems[4].Text = cboAudioChannel.Text.Equals("0") ? "Auto" : cboAudioChannel.Text;
                }
            });
        }

        private void Thread_LoadPropertiesSubtitle(object obj)
        {
            var data = obj as MediaQueueSubtitle;

            BeginInvoke((Action)delegate ()
            {
                cboSubLang.SelectedValue = data.Lang;

                foreach (ListViewItem item in lstSub.SelectedItems)
                {
                    item.SubItems[2].Text = Language.FullName(data.Lang);
                }
            });
        }

        private void Thread_LoadPropertiesAttachment(object obj)
        {
            var data = obj as MediaQueueAttachment;

            BeginInvoke((Action)delegate ()
            {                
                foreach (ListViewItem item in lstAttach.SelectedItems)
                {
                    item.SubItems[2].Text = data.Mime;
                }

                cboAttachMime.Text = data.Mime;
            });
        }

        private void ListViewItem_RefreshVideo()
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                ListViewItem_RefreshVideo(lstFile.SelectedItems[0].Tag as MediaQueue);

                foreach (ListViewItem item in lstVideo.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void ListViewItem_RefreshVideo(MediaQueue data)
        {
            lstVideo.SelectedItems.Clear();
            lstVideo.Items.Clear();
            foreach (var item in data.Video)
            {
                var res = $"{item.Info.Width}x{item.Info.Height}";
                var fps = item.Info.FrameRate.ToString();

                if (item.Quality.Width != 0 && item.Quality.Height != 0)
                    res = $"{item.Quality.Width}x{item.Quality.Height}";

                if (item.Quality.FrameRate != 0)
                    fps = $"{item.Quality.FrameRate, 3}";

                lstVideo.Items.Add(new ListViewItem(new[]
                {
                        $"{item.Id}",
                        Language.FullName(item.Lang),
                        res,
                        fps,
                        $"{item.Quality.BitDepth}",
                        $"{item.Quality.PixelFormat}"
                })
                {
                    Checked = true,
                    Tag = item
                });
            }
        }

        private void ListViewItem_RefreshAudio()
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                ListViewItem_RefreshAudio(lstFile.SelectedItems[0].Tag as MediaQueue);

                foreach (ListViewItem item in lstAudio.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void ListViewItem_RefreshAudio(MediaQueue data)
        {
            lstAudio.SelectedItems.Clear();
            lstAudio.Items.Clear();
            foreach (var item in data.Audio)
            {
                lstAudio.Items.Add(new ListViewItem(new[]
                {
                        $"{item.Id}",
                        Language.FullName(item.Lang),
                        item.Encoder.Quality.Equals(0) ? "Auto" : item.Encoder.Quality.ToString(),
                        item.Encoder.SampleRate.Equals(0) ? "Auto" : item.Encoder.SampleRate.ToString(),
                        item.Encoder.Channel.Equals(0) ? "Auto" : item.Encoder.Channel.ToString()
                    })
                {
                    Checked = true,
                    Tag = item
                });
            }
        }

        private void ListViewItem_RefreshSubtitle()
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                ListViewItem_RefreshSubtitle(lstFile.SelectedItems[0].Tag as MediaQueue);

                foreach (ListViewItem item in lstSub.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void ListViewItem_RefreshSubtitle(MediaQueue data)
        {
            lstSub.SelectedItems.Clear();
            lstSub.Items.Clear();
            foreach (var item in data.Subtitle)
            {
                lstSub.Items.Add(new ListViewItem(new[]
                {
                        $"{item.Id}",
                        Path.GetFileName(item.File),
                        Language.FullName(item.Lang)
                    })
                {
                    Checked = true,
                    Tag = item
                });
            }
        }

        private void ListViewItem_RefreshAttachment()
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                ListViewItem_RefreshAttachment(lstFile.SelectedItems[0].Tag as MediaQueue);

                foreach (ListViewItem item in lstAttach.Items)
                {
                    item.Selected = true;
                }
            }
        }

        private void ListViewItem_RefreshAttachment(MediaQueue data)
        {
            lstAttach.SelectedItems.Clear();
            lstAttach.Items.Clear();
            foreach (var item in data.Attachment)
            {
                lstAttach.Items.Add(new ListViewItem(new[]
                {
                        $"{item.Id}",
                        Path.GetFileName(item.Name),
                        item.Mime
                    })
                {
                    Checked = true
                });
            }
        }

        private bool CheckImageSeqEncoder(MediaQueueVideo data, Guid id)
        {
            if (data.IsImageSeq)
            {
                if (id.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                {
                    return true;
                }
            }

            return false;
        }

        private void ShowSupportedCodec(string value, bool imgSeq = false)
        {
            var videoCodec = new Dictionary<Guid, PluginsVideo>();
            var audioCodec = new Dictionary<Guid, PluginsAudio>();

            var videoId = new Guid();
            var audioId = new Guid();

            foreach (var item in Plugins.Items.Video)
            {
                if (item.Key.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                {
                    if (imgSeq)
                        continue;
                }

                if (item.Value.Format.Contains(value))
                {
                    videoCodec.Add(item.Key, item.Value);
                    videoId = item.Key;
                }
            }

            foreach (var item in Plugins.Items.Audio)
            {
                var exts = value;

                if (value.Contains(' '))
                    exts = value.Remove(value.IndexOf(' '));

                if (item.Value.Format.Contains(exts))
                {
                    audioCodec.Add(item.Key, item.Value);
                    audioId = item.Key;
                }
            }

            if (videoCodec.Count > 0)
            {
                cboVideoEncoder.DataSource = new BindingSource(videoCodec.ToDictionary(p => p.Key, p => p.Value.Name), null);
                cboVideoEncoder.DisplayMember = "Value";
                cboVideoEncoder.ValueMember = "Key";
            }
            else
            {
                cboVideoEncoder.DataSource = null;
            }

            if (audioCodec.Count > 0)
            {
                cboAudioEncoder.DataSource = new BindingSource(audioCodec.ToDictionary(p => p.Key, p => p.Value.Name), null);
                cboAudioEncoder.DisplayMember = "Value";
                cboAudioEncoder.ValueMember = "Key";
            }
            else
            {
                cboAudioEncoder.DataSource = null;
            }

            SetDefinedData(videoId, audioId);
        }

        private void SetDefinedData(Guid videoId, Guid audioId)
        {
            if (lstFile.Focused)
                return;

            if (cboProfile.Focused)
                return;

            if (Plugins.Items.Video.TryGetValue(videoId, out PluginsVideo vData))
            {

            }

            if (Plugins.Items.Audio.TryGetValue(audioId, out PluginsAudio aData))
            {

            }

            if (lstFile.SelectedItems.Count > 0)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    foreach (var item in (queue.Tag as MediaQueue).Video)
                    {
                        var matchVideo = cboVideoEncoder.Items.Cast<KeyValuePair<Guid, string>>().Any(index => Equals(item.Encoder.Id, index.Key));
                        if (matchVideo) break;

                        if (vData != null)
                        {
                            if (Guid.TryParse($"{vData.GUID}", out Guid guid))
                            {
                                if (Equals(guid, item.Encoder.Id))
                                    continue;

                                item.Encoder = new MediaQueueVideoEncoder
                                {
                                    Id = guid,
                                    Preset = vData.Video.PresetDefault,
                                    Tune = vData.Video.TuneDefault,
                                    Mode = 0,
                                    Value = vData.Video.Mode[0].Value.Default,
                                    MultiPass = 2,
                                    Command = string.Empty
                                };
                                item.Quality.Command = string.Empty;
                            }
                        }
                        else
                        {
                            item.Encoder = new MediaQueueVideoEncoder
                            {
                                Id = new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff") // GUID do not encode video
                            };
                        }
                    }

                    foreach (var item in (queue.Tag as MediaQueue).Audio)
                    {
                        var matchAudio = cboAudioEncoder.Items.Cast<KeyValuePair<Guid, string>>().Any(index => Equals(item.Encoder.Id, index.Key));
                        if (matchAudio) break;

                        if (aData != null)
                        {
                            if (Guid.TryParse($"{aData.GUID}", out Guid guid))
                            {
                                item.Encoder = new MediaQueueAudioEncoder
                                {
                                    Id = guid,
                                    Mode = 0,
                                    Quality = aData.Audio.Mode[0].Default,
                                    SampleRate = aData.Audio.SampleRateDefault,
                                    Channel = aData.Audio.ChannelDefault,
                                    Command = string.Empty,
                                };
                                item.Command = string.Empty;
                            }
                        }
                    }
                }
            }

            DisplayProperties_Video();
            DisplayProperties_Audio();
            DisplayProperties_Subtitle();
            DisplayProperties_Attachment();
        }

        private void SetProfileData(Profiles value)
        {
            cboFormat.SelectedIndex = (int)value.Container;

            cboVideoEncoder.SelectedValue = value.Video.Encoder.Id;
            cboVideoPreset.SelectedItem = value.Video.Encoder.Preset;
            cboVideoTune.SelectedItem = value.Video.Encoder.Tune;
            cboVideoRateControl.SelectedIndex = value.Video.Encoder.Mode;

            nudVideoRateFactor.Minimum = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.Min;
            nudVideoRateFactor.Maximum = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.Max;
            nudVideoRateFactor.Value = value.Video.Encoder.Value;
            nudVideoMultiPass.Value = value.Video.Encoder.MultiPass;

            cboVideoRes.Text = $"{(value.Video.Quality.Width == 0 || value.Video.Quality.Height == 0 ? "auto" : $"{value.Video.Quality.Width}x{value.Video.Quality.Height}")}";
            cboVideoFps.Text = $"{Math.Round(value.Video.Quality.FrameRate, 3)}";
            cboVideoBitDepth.Text = $"{value.Video.Quality.BitDepth}";
            cboVideoPixFmt.Text = $"{value.Video.Quality.PixelFormat}";

            chkVideoDeInterlace.Checked = value.Video.DeInterlace.Enable;
            cboVideoDeInterMode.SelectedIndex = value.Video.DeInterlace.Mode;
            cboVideoDeInterField.SelectedIndex = value.Video.DeInterlace.Field;

            MediaEncoding.VideoFiArgs = value.Video.Quality.CommandFilter;
            MediaEncoding.VideoDeArgs = value.Video.Quality.Command;
            MediaEncoding.VideoEnArgs = value.Video.Encoder.Command;

            cboAudioEncoder.SelectedValue = value.Audio.Encoder.Id;
            cboAudioMode.SelectedIndex = value.Audio.Encoder.Mode;
            cboAudioQuality.SelectedValue = value.Audio.Encoder.Quality;
            cboAudioSampleRate.SelectedValue = value.Audio.Encoder.SampleRate;
            cboAudioChannel.SelectedValue = value.Audio.Encoder.Channel;

            MediaEncoding.AudioFiArgs = value.Audio.CommandFilter;
            MediaEncoding.AudioDeArgs = value.Audio.Command;
            MediaEncoding.AudioEnArgs = value.Audio.Encoder.Command;

            chkVideoMP4Compt.Checked = value.TryRemuxVideo;
            chkAudioMP4Compt.Checked = value.TryRemuxAudio;

            if (lstFile.SelectedItems.Count > 0)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    (queue.Tag as MediaQueue).OutputFormat = value.Container;
                    (queue.Tag as MediaQueue).ProfileId = cboProfile.SelectedIndex;

                    var media = queue.Tag as MediaQueue;
                    if (media == null) continue;

                    media.FastMuxVideo = chkVideoMP4Compt.Checked;
                    media.FastMuxAudio = chkAudioMP4Compt.Checked;

                    // modify video collection
                    for (int i = 0; i < media.Video.Count; i++)
                    {
                        var v = new MediaQueueVideo
                        {
                            Encoder = new MediaQueueVideoEncoder
                            {
                                Id = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key,
                                Preset = cboVideoPreset.Text,
                                Tune = cboVideoTune.Text,
                                Mode = cboVideoRateControl.SelectedIndex,
                                Value = nudVideoRateFactor.Value,
                                MultiPass = (int)nudVideoMultiPass.Value,
                                Command = MediaEncoding.VideoEnArgs
                            },
                            Quality = new MediaQueueVideoQuality
                            {
                                Width = value.Video.Quality.Width,
                                Height = value.Video.Quality.Height,
                                FrameRate = float.TryParse(cboVideoFps.Text, out float fps) ? fps : 0,
                                BitDepth = int.TryParse(cboVideoBitDepth.Text, out int bpc) ? bpc : 8,
                                PixelFormat = int.TryParse(cboVideoPixFmt.Text, out int pix) ? pix : 420,
                                Command = MediaEncoding.VideoDeArgs,
                                CommandFilter = MediaEncoding.VideoFiArgs,
                            },
                            DeInterlace = new MediaQueueVideoDeInterlace
                            {
                                Enable = chkVideoDeInterlace.Checked,
                                Mode = cboVideoDeInterMode.SelectedIndex,
                                Field = cboVideoDeInterField.SelectedIndex
                            }
                        };
                        media.Video[i] = v;
                    }

                    // modify audio collection
                    for (int x = 0; x < media.Audio.Count; x++)
                    {
                        var a = new MediaQueueAudio
                        {
                            Encoder = new MediaQueueAudioEncoder
                            {
                                Id = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key,
                                Mode = cboAudioMode.SelectedIndex,
                                Quality = cboAudioQuality.SelectedText,
                                SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key,
                                Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key,
                                Command = MediaEncoding.AudioEnArgs
                            },
                            Command = MediaEncoding.AudioDeArgs,
                            CommandFilter = MediaEncoding.AudioFiArgs
                        };
                    }
                }

                DisplayProperties_Video();
                DisplayProperties_Audio();
                DisplayProperties_Subtitle();
                DisplayProperties_Attachment();
            }
        }
    }
}
