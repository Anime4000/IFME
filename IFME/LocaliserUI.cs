using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;

internal class LocaliserUI
{
    public static void Apply(Control parent, string formName, string CurrentLang = "eng")
    {
        var langFile = Path.Combine("Localiser", $"{CurrentLang}.json");

        if (!File.Exists(langFile))
            return;

        var json = JsonConvert.DeserializeObject<LocaliserData>(File.ReadAllText(langFile));

        if (json?.Forms == null || !json.Forms.TryGetValue(formName, out var formStrings))
            return;

        foreach (Control ctrl in GetAllControls(parent))
        {
            if (IsLocalisable(ctrl) && formStrings.TryGetValue(ctrl.Name, out string text))
            {
                ctrl.Text = text;
            }
        }
    }

    public static void Save(Control parent, string formName, string CurrentLang = "eng")
    {
        var langFile = Path.Combine("Localiser", $"{CurrentLang}.json");

        LocaliserData data;
        try
        {
            data = JsonConvert.DeserializeObject<LocaliserData>(File.ReadAllText(langFile)) ?? new LocaliserData();
        }
        catch
        {
            data = new LocaliserData();
        }

        var formSorted = new SortedDictionary<string, string>();
        foreach (Control ctrl in GetAllControls(parent))
            if (IsLocalisable(ctrl))
                formSorted.Add(ctrl.Name, ctrl.Text);

        data.Forms.Add(formName, formSorted);

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
               ctrl is GroupBox;
    }
}