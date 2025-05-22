using System.Drawing;
using System.Collections.Generic;

namespace IFME
{
    internal class i18nObj
    {
        // Metadata
        public string AuthorName { get; set; } = "Anime4000";
        public string AuthorEmail { get; set; } = "example@example.tld";
        public string AuthorProfile { get; set; } = "https://github.com/Anime4000";

        // UI Settings
        public Font FontUIWindows { get; set; } = new Font("Tahoma", 8);
        public Font FontUILinux { get; set; } = new Font("FreeSans", 8);

        // All Dialog UI strings, dynamically stored
        public Dictionary<string, string> Dialogs { get; set; } = new();

        // All Console Log UI strings, dynamically stored
        public Dictionary<string, string> Logs { get; set; } = new();

        // All Status UI strings, dynamically stored
        public Dictionary<string, string> Status { get; set; } = new();

        public Dictionary<int, string> DeInterlaceMode { get; set; } = new()
        {
            { 0, "Deinterlace only frame" },
            { 1, "Deinterlace each field" },
            { 2, "Skips spatial interlacing frame check" },
            { 3, "Skips spatial interlacing field check" }
        };

        public Dictionary<int, string> DeInterlaceField { get; set; } = new()
        {
            { 0, "Top Field First" },
            { 1, "Bottom Field First" }
        };

        // All form UI strings, dynamically stored
        public Dictionary<string, SortedDictionary<string, string>> Forms { get; set; } = new();
    }
}