using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using IniParser;
using IniParser.Model;

using static ifme.Properties.Settings;

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
			string lang = string.Empty;
			string codec = string.Empty;
			string format = string.Empty;

			// Besure it's right line
			if (data.Contains("Stream #"))
			{
				// ID section
				for (int i = data.IndexOf('#') + 1; i < data.Length; i++)
				{
					if (data[i] == '(')
						break;
					if (data[i] == '[')
						break;
					if (data[i] == ':' && id.Contains(':'))
						break;

					id += data[i];
				}

				// Lang section
				if (data[data.IndexOf(kind) - 3] == ')')
					lang = data.Substring(data.IndexOf('(') + 1, data.IndexOf(')') - (data.IndexOf('(') + 1));
				else
					lang = "und";

				// Codec section
				int x = data.IndexOf(kind) + (kind.Length + 2);
				for (int i = x; i < data.Length; i++)
				{
					if (data[i] == ' ')
						break;
					if (data[i] == ',')
						break;

					codec += data[i];
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
				TaskManager.Run($"\"{Plugin.PROBE}\" \"{file}\" 2> {IdFFm}");
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
				string audiobit = "0";
				string audiofreq = "0";
				string audiochan = "0";

				TaskManager.Run($"\"{Plugin.AVS4P}\" info \"{file}\" > {IdAvs}");
				foreach (var item in File.ReadAllLines(IdAvs))
				{
					if (item.Contains("a:sample_rate"))
						audiofreq = item.Substring(14);

					if (item.Contains("a:bit_depth"))
						audiobit = item.Substring(14);

					if (item.Contains("a:channels"))
						audiochan = item.Substring(14);
                }

				int freq = Convert.ToInt32(audiofreq);
                int bit = Convert.ToInt32(audiobit);
				int chan = Convert.ToInt32(audiochan);

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
				TaskManager.Run($"\"{Plugin.PROBE}\" \"{file}\" 2> {IdFFm}");
				foreach (var item in File.ReadAllLines(IdFFm))
				{
					if (item.Contains("Stream #"))
					{
						if (item.Contains(kind))
						{
							// Basic section
							var Common = Basic(kind, item);

							// Audio section
							string audiobit = string.Empty;
							string audiofreq = string.Empty;
							string audiochan = string.Empty;

							// Frequency
							for (int i = item.IndexOf("Hz, ") - 2; i > -1; i--)
							{
								if (item[i] == ' ')
									break;

								if (char.IsDigit(item[i]))
									audiofreq += item[i];
							}

							audiofreq = new string(audiofreq.Reverse().ToArray());

							// Channel
							for (int i = item.IndexOf("Hz, ") + 4; i < item.Length; i++)
							{
								if (item[i] == ',')
									break;

								if (item[i] == '(')
									break;

								audiochan += item[i];
							}

							if (string.Equals("stereo", audiochan, IC))
								audiochan = "2";
							else if (string.Equals("mono", audiochan, IC))
								audiochan = "1";
							else if (!string.IsNullOrEmpty(audiochan) && audiochan.Contains('.'))
								audiochan = $"{Convert.ToInt32(audiochan.Split('.')[0]) + Convert.ToInt32(audiochan.Split('.')[1])}";
							else if (char.IsDigit(audiochan[0]))
								audiochan = $"{audiochan[0]}";
							else
								audiochan = "2"; // default

							// Bit
							for (int i = item.IndexOf("Hz, ") + 4; i < item.Length; i++)
							{
								if (item[i] == ',')
								{
									i += 2;

									while (i < item.Length)
									{
										if (item[i] == ',')
											break;

										if (char.IsDigit(item[i]))
											audiobit += item[i];

										i++;
									}

									break;
								}
							}

							if (string.IsNullOrEmpty(audiobit))
								audiobit = "16"; // fltp (32 bits floats, planar) use for decode lossy codec

							int freq = Convert.ToInt32(audiofreq);
							int bit = Convert.ToInt32(audiobit);
							int chan = Convert.ToInt32(audiochan);

							if (freq == 0 || bit == 0 || chan == 0)
								continue;

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

			TaskManager.Run($"\"{Plugin.PROBE}\" \"{file}\" 2> {IdFFm}");
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

		public static List<matroska> MkvAttachment(string file)
		{
			List<matroska> Items = new List<matroska>();
			string Kind = "Attachment";

			TaskManager.Run($"\"{Plugin.MKVME}\" -i \"{file}\" > {IdMkv}");
			foreach (var x in File.ReadAllLines(IdMkv))
			{
				if (x.Contains(Kind))
				{
					string id = null;
					string fn = null;
					string me = null;
					char[] f = x.ToCharArray();

					for (int i = 0; i < f.Length; i++)
					{
						if (f[i] == ':')
							break;

						if (char.IsNumber(f[i]))
							id += f[i];
					}

					for (int i = f.Length - 1; i > 0; i--)
					{
						if (f[i] == '\'' && f[i - 1] == ' ')
							break;

						if (f[i] == '\'')
							continue;

						fn += f[i];
					}

					char[] tmp = fn.ToCharArray();
					Array.Reverse(tmp);
					fn = new string(tmp);

					for (int i = 0; i < f.Length; i++)
					{
						if (f[i] == '\'')
						{
							for (int c = i + 1; c < f.Length; c++)
							{
								if (f[c] == '\'')
									break;

								me += f[c];
							}
							break;
						}
					}
					Items.Add(new matroska() { ID = id, File = fn, Mime = me });
				}
			}
			return Items;
		}

		public static int FrameCount(string file)
		{
			Console.WriteLine("Reading encoded total frame, please wait...");
			TaskManager.Run($"\"{Plugin.PROBE}\" \"{file}\" -hide_banner -show_streams -count_frames -select_streams v:0 > streams.ff 2>&1");

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

			TaskManager.Run($"\"{Plugin.AVS4P}\" info \"{file}\" > avisynth.id");
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