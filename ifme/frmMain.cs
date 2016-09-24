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
            new PluginLoad();

            var video = new Dictionary<Guid, string>();
            var audio = new Dictionary<Guid, string>();

            foreach (var item in Plugin.Items)
            {
                var value = item.Value;

                if (!string.IsNullOrEmpty(value.Video.Extension))
                    video.Add(item.Key, value.Name);

                if (!string.IsNullOrEmpty(value.Audio.Extension))
                    audio.Add(item.Key, value.Name);
            }
                
            cboVideoEncoder.DataSource = new BindingSource(video, null);
            cboVideoEncoder.DisplayMember = "Value";
            cboVideoEncoder.ValueMember = "Key";
            cboVideoEncoder.SelectedValue = new Guid("deadbeef-0265-0265-0265-026502650265");

            cboAudioEncoder.DataSource = new BindingSource(audio, null);
            cboAudioEncoder.DisplayMember = "Value";
            cboAudioEncoder.ValueMember = "Key";
            cboAudioEncoder.SelectedValue = new Guid("deadbeef-eaac-eaac-eaac-eaaceaaceaac");
        }
    }
}
