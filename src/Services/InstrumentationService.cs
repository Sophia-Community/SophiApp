// <copyright file="InstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Runtime.InteropServices;

    /// <inheritdoc/>
    public class InstrumentationService : IInstrumentationService
    {
        /// <inheritdoc/>
        public OsProperties GetOsProperties()
        {
            try
            {
                using var managementObject = new ManagementObjectSearcher(scope: "root\\CIMV2", queryString: "SELECT * FROM Win32_OperatingSystem")
                    .Get()
                    .Cast<ManagementBaseObject>()
                    .First();

                var osCaption = BrandingFormatString("%WINDOWS_LONG%");
                var osProperties = new OsProperties(managementObject.Properties, osCaption);
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
        public List<ManagementObject> GetPowerPlans()
        {
            return [.. new ManagementObjectSearcher(scope: "root/CIMV2/power", queryString: "SELECT * FROM Win32_PowerPlan")
                .Get()
                .Cast<ManagementObject>()];
        }

        /// <inheritdoc/>
        public string GetProcessOwnerName(Process? process)
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
        public List<ManagementObject> GetAntivirusProducts()
        {
            try
            {
                return [.. new ManagementObjectSearcher(scope: "root\\SecurityCenter2", queryString: "SELECT * FROM AntiVirusProduct")
                    .Get()
                    .Cast<ManagementObject>()];
            }
            catch (Exception ex)
            {
                App.Logger.LogAntivirusProductsException(ex);
                return [];
            }
        }

        /// <inheritdoc/>
        public string GetProcessorCaption()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_Processor")
                .Get()
                .Cast<ManagementObject>()
                .First();

            return (managementObject.GetPropertyValue("Caption") as string) !;
        }

        /// <inheritdoc/>
        public string GetUserSid(string name)
        {
            using var managementObject = new ManagementObjectSearcher("Select * from Win32_UserAccount")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault(o => o.GetPropertyValue("Name") as string == name);

            return managementObject?.GetPropertyValue("Sid") as string ?? throw new InvalidOperationException($"Failed to obtain user SID API in the {nameof(IInstrumentationService)}");
        }

        /// <inheritdoc/>
        public bool GetAntiSpywareEnabled()
        {
            using var managementObject = new ManagementObjectSearcher(scope: "root/Microsoft/Windows/Defender", queryString: $"Select * from MSFT_MpComputerStatus")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("AntispywareEnabled") as bool? ?? throw new InvalidOperationException($"Failed to obtain AntiSpywareEnabled value from WMI class MSFT_MpComputerStatus in the {nameof(IInstrumentationService)}");
        }

        /// <inheritdoc/>
        public bool? CpuVirtualizationFirmwareIsEnabled()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_Processor")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("VirtualizationFirmwareEnabled") as bool?;
        }

        /// <inheritdoc/>
        public bool? HypervisorIsPresent()
        {
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            return managementObject?.GetPropertyValue("HypervisorPresent") as bool?;
        }

        /// <inheritdoc/>
        public void SetPageFileAutoSize()
        {
            new ManagementObjectSearcher("Select * from CIM_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault()
                ?.SetPropertyValue("AutomaticManagedPageFile", true);
        }

        /// <inheritdoc/>
        public bool DetectDACType()
        {
            // Determining whether PC has an external graphics card
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_VideoController")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            var dacType = managementObject?.GetPropertyValue("AdapterDACType") as string ?? string.Empty;
            return !(string.IsNullOrEmpty(dacType) && dacType.Equals("Internal", StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public bool DetectVM()
        {
            // Determining whether an OS is not installed on a virtual machine
            var vmTokens = new[] { "Virtual", "VMware" };
            using var managementObject = new ManagementObjectSearcher("Select * from CIM_ComputerSystem")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault();

            var model = managementObject?.GetPropertyValue("Model") as string ?? string.Empty;
            return Array.Exists(vmTokens, token => model.Contains(token, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc/>
        public List<int>? GetEventViewerConsoleProcessId()
        {
            using var managementObject = new ManagementObjectSearcher(queryString: "SELECT ProcessId FROM Win32_Process WHERE Name = 'mmc.exe' AND CommandLine LIKE '%eventvwr.msc%'").Get();
            var processesId = managementObject.Cast<ManagementBaseObject>().Select(m => Convert.ToInt32(m.GetPropertyValue("ProcessId"))).ToList();
            return processesId.Count > 0 ? processesId : null;
        }

        /// <inheritdoc/>
        public bool WindowsAIPresent()
        {
            var managementObject = new ManagementObjectSearcher("Select ClassGuid, PNPClass from Win32_PnPEntity")
                .Get()
                .Cast<ManagementObject>()
                .FirstOrDefault(e => e.GetPropertyValue("ClassGuid") is not null && e.GetPropertyValue("PNPClass").Equals("ComputeAccelerator"));
            return managementObject is not null;
        }

        [DllImport("Winbrand.dll", CharSet = CharSet.Unicode)]
        private static extern string BrandingFormatString(string sFormat);
    }
}
