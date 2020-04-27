using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME.OSManager
{
    public class MArch
    {
        public static Dictionary<string, string> GetArchName = new Dictionary<string, string>()
        {
            { "znver1", "Zen architecture" },
            { "znver2", "Zen 2 architecture" }
        };
    }
}
