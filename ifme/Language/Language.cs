using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace ifme
{
    class Language
    {
        public string AuthorName { get; set; } = "Anime4000";
        public string AuthorEmail { get; set; } = "ilham92_sakura@yahoo.com";
        public string AuthorProfile { get; set; } = "https://github.com/Anime4000";

        public Font UIFontWindows { get; set; } = new Font("Tahoma", 8);
        public Font UIFontLinux { get; set; } = new Font("FreeSans", 8);

        public Dictionary<string, string> frmMain { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> frmOption { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> frmShutdown { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> frmAbout { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> cmsNewImport { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> cmsEncodingPreset { get; set; } = new Dictionary<string, string>();

        public LanguageIO InputBoxNewMedia { get; set; } = new LanguageIO()
        {
            Title = "New video/audio",
            Message = "You about to create a blank stream of video, audio, subtitle and fonts.\nThis way you can add files or convert them or just merge (like Mkvtoolnix, MP4Box)\n\nEnter a new file name:"
        };
        public LanguageIO InputBoxEncodingPreset { get; set; } = new LanguageIO()
        {
            Title = "Save a new encoding preset",
            Message = "You about to create a new encoding preset based on current configuration.\nWith this, you can reuse for others.\n\nEnter a new name:"
        };
        public LanguageIO InputBoxCommandLine { get; set; } = new LanguageIO()
        {
            Title = "Encoder command-line",
            Message = "Modify encoder command-line arguments\nExample: --bitdepth 8"
        };

        public LanguageIO MsgBoxShutdown { get; set; } = new LanguageIO()
        {
            Title = "Question",
            Message = "Shutdown or Restart is set!\nThis computer will do that when encoding completed. Proceed?"
        };
        public LanguageIO MsgBoxSaveFolder { get; set; } = new LanguageIO()
        {
            Title = "Invalid Path",
            Message = "Over network not supported, please mount it as drive."
        };
        public LanguageIO MsgBoxCodecIncompatible { get; set; } = new LanguageIO()
        {
            Title = "Incompatile Codec",
            Message = "Output format and codec are not compatible! Choose different one."
        };

        public List<string> ComboBoxDeInterlaceMode { get; set; } = new List<string>();
        public List<string> ComboBoxDeInterlaceField { get; set; } = new List<string>();
        public List<string> ComboBoxShutdown { get; set; } = new List<string>();

        public static Dictionary<string, Language> List = new Dictionary<string, Language>();
        public static Language Lang = new Language();

        public static void Load()
        {
            // Check folder if exist
            if (!Directory.Exists("lang"))
                Directory.CreateDirectory("lang");

            // Read language JSON file
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "lang");

            foreach (var item in Directory.EnumerateFiles(folder, "*.json", SearchOption.AllDirectories).OrderBy(file => file))
            {
                var json = File.ReadAllText(item);
                var data = JsonConvert.DeserializeObject<Language>(json);
                var id = Path.GetFileNameWithoutExtension(item);

                List.Add(id, data);
            }

            if (File.Exists(Path.Combine(folder, $"{Properties.Settings.Default.Language}.json")))
            {
                Lang = List[Properties.Settings.Default.Language];
            }
            else
            {
                Lang = List["eng"];
            }
        }
    }
}
