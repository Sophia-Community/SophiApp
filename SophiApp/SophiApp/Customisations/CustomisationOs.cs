using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.ServiceProcess;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public static class CustomisationOs
    {
        public static void _100(bool IsChecked)
        {
            var diagTrack = ServiceHelper.Get(_100_DIAG_TRACK);
            var firewallRule = FirewallHelper.GetGroupRule(_100_DIAG_TRACK).FirstOrDefault();

            if (IsChecked)
            {
                ServiceHelper.SetStartMode(diagTrack, ServiceStartMode.Automatic);
                diagTrack.Start();
                firewallRule.Enabled = true;
                firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                return;
            }

            diagTrack.Stop();
            ServiceHelper.SetStartMode(diagTrack, ServiceStartMode.Disabled);
            firewallRule.Enabled = false;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

        public static void _102(bool _)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, ALLOW_TELEMETRY, DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, MAX_TELEMETRY_ALLOWED, DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL, DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _103(bool _)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine,
                                DATA_COLLECTION_PATH, ALLOW_TELEMETRY,
                                    OsHelper.IsEdition(WIN_VER_ENT) || OsHelper.IsEdition(WIN_VER_EDU)
                                        ? MIN_ENT_TELEMETRY_VALUE : MIN_TELEMETRY_VALUE,
                                            RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, MAX_TELEMETRY_ALLOWED, MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL, MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _104(bool IsChecked)
        {
            var werService = ServiceHelper.Get(_104_WER_SERVICE);

            if (IsChecked)
            {
                ScheduledTaskHelper.TryChangeTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK, true);
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _104_WER_PATH, _104_DISABLED);
                ServiceHelper.SetStartMode(werService, ServiceStartMode.Manual);
                werService.Start();
                return;
            }

            if (OsHelper.IsEdition(_104_CORE).Invert())
            {
                ScheduledTaskHelper.TryChangeTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK, false);
                RegHelper.SetValue(RegistryHive.CurrentUser, _104_WER_PATH, _104_DISABLED, _104_DISABLED_DEFAULT_VALUE, RegistryValueKind.DWord);
            }

            werService.Stop();
            ServiceHelper.SetStartMode(werService, ServiceStartMode.Disabled);
        }

        public static void _106(bool _) => RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, SIUF_PATH);

        public static void _107(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, SIUF_PATH, SIUF_PERIOD, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _109(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(_109_DATA_UPDATER_TASK_PATH, _109_DATA_UPDATER_TASK, IsChecked);

        public static void _110(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(_110_PROXY_TASK_PATH, _110_PROXY_TASK, IsChecked);

        public static void _111(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(CEIP_TASK_PATH, _111_CONS_TASK, IsChecked);

        public static void _112(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(CEIP_TASK_PATH, _112_USB_CEIP_TASK, IsChecked);

        public static void _113(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(_113_DISK_DATA_TASK_PATH, _113_DISK_DATA_TASK, IsChecked);

        public static void _114(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(MAPS_TASK_PATH, _114_MAPS_TOAST_TASK, IsChecked);

        public static void _115(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(MAPS_TASK_PATH, _115_MAPS_UPDATE, IsChecked);

        public static void _116(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(_116_FAMILY_MONITOR_TASK_PATH, _116_FAMILY_MONITOR_TASK, IsChecked);

        public static void _117(bool IsChecked) => ScheduledTaskHelper.TryChangeTaskState(_117_XBOX_SAVE_TASK_PATH, _117_XBOX_SAVE_TASK, IsChecked);

        public static void _118(bool IsChecked)
        {
            var userArso = $"{_118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}";

            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, userArso, _118_OPT_OUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, userArso, _118_OPT_OUT, _118_OPT_OUT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _119(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _119_USER_PROFILE_PATH, _119_HTTP_ACCEPT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _119_USER_PROFILE_PATH, _119_HTTP_ACCEPT, _119_HTTP_ACCEPT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _120(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _120_ADVERT_INFO_PATH,
                                                                            _120_ADVERT_ENABLED,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _121(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _121_SUB_CONTENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _122(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _122_SUB_CONTENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _123(bool IsChecked)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_93,
                                IsChecked ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_94,
                                IsChecked ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_96,
                                IsChecked ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _124(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _124_SILENT_APP_INSTALL,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _125(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _125_PROFILE_ENGAGE_PATH,
                                                                            _125_SETTING_ENABLED,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _126(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _126_PRIVACY_PATH,
                                                                            _126_TAILORED_DATA,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _127(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, POLICIES_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, POLICIES_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS, ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _201(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _202(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER);

        public static void _203(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _203_AUTO_CHECK_SELECT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _204(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _204_HIDDEN,
                                                                                IsChecked ? _204_ENABLED_VALUE
                                                                                         : _204_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _205(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _205_HIDE_FILE_EXT,
                                                                                IsChecked ? _205_SHOW_VALUE
                                                                                         : _205_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _206(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _206_HIDE_MERGE_CONF,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _208(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                ADVANCED_EXPLORER_PATH,
                                                                    LAUNCH_TO,
                                                                        LAUNCH_PC_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _209(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                ADVANCED_EXPLORER_PATH,
                                                                    LAUNCH_TO,
                                                                        LAUNCH_QA_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _210(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _210_CORTANA_BUTTON,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _211(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _211_SHOW_SYNC_PROVIDER,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _212(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _212_SNAP_ASSIST,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _214(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                STATUS_MANAGER_PATH,
                                                                    ENTHUSIAST_MODE,
                                                                        DIALOG_DETAILED_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _215(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                STATUS_MANAGER_PATH,
                                                                    ENTHUSIAST_MODE,
                                                                        DIALOG_COMPACT_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _216(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _216_RIBBON_EXPLORER_PATH,
                                                                            _216_TABLET_MODE_OFF,
                                                                                IsChecked ? _216_MINIMIZED_VALUE
                                                                                         : _216_EXPANDED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _217(bool IsChecked)
        {
            var shellState = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE);
            shellState[4] = IsChecked ? _217_SHELL_ENABLED_VALUE : _217_SHELL_DISABLED_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE, shellState, RegistryValueKind.Binary);
        }

        public static void _218(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY, _218_3D_OBJECT_HIDE_VALUE);
        }

        public static void _219(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _219_SHOW_RECENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _220(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _220_SHOW_FREQUENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _221(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _221_SHOW_TASK_VIEW,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _222(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _222_PEOPLE_EXPLORER_PATH,
                                                                            _222_PEOPLE_BAND,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _223(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _223_SHOW_SECONDS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _225(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_HIDE_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _226(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_ICON_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _227(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_BOX_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _228(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _228_PEN_WORKSPACE_PATH,
                                                                            _228_PEN_WORKSPACE_VISIBILITY,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _229(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _229_AUTO_TRAY,
                                                                                IsChecked ? _229_AUTO_TRAY_SHOW_VALUE
                                                                                         : _229_AUTO_TRAY_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _230(bool IsChecked)
        {
            var settings = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS);
            settings[9] = IsChecked ? _230_STUCK_RECTS3_SHOW_VALUE : _230_STUCK_RECTS3_HIDE_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS, settings, RegistryValueKind.Binary);
        }

        public static void _231(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _213_FEEDS_PATH,
                                                                            _231_SHELL_FEEDS,
                                                                                IsChecked ? _231_SHELL_FEEDS_ENABLED_VALUE
                                                                                         : _231_SHELL_FEEDS_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _233(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _234(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_SMALL_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _235(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_CATEGORY_VALUE, RegistryValueKind.DWord);
        }

        public static void _237(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME, LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _238(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME, DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _240(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME, LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _241(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME, DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _242(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _242_NO_NEW_APP_ALERT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _242_NO_NEW_APP_ALERT, _242_HIDE_ALERT_VALUE, RegistryValueKind.DWord);
        }

        public static void _243(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        WINLOGON_PATH,
                                                                            _243_FIRST_LOGON_ANIMATION,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _245(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                CONTROL_PANEL_DESKTOP_PATH,
                                                                    JPEG_QUALITY,
                                                                        _245_JPEG_MAX_QUALITY,
                                                                            RegistryValueKind.DWord);

        public static void _246(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser,
                                                                CONTROL_PANEL_DESKTOP_PATH,
                                                                    JPEG_QUALITY);

        public static void _247(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _247_WINDOWS_UPDATE_SETTINGS_PATH,
                                                                            _247_RESTART_NOTIFICATIONS,
                                                                                IsChecked ? _247_SHOW_VALUE
                                                                                         : _247_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _248(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _248_EXPLORER_NAMING_PATH, _248_SHORTCUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _248_EXPLORER_NAMING_PATH, _248_SHORTCUT, _248_DISABLE_VALUE);
        }

        public static void _249(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _249_CONTROL_PANEL_KEYBOARD_PATH,
                                                                            _249_PRINT_SCREEN_SNIPPING,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _250(bool IsChecked) => SystemParametersHelper.SetInputSettings(IsChecked);

        public static void _251(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _251_DISALLOW_WINDOWS_SHAKE,
                                                                                IsChecked ? _251_ENABLED_VALUE
                                                                                         : _251_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _300(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            STORAGE_POLICY_01,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _302(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, _302_STORAGE_POLICY_MONTH_VALUE, RegistryValueKind.DWord);

        public static void _303(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _304(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            _304_STORAGE_POLICY_04,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _305(bool IsChecked) => ProcessHelper.Start(POWERCFG_EXE, IsChecked ? _305_HIBERNATE_ON : _305_HIBERNATE_OFF, ProcessWindowStyle.Hidden);

        public static void _307(bool _)
        {
            var localAppDataTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_LOCAL_APPDATA}\\{TEMP_FOLDER}");
            var systemDriveTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_DRIVE}\\{TEMP_FOLDER}");
            var systemRootTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_ROOT}\\{TEMP_FOLDER}");
            var currentTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_TEMP}");
            var userName = Environment.UserName;

            ServiceHelper.Restart(SERVICE_SPOOLER);
            OneDriveHelper.StopProcesses();
            FileHelper.CreateDirectory(systemDriveTemp);
            FileHelper.DirectoryLazyDelete(systemRootTemp);

            if (FileHelper.IsSymbolicLink(currentTemp).Invert())
            {
                FileHelper.DirectoryLazyDelete(currentTemp);

                if (FileHelper.DirectoryIsEmpty(currentTemp))
                {
                    FileHelper.DirectoryDelete(localAppDataTemp);
                    FileHelper.CreateDirectoryLink(localAppDataTemp, systemDriveTemp);
                }
                else
                {
                    ScheduledTaskHelper.RegisterLogonTask(name: _307_SYMBOLIC_LINK_TASK, description: null, execute: POWERSHELL_EXE, args: _307_TASK_ARGS, username: userName);
                }
            }

            Environment.SetEnvironmentVariable(TMP, systemDriveTemp, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(TMP, systemDriveTemp, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(TMP, systemDriveTemp, EnvironmentVariableTarget.Process);
            RegHelper.SetValue(RegistryHive.CurrentUser, ENVIRONMENT, TMP, _307_SYSTEM_DRIVE_TEMP, RegistryValueKind.ExpandString);
            Environment.SetEnvironmentVariable(TEMP, systemDriveTemp, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(TEMP, systemDriveTemp, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(TEMP, systemDriveTemp, EnvironmentVariableTarget.Process);
            RegHelper.SetValue(RegistryHive.CurrentUser, ENVIRONMENT, TEMP, _307_SYSTEM_DRIVE_TEMP, RegistryValueKind.ExpandString);
            RegHelper.SetValue(RegistryHive.LocalMachine, SESSION_MANAGER_ENVIRONMENT, TMP, systemDriveTemp, RegistryValueKind.ExpandString);
            RegHelper.SetValue(RegistryHive.LocalMachine, SESSION_MANAGER_ENVIRONMENT, TEMP, systemDriveTemp, RegistryValueKind.ExpandString);
        }

        public static void _308(bool _)
        {
            var localAppDataTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_LOCAL_APPDATA}\\{TEMP_FOLDER}");
            var systemDriveTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_DRIVE}\\{TEMP_FOLDER}");
            var systemRootTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_ROOT}\\{TEMP_FOLDER}");
            var currentTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_TEMP}");
            var userName = Environment.UserName;

            ServiceHelper.Restart(SERVICE_SPOOLER);
            OneDriveHelper.StopProcesses();
            FileHelper.DirectoryDelete(localAppDataTemp);
            FileHelper.CreateDirectory(systemRootTemp, localAppDataTemp);

            if (FileHelper.IsSymbolicLink(currentTemp).Invert())
            {
                FileHelper.DirectoryLazyDelete(currentTemp);

                if (FileHelper.DirectoryIsEmpty(currentTemp))
                {
                    FileHelper.DirectoryDelete(currentTemp);
                }
                else
                {
                    ScheduledTaskHelper.RegisterLogonTask(name: _308_TEMPORARY_TASK, description: null, execute: POWERSHELL_EXE, args: _308_TASK_ARGS, username: userName);
                }
            }

            Environment.SetEnvironmentVariable(TMP, localAppDataTemp, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(TMP, systemRootTemp, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(TMP, localAppDataTemp, EnvironmentVariableTarget.Process);
            RegHelper.SetValue(RegistryHive.CurrentUser, ENVIRONMENT, TMP, _308_APPDATA_TEMP, RegistryValueKind.ExpandString);
            Environment.SetEnvironmentVariable(TEMP, localAppDataTemp, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(TEMP, systemRootTemp, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable(TEMP, localAppDataTemp, EnvironmentVariableTarget.Process);
            RegHelper.SetValue(RegistryHive.CurrentUser, ENVIRONMENT, TEMP, _308_APPDATA_TEMP, RegistryValueKind.ExpandString);
            RegHelper.SetValue(RegistryHive.LocalMachine, SESSION_MANAGER_ENVIRONMENT, TMP, systemRootTemp, RegistryValueKind.ExpandString);
            RegHelper.SetValue(RegistryHive.LocalMachine, SESSION_MANAGER_ENVIRONMENT, TEMP, systemRootTemp, RegistryValueKind.ExpandString);
        }

        public static void _309(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _309_CONTROL_FILE_SYSTEM_PATH,
                                                                            _309_LONG_PATH,
                                                                                IsChecked ? _309_ENABLED_VALUE
                                                                                         : _309_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _310(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _310_SYSTEM_CRASH_CONTROL_PATH,
                                                                            _310_DISPLAY_PARAMS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _312(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_DEFAULT_VALUE, RegistryValueKind.DWord);

        public static void _313(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_NEVER_VALUE, RegistryValueKind.DWord);

        public static void _314(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, _314_ENABLE_LINKED, _314_ENABLE_LINKED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, _314_ENABLE_LINKED);
        }

        public static void _315(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.Users, _315_DELIVERY_SETTINGS_PATH, _315_DOWNLOAD_MODE, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.Users, _315_DELIVERY_SETTINGS_PATH, _315_DOWNLOAD_MODE, DISABLED_VALUE, RegistryValueKind.DWord);
            FileHelper.TryDeleteDirectory(_315_DELIVERY_OPT_PATH);
        }

        public static void _316(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _316_WINLOGON_PATH, _316_FOREGROUND_POLICY, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _316_WINLOGON_PATH, _316_FOREGROUND_POLICY);
        }

        public static void _317(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _317_CURRENT_VERSION_WINDOWS_PATH,
                                                                            _317_PRINTER_LEGACY_MODE,
                                                                                IsChecked ? _317_ENABLED_VALUE
                                                                                         : _317_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _319(bool IsChecked) => DismHelper.SetFeatureState(_319_LEGACY_COMPONENTS_FEATURE, IsChecked);

        public static void _320(bool IsChecked) => DismHelper.SetFeatureState(_320_POWERSHELL_V2_FEATURE, IsChecked);

        public static void _321(bool IsChecked) => DismHelper.SetFeatureState(_321_POWERSHELL_V2_ROOT_FEATURE, IsChecked);

        public static void _322(bool IsChecked) => DismHelper.SetFeatureState(_322_XPS_SERVICES_FEATURE, IsChecked);

        public static void _323(bool IsChecked) => DismHelper.SetFeatureState(_323_WORK_FOLDERS_FEATURE, IsChecked);

        public static void _324(bool IsChecked) => DismHelper.SetFeatureState(_324_MEDIA_PLAYBACK_FEATURE, IsChecked);

        public static void _326(bool IsChecked) => DismHelper.SetCapabilityState(_326_STEPS_RECORDER_CAPABILITY, IsChecked);

        public static void _327(bool IsChecked) => DismHelper.SetCapabilityState(_327_QUICK_SUPPORT_CAPABILITY, IsChecked);

        public static void _328(bool IsChecked) => DismHelper.SetCapabilityState(_328_MS_PAINT_CAPABILITY, IsChecked);

        public static void _329(bool IsChecked) => DismHelper.SetCapabilityState(_329_MS_WORDPAD_CAPABILITY, IsChecked);

        public static void _330(bool IsChecked) => DismHelper.SetCapabilityState(_330_INTERNET_EXPLORER_CAPABILITY, IsChecked);

        public static void _331(bool IsChecked) => DismHelper.SetCapabilityState(_331_MATH_RECOGNIZER_CAPABILITY, IsChecked);

        public static void _332(bool IsChecked) => DismHelper.SetCapabilityState(_332_MEDIA_PLAYER_CAPABILITY, IsChecked);

        public static void _333(bool IsChecked) => DismHelper.SetCapabilityState(_333_OPENSSH_CLIENT_CAPABILITY, IsChecked);

        public static void _334(bool IsChecked)
        {
            var updateManager = ComObjectHelper.CreateFromProgID(_334_UPDATE_SERVICE_MANAGER);

            if (IsChecked)
            {
                updateManager.AddService2(_334_SERVICE_MANAGER_GUID, 7, "");
                return;
            }

            updateManager.RemoveService(_334_SERVICE_MANAGER_GUID);
        }

        public static void _336(bool _) => ProcessHelper.Start(POWERCFG_EXE, _336_HIGH_POWER_ARG, ProcessWindowStyle.Hidden);

        public static void _337(bool _) => ProcessHelper.Start(POWERCFG_EXE, _337_BALANCED_POWER_ARG, ProcessWindowStyle.Hidden);

        public static void _338(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _338_NET_FRAMEWORK64_PATH, _338_USE_LATEST_CLR, ENABLED_VALUE, RegistryValueKind.DWord);
                RegHelper.SetValue(RegistryHive.LocalMachine, _338_NET_FRAMEWORK32_PATH, _338_USE_LATEST_CLR, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _338_NET_FRAMEWORK64_PATH, _338_USE_LATEST_CLR);
            RegHelper.DeleteKey(RegistryHive.LocalMachine, _338_NET_FRAMEWORK32_PATH, _338_USE_LATEST_CLR);
        }

        public static void _339(bool IsChecked) => WmiHelper.SetNetworkAdaptersPowerSave(IsChecked);

        public static void _340(bool IsChecked) => PowerShell.Create().AddScript(IsChecked ? _340_ENABLE_NET_BINDING_PS : _340_DISABLE_NET_BINDING_PS).Invoke();

        public static void _342(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE);

        public static void _343(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE, INPUT_ENG_VALUE, RegistryValueKind.String);

        public static void _345(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                USER_SHELL_FOLDERS_PATH,
                                                                    IMAGES_FOLDER,
                                                                        RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, _345_DESKTOP_FOLDER),
                                                                            RegistryValueKind.ExpandString);

        public static void _346(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, IMAGES_FOLDER);

        public static void _348(bool _) => OsHelper.SetRecommendedTroubleshooting(_348_AUTOMATICALLY_VALUE);

        public static void _349(bool _) => OsHelper.SetRecommendedTroubleshooting(_349_DEFAULT_VALUE);

        public static void _350(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _350_SEPARATE_PROCESS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _351(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _351_RESERVE_MANAGER_PATH,
                                                                            _351_SHIPPED_RESERVES,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _352(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, _352_TYPELIB_PATH);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _352_TYPELIB_WIN64_PATH, string.Empty, string.Empty);
        }

        public static void _353(bool IsChecked) => RegHelper.SetValue(RegistryHive.Users,
                                                                        _353_DEFAULT_KEYBOARD_PATH,
                                                                            _353_INITIAL_INDICATORS,
                                                                                IsChecked ? _353_ENABLED_VALUE
                                                                                          : _353_DISABLED_VALUE,
                                                                                    RegistryValueKind.String);

        public static void _354(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _354_KEYBOARD_LAYOUT_PATH, _354_SCAN_CODE);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _354_KEYBOARD_LAYOUT_PATH, _354_SCAN_CODE, _354_DISABLED_VALUE, RegistryValueKind.Binary);
        }

        public static void _355(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _355_STICKY_KEYS_PATH,
                                                                            _355_FLAGS,
                                                                                IsChecked ? _355_ENABLED_VALUE
                                                                                          : _355_DISABLED_VALUE,
                                                                                    RegistryValueKind.String);

        public static void _356(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _356_AUTOPLAY_HANDLERS_PATH,
                                                                            _356_AUTOPLAY,
                                                                                IsChecked ? _356_ENABLED_VALUE
                                                                                          : _356_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _357(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _357_THUMBNAIL_CACHE_PATH,
                                                                            _357_AUTOPLAY,
                                                                                IsChecked ? _357_ENABLED_VALUE
                                                                                          : _357_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _358(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        WINLOGON_PATH,
                                                                            _358_RESTART_APPS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _359(bool IsChecked)
        {
            if (IsChecked)
            {
                FirewallHelper.SetGroupRule(profileMask: 2, enable: true, _359_FILE_PRINTER_SHARING_GROUP, _359_NETWORK_DISCOVERY_GROUP);
                _ = PowerShell.Create().AddCommand(_359_SET_PROFILE_PS).AddParameter(_359_NET_CATEGORY_PARAM, _359_PRIVATE_VALUE).Invoke();
                return;
            }

            FirewallHelper.SetGroupRule(profileMask: 2, enable: false, _359_FILE_PRINTER_SHARING_GROUP, _359_NETWORK_DISCOVERY_GROUP);
        }

        public static void _360(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        UPDATE_UX_SETTINGS_PATH,
                                                                            _360_ACTIVE_HOURS,
                                                                                IsChecked ? _360_MANUAL_STATE
                                                                                          : _360_AUTO_STATE,
                                                                                    RegistryValueKind.DWord);

        public static void _361(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        UPDATE_UX_SETTINGS_PATH,
                                                                            _361_IS_EXPEDITED,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _362(bool _)
        {
            var poperties = MsiHelper.GetProperties(Directory.GetFiles(_362_INSTALLER_PATH, _362_MSI_MASK))
                                     .First(property => property[_362_PRODUCT_NAME] == _362_PC_HEALTH_CHECK);

            ProcessHelper.StartWait(_362_MSIEXEC_EXE, $"/uninstall {poperties["Path"]} /quiet /norestart", ProcessWindowStyle.Hidden);
        }

        public static void _363(bool IsChecked)
        {
            if (IsChecked)
            {
                var installer = $"{Environment.GetEnvironmentVariable(TEMP)}\\{_363_VC_REDISTRX64}";
                WebHelper.Download(_363_DOWNLOAD_URL, installer);
                ProcessHelper.StartWait(installer, _363_VC_REDISTRX64_ARG);
                FileHelper.FileDelete(installer);
            }
        }

        public static void _365(bool _) => OneDriveHelper.Install();

        public static void _366(bool _) => OneDriveHelper.Uninstall();

        public static void _400(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _400_HIDE_ADDED_APPS);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _400_HIDE_ADDED_APPS, _400_DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _401(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _401_APP_SUGGESTIONS, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _401_APP_SUGGESTIONS, DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _402(bool IsChecked)
        {
            var bytes = File.ReadAllBytes(_402_POWERSHELL_LNK);
            bytes[0x15] = (byte)(IsChecked ? bytes[0x15] | 0x20 : bytes[0x15] ^ 0x20);
            File.WriteAllBytes(_402_POWERSHELL_LNK, bytes);
        }

        public static void _600(bool IsChecked)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser,
                                _600_GAME_DVR_PATH,
                                    _600_APP_CAPTURE,
                                        IsChecked ? ENABLED_VALUE : DISABLED_VALUE,
                                            RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser,
                                _600_GAME_CONFIG_PATH,
                                    _600_GAME_DVR,
                                        IsChecked ? ENABLED_VALUE : DISABLED_VALUE,
                                            RegistryValueKind.DWord);
        }

        public static void _601(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                    _601_GAME_BAR_PATH,
                                                        _601_SHOW_PANEL,
                                                            IsChecked ? ENABLED_VALUE : DISABLED_VALUE,
                                                                RegistryValueKind.DWord);

        public static void _602(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                    _602_GRAPHICS_DRIVERS_PATH,
                                                        _602_HWSCH_MODE,
                                                            IsChecked ? _602_ENABLED_VALUE : _602_DISABLED_VALUE,
                                                                RegistryValueKind.DWord);

        public static void _900(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_COM_PATH, string.Empty, _900_MSI_EXTRACT_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_PATH, MUIVERB, _900_MSI_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_PATH, _900_MSI_ICON, _900_MSI_ICON_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_PATH);
        }

        public static void _901(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _901_CAB_COM_PATH, string.Empty, _901_CAB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _901_CAB_RUNAS_PATH, MUIVERB, _901_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _901_CAB_RUNAS_PATH, _901_CAB_LUA_SHIELD, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _901_CAB_RUNAS_PATH);
        }

        public static void _902(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _902_RUNAS_USER_PATH, _902_EXTENDED);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _902_RUNAS_USER_PATH, _902_EXTENDED, string.Empty);
        }

        public static void _903(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _903_CAST_TO_DEV_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _903_CAST_TO_DEV_GUID, _903_CAST_TO_DEV_VALUE);
        }

        public static void _904(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _904_SHARE_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _904_SHARE_GUID, string.Empty);
        }

        public static void _906(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _906_BMP_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _906_BMP_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _907(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _907_GIF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _907_GIF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _908(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _908_JPE_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _908_JPE_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _909(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _909_JPEG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _909_JPEG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _910(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _910_JPG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _910_JPG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _911(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _911_PNG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _911_PNG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _912(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _912_TIF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _912_TIF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _913(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _913_TIFF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _913_TIFF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _914(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _914_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _914_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _915(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _915_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _915_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _916(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _916_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _916_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _917(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _917_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _917_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _917_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
            RegHelper.SetValue(RegistryHive.ClassesRoot, _917_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _918(bool IsChecked) => RegHelper.SetValue(RegistryHive.ClassesRoot, _918_LIB_LOCATION_PATH, string.Empty,
                                                                                IsChecked ? _918_SHOW_VALUE : _918_HIDE_VALUE);

        public static void _919(bool IsChecked) => RegHelper.SetValue(RegistryHive.ClassesRoot, _919_SEND_TO_PATH, string.Empty,
                                                                                IsChecked ? _919_SHOW_VALUE : _919_HIDE_VALUE);

        public static void _920(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _920_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _920_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _921(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_ITEM_NAME, _921_BMP_ITEM_VALUE, RegistryValueKind.ExpandString);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_NULL_FILE, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW);
        }

        public static void _922(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, DATA, _922_DATA_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, ITEM_NAME, _922_ITEM_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW);
        }

        public static void _923(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _923_ZIP_SHELLNEW_PATH, DATA, _823_ZIP_DATA, RegistryValueKind.Binary);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _923_ZIP_SHELLNEW_PATH, ITEM_NAME, _923_ITEM_DATA, RegistryValueKind.ExpandString);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _923_ZIP_SHELLNEW_PATH);
        }

        public static void _924(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _924_PROMPT_NAME, _924_PROMPT_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _924_PROMPT_NAME);
        }

        public static void _925(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _925_NO_USE_NAME);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _925_NO_USE_NAME, _925_NO_USE_VALUE, RegistryValueKind.DWord);
        }
    }
}