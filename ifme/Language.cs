using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using IniParser;
using IniParser.Model;

namespace ifme
{
	public class Language
	{
		private static StringComparison IC = StringComparison.InvariantCultureIgnoreCase; // Just ignore case what ever it is.

		public string Code;
		public string Name;
		public string Author;
		public string Contact;

		// No shorting, gonna use foreach that equal index of Combobox
		public static List<Language> Lists = new List<Language>();

		// Lookup
		public static string IdLookup(string Id)
		{
			foreach (var item in File.ReadAllLines("iso.code"))
			{
				string iso = item.Substring(0, 3);
				if (string.Equals(Id, iso, IC))
					return iso;
			}

			return "und";
		}

		public static void Display()
		{
			foreach (var item in Directory.GetFiles(Global.Folder.Language, "*.ini"))
			{
				var data = new FileIniDataParser().ReadFile(item, Encoding.UTF8);
				Lists.Add(new Language() {
					Code = data["info"]["Code"],
					Name = data["info"]["Name"],
					Author = data["info"]["Author"],
					Contact = data["info"]["Contact"]
				});
			}
		}

		// Globally access control
		public static IniData Get = new FileIniDataParser().ReadFile(Path.Combine(Global.Folder.Language, $"{Properties.Settings.Default.Language}.ini"), Encoding.UTF8);

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
		public static string QueueSave = "Do you want to save this queue? You can re-open next time or in CLI.";
		public static string QueueSaveError = "At least 2 or more queue";
		public static string QueueOpenChange = "Do you want to save changes to Untitled?";
		public static string Quit = "Do you want to quit and save this queue list?";

		// Deinterlace
		public static string DeInterlaceMode0 = "Deinterlace only frame";
		public static string DeInterlaceMode1 = "Deinterlace each field";
		public static string DeInterlaceMode2 = "Skips spatial interlacing frame check";
		public static string DeInterlaceMode3 = "Skips spatial interlacing field check";
		public static string DeInterlaceField0 = "Top Field First";
		public static string DeInterlaceField1 = "Bottom Field First";
		public static string DeInterlaceFlag0 = "Deinterlace all frames";
		public static string DeInterlaceFlag1 = "Deinterlace marked frames";

		//
		public static string Untitled = "Untitled";
		public static string Donate = "Donate";

		// Option, AviSynth tab
		public static string Installed = "Installed";
		public static string NotInstalled = "Not Found";

		// ToolTip
		public static string TipUpdateTitle = "Attention!";
		public static string TipUpdateMessage = "A new version is available to download! Click here to update!";
    }
}