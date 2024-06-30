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
        public MediaQueueAudioInfo Info { get; set; } = new MediaQueueAudioInfo();
        public string Command { get; set; }
        public string CommandFilter { get; set; }
    }

    public class MediaQueueAudioInfo
    {
        public int SampleRate { get; set; }
        public int Channel { get; set; }
    }

    public class MediaQueueAudioEncoder : MediaQueueAudioInfo
    {
        public Guid Id { get; set; }
        public int Mode { get; set; }
        public string Quality { get; set; }
        public string Command { get; set; }
    }
}
