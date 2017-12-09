using System;

namespace ifme
{
	public class MediaQueueVideo : MediaQueueCommon
	{
        public MediaQueueVideoEncoder Encoder { get; set; } = new MediaQueueVideoEncoder();
        public MediaQueueVideoQuality Quality { get; set; } = new MediaQueueVideoQuality();
        public MediaQueueVideoDeInterlace DeInterlace { get; set; } = new MediaQueueVideoDeInterlace();
	}
}
