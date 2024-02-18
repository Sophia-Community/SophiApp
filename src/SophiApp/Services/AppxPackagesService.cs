// <copyright file="AppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using Windows.Management.Deployment;

    /// <inheritdoc/>
    public class AppxPackagesService : IAppxPackagesService
    {
        private readonly PackageManager packageManager = new ();

        /// <inheritdoc/>
        public bool PackageExist(string packageId, bool forAllUser = false)
        {
            if (forAllUser)
            {
                return packageManager.FindPackages()
                    .Any(package => package.Id.Name.Equals(packageId));
            }

            return packageManager.FindPackagesForUser(string.Empty)
                .Any(package => package.Id.Name.Equals(packageId));
        }
    }
}
