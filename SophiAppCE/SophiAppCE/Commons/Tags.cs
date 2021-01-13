using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Commons
{
    internal static class Tags
    {
        internal static byte StatusPageStart = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageStart"));
        internal static byte StatusPageContent = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageContent"));
        internal static byte StatusPageFinish = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageFinish"));
    }
}
