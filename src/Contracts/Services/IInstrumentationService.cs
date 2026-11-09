// <copyright file="IInstrumentationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;
    using System.Diagnostics;
    using System.Management;

    /// <summary>
    /// A service for working with WMI API.
    /// </summary>
    public interface IInstrumentationService
    {
        /// <summary>
        /// Indicates that the DAC used in the video adapter is external type.
        /// </summary>
        bool DetectDACType();

        /// <summary>
        /// Defines the run on the virtual machine.
        /// </summary>
        bool DetectVM();

        /// <summary>
        /// Get eventvwr.msc process ID.
        /// </summary>
        List<int>? GetEventViewerConsoleProcessId();

        /// <summary>
        /// Get the properties of the Win32_OperatingSystem class.
        /// </summary>
        OsProperties GetOsProperties();

        /// <summary>
        /// Get the process owner name.
        /// </summary>
        /// <param name="process">The process for which to find an owner.</param>
        string GetProcessOwnerName(Process? process);

        /// <summary>
        /// Get data from the AntiVirusProduct class.
        /// </summary>
        List<ManagementObject> GetAntivirusProducts();

        /// <summary>
        /// Gets the WMI/CIM class instances.
        /// </summary>
        /// <param name="nameSpace">The target CIM namespace.</param>
        /// <param name="className">The name of the WMI/CIM class to query.</param>
        IEnumerable<ManagementObject> GetCimInstance(string nameSpace, string className);

        /// <summary>
        /// Get data from the PowerPlan class.
        /// </summary>
        List<ManagementObject> GetPowerPlans();

        /// <summary>
        /// Get user account SID.
        /// </summary>
        /// <param name="name">A user name.</param>
        string GetUserSid(string name);

        /// <summary>
        /// Get Microsoft Defender antispyware enabled property value.
        /// </summary>
        bool GetAntiSpywareEnabled();

        /// <summary>
        /// Get CPU virtualization state.
        /// </summary>
        bool? CPUVirtualizationIsEnabled();

        /// <summary>
        /// Get Windows Hyper-V present state.
        /// </summary>
        bool? HypervisorIsPresent();

        /// <summary>
        /// Set automatically manage paging file size for all drives.
        /// </summary>
        void SetPageFileAutoSize();
    }
}
