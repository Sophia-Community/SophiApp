using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    public class AppDataManager
    {
        private const string CODER_LINK = "https://github.com/Inestic";
        private const string DEBUG_LOG_EXT = "txt";

        //TODO: Set right designer homepage
        private const string DESIGNER_LINK = "https://instagram.com/user/VladimirNameless";

        private const string GITHUB_RELEASES = "https://api.github.com/repos/Sophia-Community/SophiApp/releases";
        private const string GITHUB_RELEASES_PAGE = "https://github.com/Sophia-Community/SophiApp/releases";
        private const string GITHUB_REPO = "https://github.com/Sophia-Community/SophiApp";
        private const string NAME = "SophiApp";
        private const string TEAM_LEADER_LINK = "https://github.com/farag2";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.49";

        internal static string DebugLogPath { get => $"{StartupFolder}{Name}.{DEBUG_LOG_EXT}"; }
        public static string CoderLink { get => CODER_LINK; }
        public static string DesignerLink { get => DESIGNER_LINK; }
        public static string GitHubReleases { get => GITHUB_RELEASES; }
        public static string GitHubReleasesPage { get => GITHUB_RELEASES_PAGE; }
        public static string GitHubRepo { get => GITHUB_REPO; }
        public static string Name { get => NAME; }
        public static string StartupFolder { get => AppDomain.CurrentDomain.BaseDirectory; }
        public static string TeamLeaderLink { get => TEAM_LEADER_LINK; }
        public static string UserAgent { get => USER_AGENT; }
        public static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version; }
    }
}