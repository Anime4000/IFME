﻿using System;
using System.IO;

using static ifme.Properties.Settings;

namespace ifme
{
	public class MediaEncoder
	{
		private static StringComparison IC = StringComparison.OrdinalIgnoreCase; // Just ignore case what ever it is.
		private static string NULL = OS.Null;

		public static void CleanUp()
		{
			foreach (var files in Directory.GetFiles(Default.DirTemp))
				File.Delete(files);
		}

		public static void Extract(string filereal, Queue item)
		{
			if (item.Data.IsFileMkv || (item.Data.IsFileAvs && Default.AvsMkvCopy))
			{
				int sc = 0;
				foreach (var subs in GetStream.Media(filereal, StreamType.Subtitle))
					TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{filereal}\" -map {subs.ID} -y sub{sc++:0000}_{subs.Lang}.{subs.Format}");


				foreach (var font in GetStream.MediaMkv(filereal, StreamType.Attachment))
					TaskManager.Run($"\"{Plugin.MKVEX}\" attachments \"{filereal}\" {font.ID}:\"{font.File}\"");

				TaskManager.Run($"\"{Plugin.MKVEX}\" chapters \"{filereal}\" > chapters.xml");
			}
		}

		public static void Audio(string filereal, Queue item)
		{
			string frequency;
			if (string.Equals(item.Audio.Frequency, "auto", IC))
				frequency = "";
			else
				frequency = "-ar " + item.Audio.Frequency;

			string channel;
			if (string.Equals(item.Audio.Channel, "auto", IC))
				channel = "";
			else if (string.Equals(item.Audio.Channel, "mono", IC))
				channel = "-ac 1";
			else
				channel = "-ac 2";

			if (string.Equals(item.Audio.Encoder, "No Audio", IC))
			{
				// Do noting
			}
			else if (string.Equals(item.Audio.Encoder, "Passthrough (Extract all audio)", IC))
			{
				// Extract all
				int counter = 0;
				foreach (var audio in GetStream.Media(filereal, StreamType.Audio))
				{
					TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{filereal}\" -map {audio.ID} -acodec copy -y audio{counter++:0000}_{audio.Lang}.{audio.Format}");
				}

				// check if got any unsupported codec
				if (!item.Data.SaveAsMkv)
				{
					foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
					{
						if (!string.Equals(Path.GetExtension(audio), ".mp4", IC))
						{
							TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{audio}\" -strict -2 -c:a aac -b:a {item.Audio.BitRate}k {frequency} {channel} -y {Path.GetFileNameWithoutExtension(audio)}.mp4");
                            File.Delete(audio); // delete unwanted
						}
					}
				}
			}
			else
			{
				int counter = 0;
				foreach (var codec in Plugin.List)
				{
					if (string.Equals(codec.Profile.Name, item.Audio.Encoder, IC))
					{
						foreach (var audio in GetStream.Media(filereal, StreamType.Audio))
						{
							string outfile = $"audio{counter++:0000}_{audio.Lang}";
							TaskManager.Run($"\"{Plugin.LIBAV}\" -i \"{filereal}\" -map {audio.ID} {frequency} {channel} -y {outfile}.wav");
							TaskManager.Run($"\"{codec.App.Bin}\" {codec.Arg.Bitrate} {item.Audio.BitRate} {item.Audio.Command} {codec.Arg.Input} {outfile}.wav {codec.Arg.Output} {outfile}.{codec.App.Ext}");
							File.Delete(Path.Combine(Default.DirTemp, outfile + ".wav"));
						}

						break;
					}
				}
			}
		}

		public static void Video(string file, Queue item)
		{
			foreach (var video in GetStream.Media(file, StreamType.Video))
			{
				// ffmpeg settings
				string resolution = string.Equals(item.Picture.Resolution, "auto", IC) ? null : $"-s {item.Picture.Resolution}";
				string framerate = string.Equals(item.Picture.FrameRate, "auto", IC) ? null : $"-r {item.Picture.FrameRate}";
				int bitdepth = Convert.ToInt32(item.Picture.BitDepth);
				string chroma = $"yuv{item.Picture.Chroma}p{(bitdepth == 10 ? "10le" : null)}";
				string yadif = item.Picture.YadifEnable ? $"-vf \"yadif={item.Picture.YadifMode}:{item.Picture.YadifField}:{item.Picture.YadifFlag}\"" : null;
				int framecount = item.Prop.FrameCount;
				string vsync = "cfr";

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
					framecount = (int)Math.Ceiling(((float)item.Prop.Duration / 1000.0) * Convert.ToDouble(item.Picture.FrameRate));
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
				string preset = item.Video.Preset;
				string tune = string.Equals(item.Video.Tune, "off", IC) ? null : $"--tune {item.Video.Tune}";
				int type = item.Video.Type;
				int pass;
				string value = item.Video.Value;
				string command = item.Video.Command;

				string decoder = item.Data.IsFileAvs ? $"\"{Plugin.AVS4P}\" video \"{file}\"" : $"\"{Plugin.LIBAV}\" -i \"{file}\" -vsync {vsync} -f yuv4mpegpipe -pix_fmt {chroma} -strict -1 {resolution} {framerate} {yadif} -";
				string encoder = $"\"{(bitdepth == 8 ? Plugin.HEVCL : Plugin.HEVCH)}\" --y4m - -p {preset} {(type == 0 ? "--crf" : type == 1 ? "--qp" : "--bitrate")} {value} {command} -o video0000_{video.Lang}.hevc";

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
			string prefix = string.IsNullOrEmpty(Default.NamePrefix) ? null : $"{Default.NamePrefix} ";
            string fileout = Path.Combine(Default.DirOutput, $"{prefix}{Path.GetFileNameWithoutExtension(item.Data.File)}");

			if (item.Data.SaveAsMkv)
			{
				fileout += ".mkv";

				string tags = string.Format(Properties.Resources.Tags, Global.App.NameFull, "Nemu System 5.1.1");
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
						cmdsubs += $"--compression 0:zlib --sub-charset 0:UTF-8 --language 0:{subs.Lang} \"{subs.File}\" ";
					}
				}
				else
				{
					foreach (var subs in Directory.GetFiles(Default.DirTemp, "sub*"))
					{
						cmdsubs += $"--compression 0:zlib --sub-charset 0:UTF-8 --language 0:{GetInfo.FileLang(subs)} \"{subs}\" ";
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
					foreach (var attach in Directory.GetFiles(Default.DirTemp, "font*.*f"))
					{
						cmdattach += $"--attachment-description No --attach-file \"{attach}\" ";
					}
				}

				if (File.Exists(Path.Combine(Default.DirTemp, "chapters.xml")))
				{
					FileInfo ChapLen = new FileInfo(Path.Combine(Default.DirTemp, "chapters.xml"));
					if (ChapLen.Length > 256)
						cmdchapter = $"--chapters \"{Path.Combine(Default.DirTemp, "chapters.xml")}\"";
				}

				File.WriteAllText(Path.Combine(Default.DirTemp, "tags.xml"), tags);
				TaskManager.Run($"\"{Plugin.MKVME}\" -o \"{fileout}\" -t 0:tags.xml --disable-track-statistics-tags {cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}");
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

				foreach (var video in Directory.GetFiles(Default.DirTemp, "video*"))
				{
					cmdvideo += $"-add \"{video}#video:name=IFME:lang={GetInfo.FileLang(video)}:fmt=HEVC\" ";
				}

				foreach (var audio in Directory.GetFiles(Default.DirTemp, "audio*"))
				{
					cmdaudio += $"-add \"{audio}#audio:name=IFME:lang={GetInfo.FileLang(audio)}\" ";
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