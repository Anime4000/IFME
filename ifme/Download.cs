using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace ifme
{
	public class Download
	{
		WebClient client = new WebClient();
		bool finish = false;
		int curX = 0;
		int curY = 0;

		public Download()
		{
			client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
			client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
		}

		/// <summary>
		/// Fetch a string from web server.
		/// </summary>
		/// <param name="url">A direct link of HTTP/HTTPS.</param>
		/// <returns>return a string that fetch from web server.</returns>
		public string GetString(string url)
		{
			var file = Path.Combine(Path.GetTempPath(), $"{DateTime.Now:yyyyMMdd-HHmmss-ffff}.cccp");
			var str = string.Empty;

			try
			{
				client.DownloadFile(url, file);
			}
			catch (Exception)
			{
				str = string.Empty;
				LogError("Not connected to the internet?");
			}

			if (File.Exists(file))
			{
				str = File.ReadAllText(file);
				File.Delete(file);
			}

			return str;
		}

		/// <summary>
		/// Download a file from web server
		/// </summary>
		/// <param name="url">A direct link of HTTP/HTTPS.</param>
		/// <param name="savefile">Save file location, full path is required.</param>
		public void GetFile(string url, string savefile)
		{
			string tempfile = Path.Combine(Path.GetTempPath(), $"{DateTime.Now:yyyyMMdd-HHmmss-ffff}.cccp");

			try
			{
				client.DownloadFile(url, tempfile);
			}
			catch (Exception)
			{
				LogError($"Problem when trying to download: {url}");
			}
			finally
			{
				if (File.Exists(tempfile)) // check temp file exist
				{
					if (File.Exists(savefile)) // check save exist
					{
						File.Delete(savefile); // delete old save file
						File.Move(tempfile, savefile); // move new downloaded file
					}
					else
					{
						File.Move(tempfile, savefile); // old save file not found, move just like that
					}
				}
			}
		}

		/// <summary>
		/// Download a archive from web server and extract.
		/// </summary>
		/// <param name="url">A direct link of HTTP/HTTPS.</param>
		/// <param name="folder">Save folder location which will contain extracted file. full path is required.</param>
		public void GetFileExtract(string url, string folder)
		{
			string tempfile = Path.Combine(Path.GetTempPath(), $"{DateTime.Now:yyyyMMdd-HHmmss-ffff}.cccp");

			try
			{
				finish = false;

				curX = Console.CursorLeft + 1;
				curY = Console.CursorTop;

				client.DownloadFileAsync(new Uri(url), tempfile);

				while (!finish) { Thread.Sleep(1); }

				if (File.Exists(tempfile))
					Extract(tempfile, folder);
				else
					LogError($"Oh no! given \"{url}\" was not saved");
			}
			catch (Exception)
			{
				LogError("File not found or Offline");
			}
		}

		private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			Console.BackgroundColor = ConsoleColor.Blue;
			Console.ForegroundColor = ConsoleColor.White;

			Console.SetCursorPosition(curX, curY);
			Console.Write($"{((float)e.BytesReceived / e.TotalBytesToReceive):P}");
			Console.SetCursorPosition(curX, curY);
		}

		private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			finish = true;
		}

		private void Extract(string archivefile, string savefolder)
		{
			string unzip = Path.Combine(Global.Folder.Root, "7za");

			Console.SetCursorPosition(curX, curY);
			Console.Write($"Extracting...");

			TaskManager.Run($"\"{unzip}\" x \"{archivefile}\" -y -o\"{savefolder}\" > {OS.Null} 2>&1");

			Console.Write("Done!");
			File.Delete(archivefile);

			Console.ResetColor();
		}

		private void LogError(string message)
		{
			Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("ERROR: ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write($"{message}");
			Console.ResetColor();
		}
	}
}
