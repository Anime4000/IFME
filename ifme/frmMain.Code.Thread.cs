using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

namespace ifme
{
	partial class frmMain
	{
		readonly StringComparison IC = StringComparison.InvariantCultureIgnoreCase;
		readonly string tempDir = Properties.Settings.Default.TempDir;
		readonly string outDir = Properties.Settings.Default.OutputDir;

		private void bgThread_DoWork(object sender, DoWorkEventArgs e)
		{
			var args = e.Argument as List<MediaQueue>;

			var FFmpeg = Path.Combine(Get.AppRootFolder, "plugin", $"ffmpeg{Properties.Settings.Default.FFmpegArch}", "ffmpeg");
			var MkvExtract = Path.Combine(Get.AppRootFolder, "plugin", $"mkvtoolnix", "mkvextract");
			var MkvMerge = Path.Combine(Get.AppRootFolder, "plugin", $"mkvtoolnix", "mkvmerge");
			var Mp4Box = Path.Combine(Get.AppRootFolder, "plugin", $"mp4box", "mp4box");

			foreach (var queue in args)
			{
				// Clean temp folder
				try
				{
					Console.WriteLine($"Clearing temp folder: {tempDir}");

					foreach (var files in Directory.GetFiles(tempDir))
						File.Delete(files);

					foreach (var folders in Directory.GetDirectories(tempDir))
						Directory.Delete(folders);

					// Prepare temp folder
					if (!Directory.Exists(Path.Combine(tempDir, "attachments")))
						Directory.CreateDirectory(Path.Combine(tempDir, "attachments"));
				}
				catch (Exception)
				{
					Console.WriteLine($"ERROR clearing temp folder: {tempDir}");
					e.Cancel = true;
					return;
				}

				// Extract
				var subId = 0;
				foreach (var item in queue.Subtitle)
				{
					if (item.Id < 0)
					{
						File.Copy(item.File, Path.Combine(tempDir, $"subtitle{subId++:D4}_{item.Lang}{Path.GetExtension(item.File)}"));
					}
					else
					{
						ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -i \"{item.File}\" -map 0:{item.Id} -y subtitle{subId++:D4}_{item.Lang}.{item.Format}");
					}
				}

				foreach (var item in queue.Attachment)
				{
					File.Copy(item.File, Path.Combine(tempDir, "attachments" ,Path.GetFileName(item.File)));
				}

				if (!string.IsNullOrEmpty(queue.File))
				{
					ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -dump_attachment:t \"\" -i \"{queue.File}\" -y", Path.Combine(tempDir, "attachments"));

					var ec = ProcessManager.Start(MkvExtract, $"chapters \"{queue.File}\" > chapters.xml");
					if (ec >= 1)
						File.Delete(Path.Combine(tempDir, "chapters.xml"));
				}

				// Audio
				var audId = 0;
				foreach (var item in queue.Audio)
				{
					Plugin codec;

					if (Plugin.Items.TryGetValue(item.Encoder, out codec))
					{
						var ac = codec.Audio;

						if (ac.Args.Pipe)
						{
							ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel quiet -i \"{item.File}\" -map 0:{item.Id} -acodec pcm_s16le -ar {item.EncoderSampleRate} -ac {item.EncoderChannel} -f wav -", Path.Combine(codec.Path, ac.Encoder), $"{ac.Mode[item.EncoderMode].Args} {item.EndoderQuality} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} audio{audId++:D4}_{item.Lang}.{ac.Extension}");
						}
						else
						{
							ProcessManager.Start(Path.Combine(codec.Path, ac.Encoder), $"{ac.Args.Input} \"{item.File}\" {ac.Args.Command} {ac.Mode[item.EncoderMode].Args} {item.EndoderQuality} {ac.Args.Output} audio{audId++:D4}_{item.Lang}.{ac.Extension}");
						}
					}
				}

				// Video
				var vidId = 0;
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

									ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel panic -i \"{item.File}\" -f yuv4mpegpipe -pix_fmt yuv{item.PixelFormat}p{(item.BitDepth == 8 ? "" : $"{item.BitDepth}le")} -strict -1 -s {item.Width}x{item.Height} -r {item.FrameRate} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {vc.Args.Preset} {item.EncoderPreset} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.BitDepth} {item.BitDepth} {vc.Args.FrameCount} {item.FrameCount} {pass} {vc.Args.Output} video{vidId++:D4}_{item.Lang}.{vc.Extension}");
								}
							}
							else
							{
								ProcessManager.Start(FFmpeg, $"-hide_banner -loglevel panic -i \"{item.File}\" -f yuv4mpegpipe -pix_fmt yuv{item.PixelFormat}p{(item.BitDepth == 8 ? "" : $"{item.BitDepth}le")} -strict -1 -s {item.Width}x{item.Height} -r {item.FrameRate} -", en, $"{vc.Args.Input} {vc.Args.Y4M} {vc.Args.Preset} {item.EncoderPreset} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.BitDepth} {item.BitDepth} {vc.Args.FrameCount} {item.FrameCount} {vc.Args.Output} video{vidId++:D4}_{item.Lang}.{vc.Extension}");
							}
						}
						else
						{
							ProcessManager.Start(en, $"{vc.Args.Input} \"{item.File}\" {vc.Args.UnPipe} {(string.IsNullOrEmpty(vc.Args.Preset) ? "" : $"{vc.Args.Preset} {item.EncoderPreset}")} {vc.Mode[item.EncoderMode].Args} {item.EncoderValue} {vc.Args.Tune} {item.EncoderTune} {vc.Args.Output} video{vidId++:D4}_{item.Lang}.{vc.Extension}");
						}
					}
				}

				// Mux
				var fileout = Path.Combine(Properties.Settings.Default.OutputDir, $"[IFME] {Path.GetFileNameWithoutExtension(queue.File)}");

				if (string.Equals(queue.OutputFormat, "mp4"))
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

					ProcessManager.Start(Mp4Box, $"{cmdvideo} {cmdaudio} -itags tool=\"Internet Friendly Media Encoder\" -new \"{fileout}.mp4\"");
				}
				else if (string.Equals(queue.OutputFormat, "mkv"))
				{
					string cmdvideo = string.Empty;
					string cmdaudio = string.Empty;
					string cmdsubs = string.Empty;
					string cmdattach = string.Empty;
					string cmdchapter = string.Empty;

					foreach (var video in Directory.GetFiles(tempDir, "video*"))
					{
						cmdvideo += $"--language 0:{Get.FileLang(video)} \"{video}\" ";
					}

					foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
					{
						cmdaudio += $"--language 0:{Get.FileLang(audio)} \"{audio}\" ";
					}

					foreach (var subs in Directory.GetFiles(tempDir, "subtitle*"))
					{
						cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{Get.FileLang(subs)} \"{subs}\" ";
					}

					foreach (var attach in Directory.GetFiles(tempDir, "*.*")
						.Where(s =>
					   s.EndsWith(".ttf", IC) ||
					   s.EndsWith(".otf", IC) ||
					   s.EndsWith(".ttc", IC) ||
					   s.EndsWith(".woff", IC)))
					{
						cmdattach += $"--attach-file \"{attach}\" ";
					}

					if (File.Exists(Path.Combine(tempDir, "chapters.xml")))
					{
						FileInfo ChapLen = new FileInfo(Path.Combine(tempDir, "chapters.xml"));
						if (ChapLen.Length > 256)
							cmdchapter = $"--chapters \"{Path.Combine(tempDir, "chapters.xml")}\"";
					}

					ProcessManager.Start(MkvMerge, $"-o \"{fileout}.mkv\" --disable-track-statistics-tags {cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}");
				}
				else if (string.Equals(queue.OutputFormat, "webm"))
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
				else
				{

				}
			}
		}

		private void bgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			btnStart.Enabled = true;
			btnStop.Enabled = false;
			btnPause.Enabled = false;
		}
	}	
}