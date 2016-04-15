using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class Queue 
    {
        public bool Enable = true;
        public bool MkvOut = true;
        
        public List<QueueVideo> Video = new List<QueueVideo>();
        public List<QueueAudio> Audio = new List<QueueAudio>();
        public List<QueueSubtitle> Subtitle = new List<QueueSubtitle>();
        public List<QueueAttachment> Attachment = new List<QueueAttachment>();

        public FFmpegDotNet.FFmpeg.Stream Properties; // use for holding original data

        // Media Info Generator
        public static string Info(Queue info)
        {
            string source = string.Empty;

            if (info.Properties.Video.Count > 0)
            {
                source = "Video:\r\n";
                foreach (var item in info.Properties.Video)
                {
                    source += $"ID {item.Id:D2}, {item.Language}, {item.Codec}, {item.Width}x{item.Height} @ {item.FrameRate:N3} fps, (YUV{item.Chroma} @ {item.BitDepth} bpc)\r\n";
                }
            }

            if (info.Properties.Audio.Count > 0)
            {
                source += $"\r\nAudio:\r\n";
                foreach (var item in info.Properties.Audio)
                {
                    source += $"ID {item.Id:D2}, {item.Language}, {item.Codec}, {item.SampleRate} Hz @ {item.BitDepth} bit, {item.Channel} channel\r\n";
                }
            }

            if (info.Properties.Subtitle.Count > 0)
            {
                source += $"\r\nSubtitle:\r\n";
                foreach (var item in info.Properties.Subtitle)
                {
                    source += $"ID {item.Id:D2}, {item.Language}, {item.Codec}\r\n";
                }
            }

            if (info.Properties.Attachment.Count > 0)
            {
                source += $"\r\nAttachment:\r\n";
                foreach (var item in info.Properties.Attachment)
                {
                    source += $"ID {item.Id:D2}, {item.MimeType}, {item.FileName}\r\n";
                }
            }

            return source;
        }
    }
}
