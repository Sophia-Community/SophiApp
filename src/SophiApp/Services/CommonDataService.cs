// <copyright file="CommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Reflection;
    using Microsoft.UI.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class CommonDataService : ICommonDataService
    {
        /// <summary>
        /// Gets Discord text.
        /// </summary>
        public const string DiscordText = "Discord";

        /// <summary>
        /// Gets GitHub text.
        /// </summary>
        public const string GitHubText = "GitHub";

        /// <summary>
        /// Gets Telegram text.
        /// </summary>
        public const string TelegramText = "Telegram";

        /// <summary>
        /// Gets app Discord link.
        /// </summary>
        public const string DiscordLink = "https://discord.gg/sSryhaEv79";

        /// <summary>
        /// Gets app GitHub link.
        /// </summary>
        public const string GitHubLink = "https://github.com/Sophia-Community/SophiApp";

        /// <summary>
        /// Gets app Telegram link.
        /// </summary>
        public const string TelegramLink = "https://t.me/sophia_chat";

        /// <summary>
        /// Gets app project manager name.
        /// </summary>
        public const string ProjectManagerName = "Dmitry \"farag\" Nefedov";

        /// <summary>
        /// Gets app developer name.
        /// </summary>
        public const string DeveloperName = "Dmitry \"Inestic\" Demin";

        /// <summary>
        /// Gets app UX developer name.
        /// </summary>
        public const string UXDeveloperName = "Vladimir Nameless";

        /// <summary>
        /// Gets app UI developer name.
        /// </summary>
        public const string UIDeveloperName = "Yaroslav Posmitiukh";

        /// <summary>
        /// Gets app animation developer name.
        /// </summary>
        public const string AnimationDeveloperName = "Maxim Nechiporenko";

        /// <summary>
        /// Gets app tester name.
        /// </summary>
        public const string AppTesterName = "Yevhenii \"lowlif3\" Zabronskyi";

#pragma warning disable S1075 // URIs should not be hardcoded

        /// <summary>
        /// Gets app project manager url.
        /// </summary>
        public const string ProjectManagerUrl = "https://github.com/farag2";

        /// <summary>
        /// Gets app developer url.
        /// </summary>
        public const string DeveloperUrl = "https://github.com/Inestic";

        /// <summary>
        /// Gets app UX developer url.
        /// </summary>
        public const string UXDeveloperUrl = "https://www.linkedin.com/in/vladimir-palii-132745a1";

        /// <summary>
        /// Gets app UI developer url.
        /// </summary>
        public const string UIDeveloperUrl = "https://www.linkedin.com/in/artenjoyers";

        /// <summary>
        /// Gets app UI developer url.
        /// </summary>
        public const string AnimationDeveloperUrl = "https://linktr.ee/crowmax";

        /// <summary>
        /// Gets app tester url.
        /// </summary>
        public const string AppTesterUrl = "https://github.com/lowl1f3";

#pragma warning restore S1075 // URIs should not be hardcoded

        private static InputCursor userCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        private static InputCursor urlCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        private readonly OsProperties osProperties;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDataService"/> class.
        /// </summary>
        /// <param name="networkService">A service for networking.</param>
        /// <param name="instrumentationService">Service for working with WMI.</param>
        public CommonDataService(INetworkService networkService, IInstrumentationService instrumentationService)
        {
            osProperties = instrumentationService.GetOsPropertiesOrDefault();
            IsOnline = networkService.IsOnline();
            App.Logger.LogAppProperties(version: assembly.Version!, directory: AppContext.BaseDirectory);
        }

        /// <summary>
        /// Gets or sets app user cursor.
        /// </summary>
        public static InputCursor UserCursor
        {
            get => userCursor;
            set
            {
                if (userCursor != value)
                {
                    userCursor = value;
                }
            }
        }

        /// <summary>
        /// Gets url hovering cursor.
        /// </summary>
        public static InputCursor UrlCursor
        {
            get => urlCursor;
        }

        /// <inheritdoc/>
        public bool IsOnline { get; init; }

        /// <inheritdoc/>
        public bool IsWindows11 { get => osProperties?.Caption.Contains("11") ?? false; }

        /// <inheritdoc/>
        public OsProperties OsProperties { get => osProperties; }

        /// <inheritdoc/>
        public string DetectedMalware { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string MsDefenderFileMissing { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string MsDefenderServiceStopped { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string AppVersionUrl => "https://raw.githubusercontent.com/Sophia-Community/SophiApp/master/sophiapp_versions.json";

        /// <inheritdoc/>
        public string AppReleaseUrl => "https://github.com/Sophia-Community/SophiApp/releases";

        /// <inheritdoc/>
        public Version AppVersion => assembly.Version!;

        /// <inheritdoc/>
        public string GetBuildName() => "Daria";

        /// <inheritdoc/>
        public string GetDelimiter() => "|";

        /// <inheritdoc/>
        public string GetFullName() => $"{assembly.Name} {assembly.Version!.Major}.{assembly.Version.Minor}.{assembly.Version.Build}";

        /// <inheritdoc/>
        public string GetName() => assembly.Name!;

        /// <inheritdoc/>
        public string GetVersionName() => "Community α";
    }
}
