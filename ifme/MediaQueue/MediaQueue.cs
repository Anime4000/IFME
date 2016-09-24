using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FFmpegDotNet;

namespace ifme
{
    public class MediaQueue
    {
        public bool Enable { get; set; }
        public string OutputFormat { get; set; }
        public List<MediaQueueAudio> Audio { get; set; } = new List<MediaQueueAudio>();
        public List<MediaQueueVideo> Video { get; set; } = new List<MediaQueueVideo>();
        public List<MediaQueueSubtitle> Subtitle { get; set; } = new List<MediaQueueSubtitle>();
        public List<MediaQueueAttachment> Attachment { get; set; } = new List<MediaQueueAttachment>();
    }
}
