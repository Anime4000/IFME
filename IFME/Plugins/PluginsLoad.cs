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
        internal static string ErrorLog = string.Empty;
        private static readonly List<Guid> Disabled = Properties.Settings.Default.PluginsDisabled.Split(',').Select(Guid.Parse).ToList();

        private static bool IsExitError(int ExitCode) => ExitCode <= -1 || ExitCode == 1 || ExitCode == 127; // bash error code 127 for 'No such file or directory'

        internal PluginsLoad()
        {
            var folder = AppPath.Combine("Plugins");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            Audio(folder);

            frmSplashScreen.SetStatus(string.Empty);
            Thread.Sleep(500);			

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

                    // Parse into fully qualified path
                    if (Path.IsPathRooted(plugin.Audio.Encoder))
                        plugin.Audio.Encoder = Path.GetFullPath(plugin.Audio.Encoder);

                    if (OS.IsProgramInPath(plugin.Audio.Encoder))
                        plugin.Version = "$PATH"; // tell the plugins in in PATH Environment
                    else
                        plugin.Audio.Encoder = Path.GetFullPath(AppPath.Combine(Path.GetDirectoryName(item), plugin.Audio.Encoder));

                    if (OS.IsLinux)
                        plugin.Audio.Encoder = Path.GetFullPath(AppPath.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(plugin.Audio.Encoder)));

                    // Skip
                    if (plugin.GUID.Equals(new Guid("aaaaaaaa-0000-0000-0000-000000000000")))
                    {
                        Plugins.Items.Audio.Add(plugin.GUID, plugin);
                        continue;
                    }

                    frmSplashScreen.SetStatus($"{plugin.Name}");

                    // Skip wrong cpu arch
                    if (OS.Is64bit != plugin.X64)
                    {
                        frmSplashScreen.SetStatusAppend(" (incompatible architecture, skipping...)");
                        continue;
                    }

                    // Ensure default sample rate is included
                    if (!plugin.Audio.SampleRate.Contains(plugin.Audio.SampleRateDefault))
                    {
                        var hz = new[] { plugin.Audio.SampleRateDefault }.Concat(plugin.Audio.SampleRate).ToArray();
                        plugin.Audio.SampleRate = hz;
                    }

                    // Ensure default channel is included
                    if (!plugin.Audio.Channels.ContainsKey(plugin.Audio.ChannelDefault))
                    {
                        plugin.Audio.Channels.Add(plugin.Audio.ChannelDefault, "Default");
                    }

                    // Add List
                    Plugins.Items.Lists.Add(plugin.GUID, plugin);

                    // Skip disabled
                    if (Disabled.Contains(plugin.GUID))
                    {
                        frmSplashScreen.SetStatusAppend(" (disabled, skipping...)");
                        continue;
                    }

                    // Add Audio Encoder List
                    Plugins.Items.Audio.Add(plugin.GUID, plugin);

                    // Test
                    if (Properties.Settings.Default.TestEncoder)
                    {
                        if (!plugin.TestRequired)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        if (!TestAudio(plugin))
                        {
                            frmSplashScreen.SetStatusAppend(" (incompatible hardware, skipping...)");
                            frmSplashScreen.PrintLogAppend($"{plugin.Name} is not compatible for this system");

                            Plugins.Items.Lists[plugin.GUID].Version += "Error"; // remove from Plugins List
                            Plugins.Items.Audio.Remove(plugin.GUID); // remove from Audio Encoder List

                            Properties.Settings.Default.PluginsDisabled += $",{plugin.GUID}";

                            continue;
                        }

                        continue;
                    }

                    frmSplashScreen.PrintLogAppend($"{plugin.Name} is skip from initialising");
                }
                catch (Exception ex)
                {
                    ErrorLog += $"[ERR ] {ex.Message}\r\n";
                    frmSplashScreen.PrintLogAppend(ex.Message);
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

                    // Parse into fully qualified path
                    foreach (var p in plugin.Video.Encoder)
                    {
                        if (Path.IsPathRooted(p.Binary))
                            p.Binary = Path.GetFullPath(p.Binary);

                        if (OS.IsProgramInPath(p.Binary))
                            plugin.Version = "$PATH"; // tell the plugins in in PATH Environment
                        else
                            p.Binary = Path.GetFullPath(AppPath.Combine(Path.GetDirectoryName(item), p.Binary));

                        if (OS.IsLinux)
                            p.Binary = Path.GetFullPath(AppPath.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(p.Binary)));
                    }

                    // Skip
                    if (plugin.GUID.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                    {
                        Plugins.Items.Video.Add(plugin.GUID, plugin);
                        continue;
                    }

                    frmSplashScreen.SetStatus($"{plugin.Name}");

                    // Skip wrong cpu arch
                    if (OS.Is64bit != plugin.X64)
                    {
                        frmSplashScreen.SetStatusAppend(" (incompatible architecture, skipping...)");
                        continue;
                    }

                    // Check if plugins already in the list
                    if (Plugins.Items.Video.ContainsKey(plugin.GUID) || Plugins.Items.Lists.ContainsKey(plugin.GUID))
                    {
                        frmSplashScreen.SetStatusAppend(" (best encoder already loaded, skipping...)");
                        continue;
                    }

                    // Add List
                    Plugins.Items.Lists.Add(plugin.GUID, plugin);

                    // Skip disabled by user
                    if (Disabled.Contains(plugin.GUID))
                    {
                        frmSplashScreen.SetStatusAppend(" (disabled, skipping...)");
                        continue;
                    }

                    // Add Video Encoder List
                    Plugins.Items.Video.Add(plugin.GUID, plugin);

                    // Test
                    if (Properties.Settings.Default.TestEncoder)
                    {
                        if (!plugin.TestRequired)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        if (!TestVideo(plugin))
                        {
                            frmSplashScreen.SetStatusAppend(" (incompatible hardware, skipping...)");
                            frmSplashScreen.PrintLogAppend($"{plugin.Name} is not compatible for this system");

                            Plugins.Items.Lists[plugin.GUID].Version += "System Incompatible"; // remove from Plugins List
                            Plugins.Items.Video.Remove(plugin.GUID); // remove from Video Encoder List

                            Properties.Settings.Default.PluginsDisabled += $",{plugin.GUID}";

                            Thread.Sleep(100);

                            continue;
                        }

                        continue;
                    }

                    frmSplashScreen.PrintLogAppend($"{plugin.Name} is skip from initialising");
                }
                catch (Exception ex)
                {
                    ErrorLog += $"[ERR ] {ex.Message}\r\n";
                    frmSplashScreen.PrintLogAppend(ex.Message);
                    Thread.Sleep(5000);
                }
            }
        }

        private bool TestAudio(PluginsAudio codec)
        {
            var ac = codec.Audio;
            var ff = MediaEncoding.FFmpeg;
            var en = ac.Encoder;
            var sampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples", "ballz.m4a");
            var outTempFile = Path.Combine(Path.GetTempPath(), $"test_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_ffff}.{ac.Extension}");
            var outTempFolder = Path.Combine(Path.GetTempPath());

            var qu = ac.Mode[0].Args.IsDisable() ? "" : $"{ac.Mode[0].Args} {ac.Mode[0].QualityPrefix}{ac.Mode[0].Default}{ac.Mode[0].QualityPostfix}";

            var sampleRate = ac.SampleRateDefault == 0 ? "" : $"{ac.SampleRateArgs} {ac.SampleRateDefault}";
            var channel = ac.ChannelDefault == 0 ? "" : $"{ac.ChannelArgs} {ac.ChannelDefault}";

            if (!ac.Mode[0].MultiChannelSupport)
                channel = $"{ac.ChannelArgs} 2";

            int exitCode;
            if (ac.Args.Pipe)
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{ff}\" -hide_banner -v error -i \"{sampleFile}\" {sampleRate} {channel} -f wav - | \"{en}\" {qu} {ac.Args.Command} {ac.Args.Input} {ac.Args.Output} \"{outTempFile}\"");
            }
            else
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{en}\" {ac.Args.Input} \"{sampleFile}\" {ac.Args.Codec} {qu} {sampleRate} {channel} {ac.Args.Output} \"{outTempFile}\"");
            }

            return !IsExitError(exitCode);
        }

        private bool TestVideo(PluginsVideo codec)
        {
            var vc = codec.Video;
            var ff = MediaEncoding.FFmpeg;
            var en = vc.Encoder[0].Binary;
            var sampleFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples", "ballz.mp4");
            var outTempFile = Path.Combine(Path.GetTempPath(), $"test_{DateTime.Now:yyyy-MM-dd_HH-mm-ss_ffff}.{vc.Extension}");
            var outTempFolder = Path.Combine(Path.GetTempPath());

            var val_w = 320;
            var val_h = 240;
            var val_fps = 15;
            var val_bit = 8;

            var ff_raw = string.IsNullOrEmpty(vc.Args.Y4M) ? "-strict -1 -f rawvideo" : "-strict -1 -f yuv4mpegpipe";

            var en_res = string.Empty;
            var en_fps = string.Empty;
            var en_bit = string.Empty;
            var en_frameCount = vc.Args.FrameCount.IsDisable() ? string.Empty : $"{vc.Args.FrameCount} 1000";

            if (!string.IsNullOrEmpty(vc.Args.Resolution))
                en_res = string.Format(vc.Args.Resolution, val_w, val_h);

            if (!string.IsNullOrEmpty(vc.Args.FrameRate))
                en_fps = $"{vc.Args.FrameRate} {val_fps}";

            if (!string.IsNullOrEmpty(vc.Args.BitDepthIn))
                en_bit = $"{vc.Args.BitDepthIn} {val_bit}";

            if (!string.IsNullOrEmpty(vc.Args.BitDepthOut))
                en_bit += $" {vc.Args.BitDepthOut} {val_bit}";

            var ff_cmd = $"-pix_fmt yuv420p -vf \"scale={val_w}:{val_h}:flags=lanczos,fps={val_fps}\"";
            var en_cmd = $"{en_res} {en_fps} {en_bit} {vc.Chroma[0].Command} {vc.Args.Preset} {vc.PresetDefault} {vc.Args.Tune} {vc.TuneDefault} {en_frameCount}";

            int exitCode;
            if (codec.Video.Args.Pipe)
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{ff}\" -hide_banner -v error -i \"{sampleFile}\" {ff_raw} {ff_cmd} - | \"{en}\" {vc.Args.Input} {vc.Args.Y4M} {en_cmd} {vc.Args.Output} {outTempFile}");
            }
            else
            {
                exitCode = ProcessManager.Start(outTempFolder, $"\"{en}\" {vc.Args.Input} \"{sampleFile}\" {vc.Args.UnPipe} {vc.Args.Output} {outTempFile}");
            }

            return !IsExitError(exitCode);
        }

        internal void MakeFile()
        {
            var Temp = new PluginsAudio
            {
                GUID = Guid.Parse("deadbeef-faac-faac-faac-faacfaacfaa1"),
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
                    SampleRateArgs = "-ar",
                    SampleRateDefault = 44100,
                    Channels = new SortedList<int, string>
                    {
                        { -1, "auto" },
                        { 0, "stereo" },
                        { 1, "joint stereo" },
                        { 2, "dual_channel" },
                        { 3, "mono" }
                    },
                    ChannelArgs = "--mode",
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
                            MultiChannelSupport = true,
                            Quality = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" },
                            Default = "12"
                        }
                    }
                }
            };

            string json = JsonConvert.SerializeObject(Temp, Formatting.Indented);
            File.WriteAllText("test.json", json);
        }
    }
}
