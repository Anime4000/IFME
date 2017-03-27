using System;
using System.IO;

namespace ifme
{
	class AviSynth
	{
		private static string AviSynthPath
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "avisynth.dll");
			}
		}

		public static bool IsInstalled
		{
			get
			{
				if (File.Exists(AviSynthPath))
					return true;

				return false;
			}
		}

		public static string InstalledVersion
		{
			get
			{
				if (IsInstalled)
				{
					if (string.Equals(CRC32.GetFile(AviSynthPath), "0x073A3318"))
					{
						return "2.6 MT 32bit (2015.02.20)";
					}
					else if (string.Equals(CRC32.GetFile(AviSynthPath), "0x30E0D263"))
					{
						return "2.6 ST 32bit (Original)";
					}
				}

				return "Unknown Version";
			}
		}
	}
}
