using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace ifme
{
    class Branding
    {
        private static Bitmap _Blank = new Bitmap(1, 1);

        private static string _PathPartnerSplashScreen = Path.Combine("branding", "partner", "1.jpj");
        private static string _PathPartnerBannerLeft = Path.Combine("branding", "partner", "a.jpj");
        private static string _PathPartnerBannerRight = Path.Combine("branding", "partner", "b.jpj");
        private static string _PathPartnerAbout = Path.Combine("branding", "partner", "z.jpj");

        private static string _PathOriginalSplashScreen = Path.Combine("branding", "ai", "1.jpj");
        private static string _PathOriginalBannerLeft = Path.Combine("branding", "ai", "a.jpj");
        private static string _PathOriginalBannerRight = Path.Combine("branding", "ai", "b.jpj");
        private static string _PathOriginalAbout = Path.Combine("branding", "ai", "z.jpj");

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
            // partner
            if (File.Exists(_PathPartnerBannerLeft) && File.Exists(_PathPartnerBannerRight))
            {
                var banner = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(banner))
                {
                    g.DrawImage(new Bitmap(_PathOriginalBannerLeft), new Point(0, 0));
                    g.DrawImage(new Bitmap(_PathOriginalBannerRight), new Point(width - 640, 0));
                }

                return banner;
            }

            // original
            if (File.Exists(_PathOriginalBannerLeft) && File.Exists(_PathOriginalBannerRight))
            {
                var banner = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(banner))
                {
                    g.DrawImage(new Bitmap(_PathOriginalBannerLeft), new Point(0, 0));
                    g.DrawImage(new Bitmap(_PathOriginalBannerRight), new Point(width - 640, 0));
                }

                return banner;
            }

            return new Bitmap(_Blank, width, height);
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
    }
}
