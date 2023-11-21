// <copyright file="UpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
            GetUpdatesForOtherMsProducts();
            GetUpdatesForUwpApps();
            GetOsUpdates();
        }

        private void GetUpdatesForOtherMsProducts()
        {
            Type type = Type.GetTypeFromProgID("Microsoft.Update.ServiceManager") !;
            dynamic? service = Activator.CreateInstance(type);
            _ = service?.AddService2("7971f918-a847-4430-9279-4a52d1efe18d", 7, string.Empty);
        }

        private void GetUpdatesForUwpApps()
        {
            _ = instrumentationService.GetUwpAppsManagement()?.InvokeMethod("UpdateScanMethod", Array.Empty<object>());
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
            catch (Exception)
            {
                // TODO: Log exception here!
            }
        }
    }
}
