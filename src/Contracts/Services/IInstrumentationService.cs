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
        /// Get IDs of the management console processes that have any of the snap-ins opened.
        /// </summary>
        /// <param name="snapIns">Snap-in file names, e.g. gpedit.msc.</param>
        List<int> GetManagementConsoleProcessIds(params string[] snapIns);

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
        /// Calls a static method of a WMI class, as the cmdlets that are built on WMI do.
        /// </summary>
        /// <param name="nameSpace">The namespace of the class.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="method">The name of the method.</param>
        /// <param name="arguments">The arguments of the method, if it has any.</param>
        /// <returns>False if the method could not be called.</returns>
        bool TryInvokeStaticMethod(string nameSpace, string className, string method, IReadOnlyDictionary<string, object>? arguments = null);

        /// <summary>
        /// Starts decrypting the system drive that is encrypted with BitLocker, as the cmdlet Disable-BitLocker does.
        /// </summary>
        /// <returns>False if the decryption was not started.</returns>
        bool TryDecryptSystemDrive();

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
        /// Get CPU virtualization state.
        /// </summary>
        bool? CPUVirtualizationIsEnabled();

        /// <summary>
        /// Get Windows Hyper-V present state.
        /// </summary>
        bool? HypervisorIsPresent();

        /// <summary>
        /// Determines whether the PC has a built-in AI accelerator (NPU).
        /// </summary>
        bool HasAIAccelerator();

        /// <summary>
        /// Set automatically manage paging file size for all drives.
        /// </summary>
        void SetPageFileAutoSize();
    }
}
