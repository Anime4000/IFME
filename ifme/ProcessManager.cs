using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ifme
{
	public class ProcessManager
	{
		[Flags()]
		enum ThreadAccess : int
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

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
		[DllImport("kernel32.dll")]
		private static extern uint SuspendThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		private static extern uint ResumeThread(IntPtr hThread);
		[DllImport("kernel32.dll")]
		private static extern bool CloseHandle(IntPtr hHandle);

		static string CrProc = string.Empty;

		public static int Start(string ExePath, string Args)
		{
			CrProc = Path.GetFileNameWithoutExtension(ExePath);
			return Run($"\"{ExePath}\" {Args}", Properties.Settings.Default.TempDir);
		}

		public static int Start(string ExePath, string Args, string WorkDir)
		{
			CrProc = Path.GetFileNameWithoutExtension(ExePath);
			return Run($"\"{ExePath}\" {Args}", WorkDir);
		}

		public static int Start(string ExePath, string Args, string PipeExePath, string PipeArgs)
		{
			CrProc = Path.GetFileNameWithoutExtension(PipeExePath);
			return Run($"\"{ExePath}\" {Args} | \"{PipeExePath}\" {PipeArgs}", Properties.Settings.Default.TempDir);
		}

		private static int Run(string EnvCmd, string workDir)
		{
			var cmd = string.Empty;
			var arg = string.Empty;

			Environment.SetEnvironmentVariable("HITOHA", EnvCmd, EnvironmentVariableTarget.Process);

			if (OS.IsWindows)
			{
				cmd = "cmd";
				arg = "/c %HITOHA%";
			}
			else
			{
				cmd = "bash";
				arg = "-c 'eval $HITOHA'";
			}

			Process Proc = new Process();
			Proc.StartInfo = new ProcessStartInfo(cmd, arg)
			{
				UseShellExecute = false,
				WorkingDirectory = workDir
			};

			Proc.Start();
			Proc.WaitForExit();
			return Proc.ExitCode;
		}

		public static void Stop()
		{
			Process[] Task = Process.GetProcessesByName(CrProc);
			foreach (Process p in Task)
			{
				p.Kill();
			}
		}

		public static void Pause()
		{
			foreach (var item in Process.GetProcessesByName(CrProc))
			{
				if (OS.IsWindows)
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
				else
				{
					Process.Start("kill", $"-STOP {item.Id} > /dev/null 2>&1");
				}
			}
		}

		public static void Resume()
		{
			foreach (var item in Process.GetProcessesByName(CrProc))
			{
				if (OS.IsWindows)
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
				else
				{
					Process.Start("kill", $"-CONT {item.Id} > /dev/null 2>&1");
				}
			}
		}
	}
}
