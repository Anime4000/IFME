using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ifme
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            FormBorderStyle = FormBorderStyle.Sizable;
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Load default
            cboVideoResolution.Text = "1920x1080";
            cboVideoFrameRate.Text = "23.976";
            cboVideoPixelFormat.SelectedIndex = 0;
            cboVideoDeinterlaceMode.SelectedIndex = 1;
            cboVideoDeinterlaceField.SelectedIndex = 0;

            // Load plugin
            PluginTest.JsonRead();
        }
    }
}
