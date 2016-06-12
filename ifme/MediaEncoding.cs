using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FFmpegDotNet;

namespace ifme
{
    public class MediaEncoding
    {
        private static StringComparison IC { get { return StringComparison.InvariantCultureIgnoreCase; } }

        private string AppDir { get { return Environment.CurrentDirectory; } }
        private string FFmpeg { get { return FFmpegDotNet.FFmpeg.Main; } }
        private string MKVExtract { get { return Path.Combine(AppDir, "plugin", "mkvtoolnix", "mkvextract"); } }
        private string MKVMerge { get { return Path.Combine(AppDir, "plugin", "mkvtoolnix", "mkvmerge"); } }
        private string MP4Box { get { return Path.Combine(AppDir, "plugin", "mp4box", "MP4Box"); } }
        private string MP4Fps { get { return Path.Combine(AppDir, "plugin", "mp4fpsmod", "mp4fpsmod"); ; } }
        private string FFms { get { return Path.Combine(AppDir, "plugin", "ffms", "ffmsindex"); } }

        string Temp { get { return Properties.Settings.Default.TempFolder; } }

        public MediaEncoding(Queue media)
        {
            ReadyTemp();

            if (Extract(media) == 0)
                if (Indexing(media) == 0)
                    if (Audio(media) == 0)
                        if (Video(media) == 0)
                            if (Muxing(media) == 0)
                                return;

            Backup(Path.Combine(Properties.Settings.Default.SaveFolder, Path.GetFileNameWithoutExtension(media.Properties.FilePath) + $"_failed-{DateTime.Now:yyyyMMdd_HHmmss}"));
        }

        public void Backup(string targetFolder)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(Temp, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(Temp, targetFolder));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(Temp, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(Temp, targetFolder), true);
        }

        private void ReadyTemp()
        {
            // Check content
            if (!Directory.Exists(Temp))
                Directory.CreateDirectory(Temp);
            else
                foreach (var files in Directory.GetFiles(Temp))
                    File.Delete(files);
        }

        private int Extract(Queue item)
        {
            int i = 0;
            foreach (var sub in item.Subtitle)
                if (sub.Id == -1)
                    if (File.Exists(sub.File))
                        File.Copy(sub.File, Path.Combine(Temp, $"subtitle{i++:D4}_{sub.Lang}.{sub.Format}"), true);
                else
                    new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -loglevel quiet -i \"{sub.File}\" -map 0:{sub.Id} -y subtitle{i++:D4}_{sub.Lang}.{sub.Format}");

            foreach (var fnt in item.Attachment)
                if (File.Exists(fnt.File))
                    File.Copy(fnt.File, Path.Combine(Temp, Path.GetFileName(fnt.File)), true);

            new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -loglevel quiet -dump_attachment:t \"\" -i \"{item.Properties.FilePath}\" -y");
            new TaskManager().RunCmd($"\"{MKVExtract}\"", $"chapters \"{item.Properties.FilePath}\" > chapters.xml");

            if (File.Exists(Path.Combine(Temp, "chapters.xml")))
            {
                FileInfo ChapLen = new FileInfo(Path.Combine(Temp, "chapters.xml"));
                if (ChapLen.Length < 256)
                    File.Delete(Path.Combine(Temp, "chapters.xml"));
            }

            return 0;
        }

        private int Indexing(Queue item)
        {
            foreach (var video in item.Video)
            {
                if (video.FrameRateVariable)
                {
                    new TaskManager().RunCmd($"\"{FFms}\"", $"-p -f -c \"{video.File}\" timecode");

                    int i = 0;
                    foreach (var tc in Directory.GetFiles(Temp, "timecod*"))
                        if (!Path.GetFileName(tc).Contains(".tc.txt"))
                            File.Delete(tc);
                        else
                            File.Move(tc, Path.Combine(Temp, $"timecode{i++:D4}.tc.txt"));
                }
            }                   

            return 0;
        }

        private int Video(Queue item)
        {
            int i = 0;
            foreach (var video in item.Video)
            {
                PluginVideo e;
                if (Plugin.Video.TryGetValue(video.Encoder, out e))
                {
                    // Get Frame count
                    var framecount = new FFmpeg().FrameCount(video.File, video.Id);

                    // FFmpeg
                    var ff_res = $"{video.Width}x{video.Height}";
                    var ff_fps = $"{video.FrameRate:N3}";
                    var ff_chr = $"yuv{video.Chroma}p{(video.BitDepth > 8 ? $"{video.BitDepth}le" : string.Empty)}";
                    var ff_dei = $"yadif={video.DeinterlaceMode}:{video.DeinterlaceField}:0";

                    // Encoder
                    var en_y4m = e.Args.Y4M;
                    var en_inf = e.Args.Input;
                    var en_ouf = $"{e.Args.Output} \"{Path.Combine(Temp, $"video{i++:D4}_{video.Lang}.{e.App.Ext}")}\"";
                    var en_pre = Get.ArgsBuilder(e.Args.Preset, video.EncoderPreset);
                    var en_tun = Get.ArgsBuilder(e.Args.Tune, video.EncoderTune);
                    var en_bit = Get.ArgsBuilder(e.Args.BitDepth, $"{video.BitDepth}");
                    var en_frc = Get.ArgsBuilder(e.Args.FrameCount, $"{framecount}");
                    var en_val = $"{e.Mode[video.EncoderMode].Arg} {video.EncoderModeValue}";

                    // Multipass
                    var mp_mode = e.Mode[video.EncoderMode].IsMultipass;
                    var mp_count = video.EncoderMultiPass;
                    var mp_first = e.Args.PassFirst;
                    var mp_last = e.Args.PassLast;
                    var mp_any = e.Args.PassAny;

                    // Run
                    var arg_ff = $"-hide_banner -loglevel panic -i \"{video.File}\" -strict -1 -s {ff_res} -r {ff_fps} -f yuv4mpegpipe -pix_fmt {ff_chr} {(video.Deinterlace ? $"-vf \"{ff_dei}\"" : string.Empty)} -";
                    var arg_en = $"{en_y4m} {en_inf} {en_ouf} {en_pre} {en_tun} {en_bit} {en_frc} {en_val} {video.EncoderArgs}";

                    // CLI
                    var DECODER = FFmpeg;
                    var ENCODER = e.App.Exe;

                    try
                    {
                        ENCODER = string.Format(ENCODER, $"{video.BitDepth:D2}");
                    }
                    catch (Exception)
                    {
                        
                    }

                    // Run
                    var r = 0;

                    if (mp_mode)
                    {
                        for (int k = 0; k < mp_count; k++)
                        {
                            if (k == 0)
                                r = new TaskManager().RunCmd($"\"{DECODER}\"", $"{arg_ff}", $"\"{ENCODER}\"", $"{arg_en} {mp_first}");
                            else if (k + 1 == mp_count)
                                r = new TaskManager().RunCmd($"\"{DECODER}\"", $"{arg_ff}", $"\"{ENCODER}\"", $"{arg_en} {mp_last}");
                            else
                                r = new TaskManager().RunCmd($"\"{DECODER}\"", $"{arg_ff}", $"\"{ENCODER}\"", $"{arg_en} {mp_any}");

                            if (r >= 1)
                                return r;
                        }
                    }
                    else
                    {
                        r = new TaskManager().RunCmd($"\"{DECODER}\"", $"{arg_ff}", $"\"{ENCODER}\"", $"{arg_en}");

                        if (r >= 1)
                            return r;
                    }
                }
            }

            return 0;
        }

        private int Audio(Queue item)
        {
            int i = 0;
            foreach (var audio in item.Audio)
            {
                if (Equals(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), audio.Encoder))
                {
                    if (item.MkvOut)
                    {
                        if (string.Equals("wma", audio.Format, IC))
                        {
                            new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -i \"{audio.File}\" -map 0:{audio.Id} -dn -vn -sn -strict -2 -c:a aac -y audio{i++:D4}_{audio.Lang}.m4a");
                        }
                        else
                        {
                            new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -i \"{audio.File}\" -map 0:{audio.Id} -dn -vn -sn -acodec copy -y audio{i++:D4}_{audio.Lang}.{audio.Format}");
                        }
                    }
                    else
                    {
                        if (string.Equals("mp4", audio.Format, IC))
                        {
                            new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -i \"{audio.File}\" -map 0:{audio.Id} -dn -vn -sn -acodec copy -y audio{i++:D4}_{audio.Lang}.{audio.Format}");
                        }
                        else
                        {
                            new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -i \"{audio.File}\" -map 0:{audio.Id} -dn -vn -sn -strict -2 -c:a aac -y audio{i++:D4}_{audio.Lang}.m4a");
                        }
                    }
                }
                else
                {
                    PluginAudio e;
                    if (Plugin.Audio.TryGetValue(audio.Encoder, out e))
                    {
                        int m = audio.EncoderMode;
                        new TaskManager().RunCmd($"\"{FFmpeg}\"", $"-hide_banner -loglevel panic -i \"{audio.File}\" -map 0:{audio.Id} -acodec pcm_s{audio.BitDepth}le -ar {audio.EncoderSampleRate} -ac {audio.EncoderChannel} -f wav -", $"\"{e.App.Exe}\"", $"{e.Args.Input} {e.Mode[m].Arg} {audio.EncoderValue} {e.Args.Advance} {e.Args.Output} audio{i++:D4}_{audio.Lang}.{e.App.Ext}");
                    }
                }
            }

            return 0;
        }

        private int Muxing(Queue item)
        {
            string savePath = Path.GetDirectoryName(item.Properties.FilePath);
            string saveName = Path.GetFileNameWithoutExtension(item.Properties.FilePath);
            string timeStamp = $"{DateTime.Now:yyyyMMdd_HHmmss}";

            if (Properties.Settings.Default.SaveFolderThis)
                savePath = Properties.Settings.Default.SaveFolder;
            
            if (item.MkvOut)
            {
                File.WriteAllText(Path.Combine(Temp, "tags.xml"), string.Format(Properties.Resources.Tags, "Internet Friendly Media Encoder", "Nemu 7"));

                string fileout = Path.Combine(savePath, $"{saveName}_encoded-{timeStamp}.mkv");

                string cmdvideo = string.Empty;
                string cmdaudio = string.Empty;
                string cmdsubs = string.Empty;
                string cmdattach = string.Empty;
                string cmdchapter = string.Empty;

                foreach (var tc in Directory.GetFiles(Temp, "timecode*"))
                {
                    cmdvideo += $"--timecodes 0:\"{tc}\" "; break;
                }

                foreach (var video in Directory.GetFiles(Temp, "video*"))
                {
                    cmdvideo += $"--language 0:{Get.FileLang(video)} \"{video}\" ";
                }

                foreach (var audio in Directory.GetFiles(Temp, "audio*"))
                {
                    cmdaudio += $"--language 0:{Get.FileLang(audio)} \"{audio}\" ";
                }

                foreach (var subs in Directory.GetFiles(Temp, "subtitle*"))
                {
                    cmdsubs += $"--sub-charset 0:UTF-8 --language 0:{Get.FileLang(subs)} \"{subs}\" ";
                }

                foreach (var attach in Directory.GetFiles(Temp, "*.*")
                    .Where(s =>
                   s.EndsWith(".ttf", IC) ||
                   s.EndsWith(".otf", IC) ||
                   s.EndsWith(".ttc", IC) ||
                   s.EndsWith(".woff", IC)))
                {
                    cmdattach += $"--attach-file \"{attach}\" ";
                }

                if (File.Exists(Path.Combine(Temp, "chapters.xml")))
                {
                    cmdchapter = $"--chapters \"{Path.Combine(Temp, "chapters.xml")}\"";
                }

                return new TaskManager().RunCmd($"\"{MKVMerge}\"", $"-o \"{fileout}\" --disable-track-statistics-tags -t 0:tags.xml {cmdvideo} {cmdaudio} {cmdsubs} {cmdattach} {cmdchapter}");
            }
            else
            {
                string fileout = Path.Combine(savePath, $"{saveName}_encoded-{timeStamp}.mp4");

                string timecode = string.Empty;
                string cmdvideo = string.Empty;
                string cmdaudio = string.Empty;

                foreach (var tc in Directory.GetFiles(Temp, "timecode*"))
                {
                    timecode = tc; break;
                }

                int cntv = 0;
                foreach (var video in Directory.GetFiles(Temp, "video*"))
                {
                    cmdvideo += $"-add \"{video}#video:name=Video {++cntv}:lang={Get.FileLang(video)}:fmt=HEVC\" ";
                }

                int cnta = 0;
                foreach (var audio in Directory.GetFiles(Temp, "audio*"))
                {
                    cmdaudio += $"-add \"{audio}#audio:name=Audio {++cnta}:lang={Get.FileLang(audio)}\" ";
                }

                if (string.IsNullOrEmpty(timecode))
                {
                    return new TaskManager().RunCmd($"\"{MP4Box}\"", $"{cmdvideo} {cmdaudio} -itags tool=\"IFME\" -new \"{fileout}\"");
                }
                else
                {
                    new TaskManager().RunCmd($"\"{MP4Box}\"", $"{cmdvideo} {cmdaudio} -itags tool=\"IFME\" -new _desu.mp4");
                    new TaskManager().RunCmd($"\"{MP4Fps}\"", $"-t \"{timecode}\" _desu.mp4 -o \"{fileout}\"");
                }
            }

            return 0;
        }
    }
}
