using System.Drawing;
using System.Collections.Generic;

internal class i18nObj
{
    // Metadata
    public string AuthorName { get; set; } = "Anime4000";
    public string AuthorEmail { get; set; } = "";
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

    // All form UI strings, dynamically stored
    public Dictionary<string, SortedDictionary<string, string>> Forms { get; set; } = new();
}
