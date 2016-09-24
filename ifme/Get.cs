using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ifme
{
    public class Get
    {
        public string CodecFormat(string codecId)
        {
            var json = File.ReadAllText("format.json");
            var format = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            var formatId = string.Empty;

            if (format.TryGetValue(codecId, out formatId))
                return formatId;

            return "mkv";
        }
    }
}
