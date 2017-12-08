using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ifme
{
    internal class MediaProject
    {
        internal static string ProjectFile { get; set; } = string.Empty;
        internal static bool StartEncode { get; set; } = false;

        internal static bool Save(string FileSave, List<MediaQueue> Data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(Data, Formatting.Indented);

                File.WriteAllText(FileSave, json, Encoding.UTF8);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static List<MediaQueue> Load(string FileLoad)
        {
            try
            {
                var json = File.ReadAllText(FileLoad);
                return JsonConvert.DeserializeObject<List<MediaQueue>>(json);
            }
            catch (Exception)
            {
                ConsoleEx.Write(LogLevel.Error, "JSON", "Invalid JSON file");
                return new List<MediaQueue>();
            }
        }
    }
}
