// <copyright file="IInstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// Service for working with WMI.
    /// </summary>
    public interface IInstrumentationService
    {
        /// <summary>
        /// Gets the WMI properties of the Win32_OperatingSystem class.
        /// </summary>
        OsProperties GetOsProperties();
    }
}
