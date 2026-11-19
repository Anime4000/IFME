using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal partial class API
{
	internal static class LinuxProcess
	{
		// Linux signal numbers (identical on x86, x86-64, ARM and ARM64).
		private const int SIGCONT = 18;
		private const int SIGSTOP = 19;

		[DllImport("libc", SetLastError = true)]
		private static extern int kill(int pid, int sig);

		private static bool HasProcFs => Directory.Exists("/proc/self");

		/// <summary>
		/// Reads the process table. On Linux this parses /proc directly, which used to be done
		/// by spawning bash + pgrep for every lookup. Other Unix systems fall back to ps.
		/// </summary>
		internal static List<ProcessEx.ProcessEntry> Snapshot()
		{
			return HasProcFs ? ReadProcFs() : ReadPs();
		}

		private static List<ProcessEx.ProcessEntry> ReadProcFs()
		{
			var list = new List<ProcessEx.ProcessEntry>();

			foreach (var dir in Directory.EnumerateDirectories("/proc"))
			{
				if (!int.TryParse(Path.GetFileName(dir), out int pid))
					continue;

				try
				{
					// Format: "pid (comm) state ppid ... starttime ...". comm may itself contain
					// spaces and parentheses, so split after the last ')'.
					var stat = File.ReadAllText(Path.Combine(dir, "stat"));
					var end = stat.LastIndexOf(')');

					if (end < 0 || end + 2 >= stat.Length)
						continue;

					var fields = stat.Substring(end + 2).Split(' ');

					// fields[0] is field 3 (state), fields[1] is field 4 (ppid),
					// fields[19] is field 22 (starttime, clock ticks after boot).
					if (fields.Length < 20 || !int.TryParse(fields[1], out int ppid))
						continue;

					long.TryParse(fields[19], out long start);

					list.Add(new ProcessEx.ProcessEntry { Id = pid, ParentId = ppid, StartTime = start });
				}
				catch
				{
					// Process exited while we were reading it.
				}
			}

			return list;
		}

		private static List<ProcessEx.ProcessEntry> ReadPs()
		{
			var list = new List<ProcessEx.ProcessEntry>();

			using (var p = Process.Start(new ProcessStartInfo("ps", "-A -o pid= -o ppid=")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true
			}))
			{
				string line;

				while ((line = p.StandardOutput.ReadLine()) != null)
				{
					var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length >= 2 && int.TryParse(parts[0], out int pid) && int.TryParse(parts[1], out int ppid))
						list.Add(new ProcessEx.ProcessEntry { Id = pid, ParentId = ppid });
				}

				p.WaitForExit();
			}

			return list;
		}

		internal static void Suspend(int[] pids)
		{
			foreach (var pid in pids)
				Signal(pid, SIGSTOP, "STOP");
		}

		internal static void Resume(int[] pids)
		{
			foreach (var pid in pids)
				Signal(pid, SIGCONT, "CONT");
		}

		private static void Signal(int pid, int number, string name)
		{
			// Signal numbers differ between Linux and macOS/BSD, so only use the raw syscall
			// when we know we are on Linux; otherwise let kill(1) translate the name.
			if (HasProcFs)
			{
				try
				{
					kill(pid, number);
					return;
				}
				catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException)
				{
					// fall through to kill(1)
				}
			}

			try
			{
				// No shell here: the old "> /dev/null 2>&1" suffix was handed to kill as extra
				// arguments, so every pause/resume printed parse errors.
				using (var p = Process.Start(new ProcessStartInfo("kill", $"-{name} {pid}")
				{
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardError = true
				}))
				{
					p.WaitForExit();
				}
			}
			catch
			{
				// Process already exited.
			}
		}
	}
}
