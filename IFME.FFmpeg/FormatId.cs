using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME.FFmpeg
{
    class FormatId
    {
        private static Dictionary<string, string> FormatExts = new Dictionary<string, string>()
        {
            { "subrip", "srt" },
            { "hdmv_pgs_subtitle", "sup" }
        };

        public static string Get(string Codec)
        {
            if (FormatExts.TryGetValue(Codec, out string Ext))
            {
                return Ext;
            }

            return Codec;
        }
    }
}
