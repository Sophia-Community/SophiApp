// <copyright file="RequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Diagnostics;
    using System.Security.Principal;
    using RegistryKey = Microsoft.Win32.RegistryKey;

    /// <inheritdoc/>
    public class RequirementsService : IRequirementsService
    {
        private readonly IAppxPackagesService packagesService;
        private readonly ICommonDataService dataService;
        private readonly IInstrumentationService instrumentationService;
        private readonly IOsService osService;
        private readonly IPowerShellService powerShellService;
        private readonly ISettingsService settingsService;
        private string actionToDebug = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsService"/> class.
        /// </summary>
        /// <param name="dataService">A service for transferring app data between DI layers.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="osService">A service for working with Windows services API.</param>
        /// <param name="packagesService">A service for working with appx packages API.</param>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        /// <param name="settingsService">A service for working with app settings.</param>
        public RequirementsService(
            ICommonDataService dataService,
            IInstrumentationService instrumentationService,
            IOsService osService,
            IAppxPackagesService packagesService,
            IPowerShellService powerShellService,
            ISettingsService settingsService)
        {
            this.dataService = dataService;
            this.instrumentationService = instrumentationService;
            this.osService = osService;
            this.packagesService = packagesService;
            this.powerShellService = powerShellService;
            this.settingsService = settingsService;
        }

        /// <inheritdoc/>
        public List<RequirementAction> GetActions() => [new (action: GetExternalServicesData, displayText: "OsRequirements_GetExternalServicesData".GetLocalized()),
            new (action: GetSupportedArchitecture, displayText: "OsRequirements_GetSupportedArchitecture".GetLocalized()),
            new (action: GetAppNewVersion, displayText: "OsRequirements_GetAppNewVersion".GetLocalized()),
            new (action: GetAppRunFromLoggedUser, displayText: "OsRequirements_GetAppRunFromLoggedUser".GetLocalized()),
            new (action: GetHarmfulTweakers, displayText: "OsRequirements_GetHarmfulTweaker".GetLocalized()),
            new (action: GetHostFileEntries, displayText: "OsRequirements_GetHostFileEntries".GetLocalized()),
            new (action: GetUWPComponents, displayText: "OsRequirements_GetUWPComponents".GetLocalized()),
            new (action:GetDefenderComponents, displayText: "OsRequirements_GetDefenderComponents".GetLocalized()),
            new (action: GetDefenderProperties, displayText: "OsRequirements_GetDefenderProperties".GetLocalized()),
            new (action: GetControlledFolderAccess, displayText: "OsRequirements_GetControlledFolderAccess".GetLocalized()),
            new (action: GetRebootPending, displayText: "OsRequirements_GetRebootPending".GetLocalized()),
            new (action: GetSystemDriveEncryptedBitLocker, displayText: "OsRequirements_GetSystemDriveEncryptedBitLocker".GetLocalized()),
            new (action: GetUEFICertificates, displayText: "OsRequirements_GetUEFICertificates".GetLocalized()),
            new (action: GetWindowsVersion, displayText: "OsRequirements_GetWindowsVersion".GetLocalized()),
            new (action: GetWindowsBuild, displayText: "OsRequirements_GetWindowsBuild".GetLocalized())];

        /// <inheritdoc/>
        public void Initialize() => actionToDebug = settingsService.ReadDebugRequirementAction();

        private RequirementsResult GetExternalServicesData()
        {
            Task.Run(async () => await dataService.GetExternalServicesDataAsync()).Wait();
            return RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetSupportedArchitecture()
        {
            var caption = instrumentationService.GetProcessorCaption();
            dataService.RequirementsResult_1 = string.Format("OsRequirements_UnsupportedArchitecture_1".GetLocalized(), caption);

            if (RunToDebug(nameof(GetSupportedArchitecture)))
            {
                return RequirementsResult.UnsupportedArchitecture;
            }

            if (caption.Contains("AMD64") || caption.Contains("Intel64"))
            {
                return RequirementsResult.AllCorrect;
            }

            return RequirementsResult.UnsupportedArchitecture;
        }

        private RequirementsResult GetAppNewVersion()
        {
            var latestVersion = dataService.LatestAppRelease?.SophiApp_release ?? new Version(0, 0, 0);
            dataService.RequirementsResult_1 = latestVersion.ToString();

            if (RunToDebug(nameof(GetAppNewVersion)) || latestVersion > dataService.AppVersion)
            {
                App.Logger.LogAppNewVersionFound(latestVersion);
                return RequirementsResult.NewAppVersionFound;
            }

            return RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetAppRunFromLoggedUser()
        {
            var appUser = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            var appUserSessionId = Process.GetCurrentProcess().SessionId;
            var explorerUser = Array.Find(array: Process.GetProcesses(), match: p => p.ProcessName.Equals("explorer") && p.SessionId.Equals(appUserSessionId));
            var processOwner = instrumentationService.GetProcessOwnerName(explorerUser);
            dataService.RequirementsResult_1 = processOwner;
            dataService.RequirementsResult_2 = appUser;

            if (RunToDebug(nameof(GetAppRunFromLoggedUser)))
            {
                return RequirementsResult.LoggedInUserNotAdmin;
            }

            return processOwner.Equals(appUser) ? RequirementsResult.AllCorrect : RequirementsResult.LoggedInUserNotAdmin;
        }

        private RequirementsResult GetHarmfulTweakers()
        {
            if (RunToDebug(nameof(GetHarmfulTweakers)))
            {
                dataService.RequirementsResult_1 = "OsRequirements_Malware_Win10Tweaker".GetLocalized();
                return RequirementsResult.HarmfulTweakerFound;
            }

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var system32 = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var malwares = new Dictionary<string, Func<bool>>()
            {
                // https://www.youtube.com/GHOSTSPECTRE
                { "OsRequirements_Malware_GhostToolbox", () => File.Exists($"{system32}\\migwiz\\dlmanifests\\run.ghost.cmd") },
                // https://win10tweaker.ru
                { "OsRequirements_Malware_Win10Tweaker", () => Registry.CurrentUser.OpenSubKey("Software\\Win 10 Tweaker") is not null },
                // https://revi.cc
                { "OsRequirements_Malware_RevisionTool", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Revision Tool")) },
                // https://github.com/Atlas-OS/Atlas
                { "OsRequirements_Malware_AtlasOS", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "AtlasModules")) },
                // https://boosterx.ru
                { "OsRequirements_Malware_BoosterX", () => File.Exists($"{programFiles}\\GameModeX\\GameModeX.exe") },
                // https://www.youtube.com/watch?v=5NBqbUUB1Pk
                { "OsRequirements_Malware_WinClean", () => Directory.Exists($"{programFiles}\\WinClean Plus Apps") },
                // https://pc-np.com
                { "OsRequirements_Malware_PCNP", () => Registry.CurrentUser.OpenSubKey("Software\\PCNP") is not null },
                // https://www.reddit.com/r/TronScript
                { "OsRequirements_Malware_Tron", () => Directory.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%SystemDrive%"), "\\logs\\tron")) },
                // https://crystalcry.ru
                { "OsRequirements_Malware_CrystalCry", () => Registry.LocalMachine.OpenSubKey("Software\\CrystalCry") is not null },
                // https://github.com/es3n1n/defendnot
                { "OsRequirements_Malware_Defendnot", () => Directory.Exists($"{system32}\\Tasks\\defendnot") },
                // https://github.com/zoicware/RemoveWindowsAI
                { "OsRequirements_Malware_RemoveWindowsAI", () => Directory.GetDirectories(path: $"{system32}\\CatRoot", searchPattern: "ZoicwareRemoveWindowsAI*", searchOption: SearchOption.AllDirectories).Length > 0 },
                // https://forum.ru-board.com/topic.cgi?forum=62&topic=30617&start=1600#14
                {
                    "OsRequirements_Malware_AutoSettingsPS", () =>
                    {
                        var exclusions = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Microsoft Defender\\Exclusions\\Paths")?.GetValueNames() ?? [];
                        return Array.Exists(exclusions, key => key.Contains("AutoSettingsPS"));
                    }
                },
                // https://forum.ru-board.com/topic.cgi?forum=5&topic=50519
                {
                    "OsRequirements_Malware_ModernTweaker", () =>
                    {
                        var shellCache = Registry.CurrentUser.OpenSubKey("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\MuiCache")?.GetValueNames() ?? [];
                        return Array.Exists(shellCache, key => key.Contains("ModernTweaker"));
                    }
                },
            };
            return malwares.Any(m =>
            {
                if (m.Value())
                {
                    var detectedMalware = m.Key.GetLocalized();
                    dataService.RequirementsResult_1 = detectedMalware;
                    App.Logger.LogMalwareDetected(detectedMalware);
                    return true;
                }

                return false;
            }) ? RequirementsResult.HarmfulTweakerFound : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetHostFileEntries()
        {
            if (RunToDebug(nameof(GetHostFileEntries)))
            {
                return RequirementsResult.HostsEntriesFound;
            }

            var hostFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            var hostFileEntries = File.ReadAllLines(hostFilePath);
            return hostFileEntries.Any(e => !(string.IsNullOrEmpty(e) || e.StartsWith('#'))) ? RequirementsResult.HostsEntriesFound : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetUWPComponents()
        {
            var appxClientCbsExist = packagesService.PackageExist("MicrosoftWindows.Client.CBS");
            var appxWindowsStoreExist = packagesService.PackageExist("Microsoft.WindowsStore");

            if (RunToDebug(nameof(GetUWPComponents)))
            {
                dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), "MicrosoftWindows.Client.CBS");
                return RequirementsResult.UWPComponentsMissing;
            }

            if (dataService.OsProperties.IsLTSC)
            {
                if (appxClientCbsExist)
                {
                    return RequirementsResult.AllCorrect;
                }

                dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), "MicrosoftWindows.Client.CBS");
                return RequirementsResult.UWPComponentsMissing;
            }

            if (appxWindowsStoreExist && appxClientCbsExist)
            {
                return RequirementsResult.AllCorrect;
            }

            dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), appxWindowsStoreExist ? "MicrosoftWindows.Client.CBS" : "Microsoft.WindowsStore");
            return RequirementsResult.UWPComponentsMissing;
        }

        private RequirementsResult GetDefenderComponents()
        {
            if (RunToDebug(nameof(GetDefenderComponents)))
            {
                dataService.RequirementsResult_1 = string.Format("OsRequirements_DefenderComponentsMissing".GetLocalized(), "SecurityHealthSystray.exe");
                return RequirementsResult.DefenderComponentsMissing;
            }

            var systemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var files = new List<string>()
            {
                Path.Combine(systemFolder, "smartscreen.exe"),
                Path.Combine(systemFolder, "SecurityHealthSystray.exe"),
                Path.Combine(systemFolder, "CompatTelRunner.exe"),
            };
            return files.TrueForAll(f =>
            {
                if (File.Exists(f))
                {
                    return true;
                }

                App.Logger.LogDefenderFileMissing(f);
                dataService.RequirementsResult_1 = string.Format("OsRequirements_DefenderComponentsMissing".GetLocalized(), f);
                return false;
            }) ? RequirementsResult.AllCorrect : RequirementsResult.DefenderComponentsMissing;
        }

        private RequirementsResult GetDefenderProperties()
        {
            if (RunToDebug(nameof(GetDefenderProperties)))
            {
                return RequirementsResult.WindowsComponentStabilityDisrupted;
            }

            var properties = new List<Func<bool>>
            {
                GetDefenderServices, TryStartSecurityHealthService, GetMSFTMpComputerStatus, GetAntiVirusProduct, GetMpPreference,
            };

            return properties.TrueForAll(p => p()) ? RequirementsResult.AllCorrect : RequirementsResult.WindowsComponentStabilityDisrupted;
        }

        private bool GetDefenderServices()
        {
            var services = new List<string>() { "Windefend", "SecurityHealthService", "wscsvc", "wdFilter" };
            return services.TrueForAll(s =>
            {
                var isExist = osService.Exist(s);
                App.Logger.LogDefenderServiceExist(s, isExist);
                return isExist;
            });
        }

        private bool TryStartSecurityHealthService() => osService.TryStart("SecurityHealthService");

        private bool GetMSFTMpComputerStatus()
        {
            try
            {
                return instrumentationService.GetAntiSpywareEnabled();
            }
            catch
            {
                return false;
            }
        }

        private bool GetAntiVirusProduct()
        {
            var properties = instrumentationService.GetAntivirusProducts();
            return properties.Count > 0;
        }

        private bool GetMpPreference()
        {
            var preference = powerShellService.Invoke("Get-MpPreference");
            return preference.Count > 0;
        }

        private RequirementsResult GetControlledFolderAccess()
        {
            if (RunToDebug(nameof(GetControlledFolderAccess)))
            {
                return RequirementsResult.DisableControlledFolderAccess;
            }

            try
            {
                var folderState = powerShellService.Invoke<byte>("(Get-MpPreference).EnableControlledFolderAccess");
                App.Logger.LogDefenderControlledFolderState(folderState);
                return folderState.Equals(1) ? RequirementsResult.DisableControlledFolderAccess : RequirementsResult.AllCorrect;
            }
            catch (Exception e)
            {
                App.Logger.LogDefenderControlledFolderException(e);
                return RequirementsResult.DisableControlledFolderAccess;
            }
        }

        private RequirementsResult GetRebootPending()
        {
            if (RunToDebug(nameof(GetRebootPending)))
            {
                return RequirementsResult.RebootPending;
            }

            var parameters = new List<RegistryKey?>()
            {
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootPending"),
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootInProgress"),
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\PackagesPending"),
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\PostRebootReporting"),
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\RebootRequired"),
            };

            return parameters.Exists(k => k is not null) ? RequirementsResult.RebootPending : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetSystemDriveEncryptedBitLocker()
        {
            if (RunToDebug(nameof(GetSystemDriveEncryptedBitLocker)))
            {
                return RequirementsResult.SystemDriveEncryptedBitLockerDisabled;
            }

            var protectionState = powerShellService.Invoke("Get-BitLockerVolume -MountPoint $env:SystemDrive | Where-Object -FilterScript {($_.ProtectionStatus -eq \"Off\") -and ($_.VolumeStatus -eq \"FullyEncrypted\")}");
            return protectionState.Count == 0 ? RequirementsResult.AllCorrect : RequirementsResult.SystemDriveEncryptedBitLockerDisabled;
        }

        private RequirementsResult GetUEFICertificates()
        {
            if (RunToDebug(nameof(GetUEFICertificates)))
            {
                return RequirementsResult.UpdateUEFICertificates;
            }

            var secureBootSupported = powerShellService.Invoke<bool>("try{Confirm-SecureBootUEFI -ErrorAction Stop}catch{$false}");
            var certificatesExpired = powerShellService.Invoke<bool>("[System.Text.Encoding]::ASCII.GetString((Get-SecureBootUEFI -Name db).Bytes) -notmatch \"Windows UEFI CA 2023\"");
            return secureBootSupported && certificatesExpired ? RequirementsResult.UpdateUEFICertificates : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetWindowsVersion()
        {
            dataService.RequirementsResult_1 = dataService.OsProperties.IsLTSC ? "OsRequirements_WrongWindowsVersion_LTSC".GetLocalized() : "OsRequirements_WrongWindowsVersion_1".GetLocalized();
            dataService.RequirementsResult_2 = $"{dataService.OsProperties.Caption} {dataService.OsProperties.DisplayVersion}";

            if (RunToDebug(nameof(GetWindowsVersion)))
            {
                return RequirementsResult.WrongWindowsVersion;
            }

            if (dataService.OsProperties.Caption.Contains("Windows 11"))
            {
                return RequirementsResult.AllCorrect;
            }

            return RequirementsResult.WrongWindowsVersion;
        }

        private RequirementsResult GetWindowsBuild()
        {
            dataService.RequirementsResult_1 = string.Format("OsRequirements_UpdateWindowsBuild_1".GetLocalized(), dataService.OsProperties.Build, dataService.SupportedUBR.Win11);
            dataService.RequirementsResult_2 = string.Format("OsRequirements_UpdateWindowsBuild_2".GetLocalized(), dataService.OsProperties.DisplayVersion, dataService.OsProperties.Build, dataService.OsProperties.UBR);

            if (RunToDebug(nameof(GetWindowsBuild)))
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.IsLTSC && dataService.OsProperties.Build != 26100)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.Build < 26200)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.UBR < (dataService.OsProperties.IsLTSC ? dataService.SupportedUBR.Win11LTSC : dataService.SupportedUBR.Win11))
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            return RequirementsResult.AllCorrect;
        }

        private bool RunToDebug(string name)
        {
            if (actionToDebug == name)
            {
                settingsService.DeleteDebugRequirementAction();
                return true;
            }

            return false;
        }
    }
}
