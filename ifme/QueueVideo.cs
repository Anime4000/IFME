using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class QueueVideo: QueueCommon
    {
        public int Width;
        public int Height;
        public float FrameRate;
        public bool FrameRateVariable;
        public int BitDepth;
        public int Chroma;

        public bool Deinterlace;
        public int DeinterlaceMode;
        public int DeinterlaceField;

        public Guid Encoder;
        public string EncoderPreset;
        public string EncoderTune;
        public int EncoderMode;
        public decimal EncoderModeValue;
        public int EncoderMultiPass;
        public string EncoderArgs;
    }
}
