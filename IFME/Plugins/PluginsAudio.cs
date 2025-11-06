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
        public string SampleRateArgs { get; set; } = "-ar";
        public int SampleRateDefault { get; set; }
        public SortedList<int, string> Channels { get; set; }
        public string ChannelArgs { get; set; } = "-ac";
        public int ChannelDefault { get; set; }
        public PluginsAudioArgs Args { get; set; } = new PluginsAudioArgs();
        public List<PluginsAudioMode> Mode { get; set; } = new List<PluginsAudioMode>();
    }

    public class PluginsAudioArgs
    {
        public bool Pipe { get; set; }
        public string Input { get; set; }
        public string Codec { get; set; }
        public string Output { get; set; }
        public string Command { get; set; }
    }

    public class PluginsAudioMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public bool MultiChannelSupport { get; set; } = true;
        public bool SingleChannelOnly { get; set; } = false;
        public bool MonoSupport { get; set; } = true;
        public string[] Quality { get; set; }
        public string QualityPrefix { get; set; } = string.Empty;
        public string QualityPostfix { get; set; } = string.Empty;
        public string Default { get; set; }
    }
}
