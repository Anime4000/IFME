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

	internal class WindowsProcess
	{
		internal int[] ListParentChild(int value)
		{
			var plist = new List<int>();
			var query = $"Select * From Win32_Process Where ParentProcessId = {value}";
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
			ManagementObjectCollection processList = searcher.Get();

			foreach (ManagementObject item in processList)
			{
				plist.Add(int.Parse(item["ProcessId"].ToString()));
			}

			return plist.ToArray();
		}

		internal void PauseParentChild(int value)
		{
			foreach (var pid in ListParentChild(value))
			{
				var id = Process.GetProcessById(pid);

				foreach (ProcessThread t in id.Threads)
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

		internal void ResumeParentChild(int value)
		{
			foreach (var pid in ListParentChild(value))
			{
				var id = Process.GetProcessById(pid);

				foreach (ProcessThread t in id.Threads)
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
}