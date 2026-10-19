// <copyright file="RedistributablePackageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System;
    using System.Diagnostics;

    /// <inheritdoc/>
    public class RedistributablePackageService : IRedistributablePackageService
    {
        /// <inheritdoc/>
        public void DeleteInstallerLogs(string logPattern)
            => Directory.GetFileSystemEntries(Path.GetTempPath(), logPattern, SearchOption.TopDirectoryOnly).ForEach(File.Delete);

        /// <inheritdoc/>
        public Version GetInstalledPackageVersionOrDefault(string name)
        {
            // Checking whether VC_redist builds installed
            var packageCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Package Cache");

            if (Directory.Exists(packageCache))
            {
                // Choose the first item if user has more than one package installed
                var installer = Directory.GetFileSystemEntries(packageCache, name, SearchOption.AllDirectories).FirstOrDefault();
                return installer is null ? new Version("0.0.0") : Version.Parse(FileVersionInfo.GetVersionInfo(installer).FileVersion!);
            }

            return new Version("0.0.0");
        }
    }
}
