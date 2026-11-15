// <copyright file="UpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Diagnostics;

    /// <inheritdoc/>
    public class UpdateService : IUpdateService
    {
        /// <inheritdoc/>
        public bool HasMicrosoftProductsUpdate()
        {
            var allowUpdate = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "AllowMUUpdateService") as int? ?? -1;
            return allowUpdate.Equals(1);
        }

        /// <inheritdoc/>
        public void RunUpdateByReason(RequirementsResult reason)
        {
            switch (reason)
            {
                case RequirementsResult.WrongWindowsVersion:
                    RunMicrosoftProductsUpdate();
                    RunOsUpdates(openSettings: false);
                    break;

                case RequirementsResult.UpdateWindowsBuild:
                    RunMicrosoftProductsUpdate();
                    RunUwpAppsUpdates();
                    RunOsUpdates(openSettings: true);
                    break;

                default:
                    break;
            }
        }

        /// <inheritdoc/>
        public void RunMicrosoftProductsUpdate()
            => Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "AllowMUUpdateService", 1, RegistryValueKind.DWord);

        /// <inheritdoc/>
        public void StopMicrosoftProductsUpdate()
            => Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "AllowMUUpdateService");

        private static void RunUwpAppsUpdates()
        {
            try
            {
                // Check for UWP apps updates
                using var searcher = new System.Management.ManagementObjectSearcher("root/CIMV2/mdm/dmmap", "SELECT * FROM MDM_EnterpriseModernAppManagement_AppManagement01");

                foreach (var appManagement in searcher.Get().Cast<System.Management.ManagementObject>())
                {
                    _ = appManagement.InvokeMethod("UpdateScanMethod", null);
                }
            }
            catch (Exception)
            {
                // The class is not available, e.g. on Windows 11 LTSC 2024
            }
        }

        private static void RunOsUpdates(bool openSettings)
        {
            // Check for updates
            _ = Process.Start(
                    new ProcessStartInfo()
                    {
                        FileName = "UsoClient.exe",
                        Arguments = "StartInteractiveScan",
                    });

            if (openSettings)
            {
                // Open the "Windows Update" page
                _ = Process.Start(
                    new ProcessStartInfo()
                    {
                        FileName = "explorer.exe",
                        Arguments = "ms-settings:windowsupdate",
                    });
            }
        }
    }
}
