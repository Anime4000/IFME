using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    class ArgsParser
    {
        internal static string Parse(string arg, string value)
        {
            if (string.IsNullOrEmpty(arg) || string.IsNullOrWhiteSpace(arg))
                return string.Empty;

            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                return string.Empty;

            switch (value.ToLowerInvariant())
            {
                case "0":
                case "no":
                case "off":
                case "non":
                case "none":
                case "blank":
                case "disable":
                case "disabled":
                    return string.Empty;

                default:
                    break;
            }

            return $"{arg} {value}";
        }
    }
}
