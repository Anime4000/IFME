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
			string[] GFI = new string[7];

			try
			{
				MediaFile AviFile = new MediaFile(file);
				GFI[0] = System.IO.Path.GetFileNameWithoutExtension(file);
				GFI[1] = System.IO.Path.GetExtension(file);

				if (AviFile.Video.Count == 0)
					return GFI;

				var v = AviFile.Video[0];
				GFI[2] = v.format;
				GFI[3] = v.width.ToString() + "x" + v.height.ToString();
				GFI[4] = "[" + v.frameRateMode + "] " + v.frameRate.ToString();
				GFI[5] = v.bitDepth.ToString() + " bits";
				GFI[6] = file;

				return GFI;
			}
			catch (Exception ex)
			{
				GFI[1] = ex.Message;
				return GFI;
			}
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

		// This block detect subtitle file, due performance issue detect via file extension,
		// there will consume process to check file is binary or plain text
		public static bool SubtitleValid(string file)
		{
			var ext = System.IO.Path.GetExtension(file);

			if (ext == ".ass")
				return true;

			if (ext == ".ssa")
				return true;

			if (ext == ".srt")
				return true;

			return false;
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

		// This block will detect font magic number, much more better then detect file extension
		// useful for binary file
		public static bool AttachmentValid(string file)
		{
			System.IO.FileInfo f = new System.IO.FileInfo(file);
			if (f.Length >= 1073741824)							// Detect 1GiB file enough, no font that large
				return false;

			byte[] data = System.IO.File.ReadAllBytes(file);
			byte[] MagicTTF = { 0x00, 0x01, 0x00, 0x00, 0x00 };
			byte[] MagicOTF = { 0x4F, 0x54, 0x54, 0x4F, 0x00 };
			byte[] MagicWOFF = { 0x77, 0x4F, 0x46, 0x46, 0x00 };
			byte[] check = new byte[5];

			Buffer.BlockCopy(data, 0, check, 0, 5);

			if (MagicTTF.SequenceEqual(check))
				return true;

			if (MagicOTF.SequenceEqual(check))
				return true;

			if (MagicWOFF.SequenceEqual(check))
				return true;

			return false;
		}
	}
}
