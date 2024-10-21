// <copyright file="Accessors.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using System.ServiceProcess;
    using System.Text;
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Models;

    /// <summary>
    /// Gets the os settings.
    /// </summary>
    public static class Accessors
    {
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IXmlService XmlService = App.GetService<IXmlService>();

        /// <summary>
        /// Gets DiagTrack service state.
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
        /// Gets Windows feature "Diagnostic data level" state.
        /// </summary>
        public static int DiagnosticDataLevel()
        {
            var allowTelemetry = Registry.LocalMachine.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\DataCollection")?.GetValue("AllowTelemetry") as int? ?? -1;
            var maxTelemetryAllowed = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\DataCollection")?.GetValue("MaxTelemetryAllowed") as int? ?? -1;
            var showedToastLevel = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack")?.GetValue("ShowedToastAtLevel") as int? ?? -1;
            return allowTelemetry.Equals(1) && maxTelemetryAllowed.Equals(1) && showedToastLevel.Equals(1) ? 2 : 1;
        }

        /// <summary>
        /// Gets Windows feature "Error reporting" state.
        /// </summary>
        public static bool ErrorReporting()
        {
            var reportingTask = ScheduledTaskService.GetTaskOrDefault("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting") ?? throw new InvalidOperationException($"Failed to find a scheduled task");
            var werDisabledValue = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\Windows Error Reporting")?.GetValue("Disabled") as int? ?? -1;
            var werService = new ServiceController("WerSvc");
            return !(reportingTask.State == TaskState.Disabled && werDisabledValue.Equals(1) && werService.StartType == ServiceStartMode.Disabled);
        }

        /// <summary>
        /// Gets Windows feature "Feedback frequency" state.
        /// </summary>
        public static int FeedbackFrequency()
        {
            var siufPeriod = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Siuf\\Rules")?.GetValue("NumberOfSIUFInPeriod") as int? ?? -1;
            return siufPeriod.Equals(0) ? 2 : 1;
        }

        /// <summary>
        /// Gets telemetry scheduled tasks state.
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
        /// Gets Windows feature "Sign-in info" state.
        /// </summary>
        public static bool SigninInfo()
        {
            var userSid = InstrumentationService.GetUserSid(Environment.UserName);
            var userArso = Registry.LocalMachine.OpenSubKey($"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{userSid}")?.GetValue("OptOut") ?? -1;
            return !userArso.Equals(1);
        }

        /// <summary>
        /// Gets language list access state.
        /// </summary>
        public static bool LanguageListAccess()
        {
            var httpAcceptLanguage = Registry.CurrentUser.OpenSubKey("Control Panel\\International\\User Profile")?.GetValue("HttpAcceptLanguageOptOut") as int? ?? -1;
            return !httpAcceptLanguage.Equals(1);
        }

        /// <summary>
        /// Gets the permission for apps to use advertising ID state.
        /// </summary>
        public static bool AdvertisingID()
        {
            var advertisingInfo = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")?.GetValue("Enabled") as int? ?? -1;
            return !advertisingInfo.Equals(0);
        }

        /// <summary>
        /// Gets the Windows welcome experiences state.
        /// </summary>
        public static bool WindowsWelcomeExperience()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-310093Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Gets Windows tips state.
        /// </summary>
        public static bool WindowsTips()
        {
            var subscribedContent = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SubscribedContent-338389Enabled") as int? ?? -1;
            return !subscribedContent.Equals(0);
        }

        /// <summary>
        /// Gets the suggested content in the Settings app state.
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
        /// Gets the automatic installing suggested apps state.
        /// </summary>
        public static bool AppsSilentInstalling()
        {
            var appsIsSilentInstalled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager")?.GetValue("SilentInstalledAppsEnabled") as int? ?? -1;
            return !appsIsSilentInstalled.Equals(0);
        }

        /// <summary>
        /// Gets the Windows feature "Whats New" state.
        /// </summary>
        public static bool WhatsNewInWindows()
        {
            var scoobeSettingIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")?.GetValue("ScoobeSystemSettingEnabled") as int? ?? -1;
            return !scoobeSettingIsEnabled.Equals(0);
        }

        /// <summary>
        /// Gets Windows feature "Tailored experiences" state.
        /// </summary>
        public static bool TailoredExperiences()
        {
            var tailoredExperiencesIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy")?.GetValue("TailoredExperiencesWithDiagnosticDataEnabled") as int? ?? -1;
            return !tailoredExperiencesIsEnabled.Equals(0);
        }

        /// <summary>
        /// Gets Windows feature "Bing search" state.
        /// </summary>
        public static bool BingSearch()
        {
            var searchBoxIsDisabled = Registry.CurrentUser.OpenSubKey("Software\\Policies\\Microsoft\\Windows\\Explorer")?.GetValue("DisableSearchBoxSuggestions") as int? ?? -1;
            return !searchBoxIsDisabled.Equals(1);
        }

        /// <summary>
        /// Gets Start menu recommendations state.
        /// </summary>
        public static bool StartRecommendationsTips()
        {
            var irisPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var irisRecommendations = Registry.CurrentUser.OpenSubKey(irisPath)?.GetValue("Start_IrisRecommendations") as int? ?? -1;
            return !irisRecommendations.Equals(0);
        }

        /// <summary>
        /// Gets Start Menu notifications state.
        /// </summary>
        public static bool StartAccountNotifications()
        {
            var notificationsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced";
            var accountNotifications = Registry.CurrentUser.OpenSubKey(notificationsPath)?.GetValue("Start_AccountNotifications") as int? ?? -1;
            return !accountNotifications.Equals(0);
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
                throw new InvalidOperationException($"Necessary appx package are not installed \"{photos}\"");
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
            var appxCortanaIsExist = AppxPackagesService.PackageExist("Microsoft.549981C3F5F10");

            if (appxCortanaIsExist)
            {
                var pathCortana = "Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\Microsoft.549981C3F5F10_8wekyb3d8bbwe\\CortanaStartupId";
                var stateCortana = Registry.ClassesRoot.OpenSubKey(pathCortana)?.GetValue("State") as int? ?? -1;
                return stateCortana != 1;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed \"Cortana\"");
        }

        /// <summary>
        /// Gets Teams auto start state.
        /// </summary>
        public static bool TeamsAutostart()
        {
            var appxTeamsIsExist = AppxPackagesService.PackageExist("MSTeams");

            if (appxTeamsIsExist)
            {
                var pathTeams = "Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\AppModel\\SystemAppData\\MicrosoftTeams_8wekyb3d8bbwe\\TeamsStartupTask";
                var stateTeams = Registry.CurrentUser.OpenSubKey(pathTeams)?.GetValue("State") as int? ?? -1;
                return stateTeams != 1;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed \"Microsoft Teams\"");
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
            var appxGamingIsExist = AppxPackagesService.PackageExist("Microsoft.GamingApp");

            if (appxGamingIsExist)
            {
                var startupPanelIsEnabled = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\GameBar")?.GetValue("ShowStartupPanel") as int? ?? -1;
                return startupPanelIsEnabled == 1;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed \"{appGaming}\"");
        }

        /// <summary>
        /// Gets GPU scheduling state.
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

            var conditions = $"DAC type is external: {isExternalDACType}, is virtual machine: {isVirtualMachine}, WDDM Version (minimal {WDDMMinimalVersion}): {wddmVersion}";
            throw new InvalidOperationException($"One of more mandatory conditions are not met, {conditions}");
        }

        /// <summary>
        /// Get scheduled task "Windows Cleanup" state.
        /// </summary>
        public static bool CleanupTask()
        {
            var cleanupTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup");

            if (cleanupTask is not null && cleanupTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The \"Windows Cleanup\" scheduled task was already created as {cleanupTask.Definition.Principal.UserId}");
            }

            return cleanupTask is not null && cleanupTask.State != TaskState.Disabled && cleanupTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Get scheduled task "SoftwareDistribution" state.
        /// </summary>
        public static bool SoftwareDistributionTask()
        {
            var distributionTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\SoftwareDistribution");

            if (distributionTask is not null && distributionTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The \"SoftwareDistribution\" scheduled task was already created as {distributionTask.Definition.Principal.UserId}");
            }

            return distributionTask is not null && distributionTask.State != TaskState.Disabled && distributionTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Get scheduled task "Temp" state.
        /// </summary>
        public static bool TempTask()
        {
            var tempTask = ScheduledTaskService.GetTaskOrDefault("Sophia\\TempTask");

            if (tempTask is not null && tempTask.Definition.Principal.UserId != Environment.UserName)
            {
                throw new InvalidOperationException($"The \"Temp\" scheduled task was already created as {tempTask.Definition.Principal.UserId}");
            }

            return tempTask is not null && tempTask.State != TaskState.Disabled && tempTask.State != TaskState.Unknown;
        }

        /// <summary>
        /// Gets Windows network protection state.
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
        /// Gets Windows PUApps detection state.
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
        /// Gets Microsoft Defender sandbox state.
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
        /// Gets Windows event viewer custom view state.
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
        /// Gets Windows PowerShell modules logging state.
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
        /// Gets Windows PowerShell scripts logging state.
        /// </summary>
        public static bool PowerShellScriptsLogging()
        {
            var scriptLoggingPath = "Software\\Policies\\Microsoft\\Windows\\PowerShell\\ScriptBlockLogging";
            var scriptLogging = Registry.LocalMachine.OpenSubKey(scriptLoggingPath)?.GetValue("EnableScriptBlockLogging") as int? ?? -1;
            return scriptLogging.Equals(1);
        }

        /// <summary>
        /// Gets Windows SmartScreen state.
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
        /// Gets Windows save zone state.
        /// </summary>
        public static bool SaveZoneInformation()
        {
            var saveZonePath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments";
            var saveZoneInformation = Registry.CurrentUser.OpenSubKey(saveZonePath)?.GetValue("SaveZoneInformation") as int? ?? -1;
            return saveZoneInformation.Equals(1);
        }

        /// <summary>
        /// Gets Windows script host state.
        /// </summary>
        public static bool WindowsScriptHost()
        {
            var blockingTasks = new[] { "SoftwareDistribution", "Temp", "Windows Cleanup", "Windows Cleanup Notification" };
            var blockingTasksExist = ScheduledTaskService.FindTaskOrDefault(blockingTasks).Any(task => task?.State == TaskState.Ready);
            var scriptHostPath = "Software\\Microsoft\\Windows Script Host\\Settings";
            var scriptHostIsEnabled = Registry.CurrentUser.OpenSubKey(scriptHostPath)?.GetValue("Enabled") as int? ?? -1;
            return blockingTasksExist ? throw new InvalidOperationException("One of the blocking tasks is in \"Ready\" state.") : !scriptHostIsEnabled.Equals(0);
        }

        /// <summary>
        /// Gets Windows Sandbox state.
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
                var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationIsEnabled() ?? throw new InvalidOperationException("This cpu does not support virtualization");
                var hypervisorPresent = InstrumentationService.HypervisorPresent() ?? throw new InvalidOperationException("Enable Virtualization in UEFI");

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
        /// Gets Local Security Authority state.
        /// </summary>
        public static bool LocalSecurityAuthority()
        {
            var virtualizationIsEnabled = InstrumentationService.CpuVirtualizationIsEnabled() ?? throw new InvalidOperationException("This cpu does not support virtualization");
            var hypervisorPresent = InstrumentationService.HypervisorPresent() ?? throw new InvalidOperationException("Enable Virtualization in UEFI");
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

            throw new InvalidOperationException($"Necessary appx package are not installed \"{clipChampAppx}\"");
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

            throw new InvalidOperationException($"Necessary appx package are not installed \"{appxPaint}\"");
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

            throw new InvalidOperationException($"File \"{paintPath}\" not exist");
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

            throw new InvalidOperationException($"File \"{wordpadPath}\" not exist");
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

            throw new InvalidOperationException($"Necessary appx package are not installed \"{appxTerminal}\"");
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
                        throw new InvalidOperationException($"The configuration file of appx package \"{appxTerminal}\" is not valid");
                    }
                }

                return true;
            }

            throw new InvalidOperationException($"Necessary appx package are not installed \"{appxTerminal}\"");
        }
    }
}
