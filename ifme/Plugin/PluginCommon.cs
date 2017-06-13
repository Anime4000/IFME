using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginCommon
    {
        public Guid GUID { get; set; }
        public string FilePath { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public bool X64 { get; set; }
        public string[] Format { get; set; }
        public PluginAuthor Author { get; set; } = new PluginAuthor();
        public PluginUpdate Update { get; set; } = new PluginUpdate();
        public PluginAudio Audio { get; set; } = new PluginAudio();
        public PluginVideo Video { get; set; } = new PluginVideo();
    }
}
