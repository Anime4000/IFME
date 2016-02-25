using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class QueueAudio : QueueCommon
    {
        public int BitDepth; // use for decoding, hidden from GUI

        public Guid Encoder;
        public int EncoderMode;
        public string EncoderValue;
        public int EncoderSampleRate;
        public int EncoderChannel;
        public string EncoderArgs;
    }
}
