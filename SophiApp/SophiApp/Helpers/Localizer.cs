using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.Helpers
{
    internal class Localizer
    {
        private List<Localization> LocalizationsData = new List<Localization>()
        {
            { new Localization() { Text = "English", Language = UILanguage.EN, Uri = new Uri("pack://application:,,,/Localizations/EN.xaml", UriKind.Absolute)} },
            { new Localization() { Text = "Русский", Language = UILanguage.RU, Uri = new Uri("pack://application:,,,/Localizations/RU.xaml", UriKind.Absolute)} }
        };

        internal Localization Current;

        public Localizer()
        {
            var language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
            Current = GetByNameOrDefault(language);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Current.Uri });
        }

        private Localization GetByNameOrDefault(string name)
        {
            var parsedName = Enum.GetNames(typeof(UILanguage)).Contains(name) ? (UILanguage)Enum.Parse(typeof(UILanguage), name) : UILanguage.EN;
            return LocalizationsData.Find(l => l.Language == parsedName);
        }

        internal void Change(UILanguage language)
        {
            var resDict = Application.Current.Resources.MergedDictionaries.Where(d => d.Source == Current.Uri).First();
            var localization = LocalizationsData.Find(l => l.Language == language);
            resDict.Source = localization.Uri;
            Current = localization;
        }

        internal List<string> GetText() => LocalizationsData.Select(l => l.Text).ToList();
    }
}