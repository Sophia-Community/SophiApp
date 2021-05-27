using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppData
    {
        private const string APP_NAME = "SophiApp";
        private const string GITHUB_API_RELEASES = "https://api.github.com/repos/Sophia-Community/SophiApp/releases";
        private const string GITHUB_RELEASES_PAGE = "https://github.com/Sophia-Community/SophiApp/releases";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.49";

        internal static string GitHubApiReleases { get => GITHUB_API_RELEASES; }
        internal static string GitHubReleasesPage { get => GITHUB_RELEASES_PAGE; }
        internal static string StartupFolder { get => AppDomain.CurrentDomain.BaseDirectory; }
        internal static string UserAgent { get => USER_AGENT; }
        internal static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version; }

        public static string AppName { get => APP_NAME; }
    }
}