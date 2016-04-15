using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginVideo
    {
        public string Name = string.Empty;
        public PluginCommonProperties Properties = new PluginCommonProperties();
        public PluginCommonUpdate Update = new PluginCommonUpdate();
        public PluginVideoApp App = new PluginVideoApp();
        public PluginVideoArgs Args = new PluginVideoArgs();
        public List<PluginVideoMode> Mode = new List<PluginVideoMode>();
    }
}
