using System;

namespace ifme
{
	class MediaDefaultAudio
	{
		public Guid Encoder { get; private set; }
		public int Mode { get; private set; }
		public decimal Quality { get; private set; }
		public int SampleRate { get; private set; }
		public int Channel { get; private set; }
		public string Command { get; private set; }

		public MediaDefaultAudio(MediaTypeAudio type)
		{
			switch (type)
			{
				case MediaTypeAudio.MP3:
					Encoder = new Guid("deadbeef-d003-d003-d003-d003d003d003");
					Mode = 0;
					Quality = 128000;
					SampleRate = 44100;
					Channel = 2;
					Command = string.Empty;
					break;
				case MediaTypeAudio.MP4:
					Encoder = new Guid("deadbeef-0aac-0aac-0aac-0aac0aac0aac");
					Mode = 0;
					Quality = 128000;
					SampleRate = 44100;
					Channel = 2;
					Command = string.Empty;
					break;
				case MediaTypeAudio.OGG:
					Encoder = new Guid("deadface-f154-f154-f154-f154f154f154");
					Mode = 0;
					Quality = 128000;
					SampleRate = 44100;
					Channel = 0;
					Command = string.Empty;
					break;
				case MediaTypeAudio.OPUS:
					Encoder = new Guid("deadface-f00d-f00d-f00d-f00df00df00d");
					Mode = 0;
					Quality = 128000;
					SampleRate = 44100;
					Channel = 0;
					Command = string.Empty;
					break;
				case MediaTypeAudio.FLAC:
					Encoder = new Guid("deadface-f1ac-f1ac-f1ac-f1acf1acf1ac");
					Mode = 0;
					Quality = 12;
					SampleRate = 44100;
					Channel = 0;
					Command = string.Empty;
					break;
				default:
					break;
			}
		}
	}
}
