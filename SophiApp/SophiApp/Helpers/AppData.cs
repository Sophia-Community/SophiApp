using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppData
    {
        private const string APP_NAME = "SophiApp";
        private const string CODER_LINK = "https://github.com/Inestic";
        private const string DEBUG_EXT = "txt";        
        private const string DESIGNER_LINK = "https://www.linkedin.com/in/vladimir-nameless-132745a1/";
        private const string GITHUB_API_RELEASES = "https://api.github.com/repos/Sophia-Community/SophiApp/releases";
        private const string GITHUB_RELEASES_PAGE = "https://github.com/Sophia-Community/SophiApp/releases";
        private const string GITHUB_REPO = "https://github.com/Sophia-Community/SophiApp";
        private const string LEADER_LINK = "https://github.com/farag2";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.49";
        internal static string DebugFilePath { get => $"{StartupFolder}{APP_NAME}.{DEBUG_EXT}"; }
        internal static string GitHubApiReleases => GITHUB_API_RELEASES;
        internal static string StartupFolder => AppDomain.CurrentDomain.BaseDirectory;
        internal static string UserAgent => USER_AGENT;
        public static string AppName => APP_NAME;
        public static string CoderLink { get => CODER_LINK; }
        public static string DesignerLink { get => DESIGNER_LINK; }
        public static string GitHubReleasesPage => GITHUB_RELEASES_PAGE;
        public static string GitHubRepo => GITHUB_REPO;
        public static string LeaderLink { get => LEADER_LINK; }
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}