using System;
using System.Collections.Generic;

enum QueueProp
{
	FormatMkv,
	FormatMp4,
	PictureResolution,
	PictureFrameRate,
	PictureBitDepth,
	PictureChroma,
	PictureYadifEnable,
	PictureYadifMode,
	PictureYadifField,
	PictureYadifFlag,
	PictureCopyVideo,
	VideoPreset,
	VideoTune,
	VideoType,
	VideoValue,
	VideoCommand,
	AudioEncoder,
	AudioBitRate,
	AudioFreq,
	AudioChannel,
	AudioMerge,
	AudioCommand,
}

namespace ifme
{
	public class Queue
	{
		public bool IsEnable;
		public data Data = new data();
		public properties Prop = new properties();

		public picture Picture = new picture();
		public video Video = new video();

		public bool AudioMerge;
		public List<audio> Audio = new List<audio>();

		public bool SubtitleEnable;
		public List<subtitle> Subtitle = new List<subtitle>();

		public bool AttachEnable;
		public List<attachment> Attach = new List<attachment>();

		public static string SaveFile;

		public class data
		{
			public string File;

			public bool IsFileMkv;
			public bool IsFileAvs;

			public bool SaveAsMkv;
		}

		public class properties
		{
			public bool IsVFR;
			public int Duration; // in ms
			public int FrameCount;
		}

		public class picture
		{
			public bool IsCopy;
			public bool IsHevc;

			public string Resolution;
			public string FrameRate;
			public int BitDepth;
			public int Chroma;

			public bool YadifEnable;
			public int YadifMode;
			public int YadifField;
			public int YadifFlag;

			public string Command;
		}

		public class video
		{
			public string Preset;
			public string Tune;
			public int Type;
			public string Value;
			public string Command;
		}

		public class audio
		{
			public bool Enable;
			public string File;
			public bool Embedded;
			public string Id;
			public string Lang;
			public string Codec;
			public string Format;

			public int RawBit;
			public int RawFreq;
			public int RawChan;

			public Guid Encoder;
			public string BitRate;
			public string Freq;
			public string Chan;
			public string Args;
		}

		public class subtitle
		{
			public string File;
			public string Lang;
		}

		public class attachment
		{
			public string File;
			public string MIME;
			public string Comment = "No";
		}
	}
}
