using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MediaInfoDotNet;

namespace ifme
{
	class GetInfo
	{
		static StringComparison IC = StringComparison.OrdinalIgnoreCase;

		public static bool IsAviSynth(string file)
		{
			string exts = Path.GetExtension(file);
			return String.Equals(exts, ".avs", IC) ? true : false;
		}

		public static bool SubtitleValid(string file)
		{
			string exts = Path.GetExtension(file);
			if (String.Equals(exts, ".ass", IC))
				return true;
			else if (String.Equals(exts, ".ssa", IC))
				return true;
			else if (String.Equals(exts, ".srt", IC))
				return true;
			else
				return false;
		}

		public static string AttachmentValid(string file)
		{
			FileInfo f = new FileInfo(file);

			if (f.Length >= 1073741824)
				return "application/octet-stream";

			byte[] data = System.IO.File.ReadAllBytes(file);
			byte[] MagicTTF = { 0x00, 0x01, 0x00, 0x00, 0x00 };
			byte[] MagicOTF = { 0x4F, 0x54, 0x54, 0x4F, 0x00 };
			byte[] MagicWOFF = { 0x77, 0x4F, 0x46, 0x46, 0x00 };
			byte[] check = new byte[5];

			Buffer.BlockCopy(data, 0, check, 0, 5);

			if (MagicTTF.SequenceEqual(check))
				return "application/x-truetype-font";

			if (MagicOTF.SequenceEqual(check))
				return "application/vnd.ms-opentype";

			if (MagicWOFF.SequenceEqual(check))
				return "application/font-woff";

			return "application/octet-stream";
		}

		public static string FileName(string file)
		{
			return Path.GetFileName(file);
		}

		public static string FileNameNoExt(string file)
		{
			return Path.GetFileNameWithoutExtension(file);
		}

		public static string FileSize(string file)
		{
			FileInfo f = new FileInfo(file);
			long byteCount = f.Length;

			string[] IEC = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
			if (byteCount == 0)
				return "0" + IEC[0];

			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + " " + IEC[place];
		}

		public static string FileLang(string file)
		{
			file = Path.GetFileNameWithoutExtension(file);
			return file.Substring(file.Length - 3);
		}

		public static string Duration(DateTime past)
		{
			TimeSpan span = DateTime.Now.Subtract(past);

			if (span.Days != 0)
				return String.Format("{0}d {1}h {2}m {3}s {4}ms", span.Days, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			else if (span.Hours != 0)
				return String.Format("{0}h {1}m {2}s {3}ms", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
			else if (span.Minutes != 0)
				return String.Format("{0}m {1}s {2}ms", span.Minutes, span.Seconds, span.Milliseconds);
			else
				return String.Format("{0}s {1}ms", span.Seconds, span.Milliseconds);
		}
	}
}
