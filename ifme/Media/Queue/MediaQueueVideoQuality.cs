using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueVideoQuality
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsVFR { get; set; }
        public float FrameRate { get; set; }
        public float FrameRateAvg { get; set; }
        public int FrameCount { get; set; }
        public int BitDepth { get; set; }
        public int PixelFormat { get; set; }
        public string Command { get; set; }
    }
}
