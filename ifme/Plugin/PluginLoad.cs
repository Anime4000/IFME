using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ifme
{
    public class PluginLoad
    {
        public PluginLoad()
        {
            // Check folder if exist
            if (!Directory.Exists("plugin"))
                Directory.CreateDirectory("plugin");

            // Read plugin JSON file
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "plugin");
            foreach (var item in Directory.GetDirectories(folder))
            {
                var json = File.ReadAllText(Path.Combine(item, "_plugin.json"));
                var plugin = JsonConvert.DeserializeObject<Plugin>(json);

                plugin.Path = item;

                Plugin.Items.Add(plugin.GUID, plugin);
            }
        }
    }
}