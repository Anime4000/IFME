using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public partial class MediaQueueVideo : MediaQueueCommon
    {
        public MediaQueueVideoEncoder Encoder { get; set; } = new MediaQueueVideoEncoder();
        public MediaQueueVideoQuality Quality { get; set; } = new MediaQueueVideoQuality();
        public MediaQueueVideoDeInterlace DeInterlace { get; set; } = new MediaQueueVideoDeInterlace();
    }

    public class MediaQueueVideoEncoder
    {
        public Guid Id { get; set; }
        public string Preset { get; set; }
        public string Tune { get; set; }
        public int Mode { get; set; }
        public decimal Value { get; set; }
        public int MultiPass { get; set; }
        public string Command { get; set; }
    }

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

    public class MediaQueueVideoDeInterlace
    {
        public bool Enable { get; set; } = false;
        public int Field { get; set; } = 1;
        public int Mode { get; set; } = 0;
    }
}
