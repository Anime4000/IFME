using System;
using System.IO;

namespace ifme
{
	class MediaEncoding
	{
		private readonly string tempDir = Get.FolderTemp;
		private readonly string saveDir = Get.FolderSave;

		private readonly string FFmpeg = Path.Combine(Get.AppRootDir, "plugin", $"ffmpeg{Properties.Settings.Default.FFmpegArch}", "ffmpeg");
		private readonly string MkvExtract = Path.Combine(Get.AppRootDir, "plugin", $"mkvtoolnix", "mkvextract");
		private readonly string MkvMerge = Path.Combine(Get.AppRootDir, "plugin", $"mkvtoolnix", "mkvmerge");
		private readonly string Mp4Box = Path.Combine(Get.AppRootDir, "plugin", $"mp4box", "mp4box");

		enum ModeSave
		{
			Temp,
			Direct
		}

		public MediaEncoding(MediaQueue queue)
		{

			// Clean temp folder
			try
			{
				ConsoleEx.Write(LogLevel.Normal, $"Clearing temp folder: {tempDir}\n");

				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, true);

				Directory.CreateDirectory(tempDir);

				// Prepare temp folder
				if (!Directory.Exists(Path.Combine(tempDir, "attachments")))
					Directory.CreateDirectory(Path.Combine(tempDir, "attachments"));
			}
			catch (Exception)
			{
				ConsoleEx.Write(LogLevel.Error, $"ERROR clearing temp folder: {tempDir}\n");
				return;
			}

			if (queue.OutputFormat.IsOneOf(TargetFormat.MP3, TargetFormat.M4A, TargetFormat.OGG, TargetFormat.OPUS, TargetFormat.FLAC))
			{
				AudioEncoding(queue, ModeSave.Direct);
			}
			else
			{
				// Extract
				MediaExtract(queue);

				// Audio
				AudioEncoding(queue, ModeSave.Temp);

				// Video
				VideoEncoding(queue);

				// Mux
				VideoMuxing(queue);
			}

			ConsoleEx.Write(LogLevel.Normal, "Yay! All media done encoding...\n");
		}

		private int MediaExtract(MediaQueue queue)
		{
			ConsoleEx.Write(LogLevel.Normal, "Extracting chapters, subtitles and fonts :)\n");

			var id = 0;
			foreach (var item in queue.Subtitle)
			{
				if (item.Id < 0)
				{
					File.Copy(item.File, Path.Combine(tempDir, $"subtitle{id++:D4}_{item.Lang}{Path.GetExtension(item.File)}"));
				}
				else
				{
					ProcessManager.Start(FFmpeg, $"-hide_banner -v quiet -i \"{item.File}\" -map 0:{item.Id} -y subtitle{id++:D4}_{item.Lang}.{item.Format}");
				}
			}

			foreach (var item in queue.Attachment)
			{
                if (string.Equals(Path.GetFileName(item.File), item.Name))
				    File.Copy(item.File, Path.Combine(tempDir, "attachments", Path.GetFileName(item.File)));
			}

			if (!string.IsNullOrEmpty(queue.File))
			{
				ProcessManager.Start(FFmpeg, $"-hide_banner -v quiet -dump_attachment:t \"\" -i \"{queue.File}\" -y", Path.Combine(tempDir, "attachments"));

				var ec = ProcessManager.Start(MkvExtract, $"chapters \"{queue.File}\" > chapters.xml");
				if (ec >= 1)
					File.Delete(Path.Combine(tempDir, "chapters.xml"));
			}

			return 0;
		}

		private int AudioEncoding(MediaQueue queue, ModeSave mode)
		{
			ConsoleEx.Write(LogLevel.Normal, "Encoding audio, please wait...\n");

			var ec = 0;
			var id = 0;

			foreach (var item in queue.Audio)
			{
				Plugin codec;

				if (Plugin.Items.TryGetValue(item.Encoder, out codec))
				{
					var ac = codec.Audio;

					var qu = (string.IsNullOrEmpty(ac.Mode[item.EncoderMode].Args) ? string.Empty : $"{ac.Mode[item.EncoderMode].Args} {item.EncoderQuality}");
					var hz = (item.EncoderSampleRate == 0 ? string.Empty : $"-ar {item.EncoderSampleRate}");
					var ch = (item.EncoderChannel == 0 ? string.Empty : $"-ac {item.EncoderChannel}");

					var newfile = (mode == ModeSave.Temp ? $"audio{id++:D4}_{item.Lang}.{ac.Extension}" : Path.Combine(saveDir, $"{Path.GetFileNameWithoutExtension(queue.File)}_ID{id++:D2}.{ac.Extension}"));

					if (ac.Args.Pipe)
					{
						ec = ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} -f wav -", Path.Combine(codec.FilePath, ac.Encoder), $"{qu} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} \"{newfile}\"");
					}
					else
					{
						ec = ProcessManager.Start(Path.Combine(codec.FilePath, ac.Encoder), $"{ac.Args.Input} \"{item.File}\" -map 0:{item.Id} {ac.Args.Command} {qu} {ac.Args.Output} \"{newfile}\"");
					}

					if (ec == 0)
						ConsoleEx.Write(LogLevel.Normal, "Audio encoding OK!\n");
					else
						ConsoleEx.Write(LogLevel.Warning, "Audio encoding might have a problem!\n");
				}
			}

			return ec;
		}

		private int VideoEncoding(MediaQueue queue)
		{
			ConsoleEx.Write(LogLevel.Normal, "Encoding video, this take a while...\n");

			var id = 0;
			foreach (var item in queue.Video)
			{
				Plugin codec;

				if (Plugin.Items.TryGetValue(item.Encoder, out codec))
				{
					var vc = codec.Video;
					var en = Path.Combine(codec.FilePath, vc.Encoder.Find(b => b.BitDepth == item.BitDepth).Binary);
                    var outfile = $"video{id++:D4}_{item.Lang}.{vc.Extension}";

					var yuv = $"yuv{item.PixelFormat}p";
                    var res = string.Empty;
                    var fps = string.Empty;
                    var deinterlace = string.Empty;

                    var preset = string.Empty;
                    var tune = string.Empty;
                    var quality = string.Empty;
                    var bitdepth = string.Empty;
                    var framecount = string.Empty;

                    // cmd builder
                    if (item.BitDepth > 8)
                    {
                        yuv += $"{item.BitDepth}le";
                    }

                    if (item.FrameRate >= 5)
                    {
                        fps = $"-r {item.FrameRate}";
                    }

                    if (item.Width >= 128 && item.Height >= 128)
                    {
                        res = $"-s {item.Width}x{item.Height}";
                    }

                    if (item.DeInterlace)
                    {
                        deinterlace = $"-vf \"yadif={item.DeInterlaceMode}:{item.DeInterlaceField}:0\"";
                    }

                    if (!vc.Args.Preset.IsDisable() && !item.EncoderPreset.IsDisable())
                    {
                        preset = $"{vc.Args.Preset} {item.EncoderPreset}";
                    }

                    if (!vc.Args.Tune.IsDisable() && !item.EncoderTune.IsDisable())
                    {
                        tune = $"{vc.Args.Tune} {item.EncoderTune}";
                    }

                    if (!vc.Mode[item.EncoderMode].Args.IsDisable())
                    {
                        quality = $"{vc.Mode[item.EncoderMode].Args} {item.EncoderValue}";
                    }

                    if (!vc.Args.BitDepth.IsDisable() && item.BitDepth >= 8)
                    {
                        bitdepth = $"{vc.Args.BitDepth} {item.BitDepth}";
                    }

                    if (!vc.Args.FrameCount.IsDisable())
                    {
                        framecount = $"{vc.Args.FrameCount} {item.FrameCount + Properties.Settings.Default.FrameCountOffset}";
                    }

                    // begin encoding
					if (vc.Mode[item.EncoderMode].MultiPass)
					{
                        var p = 1;
                        var pass = string.Empty;

                        ConsoleEx.Write(LogLevel.Warning, "Frame count is disable for Multi-pass encoding, ");
                        ConsoleEx.Write(ConsoleColor.Yellow, "Avoid inconsistent across multi-pass.\n");

                        do
                        {
                            pass = vc.Args.PassNth;

                            if (p == 1)
                                pass = vc.Args.PassFirst;

                            if (p == item.EncoderMultiPass)
                                pass = vc.Args.PassLast;

                            ConsoleEx.Write(LogLevel.Normal, $"Multi-pass encoding: ");
                            ConsoleEx.Write(ConsoleColor.Green, $"{p} of {item.EncoderMultiPass}\n");

                            if (vc.Args.Pipe)
                                ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -strict -1 -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {deinterlace} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {pass} {item.EncoderCommand} {vc.Args.Output} {outfile}");
                            else
                                ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {deinterlace} {vc.Args.UnPipe} {preset} {quality} {tune} {pass} {item.EncoderCommand} {vc.Args.Output} {outfile}");

                            p++;

                        } while (p <= item.EncoderMultiPass);
					}
					else
					{
						if (vc.Args.Pipe)
						{
							ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -strict -1 -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {deinterlace} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {framecount} {item.EncoderCommand} {vc.Args.Output} {outfile}");
						}
						else
						{
							ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {deinterlace} {vc.Args.UnPipe} {preset} {quality} {tune} {item.EncoderCommand} {vc.Args.Output} {outfile}");
						}
					}
				}
			}

			return 0;
		}

		private int VideoMuxing(MediaQueue queue)
		{
			ConsoleEx.Write(LogLevel.Normal, "Merging RAW files as single media file\n");

			// ready
			var filename = Path.GetFileNameWithoutExtension(queue.File);
			var prefix = string.Empty;
			var postfix = string.Empty;

			// prefix
			if (Properties.Settings.Default.FileNamePrefixType == 1)
				prefix = $"[{DateTime.Now:yyyyMMdd_HHmmss}] ";
			else if (Properties.Settings.Default.FileNamePrefixType == 2)
				prefix = Properties.Settings.Default.FileNamePrefix;

			// postfix
			if (Properties.Settings.Default.FileNamePostfixType == 1)
				postfix = Properties.Settings.Default.FileNamePostfix;

			// final
			var fileout = Path.Combine(saveDir, $"{prefix}{filename}{postfix}");

			// check if exist
			if (File.Exists(fileout))
				fileout = $"{fileout} NEW";

			if (queue.OutputFormat == TargetFormat.MP4)
			{
				string cmdvideo = string.Empty;
				string cmdaudio = string.Empty;

				var mp4vid = 0;
				foreach (var video in Directory.GetFiles(tempDir, "video*"))
				{
					cmdvideo += $"-add \"{video}#video:name=Video {mp4vid++}:lang={Get.FileLang(video)}\" ";
				}

				var mp4aid = 0;
				foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
				{
					cmdaudio += $"-add \"{audio}#audio:name=Audio {mp4aid++}:lang={Get.FileLang(audio)}\" ";
				}

				int exitcode = ProcessManager.Start(Mp4Box, $"{cmdvideo} {cmdaudio} -itags tool=\"{Get.AppNameLong}\" -new \"{fileout}.mp4\"");

				if (exitcode == 1)
					ConsoleEx.Write(LogLevel.Normal, "MP4Box merge perfectly!\n");
				else
					ConsoleEx.Write(LogLevel.Warning, "MP4Box merge with warning...\n");
			}
			else if (queue.OutputFormat == TargetFormat.MKV)
			{
				string cmdvideo = string.Empty;
				string cmdaudio = string.Empty;
				string cmdsubs = string.Empty;
				string cmdattach = string.Empty;
				string cmdchapter = string.Empty;

				var tags = string.Format(Properties.Resources.MkvTags, Get.AppNameLong, Get.AppNameLib);
				File.WriteAllText(Path.Combine(tempDir, "tags.xml"), tags);

				foreach (var video in Directory.GetFiles(tempDir, "video*"))
					cmdvideo += $"--language 0:{Get.FileLang(video)} \"{video}\" ";
				
				foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
					cmdaudio += $"--language 0:{Get.FileLang(audio)} \"{audio}\" ";
				
				foreach (var subs in Directory.GetFiles(tempDir, "subtitle*"))
					cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{Get.FileLang(subs)} \"{subs}\" ";
				
				foreach (var attach in Directory.GetFiles(Path.Combine(tempDir, "attachments"), "*.*"))
                {
                    foreach (var item in queue.Attachment)
                    {
                        if (string.Equals(item.Name, Path.GetFileName(attach)))
                        {
                            cmdattach += $"--attachment-mime-type \"{item.Mime}\" --attachment-description yes --attach-file \"{attach}\" ";
                            break; // save time, leave loop once found
                        }
                    }
                }

				if (File.Exists(Path.Combine(tempDir, "chapters.xml")))
				{
					FileInfo ChapLen = new FileInfo(Path.Combine(tempDir, "chapters.xml"));
					if (ChapLen.Length > 256)
						cmdchapter = $"--chapters \"{Path.Combine(tempDir, "chapters.xml")}\"";
				}

				// try
				var cmd = $"{cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}";
				var exitcode = ProcessManager.Start(MkvMerge, $"-o \"{fileout}.mkv\" --disable-track-statistics-tags -t 0:\"{Path.Combine(tempDir, "tags.xml")}\" {cmd}");

				if (exitcode == 0)
				{
					ConsoleEx.Write(LogLevel.Normal, "MkvToolNix merge perfectly on first attempt!\n");
				}
				else if (exitcode == 1)
				{
					ConsoleEx.Write(LogLevel.Warning, "MkvToolNix merge with warning on first attempt, it should work.\n");
				}
				else
				{
					ConsoleEx.Write(LogLevel.Error, "MkvToolNix can't merge on first attempt, trying to skip adding tags, chapters & fonts!\n");

					// try without chapter and fonts
					cmd = $"{cmdvideo} {cmdaudio} {cmdsubs}";
					exitcode = ProcessManager.Start(MkvMerge, $"-o \"{fileout}.mkv\" --disable-track-statistics-tags {cmd}");

					if (exitcode == 0)
					{
						ConsoleEx.Write(LogLevel.Normal, "MkvToolNix merge perfectly without tags, chapters & fonts on second attempt!\n");
					}
					else if (exitcode == 1)
					{
						ConsoleEx.Write(LogLevel.Warning, "MkvToolNix merge with warning on second attempt even without tags, chapters & fonts, it should work.\n");
					}
					else
					{
                        // copy whole thing
                        ConsoleEx.Write(LogLevel.Error, "MkvToolNix still can't merge on second attempt! BACKUP ALL RAW FILE TO SAVE FOLDER!\n");
						Get.DirectoryCopy(tempDir, Path.Combine(saveDir, fileout), true);
					}
				}
			}
			else if (queue.OutputFormat == TargetFormat.WEBM)
			{
				string cmdvideo = string.Empty;
				string cmdaudio = string.Empty;

				foreach (var video in Directory.GetFiles(tempDir, "video*"))
				{
					cmdvideo += $"-i \"{video}\" ";
				}

				foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
				{
					cmdaudio += $"-i \"{audio}\" ";
				}

				var exitcode = ProcessManager.Start(FFmpeg, $"-hide_banner -v quiet -stats {cmdvideo} {cmdaudio} -vcodec copy -acodec copy -y \"{fileout}.webm\"");

				if (exitcode == 0)
					ConsoleEx.Write(LogLevel.Normal, "FFmpeg merge WebM media perfectly!\n");
				else
					ConsoleEx.Write(LogLevel.Error, "FFmpeg having issue to merge WebM media.\n");
			}

			return 0;
		}
	}
}
