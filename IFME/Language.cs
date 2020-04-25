using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IFME
{
    public class Language
    {
        public static Dictionary<string, string> Codes = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine("Language.json")));

        public static string FullName(string id)
        {
            if (Codes.TryGetValue(id, out string name))
            {
                return name;
            }
            else
            {
                return "Undetermined";
            }
        }
    }
}
