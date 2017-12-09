using System;
using System.IO;
using System.Collections.Generic;

namespace ifme
{
	class MediaEncoding
	{
		private readonly string SaveDir = Get.FolderSave;
		private readonly string TempDir = Get.FolderTemp;
		private readonly string FontDir = Path.Combine(Get.FolderTemp, "attachments");

		private readonly string FFmpeg = Path.Combine(Get.AppRootDir, "plugin", $"ffmpeg{Properties.Settings.Default.FFmpegArch}", "ffmpeg");
		private readonly string MkvExtract = Path.Combine(Get.AppRootDir, "plugin", $"mkvtoolnix", "mkvextract");
		private readonly string MkvMerge = Path.Combine(Get.AppRootDir, "plugin", $"mkvtoolnix", "mkvmerge");
		private readonly string Mp4Box = Path.Combine(Get.AppRootDir, "plugin", $"mp4box", "mp4box");

        private readonly string FontReg = Path.Combine(Get.AppRootDir, $"FontReg{(OS.Is64bit ? "64" : "32")}");

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
				ConsoleEx.Write(LogLevel.Normal, $"Clearing temp folder: {TempDir}\n");

				if (Directory.Exists(TempDir))
					Directory.Delete(TempDir, true);

				Directory.CreateDirectory(TempDir);

				// Prepare temp folder
				if (!Directory.Exists(FontDir))
					Directory.CreateDirectory(FontDir);
			}
			catch (Exception)
			{
				ConsoleEx.Write(LogLevel.Error, $"ERROR clearing temp folder: {TempDir}\n");
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
					File.Copy(item.File, Path.Combine(TempDir, $"subtitle{id++:D4}_{item.Lang}{Path.GetExtension(item.File)}"));
				}
				else
				{
					ProcessManager.Start(FFmpeg, $"-hide_banner -v error -stats -i \"{item.File}\" -map 0:{item.Id} -map_metadata -1 -map_chapters -1 -vn -an -dn -scodec copy -y subtitle{id++:D4}_{item.Lang}.{item.Format}");
				}
			}

			id = 1;
			foreach (var item in queue.Attachment)
			{
				if (item.Id == -1)
					File.Copy(item.File, Path.Combine(FontDir, Path.GetFileName(item.File)));

				if (item.Id >= 0)
					ProcessManager.Start2(FFmpeg, $"-hide_banner -v panic -dump_attachment:{item.Id} \"{item.Name}\" -i \"{item.File}\" -y", FontDir);

				ConsoleEx.Write(LogLevel.Normal, $"Extracted {id++} attachments!\r");
			}

			Console.Write('\n');

            if (queue.HardSub)
            {
                try
                {
                    if (Elevated.IsAdmin)
                    {
                        ConsoleEx.Write(LogLevel.Normal, "Copying embedded font to library for rendering Hard Sub!\n");
                        ProcessManager.Start2(FontReg, $"/copy", FontDir);
                    }
                    else
                    {
                        ConsoleEx.Write(LogLevel.Warning, "Will proceed rendering without custom font. (NO ADMIN)\n");
                    }
                }
                catch (Exception e)
                {
                    ConsoleEx.Write(LogLevel.Error, $"Unable to install font\n");
                    ConsoleEx.Write(LogLevel.Error, e.Message);
                }
            }

            if (!string.IsNullOrEmpty(queue.FilePath))
			{
				var ec = ProcessManager.Start(MkvExtract, $"chapters \"{queue.FilePath}\" > chapters.xml");
				if (ec >= 1)
					File.Delete(Path.Combine(TempDir, "chapters.xml"));
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

				if (Plugin.Items.TryGetValue(item.Encoder.Id, out codec))
				{
					var ac = codec.Audio;
                    var m = item.Encoder.Mode;

                    var trim = (queue.Trim.Enable ? $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}" : string.Empty);

                    var qfix = $"{ac.Mode[m].QualityPrefix}{item.Encoder.Quality}{ac.Mode[m].QualityPostfix}";

					var qu = (string.IsNullOrEmpty(ac.Mode[m].Args) ? string.Empty : $"{ac.Mode[m].Args} {qfix}");
					var hz = (item.Encoder.SampleRate == 0 ? string.Empty : $"-ar {item.Encoder.SampleRate}");
					var ch = (item.Encoder.Channel == 0 ? string.Empty : $"-ac {item.Encoder.Channel}");

					var newfile = (mode == ModeSave.Temp ? $"audio{id++:D4}_{item.Lang}.{ac.Extension}" : Path.Combine(SaveDir, $"{Path.GetFileNameWithoutExtension(queue.FilePath)}_ID{id++:D2}.{ac.Extension}"));

					if (ac.Args.Pipe)
					{
						ec = ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} -f wav {item.Command} -", Path.Combine(codec.FilePath, ac.Encoder), $"{qu} {ac.Args.Command} {ac.Args.Input} {item.Encoder.Command} {ac.Args.Output} \"{newfile}\"");
					}
					else
					{
						ec = ProcessManager.Start(Path.Combine(codec.FilePath, ac.Encoder), $"{ac.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} {ac.Args.Command} {qu} {hz} {ch} {item.Encoder.Command} {ac.Args.Output} \"{newfile}\"");
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
                if (Plugin.Items.TryGetValue(item.Encoder.Id, out Plugin codec))
                {
                    var vc = codec.Video;
                    var en = Path.Combine(codec.FilePath, vc.Encoder.Find(b => b.BitDepth == item.Quality.BitDepth).Binary);
                    var outfile = $"video{id++:D4}_{item.Lang}.{vc.Extension}";

                    var m = item.Encoder.Mode;

                    var trim = string.Empty;

                    var yuv = $"yuv{item.Quality.PixelFormat}p";
                    var res = string.Empty;
                    var fps = string.Empty;

                    var preset = string.Empty;
                    var tune = string.Empty;
                    var quality = string.Empty;
                    var bitdepth = string.Empty;
                    var framecount = string.Empty;

                    var vf = string.Empty;
                    var fi = new List<string>();

                    // cmd builder
                    if (queue.Trim.Enable)
                    {
                        trim += $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}";
                    }

                    if (item.Quality.BitDepth > 8)
                    {
                        yuv += $"{item.Quality.BitDepth}le";
                    }

                    if (item.Quality.FrameRate >= 5)
                    {
                        fps = $"-r {item.Quality.FrameRate}";
                    }

                    if (item.Quality.Width >= 128 && item.Quality.Height >= 128)
                    {
                        res = $"-s {item.Quality.Width}x{item.Quality.Height}";
                    }

                    if (!vc.Args.Preset.IsDisable() && !item.Encoder.Preset.IsDisable())
                    {
                        preset = $"{vc.Args.Preset} {item.Encoder.Preset}";
                    }

                    if (!vc.Args.Tune.IsDisable() && !item.Encoder.Tune.IsDisable())
                    {
                        tune = $"{vc.Args.Tune} {item.Encoder.Tune}";
                    }

                    if (!vc.Mode[m].Args.IsDisable())
                    {
                        quality = $"{vc.Mode[m].Args} {vc.Mode[m].Prefix}{item.Encoder.Value}{vc.Mode[m].Postfix}";
                    }

                    if (!vc.Args.BitDepth.IsDisable() && item.Quality.BitDepth >= 8)
                    {
                        bitdepth = $"{vc.Args.BitDepth} {item.Quality.BitDepth}";
                    }

                    if (!vc.Args.FrameCount.IsDisable())
                    {
                        if (item.Quality.FrameCount > 0)
                            framecount = $"{vc.Args.FrameCount} {item.Quality.FrameCount + Properties.Settings.Default.FrameCountOffset}";
                    }

                    // FFmpeg Video Filter
                    if (item.DeInterlace.Enable)
                    {
                        fi.Add($"yadif={item.DeInterlace.Mode}:{item.DeInterlace.Field}:0");
                    }

                    if (queue.HardSub)
                    {
                        var files = Directory.GetFiles(TempDir, "subtitle*");

                        if (files.Length > 0)
                        {
                            var file = Path.GetFileName(files[0]);
                            var ext = Get.FileExtension(file);

                            if (ext.IsOneOf(".srt"))
                            {
                                fi.Add($"subtitles={file}");
                            }
                            else if (ext.IsOneOf(".ass", ".ssa"))
                            {
                                fi.Add($"ass={file}");
                            }
                        }
                    }

                    if (fi.Count > 0)
                        vf = $"-vf \"{string.Join(", ", fi)}\"";

                    // begin encoding
                    if (vc.Mode[item.Encoder.Mode].MultiPass)
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

                            if (p == item.Encoder.MultiPass)
                                pass = vc.Args.PassLast;

                            ConsoleEx.Write(LogLevel.Normal, $"Multi-pass encoding: ");
                            ConsoleEx.Write(ConsoleColor.Green, $"{p} of {item.Encoder.MultiPass}\n");

                            if (vc.Args.Pipe)
                                ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -strict -1 {trim} -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {vf} {item.Quality.Command} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {pass} {item.Encoder.Command} {vc.Args.Output} {outfile}");
                            else
                                ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {vf} {vc.Args.UnPipe} {preset} {quality} {tune} {pass} {item.Encoder.Command} {vc.Args.Command} {vc.Args.Output} {outfile}");

                            p++;

                        } while (p <= item.Encoder.MultiPass);

                        // add pts for raw
                        VideoUnRaw(outfile);
                    }
                    else
                    {
                        if (vc.Args.Pipe)
                            ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -strict -1 {trim} -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {vf} {item.Quality.Command} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {framecount} {item.Encoder.Command} {vc.Args.Output} {outfile}");
                        else
                            ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {vf} {vc.Args.UnPipe} {preset} {quality} {tune} {item.Encoder.Command} {vc.Args.Output} {outfile}");

                        // add pts for raw
                        VideoUnRaw(outfile);
                    }
                }
            }

			return 0;
		}

		private int VideoUnRaw(string FileName)
		{
			var newname = Path.GetFileNameWithoutExtension(FileName) + ".mp4";
			var filetype = Path.GetExtension(FileName).ToLowerInvariant();
			var exitcode = 0;

			if (filetype.IsOneOf(".h265", ".hevc", ".h264", ".avc", ".h263", ".divx", ".xvid"))
				exitcode = ProcessManager.Start(Mp4Box, $"-add \"{FileName}#video:lang={Get.FileLang(FileName)}\" -new \"{newname}\"");
			else
				return 0;

			if (exitcode == 0)
			{
				ConsoleEx.Write(LogLevel.Normal, "Processing RAW MPEG video file OK!\n");
				if (File.Exists(Path.Combine(TempDir, FileName)))
					File.Delete(Path.Combine(TempDir, FileName));
			}
			else
			{
				ConsoleEx.Write(LogLevel.Warning, "Processing RAW MPEG video file failed, maybe broken or incompatible codec!\n");
				if (File.Exists(Path.Combine(TempDir, newname)))
					File.Delete(Path.Combine(TempDir, newname));
			}

			return exitcode;
		}

		private int VideoMuxing(MediaQueue queue)
		{
			ConsoleEx.Write(LogLevel.Normal, "Merging RAW files as single media file\n");

			if (queue.OutputFormat == TargetFormat.MKV)
			{
				// get all media
				var videos = string.Empty;
				var audios = string.Empty;
				var subtitles = string.Empty;
				var attachments = string.Empty;
				var chapter = string.Empty;

				// generate tags
				File.WriteAllText(Path.Combine(TempDir, "tags.xml"), string.Format(Properties.Resources.MkvTags, Get.AppNameLong, Get.AppNameLib));

				// add raw files
				foreach (var video in Directory.GetFiles(TempDir, "video*"))
					videos += $"--language 0:{Get.FileLang(video)} \"{video}\" ";

				foreach (var audio in Directory.GetFiles(TempDir, "audio*"))
					audios += $"--language 0:{Get.FileLang(audio)} \"{audio}\" ";

				foreach (var sub in Directory.GetFiles(TempDir, "subtitle*"))
					subtitles += $"--sub-charset 0:UTF-8 --language 0:{Get.FileLang(sub)} \"{sub}\" ";

				for (int i = 0; i < queue.Attachment.Count; i++)
				{
					var file = Path.Combine(FontDir, queue.Attachment[i].Name);
					var mime = queue.Attachment[i].Mime;

					if (File.Exists(file))
						attachments += $"--attachment-mime-type \"{mime}\" --attachment-description yes --attach-file \"{file}\" ";
				}

				if (File.Exists(Path.Combine(TempDir, "chapters.xml")))
				{
					FileInfo ChapLen = new FileInfo(Path.Combine(TempDir, "chapters.xml"));
					if (ChapLen.Length > 256)
						chapter = $"--chapters \"{Path.Combine(TempDir, "chapters.xml")}\"";
				}

				var command = $"{videos}{audios}{subtitles}{attachments}{chapter}";
				var exitcode = ProcessManager.Start(MkvMerge, $"-o \"{Get.NewFilePath(SaveDir, queue.FilePath, ".mkv")}\" --disable-track-statistics-tags -t 0:\"{Path.Combine(TempDir, "tags.xml")}\" {command}");

				// if mux fail, copy raw to destination
				if (exitcode == 2)
				{
					ConsoleEx.Write(LogLevel.Error, "Damn! Video encoder make a mistake! Not my fault!\n");
					Get.DirectoryCopy(TempDir, Get.NewFilePath(SaveDir, queue.FilePath, ".raw"), true);
				}

				return exitcode;
			}
			else
			{
				// get all media
				var videos = string.Empty;
				var audios = string.Empty;

				var index = 0;
				var metadata = string.Empty;
				var command = string.Empty;

				var extra = $"-vcodec copy -acodec copy -scodec copy -map_metadata -1 -map_chapters -1 -metadata APP=\"{Get.AppNameLong}\" -metadata VER=\"{Get.AppNameLib}\" ";

				// find files
				foreach (var video in Directory.GetFiles(TempDir, "video*"))
				{
					videos += $"-i \"{Path.GetFileName(video)}\" ";
					metadata += $"-metadata:s:{index++} language={Get.FileLang(video)} ";
				}

				foreach (var audio in Directory.GetFiles(TempDir, "audio*"))
				{
					audios += $"-i \"{Path.GetFileName(audio)}\" ";
					metadata += $"-metadata:s:{index++} language={Get.FileLang(audio)} ";
				}

				// build command
				if (queue.OutputFormat == TargetFormat.MP4)
					command = $"{videos}{audios}{extra}{metadata}-y \"{Get.NewFilePath(SaveDir, queue.FilePath, ".mp4")}\"";
				else if (queue.OutputFormat == TargetFormat.WEBM)
					command = $"{videos}{audios}{extra}{metadata}-y \"{Get.NewFilePath(SaveDir, queue.FilePath, ".webm")}\"";

				// run command
				var exitcode = ProcessManager.Start(FFmpeg, $"-hide_banner -v error -stats {command}");

				// if failed
				if (exitcode > 0)
				{
					ConsoleEx.Write(LogLevel.Error, "Damn! Video encoder make a mistake! Not my fault!\n");
					Get.DirectoryCopy(TempDir, Get.NewFilePath(SaveDir, queue.FilePath, ".raw"), true);
				}

				return exitcode;
			}
		}
	}
}
