// <copyright file="Accessors.Privacy.cs" company="Team Sophia">
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
    /// The settings of the "Privacy" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Gets the scheduled tasks that collect and send diagnostic data.
        /// </summary>
        internal static IReadOnlyList<string> DiagnosticTaskPaths { get; } = new[]
        {
            // Gathers Win32 application data for App Backup scenario
            "\\Microsoft\\Windows\\Application Experience\\MareBackup",

            // Collects program telemetry information if opted-in to the Microsoft Customer Experience Improvement Program
            "\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser",
            "\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser Exp",

            // Scans startup entries and raises notification to the user if there are too many startup entries
            "\\Microsoft\\Windows\\Application Experience\\StartupAppTask",

            // Collects and uploads autochk SQM data if opted-in to the Microsoft Customer Experience Improvement Program
            "\\Microsoft\\Windows\\Autochk\\Proxy",

            // Collects and sends usage data to Microsoft if the user has consented to participate in the Windows Customer Experience Improvement Program
            "\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator",

            // Collects Universal Serial Bus related statistics and sends them to Microsoft if the user has consented to participate in Windows CEIP
            "\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip",

            // Reports general disk and system information to Microsoft for users participating in the Customer Experience Program
            "\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector",

            // Shows various Map related toasts
            "\\Microsoft\\Windows\\Maps\\MapsToastTask",

            // Checks for updates to maps which you have downloaded for offline use
            "\\Microsoft\\Windows\\Maps\\MapsUpdateTask",
        };

        /// <summary>
        /// Get DiagTrack service state.
        /// </summary>
        public static bool DiagTrackService()
        {
            if (!OsService.Exist("DiagTrack"))
            {
                throw new CustomizationException(exceptionMessage: "\"DiagTrack\" service was not found", resourceKey: "UIModel_Exception_DiagTrackServiceNotFound");
            }

            using var diagTrackService = new System.ServiceProcess.ServiceController("DiagTrack");
            var firewallRules = FirewallService.GetGroupRules("DiagTrack").ToList();

            // The firewall rules may be missing
            if (diagTrackService.StartType == ServiceStartMode.Disabled && firewallRules.TrueForAll(rule => rule.Enabled && rule.Action == NET_FW_ACTION_.NET_FW_ACTION_BLOCK))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Windows feature "Diagnostic data level" state.
        /// </summary>
        public static int DiagnosticDataLevel()
        {
            var allowTelemetry = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry") as int? ?? -1;
            var telemetryAllowed = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed") as int? ?? -1;
            var toastLevel = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel") as int? ?? -1;
            // AllowTelemetry is 0 on Enterprise and Education editions and 1 on the others when the minimal level is set
            return (allowTelemetry.Equals(0) || allowTelemetry.Equals(1)) && telemetryAllowed.Equals(1) && toastLevel.Equals(1) ? 2 : 1;
        }

        /// <summary>
        /// Get Windows feature "Error reporting" state.
        /// </summary>
        public static bool ErrorReporting()
        {
            // Windows is broken if this service is missing
            if (!OsService.Exist("WerSvc"))
            {
                throw new CustomizationException(exceptionMessage: "\"WerSvc\" service was not found", resourceKey: "UIModel_Exception_ErrorReportingServiceNotFound");
            }

            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");
            // The scheduled task may be missing
            return !((reportingTask is null || reportingTask.State == TaskState.Disabled) && werService.StartType == ServiceStartMode.Disabled);
        }

        /// <summary>
        /// Get Windows feature "Feedback frequency" state.
        /// </summary>
        public static int FeedbackFrequency()
        {
            var siufPeriod = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod") as int? ?? -1;
            return siufPeriod.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get the scheduled tasks that collect and send diagnostic data, with their state.
        /// </summary>
        public static IReadOnlyList<SettingCheckItem> ScheduledTasks()
        {
            var tasks = DiagnosticTaskPaths
                .Select(path => (Path: path, Task: ScheduledTaskService.GetTaskOrDefault(path)))
                .Where(found => found.Task is not null)
                .Select(found => new SettingCheckItem(found.Path, found.Task!.Name, GetTaskDescription(found.Task), found.Task.State != TaskState.Disabled))
                .ToList();

            return tasks.Count == 0
                ? throw new CustomizationException(exceptionMessage: "No scheduled telemetry tasks were found", resourceKey: "UIModel_Exception_ScheduledTasks")
                : tasks;
        }

        /// <summary>
        /// Get Windows feature "Sign-in info" state.
        /// </summary>
        public static bool SigninInfo()
        {
            var userSid = InstrumentationService.GetUserSid(Environment.UserName);
            var arsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}";
            var optOut = Registry.LocalMachine.GetSubKeyValue(arsoPath, "OptOut") as int? ?? -1;
            return !optOut.Equals(1);
        }

        /// <summary>
        /// Get language list access state.
        /// </summary>
        public static bool LanguageListAccess()
        {
            var httpAcceptLanguage = Registry.CurrentUser.GetSubKeyValue("Control Panel\\International\\User Profile", "HttpAcceptLanguageOptOut") as int? ?? -1;
            return !httpAcceptLanguage.Equals(1);
        }

        /// <summary>
        /// Get the permission for apps to use advertising ID state.
        /// </summary>
        public static bool AdvertisingID()
        {
            var advertisingInfo = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled") as int? ?? -1;
            return !advertisingInfo.Equals(0);
        }

        /// <summary>
        /// Get the Windows welcome experiences state.
        /// </summary>
        public static bool WindowsWelcomeExperience()
        {
            var subscribedContent = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-310093Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Get Windows tips state.
        /// </summary>
        public static bool WindowsTips()
        {
            var subscribedContent = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-338389Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Get the suggested content in the Settings app state.
        /// </summary>
        public static bool SettingsSuggestedContent()
        {
            var subscribedContent = new List<int>()
            {
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-338393Enabled") as int? ?? -1,
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-353694Enabled") as int? ?? -1,
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-353696Enabled") as int? ?? -1,
            }
            .TrueForAll(subscribed => subscribed.Equals(0));
            return !subscribedContent;
        }

        /// <summary>
        /// Get the automatic installing suggested apps state.
        /// </summary>
        public static bool AppsSilentInstalling()
        {
            var appsIsSilentInstalled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SilentInstalledAppsEnabled") as int? ?? -1;
            return !appsIsSilentInstalled.Equals(0);
        }

        /// <summary>
        /// Get the Windows feature "Whats New" state.
        /// </summary>
        public static bool WhatsNewInWindows()
        {
            var scoobeSettingIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement", "ScoobeSystemSettingEnabled") as int? ?? -1;
            return !scoobeSettingIsEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows feature "Tailored experiences" state.
        /// </summary>
        public static bool TailoredExperiences()
        {
            var tailoredExperiencesIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled") as int? ?? -1;
            return !tailoredExperiencesIsEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows feature "Bing search" state.
        /// </summary>
        public static bool BingSearch()
        {
            var searchBoxIsDisabled = Registry.CurrentUser.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions") as int? ?? -1;
            return !searchBoxIsDisabled.Equals(1);
        }

        /// <summary>
        /// Get recommended troubleshooting state.
        /// </summary>
        public static int RecommendedTroubleshooting()
        {
            // Recommended troubleshooting needs the Windows Error Reporting service
            if (!OsService.Exist("WerSvc"))
            {
                throw new CustomizationException(exceptionMessage: "\"WerSvc\" service was not found", resourceKey: "UIModel_Exception_ErrorReportingServiceNotFound");
            }

            var userPreference = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsMitigation", "UserPreference") as int? ?? -1;
            return userPreference.Equals(3) ? 1 : 2;
        }

        // What Windows itself says about a task. A task without a description shows its folder
        private static string GetTaskDescription(Task task)
        {
            try
            {
                var description = IndirectString.Resolve(task.Definition.RegistrationInfo.Description);
                return description.Length > 0 ? description : task.Folder.Path;
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException($"Reading the description of the task \"{task.Name}\"", ex);
                return task.Folder.Path;
            }
        }
    }
}
