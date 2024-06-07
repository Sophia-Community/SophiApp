// <copyright file="IAppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Models;
    using Windows.ApplicationModel;

    /// <summary>
    /// A service for working with appx packages API.
    /// </summary>
    public interface IAppxPackagesService
    {
        /// <summary>
        /// Checks that the package is installed.
        /// </summary>
        /// <param name="packageIdName">The Id of the package being checked, not to be confused with the package Display name.</param>
        /// <param name="forAllUser">Search in installed packages for all users or only for the current user.</param>
        bool PackageExist(string packageIdName, bool forAllUser = false);

        /// <summary>
        /// Retrieves information about a appx packages.
        /// </summary>
        /// <param name="forAllUsers">Get collection of UWP <see cref="UIModel"/> for all users, otherwise only for the current user.</param>
        List<Package> GetPackages(bool forAllUsers = false);

        /// <summary>
        /// Removes appx package.
        /// </summary>
        /// <param name="packageName">The appx package identity name.</param>
        /// <param name="forAllUsers">Remove a package for all users or current user only.</param>
        void RemovePackage(string packageName, bool forAllUsers);
    }
}
