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
        private System.Management.ManagementObject? defenderPreference;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsService"/> class.
        /// </summary>
        /// <param name="dataService">A service for transferring app data between DI layers.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="osService">A service for working with Windows services API.</param>
        /// <param name="packagesService">A service for working with appx packages API.</param>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        public RequirementsService(
            ICommonDataService dataService,
            IInstrumentationService instrumentationService,
            IOsService osService,
            IAppxPackagesService packagesService,
            IPowerShellService powerShellService)
        {
            this.dataService = dataService;
            this.instrumentationService = instrumentationService;
            this.osService = osService;
            this.packagesService = packagesService;
            this.powerShellService = powerShellService;
        }

        /// <inheritdoc/>
        public List<RequirementAction> GetActions() =>
        [
            new(action: GetExternalServicesData, displayText: "OsRequirements_GetExternalServicesData".GetLocalized()),
            new(action: GetWindowsComponentStability, displayText: "OsRequirements_GetWindowsComponentStability".GetLocalized()),
            new(action: GetCpuArchitecture, displayText: "OsRequirements_GetCpuArchitecture".GetLocalized()),
            new(action: GetAppRelease, displayText: "OsRequirements_GetAppNewVersion".GetLocalized()),
            new(action: RunFromLoggedUser, displayText: "OsRequirements_RunFromLoggedUser".GetLocalized()),
            new(action: GetHarmfulTweakers, displayText: "OsRequirements_GetHarmfulTweaker".GetLocalized()),
            new(action: GetUWPComponents, displayText: "OsRequirements_GetUWPComponents".GetLocalized()),
            new(action: GetDefenderComponents, displayText: "OsRequirements_GetDefenderComponents".GetLocalized()),
            new(action: GetControlledFolderAccess, displayText: "OsRequirements_GetControlledFolderAccess".GetLocalized()),
            new(action: GetRebootPending, displayText: "OsRequirements_GetRebootPending".GetLocalized()),
            new(action: GetWindowsVersion, displayText: "OsRequirements_GetWindowsVersion".GetLocalized()),
            new(action: GetWindowsBuild, displayText: "OsRequirements_GetWindowsBuild".GetLocalized()),

            // The checks that ask the user whether to continue go last: by that moment everything else has already passed
            new(action: GetHostFileEntries, displayText: "OsRequirements_GetHostFileEntries".GetLocalized()),
            new(action: GetSystemDriveEncryptedBitLocker, displayText: "OsRequirements_GetSystemDriveEncryptedBitLocker".GetLocalized()),
            new(action: GetUEFICertificates, displayText: "OsRequirements_GetUEFICertificates".GetLocalized()),
        ];

        private RequirementsResult GetExternalServicesData()
        {
            // Start the shared PowerShell session and the downloads in the background.
            // The checks wait for the data only where they need it
            _ = Task.Run(() => powerShellService.Invoke("$null"));
            dataService.BeginGetExternalServicesData();
            return RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetWindowsComponentStability()
        {
            var wmiClasses = new List<(string NameSpace, string ClassName)>
            {
                ("root/CIMV2", "CIM_OperatingSystem"),
                ("root/CIMV2", "Win32_Volume"),
                ("root/CIMV2", "Win32_ShadowStorage"),
                ("root/StandardCimv2", "MSFT_NetAdapter"),
                ("root/CIMV2/Security/MicrosoftVolumeEncryption", "Win32_EncryptableVolume"),
                ("root/Microsoft/Windows/Defender", "MSFT_MpComputerStatus"),
                ("root/SecurityCenter2", "AntiVirusProduct"),
            };

            // This class is not available on Windows 11 LTSC 2024
            if (!dataService.OsProperties.IsLTSC)
            {
                wmiClasses.Add(("root/CIMV2/mdm/dmmap", "MDM_EnterpriseModernAppManagement_AppManagement01"));
            }

            var checks = new List<(string Component, Action Check)>();

            // Checking whether necessary services and drivers exist
            foreach (var service in new[] { "Windefend", "SecurityHealthService", "wscsvc", "wdFilter", "SysMain" })
            {
                checks.Add((service, () =>
                {
                    using var controller = new System.ServiceProcess.ServiceController(service);
                    _ = controller.Status;
                }));
            }

            // These two services must be able to run. On a Windows with Microsoft Defender cut out the first one cannot be started
            checks.Add(("SecurityHealthService", () => osService.Start("SecurityHealthService")));
            checks.Add(("SysMain", () => osService.Start("SysMain", System.ServiceProcess.ServiceStartMode.Automatic)));

            // Windows manages the size of the page file itself
            checks.Add(("Get-CimInstance -Namespace root/CIMV2 -ClassName CIM_ComputerSystem", instrumentationService.SetPageFileAutoSize));

            foreach (var (nameSpace, className) in wmiClasses)
            {
                // A WMI query is executed only when its result is enumerated, so Count() is required to detect a missing class
                checks.Add(($"Get-CimInstance -Namespace {nameSpace} -ClassName {className}", () => _ = instrumentationService.GetCimInstance(nameSpace: nameSpace, className: className).Count()));
            }

            checks.Add(("Get-MpPreference", () =>
            {
                // The result is kept for the Controlled folder access check
                defenderPreference = GetDefenderPreference() ?? throw new InvalidOperationException("Microsoft Defender settings are not available");
            }));

            checks.Add(("Windows PowerShell", () =>
            {
                // Some settings are applied with cmdlets: Windows PowerShell must start and find them
                if (!powerShellService.TryInvoke("Get-Command -Name Get-AppxPackage, Get-NetAdapter, Set-DnsClientServerAddress, Get-WinLanguageBarOption, Enable-WindowsOptionalFeature -ErrorAction Stop", out _))
                {
                    throw new InvalidOperationException("Windows PowerShell cannot run the cmdlets");
                }
            }));

            // The checks do not depend on each other, so they run at the same time
            var errors = checks.Select(check => Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        check.Check();
                        return null;
                    }
                    catch (Exception ex)
                    {
                        return ex;
                    }
                },
                TaskCreationOptions.LongRunning)).ToArray();

            Task.WaitAll(errors);

            // The first failed component in the list is shown to the user
            for (var i = 0; i < checks.Count; i++)
            {
                if (errors[i].Result is { } error)
                {
                    App.Logger.LogComponentStabilityException(checks[i].Component, error);
                    SetWindowsComponentStabilityMessage(checks[i].Component);
                    return RequirementsResult.WindowsComponentStabilityDisrupted;
                }
            }

            return RequirementsResult.AllCorrect;
        }

        private void SetWindowsComponentStabilityMessage(string component)
            => dataService.RequirementsResult_1 = string.Format("OsRequirements_WindowsComponentStabilityDisrupted".GetLocalized(), component);

        private RequirementsResult GetCpuArchitecture()
        {
            // The WMI query of the processor takes about a second, so the registry is used
            var architecture = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
            var processorName = Registry.LocalMachine.GetSubKeyValue("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", "ProcessorNameString") as string ?? architecture.ToString();
            dataService.RequirementsResult_1 = string.Format("OsRequirements_UnsupportedArchitecture_1".GetLocalized(), processorName.Trim());

            return architecture == System.Runtime.InteropServices.Architecture.X64 ? RequirementsResult.AllCorrect : RequirementsResult.UnsupportedArchitecture;
        }

        private RequirementsResult GetAppRelease()
        {
            dataService.WaitForAppRelease();
            var actualVersion = dataService.LatestAppRelease?.SophiApp_release ?? new Version(0, 0, 0);
            dataService.RequirementsResult_1 = string.Format("OsRequirements_NewAppVersionFound_1".GetLocalized(), actualVersion);

            if (actualVersion > dataService.AppVersion)
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

            // Skip the check if explorer.exe is not running or its owner cannot be determined
            return string.IsNullOrEmpty(processOwner) || processOwner.Equals(appUser, StringComparison.OrdinalIgnoreCase) ? RequirementsResult.AllCorrect : RequirementsResult.LoggedUserNotAdmin;
        }

        private RequirementsResult GetHarmfulTweakers()
        {
            var malwares = new Dictionary<string, Func<bool>>()
            {
                // https://www.youtube.com/GHOSTSPECTRE
                { "OsRequirements_Malware_GhostToolbox", () => File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\migwiz\\dlmanifests\\run.ghost.cmd") },
                // https://win10tweaker.ru
                { "OsRequirements_Malware_Win10Tweaker", () => Registry.CurrentUser.SubKeyExists("Software\\Win 10 Tweaker") },
                // https://revi.cc
                { "OsRequirements_Malware_RevisionTool", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Revision Tool")) },
                // https://github.com/Atlas-OS/Atlas
                { "OsRequirements_Malware_AtlasOS", () => Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "AtlasModules")) },
                // https://boosterx.ru
                { "OsRequirements_Malware_BoosterX", () => File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\GameModeX\\GameModeX.exe") },
                // https://www.youtube.com/watch?v=5NBqbUUB1Pk
                { "OsRequirements_Malware_WinClean", () => Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\WinClean Plus Apps") },
                // https://pc-np.com
                { "OsRequirements_Malware_PCNP", () => Registry.CurrentUser.SubKeyExists("Software\\PCNP") },
                // https://www.reddit.com/r/TronScript
                { "OsRequirements_Malware_Tron", () => Directory.Exists($"{Environment.ExpandEnvironmentVariables("%SystemDrive%")}\\logs\\tron") },
                // https://crystalcry.ru
                { "OsRequirements_Malware_CrystalCry", () => Registry.LocalMachine.SubKeyExists("SOFTWARE\\CrystalCry") },
                // https://github.com/es3n1n/defendnot
                { "OsRequirements_Malware_Defendnot", () => Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\\Tasks\\defendnot") },
                // https://github.com/zoicware/RemoveWindowsAI
                { "OsRequirements_Malware_RemoveWindowsAI", () => Registry.LocalMachine.SubKeyExists("SOFTWARE\\RemoveWindowsAI") },
                // https://forum.ru-board.com/topic.cgi?forum=62&topic=30617&start=1600#14
                {
                    "OsRequirements_Malware_AutoSettingsPS", () =>
                    {
                        var exclusions = Registry.LocalMachine.GetSubKeyValueNames("SOFTWARE\\Microsoft\\Windows Defender\\Exclusions\\Paths");
                        return Array.Exists(exclusions, key => key.Contains("AutoSettingsPS"));
                    }
                },
                // https://forum.ru-board.com/topic.cgi?forum=5&topic=50519
                {
                    "OsRequirements_Malware_ModernTweaker", () =>
                    {
                        var shellCache = Registry.CurrentUser.GetSubKeyValueNames("Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\Shell\\MuiCache");
                        return Array.Exists(shellCache, key => key.Contains("ModernTweaker"));
                    }
                },
            };
            var malwareFound = malwares.Any(m =>
            {
                if (m.Value())
                {
                    var detectedMalware = m.Key.GetLocalized();
                    dataService.RequirementsResult_1 = string.Format("OsRequirements_HarmfulTweakerFound".GetLocalized(), detectedMalware);
                    App.Logger.LogMalwareDetected(detectedMalware);
                    return true;
                }

                return false;
            });

            return malwareFound ? RequirementsResult.HarmfulTweakerFound : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetHostFileEntries()
        {
            var hostFileEntries = File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts"));
            return hostFileEntries.Any(e => !string.IsNullOrWhiteSpace(e) && !e.TrimStart().StartsWith('#')) ? RequirementsResult.HostsEntriesFound : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetUWPComponents()
        {
            var clientCBSExist = packagesService.PackageExist("MicrosoftWindows.Client.CBS");
            var windowsStoreExist = packagesService.PackageExist("Microsoft.WindowsStore");

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
            var components = new List<string>()
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "smartscreen.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "SecurityHealthSystray.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "CompatTelRunner.exe"),
            };
            var componentsExist = components.TrueForAll(component =>
            {
                if (File.Exists(component))
                {
                    return true;
                }

                App.Logger.LogDefenderFileMissing(component);
                dataService.RequirementsResult_1 = string.Format("OsRequirements_DefenderComponentsMissing".GetLocalized(), component);
                return false;
            });

            return componentsExist ? RequirementsResult.AllCorrect : RequirementsResult.DefenderComponentsMissing;
        }

        // The settings that Get-MpPreference shows, read from the same WMI class without starting PowerShell
        private System.Management.ManagementObject? GetDefenderPreference()
        {
            try
            {
                return instrumentationService.GetCimInstance(nameSpace: "root/Microsoft/Windows/Defender", className: "MSFT_MpPreference").FirstOrDefault();
            }
            catch (Exception ex) when (ex is System.Management.ManagementException or System.Runtime.InteropServices.COMException or UnauthorizedAccessException)
            {
                // A missing or broken WMI class means that Microsoft Defender is broken
                return null;
            }
        }

        private RequirementsResult GetControlledFolderAccess()
        {
            // Microsoft Defender settings have usually been read by the Windows components check
            var preference = defenderPreference ?? GetDefenderPreference();

            if (preference is not null)
            {
                var folderState = Convert.ToByte(preference.GetPropertyValue("EnableControlledFolderAccess"));
                App.Logger.LogDefenderControlledFolderState(folderState);
                return folderState == 1 ? RequirementsResult.DisableControlledFolderAccess : RequirementsResult.AllCorrect;
            }

            App.Logger.LogControlledFolderException(new InvalidOperationException("Microsoft Defender settings are not available"));

            // Get-MpPreference failure means that Microsoft Defender is broken
            SetWindowsComponentStabilityMessage("Get-MpPreference");
            return RequirementsResult.WindowsComponentStabilityDisrupted;
        }

        private RequirementsResult GetRebootPending()
        {
            var parameters = new List<bool>()
            {
                Registry.LocalMachine.SubKeyExists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootPending"),
                Registry.LocalMachine.SubKeyExists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\RebootInProgress"),
                Registry.LocalMachine.SubKeyExists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\PackagesPending"),
                Registry.LocalMachine.SubKeyExists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\PostRebootReporting"),
                Registry.LocalMachine.SubKeyExists("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\WindowsUpdate\\Auto Update\\RebootRequired"),
            };

            return parameters.Exists(exists => exists) ? RequirementsResult.RebootPending : RequirementsResult.AllCorrect;
        }

        private RequirementsResult GetSystemDriveEncryptedBitLocker()
        {
            // The drive is encrypted, but BitLocker protection is turned off
            return SystemDriveIsEncryptedWithoutProtection() ? RequirementsResult.SystemDriveEncryptedBitLockerDisabled : RequirementsResult.AllCorrect;
        }

        private bool SystemDriveIsEncryptedWithoutProtection()
        {
            try
            {
                // The same WMI class that the cmdlet Get-BitLockerVolume reads
                var systemDrive = Path.GetPathRoot(Environment.SystemDirectory)!.TrimEnd('\\');
                using var volume = instrumentationService.GetCimInstance(nameSpace: "root/CIMV2/Security/MicrosoftVolumeEncryption", className: "Win32_EncryptableVolume")
                    .FirstOrDefault(encryptableVolume => systemDrive.Equals(encryptableVolume.GetPropertyValue("DriveLetter") as string, StringComparison.OrdinalIgnoreCase));

                if (volume?.GetPropertyValue("ProtectionStatus") is { } protectionStatus && volume.GetPropertyValue("ConversionStatus") is { } conversionStatus)
                {
                    // ProtectionStatus: 0 is off. ConversionStatus: 1 is fully encrypted
                    return Convert.ToUInt32(protectionStatus) == 0 && Convert.ToUInt32(conversionStatus) == 1;
                }
            }
            catch (Exception ex)
            {
                // The WMI class is not available: there is nothing to check
                App.Logger.LogSkippedException("Reading the BitLocker state of the system drive from WMI", ex);
            }

            return false;
        }

        private RequirementsResult GetUEFICertificates()
        {
            // The firmware variable that the cmdlet Confirm-SecureBootUEFI reads. It is missing if the computer does not support Secure Boot
            var secureBootEnabled = FirmwareVariable.Read("SecureBoot", FirmwareVariable.GlobalNamespace) is { Length: > 0 } secureBoot && secureBoot[0] == 1;

            // The certificates can be checked only if Secure Boot is enabled
            if (!secureBootEnabled)
            {
                return RequirementsResult.EnableSecureBoot;
            }

            // Look for the certificate name in the Secure Boot signature database
            var ca2023Exist = FirmwareVariable.Read("db", FirmwareVariable.ImageSecurityNamespace) is { } bytes
                && System.Text.Encoding.ASCII.GetString(bytes).Contains("Windows UEFI CA 2023", StringComparison.OrdinalIgnoreCase);

            return ca2023Exist ? RequirementsResult.AllCorrect : RequirementsResult.UpdateUEFICertificates;
        }

        private RequirementsResult GetWindowsVersion()
        {
            dataService.RequirementsResult_1 = dataService.OsProperties.IsLTSC ? "OsRequirements_WrongWindowsVersion_LTSC".GetLocalized() : "OsRequirements_WrongWindowsVersion_1".GetLocalized();
            dataService.RequirementsResult_2 = string.Format("OsRequirements_WrongWindowsVersion_2".GetLocalized(), dataService.OsProperties.Caption, dataService.OsProperties.DisplayVersion);

            var caption = dataService.OsProperties.Caption;
            var isWindows11 = caption.Contains("Windows 11", StringComparison.OrdinalIgnoreCase);
            var isLTSC = caption.Contains("LTSC", StringComparison.OrdinalIgnoreCase);

            // An LTSC edition must be Windows 11 LTSC, any other edition must be Windows 11 that is not LTSC
            var isSupported = dataService.OsProperties.IsLTSC
                ? isWindows11 && isLTSC
                : isWindows11 && !isLTSC;

            return isSupported ? RequirementsResult.AllCorrect : RequirementsResult.WrongWindowsVersion;
        }

        private RequirementsResult GetWindowsBuild()
        {
            // The supported builds are downloaded. All other downloads are finished here too, before the functions are read
            dataService.WaitForExternalServicesData();
            dataService.RequirementsResult_1 = string.Format(
                (dataService.OsProperties.IsLTSC ? "OsRequirements_UpdateWindowsBuild_LTSC" : "OsRequirements_UpdateWindowsBuild_1").GetLocalized(),
                dataService.OsProperties.IsLTSC ? dataService.SupportedOs.LTSC_MajorBuild : dataService.SupportedOs.MajorBuild,
                dataService.OsProperties.IsLTSC ? dataService.SupportedOs.LTSC_MinorBuild : dataService.SupportedOs.MinorBuild);
            dataService.RequirementsResult_2 = string.Format("OsRequirements_UpdateWindowsBuild_2".GetLocalized(), dataService.OsProperties.DisplayVersion, dataService.OsProperties.MajorBuild, dataService.OsProperties.MinorBuild);

            // Windows 11 LTSC 2024 has the only supported major build
            if (dataService.OsProperties.IsLTSC && (dataService.OsProperties.MajorBuild != dataService.SupportedOs.LTSC_MajorBuild || dataService.OsProperties.MinorBuild < dataService.SupportedOs.LTSC_MinorBuild))
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            if (!dataService.OsProperties.IsLTSC && dataService.OsProperties.MajorBuild < dataService.SupportedOs.MajorBuild)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            // The minor build matters only if the major build is the minimal supported one
            if (!dataService.OsProperties.IsLTSC && dataService.OsProperties.MajorBuild == dataService.SupportedOs.MajorBuild && dataService.OsProperties.MinorBuild < dataService.SupportedOs.MinorBuild)
            {
                return RequirementsResult.UpdateWindowsBuild;
            }

            return RequirementsResult.AllCorrect;
        }
    }
}
