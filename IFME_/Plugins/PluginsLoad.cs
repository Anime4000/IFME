using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IFME
{
	internal class PluginsLoad
	{
		internal PluginsLoad()
		{
			Audio();
			Video();
		}

		private void Audio()
		{
			var path = Path.Combine("Plugins");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var folder = Path.Combine(Directory.GetCurrentDirectory(), path);

			foreach (var item in Directory.EnumerateFiles(folder, "_plugin.a*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var plugin = JsonConvert.DeserializeObject<PluginsAudio>(json);

					plugin.FilePath = Path.GetDirectoryName(item);

					Plugins.Items.Audio.Add(plugin.GUID, plugin);
				}
				catch (Exception ex)
				{
					Console2.WriteLine($"[WARN] {ex.Message}");
				}
			}
		}

		private void Video()
		{
			var path = Path.Combine("Plugins");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var folder = Path.Combine(Directory.GetCurrentDirectory(), path);

			foreach (var item in Directory.EnumerateFiles(folder, "_plugin.v*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var plugin = JsonConvert.DeserializeObject<PluginsVideo>(json);

					plugin.FilePath = Path.GetDirectoryName(item);

					Plugins.Items.Video.Add(plugin.GUID, plugin);
				}
				catch (Exception ex)
				{
					Console2.WriteLine($"[WARN] {ex.Message}");
				}
			}
		}

		internal void Test()
		{
			var Temp = new PluginsAudio
			{
				GUID = Guid.Parse("deadbeef-faac-faac-faac-faacfaacfaac"),
				Name = "Test",
				Version = "10.1.1",
				X64 = true,
				Format = new string[] { "mp4", "mkv" },
				Author = new PluginsAuthor { Developer = "", URL = new Uri("https://example.com") },
				Update = new PluginsUpdate { VersionURL = new Uri("https://example.com"), DownloadURL = new Uri("https://example.com") },
				Audio = new PluginsAudioProp
				{
					Extension = "flac",
					Encoder = "../../decoder/ffmpeg64/ffmpeg",
					SampleRate = new int[] { 8000, 12000, 16000, 22050, 24000, 32000, 44100, 48000 },
					SampleRateDefault = 44100,
					Channel = new int[] { 1, 2 },
					ChannelDefault = 2,
					Args = new PluginsAudioArgs
					{
						Pipe = false,
						Input = "-hide_banner -v error -stats -i",
						Output = "-vn -sn -dn -codec:a flac -y",
						Command = "-map_metadata -1 -map_chapters -1"
					},
					Mode = new List<PluginsAudioMode>
					{
						new PluginsAudioMode
						{
							Name = "Level",
							Args = "-compression_level",
							Quality = new decimal[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
							Default = 12
						}
					}
				}
			};

			string json = JsonConvert.SerializeObject(Temp, Formatting.Indented);
			File.WriteAllText("test.json", json);
		}
	}
}
