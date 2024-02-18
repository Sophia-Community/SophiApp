// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using System.Collections.Generic;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <summary>
    /// Sets the os settings.
    /// </summary>
    public static class Mutators
    {
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();

        /// <summary>
        /// Sets DiagTrack service state.
        /// </summary>
        /// <param name="isEnabled">DiagTrack service state.</param>
        public static void DiagTrackService(bool isEnabled)
        {
            var diagTrackService = new ServiceController("DiagTrack");
            var firewallRule = FirewallService.GetGroupRules("DiagTrack").First();

            if (isEnabled)
            {
                OsService.SetServiceStartMode(diagTrackService, ServiceStartMode.Automatic);
                diagTrackService.TryStart();
                firewallRule.Enabled = true;
                firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                return;
            }

            diagTrackService.TryStop();
            OsService.SetServiceStartMode(diagTrackService, ServiceStartMode.Disabled);
            firewallRule.Enabled = true;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

        /// <summary>
        /// Sets Windows feature "Diagnostic data level" state.
        /// </summary>
        /// <param name="state">Diagnostic data level state.</param>
        public static void DiagnosticDataLevel(int state)
        {
            if (state.Equals(2))
            {
                var osEdition = CommonDataService.OsProperties.Edition;
                var isEnterpriseOrEducation = osEdition.Contains("Enterprise") || osEdition.Contains("Education");
                Registry.LocalMachine.OpenOrCreateSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection")
                    ?.SetValue("AllowTelemetry", isEnterpriseOrEducation ? 0 : 1, RegistryValueKind.DWord);
                Registry.LocalMachine.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")
                    ?.SetValue("MaxTelemetryAllowed", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")
                    ?.SetValue("ShowedToastAtLevel", 1, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")
                ?.SetValue("MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")
                ?.SetValue("ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection", true)
                ?.DeleteValue("AllowTelemetry", false);
        }

        /// <summary>
        /// Sets Windows feature "Error reporting" state.
        /// </summary>
        /// <param name="isEnabled">Feature state.</param>
        public static void ErrorReporting(bool isEnabled)
        {
            var reportingRegistryPath = "Software\\Microsoft\\Windows\\Windows Error Reporting";
            var reportingTask = TaskService.Instance.GetTask("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            var reportingService = new ServiceController("WerSvc");

            if (isEnabled)
            {
                reportingTask.Enabled = true;
                Registry.CurrentUser.OpenSubKey(reportingRegistryPath, true)?.DeleteValue("Disabled", false);
                OsService.SetServiceStartMode(reportingService, ServiceStartMode.Manual);
                reportingService.TryStart();
                return;
            }

            if (!CommonDataService.OsProperties.Edition.Equals("Core"))
            {
                reportingTask.Enabled = false;
                Registry.CurrentUser.OpenSubKey(reportingRegistryPath, true)?.SetValue("Disabled", 1, RegistryValueKind.DWord);
            }

            reportingService.TryStop();
            OsService.SetServiceStartMode(reportingService, ServiceStartMode.Disabled);
        }

        /// <summary>
        /// Sets Windows feature "Feedback frequency" state.
        /// </summary>
        /// <param name="state">Feedback frequency state.</param>
        public static void FeedbackFrequency(int state)
        {
            var siufRulesPath = "Software\\Microsoft\\Siuf\\Rules";

            if (state.Equals(2))
            {
                Registry.CurrentUser.OpenOrCreateSubKey(siufRulesPath)
                    ?.SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKey(siufRulesPath, false);
        }

        /// <summary>
        /// Sets telemetry scheduled tasks state.
        /// </summary>
        /// <param name="isEnabled">Scheduled tasks state.</param>
        public static void ScheduledTasks(bool isEnabled)
        {
            new List<Task?>()
             {
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\ProgramDataUpdater"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Autochk\\Proxy"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyMonitor"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyRefreshTask"),
                TaskService.Instance.GetTask("\\Microsoft\\XblGameSave\\XblGameSaveTask"),
                TaskService.Instance.GetTask("\\Microsoft\\XblGameSave\\XblGameSaveTask1"),
             }
            .ForEach(task =>
             {
                 if (task is not null)
                 {
                     task.Enabled = isEnabled;
                 }
             });
        }

        /// <summary>
        /// Sets Windows feature "Sign-in info" state.
        /// </summary>
        /// <param name="isEnabled">Sign-in info state.</param>
        public static void SigninInfo(bool isEnabled)
        {
            var sid = InstrumentationService.GetUserSid(Environment.UserName);
            var userArsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{sid}";
            var optOut = "OptOut";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(userArsoPath, true)?.DeleteValue(optOut, false);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(userArsoPath).SetValue(optOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets language list access state.
        /// </summary>
        /// <param name="isEnabled">Language list state.</param>
        public static void LanguageListAccess(bool isEnabled)
        {
            var userProfilePath = "Control Panel\\International\\User Profile";
            var httpOptOut = "HttpAcceptLanguageOptOut";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.DeleteValue(httpOptOut, false);
                return;
            }

            Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.SetValue(httpOptOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the permission for apps to use advertising ID state.
        /// </summary>
        /// <param name="isEnabled">Advertising ID state.</param>
        public static void AdvertisingID(bool isEnabled)
        {
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")
                .SetValue("Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the Windows welcome experiences state.
        /// </summary>
        /// <param name="isEnabled">Windows welcome experiences state.</param>
        public static void WindowsWelcomeExperience(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-310093Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows tips state.
        /// </summary>
        /// <param name="isEnabled">Windows tips state.</param>
        public static void WindowsTips(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-338389Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the suggested content in the Settings app state.
        /// </summary>
        /// <param name="isEnabled">Suggested content state.</param>
        public static void SettingsSuggestedContent(bool isEnabled)
        {
            new List<string> { "SubscribedContent-353694Enabled", "SubscribedContent-353696Enabled", "SubscribedContent-338393Enabled" }
            .ForEach(content => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue(content, isEnabled ? 1 : 0, RegistryValueKind.DWord));
        }

        /// <summary>
        /// Sets the automatic installing suggested apps state.
        /// </summary>
        /// <param name="isEnabled">Suggested apps state.</param>
        public static void AppsSilentInstalling(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SilentInstalledAppsEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="isEnabled">Whats New state.</param>
        public static void WhatsNewInWindows(bool isEnabled)
        {
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")
                ?.SetValue("ScoobeSystemSettingEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="isEnabled">Tailored experiences state.</param>
        public static void TailoredExperiences(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", true)
                ?.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows feature "Bing search" state.
        /// </summary>
        /// <param name="isEnabled">Bing search state.</param>
        public static void BingSearch(bool isEnabled)
        {
            var explorerPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var disableSuggestions = "DisableSearchBoxSuggestions";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(explorerPath, true)?.DeleteValue(disableSuggestions);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(explorerPath).SetValue(disableSuggestions, 1, RegistryValueKind.DWord);
        }
    }
}
