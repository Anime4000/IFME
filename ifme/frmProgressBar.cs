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
            Text = "";
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void frmProgressBar_Load(object sender, EventArgs e)
        {

        }

        private void frmProgressBar_Shown(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                do
                {

                } while (pbLoading.Value < 98);

                Thread.Sleep(1000);

                Invoke((MethodInvoker)delegate ()
                {
                    Close();
                });
           
            }).Start();
        }
    }
}
