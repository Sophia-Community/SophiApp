using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.Commons
{
    class UILocalization
    {
        private Dictionary<UILanguage, Uri> localizationDictionaries = new Dictionary<UILanguage, Uri>()
        {
            { UILanguage.EN, new Uri("pack://application:,,,/Localizations/EN.xaml", UriKind.Absolute) },
            { UILanguage.RU, new Uri("pack://application:,,,/Localizations/RU.xaml", UriKind.Absolute) },
        };

        private UILanguage current;
        public UILanguage Current { get => current; private set => current = value; }

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

        public UILocalization()
        {
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
            var localization = Enum.GetNames(typeof(UILanguage)).Contains(language) ? (UILanguage)Enum.Parse(typeof(UILanguage), language) : UILanguage.EN;
            var uri = localizationDictionaries[localization];
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
            Current = localization;
        }
    }
}
