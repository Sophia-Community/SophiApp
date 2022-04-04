using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.Helpers
{
    internal class LocalizationsHelper
    {
        private const string DE_NAME = "Deutsche";
        private const string DE_URI = "pack://application:,,,/Localizations/DE.xaml";
        private const string EN_NAME = "English";
        private const string EN_URI = "pack://application:,,,/Localizations/EN.xaml";
        private const string RU_NAME = "Русский";
        private const string RU_URI = "pack://application:,,,/Localizations/RU.xaml";
        private const string UA_NAME = "Українська";
        private const string UA_URI = "pack://application:,,,/Localizations/UA.xaml";

        private List<Localization> LocalizationsData = new List<Localization>()
        {
            { new Localization() { Name = DE_NAME, Language = UILanguage.DE, Uri = new Uri(DE_URI, UriKind.Absolute)} },
            { new Localization() { Name = EN_NAME, Language = UILanguage.EN, Uri = new Uri(EN_URI, UriKind.Absolute)} },
            { new Localization() { Name = RU_NAME, Language = UILanguage.RU, Uri = new Uri(RU_URI, UriKind.Absolute)} },
            { new Localization() { Name = UA_NAME, Language = UILanguage.UA, Uri = new Uri(UA_URI, UriKind.Absolute)} }
        };

        internal Localization Selected;

        public LocalizationsHelper()
        {
            var language = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper();
            Selected = FindNameOrDefault(language);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Selected.Uri });
        }

        private Localization FindNameOrDefault(string name)
        {
            var parsedName = Enum.GetNames(typeof(UILanguage)).Contains(name) ? (UILanguage)Enum.Parse(typeof(UILanguage), name) : UILanguage.EN;
            return LocalizationsData.Find(localization => localization.Language == parsedName);
        }

        internal void Change(Localization localization)
        {
            var resDict = Application.Current.Resources.MergedDictionaries.Where(d => d.Source == Selected.Uri).First();
            resDict.Source = localization.Uri;
            Selected = localization;
        }

        internal Localization FindName(string text) => LocalizationsData.Find(localization => localization.Name == text);

        internal List<string> GetNames() => LocalizationsData.Select(localization => localization.Name).ToList();
    }
}