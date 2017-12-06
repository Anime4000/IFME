using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace ifme
{
    internal class MediaQueueManagement
    {
        /// <summary>
        /// Check if Encoder has desire Bit Depth support
        /// </summary>
        /// <param name="Encoder">Video encoder to check</param>
        /// <param name="Value">Current video bit depth</param>
        /// <returns>If desire bit depth not found, return default 8 bit</returns>
        internal static int IsValidBitDepth(Guid Encoder, int Value)
        {
            Plugin temp;

            if (Plugin.Items.TryGetValue(Encoder, out temp))
            {
                foreach (var item in temp.Video.Encoder)
                {
                    if (item.BitDepth == Value)
                    {
                        return Value;
                    }
                }
            }

            return 8;
        }

        internal static string ProjectFile { get; set; } = string.Empty;

        internal static bool ProjectSave(string FileSave, List<MediaQueue> Data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(Data);

                File.WriteAllText(FileSave, json, Encoding.UTF8);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static List<MediaQueue> ProjectLoad(string FileLoad)
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
