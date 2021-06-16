using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class ProfilesAudio
    {
        public MediaQueueAudioEncoder Encoder { get; set; } = new MediaQueueAudioEncoder();
        public string Command { get; set; }
        public string CommandFilter { get; set; }
    }
}
