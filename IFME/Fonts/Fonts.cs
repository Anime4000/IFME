using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace IFME
{
    internal static partial class Fonts
    {
        private static PrivateFontCollection Library = new();

        internal static void Initialize()
        {
            Library.AddFontFile(AppPath.Combine("Fonts", "fontawesome-webfont.ttf"));
        }

        internal static Font Awesome(float size, FontStyle style)
        {
            return new Font(Library.Families[0], size, style);
        }

        internal static Font Monospace(float size = 10f, FontStyle fontStyle = FontStyle.Regular)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
            {
                return new Font(GetMonospaceFont("Cascadia Mono", "Consolas", "Lucida Console", "Courier New", "Courier"), size, fontStyle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new Font(GetMonospaceFont("DejaVu Sans Mono", "Liberation Mono", "Ubuntu Mono", "Courier"), size, fontStyle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new Font(GetMonospaceFont("Menlo", "Monaco", "Courier"), size, fontStyle);
            }
            else
            {
                return new Font("Courier", size, fontStyle);
            }
        }

        private static string GetMonospaceFont(params string[] candidates)
        {
            using (InstalledFontCollection fonts = new())
            {
                foreach (string candidate in candidates)
                {
                    if (fonts.Families.Any(f => f.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase)))
                        return candidate;
                }
            }

            return "Courier";
        }
    }
}
