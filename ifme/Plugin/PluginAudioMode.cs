using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginAudioMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public decimal[] Quality { get; set; }
        public decimal Default { get; set; }
    }
}
