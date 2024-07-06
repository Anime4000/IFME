using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace IFME
{
	public class ProfilesManager
	{
		public ProfilesManager()
		{
			Profiles.Items.Clear();

			var path = Path.Combine("Profiles");
			var folder = Path.Combine(Directory.GetCurrentDirectory(), path);

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			foreach (var item in Directory.EnumerateFiles(folder, "Profile_*.json", SearchOption.AllDirectories).OrderBy(file => file))
			{
				try
				{
					var json = File.ReadAllText(item);
					var profile = JsonConvert.DeserializeObject<Profiles>(json);

					profile.ProfilePath = item;

					Profiles.Items.Add(profile);
				}
				catch (Exception ex)
				{
					frmMain.PrintLog($"[WARN] {ex.Message}");
				}
			}
		}

		public static void Save(string name, MediaContainer container, ProfilesVideo video, ProfilesAudio audio, bool mux_v = false, bool mux_a = false)
		{
			var data = new Profiles
			{
				ProfileName = name,
				ProfileAuthor = Properties.Settings.Default.Username,
				Container = container,
				TryRemuxVideo = mux_v,
				TryRemuxAudio = mux_a,
				Video = video,
				Audio = audio
			};

			string json = JsonConvert.SerializeObject(data, Formatting.Indented);
			File.WriteAllText(Path.Combine("Profiles", $"Profile_{DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss_ffff", CultureInfo.InvariantCulture)}.json"), json);
		}

		public static void Rename(int index)
		{
			if (index > -1)
			{
				var file = Profiles.Items[index].ProfilePath;
				var json = JsonConvert.SerializeObject(Profiles.Items[index], Formatting.Indented);
				File.WriteAllText(file, json);
			}
		}

		public static void Delete(int index)
		{
			if (index > -1)
			{
				var file = Profiles.Items[index].ProfilePath;
				File.Delete(file);
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
						CommandFilter = ""
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
						Quality = "128",
						SampleRate = 44100,
						Channel = 2,
						Command = "",
					},
					Command = "",
					CommandFilter = "",
				}
			};

			string json = JsonConvert.SerializeObject(temp, Formatting.Indented);
			File.WriteAllText("test.json", json);
		}
	}
}
