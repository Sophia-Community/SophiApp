// <copyright file="AppContextService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Reflection;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class AppContextService : IAppContextService
    {
        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string GetFullName() => $"{assembly.Name} {assembly.Version!.Major}.{assembly.Version.Minor}.{assembly.Version.Build} | Private alpha";
    }
}
