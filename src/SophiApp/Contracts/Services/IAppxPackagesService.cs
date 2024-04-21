// <copyright file="IAppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with appx packages API.
    /// </summary>
    public interface IAppxPackagesService
    {
        /// <summary>
        /// Checks that the package is installed in the OS.
        /// </summary>
        /// <param name="packageId">The Id of the package being checked, not to be confused with the package Display name.</param>
        /// <param name="forAllUser">Search in installed packages for all users or only for the current user.</param>
        bool PackageExist(string packageId, bool forAllUser = false);
    }
}
