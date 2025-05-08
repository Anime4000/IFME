internal class i18nUI
{
    // UI object for hidden controls
    internal static i18nObj Text { get; set; } = new();

    internal static string Dialog(string key)
    {
        return Text.Dialogs.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Dialog.{key}";
    }

    internal static string Log(string key)
    {
        return Text.Logs.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Log.{key}";
    }

    internal static string Status(string key)
    {
        return Text.Status.TryGetValue(key, out var value) ? value : $"i18n Missing Key Value: Status.{key}";
    }
}
