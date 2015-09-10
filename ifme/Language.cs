using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
	public class Language
	{
		public static string SaveNewProfilesTitle = "Save new profile";
		public static string SaveNewProfilesInfo = "&Please enter a new profile name";

        public static string OneItem = "Please select one";
		public static string OneVideo = "Please select one video";
		public static string NotSupported = "This format was not supported for this feature";
		public static string SelectOneVideoSubtitle = "Please select one video before removing subtitles";
		public static string SelectOneVideoAttch = "Please select one video before removing attachment";
		public static string SelectOneVideoPreview = "Please select only one video for preview";
		public static string SelectNotAviSynth = "Please select video not AviSynth script";
		public static string BenchmarkNoFile = "No video file selected, use RAW 4K video as benchmark?";
		public static string BenchmarkDownload = "File not exist, let us download before start?";
		public static string SelectAviSynth = "This not an AviSynth script";
		public static string VideoToAviSynth = "Do you want to change selected file into an AviSynth script\n(used for bypass FFmpeg decoder, unsupported file will be skip)";
    }
}