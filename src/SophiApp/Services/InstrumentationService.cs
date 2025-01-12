// <copyright file="InstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class InstrumentationService : IInstrumentationService
    {
        /// <inheritdoc/>
        public OsProperties GetOsPropertiesOrDefault()
        {
            try
            {
                using var managementObject = new ManagementObjectSearcher(scope: "root\\CIMV2", queryString: "SELECT * FROM Win32_OperatingSystem")
                    .Get().Cast<ManagementBaseObject>().First();

                var osProperties = new OsProperties(managementObject.Properties);
                App.Logger.LogOsProperties(osProperties);
                return osProperties;
            }
            catch (Exception ex)
            {
                App.Logger.LogOsPropertiesException(ex);
                return new OsProperties();
            }
        }

        /// <inheritdoc/>
        public ManagementObject? GetUwpAppsManagementOrDefault()
        {
            try
            {
                return new ManagementObjectSearcher(scope: "root\\CIMV2\\mdm\\dmmap", queryString: "SELECT * FROM MDM_EnterpriseModernAppManagement_AppManagement01")
                    .Get().Cast<ManagementObject>().First();
            }
            catch (Exception ex)
            {
                App.Logger.LogUwpAppsManagementException(ex);
                return null;
            }
        }

        /// <inheritdoc/>
        public List<string> GetPowerPlanNames()
        {
            return new ManagementObjectSearcher(scope: "root/CIMV2/power", queryString: "SELECT ElementName FROM Win32_PowerPlan")
                .Get()
                .Cast<ManagementObject>()
                .Select(obj => obj.GetPropertyValue("ElementName") as string ?? string.Empty)
                .ToList();
        }

        /// <inheritdoc/>
        public string GetProcessOwnerOrDefault(Process? process)
        {
            if (process is null)
            {
                return string.Empty;
            }

            try
            {
                var results = new string[] { string.Empty, string.Empty };
                using var managementObject = new ManagementObjectSearcher($"Select * from Win32_Process Where ProcessId = {process.Id}")
                    .Get()
                    .Cast<ManagementObject>()
                    .First();

                return (uint)managementObject.InvokeMethod("GetOwner", results) == 0 ? results[0] : string.Empty;
            }
            catch (Exception ex)
            {
                App.Logger.LogProcessOwnerException(ex);
                return string.Empty;
            }
        }

        /// <inheritdoc/>
        public List<ManagementObject> GetAntivirusProductsOrDefault()
        {
            try
            {
                return new ManagementObjectSearcher(scope: "root\\SecurityCenter2", queryString: "SELECT * FROM AntiVirusProduct")
                    .Get()
                    .Cast<ManagementObject>()
                    .ToList();
            }
            catch (Exception ex)
            {
                App.Logger.LogAntivirusProductsException(ex);
                return new List<ManagementObject>();
            }
        }

        /// <inheritdoc/>
        public string GetUserSid(string name)
        {
            using var managementObject = new ManagementObjectSearcher("Select * from Win32_UserAccount")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault(obj => (string)obj.GetPropertyValue("Name") == name);

            return managementObject?.GetPropertyValue("Sid") as string ?? throw new InvalidOperationException($"Failed to obtain user SID API in the {nameof(IInstrumentationService)}");
        }

        /// <inheritdoc/>
        public bool GetAntispywareEnabled()
        {
            using var managementObject = new ManagementObjectSearcher(scope: "root/microsoft/windows/defender", queryString: $"Select * from MSFT_MpComputerStatus")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("AntispywareEnabled") as bool? ?? throw new InvalidOperationException($"Failed to obtain AntispywareEnabled value from WMI class MSFT_MpComputerStatus in the {nameof(IInstrumentationService)}");
        }

        /// <inheritdoc/>
        public bool? CpuVirtualizationIsEnabled()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_Processor")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("VirtualizationFirmwareEnabled") as bool?;
        }

        /// <inheritdoc/>
        public bool? HypervisorPresent()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("HypervisorPresent") as bool?;
        }

        /// <inheritdoc/>
        public bool IsExternalDACType()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_VideoController")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            var dacType = managementObject?.GetPropertyValue("AdapterDACType") as string ?? string.Empty;
            return !(string.IsNullOrEmpty(dacType) && dacType.Equals("Internal", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public bool IsVirtualMachine()
        {
            var vmTokens = new[] { "Virtual", "VMware" };
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            var model = managementObject?.GetPropertyValue("Model") as string ?? string.Empty;
            return Array.Exists(vmTokens, token => model.Contains(token, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
