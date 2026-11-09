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
        private readonly IPowerShellService powerShellService;
        private readonly ISettingsService settingsService;
        private string actionToDebug = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsService"/> class.
        /// </summary>
        /// <param name="dataService">A service for transferring app data between DI layers.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="packagesService">A service for working with appx packages API.</param>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        /// <param name="settingsService">A service for working with app settings.</param>
        public RequirementsService(
            ICommonDataService dataService,
            IInstrumentationService instrumentationService,
            IAppxPackagesService packagesService,
            IPowerShellService powerShellService,
            ISettingsService settingsService)
        {
            this.dataService = dataService;
            this.instrumentationService = instrumentationService;
            this.packagesService = packagesService;
            this.powerShellService = powerShellService;
            this.settingsService = settingsService;
        }

        /// <inheritdoc/>
        public List<RequirementAction> GetActions() => [new (action: GetExternalServicesData, displayText: "OsRequirements_GetExternalServicesData".GetLocalized()),
            new (action: GetWindowsComponentStability, displayText: "OsRequirements_GetWindowsComponentStability".GetLocalized()),
            new (action: GetAppRelease, displayText: "OsRequirements_GetAppNewVersion".GetLocalized()),
            new (action: RunFromLoggedUser, displayText: "OsRequirements_RunFromLoggedUser".GetLocalized()),
            new (action: GetHarmfulTweakers, displayText: "OsRequirements_GetHarmfulTweaker".GetLocalized()),
            new (action: GetHostFileEntries, displayText: "OsRequirements_GetHostFileEntries".GetLocalized()),
            new (action: GetUWPComponents, displayText: "OsRequirements_GetUWPComponents".GetLocalized()),
            new (action: GetDefenderComponents, displayText: "OsRequirements_GetDefenderComponents".GetLocalized()),
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
            Task.Run(async () => await dataService.GetExternalServicesDataAsync())
                .Wait();
            return RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetWindowsComponentStability()
        {
            if (RunToDebug(nameof(GetWindowsComponentStability)))
            {
                return RequirementsResult.WindowsComponentStabilityDisrupted;
            }

            try
            {
                _ = instrumentationService.GetCimInstance(nameSpace: "root/CIMV2", className: "CIM_OperatingSystem");
                _ = instrumentationService.GetCimInstance(nameSpace: "root/CIMV2", className: "Win32_Volume");
                _ = instrumentationService.GetCimInstance(nameSpace: "root/CIMV2", className: "Win32_ShadowStorage");
                _ = instrumentationService.GetCimInstance(nameSpace: "root/StandardCimv2", className: "MSFT_NetAdapter");
                _ = instrumentationService.GetCimInstance(nameSpace: "root/CIMV2/mdm/dmmap", className: "MDM_EnterpriseModernAppManagement_AppManagement01");
                return RequirementsResult.AllCorrect;
            }
            catch
            {
                return RequirementsResult.WindowsComponentStabilityDisrupted;
            }
        }

        private RequirementsResult GetAppRelease()
        {
            var actualVersion = dataService.LatestAppRelease?.SophiApp_release ?? new Version(0, 0, 0);
            dataService.RequirementsResult_1 = string.Format("OsRequirements_NewAppVersionFound_1".GetLocalized(), actualVersion);

            if (RunToDebug(nameof(GetAppRelease)) || actualVersion > dataService.AppVersion)
            {
                App.Logger.LogAppNewVersionFound(actualVersion);
                return RequirementsResult.NewAppVersionFound;
            }

            return RequirementsResult.AllCorrect;
        }

        private RequirementsResult RunFromLoggedUser()
        {
            var appUser = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            var appUserSessionId = Process.GetCurrentProcess().SessionId;
            var explorerUser = Array.Find(array: Process.GetProcesses(), match: p => p.ProcessName.Equals("explorer") && p.SessionId.Equals(appUserSessionId));
            var processOwner = instrumentationService.GetProcessOwnerName(explorerUser);
            dataService.RequirementsResult_1 = string.Format("OsRequirements_LoggedUserNotAdmin_1".GetLocalized(), processOwner);
            dataService.RequirementsResult_2 = string.Format("OsRequirements_LoggedUserNotAdmin_2".GetLocalized(), appUser);

            if (RunToDebug(nameof(RunFromLoggedUser)))
            {
                return RequirementsResult.LoggedUserNotAdmin;
            }

            return processOwner.Equals(appUser) ? RequirementsResult.AllCorrect : RequirementsResult.LoggedUserNotAdmin;
        }

        private RequirementsResult GetHarmfulTweakers()
        {
            if (RunToDebug(nameof(GetHarmfulTweakers)))
            {
                dataService.RequirementsResult_1 = string.Format("OsRequirements_HarmfulTweakerFound".GetLocalized(), "OsRequirements_Malware_Win10Tweaker".GetLocalized());
                return RequirementsResult.HarmfulTweakerFound;
            }

            var malwares = new Dictionary<string, Func<bool>>()
            {
                // https://www.youtube.com/GHOSTSPECTRE
                { "OsRequirements_Malware_GhostToolbox", () => File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\migwiz\\dlmanifests\\run.ghost.cmd") },
                // https://win10tweaker.ru
                { "OsRequirements_Malware_Win10Tweaker", () => Registry.CurrentUser.OpenSubKey("Software\\Win 10 Tweaker") is not null },
                // https://revi.cc
                { "OsRequirements_Malware_RevisionTool", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Revision Tool")) },
                // https://github.com/Atlas-OS/Atlas
                { "OsRequirements_Malware_AtlasOS", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "AtlasModules")) },
                // https://boosterx.ru
                { "OsRequirements_Malware_BoosterX", () => File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\GameModeX\\GameModeX.exe") },
                // https://www.youtube.com/watch?v=5NBqbUUB1Pk
                { "OsRequirements_Malware_WinClean", () => Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\WinClean Plus Apps") },
                // https://pc-np.com
                { "OsRequirements_Malware_PCNP", () => Registry.CurrentUser.OpenSubKey("Software\\PCNP") is not null },
                // https://www.reddit.com/r/TronScript
                { "OsRequirements_Malware_Tron", () => Directory.Exists(Path.Combine(Environment.ExpandEnvironmentVariables("%SystemDrive%"), "\\logs\\tron")) },
                // https://crystalcry.ru
                { "OsRequirements_Malware_CrystalCry", () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\CrystalCry") is not null },
                // https://github.com/es3n1n/defendnot
                { "OsRequirements_Malware_Defendnot", () => Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\Tasks\\defendnot") },
                // https://github.com/zoicware/RemoveWindowsAI
                { "OsRequirements_Malware_RemoveWindowsAI", () => Registry.LocalMachine.OpenSubKey("SOFTWARE\\RemoveWindowsAI") is not null },
                // https://forum.ru-board.com/topic.cgi?forum=62&topic=30617&start=1600#14
                {
                    "OsRequirements_Malware_AutoSettingsPS", () =>
                    {
                        var exclusions = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft Defender\\Exclusions\\Paths")?.GetValueNames() ?? [];
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
                    dataService.RequirementsResult_1 = string.Format("OsRequirements_HarmfulTweakerFound".GetLocalized(), detectedMalware);
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

            var hostFileEntries = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts"));
            return hostFileEntries.Any(e => !(string.IsNullOrEmpty(e) || e.StartsWith('#'))) ? RequirementsResult.HostsEntriesFound : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetUWPComponents()
        {
            var clientCBSExist = packagesService.PackageExist("MicrosoftWindows.Client.CBS");
            var windowsStoreExist = packagesService.PackageExist("Microsoft.WindowsStore");

            if (RunToDebug(nameof(GetUWPComponents)))
            {
                dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), "MicrosoftWindows.Client.CBS");
                return RequirementsResult.UWPComponentsMissing;
            }

            if (dataService.OsProperties.IsLTSC)
            {
                if (clientCBSExist)
                {
                    return RequirementsResult.AllCorrect;
                }

                dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), "MicrosoftWindows.Client.CBS");
                return RequirementsResult.UWPComponentsMissing;
            }

            if (windowsStoreExist && clientCBSExist)
            {
                return RequirementsResult.AllCorrect;
            }

            dataService.RequirementsResult_1 = string.Format("OsRequirements_UWPComponentsMissing".GetLocalized(), windowsStoreExist ? "MicrosoftWindows.Client.CBS" : "Microsoft.WindowsStore");
            return RequirementsResult.UWPComponentsMissing;
        }

        private RequirementsResult GetDefenderComponents()
        {
            if (RunToDebug(nameof(GetDefenderComponents)))
            {
                dataService.RequirementsResult_1 = string.Format("OsRequirements_DefenderComponentsMissing".GetLocalized(), "SecurityHealthSystray.exe");
                return RequirementsResult.DefenderComponentsMissing;
            }

            var components = new List<string>()
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "smartscreen.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "SecurityHealthSystray.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "CompatTelRunner.exe"),
            };
            return components.TrueForAll(component =>
            {
                if (File.Exists(component))
                {
                    return true;
                }

                App.Logger.LogDefenderFileMissing(component);
                dataService.RequirementsResult_1 = string.Format("OsRequirements_DefenderComponentsMissing".GetLocalized(), component);
                return false;
            }) ? RequirementsResult.AllCorrect : RequirementsResult.DefenderComponentsMissing;
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
                App.Logger.LogControlledFolderException(e);
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

            return parameters.Exists(key => key is not null) ? RequirementsResult.RebootPending : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetSystemDriveEncryptedBitLocker()
        {
            if (RunToDebug(nameof(GetSystemDriveEncryptedBitLocker)))
            {
                return RequirementsResult.SystemDriveEncryptedBitLockerDisabled;
            }

            var protectionStatus = powerShellService.Invoke("Get-BitLockerVolume -MountPoint $env:SystemDrive | Where-Object -FilterScript {($_.ProtectionStatus -eq \"Off\") -and ($_.VolumeStatus -eq \"FullyEncrypted\")}");
            return protectionStatus.Count == 0 ? RequirementsResult.AllCorrect : RequirementsResult.SystemDriveEncryptedBitLockerDisabled;
        }

        private RequirementsResult GetUEFICertificates()
        {
            if (RunToDebug(nameof(GetUEFICertificates)))
            {
                return RequirementsResult.UpdateUEFICertificates;
            }

            var secureBootEnabled = powerShellService.Invoke<bool>("try{Confirm-SecureBootUEFI -ErrorAction Stop}catch{$false}");
            var ca2023Exist = powerShellService.Invoke<bool>("try{[System.Text.Encoding]::ASCII.GetString((Get-SecureBootUEFI -Name db -ErrorAction Stop).Bytes) -match \"Windows UEFI CA 2023\"}catch{$false}");
            return secureBootEnabled && ca2023Exist ? RequirementsResult.AllCorrect : RequirementsResult.UpdateUEFICertificates;
        }

        private RequirementsResult GetWindowsVersion()
        {
            dataService.RequirementsResult_1 = dataService.OsProperties.IsLTSC ? "OsRequirements_WrongWindowsVersion_LTSC".GetLocalized() : "OsRequirements_WrongWindowsVersion_1".GetLocalized();
            dataService.RequirementsResult_2 = string.Format("OsRequirements_WrongWindowsVersion_2".GetLocalized(), dataService.OsProperties.Caption, dataService.OsProperties.DisplayVersion);

            if (RunToDebug(nameof(GetWindowsVersion)))
            {
                return RequirementsResult.WrongWindowsVersion;
            }

            return dataService.OsProperties.Caption.Contains("Windows 11") ? RequirementsResult.AllCorrect : RequirementsResult.WrongWindowsVersion;
        }

        private RequirementsResult GetWindowsBuild()
        {
            dataService.RequirementsResult_1 = string.Format("OsRequirements_UpdateWindowsBuild_1".GetLocalized(), dataService.SupportedOs.MajorBuild, dataService.SupportedOs.MinorBuild);
            dataService.RequirementsResult_2 = string.Format("OsRequirements_UpdateWindowsBuild_2".GetLocalized(), dataService.OsProperties.DisplayVersion, dataService.OsProperties.MajorBuild, dataService.OsProperties.MinorBuild);

            if (RunToDebug(nameof(GetWindowsBuild)))
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.IsLTSC && dataService.OsProperties.MajorBuild < dataService.SupportedOs.LTSC_MajorBuild)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.MajorBuild < dataService.SupportedOs.MajorBuild)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (dataService.OsProperties.MinorBuild < dataService.SupportedOs.MinorBuild)
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
