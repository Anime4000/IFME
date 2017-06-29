using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            Icon = Get.AppIcon;
            Text = $"About {Get.AppName}";
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void frmAbout_Load(object sender, EventArgs e)
        {
            InitializeUX();
        }

        private void lblDonatePP_Click(object sender, EventArgs e)
        {
            Process.Start("https://paypal.me/anime4000/10");
            lblDonatePP.ForeColor = Color.Purple;
        }

        private void lblDonateBTC_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("12LWHDCPShFvYh6vxxeMsntejAUm8y8rFN");
            lblDonateBTC.ForeColor = Color.Purple;
        }

        private void lblDonateETH_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("0xAdd9ba89B601e7CB5B3602643337B9db8c90EFe0");
            lblDonateETH.ForeColor = Color.Purple;
        }
    }
}
