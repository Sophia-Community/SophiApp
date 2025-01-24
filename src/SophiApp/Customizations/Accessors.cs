// <copyright file="Accessors.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Models;
    using System.ServiceProcess;
    using System.Text;

    /// <summary>
    /// Get the OS settings.
    /// </summary>
    public static class Accessors
    {
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IHttpService HttpService = App.GetService<IHttpService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IXmlService XmlService = App.GetService<IXmlService>();

        /// <summary>
        /// Get DiagTrack service state.
        /// </summary>
        public static bool DiagTrackService()
        {
            var diagTrackService = new ServiceController("DiagTrack");
            var firewallRule = FirewallService.GetGroupRules("DiagTrack").First();

            if (diagTrackService.StartType == ServiceStartMode.Disabled && firewallRule.Enabled && firewallRule.Action == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
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
            var allowTelemetry = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection")?.GetValue("AllowTelemetry") as int? ?? -1;
            var maxTelemetryAllowed = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")?.GetValue("MaxTelemetryAllowed") as int? ?? -1;
            var showedToastLevel = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")?.GetValue("ShowedToastAtLevel") as int? ?? -1;
            return allowTelemetry.Equals(1) && maxTelemetryAllowed.Equals(1) && showedToastLevel.Equals(1) ? 2 : 1;
        }

        /// <summary>
        /// Get Windows feature "Error reporting" state.
        /// </summary>
        public static bool ErrorReporting()
        {
            var queueTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting") ?? throw new InvalidOperationException($"Failed to find a QueueReporting scheduled task");
            var disabledValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Windows Error Reporting")?.GetValue("Disabled") as int? ?? -1;
            return !(queueTask.State == TaskState.Disabled && disabledValue.Equals(1) && new ServiceController("WerSvc").StartType == ServiceStartMode.Disabled);
        }

        /// <summary>
        /// Get Windows feature "Feedback frequency" state.
        /// </summary>
        public static int FeedbackFrequency()
        {
            var siufPeriod = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Siuf\\Rules")?.GetValue("NumberOfSIUFInPeriod") as int? ?? -1;
            return siufPeriod.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get telemetry scheduled tasks state.
        /// </summary>
        public static bool ScheduledTasks()
        {
            var telemetryTasks = new List<Task?>()
            {
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\ProgramDataUpdater"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Autochk\\Proxy"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Shell\\FamilySafetyMonitor"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Shell\\FamilySafetyRefreshTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\XblGameSave\\XblGameSaveTask"),
            };

            return telemetryTasks.TrueForAll(task => task is null)
                ? throw new InvalidOperationException("No scheduled telemetry tasks were found")
                : telemetryTasks.Exists(task => task?.State == TaskState.Ready);
        }

        /// <summary>
        /// Get Windows feature "Sign-in info" state.
        /// </summary>
        public static bool SigninInfo()
        {
            var userSid = InstrumentationService.GetUserSid(Environment.UserName);
            var userArso = Registry.LocalMachine.OpenSubKey($"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}")?.GetValue("OptOut") ?? -1;
            return !userArso.Equals(1);
        }

        /// <summary>
        /// Get language list access state.
        /// </summary>
        public static bool LanguageListAccess()
        {
            var httpAcceptLanguage = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\User Profile")?.GetValue("HttpAcceptLanguageOptOut") as int? ?? -1;
            return !httpAcceptLanguage.Equals(1);
        }

        /// <summary>
        /// Get the permission for apps to use advertising ID state.
        /// </summary>
        public static bool AdvertisingID()
        {
            var advertisingInfo = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")?.GetValue("Enabled") as int? ?? -1;
            return !advertisingInfo.Equals(0);
        }

        /// <summary>
        /// Get the Windows welcome experiences state.
        /// </summary>
        public static bool WindowsWelcomeExperience()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-310093Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Get Windows tips state.
        /// </summary>
        public static bool WindowsTips()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-338389Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Get the suggested content in the Settings app state.
        /// </summary>
        public static bool SettingsSuggestedContent()
        {
            var subscribedContent = new List<int>()
            {
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-338393Enabled") as int? ?? -1,
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-353694Enabled") as int? ?? -1,
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-353696Enabled") as int? ?? -1,
            }
            .TrueForAll(subscribed => subscribed.Equals(0));
            return !subscribedContent;
        }

        /// <summary>
        /// Get the automatic installing suggested apps state.
        /// </summary>
        public static bool AppsSilentInstalling()
        {
            var appsIsSilentInstalled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SilentInstalledAppsEnabled") as int? ?? -1;
            return !appsIsSilentInstalled.Equals(0);
        }

        /// <summary>
        /// Get the Windows feature "Whats New" state.
        /// </summary>
        public static bool WhatsNewInWindows()
        {
            var scoobeSettingIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")?.GetValue("ScoobeSystemSettingEnabled") as int? ?? -1;
            return !scoobeSettingIsEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows feature "Tailored experiences" state.
        /// </summary>
        public static bool TailoredExperiences()
        {
            var tailoredExperiencesIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy")?.GetValue("TailoredExperiencesWithDiagnosticDataEnabled") as int? ?? -1;
            return !tailoredExperiencesIsEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows feature "Bing search" state.
        /// </summary>
        public static bool BingSearch()
        {
            var searchBoxIsDisabled = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\Explorer")?.GetValue("DisableSearchBoxSuggestions") as int? ?? -1;
            return !searchBoxIsDisabled.Equals(1);
        }

        /// <summary>
        /// Get Start menu recommendations state.
        /// </summary>
        public static bool StartRecommendationsTips()
        {
            var irisPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var irisRecommendations = Registry.CurrentUser.OpenSubKey(irisPath)?.GetValue("Start_IrisRecommendations") as int? ?? -1;
            return !irisRecommendations.Equals(0);
        }

        /// <summary>
        /// Get Start Menu notifications state.
        /// </summary>
        public static bool StartAccountNotifications()
        {
            var notificationsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var accountNotifications = Registry.CurrentUser.OpenSubKey(notificationsPath)?.GetValue("Start_AccountNotifications") as int? ?? -1;
            return !accountNotifications.Equals(0);
        }

        /// <summary>
        /// Get the "This PC" icon on Desktop state.
        /// </summary>
        public static bool ThisPC()
        {
            var panelGuid = "{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            var panelPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel";
            var panelValue = Registry.CurrentUser.OpenSubKey(panelPath)?.GetValue(panelGuid) as int? ?? -1;
            return panelValue.Equals(0);
        }

        /// <summary>
        /// Get item check boxes state.
        /// </summary>
        public static bool CheckBoxes()
        {
            var checkPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var checkValue = Registry.CurrentUser.OpenSubKey(checkPath)?.GetValue("AutoCheckSelect") as int? ?? -1;
            return checkValue.Equals(1);
        }

        /// <summary>
        /// Get hidden files, folders, and drives state.
        /// </summary>
        public static bool HiddenItems()
        {
            var itemsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var itemsValue = Registry.CurrentUser.OpenSubKey(itemsPath)?.GetValue("Hidden") as int? ?? -1;
            return itemsValue.Equals(1);
        }

        /// <summary>
        /// Get file name extensions visibility state.
        /// </summary>
        public static bool FileExtensions()
        {
            var extensionsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var extensionsValue = Registry.CurrentUser.OpenSubKey(extensionsPath)?.GetValue("HideFileExt") as int? ?? -1;
            return extensionsValue.Equals(0);
        }

        /// <summary>
        /// Get folder merge conflicts state.
        /// </summary>
        public static bool MergeConflicts()
        {
            var mergePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var mergeValue = Registry.CurrentUser.OpenSubKey(mergePath)?.GetValue("HideMergeConflicts") as int? ?? -1;
            return mergeValue.Equals(0);
        }

        /// <summary>
        /// Get how to open File Explorer.
        /// </summary>
        public static int OpenFileExplorerTo()
        {
            var filePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var fileValue = Registry.CurrentUser.OpenSubKey(filePath)?.GetValue("LaunchTo") as int? ?? -1;
            return fileValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get File Explorer ribbon state.
        /// </summary>
        public static int FileExplorerRibbon()
        {
            var ribbonPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Ribbon";
            var ribbonValue = Registry.CurrentUser.OpenSubKey(ribbonPath)?.GetValue("MinimizedStateTabletModeOff") as int? ?? -1;
            return ribbonValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get File Explorer compact mode state.
        /// </summary>
        public static bool FileExplorerCompactMode()
        {
            var compactModePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var compactModeValue = Registry.CurrentUser.OpenSubKey(compactModePath)?.GetValue("UseCompactMode") as int? ?? -1;
            return !compactModeValue.Equals(0);
        }

        /// <summary>
        /// Get File Explorer provider notification visibility state.
        /// </summary>
        public static bool OneDriveFileExplorerAd()
        {
            var notificationsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var notificationsValue = Registry.CurrentUser.OpenSubKey(notificationsPath)?.GetValue("ShowSyncProviderNotifications") as int? ?? -1;
            return !notificationsValue.Equals(0);
        }

        /// <summary>
        /// Get snap a window state.
        /// </summary>
        public static bool SnapAssist()
        {
            var snapPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var snapValue = Registry.CurrentUser.OpenSubKey(snapPath)?.GetValue("SnapAssist") as int? ?? -1;
            return !snapValue.Equals(0);
        }

        /// <summary>
        /// Get file transfer dialog box mode.
        /// </summary>
        public static int FileTransferDialog()
        {
            var modePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\OperationStatusManager";
            var modeValue = Registry.CurrentUser.OpenSubKey(modePath)?.GetValue("EnthusiastMode") as int? ?? -1;
            return modeValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get recycle bin confirmation dialog state.
        /// </summary>
        public static bool RecycleBinDeleteConfirmation()
        {
            var shellPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var shellValue = Registry.CurrentUser.OpenSubKey(shellPath)?.GetValue("ShellState") as byte[] ?? new byte[5];
            return shellValue[4].Equals(51);
        }

        /// <summary>
        /// Get recently used Quick access files state.
        /// </summary>
        public static bool QuickAccessRecentFiles()
        {
            var recentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var recentValue = Registry.CurrentUser.OpenSubKey(recentPath)?.GetValue("ShowRecent") as int? ?? -1;
            return !recentValue.Equals(0);
        }

        /// <summary>
        /// Get frequently used Quick access folders state.
        /// </summary>
        public static bool QuickAccessFrequentFolders()
        {
            var frequentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var frequentValue = Registry.CurrentUser.OpenSubKey(frequentPath)?.GetValue("ShowFrequent") as int? ?? -1;
            return !frequentValue.Equals(0);
        }

        /// <summary>
        /// Get taskbar alignment state.
        /// </summary>
        public static int TaskbarAlignment()
        {
            var taskbarPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var taskbarValue = Registry.CurrentUser.OpenSubKey(taskbarPath)?.GetValue("TaskbarAl") as int? ?? -1;
            return taskbarValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get taskbar widgets icon state.
        /// </summary>
        public static bool TaskbarWidgets()
        {
            var appxWebExperience = "MicrosoftWindows.Client.WebExperience";

            if (AppxPackagesService.PackageExist(appxWebExperience))
            {
                var taskbarPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
                var taskbarValue = Registry.CurrentUser.OpenSubKey(taskbarPath)?.GetValue("TaskbarDa") as int? ?? -1;
                return !taskbarValue.Equals(0);
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appxWebExperience}");
        }

        /// <summary>
        /// Get Search on the taskbar state.
        /// </summary>
        public static int TaskbarSearchWindows10()
        {
            var smallIconsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var searchModePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Search";
            var smallIconsValue = Registry.CurrentUser.OpenSubKey(smallIconsPath)?.GetValue("TaskbarSmallIcons") as int? ?? -1;
            var searchModeValue = Registry.CurrentUser.OpenSubKey(searchModePath)?.GetValue("SearchboxTaskbarMode") as int? ?? -1;

            if (smallIconsValue.Equals(1))
            {
                throw new InvalidOperationException($"The small taskbar icons mode is enabled");
            }

            return searchModeValue + 1;
        }

        /// <summary>
        /// Get Search on the taskbar state.
        /// </summary>
        public static int TaskbarSearchWindows11()
        {
            var smallIconsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var searchModePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Search";
            var smallIconsValue = Registry.CurrentUser.OpenSubKey(smallIconsPath)?.GetValue("TaskbarSmallIcons") as int? ?? -1;
            var searchModeValue = Registry.CurrentUser.OpenSubKey(searchModePath)?.GetValue("SearchboxTaskbarMode") as int? ?? -1;

            if (smallIconsValue.Equals(1))
            {
                throw new InvalidOperationException($"The small taskbar icons mode is enabled");
            }

            return searchModeValue switch
            {
                0 => 1,
                1 => 2,
                2 => 4,
                _ => 3,
            };
        }

        /// <summary>
        /// Get search highlights state.
        /// </summary>
        public static bool SearchHighlightsWindows10()
        {
            var contentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Feeds\\DSB";
            var dynamicPath = "Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings";
            var contentValue = Registry.CurrentUser.OpenSubKey(contentPath)?.GetValue("ShowDynamicContent") as int? ?? -1;
            var dynamicValue = Registry.CurrentUser.OpenSubKey(dynamicPath)?.GetValue("IsDynamicSearchBoxEnabled") as int? ?? -1;
            return !(contentValue.Equals(0) && dynamicValue.Equals(0));
        }

        /// <summary>
        /// Get search highlights state.
        /// </summary>
        public static bool SearchHighlightsWindows11()
        {
            var searchPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Search";
            var suggestionPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var searchEnabled = Registry.CurrentUser.OpenSubKey(searchPath)?.GetValue("BingSearchEnabled") as int? ?? -1;
            var searchSuggestions = Registry.CurrentUser.OpenSubKey(suggestionPath)?.GetValue("DisableSearchBoxSuggestions") as int? ?? -1;

            if (searchEnabled.Equals(1) || searchSuggestions.Equals(1))
            {
                throw new InvalidOperationException("The value of the BingSearchEnabled or DisableSearchBoxSuggestions parameters is 1");
            }

            var settingsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings";
            var dynamicSearch = Registry.CurrentUser.OpenSubKey(settingsPath)?.GetValue("IsDynamicSearchBoxEnabled") as int? ?? -1;
            return dynamicSearch.Equals(0);
        }

        /// <summary>
        /// Get Cortana button taskbar state.
        /// </summary>
        public static bool CortanaButton()
        {
            var appxCortana = "Microsoft.549981C3F5F10";

            if (AppxPackagesService.PackageExist(appxCortana))
            {
                var buttonPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
                var buttonValue = Registry.CurrentUser.OpenSubKey(buttonPath)?.GetValue("ShowCortanaButton") as int? ?? -1;
                return !buttonValue.Equals(0);
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appxCortana}");
        }

        /// <summary>
        /// Get taskbar task view button state.
        /// </summary>
        public static bool TaskViewButton()
        {
            var buttonPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var buttonValue = Registry.CurrentUser.OpenSubKey(buttonPath)?.GetValue("ShowTaskViewButton") as int? ?? -1;
            return !buttonValue.Equals(0);
        }

        /// <summary>
        /// Get News and Interests state.
        /// </summary>
        public static bool NewsInterests()
        {
            var feedsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Feeds";
            var feedsValue = Registry.CurrentUser.OpenSubKey(feedsPath)?.GetValue("ShellFeedsTaskbarViewMode") as int? ?? -1;
            return !feedsValue.Equals(2);
        }

        /// <summary>
        /// Get taskbar people icon state.
        /// </summary>
        public static bool PeopleTaskbar()
        {
            var peoplePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People";
            var peopleValue = Registry.CurrentUser.OpenSubKey(peoplePath)?.GetValue("PeopleBand") as int? ?? -1;
            return !peopleValue.Equals(0);
        }

        /// <summary>
        /// Get Meet Now icon state.
        /// </summary>
        public static bool MeetNow()
        {
            var meetPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StuckRects3";
            var meetValue = Registry.CurrentUser.OpenSubKey(meetPath)?.GetValue("Settings") as byte[] ?? new byte[10];
            return !meetValue[9].Equals(128);
        }

        /// <summary>
        /// Get Windows Ink Workspace button state.
        /// </summary>
        public static bool WindowsInkWorkspace()
        {
            var workspacePath = "Software\\Microsoft\\Windows\\CurrentVersion\\PenWorkspace";
            var workspaceValue = Registry.CurrentUser.OpenSubKey(workspacePath)?.GetValue("PenWorkspaceButtonDesiredVisibility") as int? ?? -1;
            return !workspaceValue.Equals(0);
        }

        /// <summary>
        /// Get notification area icons state.
        /// </summary>
        public static bool NotificationAreaIcons()
        {
            var trayPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var trayValue = Registry.CurrentUser.OpenSubKey(trayPath)?.GetValue("EnableAutoTray") as int? ?? -1;
            return !trayValue.Equals(0);
        }

        /// <summary>
        /// Get seconds on the taskbar clock state.
        /// </summary>
        public static bool SecondsInSystemClock()
        {
            var clockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var clockValue = Registry.CurrentUser.OpenSubKey(clockPath)?.GetValue("ShowSecondsInSystemClock") as int? ?? -1;
            return clockValue.Equals(1);
        }

        /// <summary>
        /// Get taskbar combine state.
        /// </summary>
        public static int TaskbarCombine()
        {
            var levelPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var levelValue = Registry.CurrentUser.OpenSubKey(levelPath)?.GetValue("TaskbarGlomLevel") as int? ?? -1;
            return levelValue + 1;
        }

        /// <summary>
        /// Get end task in taskbar by click state.
        /// </summary>
        public static bool TaskbarEndTask()
        {
            var taskPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings";
            var taskValue = Registry.CurrentUser.OpenSubKey(taskPath)?.GetValue("TaskbarEndTask") as int? ?? -1;
            return taskValue.Equals(1);
        }

        /// <summary>
        /// Get Control Panel icons view state.
        /// </summary>
        public static int ControlPanelView()
        {
            var panelPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel";
            var viewValue = Registry.CurrentUser.OpenSubKey(panelPath)?.GetValue("AllItemsIconView") as int? ?? -1;
            var pageValue = Registry.CurrentUser.OpenSubKey(panelPath)?.GetValue("StartupPage") as int? ?? -1;

            if (viewValue.Equals(0) && pageValue.Equals(0))
            {
                return 1;
            }

            return viewValue.Equals(0) && pageValue.Equals(1) ? 2 : 3;
        }

        /// <summary>
        /// Get Windows color mode state.
        /// </summary>
        public static int WindowsColorMode()
        {
            var themePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            var themeValue = Registry.CurrentUser.OpenSubKey(themePath)?.GetValue("SystemUsesLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get apps color mode state.
        /// </summary>
        public static int AppColorMode()
        {
            var themePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Feeds";
            var themeValue = Registry.CurrentUser.OpenSubKey(themePath)?.GetValue("AppsUseLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get "New App Installed" indicator state.
        /// </summary>
        public static bool NewAppInstalledNotification()
        {
            var alertPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var alertValue = Registry.LocalMachine.OpenSubKey(alertPath)?.GetValue("NoNewAppAlert") as int? ?? -1;
            return !alertValue.Equals(1);
        }

        /// <summary>
        /// Get first sign-in animation state.
        /// </summary>
        public static bool FirstLogonAnimation()
        {
            var logonPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
            var logonValue = Registry.LocalMachine.OpenSubKey(logonPath)?.GetValue("EnableFirstLogonAnimation") as int? ?? -1;
            return !logonValue.Equals(0);
        }

        /// <summary>
        /// Get JPEG wallpapers quality state.
        /// </summary>
        public static int JPEGWallpapersQuality()
        {
            var qualityPath = "Control Panel\\Desktop";
            var qualityValue = Registry.CurrentUser.OpenSubKey(qualityPath)?.GetValue("JPEGImportQuality") as int? ?? -1;
            return qualityValue.Equals(100) ? 1 : 2;
        }

        /// <summary>
        /// Get "- Shortcut" suffix state.
        /// </summary>
        public static bool ShortcutsSuffix()
        {
            var shortcutsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates";
            var shortcutsValue = Registry.CurrentUser.OpenSubKey(shortcutsPath)?.GetValue("ShortcutNameTemplate") as string ?? string.Empty;
            return !shortcutsValue.Equals("%s.lnk");
        }

        /// <summary>
        /// Get Print screen button state.
        /// </summary>
        public static bool PrtScnSnippingTool()
        {
            var snippingPath = "Control Panel\\Keyboard";
            var snippingValue = Registry.CurrentUser.OpenSubKey(snippingPath)?.GetValue("PrintScreenKeyForSnippingEnabled") as int? ?? -1;
            return snippingValue.Equals(1);
        }

        /// <summary>
        /// Get input method for app window state.
        /// </summary>
        public static bool AppsLanguageSwitch()
        {
            return PowerShellService.Invoke<bool>("$((Get-WinLanguageBarOption).IsLegacySwitchingMode)");
        }

        /// <summary>
        /// Get Aero Shake state.
        /// </summary>
        public static bool AeroShaking()
        {
            var shakingPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var shakingValue = Registry.CurrentUser.OpenSubKey(shakingPath)?.GetValue("DisallowShaking") as int? ?? -1;
            return shakingValue.Equals(0);
        }

        /// <summary>
        /// Get "Windows 11 Cursors Concept" from Jepri Creations state.
        /// </summary>
        public static int Cursors()
        {
            HttpService.ThrowIfOffline("https://github.com");
            var cursorsScheme = Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors")?.GetValue(string.Empty) as string ?? string.Empty;

            if (cursorsScheme.Equals("W11 Cursor Dark Free by Jepri Creations"))
            {
                return 1;
            }

            if (cursorsScheme.Equals("W11 Cursor Light Free by Jepri Creations"))
            {
                return 2;
            }

            return 3;
        }

        /// <summary>
        /// Get files and folders grouping state.
        /// </summary>
        public static int FolderGroupBy()
        {
            var groupByPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderTypes\\{885a186e-a440-4ada-812b-db871b942259}\\TopViews\\{00000000-0000-0000-0000-000000000000}";
            var groupByValue = Registry.CurrentUser.OpenSubKey(groupByPath)?.GetValue("GroupBy") as string ?? string.Empty;
            return groupByValue.Equals("System.Null") ? 1 : 2;
        }

        /// <summary>
        /// Get navigation pane expand state.
        /// </summary>
        public static bool NavigationPaneExpand()
        {
            var panePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var paneValue = Registry.CurrentUser.OpenSubKey(panePath)?.GetValue("NavPaneExpandToCurrentFolder") as int? ?? -1;
            return !paneValue.Equals(0);
        }

        /// <summary>
        /// Get Start menu recently added apps state.
        /// </summary>
        public static bool RecentlyAddedApps()
        {
            var appsPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var appsValue = Registry.LocalMachine.OpenSubKey(appsPath)?.GetValue("HideRecentlyAddedApps") as int? ?? -1;
            return !appsValue.Equals(1);
        }

        /// <summary>
        /// Get Start menu app suggestions state.
        /// </summary>
        public static bool AppSuggestions()
        {
            var contentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager";
            var contentValue = Registry.CurrentUser.OpenSubKey(contentPath)?.GetValue("SubscribedContent-338388Enabled") as int? ?? -1;
            return !contentValue.Equals(0);
        }

        /// <summary>
        /// Get Start menu layout state.
        /// </summary>
        public static int StartLayout()
        {
            var layoutPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var layoutValue = Registry.CurrentUser.OpenSubKey(layoutPath)?.GetValue("Start_Layout") as int? ?? -1;
            return layoutValue.Equals(-1) ? 1 : layoutValue + 1;
        }

        /// <summary>
        /// Get recommended section state.
        /// </summary>
        public static bool HideRecommendedSection()
        {
            var os = CommonDataService.OsProperties;

            if ((os.Edition.Equals("Enterprise") || os.Edition.Equals("Education")) && !os.Caption.Contains("Windows 11 IoT Enterprise"))
            {
                var sectionPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
                var sectionValue = Registry.CurrentUser.OpenSubKey(sectionPath)?.GetValue("HideRecommendedSection") as int? ?? -1;
                return !sectionValue.Equals(1);
            }

            throw new InvalidOperationException("Only Enterprise and Education edition are supported. Version Windows 11 IoT Enterprise is not supported");
        }

        /// <summary>
        /// Gets HEVC state.
        /// </summary>
        public static bool HEVC()
        {
            var video = "Microsoft.HEVCVideoExtension";
            var photos = "Microsoft.Windows.Photos";
            var appxVideoIsExist = AppxPackagesService.PackageExist(video);
            var appxPhotosIsExist = AppxPackagesService.PackageExist(photos);

            if (appxVideoIsExist && appxPhotosIsExist)
            {
                return true;
            }
            else if (!appxPhotosIsExist)
            {
                throw new InvalidOperationException($"Necessary appx package are not installed: {photos}");
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets Cortana auto start state.
        /// </summary>
        public static bool CortanaAutostart()
        {
            if (AppxPackagesService.PackageExist("Microsoft.549981C3F5F10"))
            {
                var pathCortana = "Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.549981C3F5F10_8wekyb3d8bbwe\\CortanaStartupId";
                var stateCortana = Registry.ClassesRoot.OpenSubKey(pathCortana)?.GetValue("State") as int? ?? -1;
                return stateCortana != 1;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: Cortana");
        }

        /// <summary>
        /// Gets Xbox game bar state.
        /// </summary>
        public static bool XboxGameBar()
        {
            var appCaptureIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR")?.GetValue("AppCaptureEnabled") as int? ?? -1;
            var dvrIsEnabled = Registry.CurrentUser.OpenSubKey("System\\GameConfigStore")?.GetValue("GameDVR_Enabled") as int? ?? -1;

            if (appCaptureIsEnabled == 0 && dvrIsEnabled == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets Xbox game tips state.
        /// </summary>
        public static bool XboxGameTips()
        {
            var appGaming = "Microsoft.GamingApp";

            if (AppxPackagesService.PackageExist(appGaming))
            {
                var startupPanelIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\GameBar")?.GetValue("ShowStartupPanel") as int? ?? -1;
                return startupPanelIsEnabled == 1;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appGaming}");
        }

        /// <summary>
        /// Get GPU scheduling state.
        /// </summary>
        public static bool GPUScheduling()
        {
            const int WDDMMinimalVersion = 2700;
            var featureUsagePath = "System\\CurrentControlSet\\Control\\GraphicsDrivers\\FeatureSetUsage";
            var isExternalDACType = InstrumentationService.IsExternalDACType();
            var isVirtualMachine = InstrumentationService.IsVirtualMachine();
            var wddmVersion = Registry.LocalMachine.OpenSubKey(featureUsagePath)?.GetValue("WddmVersion_Min") as int? ?? -1;

            if (isExternalDACType && !isVirtualMachine && wddmVersion >= WDDMMinimalVersion)
            {
                var graphicsDriversPath = "System\\CurrentControlSet\\Control\\GraphicsDrivers";
                var hwSchMode = Registry.LocalMachine.OpenSubKey(graphicsDriversPath)?.GetValue("HwSchMode") as int? ?? -1;
                return hwSchMode == 2;
            }

            throw new InvalidOperationException($"DAC type is external: {isExternalDACType}, is VM: {isVirtualMachine}, WDDM version (minimal {WDDMMinimalVersion}): {wddmVersion}");
        }

        /// <summary>
        /// Get scheduled task "Windows Cleanup" state.
        /// </summary>
        public static bool CleanupTask()
        {
            if (CommonDataService.IsWindows11 && !OsService.VBSIsInstalled())
            {
                throw new InvalidOperationException("The VBSCRIPT component is not installed");
            }

            var cleanupTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup");

            if (cleanupTask is not null && cleanupTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The Windows Cleanup scheduled task was already created as {cleanupTask.Definition.Principal.UserId}");
            }

            return cleanupTask is not null && cleanupTask.State != TaskState.Disabled && cleanupTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Get scheduled task "SoftwareDistribution" state.
        /// </summary>
        public static bool SoftwareDistributionTask()
        {
            if (CommonDataService.IsWindows11 && !OsService.VBSIsInstalled())
            {
                throw new InvalidOperationException("The VBSCRIPT component is not installed");
            }

            var distributionTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\SoftwareDistribution");

            if (distributionTask is not null && distributionTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The SoftwareDistribution scheduled task was already created as {distributionTask.Definition.Principal.UserId}");
            }

            return distributionTask is not null && distributionTask.State != TaskState.Disabled && distributionTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Get scheduled task "Temp" state.
        /// </summary>
        public static bool TempTask()
        {
            if (CommonDataService.IsWindows11 && !OsService.VBSIsInstalled())
            {
                throw new InvalidOperationException("The VBSCRIPT component is not installed");
            }

            var tempTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\TempTask");

            if (tempTask is not null && tempTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The Temp scheduled task was already created as {tempTask.Definition.Principal.UserId}");
            }

            return tempTask is not null && tempTask.State != TaskState.Disabled && tempTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Get Windows network protection state.
        /// </summary>
        public static bool NetworkProtection()
        {
            if (InstrumentationService.GetAntispywareEnabled())
            {
                var networkProtectionPath = "Software\\Microsoft\\Windows Defender\\Windows Defender Exploit Guard\\Network Protection";
                var networkProtection = Registry.LocalMachine.OpenSubKey(networkProtectionPath)?.GetValue("EnableNetworkProtection") as int? ?? -1;
                return networkProtection.Equals(1);
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows PUApps detection state.
        /// </summary>
        public static bool PUAppsDetection()
        {
            if (InstrumentationService.GetAntispywareEnabled())
            {
                var puaProtection = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows Defender")?.GetValue("PUAProtection") as int? ?? -1;
                return puaProtection.Equals(1);
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Microsoft Defender sandbox state.
        /// </summary>
        public static bool DefenderSandbox()
        {
            if (InstrumentationService.GetAntispywareEnabled())
            {
                return ProcessService.ProcessExist("MsMpEngCP");
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows event viewer custom view state.
        /// </summary>
        public static bool EventViewerCustomView()
        {
            var processAuditPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit";
            var processXmlPath = $"{Environment.GetEnvironmentVariable("ALLUSERSPROFILE")}\\Microsoft\\Event Viewer\\Views\\ProcessCreation.xml";
            var auditPolicyScript = @"$OutputEncoding = [System.Console]::OutputEncoding = [System.Console]::InputEncoding = [System.Text.Encoding]::UTF8
$Enabled = auditpol /get /Subcategory:'{0CCE922B-69AE-11D9-BED3-505054503030}' /r | ConvertFrom-Csv | Select-Object -ExpandProperty 'Inclusion Setting'
if ($Enabled -eq 'Success and Failure')
{
    $true
}
else
{
    $false
}";

            var auditPolicyIsEnabled = PowerShellService.Invoke<bool>(auditPolicyScript);
            var processAuditIsEnabled = Registry.LocalMachine.OpenSubKey(processAuditPath)?.GetValue("ProcessCreationIncludeCmdLine_Enabled") as int? ?? -1;
            var xmlAuditIsEnabled = XmlService.TryLoad(processXmlPath)?.SelectSingleNode("//Select[@Path=\"Security\"]")?.InnerText ?? string.Empty;
            return auditPolicyIsEnabled && processAuditIsEnabled.Equals(1) && xmlAuditIsEnabled.Equals("*[System[(EventID=4688)]]");
        }

        /// <summary>
        /// Get Windows PowerShell modules logging state.
        /// </summary>
        public static bool PowerShellModulesLogging()
        {
            var moduleLoggingPath = "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging";
            var moduleNamePath = $"{moduleLoggingPath}\\ModuleNames";

            var moduleLoggingIsEnabled = Registry.LocalMachine.OpenSubKey(moduleLoggingPath)?.GetValue("EnableModuleLogging") as int? ?? -1;
            var moduleNamesIsAny = Registry.LocalMachine.OpenSubKey(moduleNamePath)?.GetValue("*") as string ?? string.Empty;
            return moduleLoggingIsEnabled.Equals(1) && moduleNamesIsAny.Equals("*");
        }

        /// <summary>
        /// Get Windows PowerShell scripts logging state.
        /// </summary>
        public static bool PowerShellScriptsLogging()
        {
            var scriptLoggingPath = "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging";
            var scriptLogging = Registry.LocalMachine.OpenSubKey(scriptLoggingPath)?.GetValue("EnableScriptBlockLogging") as int? ?? -1;
            return scriptLogging.Equals(1);
        }

        /// <summary>
        /// Get Windows SmartScreen state.
        /// </summary>
        public static bool AppsSmartScreen()
        {
            if (InstrumentationService.GetAntispywareEnabled())
            {
                var smartScreenPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
                var smartScreenIsEnabled = Registry.LocalMachine.OpenSubKey(smartScreenPath)?.GetValue("SmartScreenEnabled") as string ?? string.Empty;
                return !smartScreenIsEnabled.Equals("Off");
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows save zone state.
        /// </summary>
        public static bool SaveZoneInformation()
        {
            var saveZonePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments";
            var saveZoneInformation = Registry.CurrentUser.OpenSubKey(saveZonePath)?.GetValue("SaveZoneInformation") as int? ?? -1;
            return saveZoneInformation.Equals(1);
        }

        /// <summary>
        /// Get Windows script host state.
        /// </summary>
        public static bool WindowsScriptHost()
        {
            var blockingTasks = new[] { "SoftwareDistribution", "Temp", "Windows Cleanup", "Windows Cleanup Notification" };
            var blockingTasksExist = ScheduledTaskService.FindTaskOrDefault(blockingTasks).Any(task => task?.State == TaskState.Ready);
            var scriptHostPath = "Software\\Microsoft\\Windows Script Host\\Settings";
            var scriptHostIsEnabled = Registry.CurrentUser.OpenSubKey(scriptHostPath)?.GetValue("Enabled") as int? ?? -1;
            return blockingTasksExist ? throw new InvalidOperationException("One of the blocking tasks is in Ready state.") : !scriptHostIsEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows Sandbox state.
        /// </summary>
        public static bool WindowsSandbox()
        {
            bool WindowsSandboxIsEnabled()
            {
                var sandboxScript = "Get-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -Online";
                var sandboxState = PowerShellService.Invoke(sandboxScript).FirstOrDefault();
                return !sandboxState?.Properties["State"]?.Value.Equals("Disabled") ?? throw new InvalidOperationException("Windows Sandbox state undefined");
            }

            if (CommonDataService.OsProperties.Edition.Equals("Professional") || CommonDataService.OsProperties.Edition.Equals("Enterprise"))
            {
                var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationFirmwareIsEnabled() ?? throw new InvalidOperationException("This CPU does not support virtualization");
                var hypervisorPresent = InstrumentationService.HypervisorIsPresent() ?? throw new InvalidOperationException("Enable virtualization in UEFI");

                if (virtualizationIsEnabled)
                {
                    return WindowsSandboxIsEnabled();
                }
                else if (hypervisorPresent)
                {
                    return WindowsSandboxIsEnabled();
                }

                throw new InvalidOperationException("This PC does not support Windows Sandbox feature");
            }

            throw new InvalidOperationException("Unsupported Windows edition");
        }

        /// <summary>
        /// Get Local Security Authority state.
        /// </summary>
        public static bool LocalSecurityAuthority()
        {
            var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationFirmwareIsEnabled() ?? throw new InvalidOperationException("This CPU does not support virtualization");
            var hypervisorPresent = InstrumentationService.HypervisorIsPresent() ?? throw new InvalidOperationException("Enable virtualization in UEFI");
            var runAsPPL = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa")?.GetValue("RunAsPPL") ?? -1;
            var runAsPPLBoot = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa")?.GetValue("RunAsPPLBoot") ?? -1;
            var runAsPPLPolicy = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\System")?.GetValue("RunAsPPL") ?? -1;

            if (virtualizationIsEnabled)
            {
                return (runAsPPL.Equals(2) && runAsPPLBoot.Equals(2)) || runAsPPLPolicy.Equals(2);
            }
            else if (hypervisorPresent)
            {
                return (runAsPPL.Equals(2) && runAsPPLBoot.Equals(2)) || runAsPPLPolicy.Equals(2);
            }

            throw new InvalidOperationException("This PC does not support Local Security Authority feature");
        }

        /// <summary>
        /// Get "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        public static bool MSIExtractContext()
        {
            var muiVerb = Registry.ClassesRoot.OpenSubKey("Msi.Package\\shell\\Extract")?.GetValue("MUIVerb") as string;
            return muiVerb?.Equals("@shell32.dll,-37514") ?? false;
        }

        /// <summary>
        /// Get "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        public static bool CABInstallContext()
        {
            var muiVerb = Registry.ClassesRoot.OpenSubKey("CABFolder\\Shell\\runas")?.GetValue("MUIVerb") as string;
            return muiVerb?.Equals("@shell32.dll,-10210") ?? false;
        }

        /// <summary>
        /// Get "Cast to Device" item in the media files and folders context menu state.
        /// </summary>
        public static bool CastToDeviceContext()
        {
            var castToDevicePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var castToDeviceGuid = "{7AD84985-87B4-4a16-BE58-8B72A5B390F7}";

            var userCastToDevice = Registry.CurrentUser.OpenSubKey(castToDevicePath)?.GetValue(castToDeviceGuid) as string;
            var machineCastToDevice = Registry.LocalMachine.OpenSubKey(castToDevicePath)?.GetValue(castToDeviceGuid) as string;
            return userCastToDevice is null && machineCastToDevice is null;
        }

        /// <summary>
        /// Get "Share" context menu item state.
        /// </summary>
        public static bool ShareContext()
        {
            var shareContextPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var shareContextGuid = "{E2BF9676-5F8F-435C-97EB-11607A5BEDF7}";

            var userShareContext = Registry.CurrentUser.OpenSubKey(shareContextPath)?.GetValue(shareContextGuid) as string;
            var machineShareContext = Registry.LocalMachine.OpenSubKey(shareContextPath)?.GetValue(shareContextGuid) as string;
            return userShareContext is null && machineShareContext is null;
        }

        /// <summary>
        /// Get "Edit With Clipchamp" item in the media files context menu state.
        /// </summary>
        public static bool EditWithClipchampContext()
        {
            var clipChampAppx = "Clipchamp.Clipchamp";
            var clipChampPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var clipChampGuid = "{8AB635F8-9A67-4698-AB99-784AD929F3B4}";

            if (AppxPackagesService.PackageExist(clipChampAppx))
            {
                var userClipchamp = Registry.CurrentUser.OpenSubKey(clipChampPath)?.GetValue(clipChampGuid);
                var machineClipchamp = Registry.LocalMachine.OpenSubKey(clipChampPath)?.GetValue(clipChampGuid);
                return userClipchamp is null && machineClipchamp is null;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {clipChampAppx}");
        }

        /// <summary>
        /// Get "Edit With Photos" item in the media files context menu state.
        /// </summary>
        public static bool EditWithPhotosContext()
        {
            var photosAppx = "EditWithPhotosContext";
            var clipChampPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var clipChampGuid = "{8AB635F8-9A67-4698-AB99-784AD929F3B4}";

            if (AppxPackagesService.PackageExist(photosAppx))
            {
                var userPhotosContext = Registry.CurrentUser.OpenSubKey(clipChampPath)?.GetValue(clipChampGuid);
                var machinePhotosContext = Registry.LocalMachine.OpenSubKey(clipChampPath)?.GetValue(clipChampGuid);
                return userPhotosContext is null && machinePhotosContext is null;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {photosAppx}");
        }

        /// <summary>
        /// Get "Edit with Paint 3D" item in the media files context menu state.
        /// </summary>
        public static bool EditWithPaint3DContext()
        {
            var appxPaint = "Microsoft.MSPaint";

            if (AppxPackagesService.PackageExist(appxPaint))
            {
                var accessValues = new List<object?>()
                {
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.bmp\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.gif\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.jpe\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.jpeg\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.jpg\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.png\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.tif\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                    Registry.ClassesRoot.OpenSubKey("SystemFileAssociations\\.tiff\\Shell\\3D Edit")?.GetValue("ProgrammaticAccessOnly"),
                };

                return !accessValues.TrueForAll(value => value is not null);
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appxPaint}");
        }

        /// <summary>
        /// Get "Print" item in the .bat and .cmd files context menu state.
        /// </summary>
        public static bool PrintCMDContext()
        {
            var accessOnlyValues = new List<object?>()
            {
                Registry.ClassesRoot.OpenSubKey("batfile\\shell\\print")?.GetValue("ProgrammaticAccessOnly"),
                Registry.ClassesRoot.OpenSubKey("cmdfile\\shell\\print")?.GetValue("ProgrammaticAccessOnly"),
            };

            return !accessOnlyValues.TrueForAll(value => value is not null);
        }

        /// <summary>
        /// Get "Include in Library" item in the folders and drives context menu state.
        /// </summary>
        public static bool IncludeInLibraryContext()
        {
            var libraryContextValue = Registry.ClassesRoot.OpenSubKey("Folder\\ShellEx\\ContextMenuHandlers\\Library Location")?.GetValue(string.Empty) as string;
            return !libraryContextValue?.Equals("-{3dad6c5d-2167-4cae-9914-f99e41c12cfa}") ?? true;
        }

        /// <summary>
        /// Get Send to" item in the folders context menu state.
        /// </summary>
        public static bool SendToContext()
        {
            var sendToContext = Registry.ClassesRoot.OpenSubKey("AllFilesystemObjects\\shellex\\ContextMenuHandlers\\SendTo")?.GetValue(string.Empty) as string;
            return !sendToContext?.Equals("-{7BA4C740-9E81-11CF-99D3-00AA004AE837}") ?? true;
        }

        /// <summary>
        /// Get "Bitmap image" item in the "New" context menu state.
        /// </summary>
        public static bool BitmapImageNewContext()
        {
            var paintPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\mspaint.exe";

            if (File.Exists(paintPath))
            {
                var bmpShellNew = Registry.ClassesRoot.OpenSubKey(".bmp\\ShellNew");
                return !(bmpShellNew is null);
            }

            throw new InvalidOperationException($"File {paintPath} not exist");
        }

        /// <summary>
        /// Get "Rich Text Document" item in the "New" context menu state.
        /// </summary>
        public static bool RichTextDocumentNewContext()
        {
            var wordpadPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\Windows NT\\Accessories\\wordpad.exe";

            if (File.Exists(wordpadPath))
            {
                var rtfShellNew = Registry.ClassesRoot.OpenSubKey(".rtf\\ShellNew");
                return !(rtfShellNew is null);
            }

            throw new InvalidOperationException($"File {wordpadPath} not exist");
        }

        /// <summary>
        /// Get "Compressed (zipped) Folder" item in the "New" context menu state.
        /// </summary>
        public static bool CompressedFolderNewContext()
        {
            var zipShellNew = Registry.ClassesRoot.OpenSubKey(".zip\\CompressedFolder\\ShellNew");
            return !(zipShellNew is null);
        }

        /// <summary>
        /// Get "Open", "Print", and "Edit" context menu items available when selecting more than 15 files state.
        /// </summary>
        public static bool MultipleInvokeContext()
        {
            var multipleInvokePrompt = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer")?.GetValue("MultipleInvokePromptMinimum") as int?;
            return multipleInvokePrompt?.Equals(300) ?? false;
        }

        /// <summary>
        /// Get "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        public static bool UseStoreOpenWith()
        {
            var storeOpenWith = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\Explorer")?.GetValue("NoUseStoreOpenWith") as int?;
            return !storeOpenWith?.Equals(1) ?? true;
        }

        /// <summary>
        /// Get "Open in Windows Terminal" item in the folders context menu state.
        /// </summary>
        public static bool OpenWindowsTerminalContext()
        {
            var appxTerminal = "Microsoft.WindowsTerminal";

            if (AppxPackagesService.PackageExist(appxTerminal))
            {
                var extensionsBlockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
                var terminalContextGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

                var userBlockedGuid = Registry.CurrentUser.OpenSubKey(extensionsBlockPath)?.GetValue(terminalContextGuid);
                var machineBlockedGuid = Registry.LocalMachine.OpenSubKey(extensionsBlockPath)?.GetValue(terminalContextGuid);
                return userBlockedGuid is null && machineBlockedGuid is null;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appxTerminal}");
        }

        /// <summary>
        /// Get Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        public static bool OpenWindowsTerminalAdminContext()
        {
            var appxTerminal = "Microsoft.WindowsTerminal";

            if (AppxPackagesService.PackageExist(appxTerminal))
            {
                var extensionsBlockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
                var adminContextGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

                var userBlockedGuid = Registry.CurrentUser.OpenSubKey(extensionsBlockPath)?.GetValue(adminContextGuid);
                var machineBlockedGuid = Registry.LocalMachine.OpenSubKey(extensionsBlockPath)?.GetValue(adminContextGuid);

                if (userBlockedGuid is null && machineBlockedGuid is null)
                {
                    try
                    {
                        var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";
                        var jsonSettings = File.ReadAllText(terminalSettings, Encoding.UTF8);
                        var jsonProfile = JsonExtensions.ToObject<MsTerminalSettingsDto>(jsonSettings);
                        return jsonProfile?.Profiles?.Defaults?.Elevate ?? false;
                    }
                    catch (ArgumentException)
                    {
                        throw new InvalidOperationException($"The {appxTerminal} configuration file is not valid");
                    }
                }

                return true;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed: {appxTerminal}");
        }

        /// <summary>
        /// Get images edit from context menu state.
        /// </summary>
        public static bool ImagesEditContext()
        {
            var paintPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\mspaint.exe";

            if (File.Exists(paintPath))
            {
                var accessPath = "SystemFileAssociations\\image\\shell\\edit";
                var accessValue = Registry.ClassesRoot.OpenSubKey(accessPath)?.GetValue("ProgrammaticAccessOnly") as string;
                return accessValue is null;
            }

            throw new InvalidOperationException($"File {paintPath} not exist");
        }
    }
}
