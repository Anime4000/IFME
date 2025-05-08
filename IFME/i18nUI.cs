internal class i18nUI
{
    // UI object for hidden controls
    internal static i18nObj Obj { get; set; } = new();

    internal static string Dialog(string key)
    {
        return Obj.Dialogs.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Dialog.{key}";
    }

    internal static string Log(string key)
    {
        return Obj.Logs.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Log.{key}";
    }

    internal static string Status(string key)
    {
        return Obj.Status.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Status.{key}";
    }
}
