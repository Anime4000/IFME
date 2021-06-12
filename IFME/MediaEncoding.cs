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
		internal static string MP4Box = Path.Combine(Environment.CurrentDirectory, "Plugins", "mp4box", "mp4box");
		internal static int CurrentIndex = 0;

		internal static void Extract(MediaQueue queue, string tempDir)
		{
			frmMain.PrintStatus("Extracting...");

			frmMain.PrintLog("[INFO] Extracting subtitle file...");

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

			frmMain.PrintLog("[INFO] Extracting embeded attachment...");
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
			for (int i = 0; i < queue.Audio.Count; i++)
			{
				var item = queue.Audio[i];

				frmMain.PrintStatus($"Encoding, Audio #{i}");

				if (item.Copy && queue.OutputFormat == MediaContainer.MKV)
				{
					frmMain.PrintLog("[INFO] Extract audio file...");

					ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -c:a copy -y \"audio{i:D4}_{item.Lang}.mka\"");
					continue;
				}

				if (Plugins.Items.Audio.TryGetValue(item.Encoder.Id, out PluginsAudio codec))
				{
					frmMain.PrintLog("[INFO] Encoding audio file...");

					var ac = codec.Audio;
					var md = item.Encoder.Mode;
					var en = ac.Encoder;

					var trim = (queue.Trim.Enable ? $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}" : string.Empty);

					var qu = ac.Mode[md].Args.IsDisable() ? string.Empty : $"{ac.Mode[md].Args} {ac.Mode[md].QualityPrefix}{item.Encoder.Quality}{ac.Mode[md].QualityPostfix}";
					var hz = item.Encoder.SampleRate == 0 ? string.Empty : $"-ar {item.Encoder.SampleRate}";
					var ch = item.Encoder.Channel == 0 ? string.Empty : $"-ac {item.Encoder.Channel}";

					var outfmtfile = $"audio{i:D4}_{item.Lang}.{ac.Extension}";

					var af = string.Empty;
					
					if (!item.CommandFilter.IsDisable())
					{
						af = $"-af {item.CommandFilter}";
					}

					if (ac.Args.Pipe)
					{
						ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} {af} -f wav {item.Command} - | \"{en}\" {ac.Args.Input} {ac.Args.Command} {qu} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
					}
					else
					{
						ProcessManager.Start(tempDir, $"\"{en}\" {ac.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} {ac.Args.Command} {ac.Args.Codec} {qu} {hz} {ch} {af} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
					}
				}
			}
		}

		internal static void Video(MediaQueue queue, string tempDir)
		{
			for (int i = 0; i < queue.Video.Count; i++)
			{
				var item = queue.Video[i];

				if (Plugins.Items.Video.TryGetValue(item.Encoder.Id, out PluginsVideo codec))
				{
					var vc = codec.Video;

					var en = vc.Encoder.Find(b => b.BitDepth == item.Quality.BitDepth).Binary;
					
					var outrawfile = $"raw-v{i:D4}_{item.Lang}.{vc.Extension}";
					var outfmtfile = $"video{i:D4}_{item.Lang}.{codec.Format[0]}";
					var outencfile = vc.RawOutput ? outrawfile : outfmtfile;

					var mode = item.Encoder.Mode;

					var val_w = item.Quality.Width >= 128 ? item.Quality.Width : item.Quality.OriginalWidth;
					var val_h = item.Quality.Height >= 128 ? item.Quality.Height : item.Quality.OriginalHeight;
					var val_fps = item.Quality.FrameRate >= 5 ? item.Quality.FrameRate : 23.976;
					var val_bpc = item.Quality.BitDepth >= 8 ? item.Quality.BitDepth : 8;
					var val_csp = item.Quality.PixelFormat >= 420 ? item.Quality.PixelFormat : 420;

					var ff_rawcodec = string.Empty;
					var ff_trim = string.Empty;
					var ff_res = string.Empty;
					var ff_fps = string.Empty;
					var ff_yuv = string.Empty;
					var ff_vf = new List<string>();
					
					var en_res = string.Empty;
					var en_fps = string.Empty;
					var en_bit = string.Empty;
					var en_csp = string.Empty;
					var en_preset = string.Empty;
					var en_tune = string.Empty;
					var en_quality = string.Empty;
					var en_framecount = string.Empty;

					// FFmpeg RAW Type
					if (string.IsNullOrEmpty(vc.Args.Y4M))
						ff_rawcodec = "-strict -1 -f rawvideo";
					else
						ff_rawcodec = "-strict -1 -f yuv4mpegpipe";

					// FFmpeg Resolution
					ff_vf.Add($"scale={val_w}:{val_h}:flags=lanczos");

					// FFmpeg Frame Rate
					ff_fps = $"-r {item.Quality.FrameRate}";

					// FFmpeg Pixel Format
					ff_yuv = $"-pix_fmt yuv{item.Quality.PixelFormat}p{(val_bpc > 8 ? $"{val_bpc}le" : string.Empty)}";

					// FFmpeg Trim
					if (queue.Trim.Enable)
						ff_trim += $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}";

					// FFmpeg Video Filter
					if (item.DeInterlace.Enable)
						ff_vf.Add($"yadif={item.DeInterlace.Mode}:{item.DeInterlace.Field}:0");
					
					// Fmpeg Video Filter (extra)
					if (!item.Quality.CommandFilter.IsDisable())
						ff_vf.Add(item.Quality.CommandFilter);
					
					if (queue.HardSub)
					{
						var files = Directory.GetFiles(tempDir, "subtitle*");

						if (files.Length > 0)
						{
							var file = Path.GetFileName(files[0]);
							var ext = Path.GetExtension(file).ToLowerInvariant();

							if (ext.IsOneOf(".srt"))
							{
								ff_vf.Add($"subtitles={file}");
							}
							else if (ext.IsOneOf(".ass", ".ssa"))
							{
								ff_vf.Add($"ass={file}");
							}
						}
					}

					//
					// Encoder Resolution
					if (!string.IsNullOrEmpty(vc.Args.Resolution))
						en_res = string.Format(vc.Args.Resolution, val_w, val_h);

					// Encoder Frame Rate
					if (!string.IsNullOrEmpty(vc.Args.FrameRate))
						en_fps = $"{vc.Args.FrameRate} {item.Quality.FrameRate}";

					// Encoder BitDepth Input
					if (!string.IsNullOrEmpty(vc.Args.BitDepthIn))
						en_bit = $"{vc.Args.BitDepthIn} {item.Quality.BitDepth}";

					// Encoder BitDepth Output
					if (!string.IsNullOrEmpty(vc.Args.BitDepthOut))
						en_bit += $" {vc.Args.BitDepthOut} {item.Quality.BitDepth}";

					// Encoder Pixel Format/ColorSpace Format (csp)
					foreach (var c in vc.Chroma)
					{
						if (c.Value == val_csp)
							if (!string.IsNullOrEmpty(c.Command))
								en_csp = c.Command;
					}

					// Encoder Preset
					if (!vc.Args.Preset.IsDisable() && !item.Encoder.Preset.IsDisable())
					{
						en_preset = $"{vc.Args.Preset} {item.Encoder.Preset}";
					}

					// Encoder Tune
					if (!vc.Args.Tune.IsDisable() && !item.Encoder.Tune.IsDisable())
					{
						en_tune = $"{vc.Args.Tune} {item.Encoder.Tune}";
					}

					// Encoder Mode
					if (!vc.Mode[mode].Args.IsDisable())
					{
						en_quality = $"{vc.Mode[mode].Args} {vc.Mode[mode].Prefix}{item.Encoder.Value}{vc.Mode[mode].Postfix}";
					}

					// Encoder Frame Count
					if (!vc.Args.FrameCount.IsDisable())
					{
						if (item.Quality.FrameCount > 0)
							en_framecount = $"{vc.Args.FrameCount} {item.Quality.FrameCount}";
					}

					// Parse FFmpeg filter
					var ffmpeg_filter = string.Join(",", ff_vf);

					// Make commands
					var ffmpeg_command = $"{ff_trim} -map 0:{item.Id} {ff_yuv} {ff_fps} -vf {ffmpeg_filter} {item.Quality.Command}";
					var encoder_command = $"{en_res} {en_fps} {en_bit} {en_csp} {en_preset} {en_tune} {en_quality} {en_framecount}";

					var ffmpeg_encoder = string.Equals(Path.GetFileNameWithoutExtension(en).ToLowerInvariant(), "ffmpeg") ? ffmpeg_command : encoder_command;

					// begin encoding
					frmMain.PrintStatus($"Encoding, Video #{i}");
					frmMain.PrintLog($"[INFO] Encoding video file...");

					// Tell You
					frmMain.PrintLog($"[INFO] Video filter command is: {ffmpeg_filter}");

					if (vc.Mode[item.Encoder.Mode].MultiPass)
					{
						var p = 1;
						var pass = string.Empty;

						frmMain.PrintLog("[WARN] Frame count is disable for Multi-pass encoding, Avoid inconsistent across multi-pass.");

						do
						{
							pass = vc.Args.PassNth;

							if (p == 1)
								pass = vc.Args.PassFirst;

							if (p == item.Encoder.MultiPass)
								pass = vc.Args.PassLast;

							frmMain.PrintLog($"[INFO] Multi-pass encoding: {p} of {item.Encoder.MultiPass}");

							if (vc.Args.Pipe)
								ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {ff_rawcodec} {ffmpeg_command} - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {encoder_command} {pass} {item.Encoder.Command} {vc.Args.Output} {outencfile}");
							else
								ProcessManager.Start(tempDir, $"\"{en}\" {vc.Args.Input} \"{item.File}\" {ffmpeg_encoder} {vc.Args.UnPipe} {pass} {item.Encoder.Command} {vc.Args.Command} {vc.Args.Output} {outencfile}");

							++p;

						} while (p <= item.Encoder.MultiPass);
					}
					else
					{
						if (vc.Args.Pipe)
							ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {ff_rawcodec} {ffmpeg_command} - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {encoder_command} {item.Encoder.Command} {vc.Args.Output} {outencfile}");
						else
							ProcessManager.Start(tempDir, $"\"{en}\" {vc.Args.Input} \"{item.File}\" {ffmpeg_encoder} {vc.Args.UnPipe} {item.Encoder.Command} {vc.Args.Output} {outencfile}");
					}

					// Raw file dont have pts (time), need to remux
					frmMain.PrintStatus($"Restructure...");
					frmMain.PrintLog($"[INFO] Restructure RAW video file...");

					if (vc.RawOutput)
					{
						if (File.Exists(Path.Combine(tempDir, outrawfile)))
						{
							ProcessManager.Start(tempDir, $"\"{MP4Box}\" -add \"{outrawfile}#video:name=\" -itags tool=\"IFME MP4\" \"{outfmtfile}\" ");
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

		internal static int Muxing(MediaQueue queue, string tempDir, string saveDir, string saveFile)
		{
			var x = 0;
			var metadata = string.Empty;
			var metafile = string.Empty;
			var map = string.Empty;

			var argVideo = string.Empty;
			var argAudio = string.Empty;
			var argSubtitle = string.Empty;
			var argEmbed = string.Empty;

			var outFile = Path.Combine(saveDir, saveFile);

			frmMain.PrintStatus("Repacking...");
			frmMain.PrintLog($"[INFO] Multiplexing encoded files into single file...");

			if (File.Exists(Path.Combine(tempDir, "metadata.ini")))
			{
				metafile = "-f ffmetadata -i metadata.ini ";
			}

			foreach (var video in Directory.GetFiles(tempDir, "video*"))
			{
				argVideo += $"-i \"{Path.GetFileName(video)}\" ";
				metadata += $"-metadata:s:{x} title=\"{video.GetLanguageCodeFromFileName()}\" -metadata:s:{x} language={video.GetLanguageCodeFromFileName()}  ";
				map += $" -map {x}:0";
				x++;
			}

			foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
			{
				argAudio += $"-i \"{Path.GetFileName(audio)}\" ";
				metadata += $"-metadata:s:{x} title=\"{audio.GetLanguageCodeFromFileName()}\" -metadata:s:{x} language={audio.GetLanguageCodeFromFileName()} ";
				map += $" -map {x}:0";
				x++;
			}

			if (queue.OutputFormat == MediaContainer.MKV)
			{
				var d = 0;
				foreach (var subtitle in Directory.GetFiles(tempDir, "subtitle*"))
				{
					argSubtitle += $"-i \"{Path.GetFileName(subtitle)}\" ";
					metadata += $"-metadata:s:{x} title=\"{subtitle.GetLanguageCodeFromFileName()}\" -metadata:s:{x} language={subtitle.GetLanguageCodeFromFileName()} {(d == 0 ? $"-disposition:s:{d} default " : "")}";
					map += $" -map {x}:0";
					x++;
					d++;
				}

				var tempDirFont = Path.Combine(tempDir, "attachment");
				if (Directory.Exists(tempDirFont))
				{
					var files = Directory.GetFiles(tempDirFont, "*");
					for (int i = 0; i < files.Length; i++)
					{
						argEmbed += $"-attach \"{Path.Combine("attachment", Path.GetFileName(files[i]))}\" ";
						metadata += $"-metadata:s:{x} filename=\"{Path.GetFileName(files[i])}\" -metadata:s:{x} \"mimetype={queue.Attachment[i].Mime}\" ";
						x++;
					}
				}
			}

			var author = $"{Version.Name} v{Version.Release} {Version.OSPlatform} {Version.OSArch} {Version.March} @ {Version.CodeName}";
			var command = $"\"{FFmpeg}\" -hide_banner -v error -stats {argVideo}{argAudio}{argSubtitle}{metafile}{map} -c copy -metadata \"encoded_by={author}\" {argEmbed}{metadata} -y \"{outFile}\"";
			return ProcessManager.Start(tempDir, command);
		}
	}
}
