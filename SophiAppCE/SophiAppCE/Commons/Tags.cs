using System;
using System.Windows;

namespace SophiAppCE.Commons
{
    internal static class Tags
    {
        internal static string LocalizationUriRU = "pack://application:,,,/Localization/RU.xaml";
        internal static string LocalizationUriEN = "pack://application:,,,/Localization/EN.xaml";
        internal static byte LoadingView = Convert.ToByte(Application.Current.FindResource("Tag.LoadingView"));
        internal static byte ContentView = Convert.ToByte(Application.Current.FindResource("Tag.ContentView"));

    }
}