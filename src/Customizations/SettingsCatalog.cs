// <copyright file="SettingsCatalog.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using SophiApp.Helpers;
    using static SophiApp.Customizations.SettingDefinition;

    /// <summary>
    /// The settings of the app, in the order they are shown on their pages.
    /// The texts of a setting are the strings "UIModel_Name_Title", "UIModel_Name_Description" and so on.
    /// </summary>
    internal static class SettingsCatalog
    {
        private static readonly SettingDefinition[] Settings =
        [
            // Privacy: Diagnostics
            CheckBox(nameof(Accessors.DiagTrackService), UICategoryTag.Privacy, Accessors.DiagTrackService, Mutators.DiagTrackService, group: "Diagnostics"),
            RadioButtons(nameof(Accessors.DiagnosticDataLevel), UICategoryTag.Privacy, Accessors.DiagnosticDataLevel, Mutators.DiagnosticDataLevel, group: "Diagnostics"),
            CheckBox(nameof(Accessors.ErrorReporting), UICategoryTag.Privacy, Accessors.ErrorReporting, Mutators.ErrorReporting, group: "Diagnostics"),
            RadioButtons(nameof(Accessors.RecommendedTroubleshooting), UICategoryTag.Privacy, Accessors.RecommendedTroubleshooting, Mutators.RecommendedTroubleshooting, group: "Diagnostics"),
            RadioButtons(nameof(Accessors.FeedbackFrequency), UICategoryTag.Privacy, Accessors.FeedbackFrequency, Mutators.FeedbackFrequency, group: "Diagnostics"),
            CheckList(nameof(Accessors.ScheduledTasks), UICategoryTag.Privacy, Accessors.ScheduledTasks, Mutators.ScheduledTasks, group: "Diagnostics"),

            // Privacy: Recommendations
            CheckBox(nameof(Accessors.AdvertisingID), UICategoryTag.Privacy, Accessors.AdvertisingID, Mutators.AdvertisingID, group: "Recommendations"),
            CheckBox(nameof(Accessors.LanguageListAccess), UICategoryTag.Privacy, Accessors.LanguageListAccess, Mutators.LanguageListAccess, group: "Recommendations"),
            CheckBox(nameof(Accessors.TailoredExperiences), UICategoryTag.Privacy, Accessors.TailoredExperiences, Mutators.TailoredExperiences, group: "Recommendations"),
            CheckBox(nameof(Accessors.SettingsSuggestedContent), UICategoryTag.Privacy, Accessors.SettingsSuggestedContent, Mutators.SettingsSuggestedContent, group: "Recommendations"),
            CheckBox(nameof(Accessors.WindowsTips), UICategoryTag.Privacy, Accessors.WindowsTips, Mutators.WindowsTips, group: "Recommendations"),
            CheckBox(nameof(Accessors.AppsSilentInstalling), UICategoryTag.Privacy, Accessors.AppsSilentInstalling, Mutators.AppsSilentInstalling, group: "Recommendations"),
            CheckBox(nameof(Accessors.BingSearch), UICategoryTag.Privacy, Accessors.BingSearch, Mutators.BingSearch, group: "Recommendations"),
            CheckBox(nameof(Accessors.WindowsWelcomeExperience), UICategoryTag.Privacy, Accessors.WindowsWelcomeExperience, Mutators.WindowsWelcomeExperience, group: "Recommendations"),
            CheckBox(nameof(Accessors.WhatsNewInWindows), UICategoryTag.Privacy, Accessors.WhatsNewInWindows, Mutators.WhatsNewInWindows, group: "Recommendations"),
            CheckBox(nameof(Accessors.SigninInfo), UICategoryTag.Privacy, Accessors.SigninInfo, Mutators.SigninInfo, group: "Recommendations"),

            // Personalization: FileExplorer
            CheckBox(nameof(Accessors.ThisPC), UICategoryTag.Personalization, Accessors.ThisPC, Mutators.ThisPC, group: "FileExplorer"),
            CheckBox(nameof(Accessors.CheckBoxes), UICategoryTag.Personalization, Accessors.CheckBoxes, Mutators.CheckBoxes, group: "FileExplorer"),
            CheckBox(nameof(Accessors.HiddenItems), UICategoryTag.Personalization, Accessors.HiddenItems, Mutators.HiddenItems, group: "FileExplorer"),
            CheckBox(nameof(Accessors.FileExtensions), UICategoryTag.Personalization, Accessors.FileExtensions, Mutators.FileExtensions, group: "FileExplorer"),
            CheckBox(nameof(Accessors.MergeConflicts), UICategoryTag.Personalization, Accessors.MergeConflicts, Mutators.MergeConflicts, group: "FileExplorer"),
            RadioButtons(nameof(Accessors.OpenFileExplorerTo), UICategoryTag.Personalization, Accessors.OpenFileExplorerTo, Mutators.OpenFileExplorerTo, group: "FileExplorer"),
            CheckBox(nameof(Accessors.FileExplorerCompactMode), UICategoryTag.Personalization, Accessors.FileExplorerCompactMode, Mutators.FileExplorerCompactMode, group: "FileExplorer"),
            CheckBox(nameof(Accessors.OneDriveFileExplorerAd), UICategoryTag.Personalization, Accessors.OneDriveFileExplorerAd, Mutators.OneDriveFileExplorerAd, group: "FileExplorer"),
            RadioButtons(nameof(Accessors.FileTransferDialog), UICategoryTag.Personalization, Accessors.FileTransferDialog, Mutators.FileTransferDialog, group: "FileExplorer"),
            CheckBox(nameof(Accessors.RecycleBinDeleteConfirmation), UICategoryTag.Personalization, Accessors.RecycleBinDeleteConfirmation, Mutators.RecycleBinDeleteConfirmation, group: "FileExplorer"),
            CheckBox(nameof(Accessors.QuickAccessRecentFiles), UICategoryTag.Personalization, Accessors.QuickAccessRecentFiles, Mutators.QuickAccessRecentFiles, group: "FileExplorer"),
            CheckBox(nameof(Accessors.QuickAccessFrequentFolders), UICategoryTag.Personalization, Accessors.QuickAccessFrequentFolders, Mutators.QuickAccessFrequentFolders, group: "FileExplorer"),
            RadioButtons(nameof(Accessors.FolderGroupBy), UICategoryTag.Personalization, Accessors.FolderGroupBy, Mutators.FolderGroupBy, group: "FileExplorer"),
            CheckBox(nameof(Accessors.NavigationPaneExpand), UICategoryTag.Personalization, Accessors.NavigationPaneExpand, Mutators.NavigationPaneExpand, group: "FileExplorer"),
            CheckBox(nameof(Accessors.ShortcutsSuffix), UICategoryTag.Personalization, Accessors.ShortcutsSuffix, Mutators.ShortcutsSuffix, group: "FileExplorer"),

            // Personalization: Taskbar
            RadioButtons(nameof(Accessors.TaskbarPosition), UICategoryTag.Personalization, Accessors.TaskbarPosition, Mutators.TaskbarPosition, group: "Taskbar"),
            RadioButtons(nameof(Accessors.TaskbarAlignment), UICategoryTag.Personalization, Accessors.TaskbarAlignment, Mutators.TaskbarAlignment, group: "Taskbar"),
            CheckBox(nameof(Accessors.TaskbarWidgets), UICategoryTag.Personalization, Accessors.TaskbarWidgets, Mutators.TaskbarWidgets, group: "Taskbar", windows11Ltsc: false),
            RadioButtons(nameof(Accessors.TaskbarSearch), UICategoryTag.Personalization, Accessors.TaskbarSearch, Mutators.TaskbarSearch, group: "Taskbar"),
            CheckBox(nameof(Accessors.SearchHighlights), UICategoryTag.Personalization, Accessors.SearchHighlights, Mutators.SearchHighlights, group: "Taskbar"),
            CheckBox(nameof(Accessors.TaskViewButton), UICategoryTag.Personalization, Accessors.TaskViewButton, Mutators.TaskViewButton, group: "Taskbar"),
            CheckBox(nameof(Accessors.SecondsInSystemClock), UICategoryTag.Personalization, Accessors.SecondsInSystemClock, Mutators.SecondsInSystemClock, group: "Taskbar"),
            CheckBox(nameof(Accessors.ShowClockInNotificationCenter), UICategoryTag.Personalization, Accessors.ShowClockInNotificationCenter, Mutators.ShowClockInNotificationCenter, group: "Taskbar"),
            RadioButtons(nameof(Accessors.TaskbarCombine), UICategoryTag.Personalization, Accessors.TaskbarCombine, Mutators.TaskbarCombine, group: "Taskbar"),
            CheckBox(nameof(Accessors.TaskbarEndTask), UICategoryTag.Personalization, Accessors.TaskbarEndTask, Mutators.TaskbarEndTask, group: "Taskbar"),

            // Personalization: Start
            CheckBox(nameof(Accessors.RecentlyAddedStartApps), UICategoryTag.Personalization, Accessors.RecentlyAddedStartApps, Mutators.RecentlyAddedStartApps, group: "Start"),
            ActionButton(nameof(Accessors.UnpinAllStartTiles), UICategoryTag.Personalization, Accessors.UnpinAllStartTiles, Mutators.UnpinAllStartTiles, group: "Start"),
            RadioButtons(nameof(Accessors.StartLayout), UICategoryTag.Personalization, Accessors.StartLayout, Mutators.StartLayout, group: "Start", windows11: false),
            RadioButtons(nameof(Accessors.StartAppsView), UICategoryTag.Personalization, Accessors.StartAppsView, Mutators.StartAppsView, group: "Start", windows11Ltsc: false),
            CheckBox(nameof(Accessors.MostUsedStartApps), UICategoryTag.Personalization, Accessors.MostUsedStartApps, Mutators.MostUsedStartApps, group: "Start"),
            CheckBox(nameof(Accessors.StartRecommendedSection), UICategoryTag.Personalization, Accessors.StartRecommendedSection, Mutators.StartRecommendedSection, group: "Start"),
            CheckBox(nameof(Accessors.StartRecommendationsTips), UICategoryTag.Personalization, Accessors.StartRecommendationsTips, Mutators.StartRecommendationsTips, group: "Start"),
            CheckBox(nameof(Accessors.StartAccountNotifications), UICategoryTag.Personalization, Accessors.StartAccountNotifications, Mutators.StartAccountNotifications, group: "Start"),

            // Personalization: Appearance
            RadioButtons(nameof(Accessors.WindowsColorMode), UICategoryTag.Personalization, Accessors.WindowsColorMode, Mutators.WindowsColorMode, group: "Appearance"),
            RadioButtons(nameof(Accessors.AppColorMode), UICategoryTag.Personalization, Accessors.AppColorMode, Mutators.AppColorMode, group: "Appearance"),
            RadioButtons(nameof(Accessors.Cursors), UICategoryTag.Personalization, Accessors.Cursors, Mutators.Cursors, group: "Appearance", optionIcons: true, thirdParty: Accessors.CursorsThirdParty),
            RadioButtons(nameof(Accessors.JPEGWallpapersQuality), UICategoryTag.Personalization, Accessors.JPEGWallpapersQuality, Mutators.JPEGWallpapersQuality, group: "Appearance"),
            CheckBox(nameof(Accessors.FirstLogonAnimation), UICategoryTag.Personalization, Accessors.FirstLogonAnimation, Mutators.FirstLogonAnimation, group: "Appearance"),
            CheckBox(nameof(Accessors.SnapAssist), UICategoryTag.Personalization, Accessors.SnapAssist, Mutators.SnapAssist, group: "Appearance"),
            CheckBox(nameof(Accessors.AeroShaking), UICategoryTag.Personalization, Accessors.AeroShaking, Mutators.AeroShaking, group: "Appearance"),
            RadioButtons(nameof(Accessors.ControlPanelView), UICategoryTag.Personalization, Accessors.ControlPanelView, Mutators.ControlPanelView, group: "Appearance"),

            // System: Components
            Install(nameof(Accessors.OneDrive), UICategoryTag.System, Accessors.OneDrive, Mutators.OneDrive, group: "Components", iconIndex: 2, canUninstall: true, allUsersMutator: Mutators.OneDrive_AllUsers, windows11Ltsc: false),
            Choice(nameof(Accessors.InstallWSL), UICategoryTag.System, Accessors.InstallWSL, Mutators.InstallWSL, group: "Components", iconIndex: 5),
            Install(nameof(Accessors.InstallVisualC_x86), UICategoryTag.System, Accessors.InstallVisualC_x86, Mutators.InstallVisualC_x86, group: "Components", iconIndex: 4),
            Install(nameof(Accessors.InstallVisualC_x64), UICategoryTag.System, Accessors.InstallVisualC_x64, Mutators.InstallVisualC_x64, group: "Components", iconIndex: 4),
            Install(nameof(Accessors.InstallDotNetRuntime_10), UICategoryTag.System, Accessors.InstallDotNetRuntime_10, Mutators.InstallDotNetRuntime_10, group: "Components", iconIndex: 3),
            Install(nameof(Accessors.InstallDotNetRuntime_11), UICategoryTag.System, Accessors.InstallDotNetRuntime_11, Mutators.InstallDotNetRuntime_11, group: "Components", iconIndex: 3),
            CheckBox(nameof(Accessors.WindowsSandbox), UICategoryTag.System, Accessors.WindowsSandbox, Mutators.WindowsSandbox, group: "Components"),

            // System: Updates
            CheckBox(nameof(Accessors.DeliveryOptimization), UICategoryTag.System, Accessors.DeliveryOptimization, Mutators.DeliveryOptimization, group: "Updates"),
            CheckBox(nameof(Accessors.UpdateMicrosoftProducts), UICategoryTag.System, Accessors.UpdateMicrosoftProducts, Mutators.UpdateMicrosoftProducts, group: "Updates"),
            CheckBox(nameof(Accessors.RestartNotification), UICategoryTag.System, Accessors.RestartNotification, Mutators.RestartNotification, group: "Updates"),
            CheckBox(nameof(Accessors.RestartDeviceAfterUpdate), UICategoryTag.System, Accessors.RestartDeviceAfterUpdate, Mutators.RestartDeviceAfterUpdate, group: "Updates"),
            RadioButtons(nameof(Accessors.ActiveHours), UICategoryTag.System, Accessors.ActiveHours, Mutators.ActiveHours, group: "Updates"),
            CheckBox(nameof(Accessors.WindowsLatestUpdate), UICategoryTag.System, Accessors.WindowsLatestUpdate, Mutators.WindowsLatestUpdate, group: "Updates"),

            // System: Power
            CheckBox(nameof(Accessors.Hibernation), UICategoryTag.System, Accessors.Hibernation, Mutators.Hibernation, group: "Power"),
            Choice(nameof(Accessors.PowerPlan), UICategoryTag.System, Accessors.PowerPlan, Mutators.PowerPlan, group: "Power"),
            CheckList(nameof(Accessors.NetworkAdaptersSavePower), UICategoryTag.System, Accessors.NetworkAdaptersSavePower, Mutators.NetworkAdaptersSavePower, group: "Power"),

            // System: Input
            Choice(nameof(Accessors.InputMethod), UICategoryTag.System, Accessors.InputMethod, Mutators.InputMethod, group: "Input"),
            CheckBox(nameof(Accessors.AppsLanguageSwitch), UICategoryTag.System, Accessors.AppsLanguageSwitch, Mutators.AppsLanguageSwitch, group: "Input"),
            CheckBox(nameof(Accessors.PrtScnSnippingTool), UICategoryTag.System, Accessors.PrtScnSnippingTool, Mutators.PrtScnSnippingTool, group: "Input"),
            CheckBox(nameof(Accessors.F1HelpPage), UICategoryTag.System, Accessors.F1HelpPage, Mutators.F1HelpPage, group: "Input"),
            CheckBox(nameof(Accessors.NumLock), UICategoryTag.System, Accessors.NumLock, Mutators.NumLock, group: "Input"),
            CheckBox(nameof(Accessors.CapsLock), UICategoryTag.System, Accessors.CapsLock, Mutators.CapsLock, group: "Input"),
            CheckBox(nameof(Accessors.StickyShift), UICategoryTag.System, Accessors.StickyShift, Mutators.StickyShift, group: "Input"),

            // System: Storage
            CheckBox(nameof(Accessors.StorageSense), UICategoryTag.System, Accessors.StorageSense, Mutators.StorageSense, group: "Storage"),
            CheckBox(nameof(Accessors.ReservedStorage), UICategoryTag.System, Accessors.ReservedStorage, Mutators.ReservedStorage, group: "Storage"),
            CheckBox(nameof(Accessors.ThumbnailCacheRemoval), UICategoryTag.System, Accessors.ThumbnailCacheRemoval, Mutators.ThumbnailCacheRemoval, group: "Storage"),
            CheckBox(nameof(Accessors.Win32LongPathsSupport), UICategoryTag.System, Accessors.Win32LongPathsSupport, Mutators.Win32LongPathsSupport, group: "Storage"),
            UserFolders(nameof(Accessors.UserShellFolderLocation), UICategoryTag.System, Accessors.UserShellFolderLocation, Mutators.UserShellFolderLocation, group: "Storage"),

            // System: Startup
            CheckBox(nameof(Accessors.SaveRestartableApps), UICategoryTag.System, Accessors.SaveRestartableApps, Mutators.SaveRestartableApps, group: "Startup"),
            CheckBox(nameof(Accessors.RestorePreviousFolders), UICategoryTag.System, Accessors.RestorePreviousFolders, Mutators.RestorePreviousFolders, group: "Startup"),

            // System: Other
            Associations(nameof(Accessors.SetAssociation), UICategoryTag.System, Accessors.SetAssociation, Mutators.SetAssociation, Accessors.ImportAssociations, Mutators.ImportAssociations, Mutators.ExportAssociations, group: "Other"),
            RadioButtons(nameof(Accessors.DefaultTerminalApp), UICategoryTag.System, Accessors.DefaultTerminalApp, Mutators.DefaultTerminalApp, group: "Other", optionIcons: true, windows11Ltsc: false),
            CheckBox(nameof(Accessors.Autoplay), UICategoryTag.System, Accessors.Autoplay, Mutators.Autoplay, group: "Other"),
            CheckBox(nameof(Accessors.WindowsManageDefaultPrinter), UICategoryTag.System, Accessors.WindowsManageDefaultPrinter, Mutators.WindowsManageDefaultPrinter, group: "Other"),
            CheckBox(nameof(Accessors.PreventEdgeShortcutCreation), UICategoryTag.System, Accessors.PreventEdgeShortcutCreation, Mutators.PreventEdgeShortcutCreation, group: "Other"),
            CheckBox(nameof(Accessors.BSoDStopError), UICategoryTag.System, Accessors.BSoDStopError, Mutators.BSoDStopError, group: "Other"),
            CheckBox(nameof(Accessors.RegistryBackup), UICategoryTag.System, Accessors.RegistryBackup, Mutators.RegistryBackup, group: "Other"),
            CheckBox(nameof(Accessors.RemoveWindowsAI), UICategoryTag.System, Accessors.RemoveWindowsAI, Mutators.RemoveWindowsAI, group: "Other", notice: Accessors.RemoveWindowsAI_Notice, windows11Ltsc: false),
            ActionButton(nameof(Accessors.ScanRegistryPolicies), UICategoryTag.System, Accessors.ScanRegistryPolicies, Mutators.ScanRegistryPolicies, group: "Other"),

            // Gaming
            CheckBox(nameof(Accessors.XBOXGameBar), UICategoryTag.Gaming, Accessors.XBOXGameBar, Mutators.XBOXGameBar, windows11Ltsc: false),
            CheckBox(nameof(Accessors.XBOXGameTips), UICategoryTag.Gaming, Accessors.XBOXGameTips, Mutators.XBOXGameTips, windows11Ltsc: false),
            CheckBox(nameof(Accessors.GPUScheduling), UICategoryTag.Gaming, Accessors.GPUScheduling, Mutators.GPUScheduling),

            // TaskScheduler
            IconCheckBox(nameof(Accessors.CleanupTask), UICategoryTag.TaskScheduler, Accessors.CleanupTask, Mutators.CleanupTask, iconIndex: 0),
            IconCheckBox(nameof(Accessors.SoftwareDistributionTask), UICategoryTag.TaskScheduler, Accessors.SoftwareDistributionTask, Mutators.SoftwareDistributionTask, iconIndex: 1),
            IconCheckBox(nameof(Accessors.TempTask), UICategoryTag.TaskScheduler, Accessors.TempTask, Mutators.TempTask, iconIndex: 1),

            // Security
            CheckBox(nameof(Accessors.NetworkProtection), UICategoryTag.Security, Accessors.NetworkProtection, Mutators.NetworkProtection),
            CheckBox(nameof(Accessors.PUAppsDetection), UICategoryTag.Security, Accessors.PUAppsDetection, Mutators.PUAppsDetection),
            CheckBox(nameof(Accessors.DefenderSandbox), UICategoryTag.Security, Accessors.DefenderSandbox, Mutators.DefenderSandbox),
            CheckBox(nameof(Accessors.EventViewerCustomView), UICategoryTag.Security, Accessors.EventViewerCustomView, Mutators.EventViewerCustomView),
            CheckBox(nameof(Accessors.AppsSmartScreen), UICategoryTag.Security, Accessors.AppsSmartScreen, Mutators.AppsSmartScreen),
            CheckBox(nameof(Accessors.SaveZoneInformation), UICategoryTag.Security, Accessors.SaveZoneInformation, Mutators.SaveZoneInformation),
            IconRadioButtons(nameof(Accessors.DNSoverHTTPS), UICategoryTag.Security, Accessors.DNSoverHTTPS, Mutators.DNSoverHTTPS, thirdParty: Accessors.DNSoverHTTPSThirdParty),
            CheckBox(nameof(Accessors.LocalSecurityAuthority), UICategoryTag.Security, Accessors.LocalSecurityAuthority, Mutators.LocalSecurityAuthority),
            RadioButtons(nameof(Accessors.AdminApprovalMode), UICategoryTag.Security, Accessors.AdminApprovalMode, Mutators.AdminApprovalMode),

            // ContextMenu
            CheckBox(nameof(Accessors.MSIExtractContext), UICategoryTag.ContextMenu, Accessors.MSIExtractContext, Mutators.MSIExtractContext),
            CheckBox(nameof(Accessors.CABInstallContext), UICategoryTag.ContextMenu, Accessors.CABInstallContext, Mutators.CABInstallContext),
            CheckBox(nameof(Accessors.UseStoreOpenWith), UICategoryTag.ContextMenu, Accessors.UseStoreOpenWith, Mutators.UseStoreOpenWith, windows11Ltsc: false),
            CheckBox(nameof(Accessors.OpenWindowsTerminalAdminContext), UICategoryTag.ContextMenu, Accessors.OpenWindowsTerminalAdminContext, Mutators.OpenWindowsTerminalAdminContext, windows11Ltsc: false),
        ];

        /// <summary>
        /// Gets all settings.
        /// </summary>
        public static IReadOnlyList<SettingDefinition> All => Settings;
    }
}
