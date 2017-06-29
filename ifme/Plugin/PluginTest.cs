using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ifme
{
    public class PluginTest
    {
        public static void JsonWrite()
        {
            var Temp = new Plugin();
            Temp.GUID = Guid.Parse("deadbeef-faac-faac-faac-faacfaacfaac");
            Temp.Name = "Test";
            Temp.Version = "10.1.1";
            Temp.X64 = true;
            Temp.Format = new string[] { "mp4", "mkv" };
            Temp.Author.Developer = "Jebon Inc.";
            Temp.Author.URL = new Uri("http://test.com/");
            Temp.Audio.Extension = "mp4";
            Temp.Audio.Encoder = "faac.exe";
            Temp.Audio.SampleRate = new int[] { 8000, 12000, 16000, 22050, 24000, 32000, 44100, 48000 };
            Temp.Audio.SampleRateDefault = 44100;
            Temp.Audio.Channel = new int[] { 0, 1, 2 };
            Temp.Audio.ChannelDefault = 0;
            Temp.Audio.Args.Pipe = true;
            Temp.Audio.Args.Input = "-";
            Temp.Audio.Args.Output = "-o";
            Temp.Audio.Args.Command = "-w -s -c 24000";
            Temp.Audio.Mode.Add(new PluginAudioMode
            {
                Name = "Bit Rate",
                Args = "-b",
                Quality = new decimal[] { 24, 32, 48, 64, 96, 112, 128, 160, 192, 256, 384, 512 },
                Default = 128
            });
            Temp.Audio.Mode.Add(new PluginAudioMode
            {
                Name = "Quality",
                Args = "-q",
                Quality = new decimal[] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160, 170, 180, 190, 200, 210, 220, 230, 240, 250, 260, 270, 280, 290, 300, 310, 320, 330, 340, 350, 360, 370, 380, 390, 400, 410, 420, 430, 440, 450, 460, 470, 480, 490, 500 },
                Default = 120
            });
            Temp.Video.Extension = "hevc";
            Temp.Video.Encoder.Add(new PluginVideoEncoder
            {
                BitDepth = 8,
                Binary = "x265-08.exe"
            });
            Temp.Video.Encoder.Add(new PluginVideoEncoder
            {
                BitDepth = 10,
                Binary = "x265-10.exe"
            });
            Temp.Video.Encoder.Add(new PluginVideoEncoder
            {
                BitDepth = 12,
                Binary = "x265-12.exe"
            });
            Temp.Video.Preset = new string[] { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow", "placebo" };
            Temp.Video.PresetDefault = "medium";
            Temp.Video.Tune = new string[] { "psnr", "ssim", "grain", "zerolatency", "fastdecode" };
            Temp.Video.TuneDefault = "psnr";
            Temp.Video.Args.Pipe = true;
            Temp.Video.Args.Y4M = "--y4m";
            Temp.Video.Args.Input = "-";
            Temp.Video.Args.Output = "-o";
            Temp.Video.Args.Preset = "-p";
            Temp.Video.Args.Tune = "-t";
            Temp.Video.Args.BitDepth = "--output-depth";
            Temp.Video.Args.FrameCount = "-f";
            Temp.Video.Args.PassFirst = "--pass 1";
            Temp.Video.Args.PassLast = "--pass 2";
            Temp.Video.Args.PassNth = "--pass 3";
            Temp.Video.Mode.Add(new PluginVideoMode
            {
                Name = "Single pass, Quality-based",
                Args = "--crf",
                MultiPass = false,
                Value = new PluginVideoModeValue { DecimalPlace = 1, Step = 0.1m, Min = 0, Max = 51, Default = 28}
            });
            Temp.Video.Mode.Add(new PluginVideoMode
            {
                Name = "Single pass, Target bitrate (kbps)",
                Args = "--bitrate",
                MultiPass = false,
                Value = new PluginVideoModeValue { DecimalPlace = 0, Step = 1024, Min = 512, Max = 10485760, Default = 1024 }
            });
            Temp.Video.Mode.Add(new PluginVideoMode
            {
                Name = "Multi pass, Target bitrate (kbps)",
                Args = "--bitrate",
                MultiPass = true,
                Value = new PluginVideoModeValue { DecimalPlace = 0, Step = 1024, Min = 512, Max = 10485760, Default = 1024 }
            });

            string json = JsonConvert.SerializeObject(Temp, Formatting.Indented);
            File.WriteAllText("test.json", json);
        }

        public static bool JsonRead()
        {
            try
            {
                var Temp = JsonConvert.DeserializeObject<Plugin>(File.ReadAllText(Path.Combine(Get.AppRootDir, "test.json")));
                var Load = Temp;
            }
            catch (Exception)
            {
                throw;
            }

            return false;
        }
    }
}
