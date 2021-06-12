using IFME.OSManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    [Flags]
    public enum MediaType
    {
        Video = 1,
        Audio = 2,
        Image = 4,
        Subtitle = 8,
        Attachment = 16
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

            btnFileAdd.Text = Fonts.fa.plus;
            btnFileDelete.Text = Fonts.fa.minus;
            btnOptions.Text = $"{Fonts.fa.gears} {btnOptions.Text}";
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
                $"(c) {DateTime.Now.Year} {Version.TradeMark}\r\n" +
                "\r\n" +
                "Warning, DO NOT close this Terminal/Console, all useful info will be shown here.\r\n" +
                "\r\n";
        }

        private void InitializeProfiles()
        {
            new ProfilesManager();

            cboProfile.Items.Clear();
            foreach (var item in Profiles.Items)
                cboProfile.Items.Add(item.ProfileName);

            cboProfile.SelectedIndex = cboProfile.Items.Count - 1;
        }

        private string[] OpenFiles(MediaType type, bool multiSelect = true)
        {
            var extsVideo = "All video types|*.mkv;*.mp4;*.m4v;*.ts;*.mts;*.m2ts;*.flv;*.webm;*.ogv;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.avs|";
            var extsAudio = "All audio types|*.mp2;*.mp3;*.mp4;*.m4a;*.aac;*.ogg;*.opus;*.flac;*.wav|";
            var extsImg = "All image types|*.bmp;*.png;*.jpg;*.jpe;*.jpeg;*.tiff;*.gif|";
            var extsSub = "All subtitle types|*.ssa;*.ass;*.srt|";
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

        private void MediaFileListAdd(string path, string frameRate = "")
        {
            var fileData = new FFmpeg.MediaInfo(path, frameRate);
            var fileQueue = new MediaQueue()
            {
                Enable = true,
                FilePath = path,
                FileSize = fileData.FileSize,
                Duration = fileData.Duration,
                InputFormat = fileData.FormatNameFull,
                OutputFormat = MediaContainer.MKV,
                Info = fileData
            };

            foreach (var item in fileData.Video)
                fileQueue.Video.Add(MediaQueueParse.Video(path, item));

            foreach (var item in fileData.Audio)
                fileQueue.Audio.Add(MediaQueueParse.Audio(path, item));	

            foreach (var item in fileData.Subtitle)
                fileQueue.Subtitle.Add(MediaQueueParse.Subtitle(path, item));
            
            foreach (var item in fileData.Attachment)
                fileQueue.Attachment.Add(MediaQueueParse.Attachment(path, item));

            lstFile.Items.Add(new ListViewItem(new[]
            {
                Path.GetFileName(path),
                Path.GetExtension(path).Substring(1).ToUpperInvariant(),
                TimeSpan.FromSeconds(fileData.Duration).ToString("hh\\:mm\\:ss"),
                OS.PrintFileSize(fileData.FileSize),
                fileQueue.Enable ? "Ready" : "Done",
                ""
            })
            {
                Tag = fileQueue,
                Checked = true,
                Selected = true
            });			
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

            MediaShowDataVideoRe();
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

            MediaShowDataAudioRe();
        }

        private void MediaSubtitleListAdd(string path)
        {
            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                (lst.Tag as MediaQueue).Subtitle.Add(new MediaQueueSubtitle
                {
                    Enable = true,
                    File = path,
                    Id = -1,
                    Lang = "und",
                    Codec = string.Empty
                });
            }

            MediaShowDataSubtitleRe();
        }

        private void MediaSubtitleListAddEmbed(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                foreach (var item in fileData.Subtitle)
                {
                    (lst.Tag as MediaQueue).Subtitle.Add(MediaQueueParse.Subtitle(path, item));
                }
            }

            MediaShowDataSubtitleRe();
        }

        private void MediaAttachmentListAdd(string path)
        {
            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                (lst.Tag as MediaQueue).Attachment.Add(new MediaQueueAttachment
                {
                    Enable = true,
                    File = path,
                    Id = -1,
                    Name = Path.GetFileName(path),
                    Mime = Mime.GetType(path)
                });
            }

            MediaShowDataAttachmentRe();
        }

        private void MediaAttachmentListAddEmbed(string path)
        {
            var fileData = new FFmpeg.MediaInfo(path);

            foreach (ListViewItem lst in lstFile.SelectedItems)
            {
                foreach (var item in fileData.Attachment)
                {
                    (lst.Tag as MediaQueue).Attachment.Add(MediaQueueParse.Attachment(path, item));
                }
            }
        }

        private void MediaShowDataVideoRe()
        {
            if (lstVideo.SelectedItems.Count > 0)
            {
                var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[lstVideo.SelectedItems[0].Index];
                new Thread(MediaShowDataVideo).Start(data);
            }
        }

        private void MediaShowDataAudioRe()
        {
            if (lstAudio.SelectedItems.Count > 0)
            {
                var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[lstAudio.SelectedItems[0].Index];
                new Thread(MediaShowDataAudio).Start(data);
            }
        }

        private void MediaShowDataSubtitleRe()
        {
            if (lstSub.SelectedItems.Count > 0)
            {
                var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle[lstSub.SelectedItems[0].Index];
                new Thread(MediaShowDataSubtitle).Start(data);
            }
        }

        private void MediaShowDataAttachmentRe()
        {

        }

        private void MediaShowDataVideo(object obj)
        {
            var data = obj as MediaQueueVideo;

            BeginInvoke((Action)delegate ()
            {
                cboVideoLang.SelectedValue = data.Lang;
                
                cboVideoEncoder.SelectedValue = data.Encoder.Id;
                cboVideoPreset.SelectedItem = data.Encoder.Preset;
                cboVideoTune.SelectedItem = data.Encoder.Tune;
                cboVideoRateControl.SelectedIndex = data.Encoder.Mode;

                nudVideoRateFactor.Value = data.Encoder.Value;
                nudVideoMultiPass.Value = data.Encoder.MultiPass;

                cboVideoRes.Text = $"{data.Quality.Width}x{data.Quality.Height}";
                cboVideoFps.Text = $"{Math.Round(data.Quality.FrameRate, 3)}";
                cboVideoBitDepth.Text = $"{data.Quality.BitDepth}";
                cboVideoPixFmt.Text = $"{data.Quality.PixelFormat}";

                chkVideoDeInterlace.Checked = data.DeInterlace.Enable;
                cboVideoDeInterMode.SelectedIndex = data.DeInterlace.Mode;
                cboVideoDeInterField.SelectedIndex = data.DeInterlace.Field;
            });
        }

        private void MediaShowDataAudio(object obj)
        {
            var data = obj as MediaQueueAudio;

            BeginInvoke((Action)delegate ()
            {
                cboAudioLang.SelectedValue = data.Lang;
                chkAudioCopy.Checked = data.Copy;
                cboAudioEncoder.SelectedValue = data.Encoder.Id;
                cboAudioMode.SelectedIndex = data.Encoder.Mode;
            });

            Thread.Sleep(1);

            BeginInvoke((Action)delegate ()
            {
                cboAudioQuality.SelectedItem = data.Encoder.Quality;
                cboAudioSampleRate.SelectedItem = data.Encoder.SampleRate;
                cboAudioChannel.SelectedValue = data.Encoder.Channel;
            });
        }

        private void MediaShowDataSubtitle(object obj)
        {
            var data = obj as MediaQueueSubtitle;

            BeginInvoke((Action)delegate ()
            {
                cboSubLang.SelectedValue = data.Lang;
            });
        }

        private void MediaShowDataAttachment(object obj)
        {

        }

        private void ShowSupportedCodec(string value)
        {
            var videoCodec = new Dictionary<Guid, PluginsVideo>();
            var audioCodec = new Dictionary<Guid, PluginsAudio>();

            var videoId = new Guid();
            var audioId = new Guid();

            foreach (var item in Plugins.Items.Video)
            {
                if (item.Value.Format.Contains(value))
                {
                    videoCodec.Add(item.Key, item.Value);
                    videoId = item.Key;
                }
            }

            foreach (var item in Plugins.Items.Audio)
            {
                if (item.Value.Format.Contains(value))
                {
                    audioCodec.Add(item.Key, item.Value);
                    audioId = item.Key;
                }
            }

            cboVideoEncoder.DataSource = new BindingSource(videoCodec.ToDictionary(p => p.Key, p => p.Value.Name), null);
            cboVideoEncoder.DisplayMember = "Value";
            cboVideoEncoder.ValueMember = "Key";

            cboAudioEncoder.DataSource = new BindingSource(audioCodec.ToDictionary(p => p.Key, p => p.Value.Name), null);
            cboAudioEncoder.DisplayMember = "Value";
            cboAudioEncoder.ValueMember = "Key";

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
                        item.Encoder = new MediaQueueVideoEncoder
                        {
                            Id = vData.GUID,
                            Preset = vData.Video.PresetDefault,
                            Tune = vData.Video.TuneDefault,
                            Mode = 0,
                            Value = vData.Video.Mode[0].Value.Default,
                            MultiPass = 2,
                            Command = string.Empty
                        };
                        item.Quality.Command = string.Empty;
                    }

                    foreach (var item in (queue.Tag as MediaQueue).Audio)
                    {
                        item.Encoder = new MediaQueueAudioEncoder
                        {
                            Id = aData.GUID,
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

            MediaShowDataVideoRe();
            MediaShowDataAudioRe();
            MediaShowDataSubtitleRe();
            MediaShowDataAttachmentRe();
        }

        private void SetProfileData(Profiles value)
        {
            if (lstFile.SelectedItems.Count > 0)
            {
                foreach (ListViewItem queue in lstFile.SelectedItems)
                {
                    (queue.Tag as MediaQueue).OutputFormat = value.Container;
                    (queue.Tag as MediaQueue).ProfileId = cboProfile.SelectedIndex;

                    foreach (var item in (queue.Tag as MediaQueue).Video)
                    {
                        item.Encoder = value.Video.Encoder;

                        if (value.Video.Quality.Width > 0) { item.Quality.Width = value.Video.Quality.Width; }
                        if (value.Video.Quality.Height > 0) { item.Quality.Height = value.Video.Quality.Height; }
                        if (value.Video.Quality.FrameRate > 0) { item.Quality.FrameRate = value.Video.Quality.FrameRate; }
                        //item.Quality.FrameRateAvg = value.Video.Quality.FrameRateAvg;
                        //item.Quality.FrameCount = value.Video.Quality.FrameCount;
                        //item.Quality.IsVFR = value.Video.Quality.IsVFR;
                        item.Quality.BitDepth = value.Video.Quality.BitDepth;
                        item.Quality.PixelFormat = value.Video.Quality.PixelFormat;
                        item.Quality.Command = value.Video.Quality.Command;
                        item.Quality.CommandFilter = value.Video.Quality.CommandFilter;

                        item.DeInterlace = value.Video.DeInterlace;
                    }

                    foreach (var item in (queue.Tag as MediaQueue).Audio)
                    {
                        item.Copy = value.Audio.Copy;
                        item.Encoder = value.Audio.Encoder;
                        item.Command = value.Audio.Command;
                        item.CommandFilter = value.Audio.CommandFilter;
                    }
                }

                MediaShowDataVideoRe();
                MediaShowDataAudioRe();
                MediaShowDataSubtitleRe();
                MediaShowDataAttachmentRe();
            }
        }
    }
}
