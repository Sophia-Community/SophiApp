using System;
using System.Windows;

namespace SophiAppCE.Commons
{
    internal static class Tags
    {
        internal static byte StatusPageContent = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageContent"));
        internal static byte StatusPageFinish = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageFinish"));
        internal static byte StatusPageStart = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageStart"));
    }
}