// <copyright file="Accessors.ContextMenu.cs" company="Team Sophia">
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
    /// The settings of the "ContextMenu" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Get "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        public static bool MSIExtractContext()
        {
            var muiVerb = Registry.ClassesRoot.GetSubKeyValue("Msi.Package\\shell\\Extract", "MUIVerb") as string;
            return muiVerb?.Equals("@shell32.dll,-37514") ?? false;
        }

        /// <summary>
        /// Get "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        public static bool CABInstallContext()
        {
            var cabProgId = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice", "ProgId") as string ?? string.Empty;
            using var cabUserChoice = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.cab\\UserChoice");
            var cabArchiver = cabProgId.Equals("CABFolder", StringComparison.OrdinalIgnoreCase) || cabUserChoice is null;

            if (cabArchiver)
            {
                var muiVerb = Registry.ClassesRoot.GetSubKeyValue("CABFolder\\Shell\\runas", "MUIVerb") as string ?? string.Empty;
                return muiVerb.Equals("@shell32.dll,-10210");
            }

            throw new CustomizationException(exceptionMessage: "A third-party archiver is set as the default archiver", resourceKey: "UIModel_Exception_CABInstallContext_DefaultArchiver");
        }

        /// <summary>
        /// Get "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        public static bool UseStoreOpenWith()
        {
            var storeOpenWith = Registry.CurrentUser.GetSubKeyValue("Software\\Policies\\Microsoft\\Windows\\Explorer", "NoUseStoreOpenWith") as int?;
            return !storeOpenWith?.Equals(1) ?? true;
        }

        /// <summary>
        /// Get Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        public static bool OpenWindowsTerminalAdminContext()
        {
            if (AppxPackagesService.PackageExist("Microsoft.WindowsTerminal"))
            {
                var userBlockedGuid = Registry.CurrentUser.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                var machineBlockedGuid = Registry.LocalMachine.GetSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");

                if (userBlockedGuid is null && machineBlockedGuid is null)
                {
                    var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";

                    try
                    {
                        // Windows Terminal creates the configuration file on its first launch. It will be created when the option is applied
                        if (!File.Exists(terminalSettings))
                        {
                            return false;
                        }

                        var jsonSettings = File.ReadAllText(terminalSettings, Encoding.UTF8);
                        var jsonProfile = Json.ToObject<MsTerminalSettings>(jsonSettings);
                        return jsonProfile?.Profiles?.Defaults?.Elevate ?? false;
                    }
                    catch (Exception ex) when (ex is ArgumentException or System.Text.Json.JsonException or FileNotFoundException or DirectoryNotFoundException)
                    {
                        throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal configuration file is not valid", resourceKey: "UIModel_Exception_OpenWindowsTerminalAdminContext_ConfigurationNotValid") { Arguments = [terminalSettings] };
                    }
                }

                // The Windows Terminal context menu shell extension is blocked
                return false;
            }

            throw new CustomizationException(exceptionMessage: "Microsoft.WindowsTerminal package is not installed", resourceKey: "UIModel_Exception_PackageNotInstalled") { Arguments = ["Windows Terminal"] };
        }
    }
}
