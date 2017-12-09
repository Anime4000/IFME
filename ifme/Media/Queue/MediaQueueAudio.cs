using System;

namespace ifme
{
	public class MediaQueueAudio : MediaQueueCommon
	{
        public MediaQueueAudioEncoder Encoder { get; set; } = new MediaQueueAudioEncoder();

		public string Command { get; set; }
	}
}
