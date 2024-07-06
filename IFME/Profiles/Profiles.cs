using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class Profiles
    {
        public static List<Profiles> Items { get; set; } = new List<Profiles>();

        public string ProfileName { get; set; }
        public string ProfileAuthor { get; set; }
        public string ProfileWebUrl { get; set; }
        public string ProfilePath { get; set; }
        public bool TryRemuxVideo { get; set; } = false;
        public bool TryRemuxAudio { get; set; } = false;
        public MediaContainer Container { get; set; } = new MediaContainer();
        public ProfilesVideo Video { get; set; } = new ProfilesVideo();
        public ProfilesAudio Audio { get; set; } = new ProfilesAudio();
    }
}
