namespace IFME.FFmpeg
{
    internal class Stream
    {

	}

	public partial class StreamCommon
	{
		public int Id { get; internal set; }
		public string Language { get; internal set; }
		public string Codec { get; internal set; }
		public string Title { get; internal set; }
	}

	public partial class StreamVideo : StreamCommon
	{
		public int Chroma { get; internal set; }
		public int BitDepth { get; internal set; }
		public int Width { get; internal set; }
		public int Height { get; internal set; }
		public bool FrameRateConstant { get; internal set; }
		public float FrameRate { get; internal set; }
		public float FrameRateAvg { get; internal set; }
		public int FrameCount { get; internal set; }
		public float Duration { get; internal set; }
		public bool Disposition_AttachedPic { get; internal set; } = false;
    }

	public partial class StreamAudio : StreamCommon
	{
		public int SampleRate { get; internal set; }
		public int Channel { get; internal set; }
		public int BitDepth { get; internal set; }
		public float Duration { get; internal set; }
	}

	public partial class StreamSubtitle : StreamCommon
	{

	}

	public partial class StreamAttachment
	{
		public int Id { get; internal set; }
		public string FileName { get; internal set; }
		public string MimeType { get; internal set; }
	}
}
