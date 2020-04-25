using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class PluginsAudio : PluginsCommon
    {
        public PluginsAudioProp Audio { get; set; } = new PluginsAudioProp();
    }

    public class PluginsAudioProp
    {
        public string Extension { get; set; }
        public string Encoder { get; set; }
        public int[] SampleRate { get; set; }
        public int SampleRateDefault { get; set; }
        public int[] Channel { get; set; }
        public int ChannelDefault { get; set; }
        public PluginsAudioArgs Args { get; set; } = new PluginsAudioArgs();
        public List<PluginsAudioMode> Mode { get; set; } = new List<PluginsAudioMode>();
    }

    public class PluginsAudioArgs
    {
        public bool Pipe { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Command { get; set; }
    }

    public class PluginsAudioMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public decimal[] Quality { get; set; }
        public string QualityPrefix { get; set; } = string.Empty;
        public string QualityPostfix { get; set; } = string.Empty;
        public decimal Default { get; set; }
    }
}
