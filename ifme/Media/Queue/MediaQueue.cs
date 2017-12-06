using System.Collections.Generic;

namespace ifme
{
	public class MediaQueue
	{
		public bool Enable { get; set; }
		public string FilePath { get; set; }
        public bool HardSub { get; set; }
		public TargetFormat OutputFormat { get; set; }
		public MediaQueueTrim Trim { get; set; } = new MediaQueueTrim();
		public List<MediaQueueAudio> Audio { get; set; } = new List<MediaQueueAudio>();
		public List<MediaQueueVideo> Video { get; set; } = new List<MediaQueueVideo>();
		public List<MediaQueueSubtitle> Subtitle { get; set; } = new List<MediaQueueSubtitle>();
		public List<MediaQueueAttachment> Attachment { get; set; } = new List<MediaQueueAttachment>();
		public FFmpegDotNet.FFmpeg.Stream MediaInfo { get; set; }
	}
}
