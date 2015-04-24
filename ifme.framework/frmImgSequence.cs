using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ifme.framework
{
	public partial class frmImgSequence : Form
	{
		public string script { get; set; }

		public frmImgSequence()
		{
			InitializeComponent();
		}

		private void frmImgSequence_Load(object sender, EventArgs e)
		{
			this.Icon = Properties.Resources.Frame;
			cboPixFmt.SelectedIndex = cboPixFmt.Items.Count - 1;
		}

		private void frmImgSequence_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			lstImages.Items.Clear();

			lblCount.Text = String.Format("Total: {0}", lstImages.Items.Count);
			btnAdd.Enabled = true;
			btnClear.Enabled = false;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog GetFiles = new OpenFileDialog();
			GetFiles.Title = "Select image sequence";
			GetFiles.Filter = "Supported image files|*.bmp;*.dib;*.tif;*.tiff;*.jpg;*.jpeg;*.jpe;*.jfif;*.png;*.gif|"
				+ "Bitmap (Lossless)|*.bmp;*.dib|"
				+ "Tagged Image File Format|*.tif;*.tiff|"
				+ "Joint Photographic Experts Group|*.jpg;*.jpeg;*.jpe;*.jfif|"
				+ "Portable Network Graphics|*.png|"
				+ "Graphics Interchange Format|*.gif|"
				+ "All Files|*.*";
			GetFiles.FilterIndex = 1;
			GetFiles.Multiselect = true;

			if (GetFiles.ShowDialog() == DialogResult.OK)
			{
				foreach (var item in GetFiles.FileNames)
				{
					string[] data = { Path.GetFileName(item), item };

					ListViewItem DataList = new ListViewItem(data[0]);
					DataList.SubItems.Add(data[1]);
					lstImages.Items.Add(DataList);
				}

				lblCount.Text = String.Format("Total: {0}", lstImages.Items.Count);
				btnAdd.Enabled = false;
				btnClear.Enabled = true;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			var filename = lstImages.Items[0].SubItems[0].Text;
			var location = Path.GetDirectoryName(lstImages.Items[0].SubItems[1].Text);

			var filescript = Path.GetFileNameWithoutExtension(filename) + ".ifs";
			var destscript = Path.Combine(location, filescript);

			// Check file name see how many zero
			int test = 0;
			var zero = "";

			foreach (var item in filename)
			{
				if (int.TryParse(char.ToString(item), out test))
				{
					zero += item;
				}
			}

			// make %d
			var gen = String.Format("%0{0}d", zero.Length);
			var seq = filename.Replace(zero, gen);
			var fin = Path.Combine(location, seq);

			// Add
			var parser = new FileIniDataParser();
			IniData data = new IniData();

			data.Sections.AddSection("script");
			data["script"].AddKey("type", "image");
			data["script"].AddKey("samp", lstImages.Items[0].SubItems[1].Text);
			data.Sections.AddSection("image");
			data["image"].AddKey("input", fin);
			data["image"].AddKey("start", int.Parse(zero).ToString());
			data["image"].AddKey("end", lstImages.Items.Count.ToString());
			data["image"].AddKey("fps", cboFps.Text);
			data["image"].AddKey("pix", cboPixFmt.Text);

			parser.WriteFile(destscript, data, Encoding.UTF8);

			script = destscript;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
	}
}
