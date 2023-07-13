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
        public const string Discord = "Discord";

        /// <summary>
        /// Gets github text.
        /// </summary>
        public const string GitHub = "GitHub";

        /// <summary>
        /// Gets telegram text.
        /// </summary>
        public const string Telegram = "Telegram";

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
