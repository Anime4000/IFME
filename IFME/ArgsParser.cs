using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    class ArgsParser
    {
        private static readonly HashSet<string> invalidValues = new HashSet<string>
        {
            "0", "no", "off", "non", "none", "blank", "disable", "disabled"
        };

        internal static string Parse(string arg, string value)
        {
            if (string.IsNullOrWhiteSpace(arg) || string.IsNullOrWhiteSpace(value) || invalidValues.Contains(value.ToLowerInvariant()))
                return string.Empty;

            return $"{arg} {value}";
        }
    }
}
