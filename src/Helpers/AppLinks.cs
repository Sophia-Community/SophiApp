// <copyright file="AppLinks.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// The web addresses of the app, of its team and of the manuals it opens. They are the same in every language.
    /// </summary>
    public static class AppLinks
    {
        /// <summary>
        /// The page of the app on GitHub.
        /// </summary>
        public const string GitHub = $"https://github.com/{Repository}";

        /// <summary>
        /// The latest release of the app.
        /// </summary>
        public const string LatestRelease = $"{GitHub}/releases/latest";

        /// <summary>
        /// The file with the latest version of the app. It is read at the start.
        /// </summary>
        public const string ReleasesData = $"{RepositoryFiles}/SophiApp_Releases.json";

        /// <summary>
        /// The file with the Windows builds the app supports. It is read at the start.
        /// </summary>
        public const string SupportedBuildsData = $"{RepositoryFiles}/Windows_Builds.json";

        /// <summary>
        /// The Telegram chat of the app.
        /// </summary>
        public const string Telegram = "https://t.me/sophia_chat";

        /// <summary>
        /// The Discord server of the app.
        /// </summary>
        public const string Discord = "https://discord.gg/sSryhaEv79";

        /// <summary>
        /// The page of the project manager.
        /// </summary>
        public const string ProjectManager = "https://github.com/farag2";

        /// <summary>
        /// The page of the lead developer.
        /// </summary>
        public const string Developer = "https://github.com/Inestic";

        /// <summary>
        /// The page of the UI/UX designer.
        /// </summary>
        public const string UxDeveloper = "https://www.linkedin.com/in/vladimir-nameless-132745a1/";

        /// <summary>
        /// The page of the tester.
        /// </summary>
        public const string Tester = "https://github.com/lowl1f3";

        /// <summary>
        /// The page of the person who supports the app.
        /// </summary>
        public const string Supporter = "https://github.com/BenchTweakGaming";

        /// <summary>
        /// How to update the Secure Boot certificates.
        /// </summary>
        public const string UpdateUefiCertificates = "https://techcommunity.microsoft.com/blog/windows-itpro-blog/updating-microsoft-secure-boot-keys/4055324";

        /// <summary>
        /// What the 2023 UEFI certificates are signed with.
        /// </summary>
        public const string SigningUefiCertificates = "https://techcommunity.microsoft.com/blog/hardware-dev-center/signing-with-the-new-2023-microsoft-uefi-certificates-what-submitters-need-to-kn/4455787";

        /// <summary>
        /// How to turn Secure Boot on. The address has no language, so Microsoft opens the page in the language of the reader.
        /// </summary>
        public const string EnableSecureBoot = "https://support.microsoft.com/windows/security/devicesecurity/windows-11-and-secure-boot";

        // The repository of the app and its branch the data files are read from
        private const string Repository = "Sophia-Community/SophiApp";
        private const string RepositoryFiles = $"https://raw.githubusercontent.com/{Repository}/refs/heads/dev-SophiApp2";
    }
}
