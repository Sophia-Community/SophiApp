using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.Commons
{
    class Localizator
    {
        internal static Dictionary<UILanguage, string> GetLocalizedDescriptions(JsonDTO json)
        {
            return new Dictionary<UILanguage, string>
            {
                { UILanguage.RU, json.LocalizedDescriptions.RU },
                { UILanguage.EN, json.LocalizedDescriptions.EN }
            };
        }

        internal static Dictionary<UILanguage, string> GetLocalizedHeaders(JsonDTO json)
        {
            return new Dictionary<UILanguage, string>
            {
                { UILanguage.RU, json.LocalizedHeaders.RU },
                { UILanguage.EN, json.LocalizedHeaders.EN }
            };
        }

        internal static UILanguage Initializing()
        {
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
            var localization = GetName(language);
            var uri = GetUris(localization);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
            return localization;
        }

        private static Uri GetUris(UILanguage localization)
        {
            var languagesUris = new Dictionary<UILanguage, Uri>()
            {
                { UILanguage.EN, new Uri("pack://application:,,,/Localizations/EN.xaml", UriKind.Absolute) },
                { UILanguage.RU, new Uri("pack://application:,,,/Localizations/RU.xaml", UriKind.Absolute) }
            };

            return languagesUris[localization];
        }

        private static UILanguage GetName(string localizationName) => Enum.GetNames(typeof(UILanguage)).Contains(localizationName) ? (UILanguage)Enum.Parse(typeof(UILanguage), localizationName) : UILanguage.EN;
        
    }
}
