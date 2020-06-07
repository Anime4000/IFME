using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    class VSDesignerBugFixB { }

    public partial class frmMain
    {
        private static frmMain frmMainStatus = null;
        private delegate void rtfConsoleAppendText(string value);
        private delegate void lstFileProgressText(string value);
        private delegate void lstFileStatusText(string value);

        private void rtfConsole_AppendText(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new rtfConsoleAppendText(rtfConsole_AppendText), new object[] { value });
                return;
            }

            rtfConsole.AppendText(value);
            rtfConsole.ScrollToCaret();
        }

        public static void PrintLog(string value)
        {
            if (frmMainStatus != null)
                frmMainStatus.rtfConsole_AppendText(value + Environment.NewLine);
        }

        private void lstFile_ProgressText(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new lstFileProgressText(lstFile_ProgressText), new object[] { value });
                return;
            }

            lstFile.Items[MediaEncoding.CurrentIndex].SubItems[5].Text = value;
        }

        public static void PrintProgress(string value)
        {
            if (frmMainStatus != null)
                frmMainStatus.lstFile_ProgressText(value);
        }

        private void lstFile_StatusText(string value)
        {
            if (InvokeRequired)
            {
                Invoke(new lstFileStatusText(lstFile_StatusText), new object[] { value });
                return;
            }

            lstFile.Items[MediaEncoding.CurrentIndex].SubItems[4].Text = value;
        }

        public static void PrintStatus(string value)
        {
            if (frmMainStatus != null)
                frmMainStatus.lstFile_StatusText(value);
        }
    }
}
