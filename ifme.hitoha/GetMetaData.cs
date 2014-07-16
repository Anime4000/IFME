using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Asset
using ifme.hitoha;
using MediaInfoDotNet;

namespace ifme.hitoha
{
	class GetMetaData
	{
		public static string[] MediaData(string file)
		{
			string[] GFI = new string[6];

			MediaFile AviFile = new MediaFile(file);
			GFI[0] = System.IO.Path.GetFileNameWithoutExtension(file);
			GFI[1] = System.IO.Path.GetExtension(file);

			if (AviFile.Video.Count == 0)
				return GFI;

			var v = AviFile.Video[0];
			GFI[2] = v.format;
			GFI[3] = v.width.ToString() + "x" + v.height.ToString();
			GFI[4] = v.bitDepth.ToString();
			GFI[5] = file;

			return GFI;
		}

		public static string[] SubtitleData(string file)
		{
			string[] SB = new string[4];

			SB[0] = System.IO.Path.GetFileNameWithoutExtension(file);
			SB[1] = System.IO.Path.GetExtension(file);

			foreach (var item in System.IO.File.ReadAllLines(Globals.Files.ISO))
			{
				if (SB[0].Length > 3)
				{
					if (SB[0].Substring(SB[0].Length - 3) == item.Substring(0, 3))
					{
						SB[2] = item;
						break;
					}
					else
					{
						SB[2] = "und (Undetermined)";
					}
				}
			}

			SB[3] = file;
			return SB;
		}

		public static string[] AttachmentData(string file)
		{
			string[] AD = new string[4];

			AD[0] = System.IO.Path.GetFileNameWithoutExtension(file);
			AD[1] = System.IO.Path.GetExtension(file);

			if (AD[1] == ".ttf")
			{
				AD[2] = "application/x-truetype-font";
			}
			else if (AD[1] == ".otf")
			{
				AD[2] = "application/vnd.ms-opentype";
			}
			else if (AD[1] == ".woff")
			{
				AD[2] = "application/font-woff";
			}
			else
			{
				AD[2] = "application/octet-stream";
			}

			AD[3] = file;
			return AD;
		}
	}
}
