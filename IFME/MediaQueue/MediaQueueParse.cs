using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public static class MediaQueueParse
    {
		public static class Gui
		{
			public static class Video
			{
				public static Guid Id { get; set; } = new Guid("deadbeef-0265-0265-0265-026502650265");
				public static string Preset { get; set; } = Plugins.Items.Video[Id].Video.PresetDefault;
				public static string Tune { get; set; } = Plugins.Items.Video[Id].Video.TuneDefault;
				public static int Mode { get; set; } = 0;
				public static decimal Value { get; set; } = Plugins.Items.Video[Id].Video.Mode[0].Value.Default;
				public static int MultiPass { get; set; } = 2;
                public static bool DeInterlace { get; set; } = false;
				public static int DeInterlaceMode { get; set; } = 1;
				public static int DeInterlaceField { get; set; } = 0;
				public static string CmdFilter { get; set; } = string.Empty;
				public static string CmdDecoder { get; set; } = string.Empty;
				public static string CmdEncoder { get; set; } = string.Empty;
			}

			public static class Audio
			{
				public static Guid Id { get; set; } = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac");
                public static int Mode { get; set; } = 0;
				public static string Quality { get; set; } = Plugins.Items.Audio[Id].Audio.Mode[0].Default;
				public static int SampleRate { get; set;  } = Plugins.Items.Audio[Id].Audio.SampleRateDefault;
				public static int Channel { get; set; } = Plugins.Items.Audio[Id].Audio.ChannelDefault;
                public static string CmdFilter { get; set; } = string.Empty;
				public static string CmdDecoder { get; set; } = string.Empty;
				public static string CmdEncoder { get; set; } = string.Empty;
			}

			public static void UseDefaultVideo(Guid videoId)
			{
				if (Plugins.Items.Video.ContainsKey(videoId))
				{
                    Video.Id = videoId;
                    Video.DeInterlace = false;
                    Video.Preset = Plugins.Items.Video[videoId].Video.PresetDefault;
                    Video.Tune = Plugins.Items.Video[videoId].Video.TuneDefault;
                    Video.Mode = 0;
                    Video.Value = Plugins.Items.Video[videoId].Video.Mode[0].Value.Default;
                    Video.MultiPass = 2;
                    Video.DeInterlaceMode = 1;
                    Video.DeInterlaceField = 0;
                    Video.CmdEncoder = Plugins.Items.Video[videoId].Video.Args.Command;
                }
			}

			public static void UseDefaultAudio(Guid audioId)
			{
				if (Plugins.Items.Audio.ContainsKey(audioId))
				{
                    Audio.Mode = 0;
                    Audio.Quality = Plugins.Items.Audio[audioId].Audio.Mode[0].Default;
                    Audio.Channel = Plugins.Items.Audio[audioId].Audio.ChannelDefault;
                    Audio.SampleRate = Plugins.Items.Audio[audioId].Audio.SampleRateDefault;
                    Audio.CmdEncoder = Plugins.Items.Audio[audioId].Audio.Args.Command;
                }
			}
		}

		public static MediaQueueVideo Video(string path, FFmpeg.StreamVideo data, bool isImageSeq = false)
        {
			return new MediaQueueVideo
			{
				Enable = true,
				FilePath = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				IsImageSeq = isImageSeq,

				Info = new MediaQueueVideoInfo
				{
                    Width = data.Width,
                    Height = data.Height,
                    FrameRate = (float)Math.Round(data.FrameRateAvg, 3),
                    FrameRateAvg = data.FrameRateAvg,
                    FrameCount = (int)Math.Ceiling(data.Duration * data.FrameRateAvg),
                    IsVFR = !data.FrameRateConstant,
                    BitDepth = data.BitDepth,
                    PixelFormat = data.Chroma,
					Disposition_AttachedPic = data.Disposition_AttachedPic
                },

				Encoder = new MediaQueueVideoEncoder
				{
					Id = Gui.Video.Id,
					Preset = Gui.Video.Preset,
					Tune = Gui.Video.Tune,
					Mode = Gui.Video.Mode,
					Value = Gui.Video.Value,
					MultiPass = Gui.Video.MultiPass,
					Command = Gui.Video.CmdEncoder
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
					PixelFormat = data.Chroma,
					CommandFilter = Gui.Video.CmdFilter,
					Command = Gui.Video.CmdDecoder
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
			var ch = Plugins.Items.Audio[Gui.Audio.Id].Audio.ChannelDefault;

			if (ch == 0)
				ch = data.Channel;

			return new MediaQueueAudio
			{
				Enable = true,
				FilePath = path,
				Id = data.Id,
				Lang = data.Language,
				Codec = data.Codec,

				Info = new MediaQueueAudioInfo
				{
					SampleRate = data.SampleRate,
                    Channel = data.Channel
                },

				Encoder = new MediaQueueAudioEncoder
				{
					Id = Gui.Audio.Id,
					Mode = Gui.Audio.Mode,
					Quality = Gui.Audio.Quality,
					SampleRate = Gui.Audio.SampleRate,
					Channel = Gui.Audio.Channel,
					Command = Gui.Audio.CmdEncoder
				},

				CommandFilter = Gui.Audio.CmdFilter,
				Command = Gui.Audio.CmdDecoder
			};
		}

        public static MediaQueueSubtitle Subtitle(string path, FFmpeg.StreamSubtitle data)
        {
			return new MediaQueueSubtitle
			{
				Enable = true,
				FilePath = path,
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
				FilePath = path,
				Id = data.Id,
				Name = data.FileName,
				Mime = data.MimeType
			};
        }
    }
}
