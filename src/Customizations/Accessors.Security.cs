// <copyright file="Accessors.Security.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <content>
    /// The settings of the "Security" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Get administrator approval mode state.
        /// </summary>
        public static int AdminApprovalMode()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "ConsentPromptBehaviorAdmin") as int? ?? -1;
            return isEnabled.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get Windows network protection state.
        /// </summary>
        public static bool NetworkProtection()
        {
            var defenderIsDefaultAV = DataService.DefenderIsDefaultAV;

            if (defenderIsDefaultAV)
            {
                var networkProtection = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows Defender\\Windows Defender Exploit Guard\\Network Protection", "EnableNetworkProtection") as int? ?? -1;
                return networkProtection.Equals(1);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows PUApps detection state.
        /// </summary>
        public static bool PUAppsDetection()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                var puaProtection = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows Defender", "PUAProtection") as int? ?? -1;
                return puaProtection.Equals(1);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Microsoft Defender sandbox state.
        /// </summary>
        public static bool DefenderSandbox()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                // The sandbox state is set by the machine environment variable and applied after a restart only
                var forceSandbox = Environment.GetEnvironmentVariable("MP_FORCE_USE_SANDBOX", EnvironmentVariableTarget.Machine);
                return string.IsNullOrEmpty(forceSandbox) ? ProcessService.Exist("MsMpEngCP") : forceSandbox.Equals("1");
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows event viewer custom view state.
        /// </summary>
        public static bool EventViewerCustomView()
        {
            // The audit policy is read via the system API: the auditpol output is localized
            var inclusionSettingEnabled = ProcessCreationAuditIsEnabled() ?? false;
            var processCreationEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled") as int? ?? -1;
            var eventXml = XmlService.TryLoad(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml"));
            var eventQueryCount = eventXml?.SelectNodes("//QueryList/Query/Select")?.Count ?? -1;
            var moduleLoggingEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging") as int? ?? -1;
            var powerShellAsterisk = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*") as string ?? string.Empty;
            var scriptLoggingEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging") as int? ?? -1;
            return inclusionSettingEnabled && processCreationEnabled.Equals(1) && eventQueryCount.Equals(3) && moduleLoggingEnabled.Equals(1) && powerShellAsterisk.Equals("*") && scriptLoggingEnabled.Equals(1);
        }

        /// <summary>
        /// Get Windows SmartScreen state.
        /// </summary>
        public static bool AppsSmartScreen()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                var smartScreenIsEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "SmartScreenEnabled") as string ?? string.Empty;
                return !smartScreenIsEnabled.Equals("Off", StringComparison.OrdinalIgnoreCase);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows save zone state.
        /// </summary>
        public static bool SaveZoneInformation()
        {
            var saveZoneInformation = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation") as int? ?? -1;
            return !saveZoneInformation.Equals(1);
        }

        /// <summary>
        /// Get the DNS-over-HTTPS provider the network adapter in use is set to.
        /// </summary>
        /// <returns>The number of the provider, starting from 1. 0 if the queries are not encrypted. <see cref="ThirdPartyState.Id"/> if they are encrypted by other servers.</returns>
        public static int DNSoverHTTPS() => DnsService.GetProvider();

        /// <summary>
        /// Get the DNS-over-HTTPS servers that are not the ones of the providers.
        /// </summary>
        public static ThirdPartyState DNSoverHTTPSThirdParty() => new(string.Join(", ", DnsService.GetEncryptedServers()), string.Empty);

        /// <summary>
        /// Get Local Security Authority state.
        /// </summary>
        public static bool LocalSecurityAuthority()
        {
            var runAsPPL = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL") ?? -1;
            var runAsPPLBoot = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot") ?? -1;
            var runAsPPLPolicy = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\System", "RunAsPPL") ?? -1;

            // The checks below are required to enable the feature only, so it can always be disabled
            if ((runAsPPL.Equals(2) && runAsPPLBoot.Equals(2)) || runAsPPLPolicy.Equals(2))
            {
                return true;
            }

            // The feature needs virtualization: it is turned on in the firmware, or a hypervisor is already running
            if (!VirtualizationIsAvailable())
            {
                throw new CustomizationException(exceptionMessage: "Enable virtualization in UEFI", resourceKey: "UIModel_Exception_VirtualizationDisabled");
            }

            return false;
        }

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        private static extern bool AuditQuerySystemPolicy(ref Guid subCategoryGuids, uint policyCount, out nint auditPolicy);

        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        private static extern bool AuditFree(nint buffer);

        /// <summary>
        /// Determines whether success and failure auditing of the "Process Creation" subcategory is enabled.
        /// </summary>
        /// <returns>Auditing state or null if the audit policy cannot be queried.</returns>
        private static bool? ProcessCreationAuditIsEnabled()
        {
            const uint SuccessAndFailure = 0x3;
            var processCreation = new Guid("0CCE922B-69AE-11D9-BED3-505054503030");

            // The audit policy can be read only with the security privilege, which is turned off even for an administrator
            _ = PrivilegeHelper.TryEnable("SeSecurityPrivilege");

            if (!AuditQuerySystemPolicy(ref processCreation, 1, out var auditPolicy))
            {
                return null;
            }

            try
            {
                // AUDIT_POLICY_INFORMATION: GUID AuditSubCategoryGuid (16 bytes), ULONG AuditingInformation, GUID AuditCategoryGuid
                var auditingInformation = (uint)System.Runtime.InteropServices.Marshal.ReadInt32(auditPolicy, 16);
                return (auditingInformation & SuccessAndFailure) == SuccessAndFailure;
            }
            finally
            {
                _ = AuditFree(auditPolicy);
            }
        }
    }
}
