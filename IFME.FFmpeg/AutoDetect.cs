using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Http.Headers;

namespace IFME.FFmpeg
{
    public class AutoDetect
    {
        public class Crop
        {
            public static string Get(string inputFile)
            {
                var input = File.ReadAllLines(inputFile);
                List<string> cropValues = ExtractCropValues(input);

                if (cropValues.Any())
                {
                    return CalculateAverageCrop(cropValues);
                }
                else
                {
                    Console.WriteLine("No crop values found.");
                }

                return string.Empty;
            }

            static List<string> ExtractCropValues(IEnumerable<string> lines)
            {
                List<string> cropValues = new List<string>();
                string pattern = @"crop=(\d+:\d+:\d+:\d+)";

                foreach (var line in lines)
                {
                    foreach (Match match in Regex.Matches(line, pattern))
                    {
                        cropValues.Add(match.Groups[1].Value);
                    }
                }

                return cropValues;
            }

            static string CalculateAverageCrop(List<string> cropValues)
            {
                var averages = cropValues
                    .Select(crop => crop.Split(':').Select(int.Parse).ToArray())
                    .Aggregate((a, b) => a.Zip(b, (x, y) => x + y).ToArray())
                    .Select(sum => (sum / cropValues.Count).ToString());

                return string.Join(":", averages);
            }
        }
    }
}
