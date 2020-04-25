using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IFME.FFmpeg
{
    public class MediaInfo
    {
        private static StringComparison IgnoreCase { get { return StringComparison.InvariantCultureIgnoreCase; } }

        public static string FFmpegProbe { get; set; } = Path.Combine("Plugins", "ffmpeg64", "ffprobe");

        public string FilePath { get; internal set; } = string.Empty;
        public ulong FileSize { get; internal set; } = 0;
        public ulong BitRate { get; internal set; } = 0;
        public float Duration { get; internal set; } = 0;
        public string FormatName { get; internal set; } = "NEW";
        public string FormatNameFull { get; internal set; } = "Blank media";

        public List<StreamVideo> Video { get; internal set; } = new List<StreamVideo>();
        public List<StreamAudio> Audio { get; internal set; } = new List<StreamAudio>();
        public List<StreamSubtitle> Subtitle { get; internal set; } = new List<StreamSubtitle>();
        public List<StreamAttachment> Attachment { get; internal set; } = new List<StreamAttachment>();

        public MediaInfo(string path)
        {
            dynamic json = JsonConvert.DeserializeObject(new ReadFile().Media(path));

            // General info
            FilePath = json.format.filename;
            FileSize = json.format.size;
            BitRate = json.format.bit_rate;
            Duration = json.format.duration;
            FormatName = json.format.format_name;
            FormatNameFull = json.format.format_long_name;

            // Capture stream type
            foreach (var stream in json.streams)
            {
                string type = stream.codec_type;

                if (string.Equals(type, "video", IgnoreCase))
                {
                    string r = stream.r_frame_rate;
                    float.TryParse(r.Split('/')[0], out float rn);
                    float.TryParse(r.Split('/')[1], out float rd);
                    float rfps = rn / rd;

                    string a = stream.avg_frame_rate;
                    float.TryParse(a.Split('/')[0], out float an);
                    float.TryParse(a.Split('/')[1], out float ad);
                    float afps = an / ad;

                    int pix = 420;
                    if (!string.IsNullOrEmpty((string)stream.pix_fmt))
                    {
                        var mpix = Regex.Match((string)stream.pix_fmt, @"yuv(\d+)");

                        if (mpix.Success)
                            int.TryParse(mpix.Groups[1].Value, out pix);
                        else
                            pix = 420;
                    }

                    int bpc;
                    if (int.TryParse((string)stream.bits_per_raw_sample, out int x))
                    {
                        bpc = x;
                    }
                    else
                    {
                        var mbpc = Regex.Match((string)stream.pix_fmt, @"yuv\d+p(\d+)");

                        if (mbpc.Success)
                            int.TryParse(mbpc.Groups[1].Value, out bpc);
                        else
                            bpc = 8;
                    }

                    string lang = stream.tags.language;
                    if (string.IsNullOrEmpty(lang)) lang = "und";

                    Video.Add(new StreamVideo
                    {
                        Id = stream.index,
                        Language = lang,
                        Codec = stream.codec_name,
                        Chroma = pix,
                        BitDepth = bpc,
                        Width = stream.width,
                        Height = stream.height,
                        FrameRateConstant = rfps == afps,
                        FrameRate = rfps,
                        FrameRateAvg = afps,
                        FrameCount = (int)(Duration * afps),
                        Duration = Duration,
                    });
                }

                if (string.Equals(type, "audio", IgnoreCase))
                {
                    int.TryParse((string)stream.sample_rate, out int sample);
                    int.TryParse((string)stream.sample_fmt, out int bitdepth);
                    int.TryParse((string)stream.channels, out int channel);

                    if (bitdepth == 0) bitdepth = 16;
                    else if (bitdepth >= 32) bitdepth = 24;

                    string lang = stream.tags.language;
                    if (string.IsNullOrEmpty(lang)) lang = "und";

                    Audio.Add(new StreamAudio
                    {
                        Id = stream.index,
                        Language = lang,
                        Codec = stream.codec_name,
                        SampleRate = sample,
                        BitDepth = bitdepth,
                        Channel = channel,
                        Duration = Duration,
                    });
                }

                if (string.Equals(type, "subtitle", IgnoreCase))
                {
                    string lang = stream.tags.language;
                    if (string.IsNullOrEmpty(lang)) lang = "und";

                    Subtitle.Add(new StreamSubtitle
                    {
                        Id = stream.index,
                        Language = lang,
                        Codec = stream.codec_name,
                    });
                }

                if (string.Equals(type, "attachment", IgnoreCase))
                {
                    Attachment.Add(new StreamAttachment
                    {
                        Id = stream.index,
                        FileName = stream.tags.filename,
                        MimeType = stream.tags.mimetype
                    });
                }
            }
        }
    }
}
