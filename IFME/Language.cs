using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace IFME
{
    public class Language
    {
        public static Dictionary<string, string> Codes = JsonConvert.DeserializeObject<Dictionary<string, string>>(WAD.Jason.LoadText("Language.json"));

        public static string FullName(object obj)
        {
            return FullName(obj.ToString());
        }

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

        public static string TryParseCode(string id)
        {
            if (Codes.TryGetValue(id, out _))
            {
                return id;
            }
            else
            {
                return "und";
            }
        }

        public static string FromFileNameFull(string filePath)
        {
            var file = Path.GetFileNameWithoutExtension(filePath);
            return FullName(TryParseCode(file.Substring(file.Length - 3)));
        }

        public static string FromFileNameCode(string filePath)
        {
            var file = Path.GetFileNameWithoutExtension(filePath);
            return TryParseCode(file.Substring(file.Length - 3));
        }

        public static string FromFileNameCode(string filePath, object lastChoice)
        {
            var file = Path.GetFileNameWithoutExtension(filePath);
            var lang = TryParseCode(file.Substring(file.Length - 3));

            if (string.Equals(lang, "und"))
                return lastChoice.ToString();

            return lang;
        }
    }
}