using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace IFME
{
    public class Mime
    {
        public static Dictionary<string, string> Codes = JsonConvert.DeserializeObject<Dictionary<string, string>>(WAD.Jason.LoadText("Mime.json"));

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