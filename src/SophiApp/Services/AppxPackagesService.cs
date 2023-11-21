// <copyright file="AppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using Windows.Management.Deployment;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class AppxPackagesService : IAppxPackagesService
    {
        private readonly PackageManager packageManager = new ();

        /// <inheritdoc/>
        public bool PackageExist(string id, bool forAllUser = false)
        {
            if (forAllUser)
            {
                return packageManager.FindPackages().FirstOrDefault(p => p.Id.Name.Equals(id)) is not null;
            }

            return packageManager.FindPackagesForUser(string.Empty).FirstOrDefault(p => p.Id.Name.Equals(id)) is not null;
        }
    }
}
