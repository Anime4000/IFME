using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IFME.OSManager;

namespace IFME
{
	internal class ProcessManager
	{
		// Guarded by SyncRoot. Stop/Pause/Resume iterate a snapshot so they cannot
		// throw InvalidOperationException when Run() adds or removes concurrently.
		private static readonly HashSet<int> ProcessId = new HashSet<int>();
		private static readonly object SyncRoot = new object();

		private static CancellationTokenSource cancelSource = new CancellationTokenSource();

		internal static bool IsPause = false;

		/// <summary>
		/// Cooperative cancellation for the current encoding job. Replaces Thread.Abort:
		/// the pipeline checks this between stages and Run() kills the running child process.
		/// </summary>
		internal static CancellationToken Token => cancelSource.Token;

		internal static bool IsCancelled => cancelSource.IsCancellationRequested;

		private TimeSpan eta = new TimeSpan(0, 0, 0);
		private List<double> recentFps;
		private const int sampleSize = 5;

		public ProcessManager()
		{
			recentFps = new List<double>();
		}

		internal static int Start(string Command)
		{
			return new ProcessManager().Run(Command, string.Empty);
		}

		internal static int Start(string WorkingDirectory, string Command)
		{
			return new ProcessManager().Run(Command, WorkingDirectory);
		}

		private int Run(string Command, string WorkingDirectory)
		{
			// Don't launch anything else once the user has asked to stop.
			Token.ThrowIfCancellationRequested();

			// replace double or more space with single space except in quote char (" ' `)
			Command = Regex.Replace(Command, "\\s{2,}(?=(?:[^'\"`]*(['\"`])[^'\"`]*\\1)*[^'\"`]*$)", " ");

			// The command lives in the child's own environment block rather than IFME's, so two
			// jobs launched at the same moment can no longer overwrite each other's command.
			var startInfo = OS.ShellStartInfo(Command, WorkingDirectory);
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardOutput = true;

			using (Process proc = new Process { StartInfo = startInfo })
			{
#if DEBUG
				Report.Log($"[DEBG] Command Line: {Command}");
#endif

				proc.OutputDataReceived += Proc_DataReceived;
				proc.ErrorDataReceived += Proc_DataReceived;

				proc.Start();

				lock (SyncRoot)
					ProcessId.Add(proc.Id);

				try
				{
					proc.BeginOutputReadLine();
					proc.BeginErrorReadLine();

					// Poll instead of a blocking WaitForExit() so cancellation is observed
					// promptly without aborting the thread.
					while (!proc.WaitForExit(200))
					{
						if (Token.IsCancellationRequested)
						{
							TryKill(proc);
							break;
						}
					}

					// Parameterless overload blocks until the async stdout/stderr
					// handlers have drained, so no output is lost.
					proc.WaitForExit();
				}
				finally
				{
					lock (SyncRoot)
						ProcessId.Remove(proc.Id);
				}

				Token.ThrowIfCancellationRequested();

				return proc.ExitCode;
			}
		}

		private static void TryKill(Process proc)
		{
			try
			{
				if (!proc.HasExited)
					ProcessEx.Terminate(proc.Id);
			}
			catch (Exception ex)
			{
				Report.Log($"[WARN] Unable to terminate process {proc.Id}: {ex.Message}");
			}
		}

		private void Proc_DataReceived(object sender, DataReceivedEventArgs e)
		{
			// Nothing is listening (e.g. during plugin self-test at startup): skip the
			// regex work entirely rather than parsing lines that will be discarded.
			if (!Report.HasSink)
				return;

			if (!string.IsNullOrEmpty(e.Data))
			{
                var tf = @"(?<=encoded\s) ?\d+(?=> frames in \d+.\d+)?"; //x265 encoded total frame
                var tfm = Regex.Matches(e.Data, tf, RegexOptions.IgnoreCase);
                if (tfm.Count > 0)
                {
                    // Only overwrite the count when the encoder actually reported a usable
                    // number; a failed parse used to clobber the value computed in
                    // MediaEncoding with 0, which made the progress percentage divide by zero.
                    if (int.TryParse(tfm[0].Value.Trim(), out int rfc) && rfc > 0)
                        MediaEncoding.RealFrameCount = rfc;

                    return;
                }

                var regexPattern = @"( \d+ bits )|( \d+ seconds)|(\d+/\d{3})|(size=[ ]{1,}\d+)|(frame[ ]{1,}\d+)|(\d+.\d+[ ]{1,}kb/s)|(\d+.\d+[ ]{1,}fps)|(\d+[ ]{1,}frames:\s\d+.\d+[ ]{1,}fps,\s\d+.\d+[ ]{1,}kb/s,\sGPU\s\d+%,\sVE\s\d+%)";
                Match m = Regex.Match(e.Data, regexPattern, RegexOptions.IgnoreCase);
                if (m.Success)
                    Report.Progress(e.Data);
                else
                    Report.Log(e.Data);

                var patterns = new[]
				{
                    @"vvenc \[info\]: stats:  frame=\s*(\d+) .* avg_fps=\s*([\d\.]+) .* avg_bitrate=\s*([\d\.]+) kbps", // Fraunhofer VVC
					@"\[\d+\.\d+%\] (\d+)/\d+ frames, ([\d\.]+) fps, ([\d\.]+) kb/s", // x264 & x265
					@"(\d+) frames: ([\d\.]+) fps, ([\d\.]+) kb/s", // Rigaya NVEnc
					@"frame=\s*(\d+) fps=\s*([\d\.]+) .* bitrate=\s*([\d\.]+)kbits/s", // FFmpeg
					@"Encoding frame\s*(\d+)\s* ([\d\.]+) kbps\s* ([\d\.]+) fps" // SVT-AV1
                };

				foreach (var pattern in patterns)
				{
                    var match = Regex.Match(e.Data, pattern);
                    if (match.Success)
                    {
						int frame;
						double bitrate, speed;

						int.TryParse(match.Groups[1].Value, out int a);
						double.TryParse(match.Groups[2].Value, out double b);
						double.TryParse(match.Groups[3].Value, out double c);

						frame = a;

						if (pattern.EndsWith("fps")) // SVT-AV1 position fps last compared with others encoder
						{
							bitrate = b;
							speed = c;
						}
						else
						{
                            bitrate = c;
                            speed = b;
                        }

                        // Frame count is unknown for multi-pass and some sources; without
                        // this guard the division produced Infinity and a garbage ETA.
                        var totalFrames = MediaEncoding.RealFrameCount;
                        var hasTotal = totalFrames > 0;

                        double percentage = hasTotal ? (double)frame / totalFrames * 100 : 0;

                        if (percentage > 100)
                            percentage = 100; // Cap percentage at 100%

                        // Update recent fps list
                        recentFps.Add(speed);
                        if (recentFps.Count > sampleSize)
                        {
                            recentFps.RemoveAt(0);
                        }

                        // Calculate ETA
                        if (hasTotal && recentFps.Count == sampleSize)
                        {
                            double averageFps = 0;
                            foreach (var fps in recentFps)
                            {
                                averageFps += fps;
                            }
                            averageFps /= sampleSize;
                            averageFps = Math.Round(averageFps); // Round to the nearest whole number

                            if (averageFps > 0)
                            {
                                int remainingFrames = totalFrames - frame;

                                if (remainingFrames < 0)
                                    remainingFrames = 0;

                                double remainingTime = remainingFrames / averageFps;

                                if (remainingTime > TimeSpan.MaxValue.TotalSeconds)
                                {
                                    remainingTime = TimeSpan.MaxValue.TotalSeconds;
                                }
                                else if (remainingTime < TimeSpan.MinValue.TotalSeconds)
                                {
                                    remainingTime = TimeSpan.MinValue.TotalSeconds;
                                }

                                try
                                {
                                    eta = TimeSpan.FromSeconds(remainingTime);
                                }
                                catch (Exception ex)
                                {
                                    Report.Log($"[WARN] ETA Logic is crashed: {ex.Message}");

                                }
                            }
                        }

                        if (hasTotal)
                            Report.Progress($"[{percentage:0.00} %] Frame: {frame}, Bitrate: {bitrate:0} kb/s, Speed: {speed:0.00} fps, ETA: {eta:hh\\:mm\\:ss}");
                        else
                            Report.Progress($"Frame: {frame}, Bitrate: {bitrate:0} kb/s, Speed: {speed:0.00} fps");

                        return;
                    }
                }
			}
		}

		/// <summary>
		/// Begin a fresh job: clears stale process ids and arms a new cancellation token.
		/// </summary>
		internal static void Reset()
		{
			lock (SyncRoot)
				ProcessId.Clear();

			// Deliberately not disposing the previous source: a worker from the last job may
			// still be unwinding and reading its token, and Token throws once disposed.
			// An unregistered CancellationTokenSource is cheap and collectable.
			Interlocked.Exchange(ref cancelSource, new CancellationTokenSource());

			IsPause = false;
		}

		internal static void Clear()
		{
			lock (SyncRoot)
				ProcessId.Clear();
		}

		internal static void Stop()
		{
			cancelSource.Cancel();

			foreach (var pid in Snapshot())
			{
				try
				{
					ProcessEx.Terminate(pid);
				}
				catch (Exception ex)
				{
					Report.Log($"[WARN] Unable to terminate process {pid}: {ex.Message}");
				}
			}
		}

		internal static void Pause()
		{
			foreach (var pid in Snapshot())
			{
				ProcessEx.Pause(pid);
			}

			IsPause = true;
		}

		internal static void Resume()
		{
			foreach (var pid in Snapshot())
			{
				ProcessEx.Resume(pid);
			}

			IsPause = false;
		}

		private static int[] Snapshot()
		{
			lock (SyncRoot)
			{
				var copy = new int[ProcessId.Count];
				ProcessId.CopyTo(copy);
				return copy;
			}
		}

		internal static void Donate()
		{
			OS.OpenUrl("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=4CKYN7X3DGA7U");
		}
	}
}
