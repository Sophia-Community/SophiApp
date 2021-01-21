using System;
using System.Windows;

namespace SophiAppCE.Commons
{
    internal static class Tags
    {
        internal static string LocalizationUriRU = "pack://application:,,,/Localization/RU.xaml";
        internal static string LocalizationUriEN = "pack://application:,,,/Localization/EN.xaml";
        internal static byte StatusPageContent = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageContent"));
        internal static byte StatusPageFinish = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageFinish"));
        internal static byte StatusPageStart = Convert.ToByte(Application.Current.FindResource("Tag.StatusPageStart"));        
    }
}