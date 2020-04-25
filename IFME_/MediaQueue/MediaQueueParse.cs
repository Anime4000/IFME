using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public static class MediaQueueParse
    {
        public static MediaQueueVideo Video(string path, FFmpeg.StreamVideo data)
        {
			return new MediaQueueVideo
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				Encoder = new MediaQueueVideoEncoder
				{
					Id = new Guid("deadbeef-0265-0265-0265-026502650265"),
					Preset = "medium",
					Tune = "psnr",
					Mode = 0,
					Value = 23,
					MultiPass = 2,
					Command = string.Empty
				},

				Quality = new MediaQueueVideoQuality
				{
					Width = data.Width,
					Height = data.Height,
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
			return new MediaQueueAudio
			{
				Enable = true,
				File = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				Encoder = new MediaQueueAudioEncoder
				{
					Id = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac"),
					Mode = 0,
					Quality = 128,
					SampleRate = 48000,
					Channel = 2,
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
