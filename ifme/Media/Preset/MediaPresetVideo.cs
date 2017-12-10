namespace ifme
{
	public class MediaPresetVideo
	{
        public MediaQueueVideoEncoder Encoder { get; set; } = new MediaQueueVideoEncoder();
        public MediaQueueVideoQuality Quality { get; set; } = new MediaQueueVideoQuality();
        public MediaQueueVideoDeInterlace DeInterlace { get; set; } = new MediaQueueVideoDeInterlace();
    }
}
