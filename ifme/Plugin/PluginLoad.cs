using System;
using System.IO;
using System.Threading;

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

				Console.WriteLine($"Loading {plugin.Name}");

                Plugin.Items.Add(plugin.GUID, plugin);
            }

			Thread.Sleep(3000);
		}
    }
}