using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IniParser;

namespace ifme
{
    public class Plugin
    {
        public static Dictionary<Guid, PluginCommon> Commmon = new Dictionary<Guid, PluginCommon>();
        public static Dictionary<Guid, PluginVideo> Video = new Dictionary<Guid, PluginVideo>();
        public static Dictionary<Guid, PluginAudio> Audio = new Dictionary<Guid, PluginAudio>();

        public static void Initialize()
        {
            // Plugin install path
            string dir = Path.Combine(Directory.GetCurrentDirectory(), "plugin");

            // Seek plugin definition file
            foreach (var item in Directory.GetDirectories(dir))
            {
                var file = Path.Combine(item, "_plugin.ini");

                if (!File.Exists(file))
                    continue;

                var data = new FileIniDataParser().ReadFile(file);
                var guid = new Guid();
                var type = -1;

                if (Guid.TryParse(data["config"]["guid"], out guid)) 
                {
                    if (int.TryParse(data["config"]["type"], out type))
                    {
                        if (type == 0)
                        {

                        }
                        else if (type == 1)
                        {
                            var p = new PluginVideo();

                            var prop = "properties";
                            p.Properties.Name = data[prop]["name"];
                            p.Properties.Version = data[prop]["ver"];
                            p.Properties.Developer = data[prop]["dev"];
                            p.Properties.Homepage = data[prop]["web"];
                            bool.TryParse(data[prop]["x64"], out p.Properties.Is64);
                            bool.TryParse(data[prop]["mp4"], out p.Properties.MP4Compatible);

                            var upd = "update";
                            p.Update.UrlUpdate = data[upd]["version"];
                            p.Update.UrlDownload = data[upd]["download"];

                            var vid = "video";
                            p.App.Ext = data[vid]["ext"];
                            p.App.Exe = data[vid]["exe"];
                            p.App.BitDepth = data[vid]["bitdepth"].Split(',');
                            p.App.Preset = data[vid]["preset"].Split(',');
                            p.App.Tune = data[vid]["tune"].Split(',');
                            p.App.TuneDefault = data[vid]["tunedefault"];
                            p.App.PresetDefault = data[vid]["presetdefault"];
                            int.TryParse(data[vid]["mode"], out p.App.Mode);

                            var arg = $"{vid}.args";
                            p.Args.Y4M = data[arg]["y4m"];
                            p.Args.Input = data[arg]["input"];
                            p.Args.Output = data[arg]["output"];
                            p.Args.Preset = data[arg]["preset"];
                            p.Args.Tune = data[arg]["tune"];
                            p.Args.FrameCount = data[arg]["framecount"];
                            p.Args.PassFirst = data[arg]["firstpass"];
                            p.Args.PassLast = data[arg]["lastpass"];
                            p.Args.PassAny = data[arg]["nthpass"];

                            for (int i = 0; i < p.App.Mode; i++)
                            {
                                var mod = $"{vid}.mode.{i}";
                                var pm = new PluginVideoMode();

                                pm.Name = data[mod]["name"];
                                pm.Arg = data[mod]["arg"];
                                int.TryParse(data[mod]["dp"], out pm.DecimalPlaces);
                                decimal.TryParse(data[mod]["step"], out pm.Step);
                                decimal.TryParse(data[mod]["min"], out pm.ValueMin);
                                decimal.TryParse(data[mod]["max"], out pm.ValueMax);
                                decimal.TryParse(data[mod]["default"], out pm.ValueDefault);
                                bool.TryParse(data[mod]["multipadd"], out pm.IsMultipass);

                                p.Mode.Add(pm);
                            }

                            Video.Add(guid, p);
                        }
                        else if (type == 2)
                        {
                            var p = new PluginAudio();

                            var prop = "properties";
                            p.Properties.Name = data[prop]["name"];
                            p.Properties.Version = data[prop]["ver"];
                            p.Properties.Developer = data[prop]["dev"];
                            p.Properties.Homepage = data[prop]["web"];
                            bool.TryParse(data[prop]["x64"], out p.Properties.Is64);
                            bool.TryParse(data[prop]["mp4"], out p.Properties.MP4Compatible);

                            var upd = "update";
                            p.Update.UrlUpdate = data[upd]["version"];
                            p.Update.UrlDownload = data[upd]["download"];

                            var aud = "audio";
                            p.App.Ext = data[aud]["ext"];
                            p.App.Exe = data[aud]["exe"];
                            p.App.SampleRate = data[aud]["frequency"].Split(',');
                            p.App.Channel = data[aud]["channel"].Split(',');
                            int.TryParse(data[aud]["mode"], out p.App.Mode);

                            var arg = $"{aud}.args";
                            p.Args.Input = data[arg]["input"];
                            p.Args.Output = data[arg]["output"];
                            p.Args.Advance = data[arg]["adv"];

                            for (int i = 0; i < p.App.Mode; i++)
                            {
                                var mod = $"{aud}.mode.{i}";
                                var pm = new PluginAudioMode();

                                pm.Name = data[mod]["name"];
                                pm.Arg = data[mod]["arg"];
                                pm.Quality = data[mod]["quality"].Split(',');
                                pm.QualityDefault = data[mod]["default"];

                                p.Mode.Add(pm);
                            }
                        }
                    }
                }
            }
        }
    }
}
