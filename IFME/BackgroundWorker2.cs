using System.ComponentModel;
using System.Threading;

// using custom BackgroundWorker with Abort support
// refer here: http://stackoverflow.com/questions/800767/how-to-kill-background-worker-completely

public class BackgroundWorker2 : BackgroundWorker
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
