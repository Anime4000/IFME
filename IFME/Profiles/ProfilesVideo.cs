using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class ProfilesVideo
    {
        public MediaQueueVideoEncoder Encoder { get; set; } = new MediaQueueVideoEncoder();
        public MediaQueueVideoQuality Quality { get; set; } = new MediaQueueVideoQuality();
        public MediaQueueVideoDeInterlace DeInterlace { get; set; } = new MediaQueueVideoDeInterlace();
    }
}
