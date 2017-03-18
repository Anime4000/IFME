using System;
using System.Collections.Generic;
using System.IO;

namespace ifme
{
	class MediaEncoding
	{
		private readonly string tempDir = Get.FolderTemp;
		private readonly string saveDir = Get.FolderSave;

		private readonly string FFmpeg = Path.Combine(Get.AppRootFolder, "plugin", $"ffmpeg{Properties.Settings.Default.FFmpegArch}", "ffmpeg");
		private readonly string MkvExtract = Path.Combine(Get.AppRootFolder, "plugin", $"mkvtoolnix", "mkvextract");
		private readonly string MkvMerge = Path.Combine(Get.AppRootFolder, "plugin", $"mkvtoolnix", "mkvmerge");
		private readonly string Mp4Box = Path.Combine(Get.AppRootFolder, "plugin", $"mp4box", "mp4box");

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
				Console.WriteLine($"\n\nClearing temp folder: {tempDir}");

				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, true);

				Directory.CreateDirectory(tempDir);

				// Prepare temp folder
				if (!Directory.Exists(Path.Combine(tempDir, "attachments")))
					Directory.CreateDirectory(Path.Combine(tempDir, "attachments"));
			}
			catch (Exception)
			{
				Console.WriteLine($"ERROR clearing temp folder: {tempDir}");
				return;
			}

			if (queue.OutputFormat.IsOneOf(TargetFormat.MP3, TargetFormat.M4A, TargetFormat.OGG, TargetFormat.OPUS, TargetFormat.FLAC))
			{
				AudioEncoding(queue, ModeSave.Direct);
			}
			else
			{
				// Extract
				VideoExtract(queue);

				// Audio
				AudioEncoding(queue, ModeSave.Temp);

				// Video
				VideoEncoding(queue);

				// Mux
				VideoMuxing(queue);
			}

			Console.WriteLine("Yay! All media done encoding...");
		}

		private int VideoExtract(MediaQueue queue)
		{
			var id = 0;
			foreach (var item in queue.Subtitle)
			{
				if (item.Id < 0)
				{
					File.Copy(item.File, Path.Combine(tempDir, $"subtitle{id++:D4}_{item.Lang}{Path.GetExtension(item.File)}"));
				}
				else
				{
					ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -i \"{item.File}\" -map 0:{item.Id} -y subtitle{id++:D4}_{item.Lang}.{item.Format}");
				}
			}

			foreach (var item in queue.Attachment)
			{
				File.Copy(item.File, Path.Combine(tempDir, "attachments", Path.GetFileName(item.File)));
			}

			if (!string.IsNullOrEmpty(queue.File))
			{
				ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -dump_attachment:t \"\" -i \"{queue.File}\" -y", Path.Combine(tempDir, "attachments"));

				var ec = ProcessManager.Start(MkvExtract, $"chapters \"{queue.File}\" > chapters.xml");
				if (ec >= 1)
					File.Delete(Path.Combine(tempDir, "chapters.xml"));
			}

			return 0;
		}

		private int AudioEncoding(MediaQueue queue, ModeSave mode)
		{
			var ec = 0;
			var id = 0;

			foreach (var item in queue.Audio)
			{
				Plugin codec;

				if (Plugin.Items.TryGetValue(item.Encoder, out codec))
				{
					var ac = codec.Audio;

					var newfile = (mode == ModeSave.Temp ? $"audio{id++:D4}_{item.Lang}.{ac.Extension}" : Path.Combine(saveDir, $"{Path.GetFileNameWithoutExtension(queue.File)}_ID{id++:D2}.{ac.Extension}"));

					if (ac.Args.Pipe)
					{
						ec = ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -i \"{item.File}\" -map 0:{item.Id} -acodec pcm_s16le -ar {item.EncoderSampleRate} -ac {item.EncoderChannel} -f wav -", Path.Combine(codec.Path, ac.Encoder), $"{ac.Mode[item.EncoderMode].Args} {item.EndoderQuality} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} \"{newfile}\"");
					}
					else
					{
						ec = ProcessManager.Start(Path.Combine(codec.Path, ac.Encoder), $"{ac.Args.Input} \"{item.File}\" -map 0:{item.Id} {ac.Args.Command} {ac.Mode[item.EncoderMode].Args} {item.EndoderQuality} {ac.Args.Output} \"{newfile}\"");
					}

					if (ec == 0)
						Console.WriteLine("Audio encoding OK!");
					else
						Console.WriteLine("Audio encoding might have a problem!");
				}
			}

			return ec;
		}

		private int VideoEncoding(MediaQueue queue)
		{
			var id = 0;
			foreach (var item in queue.Video)
			{
				Plugin codec;

				if (Plugin.Items.TryGetValue(item.Encoder, out codec))
				{
					var vc = codec.Video;
					var en = Path.Combine(codec.Path, vc.Encoder.Find(b => b.BitDepth == item.BitDepth).Binary);

					if (vc.Args.Pipe)
					{
						if (vc.Mode[item.EncoderMode].MultiPass)
						{
							for (int i = 0; i < item.EncoderMultiPass; i++)
							{
								var pass = string.Empty;

								if (i == 0)
									pass = vc.Args.PassFirst;
								else if (i == (item.EncoderMultiPass - 1))
									pass = vc.Args.PassLast;
								else
									pass = vc.Args.PassNth;

								ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel panic -i \"{item.File}\" -f yuv4mpegpipe -pix_fmt yuv{item.PixelFormat}p{(item.BitDepth == 8 ? "" : $"{item.BitDepth}le")} -strict -1 -s {item.Width}x{item.Height} -r {item.FrameRate} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {vc.Args.Preset} {item.EncoderPreset} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.BitDepth} {item.BitDepth} {vc.Args.FrameCount} {item.FrameCount} {pass} {vc.Args.Output} video{id++:D4}_{item.Lang}.{vc.Extension}");
							}
						}
						else
						{
							ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel panic -i \"{item.File}\" -f yuv4mpegpipe -pix_fmt yuv{item.PixelFormat}p{(item.BitDepth == 8 ? "" : $"{item.BitDepth}le")} -strict -1 -s {item.Width}x{item.Height} -r {item.FrameRate} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {vc.Args.Preset} {item.EncoderPreset} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.BitDepth} {item.BitDepth} {vc.Args.FrameCount} {item.FrameCount} {vc.Args.Output} video{id++:D4}_{item.Lang}.{vc.Extension}");
						}
					}
					else
					{
						ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" {vc.Args.UnPipe} {(string.IsNullOrEmpty(vc.Args.Preset) ? "" : $"{vc.Args.Preset} {item.EncoderPreset}")} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.Output} video{id++:D4}_{item.Lang}.{vc.Extension}");
					}
				}
			}

			return 0;
		}

		private int VideoMuxing(MediaQueue queue)
		{
			var fileout = Path.Combine(saveDir, $"[IFME] {Path.GetFileNameWithoutExtension(queue.File)}");

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

				ProcessManager.Start(Mp4Box, $"{cmdvideo} {cmdaudio} -itags tool=\"{Get.AppNameLong}\" -new \"{fileout}.mp4\"");
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
					cmdattach += $"--attach-file \"{attach}\" ";
				
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
					Console.WriteLine("MkvToolNix exit perfectly on first attempt!");
				}
				else if (exitcode == 1)
				{
					Console.WriteLine("MkvToolNix exit with warning on first attempt, it should work.");
				}
				else
				{
					Console.WriteLine("MkvToolNix can't merge on first attempt, trying to skip adding tags, chapters & fonts!");

					// try without chapter and fonts
					cmd = $"{cmdvideo} {cmdaudio} {cmdsubs}";
					exitcode = ProcessManager.Start(MkvMerge, $"-o \"{fileout}.mkv\" --disable-track-statistics-tags {cmd}");

					if (exitcode == 0)
					{
						Console.WriteLine("MkvToolNix merge perfectly without tags, chapters & fonts on second attempt!");
					}
					else if (exitcode == 1)
					{
						Console.WriteLine("MkvToolNix merge with warning on second attempt even without tags, chapters & fonts, it should work.");
					}
					else
					{
						// copy whole thing
						Console.WriteLine("MkvToolNix still can't merge on second attempt! BACKUP ALL RAW FILE TO SAVE FOLDER!");
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

				ProcessManager.Start(FFmpeg, $"{cmdvideo} {cmdaudio} -vcodec copy -acodec copy -y \"{fileout}.webm\"");
			}

			return 0;
		}
	}
}
