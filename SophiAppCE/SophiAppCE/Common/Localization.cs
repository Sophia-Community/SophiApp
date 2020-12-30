using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Common
{
    internal class Localization
    {
        private Dictionary<LanguageFamily, Uri> localizedUri = new Dictionary<LanguageFamily, Uri>()
        {
            { LanguageFamily.RU, new Uri("pack://application:,,,/Localization/RU.xaml", UriKind.Absolute) },
            { LanguageFamily.EN, new Uri("pack://application:,,,/Localization/EN.xaml", UriKind.Absolute) }
        };

        private Uri currentLangUri;        

        public LanguageFamily Current { get => localizedUri.Where(k => k.Value == currentLangUri).First().Key; }

        public Localization()
        {
            InitializeLocalization();
        }

        private void InitializeLocalization()
        {
            currentLangUri = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == nameof(LanguageFamily.RU)
                                                                                                       ? localizedUri[LanguageFamily.RU]
                                                                                                       : localizedUri[LanguageFamily.EN];

            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = currentLangUri });
        }

        public void Change(Action callBack)
        {
            ResourceDictionary resDict = Application.Current.Resources.MergedDictionaries.Where(d => d.Source == currentLangUri).First();
            currentLangUri = localizedUri[Current == LanguageFamily.RU ? LanguageFamily.EN : LanguageFamily.RU];
            resDict.Source = currentLangUri;
            callBack.BeginInvoke(null, null);
        }
    }
}
