using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace ifme
{
	public class MediaPreset
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public string OutputFormat { get; set; }
        public MediaQueueVideoEncoder VideoEncoder { get; set; } = new MediaQueueVideoEncoder();
        public MediaQueueVideoQuality VideoQuality { get; set; } = new MediaQueueVideoQuality();
        public MediaQueueVideoDeInterlace VideoDeInterlace { get; set; } = new MediaQueueVideoDeInterlace();
        public MediaQueueAudioEncoder AudioEncoder { get; set; } = new MediaQueueAudioEncoder();
        public string AudioCommand { get; set; } = string.Empty;

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
                    ConsoleEx.Write(LogLevel.Error, $"Encoding Preset (JSON) seem outdated, invalid or broken {Path.GetFileName(item)}\n{ex.Message}\n\n");
                }
            }
        }

        public static void Test()
        {
            var temp = new MediaPreset
            {

            };

            File.WriteAllText("preset.example", JsonConvert.SerializeObject(temp, Formatting.Indented));
        }
	}
}
