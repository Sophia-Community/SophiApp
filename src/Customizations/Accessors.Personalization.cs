// <copyright file="Accessors.Personalization.cs" company="Team Sophia">
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
    /// The settings of the "Personalization" page.
    /// </content>
    public static partial class Accessors
    {
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

            throw new CustomizationException(exceptionMessage: "MicrosoftWindows.Client.WebExperience package is not installed", resourceKey: "UIModel_Exception_WidgetsNotInstalled");
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
        /// Get the state of unpinning all Start tiles. It is an action: there is nothing to read, so it is never chosen by itself.
        /// </summary>
        public static bool UnpinAllStartTiles()
        {
            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            return false;
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
        /// Get clock in notification center state.
        /// </summary>
        public static bool ShowClockInNotificationCenter()
        {
            var showClock = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowClockInNotificationCenter") as int? ?? -1;
            return showClock.Equals(1);
        }
    }
}
