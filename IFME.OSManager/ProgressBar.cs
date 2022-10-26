using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;

namespace IFME.OSManager
{
    public partial class ProgressBar : Form
    {
        public ProgressBar()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void ProgressBar_Load(object sender, EventArgs e)
        {
            pbLoading.Style = ProgressBarStyle.Marquee;
            pbLoading.MarqueeAnimationSpeed = 20;
        }

        private void ProgressBar_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                do
                {
                    if (pbLoading.Style == ProgressBarStyle.Marquee)
                        if (pbLoading.Value > 0)
                            Invoke((MethodInvoker)delegate ()
                            { pbLoading.Style = ProgressBarStyle.Continuous; });

                } while (pbLoading.Value < 99);

                /*Thread.Sleep(3000);

                Invoke((MethodInvoker)delegate ()
                {
                    Close();
                });*/

            }).Start();
        }

        public string Status
        {
            get { return lblStatus.Text; }
            set { lblStatus.Text = value; }
        }

        public int Progress
        {
            get { return pbLoading.Value; }
            set { try { pbLoading.Value = value; } catch { } }
        }
    }
}
