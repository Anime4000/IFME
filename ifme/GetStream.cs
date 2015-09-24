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
		public string Format;
		public string OtherInfo;
	}

	public class StreamMatroska
	{
		public string ID;
		public string Mime;
		public string File;
	}

	public class GetStream
	{
		private static IniData GetFmt = new FileIniDataParser().ReadFile(Path.Combine(Global.Folder.App, "format.ini"), Encoding.UTF8);

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

			if (GetInfo.IsAviSynth(file) && StreamType.Video == kind)
			{
				Items.Add(new StreamMedia() { ID = "0:0", Lang = "und", Format = "avs" }); // send fake data for AviSynth
				return Items;
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
					string otherinfo = string.Empty;

					if (item.Contains(Kind))
					{
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

						otherinfo = item.Substring(x);

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
						
						Items.Add(new StreamMedia() { ID = id, Lang = lang, Format = format, OtherInfo = otherinfo });
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
						Items.Add(new StreamMatroska() { ID = id, File = "font_" + fn, Mime = me });
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

		public static string AviSynthGetFile(string file)
		{
			if (string.Equals(Path.GetExtension(file), ".avs", StringComparison.OrdinalIgnoreCase))
			{
				foreach (var item in File.ReadAllLines(file))
				{
					foreach (var code in File.ReadAllLines(Path.Combine(Global.Folder.App, "avisynthsource.code")))
					{
						if (item.Contains(code))
						{
							for (int i = 0; i < item.Length; i++)
							{
								if (item[i] == '(' && item[i + 1] == '"')
								{
									i += 2;
									file = "";
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
				return file; // return file
			}
			return file; // return if not avs file
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