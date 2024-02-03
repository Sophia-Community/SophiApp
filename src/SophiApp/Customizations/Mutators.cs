// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
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
        public static void DiagnosticDataLevel(bool isEnabled)
        {
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
                reportingService.TryStop();
                OsService.SetServiceStartMode(reportingService, ServiceStartMode.Disabled);
            }
        }

        /// <summary>
        /// Sets Windows feature "Feedback frequency" state.
        /// </summary>
        public static void FeedbackFrequency(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets telemetry scheduled tasks state.
        /// </summary>
        public static void ScheduledTasks(bool isEnabled)
        {
            // var telemetryTasks = new List<Task>()
            // {
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\ProgramDataUpdater"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Autochk\\Proxy"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyMonitor"),
            //    TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyRefreshTask"),
            //    TaskService.Instance.GetTask("\\Microsoft\\XblGameSave\\XblGameSaveTask"),
            // };
        }

        /// <summary>
        /// Sets Windows feature "Sign-in info" state.
        /// </summary>
        public static void SigninInfo(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets language list access state.
        /// </summary>
        public static void LanguageListAccess(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets the permission for apps to use advertising ID state.
        /// </summary>
        public static void AdvertisingID(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets the Windows welcome experiences state.
        /// </summary>
        public static void WindowsWelcomeExperience(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets Windows tips state.
        /// </summary>
        public static void WindowsTips(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets the suggested content in the Settings app state.
        /// </summary>
        public static void SettingsSuggestedContent(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets the automatic installing suggested apps state.
        /// </summary>
        public static void AppsSilentInstalling(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets the Windows feature "Whats New" state.
        /// </summary>
        public static void WhatsNewInWindows(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets Windows feature "Tailored experiences" state.
        /// </summary>
        public static void TailoredExperiences(bool isEnabled)
        {
        }

        /// <summary>
        /// Sets Windows feature "Bing search" state.
        /// </summary>
        public static void BingSearch(bool isEnabled)
        {
        }
    }
}
