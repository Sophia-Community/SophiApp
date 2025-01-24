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
        public AppxPackagesService(IPowerShellService powerShellService)
        {
            this.powerShellService = powerShellService;
        }

        /// <inheritdoc/>
        public bool PackageExist(string packageIdName, bool forAllUser = false)
        {
            if (forAllUser)
            {
                return packageManager.FindPackages()
                    .Any(package => package.Id.Name.Equals(packageIdName));
            }

            return packageManager.FindPackagesForUser(string.Empty)
                .Any(package => package.Id.Name.Equals(packageIdName));
        }

        /// <inheritdoc/>
        public List<Package> GetPackages(bool forAllUsers = false)
        {
            var appxPackages = new List<Package>();
            var packages = new List<Package>();
            var allUsersScript = "Get-AppxPackage -PackageTypeFilter Bundle -AllUsers | Select-Object -ExpandProperty Name";
            var currentUserScript = "Get-AppxPackage -PackageTypeFilter Bundle | Select-Object -ExpandProperty Name";
            var bundles = powerShellService.Invoke(forAllUsers ? allUsersScript : currentUserScript);
            packages = [.. forAllUsers ? packageManager.FindPackages() : packageManager.FindPackagesForUser(string.Empty)];

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
        public void RemovePackage(string packageName, bool forAllUsers)
        {
            var allUsersScript = $"Get-AppxPackage -Name *{packageName}* -PackageTypeFilter Bundle -AllUsers | Remove-AppxPackage -AllUsers";
            var currentUserScript = $"Get-AppxPackage -Name *{packageName}* -PackageTypeFilter Bundle | Remove-AppxPackage";
            _ = powerShellService.Invoke(forAllUsers ? allUsersScript : currentUserScript);
        }

        /// <inheritdoc/>
        public async Task InstallFromFileAsync(string appxPath)
        {
            await packageManager.AddPackageAsync(new Uri(appxPath), null, DeploymentOptions.None);
        }
    }
}
