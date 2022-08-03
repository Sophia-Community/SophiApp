using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SophiApp.Watchers
{
    internal class RegistryWatcher
    {
        private const string PERSONALIZE_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        private const string USES_LIGHT_THEME = "AppsUseLightTheme";

        private static readonly object locked = new object();
        private static volatile RegistryWatcher instance;
        private static byte systemTheme = RegHelper.GetByteValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, USES_LIGHT_THEME);

        private RegistryWatcher()
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

        internal static RegistryWatcher GetInstance()
        {
            if (instance == null)
            {
                lock (locked)
                {
                    if (instance == null)
                        instance = new RegistryWatcher();
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