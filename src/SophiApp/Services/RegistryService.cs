// <copyright file="RegistryService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <inheritdoc/>
    public class RegistryService : IRegistryService
    {
        /// <inheritdoc/>
        public void RemoveVolumeCachesStateFlags()
        {
            using var volumeCaches = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches");
            Array.ForEach(volumeCaches?.GetSubKeyNames() ?? [], subKey => volumeCaches?.OpenSubKey(subKey, true)?.DeleteValue("StateFlags1337", false));
        }

        /// <inheritdoc/>
        public void SetVolumeCachesStateFlags()
        {
            using var volumeCaches = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches");
            new List<string>()
            {
                "BranchCache",
                "Delivery Optimization Files",
                "Device Driver Packages",
                "Language Pack",
                "Previous Installations",
                "Setup Log Files",
                "System error memory dump files",
                "System error minidump files",
                "Temporary Files",
                "Temporary Setup Files",
                "Update Cleanup",
                "Upgrade Discarded Files",
                "Windows Defender",
                "Windows ESD installation files",
                "Windows Upgrade Log Files",
            }
            .ForEach(subKey => volumeCaches?.OpenOrCreateSubKey(subKey).SetValue("StateFlags1337", 2, RegistryValueKind.DWord));
        }
    }
}
