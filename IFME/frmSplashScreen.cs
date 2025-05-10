using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using IFME.OSManager;

namespace IFME
{
    public partial class frmSplashScreen : Form
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            WindowUtils.EnableAcrylic(this, Color.FromArgb(127, 20, 20, 20));
            base.OnHandleCreated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (OS.IsWindows)
                e.Graphics.Clear(Color.Transparent);
            else
                e.Graphics.Clear(Color.Black);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Image img = Image.FromFile(Path.Combine("Resources", "SplashScreen14.png")))
            {
                e.Graphics.DrawImage(img, new Rectangle(0, 0, Width, Height));
            }
        }

        private BackgroundWorker2 bgThread = new BackgroundWorker2();

        public frmSplashScreen()
        {
            frmSplashScreenStatus = this;
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Opacity = 0;

            bgThread.DoWork += BgThread_DoWork;
            bgThread.RunWorkerCompleted += BgThread_RunWorkerCompleted;
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            lblVersion.Text = $"Release Version {Version.Release} ({Version.CodeName})";
            lblContrib.Text = $"{Version.Contrib}\n\n{Version.TradeMark}";
        }

        private void frmSplashScreen_Shown(object sender, EventArgs e)
        {
            bgThread.RunWorkerAsync();
        }

        private void BgThread_DoWork(object sender, DoWorkEventArgs e)
        {
            lblLoadingUpdate(string.Empty);
            lblStatusUpdate(string.Empty);

            Thread.Sleep(500);

            frmFadeInOut(true);

            // Detect user machine
            // TODO: Detect user GPU
            lblLoadingUpdate("Initialising...");

            // Load settings
            if (Properties.Settings.Default.FolderOutput.IsDisable())
                Properties.Settings.Default.FolderOutput = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            

            if (Properties.Settings.Default.FolderTemporary.IsDisable())
                Properties.Settings.Default.FolderTemporary = Path.Combine(Path.GetTempPath(), "IFME");

            Properties.Settings.Default.Save();

            // Temp folder
            try
            {
                if (Directory.Exists(Properties.Settings.Default.FolderTemporary))
                    Directory.Delete(Properties.Settings.Default.FolderTemporary, true);
            }
            catch (Exception ex)
            {
                lblStatusUpdate(ex.Message);
            }

            Directory.CreateDirectory(Properties.Settings.Default.FolderTemporary);

            // Load config
            new PluginsLoad();

            // Load language
            i18n.LoadLangFiles();

            // Finished loading, clear status text
            lblStatusUpdate(string.Empty);

            Thread.Sleep(500);

            lblLoadingUpdate(string.Empty);

            // Wait some CPU free
            Thread.Sleep(100);

            // If user choose not to test the encoder, wait little longer telling user IFME not test
            if (!Properties.Settings.Default.TestEncoder)
                Thread.Sleep(1000);

            frmFadeInOut(false);
        }
        private void BgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void frmFadeInOut(bool fadeIn)
        {
            if (fadeIn)
            {
                while (Opacity < 1)
                {
                    BeginInvoke((Action)delegate ()
                    {
                        Opacity += 0.02;
                    });

                    Thread.Sleep(5);
                }
            }
            else
            {
                while (Opacity > 0)
                {
                    BeginInvoke((Action)delegate ()
                    {
                        Opacity -= 0.02;
                    });

                    Thread.Sleep(5);
                }
            }
        }

        private void lblLoadingUpdate(string value)
        {
            BeginInvoke((Action)delegate ()
            {
                lblLoading.Text = value;
            });
        }

        private void lblStatusUpdate(string value)
        {
            BeginInvoke((Action)delegate ()
            {
                lblStatus.Text = value;
            });
        }
    }
}
