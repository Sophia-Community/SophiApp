// <copyright file="InstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Management;
    using System.Management.Automation;
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
            var managementObject = new ManagementObjectSearcher(scope: "root\\CIMV2", queryString: "SELECT * FROM Win32_OperatingSystem")
                .Get()
                .Cast<ManagementBaseObject>()
                .First();

            var ps = PowerShell.Create().AddScript("chcp 437 | winmgmt /verifyrepository").Invoke();
            ps[0].BaseObject // "WMI repository is consistent"
            ps[0].BaseObject.GetType().Name // "String"

            return new OsProperties(managementObject.Properties);
        }
    }
}
