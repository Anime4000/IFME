using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace ifme
{
    public class Version
    {
        public static string ThisApp { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        public static bool IsLatest(string Target)
        {
            if (!Target.Contains('.'))
                return false;

            var check = Target.Split('.');
            var source = ThisApp.Split('.');

            if (check.Length < 1)
                return false;

            if (source.Length < 1)
                return false;

            if (source.Length == check.Length)
            {
                int.TryParse(source[0], out int s1);
                int.TryParse(source[1], out int s2);

                int.TryParse(check[0], out int c1);
                int.TryParse(check[1], out int c2);

                if (s1 >= c1 && s2 >= c2)
                    return true;
            }

            return false;
        }
    }
}
