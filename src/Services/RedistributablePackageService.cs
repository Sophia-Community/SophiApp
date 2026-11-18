// <copyright file="RedistributablePackageService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System;
    using System.Diagnostics;

    /// <inheritdoc/>
    public class RedistributablePackageService : IRedistributablePackageService
    {
        private static readonly string PackageCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Package Cache");
        private readonly IHttpService httpService;
        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedistributablePackageService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        public RedistributablePackageService(IHttpService httpService, IProcessService processService)
        {
            this.httpService = httpService;
            this.processService = processService;
        }

        /// <inheritdoc/>
        public Version? GetInstalledVersion(string name)
        {
            // Choose the highest version if user has more than one package installed. Null if there is no installer
            return GetInstallers(name)
                .Select(installer => Version.TryParse(FileVersionInfo.GetVersionInfo(installer).FileVersion, out var version) ? version : null)
                .Max();
        }

        /// <inheritdoc/>
        public void Install(string downloadUrl, string installerName, string logPattern)
        {
            var downloadFolder = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}") as string;
            var offlineInstaller = Path.Combine(downloadFolder!, installerName);

            try
            {
                httpService.DownloadFile(downloadUrl, offlineInstaller);
                App.Status.SetStep("Panel_Step_Installing");
                using var installer = processService.WaitForExit(offlineInstaller, "/install /passive /norestart");
            }
            finally
            {
                // The installer and its logs are removed even if the download or the installation failed
                offlineInstaller.TryDelete();
                DeleteInstallerLogs(logPattern);
            }
        }

        /// <inheritdoc/>
        public bool IsInstalled(string name) => GetInstallers(name).Any();

        private static void DeleteInstallerLogs(string logPattern)
        {
            // A log that cannot be deleted is skipped
            foreach (var log in Directory.EnumerateFiles(Path.GetTempPath(), logPattern, SearchOption.TopDirectoryOnly))
            {
                log.TryDelete();
            }
        }

        /// <summary>
        /// Gets the package installers from the "Package Cache" folder.
        /// </summary>
        /// <param name="name">Package installer file name. Can contain wildcards.</param>
        private static IEnumerable<string> GetInstallers(string name)
        {
            // Installers are kept in the first-level subfolders of "Package Cache"
            return Directory.Exists(PackageCache)
                ? Directory.EnumerateDirectories(PackageCache).SelectMany(folder => Directory.EnumerateFiles(folder, name))
                : Enumerable.Empty<string>();
        }
    }
}
