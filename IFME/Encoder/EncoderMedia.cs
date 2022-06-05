using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IFME.OSManager;

namespace IFME
{
    internal class EncoderMedia
    {
		private static int Arch = OS.Is64bit ? 64 : 32;
		private static string FFmpeg = Path.Combine(Environment.CurrentDirectory, "Plugins", $"ffmpeg{Arch}", "ffmpeg");
		private static string MP4Box = Path.Combine(Environment.CurrentDirectory, "Plugins", "mp4box", "mp4box");

		internal string WorkingDirectory;
        internal MediaQueue Queue;

		internal void Extract()
        {
			var proc = new EncoderProcess();

            frmMain.PrintStatus("Extracting...");
            frmMain.PrintLog("[INFO] Extracting subtitle file...");

			for (int i = 0; i < Queue.Subtitle.Count; i++)
			{
				var id = Queue.Subtitle[i].Id;
				var fmt = Queue.Subtitle[i].Codec;
				var file = Queue.Subtitle[i].File;
				var lang = Queue.Subtitle[i].Lang;
				var fext = Path.GetExtension(file);

				if (id < 0)
				{
					File.Copy(file, Path.Combine(WorkingDirectory, $"subtitle{i:D4}_{lang}{fext}"));
				}
				else
				{
					proc.Start($"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{file}\" -map 0:{id} -map_metadata -1 -map_chapters -1 -vn -an -dn -scodec copy -y subtitle{i:D4}_{lang}.{fmt}", WorkingDirectory, true);
				}
			}

			frmMain.PrintLog("[INFO] Extracting embeded attachment...");

			var tempDirFont = Path.Combine(WorkingDirectory, "attachment");

			for (int i = 0; i < Queue.Attachment.Count; i++)
			{
				var id = Queue.Attachment[i].Id;
				var file = Queue.Attachment[i].File;
				var name = Queue.Attachment[i].Name;

				if (!Directory.Exists(tempDirFont))
					Directory.CreateDirectory(tempDirFont);

				if (id < 0)
				{
					File.Copy(file, Path.Combine(tempDirFont, Path.GetFileName(file)));
				}
				else
				{
					proc.Start($"\"{FFmpeg}\" -hide_banner -v panic -stats -dump_attachment:{id} {name} -i \"{file}\" -y", tempDirFont, true);
				}
			}

			// Hard Sub
			if (Queue.HardSub)
			{
				File.Copy(Path.Combine("Fonts", "fonts.conf"), Path.Combine(WorkingDirectory, "fonts.conf"), true);
				Environment.SetEnvironmentVariable("FC_CONFIG_DIR", WorkingDirectory);
				Environment.SetEnvironmentVariable("FONTCONFIG_PATH", WorkingDirectory);
				Environment.SetEnvironmentVariable("FONTCONFIG_FILE", Path.Combine(WorkingDirectory, "fonts.conf"));
			}

			// Chapters
			proc.Start($"\"{FFmpeg}\" -hide_banner -v error -stats -i \"{Queue.FilePath}\" -f ffmetadata metadata.ini -y", WorkingDirectory, true);
		}

		internal void Audio()
		{
			for (int i = 0; i < Queue.Audio.Count; i++)
			{
				var item = Queue.Audio[i];

				frmMain.PrintStatus($"Encoding, Audio #{i}");

				if (Plugins.Items.Audio.TryGetValue(item.Encoder.Id, out PluginsAudio codec))
				{
					frmMain.PrintLog("[INFO] Encoding audio file...");

					var ac = codec.Audio;
					var md = item.Encoder.Mode;
					var en = ac.Encoder;

					var trim = (Queue.Trim.Enable ? $"-ss {Queue.Trim.Start} -t {Queue.Trim.Duration}" : string.Empty);

					var qu = string.IsNullOrEmpty(ac.Mode[md].Args) ? string.Empty : $"{ac.Mode[md].Args} {ac.Mode[md].QualityPrefix}{item.Encoder.Quality}{ac.Mode[md].QualityPostfix}";
					var hz = item.Encoder.SampleRate == 0 ? string.Empty : $"-ar {item.Encoder.SampleRate}";
					var ch = item.Encoder.Channel == 0 ? string.Empty : $"-ac {item.Encoder.Channel}";

					var outfmtfile = $"audio{i:D4}_{item.Lang}.{ac.Extension}";

					var af = string.Empty;

					if (!item.CommandFilter.IsDisable())
					{
						af = $"-af {item.CommandFilter}";
					}

					if (ac.Args.Pipe)
					{
						var proc = new EncoderProcess();
						var pipe = new EncoderNamedPipe();

						proc.Start($"\"{en}\" {ac.Args.Input} {ac.Args.Command} {qu} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"", WorkingDirectory, false);
						proc.Start($"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} {af} -f wav {item.Command} pipe:1 > {pipe.CmdPipeRx}", WorkingDirectory, false);

						pipe.Start();

						ProcessManager.Start(tempDir, $"\"{FFmpeg}\" -hide_banner -v error -i \"{item.File}\" {trim} -map 0:{item.Id} -acodec pcm_s16le {hz} {ch} {af} -f wav {item.Command} - | \"{en}\" {ac.Args.Input} {ac.Args.Command} {qu} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
					}
					else
					{
						ProcessManager.Start(tempDir, $"\"{en}\" {ac.Args.Input} \"{item.File}\" {trim} -map 0:{item.Id} {ac.Args.Command} {ac.Args.Codec} {qu} {hz} {ch} {af} {item.Encoder.Command} {ac.Args.Output} \"{outfmtfile}\"");
					}
				}
			}
		}
	}
}
