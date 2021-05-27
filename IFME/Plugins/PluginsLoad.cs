using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace IFME
{
	internal class PluginsLoad
	{
		internal PluginsLoad()
		{
			var folder = Path.GetFullPath("Plugins");

			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			Audio(folder);
			Video(folder);
		}

		private void Audio(string folder)
		{
			foreach (var item in Directory.EnumerateFiles(folder, "_plugin.a*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var plugin = JsonConvert.DeserializeObject<PluginsAudio>(json);

					frmSplashScreen.SetStatus($"Initializing Audio:\n{plugin.Name}");

					// Skip wrong cpu arch
					if (OSManager.OS.Is64bit != plugin.X64)
					{
						frmSplashScreen.SetStatusAppend(" (incompatible architecture, skipping...)");
						Thread.Sleep(250);
						continue;
					}

					// Parse into fully qualified path
					if (Path.IsPathRooted(plugin.Audio.Encoder))
						plugin.Audio.Encoder = Path.GetFullPath(plugin.Audio.Encoder);
					else
						plugin.Audio.Encoder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(item), plugin.Audio.Encoder));

					if (!TestAudio(plugin))
                    {
						frmSplashScreen.SetStatusAppend(" (incompatible hardware, skipping...)");
						Thread.Sleep(2000);
						continue;
					}

					Plugins.Items.Audio.Add(plugin.GUID, plugin);

				}
				catch (Exception ex)
				{
					frmSplashScreen.SetStatusAppend($" [{ex.Message}]");
					Thread.Sleep(5000);
				}
			}
;		}

		private void Video(string folder)
		{
			foreach (var item in Directory.EnumerateFiles(folder, "_plugin.v*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var plugin = JsonConvert.DeserializeObject<PluginsVideo>(json);

					frmSplashScreen.SetStatus($"Initializing Video:\n{plugin.Name}");

					// Skip wrong cpu arch
					if (OSManager.OS.Is64bit != plugin.X64)
					{
						frmSplashScreen.SetStatusAppend(" (incompatible architecture, skipping...)");
						Thread.Sleep(250);
						continue;
					}

					// Parse into fully qualified path
					foreach (var p in plugin.Video.Encoder)
                    {
						if (Path.IsPathRooted(p.Binary))
							p.Binary = Path.GetFullPath(p.Binary);
						else
							p.Binary = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(item), p.Binary));
                    }

					// Check if plugins already in the list
					if (Plugins.Items.Video.ContainsKey(plugin.GUID))
                    {
						frmSplashScreen.SetStatusAppend(" (best encoder already loaded, skipping...)");
						Thread.Sleep(2000);
						continue;
					}

					if (!TestVideo(plugin))
                    {
						frmSplashScreen.SetStatusAppend(" (incompatible hardware, skipping...)");
						Thread.Sleep(2000);
						continue;
					}

					Plugins.Items.Video.Add(plugin.GUID, plugin);
				}
				catch (Exception ex)
				{
					frmSplashScreen.SetStatusAppend($" [{ex.Message}]");
					Thread.Sleep(5000);
				}
			}
		}

		private bool TestAudio(PluginsAudio codec)
        {
			var ac = codec.Audio;
			var ff = MediaEncoding.FFmpeg;
			var en = ac.Encoder;
			var sampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples", "wonderland_online.m4a");
			var outTempFile = Path.Combine(Path.GetTempPath(), $"test_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_ffff}.{ac.Extension}");
			var outTempFolder = Path.Combine(Path.GetTempPath());

			var qu = ac.Mode[0].Args.IsDisable() ? "" : $"{ac.Mode[0].Args} {ac.Mode[0].QualityPrefix}{ac.Mode[0].Default}{ac.Mode[0].QualityPostfix}";

			int exitCode;
			if (ac.Args.Pipe)
			{
				exitCode = ProcessManager.Start(outTempFolder, $"\"{ff}\" -hide_banner -v error -i \"{sampleFile}\" -ar {ac.SampleRateDefault} -ac {ac.ChannelDefault} -f wav - | \"{en}\" {qu} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} \"{outTempFile}\"");
			}
			else
			{
				exitCode = ProcessManager.Start(outTempFolder, $"\"{en}\" {ac.Args.Input} \"{sampleFile}\" {ac.Args.Codec} {qu} -ar {ac.SampleRateDefault} -ac {ac.ChannelDefault} {ac.Args.Output} \"{outTempFile}\"");
			}

			return exitCode == 0;
		}

		private bool TestVideo(PluginsVideo codec)
        {
            var vc = codec.Video;
            var ff = MediaEncoding.FFmpeg;
			var en = vc.Encoder[0].Binary;
			var sampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples", "wonderland_online.mp4");
			var outTempFile = Path.Combine(Path.GetTempPath(), $"test_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_ffff}.{vc.Extension}");
			var outTempFolder = Path.Combine(Path.GetTempPath());

            var frameCount = vc.Args.FrameCount.IsDisable() ? string.Empty : $"{vc.Args.FrameCount} 1000";

            int exitCode;
            if (codec.Video.Args.Pipe)
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{ff}\" -hide_banner -v error -i \"{sampleFile}\" -strict -1 -f yuv4mpegpipe -pix_fmt yuv420p -vf scale=640:-1 - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {vc.Args.Preset} {vc.PresetDefault} {vc.Args.Tune} {vc.TuneDefault} {frameCount} {vc.Args.Output} {outTempFile}");
            }
            else
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{ff}\" {vc.Args.Input} \"{sampleFile}\" -pix_fmt yuv420p {vc.Args.UnPipe} {vc.Args.Preset} {vc.PresetDefault} {vc.Args.Tune} {vc.TuneDefault} {frameCount} {vc.Args.Output} {outTempFile}");
            }

            return exitCode == 0;
        }

		internal void MakeFile()
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
