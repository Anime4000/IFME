using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginAudio
    {
        public PluginCommonProperties Properties = new PluginCommonProperties();
        public PluginCommonUpdate Update = new PluginCommonUpdate();
        public PluginAudioApp App = new PluginAudioApp();
        public PluginAudioArgs Args = new PluginAudioArgs();
        public List<PluginAudioMode> Mode = new List<PluginAudioMode>();
    }
}
