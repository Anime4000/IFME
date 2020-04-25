using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public partial class MediaQueueAudio : MediaQueueCommon
    {
        public MediaQueueAudioEncoder Encoder { get; set; } = new MediaQueueAudioEncoder();
        public bool Copy { get; set; } = false;
        public string Command { get; set; }
    }

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
