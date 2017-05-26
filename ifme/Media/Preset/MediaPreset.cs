using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
	class MediaPreset
	{
		public string Name { get; set; }
		public string Author { get; set; }
		public int OutputFormat { get; set; }
		public MediaPresetVideo Video { get; set; } = new MediaPresetVideo();
		public MediaPresetAudio Audio { get; set; } = new MediaPresetAudio();

		public static List<MediaPreset> List { get; set; } = new List<MediaPreset>();
	}
}
