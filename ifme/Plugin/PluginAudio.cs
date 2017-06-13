using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginAudio
    {
        public string Extension { get; set; }
        public string Encoder { get; set; }
        public int[] SampleRate { get; set; }
        public int SampleRateDefault { get; set; }
        public int[] Channel { get; set; }
        public int ChannelDefault { get; set; }
        public PluginAudioArgs Args { get; set; } = new PluginAudioArgs();
        public List<PluginAudioMode> Mode { get; set; } = new List<PluginAudioMode>();
    }
}
