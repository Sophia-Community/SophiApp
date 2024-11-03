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

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateService"/> class.
        /// </summary>
        /// <param name="instrumentationService">Service for working with WMI.</param>
        public UpdateService(IInstrumentationService instrumentationService)
        {
            this.instrumentationService = instrumentationService;
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
