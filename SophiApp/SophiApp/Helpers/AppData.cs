using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppData
    {
        private const string APP_NAME = "SophiApp";

        internal static string StartupFolder { get => AppDomain.CurrentDomain.BaseDirectory; }
        internal static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version; }
        public static string AppName { get => APP_NAME; }
    }
}