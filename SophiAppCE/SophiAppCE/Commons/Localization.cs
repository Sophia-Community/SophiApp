using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Commons
{
    internal static class Localization
    {
        internal static string TestWinVerText = Application.Current.FindResource("Localization.Test.WinVer.Text") as string;
        internal static string TestWinVerError = Application.Current.FindResource("Localization.Test.WinVer.Error") as string;
    }
}
