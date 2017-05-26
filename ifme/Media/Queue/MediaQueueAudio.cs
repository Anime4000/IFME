using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueAudio : MediaQueueCommon
    {
        public Guid Encoder { get; set; }
        public int EncoderMode { get; set; }
        public decimal EncoderQuality { get; set; }
        public int EncoderSampleRate { get; set; }
        public int EncoderChannel { get; set; }
        public string EncoderCommand { get; set; }
    }
}
