// <copyright file="Mutators.Privacy.cs" company="Team Sophia">
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
    /// The settings of the "Privacy" page.
    /// </content>
    public static partial class Mutators
    {
        /// <summary>
        /// Set DiagTrack service state.
        /// </summary>
        /// <param name="enable">DiagTrack service state.</param>
        public static void DiagTrackService(bool enable)
        {
            // Connected User Experiences and Telemetry
            // Disabling the "Connected User Experiences and Telemetry" service (DiagTrack) can cause you not being able to get XBOX achievements anymore and affects Feedback Hub
            using var diagTrackService = new System.ServiceProcess.ServiceController("DiagTrack");
            // The firewall rules may be missing
            var firewallRules = FirewallService.GetGroupRules("DiagTrack").ToList();

            if (enable)
            {
                OsService.SetStartMode(diagTrackService, ServiceStartMode.Automatic);
                diagTrackService.TryStart();

                // Allow connection for the Unified Telemetry Client Outbound Traffic
                firewallRules.ForEach(rule =>
                {
                    rule.Enabled = true;
                    rule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                });
                return;
            }

            diagTrackService.TryStop();
            OsService.SetStartMode(diagTrackService, ServiceStartMode.Disabled);

            // Block connection for the Unified Telemetry Client Outbound Traffic
            firewallRules.ForEach(rule =>
            {
                rule.Enabled = true;
                rule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            });
        }

        /// <summary>
        /// Set Windows feature "Diagnostic data level" state.
        /// </summary>
        /// <param name="state">Diagnostic data level state.</param>
        public static void DiagnosticDataLevel(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");

            if (state.Equals(2))
            {
                if (DataService.OsProperties.Edition.Contains("Enterprise") || DataService.OsProperties.Edition.Contains("Education"))
                {
                    // 0 — Diagnostic data off
                    Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                }
                else
                {
                    // 1 — Send required diagnostic data
                    Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 1, RegistryValueKind.DWord);
                }

                Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 1, RegistryValueKind.DWord);
                return;
            }

            // 3 — Send optional diagnostic data
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Error reporting" state.
        /// </summary>
        /// <param name="enable">Feature state.</param>
        public static void ErrorReporting(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");

            if (enable)
            {
                ScheduledTaskService.SetState(task: reportingTask, enabled: true);
                OsService.SetStartMode(werService, ServiceStartMode.Manual);
                werService.TryStart();
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", 0, RegistryValueKind.DWord);
                return;
            }

            ScheduledTaskService.TryStop(task: reportingTask);
            ScheduledTaskService.SetState(task: reportingTask, enabled: false);
            werService.TryStop();
            OsService.SetStartMode(werService, ServiceStartMode.Disabled);
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Feedback frequency" state.
        /// </summary>
        /// <param name="state">Feedback frequency state.</param>
        public static void FeedbackFrequency(int state)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "DoNotShowFeedbackNotifications");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "DoNotShowFeedbackNotifications");
            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds");

            if (state.Equals(2))
            {
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod");
        }

        /// <summary>
        /// Set the state of the scheduled tasks that collect and send diagnostic data.
        /// </summary>
        /// <param name="tasks">The paths of the tasks to change, with their new state.</param>
        public static void ScheduledTasks(IReadOnlyDictionary<string, bool> tasks)
        {
            foreach (var (path, enable) in tasks)
            {
                // Only the tasks of the list are changed
                if (Accessors.DiagnosticTaskPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
                {
                    ScheduledTaskService.SetState(ScheduledTaskService.GetTaskOrDefault(path), enable);
                }
            }
        }

        /// <summary>
        /// Set Windows feature "Sign-in info" state.
        /// </summary>
        /// <param name="enable">Sign-in info state.</param>
        public static void SigninInfo(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableAutomaticRestartSignOn");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableAutomaticRestartSignOn");
            var userSid = InstrumentationService.GetUserSid(Environment.UserName);
            var arsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}";

            if (enable)
            {
                Registry.LocalMachine.DeleteSubKeyValue(arsoPath, "OptOut");
                return;
            }

            Registry.LocalMachine.CreateKeyAndSetValue(arsoPath, "OptOut", 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set language list access state.
        /// </summary>
        /// <param name="enable">Language list state.</param>
        public static void LanguageListAccess(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Control Panel\\International\\User Profile", "HttpAcceptLanguageOptOut");
                return;
            }

            Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\International\\User Profile", "HttpAcceptLanguageOptOut", 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the permission for apps to use advertising ID state.
        /// </summary>
        /// <param name="enable">Advertising ID state.</param>
        public static void AdvertisingID(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo", "DisabledByGroupPolicy");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo", "DisabledByGroupPolicy");
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows welcome experiences state.
        /// </summary>
        /// <param name="enable">Windows welcome experiences state.</param>
        public static void WindowsWelcomeExperience(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-310093Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows tips state.
        /// </summary>
        /// <param name="enable">Windows tips state.</param>
        public static void WindowsTips(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableSoftLanding");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableSoftLanding");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-338389Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the suggested content in the Settings app state.
        /// </summary>
        /// <param name="enable">Suggested content state.</param>
        public static void SettingsSuggestedContent(bool enable)
        {
            new List<string> { "SubscribedContent-338393Enabled", "SubscribedContent-353694Enabled", "SubscribedContent-353696Enabled" }
            .ForEach(content => Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", content, enable ? 1 : 0, RegistryValueKind.DWord));
        }

        /// <summary>
        /// Set the automatic installing suggested apps state.
        /// </summary>
        /// <param name="enable">Suggested apps state.</param>
        public static void AppsSilentInstalling(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableWindowsConsumerFeatures");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableWindowsConsumerFeatures");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SilentInstalledAppsEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="enable">Whats New state.</param>
        public static void WhatsNewInWindows(bool enable)
        {
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement", "ScoobeSystemSettingEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="enable">Tailored experiences state.</param>
        public static void TailoredExperiences(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableTailoredExperiencesWithDiagnosticData");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableTailoredExperiencesWithDiagnosticData");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Bing search" state.
        /// </summary>
        /// <param name="enable">Bing search state.</param>
        public static void BingSearch(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(scope: LGPOScope.User, path: "Software\\Policies\\Microsoft\\Windows\\Explorer", name: "DisableSearchBoxSuggestions", type: "DWORD", value: "1");
        }

        /// <summary>
        /// Set recommended troubleshooting state.
        /// </summary>
        /// <param name="state">Recommended troubleshooting state.</param>
        public static void RecommendedTroubleshooting(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");
            // Recommended troubleshooting requires sending optional diagnostic data
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");

            // Turn on Windows Error Reporting
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            ScheduledTaskService.SetState(task: reportingTask, enabled: true);
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");
            OsService.SetStartMode(werService, ServiceStartMode.Manual);
            werService.TryStart();
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", 0, RegistryValueKind.DWord);

            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\WindowsMitigation", "UserPreference", state.Equals(1) ? 3 : 2, RegistryValueKind.DWord);
        }
    }
}
