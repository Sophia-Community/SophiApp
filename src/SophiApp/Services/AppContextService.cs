// <copyright file="AppContextService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Reflection;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class AppContextService : IAppContextService
    {
        /// <summary>
        /// Gets discord text.
        /// </summary>
        public const string DiscordText = "Discord";

        /// <summary>
        /// Gets github text.
        /// </summary>
        public const string GitHubText = "GitHub";

        /// <summary>
        /// Gets telegram text.
        /// </summary>
        public const string TelegramText = "Telegram";

        /// <summary>
        /// Gets app discord link.
        /// </summary>
        public const string DiscordLink = "https://discord.gg/sSryhaEv79";

        /// <summary>
        /// Gets app github link.
        /// </summary>
        public const string GitHubLink = "https://github.com/Sophia-Community/SophiApp";

        /// <summary>
        /// Gets app telegram link.
        /// </summary>
        public const string TelegramLink = "https://t.me/sophia_chat";

        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();

        /// <inheritdoc/>
        public string GetBuildName() => "Daria";

        /// <inheritdoc/>
        public string GetDelimiter() => "|";

        /// <inheritdoc/>
        public string GetFullName() => $"{assembly.Name} {assembly.Version!.Major}.{assembly.Version.Minor}.{assembly.Version.Build}";

        /// <inheritdoc/>
        public string GetVersionName() => "Community [Private alpha]";
    }
}
