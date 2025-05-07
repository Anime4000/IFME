using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using IFME;

internal class i18n
{
    public static Dictionary<string, string> Installed { get; set; } = new();

    public static void LoadLangFiles()
    {
        var langFiles = Directory.GetFiles("Localiser", "*.json");
        foreach (var file in langFiles)
        {
            var lang = Path.GetFileNameWithoutExtension(file);
            var code = Language.TryParseCode(lang);
            var name = Language.FullName(lang);

            Installed.Add(code, name);
            frmSplashScreen.SetStatus($"Language Loading {name}");
        }
    }

    public static string[] GetLangAuthor(string currentLang = "eng")
    {
        var langFile = Path.Combine("Localiser", $"{currentLang}.json");

        if (!File.Exists(langFile))
            return new string[] { "// Language File is Not Found", "// Error 19", "// Please check Json file exist at Localiser folder" };

        var json = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile));

        if (json?.Forms == null)
            return new string[] { "// Json object is broken", "// Error 20", "// Please check that Json file formatting is valid" };

        return new string[] { json.AuthorName, json.AuthorProfile, json.AuthorEmail };
    }

    public static void Apply(Control parent, string formName, string currentLang = "eng")
    {
        var langFile = Path.Combine("Localiser", $"{currentLang}.json");

        if (!File.Exists(langFile))
            return;

        var json = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile));

        if (json?.Forms == null || !json.Forms.TryGetValue(formName, out var formStrings))
            return;

        foreach (Control ctrl in GetAllControls(parent))
        {
            if (IsLocalisable(ctrl))
            {
                if (ctrl is ListView)
                {
                    foreach (ColumnHeader header in ((ListView)ctrl).Columns)
                    {
                        if (formStrings.TryGetValue($"{ctrl.Name}{header.Index}", out string text))
                            header.Text = text;
                    }
                }
                else
                {
                    if (formStrings.TryGetValue(ctrl.Name, out string text))
                    {
                        ctrl.Text = text;
                    }
                }
            }
        }
    }

    public static void Save(Control parent, string formName, string currentLang = "eng")
    {
        var langFile = Path.Combine("Localiser", $"{currentLang}.json");

        i18nObj data;
        try
        {
            data = JsonConvert.DeserializeObject<i18nObj>(File.ReadAllText(langFile)) ?? new i18nObj();
        }
        catch
        {
            data = new i18nObj();
        }

        var formSorted = new SortedDictionary<string, string>();
        foreach (Control ctrl in GetAllControls(parent))
        {
            if (IsLocalisable(ctrl))
            {
                if (ctrl is ListView)
                {
                    foreach (ColumnHeader header in ((ListView)ctrl).Columns)
                    {
                        formSorted.Add($"{ctrl.Name}{header.Index}", header.Text);
                    }
                }
                else
                {
                    formSorted.Add(ctrl.Name, ctrl.Text);
                }
            }
        }

        try
        {
            data.Forms.Add(formName, formSorted);
        }
        catch
        {
            data.Forms[formName] = formSorted;
        }

        File.WriteAllText(langFile, JsonConvert.SerializeObject(data, Formatting.Indented));
    }

    private static IEnumerable<Control> GetAllControls(Control parent)
    {
        foreach (Control ctrl in parent.Controls)
        {
            foreach (Control child in GetAllControls(ctrl))
                yield return child;

            yield return ctrl;
        }
    }

    private static bool IsLocalisable(Control ctrl)
    {
        return ctrl is Label ||
               ctrl is Button ||
               ctrl is TabPage ||
               ctrl is CheckBox ||
               ctrl is RadioButton ||
               ctrl is ListView ||
               ctrl is ContextMenuStrip ||
               ctrl is GroupBox;
    }
}