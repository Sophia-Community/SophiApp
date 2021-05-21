using Microsoft.Win32;
using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SophiApp.Helpers
{
    internal class ThemesHelper
    {
        private const string DARK_THEME_URI = "pack://application:,,,/Themes/Dark.xaml";
        private const int DARK_THEME_VALUE = 0;
        private const string LIGHT_THEME_URI = "pack://application:,,,/Themes/Light.xaml";
        private const int LIGHT_THEME_VALUE = 1;
        private const string THEME_REGISTRY_VALUE_NAME = "AppsUseLightTheme";
        private const string THEME_REGISTRY_VALUE_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private static readonly string DARK_THEME_ALIAS = "DARK";
        private static readonly string DARK_THEME_NAME = Application.Current.FindResource("Localization.Settings.Themes.Dark") as string;
        private static readonly string LIGHT_THEME_ALIAS = "LIGHT";
        private static readonly string LIGHT_THEME_NAME = Application.Current.FindResource("Localization.Settings.Themes.Light") as string;

        private List<Theme> ThemesData = new List<Theme>()
        {
            new Theme() {Alias = LIGHT_THEME_ALIAS, Name =  LIGHT_THEME_NAME, Uri = new Uri(LIGHT_THEME_URI, UriKind.Absolute) },
            new Theme() {Alias = DARK_THEME_ALIAS, Name = DARK_THEME_NAME, Uri = new Uri(DARK_THEME_URI, UriKind.Absolute)}
        };

        internal Theme Selected;

        public ThemesHelper()
        {
            var isDarkTheme = GetDarkThemeValue();
            Selected = FindName(isDarkTheme ? DARK_THEME_NAME : LIGHT_THEME_NAME);
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Selected.Uri });
        }

        private bool GetDarkThemeValue()
        {
            try
            {
                var themeValue = Registry.CurrentUser.OpenSubKey(THEME_REGISTRY_VALUE_PATH).GetValue(THEME_REGISTRY_VALUE_NAME) ?? LIGHT_THEME_VALUE;
                return themeValue.Equals(DARK_THEME_VALUE);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal void Change(Theme theme)
        {
            var resDict = Application.Current.Resources.MergedDictionaries.Where(d => d.Source == Selected.Uri).First();
            resDict.Source = theme.Uri;
            Selected = theme;
        }

        internal Theme FindName(string name) => ThemesData.Find(t => t.Name == name);

        internal List<string> GetNames() => ThemesData.Select(t => t.Name).ToList();
    }
}
