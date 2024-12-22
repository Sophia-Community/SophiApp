// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;

    /// <summary>
    /// Set the OS settings.
    /// </summary>
    public static class Mutators
    {
        private static readonly IAppNotificationService AppNotificationService = App.GetService<IAppNotificationService>();
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IFileService FileService = App.GetService<IFileService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IGroupPolicyService GroupPolicyService = App.GetService<IGroupPolicyService>();
        private static readonly IHttpService HttpService = App.GetService<IHttpService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IRegistryService RegistryService = App.GetService<IRegistryService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();

        /// <summary>
        /// Set DiagTrack service state.
        /// </summary>
        /// <param name="isEnabled">DiagTrack service state.</param>
        public static void DiagTrackService(bool isEnabled)
        {
            var diagTrackService = new ServiceController("DiagTrack");
            var firewallRule = FirewallService.GetGroupRules("DiagTrack").First();

            if (isEnabled)
            {
                OsService.SetServiceStartMode(diagTrackService, ServiceStartMode.Automatic);
                diagTrackService.TryStart();
                firewallRule.Enabled = true;
                firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                return;
            }

            diagTrackService.TryStop();
            OsService.SetServiceStartMode(diagTrackService, ServiceStartMode.Disabled);
            firewallRule.Enabled = true;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

        /// <summary>
        /// Set Windows feature "Diagnostic data level" state.
        /// </summary>
        /// <param name="state">Diagnostic data level state.</param>
        public static void DiagnosticDataLevel(int state)
        {
            if (state.Equals(2))
            {
                var osEdition = CommonDataService.OsProperties.Edition;
                var isEnterpriseOrEducation = osEdition.Contains("Enterprise") || osEdition.Contains("Education");
                Registry.LocalMachine.OpenOrCreateSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection")
                    .SetValue("AllowTelemetry", isEnterpriseOrEducation ? 0 : 1, RegistryValueKind.DWord);
                Registry.LocalMachine.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")
                    .SetValue("MaxTelemetryAllowed", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")
                    .SetValue("ShowedToastAtLevel", 1, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")
                .SetValue("MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")
                .SetValue("ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection", true)
                ?.DeleteValue("AllowTelemetry", false);
        }

        /// <summary>
        /// Set Windows feature "Error reporting" state.
        /// </summary>
        /// <param name="isEnabled">Feature state.</param>
        public static void ErrorReporting(bool isEnabled)
        {
            var reportingRegistryPath = "Software\\Microsoft\\Windows\\Windows Error Reporting";
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            var reportingService = new ServiceController("WerSvc");
            GroupPolicyService.ClearErrorReportingCache();

            if (isEnabled)
            {
                reportingTask.Enabled = true;
                Registry.CurrentUser.OpenSubKey(reportingRegistryPath, true)?.DeleteValue("Disabled", false);
                OsService.SetServiceStartMode(reportingService, ServiceStartMode.Manual);
                reportingService.TryStart();
                return;
            }

            if (!CommonDataService.OsProperties.Edition.Equals("Core"))
            {
                reportingTask.Enabled = false;
                Registry.CurrentUser.OpenSubKey(reportingRegistryPath, true)?.SetValue("Disabled", 1, RegistryValueKind.DWord);
            }

            reportingService.TryStop();
            OsService.SetServiceStartMode(reportingService, ServiceStartMode.Disabled);
        }

        /// <summary>
        /// Set Windows feature "Feedback frequency" state.
        /// </summary>
        /// <param name="state">Feedback frequency state.</param>
        public static void FeedbackFrequency(int state)
        {
            var rulesPath = "Software\\Microsoft\\Siuf\\Rules";
            GroupPolicyService.ClearFeedbackFrequencyCache();

            if (state.Equals(2))
            {
                Registry.CurrentUser.OpenOrCreateSubKey(rulesPath).SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKey(rulesPath, false);
        }

        /// <summary>
        /// Set telemetry scheduled tasks state.
        /// </summary>
        /// <param name="isEnabled">Scheduled tasks state.</param>
        public static void ScheduledTasks(bool isEnabled)
        {
            new List<Task?>()
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
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\XblGameSave\\XblGameSaveTask1"),
             }
            .ForEach(task =>
             {
                 if (task is not null)
                 {
                     task.Enabled = isEnabled;
                 }
             });
        }

        /// <summary>
        /// Set Windows feature "Sign-in info" state.
        /// </summary>
        /// <param name="isEnabled">Sign-in info state.</param>
        public static void SigninInfo(bool isEnabled)
        {
            var sid = InstrumentationService.GetUserSid(Environment.UserName);
            var userArsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{sid}";
            var optOut = "OptOut";
            GroupPolicyService.ClearSigninInfoCache();

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(userArsoPath, true)?.DeleteValue(optOut, false);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(userArsoPath).SetValue(optOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set language list access state.
        /// </summary>
        /// <param name="isEnabled">Language list state.</param>
        public static void LanguageListAccess(bool isEnabled)
        {
            var userProfilePath = "Control Panel\\International\\User Profile";
            var httpOptOut = "HttpAcceptLanguageOptOut";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.DeleteValue(httpOptOut, false);
                return;
            }

            Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.SetValue(httpOptOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the permission for apps to use advertising ID state.
        /// </summary>
        /// <param name="isEnabled">Advertising ID state.</param>
        public static void AdvertisingID(bool isEnabled)
        {
            var infoPath = "Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo";
            GroupPolicyService.ClearAdvertisingIdCache();
            Registry.CurrentUser.OpenOrCreateSubKey(infoPath).SetValue("Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows welcome experiences state.
        /// </summary>
        /// <param name="isEnabled">Windows welcome experiences state.</param>
        public static void WindowsWelcomeExperience(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-310093Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows tips state.
        /// </summary>
        /// <param name="isEnabled">Windows tips state.</param>
        public static void WindowsTips(bool isEnabled)
        {
            var contentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager";
            GroupPolicyService.ClearWindowsTipsCache();
            Registry.CurrentUser.OpenSubKey(contentPath, true)?.SetValue("SubscribedContent-338389Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the suggested content in the Settings app state.
        /// </summary>
        /// <param name="isEnabled">Suggested content state.</param>
        public static void SettingsSuggestedContent(bool isEnabled)
        {
            new List<string> { "SubscribedContent-353694Enabled", "SubscribedContent-353696Enabled", "SubscribedContent-338393Enabled" }
            .ForEach(content => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue(content, isEnabled ? 1 : 0, RegistryValueKind.DWord));
        }

        /// <summary>
        /// Set the automatic installing suggested apps state.
        /// </summary>
        /// <param name="isEnabled">Suggested apps state.</param>
        public static void AppsSilentInstalling(bool isEnabled)
        {
            GroupPolicyService.ClearAppsInstallingCache();
            var contentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager";
            Registry.CurrentUser.OpenSubKey(contentPath, true)?.SetValue("SilentInstalledAppsEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="isEnabled">Whats New state.</param>
        public static void WhatsNewInWindows(bool isEnabled)
        {
            var profilePath = "Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement";
            Registry.CurrentUser.OpenOrCreateSubKey(profilePath).SetValue("ScoobeSystemSettingEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="isEnabled">Tailored experiences state.</param>
        public static void TailoredExperiences(bool isEnabled)
        {
            var privacyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Privacy";
            GroupPolicyService.ClearTailoredExperiencesCache();
            Registry.CurrentUser.OpenSubKey(privacyPath, true)?.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Bing search" state.
        /// </summary>
        /// <param name="isEnabled">Bing search state.</param>
        public static void BingSearch(bool isEnabled)
        {
            var explorerPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var disableSuggestions = "DisableSearchBoxSuggestions";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(explorerPath, true)?.DeleteValue(disableSuggestions, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(explorerPath).SetValue(disableSuggestions, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu recommendations state.
        /// </summary>
        /// <param name="isEnabled">Start menu recommendations state.</param>
        public static void StartRecommendationsTips(bool isEnabled)
        {
            var irisPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var startIris = "Start_IrisRecommendations";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(irisPath, true)?.DeleteValue(irisPath, false);
                return;
            }

            Registry.CurrentUser.OpenSubKey(irisPath, true)?.SetValue(startIris, 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start Menu notifications state.
        /// </summary>
        /// <param name="isEnabled">Start Menu notifications state.</param>
        public static void StartAccountNotifications(bool isEnabled)
        {
            var notificationsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var startNotifications = "Start_AccountNotifications";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(notificationsPath, true)?.DeleteValue(startNotifications, false);
                return;
            }

            Registry.CurrentUser.OpenSubKey(notificationsPath, true)?.SetValue(startNotifications, 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the "This PC" icon on Desktop state.
        /// </summary>
        /// <param name="isEnabled">"This PC" icon state.</param>
        public static void ThisPC(bool isEnabled)
        {
            var pcPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\HideDesktopIcons\\NewStartPanel";
            var pcGuid = "{20D04FE0-3AEA-1069-A2D8-08002B30309D}";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenOrCreateSubKey(pcPath).SetValue(pcGuid, 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(pcPath, true)?.DeleteValue(pcGuid, false);
        }

        /// <summary>
        /// Set item check boxes state.
        /// </summary>
        /// <param name="isEnabled">Item check boxes state.</param>
        public static void CheckBoxes(bool isEnabled)
        {
            var boxesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(boxesPath, true)?.SetValue("AutoCheckSelect", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set hidden files, folders, and drives state.
        /// </summary>
        /// <param name="isEnabled">Hidden items state.</param>
        public static void HiddenItems(bool isEnabled)
        {
            var itemsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(itemsPath, true)?.SetValue("Hidden", isEnabled ? 1 : 2, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set file name extensions visibility state.
        /// </summary>
        /// <param name="isEnabled">File extensions visibility state.</param>
        public static void FileExtensions(bool isEnabled)
        {
            var extensionsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(extensionsPath, true)?.SetValue("HideFileExt", isEnabled ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set folder merge conflicts state.
        /// </summary>
        /// <param name="isEnabled">Folder merge conflicts state.</param>
        public static void MergeConflicts(bool isEnabled)
        {
            var mergePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(mergePath, true)?.SetValue("HideMergeConflicts", isEnabled ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set how to open File Explorer.
        /// </summary>
        /// <param name="state">File Explorer open state.</param>
        public static void OpenFileExplorerTo(int state)
        {
            var filePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(filePath, true)?.SetValue("LaunchTo", state, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set File Explorer ribbon state.
        /// </summary>
        /// <param name="state">File Explorer ribbon state.</param>
        public static void FileExplorerRibbon(int state)
        {
            GroupPolicyService.ClearFileExplorerRibbonCache();
            var ribbonPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Ribbon";
            Registry.CurrentUser.OpenOrCreateSubKey(ribbonPath).SetValue("MinimizedStateTabletModeOff", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set File Explorer compact mode state.
        /// </summary>
        /// <param name="isEnabled">File Explorer compact mode state.</param>
        public static void FileExplorerCompactMode(bool isEnabled)
        {
            var compactPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(compactPath, true)?.SetValue("UseCompactMode", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set File Explorer provider notification visibility state.
        /// </summary>
        /// <param name="isEnabled">File Explorer provider notification visibility state.</param>
        public static void OneDriveFileExplorerAd(bool isEnabled)
        {
            var drivePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(drivePath, true)?.SetValue("ShowSyncProviderNotifications", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set snap a window state.
        /// </summary>
        /// <param name="isEnabled">Snap Assist state.</param>
        public static void SnapAssist(bool isEnabled)
        {
            var desktopPath = "Control Panel\\Desktop";
            var snapPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(desktopPath, true)?.SetValue("WindowArrangementActive", 1, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey(snapPath, true)?.SetValue("SnapAssist", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set file transfer dialog box mode.
        /// </summary>
        /// <param name="state">File transfer dialog box state.</param>
        public static void FileTransferDialog(int state)
        {
            var statusPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\OperationStatusManager";
            Registry.CurrentUser.OpenOrCreateSubKey(statusPath).SetValue("EnthusiastMode", state.Equals(1) ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set recycle bin confirmation dialog state.
        /// </summary>
        /// <param name="isEnabled">Recycle bin dialog state.</param>
        public static void RecycleBinDeleteConfirmation(bool isEnabled)
        {
            GroupPolicyService.ClearRecycleBinDeleteConfirmationCache();
            var shellPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var shellState = "ShellState";
            var shellValue = Registry.CurrentUser.OpenSubKey(shellPath)?.GetValue(shellState) as byte[] ?? new byte[5];
            shellValue[4] = isEnabled ? (byte)51 : (byte)55;
            Registry.CurrentUser.OpenSubKey(shellPath, true)?.SetValue(shellState, shellValue, RegistryValueKind.Binary);
        }

        /// <summary>
        /// Set recently used Quick access files state.
        /// </summary>
        /// <param name="isEnabled">Quick access files state.</param>
        public static void QuickAccessRecentFiles(bool isEnabled)
        {
            GroupPolicyService.ClearQuickAccessRecentFilesCache();
            var recentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            Registry.CurrentUser.OpenSubKey(recentPath, true)?.SetValue("ShowRecent", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set frequently used Quick access folders state.
        /// </summary>
        /// <param name="isEnabled">Quick access folders state.</param>
        public static void QuickAccessFrequentFolders(bool isEnabled)
        {
            var frequentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            Registry.CurrentUser.OpenSubKey(frequentPath, true)?.SetValue("ShowFrequent", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar alignment state.
        /// </summary>
        /// <param name="state">Taskbar alignment state.</param>
        public static void TaskbarAlignment(int state)
        {
            var alignmentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(alignmentPath, true)?.SetValue("TaskbarAl", state.Equals(1) ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar widgets icon state.
        /// </summary>
        /// <param name="isEnabled">Taskbar widgets icon state.</param>
        public static void TaskbarWidgets(bool isEnabled)
        {
            GroupPolicyService.ClearTaskbarWidgetsCache();
            var advancedPath = "HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var command = $"-Command \"& {{New-ItemProperty -Path {advancedPath} -Name TaskbarDa -PropertyType DWord -Value {(isEnabled ? 1 : 0)} -Force}}\"";
            PowerShellService.InvokeCommandBypassUCPD(command);
        }

        /// <summary>
        /// Set Search on the taskbar state.
        /// </summary>
        /// <param name="state">Taskbar search state.</param>
        public static void TaskbarSearchWindows10(int state)
        {
            GroupPolicyService.ClearTaskbarSearchCache();
            var searchPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Search";
            Registry.CurrentUser.OpenSubKey(searchPath, true)?.SetValue("SearchboxTaskbarMode", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Search on the taskbar state.
        /// </summary>
        /// <param name="state">Taskbar search state.</param>
        public static void TaskbarSearchWindows11(int state)
        {
            GroupPolicyService.ClearTaskbarSearchCache();
            var searchMode = "SearchboxTaskbarMode";
            var searchPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Search";

            if (state.Equals(3))
            {
                Registry.CurrentUser.OpenSubKey(searchPath, true)?.SetValue(searchMode, 3, RegistryValueKind.DWord);
                return;
            }

            if (state.Equals(4))
            {
                Registry.CurrentUser.OpenSubKey(searchPath, true)?.SetValue(searchMode, 2, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(searchPath, true)?.SetValue(searchMode, state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set search highlights state.
        /// </summary>
        /// <param name="isEnabled">Search highlights state.</param>
        public static void SearchHighlights(bool isEnabled)
        {
            GroupPolicyService.ClearSearchHighlightsCache();
            var feedsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Feeds\\DSB";
            var searchPath = "Software\\Microsoft\\Windows\\CurrentVersion\\SearchSettings";
            Registry.CurrentUser.OpenSubKey(feedsPath, true)?.SetValue("ShowDynamicContent", isEnabled ? 1 : 0, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey(searchPath, true)?.SetValue("IsDynamicSearchBoxEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Cortana button taskbar state.
        /// </summary>
        /// <param name="isEnabled">Cortana button state.</param>
        public static void CortanaButton(bool isEnabled)
        {
            GroupPolicyService.ClearCortanaButtonCache();
            var advancedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(advancedPath, true)?.SetValue("ShowCortanaButton", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar task view button state.
        /// </summary>
        /// <param name="isEnabled">Taskbar task view button state.</param>
        public static void TaskViewButton(bool isEnabled)
        {
            var advancedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(advancedPath, true)?.SetValue("ShowTaskViewButton", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set News and Interests state.
        /// </summary>
        /// <param name="isEnabled">News and Interests state.</param>
        public static void NewsInterests(bool isEnabled)
        {
            GroupPolicyService.ClearNewsInterestsCache();
            var hashData = OsService.GetNewsInterestsHashData(isEnabled);
            var feedsCommand = $"-Command \"& {{New-ItemProperty -Path HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Feeds -Name ShellFeedsTaskbarViewMode -PropertyType DWord -Value {(isEnabled ? 0 : 2)} -Force}}\"";
            var enFeedsCommand = $"-Command \"& {{New-ItemProperty -Path HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Feeds -Name EnShellFeedsTaskbarViewMode -PropertyType DWord -Value {hashData} -Force}}\"";
            PowerShellService.InvokeCommandBypassUCPD(feedsCommand);
            PowerShellService.InvokeCommandBypassUCPD(enFeedsCommand);
        }

        /// <summary>
        /// Set taskbar people icon state.
        /// </summary>
        /// <param name="isEnabled">Taskbar people icon state.</param>
        public static void PeopleTaskbar(bool isEnabled)
        {
            GroupPolicyService.ClearPeopleTaskbarCache();
            var peoplePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\People";
            Registry.CurrentUser.OpenOrCreateSubKey(peoplePath)?.SetValue("PeopleBand", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Meet Now icon state.
        /// </summary>
        /// <param name="isEnabled">Meet Now icon state.</param>
        public static void MeetNow(bool isEnabled)
        {
            GroupPolicyService.ClearMeetNowCache();
            var stuckValue = "Settings";
            var stuckPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StuckRects3";
            var settings = Registry.CurrentUser.OpenSubKey(stuckPath)?.GetValue(stuckValue) as byte[] ?? new byte[10];
            settings[9] = isEnabled ? (byte)0 : (byte)128;
            Registry.CurrentUser.OpenSubKey(stuckPath, true)?.SetValue(stuckValue, settings, RegistryValueKind.Binary);
        }

        /// <summary>
        /// Set Windows Ink Workspace button state.
        /// </summary>
        /// <param name="isEnabled">Windows Ink Workspace button state.</param>
        public static void WindowsInkWorkspace(bool isEnabled)
        {
            GroupPolicyService.ClearWindowsInkWorkspaceCache();
            var workspacePath = "Software\\Microsoft\\Windows\\CurrentVersion\\PenWorkspace";
            Registry.CurrentUser.OpenSubKey(workspacePath, true)?.SetValue("PenWorkspaceButtonDesiredVisibility", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set notification area icons state.
        /// </summary>
        /// <param name="isEnabled">Notification area icons state.</param>
        public static void NotificationAreaIcons(bool isEnabled)
        {
            GroupPolicyService.ClearNotificationAreaIconsCache();
            var trayPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            Registry.CurrentUser.OpenSubKey(trayPath, true)?.SetValue("EnableAutoTray", isEnabled ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set seconds on the taskbar clock state.
        /// </summary>
        /// <param name="isEnabled">Seconds on the taskbar clock state.</param>
        public static void SecondsInSystemClock(bool isEnabled)
        {
            var clockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(clockPath, true)?.SetValue("ShowSecondsInSystemClock", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set taskbar combine state.
        /// </summary>
        /// <param name="state">Taskbar combine state.</param>
        public static void TaskbarCombine(int state)
        {
            GroupPolicyService.ClearTaskbarCombineCache();
            var taskbarPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(taskbarPath, true)?.SetValue("TaskbarGlomLevel", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set end task in taskbar by click state.
        /// </summary>
        /// <param name="isEnabled">Taskbar end task state.</param>
        public static void TaskbarEndTask(bool isEnabled)
        {
            var taskbarPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\\TaskbarDeveloperSettings";
            var taskbarSettings = "TaskbarEndTask";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenOrCreateSubKey(taskbarPath).SetValue(taskbarSettings, 1, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(taskbarPath, true)?.DeleteValue(taskbarSettings, false);
        }

        /// <summary>
        /// Set Control Panel icons view state.
        /// </summary>
        /// <param name="state">Control Panel icons view state.</param>
        public static void ControlPanelView(int state)
        {
            GroupPolicyService.ClearControlPanelViewCache();
            var panelPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\ControlPanel";
            var iconView = "AllItemsIconView";
            var startupPage = "StartupPage";

            switch (state)
            {
                case 1:
                    Registry.CurrentUser.OpenOrCreateSubKey(panelPath).SetValue(iconView, 0, RegistryValueKind.DWord);
                    Registry.CurrentUser.OpenSubKey(panelPath)?.SetValue(startupPage, 0, RegistryValueKind.DWord);
                    break;
                case 2:
                    Registry.CurrentUser.OpenOrCreateSubKey(panelPath).SetValue(iconView, 0, RegistryValueKind.DWord);
                    Registry.CurrentUser.OpenSubKey(panelPath)?.SetValue(startupPage, 1, RegistryValueKind.DWord);
                    break;
                default:
                    Registry.CurrentUser.OpenOrCreateSubKey(panelPath).SetValue(iconView, 1, RegistryValueKind.DWord);
                    Registry.CurrentUser.OpenSubKey(panelPath)?.SetValue(startupPage, 1, RegistryValueKind.DWord);
                    break;
            }
        }

        /// <summary>
        /// Set Windows color mode state.
        /// </summary>
        /// <param name="state">Windows color mode state.</param>
        public static void WindowsColorMode(int state)
        {
            var personalizePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            Registry.CurrentUser.OpenSubKey(personalizePath, true)?.SetValue("SystemUsesLightTheme", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set apps color mode state.
        /// </summary>
        /// <param name="state">Apps color mode state.</param>
        public static void AppColorMode(int state)
        {
            var personalizePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
            Registry.CurrentUser.OpenSubKey(personalizePath, true)?.SetValue("AppsUseLightTheme", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "New App Installed" indicator state.
        /// </summary>
        /// <param name="isEnabled">New App Installed" indicator state.</param>
        public static void NewAppInstalledNotification(bool isEnabled)
        {
            var alertPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var appAlert = "NoNewAppAlert";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(alertPath, true)?.DeleteValue(appAlert, false);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(alertPath).SetValue(appAlert, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set first sign-in animation state.
        /// </summary>
        /// <param name="isEnabled">First sign-in animation state.</param>
        public static void FirstLogonAnimation(bool isEnabled)
        {
            GroupPolicyService.ClearFirstLogonAnimationCache();
            var logonPath = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";
            Registry.LocalMachine.OpenSubKey(logonPath, true)?.SetValue("EnableFirstLogonAnimation", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set JPEG wallpapers quality state.
        /// </summary>
        /// <param name="state">JPEG wallpapers quality state.</param>
        public static void JPEGWallpapersQuality(int state)
        {
            var desktopPath = "Control Panel\\Desktop";
            var jpegQuality = "JPEGImportQuality";

            if (state.Equals(1))
            {
                Registry.CurrentUser.OpenSubKey(desktopPath, true)?.SetValue(jpegQuality, 100, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(desktopPath, true)?.DeleteValue(jpegQuality, false);
        }

        /// <summary>
        /// Set "- Shortcut" suffix state.
        /// </summary>
        /// <param name="isEnabled">"- Shortcut" suffix state.</param>
        public static void ShortcutsSuffix(bool isEnabled)
        {
            var linkPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var templatesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\NamingTemplates";
            var shortcutTemplate = "ShortcutNameTemplate";
            Registry.CurrentUser.OpenSubKey(linkPath, true)?.DeleteValue("link", false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(templatesPath, true)?.DeleteValue(shortcutTemplate, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(templatesPath)?.SetValue(shortcutTemplate, "%s.lnk", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Print screen button state.
        /// </summary>
        /// <param name="isEnabled">Print screen button state.</param>
        public static void PrtScnSnippingTool(bool isEnabled)
        {
            var keyboardPath = "Control Panel\\Keyboard";
            Registry.CurrentUser.OpenSubKey(keyboardPath, true)?.SetValue("PrintScreenKeyForSnippingEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set input method for app window state.
        /// </summary>
        /// <param name="isEnabled">Input method for app window state.</param>
        public static void AppsLanguageSwitch(bool isEnabled)
        {
            var command = isEnabled ? "Set-WinLanguageBarOption -UseLegacySwitchMode" : "Set-WinLanguageBarOption";
            _ = PowerShellService.Invoke(command);
        }

        /// <summary>
        /// Set Aero Shake state.
        /// </summary>
        /// <param name="isEnabled">Aero Shake state.</param>
        public static void AeroShaking(bool isEnabled)
        {
            GroupPolicyService.ClearAeroShakingCache();
            var shakingPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(shakingPath, true)?.SetValue("DisallowShaking", isEnabled ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "Windows 11 Cursors Concept v2" cursors from Jepri Creations state.
        /// </summary>
        /// <param name="isEnabled">"Windows 11 Cursors Concept v2" cursors state.</param>
        public static void Cursors(bool isEnabled)
        {
            // Method intentionally left empty.
        }

        /// <summary>
        /// Set files and folders grouping state.
        /// </summary>
        /// <param name="state">Files and folders grouping state.</param>
        public static void FolderGroupBy(int state)
        {
            #pragma warning disable SA1003 // Symbols should be spaced correctly

            var folderPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FolderTypes\\{885a186e-a440-4ada-812b-db871b942259}";
            var groupPath = $"{folderPath}\\TopViews\\{00000000-0000-0000-0000-000000000000}";

            if (state.Equals(1))
            {
                PowerShellService.ClearCommonDialogViews();
                using var groupKey = Registry.CurrentUser.OpenOrCreateSubKey(groupPath);
                groupKey.SetValue("ColumnList", "System.Null", RegistryValueKind.String);
                groupKey.SetValue("GroupBy", "System.Null", RegistryValueKind.String);
                groupKey.SetValue("LogicalViewMode", 1, RegistryValueKind.DWord);
                groupKey.SetValue("Name", "NoName", RegistryValueKind.String);
                groupKey.SetValue("Order", 0, RegistryValueKind.DWord);
                groupKey.SetValue("PrimaryProperty", "System.ItemNameDisplay", RegistryValueKind.String);
                groupKey.SetValue("SortByList", "prop:System.ItemNameDisplay", RegistryValueKind.String);
                return;
            }

            Registry.CurrentUser.DeleteSubKeyTree(folderPath, false);

            #pragma warning restore SA1003 // Symbols should be spaced correctly
        }

        /// <summary>
        /// Set navigation pane expand state.
        /// </summary>
        /// <param name="isEnabled">Navigation pane expand state.</param>
        public static void NavigationPaneExpand(bool isEnabled)
        {
            var panePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(panePath, true)?.SetValue("NavPaneExpandToCurrentFolder", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu recently added apps state.
        /// </summary>
        /// <param name="isEnabled">Start menu recently added apps state.</param>
        public static void RecentlyAddedApps(bool isEnabled)
        {
            GroupPolicyService.ClearRecentlyAddedAppsCache();
            var appsPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var appsValue = "HideRecentlyAddedApps";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(appsPath, true)?.DeleteValue(appsValue, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(appsPath).SetValue(appsValue, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu app suggestions state.
        /// </summary>
        /// <param name="isEnabled">Start menu app suggestions state.</param>
        public static void AppSuggestions(bool isEnabled)
        {
            GroupPolicyService.ClearAppSuggestionsCache();
            var contentPath = "Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager";
            Registry.CurrentUser.OpenSubKey(contentPath, true)?.SetValue("SubscribedContent-338388Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Start menu layout state.
        /// </summary>
        /// <param name="state">Start menu layout state.</param>
        public static void StartLayout(int state)
        {
            var layoutPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            Registry.CurrentUser.OpenSubKey(layoutPath, true)?.SetValue("Start_Layout", state - 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set HEVC state.
        /// </summary>
        /// <param name="isEnabled">HEVC state.</param>
        public static void HEVC(bool isEnabled)
        {
            if (isEnabled)
            {
                var downloadFolder = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders")?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") as string ?? Environment.GetEnvironmentVariable("TEMP");
                var appxFile = $"{downloadFolder}\\Microsoft.HEVCVideoExtension_8wekyb3d8bbwe.appx";
                HttpService.DownloadHEVCAppxAsync(appxFile).Wait();
                AppxPackagesService.InstallFromFileAsync(appxFile).Wait();
                File.Delete(appxFile);
                return;
            }

            AppxPackagesService.RemovePackage(packageName: "Microsoft.HEVCVideoExtension", forAllUsers: false);
        }

        /// <summary>
        /// Set Cortana auto start state.
        /// </summary>
        /// <param name="isEnabled">Cortana auto start state.</param>
        public static void CortanaAutostart(bool isEnabled)
        {
            var startupIdPath = "Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.549981C3F5F10_8wekyb3d8bbwe\\CortanaStartupId";
            Registry.ClassesRoot.OpenSubKey(startupIdPath, true)?.SetValue("State", isEnabled ? 2 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Teams auto start state.
        /// </summary>
        /// <param name="isEnabled">Teams auto start state.</param>
        public static void TeamsAutostart(bool isEnabled)
        {
            var startupTaskPath = "Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\MSTeams_8wekyb3d8bbwe\\TeamsTfwStartupTask";
            Registry.CurrentUser.OpenSubKey(startupTaskPath, true)?.SetValue("State", isEnabled ? 2 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Xbox game bar state.
        /// </summary>
        /// <param name="isEnabled">Xbox game bar state.</param>
        public static void XboxGameBar(bool isEnabled)
        {
            var barValue = isEnabled ? 1 : 0;
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", true)?.SetValue("AppCaptureEnabled", barValue, RegistryValueKind.DWord);
            Registry.CurrentUser.OpenSubKey("System\\GameConfigStore", true)?.SetValue("GameDVR_Enabled", barValue, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Xbox game tips state.
        /// </summary>
        /// <param name="isEnabled">Xbox game tips state.</param>
        public static void XboxGameTips(bool isEnabled)
        {
            var barValue = isEnabled ? 1 : 0;
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\GameBar", true)?.SetValue("ShowStartupPanel", barValue, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set GPU scheduling state.
        /// </summary>
        /// <param name="isEnabled">GPU scheduling state.</param>
        public static void GPUScheduling(bool isEnabled)
        {
            var hwSchValue = isEnabled ? 2 : 1;
            Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers")?.SetValue("HwSchMode", hwSchValue, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "Windows Cleanup" scheduled task state.
        /// </summary>
        /// <param name="isEnabled">Task state.</param>
        public static void CleanupTask(bool isEnabled)
        {
            ScheduledTaskService.DeleteTaskFolders(["Sophia Script", "SophiApp"]);
            RegistryService.RemoveVolumeCachesStateFlags();

            if (isEnabled)
            {
                AppNotificationService.EnableToastNotification();
                RegistryService.SetVolumeCachesStateFlags();
                AppNotificationService.RegisterAsToastSender("Sophia");
                AppNotificationService.RegisterCleanupProtocolAsToastSender();
                ScheduledTaskService.RegisterCleanupTask();
                ScheduledTaskService.RegisterCleanupNotificationTask();
                return;
            }

            ScheduledTaskService.UnregisterCleanupTask();
            ScheduledTaskService.UnregisterCleanupNotificationTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
            AppNotificationService.UnregisterCleanupProtocol();
        }

        /// <summary>
        /// Set scheduled task "SoftwareDistribution" state.
        /// </summary>
        /// <param name="isEnabled">Task state.</param>
        public static void SoftwareDistributionTask(bool isEnabled)
        {
            ScheduledTaskService.DeleteTaskFolders(["Sophia Script", "SophiApp"]);

            if (isEnabled)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender("Sophia");
                ScheduledTaskService.RegisterSoftwareDistributionTask();
                return;
            }

            ScheduledTaskService.UnregisterSoftwareDistributionTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }

        /// <summary>
        /// Set scheduled task "Temp" state.
        /// </summary>
        /// <param name="isEnabled">Task state.</param>
        public static void TempTask(bool isEnabled)
        {
            ScheduledTaskService.DeleteTaskFolders(["Sophia Script", "SophiApp"]);

            if (isEnabled)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender("Sophia");
                ScheduledTaskService.RegisterTempTask();
                return;
            }

            ScheduledTaskService.UnregisterTempTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }

        /// <summary>
        /// Set Windows network protection state.
        /// </summary>
        /// <param name="isEnabled">Network protection state.</param>
        public static void NetworkProtection(bool isEnabled)
        {
            var protectionScript = $"Set-MpPreference -EnableNetworkProtection {(isEnabled ? "Enabled" : "Disabled")}";
            _ = PowerShellService.Invoke(protectionScript);
        }

        /// <summary>
        /// Set Windows PUApps detection state.
        /// </summary>
        /// <param name="isEnabled">PUApps detection state.</param>
        public static void PUAppsDetection(bool isEnabled)
        {
            var detectionScript = $"Set-MpPreference -PUAProtection {(isEnabled ? "Enabled" : "Disabled")}";
            _ = PowerShellService.Invoke(detectionScript);
        }

        /// <summary>
        /// Set Microsoft Defender sandbox state.
        /// </summary>
        /// <param name="isEnabled">Microsoft Defender sandbox state.</param>
        public static void DefenderSandbox(bool isEnabled)
        {
            var sandboxScript = $"setx /M MP_FORCE_USE_SANDBOX {(isEnabled ? "1" : "0")}";
            _ = PowerShellService.Invoke(sandboxScript);
        }

        /// <summary>
        /// Set Windows Event Viewer custom view state.
        /// </summary>
        /// <param name="isEnabled">Event Viewer custom view state.</param>
        public static void EventViewerCustomView(bool isEnabled)
        {
            var auditValueName = "ProcessCreationIncludeCmdLine_Enabled";
            var viewerXmlPath = $"{Environment.GetEnvironmentVariable("ALLUSERSPROFILE")}\\Microsoft\\Event Viewer\\Views\\ProcessCreation.xml";
            var viewerAuditPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit";
            var viewerGuid = "{0CCE922B-69AE-11D9-BED3-505054503030}";
            var viewerXml = @$"<ViewerConfig>
  <QueryConfig>
    <QueryParams>
      <UserQuery />
    </QueryParams>
    <QueryNode>
      <Name>{"EventViewerCustomView_ProcessCreationXml_Name".GetLocalized()}</Name>
      <Description>{"EventViewerCustomView_ProcessCreationXml_Description".GetLocalized()}</Description>
      <QueryList>
        <Query Id=""0"" Path=""Security"">
          <Select Path=""Security"">*[System[(EventID=4688)]]</Select>
        </Query>
      </QueryList>
    </QueryNode>
  </QueryConfig>
</ViewerConfig>";

            if (isEnabled)
            {
                _ = PowerShellService.Invoke($"auditpol /set /subcategory:\"{viewerGuid}\" /success:enable /failure:enable");
                Registry.LocalMachine.OpenSubKey(viewerAuditPath, true)?.SetValue(auditValueName, 1, RegistryValueKind.DWord);
                FileService.Save(file: viewerXmlPath, content: viewerXml);
                return;
            }

            if (!CommonDataService.IsWindows11)
            {
                _ = PowerShellService.Invoke($"auditpol / set / subcategory:\"{viewerGuid}\" / success:disable / failure:disable");
            }

            Registry.LocalMachine.OpenSubKey(viewerAuditPath, true)?.DeleteValue(auditValueName, false);
            File.Delete(viewerXmlPath);
        }

        /// <summary>
        /// Set Windows PowerShell modules logging state.
        /// </summary>
        /// <param name="isEnabled">PowerShell modules logging state.</param>
        public static void PowerShellModulesLogging(bool isEnabled)
        {
            var moduleLoggingPath = "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging";
            var moduleNamesPath = $"{moduleLoggingPath}\\ModuleNames";
            var moduleLoggingValueName = "EnableModuleLogging";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenOrCreateSubKey(moduleNamesPath).SetValue("*", "*");
                Registry.LocalMachine.OpenSubKey(moduleLoggingPath, true)?.SetValue(moduleLoggingValueName, 1, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.OpenSubKey(moduleLoggingPath, true)?.DeleteValue(moduleLoggingValueName, false);
            Registry.LocalMachine.OpenSubKey(moduleNamesPath, true)?.DeleteValue("*", false);
        }

        /// <summary>
        /// Set Windows PowerShell scripts logging state.
        /// </summary>
        /// <param name="isEnabled">PowerShell scripts logging state.</param>
        public static void PowerShellScriptsLogging(bool isEnabled)
        {
            var scriptLoggingPath = "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging";
            var scriptLoggingValueName = "EnableScriptBlockLogging";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenOrCreateSubKey(scriptLoggingPath).SetValue(scriptLoggingValueName, 1, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.OpenSubKey(scriptLoggingPath, true)?.DeleteValue(scriptLoggingValueName);
        }

        /// <summary>
        /// Set Windows SmartScreen state.
        /// </summary>
        /// <param name="isEnabled">Windows SmartScreen state.</param>
        public static void AppsSmartScreen(bool isEnabled)
        {
            Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", true)
                ?.SetValue("SmartScreenEnabled", $"{(isEnabled ? "Warn" : "Off")}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Windows save zone state.
        /// </summary>
        /// <param name="isEnabled">Windows save zone state.</param>
        public static void SaveZoneInformation(bool isEnabled)
        {
            GroupPolicyService.ClearSaveZoneInformationCache();
            var safeZonePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments";
            var safeZoneValueName = "SaveZoneInformation";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(safeZonePath, true)?.DeleteValue(safeZoneValueName, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(safeZonePath).SetValue(safeZoneValueName, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows script host state.
        /// </summary>
        /// <param name="isEnabled">Windows script host state.</param>
        public static void WindowsScriptHost(bool isEnabled)
        {
            var scriptHostPath = "Software\\Microsoft\\Windows Script Host\\Settings";
            var scriptHostValueName = "Enabled";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(scriptHostPath, true)?.DeleteValue(scriptHostValueName, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(scriptHostPath).SetValue(scriptHostValueName, 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows Sandbox state.
        /// </summary>
        /// <param name="isEnabled">Windows Sandbox state.</param>
        public static void WindowsSandbox(bool isEnabled)
        {
            var enableSandboxScript = "Enable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -All -Online -NoRestart";
            var disableSandboxScript = "Disable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -All -Online -NoRestart";
            _ = PowerShellService.Invoke($"{(isEnabled ? enableSandboxScript : disableSandboxScript)}");
        }

        /// <summary>
        /// Set Local Security Authority state.
        /// </summary>
        /// <param name="isEnabled">ocal Security Authority state.</param>
        public static void LocalSecurityAuthority(bool isEnabled)
        {
            static void WriteLSARegistryValues()
            {
                var lsaPath = "System\\CurrentControlSet\\Control\\Lsa";
                Registry.LocalMachine.OpenSubKey(lsaPath, true)?.SetValue("RunAsPPL", 2, RegistryValueKind.DWord);
                Registry.LocalMachine.OpenSubKey(lsaPath, true)?.SetValue("RunAsPPLBoot", 2, RegistryValueKind.DWord);
            }

            var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationIsEnabled() ?? false;
            var hypervisorPresent = InstrumentationService.HypervisorPresent() ?? false;

            if (isEnabled)
            {
                if (virtualizationIsEnabled || hypervisorPresent)
                {
                    WriteLSARegistryValues();
                }

                return;
            }

            Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa", true)?.DeleteValue("RunAsPPL", false);
            Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa", true)?.DeleteValue("RunAsPPLBoot", false);
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\System", true)?.DeleteValue("RunAsPPL", false);
        }

        /// <summary>
        /// Set "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        /// <param name="isEnabled">"Extract all" item state.</param>
        public static void MSIExtractContext(bool isEnabled)
        {
            var msiExtractPath = "Msi.Package\\shell\\Extract";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey($"{msiExtractPath}\\Command").SetValue(string.Empty, "msiexec.exe /a \"%1\" /qb TARGETDIR=\"%1 extracted\"", RegistryValueKind.String);

                Registry.ClassesRoot.OpenSubKey(msiExtractPath, true)?.SetValue("MUIVerb", "@shell32.dll,-37514", RegistryValueKind.String);

                Registry.ClassesRoot.OpenSubKey(msiExtractPath, true)?.SetValue("Icon", "shell32.dll,-16817", RegistryValueKind.String);

                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(msiExtractPath, false);
        }

        /// <summary>
        /// Set "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        /// <param name="isEnabled">"Install" item state.</param>
        public static void CABInstallContext(bool isEnabled)
        {
            var runAsPath = "CABFolder\\Shell\\runas";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey($"{runAsPath}\\Command")
                    .SetValue(string.Empty, "cmd /c DISM.exe /Online /Add-Package /PackagePath:\"%1\" /NoRestart & pause", RegistryValueKind.String);

                Registry.ClassesRoot.OpenSubKey(runAsPath, true)
                    ?.SetValue("MUIVerb", "@shell32.dll,-10210", RegistryValueKind.String);

                Registry.ClassesRoot.OpenSubKey(runAsPath, true)
                    ?.SetValue("HasLUAShield", string.Empty, RegistryValueKind.String);

                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(runAsPath, false);
        }

        /// <summary>
        /// Set "Cast to Device" item in the media files and folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Cast to Device" item state.</param>
        public static void CastToDeviceContext(bool isEnabled)
        {
            var shellBlockedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var castToDeviceGuid = "{7AD84985-87B4-4a16-BE58-8B72A5B390F7}";

            Registry.LocalMachine.OpenSubKey(shellBlockedPath, true)?.DeleteValue(castToDeviceGuid, false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(shellBlockedPath, true)?.DeleteValue(castToDeviceGuid, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(shellBlockedPath).SetValue(castToDeviceGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Share" context menu item state.
        /// </summary>
        /// <param name="isEnabled">"Share" item state.</param>
        public static void ShareContext(bool isEnabled)
        {
            var shellBlockedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var shareContextGuid = "{E2BF9676-5F8F-435C-97EB-11607A5BEDF7}";

            Registry.LocalMachine.OpenSubKey(shellBlockedPath, true)?.DeleteValue(shareContextGuid, false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(shellBlockedPath, true)?.DeleteValue(shareContextGuid, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(shellBlockedPath).SetValue(shareContextGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Edit With Clipchamp" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit With Clipchamp" item state.</param>
        public static void EditWithClipchampContext(bool isEnabled)
        {
            var clipChampPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var clipChampGuid = "{8AB635F8-9A67-4698-AB99-784AD929F3B4}";

            Registry.LocalMachine.OpenSubKey(clipChampPath, true)?.DeleteValue(clipChampGuid, false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(clipChampPath, true)?.DeleteValue(clipChampGuid, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(clipChampPath).SetValue(clipChampGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Edit With Photos" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit With Photos" item state.</param>
        public static void EditWithPhotosContext(bool isEnabled)
        {
            var clipChampPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var clipChampGuid = "{BFE0E2A4-C70C-4AD7-AC3D-10D1ECEBB5B4}";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(clipChampPath, true)?.DeleteValue(clipChampPath, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(clipChampPath).SetValue(clipChampGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Edit with Paint 3D" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit with Paint 3D" item state.</param>
        public static void EditWithPaint3DContext(bool isEnabled)
        {
            var paintContextValue = "ProgrammaticAccessOnly";
            new List<string>()
            {
                ".bmp", ".gif", ".jpe", ".jpeg", ".jpg", ".png", ".tif", ".tiff",
            }
            .ForEach(fileType =>
            {
                var fileTypePath = $"SystemFileAssociations\\{fileType}\\Shell\\3D Edit";

                if (isEnabled)
                {
                    Registry.ClassesRoot.OpenSubKey(fileTypePath, true)?.DeleteValue(paintContextValue, false);
                    return;
                }

                Registry.ClassesRoot.OpenSubKey(fileTypePath, true)?.SetValue(paintContextValue, string.Empty, RegistryValueKind.String);
            });
        }

        /// <summary>
        /// Set "Print" item in the .bat and .cmd files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Print" item state.</param>
        public static void PrintCMDContext(bool isEnabled)
        {
            var batPrintPath = "batfile\\shell\\print";
            var cmdPrintPath = "cmdfile\\shell\\print";
            var printContextValue = "ProgrammaticAccessOnly";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(batPrintPath, true)?.DeleteValue(printContextValue, false);
                Registry.ClassesRoot.OpenSubKey(cmdPrintPath, true)?.DeleteValue(printContextValue, false);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(batPrintPath, true)?.SetValue(printContextValue, string.Empty, RegistryValueKind.String);
            Registry.ClassesRoot.OpenSubKey(cmdPrintPath, true)?.SetValue(printContextValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Include in Library" item in the folders and drives context menu state.
        /// </summary>
        /// <param name="isEnabled">"Include in Library" item state.</param>
        public static void IncludeInLibraryContext(bool isEnabled)
        {
            var libraryContextPath = "Folder\\ShellEx\\ContextMenuHandlers\\Library Location";
            var enableValue = "{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";
            var disableValue = "-{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";
            var contextValue = isEnabled ? enableValue : disableValue;
            Registry.ClassesRoot.OpenSubKey(libraryContextPath, true)?.SetValue(string.Empty, contextValue, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Send to" item in the folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Send to" item state.</param>
        public static void SendToContext(bool isEnabled)
        {
            var sendToPath = "AllFilesystemObjects\\shellex\\ContextMenuHandlers\\SendTo";
            var enableValue = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
            var disableValue = "-{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
            var contextValue = isEnabled ? enableValue : disableValue;
            Registry.ClassesRoot.OpenSubKey(sendToPath, true)?.SetValue(string.Empty, contextValue, RegistryValueKind.String);
        }

        /// <summary>
        /// Set "Bitmap image" item in the "New" context menu state.
        /// </summary>
        /// <param name="isEnabled">"Bitmap image" item state.</param>
        public static void BitmapImageNewContext(bool isEnabled)
        {
            var bmpShellPath = ".bmp\\ShellNew";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey(bmpShellPath).SetValue("ItemName", "@%SystemRoot%\\System32\\mspaint.exe,-59414", RegistryValueKind.ExpandString);
                Registry.ClassesRoot.OpenSubKey(bmpShellPath, true)?.SetValue("NullFile", string.Empty, RegistryValueKind.String);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(bmpShellPath, false);
        }

        /// <summary>
        /// Set "Rich Text Document" item in the "New" context menu state.
        /// </summary>
        /// <param name="isEnabled">"Rich Text Document" item state.</param>
        public static void RichTextDocumentNewContext(bool isEnabled)
        {
            var rtfShellPath = ".rtf\\ShellNew";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey(rtfShellPath).SetValue("Data", @"{\rtf1}", RegistryValueKind.String);
                Registry.ClassesRoot.OpenSubKey(rtfShellPath, true)?.SetValue("ItemName", "@%ProgramFiles%\\Windows NT\\Accessories\\WORDPAD.EXE,-213", RegistryValueKind.ExpandString);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(rtfShellPath, false);
        }

        /// <summary>
        /// Set "Compressed (zipped) Folder" item in the "New" context menu state.
        /// </summary>
        /// <param name="isEnabled">"Compressed (zipped) Folder" item state.</param>
        public static void CompressedFolderNewContext(bool isEnabled)
        {
            var zipShellPath = ".zip\\CompressedFolder\\ShellNew";
            var zipContextValue = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey(zipShellPath).SetValue("Data", zipContextValue, RegistryValueKind.Binary);
                Registry.ClassesRoot.OpenSubKey(zipShellPath, true)?.SetValue("ItemName", "@%SystemRoot%\\System32\\zipfldr.dll,-10194", RegistryValueKind.ExpandString);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(zipShellPath, false);
        }

        /// <summary>
        /// Set "Open", "Print", and "Edit" context menu items available when selecting more than 15 files state.
        /// </summary>
        /// <param name="isEnabled">"Open", "Print", and "Edit" context menu items state.</param>
        public static void MultipleInvokeContext(bool isEnabled)
        {
            var multipleContextPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var multipleContextValue = "MultipleInvokePromptMinimum";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(multipleContextPath, true)?.SetValue(multipleContextValue, 300, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(multipleContextPath, true)?.DeleteValue(multipleContextValue, false);
        }

        /// <summary>
        /// Set "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        /// <param name="isEnabled">"Look for an app in the Microsoft Store" items state.</param>
        public static void UseStoreOpenWith(bool isEnabled)
        {
            var storeContextPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var storeContextValue = "NoUseStoreOpenWith";

            Registry.LocalMachine.OpenSubKey(storeContextPath, true)?.DeleteValue(storeContextValue, false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(storeContextPath, true)?.DeleteValue(storeContextValue, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(storeContextPath).SetValue(storeContextValue, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "Open in Windows Terminal" item in the folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Open in Windows Terminal" item state.</param>
        public static void OpenWindowsTerminalContext(bool isEnabled)
        {
            var extensionsBlockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var terminalGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

            Registry.LocalMachine.OpenSubKey(extensionsBlockPath, true)?.DeleteValue(terminalGuid, false);

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(extensionsBlockPath, true)?.DeleteValue(terminalGuid, false);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(extensionsBlockPath).SetValue(terminalGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        /// <param name="isEnabled">"Open in Windows Terminal as Administrator" item state.</param>
        public static void OpenWindowsTerminalAdminContext(bool isEnabled)
        {
            try
            {
                var terminalSettings = $"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\\Packages\\Microsoft.WindowsTerminal_8wekyb3d8bbwe\\LocalState\\settings.json";
                var deserializedSettings = JsonConvert.DeserializeObject(File.ReadAllText(terminalSettings, Encoding.UTF8)) as JObject;
                var elevateSetting = deserializedSettings?.SelectToken("profiles.defaults.elevate");

                if (elevateSetting is null)
                {
                    var defaultsSetting = deserializedSettings!.SelectToken("profiles.defaults") as JObject;
                    defaultsSetting!.Add(new JProperty("elevate", string.Empty));
                    elevateSetting = deserializedSettings!.SelectToken("profiles.defaults.elevate");
                }

                elevateSetting!.Replace(isEnabled);
                File.WriteAllText(terminalSettings, deserializedSettings!.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to write data to \"Microsoft.WindowsTerminal\" configuration file", ex);
            }
        }
    }
}
