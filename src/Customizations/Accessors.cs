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
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <summary>
    /// Get OS settings.
    /// </summary>
    public static class Accessors
    {
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService DataService = App.GetService<ICommonDataService>();
        private static readonly IRedistributablePackageService PackageService = App.GetService<IRedistributablePackageService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IHttpService HttpService = App.GetService<IHttpService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly INetworkAdapterService NetworkAdapterService = App.GetService<INetworkAdapterService>();
        private static readonly IOneDriveService OneDriveService = App.GetService<IOneDriveService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IUpdateService UpdateService = App.GetService<IUpdateService>();
        private static readonly IXmlService XmlService = App.GetService<IXmlService>();

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
        /// Get telemetry scheduled tasks state.
        /// </summary>
        public static bool ScheduledTasks()
        {
            var telemetryTasks = new List<Task?>()
            {
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser Exp"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Autochk\\Proxy"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
            };

            return telemetryTasks.TrueForAll(task => task is null)
                ? throw new CustomizationException(exceptionMessage: "No scheduled telemetry tasks were found", resourceKey: "UIModel_Exception_ScheduledTasks")
                : telemetryTasks.Exists(task => task is not null && task.State != TaskState.Disabled);
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
        /// Get Start menu recommendations state.
        /// </summary>
        public static bool StartRecommendationsTips()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var startIrisRecommendations = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations") as int? ?? -1;
            return !startIrisRecommendations.Equals(0);
        }

        /// <summary>
        /// Get Start Menu notifications state.
        /// </summary>
        public static bool StartAccountNotifications()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var startAccountNotifications = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_AccountNotifications") as int? ?? -1;
            return !startAccountNotifications.Equals(0);
        }

        /// <summary>
        /// Get the "This PC" icon on Desktop state.
        /// </summary>
        public static bool ThisPC()
        {
            var panelValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel", "{20D04FE0-3AEA-1069-A2D8-08002B30309D}") as int? ?? -1;
            return panelValue.Equals(0);
        }

        /// <summary>
        /// Get item check boxes state.
        /// </summary>
        public static bool CheckBoxes()
        {
            var checkValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "AutoCheckSelect") as int? ?? -1;
            return !checkValue.Equals(0);
        }

        /// <summary>
        /// Get hidden files, folders, and drives state.
        /// </summary>
        public static bool HiddenItems()
        {
            var itemsValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden") as int? ?? -1;
            return itemsValue.Equals(1);
        }

        /// <summary>
        /// Get file name extensions visibility state.
        /// </summary>
        public static bool FileExtensions()
        {
            var extensionsValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt") as int? ?? -1;
            return extensionsValue.Equals(0);
        }

        /// <summary>
        /// Get folder merge conflicts state.
        /// </summary>
        public static bool MergeConflicts()
        {
            var mergeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideMergeConflicts") as int? ?? -1;
            return mergeValue.Equals(0);
        }

        /// <summary>
        /// Get how to open File Explorer.
        /// </summary>
        public static int OpenFileExplorerTo()
        {
            var fileValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "LaunchTo") as int? ?? -1;
            return fileValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get File Explorer compact mode state.
        /// </summary>
        public static bool FileExplorerCompactMode()
        {
            var compactModeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "UseCompactMode") as int? ?? -1;
            return compactModeValue.Equals(1);
        }

        /// <summary>
        /// Get File Explorer provider notification visibility state.
        /// </summary>
        public static bool OneDriveFileExplorerAd()
        {
            var notificationsValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSyncProviderNotifications") as int? ?? -1;
            return !notificationsValue.Equals(0);
        }

        /// <summary>
        /// Get snap a window state.
        /// </summary>
        public static bool SnapAssist()
        {
            var snapValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "SnapAssist") as int? ?? -1;
            return !snapValue.Equals(0);
        }

        /// <summary>
        /// Get file transfer dialog box mode.
        /// </summary>
        public static int FileTransferDialog()
        {
            var modeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\OperationStatusManager", "EnthusiastMode") as int? ?? -1;
            return modeValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get recycle bin confirmation dialog state.
        /// </summary>
        public static bool RecycleBinDeleteConfirmation()
        {
            var shellValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "ShellState") as byte[];
            return shellValue is { Length: > 4 } && (shellValue[4] & 0x04) == 0;
        }

        /// <summary>
        /// Get recently used Quick access files state.
        /// </summary>
        public static bool QuickAccessRecentFiles()
        {
            var recentValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "ShowRecent") as int? ?? -1;
            return !recentValue.Equals(0);
        }

        /// <summary>
        /// Get frequently used Quick access folders state.
        /// </summary>
        public static bool QuickAccessFrequentFolders()
        {
            var frequentValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "ShowFrequent") as int? ?? -1;
            return !frequentValue.Equals(0);
        }

        /// <summary>
        /// Get taskbar position.
        /// </summary>
        public static int TaskbarPosition()
        {
            // The taskbar is at the bottom if the value is missing
            var location = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarLocation") as int? ?? 3;
            return location switch
            {
                1 => 2,
                0 => 3,
                2 => 4,
                _ => 1,
            };
        }

        /// <summary>
        /// Get taskbar alignment state.
        /// </summary>
        public static int TaskbarAlignment()
        {
            var taskbarValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarAl") as int? ?? -1;
            return taskbarValue.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get taskbar widgets icon state.
        /// </summary>
        public static bool TaskbarWidgets()
        {
            if (AppxPackagesService.PackageExist("MicrosoftWindows.Client.WebExperience"))
            {
                var taskbarValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarDa") as int? ?? -1;
                return !taskbarValue.Equals(0);
            }

            throw new CustomizationException(exceptionMessage: "MicrosoftWindows.Client.WebExperience package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled");
        }

        /// <summary>
        /// Get Search on the taskbar state.
        /// </summary>
        public static int TaskbarSearch()
        {
            var searchModeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Search", "SearchboxTaskbarMode") as int? ?? -1;

            return searchModeValue switch
            {
                0 => 1,
                1 => 2,
                3 => 3,
                _ => 4,
            };
        }

        /// <summary>
        /// Get search highlights state.
        /// </summary>
        public static bool SearchHighlights()
        {
            var searchEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled") as int? ?? -1;
            var searchSuggestions = Registry.CurrentUser.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions") as int? ?? -1;

            // Search highlights are also hidden when "Ask Copilot" and "Find results in Web" are disabled.
            // Showing them enables web search back
            if (searchEnabled.Equals(0) || searchSuggestions.Equals(1))
            {
                return false;
            }

            var dynamicSearch = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings", "IsDynamicSearchBoxEnabled") as int? ?? -1;
            return !dynamicSearch.Equals(0);
        }

        /// <summary>
        /// Get taskbar task view button state.
        /// </summary>
        public static bool TaskViewButton()
        {
            var buttonValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton") as int? ?? -1;
            return !buttonValue.Equals(0);
        }

        /// <summary>
        /// Get seconds on the taskbar clock state.
        /// </summary>
        public static bool SecondsInSystemClock()
        {
            var clockValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSecondsInSystemClock") as int? ?? -1;
            return clockValue.Equals(1);
        }

        /// <summary>
        /// Get taskbar combine state.
        /// </summary>
        public static int TaskbarCombine()
        {
            var levelValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarGlomLevel") as int? ?? -1;
            return levelValue.Equals(-1) ? 1 : levelValue + 1;
        }

        /// <summary>
        /// Get end task in taskbar by click state.
        /// </summary>
        public static bool TaskbarEndTask()
        {
            var taskValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings", "TaskbarEndTask") as int? ?? -1;
            return taskValue.Equals(1);
        }

        /// <summary>
        /// Get Control Panel icons view state.
        /// </summary>
        public static int ControlPanelView()
        {
            var viewValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "AllItemsIconView") as int? ?? 0;
            var pageValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "StartupPage") as int? ?? 0;

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
            var themeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get apps color mode state.
        /// </summary>
        public static int AppColorMode()
        {
            var themeValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get first sign-in animation state.
        /// </summary>
        public static bool FirstLogonAnimation()
        {
            var logonValue = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "EnableFirstLogonAnimation") as int? ?? -1;
            return !logonValue.Equals(0);
        }

        /// <summary>
        /// Get JPEG wallpapers quality state.
        /// </summary>
        public static int JPEGWallpapersQuality()
        {
            var qualityValue = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Desktop", "JPEGImportQuality") as int? ?? -1;
            return qualityValue.Equals(100) ? 1 : 2;
        }

        /// <summary>
        /// Get "- Shortcut" suffix state.
        /// </summary>
        public static bool ShortcutsSuffix()
        {
            var shortcutsValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates", "ShortcutNameTemplate") as string ?? string.Empty;
            return !shortcutsValue.Equals("%s.lnk");
        }

        /// <summary>
        /// Get Print screen button state.
        /// </summary>
        public static bool PrtScnSnippingTool()
        {
            var snippingValue = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Keyboard", "PrintScreenKeyForSnippingEnabled") as int? ?? -1;
            return snippingValue.Equals(1);
        }

        /// <summary>
        /// Get input method for app window state.
        /// </summary>
        public static bool AppsLanguageSwitch()
            => PowerShellService.Invoke("Get-WinLanguageBarOption").FirstOrDefault()?.GetPropertyValue<bool>("IsLegacySwitchingMode") ?? false;

        /// <summary>
        /// Get Aero Shake state.
        /// </summary>
        public static bool AeroShaking()
        {
            var shakingValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "DisallowShaking") as int? ?? -1;
            return shakingValue.Equals(0);
        }

        /// <summary>
        /// Get "Windows 11 Cursors Concept" from Jepri Creations state.
        /// </summary>
        public static int Cursors()
        {
            var cursors = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Cursors", string.Empty) as string ?? string.Empty;
            return cursors switch
            {
                "W11 Cursor Dark Free by Jepri Creations" => 1,
                "W11 Cursor Light Free by Jepri Creations" => 2,
                _ => 3,
            };
        }

        /// <summary>
        /// Get files and folders grouping state.
        /// </summary>
        public static int FolderGroupBy()
        {
            var groupByPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderTypes\\{885a186e-a440-4ada-812b-db871b942259}\\TopViews\\{00000000-0000-0000-0000-000000000000}";
            var groupByValue = Registry.CurrentUser.GetSubKeyValue(groupByPath, "GroupBy") as string ?? string.Empty;
            return groupByValue.Equals("System.Null") ? 1 : 2;
        }

        /// <summary>
        /// Get navigation pane expand state.
        /// </summary>
        public static bool NavigationPaneExpand()
        {
            var paneValue = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "NavPaneExpandToCurrentFolder") as int? ?? -1;
            return paneValue.Equals(1);
        }

        /// <summary>
        /// Get Start menu recently added apps state.
        /// </summary>
        public static bool RecentlyAddedStartApps()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var showRecentList = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList") as int? ?? -1;
            return !showRecentList.Equals(0);
        }

        /// <summary>
        /// Get Start all apps view state.
        /// </summary>
        public static int StartAppsView()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var appsView = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "AllAppsViewMode") as int? ?? 0;
            return appsView + 1;
        }

        /// <summary>
        /// Get most used apps in Start.
        /// </summary>
        public static bool MostUsedStartApps()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var usedStartApps = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowFrequentList") as int? ?? -1;
            return usedStartApps.Equals(1);
        }

        /// <summary>
        /// Get Start menu layout state.
        /// </summary>
        public static int StartLayout()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            // Default — 0
            // Show More Pins — 1
            // Show More Recommendations — 2
            var startLayout = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_Layout") as int? ?? 0;
            return startLayout + 1;
        }

        /// <summary>
        /// Get recommended section state.
        /// </summary>
        public static bool StartRecommendedSection()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            var startRecommended = new List<int>()
            {
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList") as int? ?? -1,
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowFrequentList") as int? ?? -1,
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations") as int? ?? -1,
                Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_TrackDocs") as int? ?? -1,
            };

            return !startRecommended.TrueForAll(value => value.Equals(0));
        }

        /// <summary>
        /// Get One Drive state.
        /// </summary>
        public static bool OneDrive()
        {
            if (OneDriveService.IsInstalled())
            {
                if (OneDriveService.UserIsLogged())
                {
                    throw new CustomizationException(exceptionMessage: "Log out from OneDrive account before uninstalling the application", resourceKey: "UIModel_Exception_OneDrive_LogOut");
                }

                return true;
            }
            else
            {
                var setupFile = OneDriveService.GetSetupFileOrDefault();
                App.Logger.LogOneDriveSetupFile(setupFile);

                if (!string.IsNullOrEmpty(setupFile) || HttpService.UrlIsAvailable("https://g.live.com/1rewlive5skydrive/OneDriveProductionV2"))
                {
                    return false;
                }

                throw new CustomizationException(exceptionMessage: "OneDriveSetup.exe was not found and there is no Internet access to download OneDrive installer", resourceKey: "UIModel_Exception_OneDrive_InstallerNotFound");
            }
        }

        /// <summary>
        /// Get storage sense state.
        /// </summary>
        public static bool StorageSense()
        {
            var storagePolicy01 = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "01") as int? ?? -1;

            // "01" is the Storage Sense main switch. Other values are its options that the user may change
            return storagePolicy01.Equals(1);
        }

        /// <summary>
        /// Get hibernation state.
        /// </summary>
        public static bool Hibernation()
        {
            using var powerKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Power");

            // Windows uses HibernateEnabledDefault if HibernateEnabled is not set
            var isEnabled = powerKey?.GetValue("HibernateEnabled") as int? ?? powerKey?.GetValue("HibernateEnabledDefault") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get long path limit state.
        /// </summary>
        public static bool Win32LongPathsSupport()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get BSOD error state.
        /// </summary>
        public static bool BSoDStopError()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\CrashControl", "DisplayParameters") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get administrator approval mode state.
        /// </summary>
        public static int AdminApprovalMode()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "ConsentPromptBehaviorAdmin") as int? ?? -1;
            return isEnabled.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get delivery optimization state.
        /// </summary>
        public static bool DeliveryOptimization()
        {
            var isEnabled = Registry.Users.GetSubKeyValue("S-1-5-20\\Software\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Settings", "DownloadMode") as int? ?? -1;
            return !isEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows manage default printer state.
        /// </summary>
        public static bool WindowsManageDefaultPrinter()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows", "LegacyDefaultPrinterMode") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get update Microsoft products state.
        /// </summary>
        public static bool UpdateMicrosoftProducts() => UpdateService.HasMicrosoftProductsUpdate();

        /// <summary>
        /// Get restart notification state.
        /// </summary>
        public static bool RestartNotification()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "RestartNotificationsAllowed2") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get device restart after update state.
        /// </summary>
        public static bool RestartDeviceAfterUpdate()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsExpedited") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get active hours restart state.
        /// </summary>
        public static int ActiveHours()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "SmartActiveHoursState") as int? ?? -1;
            return isEnabled.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get Windows latest update state.
        /// </summary>
        public static bool WindowsLatestUpdate()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsContinuousInnovationOptedIn") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get network adapters power state.
        /// </summary>
        public static bool NetworkAdaptersSavePower()
        {
            return NetworkAdapterService.GetTurnOffDevice()
                ?? throw new CustomizationException(exceptionMessage: "There is no network adapter which has an AllowComputerToTurnOffDevice property", resourceKey: "UIModel_Exception_NetworkAdaptersSavePower");
        }

        /// <summary>
        /// Get input method state.
        /// </summary>
        public static int InputMethod()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Control Panel\\International\\User Profile", "InputMethodOverride") as string ?? string.Empty;
            return isEnabled.Equals("0409:00000409") ? 1 : 2;
        }

        /// <summary>
        /// Get Print Screen folder state.
        /// </summary>
        public static int WinPrtScrFolder()
        {
            if (OneDriveService.UserIsLogged())
            {
                throw new CustomizationException(exceptionMessage: "Log out from OneDrive account", resourceKey: "UIModel_Exception_OneDrive_LogOut");
            }

            var prtScrPath = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}") as string ?? string.Empty;
            var desktopPath = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "Desktop") as string;
            return prtScrPath.Equals(desktopPath, StringComparison.OrdinalIgnoreCase) ? 1 : 2;
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

        /// <summary>
        /// Get reserved storage state.
        /// </summary>
        public static bool ReservedStorage()
        {
            using var reserveKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ReserveManager");
            var miscPolicy = reserveKey?.GetValue("MiscPolicyInfo") as int? ?? -1;
            var passedPolicy = reserveKey?.GetValue("PassedPolicy") as int? ?? -1;
            var shippedReserves = reserveKey?.GetValue("ShippedWithReserves") as int? ?? -1;
            return !(miscPolicy.Equals(2) && passedPolicy.Equals(0) && shippedReserves.Equals(1));
        }

        /// <summary>
        /// Get help page state.
        /// </summary>
        public static bool F1HelpPage()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Classes\\Typelib\\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\\1.0\\0\\win64", string.Empty) as string;
            return isEnabled is null;
        }

        /// <summary>
        /// Get Num Lock state.
        /// </summary>
        public static bool NumLock()
        {
            var isEnabled = Registry.Users.GetSubKeyValue(".DEFAULT\\Control Panel\\Keyboard", "InitialKeyboardIndicators") as string ?? string.Empty;

            // Num Lock is on when the second bit is set: "2" means the same as "2147483650"
            return uint.TryParse(isEnabled, out var indicators) && (indicators & 2) != 0;
        }

        /// <summary>
        /// Get Caps Lock state.
        /// </summary>
        public static bool CapsLock()
        {
            var scancodeMap = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 58, 0, 0, 0, 0, 0 };
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", "Scancode Map") as byte[] ?? [];
            return !isEnabled.SequenceEqual(scancodeMap);
        }

        /// <summary>
        /// Get sticky shift state.
        /// </summary>
        public static bool StickyShift()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Accessibility\\StickyKeys", "Flags") as string ?? string.Empty;
            return !isEnabled.Equals("506");
        }

        /// <summary>
        /// Get autoplay state.
        /// </summary>
        public static bool Autoplay()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers", "DisableAutoplay") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get thumbnail cache state.
        /// </summary>
        public static bool ThumbnailCacheRemoval()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun") as int? ?? -1;
            var isEnabledWow = Registry.LocalMachine.GetSubKeyValue("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun") as int? ?? -1;
            return !(isEnabled.Equals(0) && isEnabledWow.Equals(0));
        }

        /// <summary>
        /// Get save restartable apps state.
        /// </summary>
        public static bool SaveRestartableApps()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "RestartApps") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get power plan state.
        /// </summary>
        public static int PowerPlan()
        {
            var activePlan = InstrumentationService.GetPowerPlans()
                .Select(plan => (IsActive: (bool)plan.GetPropertyValue("IsActive"), InstanceID: (string)plan.GetPropertyValue("InstanceID")))
                .First(plan => plan.IsActive);

            return activePlan.InstanceID.Contains("{8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c}", StringComparison.OrdinalIgnoreCase) ? 1 : 2;
        }

        /// <summary>
        /// Get registry backup state.
        /// </summary>
        public static bool RegistryBackup()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Configuration Manager", "EnablePeriodicBackup") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get restore previous folders state.
        /// </summary>
        public static bool RestorePreviousFolders()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "PersistBrowsers") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get Windows Terminal default app state.
        /// </summary>
        public static int DefaultTerminalApp()
        {
            var appxPackage = AppxPackagesService.GetPackageOrDefault("Microsoft.WindowsTerminal");

            // The option is not available without Windows Terminal
            if (appxPackage is null)
            {
                throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled");
            }

            var appxPath = $"Software\\Classes\\PackagedCom\\Package\\{appxPackage.Id.FullName}\\Class";
            var consoleId = string.Empty;
            var consolePath = "Console\\%%Startup";
            var terminalId = string.Empty;
            using var packageClasses = Registry.LocalMachine.OpenSubKey(appxPath);
            packageClasses?.GetSubKeyNames()
                .ForEach(key =>
                {
                    switch (Registry.LocalMachine.GetSubKeyValue(Path.Combine(appxPath, key), "ServerId") ?? -1)
                    {
                        case 0:
                            consoleId = key;
                            break;
                        case 1:
                            terminalId = key;
                            break;
                        default:
                            break;
                    }
                });
            var delegationConsole = Registry.CurrentUser.GetSubKeyValue(consolePath, "DelegationConsole") as string ?? string.Empty;
            var delegationTerminal = Registry.CurrentUser.GetSubKeyValue(consolePath, "DelegationTerminal") as string ?? string.Empty;
            return consoleId.Length > 0 && delegationConsole.Equals(consoleId, StringComparison.OrdinalIgnoreCase) && delegationTerminal.Equals(terminalId, StringComparison.OrdinalIgnoreCase) ? 1 : 2;
        }

        /// <summary>
        /// Get clock in notification center state.
        /// </summary>
        public static bool ShowClockInNotificationCenter()
        {
            var showClock = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowClockInNotificationCenter") as int? ?? -1;
            return showClock.Equals(1);
        }

        /// <summary>
        /// Get .NET 10 desktop runtime install state.
        /// </summary>
        public static bool InstallDotNetRuntime_10()
        {
            var latestVersion = DataService.LatestReleaseNET10?.Version ?? throw new CustomizationException(exceptionMessage: "Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired");

            // The latest version is installed if its installer is in the "Package Cache" folder
            return PackageService.IsInstalled($"windowsdesktop-runtime-{latestVersion}-win-x64.exe")
                ? throw new CustomizationException(exceptionMessage: "Latest .NET version is installed", resourceKey: "UIModel_Exception_LatestVersionInstalled")
                : false;
        }

        /// <summary>
        /// Determines whether .NET 10 desktop runtime is installed and a newer build is available.
        /// </summary>
        public static bool InstallDotNetRuntime_10_UpdateAvailable()
            => DataService.LatestReleaseNET10?.Version is string latestVersion
                && PackageService.IsInstalled("windowsdesktop-runtime-10.*-win-x64.exe")
                && !PackageService.IsInstalled($"windowsdesktop-runtime-{latestVersion}-win-x64.exe");

        /// <summary>
        /// Get .NET 11 desktop runtime install state.
        /// </summary>
        public static bool InstallDotNetRuntime_11()
        {
            var latestVersion = DataService.LatestReleaseNET11?.Version ?? throw new CustomizationException(exceptionMessage: "Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired");

            // The latest version is installed if its installer is in the "Package Cache" folder
            return PackageService.IsInstalled($"windowsdesktop-runtime-{latestVersion}-win-x64.exe")
                ? throw new CustomizationException(exceptionMessage: "Latest .NET version is installed", resourceKey: "UIModel_Exception_LatestVersionInstalled")
                : false;
        }

        /// <summary>
        /// Determines whether .NET 11 desktop runtime is installed and a newer build is available.
        /// </summary>
        public static bool InstallDotNetRuntime_11_UpdateAvailable()
            => DataService.LatestReleaseNET11?.Version is string latestVersion
                && PackageService.IsInstalled("windowsdesktop-runtime-11.*-win-x64.exe")
                && !PackageService.IsInstalled($"windowsdesktop-runtime-{latestVersion}-win-x64.exe");

        /// <summary>
        /// Get Microsoft Visual C++ x86 redistributable package version.
        /// </summary>
        public static bool InstallVisualC_x86()
        {
            var latestVersion = DataService.LatestReleaseVC?.Version ?? throw new CustomizationException(exceptionMessage: "Url https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired");
            var installedVersion = PackageService.GetInstalledVersion("vc_redist.x86.exe");

            // Not installed, or a newer build is available
            return installedVersion is null || latestVersion > installedVersion ? false : throw new CustomizationException(exceptionMessage: "Latest Visual C++ x86 version is installed", resourceKey: "UIModel_Exception_LatestVersionInstalled");
        }

        /// <summary>
        /// Determines whether Microsoft Visual C++ x86 redistributable package is installed and a newer build is available.
        /// </summary>
        public static bool InstallVisualC_x86_UpdateAvailable()
            => PackageService.GetInstalledVersion("vc_redist.x86.exe") is Version installedVersion && DataService.LatestReleaseVC?.Version > installedVersion;

        /// <summary>
        /// Get Microsoft Visual C++ x64 redistributable package version.
        /// </summary>
        public static bool InstallVisualC_x64()
        {
            var latestVersion = DataService.LatestReleaseVC?.Version ?? throw new CustomizationException(exceptionMessage: "Url https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired");
            var installedVersion = PackageService.GetInstalledVersion("vc_redist.x64.exe");

            // Not installed, or a newer build is available
            return installedVersion is null || latestVersion > installedVersion ? false : throw new CustomizationException(exceptionMessage: "Latest Visual C++ x64 version is installed", resourceKey: "UIModel_Exception_LatestVersionInstalled");
        }

        /// <summary>
        /// Determines whether Microsoft Visual C++ x64 redistributable package is installed and a newer build is available.
        /// </summary>
        public static bool InstallVisualC_x64_UpdateAvailable()
            => PackageService.GetInstalledVersion("vc_redist.x64.exe") is Version installedVersion && DataService.LatestReleaseVC?.Version > installedVersion;

        /// <summary>
        /// Get Windows AI state.
        /// </summary>
        public static bool RemoveWindowsAI()
        {
            var recall = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\Notifications\\OptionalFeatures\\Recall", "Selection") as int? ?? -1;
            // Windows AI is considered removed when Recall is not enabled and the Copilot app is not installed
            return !recall.Equals(1) && !AppxPackagesService.PackageExist("Microsoft.Copilot");
        }

        /// <summary>
        /// Gets XBOX game bar state.
        /// </summary>
        public static bool XBOXGameBar()
        {
            var appCaptureIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled") as int? ?? -1;
            var dvrIsEnabled = Registry.CurrentUser.GetSubKeyValue("System\\GameConfigStore", "GameDVR_Enabled") as int? ?? -1;

            if (appCaptureIsEnabled == 0 && dvrIsEnabled == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets XBOX game tips state.
        /// </summary>
        public static bool XBOXGameTips()
        {
            if (AppxPackagesService.PackageExist("Microsoft.XboxGamingOverlay"))
            {
                var startupPanelIsEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\GameBar", "ShowStartupPanel") as int? ?? -1;
                return startupPanelIsEnabled != 0;
            }

            throw new CustomizationException(exceptionMessage: "XBOX Game Bar package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled");
        }

        /// <summary>
        /// Get GPU scheduling state.
        /// </summary>
        public static bool GPUScheduling()
        {
            const int WDDMMinimalVersion = 2700;
            // Determining whether PC has an external graphics card
            var isExternalDAC = InstrumentationService.DetectDACType();
            var isVM = InstrumentationService.DetectVM();
            var wddmVersion = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\\FeatureSetUsage", "WddmVersion_Min") as int? ?? -1;

            var hwSchMode = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode") as int? ?? -1;

            // The checks below are required to enable the feature only, so it can always be disabled
            if (hwSchMode == 2)
            {
                return true;
            }

            // Checking whether a WDDM version is 2.7 or higher
            if (isExternalDAC && !isVM && wddmVersion >= WDDMMinimalVersion)
            {
                return false;
            }

            if (isVM)
            {
                throw new CustomizationException(exceptionMessage: "Windows is running on a virtual machine", resourceKey: "UIModel_Exception_GPUScheduling_VirtualMachine");
            }

            if (!isExternalDAC)
            {
                throw new CustomizationException(exceptionMessage: "No dedicated graphics card found", resourceKey: "UIModel_Exception_GPUScheduling_DedicatedGPUNotFound");
            }

            throw new CustomizationException(exceptionMessage: $"Supported WDDM version is {WDDMMinimalVersion} and higher. Current WDDM version is {wddmVersion}", resourceKey: "UIModel_Exception_GPUScheduling_WDDMVersion");
        }

        /// <summary>
        /// Get Windows Cleanup scheduled task state.
        /// </summary>
        public static bool CleanupTask()
        {
            var cleanupTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup") is not null;
            var notificationTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup Notification") is not null;
            var cleanupPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1"));
            var notificationPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1"));
            var cleanupXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows Cleanup"));
            var notificationXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows Cleanup Notification"));
            var cleanupArgumentsPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1")}";
            var notificationTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1")}";

            // Letter case is ignored: the same path can be written as "System32" or "system32"
            return cleanupTaskExist && notificationTaskExist && cleanupPsExist && notificationPsExist && (cleanupXmlArguments?.InnerText.Equals(cleanupArgumentsPattern, StringComparison.OrdinalIgnoreCase) ?? false) && (notificationXmlArguments?.InnerText.Equals(notificationTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Get SoftwareDistribution scheduled task state.
        /// </summary>
        public static bool SoftwareDistributionTask()
        {
            var distributionTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\SoftwareDistribution") is not null;
            var distributionPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1"));
            var distributionXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistribution"));
            var distributionTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1")}";
            return distributionTaskExist && distributionPsExist && (distributionXmlArguments?.InnerText.Equals(distributionTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Get "Temp" scheduled task state.
        /// </summary>
        public static bool TempTask()
        {
            var tempTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Temp") is not null;
            var tempPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1"));
            var tempXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Temp"));
            var tempTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1")}";
            return tempTaskExist && tempPsExist && (tempXmlArguments?.InnerText.Equals(tempTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Get Windows network protection state.
        /// </summary>
        public static bool NetworkProtection()
        {
            var defenderIsDefaultAV = DataService.DefenderIsDefaultAV;

            if (defenderIsDefaultAV)
            {
                var networkProtection = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows Defender\\Windows Defender Exploit Guard\\Network Protection", "EnableNetworkProtection") as int? ?? -1;
                return networkProtection.Equals(1);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows PUApps detection state.
        /// </summary>
        public static bool PUAppsDetection()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                var puaProtection = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows Defender", "PUAProtection") as int? ?? -1;
                return puaProtection.Equals(1);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Microsoft Defender sandbox state.
        /// </summary>
        public static bool DefenderSandbox()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                // The sandbox state is set by the machine environment variable and applied after a restart only
                var forceSandbox = Environment.GetEnvironmentVariable("MP_FORCE_USE_SANDBOX", EnvironmentVariableTarget.Machine);
                return string.IsNullOrEmpty(forceSandbox) ? ProcessService.Exist("MsMpEngCP") : forceSandbox.Equals("1");
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows event viewer custom view state.
        /// </summary>
        public static bool EventViewerCustomView()
        {
            // The audit policy is read via the system API: the auditpol output is localized
            var inclusionSettingEnabled = ProcessCreationAuditIsEnabled() ?? false;
            var processCreationEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled") as int? ?? -1;
            var eventXml = XmlService.TryLoad(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml"));
            var eventQueryCount = eventXml?.SelectNodes("//QueryList/Query/Select")?.Count ?? -1;
            var moduleLoggingEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging") as int? ?? -1;
            var powerShellAsterisk = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*") as string ?? string.Empty;
            var scriptLoggingEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging") as int? ?? -1;
            return inclusionSettingEnabled && processCreationEnabled.Equals(1) && eventQueryCount.Equals(3) && moduleLoggingEnabled.Equals(1) && powerShellAsterisk.Equals("*") && scriptLoggingEnabled.Equals(1);
        }

        /// <summary>
        /// Get Windows SmartScreen state.
        /// </summary>
        public static bool AppsSmartScreen()
        {
            var defenderEnabled = DataService.DefenderIsDefaultAV;

            if (defenderEnabled)
            {
                var smartScreenIsEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "SmartScreenEnabled") as string ?? string.Empty;
                return !smartScreenIsEnabled.Equals("Off", StringComparison.OrdinalIgnoreCase);
            }

            throw new CustomizationException(exceptionMessage: "Microsoft Defender is not the default antivirus", resourceKey: "UIModel_Exception_AntispywareProtectionDisabled");
        }

        /// <summary>
        /// Get Windows save zone state.
        /// </summary>
        public static bool SaveZoneInformation()
        {
            var saveZoneInformation = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation") as int? ?? -1;
            return !saveZoneInformation.Equals(1);
        }

        /// <summary>
        /// Get Windows Sandbox state.
        /// </summary>
        public static bool WindowsSandbox()
        {
            static bool GetSandboxEnabled()
            {
                // Checking whether x86 virtualization is enabled in the firmware
                // The feature state is read from the registry: it is instant
                var selection = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\Notifications\\OptionalFeatures\\Containers-DisposableClientVM", "Selection") as int?
                    ?? throw new CustomizationException(exceptionMessage: "Windows Sandbox state undefined", resourceKey: "UIModel_Exception_WindowsSandbox_UndefinedState");

                // 1 means that the feature is enabled or will be enabled after a restart
                return selection == 1;
            }

            if (DataService.OsProperties.Edition.Contains("Professional", StringComparison.OrdinalIgnoreCase) || DataService.OsProperties.Edition.Contains("Enterprise", StringComparison.OrdinalIgnoreCase) || DataService.OsProperties.Edition.Contains("Education", StringComparison.OrdinalIgnoreCase))
            {
                // The checks below are required to enable the feature only, so it can always be disabled
                if (GetSandboxEnabled())
                {
                    return true;
                }

                // Determining whether Hyper-V is enabled
                var virtualizationIsEnabled = InstrumentationService.CPUVirtualizationIsEnabled() ?? throw new CustomizationException(exceptionMessage: "This CPU does not support virtualization", resourceKey: "UIModel_Exception_VirtualizationNotSupported");
                var hypervisorPresent = InstrumentationService.HypervisorIsPresent() ?? throw new CustomizationException(exceptionMessage: "Enable virtualization in UEFI", resourceKey: "UIModel_Exception_VirtualizationDisabled");

                if (virtualizationIsEnabled)
                {
                    return false;
                }
                else if (hypervisorPresent)
                {
                    return false;
                }

                throw new CustomizationException(exceptionMessage: "This PC does not support Windows Sandbox feature", resourceKey: "UIModel_Exception_UnsupportedFeature");
            }

            throw new CustomizationException(exceptionMessage: "Unsupported Windows edition", resourceKey: "UIModel_Exception_WindowsSandbox_UnsupportedEdition");
        }

        /// <summary>
        /// Get Local Security Authority state.
        /// </summary>
        public static bool LocalSecurityAuthority()
        {
            var runAsPPL = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL") ?? -1;
            var runAsPPLBoot = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot") ?? -1;
            var runAsPPLPolicy = Registry.LocalMachine.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\System", "RunAsPPL") ?? -1;

            // The checks below are required to enable the feature only, so it can always be disabled
            if ((runAsPPL.Equals(2) && runAsPPLBoot.Equals(2)) || runAsPPLPolicy.Equals(2))
            {
                return true;
            }

            var virtualizationIsEnabled = InstrumentationService.CPUVirtualizationIsEnabled() ?? throw new CustomizationException(exceptionMessage: "This CPU does not support virtualization", resourceKey: "UIModel_Exception_VirtualizationNotSupported");
            var hypervisorPresent = InstrumentationService.HypervisorIsPresent() ?? throw new CustomizationException(exceptionMessage: "Enable virtualization in UEFI", resourceKey: "UIModel_Exception_VirtualizationDisabled");

            if (virtualizationIsEnabled)
            {
                return false;
            }
            else if (hypervisorPresent)
            {
                return false;
            }

            throw new CustomizationException(exceptionMessage: "This PC does not support Local Security Authority feature", resourceKey: "UIModel_Exception_UnsupportedFeature");
        }

        /// <summary>
        /// Get "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        public static bool MSIExtractContext()
        {
            var muiVerb = Registry.ClassesRoot.GetSubKeyValue("Msi.Package\\shell\\Extract", "MUIVerb") as string;
            return muiVerb?.Equals("@shell32.dll,-37514") ?? false;
        }

        /// <summary>
        /// Get "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        public static bool CABInstallContext()
        {
            var cabProgId = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice", "ProgId") as string ?? string.Empty;
            using var cabUserChoice = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice");
            var cabArchiver = cabProgId.Equals("CABFolder", StringComparison.OrdinalIgnoreCase) || cabUserChoice is null;

            if (cabArchiver)
            {
                var muiVerb = Registry.ClassesRoot.GetSubKeyValue("CABFolder\\Shell\\runas", "MUIVerb") as string ?? string.Empty;
                return muiVerb.Equals("@shell32.dll,-10210");
            }

            throw new CustomizationException(exceptionMessage: "A third-party archiver is set as the default archiver", resourceKey: "UIModel_Exception_CABInstallContext_DefaultArchiver");
        }

        /// <summary>
        /// Get "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        public static bool UseStoreOpenWith()
        {
            var storeOpenWith = Registry.CurrentUser.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith") as int?;
            return !storeOpenWith?.Equals(1) ?? true;
        }

        /// <summary>
        /// Get Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        public static bool OpenWindowsTerminalAdminContext()
        {
            if (AppxPackagesService.PackageExist("Microsoft.WindowsTerminal"))
            {
                var userBlockedGuid = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                var machineBlockedGuid = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");

                if (userBlockedGuid is null && machineBlockedGuid is null)
                {
                    try
                    {
                        var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";

                        // Windows Terminal creates the configuration file on its first launch. It will be created when the option is applied
                        if (!File.Exists(terminalSettings))
                        {
                            return false;
                        }

                        var jsonSettings = File.ReadAllText(terminalSettings, Encoding.UTF8);
                        var jsonProfile = Json.ToObject<MsTerminalSettings>(jsonSettings);
                        return jsonProfile?.Profiles?.Defaults?.Elevate ?? false;
                    }
                    catch (Exception ex) when (ex is ArgumentException or Newtonsoft.Json.JsonException or FileNotFoundException or DirectoryNotFoundException)
                    {
                        throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal configuration file is not valid", resourceKey: "UIModel_Exception_OpenWindowsTerminalAdminContext_ConfigurationNotValid");
                    }
                }

                // The Windows Terminal context menu shell extension is blocked
                return false;
            }

            throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled");
        }

        [System.Runtime.InteropServices.DllImport("advapi32.dll", SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        private static extern bool AuditQuerySystemPolicy(ref Guid subCategoryGuids, uint policyCount, out nint auditPolicy);

        [System.Runtime.InteropServices.DllImport("advapi32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.U1)]
        private static extern bool AuditFree(nint buffer);

        /// <summary>
        /// Determines whether success and failure auditing of the "Process Creation" subcategory is enabled.
        /// </summary>
        /// <returns>Auditing state or null if the audit policy cannot be queried.</returns>
        private static bool? ProcessCreationAuditIsEnabled()
        {
            const uint SuccessAndFailure = 0x3;
            var processCreation = new Guid("0CCE922B-69AE-11D9-BED3-505054503030");

            if (!AuditQuerySystemPolicy(ref processCreation, 1, out var auditPolicy))
            {
                return null;
            }

            try
            {
                // AUDIT_POLICY_INFORMATION: GUID AuditSubCategoryGuid (16 bytes), ULONG AuditingInformation, GUID AuditCategoryGuid
                var auditingInformation = (uint)System.Runtime.InteropServices.Marshal.ReadInt32(auditPolicy, 16);
                return (auditingInformation & SuccessAndFailure) == SuccessAndFailure;
            }
            finally
            {
                _ = AuditFree(auditPolicy);
            }
        }
    }
}
