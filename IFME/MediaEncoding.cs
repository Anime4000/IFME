using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFME.OSManager;

namespace IFME
{
	internal class MediaEncoding
	{
		private static int Arch = OS.Is64bit ? 64 : 32;
		internal static string FFmpeg = Path.Combine(Environment.CurrentDirectory, "Plugins", $"ffmpeg{Arch}", "ffmpeg");
		internal static string MP4Box = Path.Combine(Environment.CurrentDirectory, "Plugins", "MP4Box", "MP4Box");

		internal static void Extract(MediaQueue queue, string tempDir)
		{
			Console2.WriteLine("[INFO] Extracting subtitle file...");
			for (int i = 0; i < queue.Subtitle.Count; i++)
			{
				var id = queue.Subtitle[i].Id;
				var fmt = queue.Subtitle[i].Codec;
				var file = queue.Subtitle[i].File;
				var lang = queue.Subtitle[i].Lang;
				var fext = Path.GetExtension(file);

				if (id < 0)
				{
					File.Copy(file, Path.Combine(tempDir, $"subtitle{i:D4}_{lang}{fext}"));
				}
				else
				{
					ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{file}\" -map 0:{id} -map_metadata -1 -map_chapters -1 -vn -an -dn -scodec copy -y subtitle{i:D4}_{lang}.{fmt}");
				}
			}

			Console2.WriteLine("[INFO] Extracting embeded attachment...");
			var tempDirFont = Path.Combine(tempDir, "attachment");
			for (int i = 0; i < queue.Attachment.Count; i++)
			{
				var id = queue.Attachment[i].Id;
				var file = queue.Attachment[i].File;
				var name = queue.Attachment[i].Name;

				if (!Directory.Exists(tempDirFont))
					Directory.CreateDirectory(tempDirFont);
				
				if (id < 0)
				{
					File.Copy(file, Path.Combine(tempDirFont, Path.GetFileName(file)));
				}
				else
				{
					ProcessManager.Start(tempDirFont, $"\"{FFmpeg}\" -hide_banner -v panic -stats -dump_attachment:{id} {name} -i \"{file}\" -y");
				}
			}

			// Hard Sub

			// Chapters
			ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{queue.FilePath}\" -f ffmetadata metadata.ini -y");
		}

		internal static void Audio(MediaQueue queue, string tempDir)
		{
			var i = 0;
			foreach (var item in queue.Audio)
			{
				Console2.WriteLine("[INFO] Encoding audio file...");
				if (Plugins.Items.Audio.TryGetValue(item.Encoder.Id, out PluginsAudio codec))
				{
					var ac = codec.Audio;
					var en = Path.Combine(codec.FilePath, ac.Encoder);
					var m = item.Encoder.Mode;

					var trim = (queue.Trim.Enable ? $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}" : string.Empty);

					var qfix = $"{ac.Mode[m].QualityPrefix}{item.Encoder.Quality}{ac.Mode[m].QualityPostfix}";

					var qu = (string.IsNullOrEmpty(ac.Mode[m].Args) ? string.Empty : $"{ac.Mode[m].Args} {qfix}");
					var hz = (item.Encoder.SampleRate == 0 ? string.Empty : $"-ar {item.Encoder.SampleRate}");
					var ch = (item.Encoder.Channel == 0 ? string.Empty : $"-ac {item.Encoder.Channel}");

					var outfile = $"audio{i++:D4}_{item.Lang}.{ac.Extension}";

					if (ac.Args.Pipe)
					{
						ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} -f wav {item.Command} - | \"{Path.Combine(codec.FilePath, ac.Encoder)}\" {qu} {ac.Args.Command} {ac.Args.Input} {item.Encoder.Command} {ac.Args.Output} \"{outfile}\"");
					}
					else
					{
						ProcessManager.Start(tempDir, $"\"{en}\" {ac.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} {ac.Args.Command} {qu} {hz} {ch} {item.Encoder.Command} {ac.Args.Output} \"{outfile}\"");
					}
				}
			}
		}

		internal static void Video(MediaQueue queue, string tempDir)
		{
			var original_data = new FFmpeg.MediaInfo(queue.FilePath);

			for (int i = 0; i < queue.Video.Count; i++)
			{
				var item = queue.Video[i];

				if (Plugins.Items.Video.TryGetValue(item.Encoder.Id, out PluginsVideo codec))
				{
					var vc = codec.Video;
					var en = Path.Combine(codec.FilePath, vc.Encoder.Find(b => b.BitDepth == item.Quality.BitDepth).Binary);
					var outrawfile = $"raw-v{i:D4}_{item.Lang}.{vc.Extension}";
					var outfmtfile = $"video{i:D4}_{item.Lang}.{codec.Format[0]}";

					var m = item.Encoder.Mode;

					var trim = string.Empty;

					var yuv = $"yuv{item.Quality.PixelFormat}p";
					var res = string.Empty;
					var fps = $"-r 23.976";

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
					else
					{
						res = $"-s {original_data.Video[i].Width}x{original_data.Video[i].Height}";
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
							framecount = $"{vc.Args.FrameCount} {item.Quality.FrameCount}";
					}

					// FFmpeg Video Filter
					if (item.DeInterlace.Enable)
					{
						fi.Add($"yadif={item.DeInterlace.Mode}:{item.DeInterlace.Field}:0");
					}

					if (queue.HardSub)
					{
						var files = Directory.GetFiles(tempDir, "subtitle*");

						if (files.Length > 0)
						{
							var file = Path.GetFileName(files[0]);
							var ext = Path.GetExtension(file).ToLowerInvariant();

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
					Console2.WriteLine($"[INFO] Encoding video file...");

					if (vc.Mode[item.Encoder.Mode].MultiPass)
					{
						var p = 1;
						var pass = string.Empty;

						Console2.Write("[WARN] Frame count is disable for Multi-pass encoding, Avoid inconsistent across multi-pass.");

						do
						{
							pass = vc.Args.PassNth;

							if (p == 1)
								pass = vc.Args.PassFirst;

							if (p == item.Encoder.MultiPass)
								pass = vc.Args.PassLast;

							Console2.WriteLine($"[INFO] Multi-pass encoding: {p} of {item.Encoder.MultiPass}");

							if (vc.Args.Pipe)
								ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -strict -1 {trim} -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {vf} {item.Quality.Command} - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {pass} {item.Encoder.Command} {vc.Args.Output} {outrawfile}");
							else
								ProcessManager.Start(tempDir, $"\"{en}\" {vc.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {vf} {vc.Args.UnPipe} {preset} {quality} {tune} {pass} {item.Encoder.Command} {vc.Args.Command} {vc.Args.Output} {outrawfile}");

							++p;

						} while (p <= item.Encoder.MultiPass);
					}
					else
					{
						if (vc.Args.Pipe)
							ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -strict -1 {trim} -map 0:{item.Id} -f yuv4mpegpipe -pix_fmt {yuv} {res} {fps} {vf} {item.Quality.Command} - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {preset} {quality} {tune} {bitdepth} {framecount} {item.Encoder.Command} {vc.Args.Output} {outrawfile}");
						else
							ProcessManager.Start(tempDir, $"\"{en}\" {vc.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} -pix_fmt {yuv} {res} {fps} {vf} {vc.Args.UnPipe} {preset} {quality} {tune} {item.Encoder.Command} {vc.Args.Output} {outrawfile}");
					}

					// Raw file dont have pts (time), need to remux
					Console2.WriteLine($"[INFO] Restructure RAW video file...");

					if (vc.RawOutput)
					{
						if (File.Exists(Path.Combine(tempDir, outrawfile)))
						{
							ProcessManager.Start(tempDir, $"\"{MP4Box}\" -add \"{outrawfile}\" -set-meta 0 -new \"{outfmtfile}\" ");
							File.Delete(Path.Combine(tempDir, outrawfile));
						}
					}
					else
					{
						if (File.Exists(Path.Combine(tempDir, outrawfile)))
						{
							File.Move(Path.Combine(tempDir, outrawfile), Path.Combine(tempDir, outfmtfile));
						}
					}
				}
			}
		}

		internal static void Muxing(MediaQueue queue, string tempDir, string saveDir)
		{
			var x = 0;
			var metadata = string.Empty;
			var metafile = string.Empty;
			var map = string.Empty;

			var argVideo = string.Empty;
			var argAudio = string.Empty;
			var argSubtitle = string.Empty;
			var argEmbed = string.Empty;

			var outFile = Path.Combine(saveDir, $"{Path.GetFileNameWithoutExtension(queue.FilePath)}_encoded.{queue.OutputFormat.ToString().ToLowerInvariant()}");

			Console2.WriteLine($"[INFO] Multiplexing encoded files into single file...");

			if (File.Exists(Path.Combine(tempDir, "metadata.ini")))
			{
				metafile = "-i metadata.ini ";
			}

			foreach (var video in Directory.GetFiles(tempDir, "video*"))
			{
				argVideo += $"-i \"{Path.GetFileName(video)}\" ";
				metadata += $"-metadata:s:{x} language={video.GetLanguageCodeFromFileName()}  ";
				map += $"-map {x} ";
				x++;
			}

			foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
			{
				argAudio += $"-i \"{Path.GetFileName(audio)}\" ";
				metadata += $"-metadata:s:{x} language={audio.GetLanguageCodeFromFileName()} ";
				map += $"-map {x} ";
				x++;
			}

			if (queue.OutputFormat == MediaContainer.MKV)
			{
				var d = 0;
				foreach (var subtitle in Directory.GetFiles(tempDir, "subtitle*"))
				{
					argSubtitle += $"-i \"{Path.GetFileName(subtitle)}\" ";
					metadata += $"-metadata:s:{x} language={subtitle.GetLanguageCodeFromFileName()} {(d == 0 ? $"-disposition:s:{d} default " : "")}";
					map += $"-map {x} ";
					x++;
					d++;
				}

				var tempDirFont = Path.Combine(tempDir, "attachment");
				if (Directory.Exists(tempDirFont))
				{
					var files = Directory.GetFiles(tempDirFont, "*");
					for (int i = 0; i < files.Length; i++)
					{
						argEmbed += $"-attach \"{Path.GetFileName(files[i])}\" ";
						metadata += $"-metadata:s:{x++} \"mimetype={queue.Attachment[i].Mime}\" ";
					}
				}
			}

			var author = $"{Version.Name} v{Version.Release} {Version.OSPlatform} {Version.OSArch} {Version.March} @ {Version.CodeName}";
			var command = $"\"{FFmpeg}\" -hide_banner -v error -stats {argVideo}{argAudio}{argSubtitle}{argEmbed}{metafile}{map}{metadata}-metadata \"encoded_by={author}\" -c copy -y \"{outFile}\"";
			ProcessManager.Start(tempDir, command);
		}
	}
}
