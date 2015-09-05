using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

using ifmelib;

[Flags()]
public enum ThreadAccess : int
{
	TERMINATE = 1,
	SUSPEND_RESUME = 2,
	GET_CONTEXT = 8,
	SET_CONTEXT = 16,
	SET_INFORMATION = 32,
	QUERY_INFORMATION = 64,
	SET_THREAD_TOKEN = 128,
	IMPERSONATE = 256,
	DIRECT_IMPERSONATION = 512,
}

namespace ifme
{
	public class TaskManager
	{
		static string CurrentProc;

		public static int Run(string command)
		{
			string exe;
			string arg;

			Environment.SetEnvironmentVariable("IFMECMD", command, EnvironmentVariableTarget.Process);

			if (OS.IsWindows)
			{
				exe = "cmd";
				arg = "/c %IFMECMD%"; // allow max args to pass, Windows limit 8191
			}
			else
			{
				exe = "bash";
				arg = "-c '" + command + "'"; // POSIX has 2091769, silly Windows
			}

			GetProcess(command); // split command args and capture any binary file, allow modify CPU affinity and priority

			var p = new Process();
			p.StartInfo = new ProcessStartInfo(exe, arg)
			{
				UseShellExecute = false,
				WorkingDirectory = Properties.Settings.Default.DirTemp,
			};

			p.Start(); CPU.SetPriority(CurrentProc); // set cpu affinity and priority (windows only, linux require root)
			p.WaitForExit();

			return p.ExitCode;
		}

		public static void Pause()
		{
			if (OS.IsLinux)
				Linux.SuspendProcess(CurrentProc);
			else
				Windows.SuspendProcess(CurrentProc);
		}

		public static void Resume()
		{
			if (OS.IsLinux)
				Linux.ResumeProcess(CurrentProc);
			else
				Windows.ResumeProcess(CurrentProc);
		}

		public static void Kill()
		{
			Process[] Task = Process.GetProcessesByName(CurrentProc);
			foreach (Process P in Task)
			{
				P.Kill();
			}
		}

		static void GetProcess(string commandLine)
		{
			string[] args = SplitArguments(commandLine);
			for (int i = args.Length - 1; i > 0; i--)
			{
				if (args[i].Contains('\\') || args[i].Contains('/'))
				{
					if (args[i].Contains("plugins"))
					{
						CurrentProc = Path.GetFileNameWithoutExtension(args[i]);
						break;
					}
				}
			}
		}

		static string[] SplitArguments(string CmdLine)
		{
			var re = @"\G(""((""""|[^""])+)""|(\S+)) *";
			var ms = Regex.Matches(CmdLine, re);
			return ms.Cast<Match>()
						 .Select(m => Regex.Replace(
							 m.Groups[2].Success
								 ? m.Groups[2].Value
								 : m.Groups[4].Value, @"""""", @"""")).ToArray();

			// Ref: http://stackoverflow.com/a/19091999/3827425
		}

		public class CPU
		{
			public static bool[] Affinity = new bool[Environment.ProcessorCount];

			public static string DefaultAll(bool yes)
			{
				string x = "";
				for (int i = 0; i < Environment.ProcessorCount; i++)
				{
					Affinity[i] = yes;
					x += yes.ToString() + ",";
				}
				x = x.Remove(x.Length - 1);

				return x;
			}

			public static string GetAffinity()
			{
				BitArray bin = new BitArray(TaskManager.CPU.Affinity);
				byte[] data = new byte[1];
				bin.CopyTo(data, 0);
				return BitConverter.ToString(data, 0);
			}

			public static void SetPriority(string app)
			{
				var Nice = Properties.Settings.Default.Nice;

				try
				{
					Process[] Task = Process.GetProcessesByName(app);
					foreach (Process P in Task)
					{
						P.ProcessorAffinity = (IntPtr)Int32.Parse(GetAffinity(), NumberStyles.HexNumber);

						if (Nice == 0)
							P.PriorityClass = ProcessPriorityClass.RealTime;
						else if (Nice == 1)
							P.PriorityClass = ProcessPriorityClass.High;
						else if (Nice == 2)
							P.PriorityClass = ProcessPriorityClass.AboveNormal;
						else if (Nice == 3)
							P.PriorityClass = ProcessPriorityClass.Normal;
						else if (Nice == 4)
							P.PriorityClass = ProcessPriorityClass.BelowNormal;
						else if (Nice == 5)
							P.PriorityClass = ProcessPriorityClass.Idle;
						else
							P.PriorityClass = ProcessPriorityClass.Normal;
					}
				}
				catch (Exception ex)
				{
					EventLog.WriteEntry("IFME", ex.Message);
				}
			}
		}

		public class Windows
		{
			[DllImport("kernel32.dll")]
			private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, System.UInt32 dwThreadId);
			[DllImport("kernel32.dll")]
			private static extern System.UInt32 SuspendThread(IntPtr hThread);
			[DllImport("kernel32.dll")]
			private static extern System.UInt32 ResumeThread(IntPtr hThread);
			[DllImport("kernel32.dll")]
			private static extern bool CloseHandle(IntPtr hHandle);

			public static void SuspendProcess(string exe)
			{
				foreach (var item in Process.GetProcessesByName(exe))
				{
					foreach (ProcessThread t in item.Threads)
					{
						IntPtr th;
						th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, Convert.ToUInt32(t.Id));
						if ((th != IntPtr.Zero))
						{
							SuspendThread(th);
							CloseHandle(th);
						}
					}
				}
			}

			public static void ResumeProcess(string exe)
			{
				foreach (var item in Process.GetProcessesByName(exe))
				{
					foreach (ProcessThread t in item.Threads)
					{
						IntPtr th;
						th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, Convert.ToUInt32(t.Id));
						if ((th != IntPtr.Zero))
						{
							ResumeThread(th);
							CloseHandle(th);
						}
					}
				}
			}
		}

		public class Linux
		{
			public static void SuspendProcess(string exe)
			{
				foreach (var item in Process.GetProcessesByName(exe))
				{
					Run($"kill -STOP {item.Id} > /dev/null 2>&1");
				}
			}

			public static void ResumeProcess(string exe)
			{
				foreach (var item in Process.GetProcessesByName(exe))
				{
					Run($"kill -CONT {item.Id} > /dev/null 2>&1");
				}
			}
		}
	}
}
