using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;

namespace SophiApp.Commons
{
    internal class Localizator
    {
        private static UILanguage GetName(string localizationName) => Enum.GetNames(typeof(UILanguage)).Contains(localizationName) ? (UILanguage)Enum.Parse(typeof(UILanguage), localizationName) : UILanguage.EN;

        private static Uri GetUris(UILanguage localization)
        {
            var languagesUris = new Dictionary<UILanguage, Uri>()
            {
                { UILanguage.EN, new Uri("pack://application:,,,/Localizations/EN.xaml", UriKind.Absolute) },
                { UILanguage.RU, new Uri("pack://application:,,,/Localizations/RU.xaml", UriKind.Absolute) }
            };

            return languagesUris[localization];
        }

        internal static UILanguage Initializing()
        {            
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
            var localization = GetName(language);
            var uri = GetUris(localization);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
            return localization;
        }
    }
}