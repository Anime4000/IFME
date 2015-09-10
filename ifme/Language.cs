using System.Collections.Generic;
using System.IO;

using IniParser;

namespace ifme
{
	public class Language
	{
		public string ISO;
		public string Name;

		public static List<Language> Lists = new List<Language>(); // No shorting, gonna use foreach that equal index of Combobox

		public static void Load()
		{
			foreach (var item in Directory.GetFiles(Global.Folder.Language, "*.ini"))
			{
				var data = new FileIniDataParser().ReadFile(item);
				Lists.Add(new Language() { ISO = data["info"]["ISO"], Name = data["info"]["Name"] });
			}
		}

		// Main, save profiles
		public static string SaveNewProfilesTitle = "Save new profile";
		public static string SaveNewProfilesInfo = "&Please enter a new profile name";

		// Main, MessageBox
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

		// Option, AviSynth tab
		public static string Installed = "Installed";
		public static string NotInstalled = "Not Found";
    }
}