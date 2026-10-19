// <copyright file="AppxPackagesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Collections.Generic;
    using SophiApp.Contracts.Services;
    using Windows.ApplicationModel;
    using Windows.Management.Deployment;

    /// <inheritdoc/>
    public class AppxPackagesService : IAppxPackagesService
    {
        private readonly PackageManager packageManager = new ();
        private readonly IPowerShellService powerShellService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppxPackagesService"/> class.
        /// </summary>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        public AppxPackagesService(IPowerShellService powerShellService) => this.powerShellService = powerShellService;

        /// <inheritdoc/>
        public bool PackageExist(string packageId, bool allUsers = false)
        {
            var packages = allUsers ? packageManager.FindPackages() : packageManager.FindPackagesForUser(string.Empty);
            return packages.Any(package => package.Id.Name.Equals(packageId));
        }

        /// <inheritdoc/>
        public List<Package> GetPackages(bool allUsers = false)
        {
            var appxPackages = new List<Package>();
            var packages = new List<Package>();
            var allUsersScript = "Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Select-Object -ExpandProperty Name";
            var currentUserScript = "Get-AppxPackage -PackageTypeFilter Bundle | Select-Object -ExpandProperty Name";
            var bundles = powerShellService.Invoke(allUsers ? allUsersScript : currentUserScript);
            packages = [.. allUsers ? packageManager.FindPackages() : packageManager.FindPackagesForUser(string.Empty)];

            for (int i = 0; i < packages.Count; i++)
            {
                var bundlesIndex = bundles.FindIndex(b => b.BaseObject.Equals(packages[i].Id.Name));

                if (bundlesIndex >= 0)
                {
                    appxPackages.Add(packages[i]);
                    bundles.RemoveAt(bundlesIndex);
                }
            }

            return appxPackages;
        }

        /// <inheritdoc/>
        public void RemovePackage(string packageId, bool allUsers = false)
        {
            var allUsersScript = $"Get-AppxPackage -Name *{packageId}* -PackageTypeFilter Bundle -AllUsers | Remove-AppxPackage -AllUsers";
            var currentUserScript = $"Get-AppxPackage -Name *{packageId}* -PackageTypeFilter Bundle | Remove-AppxPackage";
            _ = powerShellService.Invoke(allUsers ? allUsersScript : currentUserScript);
        }
    }
}
