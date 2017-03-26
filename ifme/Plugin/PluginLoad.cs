using System;
using System.IO;
using System.Threading;
using System.Linq;

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
			
			foreach (var item in Directory.EnumerateFiles(folder, "_plugin*.json", SearchOption.AllDirectories).OrderBy(file => file))
            {
				var json = File.ReadAllText(item);
                var plugin = JsonConvert.DeserializeObject<Plugin>(json);

                plugin.Path = Path.GetDirectoryName(item);

				Console.WriteLine($"Loading {plugin.Name}");

                Plugin.Items.Add(plugin.GUID, plugin);
            }

			Thread.Sleep(5000);
		}
    }
}