// <copyright file="IAppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;
    using Windows.ApplicationModel;

    /// <summary>
    /// A service for working with appx packages API.
    /// </summary>
    public interface IAppxPackagesService
    {
        /// <summary>
        /// Gets by ID the package if installed.
        /// </summary>
        /// <param name="packageId">The ID of the package being checked, not to be confused with the package Display name.</param>
        /// <param name="allUsers">Search in installed packages for all users or only for the current user.</param>
        bool PackageExist(string packageId, bool allUsers = false);

        /// <summary>
        /// Gets the UWP apps that can be uninstalled, sorted by their localized names.
        /// </summary>
        /// <param name="allUsers">Search in installed packages for all users or only for the current user.</param>
        List<UwpPackage> GetRemovablePackages(bool allUsers);

        /// <summary>
        /// Get the package of the current user by its name.
        /// </summary>
        /// <param name="packageId">Package name, e.g. Microsoft.WindowsTerminal.</param>
        Package? GetPackageOrDefault(string packageId);

        /// <summary>
        /// Removes appx package.
        /// </summary>
        /// <param name="packageId">The appx package identity name.</param>
        /// <param name="allUsers">Remove a package for all users or current user only.</param>
        void RemovePackage(string packageId, bool allUsers = false);

        /// <summary>
        /// Removes appx packages by their full names.
        /// </summary>
        /// <param name="packageFullNames">Full names of the packages.</param>
        /// <param name="allUsers">Remove the packages for all users or current user only.</param>
        void RemovePackages(IEnumerable<string> packageFullNames, bool allUsers);
    }
}
