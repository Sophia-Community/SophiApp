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
    /// Sets the os settings.
    /// </summary>
    public static class Mutators
    {
        private static readonly ICommonDataService CommonDataService = App.GetService<ICommonDataService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();

        /// <summary>
        /// Sets DiagTrack service state.
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
        /// Sets Windows feature "Diagnostic data level" state.
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
                ?.DeleteValue("AllowTelemetry", true);
        }

        /// <summary>
        /// Sets Windows feature "Error reporting" state.
        /// </summary>
        /// <param name="isEnabled">Feature state.</param>
        public static void ErrorReporting(bool isEnabled)
        {
            var reportingRegistryPath = "Software\\Microsoft\\Windows\\Windows Error Reporting";
            var reportingTask = TaskService.Instance.GetTask("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting");
            var reportingService = new ServiceController("WerSvc");

            if (isEnabled)
            {
                reportingTask.Enabled = true;
                Registry.CurrentUser.OpenSubKey(reportingRegistryPath, true)?.DeleteValue("Disabled", true);
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
        /// Sets Windows feature "Feedback frequency" state.
        /// </summary>
        /// <param name="state">Feedback frequency state.</param>
        public static void FeedbackFrequency(int state)
        {
            var siufRulesPath = "Software\\Microsoft\\Siuf\\Rules";

            if (state.Equals(2))
            {
                Registry.CurrentUser.OpenOrCreateSubKey(siufRulesPath)
                    .SetValue("NumberOfSIUFInPeriod", 0, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.DeleteSubKey(siufRulesPath, false);
        }

        /// <summary>
        /// Sets telemetry scheduled tasks state.
        /// </summary>
        /// <param name="isEnabled">Scheduled tasks state.</param>
        public static void ScheduledTasks(bool isEnabled)
        {
            new List<Task?>()
             {
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\MareBackup"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\Microsoft Compatibility Appraiser"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\StartupAppTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Application Experience\\ProgramDataUpdater"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Autochk\\Proxy"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\Consolidator"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Customer Experience Improvement Program\\UsbCeip"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsToastTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Maps\\MapsUpdateTask"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyMonitor"),
                TaskService.Instance.GetTask("\\Microsoft\\Windows\\Shell\\FamilySafetyRefreshTask"),
                TaskService.Instance.GetTask("\\Microsoft\\XblGameSave\\XblGameSaveTask"),
                TaskService.Instance.GetTask("\\Microsoft\\XblGameSave\\XblGameSaveTask1"),
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
        /// Sets Windows feature "Sign-in info" state.
        /// </summary>
        /// <param name="isEnabled">Sign-in info state.</param>
        public static void SigninInfo(bool isEnabled)
        {
            var sid = InstrumentationService.GetUserSid(Environment.UserName);
            var userArsoPath = $"Software\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon\\UserARSO\\{sid}";
            var optOut = "OptOut";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(userArsoPath, true)?.DeleteValue(optOut, true);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(userArsoPath).SetValue(optOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets language list access state.
        /// </summary>
        /// <param name="isEnabled">Language list state.</param>
        public static void LanguageListAccess(bool isEnabled)
        {
            var userProfilePath = "Control Panel\\International\\User Profile";
            var httpOptOut = "HttpAcceptLanguageOptOut";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.DeleteValue(httpOptOut, true);
                return;
            }

            Registry.CurrentUser.OpenSubKey(userProfilePath, true)?.SetValue(httpOptOut, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the permission for apps to use advertising ID state.
        /// </summary>
        /// <param name="isEnabled">Advertising ID state.</param>
        public static void AdvertisingID(bool isEnabled)
        {
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo")
                .SetValue("Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the Windows welcome experiences state.
        /// </summary>
        /// <param name="isEnabled">Windows welcome experiences state.</param>
        public static void WindowsWelcomeExperience(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-310093Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows tips state.
        /// </summary>
        /// <param name="isEnabled">Windows tips state.</param>
        public static void WindowsTips(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SubscribedContent-338389Enabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the suggested content in the Settings app state.
        /// </summary>
        /// <param name="isEnabled">Suggested content state.</param>
        public static void SettingsSuggestedContent(bool isEnabled)
        {
            new List<string> { "SubscribedContent-353694Enabled", "SubscribedContent-353696Enabled", "SubscribedContent-338393Enabled" }
            .ForEach(content => Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue(content, isEnabled ? 1 : 0, RegistryValueKind.DWord));
        }

        /// <summary>
        /// Sets the automatic installing suggested apps state.
        /// </summary>
        /// <param name="isEnabled">Suggested apps state.</param>
        public static void AppsSilentInstalling(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager", true)
                ?.SetValue("SilentInstalledAppsEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets the Windows feature "Whats New" state.
        /// </summary>
        /// <param name="isEnabled">Whats New state.</param>
        public static void WhatsNewInWindows(bool isEnabled)
        {
            Registry.CurrentUser.OpenOrCreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\UserProfileEngagement")
                .SetValue("ScoobeSystemSettingEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows feature "Tailored experiences" state.
        /// </summary>
        /// <param name="isEnabled">Tailored experiences state.</param>
        public static void TailoredExperiences(bool isEnabled)
        {
            Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Privacy", true)
                ?.SetValue("TailoredExperiencesWithDiagnosticDataEnabled", isEnabled ? 1 : 0, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets Windows feature "Bing search" state.
        /// </summary>
        /// <param name="isEnabled">Bing search state.</param>
        public static void BingSearch(bool isEnabled)
        {
            var explorerPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var disableSuggestions = "DisableSearchBoxSuggestions";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(explorerPath, true)?.DeleteValue(disableSuggestions, true);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(explorerPath).SetValue(disableSuggestions, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets "Extract all" item in the Windows Installer (.msi) context menu state.
        /// </summary>
        /// <param name="isEnabled">"Extract all" item state.</param>
        public static void MSIExtractContext(bool isEnabled)
        {
            var msiExtractPath = "Msi.Package\\shell\\Extract";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey($"{msiExtractPath}\\Command")
                    .SetValue(string.Empty, "msiexec.exe /a \"%1\" /qb TARGETDIR=\"%1 extracted\"", RegistryValueKind.String);

                Registry.ClassesRoot.OpenOrCreateSubKey(msiExtractPath)
                    .SetValue("MUIVerb", "@shell32.dll,-37514", RegistryValueKind.String);

                Registry.ClassesRoot.OpenOrCreateSubKey(msiExtractPath)
                    .SetValue("Icon", "shell32.dll,-16817", RegistryValueKind.String);

                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(msiExtractPath, true);
        }

        /// <summary>
        /// Sets "Install" item in the Cabinet archives (.cab) context menu state.
        /// </summary>
        /// <param name="isEnabled">"Install" item state.</param>
        public static void CABInstallContext(bool isEnabled)
        {
            var runasPath = "CABFolder\\Shell\\runas";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey($"{runasPath}\\Command")
                    .SetValue(string.Empty, "cmd /c DISM.exe /Online /Add-Package /PackagePath:\"%1\" /NoRestart & pause", RegistryValueKind.String);

                Registry.ClassesRoot.OpenOrCreateSubKey(runasPath)
                    .SetValue("MUIVerb", "@shell32.dll,-10210", RegistryValueKind.String);

                Registry.ClassesRoot.OpenOrCreateSubKey(runasPath)
                    .SetValue("HasLUAShield", string.Empty, RegistryValueKind.String);

                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(runasPath, true);
        }

        /// <summary>
        /// Sets "Run as different user" item in the executable files context menu (.exe) state.
        /// </summary>
        /// <param name="isEnabled">"Run as different user" item state.</param>
        public static void RunAsDifferentUserContext(bool isEnabled)
        {
            var runasPath = "exefile\\shell\\runasuser";
            var runExtendedValue = "Extended";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(runasPath, true)?.DeleteValue(runExtendedValue, true);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(runasPath, true)?.SetValue(runExtendedValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Cast to Device" item in the media files and folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Cast to Device" item state.</param>
        public static void CastToDeviceContext(bool isEnabled)
        {
            var blockedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var castToDeviceGuid = "{7AD84985-87B4-4a16-BE58-8B72A5B390F7}";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(blockedPath, true)?.DeleteValue(castToDeviceGuid, true);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(blockedPath).SetValue(castToDeviceGuid, "Play to menu", RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Share" context menu item state.
        /// </summary>
        /// <param name="isEnabled">"Share" item state.</param>
        public static void ShareContext(bool isEnabled)
        {
            var blockedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var shareContextGuid = "{E2BF9676-5F8F-435C-97EB-11607A5BEDF7}";

            if (isEnabled)
            {
                Registry.LocalMachine.OpenSubKey(blockedPath, true)?.DeleteValue(shareContextGuid, true);
                return;
            }

            Registry.LocalMachine.OpenOrCreateSubKey(blockedPath).SetValue(shareContextGuid, "Play to menu", RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Edit With Clipchamp" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit With Clipchamp" item state.</param>
        public static void EditWithClipchampContext(bool isEnabled)
        {
            var clipChampPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var clipChampGuid = "{8AB635F8-9A67-4698-AB99-784AD929F3B4}";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(clipChampPath, true)?.DeleteValue(clipChampGuid, true);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(clipChampPath).SetValue(clipChampGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Edit with Paint 3D" item in the media files context menu state.
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
                    Registry.ClassesRoot.OpenSubKey(fileTypePath, true)?.DeleteValue(paintContextValue, true);
                    return;
                }

                Registry.ClassesRoot.OpenSubKey(fileTypePath, true)?.SetValue(paintContextValue, string.Empty, RegistryValueKind.String);
            });
        }

        /// <summary>
        /// Sets "Edit with Photos" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit with Photos" item state.</param>
        public static void EditWithPhotosContext(bool isEnabled)
        {
            var photoContextPath = "AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\\Shell\\ShellEdit";
            var photosContextValue = "ProgrammaticAccessOnly";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(photoContextPath, true)?.DeleteValue(photosContextValue, true);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(photoContextPath, true)?.SetValue(photosContextValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Create a new video" item in the media files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Create a new video" item state.</param>
        public static void CreateANewVideoContext(bool isEnabled)
        {
            var videoCreatePath = "AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\\Shell\\ShellCreateVideo";
            var videoContextValue = "ProgrammaticAccessOnly";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(videoCreatePath, true)?.DeleteValue(videoContextValue, true);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(videoCreatePath, true)?.SetValue(videoContextValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Edit" item in the image files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Edit" item state.</param>
        public static void ImagesEditContext(bool isEnabled)
        {
            var editContextPath = "SystemFileAssociations\\image\\shell\\edit";
            var imagesContextValue = "ProgrammaticAccessOnly";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(editContextPath, true)?.DeleteValue(imagesContextValue, true);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(editContextPath, true)?.SetValue(imagesContextValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Print" item in the .bat and .cmd files context menu state.
        /// </summary>
        /// <param name="isEnabled">"Print" item state.</param>
        public static void PrintCMDContext(bool isEnabled)
        {
            var batPrintPath = "batfile\\shell\\print";
            var cmdPrintPath = "cmdfile\\shell\\print";
            var printContextValue = "ProgrammaticAccessOnly";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(batPrintPath, true)?.DeleteValue(printContextValue, true);
                Registry.ClassesRoot.OpenSubKey(cmdPrintPath, true)?.DeleteValue(printContextValue, true);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(batPrintPath, true)?.SetValue(printContextValue, string.Empty, RegistryValueKind.String);
            Registry.ClassesRoot.OpenSubKey(cmdPrintPath, true)?.SetValue(printContextValue, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Include in Library" item in the folders and drives context menu state.
        /// </summary>
        /// <param name="isEnabled">"Include in Library" item state.</param>
        public static void IncludeInLibraryContext(bool isEnabled)
        {
            var libraryPath = "Folder\\ShellEx\\ContextMenuHandlers\\Library Location";
            var enableContextValue = "{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";
            var disableContextValue = "-{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(libraryPath, true)?.SetValue(string.Empty, enableContextValue, RegistryValueKind.String);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(libraryPath, true)?.SetValue(string.Empty, disableContextValue, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Send to" item in the folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Send to" item state.</param>
        public static void SendToContext(bool isEnabled)
        {
            var sendToPath = "AllFilesystemObjects\\shellex\\ContextMenuHandlers\\SendTo";
            var enableContextValue = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
            var disableContextValue = "-{7BA4C740-9E81-11CF-99D3-00AA004AE837}";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenSubKey(sendToPath, true)?.SetValue(string.Empty, enableContextValue, RegistryValueKind.String);
                return;
            }

            Registry.ClassesRoot.OpenSubKey(sendToPath, true)?.SetValue(string.Empty, disableContextValue, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets "Bitmap image" item in the "New" context menu state.
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

            Registry.ClassesRoot.DeleteSubKeyTree(bmpShellPath, true);
        }

        /// <summary>
        /// Sets "Rich Text Document" item in the "New" context menu state.
        /// </summary>
        /// <param name="isEnabled">"Rich Text Document" item state.</param>
        public static void RichTextDocumentNewContext(bool isEnabled)
        {
            var rtfShellPath = ".rtf\\ShellNew";
            var wordPadPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)}\\Windows NT\\Accessories\\wordpad.exe";

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey(rtfShellPath).SetValue("Data", @"{\rtf1}", RegistryValueKind.String);
                Registry.ClassesRoot.OpenSubKey(rtfShellPath, true)?.SetValue("ItemName", $"{wordPadPath},-213", RegistryValueKind.ExpandString);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(rtfShellPath, true);
        }

        /// <summary>
        /// Sets "Compressed (zipped) Folder" item in the "New" context menu state.
        /// </summary>
        /// <param name="isEnabled">"Compressed (zipped) Folder" item state.</param>
        public static void CompressedFolderNewContext(bool isEnabled)
        {
            var zipShellPath = ".zip\\CompressedFolder\\ShellNew";
            var zipDllPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\\System32\\zipfldr.dll";
            var zipContextValue = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            if (isEnabled)
            {
                Registry.ClassesRoot.OpenOrCreateSubKey(zipShellPath).SetValue("Data", zipContextValue, RegistryValueKind.Binary);
                Registry.ClassesRoot.OpenSubKey(zipShellPath, true)?.SetValue("ItemName", $"{zipDllPath},-10194", RegistryValueKind.ExpandString);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(zipDllPath, true);
        }

        /// <summary>
        /// Sets "Open", "Print", and "Edit" context menu items available when selecting more than 15 files state.
        /// </summary>
        /// <param name="isEnabled">"Open", "Print", and "Edit" context menu items state.</param>
        public static void MultipleInvokeContext(bool isEnabled)
        {
            var invokeContextPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer";
            var invokeContextName = "MultipleInvokePromptMinimum";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(invokeContextPath, true)?.SetValue(invokeContextName, 300, RegistryValueKind.DWord);
                return;
            }

            Registry.CurrentUser.OpenSubKey(invokeContextPath, true)?.DeleteValue(invokeContextName, true);
        }

        /// <summary>
        /// Sets "Look for an app in the Microsoft Store" items in the "Open with" dialog state.
        /// </summary>
        /// <param name="isEnabled">"Look for an app in the Microsoft Store" items state.</param>
        public static void UseStoreOpenWith(bool isEnabled)
        {
            var storeContextPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var storeContextValue = "NoUseStoreOpenWith";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(storeContextPath, true)?.DeleteValue(storeContextValue, true);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(storeContextPath).SetValue(storeContextValue, 1, RegistryValueKind.DWord);
        }

        /// <summary>
        /// Sets "Open in Windows Terminal" item in the folders context menu state.
        /// </summary>
        /// <param name="isEnabled">"Open in Windows Terminal" item state.</param>
        public static void OpenWindowsTerminalContext(bool isEnabled)
        {
            var extensionsBlockPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
            var terminalGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenSubKey(extensionsBlockPath, true)?.DeleteValue(terminalGuid, true);
                return;
            }

            Registry.CurrentUser.OpenOrCreateSubKey(extensionsBlockPath).SetValue(terminalGuid, string.Empty, RegistryValueKind.String);
        }

        /// <summary>
        /// Sets Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        /// <param name="isEnabled">"Open in Windows Terminal as Administrator" item state.</param>
        public static void OpenWindowsTerminalAdminContext(bool isEnabled)
        {
            try
            {
                var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";
                var deserializedSettings = JsonConvert.DeserializeObject(File.ReadAllText(terminalSettings, Encoding.UTF8)) as JObject;
                var elevateSetting = deserializedSettings?.SelectToken("profiles.defaults.elevate");

                if (elevateSetting is null)
                {
                    var defaultsSetting = deserializedSettings!.SelectToken("profiles.defaults") as JObject;
                    defaultsSetting!.Add(new JProperty("elevate", string.Empty));
                    elevateSetting = deserializedSettings!.SelectToken("profiles.defaults.elevate");
                }

                elevateSetting!.Replace(isEnabled);
                File.WriteAllText(terminalSettings, deserializedSettings!.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to write data to \"Microsoft.WindowsTerminal\" configuration file", ex);
            }
        }

        /// <summary>
        /// Sets Windows 10 context menu style state.
        /// </summary>
        /// <param name="isEnabled">Windows 10 context menu style state.</param>
        public static void Windows10ContextMenu(bool isEnabled)
        {
            var contextMenuPath = "Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}";
            var contextMenuValue = "InprocServer32";

            if (isEnabled)
            {
                Registry.CurrentUser.OpenOrCreateSubKey($"{contextMenuPath}\\{contextMenuValue}")
                    .SetValue(string.Empty, string.Empty, RegistryValueKind.String);

                return;
            }

            Registry.CurrentUser.DeleteSubKeyTree(contextMenuPath, true);
        }
    }
}
