using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueVideo : MediaQueueCommon
    {
        public Guid Encoder { get; set; }
        public string EncoderPreset { get; set; }
        public string EncoderTune { get; set; }
        public int EncoderMode { get; set; }
        public decimal EncoderValue { get; set; }
        public int EncoderMultiPass { get; set; }
        public string EncoderCommand { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsVFR { get; set; }
        public float FrameRate { get; set; }
        public int BitDepth { get; set; }
        public int PixelFormat { get; set; }

        public bool DeInterlace { get; set; }
        public int DeInterlaceMode { get; set; }
        public int DeInterlaceField { get; set; }
    }
}
