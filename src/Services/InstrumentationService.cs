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
            return
            [
                .. new ManagementObjectSearcher(scope: "root/CIMV2/power", queryString: "SELECT * FROM Win32_PowerPlan")
                    .Get()
                    .Cast<ManagementObject>(),
            ];
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
                return
                [
                    .. new ManagementObjectSearcher(scope: "root\\SecurityCenter2", queryString: "SELECT * FROM AntiVirusProduct")
                        .Get()
                        .Cast<ManagementObject>(),
                ];
            }
            catch (Exception ex)
            {
                App.Logger.LogAntivirusProductsException(ex);
                return [];
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ManagementObject> GetCimInstance(string nameSpace, string className) => new ManagementObjectSearcher(scope: nameSpace, queryString: $"SELECT * FROM {className}")
            .Get()
            .Cast<ManagementObject>();

        /// <inheritdoc/>
        public string GetUserSid(string name)
        {
            try
            {
                // The system translates the account name to SID itself. Enumerating all accounts via WMI is slow on domain-joined PCs
                return new System.Security.Principal.NTAccount(name).Translate(typeof(System.Security.Principal.SecurityIdentifier)).Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to obtain user SID API in the {nameof(IInstrumentationService)}", ex);
            }
        }

        /// <inheritdoc/>
        public bool? CPUVirtualizationIsEnabled()
        {
            // The same flag that the VirtualizationFirmwareEnabled property of CIM_Processor shows, but without a WMI query, which takes about a second
            const uint VirtualizationFirmwareEnabled = 21;
            return IsProcessorFeaturePresent(VirtualizationFirmwareEnabled);
        }

        /// <inheritdoc/>
        public bool HasAIAccelerator()
        {
            // The query is filtered by the device class, so it takes milliseconds
            using var searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT ClassGuid FROM Win32_PnPEntity WHERE PNPClass = 'ComputeAccelerator'");
            using var devices = searcher.Get();
            return devices.Cast<ManagementObject>().Any(device => device["ClassGuid"] is not null);
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
            return new ManagementObjectSearcher("Select * from CIM_VideoController")
                .Get()
                .Cast<ManagementObject>()
                .Select(controller => controller.GetPropertyValue("AdapterDACType") as string)
                .Any(dacType => !string.IsNullOrEmpty(dacType) && !dacType.Equals("Internal", StringComparison.InvariantCultureIgnoreCase));
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
        public List<int> GetManagementConsoleProcessIds(params string[] snapIns)
        {
            // The command line of a process is available via WMI only, and the query is slow: skip it if no console is running
            var consoleProcesses = Process.GetProcessesByName("mmc");
            Array.ForEach(consoleProcesses, process => process.Dispose());

            if (consoleProcesses.Length == 0)
            {
                return [];
            }

            using var consoles = new ManagementObjectSearcher(queryString: "SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'mmc.exe'").Get();
            return consoles.Cast<ManagementBaseObject>()
                .Where(console => console.GetPropertyValue("CommandLine") is string commandLine && Array.Exists(snapIns, snapIn => commandLine.Contains(snapIn, StringComparison.OrdinalIgnoreCase)))
                .Select(console => Convert.ToInt32(console.GetPropertyValue("ProcessId")))
                .ToList();
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsProcessorFeaturePresent(uint processorFeature);

        [DllImport("Winbrand.dll", CharSet = CharSet.Unicode)]
        private static extern string BrandingFormatString(string sFormat);
    }
}
