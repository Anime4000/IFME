using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MediaInfoDotNet;

namespace ifme.framework
{
	public partial class frmAviSynthHFR : Form
	{
		string video { get; set; }
		string folder { get; set; }
		string filename { get; set; }
		string fileexts { get; set; }
		float fps { get; set; }
		public string script { get; set; }
		string currentdir { set; get; }

		public frmAviSynthHFR(string file)
		{
			InitializeComponent();
			this.Icon = Properties.Resources.Frame;

			cboInputType.SelectedIndex = 0;
			cboPreset.SelectedIndex = 0;
			cboTuning.SelectedIndex = 0;

			video = file;
			folder = Path.GetDirectoryName(file);
			filename = Path.GetFileNameWithoutExtension(file);
			fileexts = Path.GetExtension(file);
			currentdir = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
		}

		private void frmAviSynthHFR_Load(object sender, EventArgs e)
		{
			MediaFile AviFile = new MediaFile(video);
			fps = AviFile.Video[0].frameRate;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			script = Path.Combine(folder, filename + ".avs");
			string[] Data = new string[10];

			Data[0] = String.Format("{0}", Environment.ProcessorCount);
			Data[1] = Path.Combine(currentdir, "tools", "avisynth_plugin");
			Data[2] = video;
			Data[3] = String.Format("{0}", fps);
			Data[4] = String.Format("{0},{1}", fpsRatio(fps));
			Data[5] = cboPreset.Text;
			Data[6] = cboTuning.Text;
			Data[7] = chkDoubleFps.Checked ? "true" : "false";
			Data[8] = chkUseGPU.Checked ? "true" : "false";
			Data[9] = cboInputType.Text;

			string generated = String.Format(Properties.Resources.AviSynthHFR, Data);
			File.WriteAllText(script, generated);

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void cboInputType_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cboInputType.SelectedIndex;
			switch (i)
			{
				case 0:
					lblInputTypeInfo.Text = "This is for regular videos.";
					break;
				case 1:
					lblInputTypeInfo.Text = "This is for 3D Side-By-Side content, usually the resolution will be 3840x1080 (1920x1080 for each eye).";
					break;
				case 2:
					lblInputTypeInfo.Text = "This is for 3D Over-Under content (AKA Top-to-Bottom), usually the resolution will be 1920x2160 (1920x1080 for each eye).";
					break;
				case 3:
					lblInputTypeInfo.Text = "This is for 3D Side-By-Side content at half the resolution, usually the resolution will be 1920x1080 (960x1080 for each eye).";
					break;
				case 4:
					lblInputTypeInfo.Text = "This is for 3D Over-Under content (AKA Top-to-Bottom), usually the resolution will be 1920x1080 (1920x540 for each eye).";
					break;
				default:
					break;
			}
			lblInputTypeInfo.Text += "\n\nNote: This setting does not create 3D from 2D, it just properly interpolates content that is already 3D. The 3D options approximately halve the speed - because they calculate each half separately - so if you are OK with less 3D accuracy you can use the 2D (default) mode for faster performance.";
		}

		private void cboPreset_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cboPreset.SelectedIndex;
			switch (i)
			{
				case 0:
					lblPresetInfo.Text = "Very good quality (Recommended).";
					break;
				case 1:
					lblPresetInfo.Text = "Faster than Medium but slightly lower quality.";
					break;
				case 2:
					lblPresetInfo.Text = "Faster than Fast but lower quality.";
					break;
				default:
					break;
			}
		}

		private void cboTuning_SelectedIndexChanged(object sender, EventArgs e)
		{
			int i = cboTuning.SelectedIndex;
			switch (i)
			{
				case 0:
					lblTuningInfo.Text = "This offers a good balance between the accuracy of individual moving things and the cohesiveness of the frame. Useful for content that was filmed or rendered (Recommended).";
					break;
				case 1:
					lblTuningInfo.Text = "This should be used for cartoons/anime.";
					break;
				case 2:
					lblTuningInfo.Text = "This increases the accuracy of individual moving things while decreasing the cohesiveness of the frame. Some people prefer it since it gives the motion an overall \"smooth\" feeling.";
					break;
				case 3:
					lblTuningInfo.Text = "This decreases the accuracy of individual moving things while increasing the cohesiveness of the frame.";
					break;
				default:
					break;
			}
			lblTuningInfo.Text += "\n\nNote: This will weaken the interpolation a lot, meaning the motion isn't as smooth. Some people will prefer to use the Film tuning even for animated content, so don't automatically assume this is the right tuning for you; use with caution.";
		}

		private string[] fpsRatio(float given)
		{
			// Help me solve this algorithm :)
			// Converting decimal into ratio, example: 23.976 = 24000/1001
			int a = 0;
			int b = 1;

			string input = given.ToString();
			string[] ab = input.Split('.');

			a = Int32.Parse(ab[0]);

			if (ab.Length > 1)
				if (b != 0)
					b = Int32.Parse(ab[1]);

			if (b != 0)
			{
				if (b > 10)
				{
					a = (a + 1) * 1000;
					b = 1001;
				}
			}
			return new[] { a.ToString(), b.ToString() };
		}
	}
}
