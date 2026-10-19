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
        /// Get processor caption from the CIM_Processor class.
        /// </summary>
        string GetProcessorCaption();

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
        /// Get the processor virtualization state.
        /// </summary>
        bool? CpuVirtualizationFirmwareIsEnabled();

        /// <summary>
        /// Get Windows Hyper-V present state.
        /// </summary>
        bool? HypervisorIsPresent();

        /// <summary>
        /// Set automatically manage paging file size for all drives.
        /// </summary>
        void SetPageFileAutoSize();

        /// <summary>
        /// Get Windows AI present state.
        /// </summary>
        bool WindowsAIPresent();
    }
}
