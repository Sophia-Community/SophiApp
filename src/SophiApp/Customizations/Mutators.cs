// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32.TaskScheduler;

    /// <summary>
    /// Sets the os settings.
    /// </summary>
    public static class Mutators
    {
        /// <summary>
        /// Sets DiagTrack service state.
        /// </summary>
        public static void DiagTrackService(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows feature "Diagnostic data level" state.
        /// </summary>
        public static void DiagnosticDataLevel(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows feature "Error reporting" state.
        /// </summary>
        public static void ErrorReporting(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows feature "Feedback frequency" state.
        /// </summary>
        public static void FeedbackFrequency(bool isChecked)
        {
        }

        /// <summary>
        /// Sets telemetry scheduled tasks state.
        /// </summary>
        public static void ScheduledTasks(bool isChecked)
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
        }

        /// <summary>
        /// Sets Windows feature "Sign-in info" state.
        /// </summary>
        public static void SigninInfo(bool isChecked)
        {
        }

        /// <summary>
        /// Sets language list access state.
        /// </summary>
        public static void LanguageListAccess(bool isChecked)
        {
        }

        /// <summary>
        /// Sets the permission for apps to use advertising ID state.
        /// </summary>
        public static void AdvertisingID(bool isChecked)
        {
        }

        /// <summary>
        /// Sets the Windows welcome experiences state.
        /// </summary>
        public static void WindowsWelcomeExperience(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows tips state.
        /// </summary>
        public static void WindowsTips(bool isChecked)
        {
        }

        /// <summary>
        /// Sets the suggested content in the Settings app state.
        /// </summary>
        public static void SettingsSuggestedContent(bool isChecked)
        {
        }

        /// <summary>
        /// Sets the automatic installing suggested apps state.
        /// </summary>
        public static void AppsSilentInstalling(bool isChecked)
        {
        }

        /// <summary>
        /// Sets the Windows feature "Whats New" state.
        /// </summary>
        public static void WhatsNewInWindows(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows feature "Tailored experiences" state.
        /// </summary>
        public static void TailoredExperiences(bool isChecked)
        {
        }

        /// <summary>
        /// Sets Windows feature "Bing search" state.
        /// </summary>
        public static void BingSearch(bool isChecked)
        {
        }
    }
}
