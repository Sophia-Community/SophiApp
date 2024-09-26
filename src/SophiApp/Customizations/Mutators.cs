// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;

    /// <summary>
    /// Set the os settings.
    /// </summary>
    public static class Mutators
    {
        private static readonly IAppNotificationService AppNotificationService = App.GetService<IAppNotificationService>();
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IFileService FileService = App.GetService<IFileService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
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
            var reportingPoliciesPath = "Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting";
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            var reportingService = new ServiceController("WerSvc");

            Registry.LocalMachine.OpenSubKey(reportingPoliciesPath, true)?.DeleteValue("Disabled", false);
            Registry.CurrentUser.OpenSubKey(reportingPoliciesPath, true)?.DeleteValue("Disabled", false);

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
            var siufRulesPath = "Software\\Microsoft\\Siuf\\Rules";

            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection", true)?.DeleteValue("DoNotShowFeedbackNotifications", false);

            if (state.Equals(2))
            {
                Registry.CurrentUser.OpenOrCreateSubKey(siufRulesPath)
                    .SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKey(siufRulesPath, false);
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

            Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true)?.DeleteValue("DisableAutomaticRestartSignOn", false);

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
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo", true)
                ?.DeleteValue("DisabledByGroupPolicy", false);

            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")
                .SetValue("Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
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
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\CloudContent", true)
                ?.DeleteValue("DisableSoftLanding", false);

            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-338389Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
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
            Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\CloudContent", true)
                ?.DeleteValue("DisableWindowsConsumerFeatures", false);

            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SilentInstalledAppsEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="isEnabled">Whats New state.</param>
        public static void WhatsNewInWindows(bool isEnabled)
        {
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")
                .SetValue("ScoobeSystemSettingEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="isEnabled">Tailored experiences state.</param>
        public static void TailoredExperiences(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\CloudContent", true)
                ?.DeleteValue("DisableTailoredExperiencesWithDiagnosticData", false);

            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", true)
                ?.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
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
