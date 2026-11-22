// <copyright file="Mutators.ContextMenu.cs" company="Team Sophia">
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
    /// The settings of the "ContextMenu" page.
    /// </content>
    public static partial class Mutators
    {
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

                // Only one value is changed, the rest of the file is written back as it was read
                var settings = Json.ToObject<JsonObject>(File.ReadAllText(terminalSettings, Encoding.UTF8));
                var defaults = settings["profiles"]?["defaults"] as JsonObject ?? throw new InvalidOperationException("The default profile settings were not found in the terminal configuration file");

                if (enable)
                {
                    // Unblock the Windows Terminal context menu shell extension
                    Registry.CurrentUser.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                    Registry.LocalMachine.DeleteSubKeyValue("Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked", "{9F156763-7844-4DC4-B2B1-901F640F5155}");
                }

                defaults["elevate"] = enable;
                File.WriteAllText(terminalSettings, Json.Stringify(settings), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed write data to terminal configuration file", ex);
            }
        }
    }
}
