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
        private const string CZ_NAME = "čeština";
        private const string CZ_URI = "pack://application:,,,/Localizations/CZ.xaml";
        private const string DE_NAME = "Deutsche";
        private const string DE_URI = "pack://application:,,,/Localizations/DE.xaml";
        private const string EN_NAME = "English";
        private const string EN_URI = "pack://application:,,,/Localizations/EN.xaml";
        private const string ES_NAME = "Español";
        private const string ES_URI = "pack://application:,,,/Localizations/ES.xaml";
        private const string FR_NAME = "Français";
        private const string FR_URI = "pack://application:,,,/Localizations/FR.xaml";
        private const string IT_NAME = "Italiano";
        private const string IT_URI = "pack://application:,,,/Localizations/IT.xaml";
        private const string PL_NAME = "Polski";
        private const string PL_URI = "pack://application:,,,/Localizations/PL.xaml";
        private const string RU_NAME = "Русский";
        private const string RU_URI = "pack://application:,,,/Localizations/RU.xaml";
        private const string TR_NAME = "Turkçe";
        private const string TR_URI = "pack://application:,,,/Localizations/TR.xaml";
        private const string UA_NAME = "Українська";
        private const string UA_URI = "pack://application:,,,/Localizations/UA.xaml";
        private const string zh_CN_NAME = "汉语";
        private const string zh_CN_URI = "pack://application:,,,/Localizations/zh_CN.xaml";

        private List<Localization> LocalizationsData = new List<Localization>()
        {
            { new Localization() { Name = CZ_NAME, Language = UILanguage.CZ, Uri = new Uri(CZ_URI, UriKind.Absolute)} },
            { new Localization() { Name = DE_NAME, Language = UILanguage.DE, Uri = new Uri(DE_URI, UriKind.Absolute)} },
            { new Localization() { Name = EN_NAME, Language = UILanguage.EN, Uri = new Uri(EN_URI, UriKind.Absolute)} },
            { new Localization() { Name = ES_NAME, Language = UILanguage.ES, Uri = new Uri(ES_URI, UriKind.Absolute)} },
            { new Localization() { Name = FR_NAME, Language = UILanguage.FR, Uri = new Uri(FR_URI, UriKind.Absolute)} },
            { new Localization() { Name = IT_NAME, Language = UILanguage.IT, Uri = new Uri(IT_URI, UriKind.Absolute)} },
            { new Localization() { Name = PL_NAME, Language = UILanguage.PL, Uri = new Uri(PL_URI, UriKind.Absolute)} },
            { new Localization() { Name = RU_NAME, Language = UILanguage.RU, Uri = new Uri(RU_URI, UriKind.Absolute)} },
            { new Localization() { Name = TR_NAME, Language = UILanguage.TR, Uri = new Uri(TR_URI, UriKind.Absolute)} },
            { new Localization() { Name = UA_NAME, Language = UILanguage.UA, Uri = new Uri(UA_URI, UriKind.Absolute)} },
            { new Localization() { Name = zh_CN_NAME, Language = UILanguage.zh_CN, Uri = new Uri(zh_CN_URI, UriKind.Absolute)} }
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