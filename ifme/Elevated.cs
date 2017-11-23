using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace ifme
{
    internal class Elevated
    {
        private static bool IsElevated { get; set; }

        internal static bool IsAdmin
        {
            get
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);
                    IsElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
                }

                return IsElevated;
            }
        }

        internal static void RunAsAdmin()
        {
            if (IsAdmin == false)
            {
                var startInfo = new ProcessStartInfo(Get.AppPath)
                {
                    Verb = "runas"
                };

                Process.Start(startInfo);

                Application.Exit();

                return;
            }
        }
    }
}
