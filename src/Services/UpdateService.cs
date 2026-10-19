// <copyright file="UpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
    using System.Diagnostics;

    /// <inheritdoc/>
    public class UpdateService : IUpdateService
    {
        /// <inheritdoc/>
        public bool HasMicrosoftProductsUpdate()
        {
            var allowUpdate = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings")?.GetValue("AllowMUUpdateService") as int? ?? -1;
            return allowUpdate.Equals(1);
        }

        /// <inheritdoc/>
        public void RunUpdateByReason(RequirementsResult reason)
        {
            switch (reason)
            {
                case RequirementsResult.UpdateWindowsBuild:
                    RunMicrosoftProductsUpdate();
                    RunOsUpdates();
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc/>
        public void RunMicrosoftProductsUpdate()
            => Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", true)?.SetValue("AllowMUUpdateService", 1, RegistryValueKind.DWord);

        /// <inheritdoc/>
        public void StopMicrosoftProductsUpdate()
            => Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", true)?.DeleteValue("AllowMUUpdateService", false);

        private void RunOsUpdates()
        {
            _ = Process.Start(
                    new ProcessStartInfo()
                    {
                        FileName = "UsoClient.exe",
                        Arguments = "StartInteractiveScan",
                    });

            _ = Process.Start(
                new ProcessStartInfo()
                {
                    FileName = "explorer.exe",
                    Arguments = "ms-settings:windowsupdate",
                });
        }
    }
}
