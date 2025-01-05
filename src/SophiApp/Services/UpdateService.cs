// <copyright file="UpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class UpdateService : IUpdateService
    {
        private readonly IInstrumentationService instrumentationService;
        private readonly ICommonDataService commonDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        /// <param name="instrumentationService">Service for working with WMI.</param>
        /// <param name="commonDataService">Service for transferring app data between layers of DI.</param>
        public UpdateService(IInstrumentationService instrumentationService, ICommonDataService commonDataService)
        {
            this.instrumentationService = instrumentationService;
            this.commonDataService = commonDataService;
        }

        /// <inheritdoc/>
        public void RunOsUpdate()
        {
            try
            {
                GetUpdatesForOtherMsProducts();
                GetUpdatesForUwpApps();
                GetOsUpdates();
            }
            catch (Exception ex)
            {
                App.Logger.LogOsUpdateException(ex);
            }
        }

        private void GetUpdatesForOtherMsProducts()
        {
            if (commonDataService.IsWindows11)
            {
                var settingsPath = "Software\\Microsoft\\WindowsUpdate\\UX\\Settings";
                Registry.LocalMachine.OpenSubKey(settingsPath)?.SetValue("AllowMUUpdateService", 1, RegistryValueKind.DWord);
                return;
            }

            Type type = Type.GetTypeFromProgID("Microsoft.Update.ServiceManager") !;
            dynamic? service = Activator.CreateInstance(type);
            _ = service?.AddService2("7971f918-a847-4430-9279-4a52d1efe18d", 7, string.Empty);
        }

        private void GetUpdatesForUwpApps()
        {
            _ = instrumentationService.GetUwpAppsManagementOrDefault()?.InvokeMethod("UpdateScanMethod", Array.Empty<object>());
        }

        private void GetOsUpdates()
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
