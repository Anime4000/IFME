using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

					frmSplashScreen.SetStatus($"Loading Plugins: {plugin.Name}");

					if (!TestAudio(plugin))
                    {
						frmSplashScreen.SetStatus($"Loading Plugins: {plugin.Name} (incompatible host, skipping...)");
						Thread.Sleep(2000);
						continue;
					}

					Plugins.Items.Audio.Add(plugin.GUID, plugin);

				}
				catch (Exception ex)
				{
					frmMain.PrintLog($"[WARN] {ex.Message}");
				}
			}
;		}

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

					frmSplashScreen.SetStatus($"Loading Plugins: {plugin.Name}");

					if (!TestVideo(plugin))
                    {
						frmSplashScreen.SetStatus($"Loading Plugins: {plugin.Name} (incompatible host, skipping...)");
						Thread.Sleep(2000);
						continue;
					}

					Plugins.Items.Video.Add(plugin.GUID, plugin);
				}
				catch (Exception ex)
				{
					frmMain.PrintLog($"[WARN] {ex.Message}");
				}
			}
		}

		private bool TestAudio(PluginsAudio codec)
        {
			var ac = codec.Audio;
			var ff = MediaEncoding.FFmpeg;
			var en = Path.Combine(codec.FilePath, ac.Encoder);
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
			var en = Path.Combine(codec.FilePath, vc.Encoder[0].Binary);
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
