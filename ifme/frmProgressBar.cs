using System;
using System.Threading;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmProgressBar : Form
    {
        public frmProgressBar()
        {
            InitializeComponent();

            Icon = Get.AppIcon;
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void frmProgressBar_Load(object sender, EventArgs e)
        {
            pbLoading.Style = ProgressBarStyle.Marquee;
            pbLoading.MarqueeAnimationSpeed = 25;
        }

        private void frmProgressBar_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                do
                {
                    if (pbLoading.Value == 1)
                        Invoke((MethodInvoker)delegate ()
                        {
                            pbLoading.Style = ProgressBarStyle.Continuous;
                        });

                } while (pbLoading.Value < 99);

                /*Thread.Sleep(3000);

                Invoke((MethodInvoker)delegate ()
                {
                    Close();
                });*/

            }).Start();
        }
    }
}
