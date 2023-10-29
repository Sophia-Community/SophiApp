// <copyright file="IInstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Diagnostics;
    using System.Management;
    using SophiApp.Helpers;

    /// <summary>
    /// Service for working with WMI.
    /// </summary>
    public interface IInstrumentationService
    {
        /// <summary>
        /// Gets the WMI properties of the Win32_OperatingSystem class.
        /// </summary>
        OsProperties? GetOsProperties();

        /// <summary>
        /// Get UWP apps management.
        /// </summary>
        ManagementObject? GetUwpAppsManagement();

        /// <summary>
        /// Gets the owner of the process.
        /// </summary>
        /// <param name="process">The process for which to find an owner.</param>
        string GetProcessOwner(Process? process);
    }
}
