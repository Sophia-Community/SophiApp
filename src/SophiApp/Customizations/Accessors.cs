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
        private static readonly IRedistributablePackageService RedistributablePackageService = App.GetService<IRedistributablePackageService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IHttpService HttpService = App.GetService<IHttpService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly IOneDriveService OneDriveService = App.GetService<IOneDriveService>();
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
            var diagTrackService = new System.ServiceProcess.ServiceController("DiagTrack");
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
            var allowTelemetry = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")?.GetValue("AllowTelemetry") as int? ?? -1;
            var telemetryAllowed = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")?.GetValue("MaxTelemetryAllowed") as int? ?? -1;
            var toastLevel = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")?.GetValue("ShowedToastAtLevel") as int? ?? -1;
            return allowTelemetry.Equals(1) && telemetryAllowed.Equals(1) && toastLevel.Equals(1) ? 2 : 1;
        }

        /// <summary>
        /// Get Windows feature "Error reporting" state.
        /// </summary>
        public static bool ErrorReporting()
        {
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting") ?? throw new InvalidOperationException("Failed to find a QueueReporting scheduled task");
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");
            return !(reportingTask.State == TaskState.Disabled && werService.StartType == ServiceStartMode.Disabled);
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
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser Exp"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\ProgramDataUpdater"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Autochk\\Proxy"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
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
            var arsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}";
            var optOut = Registry.LocalMachine.OpenSubKey(arsoPath)?.GetValue("OptOut") as int? ?? -1;
            return !optOut.Equals(0);
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
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var startIrisRecommendations = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("Start_IrisRecommendations") as int? ?? -1;
            return !startIrisRecommendations.Equals(0);
        }

        /// <summary>
        /// Get Start Menu notifications state.
        /// </summary>
        public static bool StartAccountNotifications()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var startAccountNotifications = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("Start_AccountNotifications") as int? ?? -1;
            return !startAccountNotifications.Equals(0);
        }

        /// <summary>
        /// Get the "This PC" icon on Desktop state.
        /// </summary>
        public static bool ThisPC()
        {
            var panelValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel")?.GetValue("{20D04FE0-3AEA-1069-A2D8-08002B30309D}") as int? ?? -1;
            return panelValue.Equals(0);
        }

        /// <summary>
        /// Get item check boxes state.
        /// </summary>
        public static bool CheckBoxes()
        {
            var checkValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("AutoCheckSelect") as int? ?? -1;
            return checkValue.Equals(1);
        }

        /// <summary>
        /// Get hidden files, folders, and drives state.
        /// </summary>
        public static bool HiddenItems()
        {
            var itemsValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("Hidden") as int? ?? -1;
            return itemsValue.Equals(1);
        }

        /// <summary>
        /// Get file name extensions visibility state.
        /// </summary>
        public static bool FileExtensions()
        {
            var extensionsValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("HideFileExt") as int? ?? -1;
            return extensionsValue.Equals(0);
        }

        /// <summary>
        /// Get folder merge conflicts state.
        /// </summary>
        public static bool MergeConflicts()
        {
            var mergeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("HideMergeConflicts") as int? ?? -1;
            return mergeValue.Equals(0);
        }

        /// <summary>
        /// Get how to open File Explorer.
        /// </summary>
        public static int OpenFileExplorerTo()
        {
            var fileValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("LaunchTo") as int? ?? -1;
            return fileValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get File Explorer compact mode state.
        /// </summary>
        public static bool FileExplorerCompactMode()
        {
            var compactModeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("UseCompactMode") as int? ?? -1;
            return !compactModeValue.Equals(0);
        }

        /// <summary>
        /// Get File Explorer provider notification visibility state.
        /// </summary>
        public static bool OneDriveFileExplorerAd()
        {
            var notificationsValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("ShowSyncProviderNotifications") as int? ?? -1;
            return !notificationsValue.Equals(0);
        }

        /// <summary>
        /// Get snap a window state.
        /// </summary>
        public static bool SnapAssist()
        {
            var snapValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("SnapAssist") as int? ?? -1;
            return !snapValue.Equals(0);
        }

        /// <summary>
        /// Get file transfer dialog box mode.
        /// </summary>
        public static int FileTransferDialog()
        {
            var modeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\OperationStatusManager")?.GetValue("EnthusiastMode") as int? ?? -1;
            return modeValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get recycle bin confirmation dialog state.
        /// </summary>
        public static bool RecycleBinDeleteConfirmation()
        {
            var shellValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer")?.GetValue("ShellState") as byte[] ?? new byte[5];
            return shellValue[4].Equals(51);
        }

        /// <summary>
        /// Get recently used Quick access files state.
        /// </summary>
        public static bool QuickAccessRecentFiles()
        {
            var recentValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer")?.GetValue("ShowRecent") as int? ?? -1;
            return !recentValue.Equals(0);
        }

        /// <summary>
        /// Get frequently used Quick access folders state.
        /// </summary>
        public static bool QuickAccessFrequentFolders()
        {
            var frequentValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer")?.GetValue("ShowFrequent") as int? ?? -1;
            return !frequentValue.Equals(0);
        }

        /// <summary>
        /// Get taskbar alignment state.
        /// </summary>
        public static int TaskbarAlignment()
        {
            var taskbarValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("TaskbarAl") as int? ?? -1;
            return taskbarValue.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get taskbar widgets icon state.
        /// </summary>
        public static bool TaskbarWidgets()
        {
            if (AppxPackagesService.PackageExist("MicrosoftWindows.Client.WebExperience"))
            {
                var taskbarValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("TaskbarDa") as int? ?? -1;
                return !taskbarValue.Equals(0);
            }

            throw new InvalidOperationException("AppX package MicrosoftWindows.Client.WebExperience is not installed");
        }

        /// <summary>
        /// Get Search on the taskbar state.
        /// </summary>
        public static int TaskbarSearch()
        {
            var smallIconsValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("TaskbarSmallIcons") as int? ?? -1;
            var searchModeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Search")?.GetValue("SearchboxTaskbarMode") as int? ?? -1;

            if (smallIconsValue.Equals(1))
            {
                throw new InvalidOperationException("Small taskbar icons mode is enabled");
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
        public static bool SearchHighlights()
        {
            var searchEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Search")?.GetValue("BingSearchEnabled") as int? ?? -1;
            var searchSuggestions = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\Explorer")?.GetValue("DisableSearchBoxSuggestions") as int? ?? -1;

            // Checking whether "Ask Copilot" and "Find results in Web" were disabled. They also disable Search Highlights automatically
            if (searchEnabled.Equals(1) || searchSuggestions.Equals(1))
            {
                throw new InvalidOperationException($"Search highlights already hidden.");
            }

            var dynamicSearch = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings")?.GetValue("IsDynamicSearchBoxEnabled") as int? ?? -1;
            return !dynamicSearch.Equals(0);
        }

        /// <summary>
        /// Get taskbar task view button state.
        /// </summary>
        public static bool TaskViewButton()
        {
            var buttonValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("ShowTaskViewButton") as int? ?? -1;
            return !buttonValue.Equals(0);
        }

        /// <summary>
        /// Get taskbar people icon state.
        /// </summary>
        public static bool PeopleTaskbar()
        {
            var peopleValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People")?.GetValue("PeopleBand") as int? ?? -1;
            return !peopleValue.Equals(0);
        }

        /// <summary>
        /// Get seconds on the taskbar clock state.
        /// </summary>
        public static bool SecondsInSystemClock()
        {
            var clockValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("ShowSecondsInSystemClock") as int? ?? -1;
            return clockValue.Equals(1);
        }

        /// <summary>
        /// Get taskbar combine state.
        /// </summary>
        public static int TaskbarCombine()
        {
            var levelValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("TaskbarGlomLevel") as int? ?? -1;
            return levelValue.Equals(-1) ? 1 : levelValue + 1;
        }

        /// <summary>
        /// Get end task in taskbar by click state.
        /// </summary>
        public static bool TaskbarEndTask()
        {
            var taskValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings")?.GetValue("TaskbarEndTask") as int? ?? -1;
            return taskValue.Equals(1);
        }

        /// <summary>
        /// Get Control Panel icons view state.
        /// </summary>
        public static int ControlPanelView()
        {
            var viewValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel")?.GetValue("AllItemsIconView") as int? ?? 0;
            var pageValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel")?.GetValue("StartupPage") as int? ?? 0;

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
            var themeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize")?.GetValue("SystemUsesLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get apps color mode state.
        /// </summary>
        public static int AppColorMode()
        {
            var themeValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Feeds")?.GetValue("AppsUseLightTheme") as int? ?? -1;
            return themeValue.Equals(0) ? 1 : 2;
        }

        /// <summary>
        /// Get first sign-in animation state.
        /// </summary>
        public static bool FirstLogonAnimation()
        {
            var logonValue = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon")?.GetValue("EnableFirstLogonAnimation") as int? ?? -1;
            return !logonValue.Equals(0);
        }

        /// <summary>
        /// Get JPEG wallpapers quality state.
        /// </summary>
        public static int JPEGWallpapersQuality()
        {
            var qualityValue = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop")?.GetValue("JPEGImportQuality") as int? ?? -1;
            return qualityValue.Equals(100) ? 1 : 2;
        }

        /// <summary>
        /// Get "- Shortcut" suffix state.
        /// </summary>
        public static bool ShortcutsSuffix()
        {
            var shortcutsValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates")?.GetValue("ShortcutNameTemplate") as string ?? string.Empty;
            return !shortcutsValue.Equals("%s.lnk");
        }

        /// <summary>
        /// Get Print screen button state.
        /// </summary>
        public static bool PrtScnSnippingTool()
        {
            var snippingValue = Registry.CurrentUser.OpenSubKey("Control Panel\\Keyboard")?.GetValue("PrintScreenKeyForSnippingEnabled") as int? ?? -1;
            return snippingValue.Equals(1);
        }

        /// <summary>
        /// Get input method for app window state.
        /// </summary>
        public static bool AppsLanguageSwitch() => PowerShellService.Invoke<bool>("$((Get-WinLanguageBarOption).IsLegacySwitchingMode)");

        /// <summary>
        /// Get Aero Shake state.
        /// </summary>
        public static bool AeroShaking()
        {
            var shakingValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("DisallowShaking") as int? ?? -1;
            return shakingValue.Equals(0);
        }

        /// <summary>
        /// Get "Windows 11 Cursors Concept" from Jepri Creations state.
        /// </summary>
        public static int Cursors()
        {
            var cursors = Registry.CurrentUser.OpenSubKey("Control Panel\\Cursors")?.GetValue(string.Empty) as string ?? string.Empty;
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
            var groupByValue = Registry.CurrentUser.OpenSubKey(groupByPath)?.GetValue("GroupBy") as string ?? string.Empty;
            return groupByValue.Equals("System.Null") ? 1 : 2;
        }

        /// <summary>
        /// Get navigation pane expand state.
        /// </summary>
        public static bool NavigationPaneExpand()
        {
            var paneValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("NavPaneExpandToCurrentFolder") as int? ?? -1;
            return !paneValue.Equals(0);
        }

        /// <summary>
        /// Get Start menu recently added apps state.
        /// </summary>
        public static bool RecentlyAddedStartApps()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var showRecentList = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Start")?.GetValue("ShowRecentList") as int? ?? -1;
            return !showRecentList.Equals(0);
        }

        /// <summary>
        /// Get Start all apps view state.
        /// </summary>
        public static int StartAppsView()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var appsView = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Start")?.GetValue("AllAppsViewMode") as int? ?? 0;
            return appsView + 1;
        }

        /// <summary>
        /// Get most used apps in Start.
        /// </summary>
        public static bool MostUsedStartApps()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var usedStartApps = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Start")?.GetValue("ShowFrequentList") as int? ?? -1;
            return !usedStartApps.Equals(0);
        }

        /// <summary>
        /// Get Start menu layout state.
        /// </summary>
        public static int StartLayout()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            // Default — 0
            // Show More Pins — 1
            // Show More Recommendations — 2
            var startLayout = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")?.GetValue("Start_Layout") as int? ?? 0;
            return startLayout + 1;
        }

        /// <summary>
        /// Get recommended section state.
        /// </summary>
        public static bool StartRecommendedSection()
        {
            if (ProcessService.Exist("Start11Srv", "StartAllBackCfg", "StartMenu"))
            {
                throw new InvalidOperationException("A third-party Start Menu is installed");
            }

            var startRecommended = new List<int>()
            {
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Start", false)?.GetValue("ShowRecentList") as int? ?? -1,
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Start", false)?.GetValue("ShowFrequentList") as int? ?? -1,
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", false)?.GetValue("Start_IrisRecommendations") as int? ?? -1,
                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", false)?.GetValue("Start_TrackDocs") as int? ?? -1,
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
                if (Path.Exists(OneDriveService.GetUserDataFolderOrDefault()))
                {
                    if (OneDriveService.UserIsLogged())
                    {
                        throw new InvalidOperationException("Please log out from OneDrive account before uninstalling the application");
                    }

                    return true;
                }

                throw new InvalidOperationException("A user data folder does not exist");
            }
            else
            {
                var setupFile = OneDriveService.GetSetupFileOrDefault();
                App.Logger.LogOneDriveSetupFile(setupFile);

                if (!string.IsNullOrEmpty(setupFile) || HttpService.UrlIsAvailable("https://g.live.com/1rewlive5skydrive/OneDriveProductionV2"))
                {
                    return false;
                }

                throw new InvalidOperationException("OneDriveSetup.exe was not found and there is no Internet access to download OneDrive installer");
            }
        }

        /// <summary>
        /// Get storage sense state.
        /// </summary>
        public static bool StorageSense()
        {
            var storagePolicy01 = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy")?.GetValue("01") as int? ?? -1;
            var storagePolicy04 = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy")?.GetValue("04") as int? ?? -1;
            var storagePolicy2048 = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy")?.GetValue("2048") as int? ?? -1;
            return storagePolicy01.Equals(1) && storagePolicy04.Equals(1) && storagePolicy2048.Equals(30);
        }

        /// <summary>
        /// Get hibernation state.
        /// </summary>
        public static bool Hibernation()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Power")
                ?.GetValue("HibernateEnabled") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get long path limit state.
        /// </summary>
        public static bool Win32LongPathsSupport()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\FileSystem")?.GetValue("LongPathsEnabled") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get BSOD error state.
        /// </summary>
        public static bool BSoDStopError()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\CrashControl")?.GetValue("DisplayParameters") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get administrator approval mode state.
        /// </summary>
        public static int AdminApprovalMode()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System")?.GetValue("ConsentPromptBehaviorAdmin") as int? ?? -1;
            return isEnabled.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get delivery optimization state.
        /// </summary>
        public static bool DeliveryOptimization()
        {
            var isEnabled = Registry.Users.OpenSubKey("S-1-5-20\\Software\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Settings")?.GetValue("DownloadMode") as int? ?? -1;
            return !isEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows manage default printer state.
        /// </summary>
        public static bool WindowsManageDefaultPrinter()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows")?.GetValue("LegacyDefaultPrinterMode") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get update Microsoft products state.
        /// </summary>
        public static bool UpdateMicrosoftProducts()
        {
            return UpdateService.HasMicrosoftProductsUpdate();
        }

        /// <summary>
        /// Get restart notification state.
        /// </summary>
        public static bool RestartNotification()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings")?.GetValue("RestartNotificationsAllowed2") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get device restart after update state.
        /// </summary>
        public static bool RestartDeviceAfterUpdate()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings")?.GetValue("IsExpedited") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get active hours restart state.
        /// </summary>
        public static int ActiveHours()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings")?.GetValue("SmartActiveHoursState") as int? ?? -1;
            return isEnabled.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Get Windows latest update state.
        /// </summary>
        public static bool WindowsLatestUpdate()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsUpdate\\UX\\Settings")?.GetValue("IsContinuousInnovationOptedIn") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get network adapters power state.
        /// </summary>
        public static bool NetworkAdaptersSavePower()
        {
            return PowerShellService.TurnOffDeviceNetworkAdapterExist()
                ?? throw new InvalidOperationException("There is no network adapter which has an AllowComputerToTurnOffDevice property");
        }

        /// <summary>
        /// Get input method state.
        /// </summary>
        public static int InputMethod()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\User Profile")?.GetValue("InputMethodOverride") as string ?? string.Empty;
            return isEnabled.Equals("0409:00000409") ? 1 : 2;
        }

        /// <summary>
        /// Get Print Screen folder state.
        /// </summary>
        public static int WinPrtScrFolder()
        {
            if (OneDriveService.UserIsLogged())
            {
                throw new InvalidOperationException("Please log out from OneDrive account");
            }

            var prtScrPath = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders")
                ?.GetValue("{B7BEDE81-DF94-4682-A7D8-57A52620B86F}") as string ?? string.Empty;
            var desktopPath = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders")
                ?.GetValue("Desktop") as string;
            return prtScrPath.Equals(desktopPath) ? 1 : 2;
        }

        /// <summary>
        /// Get recommended troubleshooting state.
        /// </summary>
        public static int RecommendedTroubleshooting()
        {
            var userPreference = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\WindowsMitigation")?.GetValue("UserPreference") as int? ?? -1;
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
            var isEnabled = Registry.CurrentUser.OpenSubKey("Software\\Classes\\Typelib\\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\\1.0\\0\\win64")?.GetValue(string.Empty) as string;
            return isEnabled is null;
        }

        /// <summary>
        /// Get Num Lock state.
        /// </summary>
        public static bool NumLock()
        {
            var isEnabled = Registry.Users.OpenSubKey(".DEFAULT\\Control Panel\\Keyboard")?.GetValue("InitialKeyboardIndicators") as string ?? string.Empty;
            return isEnabled.Equals("2147483650");
        }

        /// <summary>
        /// Get Caps Lock state.
        /// </summary>
        public static bool CapsLock()
        {
            var scancodeMap = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 58, 0, 0, 0, 0, 0 };
            var isEnabled = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Keyboard Layout")?.GetValue("Scancode Map") as byte[] ?? [];
            return !isEnabled.SequenceEqual(scancodeMap);
        }

        /// <summary>
        /// Get sticky shift state.
        /// </summary>
        public static bool StickyShift()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Control Panel\\Accessibility\\StickyKeys")?.GetValue("Flags") as string ?? string.Empty;
            return !isEnabled.Equals("506");
        }

        /// <summary>
        /// Get autoplay state.
        /// </summary>
        public static bool Autoplay()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers")?.GetValue("DisableAutoplay") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get thumbnail cache state.
        /// </summary>
        public static bool ThumbnailCacheRemoval()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache")?.GetValue("Autorun") as int? ?? -1;
            var isEnabledWow = Registry.LocalMachine.OpenSubKey("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache")?.GetValue("Autorun") as int? ?? -1;
            return !(isEnabled.Equals(0) && isEnabledWow.Equals(0));
        }

        /// <summary>
        /// Get save restartable apps state.
        /// </summary>
        public static bool SaveRestartableApps()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon")?.GetValue("RestartApps") as int? ?? -1;
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

            return activePlan.InstanceID.Contains("{8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c}") ? 1 : 2;
        }

        /// <summary>
        /// Get registry backup state.
        /// </summary>
        public static bool RegistryBackup()
        {
            var isEnabled = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Session Manager\\Configuration Manager")
                ?.GetValue("EnablePeriodicBackup") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get restore previous folders state.
        /// </summary>
        public static bool RestorePreviousFolders()
        {
            var isEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")
                ?.GetValue("PersistBrowsers") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get Windows Terminal default app state.
        /// </summary>
        public static int DefaultTerminalApp()
        {
            var appxPackage = AppxPackagesService.GetPackages()
                .FirstOrDefault(package => package.Id.Name.Equals("Microsoft.WindowsTerminal"));

            if (appxPackage is not null)
            {
                var requiredVersion = new PackageVersion(1, 11, 0, 0);

                if (appxPackage.Id.Version.Major >= requiredVersion.Major
                    && appxPackage.Id.Version.Minor >= requiredVersion.Minor)
                {
                    var appxPath = $"Software\\Classes\\PackagedCom\\Package\\{appxPackage.Id.FullName}\\Class";
                    var consoleId = string.Empty;
                    var consolePath = "Console\\%%Startup";
                    var terminalId = string.Empty;
                    Registry.LocalMachine.OpenSubKey(appxPath)?.GetSubKeyNames()
                        .ForEach(key =>
                        {
                            switch (Registry.LocalMachine.OpenSubKey(Path.Combine(appxPath, key))?.GetValue("ServerId") ?? -1)
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
                    var delegationConsole = Registry.CurrentUser.OpenSubKey(consolePath)?.GetValue("DelegationConsole") as string ?? string.Empty;
                    var delegationTerminal = Registry.CurrentUser.OpenSubKey(consolePath)?.GetValue("DelegationTerminal") as string ?? string.Empty;
                    return delegationConsole.Equals(consoleId) && delegationTerminal.Equals(terminalId) ? 1 : 2;
                }

                throw new InvalidOperationException($"Unsupported Windows Terminal version: {appxPackage.Id.Version.Major}.{appxPackage.Id.Version.Minor} required version {requiredVersion.Major}.{requiredVersion.Minor} or above");
            }

            throw new InvalidOperationException("AppX package Windows Terminal is not installed");
        }

        /// <summary>
        /// Get clock in notification center state.
        /// </summary>
        public static bool ShowClockInNotificationCenter()
        {
            var showClock = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced")
                ?.GetValue("ShowClockInNotificationCenter", false) as int? ?? -1;
            return showClock.Equals(1);
        }

        /// <summary>
        /// Get .NET 8 desktop runtime version.
        /// </summary>
        public static bool InstallDotNetRuntime_8()
        {
            var latestVersion = DataService.LatestReleaseNET8?.Version ?? throw new InvalidOperationException("Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/8.0/releases.json is not available");
            var installedVersion = RedistributablePackageService.GetInstalledPackageVersionOrDefault($"windowsdesktop-runtime-{latestVersion}-win-x64.exe");
            return latestVersion > installedVersion ? false : throw new InvalidOperationException("Latest .NET version is installed");
        }

        /// <summary>
        /// Get .NET 9 desktop runtime version.
        /// </summary>
        public static bool InstallDotNetRuntime_9()
        {
            var latestVersion = DataService.LatestReleaseNET9?.Version ?? throw new InvalidOperationException("Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/9.0/releases.json is not available");
            var installedVersion = RedistributablePackageService.GetInstalledPackageVersionOrDefault($"windowsdesktop-runtime-{latestVersion}-win-x64.exe");
            return latestVersion > installedVersion ? false : throw new InvalidOperationException("Latest .NET version is installed");
        }

        /// <summary>
        /// Get .NET 10 desktop runtime version.
        /// </summary>
        public static bool InstallDotNetRuntime_10()
        {
            var latestVersion = DataService.LatestReleaseNET10?.Version ?? throw new InvalidOperationException("Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/10.0/releases.json is not available");
            var installedVersion = RedistributablePackageService.GetInstalledPackageVersionOrDefault($"windowsdesktop-runtime-{latestVersion}-win-x64.exe");
            return latestVersion > installedVersion ? false : throw new InvalidOperationException("Latest .NET version is installed");
        }

        /// <summary>
        /// Get Microsoft Visual C++ x86 redistributable package version.
        /// </summary>
        public static bool InstallVisualC_x86()
        {
            var latestVersion = DataService.LatestReleaseVC?.Version ?? throw new InvalidOperationException("Url https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json is not available");
            var installedVersion = RedistributablePackageService.GetInstalledPackageVersionOrDefault("VC_redist.x86.exe");
            return latestVersion > installedVersion ? false : throw new InvalidOperationException("Latest Visual C++ x86 version is installed");
        }

        /// <summary>
        /// Get Microsoft Visual C++ x64 redistributable package version.
        /// </summary>
        public static bool InstallVisualC_x64()
        {
            var latestVersion = DataService.LatestReleaseVC?.Version ?? throw new InvalidOperationException("Url https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json is not available");
            var installedVersion = RedistributablePackageService.GetInstalledPackageVersionOrDefault("VC_redist.x64.exe");
            return latestVersion > installedVersion ? false : throw new InvalidOperationException("Latest Visual C++ x64 version is installed");
        }

        /// <summary>
        /// Get Windows AI state.
        /// </summary>
        public static bool RemoveWindowsAI()
        {
            var recall = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\Notifications\\OptionalFeatures\\Recall")?.GetValue("Selection") as int? ?? -1;
            return !recall.Equals(0) && AppxPackagesService.PackageExist("Microsoft.Copilot");
        }

        /// <summary>
        /// Gets Xbox game bar state.
        /// </summary>
        public static bool XboxGameBar()
        {
            var appCaptureIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR")
                ?.GetValue("AppCaptureEnabled") as int? ?? -1;
            var dvrIsEnabled = Registry.CurrentUser.OpenSubKey("System\\GameConfigStore")
                ?.GetValue("GameDVR_Enabled") as int? ?? -1;

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
            if (AppxPackagesService.PackageExist("Microsoft.GamingApp"))
            {
                var startupPanelIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\GameBar")?.GetValue("ShowStartupPanel") as int? ?? -1;
                return startupPanelIsEnabled == 1;
            }

            throw new InvalidOperationException("AppX package Microsoft.GamingApp is not installed");
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
            var wddmVersion = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\GraphicsDrivers\\FeatureSetUsage")?.GetValue("WddmVersion_Min") as int? ?? -1;

            // Checking whether a WDDM verion is 2.7 or higher
            if (isExternalDAC && !isVM && wddmVersion >= WDDMMinimalVersion)
            {
                var hwSchMode = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\GraphicsDrivers")?.GetValue("HwSchMode") as int? ?? -1;
                return hwSchMode == 2;
            }

            throw new InvalidOperationException($"DAC type is external: {isExternalDAC}. PC is a VM: {isVM}. WDDM version (minimal {WDDMMinimalVersion}): {wddmVersion}");
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
            return cleanupTaskExist && notificationTaskExist && cleanupPsExist && notificationPsExist && (cleanupXmlArguments?.InnerText.Equals(cleanupArgumentsPattern) ?? false) && (notificationXmlArguments?.InnerText?.Equals(notificationTaskPattern) ?? false);
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
            return distributionTaskExist && distributionPsExist && (distributionXmlArguments?.InnerText.Equals(distributionTaskPattern) ?? false);
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
            return tempTaskExist && tempPsExist && (tempXmlArguments?.InnerText.Equals(tempTaskPattern) ?? false);
        }

        /// <summary>
        /// Get Windows network protection state.
        /// </summary>
        public static bool NetworkProtection()
        {
            var defenderEnabled = DataService.DefenderEnabled;
            var antiSpywareEnabled = InstrumentationService.GetAntiSpywareEnabled();

            if (defenderEnabled && antiSpywareEnabled)
            {
                var networkProtection = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows Defender\\Windows Defender Exploit Guard\\Network Protection")?.GetValue("EnableNetworkProtection") as int? ?? -1;
                return networkProtection.Equals(1);
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows PUApps detection state.
        /// </summary>
        public static bool PUAppsDetection()
        {
            var defenderEnabled = DataService.DefenderEnabled;
            var antiSpywareEnabled = InstrumentationService.GetAntiSpywareEnabled();

            if (defenderEnabled && antiSpywareEnabled)
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
            var defenderEnabled = DataService.DefenderEnabled;
            var antiSpywareEnabled = InstrumentationService.GetAntiSpywareEnabled();

            if (defenderEnabled && antiSpywareEnabled)
            {
                return ProcessService.Exist("MsMpEngCP");
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows event viewer custom view state.
        /// </summary>
        public static bool EventViewerCustomView()
        {
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
            var inclusionSettingEnabled = PowerShellService.Invoke<bool>(auditPolicyScript);
            var processCreationEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit")?.GetValue("ProcessCreationIncludeCmdLine_Enabled") as int? ?? -1;
            var eventXml = XmlService.TryLoad(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml"));
            var eventQueryCount = eventXml?.SelectNodes("//QueryList/Query/Select")?.Count ?? -1;
            var moduleLoggingEnabled = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging")?.GetValue("EnableModuleLogging") as int? ?? -1;
            var powerShellAsterisk = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames")?.GetValue("*") as string ?? string.Empty;
            var scriptLoggingEnabled = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging")?.GetValue("EnableScriptBlockLogging") as int? ?? -1;
            return inclusionSettingEnabled && processCreationEnabled.Equals(1) && eventQueryCount.Equals(3) && moduleLoggingEnabled.Equals(1) && powerShellAsterisk.Equals("*") && scriptLoggingEnabled.Equals(1);
        }

        /// <summary>
        /// Get Windows SmartScreen state.
        /// </summary>
        public static bool AppsSmartScreen()
        {
            var defenderEnabled = DataService.DefenderEnabled;
            var antiSpywareEnabled = InstrumentationService.GetAntiSpywareEnabled();

            if (defenderEnabled && antiSpywareEnabled)
            {
                var smartScreenIsEnabled = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer")?.GetValue("SmartScreenEnabled") as string ?? string.Empty;
                return !smartScreenIsEnabled.Equals("Off");
            }

            throw new InvalidOperationException("Microsoft Defender antispyware protection is disabled");
        }

        /// <summary>
        /// Get Windows save zone state.
        /// </summary>
        public static bool SaveZoneInformation()
        {
            var saveZoneInformation = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments")?.GetValue("SaveZoneInformation") as int? ?? -1;
            return saveZoneInformation.Equals(1);
        }

        /// <summary>
        /// Get Windows Sandbox state.
        /// </summary>
        public static bool WindowsSandbox()
        {
            bool GetSandboxEnabled()
            {
                // Checking whether x86 virtualization is enabled in the firmware
                var sandboxScript = "Get-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -Online";
                var sandboxState = PowerShellService.Invoke(sandboxScript).FirstOrDefault();
                return !sandboxState?.Properties["State"]?.Value.Equals("Disabled") ?? throw new InvalidOperationException("Windows Sandbox state undefined");
            }

            if (DataService.OsProperties.Edition.Equals("Professional") || DataService.OsProperties.Edition.Equals("Enterprise"))
            {
                // Determining whether Hyper-V is enabled
                var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationFirmwareIsEnabled() ?? throw new InvalidOperationException("This CPU does not support virtualization");
                var hypervisorPresent = InstrumentationService.HypervisorIsPresent() ?? throw new InvalidOperationException("Enable virtualization in UEFI");

                if (virtualizationIsEnabled)
                {
                    return GetSandboxEnabled();
                }
                else if (hypervisorPresent)
                {
                    return GetSandboxEnabled();
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
            var cabProgId = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice")?.GetValue("ProgId") as string ?? string.Empty;
            var cabUserChoice = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice")?.GetSubKeyNames() ?? [];
            var explorerDefaultArchiver = cabProgId.Equals("CABFolder") || cabUserChoice.Length == 0;

            if (explorerDefaultArchiver)
            {
                var muiVerb = Registry.ClassesRoot.OpenSubKey("CABFolder\\Shell\\runas")?.GetValue("MUIVerb") as string ?? string.Empty;
                return muiVerb.Equals("@shell32.dll,-10210");
            }

            throw new InvalidOperationException("A third-party archiver is set as the default archiver");
        }

        /// <summary>
        /// Get "Edit With Clipchamp" item in the media files context menu state.
        /// </summary>
        public static bool EditWithClipchampContext()
        {
            if (AppxPackagesService.PackageExist("Clipchamp.Clipchamp"))
            {
                var userClipchamp = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{8AB635F8-9A67-4698-AB99-784AD929F3B4}");
                var machineClipchamp = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{8AB635F8-9A67-4698-AB99-784AD929F3B4}");
                return userClipchamp is null && machineClipchamp is null;
            }

            throw new InvalidOperationException("AppX package Clipchamp.Clipchamp is not installed");
        }

        /// <summary>
        /// Get "Edit With Photos" item in the media files context menu state.
        /// </summary>
        public static bool EditWithPhotosContext()
        {
            if (AppxPackagesService.PackageExist("Microsoft.Windows.Photos"))
            {
                var userPhotosContext = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{8AB635F8-9A67-4698-AB99-784AD929F3B4}");
                var machinePhotosContext = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{8AB635F8-9A67-4698-AB99-784AD929F3B4}");
                return userPhotosContext is null && machinePhotosContext is null;
            }

            throw new InvalidOperationException("AppX package Microsoft.Windows.Photos is not installed");
        }

        /// <summary>
        /// Get "Edit With Paint Context" item in the media files context menu state.
        /// </summary>
        public static bool EditWithPaintContext()
        {
            if (AppxPackagesService.PackageExist("Microsoft.Paint"))
            {
                var paintContext = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{2430F218-B743-4FD6-97BF-5C76541B4AE9}");
                return paintContext is null;
            }

            throw new InvalidOperationException("AppX package Microsoft.Paint is not installed");
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
        /// Get Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        public static bool OpenWindowsTerminalAdminContext()
        {
            var appxTerminal = "Microsoft.WindowsTerminal";

            if (AppxPackagesService.PackageExist(appxTerminal))
            {
                var userBlockedGuid = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{9F156763-7844-4DC4-B2B1-901F640F5155}");
                var machineBlockedGuid = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked")?.GetValue("{9F156763-7844-4DC4-B2B1-901F640F5155}");

                if (userBlockedGuid is null && machineBlockedGuid is null)
                {
                    try
                    {
                        var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";
                        var jsonSettings = File.ReadAllText(terminalSettings, Encoding.UTF8);
                        var jsonProfile = Json.ToObject<MsTerminalSettings>(jsonSettings);
                        return jsonProfile?.Profiles?.Defaults?.Elevate ?? false;
                    }
                    catch (ArgumentException)
                    {
                        throw new InvalidOperationException($"{appxTerminal} configuration file is not valid");
                    }
                }

                return true;
            }

            throw new InvalidOperationException($"AppX package {appxTerminal} is not installed");
        }
    }
}
