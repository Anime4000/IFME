using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace IFME
{
    internal class Images
    {
		internal static Bitmap Resize(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}

			return destImage;
		}

		internal static bool IsValidImageFile(string file)
		{
			try
			{
				var inImage = Image.FromFile(file);
				var inGraphics = Graphics.FromImage(inImage);

				inImage.Dispose();
				inGraphics.Dispose();
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		internal static bool IsValidImageFileExt(string file)
        {
			string[] imgExt = { ".bmp", ".png", ".gif", ".jpg", ".jpe", ".jpeg", ".tiff" };

			foreach (var item in imgExt)
				if (string.Equals(item, Path.GetExtension(file), StringComparison.InvariantCultureIgnoreCase))
					return true;

			return false;
        }

		internal static string GetImageBitDepth(PixelFormat csp)
        {
            var value = string.Empty;

            switch (csp)
            {
                case PixelFormat.Indexed:
                    value = "Indexed";
                    break;
                case PixelFormat.Gdi:
                    break;
                case PixelFormat.Alpha:
                    break;
                case PixelFormat.PAlpha:
                    break;
                case PixelFormat.Extended:
                    break;
                case PixelFormat.Canonical:
                    value = "8-bit RGB, 8-bit Aplha";
                    break;
                case PixelFormat.Undefined:
                    value = "No Data";
                    break;
                case PixelFormat.Format1bppIndexed:
                    value = "1-bit Indexed";
                    break;
                case PixelFormat.Format4bppIndexed:
                    value = "4-bit Indexed";
                    break;
                case PixelFormat.Format8bppIndexed:
                    value = "8-bit Indexed";
                    break;
                case PixelFormat.Format16bppGrayScale:
                    value = "16-bit Gray Scale";
                    break;
                case PixelFormat.Format16bppRgb555:
                    value = "5-bit";
                    break;
                case PixelFormat.Format16bppRgb565:
                    value = "5-bit";
                    break;
                case PixelFormat.Format16bppArgb1555:
                    value = "8-bit";
                    break;
                case PixelFormat.Format24bppRgb:
                    value = "8-bit";
                    break;
                case PixelFormat.Format32bppRgb:
                    value = "8-bit";
                    break;
                case PixelFormat.Format32bppArgb:
                    value = "8-bit";
                    break;
                case PixelFormat.Format32bppPArgb:
                    value = "8-bit";
                    break;
                case PixelFormat.Format48bppRgb:
                    value = "16-bit";
                    break;
                case PixelFormat.Format64bppArgb:
                    value = "16-bit";
                    break;
                case PixelFormat.Format64bppPArgb:
                    value = "16-bit";
                    break;
                case PixelFormat.Max:
                    break;
                default:
                    break;
            }

            return value;
        }

        internal static string GetImagePixelFormat(PixelFormat csp)
        {
            var value = string.Empty;

            switch (csp)
            {
                case PixelFormat.Indexed:
                    value = "Indexed";
                    break;
                case PixelFormat.Gdi:
                    break;
                case PixelFormat.Alpha:
                    break;
                case PixelFormat.PAlpha:
                    break;
                case PixelFormat.Extended:
                    break;
                case PixelFormat.Canonical:
                    value = "RGBA8";
                    break;
                case PixelFormat.Undefined:
                    value = "No Data";
                    break;
                case PixelFormat.Format1bppIndexed:
                    value = "Indexed 1-bit";
                    break;
                case PixelFormat.Format4bppIndexed:
                    value = "Indexed 4-bit";
                    break;
                case PixelFormat.Format8bppIndexed:
                    value = "Indexed 8-bit";
                    break;
                case PixelFormat.Format16bppGrayScale:
                    value = "Gray Scale (16-bit)";
                    break;
                case PixelFormat.Format16bppRgb555:
                    value = "RGB555";
                    break;
                case PixelFormat.Format16bppRgb565:
                    value = "RGB565";
                    break;
                case PixelFormat.Format16bppArgb1555:
                    value = "RGBA5551";
                    break;
                case PixelFormat.Format24bppRgb:
                    value = "RGB888";
                    break;
                case PixelFormat.Format32bppRgb:
                    value = "RGBA8880";
                    break;
                case PixelFormat.Format32bppArgb:
                    value = "RGBA8888";
                    break;
                case PixelFormat.Format32bppPArgb:
                    value = "RGBA8888";
                    break;
                case PixelFormat.Format48bppRgb:
                    value = "RGB16";
                    break;
                case PixelFormat.Format64bppArgb:
                    value = "RGBA16";
                    break;
                case PixelFormat.Format64bppPArgb:
                    value = "RGBA16";
                    break;
                case PixelFormat.Max:
                    value = "RGB32";
                    break;
                default:
                    break;
            }

            return value;
        }

        internal static string GetImageSeq(string file)
        {
			var filePath = Path.GetDirectoryName(file);
			var fileName = Path.GetFileNameWithoutExtension(file);
			var fileExts = Path.GetExtension(file);

			if (fileName.Any(char.IsDigit))
            {
				var intCount = fileName.Count(char.IsDigit);

				var x = Regex.Matches(fileName, "\\d{1,}", RegexOptions.IgnoreCase);

				var d = string.Empty;

				if (x.Count > 0)
                {
					if (intCount == 1)
						d = "%d";
					else
						d = $"%0{intCount}d";

					return AppPath.Combine(filePath, fileName.Replace($"{x[0]}", $"{d}") + fileExts);
                }
			}

			return string.Empty;
        }
	}
}
