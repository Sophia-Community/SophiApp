// <copyright file="Accessors.System.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <content>
    /// The settings of the "System" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Get Print screen button state.
        /// </summary>
        public static bool PrtScnSnippingTool()
        {
            var snippingValue = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Keyboard", "PrintScreenKeyForSnippingEnabled") as int? ?? -1;
            return snippingValue.Equals(1);
        }

        /// <summary>
        /// Get input method for app window state.
        /// </summary>
        public static bool AppsLanguageSwitch()
            => PowerShellService.Invoke("Get-WinLanguageBarOption").FirstOrDefault()?.GetPropertyValue<bool>("IsLegacySwitchingMode") ?? false;

        /// <summary>
        /// Get One Drive state.
        /// </summary>
        public static InstallState OneDrive()
        {
            if (OneDriveService.IsInstalled())
            {
                if (OneDriveService.UserIsLogged())
                {
                    throw new CustomizationException(exceptionMessage: "Log out from OneDrive account before uninstalling the application", resourceKey: "UIModel_Exception_OneDrive_LogOut") { InstallState = InstallState.Installed };
                }

                return InstallState.Installed;
            }
            else
            {
                var setupFile = OneDriveService.GetSetupFileOrDefault();
                App.Logger.LogOneDriveSetupFile(setupFile);

                if (!string.IsNullOrEmpty(setupFile) || HttpService.UrlIsAvailable("https://g.live.com/1rewlive5skydrive/OneDriveProductionV2"))
                {
                    return InstallState.NotInstalled;
                }

                throw new CustomizationException(exceptionMessage: "OneDriveSetup.exe was not found and there is no Internet access to download OneDrive installer", resourceKey: "UIModel_Exception_InternetConnectionRequired") { InstallState = InstallState.NotInstalled, Arguments = ["https://g.live.com/1rewlive5skydrive/OneDriveProductionV2"] };
            }
        }

        /// <summary>
        /// Get storage sense state.
        /// </summary>
        public static bool StorageSense()
        {
            var storagePolicy01 = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy", "01") as int? ?? -1;

            // "01" is the Storage Sense main switch. Other values are its options that the user may change
            return storagePolicy01.Equals(1);
        }

        /// <summary>
        /// Get hibernation state.
        /// </summary>
        public static bool Hibernation()
        {
            // On a device without the S4 sleep state, such as a virtual machine, hibernation cannot be enabled
            if (!PowerCapabilities.HibernationIsSupported)
            {
                throw new CustomizationException(exceptionMessage: "The firmware does not support hibernation", resourceKey: "UIModel_Exception_HibernationNotSupported");
            }

            using var powerKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Power");

            // Windows uses HibernateEnabledDefault if HibernateEnabled is not set
            var isEnabled = powerKey?.GetValue("HibernateEnabled") as int? ?? powerKey?.GetValue("HibernateEnabledDefault") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get long path limit state.
        /// </summary>
        public static bool Win32LongPathsSupport()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get BSOD error state.
        /// </summary>
        public static bool BSoDStopError()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\CrashControl", "DisplayParameters") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get delivery optimization state.
        /// </summary>
        public static bool DeliveryOptimization()
        {
            var isEnabled = Registry.Users.GetSubKeyValue("S-1-5-20\\Software\\Microsoft\\Windows\\CurrentVersion\\DeliveryOptimization\\Settings", "DownloadMode") as int? ?? -1;
            return !isEnabled.Equals(0);
        }

        /// <summary>
        /// Get Windows manage default printer state.
        /// </summary>
        public static bool WindowsManageDefaultPrinter()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows", "LegacyDefaultPrinterMode") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get update Microsoft products state.
        /// </summary>
        public static bool UpdateMicrosoftProducts() => UpdateService.HasMicrosoftProductsUpdate();

        /// <summary>
        /// Get restart notification state.
        /// </summary>
        public static bool RestartNotification()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "RestartNotificationsAllowed2") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get device restart after update state.
        /// </summary>
        public static bool RestartDeviceAfterUpdate()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsExpedited") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get active hours restart state.
        /// </summary>
        public static int ActiveHours()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "SmartActiveHoursState") as int? ?? -1;
            return isEnabled.Equals(1) ? 1 : 2;
        }

        /// <summary>
        /// Get Windows latest update state.
        /// </summary>
        public static bool WindowsLatestUpdate()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\WindowsUpdate\\UX\\Settings", "IsContinuousInnovationOptedIn") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get the connected network adapters that can be turned off to save power, with their state.
        /// </summary>
        public static IReadOnlyList<SettingCheckItem> NetworkAdaptersSavePower()
        {
            var adapters = NetworkAdapterService.GetTurnOffDevice();

            return adapters.Count == 0
                ? throw new CustomizationException(exceptionMessage: "There is no network adapter which has an AllowComputerToTurnOffDevice property", resourceKey: "UIModel_Exception_NetworkAdaptersSavePower")
                : adapters.Select(adapter => new SettingCheckItem(
                    adapter.Name,
                    adapter.Title,
                    adapter.IsConnected ? adapter.Description : string.Format("UIModel_NetworkAdaptersSavePower_NotConnected".GetLocalized(), adapter.Description),
                    adapter.IsEnabled)).ToList();
        }

        /// <summary>
        /// Get the input methods of the language list. The one Windows starts with is marked.
        /// </summary>
        public static IReadOnlyList<SettingChoice> InputMethod()
        {
            var startTip = Registry.CurrentUser.GetSubKeyValue("Control Panel\\International\\User Profile", "InputMethodOverride") as string ?? string.Empty;
            var methods = new List<SettingChoice>
            {
                new(InputMethodHelper.LanguageListId, "UIModel_InputMethod_LanguageList".GetLocalized(), string.Empty, true) { IsCurrent = startTip.Length == 0 },
            };

            methods.AddRange(InputMethodHelper.GetInstalled().Select(method => new SettingChoice(method.Tip, method.Language, method.Layout, true)
            {
                IsCurrent = method.Tip.Equals(startTip, StringComparison.OrdinalIgnoreCase),
            }));

            // An input method that was removed from the language list can still be the one Windows starts with
            if (!methods.Exists(method => method.IsCurrent))
            {
                methods.Add(new(startTip, startTip, string.Empty, true) { IsCurrent = true });
            }

            return methods;
        }

        /// <summary>
        /// Get the location of a user folder.
        /// </summary>
        /// <param name="folder">A user folder.</param>
        public static string UserShellFolderLocation(UserFolder folder)
        {
            // OneDrive keeps the screenshots folder for itself while the user is signed in
            if (folder == UserFolder.Screenshots && OneDriveService.UserIsLogged())
            {
                throw new CustomizationException(exceptionMessage: "Log out from OneDrive account", resourceKey: "UIModel_UserShellFolderLocation_ScreenshotsOneDrive");
            }

            return UserFolderHelper.GetPath(folder);
        }

        /// <summary>
        /// Get reserved storage state.
        /// </summary>
        public static bool ReservedStorage()
        {
            using var reserveKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ReserveManager");
            var miscPolicy = reserveKey?.GetValue("MiscPolicyInfo") as int? ?? -1;
            var passedPolicy = reserveKey?.GetValue("PassedPolicy") as int? ?? -1;
            var shippedReserves = reserveKey?.GetValue("ShippedWithReserves") as int? ?? -1;
            return !(miscPolicy.Equals(2) && passedPolicy.Equals(0) && shippedReserves.Equals(1));
        }

        /// <summary>
        /// Get help page state.
        /// </summary>
        public static bool F1HelpPage()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Classes\\Typelib\\{8cec5860-07a1-11d9-b15e-000d56bfe6ee}\\1.0\\0\\win64", string.Empty) as string;
            return isEnabled is null;
        }

        /// <summary>
        /// Get Num Lock state.
        /// </summary>
        public static bool NumLock()
        {
            var isEnabled = Registry.Users.GetSubKeyValue(".DEFAULT\\Control Panel\\Keyboard", "InitialKeyboardIndicators") as string ?? string.Empty;

            // Num Lock is on when the second bit is set: "2" means the same as "2147483650"
            return uint.TryParse(isEnabled, out var indicators) && (indicators & 2) != 0;
        }

        /// <summary>
        /// Get Caps Lock state.
        /// </summary>
        public static bool CapsLock()
        {
            var scancodeMap = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 58, 0, 0, 0, 0, 0 };
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout", "Scancode Map") as byte[] ?? [];
            return !isEnabled.SequenceEqual(scancodeMap);
        }

        /// <summary>
        /// Get sticky shift state.
        /// </summary>
        public static bool StickyShift()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Control Panel\\Accessibility\\StickyKeys", "Flags") as string ?? string.Empty;
            return !isEnabled.Equals("506");
        }

        /// <summary>
        /// Get mouse acceleration ("Enhance pointer precision") state.
        /// </summary>
        public static bool MouseAcceleration() => MouseHelper.GetAcceleration();

        /// <summary>
        /// Get autoplay state.
        /// </summary>
        public static bool Autoplay()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\AutoplayHandlers", "DisableAutoplay") as int? ?? -1;
            return !isEnabled.Equals(1);
        }

        /// <summary>
        /// Get thumbnail cache state.
        /// </summary>
        public static bool ThumbnailCacheRemoval()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun") as int? ?? -1;
            var isEnabledWow = Registry.LocalMachine.GetSubKeyValue("Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Explorer\\VolumeCaches\\Thumbnail Cache", "Autorun") as int? ?? -1;
            return !(isEnabled.Equals(0) && isEnabledWow.Equals(0));
        }

        /// <summary>
        /// Get save restartable apps state.
        /// </summary>
        public static bool SaveRestartableApps()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", "RestartApps") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get the power plans of this PC. The active one is marked.
        /// </summary>
        public static IReadOnlyList<SettingChoice> PowerPlan()
        {
            return InstrumentationService.GetPowerPlans()
                .Select(plan => new SettingChoice(GetPowerPlanId((string)plan.GetPropertyValue("InstanceID")), (string)plan.GetPropertyValue("ElementName"), string.Empty, true)
                {
                    IsCurrent = (bool)plan.GetPropertyValue("IsActive"),
                })
                .Where(plan => plan.Id.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Get whether desktop shortcut creation upon Microsoft Edge update is prevented.
        /// </summary>
        public static bool PreventEdgeShortcutCreation()
        {
            if (!InstalledPrograms.Exists(EdgeShortcutPolicy.StableProgram))
            {
                throw new CustomizationException(exceptionMessage: "Microsoft Edge is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled") { Arguments = [EdgeShortcutPolicy.StableProgram] };
            }

            // The stable channel is the one that is always installed. Windows creates the shortcut by default
            var createShortcut = Registry.LocalMachine.GetSubKeyValue(EdgeShortcutPolicy.Key, EdgeShortcutPolicy.Channels[0].ValueName) as int? ?? 1;
            return createShortcut == 0;
        }

        /// <summary>
        /// Get registry backup state.
        /// </summary>
        public static bool RegistryBackup()
        {
            var isEnabled = Registry.LocalMachine.GetSubKeyValue("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Configuration Manager", "EnablePeriodicBackup") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get restore previous folders state.
        /// </summary>
        public static bool RestorePreviousFolders()
        {
            var isEnabled = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced", "PersistBrowsers") as int? ?? -1;
            return isEnabled.Equals(1);
        }

        /// <summary>
        /// Get the name of the app that opens an extension or a protocol.
        /// </summary>
        /// <param name="extension">The extension with the dot, or the name of the protocol.</param>
        public static string SetAssociation(string extension) => AssociationService.GetDefaultApp(extension);

        /// <summary>
        /// Get the default apps saved to a file.
        /// </summary>
        /// <param name="file">The path of the JSON file.</param>
        public static IReadOnlyList<AssociationRecord> ImportAssociations(string file) => AssociationService.ReadExported(file);

        /// <summary>
        /// Get Windows Terminal default app state.
        /// </summary>
        public static int DefaultTerminalApp()
        {
            var appxPackage = AppxPackagesService.GetPackageOrDefault("Microsoft.WindowsTerminal");

            // The option is not available without Windows Terminal
            if (appxPackage is null)
            {
                throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled") { Arguments = ["Windows Terminal"] };
            }

            var appxPath = $"Software\\Classes\\PackagedCom\\Package\\{appxPackage.Id.FullName}\\Class";
            var consoleId = string.Empty;
            var consolePath = "Console\\%%Startup";
            var terminalId = string.Empty;
            using var packageClasses = Registry.LocalMachine.OpenSubKey(appxPath);
            packageClasses?.GetSubKeyNames()
                .ForEach(key =>
                {
                    switch (Registry.LocalMachine.GetSubKeyValue(Path.Combine(appxPath, key), "ServerId") ?? -1)
                    {
                        case 0:
                            consoleId = key;
                            break;
                        case 1:
                            terminalId = key;
                            break;
                        default:
                            break;
                    }
                });
            var delegationConsole = Registry.CurrentUser.GetSubKeyValue(consolePath, "DelegationConsole") as string ?? string.Empty;
            var delegationTerminal = Registry.CurrentUser.GetSubKeyValue(consolePath, "DelegationTerminal") as string ?? string.Empty;
            return consoleId.Length > 0 && delegationConsole.Equals(consoleId, StringComparison.OrdinalIgnoreCase) && delegationTerminal.Equals(terminalId, StringComparison.OrdinalIgnoreCase) ? 1 : 2;
        }

        /// <summary>
        /// Get .NET 10 desktop runtime install state.
        /// </summary>
        public static InstallState InstallDotNetRuntime_10() => GetDotNetRuntimeState("10", DataService.LatestReleaseNET10?.Version);

        /// <summary>
        /// Get .NET 11 desktop runtime install state.
        /// </summary>
        public static InstallState InstallDotNetRuntime_11() => GetDotNetRuntimeState("11", DataService.LatestReleaseNET11?.Version);

        /// <summary>
        /// Get Microsoft Visual C++ x86 redistributable package install state.
        /// </summary>
        public static InstallState InstallVisualC_x86() => GetVisualCState("vc_redist.x86.exe");

        /// <summary>
        /// Get Microsoft Visual C++ x64 redistributable package install state.
        /// </summary>
        public static InstallState InstallVisualC_x64() => GetVisualCState("vc_redist.x64.exe");

        /// <summary>
        /// Get Windows AI state.
        /// </summary>
        public static bool RemoveWindowsAI()
        {
            // Windows AI is considered removed when Recall is not enabled and the Copilot app is not installed
            return GetOptionalFeatureEnabled("Recall") != true && !AppxPackagesService.PackageExist("Microsoft.Copilot");
        }

        /// <summary>
        /// Gets a notice for the Windows AI setting: without an AI accelerator only Recall and the Copilot app are removed.
        /// </summary>
        public static string? RemoveWindowsAI_Notice() => InstrumentationService.HasAIAccelerator() ? null : "UIModel_Notice_NoAIAccelerator";

        /// <summary>
        /// Get the Linux distributions that can be installed with WSL. The installed ones are shown but cannot be chosen.
        /// </summary>
        public static IReadOnlyList<SettingChoice> InstallWSL()
        {
            var installed = WslService.GetInstalledDistributions();
            var installedText = "UIModel_InstallWSL_Installed".GetLocalized();

            return WslService.GetDistributions()
                .Select(distribution =>
                {
                    var isInstalled = installed.Contains(distribution.Name, StringComparer.OrdinalIgnoreCase);
                    var subtitle = isInstalled ? string.Format(installedText, distribution.Name) : distribution.Name;

                    // A new family has no logo until its file is added to the Assets folder
                    var icon = WslLogos.TryGetValue(distribution.Family, out var logo) ? new Uri($"ms-appx:///Assets/{logo}") : null;
                    return new SettingChoice(distribution.Name, distribution.FriendlyName, subtitle, !isInstalled, icon);
                })
                .ToArray();
        }

        /// <summary>
        /// Get the state of scanning the registry for policies. It is an action: there is nothing to read, so it is never chosen by itself.
        /// </summary>
        public static bool ScanRegistryPolicies()
        {
            if (!GroupPolicyService.EditorExists)
            {
                throw new CustomizationException(exceptionMessage: "The Local Group Policy Editor snap-in is not found", resourceKey: "UIModel_Exception_GpeditNotSupported");
            }

            return false;
        }

        /// <summary>
        /// Get Windows Sandbox state.
        /// </summary>
        public static bool WindowsSandbox()
        {
            static bool GetSandboxEnabled()
            {
                return GetOptionalFeatureEnabled("Containers-DisposableClientVM")
                    ?? throw new CustomizationException(exceptionMessage: "Windows Sandbox state undefined", resourceKey: "UIModel_Exception_WindowsSandbox_UndefinedState");
            }

            if (DataService.OsProperties.Edition.Contains("Professional", StringComparison.OrdinalIgnoreCase) || DataService.OsProperties.Edition.Contains("Enterprise", StringComparison.OrdinalIgnoreCase) || DataService.OsProperties.Edition.Contains("Education", StringComparison.OrdinalIgnoreCase))
            {
                // The checks below are required to enable the feature only, so it can always be disabled
                if (GetSandboxEnabled())
                {
                    return true;
                }

                // The feature needs virtualization: it is turned on in the firmware, or a hypervisor is already running
                if (!VirtualizationIsAvailable())
                {
                    throw new CustomizationException(exceptionMessage: "Enable virtualization in UEFI", resourceKey: "UIModel_Exception_VirtualizationDisabled");
                }

                return false;
            }

            throw new CustomizationException(exceptionMessage: "Unsupported Windows edition", resourceKey: "UIModel_Exception_WindowsSandbox_UnsupportedEdition") { Arguments = ["Windows Sandbox"] };
        }

        // The registry knows the state of a feature only after it was switched at least once: 1 is enabled or will be after a restart.
        // The registry is instant; Windows itself, asked otherwise, answers in about a second
        private static bool? GetOptionalFeatureEnabled(string name)
        {
            var selection = Registry.LocalMachine.GetSubKeyValue($"Software\\Microsoft\\Windows\\CurrentVersion\\Component Based Servicing\\Notifications\\OptionalFeatures\\{name}", "Selection") as int?;
            return selection is null ? InstrumentationService.GetOptionalFeatureEnabled(name) : selection == 1;
        }

        private static InstallState GetDotNetRuntimeState(string majorVersion, string? latestVersion)
        {
            // A runtime is installed if its installer is in the "Package Cache" folder
            var installed = PackageService.IsInstalled($"windowsdesktop-runtime-{majorVersion}.*-win-x64.exe");

            if (latestVersion is null)
            {
                // Without the list of releases a runtime cannot be installed or updated
                return installed
                    ? InstallState.Installed
                    : throw new CustomizationException(exceptionMessage: $"Url https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired") { Arguments = [$"https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json"] };
            }

            if (!installed)
            {
                return InstallState.NotInstalled;
            }

            return PackageService.IsInstalled($"windowsdesktop-runtime-{latestVersion}-win-x64.exe") ? InstallState.Installed : InstallState.UpdateAvailable;
        }

        private static InstallState GetVisualCState(string installerName)
        {
            var installedVersion = PackageService.GetInstalledVersion(installerName);
            var latestVersion = DataService.LatestReleaseVC?.Version;

            if (installedVersion is null)
            {
                // Without the latest release data the package cannot be installed
                return latestVersion is null
                    ? throw new CustomizationException(exceptionMessage: "Url https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired") { Arguments = ["https://raw.githubusercontent.com/ScoopInstaller/Extras/refs/heads/master/bucket/vcredist2022.json"] }
                    : InstallState.NotInstalled;
            }

            return latestVersion > installedVersion ? InstallState.UpdateAvailable : InstallState.Installed;
        }

        // "Microsoft:PowerPlan\{381b4222-f694-41f0-9685-ff5bb260df2e}" has the id of the plan in braces
        private static string GetPowerPlanId(string instanceId)
        {
            var start = instanceId.IndexOf('{');
            var end = instanceId.IndexOf('}');
            return start >= 0 && end > start ? instanceId[(start + 1)..end] : string.Empty;
        }
    }
}
