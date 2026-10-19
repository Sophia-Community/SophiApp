// <copyright file="IRedistributablePackageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with redistributable package API.
    /// </summary>
    public interface IRedistributablePackageService
    {
        /// <summary>
        /// Delete offline installer log files.
        /// </summary>
        /// <param name="logPattern">The search pattern to be compared against the log file names.</param>
        void DeleteInstallerLogs(string logPattern);

        /// <summary>
        /// Get package offline installer version or default.
        /// </summary>
        /// <param name="name">Package installer file name.</param>
        Version GetInstalledPackageVersionOrDefault(string name);
    }
}
