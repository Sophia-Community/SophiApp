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
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();

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
            var reportingTask = TaskService.Instance.GetTask("Microsoft\\Windows\\Windows Error Reporting\\QueueReporting") ?? throw new InvalidOperationException($"Failed to find a scheduled task");
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
            return string.IsNullOrEmpty(userCastToDevice) || string.IsNullOrEmpty(machineCastToDevice);
        }

        /// <summary>
        /// Get "Share" context menu item state.
        /// </summary>
        public static bool ShareContext()
        {
            var shareContext = Registry.ClassesRoot.OpenSubKey("AllFilesystemObjects\\shellex\\ContextMenuHandlers\\ModernSharing")?.GetValue("(default)") as string;
            return shareContext?.Equals("{e2bf9676-5f8f-435c-97eb-11607a5bedf7}") ?? false;
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

            throw new InvalidOperationException($"Appx package \"{clipChampAppx}\" not found in current user environment");
        }

        /// <summary>
        /// Get "Edit with Paint 3D" item in the media files context menu state.
        /// </summary>
        public static bool EditWithPaint3DContext()
        {
            var paintAppx = "Microsoft.MSPaint";

            if (AppxPackagesService.PackageExist(paintAppx))
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

            throw new InvalidOperationException($"Appx package \"{paintAppx}\" not found in current user environment");
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
            var libraryLocation = Registry.ClassesRoot.OpenSubKey("Folder\\ShellEx\\ContextMenuHandlers\\Library Location")?.GetValue("(default)") as string;
            return !libraryLocation?.Equals("-{3dad6c5d-2167-4cae-9914-f99e41c12cfa}") ?? true;
        }

        /// <summary>
        /// Get Send to" item in the folders context menu state.
        /// </summary>
        public static bool SendToContext()
        {
            var sendTo = Registry.ClassesRoot.OpenSubKey("AllFilesystemObjects\\shellex\\ContextMenuHandlers\\SendTo")?.GetValue("(default)") as string;
            return !sendTo?.Equals("-{7BA4C740-9E81-11CF-99D3-00AA004AE837}") ?? true;
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
            var terminalAppx = "Microsoft.WindowsTerminal";

            if (AppxPackagesService.PackageExist(terminalAppx))
            {
                var extensionsBlockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
                var terminalContextGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

                var userBlockedGuid = Registry.CurrentUser.OpenSubKey(extensionsBlockPath)?.GetValue(terminalContextGuid);
                var machineBlockedGuid = Registry.LocalMachine.OpenSubKey(extensionsBlockPath)?.GetValue(terminalContextGuid);
                return userBlockedGuid is null && machineBlockedGuid is null;
            }

            throw new InvalidOperationException($"Appx package \"{terminalAppx}\" not found in current user environment");
        }

        /// <summary>
        /// Get Open Windows Terminal from context menu as administrator by default state.
        /// </summary>
        public static bool OpenWindowsTerminalAdminContext()
        {
            var terminalAppx = "Microsoft.WindowsTerminal";

            if (AppxPackagesService.PackageExist(terminalAppx))
            {
                var extensionsBlockPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Blocked";
                var adminContextGuid = "{9F156763-7844-4DC4-B2B1-901F640F5155}";

                var userBlockedGuid = Registry.CurrentUser.OpenSubKey(extensionsBlockPath)?.GetValue(adminContextGuid);
                var machineBlockedGuid = Registry.LocalMachine.OpenSubKey(extensionsBlockPath)?.GetValue(adminContextGuid);

                if (userBlockedGuid is not null && machineBlockedGuid is not null)
                {
                    try
                    {
                        var terminalSettings = $@"{Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")}\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json";
                        var jsonSettings = File.ReadAllText(terminalSettings, Encoding.UTF8);
                        var jsonProfile = JsonExtensions.ToObject<MsTerminalSettingsDto>(jsonSettings);
                        return jsonProfile.Profiles.Defaults.Elevate ?? false;
                    }
                    catch (ArgumentException)
                    {
                        throw new InvalidOperationException($"The configuration file of appx package \"{terminalAppx}\" is not valid");
                    }
                }

                return false;
            }

            throw new InvalidOperationException($"Appx package \"{terminalAppx}\" not found in current user environment");
        }

        /// <summary>
        /// Get Windows 10 context menu style state.
        /// </summary>
        public static bool Windows10ContextMenu()
        {
            var contextMenuPath = "Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32";
            var contextMenuValue = Registry.CurrentUser.OpenSubKey(contextMenuPath)?.GetValue("(default)") as string;
            return contextMenuValue?.Equals(string.Empty) ?? false;
        }
    }
}
