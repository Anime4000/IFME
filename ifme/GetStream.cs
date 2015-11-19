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
	public enum StreamType
	{
		Video,
		Audio,
		Subtitle,
		Attachment
	}

	public class StreamMedia
	{
		public string ID;
		public string Lang;
		public string Codec;
		public string Format;

		public int AudioRawBit;
		public int AudioRawFreq;
		public int AudioRawChan;
	}

	public class StreamMatroska
	{
		public string ID;
		public string Mime;
		public string File;
	}

	public class GetStream
	{
		private static StringComparison IC = StringComparison.InvariantCultureIgnoreCase; // Just ignore case what ever it is.

		private static IniData GetFmt { get { return new FileIniDataParser().ReadFile("format.ini", Encoding.UTF8); } }

        public static List<StreamMedia> Media(string file, StreamType kind)
		{
			List<StreamMedia> Items = new List<StreamMedia>();
			string Kind = string.Empty;

			switch (kind)
			{
				case StreamType.Video:
					Kind = "Video";
					break;
				case StreamType.Audio:
					Kind = "Audio";
					break;
				case StreamType.Subtitle:
					Kind = "Subtitle";
					break;
				case StreamType.Attachment:
					Kind = "Attachment";
					break;
			}

			if (IsAviSynth(file) && StreamType.Video == kind)
			{
				Items.Add(new StreamMedia() { ID = "0:0", Lang = "und", Format = "avs", Codec = "avs" }); // send fake data for AviSynth
				return Items;
			}
			else
			{
				// Get internal media file
				file = AviSynthGetFile(file);
			}

			TaskManager.Run($"\"{Plugin.PROBE}\" \"{file}\" 2> streams.id");
			foreach (var item in File.ReadAllLines(Path.Combine(Default.DirTemp, "streams.id")))
			{
				if (item.Contains("Stream #"))
				{
					string id = string.Empty;
					string lang = string.Empty;
					string codec = string.Empty;
					string format = string.Empty;

					// Audio
					string audiobit = "0";
					string audiofreq = "0";
					string audiochan = "0";

					if (item.Contains(Kind))
					{
						// Basic data
						for (int i = item.IndexOf('#') + 1; i < item.Length; i++)
						{
							if (item[i] == '(')
								break;
							if (item[i] == '[')
								break;
							if (item[i] == ':' && id.Contains(':'))
								break;

							id += item[i];
						}

						if (item[item.IndexOf(Kind) - 3] == ')')
							lang = item.Substring(item.IndexOf('(') + 1, item.IndexOf(')') - (item.IndexOf('(') + 1));
						else
							lang = "und";

						int x = item.IndexOf(Kind) + (Kind.Length + 2);
						for (int i = x; i < item.Length; i++)
						{
							if (item[i] == ' ')
								break;
							if (item[i] == ',')
								break;

							codec += item[i];
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

						// Audio data
						if (kind == StreamType.Audio)
						{
							// Re init
							audiobit = string.Empty;
							audiofreq = string.Empty;
							audiochan = string.Empty;

							if (item.Contains(Kind))
							{
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

								if (audiobit.Length > 2)
									if (audiobit[0] == '3' && audiobit[1] == '2')
										audiobit = "24"; // force to 24
								
								if (string.IsNullOrEmpty(audiobit))
									audiobit = "16"; // fltp (32 bits floats, planar) use for decode lossy codec
							}
                        }

						// Add
						Items.Add(new StreamMedia() {
							ID = id,
							Lang = Language.IdLookup(lang),
							Codec = codec,
							Format = format,

							AudioRawBit = Convert.ToInt32(audiobit),
							AudioRawFreq = Convert.ToInt32(audiofreq),
							AudioRawChan = Convert.ToInt32(audiochan)
						});
					}
				}
			}
			return Items;
		}

		public static List<StreamMatroska> MediaMkv(string file, StreamType kind)
		{
			List<StreamMatroska> Items = new List<StreamMatroska>();
			string Kind = string.Empty;

			switch (kind)
			{
				case StreamType.Video:
					Kind = "video";
					break;
				case StreamType.Audio:
					Kind = "audio";
					break;
				case StreamType.Subtitle:
					Kind = "subtitles";
					break;
				case StreamType.Attachment:
					Kind = "Attachment";
					break;
			}

			TaskManager.Run($"\"{Path.Combine(Global.Folder.Plugins, "mkvtool", "mkvmerge")}\" -i \"{file}\" > list.id");
			foreach (var x in File.ReadAllLines(Path.Combine(Default.DirTemp, "list.id")))
			{
				if (kind == StreamType.Attachment)
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
						Items.Add(new StreamMatroska() { ID = id, File = fn, Mime = me });
					}
				}
				else
				{
					if (x.Contains(Kind))
					{
						string id = null;

						for (int i = 9; i < x.Length; i++)
						{
							if (x[i] == ':')
								break;

							id += x[i];
						}
						Items.Add(new StreamMatroska() { ID = id, File = GetInfo.FileNameNoExt(file) });
					}
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
			foreach (var item in File.ReadAllLines(Path.Combine(Properties.Settings.Default.DirTemp, "streams.ff")))
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

			if (file[0] == '/' || file?[1] == ':') // check path is full or just file
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