// <copyright file="SettingNeeds.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    /// <summary>
    /// What Windows needs after a setting is applied, so that the change is seen.
    /// </summary>
    internal static class SettingNeeds
    {
        // The settings that do not touch File Explorer, the taskbar or the Start menu: services, scheduled tasks, Windows Update,
        // power, Microsoft Defender, installs. A setting that is not listed is taken as one that needs File Explorer restarted,
        // so a new setting is safe until it is looked at
        private static readonly HashSet<string> WithoutFileExplorer =
        [
            // Privacy
            nameof(Accessors.DiagTrackService),
            nameof(Accessors.DiagnosticDataLevel),
            nameof(Accessors.ErrorReporting),
            nameof(Accessors.RecommendedTroubleshooting),
            nameof(Accessors.FeedbackFrequency),
            nameof(Accessors.ScheduledTasks),
            nameof(Accessors.AdvertisingID),
            nameof(Accessors.LanguageListAccess),
            nameof(Accessors.TailoredExperiences),
            nameof(Accessors.SettingsSuggestedContent),
            nameof(Accessors.WindowsTips),
            nameof(Accessors.AppsSilentInstalling),
            nameof(Accessors.WindowsWelcomeExperience),
            nameof(Accessors.WhatsNewInWindows),
            nameof(Accessors.SigninInfo),

            // System
            nameof(Accessors.InstallWSL),
            nameof(Accessors.InstallVisualC_x86),
            nameof(Accessors.InstallVisualC_x64),
            nameof(Accessors.InstallDotNetRuntime_10),
            nameof(Accessors.InstallDotNetRuntime_11),
            nameof(Accessors.WindowsSandbox),
            nameof(Accessors.DeliveryOptimization),
            nameof(Accessors.UpdateMicrosoftProducts),
            nameof(Accessors.RestartNotification),
            nameof(Accessors.RestartDeviceAfterUpdate),
            nameof(Accessors.ActiveHours),
            nameof(Accessors.WindowsLatestUpdate),
            nameof(Accessors.Hibernation),
            nameof(Accessors.PowerPlan),
            nameof(Accessors.NetworkAdaptersSavePower),
            nameof(Accessors.InputMethod),
            nameof(Accessors.AppsLanguageSwitch),
            nameof(Accessors.NumLock),
            nameof(Accessors.CapsLock),
            nameof(Accessors.StickyShift),
            nameof(Accessors.MouseAcceleration),
            nameof(Accessors.StorageSense),
            nameof(Accessors.ReservedStorage),
            nameof(Accessors.ThumbnailCacheRemoval),
            nameof(Accessors.Win32LongPathsSupport),
            nameof(Accessors.SaveRestartableApps),
            nameof(Accessors.DefaultTerminalApp),
            nameof(Accessors.WindowsManageDefaultPrinter),
            nameof(Accessors.PreventEdgeShortcutCreation),
            nameof(Accessors.BSoDStopError),
            nameof(Accessors.RegistryBackup),
            nameof(Accessors.ScanRegistryPolicies),

            // Gaming
            nameof(Accessors.XBOXGameBar),
            nameof(Accessors.XBOXGameTips),
            nameof(Accessors.GPUScheduling),

            // Task Scheduler
            nameof(Accessors.CleanupTask),
            nameof(Accessors.SoftwareDistributionTask),
            nameof(Accessors.TempTask),

            // Security
            nameof(Accessors.NetworkProtection),
            nameof(Accessors.PUAppsDetection),
            nameof(Accessors.DefenderSandbox),
            nameof(Accessors.EventViewerCustomView),
            nameof(Accessors.DNSoverHTTPS),
            nameof(Accessors.LocalSecurityAuthority),
            nameof(Accessors.AdminApprovalMode),
        ];

        /// <summary>
        /// Gets a value indicating whether File Explorer and the Start menu are restarted after a setting is applied.
        /// </summary>
        /// <param name="setting">The name of the setting.</param>
        public static bool RestartsFileExplorer(string setting) => !WithoutFileExplorer.Contains(setting);
    }
}
