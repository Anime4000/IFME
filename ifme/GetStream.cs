using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IniParser;
using IniParser.Model;

using static ifme.Properties.Settings;
using System.Text.RegularExpressions;

namespace ifme
{
	public class GetStream
	{
		public class basic
		{
			public string Id;
			public string Lang;
			public string Codec;
			public string Format;
		}

		public class audio
		{
			public basic Basic;
			public int RawBit;
			public int RawFreq;
			public int RawChan;
		}

		public class matroska
		{
			public string ID;
			public string Mime;
			public string File;
		}

		private static StringComparison IC { get { return StringComparison.InvariantCultureIgnoreCase; } }
		private static IniData GetFmt { get { return new FileIniDataParser().ReadFile("format.ini", Encoding.UTF8); } }
		private static string IdFFm { get { return Path.Combine(Default.DirTemp, "ffmpeg.id"); } }
		private static string IdMkv { get { return Path.Combine(Default.DirTemp, "mkvtoolnix.id"); } }
		private static string IdAvs { get { return Path.Combine(Default.DirTemp, "avisynth.id"); } }

		private static basic Basic(string kind, string data)
		{
			// Basic
			string id = string.Empty;
			string lang = "und";
			string codec = string.Empty;
			string format = string.Empty;

			// Besure it's right line
			if (data.Contains("Stream #"))
			{
				// ID section
				Regex regId = new Regex(@"#(\d+:\d+)");
				Match matchId = regId.Match(data);

				if (matchId.Success)
				{
					id = matchId.Groups[1].Value;
                }

				// Lang section
				Regex regLang = new Regex(@"\(([a-z]{3})\):");
				Match matchLang = regLang.Match(data);

				if (matchLang.Success)
				{
					lang = matchLang.Groups[1].Value;
                }

				// Codec section
				Regex regCodec = new Regex($@"{kind}: (\w+)");
				Match matchCodec = regCodec.Match(data);

				if (matchCodec.Success)
				{
					codec = matchCodec.Groups[1].Value;
                }

				try
				{
					format = GetFmt["format"][codec];
				}
				catch (Exception)
				{
					Console.WriteLine("Requested query not found, using default");
				}
				finally
				{
					if (string.IsNullOrEmpty(format))
						format = codec;
				}
			}

			return new basic { Id = id, Lang = lang, Codec = codec, Format = format };
		}

		public static List<basic> Video(string file)
		{
			List<basic> Items = new List<basic>();
			string kind = "Video";

			if (IsAviSynth(file))
			{
				Items.Add(new basic() { Id = "0:0", Lang = "und", Format = "avs", Codec = "avs" });
				return Items;
			}
			else
			{
				TaskManager.Run($"\"{Plugin.FFPROBE}\" \"{file}\" 2> {IdFFm}");
				foreach (var item in File.ReadAllLines(IdFFm))
				{
					if (item.Contains("Stream #"))
					{
						if (item.Contains(kind))
						{
							// Basic section
							var Common = Basic(kind, item);

							// Add
							Items.Add(new basic()
							{
								Id = Common.Id,
								Lang = Common.Lang,
								Codec = Common.Codec,
								Format = Common.Format
							});
						}
					}
				}
            }

			return Items;
		}

		public static List<audio> Audio(string file)
		{
			List<audio> Items = new List<audio>();
			string kind = "Audio";

			if (IsAviSynth(file))
			{
				int bit = 0;
				int freq = 0;
				int chan = 0;

				TaskManager.Run($"\"{Plugin.AVSPIPE}\" info \"{file}\" > {IdAvs}");
				foreach (var item in File.ReadAllLines(IdAvs))
				{
					if (item.Contains("a:sample_rate"))
						freq = int.Parse(item.Substring(14));

					if (item.Contains("a:bit_depth"))
						bit = int.Parse(item.Substring(14));

					if (item.Contains("a:channels"))
						chan = int.Parse(item.Substring(14));
                }

				if (freq == 0 || bit == 0 || chan == 0)
					return Items;

				Items.Add(new audio()
				{
					Basic = new basic
					{
						Id = "0:0",
						Lang = "und",
						Codec = "pcm",
						Format = "avs",
					},

					RawFreq = freq,
					RawBit = bit,
					RawChan = chan
				});

				return Items;
			}
			else
			{
				TaskManager.Run($"\"{Plugin.FFPROBE}\" \"{file}\" 2> {IdFFm}");
				foreach (var item in File.ReadAllLines(IdFFm))
				{
					if (item.Contains("Stream #"))
					{
						if (item.Contains(kind))
						{
							// Basic section
							var Common = Basic(kind, item);

							// Audio section
							int bit = 24;
							int freq = 48000;
							int chan = 2;

							// Frequency
							Regex regFreq = new Regex(@"\d{4,6} Hz");
							Match matchFreq = regFreq.Match(item);

							if (matchFreq.Success)
							{
								freq = int.Parse(new Regex(@"\d+").Match(matchFreq.Value).Value);
							}

							// Channel
							Regex regChan = new Regex(@"(stereo|mono|\d\.\d)");
							Match matchChan = regChan.Match(item);

							if (matchChan.Success)
							{
								if (matchChan.Value.Contains('.'))
									chan = int.Parse(matchChan.Value.Split('.')[0]) + int.Parse(matchChan.Value.Split('.')[1]);
								else if (string.Equals(matchChan.Value, "mono", IC))
									chan = 1;
								else
									chan = 2;
							}

							// Bit
							Regex regBit = new Regex(@"(flt|fltp|s\d{1,2})");
							Match matchBit = regBit.Match(item);

							if (matchBit.Success)
							{
								if (matchBit.Value.Contains('s'))
									bit = int.Parse(matchBit.Value.Substring(1));

								if (bit >= 32)
									bit = 24;
							}

							Items.Add(new audio()
							{
								Basic = new basic
								{
									Id = Common.Id,
									Lang = Common.Lang,
									Codec = Common.Codec,
									Format = Common.Format,
								},

								RawFreq = freq,
								RawBit = bit,
								RawChan = chan
							});
						}
                    }
                }
			}

			return Items;
		}

		public static List<basic> Subtitle(string file)
		{
			List<basic> Items = new List<basic>();
			string kind = "Subtitle";

			TaskManager.Run($"\"{Plugin.FFPROBE}\" \"{file}\" 2> {IdFFm}");
			foreach (var item in File.ReadAllLines(IdFFm))
			{
				if (item.Contains("Stream #"))
				{
					if (item.Contains(kind))
					{
						var Common = Basic(kind, item);

						Items.Add(new basic()
						{
							Id = Common.Id,
							Lang = Common.Lang,
							Codec = Common.Codec,
							Format = Common.Format
						});
					}
                }
			}

			return Items;
		}

		public static int FrameCount(string file)
		{
			Console.WriteLine("Reading encoded total frame, please wait...");
			TaskManager.Run($"\"{Plugin.FFPROBE}\" \"{file}\" -hide_banner -show_streams -count_frames -select_streams v:0 > streams.ff 2>&1");

			string frame = null;
			bool equal = false;
			foreach (var item in File.ReadAllLines(Path.Combine(Default.DirTemp, "streams.ff")))
			{
				if (item.Contains("nb_read_frames"))
				{
					foreach (var ch in item)
					{
						if (ch == '=')
						{
							equal = true;
							continue;
						}

						if (equal)
						{
							frame += ch;
						}
					}
				}
			}

			return Convert.ToInt32(frame);
		}

		public static bool IsAviSynth(string file)
		{
			string exts = Path.GetExtension(file);
			return string.Equals(exts, ".avs", IC) ? true : false;
		}

		public static string AviSynthGetFile(string file)
		{
			string folder = Path.GetDirectoryName(file);

			if (string.Equals(Path.GetExtension(file), ".avs", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var item in File.ReadAllLines(file))
				{
					foreach (var code in File.ReadAllLines("avisynthsource.code"))
					{
						if (item.Contains(code))
						{
							for (int i = item.IndexOf(code) + code.Length; i < item.Length; i++)
							{
								if (item[i] == '(' && item[i + 1] == '"')
								{
									i += 2;
									file = string.Empty;
									while (i < item.Length)
									{
										if (item[i] == '"' && (item[i + 1] == ',' || item[i + 1] == ')'))
										{
											break;
										}
										else
										{
											file += item[i];
										}
										i++;
									}
								}
							}
						}
					}
				}
			}

			if (file[0] == '/' || file[1] == ':') // check path is full or just file
			{
				return file; // return if file is full path
			}
			else
			{
				return Path.Combine(folder, file); // merge current folder & path
			}
		}

		public static int AviSynthFrameCount(string file)
		{
			string test = "";

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Please wait while IFME analysing AviSynth file...");
			Console.ResetColor();

			TaskManager.Run($"\"{Plugin.AVSPIPE}\" info \"{file}\" > avisynth.id");
			string[] result = File.ReadAllLines(Path.Combine(Default.DirTemp, "avisynth.id"));

			foreach (var item in result)
			{
				if (item.Contains("frames"))
				{
					foreach (var thing in item)
					{
						int x;
						if (int.TryParse(thing.ToString(), out x))
						{
							test += x.ToString();
						}
					}
				}
			}
			return string.IsNullOrEmpty(test) ? 0 : Convert.ToInt32(test);
		}
	}
}