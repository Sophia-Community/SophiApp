// <copyright file="IInstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Diagnostics;
    using System.Management;
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with WMI.
    /// </summary>
    public interface IInstrumentationService
    {
        /// <summary>
        /// Get the properties of the Win32_OperatingSystem class.
        /// </summary>
        OsProperties GetOsPropertiesOrDefault();

        /// <summary>
        /// Get UWP apps management.
        /// </summary>
        ManagementObject? GetUwpAppsManagementOrDefault();

        /// <summary>
        /// Get the owner of the process.
        /// </summary>
        /// <param name="process">The process for which to find an owner.</param>
        string GetProcessOwnerOrDefault(Process? process);

        /// <summary>
        /// Get data from the AntiVirusProduct class.
        /// </summary>
        List<ManagementObject> GetAntivirusProductsOrDefault();

        /// <summary>
        /// Get user account SID.
        /// </summary>
        /// <param name="name">A user name.</param>
        string GetUserSid(string name);
    }
}
