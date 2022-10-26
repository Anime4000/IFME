using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    public partial class frmProgressBar : Form
    {
        public frmProgressBar()
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Text = string.Empty;
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void frmProgressBar_Load(object sender, EventArgs e)
        {
            pbLoading.Style = ProgressBarStyle.Marquee;
            pbLoading.MarqueeAnimationSpeed = 20;
        }

        private void frmProgressBar_Shown(object sender, EventArgs e)
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
