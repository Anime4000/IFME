using System.Collections.Generic;

namespace ifme
{
    public class PluginVideo
    {
        public string Extension { get; set; }
        public List<PluginVideoEncoder> Encoder { get; set; } = new List<PluginVideoEncoder>();
        public string[] Preset { get; set; }
        public string PresetDefault { get; set; }
        public string[] Tune { get; set; }
        public string TuneDefault { get; set; }
        public PluginVideoArgs Args { get; set; } = new PluginVideoArgs();
        public List<PluginVideoMode> Mode { get; set; } = new List<PluginVideoMode>();
    }
}
