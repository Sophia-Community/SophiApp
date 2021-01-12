using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class AppHelper
    {
        internal static string GetName() => "SophiApp Community Edition";

        internal static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);

        internal static string GetFullName() => $"{GetName()} {GetVersion()}";
        
    }
}
