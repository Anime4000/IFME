using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IFME
{
    internal class TimespanTextBox : TextBox
    {
        private string lastValidText = "";
        public TimeSpan? MaxValue { get; set; } = null;

        public TimeSpan TimeSpan
        {
            get
            {
                TimeSpan.TryParse(this.Text, out var timeStart);
                return timeStart;
            }
            set
            {
                setValueTimespan(value);
            }
        }

        public TimespanTextBox()
        {
            this.KeyPress += new KeyPressEventHandler(this.textBox_KeyPress);
            this.TextChanged += new EventHandler(this.textBox_Validating);
        }

        public TimespanTextBox(TimeSpan maxValue) : base()
        {
            this.MaxValue = maxValue;
        }

        public void setValueSeconds(float seconds)
        {
            setValueTimespan(TimeSpan.FromSeconds((double)(new decimal(seconds))));
        }

        private void setValueTimespan(TimeSpan timespan)
        {
            string sign = (timespan.TotalMilliseconds < 0) ? "-" : "";
            this.Text = $"{sign}{timespan:hh\\:mm\\:ss\\.fff}";
        }

        private void textBox_Validating(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.Text) || this.Text.Equals("0"))
                this.Text = "00:00:00.000";

            var rTime = new Regex(@"-?[0-9][0-9]\:[0-6][0-9]\:[0-6][0-9]\.[0-9]{1,3}");

            if (!rTime.IsMatch(this.Text))
            {
                MessageBox.Show("Please provide the time in hh:mm:ss.xxx format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = lastValidText;
            }
            else if (this.MaxValue != null && this.TimeSpan > this.MaxValue)
            {
                MessageBox.Show("Time set exceeds file duration", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Text = lastValidText;
            }
            else
            {
                lastValidText = this.Text;
            }

            if (this.Text.Contains("-"))
            {
                this.BackColor = Color.PaleVioletRed;
                this.ForeColor = Color.DarkRed;
            } else
            {
                this.BackColor = TextBox.DefaultBackColor;
                this.ForeColor = TextBox.DefaultForeColor;
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || (e.KeyChar.ToString() == ":") || (e.KeyChar.ToString() == "."))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
