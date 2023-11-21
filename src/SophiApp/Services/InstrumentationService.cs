// <copyright file="InstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using System.Management;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class InstrumentationService : IInstrumentationService
    {
        /// <inheritdoc/>
        public OsProperties? GetOsProperties()
        {
            try
            {
                using var managementObject = new ManagementObjectSearcher(scope: "root\\CIMV2", queryString: "SELECT * FROM Win32_OperatingSystem")
                    .Get().Cast<ManagementBaseObject>().First();

                return new OsProperties(managementObject.Properties);
            }
            catch (Exception)
            {
                // TODO: Log error handler here!
                return null;
            }
        }

        /// <inheritdoc/>
        public ManagementObject? GetUwpAppsManagement()
        {
            try
            {
                return new ManagementObjectSearcher(scope: "root\\CIMV2\\mdm\\dmmap", queryString: "SELECT * FROM MDM_EnterpriseModernAppManagement_AppManagement01")
                    .Get().Cast<ManagementObject>().First();
            }
            catch (Exception)
            {
                // TODO: Log error handler here!
                return null;
            }
        }

        /// <inheritdoc/>
        public string GetProcessOwner(Process? process)
        {
            try
            {
                if (process is null)
                {
                    return string.Empty;
                }

                var results = new string[] { string.Empty, string.Empty };
                using var managementObject = new ManagementObjectSearcher($"Select * from Win32_Process Where ProcessId = {process.Id}")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();

                return (uint)managementObject.InvokeMethod("GetOwner", results) == 0 ? results[0] : string.Empty;
            }
            catch (Exception)
            {
                // TODO: Log error handler here!
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public List<ManagementObject> GetAntivirusProducts()
        {
            try
            {
                return new ManagementObjectSearcher(scope: "root\\SecurityCenter2", queryString: "SELECT * FROM AntiVirusProduct")
                    .Get().Cast<ManagementObject>().ToList();
            }
            catch (Exception)
            {
                // TODO: Log error handler here!
                return new List<ManagementObject>();
            }
        }
    }
}
