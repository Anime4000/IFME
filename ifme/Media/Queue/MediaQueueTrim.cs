namespace ifme
{
	public class MediaQueueTrim
	{
		public bool Enable { get; set; } = false;
		public string Start { get; set; } = "00:00:00";
		public string End { get; set; } = "00:00:00";
		public string Duration { get; set; } = "00:00:00";
	}
}
