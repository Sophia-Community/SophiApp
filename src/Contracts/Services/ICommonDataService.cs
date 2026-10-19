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
        /// Gets a value indicating whether Microsoft Defender enabled state.
        /// </summary>
        bool DefenderEnabled { get; }

        /// <summary>
        /// Gets app latest release from GitHub repository.
        /// </summary>
        AppVersion? LatestAppRelease { get; }

        /// <summary>
        /// Gets .NET 8 latest release.
        /// </summary>
        DotNetRelease? LatestReleaseNET8 { get; }

        /// <summary>
        /// Gets .NET 9 latest release.
        /// </summary>
        DotNetRelease? LatestReleaseNET9 { get; }

        /// <summary>
        /// Gets .NET 10 latest release.
        /// </summary>
        DotNetRelease? LatestReleaseNET10 { get; }

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
        /// Gets app supported OS builds version.
        /// </summary>
        SupportedUBR SupportedUBR { get; }

        /// <summary>
        /// Initialize <see cref="ICommonDataService"/> data.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Get external services data.
        /// </summary>
        Task GetExternalServicesDataAsync();

        /// <summary>
        /// Gets app name and version.
        /// </summary>
        string GetFullName();
    }
}
