using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using Newtonsoft.Json;

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
        // Guards against re-entrancy: assigning SelectedIndex/SelectedValue below raises
        // SelectedIndexChanged synchronously, and some of those handlers call back into
        // DisplayProperties_*.
        private bool suspendPropertyEvents;

        /// <summary>
        /// Queues a property-panel refresh to run once the current event chain has unwound.
        ///
        /// This deferral is load bearing, not cosmetic. cboVideoEncoder_SelectedIndexChanged
        /// assigns cboVideoBitDepth / cboVideoPixFmt / cboVideoRes part way through its own
        /// body, and each of those handlers calls back here. Running the refresh inline
        /// re-enters the encoder handler and leaves the combo mid-rebind, so the outer frame
        /// then reads a null SelectedValue and throws.
        ///
        /// The original code got the same ordering from a thread that slept 50 ms before
        /// posting. Posting directly costs one message-loop turn instead.
        /// </summary>
        private void DeferPropertyLoad(Action load)
        {
            if (!IsHandleCreated || IsDisposed)
                return;

            BeginInvoke(load);
        }

        private void Initialize_i18n()
        {
            var lang = Properties.Settings.Default.UILanguage;
            i18n.Apply(this, Name, lang);

            var a = cboVideoDeInterMode.SelectedIndex;
            cboVideoDeInterMode.DataSource = new BindingSource(i18nUI.Obj.DeInterlaceMode, null);
            cboVideoDeInterMode.DisplayMember = "Value";
            cboVideoDeInterMode.ValueMember = "Key";
            cboVideoDeInterMode.SelectedIndex = a;

            var b = cboVideoDeInterField.SelectedIndex;
            cboVideoDeInterField.DataSource = new BindingSource(i18nUI.Obj.DeInterlaceField, null);
            cboVideoDeInterField.DisplayMember = "Value";
            cboVideoDeInterField.ValueMember = "Key";
            cboVideoDeInterField.SelectedIndex = b;

            PrintLog($"[i18n] {lang} @ {i18n.GetLangAuthor(lang)[0]}");
        }

        private void InitializeFonts()
        {
            btnStop.Font = new Font(btnStop.Font.FontFamily, 14f);
            btnStart.Font = new Font(btnStart.Font.FontFamily, 10f);
            txtMediaInfo.Font = Fonts.Monospace();
            rtfConsole.Font = Fonts.Monospace();
            cboFormat.Font = Fonts.Monospace();
            cboProfile.Font = cboFormat.Font;
            txtOutputPath.Font = cboFormat.Font;
        }

        private void InitializeLog()
        {
            var old = rtfConsole.Text;

            rtfConsole.Text = $"{Version.Title} {Version.Release} ( '{Version.CodeName}' )\n" +
                $"{CPU.GetCpuBrandString}, {CPU.GetSIMDFlags}, {(CPU.HasAVX ? "AVX" : "-")}, {(CPU.HasAVX2 ? "AVX2" : "-")}, {(CPU.HasFMA3 ? "FMA3" : "-")}\r\n" +
                "\r\n" +
                $"(c) {DateTime.Now.Year} {Version.TradeMark}\r\n\r\n(s) {Version.Contrib}\r\n" +
                "\r\n" +
                $"{i18nUI.Log("WarningInfo")}\r\n" +
                "\r\n";

            if (string.IsNullOrEmpty(PluginsLoad.ErrorLog))
                rtfConsole.AppendText(i18nUI.Log("PluginLoadOK"));
            else
                rtfConsole.AppendText(PluginsLoad.ErrorLog);

            rtfConsole.AppendText(old);
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
            InitializeTabLayout();
        }

        private void UpdateComboBoxItems(ComboBox combo, string[] newItems, ComboBoxStyle style)
        {
            // Null safety
            newItems ??= Array.Empty<string>();

            // Check if items differ (same order and content)
            bool isDifferent = combo.Items.Count != newItems.Length ||
                               !combo.Items.Cast<string>().SequenceEqual(newItems);

            if (isDifferent)
            {
                combo.BeginUpdate();
                combo.Items.Clear();
                combo.Items.AddRange(newItems);
                combo.EndUpdate();
            }

            if (combo.DropDownStyle != style)
                combo.DropDownStyle = style;
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
                if ((FileContainer)cboFormat.SelectedIndex == FileContainer.MP4)
                    exts = extsSub2;
                else
                    exts += extsSub;
            }

            if (type.HasFlag(MediaType.Attachment))
            {
                exts += extsAtt;
            }

            // RestoreDirectory keeps the process working directory stable, so relative
            // plugin lookups are not affected by wherever the user browsed to.
            using (var ofd = new OpenFileDialog
            {
                Filter = exts += "All types|*.*",
                FilterIndex = 1,
                Multiselect = multiSelect,
                RestoreDirectory = true
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                    return ofd.FileNames;
            }

            return Array.Empty<string>();
        }

        /// <summary>
        /// Snapshot of the encoder defaults currently shown in the UI. Taken once on the UI
        /// thread so queue construction can run on a worker without touching controls.
        /// </summary>
        private sealed class QueueDefaults
        {
            public FileContainer OutputFormat;
            public int ProfileId;

            public Guid VideoEncoderId;
            public string VideoPreset;
            public string VideoTune;
            public int VideoRateControl;
            public decimal VideoRateFactor;
            public int VideoMultiPass;

            public bool DeInterlaceEnable;
            public int DeInterlaceMode;
            public int DeInterlaceField;

            public Guid AudioEncoderId;
            public int AudioMode;
            public string AudioQuality;
            public int AudioSampleRate;
            public int AudioChannel;
        }

        /// <summary>
        /// Reads the plugin GUID out of a data-bound encoder combo. Returns null when the
        /// combo is unbound, which happens whenever the chosen container supports no codec
        /// of that kind: ShowSupportedCodec sets DataSource to null and SelectedItem with it.
        /// Unboxing that null throws NullReferenceException, not InvalidCastException.
        /// </summary>
        private static Guid? SelectedPluginId(ComboBox combo)
        {
            return combo.SelectedItem is KeyValuePair<Guid, string> kv ? kv.Key : (Guid?)null;
        }

        private static int SelectedIntKey(ComboBox combo, int fallback = 0)
        {
            return combo.SelectedItem is KeyValuePair<int, string> kv ? kv.Key : fallback;
        }

        private QueueDefaults CaptureQueueDefaults()
        {
            return new QueueDefaults
            {
                OutputFormat = (FileContainer)cboFormat.SelectedIndex,
                ProfileId = cboProfile.SelectedIndex,

                // Falls back to the copy-stream codec when the combo is unbound. That happens
                // on audio-only containers, where no video stream is encoded anyway.
                VideoEncoderId = SelectedPluginId(cboVideoEncoder) ?? Guid.Empty,
                VideoPreset = cboVideoPreset.Text,
                VideoTune = cboVideoTune.Text,
                VideoRateControl = cboVideoRateControl.SelectedIndex,
                VideoRateFactor = nudVideoRateFactor.Value,
                VideoMultiPass = (int)nudVideoMultiPass.Value,

                DeInterlaceEnable = chkVideoDeInterlace.Checked,
                DeInterlaceMode = cboVideoDeInterMode.SelectedIndex,
                DeInterlaceField = cboVideoDeInterField.SelectedIndex,

                AudioEncoderId = SelectedPluginId(cboAudioEncoder) ?? Guid.Empty,
                AudioMode = cboAudioMode.SelectedIndex,
                AudioQuality = cboAudioQuality.Text,
                AudioSampleRate = SelectedIntKey(cboAudioSampleRate),
                AudioChannel = SelectedIntKey(cboAudioChannel)
            };
        }

        private void ImportFiles(string[] files)
        {
            if (files == null || files.Length == 0)
                return;

            lstFile.SelectedIndices.Clear();

            var defaults = CaptureQueueDefaults();

            var frm = new frmProgressBar();
            frm.Show(this);
            frm.Text = i18nUI.Dialog("Importing");
            frm.Status = i18nUI.Dialog("Indexing");

            var thread = new BackgroundWorker { WorkerReportsProgress = true };

            thread.DoWork += delegate (object o, DoWorkEventArgs r)
            {
                var worker = (BackgroundWorker)o;

                for (int i = 0; i < files.Length; i++)
                {
                    // Abandon the import if the window went away mid-run.
                    if (IsDisposed || !IsHandleCreated)
                        return;

                    var path = files[i];

                    // The expensive part (ffprobe via FFmpeg.MediaInfo) genuinely runs here,
                    // off the UI thread. Only the ListViewItem insert is marshalled back.
                    try
                    {
                        var item = BuildMediaQueue(path, false, string.Empty, defaults);

                        Invoke(new MethodInvoker(delegate
                        {
                            MediaFileListInsert(item, path, defaults);
                        }));
                    }
                    catch (Exception ex)
                    {
                        // Report and carry on: one unreadable file must not abort the import.
                        PrintLog(string.Format(i18nUI.Log("ErrorInfo"), Path.GetFileName(path), ex.Message));
                    }

                    worker.ReportProgress((int)(((float)(i + 1) / files.Length) * 100.0),
                        new object[] { i + 1, files.Length, path });
                }
            };

            thread.ProgressChanged += delegate (object o, ProgressChangedEventArgs r)
            {
                var state = (object[])r.UserState;

                frm.Progress = r.ProgressPercentage;
                frm.Status = string.Format(i18nUI.Dialog("ImportStatus"), state[0], state[1], state[2]);
                frm.Title = string.Format(i18nUI.Dialog("ImportTitle"), r.ProgressPercentage);
            };

            thread.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs r)
            {
                if (r.Error != null)
                    PrintLog(string.Format(i18nUI.Log("ErrorInfo"), "ImportFiles()", r.Error.Message));

                frm.Close();
                frm.Dispose();

                thread.Dispose();
            };

            thread.RunWorkerAsync();
        }

        private void MediaFileListAdd(string path, bool isImages, string frameRate = "")
        {
            var defaults = CaptureQueueDefaults();
            MediaFileListInsert(BuildMediaQueue(path, isImages, frameRate, defaults), path, defaults);
        }

        /// <summary>
        /// Builds the queue entry for a file. Contains no UI access and is safe to call
        /// from a worker thread.
        /// </summary>
        private static MediaQueue BuildMediaQueue(string path, bool isImages, string frameRate, QueueDefaults ui)
        {
            var fileData = new FFmpeg.MediaInfo(path, frameRate);
            var fileQueue = new MediaQueue()
            {
                Enable = true,

                FilePath = path,
                FileSize = fileData.FileSize,
                Duration = fileData.Duration,
                InputFormat = fileData.FormatNameFull,
                OutputFormat = ui.OutputFormat,
                ProfileId = ui.ProfileId,
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
                        Id = ui.VideoEncoderId,
                        Preset = ui.VideoPreset,
                        Tune = ui.VideoTune,
                        Mode = ui.VideoRateControl,
                        Value = ui.VideoRateFactor,
                        MultiPass = ui.VideoMultiPass,
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
                        Enable = ui.DeInterlaceEnable,
                        Mode = ui.DeInterlaceMode,
                        Field = ui.DeInterlaceField
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
                    Default = item.Default,

                    Info = new MediaQueueAudioInfo
                    {
                        SampleRate = item.SampleRate,
                        Channel = item.Channel
                    },

                    Encoder = new MediaQueueAudioEncoder
                    {
                        Id = ui.AudioEncoderId,
                        Mode = ui.AudioMode,
                        Quality = ui.AudioQuality,
                        SampleRate = ui.AudioSampleRate,
                        Channel = ui.AudioChannel,
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
                    Default = item.Default,
                    Forced = item.Forced,
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
            if (ui.OutputFormat == FileContainer.MKV)
                fileQueue.HardSub = false;
            else if (ui.OutputFormat == FileContainer.MP4)
                fileQueue.HardSub = false;
            else
                fileQueue.HardSub = true;

            return fileQueue;
        }

        /// <summary>
        /// Adds a prepared queue entry to the list. UI thread only.
        /// </summary>
        private void MediaFileListInsert(MediaQueue fileQueue, string path, QueueDefaults ui)
        {
            if (fileQueue == null)
                return;

            var ext = Path.GetExtension(path);
            var extName = string.IsNullOrEmpty(ext) ? "?" : ext.Substring(1).ToUpperInvariant();

            var lst = new ListViewItem(new[]
            {
                Path.GetFileName(path),
                $"{extName} ► {Enum.GetName(typeof(FileContainer), ui.OutputFormat)}",
                TimeSpan.FromSeconds(fileQueue.Duration).ToString("hh\\:mm\\:ss"),
                OS.PrintFileSize(fileQueue.FileSize),
                fileQueue.Enable ? "Ready" : "Done",
                ""
            })
            {
                Tag = fileQueue,
                Checked = true,
                Selected = true
            };

            lstFile.Focus();
            lstFile.Items.Add(lst);
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
            txtMediaInfo.Text = i18nUI.Dialog("MediaInfo");
            chkVideoDeInterlace.Checked = false;
            chkVideoMP4Compt.Checked = false;
            chkAudioMP4Compt.Checked = false;
            chkSubHard.Checked = false;
            chkAdvTrim.Checked = false;
        }

        private void DisplayProperties_Video()
        {
            if (suspendPropertyEvents)
                return;

            if ((FileContainer)cboFormat.SelectedIndex >= FileContainer.MP2)
                return;

            if (lstVideo.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Video[lstVideo.SelectedItems[0].Index];
                    DeferPropertyLoad(() => LoadPropertiesVideo(data));
                }
                catch (Exception ex)
                {
                    PrintLog(string.Format(i18nUI.Log("ErrorInfo"), "DisplayProperties_Video()", ex.Message));
                }
            }
        }

        private void DisplayProperties_Audio()
        {
            if (suspendPropertyEvents)
                return;

            if (lstAudio.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Audio[lstAudio.SelectedItems[0].Index];
                    DeferPropertyLoad(() => LoadPropertiesAudio(data));
                }
                catch (Exception ex)
                {
                    PrintLog(string.Format(i18nUI.Log("ErrorInfo"), "DisplayProperties_Audio()", ex.Message));
                }
            }
        }

        private void DisplayProperties_Subtitle()
        {
            if (suspendPropertyEvents)
                return;

            if (lstSub.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Subtitle[lstSub.SelectedItems[0].Index];
                    DeferPropertyLoad(() => LoadPropertiesSubtitle(data));
                }
                catch (Exception ex)
                {
                    PrintLog(string.Format(i18nUI.Log("ErrorInfo"), "DisplayProperties_Subtitle()", ex.Message));
                }
            }
        }

        private void DisplayProperties_Attachment()
        {
            if (suspendPropertyEvents)
                return;

            if (lstAttach.SelectedItems.Count > 0)
            {
                try
                {
                    var data = (lstFile.SelectedItems[0].Tag as MediaQueue).Attachment[lstAttach.SelectedItems[0].Index];
                    DeferPropertyLoad(() => LoadPropertiesAttachment(data));
                }
                catch (Exception ex)
                {
                    PrintLog(string.Format(i18nUI.Log("ErrorInfo"), "DisplayProperties_Attachment()", ex.Message));
                }
            }
        }

        private void InitializeTabLayout()
        {
            // Force each tab page to build its control handles up front so the first
            // real switch is instant. SelectedTab alone defers layout, so lay out explicitly
            // instead of sleeping and hoping GDI+ caught up.
            tabConfig.SuspendLayout();

            foreach (TabPage item in tabConfig.TabPages)
            {
                tabConfig.SelectedTab = item;
                item.PerformLayout();
            }

            tabConfig.SelectedTab = tabConfigLog;
            tabConfig.ResumeLayout(true);

            cboProfile.Focus();

            if (OS.IsLinux)
            {
                cboProfile.SelectedIndex = -1; // no profile for Linux
                cboVideoEncoder.SelectedValue = new Guid("deadbeef-0011-0011-0011-001100110011");
                cboAudioEncoder.SelectedValue = new Guid("deadface-f00d-f00d-f00d-f00df00df00d");
            }
            else
            {
                cboProfile.SelectedIndex = cboProfile.Items.Count - 1;
            }
        }

        private void LoadPropertiesVideo(MediaQueueVideo data)
        {
            if (data == null)
                return;

            suspendPropertyEvents = true;

            try
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

                cboVideoRes.TextChanged -= cboVideoRes_TextChanged;
                cboVideoRes.Text = $"{(data.Quality.Width == 0 || data.Quality.Height == 0 ? "auto" : $"{data.Quality.Width}x{data.Quality.Height}" )}";
                cboVideoRes.TextChanged += cboVideoRes_TextChanged;

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
            }
            finally
            {
                suspendPropertyEvents = false;
            }
        }

        private void LoadPropertiesAudio(MediaQueueAudio data)
        {
            if (data == null)
                return;

            suspendPropertyEvents = true;

            try
            {
                cboAudioLang.SelectedValue = data.Lang;
                cboAudioEncoder.SelectedValue = data.Encoder.Id;

                // Raises cboAudioMode_SelectedIndexChanged synchronously, which repopulates
                // cboAudioQuality. The quality/sample/channel assignments below therefore
                // run against the refreshed item list without needing a sleep in between.
                cboAudioMode.SelectedIndex = data.Encoder.Mode;

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
            }
            finally
            {
                suspendPropertyEvents = false;
            }
        }

        private void LoadPropertiesSubtitle(MediaQueueSubtitle data)
        {
            if (data == null)
                return;

            suspendPropertyEvents = true;

            try
            {
                cboSubLang.SelectedValue = data.Lang;

                foreach (ListViewItem item in lstSub.SelectedItems)
                {
                    item.SubItems[2].Text = Language.FullName(data.Lang);
                }
            }
            finally
            {
                suspendPropertyEvents = false;
            }
        }

        private void LoadPropertiesAttachment(MediaQueueAttachment data)
        {
            if (data == null)
                return;

            suspendPropertyEvents = true;

            try
            {
                foreach (ListViewItem item in lstAttach.SelectedItems)
                {
                    item.SubItems[2].Text = data.Mime;
                }

                cboAttachMime.Text = data.Mime;
            }
            finally
            {
                suspendPropertyEvents = false;
            }
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

            // SelectedItem is null whenever the combo was unbound by a previous call, which
            // happens as soon as the user picks an audio-only container: no video codec
            // matches, so DataSource is set to null below. Unboxing that null threw
            // NullReferenceException on the next container change.
            // Nullable rather than Guid.Empty, because Guid.Empty is a real plugin key
            // (the copy-stream codec).
            Guid? idVideo = cboVideoEncoder.SelectedItem is KeyValuePair<Guid, string> curVideo
                ? curVideo.Key
                : (Guid?)null;

            Guid? idAudio = cboAudioEncoder.SelectedItem is KeyValuePair<Guid, string> curAudio
                ? curAudio.Key
                : (Guid?)null;

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
                    // Members first: assigning DataSource raises SelectedIndexChanged
                    // synchronously, and until ValueMember is applied SelectedValue returns
                    // null even though SelectedItem is already populated.
                    cboVideoEncoder.DisplayMember = "Value";
                    cboVideoEncoder.ValueMember = "Key";
                    cboVideoEncoder.DataSource = new BindingSource(newDataSource, null);
                }

                if (idVideo.HasValue && newDataSource.ContainsKey(idVideo.Value))
                {
                    cboVideoEncoder.SelectedValue = idVideo.Value;
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
                    cboAudioEncoder.DisplayMember = "Value";
                    cboAudioEncoder.ValueMember = "Key";
                    cboAudioEncoder.DataSource = new BindingSource(newDataSource, null);
                }

                if (idAudio.HasValue && newDataSource.ContainsKey(idAudio.Value))
                {
                    cboAudioEncoder.SelectedValue = idAudio.Value;
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
                    var outExt = Enum.GetName(typeof(FileContainer), cboFormat.SelectedIndex);
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
