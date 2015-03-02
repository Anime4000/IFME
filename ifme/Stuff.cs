using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ifme.hitoha
{
	class Stuff
	{
		/// <summary>
		/// Change and override MKV "Writing application"
		/// </summary>
		/// <param name="file">MKV file</param>
		public static void WriteMkvTagWApp(string file)
		{
			byte[] bytes = { 0x49, 0x46, 0x4D, 0x45, 0x20, 0x34, 0x2E, 0x78, 0x20, 
							   0xE3, 0x80, 0x8C, 0xE3, 0x83, 0x8D, 0xE3, 0x83, 0xA0, 
							   0xE3, 0x81, 0x95, 0xE3, 0x82, 0x93, 0xE3, 0x81, 0xAE, 
							   0xE6, 0x9C, 0xAA, 0xE6, 0x9D, 0xA5, 0xE3, 0x80, 0x8D };

			using (var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite))
			{
				stream.Position = 4205;
				for (int i = 0; stream.Position != 4270; i++)
				{
					if (i < bytes.Length)
						stream.WriteByte(bytes[i]);
					else
						stream.WriteByte(0x00);
				}
			}
		}

		public static string[] MediaMap(string file, string type)
		{
			string test = null;
			string[] output = PrintFFmpeg(file).Split('\n');

			foreach (var item in output)
			{
				if (item.Contains("Stream") && item.Contains(type))
				{
					if (item[11] == '#')
					{
						int i = 12;
						while (true)
						{
							if (new[] { '(', '[' }.Contains(item[i]))
								break;
							if (item[i + 1] == ' ')
								break;

							test += item[i];
							i++;
						}
						test += "|";
					}
				}
			}
			return test.Remove(test.Length - 1).Split('|');
		}

		private static string PrintFFmpeg(string file)
		{
			string dir = Properties.Settings.Default.TemporaryFolder;
			string ffmpeg = Addons.BuildIn.FFmpeg;
			string pathmap = Path.Combine(dir, "map.gg");
			string pathscript = Path.Combine(dir, (OS.IsWindows ? "map.cmd" : "map.sh"));
			string filescript = String.Format((OS.IsWindows ? Properties.Resources.ScriptMapWindows : Properties.Resources.ScriptMapLinux), ffmpeg, file, pathmap);
			string line = null;

			File.WriteAllText(pathscript, filescript);

			Process P = new Process();
			var SI = P.StartInfo;

			SI.FileName = OS.IsWindows ? "cmd" : "bash";
			SI.Arguments = String.Format((OS.IsWindows ? "/c {0}" : "-c \"sh '{0}'\""), pathscript);
			SI.WorkingDirectory = dir;
			SI.CreateNoWindow = true;
			SI.UseShellExecute = false;

			P.Start();
			P.WaitForExit();
			P.Close();

			// ----- Read file even used by another process
			FileStream logFileStream = new FileStream(pathmap, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			StreamReader logFileReader = new StreamReader(logFileStream);

			while (!logFileReader.EndOfStream)
				line += logFileReader.ReadLine() + "\n";
			
			logFileReader.Close();
			logFileStream.Close();
			// -----

			return line.Remove(line.Length - 1);
		}
	}
}