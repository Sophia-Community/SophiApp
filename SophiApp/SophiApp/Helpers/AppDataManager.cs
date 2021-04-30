using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    public class AppDataManager
    {
        private const string DEBUG_LOG_EXT = "sad";
        private const string DESIGNER_HOME_PAGE = "https://instagram.com/user/VladimirNameless";
        private const string GITHUB_RELEASES = "https://api.github.com/repos/Sophia-Community/SophiApp/releases";
        private const string GITHUB_RELEASES_PAGE = "https://github.com/Sophia-Community/SophiApp/releases";
        private const string GITHUB_REPO = "https://github.com/Sophia-Community/SophiApp";

        //TODO: Set right designer homepage
        private const string NAME = "SophiApp";

        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.49";
        internal static string DebugLogPath { get => $"{StartupFolder}{Name}.{DEBUG_LOG_EXT}"; }
        public static string DesignerHomePage { get => DESIGNER_HOME_PAGE; }
        public static string GitHubReleases { get => GITHUB_RELEASES; }
        public static string GitHubReleasesPage { get => GITHUB_RELEASES_PAGE; }
        public static string GitHubRepo { get => GITHUB_REPO; }
        public static string Name { get => NAME; }
        public static string StartupFolder { get => AppDomain.CurrentDomain.BaseDirectory; }
        public static string UserAgent { get => USER_AGENT; }
        public static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version; }
    }
}