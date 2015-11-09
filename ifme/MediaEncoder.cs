using System;
using System.Globalization;
using System.IO;
using System.Linq;

using static ifme.Properties.Settings;

namespace ifme
{
	public class MediaEncoder
	{
		private static StringComparison IC = StringComparison.InvariantCultureIgnoreCase; // Just ignore case what ever it is.
		private static string NULL = OS.Null;

		public static void CleanUp()
		{
			Console.WriteLine($"Clearing temp folder: {Default.DirTemp}");

			foreach (var files in Directory.GetFiles(Default.DirTemp))
				File.Delete(files);
		}

		public static void Extract(string file, Queue item)
		{
			if (item.Data.IsFileAvs)
				file = GetStream.AviSynthGetFile(file);

			if (string.IsNullOrEmpty(file))
				return;

			if (item.Data.IsFileMkv || (item.Data.IsFileAvs && Default.AvsMkvCopy))
			{
				int sc = 0;
				foreach (var subs in GetStream.Media(file, StreamType.Subtitle))
					TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {subs.ID} -y sub{sc++:0000}_{subs.Lang}.{subs.Format}");


				foreach (var font in GetStream.MediaMkv(file, StreamType.Attachment))
					TaskManager.Run($"\"{Plugin.MKVEX}\" attachments \"{file}\" {font.ID}:\"{font.File}\"");

				TaskManager.Run($"\"{Plugin.MKVEX}\" chapters \"{file}\" > chapters.xml");
			}
		}

		public static void Audio(string file, Queue item)
		{
			if (item.Data.IsFileAvs)
				file = GetStream.AviSynthGetFile(file);

			if (string.IsNullOrEmpty(file))
				return;

			string ffmap = "";
			string ffcmd = item.Picture.Command;

			int counter = 0;
			foreach (var track in item.Audio)
			{
				string bit = $"{track.RawBit}";
				string frequency = string.Equals(track.Freq, "auto", IC) ? $"{track.RawFreq}" : $"{track.Freq}";
				string channel = string.Equals(track.Chan, "auto", IC) ? $"{track.RawChan}" : string.Equals(track.Chan, "stereo", IC) ? "2" : "1";

				if (item.AudioMerge)
				{
					counter++;
					ffmap += $"{track.Id} ";
					
					if (item.Audio.Count == counter)
					{
						foreach (var codec in Plugin.List)
						{
							if (string.Equals(codec.Profile.Name, track.Encoder, IC))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {ffmap} -filter_complex amix=inputs={counter}:duration=first:dropout_transition=0 -f s{bit}le -ar {frequency} -ac {channel} {ffcmd} - 2> {NULL} | \"{codec.App.Bin}\" {codec.Arg.Raw} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio0000_und.{codec.App.Ext}");
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
						if (item.Data.SaveAsMkv)
						{
							TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
						}
						else
						{
							if (string.Equals("mp4", track.Format))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -acodec copy {ffcmd} -y audio{counter++:0000}_{track.Lang}.{track.Format}");
							}
							else
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -dn -vn -sn -ar {frequency} -ac {channel} -strict -2 -c:a aac -b:a {track.BitRate}k -y audio{counter++:0000}_{track.Lang}.mp4");
							}
						}
					}
					else
					{
						foreach (var codec in Plugin.List)
						{
							if (string.Equals(codec.Profile.Name, track.Encoder, IC))
							{
								TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {track.Id} -f s{bit}le -ar {frequency} -ac {channel} {ffcmd} - 2> {NULL} | \"{codec.App.Bin}\" {string.Format(codec.Arg.Raw, frequency, bit, channel)} {codec.Arg.Input} {codec.Arg.Bitrate} {track.BitRate} {track.Args} {codec.Arg.Output} audio{counter++:0000}_{track.Lang}.{codec.App.Ext}");
								break;
							}
						}
					}
				}
			}
		}

		public static void Video(string file, Queue item)
		{
			foreach (var video in GetStream.Media(file, StreamType.Video))
			{
				// Copy
				if (item.Picture.IsHevc)
				{
					if (item.Picture.IsCopy)
					{
						TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{file}\" -map {video.ID} -c:v copy -bsf hevc_mp4toannexb -y video0000_{video.Lang}.hevc");
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
			string newfile = Path.GetFileNameWithoutExtension(item.Data.File);
			string prefix = Default.IsDirOutput ? Default.NamePrefix : string.IsNullOrEmpty(Default.NamePrefix) ? "[encoded] " : Default.NamePrefix + " ";
            string fileout = Path.Combine(savedir, $"{prefix}{newfile}");

			// Destinantion folder check
			if (!Directory.Exists(Default.DirOutput))
				Directory.CreateDirectory(Default.DirOutput);

			if (item.Data.SaveAsMkv)
			{
				fileout += ".mkv";

				string tags = string.Format(Properties.Resources.Tags, Global.App.NameFull, Global.App.VersionCompiled);
                string cmdvideo = null;
				string cmdaudio = null;
				string cmdsubs = null;
				string cmdattach = null;
				string cmdchapter = null;

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

				string timecode = null;
				string cmdvideo = null;
				string cmdaudio = null;

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
