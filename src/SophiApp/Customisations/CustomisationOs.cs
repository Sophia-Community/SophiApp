using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SophiApp.Dto;
using SophiApp.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public static class CustomisationOs
    {
        public static void _100(bool IsChecked)
        {
            var diagTrack = ServiceHelper.Get(_100_DIAG_TRACK);
            var firewallRule = FirewallHelper.GetGroupRule(_100_DIAG_TRACK).First();

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
            firewallRule.Enabled = true;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

        public static void _102(bool _)
        {
            RegHelper.DeleteKey(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, ALLOW_TELEMETRY);
            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH_ALLOWED, MAX_TELEMETRY_ALLOWED, DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL, DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _103(bool _)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine,
                                DATA_COLLECTION_PATH, ALLOW_TELEMETRY,
                                    OsHelper.IsEdition(WIN_VER_ENT) || OsHelper.IsEdition(WIN_VER_EDU)
                                        ? MIN_ENT_TELEMETRY_VALUE : MIN_TELEMETRY_VALUE,
                                            RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH_ALLOWED, MAX_TELEMETRY_ALLOWED, MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL, MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _104(bool IsChecked)
        {
            var werService = ServiceHelper.Get(_104_WER_SERVICE);

            if (IsChecked)
            {
                ScheduledTaskHelper.TryChangeTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK, true);
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _104_WER_PATH, DISABLED);
                ServiceHelper.SetStartMode(werService, ServiceStartMode.Manual);

                if (werService.Status == ServiceControllerStatus.Stopped)
                {
                    werService.Start();
                }

                return;
            }

            if (OsHelper.IsEdition(_104_CORE).Invert())
            {
                ScheduledTaskHelper.TryChangeTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK, false);
                RegHelper.SetValue(RegistryHive.CurrentUser, _104_WER_PATH, DISABLED, _104_DISABLED_DEFAULT_VALUE, RegistryValueKind.DWord);
            }

            if (werService.Status == ServiceControllerStatus.Running)
            {
                werService.Stop();
            }

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
                                                                            ENABLED,
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
                RegHelper.DeleteKey(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS, ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _201(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _202(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER, false);

        public static void _204(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _204_AUTO_CHECK_SELECT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _205(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _205_HIDDEN,
                                                                                IsChecked ? _205_ENABLED_VALUE
                                                                                          : _205_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _206(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _206_HIDE_FILE_EXT,
                                                                                IsChecked ? _206_SHOW_VALUE
                                                                                          : _206_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _207(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _207_HIDE_MERGE_CONF,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _209(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                ADVANCED_EXPLORER_PATH,
                                                                    LAUNCH_TO,
                                                                        LAUNCH_PC_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _210(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                ADVANCED_EXPLORER_PATH,
                                                                    LAUNCH_TO,
                                                                        LAUNCH_QA_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _211(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _211_CORTANA_BUTTON,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _212(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _212_EXPLORER_COMPACT_MODE,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _213(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _213_SHOW_SYNC_PROVIDER,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _214(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _214_SNAP_ASSIST,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _215(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _215_SNAP_ASSIST_FLYOUT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _217(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                STATUS_MANAGER_PATH,
                                                                    ENTHUSIAST_MODE,
                                                                        DIALOG_DETAILED_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _218(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                STATUS_MANAGER_PATH,
                                                                    ENTHUSIAST_MODE,
                                                                        DIALOG_COMPACT_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _219(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _219_RIBBON_EXPLORER_PATH,
                                                                            _219_TABLET_MODE_OFF,
                                                                                IsChecked ? _219_MINIMIZED_VALUE
                                                                                          : _219_EXPANDED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _220(bool IsChecked)
        {
            var shellState = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _220_SHELL_STATE);
            shellState[4] = IsChecked ? _220_SHELL_ENABLED_VALUE : _220_SHELL_DISABLED_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _220_SHELL_STATE, shellState, RegistryValueKind.Binary);
        }

        public static void _221(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _221_3D_OBJECT_PROPERTY_PATH, _221_PC_POLICY);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _221_3D_OBJECT_PROPERTY_PATH, _221_PC_POLICY, _221_3D_OBJECT_HIDE_VALUE);
        }

        public static void _222(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_VERSION_EXPLORER_PATH,
                                                                            _222_SHOW_RECENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _223(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_VERSION_EXPLORER_PATH,
                                                                            _223_SHOW_FREQUENT,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _225(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, EXPLORER_TASKBAR_ALIGNMENT, _225_TASKBAR_ALIGNMENT_LEFT, RegistryValueKind.DWord);

        public static void _226(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, EXPLORER_TASKBAR_ALIGNMENT, _226_TASKBAR_ALIGNMENT_CENTER, RegistryValueKind.DWord);

        public static void _227(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        TASKBAR_SEARCH_PATH,
                                                                            TASKBAR_SEARCH_MODE,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _228(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            SHOW_TASKVIEW_BUTTON,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _229(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            SHOW_TASKVIEW_BUTTON,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _230(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _230_WIDGETS_IN_TASKBAR,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _231(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _231_PEOPLE_EXPLORER_PATH,
                                                                            _231_PEOPLE_BAND,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _232(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _232_SHOW_SECONDS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _234(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_HIDE_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _235(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_ICON_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _236(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                TASKBAR_SEARCH_PATH,
                                                                    TASKBAR_SEARCH_MODE,
                                                                        TASKBAR_SEARCH_BOX_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _237(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _237_PEN_WORKSPACE_PATH,
                                                                            _237_PEN_WORKSPACE_VISIBILITY,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _238(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_VERSION_EXPLORER_PATH,
                                                                            _238_AUTO_TRAY,
                                                                                IsChecked ? _238_AUTO_TRAY_SHOW_VALUE
                                                                                          : _238_AUTO_TRAY_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _239(bool IsChecked)
        {
            var settings = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, _239_STUCK_RECTS3_PATH, _239_STUCK_RECTS3_SETTINGS);
            settings[9] = IsChecked ? _239_STUCK_RECTS3_SHOW_VALUE : _239_STUCK_RECTS3_HIDE_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, _239_STUCK_RECTS3_PATH, _239_STUCK_RECTS3_SETTINGS, settings, RegistryValueKind.Binary);
        }

        public static void _240(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _240_FEEDS_POLICY_PATH, _240_ENABLE_FEEDS);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _240_FEEDS_POLICY_PATH, _240_ENABLE_FEEDS, _240_SHELL_FEEDS_ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _241(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _241_TASKBAR_TEAMS_ICON,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _243(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _244(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_SMALL_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _245(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW, ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE, STARTUP_PAGE_CATEGORY_VALUE, RegistryValueKind.DWord);
        }

        public static void _247(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME, LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _248(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME, DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _250(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME, LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _251(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME, DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _252(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _252_NO_NEW_APP_ALERT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _252_NO_NEW_APP_ALERT, _252_HIDE_ALERT_VALUE, RegistryValueKind.DWord);
        }

        public static void _253(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        WINLOGON_PATH,
                                                                            _253_FIRST_LOGON_ANIMATION,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _255(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                CONTROL_PANEL_DESKTOP_PATH,
                                                                    JPEG_QUALITY,
                                                                        _255_JPEG_MAX_QUALITY,
                                                                            RegistryValueKind.DWord);

        public static void _256(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser,
                                                                CONTROL_PANEL_DESKTOP_PATH,
                                                                    JPEG_QUALITY);

        public static void _257(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _257_WINDOWS_UPDATE_SETTINGS_PATH,
                                                                            _257_RESTART_NOTIFICATIONS,
                                                                                IsChecked ? _257_SHOW_VALUE
                                                                                          : _257_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _258(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _258_EXPLORER_NAMING_PATH, _258_SHORTCUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _258_EXPLORER_NAMING_PATH, _258_SHORTCUT, _258_DISABLE_VALUE);
        }

        public static void _259(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _259_CONTROL_PANEL_KEYBOARD_PATH,
                                                                            _259_PRINT_SCREEN_SNIPPING,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _260(bool IsChecked) => SystemParametersHelper.SetInputSettings(IsChecked);

        public static void _261(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _261_DISALLOW_WINDOWS_SHAKE,
                                                                                IsChecked ? _261_ENABLED_VALUE
                                                                                         : _261_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _263(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT, START_LAYOUT_DEFAULT_VALUE, RegistryValueKind.DWord);

        public static void _264(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT, START_LAYOUT_PINS_VALUE, RegistryValueKind.DWord);

        public static void _265(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT, START_LAYOUT_RECOMMENDATIONS_VALUE, RegistryValueKind.DWord);

        public static void _266(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _266_HIDE_ADDED_APPS);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _266_HIDE_ADDED_APPS, _266_DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _267(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _267_APP_SUGGESTIONS, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _267_APP_SUGGESTIONS, DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _268(bool IsChecked)
        {
            var bytes = File.ReadAllBytes(_268_POWERSHELL_LNK);
            bytes[0x15] = (byte)(IsChecked ? bytes[0x15] | 0x20 : bytes[0x15] ^ 0x20);
            File.WriteAllBytes(_268_POWERSHELL_LNK, bytes);
        }

        public static void _269(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, _269_FEEDS_DSB_PATH, _269_SHOW_DYNAMIC_CONTENT, ENABLED_VALUE, RegistryValueKind.DWord);
                RegHelper.SetValue(RegistryHive.CurrentUser, _269_SEARCH_SETTINGS_PATH, _269_DYNAMIC_SEARCH_BOX, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _269_FEEDS_DSB_PATH, _269_SHOW_DYNAMIC_CONTENT, DISABLED_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, _269_SEARCH_SETTINGS_PATH, _269_DYNAMIC_SEARCH_BOX, DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _270(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_LEARN_ABOUT_THIS_PICTURE, false);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_LEARN_ABOUT_THIS_PICTURE, ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _300(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            STORAGE_POLICY_01,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _302(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01, ENABLED_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, _302_STORAGE_POLICY_MONTH_VALUE, RegistryValueKind.DWord);
        }

        public static void _303(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01, ENABLED_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _304(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            _304_STORAGE_POLICY_04,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _305(bool IsChecked) => ProcessHelper.StartWait(POWERCFG_EXE, IsChecked ? _305_HIBERNATE_ON : _305_HIBERNATE_OFF, ProcessWindowStyle.Hidden);

        public static void _307(bool _)
        {
            var localAppDataTemp = Environment.ExpandEnvironmentVariables($"{LOCAL_APPDATA}\\{TEMP_FOLDER}");
            var systemDriveTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_DRIVE}\\{TEMP_FOLDER}");
            var systemRootTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_ROOT}\\{TEMP_FOLDER}");
            var currentTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_TEMP}");
            var userName = Environment.UserName;

            ServiceHelper.TryRestart(SERVICE_SPOOLER);
            OneDriveHelper.StopProcesses();
            FileHelper.CreateDirectory(systemDriveTemp);
            FileHelper.DirectoryLazyDelete(systemRootTemp);

            if (FileHelper.IsSymbolicLink(currentTemp).Invert())
            {
                FileHelper.DirectoryLazyDelete(currentTemp);

                if (FileHelper.DirectoryIsEmpty(currentTemp))
                {
                    FileHelper.TryDirectoryDelete(localAppDataTemp);
                    FileHelper.CreateDirectoryLink(localAppDataTemp, systemDriveTemp);
                }
                else
                {
                    ScheduledTaskHelper.RegisterLogonTask(name: _307_SYMBOLIC_LINK_TASK, description: null, execute: POWERSHELL_EXE, arg: _307_TASK_ARGS, username: userName);
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
            var localAppDataTemp = Environment.ExpandEnvironmentVariables($"{LOCAL_APPDATA}\\{TEMP_FOLDER}");
            var systemRootTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_ROOT}\\{TEMP_FOLDER}");
            var currentTemp = Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_TEMP}");
            var userName = Environment.UserName;

            ServiceHelper.TryRestart(SERVICE_SPOOLER);
            OneDriveHelper.StopProcesses();
            FileHelper.DirectoryLazyDelete(localAppDataTemp);
            FileHelper.CreateDirectory(systemRootTemp, localAppDataTemp);

            if (FileHelper.IsSymbolicLink(currentTemp).Invert())
            {
                FileHelper.DirectoryLazyDelete(currentTemp);

                if (Directory.Exists(currentTemp) && FileHelper.DirectoryIsEmpty(currentTemp))
                {
                    FileHelper.TryDirectoryDelete(currentTemp);
                }
                else
                {
                    ScheduledTaskHelper.RegisterLogonTask(name: _308_TEMPORARY_TASK, description: null, execute: POWERSHELL_EXE, arg: _308_TASK_ARGS, username: userName);
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

        public static void _311(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, _311_ENABLE_LINKED, _311_ENABLE_LINKED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, _311_ENABLE_LINKED);
        }

        public static void _312(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.Users, _312_DELIVERY_SETTINGS_PATH, _312_DOWNLOAD_MODE, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.Users, _312_DELIVERY_SETTINGS_PATH, _312_DOWNLOAD_MODE, DISABLED_VALUE, RegistryValueKind.DWord);
            FileHelper.TryDeleteDirectory(_312_DELIVERY_OPT_PATH);
        }

        public static void _313(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _313_WINLOGON_PATH, _313_FOREGROUND_POLICY, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _313_WINLOGON_PATH, _313_FOREGROUND_POLICY);
        }

        public static void _314(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _314_CURRENT_VERSION_WINDOWS_PATH,
                                                                            _314_PRINTER_LEGACY_MODE,
                                                                                IsChecked ? _314_ENABLED_VALUE
                                                                                          : _314_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _315(bool IsChecked)
        {
            var updateManager = ComObjectHelper.CreateFromProgID(_315_UPDATE_SERVICE_MANAGER);

            if (IsChecked)
            {
                updateManager.AddService2(_315_SERVICE_MANAGER_GUID, 7, "");
                return;
            }

            updateManager.RemoveService(_315_SERVICE_MANAGER_GUID);
        }

        public static void _317(bool _) => ProcessHelper.Start(POWERCFG_EXE, _317_HIGH_POWER_ARGS, ProcessWindowStyle.Hidden);

        public static void _318(bool _) => ProcessHelper.Start(POWERCFG_EXE, _318_BALANCED_POWER_ARGS, ProcessWindowStyle.Hidden);

        public static void _319(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _319_NET_FRAMEWORK64_PATH, _319_USE_LATEST_CLR, ENABLED_VALUE, RegistryValueKind.DWord);
                RegHelper.SetValue(RegistryHive.LocalMachine, _319_NET_FRAMEWORK32_PATH, _319_USE_LATEST_CLR, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _319_NET_FRAMEWORK64_PATH, _319_USE_LATEST_CLR);
            RegHelper.DeleteKey(RegistryHive.LocalMachine, _319_NET_FRAMEWORK32_PATH, _319_USE_LATEST_CLR);
        }

        public static void _320(bool IsChecked) => WmiHelper.SetNetworkAdaptersPowerSave(IsChecked);

        public static void _322(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE);

        public static void _323(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE, INPUT_ENG_VALUE, RegistryValueKind.String);

        public static void _325(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                USER_SHELL_FOLDERS_PATH,
                                                                    IMAGES_FOLDER,
                                                                        RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, _325_DESKTOP_FOLDER),
                                                                            RegistryValueKind.ExpandString);

        public static void _326(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, IMAGES_FOLDER);

        public static void _328(bool _) => OsHelper.SetRecommendedTroubleshooting(_328_AUTOMATICALLY_VALUE);

        public static void _329(bool _) => OsHelper.SetRecommendedTroubleshooting(_329_DEFAULT_VALUE);

        public static void _330(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _330_SEPARATE_PROCESS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _331(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _331_RESERVE_MANAGER_PATH,
                                                                            _331_SHIPPED_RESERVES,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _332(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, _332_TYPELIB_PATH);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _332_TYPELIB_WIN64_PATH, string.Empty, string.Empty);
        }

        public static void _333(bool IsChecked) => RegHelper.SetValue(RegistryHive.Users,
                                                                        _333_DEFAULT_KEYBOARD_PATH,
                                                                            _333_INITIAL_INDICATORS,
                                                                                IsChecked ? _333_ENABLED_VALUE
                                                                                          : _333_DISABLED_VALUE,
                                                                                    RegistryValueKind.String);

        public static void _334(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _334_KEYBOARD_LAYOUT_PATH, _334_SCAN_CODE);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _334_KEYBOARD_LAYOUT_PATH, _334_SCAN_CODE, _334_DISABLED_VALUE, RegistryValueKind.Binary);
        }

        public static void _335(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _335_STICKY_KEYS_PATH,
                                                                            _335_FLAGS,
                                                                                IsChecked ? _335_ENABLED_VALUE
                                                                                          : _335_DISABLED_VALUE,
                                                                                    RegistryValueKind.String);

        public static void _336(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _336_AUTOPLAY_HANDLERS_PATH,
                                                                            _336_AUTOPLAY,
                                                                                IsChecked ? _336_ENABLED_VALUE
                                                                                          : _336_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _337(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _337_THUMBNAIL_CACHE_PATH,
                                                                            _337_AUTORUN,
                                                                                IsChecked ? _337_ENABLED_VALUE
                                                                                          : _337_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _338(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        WINLOGON_PATH,
                                                                            _338_RESTART_APPS,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _339(bool IsChecked)
        {
            if (IsChecked)
            {
                FirewallHelper.SetGroupRule(profileMask: 2, enable: true, _339_FILE_PRINTER_SHARING_GROUP, _339_NETWORK_DISCOVERY_GROUP);
                _ = PowerShell.Create().AddCommand(_339_SET_PROFILE_PS).AddParameter(_339_NET_CATEGORY_PARAM, _339_PRIVATE_VALUE).Invoke();
                return;
            }

            FirewallHelper.SetGroupRule(profileMask: 2, enable: false, _339_FILE_PRINTER_SHARING_GROUP, _339_NETWORK_DISCOVERY_GROUP);
        }

        public static void _340(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        UPDATE_UX_SETTINGS_PATH,
                                                                            _340_ACTIVE_HOURS,
                                                                                IsChecked ? _340_AUTO_STATE
                                                                                          : _340_MANUAL_STATE,
                                                                                    RegistryValueKind.DWord);

        public static void _341(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        UPDATE_UX_SETTINGS_PATH,
                                                                            _341_IS_EXPEDITED,
                                                                                IsChecked ? ENABLED_VALUE
                                                                                          : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _343(bool _)
        {
            var windowsTerminalAppx = UwpHelper.GetPackage(UWP_WINDOWS_TERMINAL);
            var windowsTerminalPath = $@"SOFTWARE\Classes\PackagedCom\Package\{windowsTerminalAppx.Id.FullName}\Class";
            var subkeys = RegHelper.GetSubKeyNames(RegistryHive.LocalMachine, windowsTerminalPath);

            foreach (var subkey in subkeys)
            {
                var delegationGuid = subkey.Substring(subkey.LastIndexOf("\\") + 1);

                if (RegHelper.GetByteValue(RegistryHive.LocalMachine, subkey, TERMINAL_REGISTRY_SERVER_ID) == DISABLED_VALUE)
                {
                    RegHelper.SetValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_CONSOLE, delegationGuid, RegistryValueKind.String);
                }

                if (RegHelper.GetByteValue(RegistryHive.LocalMachine, subkey, TERMINAL_REGISTRY_SERVER_ID) == ENABLED_VALUE)
                {
                    RegHelper.SetValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_TERMINAL, delegationGuid, RegistryValueKind.String);
                }
            }
        }

        public static void _344(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_CONSOLE, DELEGATION_CONSOLE_VALUE, RegistryValueKind.String);
            RegHelper.SetValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_TERMINAL, DELEGATION_CONSOLE_VALUE, RegistryValueKind.String);
        }

        public static void _346(bool _)
        {
            var properties = MsiHelper.GetProperties(Directory.GetFiles(_346_INSTALLER_PATH, _346_MSI_MASK))
                                      .First(property => property[_346_PRODUCT_NAME] == _346_PCHC);

            ProcessHelper.StartWait(_346_MSIEXEC_EXE, $"/uninstall {properties["Path"]} /quiet /norestart", ProcessWindowStyle.Hidden);
            RegHelper.SetValue(RegistryHive.LocalMachine, _346_PCHC_PATH, _346_PCHC_PREVIOUS_UNINSTALL, ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _348(bool _) => OneDriveHelper.Install();

        public static void _349(bool _) => OneDriveHelper.Uninstall();

        public static void _351(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var installer = $"{temp}\\{VC_REDISTRX64_EXE}";
            WebHelper.Download(VC_X64_DOWNLOAD_URL, installer);
            ProcessHelper.StartWait(installer, VC_INSTALL_ARGS);
            FileHelper.TryDeleteFile(installer);
            Directory.EnumerateFileSystemEntries(temp, VC_REDISTRX64_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _352(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var registryPathRedistrLib = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, VC_REDISTR_REGISTRY_PATH).First(key => key.Contains(_352_REDISTRX64_REGISTRY_NAME_PATTERN));
            var registryGuidRedistrLib = RegHelper.GetValue(RegistryHive.ClassesRoot, registryPathRedistrLib, null);
            var localRedistrLibPath = $@"{ENVIRONMENT_PROGRAM_DATA}\{PACKAGE_CACHE_NAME}\{registryGuidRedistrLib}\{VC_REDISTRX64_EXE}";
            var localRedistrLib = FileVersionInfo.GetVersionInfo(localRedistrLibPath);

            if (localRedistrLib.ProductName.Contains(_352_VC_REDISTRX64_NAME_PATTERN))
            {
                ProcessHelper.StartWait(localRedistrLibPath, VC_UNINSTALL_ARGS);

                Directory.EnumerateFileSystemEntries(temp, VC_REDISTRX64_LOG_PATTERN)
                         .ToList()
                         .ForEach(log => FileHelper.TryDeleteFile(log));
            }
        }

        public static void _354(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var cloudNet6 = WebHelper.GetJsonResponse<MsNetDto>(@"https://builds.dotnet.microsoft.com/dotnet/release-metadata/6.0/releases.json");
            var latestRelease = cloudNet6.Releases.Where(release => release.ReleaseVersion == $"{cloudNet6.LatestRelease}").First();
            var latestRuntime = latestRelease.WindowsDesktop.Files.Where(file => file.Name == "windowsdesktop-runtime-win-x86.exe").First();
            var installer = $@"{temp}\{latestRuntime.Url.Substring(latestRuntime.Url.LastIndexOf('/') + 1)}";
            WebHelper.Download(latestRuntime.Url, installer);
            ProcessHelper.StartWait(installer, DOTNET_INSTALL_ARGS);
            FileHelper.TryDeleteFile(installer);
            Directory.EnumerateFileSystemEntries(temp, DOTNET_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _355(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            DotNetHelper.Uninstall("windowsdesktop-runtime-6.*-win-x86.exe");
            Directory.EnumerateFileSystemEntries(temp, DOTNET_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _357(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var cloudNet6 = WebHelper.GetJsonResponse<MsNetDto>(@"https://builds.dotnet.microsoft.com/dotnet/release-metadata/6.0/releases.json");
            var latestRelease = cloudNet6.Releases.Where(release => release.ReleaseVersion == $"{cloudNet6.LatestRelease}").First();
            var latestRuntime = latestRelease.WindowsDesktop.Files.Where(file => file.Name == "windowsdesktop-runtime-win-x64.exe").First();
            var installer = $@"{temp}\{latestRuntime.Url.Substring(latestRuntime.Url.LastIndexOf('/') + 1)}";
            WebHelper.Download(latestRuntime.Url, installer);
            ProcessHelper.StartWait(installer, DOTNET_INSTALL_ARGS);
            FileHelper.TryDeleteFile(installer);
            Directory.EnumerateFileSystemEntries(temp, DOTNET_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _358(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            DotNetHelper.Uninstall("windowsdesktop-runtime-6*-win-x64.exe");
            Directory.EnumerateFileSystemEntries(temp, DOTNET_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _359(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, _359_INTERNET_SETTINGS_PATH, _359_AUTO_CONFIG_URL, _359_ANTIZAPRET_PROXY_LINK, RegistryValueKind.String);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, _359_INTERNET_SETTINGS_PATH, _359_AUTO_CONFIG_URL);
        }

        public static void _361(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var installer = $"{temp}\\{VC_REDISTRX86_EXE}";
            WebHelper.Download(VC_X86_DOWNLOAD_URL, installer);
            ProcessHelper.StartWait(installer, VC_INSTALL_ARGS);
            FileHelper.TryDeleteFile(installer);
            Directory.EnumerateFileSystemEntries(temp, VC_REDISTRX86_LOG_PATTERN)
                     .ToList()
                     .ForEach(log => FileHelper.TryDeleteFile(log));
        }

        public static void _362(bool _)
        {
            var temp = Environment.GetEnvironmentVariable(TEMP);
            var registryPathRedistrLib = RegHelper.GetSubKeyNames(RegistryHive.ClassesRoot, VC_REDISTR_REGISTRY_PATH).First(key => key.Contains(_362_REDISTRX86_REGISTRY_NAME_PATTERN));
            var registryGuidRedistrLib = RegHelper.GetValue(RegistryHive.ClassesRoot, registryPathRedistrLib, null);
            var localRedistrLibPath = $@"{ENVIRONMENT_PROGRAM_DATA}\{PACKAGE_CACHE_NAME}\{registryGuidRedistrLib}\{VC_REDISTRX86_EXE}";
            var localRedistrLib = FileVersionInfo.GetVersionInfo(localRedistrLibPath);

            if (localRedistrLib.ProductName.Contains(_362_REDISTRX86_REGISTRY_NAME_PATTERN))
            {
                ProcessHelper.StartWait(localRedistrLibPath, VC_UNINSTALL_ARGS);

                Directory.EnumerateFileSystemEntries(temp, VC_REDISTRX86_LOG_PATTERN)
                         .ToList()
                         .ForEach(log => FileHelper.TryDeleteFile(log));
            }
        }

        public static void _500(bool IsChecked)
        {
            if (IsChecked)
            {
                var hevcvAppx = $@"{RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, USER_DOWNLOAD_FOLDER)}\{_500_HEVC_APPX_NAME}";
                WebHelper.Download(_500_HEVC_APPX_LINK, hevcvAppx, true);
                UwpHelper.InstallPackage(hevcvAppx);
                FileHelper.TryDeleteFile(hevcvAppx);
                return;
            }

            var hevcPackage = UwpHelper.GetPackage(_500_UWP_HEVC_VIDEO);
            UwpHelper.RemovePackage(packageFullName: hevcPackage.Id.FullName, allUsers: false);
        }

        public static void _501(bool IsChecked) => RegHelper.SetValue(RegistryHive.ClassesRoot,
                                                                        _501_CORTANA_STARTUP_PATH,
                                                                            _501_CORTANA_STATE,
                                                                                IsChecked ? _501_ENABLED_VALUE
                                                                                          : _501_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _502(bool IsChecked) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _502_TEAMS_STARTUP_PATH,
                                                                            STATE,
                                                                                IsChecked ? _502_TEAMS_ENABLED_VALUE
                                                                                          : _502_TEAMS_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _504(bool _) => ProcessHelper.Start(processName: WSRESET_EXE, args: _504_MS_STORE_RESET_ARG);

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

        public static void _700(bool IsChecked)
        {
            var appCleanupTask = $@"{SOPHIA_SCHEDULED_PATH}\{_700_SOPHIA_CLEANUP_TASK}";
            var appNotificationTask = $@"{SOPHIA_SCHEDULED_PATH}\{_700_SOPHIA_CLEANUP_NOTIFICATION_TASK}";

            var volumeCachesKeys = RegHelper.GetSubKeyNames(RegistryHive.LocalMachine, _700_VOLUME_CACHES_PATH);
            RegHelper.TryDeleteKey(RegistryHive.LocalMachine, volumeCachesKeys, _700_STATE_FLAGS_1337);
            ScheduledTaskHelper.TryDeleteFolder(SOPHIA_SCRIPT_SCHEDULED_FOLDER, SOPHIAPP_SCHEDULED_FOLDER);

            if (IsChecked)
            {
                var cleanupTaskDescription = Application.Current.FindResource("Localization.CleanupTask.Description") as string;

                var notificationTaskDescription = Application.Current.FindResource("Localization.CleanupTask.NotificationTask.Description") as string;
                var notificationTaskTrigger = new DailyTrigger(daysInterval: _700_30_DAYS_INTERVAL) { StartBoundary = _21_PM_TASK_START };
                var notificationTaskArg = TextHelper.LocalizeCleanupTaskToast(_700_CLEANUP_TOAST_TASK_ARGS);

                RegHelper.SetValue(RegistryHive.LocalMachine, _700_VOLUME_CACHES_PATH, _700_VOLUME_CACHES_NAMES, _700_STATE_FLAGS_1337, _700_STATE_FLAGS_1337_VALUE, RegistryValueKind.DWord);

                ScheduledTaskHelper.RegisterTask(taskName: appCleanupTask, taskDescription: cleanupTaskDescription, execute: POWERSHELL_EXE,
                                                    arg: _700_CLEANUP_TASK_ARGS, userName: Environment.UserName, runLevel: TaskRunLevel.Highest);

                // Persist the Settings notifications to prevent to immediately disappear from Action Center
                RegHelper.SetValue(RegistryHive.CurrentUser, ACTION_CENTER_APPX_PATH, SHOW_IN_ACTION_CENTER, ENABLED_VALUE, RegistryValueKind.DWord);
                // Register the "WindowsCleanup" protocol to be able to run the scheduled task by clicking the "Run" button in a toast
                RegHelper.SetValue(RegistryHive.ClassesRoot, _700_WINDOWS_CLEANUP, string.Empty, _700_WINDOWS_CLEANUP_URL, RegistryValueKind.String);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _700_WINDOWS_CLEANUP, _700_URL_PROTOCOL, string.Empty, RegistryValueKind.String);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _700_WINDOWS_CLEANUP, _700_EDIT_FLAGS, _700_EDIT_FLAGS_VALUE, RegistryValueKind.DWord);
                //# Start the "Windows Cleanup" task if the "Run" button clicked
                RegHelper.SetValue(RegistryHive.ClassesRoot, _700_WINDOWS_CLEANUP_OPEN_PATH, string.Empty, _700_WINDOWS_CLEANUP_COMMAND, RegistryValueKind.String);

                ScheduledTaskHelper.RegisterTask(taskName: appNotificationTask, taskDescription: notificationTaskDescription, execute: POWERSHELL_EXE,
                                                    arg: notificationTaskArg, userName: Environment.UserName, runLevel: TaskRunLevel.Highest, trigger: notificationTaskTrigger);

                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, ACTION_CENTER_APPX_PATH, SHOW_IN_ACTION_CENTER);
            ScheduledTaskHelper.TryDeleteTask(appCleanupTask, appNotificationTask);
            ScheduledTaskHelper.TryDeleteFolder(SOPHIA_SCHEDULED_PATH);
            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _700_WINDOWS_CLEANUP);
        }

        public static void _701(bool ISChecked)
        {
            var appSoftwareDistributionTask = $@"{SOPHIA_SCHEDULED_PATH}\{_701_SOPHIA_SOFTWARE_DISTRIBUTION_TASK}";
            var scriptSoftwareDistributionTask = $@"{SOPHIA_SCHEDULED_PATH}\{_701_SOPHIA_SOFTWARE_DISTRIBUTION_TASK}";

            if (ISChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, ACTION_CENTER_APPX_PATH, SHOW_IN_ACTION_CENTER, ENABLED_VALUE, RegistryValueKind.DWord);
                var softwareDistributionTaskDescription = Application.Current.FindResource("Localization.SoftwareDistributionTask.Description") as string;
                var softwareDistributionTaskTrigger = new DailyTrigger(daysInterval: _701_90_DAYS_INTERVAL) { StartBoundary = _21_PM_TASK_START };
                var softwareDistributionTaskArg = TextHelper.LocalizeSoftwareDistributionTaskToast(_701_SOFTWARE_DISTRIBUTION_TASK_ARGS);
                ScheduledTaskHelper.RegisterTask(taskName: appSoftwareDistributionTask, taskDescription: softwareDistributionTaskDescription, execute: POWERSHELL_EXE,
                                                    arg: softwareDistributionTaskArg, userName: Environment.UserName, runLevel: TaskRunLevel.Highest, trigger: softwareDistributionTaskTrigger);

                return;
            }

            ScheduledTaskHelper.TryDeleteTask(appSoftwareDistributionTask, scriptSoftwareDistributionTask);
            ScheduledTaskHelper.TryDeleteFolder(SOPHIA_SCHEDULED_PATH);
        }

        public static void _702(bool IsChecked)
        {
            var appClearTempTask = $@"{SOPHIA_SCHEDULED_PATH}\{_702_SOPHIA_CLEAR_TEMP_TASK}";

            if (IsChecked)
            {
                var clearTempTaskDescription = Application.Current.FindResource("Localization.ClearTempTask.Description") as string;
                var clearTempTaskTrigger = new DailyTrigger(daysInterval: _702_60_DAYS_INTERVAL) { StartBoundary = _21_PM_TASK_START };
                var clearTempTaskArg = TextHelper.LocalizeClearTempTaskToast(_702_CLEAR_TEMP_ARGS);
                ScheduledTaskHelper.RegisterTask(taskName: appClearTempTask, taskDescription: clearTempTaskDescription, execute: POWERSHELL_EXE,
                                                    arg: clearTempTaskArg, userName: Environment.UserName, runLevel: TaskRunLevel.Highest, trigger: clearTempTaskTrigger);

                return;
            }

            ScheduledTaskHelper.TryDeleteTask(appClearTempTask);
            ScheduledTaskHelper.TryDeleteFolder(SOPHIA_SCHEDULED_PATH);
        }

        public static void _800(bool IsChecked) => PowerShellHelper.InvokeScript($"{_800_SET_NETWORK_PROTECTION_PS} {(IsChecked ? ENABLED : DISABLED)}");

        public static void _801(bool IsChecked) => PowerShellHelper.InvokeScript($"{_801_SET_PUA_PROTECTION_PS} {(IsChecked ? ENABLED : DISABLED)}");

        public static void _802(bool IsChecked) => ProcessHelper.StartWait(processName: _802_SETX_APP, args: $"{_802_DEFENDER_USE_SANDBOX_ARGS} {(IsChecked ? ENABLED_VALUE : DISABLED_VALUE)}", ProcessWindowStyle.Hidden);

        public static void _803(bool IsChecked) => ProcessHelper.StartWait(processName: AUDITPOL_APP, args: IsChecked ? PROCESS_AUDIT_ENABLED_ARGS : PROCESS_AUDIT_DISABLED_ARGS, ProcessWindowStyle.Hidden);

        public static void _804(bool IsChecked)
        {
            if (IsChecked)
            {
                ProcessHelper.StartWait(AUDITPOL_APP, PROCESS_AUDIT_ENABLED_ARGS, ProcessWindowStyle.Hidden);
                RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_AUDIT_PATH, PROCESS_CREATION_ENABLED, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICY_AUDIT_PATH, PROCESS_CREATION_ENABLED);
        }

        public static void _805(bool IsChecked)
        {
            var processCreationXml = $@"{_805_EVENT_VIEWS_PATH}\{_805_PROCESS_CREATION_XML}";

            if (IsChecked)
            {
                var eventViewerXml = TextHelper.LocalizeEventViewerCustomXml(_805_PROCESS_CREATION_XML_DATA);
                ProcessHelper.StartWait(AUDITPOL_APP, PROCESS_AUDIT_ENABLED_ARGS, ProcessWindowStyle.Hidden);
                RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_AUDIT_PATH, PROCESS_CREATION_ENABLED, ENABLED_VALUE, RegistryValueKind.DWord);
                FileHelper.WriteAllText(processCreationXml, eventViewerXml);
                return;
            }

            FileHelper.TryDeleteFile(processCreationXml);
        }

        public static void _806(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_LOGGING_PATH, _806_ENABLE_MODULE_LOGGING, ENABLED_VALUE, RegistryValueKind.DWord);
                RegHelper.SetValue(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_NAMES_PATH, _806_ALL_MODULE, _806_ALL_MODULE, RegistryValueKind.String);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_LOGGING_PATH, _806_ENABLE_MODULE_LOGGING);
            RegHelper.DeleteKey(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_NAMES_PATH, _806_ALL_MODULE);
        }

        public static void _807(bool ISChecked)
        {
            if (ISChecked)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _807_POWERSHELL_SCRIPT_BLOCK_LOGGING_PATH, _807_ENABLE_SCRIPT_BLOCK_LOGGING, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _807_POWERSHELL_SCRIPT_BLOCK_LOGGING_PATH, _807_ENABLE_SCRIPT_BLOCK_LOGGING);
        }

        public static void _808(bool IsChecked) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        CURRENT_VERSION_EXPLORER_PATH,
                                                                            _808_SMART_SCREEN_ENABLED,
                                                                                IsChecked ? _808_SMART_SCREEN_ENABLED_VALUE : _808_SMART_SCREEN_DISABLED_VALUE,
                                                                                    RegistryValueKind.String);

        public static void _809(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _809_CURRENT_POLICIES_ATTACHMENTS_PATH, _809_SAFE_ZONE_INFO);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _809_CURRENT_POLICIES_ATTACHMENTS_PATH, _809_SAFE_ZONE_INFO, _809_DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _810(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _810_WSH_SETTINGS_PATH, ENABLED);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _810_WSH_SETTINGS_PATH, ENABLED, DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _811(bool IsChecked) => DismHelper.SetFeatureState(_811_WINDOWS_SANDBOX_FEATURE, IsChecked);

        public static void _813(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_DEFAULT_VALUE, RegistryValueKind.DWord);

        public static void _814(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_NEVER_VALUE, RegistryValueKind.DWord);

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
                RegHelper.SetValue(RegistryHive.ClassesRoot, _904_CONTEXT_MENU_MODERN_SHARE_PATH, string.Empty, CONTEXT_MENU_SHARE_GUID, RegistryValueKind.String);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _904_CONTEXT_MENU_MODERN_SHARE_PATH);
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
                RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _924_PROMPT_NAME, _924_PROMPT_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _924_PROMPT_NAME);
        }

        public static void _925(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _925_NO_USE_NAME);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _925_NO_USE_NAME, _925_NO_USE_VALUE, RegistryValueKind.DWord);
        }

        public static void _926(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICY_BLOCKED_PATH, WIN_TERMINAL_ID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICY_BLOCKED_PATH, WIN_TERMINAL_ID, _926_WINDOWS_TERMINAL, RegistryValueKind.String);
        }

        public static void _927(bool IsChecked)
        {
            JObject config = JsonConvert.DeserializeObject(File.ReadAllText(TERMINAL_SETTINGS_JSON_PATH)) as JObject;
            JToken elevate = config.SelectToken("profiles.defaults.elevate");

            if (elevate == null)
            {
                var defaults = config.SelectToken("profiles.defaults") as JObject;
                defaults.Add(new JProperty("elevate", string.Empty));
                elevate = config.SelectToken("profiles.defaults.elevate");
            }

            elevate.Replace(IsChecked);
            File.WriteAllText(TERMINAL_SETTINGS_JSON_PATH, config.ToString());
        }

        public static void _928(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, _928_WIN10_CONTEXT_MENU_PATH, null, string.Empty, RegistryValueKind.String);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, _928_WIN10_CONTEXT_MENU_PATH);
        }

        public static void _929(bool IsChecked)
        {
            if (IsChecked)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, CONTEXT_MENU_SHARE_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, CONTEXT_MENU_SHARE_GUID, string.Empty, RegistryValueKind.String);
        }
    }
}