using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

[JsonConverter(typeof(StringEnumConverter))]
public enum MediaContainer
{
	AVI,
	MP4,
	MKV,
	WEBM,
	TS,
	M2TS,

	MP2,
	MP3,
	M4A,
	OGG,
	OPUS,
	FLAC
}

namespace IFME
{
	public class MediaQueue
    {
		public bool Enable { get; set; }
		public bool HardSub { get; set; }
		public bool AudioOnly { get; set; } = false;
		public bool FastMuxVideo { get; set; } = false;
		public bool FastMuxAudio { get; set; } = false;

		public string FilePath { get; set; }
		public ulong FileSize { get; set; }
		public ulong BitRate { get; set; }
		public float Duration { get; set; }
		public string InputFormat { get; set; }
		public MediaContainer OutputFormat { get; set; }
		public int ProfileId { get; set; } = -1;
		public MediaQueueTrim Trim { get; set; } = new MediaQueueTrim();
		public MediaQueueCrop Crop { get; set; } = new MediaQueueCrop();
		public List<MediaQueueAudio> Audio { get; set; } = new List<MediaQueueAudio>();
		public List<MediaQueueVideo> Video { get; set; } = new List<MediaQueueVideo>();
		public List<MediaQueueSubtitle> Subtitle { get; set; } = new List<MediaQueueSubtitle>();
		public List<MediaQueueAttachment> Attachment { get; set; } = new List<MediaQueueAttachment>();
		public FFmpeg.MediaInfo Info { get; set; } = new FFmpeg.MediaInfo();
	}

	public partial class MediaQueueCommon
	{
		public bool Enable { get; set; }
		public int Id { get; set; }
		public float Duration { get; set; }
		public string Lang { get; set; }
		public string File { get; set; }
		public string Codec { get; set; }
	}

	public class MediaQueueTrim
	{
		public bool Enable { get; set; } = false;
		public string Start { get; set; } = "00:00:00.000";
		public string End { get; set; } = "00:00:00.000";
		public string Duration { get; set; } = "00:00:00.000";
	}

	public class MediaQueueCrop
	{
		public bool Enable { get; set; } = false;
		public string Start { get; set; } = "00:00:10.000";
		public string Duration { get; set; } = "00:00:30.000";
	}
}
