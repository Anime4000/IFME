using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using IFME.OSManager;
using Newtonsoft.Json;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;

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
                $"{Version.Name} v{Version.Release} {Version.OSPlatform} {Version.OSArch} {Version.March} ({MArch.GetArchName[Version.March]})\r\n" +
                "\r\n" +
                $"(c) {DateTime.Now.Year} {Version.TradeMark}\r\n\r\n(s) {Version.Contrib}\r\n" +
                "\r\n" +
                $"{i18n.UI.Logs["WarningInfo"]}\r\n" +
                "\r\n";

            if (string.IsNullOrEmpty(PluginsLoad.ErrorLog))
                rtfConsole.AppendText(i18n.UI.Logs["PluginLoadOK"]);
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

        private void ImportFiles(string[] files)
        {
            var frm = new frmProgressBar();

            lstFile.SelectedIndices.Clear();

            frm.Show();
            frm.Text = i18n.UI.Dialogs["Importing"];
            frm.Status = i18n.UI.Dialogs["Indexing"];

            var thread = new BackgroundWorker();

            thread.DoWork += delegate (object o, DoWorkEventArgs r)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (InvokeRequired)
                    {
                        if (frm.Visible)
                        {
                            Invoke(new MethodInvoker(delegate
                            {
                                MediaFileListAdd(files[i], false);
                                frm.Progress = (int)(((float)(i + 1) / files.Length) * 100.0);
                                frm.Status = String.Format(i18n.UI.Dialogs["ImportStatus"], i + 1, files.Length, files[i]);
                                frm.Title = String.Format(i18n.UI.Dialogs["ImportTitle"], frm.Progress);
                            }));
                        }
                    }
                }

                Thread.Sleep(500);
                frm.ProgBarStyle = ProgressBarStyle.Marquee;
                Thread.Sleep(999);
            };

            thread.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs r)
            {
                frm.Close();
            };

            thread.RunWorkerAsync();
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
                ProfileId = cboProfile.SelectedIndex,
                Info = fileData
            };

            foreach (var item in fileData.Video)
                fileQueue.Video.Add(new MediaQueueVideo
                {
                    Enable = true,
                    FilePath = path,
                    Id = item.Id,
                    Lang = item.Language,
                    Codec = item.Codec,

                    IsImageSeq = isImages,

                    Info = new MediaQueueVideoInfo
                    {
                        Width = item.Width,
                        Height = item.Height,
                        FrameRate = (float)Math.Round(item.FrameRateAvg, 3),
                        FrameRateAvg = item.FrameRateAvg,
                        FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRateAvg),
                        IsVFR = !item.FrameRateConstant,
                        BitDepth = item.BitDepth,
                        PixelFormat = item.Chroma,
                        Disposition_AttachedPic = item.Disposition_AttachedPic
                    },

                    Encoder = new MediaQueueVideoEncoder
                    {
                        Id = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key,
                        Preset = cboVideoPreset.Text,
                        Tune = cboVideoTune.Text,
                        Mode = cboVideoRateControl.SelectedIndex,
                        Value = nudVideoRateFactor.Value,
                        MultiPass = (int)nudVideoMultiPass.Value,
                        Command = string.Empty
                    },

                    Quality = new MediaQueueVideoQuality
                    {
                        Width = item.Width,
                        Height = item.Height,
                        FrameRate = (float)Math.Round(item.FrameRateAvg, 3),
                        FrameRateAvg = item.FrameRateAvg,
                        FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRate),
                        IsVFR = !item.FrameRateConstant,
                        BitDepth = item.BitDepth,
                        PixelFormat = item.Chroma,
                        CommandFilter = string.Empty,
                        Command = string.Empty
                    },

                    DeInterlace = new MediaQueueVideoDeInterlace
                    {
                        Enable = chkVideoDeInterlace.Checked,
                        Mode = cboVideoDeInterMode.SelectedIndex,
                        Field = cboVideoDeInterField.SelectedIndex
                    }
                });

            foreach (var item in fileData.Audio)
                fileQueue.Audio.Add(new MediaQueueAudio
                {
                    Enable = true,
                    FilePath = path,
                    Id = item.Id,
                    Lang = item.Language,
                    Codec = item.Codec,

                    Info = new MediaQueueAudioInfo
                    {
                        SampleRate = item.SampleRate,
                        Channel = item.Channel
                    },

                    Encoder = new MediaQueueAudioEncoder
                    {
                        Id = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key,
                        Mode = cboAudioMode.SelectedIndex,
                        Quality = cboAudioQuality.Text,
                        SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key,
                        Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key,
                        Command = string.Empty
                    },

                    CommandFilter = string.Empty,
                    Command = string.Empty
                });	

            foreach (var item in fileData.Subtitle)
                fileQueue.Subtitle.Add(new MediaQueueSubtitle
                {
                    Enable = true,
                    FilePath = path,
                    Id = item.Id,
                    Lang = item.Language,
                    Codec = item.Codec,
                });
            
            foreach (var item in fileData.Attachment)
                fileQueue.Attachment.Add(new MediaQueueAttachment
                {
                    Enable = true,
                    FilePath = path,
                    Id = item.Id,
                    Name = item.FileName,
                    Mime = item.MimeType
                });

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
                    (lst.Tag as MediaQueue).Video.Add(new MediaQueueVideo
                    {
                        Enable = true,
                        FilePath = path,
                        Id = item.Id,
                        Lang = item.Language,
                        Codec = item.Codec,

                        IsImageSeq = false,

                        Info = new MediaQueueVideoInfo
                        {
                            Width = item.Width,
                            Height = item.Height,
                            FrameRate = (float)Math.Round(item.FrameRateAvg, 3),
                            FrameRateAvg = item.FrameRateAvg,
                            FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRateAvg),
                            IsVFR = !item.FrameRateConstant,
                            BitDepth = item.BitDepth,
                            PixelFormat = item.Chroma,
                            Disposition_AttachedPic = item.Disposition_AttachedPic
                        },

                        Encoder = new MediaQueueVideoEncoder
                        {
                            Id = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key,
                            Preset = cboVideoPreset.Text,
                            Tune = cboVideoTune.Text,
                            Mode = cboVideoRateControl.SelectedIndex,
                            Value = nudVideoRateFactor.Value,
                            MultiPass = (int)nudVideoMultiPass.Value,
                            Command = string.Empty
                        },

                        Quality = new MediaQueueVideoQuality
                        {
                            Width = item.Width,
                            Height = item.Height,
                            FrameRate = (float)Math.Round(item.FrameRateAvg, 3),
                            FrameRateAvg = item.FrameRateAvg,
                            FrameCount = (int)Math.Ceiling(item.Duration * item.FrameRate),
                            IsVFR = !item.FrameRateConstant,
                            BitDepth = item.BitDepth,
                            PixelFormat = item.Chroma,
                            CommandFilter = string.Empty,
                            Command = string.Empty
                        },

                        DeInterlace = new MediaQueueVideoDeInterlace
                        {
                            Enable = chkVideoDeInterlace.Checked,
                            Mode = cboVideoDeInterMode.SelectedIndex,
                            Field = cboVideoDeInterField.SelectedIndex
                        }
                    });
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
                    (lst.Tag as MediaQueue).Audio.Add(new MediaQueueAudio
                    {
                        Enable = true,
                        FilePath = path,
                        Id = item.Id,
                        Lang = item.Language,
                        Codec = item.Codec,

                        Info = new MediaQueueAudioInfo
                        {
                            SampleRate = item.SampleRate,
                            Channel = item.Channel
                        },

                        Encoder = new MediaQueueAudioEncoder
                        {
                            Id = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key,
                            Mode = cboAudioMode.SelectedIndex,
                            Quality = cboAudioQuality.Text,
                            SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key,
                            Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key,
                            Command = string.Empty
                        },

                        CommandFilter = string.Empty,
                        Command = string.Empty
                    });
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
                    FilePath = path,
                    Id = id,
                    Lang = Language.FromFileNameCode(path, cboSubLang.SelectedValue),
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
                    FilePath = path,
                    Id = id,
                    Name = name,
                    Mime = mime
                });
            }

            DisplayProperties_Attachment();
        }

        private void DisplayProperties_Clear()
        {
            lstVideo.Items.Clear();
            lstAudio.Items.Clear();
            lstSub.Items.Clear();
            lstAttach.Items.Clear();
            txtMediaInfo.Text = i18n.UI.Dialogs["MediaInfo"];
            chkVideoDeInterlace.Checked = false;
            chkVideoMP4Compt.Checked = false;
            chkAudioMP4Compt.Checked = false;
            chkSubHard.Checked = false;
            chkAdvTrim.Checked = false;
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
                    PrintLog(String.Format(i18n.UI.Logs["ErrorInfo"], "DisplayProperties_Video()", ex.Message));
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
                    PrintLog(String.Format(i18n.UI.Logs["ErrorInfo"], "DisplayProperties_Audio()", ex.Message));
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
                    PrintLog(String.Format(i18n.UI.Logs["ErrorInfo"], "DisplayProperties_Subtitle()", ex.Message));
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
                    PrintLog(String.Format(i18n.UI.Logs["ErrorInfo"], "DisplayProperties_Attachment()", ex.Message));
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

                cboProfile.Focus();
                cboProfile.SelectedIndex = cboProfile.Items.Count - 1;
            });
        }

        private void Thread_LoadPropertiesVideo(object obj)
        {
            Thread.Sleep(50);

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
            Thread.Sleep(50);

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
            Thread.Sleep(50);

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
            Thread.Sleep(50);

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
                        Path.GetFileName(item.FilePath),
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
            var newVideoCodec = new Dictionary<Guid, PluginsVideo>();
            var newAudioCodec = new Dictionary<Guid, PluginsAudio>();
            var idVideo = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
            var idAudio = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;

            foreach (var item in Plugins.Items.Video)
            {
                if (item.Key.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                {
                    if (imgSeq)
                        continue;
                }

                var exts = value;

                if (value.Contains(' '))
                    exts = value.Remove(value.IndexOf(' '));

                if (item.Value.Format.Contains(exts))
                {
                    newVideoCodec.Add(item.Key, item.Value);
                }
            }

            foreach (var item in Plugins.Items.Audio)
            {
                var exts = value;

                if (value.Contains(' '))
                    exts = value.Remove(value.IndexOf(' '));

                if (item.Value.Format.Contains(exts))
                {
                    newAudioCodec.Add(item.Key, item.Value);
                }
            }

            // Compare current and new dict array, if hash not same then load new dict and use default
            if (newVideoCodec.Count > 0)
            {
                var newDataSource = newVideoCodec.ToDictionary(p => p.Key, p => p.Value.Name);

                var currentDataSource = cboVideoEncoder.DataSource as BindingSource;
                var currentDataSourceDict = currentDataSource?.DataSource as Dictionary<Guid, string>;

                var newDataSourceJson = JsonConvert.SerializeObject(newDataSource);
                var currentDataSourceJson = currentDataSourceDict != null ? JsonConvert.SerializeObject(currentDataSourceDict) : string.Empty;

                var newDataSourceHash = OS.ComputeSHA256(newDataSourceJson);
                var currentDataSourceHash = OS.ComputeSHA256(currentDataSourceJson);

                if (newDataSourceHash != currentDataSourceHash)
                {
                    cboVideoEncoder.DataSource = new BindingSource(newDataSource, null);
                    cboVideoEncoder.DisplayMember = "Value";
                    cboVideoEncoder.ValueMember = "Key";
                }

                if (newDataSource.ContainsKey(idVideo))
                {
                    cboVideoEncoder.SelectedValue = idVideo;
                }
                else
                {
                    cboVideoEncoder.SelectedIndex = cboVideoEncoder.Items.Count - 1;
                }
            }
            else
            {
                cboVideoEncoder.DataSource = null;
            }

            if (newAudioCodec.Count > 0)
            {
                var newDataSource = newAudioCodec.ToDictionary(p => p.Key, p => p.Value.Name);

                var currentDataSource = cboAudioEncoder.DataSource as BindingSource;
                var currentDataSourceDict = currentDataSource?.DataSource as Dictionary<Guid, string>;

                var newDataSourceJson = JsonConvert.SerializeObject(newDataSource);
                var currentDataSourceJson = currentDataSourceDict != null ? JsonConvert.SerializeObject(currentDataSourceDict) : string.Empty;

                var newDataSourceHash = OS.ComputeSHA256(newDataSourceJson);
                var currentDataSourceHash = OS.ComputeSHA256(currentDataSourceJson);

                if (newDataSourceHash != currentDataSourceHash)
                {
                    cboAudioEncoder.DataSource = new BindingSource(newDataSource, null);
                    cboAudioEncoder.DisplayMember = "Value";
                    cboAudioEncoder.ValueMember = "Key";
                }

                if (newDataSource.ContainsKey(idAudio))
                {
                    cboAudioEncoder.SelectedValue = idAudio;
                }
                else
                {
                    cboAudioEncoder.SelectedIndex = cboAudioEncoder.Items.Count - 1;
                }
            }
            else
            {
                cboAudioEncoder.DataSource = null;
            }
        }

        private void SetProfileData(Profiles value)
        {
            cboFormat.SelectedIndex = (int)value.Container;

            cboVideoEncoder.SelectedIndex = -1;
            cboAudioEncoder.SelectedIndex = -1;

            cboVideoEncoder.SelectedValue = value.Video.Encoder.Id;
            cboVideoPreset.SelectedItem = value.Video.Encoder.Preset;
            cboVideoTune.SelectedItem = value.Video.Encoder.Tune;
            cboVideoRateControl.SelectedIndex = value.Video.Encoder.Mode;

            nudVideoRateFactor.DecimalPlaces = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.DecimalPlace;
            nudVideoRateFactor.Minimum = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.Min;
            nudVideoRateFactor.Maximum = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.Max;
            nudVideoRateFactor.Value = nudVideoRateFactor.DecimalPlaces > 0 ? value.Video.Encoder.Value : Math.Ceiling(value.Video.Encoder.Value);
            nudVideoRateFactor.Increment = Plugins.Items.Video[value.Video.Encoder.Id].Video.Mode[value.Video.Encoder.Mode].Value.Step;

            nudVideoMultiPass.Value = value.Video.Encoder.MultiPass;

            cboVideoRes.Text = $"{(value.Video.Quality.Width == 0 || value.Video.Quality.Height == 0 ? "auto" : $"{value.Video.Quality.Width}x{value.Video.Quality.Height}")}";
            cboVideoFps.Text = $"{Math.Round(value.Video.Quality.FrameRate, 3)}";
            cboVideoBitDepth.Text = $"{value.Video.Quality.BitDepth}";
            cboVideoPixFmt.Text = $"{value.Video.Quality.PixelFormat}";

            chkVideoDeInterlace.Checked = value.Video.DeInterlace.Enable;
            cboVideoDeInterMode.SelectedIndex = value.Video.DeInterlace.Mode;
            cboVideoDeInterField.SelectedIndex = value.Video.DeInterlace.Field;

            cboAudioEncoder.SelectedValue = value.Audio.Encoder.Id;
            cboAudioMode.SelectedIndex = value.Audio.Encoder.Mode;
            cboAudioQuality.Text = value.Audio.Encoder.Quality;
            cboAudioSampleRate.SelectedValue = value.Audio.Encoder.SampleRate;
            cboAudioChannel.SelectedValue = value.Audio.Encoder.Channel;

            chkVideoMP4Compt.Checked = value.TryRemuxVideo;
            chkAudioMP4Compt.Checked = value.TryRemuxAudio;

            // Make sure ComboBox are not in -1 Index
            Control ctrl = this;
            do
            {
                ctrl = GetNextControl(ctrl, true);

                if (ctrl != null)
                {
                    if (ctrl is ComboBox comboBox)
                    {
                        if (comboBox.SelectedIndex == -1 && comboBox.Items.Count > 0)
                        {
                            comboBox.SelectedIndex = comboBox.Items.Count - 1;
                        }
                    }
                }
            } while (ctrl != null);

            // Modify selected item data
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
                        media.Video[i].Encoder.Id = ((KeyValuePair<Guid, string>)cboVideoEncoder.SelectedItem).Key;
                        media.Video[i].Encoder.Preset = cboVideoPreset.Text;
                        media.Video[i].Encoder.Tune = cboVideoTune.Text;
                        media.Video[i].Encoder.Mode = cboVideoRateControl.SelectedIndex;
                        media.Video[i].Encoder.Value = nudVideoRateFactor.Value;
                        media.Video[i].Encoder.MultiPass = (int)nudVideoMultiPass.Value;

                        media.Video[i].Quality.Width = value.Video.Quality.Width == 0 ? media.Video[i].Info.Width : value.Video.Quality.Width;
                        media.Video[i].Quality.Height = value.Video.Quality.Height == 0 ? media.Video[i].Info.Height : value.Video.Quality.Height;
                        media.Video[i].Quality.FrameRate = float.TryParse(cboVideoFps.Text, out float fps) ? fps : media.Video[i].Info.FrameRate;
                        media.Video[i].Quality.FrameRateAvg = float.TryParse(cboVideoFps.Text, out float fpa) ? fpa : media.Video[i].Info.FrameRateAvg;
                        media.Video[i].Quality.BitDepth = int.TryParse(cboVideoBitDepth.Text, out int bpc) ? bpc : 8;
                        media.Video[i].Quality.PixelFormat = int.TryParse(cboVideoPixFmt.Text, out int pix) ? pix : 420;

                        media.Video[i].DeInterlace.Enable = chkVideoDeInterlace.Checked;
                        media.Video[i].DeInterlace.Mode = cboVideoDeInterMode.SelectedIndex;
                        media.Video[i].DeInterlace.Field = cboVideoDeInterField.SelectedIndex;
                    }

                    // modify audio collection
                    for (int x = 0; x < media.Audio.Count; x++)
                    {
                        media.Audio[x].Encoder.Id = ((KeyValuePair<Guid, string>)cboAudioEncoder.SelectedItem).Key;
                        media.Audio[x].Encoder.Mode = cboAudioMode.SelectedIndex;
                        media.Audio[x].Encoder.Quality = cboAudioQuality.Text;
                        media.Audio[x].Encoder.SampleRate = ((KeyValuePair<int, string>)cboAudioSampleRate.SelectedItem).Key;
                        media.Audio[x].Encoder.Channel = ((KeyValuePair<int, string>)cboAudioChannel.SelectedItem).Key;
                    }

                    // update main list
                    var inExt = Path.GetExtension((queue.Tag as MediaQueue).FilePath).Substring(1).ToUpperInvariant();
                    var outExt = Enum.GetName(typeof(MediaContainer), cboFormat.SelectedIndex);
                    queue.SubItems[1].Text = $"{inExt} ► {outExt}";
                }

                DisplayProperties_Video();
                DisplayProperties_Audio();
                DisplayProperties_Subtitle();
                DisplayProperties_Attachment();
            }
        }
    }
}
