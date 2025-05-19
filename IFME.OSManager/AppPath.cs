using System;
using System.IO;

public static class AppPath 
{
    /// <summary>
    /// Builds a full path from the given parts. If the first part is not rooted, it will be combined with the application base directory.
    /// </summary>
    /// <param name="parts">Path parts to combine</param>
    /// <returns>Fully qualified path</returns>
    public static string Combine(params string[] parts)
    {
        if (parts == null || parts.Length == 0)
            throw new ArgumentException("No path parts provided.", nameof(parts));

        string first = parts[0];

        // If the first part is already a rooted path (absolute), combine as-is
        if (Path.IsPathRooted(first))
        {
            return Path.GetFullPath(Path.Combine(parts));
        }
        else
        {
            // Prepend base directory
            var fullParts = new string[parts.Length + 1];
            fullParts[0] = AppDomain.CurrentDomain.BaseDirectory;
            Array.Copy(parts, 0, fullParts, 1, parts.Length);
            return Path.GetFullPath(Path.Combine(fullParts));
        }
    }
}
