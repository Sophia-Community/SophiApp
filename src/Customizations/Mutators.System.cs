// <copyright file="Mutators.System.cs" company="Team Sophia">
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
    /// The settings of the "System" page.
    /// </content>
    public static partial class Mutators
    {
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
        /// Set OneDrive state.
        /// </summary>
        /// <param name="enable">True to install, false to uninstall.</param>
        public static void OneDrive(bool enable) => SetOneDrive(enable, allUsers: false);

        /// <summary>
        /// Set OneDrive state. OneDrive is installed for all users of the computer.
        /// </summary>
        /// <param name="enable">True to install, false to uninstall.</param>
        public static void OneDrive_AllUsers(bool enable) => SetOneDrive(enable, allUsers: true);

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
            GroupPolicyService.RemovePolicies(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
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
            GroupPolicyService.RemovePolicies(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU", "NoAutoRebootWithLoggedOnUsers", "AlwaysAutoRebootAtScheduledTime");
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.RemovePolicies(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "ActiveHoursStart", "ActiveHoursEnd", "SetActiveHours");
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
            GroupPolicyService.RemovePolicies(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Windows\\WindowsUpdate", "AllowOptionalContent", "SetAllowOptionalContent");
            Registry.LocalMachine.SetValueIfKeyExists("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsContinuousInnovationOptedIn", enable ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Set network adapters power state.
        /// </summary>
        /// <param name="adapters">The names of the adapters to change, with their new state.</param>
        public static void NetworkAdaptersSavePower(IReadOnlyDictionary<string, bool> adapters)
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

            NetworkAdapterService.SetTurnOffDevice(adapters);
        }

        /// <summary>
        /// Set the input method Windows starts with.
        /// </summary>
        /// <param name="inputTip">The id of an input method of the language list, or the id of the choice to start with the first one of the list.</param>
        public static void InputMethod(string inputTip)
        {
            if (inputTip == InputMethodHelper.LanguageListId)
            {
                Registry.CurrentUser.DeleteSubKeyValue("Control Panel\\International\\User Profile", "InputMethodOverride");
                return;
            }

            // Nothing but the id of an input method is passed to the cmdlet
            if (!InputMethodHelper.IsTip(inputTip))
            {
                throw new ArgumentException($"\"{inputTip}\" is not the id of an input method", nameof(inputTip));
            }

            _ = PowerShellService.Invoke($"Set-WinDefaultInputMethodOverride -InputTip \"{inputTip}\"");
        }

        /// <summary>
        /// Change the location of a user folder. The files are not moved to the new location.
        /// </summary>
        /// <param name="folder">A user folder.</param>
        /// <param name="path">The new location.</param>
        public static void UserShellFolderLocation(UserFolder folder, string path) => UserFolderHelper.SetPath(folder, path);

        /// <summary>
        /// Set reserved storage state.
        /// </summary>
        /// <param name="enable">Reserved storage state.</param>
        public static void ReservedStorage(bool enable)
        {
            var command = enable ? "Set-WindowsReservedStorageState -State Enabled" : "Set-WindowsReservedStorageState -State Disabled";

            // Reserved storage cannot be turned off while Windows is using it, for example when an update is pending
            if (!PowerShellService.TryInvoke(command, out _) && !enable)
            {
                throw new CustomizationException(exceptionMessage: "Reserved storage is in use", resourceKey: "UIModel_Exception_ReservedStorageInUse");
            }
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
        /// <param name="planId">The id of the power plan to make active.</param>
        public static void PowerPlan(string planId)
        {
            // The id comes from the list of the plans of this PC
            if (!Guid.TryParse(planId, out var plan))
            {
                throw new ArgumentException($"\"{planId}\" is not the id of a power plan", nameof(planId));
            }

            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "Software\\Policies\\Microsoft\\Power\\PowerSettings", "ActivePowerScheme");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "Software\\Policies\\Microsoft\\Power\\PowerSettings", "ActivePowerScheme");

            var arguments = $"/SETACTIVE {plan:D}";
            var powerConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "powercfg.exe");
            _ = ProcessService.WaitForExit(powerConfig, arguments);
        }

        /// <summary>
        /// Set desktop shortcut creation upon Microsoft Edge update.
        /// </summary>
        /// <param name="enable">True to prevent the shortcut creation for every installed channel of Microsoft Edge.</param>
        public static void PreventEdgeShortcutCreation(bool enable)
        {
            if (!InstalledPrograms.Exists(EdgeShortcutPolicy.StableProgram))
            {
                throw new CustomizationException(exceptionMessage: "Microsoft Edge is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled") { Arguments = [EdgeShortcutPolicy.StableProgram] };
            }

            // msedgeupdate.admx is not a default ADMX template
            var templateExists = EdgeShortcutPolicy.TemplateExists;

            foreach (var (program, valueName) in EdgeShortcutPolicy.Channels)
            {
                if (!enable)
                {
                    Registry.LocalMachine.DeleteSubKeyValue(EdgeShortcutPolicy.Key, valueName);

                    if (templateExists)
                    {
                        GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, EdgeShortcutPolicy.Key, valueName);
                    }

                    continue;
                }

                if (InstalledPrograms.Exists(program))
                {
                    Registry.LocalMachine.CreateKeyAndSetValue(EdgeShortcutPolicy.Key, valueName, 0, RegistryValueKind.DWord);

                    if (templateExists)
                    {
                        GroupPolicyService.ClearPolicyCache(scope: LGPOScope.Computer, path: EdgeShortcutPolicy.Key, name: valueName, type: "DWORD", value: "0");
                    }
                }
            }
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
        /// Download and install the latest .NET 10 desktop runtime from the Microsoft resources.
        /// </summary>
        /// <param name="enable">True to install or update.</param>
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
        /// Download and install the latest .NET 11 desktop runtime from the Microsoft resources.
        /// </summary>
        /// <param name="enable">True to install or update.</param>
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
        /// Download and install the latest Visual C++ x86 from the Microsoft resources.
        /// </summary>
        /// <param name="enable">True to install or update.</param>
        public static void InstallVisualC_x86(bool enable)
        {
            if (enable)
            {
                RedistributablePackageService.Install("https://aka.ms/vc14/vc_redist.x86.exe", "vc_redist.x86.exe", "dd_vcredist_*.log");
            }
        }

        /// <summary>
        /// Download and install the latest Visual C++ x64 from the Microsoft resources.
        /// </summary>
        /// <param name="enable">True to install or update.</param>
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
                App.Status.SetStep("Panel_Step_WindowsFeature");
                _ = PowerShellService.Invoke("Enable-WindowsOptionalFeature -Online -FeatureName Recall -All -NoRestart");

                // Open the Copilot page in Microsoft Store
                ExplorerHelper.Open("ms-windows-store://pdp/?ProductId=9NHT9RB2F4HD");
                return;
            }

            App.Status.SetStep("Panel_Step_WindowsFeature");
            _ = PowerShellService.Invoke("Disable-WindowsOptionalFeature -Online -FeatureName Recall -NoRestart");
            AppxPackagesService.RemovePackage("Microsoft.Copilot");
        }

        /// <summary>
        /// Install Windows Subsystem for Linux (WSL) and a Linux distribution.
        /// </summary>
        /// <param name="distribution">The name of the distribution. An empty name does nothing.</param>
        public static void InstallWSL(string distribution)
        {
            if (string.IsNullOrEmpty(distribution))
            {
                return;
            }

            WslService.Install(distribution);

            // Receive updates for other Microsoft products: the Linux kernel is updated this way
            UpdateService.RunMicrosoftProductsUpdate();

            // Check for updates
            _ = ProcessService.StartProcessByName(Path.Combine(Environment.SystemDirectory, "UsoClient.exe"), "StartInteractiveScan");
        }

        /// <summary>
        /// Scan the registry and show all policies, even set by hand, in the Local Group Policy Editor.
        /// </summary>
        /// <param name="enable">True to scan. It is an action, so false does nothing.</param>
        public static void ScanRegistryPolicies(bool enable)
        {
            if (enable)
            {
                GroupPolicyService.AddRegistryPolicies();
            }
        }

        /// <summary>
        /// Set Windows Sandbox state.
        /// </summary>
        /// <param name="enable">Windows Sandbox state.</param>
        public static void WindowsSandbox(bool enable)
        {
            App.Status.SetStep("Panel_Step_WindowsFeature");
            _ = PowerShellService.Invoke($"{(enable ? "Enable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -All -Online -NoRestart" : "Disable-WindowsOptionalFeature -FeatureName Containers-DisposableClientVM -Online -NoRestart")}");
        }

        private static void SetOneDrive(bool enable, bool allUsers)
        {
            GroupPolicyService.DeleteRegistryValue(Registry.LocalMachine, "SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive", "DisableFileSyncNGSC");
            // Make changes in"C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database
            GroupPolicyService.ClearPolicyCache(LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\OneDrive", "DisableFileSyncNGSC");

            if (enable)
            {
                OneDriveService.Install(allUsers);
                return;
            }

            OneDriveService.Uninstall();
        }
    }
}
