using System;
using System.Globalization;
using System.IO;
using System.Linq;

using static ifme.Properties.Settings;

namespace ifme
{
	public class MediaEncoder
	{
		StringComparison IC { get { return StringComparison.InvariantCultureIgnoreCase; } }
		private static string NULL { get { return OS.Null; } }

		public static void CleanUp()
		{
			Console.WriteLine($"Clearing temp folder: {Default.DirTemp}");

			foreach (var files in Directory.GetFiles(Default.DirTemp))
				File.Delete(files);
		}

		public static void Extract(string file, Queue item)
		{
			if (string.IsNullOrEmpty(file))
				return;

			string realfile = GetStream.AviSynthGetFile(file);

			if (item.Data.IsFileMkv || (item.Data.IsFileAvs && Default.AvsMkvCopy))
			{
				int sc = 0;
				foreach (var subs in GetStream.Subtitle(realfile))
					TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{realfile}\" -map {subs.Id} -y sub{sc++:0000}_{subs.Lang}.{subs.Format}");


				foreach (var font in GetStream.MkvAttachment(realfile))
					TaskManager.Run($"\"{Plugin.MKVEX}\" attachments \"{realfile}\" {font.ID}:\"{font.File}\"");

				TaskManager.Run($"\"{Plugin.MKVEX}\" chapters \"{realfile}\" > chapters.xml");
			}
		}

		public static void Audio(string file, Queue item)
		{
			if (string.IsNullOrEmpty(file))
				return;

			string ffmap = "";
			string ffcmd = item.Picture.Command;

			int counter = 0;
			foreach (var track in item.Audio)
			{
				string frequency = string.Equals(track.Freq, "auto", IC) ? $"{track.RawFreq}" : $"{track.Freq}";
				string bit = $"{track.RawBit}";
				string channel = string.Equals(track.Chan, "auto", IC) ? $"{track.RawChan}" : string.Equals(track.Chan, "stereo", IC) ? "2" : "1";

				if (item.AudioMerge)
				{
					counter++;
					ffmap += $"-map {track.Id} ";
					
					if (item.Audio.Count == counter)
					{
						foreach (var codec in Plugin.List)
						{
							if (string.Equals(item.Audio[0].Encoder, codec.Profile.Name, IC))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" {ffmap} -filter_complex amix=inputs={counter}:duration=first:dropout_transition=0 -acodec pcm_s{bit}le -ar {frequency} -ac {channel} -f wav {ffcmd} - 2> {NULL} | \"{codec.App.Bin}\" {(string.IsNullOrEmpty(codec.Arg.Raw) ? string.Empty : string.Format(codec.Arg.Raw, frequency, bit, channel))} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio0000_und.{codec.App.Ext}");
							}
						}
					}
				}
				else
				{
					if (!track.Enable)
						continue;

					if (string.Equals(track.Encoder, "No Audio", IC))
					{
						// Do noting
					}
					else if (string.Equals(track.Encoder, "Passthrough (Extract all audio)", IC))
					{
						if (item.Data.IsFileAvs)
						{
							TaskManager.Run($"{Plugin.AVS4P} audio \"{file}\" | {Plugin.LIBAV} -i - -dn -vn -sn -strict -2 -c:a aac -b:a {track.BitRate}k -ar {frequency} -ac {channel} -y audio{counter++:0000}_{track.Lang}.mp4");
						}
						else if (item.Data.SaveAsMkv)
						{
							if (string.Equals("wma", track.Format, IC))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -strict -2 -c:a aac -b:a {track.BitRate}k -ar {frequency} -ac {channel} -y audio{counter++:0000}_{track.Lang}.mp4");
							}
							else
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
							}
						}
						else
						{
							if (string.Equals("mp4", track.Format, IC))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
							}
							else
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -strict -2 -c:a aac -b:a {track.BitRate}k -ar {frequency} -ac {channel} -y audio{counter++:0000}_{track.Lang}.mp4");
							}
						}
					}
					else
					{
						foreach (var codec in Plugin.List)
						{
							if (string.Equals(track.Encoder, codec.Profile.Name, IC))
							{
								if (Convert.ToInt32(bit) >= 32)
									bit = "24"; // force to 24bit max

								string rawArgs = string.Empty;

								try
								{
									if (!string.IsNullOrEmpty(codec.Arg.Raw))
										rawArgs = string.Format(codec.Arg.Raw, frequency, bit, channel);
                                }
								catch (Exception e)
								{
									Console.ForegroundColor = ConsoleColor.Red;
									Console.Error.WriteLine($"Raw arguments incomplete, ignored!\n{e}");
									Console.ResetColor();
								}

                                string encArgs = $"\"{codec.App.Bin}\" {rawArgs} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio{counter++:0000}_{track.Lang}.{codec.App.Ext}";

								if (item.Data.IsFileAvs)
								{
									TaskManager.Run($"\"{Plugin.AVS4P}\" audio \"{file}\" | \"{Plugin.LIBAV}\" -loglevel panic -i - -acodec pcm_s{bit}le -f wav - | {encArgs}"); // double pipe due some encoder didn't read avs2pipe properly, example: opusenc.exe
                                    break;
								}
								else
								{
									TaskManager.Run($"\"{Plugin.LIBAV}\" -loglevel panic -i \"{file}\" -map {track.Id} -acodec pcm_s{bit}le -ar {frequency} -ac {channel} -f wav {ffcmd} - | {encArgs}");
									break;
								}
							}
						}
					}
				}
			}
		}

		public static void Video(string file, Queue item)
		{
			foreach (var video in GetStream.Video(file))
			{
				// Copy
				if (item.Picture.IsHevc)
				{
					if (item.Picture.IsCopy)
					{
						TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {video.Id} -c:v copy -bsf hevc_mp4toannexb -y video0000_{video.Lang}.hevc");
						return;
					}
				}

				
				// ffmpeg settings
				string resolution = string.Equals(item.Picture.Resolution, "auto", IC) ? "" : $"-s {item.Picture.Resolution}";
				string framerate = string.Equals(item.Picture.FrameRate, "auto", IC) ? "" : $"-r {item.Picture.FrameRate}";
				int bitdepth = item.Picture.BitDepth;
				string chroma = $"yuv{item.Picture.Chroma}p{(bitdepth > 8 ? $"{bitdepth}le" : "")}";
				string yadif = item.Picture.YadifEnable ? $"-vf \"yadif={item.Picture.YadifMode}:{item.Picture.YadifField}:{item.Picture.YadifFlag}\"" : "";
				int framecount = item.Prop.FrameCount;
				string vsync = "cfr";
				string ffcmd = item.Picture.Command;

				if (string.IsNullOrEmpty(framerate))
				{
					if (item.Prop.IsVFR)
					{
						TaskManager.Run($"\"{Plugin.FFMS2}\" -f -c \"{file}\" timecode");
						vsync = "vfr";
					}
				}
				else // when fps is set, generate new framecount
				{
					framecount = (int)Math.Ceiling(((float)item.Prop.Duration / 1000.0) * Convert.ToDouble(item.Picture.FrameRate, CultureInfo.InvariantCulture));
				}

				if (item.Picture.YadifEnable)
				{
					if (item.Picture.YadifMode == 1)
					{
						framecount *= 2; // make each fields as new frame
					}
				}

				if (item.Data.IsFileAvs) // get new framecount for avisynth
					framecount = GetStream.AviSynthFrameCount(file);

				// x265 settings
				string decbin = Plugin.LIBAV;
                string encbin = Plugin.HEVC08;
				string preset = item.Video.Preset;
				string tune = string.Equals(item.Video.Tune, "off", IC) ? "" : $"--tune {item.Video.Tune}";
				int type = item.Video.Type;
				int pass;
				string value = item.Video.Value;
				string command = item.Video.Command;

				if (item.Data.IsFileAvs)
					decbin = Plugin.AVS4P;

				string decoder = item.Data.IsFileAvs ? 
					$"\"{decbin}\" video \"{file}\"" : 
					$"\"{decbin}\" -i \"{file}\" -vsync {vsync} -f yuv4mpegpipe -pix_fmt {chroma} -strict -1 {resolution} {framerate} {yadif} {ffcmd} -";

				if (bitdepth == 10)
					encbin = Plugin.HEVC10;
				else if (bitdepth == 12)
					encbin = Plugin.HEVC12;

				string encoder = $"\"{encbin}\" --y4m - -p {preset} {(type == 0 ? "--crf" : type == 1 ? "--qp" : "--bitrate")} {value} {command} -o video0000_{video.Lang}.hevc";

				// Encoding start
				if (type-- >= 3) // multi pass
				{
					for (int i = 0; i < type; i++)
					{
						if (i == 0)
							pass = 1;
						else if (i == type)
							pass = 2;
						else
							pass = 3;

						if (i == 1) // get actual frame count
							framecount = GetStream.FrameCount(Path.Combine(Default.DirTemp, $"video0000_{video.Lang}.hevc"));

						Console.WriteLine($"Pass {i + 1} of {type + 1}"); // human read no index
						TaskManager.Run($"{decoder} 2> {NULL} | {encoder} -f {framecount} --pass {pass}");
					}
				}
				else
				{
					TaskManager.Run($"{decoder} 2> {NULL} | {encoder} -f {framecount}");
				}

				break;
			}
		}

		public static void Mux(Queue item)
		{
			// Final output, a file name without extension
			string savedir = Default.IsDirOutput ? Default.DirOutput : Path.GetDirectoryName(item.Data.File);
			string prefix = string.IsNullOrEmpty(Default.NamePrefix) ? string.Empty : Default.NamePrefix + " ";
            string newfile = Path.GetFileNameWithoutExtension(item.Data.File);
			string fileout = Path.Combine(savedir, $"{prefix}{newfile}");

			// Destinantion folder check
			if (!Directory.Exists(Default.DirOutput))
				Directory.CreateDirectory(Default.DirOutput);

			// File exist check
			if (File.Exists($"{fileout}.mp4") || File.Exists($"{fileout}.mkv"))
				fileout += $"_encoded-{DateTime.Now:yyyyMMdd_HHmmss}";

			if (item.Data.SaveAsMkv)
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
					cmdvideo += $"--language 0:{GetInfo.FileLang(video)} \"{video}\" ";
				}

				foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
				{
					cmdaudio += $"--language 0:{GetInfo.FileLang(audio)} \"{audio}\" ";
				}

				if (item.SubtitleEnable)
				{
					foreach (var subs in item.Subtitle)
					{
						cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{subs.Lang.Substring(0, 3)} \"{subs.File}\" ";
					}
				}
				else
				{
					foreach (var subs in Directory.GetFiles(Default.DirTemp, "sub*"))
					{
						cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{GetInfo.FileLang(subs)} \"{subs}\" ";
					}
				}

				if (item.AttachEnable)
				{
					foreach (var attach in item.Attach)
					{
						cmdattach += $"--attachment-mime-type {attach.MIME} --attachment-description {attach.Comment} --attach-file \"{attach.File}\" ";
					}
				}
				else
				{
					foreach (var attach in Directory.GetFiles(Default.DirTemp, "*.ttf"))
					{
						cmdattach += $"--attach-file \"{attach}\" ";
					}

					foreach (var attach in Directory.GetFiles(Default.DirTemp, "*.otf"))
					{
						cmdattach += $"--attach-file \"{attach}\" ";
					}

					foreach (var attach in Directory.GetFiles(Default.DirTemp, "*.woff"))
					{
						cmdattach += $"--attach-file \"{attach}\" ";
					}

					foreach (var attach in Directory.GetFiles(Default.DirTemp, "*.pfb"))
					{
						cmdattach += $"--attachment-mime-type application/x-font --attach-file \"{attach}\" ";
					}
				}

				if (File.Exists(Path.Combine(Default.DirTemp, "chapters.xml")))
				{
					FileInfo ChapLen = new FileInfo(Path.Combine(Default.DirTemp, "chapters.xml"));
					if (ChapLen.Length > 256)
						cmdchapter = $"--chapters \"{Path.Combine(Default.DirTemp, "chapters.xml")}\"";
				}

				File.WriteAllText(Path.Combine(Default.DirTemp, "tags.xml"), tags);
                TaskManager.Run($"\"{Plugin.MKVME}\" -o \"{fileout}\" --disable-track-statistics-tags -t 0:tags.xml {cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}");
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
					cmdvideo += $"-add \"{video}#video:name=Video {++cntv}:lang={GetInfo.FileLang(video)}:fmt=HEVC\" ";
				}

				int cnta = 0;
				foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
				{
					cmdaudio += $"-add \"{audio}#audio:name=Audio {++cnta}:lang={GetInfo.FileLang(audio)}\" ";
				}

				if (string.IsNullOrEmpty(timecode))
				{
					TaskManager.Run($"\"{Plugin.MP4BX}\" {cmdvideo} {cmdaudio} -itags tool=\"{Global.App.NameFull}\" -new \"{fileout}\"");
				}
				else
				{
					TaskManager.Run($"\"{Plugin.MP4BX}\" {cmdvideo} {cmdaudio} -itags tool=\"{Global.App.NameFull}\" -new _desu.mp4");
					TaskManager.Run($"\"{Plugin.MP4FP}\" -t \"{timecode}\" _desu.mp4 -o \"{fileout}\"");
				}
			}
		}
	}
}
