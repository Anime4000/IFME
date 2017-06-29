using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace ifme
{
    class Branding
    {
        private static Bitmap _Blank = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        private static string _PathPartnerSplashScreen = Path.Combine(Get.AppRootDir, "branding", "partner", "i0.ocx");
        private static string _PathPartnerBannerLeft = Path.Combine(Get.AppRootDir, "branding", "partner", "i1.vxd");
        private static string _PathPartnerBannerRight = Path.Combine(Get.AppRootDir, "branding", "partner", "i2.vxd");
        private static string _PathPartnerAbout = Path.Combine(Get.AppRootDir, "branding", "partner", "ia.vxd");
        private static string _PathPartnerName = Path.Combine(Get.AppRootDir, "branding", "partner", "t0.zip");
        private static string _PathPartnerCode = Path.Combine(Get.AppRootDir, "branding", "partner", "t1.zip");
        private static string _PathPartnerCopy = Path.Combine(Get.AppRootDir, "branding", "partner", "t2.zip");

        private static string _PathOriginalSplashScreen = Path.Combine(Get.AppRootDir, "branding", "ai", "1.jpj");
        private static string _PathOriginalBannerLeft = Path.Combine(Get.AppRootDir, "branding", "ai", "a.jpj");
        private static string _PathOriginalBannerRight = Path.Combine(Get.AppRootDir, "branding", "ai", "b.jpj");
        private static string _PathOriginalAbout = Path.Combine(Get.AppRootDir, "branding", "ai", "z.jpj");

        public static Bitmap SplashScreen()
        {
            // partner
            if (File.Exists(_PathPartnerSplashScreen))
            {
                return new Bitmap(_PathPartnerSplashScreen);
            }

            // original
            if (File.Exists(_PathOriginalSplashScreen))
            {
                return new Bitmap(_PathOriginalSplashScreen);
            }

            // fall back
            return new Bitmap(_Blank, 854, 480);
        }

        public static Bitmap Banner(int width, int height)
        {
            var banner1 = string.Empty;
            var banner2 = string.Empty;

            if (File.Exists(_PathPartnerBannerLeft) && File.Exists(_PathPartnerBannerRight))
            {
                banner1 = _PathPartnerBannerLeft;
                banner2 = _PathPartnerBannerRight;
            }
            else if (File.Exists(_PathOriginalBannerLeft) && File.Exists(_PathOriginalBannerRight))
            {
                banner1 = _PathOriginalBannerLeft;
                banner2 = _PathOriginalBannerRight;
            }

            try
            {
                using (var png = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                using (var g = Graphics.FromImage(png))
                {
                    g.DrawImage(new Bitmap(banner1), 0, 0);
                    g.DrawImage(new Bitmap(banner2), width - 640, 0);

                    return new Bitmap(png);
                }
            }
            catch (Exception)
            {
                return new Bitmap(_Blank, width, height);
            }
        }

        public static Bitmap About()
        {
            // partner
            if (File.Exists(_PathPartnerAbout))
            {
                return new Bitmap(_PathPartnerAbout);
            }

            // original
            if (File.Exists(_PathOriginalAbout))
            {
                return new Bitmap(_PathOriginalAbout);
            }

            // fall back
            return new Bitmap(_Blank, 600, 200);
        }

        public static string Title()
        {
            // partner
            if (File.Exists(_PathPartnerName))
            {
                return File.ReadAllText(_PathPartnerName);
            }
            else
            {
                return Properties.Resources.AppTitle;
            }
        }

        public static string TitleShort()
        {
            if (File.Exists(_PathPartnerCode))
            {
                return File.ReadAllText(_PathPartnerCode);
            }
            else
            {
                return "IFME";
            }
        }

        public static string CopyRight()
        {
            if (File.Exists(_PathPartnerCopy))
            {
                return File.ReadAllText(_PathPartnerCopy);
            }
            else
            {
                return "Anime4000, FFmpeg, MulticoreWare, VideoLAN, GPAC\nXiph.Org Foundation, Google Inc., Nero AG, Moritz Bunkus, et al.";
            }
        }
    }
}
