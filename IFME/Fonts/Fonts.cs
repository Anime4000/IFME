using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFME
{
    internal static partial class Fonts
    {
        private static PrivateFontCollection Library = new PrivateFontCollection();

        internal static void Initialize()
        {
            Library.AddFontFile(Path.Combine("Fonts", "fontawesome-webfont.ttf"));
            Library.AddFontFile(Path.Combine("Fonts", "unifont-12.1.04.ttf"));
        }

        internal static Font Awesome(float size, FontStyle style)
        {
            return new Font(Library.Families[0], size, style);
        }

        internal static Font Uni(float size, FontStyle style)
        {
            return new Font(Library.Families[1], size, style);
        }
    }
}
