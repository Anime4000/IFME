using System;
using System.IO;

public static class AppPath 
{
    /// <summary>
    /// Builds a path from the given parts, ensuring that it is a full path.
    /// </summary>
    /// <param name="parts"></param>
    /// <returns></returns>
    public static string Combine(params string[] parts)
    {
        return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.Combine(parts)));
    }
}
