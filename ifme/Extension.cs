using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ifme
{
	public class Extension
	{
		public string FileName;
		public string Name;
		public string Developer;
		public string Type;
		public string Version;
		public string UrlWeb;
		public string UrlVersion;
		public string UrlDownload;

		public static List<Extension> Items = new List<Extension>();

		public static void Load()
		{
			Items.Clear();

			foreach (var exts in Directory.GetFiles(Global.Folder.Extension, "*.dll", SearchOption.TopDirectoryOnly))
			{
				string filename = Path.GetFileName(exts); // Get filename without path

				if (filename.Contains(' ')) // space aware, do not include file name contain space
					continue;

				FileVersionInfo file = FileVersionInfo.GetVersionInfo(exts);
				string[] inet = file.LegalTrademarks.Split('|');

				var e = new Extension();
				e.FileName = filename;
				e.Name = file.FileDescription;
				e.Developer = String.Format("{0} ({1})", file.CompanyName, file.LegalCopyright);
				e.Type = file.Comments;
				e.Version = file.FileVersion;
				e.UrlWeb = inet[0];
				e.UrlVersion = inet[1];
				e.UrlDownload = inet[2];

				Items.Add(e);
			}
		}

		public static void CheckDefault()
		{
			if (!File.Exists(Path.Combine(Global.Folder.Extension, Properties.Settings.Default.DefaultNotepad)))
				Properties.Settings.Default.DefaultNotepad = "nemupad.dll";

			if (!File.Exists(Path.Combine(Global.Folder.Extension, Properties.Settings.Default.DefaultBenchmark)))
				Properties.Settings.Default.DefaultNotepad = "holobenchmark.dll";
		}
	}
}
