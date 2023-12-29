// <copyright file="IAppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with appx packages.
    /// </summary>
    public interface IAppxPackagesService
    {
        /// <summary>
        /// Checks that the package is installed in the OS.
        /// </summary>
        /// <param name="id">The ID name of the package being checked, not to be confused with the Display name.</param>
        /// <param name="forAllUser">Search in installed packages for all users or only for the current user.</param>
        bool PackageExist(string id, bool forAllUser = false);
    }
}
