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
        /// Get the highest version of the package offline installers in the "Package Cache" folder.
        /// </summary>
        /// <param name="name">Package installer file name.</param>
        /// <returns>The installed version, or null if the package is not installed.</returns>
        Version? GetInstalledVersion(string name);

        /// <summary>
        /// Download the package offline installer to the user's Downloads folder, run it, and then delete the installer and its log files.
        /// </summary>
        /// <param name="downloadUrl">The installer download url.</param>
        /// <param name="installerName">Package installer file name.</param>
        /// <param name="logPattern">The search pattern to be compared against the log file names.</param>
        void Install(string downloadUrl, string installerName, string logPattern);

        /// <summary>
        /// Determines whether a package offline installer exists in the "Package Cache" folder.
        /// </summary>
        /// <param name="name">Package installer file name.</param>
        bool IsInstalled(string name);
    }
}
