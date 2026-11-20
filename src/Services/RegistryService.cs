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
        private readonly IProcessService processService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryService"/> class.
        /// </summary>
        /// <param name="processService">A service for working with Windows process API.</param>
        public RegistryService(IProcessService processService)
        {
            this.processService = processService;
        }

        /// <inheritdoc/>
        public void ClearCommonDialogViews()
        {
            const string bagsPath = @"Software\Classes\Local Settings\Software\Microsoft\Windows\Shell\Bags";

            // Saved view settings of the common Open/Save dialogs
            const string viewKeyName = "{885A186E-A440-4ADA-812B-DB871B942259}";

            using var bags = Registry.CurrentUser.OpenSubKey(bagsPath);

            if (bags is null)
            {
                return;
            }

            // Delete the saved view of the common dialogs in every bag
            foreach (var bag in bags.GetSubKeyNames())
            {
                try
                {
                    using var shell = Registry.CurrentUser.OpenSubKey($@"{bagsPath}\{bag}\Shell", writable: true);
                    shell?.DeleteSubKeyTree(viewKeyName, throwOnMissingSubKey: false);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is IOException)
                {
                    // The key cannot be opened or deleted: skip it and continue
                    App.Logger.LogSkippedException($"Deleting the saved view in the bag {bag}", ex);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveVolumeCachesStateFlags()
        {
            using var volumeCaches = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches");
            Array.ForEach(volumeCaches?.GetSubKeyNames() ?? [], subKey => volumeCaches?.DeleteSubKeyValue(subKey, "StateFlags1337"));
        }

        /// <inheritdoc/>
        public void SetRegistryValueBypassUCPD(string key, string name, int value)
        {
            // Some registry values cannot be set via any API, because the UCPD driver tracks executables to block access to the registry.
            // A copy of reg.exe under a different name is not tracked
            var regCopy = Path.Combine(Path.GetTempPath(), $"reg_{Guid.NewGuid():N}.exe");
            File.Copy(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "reg.exe"), regCopy, true);

            try
            {
                _ = processService.WaitForExit(name: regCopy, arguments: $"add \"{key}\" /v {name} /t REG_DWORD /d {value} /f");
            }
            finally
            {
                regCopy.TryDelete();
            }
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
            .ForEach(subKey => volumeCaches?.CreateKeyAndSetValue(subKey, "StateFlags1337", 2, RegistryValueKind.DWord));
        }

        /// <inheritdoc/>
        public void UseLatestCLR()
        {
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\.NETFramework", "OnlyUseLatestCLR");
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Wow6432Node\\Microsoft\\.NETFramework", "OnlyUseLatestCLR");
        }
    }
}
