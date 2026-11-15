// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <summary>
    /// Set OS settings.
    /// </summary>
    public static class Mutators
    {
        private static readonly IAppNotificationService AppNotificationService = App.GetService<IAppNotificationService>();
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService DataService = App.GetService<ICommonDataService>();
        private static readonly ICursorsService CursorsService = App.GetService<ICursorsService>();
        private static readonly IRedistributablePackageService RedistributablePackageService = App.GetService<IRedistributablePackageService>();
        private static readonly IFileService FileService = App.GetService<IFileService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IGroupPolicyService GroupPolicyService = App.GetService<IGroupPolicyService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly INetworkAdapterService NetworkAdapterService = App.GetService<INetworkAdapterService>();
        private static readonly IOneDriveService OneDriveService = App.GetService<IOneDriveService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IRegistryService RegistryService = App.GetService<IRegistryService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IUpdateService UpdateService = App.GetService<IUpdateService>();

        /// <summary>
        /// Set DiagTrack service state.
        /// </summary>
        /// <param name="enable">DiagTrack service state.</param>
        public static void DiagTrackService(bool enable)
        {
            // Connected User Experiences and Telemetry
            // Disabling the "Connected User Experiences and Telemetry" service (DiagTrack) can cause you not being able to get XBOX achievements anymore and affects Feedback Hub
            using var diagTrackService = new System.ServiceProcess.ServiceController("DiagTrack");
            // The firewall rules may be missing
            var firewallRules = FirewallService.GetGroupRules("DiagTrack").ToList();

            if (enable)
            {
                OsService.SetStartMode(diagTrackService, ServiceStartMode.Automatic);
                diagTrackService.TryStart();

                // Allow connection for the Unified Telemetry Client Outbound Traffic
                firewallRules.ForEach(rule =>
                {
                    rule.Enabled = true;
                    rule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                });
                return;
            }

            diagTrackService.TryStop();
            OsService.SetStartMode(diagTrackService, ServiceStartMode.Disabled);

            // Block connection for the Unified Telemetry Client Outbound Traffic
            firewallRules.ForEach(rule =>
            {
                rule.Enabled = true;
                rule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            });
        }

        /// <summary>
        /// Set Windows feature "Diagnostic data level" state.
        /// </summary>
        /// <param name="state">Diagnostic data level state.</param>
        public static void DiagnosticDataLevel(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");

            if (state.Equals(2))
            {
                if (DataService.OsProperties.Edition.Contains("Enterprise") || DataService.OsProperties.Edition.Contains("Education"))
                {
                    // 0 — Diagnostic data off
                    Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                }
                else
                {
                    // 1 — Send required diagnostic data
                    Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 1, RegistryValueKind.DWord);
                }

                Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 1, RegistryValueKind.DWord);
                return;
            }

            // 3 — Send optional diagnostic data
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Error reporting" state.
        /// </summary>
        /// <param name="enable">Feature state.</param>
        public static void ErrorReporting(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");

            if (enable)
            {
                ScheduledTaskService.SetState(task: reportingTask, enabled: true);
                OsService.SetStartMode(werService, ServiceStartMode.Manual);
                werService.TryStart();
                _ = PowerShellService.Invoke("Enable-WindowsErrorReporting");
                return;
            }

            ScheduledTaskService.TryStop(task: reportingTask);
            ScheduledTaskService.SetState(task: reportingTask, enabled: false);
            werService.TryStop();
            OsService.SetStartMode(werService, ServiceStartMode.Disabled);
            _ = PowerShellService.Invoke("Disable-WindowsErrorReporting");
        }

        /// <summary>
        /// Set Windows feature "Feedback frequency" state.
        /// </summary>
        /// <param name="state">Feedback frequency state.</param>
        public static void FeedbackFrequency(int state)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "DoNotShowFeedbackNotifications");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "DoNotShowFeedbackNotifications");
            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Siuf\\Rules", "PeriodInNanoSeconds");

            if (state.Equals(2))
            {
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Siuf\\Rules", "NumberOfSIUFInPeriod");
        }

        /// <summary>
        /// Set telemetry scheduled tasks state.
        /// </summary>
        /// <param name="enable">Scheduled tasks state.</param>
        public static void ScheduledTasks(bool enable)
        {
            new List<TaskScheduler.Task?>()
             {
                // Gathers Win32 application data for App Backup scenario
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
                // Collects program telemetry information if opted-in to the Microsoft Customer Experience Improvement Program
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
                // Collects program telemetry information if opted-in to the Microsoft Customer Experience Improvement Program
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser Exp"),
                // Scans startup entries and raises notification to the user if there are too many startup entries
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                // This task collects and uploads autochk SQM data if opted-in to the Microsoft Customer Experience Improvement Program
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Autochk\\Proxy"),
                // If the user has consented to participate in the Windows Customer Experience Improvement Program, this job collects and sends usage data to Microsoft
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                // The USB CEIP (Customer Experience Improvement Program) task collects Universal Serial Bus related statistics and information about your machine and sends it to the Windows Device Connectivity engineering group at Microsoft
                // The information received is used to help improve the reliability, stability, and overall functionality of USB in Windows
                // If the user has not consented to participate in Windows CEIP, this task does not do anything
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                // The Windows Disk Diagnostic reports general disk and system information to Microsoft for users participating in the Customer Experience Program
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                // This task shows various Map related toasts
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                // This task checks for updates to maps which you have downloaded for offline use
                ScheduledTaskService.GetTaskOrDefault("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
             }
            .ForEach(task => ScheduledTaskService.SetState(task, enable));
        }

        /// <summary>
        /// Set Windows feature "Sign-in info" state.
        /// </summary>
        /// <param name="enable">Sign-in info state.</param>
        public static void SigninInfo(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableAutomaticRestartSignOn");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", "DisableAutomaticRestartSignOn");
            var userSid = InstrumentationService.GetUserSid(Environment.UserName);
            var arsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}";

            if (enable)
            {
                Registry.LocalMachine.DeleteSubKeyValue(arsoPath, "OptOut");
                return;
            }

            Registry.LocalMachine.CreateKeyAndSetValue(arsoPath, "OptOut", 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set language list access state.
        /// </summary>
        /// <param name="enable">Language list state.</param>
        public static void LanguageListAccess(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Control Panel\\International\\User Profile", "HttpAcceptLanguageOptOut");
                return;
            }

            Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\International\\User Profile", "HttpAcceptLanguageOptOut", 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the permission for apps to use advertising ID state.
        /// </summary>
        /// <param name="enable">Advertising ID state.</param>
        public static void AdvertisingID(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo", "DisabledByGroupPolicy");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo", "DisabledByGroupPolicy");
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo", "Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows welcome experiences state.
        /// </summary>
        /// <param name="enable">Windows welcome experiences state.</param>
        public static void WindowsWelcomeExperience(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-310093Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows tips state.
        /// </summary>
        /// <param name="enable">Windows tips state.</param>
        public static void WindowsTips(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableSoftLanding");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableSoftLanding");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SubscribedContent-338389Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the suggested content in the Settings app state.
        /// </summary>
        /// <param name="enable">Suggested content state.</param>
        public static void SettingsSuggestedContent(bool enable)
        {
            new List<string> { "SubscribedContent-338393Enabled", "SubscribedContent-353694Enabled", "SubscribedContent-353696Enabled" }
            .ForEach(content => Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", content, enable ? 1 : 0, RegistryValueKind.DWord));
        }

        /// <summary>
        /// Set the automatic installing suggested apps state.
        /// </summary>
        /// <param name="enable">Suggested apps state.</param>
        public static void AppsSilentInstalling(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableWindowsConsumerFeatures");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableWindowsConsumerFeatures");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", "SilentInstalledAppsEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="enable">Whats New state.</param>
        public static void WhatsNewInWindows(bool enable)
        {
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement", "ScoobeSystemSettingEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="enable">Tailored experiences state.</param>
        public static void TailoredExperiences(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.CurrentUser, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableTailoredExperiencesWithDiagnosticData");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\CloudContent", "DisableTailoredExperiencesWithDiagnosticData");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Bing search" state.
        /// </summary>
        /// <param name="enable">Bing search state.</param>
        public static void BingSearch(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "DisableSearchBoxSuggestions", 1, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(scope: LGPOScope.User, path: "Software\\Policies\\Microsoft\\Windows\\Explorer", name: "DisableSearchBoxSuggestions", type: "DWORD", value: "1");
        }

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
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\Windows Search", "DisableSearch", "SearchOnTaskbarMode");

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
        /// Set Print screen button state.
        /// </summary>
        /// <param name="enable">Print screen button state.</param>
        public static void PrtScnSnippingTool(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\Keyboard", "PrintScreenKeyForSnippingEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set input method for app window state.
        /// </summary>
        /// <param name="enable">Input method for app window state.</param>
        public static void AppsLanguageSwitch(bool enable)
        {
            _ = PowerShellService.Invoke(enable ? "Set-WinLanguageBarOption -UseLegacySwitchMode" : "Set-WinLanguageBarOption");
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
                    CursorsService.SetJepriCreationsCursors(JepriCursorsTheme.Dark);
                    break;

                case 2:
                    CursorsService.SetJepriCreationsCursors(JepriCursorsTheme.Light);
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
        /// Set OneDrive state.
        /// </summary>
        /// <param name="enable">OneDrive state.</param>
        public static void OneDrive(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive", "DisableFileSyncNGSC");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive", "DisableFileSyncNGSC");

            if (enable)
            {
                OneDriveService.Install();
                return;
            }

            OneDriveService.Uninstall();
        }

        /// <summary>
        /// Set storage sense state.
        /// </summary>
        /// <param name="enable">Storage sense state.</param>
        public static void StorageSense(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\StorageSense", "AllowStorageSenseGlobal");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\StorageSense", "AllowStorageSenseGlobal");

            if (enable)
            {
                // Turn on Storage Sense
                Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "01", 1, RegistryValueKind.DWord);
                // Turn on automatic cleaning up temporary system and app files
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "04", 1, RegistryValueKind.DWord);
                // Run Storage Sense every month
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "2048", 30, RegistryValueKind.DWord);
                return;
            }

            // Turn off Storage Sense
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "01", 0, RegistryValueKind.DWord);
            // Turn off automatic cleaning up temporary system and app files
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "04", 0, RegistryValueKind.DWord);
            // Run Storage Sense during low free disk space
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "2048", 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set hibernation state.
        /// </summary>
        /// <param name="enable">Hibernation state.</param>
        public static void Hibernation(bool enable)
        {
            var powerConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "powercfg.exe");
            _ = ProcessService.WaitForExit(name: powerConfig, arguments: $"/HIBERNATE {(enable ? "ON" : "OFF")}");
        }

        /// <summary>
        /// Set long path limit state.
        /// </summary>
        /// <param name="enable">Long path limit state.</param>
        public static void Win32LongPathsSupport(bool enable)
        {
            if (enable)
            {
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled", 1, RegistryValueKind.DWord);
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(scope: LGPOScope.Computer, path: "SYSTEM\\CurrentControlSet\\Control\\FileSystem", name: "LongPathsEnabled", type: "DWORD", value: "1");

                return;
            }

            Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled", 0, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled");
        }

        /// <summary>
        /// Set BSOD error state.
        /// </summary>
        /// <param name="enable">BSOD error state.</param>
        public static void BSoDStopError(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\CrashControl", "DisplayParameters", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set administrator approval mode state.
        /// </summary>
        /// <param name="state">Administrator approval mode state.</param>
        public static void AdminApprovalMode(int state)
        {
            using var approvalKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
            approvalKey?.DeleteValue("FilterAdministratorToken", false);
            approvalKey?.SetValue("ConsentPromptBehaviorUser", 3, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableInstallerDetection", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("ValidateAdminCodeSignatures", 0, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableSecureUIAPaths", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableLUA", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("PromptOnSecureDesktop", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableVirtualization", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("EnableUIADesktopToggle", 1, RegistryValueKind.DWord);
            approvalKey?.SetValue("ConsentPromptBehaviorAdmin", state.Equals(1) ? 5 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set delivery optimization state.
        /// </summary>
        /// <param name="enable">Delivery optimization state.</param>
        public static void DeliveryOptimization(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DeliveryOptimization", "DODownloadMode");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DeliveryOptimization", "DODownloadMode");
            Registry.Users.SetValueIfKeyExists("S-1-5-20\\Software\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Settings", "DownloadMode", enable ? 1 : 0, RegistryValueKind.DWord);

            if (!enable)
            {
                _ = PowerShellService.Invoke("Delete-DeliveryOptimizationCache -Force");
            }
        }

        /// <summary>
        /// Set Windows manage default printer state.
        /// </summary>
        /// <param name="enable">Default printer manage state.</param>
        public static void WindowsManageDefaultPrinter(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows", "LegacyDefaultPrinterMode");
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows", "LegacyDefaultPrinterMode", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set update Microsoft products state.
        /// </summary>
        /// <param name="enable">Update Microsoft products state.</param>
        public static void UpdateMicrosoftProducts(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "AllowMUUpdateService");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "AllowMUUpdateService");

            if (enable)
            {
                UpdateService.RunMicrosoftProductsUpdate();
                return;
            }

            UpdateService.StopMicrosoftProductsUpdate();
        }

        /// <summary>
        /// Set restart notification state.
        /// </summary>
        /// <param name="enable">Restart notification state.</param>
        public static void RestartNotification(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "SetAutoRestartNotificationDisable");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "SetAutoRestartNotificationDisable");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "RestartNotificationsAllowed2", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set restart device after update state.
        /// </summary>
        /// <param name="enable">Restart device after update state.</param>
        public static void RestartDeviceAfterUpdate(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsExpedited", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set active hours restart state.
        /// </summary>
        /// <param name="state">Active hours restart state.</param>
        public static void ActiveHours(int state)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "NoAutoRebootWithLoggedOnUsers", "AlwaysAutoRebootAtScheduledTime");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "NoAutoRebootWithLoggedOnUsers", "AlwaysAutoRebootAtScheduledTime");
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "SmartActiveHoursState", state.Equals(1) ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows latest update state.
        /// </summary>
        /// <param name="enable">Latest update state.</param>
        public static void WindowsLatestUpdate(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "AllowOptionalContent", "SetAllowOptionalContent");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "AllowOptionalContent", "SetAllowOptionalContent");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsContinuousInnovationOptedIn", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set network adapters power state.
        /// </summary>
        /// <param name="enable">Network adapters power state.</param>
        public static void NetworkAdaptersSavePower(bool enable)
        {
            // Turn on desktop apps access to location: "netsh wlan" requires location permission to get the connected Wi-Fi network SSID
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location", "Value", "Allow", RegistryValueKind.String);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location", "Value", "Allow", RegistryValueKind.String);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\CapabilityAccessManager\\ConsentStore\\location\\NonPackaged", "Value", "Allow", RegistryValueKind.String);

            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\LocationAndSensors", "DisableLocation");
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\AppPrivacy", "LetAppsAccessLocation");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\LocationAndSensors", "DisableLocation");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\AppPrivacy", "LetAppsAccessLocation");

            NetworkAdapterService.SetTurnOffDevice(enable);
        }

        /// <summary>
        /// Set input method state.
        /// </summary>
        /// <param name="state">Input method state.</param>
        public static void InputMethod(int state)
        {
            if (state.Equals(1))
            {
                _ = PowerShellService.Invoke("Set-WinDefaultInputMethodOverride -InputTip \"0409:00000409\"");
                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Control Panel\\International\\User Profile", "InputMethodOverride");
        }

        /// <summary>
        /// Set Print Screen folder state.
        /// </summary>
        /// <param name="state">Print Screen folder state.</param>
        public static void WinPrtScrFolder(int state)
        {
            if (state.Equals(1))
            {
                var desktopPath = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "Desktop") as string;
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}", desktopPath!, RegistryValueKind.ExpandString);

                return;
            }

            Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders", "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}");
        }

        /// <summary>
        /// Set recommended troubleshooting state.
        /// </summary>
        /// <param name="state">Recommended troubleshooting state.</param>
        public static void RecommendedTroubleshooting(int state)
        {
            // Remove all policies in order to make changes visible in UI
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");
            // Recommended troubleshooting requires sending optional diagnostic data
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "MaxTelemetryAllowed", 3, RegistryValueKind.DWord);
            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection", "AllowTelemetry", 3, RegistryValueKind.DWord);
            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack", "ShowedToastAtLevel", 3, RegistryValueKind.DWord);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\DataCollection", "AllowTelemetry");

            // Turn on Windows Error Reporting
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting", "Disabled", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\PCHealth\\ErrorReporting", "DoReport");
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            ScheduledTaskService.SetState(task: reportingTask, enabled: true);
            using var werService = new System.ServiceProcess.ServiceController("WerSvc");
            OsService.SetStartMode(werService, ServiceStartMode.Manual);
            werService.TryStart();
            _ = PowerShellService.Invoke("Enable-WindowsErrorReporting");

            Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\WindowsMitigation", "UserPreference", state.Equals(1) ? 3 : 2, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set reserved storage state.
        /// </summary>
        /// <param name="enable">Reserved storage state.</param>
        public static void ReservedStorage(bool enable)
        {
            var command = enable ? "Set-WindowsReservedStorageState -State Enabled" : "Set-WindowsReservedStorageState -State Disabled";
            _ = PowerShellService.Invoke(command);
        }

        /// <summary>
        /// Set help page state.
        /// </summary>
        /// <param name="enable">Help page state.</param>
        public static void F1HelpPage(bool enable)
        {
            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\Typelib\\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}", false);
                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Classes\\Typelib\\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\\1.0\\0\\win64", string.Empty, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Set Num Lock state.
        /// </summary>
        /// <param name="enable">Num Lock state.</param>
        public static void NumLock(bool enable)
        {
            Registry.Users.SetValueIfKeyExists(".DEFAULT\\Control Panel\\Keyboard", "InitialKeyboardIndicators", $"{(enable ? "2147483650" : "2147483648")}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Caps Lock state.
        /// </summary>
        /// <param name="enable">Caps Lock state.</param>
        public static void CapsLock(bool enable)
        {
            Registry.CurrentUser.DeleteSubKeyValue("Keyboard Layout", "Attributes");

            if (enable)
            {
                Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", "Scancode Map");
                return;
            }

            Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", "Scancode Map", new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 58, 0, 0, 0, 0, 0 }, RegistryValueKind.Binary);
        }

        /// <summary>
        /// Set sticky shift state.
        /// </summary>
        /// <param name="enable">Sticky shift state.</param>
        public static void StickyShift(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Control Panel\\Accessibility\\StickyKeys", "Flags", $"{(enable ? "510" : "506")}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set autoplay state.
        /// </summary>
        /// <param name="enable">Autoplay state.</param>
        public static void Autoplay(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoDriveTypeAutoRun", Registry.LocalMachine, Registry.CurrentUser);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoDriveTypeAutoRun", LGPOScope.Computer, LGPOScope.User);
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers", "DisableAutoplay", enable ? 0 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set thumbnail cache state.
        /// </summary>
        /// <param name="enable">Thumbnail cache state.</param>
        public static void ThumbnailCacheRemoval(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun", enable ? 3 : 0, RegistryValueKind.DWord);
            Registry.LocalMachine.SetValueIfKeyExists("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun", enable ? 3 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set save restartable apps state.
        /// </summary>
        /// <param name="enable">Restartable apps state.</param>
        public static void SaveRestartableApps(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "RestartApps", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set power plan state.
        /// </summary>
        /// <param name="state">Power plan state.</param>
        public static void PowerPlan(int state)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Power\\PowerSettings", "ActivePowerScheme");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Power\\PowerSettings", "ActivePowerScheme");

            var arguments = $"/SETACTIVE {(state.Equals(1) ? "SCHEME_MIN" : "SCHEME_BALANCED")}";
            var powerConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "powercfg.exe");
            _ = ProcessService.WaitForExit(powerConfig, arguments);
        }

        /// <summary>
        /// Set registry backup state.
        /// </summary>
        /// <param name="enable">Registry backup state.</param>
        public static void RegistryBackup(bool enable)
        {
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Schedule\\Maintenance", "MaintenanceDisabled");
            ScheduledTaskService.SetState(ScheduledTaskService.FindTaskOrDefault("RegIdleBackup"), true);

            if (enable)
            {
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Configuration Manager", "EnablePeriodicBackup", 1, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Configuration Manager", "EnablePeriodicBackup");
        }

        /// <summary>
        /// Set restore previous folders state.
        /// </summary>
        /// <param name="enable">Previous folders state.</param>
        public static void RestorePreviousFolders(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "PersistBrowsers", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows Terminal default app state.
        /// </summary>
        /// <param name="state">Windows Terminal state.</param>
        public static void DefaultTerminalApp(int state)
        {
            if (state.Equals(1))
            {
                var appxPackage = AppxPackagesService.GetPackageOrDefault("Microsoft.WindowsTerminal")!;
                var appxPath = $"Software\\Classes\\PackagedCom\\Package\\{appxPackage.Id.FullName}\\Class";

                using var packageClasses = Registry.LocalMachine.OpenSubKey(appxPath);
                packageClasses?.GetSubKeyNames()
                    .ForEach(key =>
                    {
                        switch (Registry.LocalMachine.GetSubKeyValue(Path.Combine(appxPath, key), "ServerId") ?? -1)
                        {
                            case 0:
                                Registry.CurrentUser.CreateKeyAndSetValue("Console\\%%Startup", "DelegationConsole", key, RegistryValueKind.String);

                                break;
                            case 1:
                                Registry.CurrentUser.CreateKeyAndSetValue("Console\\%%Startup", "DelegationTerminal", key, RegistryValueKind.String);

                                break;
                            default:
                                break;
                        }
                    });

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Console\\%%Startup", "DelegationConsole", "{B23D10C0-E52E-411E-9D5B-C09FDF709C7D}", RegistryValueKind.String);
            Registry.CurrentUser.SetValueIfKeyExists("Console\\%%Startup", "DelegationTerminal", "{B23D10C0-E52E-411E-9D5B-C09FDF709C7D}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set clock in notification center state.
        /// </summary>
        /// <param name="enable">Clock state.</param>
        public static void ShowClockInNotificationCenter(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "ShowClockInNotificationCenter", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Download and install latest version .NET 10 desktop runtime from the Microsoft resources.
        /// </summary>
        /// <param name="enable">.NET Desktop install state.</param>
        public static void InstallDotNetRuntime_10(bool enable)
        {
            if (enable)
            {
                var latestVersion = DataService.LatestReleaseNET10!.Version;
                var installerName = $"windowsdesktop-runtime-{latestVersion}-win-x64.exe";
                RedistributablePackageService.Install($"https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/{latestVersion}/{installerName}", installerName, "Microsoft_Windows_Desktop_Runtime*.log");
            }
        }

        /// <summary>
        /// Download and install latest version .NET 11 desktop runtime from the Microsoft resources.
        /// </summary>
        /// <param name="enable">.NET Desktop install state.</param>
        public static void InstallDotNetRuntime_11(bool enable)
        {
            if (enable)
            {
                var latestVersion = DataService.LatestReleaseNET11!.Version;
                var installerName = $"windowsdesktop-runtime-{latestVersion}-win-x64.exe";
                RedistributablePackageService.Install($"https://builds.dotnet.microsoft.com/dotnet/WindowsDesktop/{latestVersion}/{installerName}", installerName, "Microsoft_Windows_Desktop_Runtime*.log");
            }
        }

        /// <summary>
        /// Download and install latest version Visual C++ x86 from the Microsoft resources.
        /// </summary>
        /// <param name="enable">Visual C++ install state.</param>
        public static void InstallVisualC_x86(bool enable)
        {
            if (enable)
            {
                RedistributablePackageService.Install("https://aka.ms/vc14/vc_redist.x86.exe", "vc_redist.x86.exe", "dd_vcredist_*.log");
            }
        }

        /// <summary>
        /// Download and install latest version Visual C++ x64 from the Microsoft resources.
        /// </summary>
        /// <param name="enable">Visual C++ install state.</param>
        public static void InstallVisualC_x64(bool enable)
        {
            if (enable)
            {
                RedistributablePackageService.Install("https://aka.ms/vc14/vc_redist.x64.exe", "vc_redist.x64.exe", "dd_vcredist_*.log");
            }
        }

        /// <summary>
        /// Set Windows AI state.
        /// </summary>
        /// <param name="enable">Windows AI state.</param>
        public static void RemoveWindowsAI(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\WindowsAI", "DisableAIDataAnalysis", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsAI", "AllowRecallEnablement");
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\Windows\\WindowsCopilot", "TurnOffWindowsCopilot", Registry.LocalMachine, Registry.CurrentUser);
            GroupPolicyService.SetRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\PolicyManager\\default\\WindowsAI\\DisableAIDataAnalysis", "value", 0, RegistryValueKind.DWord);
            GroupPolicyService.DeleteRegistryValue("Software\\Policies\\Microsoft\\WindowsStore", "RemoveWindowsStore", Registry.LocalMachine, Registry.CurrentUser);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\WindowsAI", "DisableAIDataAnalysis", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsAI", "AllowRecallEnablement");
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\Windows\\WindowsCopilot", "TurnOffWindowsCopilot", LGPOScope.Computer, LGPOScope.User);
            GroupPolicyService.ClearPolicyCache("Software\\Policies\\Microsoft\\WindowsStore", "RemoveWindowsStore", LGPOScope.Computer, LGPOScope.User);

            // The "enable" parameter means that Windows AI has to be removed
            if (!enable)
            {
                _ = PowerShellService.Invoke("Enable-WindowsOptionalFeature -Online -FeatureName Recall -All -NoRestart");

                // Open the Copilot page in Microsoft Store
                ExplorerHelper.Open("ms-windows-store://pdp/?ProductId=9NHT9RB2F4HD");
                return;
            }

            _ = PowerShellService.Invoke("Disable-WindowsOptionalFeature -Online -FeatureName Recall -NoRestart");
            AppxPackagesService.RemovePackage("Microsoft.Copilot");
        }

        /// <summary>
        /// Set XBOX game bar state.
        /// </summary>
        /// <param name="enable">XBOX game bar state.</param>
        public static void XBOXGameBar(bool enable)
        {
            Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled", enable ? 1 : 0, RegistryValueKind.DWord);
            Registry.CurrentUser.SetValueIfKeyExists("System\\GameConfigStore", "GameDVR_Enabled", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set XBOX game tips state.
        /// </summary>
        /// <param name="enable">XBOX game tips state.</param>
        public static void XBOXGameTips(bool enable)
        {
            if (enable)
            {
                // We need to turn XBOX Game Bar on first
                Registry.CurrentUser.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\GameDVR", "AppCaptureEnabled", 1, RegistryValueKind.DWord);
                Registry.CurrentUser.SetValueIfKeyExists("System\\GameConfigStore", "GameDVR_Enabled", 1, RegistryValueKind.DWord);
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\GameBar", "ShowStartupPanel", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set GPU scheduling state.
        /// </summary>
        /// <param name="enable">GPU scheduling state.</param>
        public static void GPUScheduling(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers", "HwSchMode", enable ? 2 : 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set "Windows Cleanup" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void CleanupTask(bool enable)
        {
            var cleanupTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.vbs");
            var notificationTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.vbs");
            cleanupTaskVbs.TryDelete();
            notificationTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                RegistryService.RemoveVolumeCachesStateFlags();
                RegistryService.SetVolumeCachesStateFlags();
                AppNotificationService.RegisterAsToastSender();
                AppNotificationService.RegisterWindowsCleanupAsToastSender();
                ScheduledTaskService.RegisterCleanupTask();
                ScheduledTaskService.RegisterCleanupNotificationTask();
                return;
            }

            ScheduledTaskService.UnregisterCleanupTask();
            ScheduledTaskService.UnregisterCleanupNotificationTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
            RegistryService.RemoveVolumeCachesStateFlags();
            AppNotificationService.UnregisterCleanupProtocol();
        }

        /// <summary>
        /// Set "SoftwareDistribution" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void SoftwareDistributionTask(bool enable)
        {
            var distributionTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.vbs");
            distributionTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender();
                ScheduledTaskService.RegisterSoftwareDistributionTask();
                return;
            }

            ScheduledTaskService.UnregisterSoftwareDistributionTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }

        /// <summary>
        /// Set "Temp" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void TempTask(bool enable)
        {
            var tempTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.vbs");
            tempTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender();
                ScheduledTaskService.RegisterTempTask();
                return;
            }

            ScheduledTaskService.UnregisterTempTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }

        /// <summary>
        /// Set Windows network protection state.
        /// </summary>
        /// <param name="enable">Network protection state.</param>
        public static void NetworkProtection(bool enable)
        {
            _ = PowerShellService.Invoke($"Set-MpPreference -EnableNetworkProtection {(enable ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Set Windows PUApps detection state.
        /// </summary>
        /// <param name="enable">PUApps detection state.</param>
        public static void PUAppsDetection(bool enable)
        {
            _ = PowerShellService.Invoke($"Set-MpPreference -PUAProtection {(enable ? "Enabled" : "Disabled")}");
        }

        /// <summary>
        /// Set Microsoft Defender sandbox state.
        /// </summary>
        /// <param name="enable">Microsoft Defender sandbox state.</param>
        public static void DefenderSandbox(bool enable)
        {
            // The machine environment variable is set by .NET itself: starting PowerShell to run setx.exe is not needed
            Environment.SetEnvironmentVariable("MP_FORCE_USE_SANDBOX", enable ? "1" : "0", EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Set Windows Event Viewer custom view state.
        /// </summary>
        /// <param name="enable">Event Viewer custom view state.</param>
        public static void EventViewerCustomView(bool enable)
        {
            ProcessService.CloseManagementConsoles("eventvwr.msc");

            if (enable)
            {
                // Enable events auditing generated when a process is created (starts)
                _ = ProcessService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "auditpol.exe"), "/set /subcategory:\"{0CCE922B-69AE-11D9-BED3-505054503030}\" /success:enable /failure:enable");
                // Include command line in process creation events
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled", "DWORD", "1");
                // Enable logging for all Windows PowerShell modules
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*", "*", RegistryValueKind.String);
                Registry.LocalMachine.SetValueIfKeyExists("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*", "SZ", "*");
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging", "DWORD", "1");
                // Enable logging for all PowerShell scripts input to the Windows PowerShell event log
                Registry.LocalMachine.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging", 1, RegistryValueKind.DWord);
                GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging", "DWORD", "1");
                // Create custom "Process Creation" view in the Event Viewer
                var eventsXml = $@"<ViewerConfig>
	<QueryConfig>
		<QueryNode>
			<Name>{"UIModel_EventViewerCustomView_QueryName".GetLocalized()}</Name>
			<Description>{"UIModel_EventViewerCustomView_QueryDescription".GetLocalized()}</Description>
			<QueryList>
				<Query Id=""0"">
					<Select Path=""Microsoft-Windows-PowerShell/Operational"">*[System[(EventID=4103 or EventID=4104)]]</Select>
					<Select Path=""Windows PowerShell"">*[System[(EventID=400 or EventID=403 or EventID=800)]]</Select>
					<Select Path=""Security"">*[System[(EventID=4688)]]</Select>
				</Query>
			</QueryList>
		</QueryNode>
	</QueryConfig>
</ViewerConfig>";
                // Save ProcessCreation.xml in UTF-8 with BOM encoding
                FileService.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml"), eventsXml, Encoding.UTF8);
                return;
            }

            // Disable events auditing generated when a process is created (starts)
            _ = ProcessService.WaitForExit(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "auditpol.exe"), "/set /subcategory:\"{0CCE922B-69AE-11D9-BED3-505054503030}\" /success:disable /failure:disable");
            // Do not include command line in process creation events
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System\\Audit", "ProcessCreationIncludeCmdLine_Enabled");
            // Disable logging for all Windows PowerShell modules
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging");
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging", "EnableModuleLogging");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ModuleLogging\\ModuleNames", "*");
            // Disable logging for all PowerShell scripts input to the Windows PowerShell event log
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging", "EnableScriptBlockLogging");
            // Remove the "Process Creation" custom view in the Event Viewer
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Event Viewer\\Views\\ProcessCreation.xml").TryDelete();
        }

        /// <summary>
        /// Set Windows SmartScreen state.
        /// </summary>
        /// <param name="enable">Windows SmartScreen state.</param>
        public static void AppsSmartScreen(bool enable)
        {
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer", "SmartScreenEnabled", $"{(enable ? "Warn" : "Off")}", RegistryValueKind.String);
        }

        /// <summary>
        /// Set Windows save zone state.
        /// </summary>
        /// <param name="enable">Windows save zone state.</param>
        public static void SaveZoneInformation(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation", 1, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments", "SaveZoneInformation", "DWORD", "1");
        }

        /// <summary>
        /// Set Windows Sandbox state.
        /// </summary>
        /// <param name="enable">Windows Sandbox state.</param>
        public static void WindowsSandbox(bool enable)
            => PowerShellService.Invoke($"{(enable ? "Enable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -All -Online -NoRestart" : "Disable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -Online -NoRestart")}");

        /// <summary>
        /// Set Local Security Authority state.
        /// </summary>
        /// <param name="enable">Local Security Authority state.</param>
        public static void LocalSecurityAuthority(bool enable)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\System", "RunAsPPL");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\System", "RunAsPPL");

            if (enable)
            {
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL", 2, RegistryValueKind.DWord);
                Registry.LocalMachine.SetValueIfKeyExists("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot", 2, RegistryValueKind.DWord);
                return;
            }

            Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPL");
            Registry.LocalMachine.DeleteSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Lsa", "RunAsPPLBoot");
        }

        /// <summary>
        /// Set "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        /// <param name="enable">"Extract all" item state.</param>
        public static void MSIExtractContext(bool enable)
        {
            if (enable)
            {
                Registry.ClassesRoot.CreateKeyAndSetValue("Msi.Package\\shell\\Extract\\Command", string.Empty, "msiexec.exe /a \"%1\" /qb TARGETDIR=\"%1 extracted\"", RegistryValueKind.String);
                Registry.ClassesRoot.SetValueIfKeyExists("Msi.Package\\shell\\Extract", "MUIVerb", "@shell32.dll,-37514", RegistryValueKind.String);
                Registry.ClassesRoot.SetValueIfKeyExists("Msi.Package\\shell\\Extract", "Icon", "%SystemRoot%\\System32\\shell32.dll,-16817", RegistryValueKind.String);

                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree("Msi.Package\\shell\\Extract", false);
        }

        /// <summary>
        /// Set "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        /// <param name="enable">"Install" item state.</param>
        public static void CABInstallContext(bool enable)
        {
            if (enable)
            {
                Registry.ClassesRoot.CreateKeyAndSetValue("CABFolder\\Shell\\runas\\Command", string.Empty, "cmd /c DISM.exe /Online /Add-Package /PackagePath:\"%1\" /NoRestart & pause", RegistryValueKind.String);
                Registry.ClassesRoot.SetValueIfKeyExists("CABFolder\\Shell\\runas", "MUIVerb", "@shell32.dll,-10210", RegistryValueKind.String);
                Registry.ClassesRoot.SetValueIfKeyExists("CABFolder\\Shell\\runas", "HasLUAShield", string.Empty, RegistryValueKind.String);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree("CABFolder\\Shell\\runas", false);
        }

        /// <summary>
        /// Set "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        /// <param name="enable">"Look for an app in the Microsoft Store" items state.</param>
        public static void UseStoreOpenWith(bool enable)
        {
            // Remove all policies in order to make changes visible in UI
            Registry.LocalMachine.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith");
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith");

            if (enable)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith");
                // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
                GroupPolicyService.ClearPolicyCache(LGPOScope.User, "Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith");

                return;
            }

            Registry.CurrentUser.CreateKeyAndSetValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith", 1, RegistryValueKind.DWord);
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(scope: LGPOScope.User, path: "Software\\Policies\\Microsoft\\Windows\\Explorer", name: "NoUseStoreOpenWith", type: "DWORD", value: "1");
        }

        /// <summary>
        /// Set Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        /// <param name="enable">"Open in Windows Terminal as Administrator" item state.</param>
        public static void OpenWindowsTerminalAdminContext(bool enable)
        {
            try
            {
                var terminalSettings = $"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\\Packages\\Microsoft.WindowsTerminal_8wekyb3d8bbwe\\LocalState\\settings.json";

                // Windows Terminal creates the configuration file on its first launch
                if (!File.Exists(terminalSettings))
                {
                    var terminalFolder = AppxPackagesService.GetPackageOrDefault("Microsoft.WindowsTerminal")!.InstalledLocation.Path;
                    using var terminal = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Path.Combine(terminalFolder, "wt.exe")) { UseShellExecute = true });
                    Thread.Sleep(2000);
                    ProcessService.KillProcessByName(3000, "WindowsTerminal");
                }

                var deserializeSettings = JsonConvert.DeserializeObject(File.ReadAllText(terminalSettings, Encoding.UTF8)) as JObject;
                var elevateSetting = deserializeSettings?.SelectToken("profiles.defaults.elevate");

                if (elevateSetting is null)
                {
                    var defaultsSetting = deserializeSettings!.SelectToken("profiles.defaults") as JObject;
                    defaultsSetting!.Add(new JProperty("elevate", string.Empty));
                    elevateSetting = deserializeSettings.SelectToken("profiles.defaults.elevate");
                }

                if (enable)
                {
                    // Unblock the Windows Terminal context menu shell extension
                    Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                    Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                }

                elevateSetting!.Replace(enable);
                File.WriteAllText(terminalSettings, deserializeSettings!.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed write data to terminal configuration file", ex);
            }
        }
    }
}
