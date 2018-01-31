using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace ifme
{
	partial class frmMain
	{
		enum ListViewItemType
		{
			Media,
			Video,
			Audio,
			Subtitle
		}

		enum Direction
		{
			Up,
			Down
		}

		// BackgroundWorker with Abort support
		// refer to BackgroundWorkerEx.cs file
		private BackgroundWorkerEx bgThread = new BackgroundWorkerEx();

		public void InitializeUX()
		{
            // Show Splash Screen (hold) & Loading everyting!
            var ss = new frmSplashScreen();
			ss.ShowDialog();

			// Load user settings
			txtFolderOutput.Text = Get.FolderSave;

			cboVideoResolution.Text = "1920x1080";
			cboVideoFrameRate.Text = "23.976";
			cboVideoPixelFormat.SelectedIndex = 0;
			cboVideoDeinterlaceMode.SelectedIndex = 1;
			cboVideoDeinterlaceField.SelectedIndex = 0;

            // Display Language ComboBox
            cboSubLang.DataSource = new BindingSource(Get.LanguageCode, null);
			cboSubLang.DisplayMember = "Value";
			cboSubLang.ValueMember = "Key";
			cboSubLang.SelectedValue = "und";

			cboVideoStreamLang.DataSource = new BindingSource(Get.LanguageCode, null);
			cboVideoStreamLang.DisplayMember = "Value";
			cboVideoStreamLang.ValueMember = "Key";
			cboVideoStreamLang.SelectedValue = "und";

			cboAudioStreamLang.DataSource = new BindingSource(Get.LanguageCode, null);
			cboAudioStreamLang.DisplayMember = "Value";
			cboAudioStreamLang.ValueMember = "Key";
			cboAudioStreamLang.SelectedValue = "und";

			// Display MIME ComboBox
			foreach (var item in Get.MimeTypeList)
				cboAttachMime.Items.Add(item);
			cboAttachMime.Text = "application/octet-stream";

            // Load Target Format
            cboTargetFormat.DataSource = new BindingSource(Get.TargetFormat, null);
            cboTargetFormat.DisplayMember = "Value";
            cboTargetFormat.ValueMember = "Key";
            cboTargetFormat.SelectedValue = "mkv";

			// Load everyting
			ApplyLanguage();
			ApplyPlugins();
			ApplyEncodingPreset();

            // OS feature stuff
            if (OS.IsLinux)
            {
                chkSubHard.Enabled = false;
            }

			// Draw
			DrawBanner();

            // Open project
            if (!string.IsNullOrEmpty(MediaProject.ProjectFile))
            {
                ProjectOpen(MediaProject.ProjectFile);
            }
            else
            {
                Text = Get.AppNameProject(Language.Lang.NewProject);
                CheckVersion();
            }                
        }

		private void ApplyLanguage()
		{
			if (OS.IsWindows)
			{
				Font = Language.Lang.UIFontWindows;
				txtMediaInfo.Font = new System.Drawing.Font("Lucida Console", 10F);
			}
			else
			{
				Font = Language.Lang.UIFontLinux;
				txtMediaInfo.Font = new System.Drawing.Font("FreeMono", 10F);
			}
			
			cboEncodingPreset.Font = new System.Drawing.Font(Font.Name, 9);
			txtFolderOutput.Font = new System.Drawing.Font(Font.Name, 9);

			cmsNewImport.Font = Font;
			cmsEncodingPreset.Font = Font;

			var frm = Language.Lang.frmMain;
			Control ctrl = this;

			do
			{
				ctrl = GetNextControl(ctrl, true);

				if (ctrl != null)
					if (ctrl is Label ||
						ctrl is Button ||
						ctrl is TabPage ||
						ctrl is CheckBox ||
						ctrl is RadioButton ||
						ctrl is GroupBox)
						if (frm.ContainsKey(ctrl.Name))
							ctrl.Text = frm[ctrl.Name];

			} while (ctrl != null);

			foreach (ToolStripMenuItem item in cmsNewImport.Items.OfType<ToolStripMenuItem>())
				if (Language.Lang.cmsNewImport.ContainsKey(item.Name))
					item.Text = Language.Lang.cmsNewImport[item.Name];

			foreach (ToolStripMenuItem item in cmsEncodingPreset.Items.OfType<ToolStripMenuItem>())
				if (Language.Lang.cmsEncodingPreset.ContainsKey(item.Name))
					item.Text = Language.Lang.cmsEncodingPreset[item.Name];

			cboVideoDeinterlaceMode.Items.Clear();
			cboVideoDeinterlaceMode.Items.AddRange(Language.Lang.ComboBoxDeInterlaceMode.ToArray());

			cboVideoDeinterlaceField.Items.Clear();
			cboVideoDeinterlaceField.Items.AddRange(Language.Lang.ComboBoxDeInterlaceField.ToArray());
		}

		private void ApplyPlugins()
		{
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
			cboAudioEncoder.SelectedValue = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac");
		}

		private void ApplyEncodingPreset()
		{
			var encpre = new Dictionary<string, string>();

			MediaPreset.Load();

			foreach (var item in MediaPreset.List.AsEnumerable().Reverse())
				encpre.Add(item.Key, item.Value.Name);

			cboEncodingPreset.DataSource = new BindingSource(encpre, null);
			cboEncodingPreset.DisplayMember = "Value";
			cboEncodingPreset.ValueMember = "Key";

			if (cboEncodingPreset.Items.Count > 0)
				cboEncodingPreset.SelectedIndex = 0; 
		}

		private void CheckVersion()
		{
            var thread = new BackgroundWorker();

            thread.DoWork += delegate (object o, DoWorkEventArgs r)
            {
                var data = new string[2];

                data[0] = new Download().GetString("https://raw.githubusercontent.com/Anime4000/IFME/master/version.txt");
                data[1] = new Download().GetString("https://github.com/Anime4000/IFME/raw/master/changelog.txt");

                r.Result = data;
            };

            thread.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs r)
            {
                var data = r.Result as string[];

                if (string.IsNullOrEmpty(data[0]))
                    return;

                if (Version.IsLatest(data[0]))
                    return; // simple version check

                new frmCheckUpdate(data[1]).ShowDialog();
            };

            thread.RunWorkerAsync();
		}

		private void DrawBanner()
		{
			try
			{
				if (pbxBanner.Width <= 0 || pbxBanner.Height <= 0)
					return;

				pbxBanner.BackgroundImage = Branding.Banner(pbxBanner.Width, pbxBanner.Height);
			}
			catch (Exception ex)
			{
				pbxBanner.BackgroundImage = Properties.Resources.Banner;
				ConsoleEx.Write(LogLevel.Warning, $"Error to re-draw banner, program is minimise? file missing? no permission? ");
				ConsoleEx.Write(ConsoleColor.DarkRed, $"{ex.Message}\n");
			}
		}

		private void EncodingPreset(string id, string name)
		{
            var videoCmd = string.Empty;
            var audioCmd = string.Empty;

            if (lstMedia.SelectedItems.Count > 0)
                if (lstVideo.Items.Count > 0)
                    videoCmd = (lstMedia.SelectedItems[0].Tag as MediaQueue).Video[0].Encoder.Command;

            if (lstMedia.SelectedItems.Count > 0)
                if (lstAudio.Items.Count > 0)
                    audioCmd = (lstMedia.SelectedItems[0].Tag as MediaQueue).Audio[0].Encoder.Command;

            int.TryParse(cboVideoResolution.Text.Split('x')[0], out int width);
            int.TryParse(cboVideoResolution.Text.Split('x')[1], out int height);
            double.TryParse(cboVideoFrameRate.Text, out double fps);
            int.TryParse(cboVideoBitDepth.Text, out int bpc);
            int.TryParse(cboVideoPixelFormat.Text, out int pix);

            decimal.TryParse(cboAudioQuality.Text, out decimal quality);
            int.TryParse(cboAudioSampleRate.Text, out int samplerate);
            int.TryParse(cboAudioChannel.Text, out int channel);

            var preset = new MediaPreset
            {
                OutputFormat = (string)cboTargetFormat.SelectedValue,

                VideoEncoder = new MediaQueueVideoEncoder
                {
                    Id = new Guid($"{cboVideoEncoder.SelectedValue}"),
                    Preset = cboVideoPreset.Text,
                    Tune = cboVideoTune.Text,
                    Mode = cboVideoRateControl.SelectedIndex,
                    Value = nudVideoRateFactor.Value,
                    MultiPass = (int)nudVideoMultiPass.Value,
                    Command = videoCmd
                },

                VideoQuality = new MediaQueueVideoQuality
                {
                    Width = width,
                    Height = height,
                    FrameRate = (float)fps,
                    BitDepth = bpc,
                    PixelFormat = pix
                },

                VideoDeInterlace = new MediaQueueVideoDeInterlace
                {
                    Enable = chkVideoDeinterlace.Checked,
                    Mode = cboVideoDeinterlaceMode.SelectedIndex,
                    Field = cboVideoDeinterlaceField.SelectedIndex
                },

                AudioEncoder = new MediaQueueAudioEncoder
                {
                    Id = new Guid($"{cboAudioEncoder.SelectedValue}"),
                    Mode = cboAudioMode.SelectedIndex,
                    Quality = quality,
                    SampleRate = samplerate,
                    Channel = channel,
                },

                AudioCommand = audioCmd
            };

			if (MediaPreset.List.ContainsKey(id))
			{
				preset.Name = MediaPreset.List[id].Name;
				preset.Author = MediaPreset.List[id].Author;

				var json = Newtonsoft.Json.JsonConvert.SerializeObject(preset, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(Path.Combine("preset", $"{id}.json"), json);
			}
			else
			{
				preset.Name = name;
				preset.Author = Environment.MachineName;

				var json = Newtonsoft.Json.JsonConvert.SerializeObject(preset, Newtonsoft.Json.Formatting.Indented);
				File.WriteAllText(Path.Combine("preset", $"{DateTime.Now:yyyyMMdd_HHmmss-ffff}_{id.Substring(0, 4)}.json"), json);
			}

			// reload listing
			ApplyEncodingPreset();
		}

		private string[] OpenFiles(MediaType type)
		{
			var exts = string.Empty;
			var extsVideo = "All video types|*.mkv;*.mp4;*.m4v;*.mts;*.m2ts;*.flv;*.webm;*.ogv;*.avi;*.divx;*.wmv;*.mpg;*.mpeg;*.mpv;*.m1v;*.dat;*.vob;*.avs|";
			var extsAudio = "All audio types|*.mp2;*.mp3;*.mp4;*.m4a;*.aac;*.ogg;*.opus;*.flac;*.wav|";
			var extsSub = "All subtitle types|*.ssa;*.ass;*.srt|";
			var extsAtt = "All font types|*.ttf;*.otf;*.woff;*.woff2;*.eot|";

			switch (type)
			{
				case MediaType.Video:
					exts = extsVideo;
					break;
				case MediaType.Audio:
					exts = extsAudio;
					break;
				case MediaType.Subtitle:
					exts = extsSub;
					break;
				case MediaType.Attachment:
					exts = extsAtt;
					break;
				default:
					exts = extsVideo + extsAudio;
					break;
			}

            var ofd = new OpenFileDialog
            {
                Filter = exts += "All types|*.*",
                FilterIndex = 1,
                Multiselect = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
				return ofd.FileNames;

			return new string[0];
		}

        private string OpenFileProject()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "IFME project file|*.ifp",
                FilterIndex = 1,
                Multiselect = false
            };

            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileNames[0];

            return string.Empty; ;
        }

		private void ListViewItemMove(ListViewItemType type, Direction direction)
		{
			try
			{
				if (type == ListViewItemType.Media)
				{
					if (lstMedia.SelectedItems.Count > 0)
					{
						ListViewItem selected = lstMedia.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstMedia.Items.Count;

						if (direction == Direction.Up)
						{
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
						else
						{
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
				}
				else if (type == ListViewItemType.Video)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstVideo.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Video;
						for (int i = 0; i < data.Count; i++)
							lstVideo.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstVideo.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstVideo.Items.Count;

						if (direction == Direction.Up)
						{
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
						}
						else
						{
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

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstVideo.Items[i].Tag as MediaQueueVideo;

						// refresh UI
						UXReloadVideo();
					}
				}
				else if (type == ListViewItemType.Audio)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstAudio.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Audio;
						for (int i = 0; i < data.Count; i++)
							lstAudio.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstAudio.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstAudio.Items.Count;

						if (direction == Direction.Up)
						{
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
						else
						{
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

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstAudio.Items[i].Tag as MediaQueueAudio;

						// refresh UI
						UXReloadAudio();
					}
				}
				else if (type == ListViewItemType.Subtitle)
				{
					if (lstMedia.SelectedItems.Count > 0 && lstSub.SelectedItems.Count > 0)
					{
						// copy
						var data = (lstMedia.SelectedItems[0].Tag as MediaQueue).Subtitle;
						for (int i = 0; i < data.Count; i++)
							lstSub.Items[i].Tag = data[i];

						// arrange item
						ListViewItem selected = lstSub.SelectedItems[0];
						int indx = selected.Index;
						int totl = lstSub.Items.Count;

						if (direction == Direction.Up)
						{
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
						else
						{
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

						// copy back new arrange data
						for (int i = 0; i < data.Count; i++)
							data[i] = lstSub.Items[i].Tag as MediaQueueSubtitle;

						// refresh
						UXReloadSubtitle();
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void MediaSelect()
		{
			if (lstMedia.Items.Count > 0)
			{
				var index = lstMedia.Items.Count - 1;
				lstMedia.SelectedIndices.Clear();
				lstMedia.Items[index].Selected = true;
			}
		}

		private void UXReloadMedia()
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
                lstMedia_SelectedIndexChanged(null, null);
            }
		}

		private void UXReloadVideo()
		{
			Thread.Sleep(1);
			var id = lstVideo.SelectedItems[0].Index;
			lstVideo.SelectedItems[0].Selected = false;
			lstVideo.Items[id].Selected = true;
		}

		private void UXReloadAudio()
		{
			Thread.Sleep(1);
			var id = lstAudio.SelectedItems[0].Index;
			lstAudio.SelectedItems[0].Selected = false;
			lstAudio.Items[id].Selected = true;

		}

		private void UXReloadSubtitle()
		{
			Thread.Sleep(1);
			var id = lstSub.SelectedItems[0].Index;
			lstSub.SelectedItems[0].Selected = false;
			lstSub.Items[id].Selected = true;
		}

        private void ProjectSave(string fileName)
        {
            var queue = new List<MediaQueue>();

            foreach (ListViewItem item in lstMedia.Items)
                queue.Add(item.Tag as MediaQueue);

            MediaProject.Save(fileName, queue);

            Text = Get.AppNameProject(fileName);

            MediaProject.ProjectFile = fileName; // reference, allow to save existing
        }

        private void ProjectOpen(string fileName)
        {
            var frm = new frmProgressBar();

            frm.Show();
            frm.Text = Language.Lang.PleaseWait;
            frm.Status = Language.Lang.ReadProjectFile;

            var thread = new BackgroundWorker();

            thread.DoWork += delegate (object o, DoWorkEventArgs r)
            {
                if (!string.IsNullOrEmpty(fileName))
                {
                    var queue = MediaProject.Load(fileName);

                    for (int i = 0; i < queue.Count; i++)
                    {
                        if (File.Exists(queue[i].FilePath))
                        {
                            queue[i].MediaInfo = new FFmpegDotNet.FFmpeg.Stream(queue[i].FilePath);
                        }

                        for (int v = 0; v < queue[i].Video.Count; v++)
                        {
                            if (!File.Exists(queue[i].Video[v].File))
                            {
                                ConsoleEx.Write(LogLevel.Error, "IFME", $"Video stream not found, skipping...\n");
                                queue[i].Video.RemoveAt(v);
                            }
                        }

                        for (int a = 0; a < queue[i].Audio.Count; a++)
                        {
                            if (!File.Exists(queue[i].Audio[a].File))
                            {
                                ConsoleEx.Write(LogLevel.Error, "IFME", $"Audio stream not found, removing stream.\n");
                                queue[i].Audio.RemoveAt(a);
                            }
                        }

                        for (int s = 0; s < queue[i].Subtitle.Count; s++)
                        {
                            if (!File.Exists(queue[i].Subtitle[s].File))
                            {
                                ConsoleEx.Write(LogLevel.Error, "IFME", $"Subtitle stream not found, removing stream.\n");
                                queue[i].Subtitle.RemoveAt(s);
                            }
                        }

                        MediaQueueAdd(queue[i]);

                        if (InvokeRequired)
                        {
                            Invoke(new MethodInvoker(delegate
                            {
                                frm.Status = string.Format(Language.Lang.ProgressBarImport.Message, i + 1, queue.Count, queue[i].FilePath);
                                frm.Progress = (int)(((float)(i + 1) / queue.Count) * 100.0);
                                frm.Text = Language.Lang.ProgressBarImport.Title + $": {frm.Progress}%";

                                //Application.DoEvents();
                            }));
                        }
                    }
                }
            };

            thread.RunWorkerCompleted += delegate (object o, RunWorkerCompletedEventArgs r)
            {
                frm.Close();

                if (MediaProject.StartEncode)
                {
                    MediaProject.StartEncode = false;
                    btnStart.PerformClick();
                }
            };

            thread.RunWorkerAsync();

            Text = Get.AppNameProject(fileName);

            MediaProject.ProjectFile = fileName; // reference, allow to save existing
        }

		private void AddMedia(string file)
		{
			var queue = new MediaQueue();
            var media = new FFmpegDotNet.FFmpeg.Stream(file);

            if (media.Video.Count == 0 && media.Audio.Count == 0)
				return;

			queue.Enable = true;
			queue.FilePath = file;
			queue.OutputFormat = "mkv";

			// if input only have audio
			if (media.Video.Count == 0)
				queue.OutputFormat = "ogg";

			queue.MediaInfo = media;

            MediaValidator.GetCodecVideo("mkv", out var vid);
            MediaValidator.GetCodecAudio("m4a", out var aid);

            // get default encoder of choice
            if (!Plugin.Items.TryGetValue(Properties.Settings.Default.EncoderIdVideo, out Plugin defVideo))
            {
                // something default if plugin of choice not found
            }

            if (!Plugin.Items.TryGetValue(Properties.Settings.Default.EncoderIdAudio, out Plugin defAudio))
            {
                // something default if plugin of choice not found
            }

            foreach (var item in media.Video)
			{
                queue.Video.Add(new MediaQueueVideo
                {
                    Enable = true,
                    File = file,
                    Id = item.Id,
                    Duration = item.Duration,
                    Lang = Get.LangCheck(item.Language),
                    Format = Get.CodecFormat(item.Codec),

                    Encoder = new MediaQueueVideoEncoder
                    {
                        Id = defVideo.GUID,
                        Preset = defVideo.Video.PresetDefault,
                        Tune = defVideo.Video.TuneDefault,
                        Mode = 0,
                        Value = defVideo.Video.Mode[0].Value.Default,
                        MultiPass = 2,
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
                        BitDepth = MediaValidator.IsValidBitDepth(vid.Encoder.Id, item.BitDepth),
                        PixelFormat = item.Chroma,
                    },

                    DeInterlace = new MediaQueueVideoDeInterlace
                    {
                        Enable = false,
                        Mode = 1,
                        Field = 0
                    }
				});
			}

			foreach (var item in media.Audio)
			{
                queue.Audio.Add(new MediaQueueAudio
                {
                    Enable = true,
                    File = file,
                    Id = item.Id,
                    Lang = Get.LangCheck(item.Language),
                    Format = Get.CodecFormat(item.Codec),

                    Encoder = new MediaQueueAudioEncoder
                    {
                        Id = defAudio.GUID,
                        Mode = 0,
                        Quality = defAudio.Audio.Mode[0].Default,
                        SampleRate = defAudio.Audio.SampleRateDefault,
                        Channel = defAudio.Audio.ChannelDefault,
                        Command = string.Empty
                    }
				});
			}

			foreach (var item in media.Subtitle)
			{
				queue.Subtitle.Add(new MediaQueueSubtitle
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Lang = Get.LangCheck(item.Language),
					Format = Get.CodecFormat(item.Codec),
				});
			}

			foreach (var item in media.Attachment)
			{
				queue.Attachment.Add(new MediaQueueAttachment
				{
					Enable = true,
					File = file,
					Id = item.Id,
					Name = item.FileName,
					Mime = item.MimeType
				});
			}

            MediaQueueAdd(queue);
		}

		private void AddVideo(string file)
		{
            if (MediaValidator.GetCodecVideo((string)cboTargetFormat.SelectedValue, out var video))
            {
                var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

                foreach (var item in new FFmpegDotNet.FFmpeg.Stream(file).Video)
                {
                    media.Video.Add(new MediaQueueVideo
                    {
                        Enable = true,
                        File = file,
                        Id = item.Id,
                        Duration = item.Duration,
                        Lang = Get.LangCheck(item.Language),
                        Format = Get.CodecFormat(item.Codec),

                        Encoder = video.Encoder,

                        Quality = new MediaQueueVideoQuality
                        {
                            Width = item.Width,
                            Height = item.Height,
                            IsVFR = (item.FrameRate != item.FrameRateAvg),
                            FrameRate = item.FrameRate,
                            FrameRateAvg = item.FrameRateAvg,
                            FrameCount = item.FrameCount,
                            BitDepth = item.BitDepth,
                            PixelFormat = item.Chroma,
                            Command = string.Empty
                        }
                    });
                }
            }
		}

		private void AddAudio(string file)
		{
            if (MediaValidator.GetCodecAudio((string)cboTargetFormat.SelectedValue, out var audio))
            {
                var media = (MediaQueue)lstMedia.SelectedItems[0].Tag;

                foreach (var item in new FFmpegDotNet.FFmpeg.Stream(file).Audio)
                {
                    media.Audio.Add(new MediaQueueAudio
                    {
                        Enable = true,
                        File = file,
                        Id = item.Id,
                        Lang = Get.LangCheck(item.Language),
                        Format = Get.CodecFormat(item.Codec),

                        Encoder = new MediaQueueAudioEncoder
                        {
                            Id = audio.Encoder.Id,
                            Mode = audio.Encoder.Mode,
                            Quality = audio.Encoder.Quality,
                            SampleRate = audio.Encoder.SampleRate,
                            Channel = audio.Encoder.Channel,
                            Command = audio.Encoder.Command
                        }
                    });
                }
            }
		}

		private void AddSubtitle(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;

				queue.Subtitle.Add(new MediaQueueSubtitle
				{
					Enable = true,
					File = file,
					Id = -1,
					Lang = Get.LangFile(file),
					Format = Path.GetExtension(file).Remove(1)
				});
			}
		}

		private void AddSubtitle2(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;

				var stream = new FFmpegDotNet.FFmpeg.Stream(file).Subtitle;

				foreach (var item in stream)
				{
					queue.Subtitle.Add(new MediaQueueSubtitle
					{
						Enable = true,
						File = file,
						Id = item.Id,
						Lang = item.Language,
						Format = Get.CodecFormat(item.Codec)
					});
				}
			}
		}

		private void AddAttachment(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;
				var mime = "application/octet-stream";

				mime = Get.MimeType(Path.GetExtension(file));

				queue.Attachment.Add(new MediaQueueAttachment
				{
					Enable = true,
					File = file,
					Name = Path.GetFileName(file),
					Id = -1,
					Mime = mime
				});
			}
		}

		private void AddAttachment2(string file)
		{
			if (lstMedia.SelectedItems.Count > 0)
			{
				var queue = (MediaQueue)lstMedia.SelectedItems[0].Tag;

				var stream = new FFmpegDotNet.FFmpeg.Stream(file).Attachment;

				if (stream.Count > 0)
				{
					foreach (var item in stream)
					{
						queue.Attachment.Add(new MediaQueueAttachment
						{
							Enable = true,
							File = file,
							Name = item.FileName,
							Id = item.Id,
							Mime = item.MimeType
						});
					}
				}
			}
		}

        private void MediaQueueAdd(MediaQueue queue)
        {
            var lst = new ListViewItem(new[]
            {
                Path.GetFileName(queue.FilePath),
                TimeSpan.FromSeconds(queue.MediaInfo.Duration).ToString("hh\\:mm\\:ss"),
                Path.GetExtension(queue.FilePath).Substring(1).ToUpperInvariant(),
                queue.OutputFormat.ToUpperInvariant(),
                queue.Enable ? "Ready" : "Done"
            })
            {
                Tag = queue,
                Checked = queue.Enable
            };

            if(InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate { lstMedia.Items.Add(lst); }));
            }
            else
            {
                lstMedia.Items.Add(lst);
            }
        }

		// Minimise code, all controls subscribe one function :)
		private void MediaApply(object sender, EventArgs e)
		{
			var ctrl = (sender as Control).Name;

			// data update
			foreach (ListViewItem q in lstMedia.SelectedItems)
			{
				var m = q.Tag as MediaQueue;

                m.OutputFormat = (string)cboTargetFormat.SelectedValue;
				q.SubItems[3].Text = m.OutputFormat.ToUpperInvariant();

                if (lstMedia.SelectedItems.Count > 1)
				{
					for (int i = 0; i < m.Video.Count; i++)
					{
						var temp = m.Video[i];
						MediaApplyVideo(ctrl, ref temp);
					}

					for (int i = 0; i < m.Audio.Count; i++)
					{
						var temp = m.Audio[i];
						MediaApplyAudio(ctrl, ref temp);
					}

					for (int i = 0; i < m.Subtitle.Count; i++)
					{
						var temp = m.Subtitle[i];
						MediaApplySubtitle(ctrl, ref temp);
					}

					for (int i = 0; i < m.Attachment.Count; i++)
					{
						var temp = m.Attachment[i];
						MediaApplyAttachment(ctrl, ref temp);
					}
				}
				else
				{
					foreach (ListViewItem i in lstVideo.SelectedItems)
					{
						var temp = m.Video[i.Index];
						MediaApplyVideo(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstAudio.SelectedItems)
					{
						var temp = m.Audio[i.Index];
						MediaApplyAudio(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstSub.SelectedItems)
					{
						var temp = m.Subtitle[i.Index];
						MediaApplySubtitle(ctrl, ref temp);
					}

					foreach (ListViewItem i in lstAttach.SelectedItems)
					{
						var temp = m.Attachment[i.Index];
						MediaApplyAttachment(ctrl, ref temp);
					}
				}

                if (string.Equals(ctrl, chkSubHard.Name))
                {
                    m.HardSub = chkSubHard.Checked;
                }

				if (string.Equals(ctrl, chkAdvTrim.Name))
				{
					m.Trim.Enable = chkAdvTrim.Checked;
				}
			}

			// refresh
			UXReloadMedia();
		}

		private void MediaApplyVideo(string ctrl, ref MediaQueueVideo video)
		{
			if (string.Equals(ctrl, cboVideoStreamLang.Name))
			{
				video.Lang = $"{cboVideoStreamLang.SelectedValue}";
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name))
			{
				var id = new Guid($"{cboVideoEncoder.SelectedValue}");

				// if user change encoder, update command-line as well
				if (!Guid.Equals(video.Encoder, id))
				{
                    if (Plugin.Items.TryGetValue(id, out Plugin temp))
                    {
                        video.Encoder.Command = temp.Video.Args.Command;
                    }
                }

				video.Encoder.Id = new Guid($"{cboVideoEncoder.SelectedValue}");
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoPreset.Name))
			{
				video.Encoder.Preset = cboVideoPreset.Text;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoTune.Name))
			{
				video.Encoder.Tune = cboVideoTune.Text;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoRateControl.Name))
			{
				video.Encoder.Mode = cboVideoRateControl.SelectedIndex;
				video.Encoder.Value = nudVideoRateFactor.Value; // changing mode give default value
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, nudVideoRateFactor.Name))
			{
				video.Encoder.Value = nudVideoRateFactor.Value;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, nudVideoMultiPass.Name))
			{
				video.Encoder.MultiPass = Convert.ToInt32(nudVideoMultiPass.Value);
			}

			// Video pixel
			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoResolution.Name))
			{
				var w = 0;
				var h = 0;
				var x = cboVideoResolution.Text;
				if (x.Contains('x'))
				{
					int.TryParse(x.Split('x')[0], out w);
					int.TryParse(x.Split('x')[1], out h);
				}
				video.Quality.Width = w;
				video.Quality.Height = h;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoFrameRate.Name))
			{
				float f = 0;
				float.TryParse(cboVideoFrameRate.Text, out f);
				video.Quality.FrameRate = f;
				video.Quality.FrameCount = (int)Math.Ceiling(video.Duration * f);
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoBitDepth.Name))
			{
				var b = video.Quality.BitDepth;

				if (string.Equals(ctrl, cboVideoEncoder.Name))
				{
					video.Quality.BitDepth = MediaValidator.IsValidBitDepth(video.Encoder.Id, b);
				}
				else
				{
					int.TryParse(cboVideoBitDepth.Text, out b);
					video.Quality.BitDepth = MediaValidator.IsValidBitDepth(video.Encoder.Id, b);
				}
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoPixelFormat.Name))
			{
				var y = video.Quality.PixelFormat;
				
				if (string.Equals(ctrl, cboVideoEncoder.Name))
				{
					video.Quality.PixelFormat = y;
				}
				else
				{
					int.TryParse(cboVideoPixelFormat.Text, out y);
					video.Quality.PixelFormat = y;
				}
			}

			// Video Interlace
			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, chkVideoDeinterlace.Name))
			{
				video.DeInterlace.Enable = chkVideoDeinterlace.Checked;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoDeinterlaceMode.Name))
			{
				video.DeInterlace.Mode = cboVideoDeinterlaceMode.SelectedIndex;
			}

			if (string.Equals(ctrl, cboVideoEncoder.Name) || string.Equals(ctrl, cboVideoDeinterlaceField.Name))
			{
				video.DeInterlace.Field = cboVideoDeinterlaceField.SelectedIndex;
			}
		}

		private void MediaApplyAudio(string ctrl, ref MediaQueueAudio audio)
		{
			if (string.Equals(ctrl, cboAudioStreamLang.Name))
			{
				audio.Lang = $"{cboAudioStreamLang.SelectedValue}";
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name))
			{
				audio.Encoder.Id = new Guid($"{cboAudioEncoder.SelectedValue}");
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioMode.Name))
			{
				audio.Encoder.Mode = cboAudioMode.SelectedIndex;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioQuality.Name))
			{
				decimal q = 0;
				decimal.TryParse(cboAudioQuality.Text, out q);
				audio.Encoder.Quality = q;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioSampleRate.Name))
			{
				var hz = 0;
				int.TryParse(cboAudioSampleRate.Text, out hz);
				audio.Encoder.SampleRate = hz;
			}

			if (string.Equals(ctrl, cboAudioEncoder.Name) || string.Equals(ctrl, cboAudioChannel.Name))
			{
				double ch = 0;
				double.TryParse(cboAudioChannel.Text, out ch);
				audio.Encoder.Channel = (int)Math.Ceiling(ch); // when value 5.1 become 6, 7.1 become 8
			}
		}

		private void MediaApplySubtitle(string ctrl, ref MediaQueueSubtitle subtitle)
		{
			if (string.Equals(ctrl, cboSubLang.Name))
			{
				subtitle.Lang = $"{cboSubLang.SelectedValue}";
			}
		}

		private void MediaApplyAttachment(string ctrl, ref MediaQueueAttachment attachment)
		{
			if (string.Equals(ctrl, cboAttachMime.Name))
			{
				attachment.Mime = cboAttachMime.Text;
			}
		}

		private void MediaPopulate(MediaQueue media)
		{
			// Media Info
			try
			{
				var mf = media.MediaInfo;
				var md = string.Empty;
				var du = TimeSpan.FromSeconds(mf.Duration);

				md =
					$"File path          : {mf.FilePath}\r\n" +
					$"File size          : {Get.FileSizeDEC((long)mf.FileSize)} (base 10), {Get.FileSizeIEC((long)mf.FileSize)} (base 2)\r\n" +
					$"Bitrate            : {mf.BitRate}\r\n" +
					$"Duration           : {du.Hours:D2}:{du.Minutes:D2}:{du.Seconds:D2} (estimated)\r\n" +
					$"Format             : {mf.FormatName} ({mf.FormatNameFull})\r\n";

				if (mf.Video.Count > 0)
				{

					foreach (var item in mf.Video)
					{
						md +=
							"\r\n" +
							$"ID                 : {item.Id:00}\r\n" +
							$"Type:              : Video\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Width              : {item.Width}\r\n" +
							$"Height             : {item.Height}\r\n" +
							$"Frame rate         : {item.FrameRate:00.000}fps\r\n" +
							$"Frame rate (avg)   : {item.FrameRateAvg:00.000}fps\r\n" +
							$"Bit Depth          : {item.BitDepth}bit per channel\r\n" +
							$"Chroma             : {item.Chroma}\r\n";
					}
				}

				if (mf.Audio.Count > 0)
				{
					foreach (var item in mf.Audio)
					{
						md +=
							"\r\n" +
							$"ID                 : {item.Id:00}\r\n" +
							$"Type               : Audio\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Sample rate        : {item.SampleRate}Hz\r\n" +
							$"Channel            : {item.Channel}Ch\r\n";
					}
				}

				if (mf.Subtitle.Count > 0)
				{
					foreach (var item in mf.Subtitle)
					{
						md +=
							"\r\n" +
							$"ID                 : {item.Id:00}\r\n" +
							$"Type               : Subtitle\r\n" +
							$"Codec              : {item.Codec}\r\n" +
							$"Language           : {item.Language}\r\n";
					}
				}

				if (mf.Attachment.Count > 0)
				{
					foreach (var item in mf.Attachment)
					{
						md +=
							"\r\n" +
							$"ID                 : {item.Id:00}\r\n" +
							$"Type               : Attachment\r\n" +
							$"File name          : {item.FileName}\r\n" +
							$"Mime               : {item.MimeType}\r\n";
					}
				}

				txtMediaInfo.Text = md;
			}
			catch
			{
				txtMediaInfo.Text = "New file, no main data to display";
			}

			// Format choice
            cboTargetFormat.SelectedValue = media.OutputFormat;

            // Video
            lstVideo.SelectedItems.Clear();
            lstVideo.Items.Clear();
			if (media.Video.Count > 0)
			{
                foreach (var item in media.Video)
				{
					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						$"{item.Lang}",
						$"{(item.Quality.Width > 0 && item.Quality.Height > 0 ? $"{item.Quality.Width}x{item.Quality.Height}" : "auto")} @ {(item.Quality.FrameRate > 0 ? $"{item.Quality.FrameRate} fps" : "auto")} ({item.Quality.BitDepth} bit @ YUV{item.Quality.PixelFormat})"
					});
					lst.Checked = item.Enable;
					lst.Tag = item; // allow lstVideo to arrange item UP or DOWN

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
						$"{item.Id}",
						$"{item.Lang}",
						$"{item.Encoder.Quality} @ {item.Encoder.SampleRate}Hz {(item.Encoder.Channel == 0 ? "auto" : $"{item.Encoder.Channel} Ch")}"
					});
					lst.Checked = item.Enable;
					lst.Tag = item; // allow lstAudio to arrange item UP or DOWN

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
					var langFull = string.Empty;
					Get.LanguageCode.TryGetValue(item.Lang, out langFull);

					var lst = new ListViewItem(new[]
					{
						$"{item.Id}",
						Path.GetFileName(item.File),
						langFull
					});
					lst.Checked = item.Enable;
					lst.Tag = item; //allow lstSub to arrange item UP or DOWN

					lstSub.Items.Add(lst);
				}

				lstSub.Items[0].Selected = true;
			}
            chkSubHard.Checked = media.HardSub;

            // Attachment
            lstAttach.Items.Clear();
			if (media.Attachment.Count > 0)
			{
				foreach (var item in media.Attachment)
				{
					var fName = Path.GetFileName(item.File);

					lstAttach.Items.Add(new ListViewItem(new[]
					{
						$"{item.Id}",
						$"{(string.Equals(fName, item.Name) ? fName : $"{fName} ({item.Name})")}",
						$"{item.Mime}"
					}));
				}
			}

			// Advance
			chkAdvTrim.Checked = media.Trim.Enable;
			mtxAdvTimeStart.Text = media.Trim.Start;
			mtxAdvTimeEnd.Text = media.Trim.End;
			mtxAdvTimeDuration.Text = media.Trim.Duration;
		}

		private void MediaPopulateVideo(object video)
		{
			// delay
			Thread.Sleep(1);

			// populate
			var v = video as MediaQueueVideo;

			// not related
			BeginInvoke((Action)delegate () { cboVideoStreamLang.SelectedValue = v.Lang; });

			// select encoder and wait ui thread to load
			BeginInvoke((Action)delegate () { cboVideoEncoder.SelectedValue = v.Encoder.Id; });
			Thread.Sleep(1);

			// select mode and wait ui thread to load
			BeginInvoke((Action)delegate () { cboVideoRateControl.SelectedIndex = v.Encoder.Mode; });
			Thread.Sleep(1);

			// when control is loaded, begin to display
			BeginInvoke((Action)delegate ()
			{
				cboVideoPreset.SelectedItem = v.Encoder.Preset;
				cboVideoTune.SelectedItem = v.Encoder.Tune;

				// this can be buggy
				try
				{
					nudVideoRateFactor.Value = v.Encoder.Value;
				}
				catch (Exception e)
				{
					ConsoleEx.Write(LogLevel.Warning, "Slow GUI, trying to display Rate Factor value first before change min/max value, cause current value are not inside min/max range, you can re-select again, don't worry :)");
					ConsoleEx.Write(ConsoleColor.DarkYellow, $" ({e.Message})\n");

					lstVideo.SelectedItems.Clear();
				}

				nudVideoMultiPass.Value = v.Encoder.MultiPass;

				cboVideoResolution.Text = $"{v.Quality.Width}x{v.Quality.Height}";
				cboVideoFrameRate.Text = $"{Math.Round(v.Quality.FrameRate, 3)}";
				cboVideoBitDepth.Text = $"{v.Quality.BitDepth}";
				cboVideoPixelFormat.Text = $"{v.Quality.PixelFormat}";

				chkVideoDeinterlace.Checked = v.DeInterlace.Enable;
				cboVideoDeinterlaceMode.SelectedIndex = v.DeInterlace.Mode;
				cboVideoDeinterlaceField.SelectedIndex = v.DeInterlace.Field;
			});
		}

		private void MediaPopulateAudio(object audio)
		{
			// delay
			Thread.Sleep(1);

			// populate
			var a = audio as MediaQueueAudio;

			// not related
			BeginInvoke((Action)delegate () { cboAudioStreamLang.SelectedValue = a.Lang; });

			// select encoder and wait ui thread to load
			BeginInvoke((Action)delegate () { cboAudioEncoder.SelectedValue = a.Encoder.Id; });
			Thread.Sleep(1);

			// select mode and wait ui thread to load
			BeginInvoke((Action)delegate () { cboAudioMode.SelectedIndex = a.Encoder.Mode; });
			Thread.Sleep(1);

			// when ui is loaded, begin to display
			BeginInvoke((Action)delegate ()
			{
				cboAudioQuality.Text = $"{a.Encoder.Quality}";
				cboAudioSampleRate.Text = $"{a.Encoder.SampleRate}";
				cboAudioChannel.SelectedValue = a.Encoder.Channel;
			});
		}

		private void MediaPopulateSubtitle(object subtitle)
		{
		
		}

		private void bgThread_DoWork(object sender, DoWorkEventArgs e)
		{
			var media = e.Argument as Dictionary<int, MediaQueue>;

			foreach (var item in media)
			{
				var id = item.Key;
				var mq = item.Value;

				if (mq.Enable)
				{
					var tt = DateTime.Now;

					lstMedia.Invoke((MethodInvoker)delegate
					{
						lstMedia.Items[id].SubItems[4].Text = "Encoding...";
					});

					new MediaEncoding(mq);

					lstMedia.Invoke((MethodInvoker)delegate
					{
						lstMedia.Items[id].Checked = false;
						lstMedia.Items[id].SubItems[4].Text = $"Done! ({Get.Duration(tt)})";
					});
				}
                else
                {
                    lstMedia.Invoke((MethodInvoker)delegate
                    {
                        lstMedia.Items[id].SubItems[4].Text = "Skip...";
                    });
                }
			}
		}

		private void bgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				ProcessManager.Stop();

				Console.Write("\n\n");
				ConsoleEx.Write(LogLevel.Warning, "Encoding cancel by user...\n");

				foreach (ListViewItem item in lstMedia.Items)
					item.SubItems[4].Text = "Abort by user";
			}

			Console.Write("\n\n");

			if (Properties.Settings.Default.ShutdownType == 1)
			{
				ProcessManager.ComputerReboot();
			}
			else if (Properties.Settings.Default.ShutdownType == 2)
			{
				ProcessManager.ComputerShutdown();
			}

			btnStop.Enabled = false;
			btnPause.Enabled = false;
			btnStart.Enabled = true;
		}
	}
}
