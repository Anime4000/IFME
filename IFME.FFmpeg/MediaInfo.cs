using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json;


namespace IFME.FFmpeg
{
	public class MediaInfo
	{
		private static StringComparison IgnoreCase { get { return StringComparison.InvariantCultureIgnoreCase; } }

		public string FilePath { get; internal set; } = string.Empty;
		public ulong FileSize { get; internal set; } = 0;
		public ulong BitRate { get; internal set; } = 0;
		public float Duration { get; internal set; } = 0;
		public string FormatName { get; internal set; } = "NEW";
		public string FormatNameFull { get; internal set; } = "Blank media";
		public string Encoder { get; internal set; } = string.Empty;

		public List<StreamVideo> Video { get; internal set; } = new List<StreamVideo>();
		public List<StreamAudio> Audio { get; internal set; } = new List<StreamAudio>();
		public List<StreamSubtitle> Subtitle { get; internal set; } = new List<StreamSubtitle>();
		public List<StreamAttachment> Attachment { get; internal set; } = new List<StreamAttachment>();

        public static string FFmpegProbe
		{ 
			get 
			{
                if (OS.IsProgramInPath("ffprobe"))
                    return "ffprobe";
                else
                    return Path.Combine("Plugins", "ffmpeg64", "ffprobe");
			} 
		}

        public MediaInfo()
		{

		}

		public MediaInfo(string filePath, string frameRate = "")
		{
			dynamic json = JsonConvert.DeserializeObject(new ReadFile().Media(filePath, frameRate));

			try
			{
				// General info
				FilePath = filePath;
				FileSize = ulong.TryParse((string)json.format.size, out ulong fs) ? fs : 0;
				BitRate = ulong.TryParse((string)json.format.bit_rate, out ulong br) ? br : 0;
				Duration = float.TryParse((string)json.format.duration, out float d) ? d : 0;
				FormatName = (string)json.format.format_name;
				FormatNameFull = (string)json.format.format_long_name;

				try { Encoder = (string)json.format.tags.encoder; }
				catch (Exception e) { Console.WriteLine(e.Message); }

				// Capture stream type
				foreach (var stream in json.streams)
				{
					string type = stream.codec_type;

					if (string.Equals(type, "video", IgnoreCase))
					{
						int id = 0;
						try { id = int.Parse((string)stream.index); }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string lang = "und";
						try { lang = stream.tags.language; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(lang)) lang = "und";

						string codec = "unknown";
						try { codec = stream.codec_name; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int width = 0;
						try { width = int.Parse((string)stream.width); }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int height = 0;
						try { height = int.Parse((string)stream.height); }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string r = "0/0";
						try { r = stream.r_frame_rate; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						float.TryParse(r.Split('/')[0], out float rn);
						float.TryParse(r.Split('/')[1], out float rd);
						float rfps = rn / rd;

						string a = "0/0";
						try { a = stream.avg_frame_rate; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						float.TryParse(a.Split('/')[0], out float an);
						float.TryParse(a.Split('/')[1], out float ad);
						float afps = an / ad;

						string title = "";
						try { title = stream.tags.title; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(title)) title = string.Empty;

						int pix = 420;
						try
						{
							if (!string.IsNullOrEmpty((string)stream.pix_fmt))
							{
								var mpix = Regex.Match((string)stream.pix_fmt, @"yuv(\d+)");

								if (mpix.Success)
									int.TryParse(mpix.Groups[1].Value, out pix);
								else
									pix = 420;
							}
						}
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int bpc = 8;
						try
						{
							if (int.TryParse((string)stream.bits_per_raw_sample, out int x))
							{
								bpc = x;
							}
							else
							{
								var mbpc = Regex.Match((string)stream.pix_fmt, @"yuv\d+p(\d+)");

								if (mbpc.Success)
									int.TryParse(mbpc.Groups[1].Value, out bpc);
								else
									bpc = 8;
							}
						}
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						bool dap = false;
						try { dap = stream.disposition.attached_pic; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						Video.Add(new StreamVideo
						{
							Id = id,
							Language = lang,
							Codec = codec,
							Title = title,
							Chroma = pix,
							BitDepth = bpc,
							Width = width,
							Height = height,
							FrameRateConstant = rfps == afps,
							FrameRate = rfps,
							FrameRateAvg = afps,
							FrameCount = (int)(Duration * afps),
							Duration = Duration,
							Disposition_AttachedPic = dap,
						});
					}

					if (string.Equals(type, "audio", IgnoreCase))
					{
						int id = 1;
						try { id = stream.index; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string lang = "und";
						try { lang = stream.tags.language; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(lang)) lang = "und";

						string codec = "unknown";
						try { codec = stream.codec_name; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int sample = 44100;
						try { sample = stream.sample_rate; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int bitdepth = 16;
						try { bitdepth = stream.sample_fmt; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						int channel = 2;
						try { channel = stream.channels; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						if (bitdepth == 0) bitdepth = 16;
						else if (bitdepth >= 32) bitdepth = 24;

						string title = "";
						try { title = stream.tags.title; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(title)) title = string.Empty;

						Audio.Add(new StreamAudio
						{
							Id = id,
							Language = lang,
							Codec = codec,
							Title = title,
							SampleRate = sample,
							BitDepth = bitdepth,
							Channel = channel,
							Duration = Duration,
						});
					}

					if (string.Equals(type, "subtitle", IgnoreCase))
					{
						int id = 2;
						try { id = stream.index; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string lang = "und";
						try { lang = stream.tags.language; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(lang)) lang = "und";

						string codec = "unknown";
						try { codec = stream.codec_name; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string title = "";
						try { title = stream.tags.title; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }
						if (string.IsNullOrEmpty(title)) title = string.Empty;

						Subtitle.Add(new StreamSubtitle
						{
							Id = id,
							Language = lang,
							Codec = FormatId.Get(codec),
							Title = title
						});
					}

					if (string.Equals(type, "attachment", IgnoreCase))
					{
						int id = 3;
						try { id = stream.index; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string fname = "unknown";
						try { fname = stream.tags.filename; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						string mtype = "application/octet-stream";
						try { mtype = stream.tags.mimetype; }
						catch (Exception ex) { Console.WriteLine(ex.Message); }

						Attachment.Add(new StreamAttachment
						{
							Id = id,
							FileName = fname,
							MimeType = mtype
						});
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public static string Print(MediaInfo value)
		{
			var info = $"General{Environment.NewLine}" +
				$"Complete name               : {value.FilePath}{Environment.NewLine}" +
				$"Format                      : {value.FormatName} ({value.FormatNameFull}){Environment.NewLine}" +
				$"File size                   : {value.FileSize} bytes{Environment.NewLine}" +
				$"Duration                    : {value.Duration} seconds{Environment.NewLine}" +
				$"Overall bit rate            : {value.BitRate} bps{Environment.NewLine}" +
				$"Writing Application         : {value.Encoder}{Environment.NewLine}";

			var v = 0;
			var video = string.Empty;
			foreach (var item in value.Video)
			{
				video += $"{Environment.NewLine}Video #{v++}{Environment.NewLine}" +
					$"ID                          : {item.Id}{Environment.NewLine}" +
					$"Format                      : {item.Codec}{Environment.NewLine}" +
					$"Width                       : {item.Width}{Environment.NewLine}" +
					$"Height                      : {item.Height}{Environment.NewLine}" +
					$"Frame rate mode             : {(item.FrameRateConstant ? "Constant" : "Variable")}{Environment.NewLine}" +
					$"Frame rate                  : {item.FrameRate} fps ({item.FrameRateAvg} fps){Environment.NewLine}" +
					$"Chroma subsampling          : {item.Chroma}{Environment.NewLine}" +
					$"Bit depth                   : {item.BitDepth} bits{Environment.NewLine}" +
					$"Language                    : {item.Language}{Environment.NewLine}" +
					$"Title                       : {item.Title}{Environment.NewLine}";
			}

			var a = 0;
			var audio = string.Empty;
			foreach (var item in value.Audio)
			{
				audio += $"{Environment.NewLine}Audio #{a++}{Environment.NewLine}" +
					$"ID                          : {item.Id}{Environment.NewLine}" +
					$"Format                      : {item.Codec}{Environment.NewLine}" +
					$"Channel(s)                  : {item.Channel}{Environment.NewLine}" +
					$"Sampling rate               : {item.SampleRate} Hz{Environment.NewLine}" +
					$"Bit Depth                   : {item.BitDepth} bits{Environment.NewLine}" +
					$"Language                    : {item.Language}{Environment.NewLine}" +
					$"Title                       : {item.Title}{Environment.NewLine}";
			}

			var s = 0;
			var subtitle = string.Empty;
			foreach (var item in value.Subtitle)
			{
				subtitle += $"{Environment.NewLine}Subtitles #{s++}{Environment.NewLine}" +
					$"ID                          : {item.Id}{Environment.NewLine}" +
					$"Format                      : {item.Codec}{Environment.NewLine}" +
					$"Language                    : {item.Language}{Environment.NewLine}" +
					$"Title                       : {item.Title}{Environment.NewLine}";
			}

			var t = 0;
			var attach = string.Empty;
			foreach (var item in value.Attachment)
			{
				attach += $"{Environment.NewLine}Attachments #{t++}{Environment.NewLine}" +
					$"ID                          : {item.Id}{Environment.NewLine}" +
					$"Name                        : {item.FileName}{Environment.NewLine}" +
					$"MIME                        : {item.MimeType}{Environment.NewLine}";
			}

			return info + video + audio + subtitle + attach;
		}
	}
}
