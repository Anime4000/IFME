using System;
using System.Globalization;
using System.IO;

using FFmpegDotNet;

using static ifme.Properties.Settings;
using System.Linq;

namespace ifme
{
	public class MediaEncoder
	{
		static StringComparison IC { get { return StringComparison.InvariantCultureIgnoreCase; } }
		private static string NULL { get { return OS.Null; } }

		public static void CleanUp()
		{
			Console.WriteLine($"Clearing temp folder: {Default.DirTemp}");

			foreach (var files in Directory.GetFiles(Default.DirTemp))
				File.Delete(files);
		}

		public static void Extract(Queue item)
		{
            if (string.IsNullOrEmpty(item.FilePath))
				return;

			int count = 0;

			if (item.General.IsAviSynth)
			{
				if (Default.AvsMkvCopy)
				{
					var file = GetStream.AviSynthGetFile(item.FilePath);
                    var ff = new FFmpeg.Stream(file);

					foreach (var subs in ff.Subtitle)
						TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{file}\" -map 0:{subs.Id} -y subtitle{count++:D4}_{subs.Language}.{Get.MediaContainer(subs.Codec)}");

					TaskManager.Run($"\"{Plugin.FFMPEG}\" -dump_attachment:t \"\" -i \"{file}\" -y");

					TaskManager.Run($"\"{Plugin.MKVEXT}\" chapters \"{file}\" > chapters.xml");
				}

				if (item.SubtitleEnable)
				{
					foreach (var subs in item.Subtitle)
					{
						if (File.Exists(subs.File))
						{
							if (subs.Id < 0)
							{
								File.Copy(subs.File, Path.Combine(Default.DirTemp, $"subtitle{count++:D4}_{subs.Lang}.{subs.Format}"), true);
							}
						}
					}
				}

				if (item.AttachEnable)
				{
					foreach (var font in item.Attach)
					{
						if (File.Exists(font.File))
						{
							File.Copy(font.File, Path.Combine(Default.DirTemp, Get.FileName(font.File)), true);
						}
					}
				}
			}
			else
			{
				if (item.SubtitleEnable)
				{
					foreach (var subs in item.Subtitle)
					{
						if (File.Exists(subs.File))
						{
							if (subs.Id > -1)
							{
								TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{subs.File}\" -map 0:{subs.Id} -y subtitle{count++:D4}_{subs.Lang}.{subs.Format}");
							}
							else
							{
								File.Copy(subs.File, Path.Combine(Default.DirTemp, $"subtitle{count++:D4}_{subs.Lang}.{subs.Format}"), true);
							}
						}
					}
				}

				if (item.AttachEnable)
				{
					foreach (var font in item.Attach)
					{
						if (File.Exists(font.File))
						{
							File.Copy(font.File, Path.Combine(Default.DirTemp, Get.FileName(font.File)), true);
						}
					}
				}
				else
				{
					TaskManager.Run($"\"{Plugin.FFMPEG}\" -dump_attachment:t \"\" -i \"{item.FilePath}\" -y");
				}

				TaskManager.Run($"\"{Plugin.MKVEXT}\" chapters \"{item.FilePath}\" > chapters.xml");
			}
		}

		public static void Audio(Queue item)
		{
			string ffile = string.Empty;
			string ffmap = string.Empty;
			string ffcmd = item.Picture.Command;

			int counter = 0;
			foreach (var track in item.Audio)
			{
				string file = track.File;

				if (string.IsNullOrEmpty(file))
					continue;

				string freq = string.Equals(track.Freq, "auto", IC) ? $"{track.RawFreq}" : $"{track.Freq}";
				string bit = $"{track.RawBit}";
				string chan = string.Equals(track.Chan, "auto", IC) ? $"{track.RawChan}" : string.Equals(track.Chan, "stereo", IC) ? "2" : "1";

				if (item.AudioMerge)
				{
					counter++;
					ffile += $"-i \"{file}\" ";
                    ffmap += $"-map 0:{track.Id} ";

					if (item.Audio.Count == counter)
					{
						Plugin codec;
						if (Plugin.List.TryGetValue(item.Audio[0].Encoder, out codec))
						{
							TaskManager.Run($"\"{Plugin.FFMPEG}\" {ffile} {ffmap} -filter_complex amix=inputs={counter}:duration=first:dropout_transition=0 -acodec pcm_s{bit}le -ar {freq} -ac {chan} -f wav {ffcmd} - | \"{codec.App.Bin}\" {(string.IsNullOrEmpty(codec.Arg.Raw) ? string.Empty : string.Format(codec.Arg.Raw, freq, bit, chan))} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio0000_und.{codec.App.Ext}");
						}
					}
				}
				else
				{
					if (!track.Enable)
						continue;

					if (Equals(new Guid("00000000-0000-0000-0000-000000000000"), track.Encoder))
					{
						continue;
					}

					Plugin codec;

					if (Plugin.List.TryGetValue(track.Encoder, out codec) && !Equals(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), track.Encoder))
					{
						if (Convert.ToInt32(bit) >= 32)
							bit = "24"; // force to 24bit max

						string rawArgs = string.Empty;

						try
						{
							if (!string.IsNullOrEmpty(codec.Arg.Raw))
								rawArgs = string.Format(codec.Arg.Raw, freq, bit, chan);
						}
						catch (Exception e)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Error.WriteLine($"Raw arguments incomplete, ignored!\n{e}");
							Console.ResetColor();
						}

						string encArgs = $"\"{codec.App.Bin}\" {rawArgs} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio{counter++:0000}_{track.Lang}.{codec.App.Ext}";

						TaskManager.Run($"\"{Plugin.FFMPEG}\" -loglevel panic -i \"{file}\" -map 0:{track.Id} -acodec pcm_s{bit}le -ar {freq} -ac {chan} -f wav {ffcmd} - | {encArgs}");
					}
					else
					{
						if (item.General.IsOutputMKV)
						{
							if (string.Equals("wma", track.Format, IC))
							{
								TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{file}\" -map 0:{track.Id} -dn -vn -sn -strict -2 -c:a aac -b:a {track.BitRate}k -ar {freq} -ac {chan} -y audio{counter++:0000}_{track.Lang}.mp4");
							}
							else
							{
								TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{file}\" -map 0:{track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
							}
						}
						else
						{
							if (string.Equals("mp4", track.Format, IC))
							{
								TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{file}\" -map 0:{track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
							}
							else
							{
								TaskManager.Run($"\"{Plugin.FFMPEG}\" -i \"{file}\" -map 0:{track.Id} -dn -vn -sn -strict -2 -c:a aac -b:a {track.BitRate}k -ar {freq} -ac {chan} -y audio{counter++:0000}_{track.Lang}.mp4");
							}
						}
					}
				}
			}
		}

		public static void Video(Queue item)
		{
			string file = item.FilePath;

            foreach (var video in GetStream.Video(file))
			{
				// FFmpeg args
				string resolution = string.Equals(item.Picture.Resolution, "auto", IC) ? string.Empty : $"-s {item.Picture.Resolution}";
				string framerate = string.Equals(item.Picture.FrameRate, "auto", IC) ? string.Empty : $"-r {item.Picture.FrameRate}";
				int bitdepth = item.Picture.BitDepth;
				string chroma = $"yuv{item.Picture.Chroma}p{(bitdepth > 8 ? $"{bitdepth}le" : "")}";
				string yadif = item.Picture.YadifEnable ? $"-vf \"yadif={item.Picture.YadifMode}:{item.Picture.YadifField}:{item.Picture.YadifFlag}\"" : "";
				int framecount = item.Picture.FrameCount;
				string ffcmd = item.Picture.Command;

				// Indexing
				if (!item.General.IsAviSynth)
				{
					if (framecount <= 0)
					{
						if (Default.UseFrameAccurate)
						{
							Console.WriteLine("Indexing... This may take very long time.");
							new FFmpeg().FrameCountAccurate(file);
						}
						else
						{
							Console.WriteLine("Indexing... Please Wait.");
							new FFmpeg().FrameCount(file);
						}
					}

					Console.WriteLine("Indexing... Make sure in sync :)");
					TaskManager.Run($"\"{Plugin.FFMS2}\" -f -c \"{file}\" timecode");
				}

				if (!string.IsNullOrEmpty(framerate))
					framecount = (int)Math.Ceiling((item.General.Duration * Convert.ToDouble(item.Picture.FrameRate, CultureInfo.InvariantCulture)));

				if (item.Picture.YadifEnable)
					if (item.Picture.YadifMode == 1)
						framecount *= 2; // make each fields as new frame

				// x265 settings
				string decbin = Plugin.FFMPEG;
                string encbin = Plugin.HEVC08;
				string preset = item.Video.Preset;
				string tune = string.Equals(item.Video.Tune, "off", IC) ? "" : $"--tune {item.Video.Tune}";
				int type = item.Video.Type;
				int pass;
				string value = item.Video.Value;
				string command = item.Video.Command;

				if (bitdepth == 10)
					encbin = Plugin.HEVC10;
				else if (bitdepth == 12)
					encbin = Plugin.HEVC12;

				string decoder = $"\"{decbin}\" -loglevel panic -i \"{file}\" -f yuv4mpegpipe -pix_fmt {chroma} -strict -1 {resolution} {framerate} {yadif} {ffcmd} -";

				string encoder = $"\"{encbin}\" --y4m - -p {preset} {(type == 0 ? "--crf" : type == 1 ? "--qp" : "--bitrate")} {value} {command} -o video0000_{video.Lang}.hevc";

				// Encoding start
				if (type >= 3) // multi pass
				{
					type--; // re-use

					for (int i = 0; i < type; i++)
					{
						if (i == 0)
							pass = 1;
						else if (i == type)
							pass = 2;
						else
							pass = 3;

						if (i == 1) // get actual frame count
						{
							Console.WriteLine("Re-indexing encoded file, make sure no damage.");
							framecount = new FFmpeg().FrameCount(Path.Combine(Default.DirTemp, $"video0000_{video.Lang}.hevc"));
						}

						Console.WriteLine($"Pass {i + 1} of {type}"); // human read no index
						TaskManager.Run($"{decoder} | {encoder} -f {framecount} --pass {pass}");
					}
				}
				else
				{
					TaskManager.Run($"{decoder} | {encoder} -f {framecount}");
				}

				break;
			}
		}

		public static void Mux(Queue item)
		{
			// Final output, a file name without extension
			string savedir = Default.IsDirOutput ? Default.DirOutput : Path.GetDirectoryName(item.FilePath);
			string prefix = string.IsNullOrEmpty(Default.NamePrefix) ? string.Empty : Default.NamePrefix + " ";
            string newfile = Path.GetFileNameWithoutExtension(item.FilePath);
			string fileout = Path.Combine(savedir, $"{prefix}{newfile}");

			// Destinantion folder check
			if (!Directory.Exists(Default.DirOutput))
				Directory.CreateDirectory(Default.DirOutput);

			// File exist check
			if (File.Exists($"{fileout}.mp4") || File.Exists($"{fileout}.mkv"))
				fileout += $"_encoded-{DateTime.Now:yyyyMMdd_HHmmss}";

			if (item.General.IsOutputMKV)
			{
				fileout += ".mkv";

				string tags = string.Format(Properties.Resources.Tags, Global.App.NameFull, Global.App.VersionCompiled);
                string cmdvideo = string.Empty;
				string cmdaudio = string.Empty;
				string cmdsubs = string.Empty;
				string cmdattach = string.Empty;
				string cmdchapter = string.Empty;

				foreach (var tc in Directory.GetFiles(Default.DirTemp, "timecode_*"))
				{
					cmdvideo += $"--timecodes 0:\"{tc}\" "; break;
				}

				foreach (var video in Directory.GetFiles(Default.DirTemp, "video*"))
				{
					cmdvideo += $"--language 0:{Get.FileLang(video)} \"{video}\" ";
				}

				foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
				{
					cmdaudio += $"--language 0:{Get.FileLang(audio)} \"{audio}\" ";
				}

				foreach (var subs in Directory.GetFiles(Default.DirTemp, "subtitle*"))
				{
					cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{Get.FileLang(subs)} \"{subs}\" ";
				}

				foreach (var attach in Directory.GetFiles(Default.DirTemp, "*.*")
					.Where(	s => 
					s.EndsWith(".ttf", IC) ||
					s.EndsWith(".otf", IC) ||
					s.EndsWith(".ttc", IC) ||
					s.EndsWith(".woff", IC)))
				{
					cmdattach += $"--attach-file \"{attach}\" ";
				}

				if (File.Exists(Path.Combine(Default.DirTemp, "chapters.xml")))
				{
					FileInfo ChapLen = new FileInfo(Path.Combine(Default.DirTemp, "chapters.xml"));
					if (ChapLen.Length > 256)
						cmdchapter = $"--chapters \"{Path.Combine(Default.DirTemp, "chapters.xml")}\"";
				}

				File.WriteAllText(Path.Combine(Default.DirTemp, "tags.xml"), tags);
                TaskManager.Run($"\"{Plugin.MKVMER}\" -o \"{fileout}\" --disable-track-statistics-tags -t 0:tags.xml {cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}");
			}
			else
			{
				fileout += ".mp4";

				string timecode = string.Empty;
				string cmdvideo = string.Empty;
				string cmdaudio = string.Empty;

				foreach (var tc in Directory.GetFiles(Default.DirTemp, "timecode_*"))
				{
					timecode = tc; break;
				}

				int cntv = 0;
				foreach (var video in Directory.GetFiles(Default.DirTemp, "video*"))
				{
					cmdvideo += $"-add \"{video}#video:name=Video {++cntv}:lang={Get.FileLang(video)}:fmt=HEVC\" ";
				}

				int cnta = 0;
				foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
				{
					cmdaudio += $"-add \"{audio}#audio:name=Audio {++cnta}:lang={Get.FileLang(audio)}\" ";
				}

				if (string.IsNullOrEmpty(timecode))
				{
					TaskManager.Run($"\"{Plugin.MP4BOX}\" {cmdvideo} {cmdaudio} -itags tool=\"{Global.App.NameFull}\" -new \"{fileout}\"");
				}
				else
				{
					TaskManager.Run($"\"{Plugin.MP4BOX}\" {cmdvideo} {cmdaudio} -itags tool=\"{Global.App.NameFull}\" -new _desu.mp4");
					TaskManager.Run($"\"{Plugin.MP4FPS}\" -t \"{timecode}\" _desu.mp4 -o \"{fileout}\"");
				}
			}
		}
	}
}
