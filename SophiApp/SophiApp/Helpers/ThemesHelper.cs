using Microsoft.Win32;
using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SophiApp.Helpers
{
    internal class ThemesHelper
    {
        private const string DARK_THEME_URI = "pack://application:,,,/Themes/Dark.xaml";
        private const string LIGHT_THEME_URI = "pack://application:,,,/Themes/Light.xaml";
        private const string THEME_REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string THEME_REGISTRY_VALUE = "AppsUseLightTheme";
        private static readonly string DARK_THEME_ALIAS = "DARK";
        private static readonly string LIGHT_THEME_ALIAS = "LIGHT";
        private Theme selectedTheme;

        public ThemesHelper()
        {
            Init();
        }

        internal Theme SelectedTheme
        {
            get => Themes.Find(theme => theme.Uri == selectedTheme.Uri);
            private set => selectedTheme = value;
        }

        internal List<Theme> Themes => new List<Theme>()
        {
            { new Theme() { Alias = DARK_THEME_ALIAS, Name = Application.Current.FindResource("Localization.Settings.Themes.Dark") as string, Uri = new Uri(DARK_THEME_URI, UriKind.Absolute) } },
            { new Theme() { Alias = LIGHT_THEME_ALIAS, Name = Application.Current.FindResource("Localization.Settings.Themes.Light") as string, Uri = new Uri(LIGHT_THEME_URI, UriKind.Absolute) } }
        };

        private void Init()
        {
            var regThemeValue = RegHelper.GetByteValue(RegistryHive.CurrentUser, THEME_REGISTRY_PATH, THEME_REGISTRY_VALUE);
            SelectedTheme = Themes[regThemeValue];
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = SelectedTheme.Uri });
        }

        internal void ChangeTheme(Theme theme)
        {
            var dictionary = Application.Current.Resources.MergedDictionaries.First(dict => dict.Source == SelectedTheme.Uri);
            dictionary.Source = theme.Uri;
            SelectedTheme = theme;
        }

        internal Theme Find(string name) => Themes.Find(theme => theme.Name == name);
    }
}