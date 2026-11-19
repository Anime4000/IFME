using System;
using System.Management;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

internal partial class API
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

	internal static class WindowsProcess
	{
		/// <summary>
		/// Reads the whole process table in a single WMI query, including creation time so
		/// stale parent links (PID reuse) can be detected.
		/// </summary>
		internal static List<ProcessEx.ProcessEntry> Snapshot()
		{
			var list = new List<ProcessEx.ProcessEntry>();

			using (var searcher = new ManagementObjectSearcher("SELECT ProcessId, ParentProcessId, CreationDate FROM Win32_Process"))
			using (var processList = searcher.Get())
			{
				foreach (ManagementObject item in processList)
				{
					using (item)
					{
						long start = 0;

						try
						{
							if (item["CreationDate"] is string created && created.Length > 0)
								start = ManagementDateTimeConverter.ToDateTime(created).ToUniversalTime().Ticks;
						}
						catch
						{
							// Leave unknown
						}

						list.Add(new ProcessEx.ProcessEntry
						{
							Id = Convert.ToInt32(item["ProcessId"]),
							ParentId = Convert.ToInt32(item["ParentProcessId"]),
							StartTime = start
						});
					}
				}
			}

			return list;
		}

		internal static void Suspend(int[] pids)
		{
			ForEachThread(pids, th => SuspendThread(th));
		}

		internal static void Resume(int[] pids)
		{
			ForEachThread(pids, th => ResumeThread(th));
		}

		private static void ForEachThread(int[] pids, Action<IntPtr> action)
		{
			foreach (var pid in pids)
			{
				try
				{
					using (var proc = Process.GetProcessById(pid))
					{
						foreach (ProcessThread t in proc.Threads)
						{
							IntPtr th = OpenThread(ThreadAccess.SUSPEND_RESUME, false, Convert.ToUInt32(t.Id));

							if (th != IntPtr.Zero)
							{
								action(th);
								CloseHandle(th);
							}
						}
					}
				}
				catch
				{
					// Process exited between the snapshot and now.
				}
			}
		}
	}
}
