using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class PluginsVideo : PluginsCommon
    {
        public PluginsVideoProp Video { get; set; } = new PluginsVideoProp();
    }

    public class PluginsVideoProp
    {
        public bool RawOutput { get; set; }
        public string Extension { get; set; }
        public List<PluginsVideoEncoder> Encoder { get; set; } = new List<PluginsVideoEncoder>();
        public List<PluginsVideoChroma> Chroma { get; set; } = new List<PluginsVideoChroma>();
        public string[] Preset { get; set; }
        public string PresetDefault { get; set; }
        public string[] Tune { get; set; }
        public string TuneDefault { get; set; }
        public string[] Resolution { get; set; }
        public int ResolutionDefault { get; set; } = -1;
        public PluginsVideoArgs Args { get; set; } = new PluginsVideoArgs();
        public List<PluginsVideoMode> Mode { get; set; } = new List<PluginsVideoMode>();
    }

    public class PluginsVideoEncoder
    {
        public int BitDepth { get; set; }
        public string Binary { get; set; }
    }

    public class PluginsVideoChroma
    {
        public int Value { get; set; }
        public string Command { get; set; }
    }

    public class PluginsVideoArgs
    {
        public bool Pipe { get; set; }
        public string UnPipe { get; set; }
        public string Y4M { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public string Resolution { get; set; }
        public string FrameRate { get; set; }
        public string BitDepthIn { get; set; }
        public string BitDepthOut { get; set; }
        public string FrameCount { get; set; }
        public string PassFirst { get; set; }
        public string PassLast { get; set; }
        public string PassNth { get; set; }
        public string Command { get; set; }
    }

    public class PluginsVideoMode
    {
        public string Name { get; set; }
        public string Args { get; set; }
        public bool MultiPass { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public string Postfix { get; set; } = string.Empty;
        public PluginsVideoModeValue Value { get; set; } = new PluginsVideoModeValue();
    }

    public class PluginsVideoModeValue
    {
        public int DecimalPlace { get; set; }
        public decimal Step { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public decimal Default { get; set; }
    }
}
