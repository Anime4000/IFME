using System.Drawing;
using System.Collections.Generic;

internal class LocaliserData
{
    // Metadata
    public string AuthorName { get; set; } = "Anime4000";
    public string AuthorEmail { get; set; } = "hitoha4000@outlook.com";
    public string AuthorProfile { get; set; } = "https://github.com/Anime4000";

    // UI Settings
    public Font FontUIWindows { get; set; } = new Font("Tahoma", 8);
    public Font FontUILinux { get; set; } = new Font("FreeSans", 8);

    // All form UI strings, dynamically stored
    public Dictionary<string, SortedDictionary<string, string>> Forms { get; set; } = new();
}
