using SophiAppCE.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.Helpers
{
    internal class AppLocalization
    {
        private readonly Dictionary<LanguageName, Uri> languagesUri = new Dictionary<LanguageName, Uri>()
        {
            { LanguageName.RU, new Uri(Tags.LocalizationUriRU, UriKind.Absolute) },
            { LanguageName.EN, new Uri(Tags.LocalizationUriEN, UriKind.Absolute) }
        };

        private LanguageName appLocalization;

        public AppLocalization()
        {
            SetLanguage();
        }

        private void SetLanguage()
        {
            try
            {
                appLocalization = (LanguageName)Enum.Parse(typeof(LanguageName), Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper());
            }
            catch (ArgumentException)
            {
                appLocalization = LanguageName.EN;
            }

            if (appLocalization != LanguageName.RU)
            {
                ResourceDictionary localization = Application.Current.Resources.MergedDictionaries.Where(d => d.Source.AbsoluteUri == Tags.LocalizationUriRU).First();
                localization.Source = languagesUri[appLocalization];
            }            
        }

        private void SetLanguage(LanguageName language)
        {
            ResourceDictionary resource = Application.Current.Resources.MergedDictionaries.Where(d => d.Source == languagesUri[appLocalization]).First();            
            resource.Source = languagesUri[language];
            appLocalization = language;
        }

        internal LanguageName Language
        {
            get => appLocalization;
            set => SetLanguage(value);
        }        
    }
}
