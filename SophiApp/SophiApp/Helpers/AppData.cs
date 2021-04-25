using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    public class AppData
    {
        public static string Name { get => "SophiApp"; }

        public static string StartupFolder { get => AppDomain.CurrentDomain.BaseDirectory; }

        public static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version; }

        public static string VersionString { get => Version.ToString().Substring(0, 5); }
    }
}