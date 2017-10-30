using System;
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
    }
}
