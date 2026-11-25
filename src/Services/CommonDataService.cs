// <copyright file="CommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.UI.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <inheritdoc/>
    public class CommonDataService : ICommonDataService
    {
        private const string DotNetReleasesUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json";
        private const string VisualCReleaseUrl = "https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json";

        private readonly AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
        private readonly IHttpService httpService;
        private readonly IInstrumentationService instrumentationService;
        private readonly IWslService wslService;
        // The addresses that did not answer, and the string that names what was wanted from each
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> unavailableAddresses = new();
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
                SetNetReleasesAsync(),
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
        public IReadOnlyList<(string Url, string Purpose)> GetUnavailableAddresses()
        {
            WaitForExternalServicesData();

            if (wslService.GetUnavailableAddress() is string address)
            {
                unavailableAddresses[address] = "OsRequirements_Data_Linux";
            }

            // What matters most goes first
            string[] order = ["OsRequirements_Data_AppRelease", "OsRequirements_Data_WindowsBuilds", "OsRequirements_Data_DotNet", "OsRequirements_Data_VisualC", "OsRequirements_Data_Linux"];

            return unavailableAddresses.OrderBy(item => Array.IndexOf(order, item.Value)).Select(item => (item.Key, item.Value.GetLocalized())).ToList();
        }

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
                LatestAppRelease = await httpService.GetFromJsonAsync<AppVersion>(AppLinks.ReleasesData, 5);
            }
            catch (Exception ex)
            {
                // No connection: the app goes on without this data
                App.Logger.LogSkippedException("Getting the latest app release", ex);
                unavailableAddresses[AppLinks.ReleasesData] = "OsRequirements_Data_AppRelease";
            }
        }

        private async Task SetNetReleasesAsync()
        {
            try
            {
                // The index is a few kilobytes and names the latest release of every .NET version.
                // The full lists of releases are hundreds of times larger and do not come in time on a slow connection
                var index = await httpService.GetFromJsonAsync<DotNetReleaseIndex>(DotNetReleasesUrl, 10);
                LatestReleaseNET10 = index.Releases.Find(release => release.Channel == "10.0");
                LatestReleaseNET11 = index.Releases.Find(release => release.Channel == "11.0");
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Getting the latest .NET releases", ex);
                unavailableAddresses[DotNetReleasesUrl] = "OsRequirements_Data_DotNet";
            }
        }

        private async Task SetVCReleaseAsync()
        {
            try
            {
                LatestReleaseVC = await httpService.GetFromJsonAsync<VCRelease>(VisualCReleaseUrl, 5);
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Getting the latest Visual C++ Redistributable release", ex);
                unavailableAddresses[VisualCReleaseUrl] = "OsRequirements_Data_VisualC";
            }
        }

        private async Task SetSupportedBuildsAsync()
        {
            try
            {
                SupportedOs = await httpService.GetFromJsonAsync<SupportedOs>(AppLinks.SupportedBuildsData, 5);
            }
            catch (Exception ex)
            {
                // The builds known to the app are used
                App.Logger.LogSkippedException("Getting the supported Windows builds", ex);
                unavailableAddresses[AppLinks.SupportedBuildsData] = "OsRequirements_Data_WindowsBuilds";
            }
        }
    }
}
