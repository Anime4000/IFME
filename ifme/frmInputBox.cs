using System;
using System.Drawing;
using System.Windows.Forms;

namespace ifme
{
    public partial class frmInputBox : Form
    {
        public string ReturnValue { get; private set; }

        public frmInputBox(string title, string input)
        {
            InitializeComponent();

            Text = title;
            txtInput.Text = input;
        }

        private void frmInputBox_Load(object sender, EventArgs e)
        {
            FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void txtInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK.PerformClick(); // use OK button
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ReturnValue = txtInput.Text;
            Close(); // check designer, this button has DialogResult set to OK
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close(); // check designer, this button has DialogResult set to Cancel
        }
    }
}
