using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace ifme
{
	class AviSynth
	{
		internal static Dictionary<string, string> Version = new Dictionary<string, string>();

		private static string AviSynthPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "avisynth.dll");
			}
		}

		private static string AviSynthPath64
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "avisynth.dll");
			}
		}

		internal static void Load()
		{
			Version.Clear();
			try
			{
				Version = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(Get.AppRootDir, "avisynth.json")));
			}
			catch (Exception ex)
			{
				ConsoleEx.Write(LogLevel.Error, "IFME cannot detect which version AviSynth. (");
				ConsoleEx.Write(ConsoleColor.Red, ex.Message);
				ConsoleEx.Write(") however, you can ignore this error.\n");
			}
		}

		internal static bool IsInstalled
		{
			get
			{
				if (File.Exists(AviSynthPath))
					return true;

				return false;
			}
		}

		internal static bool IsInstalled64
		{
			get
			{
				if (File.Exists(AviSynthPath64))
					return true;

				return false;
			}
		}

		internal static string InstalledVersion
		{
			get
			{
				var temp = string.Empty;

				if (Version.TryGetValue(CRC32.GetFile(AviSynthPath), out temp))
				{
					return temp;
				}

				return $"[32-bit] Unknown Version (CRC: {CRC32.GetFile(AviSynthPath)})";
			}
		}

		internal static string InstalledVersion64
		{
			get
			{
				var temp = string.Empty;

				if (Version.TryGetValue(CRC32.GetFile(AviSynthPath64), out temp))
				{
					return temp;
				}

				return $"[64-bit] Unknown Version (CRC: {CRC32.GetFile(AviSynthPath)})";
			}
		}
	}
}
