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

			if (!string.IsNullOrEmpty(queue.File))
			{
				var ec = ProcessManager.Start(MkvExtract, $"chapters \"{queue.File}\" > chapters.xml");
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

				if (Plugin.Items.TryGetValue(item.Encoder, out codec))
				{
					var ac = codec.Audio;

					var qu = (string.IsNullOrEmpty(ac.Mode[item.EncoderMode].Args) ? string.Empty : $"{ac.Mode[item.EncoderMode].Args} {item.EncoderQuality}");
					var hz = (item.EncoderSampleRate == 0 ? string.Empty : $"-ar {item.EncoderSampleRate}");
					var ch = (item.EncoderChannel == 0 ? string.Empty : $"-ac {item.EncoderChannel}");

					var newfile = (mode == ModeSave.Temp ? $"audio{id++:D4}_{item.Lang}.{ac.Extension}" : Path.Combine(SaveDir, $"{Path.GetFileNameWithoutExtension(queue.File)}_ID{id++:D2}.{ac.Extension}"));

					if (ac.Args.Pipe)
					{
						ec = ProcessManager.Start(FFmpeg, $"-hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} -f wav -", Path.Combine(codec.FilePath, ac.Encoder), $"{qu} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} \"{newfile}\"");
					}
					else
					{
						ec = ProcessManager.Start(Path.Combine(codec.FilePath, ac.Encoder), $"{ac.Args.Input} \"{item.File}\" -map 0:{item.Id} -map_metadata -1 -map_chapters -1 {ac.Args.Command} {qu} {ac.Args.Output} \"{newfile}\"");
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
						if (item.FrameCount > 0)
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
								ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" -map 0:{item.Id} -map_metadata -1 -map_chapters -1 -pix_fmt {yuv} {res} {fps} {deinterlace} {vc.Args.UnPipe} {preset} {quality} {tune} {pass} {item.EncoderCommand} {vc.Args.Output} {outfile}");

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
							ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" -map 0:{item.Id} -map_metadata -1 -map_chapters -1 -pix_fmt {yuv} {res} {fps} {deinterlace} {vc.Args.UnPipe} {preset} {quality} {tune} {item.EncoderCommand} {vc.Args.Output} {outfile}");
						}
					}
				}
			}

			return 0;
		}

		private int VideoMuxing(MediaQueue queue)
		{
			ConsoleEx.Write(LogLevel.Normal, "Merging RAW files as single media file\n");

			// get all media
			var videos = string.Empty;
			var audios = string.Empty;
			var subtitles = string.Empty;
			var attachments = string.Empty;

			var index = 0;
			var metadata = string.Empty;
			var command = string.Empty;

			var extra = $"-vcodec copy -acodec copy -scodec copy -metadata application=\"{Get.AppNameLong}\" -metadata version=\"{Get.AppNameLib}\" ";

			// find files
			foreach (var video in Directory.GetFiles(TempDir, "video*"))
			{
				videos += $"-i \"{Path.Combine("..", Path.GetFileName(video))}\" ";
				metadata += $"-metadata:s:{index++} language={Get.FileLang(video)} ";
			}

			foreach (var audio in Directory.GetFiles(TempDir, "audio*"))
			{
				audios += $"-i \"{Path.Combine("..", Path.GetFileName(audio))}\" ";
				metadata += $"-metadata:s:{index++} language={Get.FileLang(audio)} ";
			}

			foreach (var sub in Directory.GetFiles(TempDir, "subtitle*"))
			{
				subtitles += $"-i \"{Path.Combine("..", Path.GetFileName(sub))}\" ";
				metadata += $"-metadata:s:{index++} language={Get.FileLang(sub)} ";
			}

			// build command
			if (queue.OutputFormat == TargetFormat.MKV)
			{
				for (int i = 0; i < queue.Attachment.Count; i++)
				{
					var file = Path.Combine(FontDir, queue.Attachment[i].Name);
					var mime = queue.Attachment[i].Mime;

					if (File.Exists(file))
					{
						attachments += $"-attach \"{Path.GetFileName(file)}\" ";
						metadata += $"-metadata:s:{index++} mimetype=\"{mime}\" ";
					}
				}

				command = $"{videos}{audios}{subtitles}{attachments}{extra}{metadata}-y \"{Get.NewFilePath(SaveDir, queue.File, ".mp4")}\"";
			}
			else if (queue.OutputFormat == TargetFormat.MP4)
			{
				command = $"{videos}{audios}{subtitles}{extra}{metadata}-y \"{Get.NewFilePath(SaveDir, queue.File, ".mkv")}\"";
			}
			else if (queue.OutputFormat == TargetFormat.WEBM)
			{
				command = $"{videos}{audios}{subtitles}{extra}{metadata}-y \"{Get.NewFilePath(SaveDir, queue.File, ".webm")}\"";
			}

			// run command
			var ec = ProcessManager.Start(FFmpeg, $"-hide_banner -v error -stats {command}", FontDir);

			// if failed
			if (ec > 0)
			{
				ConsoleEx.Write(LogLevel.Error, "Damn! Video encoder make a mistake! Not my fault!\n");
				Get.DirectoryCopy(TempDir, Get.NewFilePath(SaveDir, queue.File, ".raw"), true);
			}

			return 0;
		}
	}
}
