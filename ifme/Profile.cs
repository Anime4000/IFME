using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ifme
{
    public class Profile
    {
        public static List<Profile> Item = new List<Profile>();

        public bool SaveMKV;
        public QueueVideo Video;
        public QueueAudio Audio;
    }
}
