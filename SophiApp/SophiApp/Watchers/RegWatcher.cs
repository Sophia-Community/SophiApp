using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SophiApp.Watchers
{
    internal class RegWatcher
    {
        private const string PERSONALIZE_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string USES_LIGHT_THEME = "AppsUseLightTheme";

        private static readonly object locked = new object();
        private static RegWatcher instance;
        private static byte systemTheme = RegHelper.GetByteValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, USES_LIGHT_THEME);

        private RegWatcher()
        {
        }

        internal event EventHandler<byte> SystemThemeChangedEvent;

        private void SystemThemeChanged()
        {
            var currentTheme = RegHelper.GetByteValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, USES_LIGHT_THEME);

            if (currentTheme != systemTheme)
            {
                systemTheme = currentTheme;
                SystemThemeChangedEvent?.Invoke(null, currentTheme);
            }
        }

        internal static RegWatcher GetInstance()
        {
            if (instance == null)
            {
                lock (locked)
                {
                    if (instance == null)
                        instance = new RegWatcher();
                }
            }
            return instance;
        }

        internal Task Start()
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    SystemThemeChanged();
                    Thread.Sleep(2500);
                }
            });
        }
    }
}