// <copyright file="UpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
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
        /// <param name="commonDataService">A service for transferring common app data between layers of abstractions.</param>
        public UpdateService(IInstrumentationService instrumentationService, ICommonDataService commonDataService)
        {
            this.instrumentationService = instrumentationService;
            this.commonDataService = commonDataService;
        }

        /// <inheritdoc/>
        public void RunOsUpdate()
        {
            if (commonDataService.IsOnline)
            {
                GetUpdatesForOtherMsProducts();
                GetUpdatesForUwpApps();
                GetOsUpdates();
            }
        }

        private void GetUpdatesForOtherMsProducts()
        {
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
            try
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
            catch (Exception ex)
            {
                App.Logger.LogOsUpdateException(ex);
            }
        }
    }
}
