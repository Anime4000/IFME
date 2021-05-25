using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace IFME
{
    public partial class frmSplashScreen : Form
    {
        private BackgroundWorker2 bgThread = new BackgroundWorker2();

        public frmSplashScreen()
        {
            InitializeComponent();
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Opacity = 0;

            bgThread.DoWork += BgThread_DoWork;
            bgThread.RunWorkerCompleted += BgThread_RunWorkerCompleted;
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {

        }

        private void frmSplashScreen_Shown(object sender, EventArgs e)
        {
            bgThread.RunWorkerAsync();
        }

        private void BgThread_DoWork(object sender, DoWorkEventArgs e)
        {
            // Fade In
            while (Opacity < 1)
            {
                BeginInvoke((Action)delegate ()
                {
                    Opacity += 0.02;
                });

                Thread.Sleep(1);
            }

            // Detect user machine
            // TODO: Detect user GPU

            // Load everything
            new PluginsLoad();

            // Wait some CPU free
            Thread.Sleep(3000);

            // Fade Out
            while (Opacity > 0)
            {
                BeginInvoke((Action)delegate ()
                {
                    Opacity -= 0.02;
                });

                Thread.Sleep(1);
            }
        }
        private void BgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }
    }
}
