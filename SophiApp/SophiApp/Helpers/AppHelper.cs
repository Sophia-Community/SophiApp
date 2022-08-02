using System;
using System.Reflection;

namespace SophiApp.Helpers
{
    internal class AppHelper
    {
        private const string CODER_LINK = "https://github.com/Inestic";
        private const string DEBUG_EXT = "txt";
        private const string DESIGNER_LINK = "https://www.linkedin.com/in/vladimir-nameless-132745a1/";
        private const string GITHUB_RELEASES_PAGE = "https://github.com/Sophia-Community/SophiApp/releases";
        private const string GITHUB_REPO = "https://github.com/Sophia-Community/SophiApp";
        private const string TELEGRAM_LINK = "https://t.me/sophia_chat";
        private const bool IS_RELEASE = true;
        private const string LEADER_LINK = "https://github.com/farag2";
        private const string LOGOTYPE_CREATOR_LINK = "https://www.linkedin.com/mwlite/in/наталия-гуменюк-ba4a04161";
        private const string SOPHIAPP_VERSIONS_JSON = "https://raw.githubusercontent.com/Sophia-Community/SophiApp/master/sophiapp_versions.json";
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36 Edg/90.0.818.49";
        private static readonly string APP_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string FRAMEWORK_LOG = Environment.ExpandEnvironmentVariables(@"%LOCALAPPDATA%\Microsoft\CLR_v4.0\UsageLogs\SophiApp.exe.log");
        private static readonly string LOGS_FOLDER = "Logs";
        internal static string AppFrameworkLog => FRAMEWORK_LOG;
        internal static string DebugFile => $@"{StartupFolder}{LOGS_FOLDER}\{APP_NAME}-{Environment.MachineName}-{DateTime.Now.ToFileTime()}.{DEBUG_EXT}";
        internal static string SophiAppVersionsJson => SOPHIAPP_VERSIONS_JSON;
        internal static string StartupFolder => AppDomain.CurrentDomain.BaseDirectory;
        internal static string UserAgent => USER_AGENT;
        public static string AppName => APP_NAME;
        public static string CoderLink => CODER_LINK;
        public static string DesignerLink => DESIGNER_LINK;
        public static string GitHubReleasesPage => GITHUB_RELEASES_PAGE;
        public static string GitHubRepo => GITHUB_REPO;
        public static string TelegramLink => TELEGRAM_LINK;
        public static bool IsRelease => IS_RELEASE;
        public static string LeaderLink => LEADER_LINK;
        public static string LogotypeCreatorLink => LOGOTYPE_CREATOR_LINK;

        public static string ShortVersion
        {
            get
            {
                var version = Version;
                return $"{version.Major}.{version.Minor}.{version.Build}";
            }
        }

        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;
    }
}