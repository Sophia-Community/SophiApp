// <copyright file="CommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.UI.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <inheritdoc/>
    public class CommonDataService : ICommonDataService
    {
        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        private readonly IHttpService httpService;
        private readonly IInstrumentationService instrumentationService;
        private readonly IWslService wslService;
        private Task appReleaseTask = Task.CompletedTask;
        private Task externalDataTask = Task.CompletedTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDataService"/> class.
        /// </summary>
        /// <param name="instrumentationService">Service for working with WMI.</param>
        /// <param name="httpService">Service for working with HTTP API.</param>
        /// <param name="wslService">Service for working with Windows Subsystem for Linux.</param>
        public CommonDataService(IInstrumentationService instrumentationService, IHttpService httpService, IWslService wslService)
        {
            this.instrumentationService = instrumentationService;
            this.httpService = httpService;
            this.wslService = wslService;
            OsProperties = new();
            SupportedOs = new();
        }

        /// <inheritdoc/>
        public OsProperties OsProperties { get; private set; }

        /// <inheritdoc/>
        public bool DefenderIsDefaultAV { get; private set; } = false;

        /// <inheritdoc/>
        public Version AppVersion => assembly.Version!;

        /// <inheritdoc/>
        public AppVersion? LatestAppRelease { get; private set; }

        /// <inheritdoc/>
        public DotNetRelease? LatestReleaseNET10 { get; private set; }

        /// <inheritdoc/>
        public DotNetRelease? LatestReleaseNET11 { get; private set; }

        /// <inheritdoc/>
        public VCRelease? LatestReleaseVC { get; private set; }

        /// <inheritdoc/>
        public string RequirementsResult_1 { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string RequirementsResult_2 { get; set; } = string.Empty;

        /// <inheritdoc/>
        public SupportedOs SupportedOs { get; private set; }

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            await Task.Run(() =>
            {
                OsProperties = instrumentationService.GetOsProperties();
                OsProperties.IsLTSC = OsProperties.Caption.Contains("LTSC");
                App.Logger.LogAppProperties(version: assembly.Version!, directory: EnvironmentHelper.AppFolder);
            });
        }

        /// <inheritdoc/>
        public void BeginGetExternalServicesData()
        {
            appReleaseTask = Task.Run(SetLatestAppReleaseAsync);
            externalDataTask = Task.Run(() => Task.WhenAll(
                appReleaseTask,
                SetDefenderStateAsync(),
                SetNet10ReleaseAsync(),
                SetNet11ReleaseAsync(),
                SetVCReleaseAsync(),
                SetSupportedBuildsAsync()));

            // The list of Linux distributions is read with the settings, so by then it is already here
            wslService.BeginGetDistributions();
        }

        /// <inheritdoc/>
        public void WaitForAppRelease() => appReleaseTask.Wait();

        /// <inheritdoc/>
        public void WaitForExternalServicesData() => externalDataTask.Wait();

        /// <inheritdoc/>
        public string GetFullName() => "SophiApp for Windows (Preview 1)";

        private async Task SetDefenderStateAsync()
        {
            await Task.Run(() =>
            {
                var defenderIsDefaultAV = false;

                try
                {
                    // AMRunningMode is "Normal" only if Microsoft Defender is the active antivirus
                    // Otherwise it is "Passive Mode", "SxS Passive Mode" or "EDR Block Mode"
                    var runningMode = instrumentationService.GetCimInstance(nameSpace: "root/Microsoft/Windows/Defender", className: "MSFT_MpComputerStatus")
                        .FirstOrDefault()
                        ?.GetPropertyValue("AMRunningMode") as string;

                    // Checking whether there's a turned on third-party AV. {D68DDC3A-831F-4fae-9E44-DA132C1ACF46} is Microsoft Defender
                    // Bits 12-15 of productState are WSC_SECURITY_PRODUCT_STATE: 0 is off, 1 is on, 2 is snoozed, 3 is expired
                    var thirdPartyAV = instrumentationService.GetAntivirusProducts().Exists(product =>
                        !"{D68DDC3A-831F-4fae-9E44-DA132C1ACF46}".Equals(product.GetPropertyValue("instanceGuid") as string, StringComparison.OrdinalIgnoreCase)
                        && (Convert.ToUInt32(product.GetPropertyValue("productState")) & 0xF000) == 0x1000);

                    defenderIsDefaultAV = "Normal".Equals(runningMode, StringComparison.OrdinalIgnoreCase) && !thirdPartyAV;
                }
                catch (Exception ex)
                {
                    // Microsoft Defender WMI provider is not available
                    App.Logger.LogSkippedException("Reading the Microsoft Defender state", ex);
                }

                App.Logger.LogDefenderIsDefault(defenderIsDefaultAV);
                DefenderIsDefaultAV = defenderIsDefaultAV;
            });
        }

        private async Task SetLatestAppReleaseAsync()
        {
            try
            {
                LatestAppRelease = await httpService.GetFromJsonAsync<AppVersion>("https://raw.githubusercontent.com/Sophia-Community/SophiApp/refs/heads/dev-SophiApp2/SophiApp_Releases.json", 5);
            }
            catch (Exception ex)
            {
                // No connection: the app goes on without this data
                App.Logger.LogSkippedException("Getting the latest app release", ex);
            }
        }

        private async Task SetNet10ReleaseAsync()
        {
            try
            {
                LatestReleaseNET10 = await httpService.GetFromJsonAsync<DotNetRelease>("https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json", 5);
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Getting the latest .NET 10 release", ex);
            }
        }

        private async Task SetNet11ReleaseAsync()
        {
            try
            {
                LatestReleaseNET11 = await httpService.GetFromJsonAsync<DotNetRelease>("https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json", 5);
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Getting the latest .NET 11 release", ex);
            }
        }

        private async Task SetVCReleaseAsync()
        {
            try
            {
                LatestReleaseVC = await httpService.GetFromJsonAsync<VCRelease>("https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json", 5);
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Getting the latest Visual C++ Redistributable release", ex);
            }
        }

        private async Task SetSupportedBuildsAsync()
        {
            try
            {
                SupportedOs = await httpService.GetFromJsonAsync<SupportedOs>("https://raw.githubusercontent.com/Sophia-Community/SophiApp/refs/heads/dev-SophiApp2/Windows_Builds.json", 5);
            }
            catch (Exception ex)
            {
                // The builds known to the app are used
                App.Logger.LogSkippedException("Getting the supported Windows builds", ex);
            }
        }
    }
}
