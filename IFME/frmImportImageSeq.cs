using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IFME
{
    public partial class frmImportImageSeq : Form
    {
        public string FilePath { get; set; }
        public string FrameRate { get; set; }

        public frmImportImageSeq(string filePath)
        {
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            FormBorderStyle = FormBorderStyle.Sizable;

            lblSourceFile.Text = filePath;
            lblSourceFileParse.Text = Images.GetImageSeq(filePath);
        }

        private void frmImportImageSeq_Load(object sender, EventArgs e)
        {
#if SAVE_LANG
            i18n.Save(this, Name);
#else
            i18n.Apply(this, Name, Properties.Settings.Default.UILanguage);
#endif
        }

        private void frmImportImageSeq_Shown(object sender, EventArgs e)
        {
            try
            {
                using (var bmp = new Bitmap(lblSourceFile.Text))
                {
                    txtResolution.Text = $"{bmp.Width}x{bmp.Height}";
                    txtPixelFormat.Text = Images.GetImagePixelFormat(bmp.PixelFormat);
                    txtBitDepth.Text = Images.GetImageBitDepth(bmp.PixelFormat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Broken Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FilePath = lblSourceFileParse.Text;
            FrameRate = txtFrameRate.Text;

            Close();
        }
    }
}
