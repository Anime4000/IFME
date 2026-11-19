using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class OS
{
	/// <summary>
	/// Name of the environment variable that carries a command into the shell. Kept lower
	/// case on purpose: .NET Framework stores ProcessStartInfo.EnvironmentVariables in a
	/// StringDictionary that lower-cases keys, and bash variable names are case sensitive.
	/// </summary>
	private const string ShellCommandVariable = "ifme_shell_cmd";

	public static bool IsWindows
	{
		get
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT;
		}
	}

	public static bool IsUnix
	{
		get
		{
			return Environment.OSVersion.Platform == PlatformID.Unix;
		}
	}

	public static bool IsMacOSX
	{
		get
		{
			return Environment.OSVersion.Platform == PlatformID.MacOSX;
		}
	}

	public static bool IsLinux
	{
		get
		{
			return (IsUnix) || (IsMacOSX) || (Environment.OSVersion.Platform == (PlatformID)128);
		}
	}

	public static bool Is64bit
	{
		get
		{
			return Environment.Is64BitOperatingSystem;
		}
	}

	public static string Null
	{
		get
		{
			if (IsWindows)
				return "nul";
			else
				return "/dev/null";
		}
	}

	public static string Console
	{
		get
		{
			if (IsWindows)
				return "cmd";
			else
				return "bash";
		}
	}

	public static string ConsoleCmd
	{
		get
		{
			if (IsWindows)
				return "/c";

			return "-c";
		}
	}

	public static string ConsoleEnv(string EnvironmentVariable)
	{
		if (IsWindows)
			return $"/c %{EnvironmentVariable}%";

		// The quotes around the expansion matter: unquoted, bash word-splits and glob-expands
		// the command before eval sees it, so runs of whitespace inside quoted file names were
		// collapsed ("Episode 01  [BD].mkv" became "Episode 01 [BD].mkv" -> file not found).
		return $"-c 'eval \"${EnvironmentVariable}\"'";
	}

	/// <summary>
	/// Builds a <see cref="ProcessStartInfo"/> that runs <paramref name="command"/> through the
	/// platform shell (cmd on Windows, bash elsewhere), so pipes and redirection work.
	/// The command is handed over in an environment variable that is private to the child
	/// process, which avoids a second round of argument parsing by the runtime and cannot
	/// race with other commands started at the same time.
	/// </summary>
	public static ProcessStartInfo ShellStartInfo(string command, string workingDirectory = "")
	{
		if (!IsWindows)
		{
			// bash would treat a newline as a command separator; cmd never sees one here.
			command = command.Replace("\r\n", " ").Replace('\r', ' ').Replace('\n', ' ');
		}

		var info = new ProcessStartInfo(Console, ConsoleEnv(ShellCommandVariable))
		{
			UseShellExecute = false,
			CreateNoWindow = true,
			WorkingDirectory = workingDirectory ?? string.Empty
		};

		info.EnvironmentVariables[ShellCommandVariable] = command;

		return info;
	}

	public static void PowerOff(int delay = 0)
	{
		if (IsWindows)
		{
			Process.Start("shutdown.exe", $"-s -f -t {delay}");
		}
		else
		{
			// `poweroff` needs root on most distros. `systemctl poweroff` is allowed for the
			// user of the active desktop session through logind/polkit, so try it first.
			Process.Start(new ProcessStartInfo("bash", $"-c \"sleep {Math.Max(0, delay)}; systemctl poweroff || shutdown -h now || poweroff\"")
			{
				UseShellExecute = false,
				CreateNoWindow = true
			});
		}
	}

	/// <summary>
	/// Opens a URL in the user's default browser.
	/// </summary>
	public static void OpenUrl(string url)
	{
		try
		{
			if (IsWindows)
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				Process.Start(new ProcessStartInfo("open", $"\"{url}\"") { UseShellExecute = false });
			else
				Process.Start(new ProcessStartInfo("xdg-open", $"\"{url}\"") { UseShellExecute = false });
		}
		catch (Exception ex)
		{
			System.Console.Error.WriteLine($"Failed to open URL {url}: {ex.Message}");
		}
	}

	public static string PrintFileSize(ulong value)
	{
		if (IsWindows)
		{
			string[] IEC = { "Bytes", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

			if (value == 0)
				return $"0 {IEC[0]}";

			long bytes = Math.Abs((long)value);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return $"{(Math.Sign((long)value) * num)} {IEC[place]}";
		}
		else
		{
			string[] DEC = { "Bytes", "KB", "MB", "GB", "TB", "PB", "EB" };

			if (value == 0)
				return $"0 {DEC[0]}";

			long bytes = Math.Abs((long)value);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1000)));
			double num = Math.Round(bytes / Math.Pow(1000, place), 1);
			return $"{(Math.Sign((long)value) * num)} {DEC[place]}";
		}
	}

	public static List<string> FilesRecursive()
	{
		var files = new List<string>();
		var fbd = new FolderBrowserDialog
		{
			ShowNewFolderButton = false,
			SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
		};

		if (fbd.ShowDialog() == DialogResult.OK)
		{
			var dirs = new List<string>(Directory.GetDirectories(fbd.SelectedPath));

			if (dirs.Count == 0)
				dirs.Add(fbd.SelectedPath);

			foreach (var d in dirs)
			{
				foreach (var f in Directory.GetFiles(d))
				{
					files.Add(f);
				}
			}
		}

		return files;
	}

	public static bool IsProgramInPath(string programName)
	{
		string pathEnv = Environment.GetEnvironmentVariable("PATH");

		if (string.IsNullOrEmpty(pathEnv) || string.IsNullOrEmpty(programName))
			return false;

		foreach (string entry in pathEnv.Split(Path.PathSeparator))
		{
			// Windows users often wrap PATH entries in quotes; an empty entry would make
			// Path.Combine resolve against the working directory instead of PATH.
			string path = entry.Trim().Trim('"');

			if (path.Length == 0)
				continue;

			try
			{
				// Check without extension
				string fullPath = Path.Combine(path, programName);

				if (File.Exists(fullPath) && IsExecutable(fullPath))
					return true;

				// Check with .exe extension for compatibility
				fullPath = Path.Combine(path, programName + ".exe");

				if (File.Exists(fullPath) && IsExecutable(fullPath))
					return true;
			}
			catch (ArgumentException)
			{
				// Malformed PATH entry (illegal characters): skip it rather than failing the lookup.
			}
		}

		return false;
	}

	public static bool IsExecutable(string filePath)
	{
		try
		{
			if (IsWindows)
			{
				// Unchanged Windows behaviour: reparse points (App Execution Alias stubs and
				// symlinks) are not treated as runnable programs.
				return (new FileInfo(filePath).Attributes & FileAttributes.ReparsePoint) == 0;
			}

			// On Unix, Mono reports symlinks as reparse points, which wrongly rejected tools
			// installed through update-alternatives, Homebrew or snap (all symlinks).
			// Ask the kernel whether the file is actually executable instead.
			return access(filePath, X_OK) == 0;
		}
		catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
		{
			return File.Exists(filePath);
		}
		catch
		{
			return false;
		}
	}

	private const int X_OK = 1;

	[DllImport("libc", SetLastError = true)]
	private static extern int access(string pathname, int mode);

	public static byte[] ComputeSHA256(string input)
	{
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        }
    }
}