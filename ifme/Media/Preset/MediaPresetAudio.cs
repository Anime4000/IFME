namespace ifme
{
	public class MediaPresetAudio
	{
        public MediaQueueAudioEncoder Encoder { get; set; } = new MediaQueueAudioEncoder();
        public string Command { get; set; }
    }
}
