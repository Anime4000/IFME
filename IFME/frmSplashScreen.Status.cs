using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IFME
{
    class VSDesignerBugFixC { }

    public partial class frmSplashScreen
    {
        private static frmSplashScreen frmSplashScreenStatus = null;
        private delegate void SetStatusUpdate(string text);
        private delegate void SetStatusUpdateAppend(string text);
        private delegate void SetStatusLogAppend(string text);

        private static List<string> Log = new List<string>();

        private void lblStatus_Update(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new SetStatusUpdate(lblStatus_Update), new object[] { value });
                return;
            }

            lblStatus.Text = value;
        }

        internal static void SetStatus(string value)
        {
            if (frmSplashScreenStatus != null)
                frmSplashScreenStatus.lblStatus_Update(value);
        }

        private void lblStatus_UpdateAppend(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new SetStatusUpdateAppend(lblStatus_UpdateAppend), new object[] { value });
                return;
            }

            lblStatus.Text += value;
        }

        private void lblStatus_LogAppend(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new SetStatusLogAppend(lblStatus_LogAppend), new object[] { value });
                return;
            }

            Log.Add(value);

            if (Log.Count > 12)
                Log.RemoveAt(0);

            lblLog.Text = string.Join("\n", Log);
        }

        internal static void SetStatusAppend(string value)
        {
            if (frmSplashScreenStatus != null)
                frmSplashScreenStatus.lblStatus_UpdateAppend(value);
        }

        internal static void PrintLogAppend(string value)
        {
            if (frmSplashScreenStatus != null)
                frmSplashScreenStatus.lblStatus_LogAppend(value);
        }
    }
}
