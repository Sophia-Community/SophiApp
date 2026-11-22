// <copyright file="Mutators.Security.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.Json.Nodes;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <content>
    /// The settings of the "Security" page.
    /// </content>
    public static partial class Mutators
    {
        /// <summary>
        /// Set administrator approval mode state.
        /// </summary>
        /// <param name="state">Administrator approval mode state.</param>
        public static void AdminApprovalMode(int state)
        {
            using var approvalKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            approvalKey?.DeleteValue("FilterAdministratorToken", false);
            approvalKey?.SetValue("ConsentPromptBehaviorUser", 3, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableInstallerDetection", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("ValidateAdminCodeSignatures", 0, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableSecureUIAPaths", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableLUA", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("PromptOnSecureDesktop", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableVirtualization", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableUIADesktopToggle", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("ConsentPromptBehaviorAdmin", state.Equals(1) ? 5 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows network protection state.
        /// </summary>
        /// <param name="enable">Network protection state.</param>
        public static void NetworkProtection(bool enable)
        {
            // What the cmdlet Set-MpPreference calls. 1 is "Enabled", 0 is "Disabled"
            _ = InstrumentationService.TryInvokeStaticMethod("root\\Microsoft\\Windows\\Defender", "MSFT_MpPreference", "Set", new Dictionary<string, object> { ["EnableNetworkProtection"] = (byte)(enable ? 1 : 0) });
        }

        /// <summary>
        /// Set Windows PUApps detection state.
        /// </summary>
        /// <param name="enable">PUApps detection state.</param>
        public static void PUAppsDetection(bool enable)
        {
            // What the cmdlet Set-MpPreference calls. 1 is "Enabled", 0 is "Disabled"
            _ = InstrumentationService.TryInvokeStaticMethod("root\\Microsoft\\Windows\\Defender", "MSFT_MpPreference", "Set", new Dictionary<string, object> { ["PUAProtection"] = (byte)(enable ? 1 : 0) });
        }

        /// <summary>
        /// Set Microsoft Defender sandbox state.
        /// </summary>
        /// <param name="enable">Microsoft Defender sandbox state.</param>
        public static void DefenderSandbox(bool enable)
        {
            // The machine environment variable is set by .NET itself: starting PowerShell to run setx.exe is not needed
            Environment.SetEnvironmentVariable("MP_FORCE_USE_SANDBOX", enable ? "1" : "0", EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Set Windows Event Viewer custom view state.
        /// </summary>
        /// <param name="enable">Event Viewer custom view state.</param>
        public static void EventViewerCustomView(bool enable)
        {
            ProcessService.CloseManagementConsoles("eventvwr.msc");

            if (enable)
            {
                // With the audit policy file of the Local Group Policy every policy refresh turns off all audit subcategories that are not listed there
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "GroupPolicy\\Machine\\Microsoft\\Windows NT\\Audit\\audit.csv").TryDelete();
                // Enable events auditing generated when a process is created (starts)
                _ = ProcessService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "auditpol.exe"), "/set /subcategory:\"{0CCE922B-69AE-11D9-BED3-505054503030}\" /success:enable /failure:enable");
                // Include command line in process creation events
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled", "DWORD", "1");
                // Enable logging for all Windows PowerShell modules
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*", "*", RegistryValueKind.String);
                Registry.LocalMachine.SetValueIfKeyExists("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*", "SZ", "*");
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging", "DWORD", "1");
                // Enable logging for all PowerShell scripts input to the Windows PowerShell event log
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging", "DWORD", "1");
                // Create custom "Process Creation" view in the Event Viewer
                var eventsXml = $@"<ViewerConfig>
	<QueryConfig>
		<QueryNode>
			<Name>{"UIModel_EventViewerCustomView_QueryName".GetLocalized()}</Name>
			<Description>{"UIModel_EventViewerCustomView_QueryDescription".GetLocalized()}</Description>
			<QueryList>
				<Query Id=""0"">
					<Select Path=""Microsoft-Windows-PowerShell/Operational"">*[System[(EventID=4103 or EventID=4104)]]</Select>
					<Select Path=""Windows PowerShell"">*[System[(EventID=400 or EventID=403 or EventID=800)]]</Select>
					<Select Path=""Security"">*[System[(EventID=4688)]]</Select>
				</Query>
			</QueryList>
		</QueryNode>
	</QueryConfig>
</ViewerConfig>";
                // Save ProcessCreation.xml in UTF-8 with BOM encoding
                FileService.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml"), eventsXml, Encoding.UTF8);
                return;
            }

            // Disable events auditing generated when a process is created (starts)
            _ = ProcessService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "auditpol.exe"), "/set /subcategory:\"{0CCE922B-69AE-11D9-BED3-505054503030}\" /success:disable /failure:disable");
            // Do not include command line in process creation events
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled");
            // Disable logging for all Windows PowerShell modules
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging");
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*");
            // Disable logging for all PowerShell scripts input to the Windows PowerShell event log
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging");
            // Remove the "Process Creation" custom view in the Event Viewer
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml").TryDelete();
        }

        /// <summary>
        /// Set Windows SmartScreen state.
        /// </summary>
        /// <param name="enable">Windows SmartScreen state.</param>
        public static void AppsSmartScreen(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "SmartScreenEnabled", $"{(enable ? "Warn" : "Off")}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Windows save zone state.
        /// </summary>
        /// <param name="enable">Windows save zone state.</param>
        public static void SaveZoneInformation(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation", 1, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation", "DWORD", "1");
        }

        /// <summary>
        /// Set the DNS-over-HTTPS provider.
        /// </summary>
        /// <param name="state">The number of the provider, starting from 1. 0 sets the default DNS servers of the network.</param>
        public static void DNSoverHTTPS(int state)
        {
            // Remove all policies in order to make changes visible in UI
            foreach (var policy in new[] { "DoHPolicy", "DohPolicySetting", "DotPolicySetting" })
            {
                GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows NT\\DNSClient", policy);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows NT\\DNSClient", policy);
            }

            DnsService.SetProvider(state);
        }

        /// <summary>
        /// Set Local Security Authority state.
        /// </summary>
        /// <param name="enable">Local Security Authority state.</param>
        public static void LocalSecurityAuthority(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\System", "RunAsPPL");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\System", "RunAsPPL");

            if (enable)
            {
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL", 2, RegistryValueKind.DWord);
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot", 2, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL");
            Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot");
        }
    }
}
