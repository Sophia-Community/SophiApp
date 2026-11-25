// <copyright file="ICommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// A service for transferring app data between DI layers.
    /// </summary>
    public interface ICommonDataService
    {
        /// <summary>
        /// Gets a values of OS properties.
        /// </summary>
        OsProperties OsProperties { get; }

        /// <summary>
        /// Gets app version.
        /// </summary>
        Version AppVersion { get; }

        /// <summary>
        /// Gets a value indicating whether Microsoft Defender is default AV.
        /// </summary>
        bool DefenderIsDefaultAV { get; }

        /// <summary>
        /// Gets app latest release from GitHub repository.
        /// </summary>
        AppVersion? LatestAppRelease { get; }

        /// <summary>
        /// Gets .NET 10 latest release.
        /// </summary>
        DotNetRelease? LatestReleaseNET10 { get; }

        /// <summary>
        /// Gets .NET 11 latest release.
        /// </summary>
        DotNetRelease? LatestReleaseNET11 { get; }

        /// <summary>
        /// Gets Visual C++ latest release.
        /// </summary>
        VCRelease? LatestReleaseVC { get; }

        /// <summary>
        /// Gets or sets first localized description string for <see cref="IRequirementsService"/> actions result.
        /// </summary>
        string RequirementsResult_1 { get; set; }

        /// <summary>
        /// Gets or sets second localized description string for <see cref="IRequirementsService"/> actions result.
        /// </summary>
        string RequirementsResult_2 { get; set; }

        /// <summary>
        /// Gets supported OS version.
        /// </summary>
        SupportedOs SupportedOs { get; }

        /// <summary>
        /// Initialize <see cref="ICommonDataService"/> data.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Start getting the data from the Internet and from Windows in the background.
        /// </summary>
        void BeginGetExternalServicesData();

        /// <summary>
        /// Wait until the latest app release is received.
        /// </summary>
        void WaitForAppRelease();

        /// <summary>
        /// Wait until all the data is received.
        /// </summary>
        void WaitForExternalServicesData();

        /// <summary>
        /// Waits until all the data is received and gets the addresses that did not answer.
        /// </summary>
        /// <returns>The address and what the app wanted from it, in the language of the app.</returns>
        IReadOnlyList<(string Url, string Purpose)> GetUnavailableAddresses();

        /// <summary>
        /// Gets the app name shown in the window title and on the Settings page.
        /// </summary>
        string GetFullName();
    }
}
