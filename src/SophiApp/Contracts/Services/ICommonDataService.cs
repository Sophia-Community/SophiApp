// <copyright file="ICommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// A service for transferring common app data between layers of abstractions.
    /// </summary>
    public interface ICommonDataService
    {
        /// <summary>
        /// Gets the url with the version of the app.
        /// </summary>
        string AppVersionUrl { get; }

        /// <summary>
        /// Gets the url to download the new release of the app.
        /// </summary>
        string AppReleaseUrl { get; }

        /// <summary>
        /// Gets a value indicating whether Internet access is available.
        /// </summary>
        bool IsOnline { get; }

        /// <summary>
        /// Gets a value indicating whether the OS is Windows 11.
        /// </summary>
        bool IsWindows11 { get; }

        /// <summary>
        /// Gets a values of OS properties.
        /// </summary>
        OsProperties OsProperties { get; }

        /// <summary>
        /// Gets or sets malware name detected by <see cref="IRequirementsService"/>.
        /// </summary>
        string DetectedMalware { get; set; }

        /// <summary>
        /// Gets or sets Microsoft Defender missing files name.
        /// </summary>
        string MsDefenderFileMissing { get; set; }

        /// <summary>
        /// Gets or sets Microsoft Defender stopped service name.
        /// </summary>
        string MsDefenderServiceStopped { get; set; }

        /// <summary>
        /// Gets app version.
        /// </summary>
        public Version AppVersion { get; }

        /// <summary>
        /// Gets app name and version.
        /// </summary>
        string GetFullName();

        /// <summary>
        /// Gets the code name of the application build.
        /// </summary>
        string GetBuildName();

        /// <summary>
        /// Gets the name of the program version: Community or Pro.
        /// </summary>
        string GetVersionName();

        /// <summary>
        /// Gets app name and version delimiter.
        /// </summary>
        string GetDelimiter();
    }
}
