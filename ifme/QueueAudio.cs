using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class QueueAudio : QueueCommon
    {
        public Guid Encoder;
        public string EncoderMode;
        public string EncoderValue;
        public string EncoderSampleRate;
        public string EncoderChannel;
        public string EncoderArgs;
    }
}
