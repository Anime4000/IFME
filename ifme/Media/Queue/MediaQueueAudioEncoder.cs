using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class MediaQueueAudioEncoder
    {
        public Guid Id { get; set; }
        public int Mode { get; set; }
        public decimal Quality { get; set; }
        public int SampleRate { get; set; }
        public int Channel { get; set; }
        public string Command { get; set; }
    }
}
