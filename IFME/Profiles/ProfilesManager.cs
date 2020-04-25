using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
	public class ProfilesManager
	{
		public ProfilesManager()
		{
			var path = Path.Combine("Profiles");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			var folder = Path.Combine(Directory.GetCurrentDirectory(), path);

			foreach (var item in Directory.EnumerateFiles(folder, "Profile_*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var profile = JsonConvert.DeserializeObject<Profiles>(json);

					Profiles.Items.Add(profile);
				}
				catch (Exception ex)
				{
					Console2.WriteLine($"[WARN] {ex.Message}");
				}
			}
		}

		public static void Test()
		{
			var temp = new Profiles
			{
				ProfileName = "Example to test",
				ProfileAuthor = "Example Inc.",
				Container = MediaContainer.MKV,
				Video = new ProfilesVideo
				{
					Encoder = new MediaQueueVideoEncoder
					{
						Id = new Guid(),
						Preset = "veryslow",
						Tune = "psnr",
						Mode = 0,
						Value = 23,
						MultiPass = 2,
						Command = "",
					},

					Quality = new MediaQueueVideoQuality
					{
						Width = 1920,
						Height = 1080,
						FrameRate = 23.976f,
						BitDepth = 8,
						PixelFormat = 420,
						IsVFR = false,
						Command = "",
					},

					DeInterlace = new MediaQueueVideoDeInterlace
					{
						Enable = false,
						Mode = 1,
						Field = 0,
					},
				},
				Audio = new ProfilesAudio
				{
					Encoder = new MediaQueueAudioEncoder
					{
						Id = new Guid(),
						Mode = 0,
						Quality = 128,
						SampleRate = 44100,
						Channel = 2,
						Command = "",
					},
					Copy = false,
					Command = ""
				}
			};

			string json = JsonConvert.SerializeObject(temp, Formatting.Indented);
			File.WriteAllText("test.json", json);
		}
	}
}
