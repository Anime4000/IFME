using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginAudioApp
    {
        public string Ext;
        public string Exe;
        public string[] SampleRate;
        public int SampleRateDefault = 44100;
        public string[] Channel;
        public int ChannelDefault = 0;
        public int Mode = 0;
    }
}
