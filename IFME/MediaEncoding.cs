using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using IFME.OSManager;

namespace IFME
{
    internal class MediaEncoding
    {
        private static int Arch = OS.Is64bit ? 64 : 32;
        internal static string FFmpeg = Path.Combine(Environment.CurrentDirectory, "Plugins", $"ffmpeg{Arch}", "ffmpeg");
        internal static string MP4Box = Path.Combine(Environment.CurrentDirectory, "Plugins", "mp4box", "mp4box");
        internal static int CurrentIndex = 0;
        internal static int RealFrameCount = 0;

        private static bool IsExitError(int ExitCode) => ExitCode <= -1 || ExitCode == 1;

        private static string FileContainerCompact(MediaContainer Cont)
        {
            switch (Cont)
            {
                case MediaContainer.AVI:
                    return "avi";
                case MediaContainer.MP4:
                    return "mov";
                case MediaContainer.MKV:
                    return "mp4";
                case MediaContainer.WEBM:
                    return "webm";
                case MediaContainer.TS:
                    return "m4v";
                case MediaContainer.M2TS:
                    return "m4v";
                case MediaContainer.MP2:
                    return "mp2";
                case MediaContainer.MP3:
                    return "mp3";
                case MediaContainer.M4A:
                    return "m4a";
                case MediaContainer.OGG:
                    return "ogg";
                case MediaContainer.OPUS:
                    return "opus";
                case MediaContainer.FLAC:
                    return "flac";
                default:
                    return "mkv";
            }
        }

        internal static void Extract(MediaQueue queue, string tempDir)
        {
            // Dump Metadata
            frmMain.PrintStatus("Extracting...");

            frmMain.PrintLog("[INFO] Extracting metadata...");
            ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{queue.FilePath}\" -f ffmetadata metadata.ini -y");

            if (queue.Video.Count == 0)
                return;

            if (queue.Subtitle.Count == 0 && queue.Attachment.Count == 0)
                return;

            frmMain.PrintLog("[INFO] Extracting subtitle file...");

            for (int i = 0; i < queue.Subtitle.Count; i++)
            {
                var id = queue.Subtitle[i].Id;
                var fmt = queue.Subtitle[i].Codec;
                var file = queue.Subtitle[i].File;
                var lang = queue.Subtitle[i].Lang;
                var fext = Path.GetExtension(file);

                if (id >= 0)
                {
                    ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{file}\" -map 0:{id} -map_metadata -1 -map_chapters -1 -vn -an -dn -scodec copy -y subtitle0000_{i:D4}_{lang}.{fmt}");
                }
                else if (id == -1)
                {
                    File.Copy(file, Path.Combine(tempDir, $"subtitle0000_{i:D4}_{lang}{fext}"));
                }
                else if (id == -2)
                {
                    var embed = new FFmpeg.MediaInfo(file);

                    for (int e = 0; e < embed.Subtitle.Count; e++)
                    {
                        var e_id = embed.Subtitle[e].Id;
                        var e_fmt = embed.Subtitle[e].Codec;
                        var e_lang = embed.Subtitle[e].Language;

                        ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{file}\" -map 0:{e_id} -map_metadata -1 -map_chapters -1 -vn -an -dn -scodec copy -y subtitle{i:D4}_{e:D4}_{e_lang}.{e_fmt}");
                    }
                }
            }

            frmMain.PrintLog("[INFO] Extracting embeded attachment...");
            var tempDirFont = Path.Combine(tempDir, "attachment");
            for (int i = 0; i < queue.Attachment.Count; i++)
            {
                var id = queue.Attachment[i].Id;
                var file = queue.Attachment[i].File;
                var name = queue.Attachment[i].Name;

                if (!Directory.Exists(tempDirFont))
                    Directory.CreateDirectory(tempDirFont);

                if (id > 0)
                {
                    ProcessManager.Start(tempDirFont, $"\"{FFmpeg}\" -hide_banner -v panic -stats -dump_attachment:{id} \"{name}\" -i \"{file}\" -y");
                }
                else if (id == -1)
                {
                    File.Copy(file, Path.Combine(tempDirFont, Path.GetFileName(file)));
                }
                else if (id == -2)
                {
                    var embed = new FFmpeg.MediaInfo(file);

                    for (int e = 0; i < embed.Attachment.Count; i++)
                    {
                        var e_id = embed.Attachment[e].Id;
                        var e_name = embed.Attachment[e].FileName;

                        ProcessManager.Start(tempDirFont, $"\"{FFmpeg}\" -hide_banner -v panic -stats -dump_attachment:{e_id} \"{e_name}\" -i \"{file}\" -y");
                    }
                }
            }

            // Hard Sub
            if (queue.HardSub)
            {
                var fontConfigFile = File.ReadAllText(Path.Combine("Fonts", "fonts.conf"));
                var fontConfigData = string.Format(fontConfigFile, tempDirFont);
                File.WriteAllText(Path.Combine(tempDir, "fonts.conf"), fontConfigData);

                Environment.SetEnvironmentVariable("FC_CONFIG_DIR", tempDirFont);
                Environment.SetEnvironmentVariable("FONTCONFIG_PATH", tempDir);
                Environment.SetEnvironmentVariable("FONTCONFIG_FILE", Path.Combine(tempDir, "fonts.conf"));
            }
        }

        internal static void Audio(MediaQueue queue, string tempDir)
        {
            for (int i = 0; i < queue.Audio.Count; i++)
            {
                var item = queue.Audio[i];

                frmMain.PrintStatus($"Encoding, Audio #{i}");

                if (Plugins.Items.Audio.TryGetValue(item.Encoder.Id, out PluginsAudio codec))
                {
                    frmMain.PrintLog("[INFO] Encoding audio file...");

                    var ac = codec.Audio;
                    var md = item.Encoder.Mode;
                    var en = ac.Encoder;

                    var trim = queue.Trim.Enable ? $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}" : string.Empty;

                    var qu = $"{ac.Mode[md].Args} {ac.Mode[md].QualityPrefix}{item.Encoder.Quality}{ac.Mode[md].QualityPostfix}";
                    var hz = item.Encoder.SampleRate == 0 ? string.Empty : $"-ar {item.Encoder.SampleRate}";
                    var ch = item.Encoder.Channel == 0 ? string.Empty : $"-ac {item.Encoder.Channel}";

                    var outfmtfile = $"audio{i:D4}_{item.Lang}.{ac.Extension}";

                    var af = string.Empty;

                    if (!ac.Mode[md].MultiChannelSupport) // Mode didn't support MultiChannel, for example eSBR on exhale. Down-mixing to stereo
                    {
                        frmMain.PrintLog($"[WARN] {codec.Name}, {codec.Audio.Mode[md].Name} doesn't support Multi Channel...");

                        if (item.Info.Channel >= 2)
                            ch = $"-ac 2";
                    }

                    if (!ac.Mode[md].MonoSupport) // Some audio encode mode doesn't support Mono, so, need to up-mixing to stereo
                    {
                        frmMain.PrintLog($"[WARN] {codec.Name}, {codec.Audio.Mode[md].Name} doesn't support Mono Channel...");

                        if (item.Info.Channel == 1)
                            ch = $"-ac 2";
                    }

                    if(queue.FastMuxAudio && !queue.Trim.Enable)
                    {
                        frmMain.PrintStatus($"Fast Remux, Audio #{i}");
                        frmMain.PrintLog($"[INFO] Fast Remuxing Audio...");

                        var tempName = $"audio{i:D4}_{item.Lang}.{FileContainerCompact(queue.OutputFormat)}";

                        var exitCode = ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -c:a copy -y {tempName}");

                        if (new FileInfo(Path.Combine(tempDir, tempName)).Length <= 1024)
                            exitCode = 1;

                        if (!IsExitError(exitCode))
                            continue;

                        frmMain.PrintLog($"[INFO] Remuxing incompatible codec require to re-encode to compatible one... Exit Code {exitCode}");
                        File.Delete(Path.Combine(tempDir, tempName));
                    }

                    if (!item.CommandFilter.IsDisable())
                    {
                        af = $"-af {item.CommandFilter}";
                    }

                    if (ac.Args.Pipe)
                    {
                        ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} {af} -f wav {item.Command} - | \"{en}\" {ac.Args.Input} {ac.Args.Command} {qu} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
                    }
                    else
                    {
                        ProcessManager.Start(tempDir, $"\"{en}\" {ac.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} {ac.Args.Command} {ac.Args.Codec} {qu} {hz} {ch} {af} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
                    }
                }
            }
        }

        internal static void Video(MediaQueue queue, string tempDir)
        {
            for (int i = 0; i < queue.Video.Count; i++)
            {
                var item = queue.Video[i];

                if (item.Info.Disposition_AttachedPic)
                {
                    frmMain.PrintStatus($"Extracting Thumbnail #{i}");
                    frmMain.PrintLog($"[INFO] Extracting Thumbnail...");

                    var exts = item.Codec;

                    if (exts.Equals("mjpeg", StringComparison.CurrentCultureIgnoreCase))
                        exts = "jpeg";

                    ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -vcodec copy -y \"album_art{i:D4}.{exts}\"");

                    return;
                }

                if (item.Encoder.Id.Equals(new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff")))
                    continue; // skip encode video when choosing audio only

                if (Plugins.Items.Video.TryGetValue(item.Encoder.Id, out PluginsVideo codec))
                {
                    var vc = codec.Video;

                    var en = vc.Encoder.Find(b => b.BitDepth == item.Quality.BitDepth).Binary;

                    var ff = string.Equals(Path.GetFileNameWithoutExtension(en).ToLowerInvariant(), "ffmpeg");

                    var mode = item.Encoder.Mode;

                    var outrawfile = $"raw-v{i:D4}_{item.Lang}.{vc.Extension}";
                    var outfmtfile = $"video{i:D4}_{item.Lang}.{codec.Format[0]}";
                    var outencfile = vc.RawOutput ? outrawfile : outfmtfile;

                    var val_w = item.Quality.Width;
                    var val_h = item.Quality.Height;
                    var val_fps = item.Quality.FrameRate >= 5 ? item.Quality.FrameRate : 23.976;
                    var val_bpc = item.Quality.BitDepth >= 8 ? item.Quality.BitDepth : 8;
                    var val_csp = item.Quality.PixelFormat >= 420 ? item.Quality.PixelFormat : 420;

                    var ff_rawcodec = string.Empty;
                    var ff_infps = string.Empty;
                    var ff_trim = string.Empty;
                    var ff_res = string.Empty;
                    var ff_yuv = string.Empty;
                    var ff_vf = new List<string>();
                    
                    var en_res = string.Empty;
                    var en_fps = string.Empty;
                    var en_bit = string.Empty;
                    var en_csp = string.Empty;
                    var en_preset = string.Empty;
                    var en_tune = string.Empty;
                    var en_mode = string.Empty;
                    var en_framecount = string.Empty;

                    var yuv_bit_enc = val_bpc > 8 ? $"{val_bpc}le" : string.Empty;

                    // FFmpeg RAW Type
                    if (string.IsNullOrEmpty(vc.Args.Y4M))
                        ff_rawcodec = "-strict -1 -f rawvideo";
                    else
                        ff_rawcodec = "-strict -1 -f yuv4mpegpipe";

                    // FFmpeg Frame Rate (Input) for Image Sequence
                    ff_infps = item.IsImageSeq ? $"-framerate {item.Info.FrameRate}" : string.Empty;

                    // FFmpeg Resolution
                    if (item.Quality.Width >= 128 || item.Quality.Height >= 128)
                        ff_vf.Add($"scale={val_w}:{val_h}:flags=lanczos");

                    // FFmpeg Frame Rate (force encode to target frame rate, become constant fps)
                    ff_vf.Add($"fps={item.Quality.FrameRate}");

                    // FFmpeg Pixel Format
                    ff_yuv = $"-pix_fmt yuv{item.Quality.PixelFormat}p{yuv_bit_enc}";

                    // FFmpeg Trim
                    if (queue.Trim.Enable)
                        ff_trim += $"-ss {queue.Trim.Start} -t {queue.Trim.Duration}";

                    // FFmpeg Video Filter
                    if (item.DeInterlace.Enable)
                        ff_vf.Add($"yadif={item.DeInterlace.Mode}:{item.DeInterlace.Field}:0");
                    
                    // Fmpeg Video Filter (extra)
                    if (!item.Quality.CommandFilter.IsDisable())
                        ff_vf.Add(item.Quality.CommandFilter);
                    
                    if (queue.HardSub)
                    {
                        var files = Directory.GetFiles(tempDir, "subtitle*");

                        if (files.Length > 0)
                        {
                            var file = Path.GetFileName(files[0]);
                            var ext = Path.GetExtension(file).ToLowerInvariant();

                            if (ext.IsOneOf(".srt"))
                            {
                                ff_vf.Add($"subtitles={file}");
                            }
                            else if (ext.IsOneOf(".ass", ".ssa"))
                            {
                                ff_vf.Add($"subtitles=f={file}:fontsdir=attachment");
                            }
                        }
                    }

                    //
                    // Encoder Resolution
                    if (!string.IsNullOrEmpty(vc.Args.Resolution))
                        en_res = string.Format(vc.Args.Resolution, val_w, val_h);

                    // Encoder Frame Rate
                    if (!string.IsNullOrEmpty(vc.Args.FrameRate))
                        en_fps = $"{vc.Args.FrameRate} {item.Quality.FrameRate}";

                    // Encoder BitDepth Input
                    if (!string.IsNullOrEmpty(vc.Args.BitDepthIn))
                        en_bit = $"{vc.Args.BitDepthIn} {item.Quality.BitDepth}";

                    // Encoder BitDepth Output
                    if (!string.IsNullOrEmpty(vc.Args.BitDepthOut))
                        en_bit += $" {vc.Args.BitDepthOut} {item.Quality.BitDepth}";

                    // Encoder Pixel Format/ColorSpace Format (csp)
                    foreach (var c in vc.Chroma)
                    {
                        if (c.Value == val_csp)
                            if (!string.IsNullOrEmpty(c.Command))
                                en_csp = c.Command;
                    }

                    // Encoder Pixel Format BitDepth (only to YUV)
                    if (en_csp.Contains("yuv"))
                        en_csp += yuv_bit_enc;

                    // Encoder Preset
                    en_preset = ArgsParser.Parse(vc.Args.Preset, item.Encoder.Preset);

                    // Encoder Tune
                    en_tune = ArgsParser.Parse(vc.Args.Tune, item.Encoder.Tune);

                    // Encoder Mode
                    if (!string.IsNullOrEmpty(vc.Mode[mode].Args))
                    {
                        if (vc.Mode[mode].Args.HasFormatSpecifiers())
                            en_mode = string.Format(vc.Mode[mode].Args, item.Encoder.Value);
                        else
                            en_mode = $"{vc.Mode[mode].Args} {vc.Mode[mode].Prefix}{item.Encoder.Value}{vc.Mode[mode].Postfix}";
                    }

                    // Encoder Mode (Native)

                    // Encoder Frame Count
                    if (!string.IsNullOrEmpty(vc.Args.FrameCount))
                    {
                        var numFrames = (int)(queue.Duration * item.Quality.FrameRate);

                        if (item.Quality.FrameCount > 0)
                            numFrames = item.Quality.FrameCount;

                        if (queue.Trim.Enable)
                            numFrames = (int)(TimeSpan.Parse(queue.Trim.Duration).TotalMilliseconds / (1000 / item.Quality.FrameRate));

                        en_framecount = $"{vc.Args.FrameCount} {numFrames}";
                    }

                    // Copy Streams
                    if (codec.GUID.Equals(new Guid("00000000-0000-0000-0000-000000000000")))
                    {
                        frmMain.PrintStatus($"Copying, Video #{i}");
                        frmMain.PrintLog($"[INFO] Copying video stream...");

                        ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error {vc.Args.Input} \"{item.File}\" {vc.Args.UnPipe} {vc.Args.Output} {outencfile}");
                        continue;
                    }

                    // MP4 Remux Test
                    if (queue.FastMuxVideo && !queue.Trim.Enable)
                    {
                        frmMain.PrintStatus($"Fast Remux, Video #{i}");
                        frmMain.PrintLog($"[INFO] Fast Remuxing Video...");

                        var tempName = $"video{i:D4}_{item.Lang}.{FileContainerCompact(queue.OutputFormat)}";

                        var exitCode = ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" -map 0:{item.Id} -c:v copy -y {tempName}");

                        if (new FileInfo(Path.Combine(tempDir, tempName)).Length <= 8192)
                        {
                            frmMain.PrintLog($"[ERR ] Fast Remux is complete but remuxed file is corrupted... Exit Code {exitCode}");
                            exitCode = 1;
                        }

                        if (!IsExitError(exitCode))
                        {
                            frmMain.PrintLog($"[ OK ] Fast Remux is complete... Exit Code {exitCode}");
                            continue;
                        }

                        frmMain.PrintLog($"[WARN] Remuxing incompatible codec require to re-encode to compatible one... Exit Code {exitCode}");
                        File.Delete(Path.Combine(tempDir, tempName));
                    }

                    // Auto detect crop
                    if (queue.Crop.Enable)
                    {
                        var exitCode = ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -ss \"{queue.Crop.Start}\" -t \"{queue.Crop.Duration}\" -i \"{item.File}\" -vf cropdetect -f null - 2> crop.log");

                        if (File.Exists(Path.Combine(tempDir, "crop.log")))
                        {
                            var crop = IFME.FFmpeg.AutoDetect.Crop.Get(Path.Combine(tempDir, "crop.log"));

                            if (!string.IsNullOrEmpty(crop))
                            {
                                int oldVideoRes = ff_vf.FindIndex(rs => rs.StartsWith("scale="));

                                if (oldVideoRes != -1)
                                {
                                    ff_vf.RemoveAt(oldVideoRes);
                                }

                                int oldCropIndex = ff_vf.FindIndex(vf => vf.StartsWith("crop="));

                                if (oldCropIndex != -1)
                                {
                                    ff_vf[oldCropIndex] = $"crop={crop}";
                                }
                                else
                                {
                                    ff_vf.Add($"crop={crop}");
                                }
                            }
                        }
                    }

                    // Begin encoding
                    frmMain.PrintStatus($"Encoding, Video #{i}");
                    frmMain.PrintLog($"[INFO] Encoding video file...");

                    var cmd_ff = $"-map 0:{item.Id} {ff_trim} {ff_yuv} -vf {string.Join(",", ff_vf)}";

                    var cmd_en = $"{en_res} {en_fps} {en_bit} {en_csp}";

                    var cmd_ff_en = ff ? cmd_ff : cmd_en;

                    RealFrameCount = item.Quality.FrameCount;

                    if (vc.Mode[item.Encoder.Mode].MultiPass)
                    {
                        var p = 1;
                        var pass = string.Empty;

                        frmMain.PrintLog("[WARN] Frame count is disable for Multi-pass encoding, Avoid inconsistent across multi-pass.");

                        do
                        {
                            pass = vc.Args.PassNth;

                            if (p == 1)
                                pass = vc.Args.PassFirst;

                            if (p == item.Encoder.MultiPass)
                                pass = vc.Args.PassLast;

                            frmMain.PrintLog($"[INFO] Multi-pass encoding: {p} of {item.Encoder.MultiPass}");

                            if (vc.Args.Pipe)
                                ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error {ff_infps} -i \"{item.File}\" {cmd_ff} {ff_rawcodec} {item.Quality.Command} - | \"{en}\" {vc.Args.Y4M} {vc.Args.Input} {cmd_en} {en_preset} {en_tune} {en_mode} {vc.Args.Command} {item.Encoder.Command} {pass} {vc.Args.Output} {outencfile}");
                            else
                                ProcessManager.Start(tempDir, $"\"{en}\" {ff_infps} {vc.Args.Input} \"{(string.IsNullOrEmpty(vc.Args.Y4M) ? item.File : vc.Args.Y4M)}\" {cmd_ff_en} {en_mode} {vc.Args.UnPipe} {item.Encoder.Command} {vc.Args.Command} {pass} {vc.Args.Output} {outencfile}");

                            if (p == 1)
                                en_framecount = $"{vc.Args.FrameCount} {RealFrameCount}";

                            ++p;

                            Thread.Sleep(1500);

                        } while (p <= item.Encoder.MultiPass);
                    }
                    else
                    {
                        if (vc.Args.Pipe)
                            ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error {ff_infps} -i \"{item.File}\" {cmd_ff} {ff_rawcodec} {item.Quality.Command} - | \"{en}\" {vc.Args.Y4M} {vc.Args.Input} {en_framecount} {cmd_en} {en_preset} {en_tune} {en_mode} {vc.Args.Command} {item.Encoder.Command} {vc.Args.Output} {outencfile}");
                        else
                            ProcessManager.Start(tempDir, $"\"{en}\" {ff_infps} {vc.Args.Input} \"{(string.IsNullOrEmpty(vc.Args.Y4M) ? item.File : vc.Args.Y4M)}\" {cmd_ff_en} {en_mode} {vc.Args.UnPipe} {item.Encoder.Command} {vc.Args.Command} {vc.Args.Output} {outencfile}");

                        Thread.Sleep(1500);
                    }

                    // Raw file dont have pts (time), need to remux
                    frmMain.PrintStatus($"Restructure...");
                    frmMain.PrintLog($"[INFO] Restructure RAW video file...");

                    if (vc.RawOutput)
                    {
                        if (File.Exists(Path.Combine(tempDir, outrawfile)))
                        {
                            ProcessManager.Start(tempDir, $"\"{MP4Box}\" -add \"{outrawfile}#video:name=\" -itags tool=\"IFME MP4\" \"{outfmtfile}\" ");
                            File.Delete(Path.Combine(tempDir, outrawfile));
                        }
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(tempDir, outrawfile)))
                        {
                            File.Move(Path.Combine(tempDir, outrawfile), Path.Combine(tempDir, outfmtfile));
                        }
                    }
                }
            }
        }

        internal static int Muxing(MediaQueue queue, string tempDir, string saveDir, string saveFile)
        {
            var x = 0;
            var metadata = string.Empty;
            var metafile = string.Empty;
            var map = string.Empty;

            var argVideo = string.Empty;
            var argAudio = string.Empty;
            var argSubtitle = string.Empty;
            var argEmbed = string.Empty;
            var argArt = string.Empty;

            var outFile = Path.Combine(saveDir, saveFile);

            frmMain.PrintStatus("Repacking...");
            frmMain.PrintLog($"[INFO] Multiplexing encoded files into single file...");

            Thread.Sleep(1500); // Wait NTFS finish updating the content

            foreach (var video in Directory.GetFiles(tempDir, "video*"))
            {
                argVideo += $"-i \"{Path.GetFileName(video)}\" ";
                metadata += $"-metadata:s:{x} title=\"{Language.FromFileNameFull(video)}\" -metadata:s:{x} language={Language.FromFileNameCode(video)}  ";
                map += $" -map {x}:0";
                x++;
            }

            foreach (var audio in Directory.GetFiles(tempDir, "audio*"))
            {
                argAudio += $"-i \"{Path.GetFileName(audio)}\" ";
                metadata += $"-metadata:s:{x} title=\"{Language.FromFileNameFull(audio)}\" -metadata:s:{x} language={Language.FromFileNameCode(audio)} ";
                map += $" -map {x}:0";
                x++;
            }

            if (queue.OutputFormat == MediaContainer.MKV || queue.OutputFormat == MediaContainer.MP4)
            {
                if (!queue.HardSub)
                {
                    var d = 0;
                    foreach (var subtitle in Directory.GetFiles(tempDir, "subtitle*"))
                    {
                        argSubtitle += $"-i \"{Path.GetFileName(subtitle)}\" ";
                        metadata += $"-metadata:s:{x} title=\"{Language.FromFileNameFull(subtitle)}\" -metadata:s:{x} language={Language.FromFileNameCode(subtitle)} {(d == 0 ? $"-disposition:s:{d} default " : "")}";
                        map += $" -map {x}:0";
                        x++;
                        d++;

                        if (queue.OutputFormat == MediaContainer.MP4)
                                metadata += $" -c:s mov_text ";
                    }

                    var tempDirFont = Path.Combine(tempDir, "attachment");
                    if (Directory.Exists(tempDirFont))
                    {
                        var files = Directory.GetFiles(tempDirFont, "*");
                        for (int i = 0; i < files.Length; i++)
                        {
                            argEmbed += $"-attach \"{Path.Combine("attachment", Path.GetFileName(files[i]))}\" ";
                            metadata += $"-metadata:s:{x} filename=\"{Path.GetFileName(files[i])}\" -metadata:s:{x} \"mimetype={queue.Attachment[i].Mime}\" ";
                            x++;
                        }
                    }
                }
            }


            if (queue.OutputFormat == MediaContainer.MKV || 
                queue.OutputFormat == MediaContainer.MP4 ||
                queue.OutputFormat == MediaContainer.M4A ||
                queue.OutputFormat == MediaContainer.MP3 ||
                queue.OutputFormat == MediaContainer.FLAC)
            {
                foreach (var art in Directory.GetFiles(tempDir, "album_art*"))
                {
                    argArt = $"-i \"{Path.GetFileName(art)}\" ";
                    metadata += $"-disposition:{x} attached_pic";
                    map += $" -map {x}:0";
                    x++;
                    break;
                }
            }

            if (File.Exists(Path.Combine(tempDir, "metadata.ini")))
            {
                metafile = $"-f ffmetadata -i metadata.ini -map_metadata {x}";
            }

            var author = $"{Version.Name} {Version.Release} {Version.OSPlatform} {Version.OSArch}";
            var command = $"\"{FFmpeg}\" -strict -2 -hide_banner -v error -stats {argVideo}{argAudio}{argSubtitle}{argArt}{metafile}{map} -c copy -metadata:g \"encoding_tool={author}\" {argEmbed}{metadata} -y \"{outFile}\"";
            return ProcessManager.Start(tempDir, command);
        }
    }
}
