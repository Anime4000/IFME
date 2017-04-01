using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ifme
{
	class MediaPresetLoad
	{
		public MediaPresetLoad()
		{
			// Check folder if exist
			if (!Directory.Exists("preset"))
				Directory.CreateDirectory("preset");

			// Read plugin JSON file
			var folder = Path.Combine(Directory.GetCurrentDirectory(), "preset");

			foreach (var item in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				var json = File.ReadAllText(item);
				var preset = JsonConvert.DeserializeObject<MediaPreset>(json);

				MediaPreset.List.Add(preset);
			}
		}
	}
}
