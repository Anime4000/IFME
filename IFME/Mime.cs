using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    public class Mime
    {
        public static Dictionary<string, string> Codes = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(AppPath.Combine("Mime.json")));

        public static string GetType(string value)
        {
            if (Codes.TryGetValue(Path.GetExtension(value), out string exts))
            {
                return exts;
            }

            return "application/octet-stream";
        }
    }
}
