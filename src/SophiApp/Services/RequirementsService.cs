// <copyright file="RequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using System.Net.Http.Json;
    using System.Security.Principal;
    using System.ServiceProcess;
    using CSharpFunctionalExtensions;
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class RequirementsService : IRequirementsService
    {
        private readonly ICommonDataService commonDataService;
        private readonly IInstrumentationService instrumentationService;
        private readonly IAppxPackagesService appxPackagesService;
        private readonly IAppNotificationService appNotificationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsService"/> class.
        /// </summary>
        /// <param name="commonDataService">A service for working with common app data.</param>
        /// <param name="instrumentationService">A service for working with WMI.</param>
        /// <param name="appxPackagesService">A service for working with appx packages.</param>
        /// <param name="appNotificationService">A service for working with notifications.</param>
        public RequirementsService(
            ICommonDataService commonDataService,
            IInstrumentationService instrumentationService,
            IAppxPackagesService appxPackagesService,
            IAppNotificationService appNotificationService)
        {
            this.commonDataService = commonDataService;
            this.instrumentationService = instrumentationService;
            this.appxPackagesService = appxPackagesService;
            this.appNotificationService = appNotificationService;
        }

        /// <inheritdoc/>
        public Result GetOsBitness()
        {
            return Environment.Is64BitOperatingSystem ? Result.Success() : Result.Failure(nameof(RequirementsFailure.Is32BitOs));
        }

        /// <inheritdoc/>
        public Result GetWmiState()
        {
            try
            {
                var repoState = "chcp 437 | winmgmt /verifyrepository".InvokeAsCmd();
                var serviceIsRun = new ServiceController("Winmgmt").Status == ServiceControllerStatus.Running;
                var repoIsConsistent = repoState.Equals("WMI repository is consistent\n");
                var osCaptionIsCorrect = !string.IsNullOrEmpty(commonDataService.OsProperties.Caption);

                // TODO: Log WMI state here!
                return serviceIsRun && repoIsConsistent && osCaptionIsCorrect ? Result.Success() : Result.Failure(nameof(RequirementsFailure.WMIBroken));
            }
            catch (Exception)
            {
                // TODO: Log error here!
                return Result.Failure(nameof(RequirementsFailure.WMIBroken));
            }
        }

        /// <inheritdoc/>
        public Result GetOsVersion()
        {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous

            return commonDataService.OsProperties.BuildNumber switch
            {
                var build when commonDataService.IsWindows11 && build < 22631 => Result.Failure(nameof(RequirementsFailure.Win11BuildLess22631)),
                var build when commonDataService.IsWindows11 && build.Equals(22631) => Result.Failure(nameof(RequirementsFailure.Win11BuildEqual22631)),
                var build when !build.Equals(19045) && commonDataService.OsProperties.Edition.Contains("EnterpriseS", StringComparison.InvariantCultureIgnoreCase) => Result.Failure(nameof(RequirementsFailure.Win10EnterpriseSVersion)),
                var build when !build.Equals(19045) => Result.Failure(nameof(RequirementsFailure.Win10UnsupportedBuild)),
                var build when build.Equals(19045) && commonDataService.OsProperties.UpdateBuildRevision < 3448 => Result.Failure(nameof(RequirementsFailure.Win10UpdateBuildRevisionLess3448)),
                _ => Result.Success()
            };

#pragma warning restore S2589 // Boolean expressions should not be gratuitous
        }

        /// <inheritdoc/>
        public Result AppRunFromLoggedUser()
        {
            var currentUserName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            var loggedUserProcess = Array.Find(array: Process.GetProcesses(), match: p => p.ProcessName.Equals("explorer") && p.SessionId.Equals(Process.GetCurrentProcess().SessionId));
            return instrumentationService.GetProcessOwner(loggedUserProcess).Equals(currentUserName) ? Result.Success() : Result.Failure(nameof(RequirementsFailure.RunByNotLoggedUser));
        }

        /// <inheritdoc/>
        public Result MalwareDetection()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var programData = Environment.ExpandEnvironmentVariables("%ProgramData%");
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var system32Folder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var systemDrive = Environment.ExpandEnvironmentVariables("%SystemDrive%");
            var systemRoot = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            var tempFolder = Environment.ExpandEnvironmentVariables("%TEMP%");

            var malwares = new Dictionary<string, Func<bool>>()
            {
                { "OsRequirements_Malware_Windows10Debloater", () => Directory.Exists($"{systemDrive}\\Temp\\Windows10Debloater") },
                { "OsRequirements_Malware_Win10BloatRemover", () => Directory.Exists($"{tempFolder}\\.net\\Win10BloatRemover") },
                {
                    "OsRequirements_Malware_BloatwareRemoval", () =>
                    {
                        try
                        {
                            return Directory.GetFileSystemEntries(path: $"{systemDrive}\\BRU", searchPattern: "Bloatware-Removal*.log").Length > 0;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                },
                { "OsRequirements_Malware_GhostToolbox", () => File.Exists($"{system32Folder}\\migwiz\\dlmanifests\\run.ghost.cmd") },
                {
                    "OsRequirements_Malware_Optimizer", () =>
                    {
                        var downloadFolder = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders")?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") as string;
                        return downloadFolder is not null && Directory.Exists($"{downloadFolder}\\OptimizerDownloads");
                    }
                },
                { "OsRequirements_Malware_Win10Tweaker", () => Registry.CurrentUser.OpenSubKey("Software\\Win 10 Tweaker") is not null },
                { "OsRequirements_Malware_ModernTweaker", () => Registry.ClassesRoot.OpenSubKey("CLSID\\{645FF040-5081-101B-9F08-00AA002F954E}\\shell\\Modern Cleaner") is not null },
                { "OsRequirements_Malware_BoosterX", () => File.Exists($"{programFiles}\\GameModeX\\GameModeX.exe") },
                { "OsRequirements_Malware_DefenderControl", () => Directory.Exists($"{appData}\\Defender Control") },
                { "OsRequirements_Malware_DefenderSwitch", () => Directory.Exists($"{programData}\\DSW") },
                { "OsRequirements_Malware_RevisionTool", () => Directory.Exists($"{programFilesX86}\\Revision Tool") },
                {
                    "OsRequirements_Malware_WinterOsTweaker", () =>
                    {
                        try
                        {
                            return Directory.GetFileSystemEntries(path: $"{systemRoot}", searchPattern: "WinterOS*").Length > 0;
                        }
                        catch (Exception)
                        {
                            return false;
                        }
                    }
                },
                { "OsRequirements_Malware_WinCry", () => File.Exists($"{systemRoot}\\TempCleaner.exe") },
                {
                    "OsRequirements_Malware_FlibustierWindowsImage", () =>
                    {
                        var values = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\.NETFramework\\Performance")?.GetValueNames() ?? new string[0];
                        return Array.Exists(values, k => k.Contains("flibustier"));
                    }
                },
            };

            return malwares.Any(malware =>
            {
                if (malware.Value.Invoke())
                {
                    commonDataService.DetectedMalware = malware.Key.GetLocalized();
                    return true;
                }

                return false;
            }) ? Result.Failure(nameof(RequirementsFailure.MalwareDetected)) : Result.Success();
        }

        /// <inheritdoc/>
        public Result GetFeatureExperiencePackState()
        {
            return appxPackagesService.PackageExist("MicrosoftWindows.Client.CBS") ? Result.Success() : Result.Failure(nameof(RequirementsFailure.FeatureExperiencePackRemoved));
        }

        /// <inheritdoc/>
        public Result GetPendingRebootState()
        {
            var rebootParameters = new List<Func<bool>>()
            {
                () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootPending") is not null,
                () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootInProgress") is not null,
                () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\PackagesPending") is not null,
                () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\PostRebootReporting") is not null,
                () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\RebootRequired") is not null,
            };

            return rebootParameters.Exists(p => p.Invoke()) ? Result.Failure(nameof(RequirementsFailure.RebootRequired)) : Result.Success();
        }

        /// <inheritdoc/>
        public Result UpdateDetection()
        {
            // TODO: Log online state.
            if (commonDataService.IsOnline)
            {
                try
                {
                    using var client = new HttpClient();
                    var wrapper = client.GetFromJsonAsync<AppVersionWrapper>(commonDataService.AppVersionUrl).Result;
                    var wrapperVersion = wrapper?.SophiApp_release ?? new Version(0, 0, 0);

                    if (wrapperVersion > commonDataService.AppVersion)
                    {
                        var payload = string.Format("AppUpdateNotification".GetLocalized(), wrapperVersion.ToString(3), commonDataService.AppReleaseUrl);
                        appNotificationService.Show(payload);
                    }
                }
                catch (Exception e)
                {
                    // TODO: Log exception.
                    throw;
                }
            }

            return Result.Success();
        }

        /// <inheritdoc/>
        public Result GetMsDefenderFilesExist()
        {
            var system32Folder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var components = new List<string>()
            {
                $"{system32Folder}\\smartscreen.exe",
                $"{system32Folder}\\SecurityHealthSystray.exe",
                $"{system32Folder}\\CompatTelRunner.exe",
            };

            return components.TrueForAll(component =>
            {
                if (File.Exists(component))
                {
                    return true;
                }

                commonDataService.MsDefenderFileMissing = component;
                return false;
            }) ? Result.Success() : Result.Failure(nameof(RequirementsFailure.MsDefenderFilesMissing));
        }

        /// <inheritdoc/>
        public Result GetMsDefenderServicesState()
        {
            var services = new List<string>() { "Windefend", "SecurityHealthService", "Wscsvc" };
            var serviceName = string.Empty;

            return services.TrueForAll(s =>
            {
                serviceName = $"MsDefenderService_{s}";

                try
                {
                    var service = new ServiceController(s);

                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        return true;
                    }

                    commonDataService.MsDefenderServiceStopped = serviceName.GetLocalized();
                    return false;
                }
                catch (Exception)
                {
                    // TODO: Log error here!
                    commonDataService.MsDefenderServiceStopped = serviceName.GetLocalized();
                    return false;
                }
            }) ? Result.Success() : Result.Failure(nameof(RequirementsFailure.MsDefenderServiceStopped));
        }

        /// <inheritdoc/>
        public Result GetMsDefenderState()
        {
            var isEnterpriseG = commonDataService.OsProperties.Edition.Contains("EnterpriseG", StringComparison.InvariantCultureIgnoreCase);
            var msDefenderProductState = instrumentationService.GetAntivirusProducts().Find(product => product.GetPropertyValue("instanceGuid")
            .Equals("{D68DDC3A-831F-4fae-9E44-DA132C1ACF46}"))?.GetPropertyValue("productState");
            var msDefenderState = msDefenderProductState is null ? "00" : string.Format("0x{0:x}", msDefenderProductState).Substring(3, 2);
            var msDefenderEnabled = !(msDefenderState.Equals("00") || msDefenderState.Equals("01")) && !isEnterpriseG;
            var msDefenderAntiSpywareEnabled = !Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender")?.GetValue("DisableAntiSpyware", 0) !.Equals(1) ?? false;
            var msDefenderRealtimeMonitoringEnabled = !Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection")?.GetValue("DisableRealtimeMonitoring", 0) !.Equals(1) ?? true;
            var msDefenderBehaviorMonitoringEnabled = !Registry.LocalMachine.OpenSubKey("SOFTWARE\\Policies\\Microsoft\\Windows Defender\\Real-Time Protection")?.GetValue("DisableBehaviorMonitoring", 0) !.Equals(1) ?? true;
            return msDefenderEnabled && msDefenderAntiSpywareEnabled && msDefenderRealtimeMonitoringEnabled && msDefenderBehaviorMonitoringEnabled ? Result.Success() : Result.Failure(nameof(RequirementsFailure.MsDefenderIsBroken));
        }
    }
}
