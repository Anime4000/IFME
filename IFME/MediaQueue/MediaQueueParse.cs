using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public static class MediaQueueParse
    {
		public static Guid CurrentId_Video { get; set; } = new Guid("deadbeef-0265-0265-0265-026502650265");
		public static Guid CurrentId_Audio { get; set; } = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac");


		public static MediaQueueVideo Video(string path, FFmpeg.StreamVideo data, bool isImageSeq = false)
        {
			return new MediaQueueVideo
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				IsImageSeq = isImageSeq,

				Encoder = new MediaQueueVideoEncoder
				{
					Id = CurrentId_Video,
					Preset = Plugins.Items.Video[CurrentId_Video].Video.PresetDefault,
					Tune = Plugins.Items.Video[CurrentId_Video].Video.TuneDefault,
					Mode = 0,
					Value = Plugins.Items.Video[CurrentId_Video].Video.Mode[0].Value.Default,
					MultiPass = 2,
					Command = string.Empty
				},

				Quality = new MediaQueueVideoQuality
				{
					Width = data.Width,
					Height = data.Height,
					OriginalWidth = data.Width,
					OriginalHeight = data.Height,
					OriginalFrameRate = data.FrameRateAvg,
					FrameRate = (float)Math.Round(data.FrameRateAvg, 3),
					FrameRateAvg = data.FrameRateAvg,
					FrameCount = (int)Math.Ceiling(data.Duration * data.FrameRate),
					IsVFR = !data.FrameRateConstant,
					BitDepth = data.BitDepth,
					PixelFormat = data.Chroma
				},

				DeInterlace = new MediaQueueVideoDeInterlace
				{
					Enable = false,
					Mode = 1,
					Field = 0
				}
			};
		}

        public static MediaQueueAudio Audio(string path, FFmpeg.StreamAudio data)
        {
			var ch = Plugins.Items.Audio[CurrentId_Audio].Audio.ChannelDefault;

			if (ch == 0)
				ch = data.Channel;

			return new MediaQueueAudio
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				Encoder = new MediaQueueAudioEncoder
				{
					Id = CurrentId_Audio,
					Mode = 0,
					Quality = Plugins.Items.Audio[CurrentId_Audio].Audio.Mode[0].Default,
					SampleRate = Plugins.Items.Audio[CurrentId_Audio].Audio.SampleRateDefault,
					Channel = ch,
					Command = string.Empty
				}
			};
		}

        public static MediaQueueSubtitle Subtitle(string path, FFmpeg.StreamSubtitle data)
        {
			return new MediaQueueSubtitle
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,
			};
		}

        public static MediaQueueAttachment Attachment(string path, FFmpeg.StreamAttachment data)
        {
			return new MediaQueueAttachment
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Name = data.FileName,
				Mime = data.MimeType
			};
        }
    }
}
