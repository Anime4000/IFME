using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class PluginVideoArgs
    {
        public bool Pipe { get; set; }
        public string Y4M { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public string BitDepth { get; set; }
        public string FrameCount { get; set; }
        public string PassFirst { get; set; }
        public string PassLast { get; set; }
        public string PassNth { get; set; }
    }
}
