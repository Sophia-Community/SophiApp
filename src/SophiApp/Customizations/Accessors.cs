// <copyright file="Accessors.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using System.ServiceProcess;
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// Gets the os settings.
    /// </summary>
    public static class Accessors
    {
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();

        /// <summary>
        /// Gets DiagTrack service state.
        /// </summary>
        public static bool DiagTrackService()
        {
            var diagService = new ServiceController("DiagTrack");
            var diagFwRule = FirewallService.GetGroupRules("DiagTrack").First();
            return diagService.StartType == ServiceStartMode.Automatic && diagFwRule.Enabled && diagFwRule.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        }

        /// <summary>
        /// Gets Windows feature "Diagnostic data level" state.
        /// </summary>
        public static int DiagnosticDataLevel()
        {
            var allowTelemetry = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection")?.GetValue("AllowTelemetry") as int? ?? -1;
            var maxTelemetryAllowed = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")?.GetValue("MaxTelemetryAllowed") as int? ?? -1;
            var showedToastLevel = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")?.GetValue("ShowedToastAtLevel") as int? ?? -1;
            return allowTelemetry.Equals(1) && maxTelemetryAllowed.Equals(1) && showedToastLevel.Equals(1) ? 2 : 1;
        }

        /// <summary>
        /// Gets Windows feature "Error reporting" state.
        /// </summary>
        public static bool ErrorReporting()
        {
            var taskPath = "Microsoft\\Windows\\Windows Error Reporting\\QueueReporting";
            var reportingTask = TaskService.Instance.GetTask(taskPath) ?? throw new InvalidOperationException($"Failed to find a scheduled task");
            var reportingRegistryValue = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\Windows Error Reporting")?.GetValue("Disabled") as int? ?? -1;
            var reportingServiceStartType = new ServiceController("WerSvc").StartType;
            return reportingTask.State == TaskState.Disabled && reportingRegistryValue.Equals(1) && reportingServiceStartType == ServiceStartMode.Disabled;
        }

        /// <summary>
        /// Gets Windows feature "Feedback frequency" state.
        /// </summary>
        public static int FeedbackFrequency()
        {
            var numberOfSIUF = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Siuf\\Rules")?.GetValue("NumberOfSIUFInPeriod") as int? ?? -1;
            return numberOfSIUF.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Gets telemetry scheduled tasks state.
        /// </summary>
        public static bool ScheduledTasks()
        {
            var telemetryTasks = new List<Task>()
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
            };

            return telemetryTasks.TrueForAll(t => t is null) ? throw new InvalidOperationException("No scheduled telemetry tasks were found") : telemetryTasks.Exists(t => t.State == TaskState.Ready);
        }

        /// <summary>
        /// Gets Windows feature "Sign-in info" state.
        /// </summary>
        public static bool SigninInfo()
        {
            var sid = InstrumentationService.GetUserSidOrDefault(Environment.UserName);
            var userArso = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{sid}")?.GetValue("OptOut") ?? -1;
            return !userArso.Equals(1);
        }

        /// <summary>
        /// Gets language list access state.
        /// </summary>
        public static bool LanguageListAccess()
        {
            var httpAcceptLanguage = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\User Profile")?.GetValue("HttpAcceptLanguageOptOut") as int? ?? -1;
            return !httpAcceptLanguage.Equals(1);
        }

        /// <summary>
        /// Gets the permission for apps to use advertising ID state.
        /// </summary>
        public static bool AdvertisingID()
        {
            var advertisingInfo = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")?.GetValue("Enabled") as int? ?? -1;
            return !advertisingInfo.Equals(0);
        }

        /// <summary>
        /// Gets the Windows welcome experiences state.
        /// </summary>
        public static bool WindowsWelcomeExperience()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-310093Enabled") as int? ?? -1;
            return subscribedContent.Equals(1);
        }

        /// <summary>
        /// Gets Windows tips state.
        /// </summary>
        public static bool WindowsTips()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-338389Enabled") as int? ?? -1;
            return subscribedContent.Equals(1);
        }

        /// <summary>
        /// Gets the suggested content in the Settings app state.
        /// </summary>
        public static bool SettingsSuggestedContent()
        {
            var subscribedContent_338393 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-338393Enabled") as int? ?? -1;
            var subscribedContent_353694 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-353694Enabled") as int? ?? -1;
            var subscribedContent_353696 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-353696Enabled") as int? ?? -1;
            return !(subscribedContent_338393.Equals(0) && subscribedContent_353694.Equals(0) && subscribedContent_353696.Equals(0));
        }

        /// <summary>
        /// Gets the automatic installing suggested apps state.
        /// </summary>
        public static bool AppsSilentInstalling()
        {
            var silentInstalledApps = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SilentInstalledAppsEnabled") as int? ?? -1;
            return !silentInstalledApps.Equals(0);
        }

        /// <summary>
        /// Gets the Windows feature "Whats New" state.
        /// </summary>
        public static bool WhatsNewInWindows()
        {
            var scoobeSystemSetting = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")?.GetValue("ScoobeSystemSettingEnabled") as int? ?? -1;
            return !scoobeSystemSetting.Equals(0);
        }

        /// <summary>
        /// Gets Windows feature "Tailored experiences" state.
        /// </summary>
        public static bool TailoredExperiences()
        {
            var tailoredExperiences = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Privacy")?.GetValue("TailoredExperiencesWithDiagnosticDataEnabled") as int? ?? -1;
            return !tailoredExperiences.Equals(0);
        }

        /// <summary>
        /// Gets Windows feature "Bing search" state.
        /// </summary>
        public static bool BingSearch()
        {
            var disableSearchBox = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows\\Explorer")?.GetValue("DisableSearchBoxSuggestions") as int? ?? -1;
            return !disableSearchBox.Equals(0);
        }
    }
}
