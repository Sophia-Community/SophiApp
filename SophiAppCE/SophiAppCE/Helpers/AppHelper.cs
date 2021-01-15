using System.Reflection;

namespace SophiAppCE.Helpers
{
    internal static class AppHelper
    {
        internal static string GetFullName() => $"{GetName()} {GetVersion()}";

        internal static string GetName() => "SophiApp Community Edition";

        internal static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
    }
}