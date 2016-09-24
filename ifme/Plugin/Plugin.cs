using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace ifme
{
    public class Plugin : PluginCommon
    {
        public static Dictionary<Guid, Plugin> Items { get; set; } = new Dictionary<Guid, Plugin>();
    }
}
