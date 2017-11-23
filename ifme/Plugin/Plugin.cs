using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ifme
{
    public class Plugin : PluginCommon
    {
        public static Dictionary<Guid, Plugin> Items { get; set; } = new Dictionary<Guid, Plugin>();

        public static void Load()
        {
            // Check folder if exist
            if (!Directory.Exists("plugin"))
                Directory.CreateDirectory("plugin");

            // Read plugin JSON file
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "plugin");

            foreach (var item in Directory.EnumerateFiles(folder, "_plugin*.json", SearchOption.AllDirectories).OrderBy(file => file))
            {
                try
                {
                    var json = File.ReadAllText(item);
                    var plugin = JsonConvert.DeserializeObject<Plugin>(json);

                    plugin.FilePath = Path.GetDirectoryName(item);

                    ConsoleEx.ClearCurrentLine();
                    Console.Write($"\rLoading plugin: {plugin.Name}");
                    Thread.Sleep(100);

                    Items.Add(plugin.GUID, plugin);
                }
                catch (Exception ex)
                {
                    ConsoleEx.Write(LogLevel.Error, $"Plugin JSON file ");
                    ConsoleEx.Write(ConsoleColor.Red, $"`{Path.GetFileName(item)}'");
                    ConsoleEx.Write($" appears to be invalid.\nException thrown: {ex.Message}\n");
                }
            }

            ConsoleEx.ClearCurrentLine();
            Console.WriteLine("Loading plugins complete!");

            Thread.Sleep(5000);
        }
    }
}
