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
            FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void frmProgressBar_Load(object sender, EventArgs e)
        {
            pbLoading.Style = ProgressBarStyle.Continuous;
            pbLoading.MarqueeAnimationSpeed = 5;

#if SAVE_LANG
            i18n.Save(this, Name);
#else
            i18n.Apply(this, Name, Properties.Settings.Default.UILanguage);
#endif
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public string Title
        {
            get { return Text; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => Text = value));
                }
                else
                {
                    Text = value;
                }
            }
        }

        public string Status
        {
            get { return lblStatus.Text; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => lblStatus.Text = value));
                }
                else
                {
                    lblStatus.Text = value;
                }
            }
        }

        public int Progress
        {
            get { return pbLoading.Value; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => pbLoading.Value = value));
                }
                else
                {
                    pbLoading.Value = value;
                }
            }
        }

        public ProgressBarStyle ProgBarStyle
        {
            get { return pbLoading.Style; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => pbLoading.Style = value));
                }
                else
                {
                    pbLoading.Style = value;
                }
            }
        }
    }
}
