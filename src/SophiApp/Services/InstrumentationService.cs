// <copyright file="InstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Management;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class InstrumentationService : IInstrumentationService
    {
        /// <inheritdoc/>
        public OsProperties GetOsProperties()
        {
            try
            {
                var managementObject = new ManagementObjectSearcher(scope: "root\\CIMV2", queryString: "SELECT * FROM Win32_OperatingSystem")
                .Get()
                .Cast<ManagementBaseObject>()
                .First();

                return new OsProperties(managementObject.Properties);
            }
            catch (Exception)
            {
                // TODO: Log error handler here!
                return new OsProperties();
            }
        }
    }
}
