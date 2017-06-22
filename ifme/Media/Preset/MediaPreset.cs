using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ifme
{
	class MediaPreset
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public int OutputFormat { get; set; }
		public MediaPresetVideo Video { get; set; } = new MediaPresetVideo();
		public MediaPresetAudio Audio { get; set; } = new MediaPresetAudio();

        public static Dictionary<string, MediaPreset> List = new Dictionary<string, MediaPreset>();

        public static void Load()
        {
            // Check folder if exist
            if (!Directory.Exists("preset"))
                Directory.CreateDirectory("preset");

            // Read plugin JSON file
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "preset");

            // Clear
            List.Clear();

            foreach (var item in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories).OrderBy(file => file))
            {
                try
                {
                    var json = File.ReadAllText(item);
                    var preset = JsonConvert.DeserializeObject<MediaPreset>(json);

                    List.Add(Path.GetFileNameWithoutExtension(item), preset);
                }
                catch (Exception ex)
                {
                    ConsoleEx.Write(LogLevel.Error, $"Encoding Preset JSON file ");
                    ConsoleEx.Write(ConsoleColor.Red, $"`{Path.GetFileName(item)}'");
                    ConsoleEx.Write($" appears to be invalid.\nException thrown: {ex.Message}\n");
                }
            }
        }
	}
}
