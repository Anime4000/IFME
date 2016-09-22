using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginVideoMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public bool MultiPass { get; set; }
        public PluginVideoModeValue Value { get; set; } = new PluginVideoModeValue();
    }
}
