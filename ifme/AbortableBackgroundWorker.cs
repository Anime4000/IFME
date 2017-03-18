using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace ifme
{
	public class AbortableBackgroundWorker : BackgroundWorker
	{
		private Thread WorkerThread;

		protected override void OnDoWork(DoWorkEventArgs e)
		{
			WorkerThread = Thread.CurrentThread;

			try
			{
				base.OnDoWork(e);
			}
			catch (ThreadAbortException)
			{
				e.Cancel = true; //We must set Cancel property to true!
				Thread.ResetAbort(); //Prevents ThreadAbortException propagation
			}
		}

		public void Abort()
		{
			if (WorkerThread != null)
			{
				WorkerThread.Abort();
				WorkerThread = null;
			}
		}
	}
}
