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

        internal static void SetStatusAppend(string value)
        {
            if (frmSplashScreenStatus != null)
                frmSplashScreenStatus.lblStatus_UpdateAppend(value);
        }
    }
}
