// <copyright file="Mutators.Personalization.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.Json.Nodes;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <content>
    /// The settings of the "Personalization" page.
    /// </content>
    public static partial class Mutators
    {
        /// <summary>
        /// Set Start menu recommendations state.
        /// </summary>
        /// <param name="enable">Start menu recommendations state.</param>
        public static void StartRecommendationsTips(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations");
                return;
            }

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations", 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start Menu notifications state.
        /// </summary>
        /// <param name="enable">Start Menu notifications state.</param>
        public static void StartAccountNotifications(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_AccountNotifications");
                return;
            }

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_AccountNotifications", 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the "This PC" icon on Desktop state.
        /// </summary>
        /// <param name="enable">"This PC" icon state.</param>
        public static void ThisPC(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel", "{20D04FE0-3AEA-1069-A2D8-08002B30309D}", 0, RegistryValueKind.DWord);

                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel", "{20D04FE0-3AEA-1069-A2D8-08002B30309D}");
        }

        /// <summary>
        /// Set item check boxes state.
        /// </summary>
        /// <param name="enable">Item check boxes state.</param>
        public static void CheckBoxes(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "AutoCheckSelect", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set hidden files, folders, and drives state.
        /// </summary>
        /// <param name="enable">Hidden items state.</param>
        public static void HiddenItems(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Hidden", enable ? 1 : 2, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set file name extensions visibility state.
        /// </summary>
        /// <param name="enable">File extensions visibility state.</param>
        public static void FileExtensions(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideFileExt", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set folder merge conflicts state.
        /// </summary>
        /// <param name="enable">Folder merge conflicts state.</param>
        public static void MergeConflicts(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "HideMergeConflicts", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set how to open File Explorer.
        /// </summary>
        /// <param name="state">File Explorer open state.</param>
        public static void OpenFileExplorerTo(int state)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "LaunchTo", state, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set File Explorer compact mode state.
        /// </summary>
        /// <param name="enable">File Explorer compact mode state.</param>
        public static void FileExplorerCompactMode(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "UseCompactMode", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set File Explorer provider notification visibility state.
        /// </summary>
        /// <param name="enable">File Explorer provider notification visibility state.</param>
        public static void OneDriveFileExplorerAd(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSyncProviderNotifications", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set snap a window state.
        /// </summary>
        /// <param name="enable">Snap Assist state.</param>
        public static void SnapAssist(bool enable)
        {
            // Property type is string
            Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\Desktop", "WindowArrangementActive", "1", RegistryValueKind.String);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "SnapAssist", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set file transfer dialog box mode.
        /// </summary>
        /// <param name="state">File transfer dialog box state.</param>
        public static void FileTransferDialog(int state)
        {
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\OperationStatusManager", "EnthusiastMode", state.Equals(1) ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set recycle bin confirmation dialog state.
        /// </summary>
        /// <param name="enable">Recycle bin dialog state.</param>
        public static void RecycleBinDeleteConfirmation(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "ConfirmFileDelete", Registry.LocalMachine, Registry.CurrentUser);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "ConfirmFileDelete", LGPOScope.Computer, LGPOScope.User);

            using var explorerKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", true);

            if (explorerKey?.GetValue("ShellState") is byte[] shellState && shellState.Length > 4)
            {
                // The 0x04 bit of the fifth byte turns the delete confirmation dialog off
                shellState[4] = enable ? (byte)(shellState[4] & ~0x04) : (byte)(shellState[4] | 0x04);
                explorerKey.SetValue("ShellState", shellState, RegistryValueKind.Binary);
            }
        }

        /// <summary>
        /// Set recently used Quick access files state.
        /// </summary>
        /// <param name="enable">Quick access files state.</param>
        public static void QuickAccessRecentFiles(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRecentDocsHistory");
            GroupPolicyService.DeleteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRecentDocsHistory");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRecentDocsHistory", LGPOScope.Computer, LGPOScope.User);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "ShowRecent", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set frequently used Quick access folders state.
        /// </summary>
        /// <param name="enable">Quick access folders state.</param>
        public static void QuickAccessFrequentFolders(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "ShowFrequent", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar position.
        /// </summary>
        /// <param name="state">Taskbar position: bottom, top, left or right.</param>
        public static void TaskbarPosition(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableTaskbarPosition", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableTaskbarPosition", LGPOScope.User, LGPOScope.Computer);

            var location = state switch
            {
                2 => 1,
                3 => 0,
                4 => 2,
                _ => 3,
            };

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarLocation", location, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar alignment state.
        /// </summary>
        /// <param name="state">Taskbar alignment state.</param>
        public static void TaskbarAlignment(int state)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarAl", state.Equals(1) ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar widgets icon state.
        /// </summary>
        /// <param name="enable">Taskbar widgets icon state.</param>
        public static void TaskbarWidgets(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\PolicyManager\\default\\NewsAndInterests\\AllowNewsAndInterests", "value");
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Dsh", "AllowNewsAndInterests");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Dsh", "AllowNewsAndInterests");
            // We cannot set a value to TaskbarDa via any API, except by copying reg.exe under a different name,
            // due to a UCPD driver tracks all executables to block the access to the registry
            RegistryService.SetRegistryValueBypassUCPD("HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarDa", enable ? 1 : 0);
        }

        /// <summary>
        /// Set Search on the taskbar state.
        /// </summary>
        /// <param name="state">Taskbar search state.</param>
        public static void TaskbarSearch(int state)
        {
            GroupPolicyService.SetRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\PolicyManager\\default\\Search\\DisableSearch", "value", 0, RegistryValueKind.DWord);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\Windows Search", "DisableSearch", "SearchOnTaskbarMode");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.RemovePolicies(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\Windows Search", "DisableSearch", "SearchOnTaskbarMode");

            var searchMode = state switch
            {
                3 => 3,
                4 => 2,
                _ => state - 1,
            };

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Search", "SearchboxTaskbarMode", searchMode, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set search highlights state.
        /// </summary>
        /// <param name="enable">Search highlights state.</param>
        public static void SearchHighlights(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\Windows Search", "EnableDynamicContentInWSB");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\Windows Search", "EnableDynamicContentInWSB");

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled");
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
            }
            else
            {
                var bingSearchEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Search", "BingSearchEnabled") as int? ?? -1;
                var searchSuggestionsDisabled = Registry.CurrentUser.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions") as int? ?? -1;

                // Search highlights are already hidden if Bing search is disabled
                if (bingSearchEnabled.Equals(0) || searchSuggestionsDisabled.Equals(1))
                {
                    return;
                }
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings", "IsDynamicSearchBoxEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar task view button state.
        /// </summary>
        /// <param name="enable">Taskbar task view button state.</param>
        public static void TaskViewButton(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideTaskViewButton", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideTaskViewButton", LGPOScope.User, LGPOScope.Computer);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowTaskViewButton", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set seconds on the taskbar clock state.
        /// </summary>
        /// <param name="enable">Seconds on the taskbar clock state.</param>
        public static void SecondsInSystemClock(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowSecondsInSystemClock", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar combine state.
        /// </summary>
        /// <param name="state">Taskbar combine state.</param>
        public static void TaskbarCombine(int state)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTaskGrouping", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoTaskGrouping", LGPOScope.Computer, LGPOScope.User);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "TaskbarGlomLevel", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set end task in taskbar by click state.
        /// </summary>
        /// <param name="enable">Taskbar end task state.</param>
        public static void TaskbarEndTask(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings", "TaskbarEndTask", 1, RegistryValueKind.DWord);

                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings", "TaskbarEndTask");
        }

        /// <summary>
        /// Set Control Panel icons view state.
        /// </summary>
        /// <param name="state">Control Panel icons view state.</param>
        public static void ControlPanelView(int state)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "ForceClassicControlPanel");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "ForceClassicControlPanel");

            switch (state)
            {
                case 1:
                    Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "AllItemsIconView", 0, RegistryValueKind.DWord);
                    Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "StartupPage", 0, RegistryValueKind.DWord);

                    break;
                case 2:
                    Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "AllItemsIconView", 0, RegistryValueKind.DWord);
                    Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "StartupPage", 1, RegistryValueKind.DWord);

                    break;
                default:
                    Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "AllItemsIconView", 1, RegistryValueKind.DWord);
                    Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel", "StartupPage", 1, RegistryValueKind.DWord);

                    break;
            }
        }

        /// <summary>
        /// Set Windows color mode state.
        /// </summary>
        /// <param name="state">Windows color mode state.</param>
        public static void WindowsColorMode(int state)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "SystemUsesLightTheme", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set apps color mode state.
        /// </summary>
        /// <param name="state">Apps color mode state.</param>
        public static void AppColorMode(int state)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set first sign-in animation state.
        /// </summary>
        /// <param name="enable">First sign-in animation state.</param>
        public static void FirstLogonAnimation(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "EnableFirstLogonAnimation");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "EnableFirstLogonAnimation");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "EnableFirstLogonAnimation", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set JPEG wallpapers quality state.
        /// </summary>
        /// <param name="state">JPEG wallpapers quality state.</param>
        public static void JPEGWallpapersQuality(int state)
        {
            if (state.Equals(1))
            {
                Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\Desktop", "JPEGImportQuality", 100, RegistryValueKind.DWord);

                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Control Panel\\Desktop", "JPEGImportQuality");
        }

        /// <summary>
        /// Set "- Shortcut" suffix state.
        /// </summary>
        /// <param name="enable">"- Shortcut" suffix state.</param>
        public static void ShortcutsSuffix(bool enable)
        {
            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "link");

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates", "ShortcutNameTemplate");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates", "ShortcutNameTemplate", "%s.lnk", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Aero Shake state.
        /// </summary>
        /// <param name="enable">Aero Shake state.</param>
        public static void AeroShaking(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoWindowMinimizingShortcuts", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoWindowMinimizingShortcuts", LGPOScope.User, LGPOScope.Computer);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "DisallowShaking", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "Windows 11 Cursors Concept" from Jepri Creations state.
        /// </summary>
        /// <param name="state">Cursors state.</param>
        public static void Cursors(int state)
        {
            switch (state)
            {
                case 1:
                    SetJepriCreationsCursors(JepriCursorsTheme.Dark);
                    break;

                case 2:
                    SetJepriCreationsCursors(JepriCursorsTheme.Light);
                    break;

                default:
                    CursorsService.SetDefaultCursors();
                    break;
            }

            CursorsService.ReloadCursors();
        }

        /// <summary>
        /// Set files and folders grouping state.
        /// </summary>
        /// <param name="state">Files and folders grouping state.</param>
        public static void FolderGroupBy(int state)
        {
            var folderPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderTypes\\{885a186e-a440-4ada-812b-db871b942259}\\TopViews\\{00000000-0000-0000-0000-000000000000}";
            if (state.Equals(1))
            {
                // Clear any Common Dialog views
                RegistryService.ClearCommonDialogViews();
                // https://learn.microsoft.com/en-us/windows/win32/properties/props-system-null
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "ColumnList", "System.Null", RegistryValueKind.String);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "GroupBy", "System.Null", RegistryValueKind.String);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "LogicalViewMode", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "Name", "NoName", RegistryValueKind.String);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "Order", 0, RegistryValueKind.DWord);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "PrimaryProperty", "System.ItemNameDisplay", RegistryValueKind.String);
                Registry.CurrentUser.CreateKeyAndSetValue(folderPath, "SortByList", "prop:System.ItemNameDisplay", RegistryValueKind.String);
                return;
            }

            Registry.CurrentUser.DeleteSubKeyTree("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderTypes\\{885a186e-a440-4ada-812b-db871b942259}", false);
        }

        /// <summary>
        /// Set navigation pane expand state.
        /// </summary>
        /// <param name="enable">Navigation pane expand state.</param>
        public static void NavigationPaneExpand(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "NavPaneExpandToCurrentFolder", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu recently added apps state.
        /// </summary>
        /// <param name="enable">Start menu recently added apps state.</param>
        public static void RecentlyAddedStartApps(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecentlyAddedApps", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecentlyAddedApps", LGPOScope.User, LGPOScope.Computer);

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList");
                return;
            }

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList", 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Unpin all Start tiles.
        /// </summary>
        /// <param name="enable">True to unpin the tiles. It is an action, so false does nothing.</param>
        public static void UnpinAllStartTiles(bool enable)
        {
            if (!enable)
            {
                return;
            }

            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMorePrograms", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMorePrograms", LGPOScope.User, LGPOScope.Computer);

            if (StartMenuHelper.ThirdPartyMenuExists())
            {
                throw new CustomizationException(exceptionMessage: "A third-party Start Menu is installed", resourceKey: "UIModel_Exception_StartMenuInstalled");
            }

            // https://learn.microsoft.com/en-us/windows/configuration/start/layout
            var emptyLayout = Path.Combine(Path.GetTempPath(), "Nopins.json");
            File.WriteAllText(emptyLayout, "{\"pinnedList\":[]}");

            try
            {
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "ConfigureStartPins", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "ConfigureStartPinsJSON", emptyLayout, RegistryValueKind.ExpandString);

                // The Start menu needs a moment to read the empty layout. The policy is removed right after, so the user can pin again
                Thread.Sleep(3000);
            }
            finally
            {
                GroupPolicyService.DeleteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\Explorer", "ConfigureStartPins", "ConfigureStartPinsJSON");
                File.Delete(emptyLayout);
            }
        }

        /// <summary>
        /// Set Start all apps view state.
        /// </summary>
        /// <param name="state">Apps view state.</param>
        public static void StartAppsView(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideCategoryView", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideCategoryView", LGPOScope.User, LGPOScope.Computer);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "AllAppsViewMode", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set most used apps in Start.
        /// </summary>
        /// <param name="enable">Start menu used apps state.</param>
        public static void MostUsedStartApps(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "ShowOrHideMostUsedApps", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "ShowOrHideMostUsedApps", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMFUprogramsList", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoInstrumentation", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMFUprogramsList", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoInstrumentation", LGPOScope.User, LGPOScope.Computer);

            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowFrequentList", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu layout state.
        /// </summary>
        /// <param name="state">Start menu layout state.</param>
        public static void StartLayout(int state)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_Layout", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set recommended section state.
        /// </summary>
        /// <param name="enable">Recommended section state.</param>
        public static void StartRecommendedSection(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecommendedSection", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecommendedSection", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\PolicyManager\\current\\device\\Education", "IsEducationEnvironment");
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\PolicyManager\\current\\device\\Start", "HideRecommendedSection");
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRecentDocsHistory", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoRecentDocsHistory", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecentlyAddedApps", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "HideRecentlyAddedApps", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "ShowOrHideMostUsedApps", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Explorer", "ShowOrHideMostUsedApps", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMFUprogramsList", Registry.CurrentUser, Registry.LocalMachine);
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoInstrumentation", Registry.CurrentUser, Registry.LocalMachine);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoStartMenuMFUprogramsList", LGPOScope.User, LGPOScope.Computer);
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoInstrumentation", LGPOScope.User, LGPOScope.Computer);

            if (enable)
            {
                // Show recently added apps in Start
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList");
                // Show most used Apps in Start
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowFrequentList", 1, RegistryValueKind.DWord);
                // Show recommendations for tips, shortcuts, new apps, and more in Start
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations");
                // Show recommended files in Start, recent files in File Explorer, and items in jump lists
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_TrackDocs");
                return;
            }

            // Hide recently added apps in Start
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowRecentList", 0, RegistryValueKind.DWord);
            // Hide most used Apps in Start
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Start", "ShowFrequentList", 0, RegistryValueKind.DWord);
            // Hide recommendations for tips, shortcuts, new apps, and more in Start
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_IrisRecommendations", 0, RegistryValueKind.DWord);
            // Hide recommended files in Start, recent files in File Explorer, and items in jump lists
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "Start_TrackDocs", 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set clock in notification center state.
        /// </summary>
        /// <param name="enable">Clock state.</param>
        public static void ShowClockInNotificationCenter(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowClockInNotificationCenter", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        private static void SetJepriCreationsCursors(JepriCursorsTheme theme)
        {
            const string CursorsUrl = "https://raw.githubusercontent.com/farag2/Sophia-Script-for-Windows/refs/heads/main/Cursors/w11-cursor-concept-free.zip";

            try
            {
                CursorsService.SetJepriCreationsCursors(theme);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex.InnerException is HttpRequestException || ex is TaskCanceledException || ex.InnerException is TaskCanceledException)
            {
                // The cursors are downloaded, so the setting cannot be applied without a connection
                throw new CustomizationException(exceptionMessage: $"Url {CursorsUrl} is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired") { Arguments = [CursorsUrl] };
            }
        }
    }
}
