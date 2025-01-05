// <copyright file="IInstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Diagnostics;
    using System.Management;
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with WMI API.
    /// </summary>
    public interface IInstrumentationService
    {
        /// <summary>
        /// Indicates that the DAC used in the video adapter is external type.
        /// </summary>
        bool IsExternalDACType();

        /// <summary>
        /// Defines the use of the virtual machine.
        /// </summary>
        bool IsVirtualMachine();

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
        /// Get power plan names.
        /// </summary>
        List<string> GetPowerPlanNames();

        /// <summary>
        /// Get user account SID.
        /// </summary>
        /// <param name="name">A user name.</param>
        string GetUserSid(string name);

        /// <summary>
        /// Get Microsoft Defender antispyware enabled property value.
        /// </summary>
        bool GetAntispywareEnabled();

        /// <summary>
        /// Get the processor virtualization state.
        /// </summary>
        bool? CpuVirtualizationIsEnabled();

        /// <summary>
        /// Get Windows Hyper-V present state.
        /// </summary>
        bool? HypervisorPresent();
    }
}
