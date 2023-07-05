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
        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();

        /// <inheritdoc/>
        public string GetBuildName() => "Daria";

        /// <inheritdoc/>
        public string GetFullName() => $"{assembly.Name} {assembly.Version!.Major}.{assembly.Version.Minor}.{assembly.Version.Build}";

        /// <inheritdoc/>
        public string GetVersionName() => "Community";
    }
}
