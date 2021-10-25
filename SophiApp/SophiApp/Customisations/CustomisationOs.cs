using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public static class CustomisationOs
    {
        public static void _100(bool IsActive)
        {
            var diagTrack = ServiceHelper.Get(_100_DIAG_TRACK);
            var firewallRule = FirewallHelper.GetGroupRule(_100_DIAG_TRACK).FirstOrDefault();

            if (IsActive)
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

        public static void _104(bool IsActive)
        {
            var werService = ServiceHelper.Get(_104_WER_SERVICE);

            if (IsActive)
            {
                ScheduledTaskHelper.EnableTask(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK);
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _104_WER_PATH, _104_DISABLED);
                ServiceHelper.SetStartMode(werService, ServiceStartMode.Manual);
                werService.Start();
                return;
            }

            if (OsHelper.IsEdition(_104_CORE).Invert())
            {
                ScheduledTaskHelper.DisableTask(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK);
                RegHelper.SetValue(RegistryHive.CurrentUser, _104_WER_PATH, _104_DISABLED, _104_DISABLED_DEFAULT_VALUE, RegistryValueKind.DWord);
            }

            werService.Stop();
            ServiceHelper.SetStartMode(werService, ServiceStartMode.Disabled);
        }

        public static void _106(bool _) => RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, SIUF_PATH);

        public static void _107(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, SIUF_PATH, SIUF_PERIOD, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _109(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(_109_DATA_UPDATER_TASK_PATH, _109_DATA_UPDATER_TASK, IsActive);

        public static void _110(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(_110_PROXY_TASK_PATH, _110_PROXY_TASK, IsActive);

        public static void _111(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(CEIP_TASK_PATH, _111_CONS_TASK, IsActive);

        public static void _112(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(CEIP_TASK_PATH, _112_USB_CEIP_TASK, IsActive);

        public static void _113(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(_113_DISK_DATA_TASK_PATH, _113_DISK_DATA_TASK, IsActive);

        public static void _114(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(MAPS_TASK_PATH, _114_MAPS_TOAST_TASK, IsActive);

        public static void _115(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(MAPS_TASK_PATH, _115_MAPS_UPDATE, IsActive);

        public static void _116(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(_116_FAMILY_MONITOR_TASK_PATH, _116_FAMILY_MONITOR_TASK, IsActive);

        public static void _117(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(_117_XBOX_SAVE_TASK_PATH, _117_XBOX_SAVE_TASK, IsActive);

        public static void _118(bool IsActive)
        {
            var userArso = $"{_118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}";

            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, userArso, _118_OPT_OUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, userArso, _118_OPT_OUT, _118_OPT_OUT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _119(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _119_USER_PROFILE_PATH, _119_HTTP_ACCEPT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _119_USER_PROFILE_PATH, _119_HTTP_ACCEPT, _119_HTTP_ACCEPT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _120(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _120_ADVERT_INFO_PATH,
                                                                            _120_ADVERT_ENABLED,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _121(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _121_SUB_CONTENT,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _122(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _122_SUB_CONTENT,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _123(bool IsActive)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_93,
                                IsActive ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_94,
                                IsActive ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_96,
                                IsActive ? ENABLED_VALUE : DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _124(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CONTENT_DELIVERY_MANAGER_PATH,
                                                                            _124_SILENT_APP_INSTALL,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _125(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _125_PROFILE_ENGAGE_PATH,
                                                                            _125_SETTING_ENABLED,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _126(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _126_PRIVACY_PATH,
                                                                            _126_TAILORED_DATA,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _127(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, POLICIES_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, POLICIES_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS, ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _201(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _202(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER);

        public static void _203(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _203_AUTO_CHECK_SELECT,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _204(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _204_HIDDEN,
                                                                                IsActive ? _204_ENABLED_VALUE
                                                                                         : _204_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _205(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _205_HIDE_FILE_EXT,
                                                                                IsActive ? _205_SHOW_VALUE
                                                                                         : _205_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _206(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _206_HIDE_MERGE_CONF,
                                                                                IsActive ? ENABLED_VALUE
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

        public static void _210(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _210_CORTANA_BUTTON,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _211(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _211_SHOW_SYNC_PROVIDER,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _212(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _212_SNAP_ASSIST,
                                                                                IsActive ? ENABLED_VALUE
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

        public static void _216(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _216_RIBBON_EXPLORER_PATH,
                                                                            _216_TABLET_MODE_OFF,
                                                                                IsActive ? _216_MINIMIZED_VALUE
                                                                                         : _216_EXPANDED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _217(bool IsActive)
        {
            var shellState = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE);
            shellState[4] = IsActive ? _217_SHELL_ENABLED_VALUE : _217_SHELL_DISABLED_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE, shellState, RegistryValueKind.Binary);
        }

        public static void _218(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY, _218_3D_OBJECT_HIDE_VALUE);
        }

        public static void _219(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _219_SHOW_RECENT,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _220(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _220_SHOW_FREQUENT,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _221(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _221_SHOW_TASK_VIEW,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _222(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _222_PEOPLE_EXPLORER_PATH,
                                                                            _222_PEOPLE_BAND,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _223(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _223_SHOW_SECONDS,
                                                                                IsActive ? ENABLED_VALUE
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

        public static void _228(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _228_PEN_WORKSPACE_PATH,
                                                                            _228_PEN_WORKSPACE_VISIBILITY,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _229(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        CURRENT_EXPLORER_PATH,
                                                                            _229_AUTO_TRAY,
                                                                                IsActive ? _229_AUTO_TRAY_SHOW_VALUE
                                                                                         : _229_AUTO_TRAY_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _230(bool IsActive)
        {
            var settings = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS);
            settings[9] = IsActive ? _230_STUCK_RECTS3_SHOW_VALUE : _230_STUCK_RECTS3_HIDE_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS, settings, RegistryValueKind.Binary);
        }

        public static void _231(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _213_FEEDS_PATH,
                                                                            _231_SHELL_FEEDS,
                                                                                IsActive ? _231_SHELL_FEEDS_ENABLED_VALUE
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

        public static void _242(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _242_NO_NEW_APP_ALERT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _242_NO_NEW_APP_ALERT, _242_HIDE_ALERT_VALUE, RegistryValueKind.DWord);
        }

        public static void _243(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _243_WINLOGON_PATH,
                                                                            _243_FIRST_LOGON_ANIMATION,
                                                                                IsActive ? ENABLED_VALUE
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

        public static void _247(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _247_WINDOWS_UPDATE_SETTINGS_PATH,
                                                                            _247_RESTART_NOTIFICATIONS,
                                                                                IsActive ? _247_SHOW_VALUE
                                                                                         : _247_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _248(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, _248_EXPLORER_NAMING_PATH, _248_SHORTCUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, _248_EXPLORER_NAMING_PATH, _248_SHORTCUT, _248_DISABLE_VALUE);
        }

        public static void _249(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _249_CONTROL_PANEL_KEYBOARD_PATH,
                                                                            _249_PRINT_SCREEN_SNIPPING,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _250(bool IsActive) => SystemParametersHelper.SetInputSettings(IsActive);

        public static void _251(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        ADVANCED_EXPLORER_PATH,
                                                                            _251_DISALLOW_WINDOWS_SHAKE,
                                                                                IsActive ? _251_ENABLED_VALUE
                                                                                         : _251_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _300(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            STORAGE_POLICY_01,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _302(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, _302_STORAGE_POLICY_MONTH_VALUE, RegistryValueKind.DWord);

        public static void _303(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048, DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _304(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        STORAGE_POLICY_PATH,
                                                                            _304_STORAGE_POLICY_04,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _305(bool IsActive) => ProcessHelper.Start(_305_POWERCFG, IsActive ? _305_HIBERNATE_ON : _305_HIBERNATE_OFF, ProcessWindowStyle.Hidden);

        public static void _309(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _309_CONTROL_FILE_SYSTEM_PATH,
                                                                            _309_LONG_PATH,
                                                                                IsActive ? _309_ENABLED_VALUE
                                                                                         : _309_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _310(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        _310_SYSTEM_CRASH_CONTROL_PATH,
                                                                            _310_DISPLAY_PARAMS,
                                                                                IsActive ? ENABLED_VALUE
                                                                                         : DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _312(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_DEFAULT_VALUE, RegistryValueKind.DWord);

        public static void _313(bool _) => RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, ADMIN_PROMPT, ADMIN_PROMPT_NEVER_VALUE, RegistryValueKind.DWord);

        public static void _314(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, _314_ENABLE_LINKED, _314_ENABLE_LINKED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, _314_ENABLE_LINKED);
        }

        public static void _315(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.Users, _315_DELIVERY_SETTINGS_PATH, _315_DOWNLOAD_MODE, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.SetValue(RegistryHive.Users, _315_DELIVERY_SETTINGS_PATH, _315_DOWNLOAD_MODE, DISABLED_VALUE, RegistryValueKind.DWord);
            FileHelper.TryDeleteDirectory(_315_DELIVERY_OPT_PATH);
        }

        public static void _316(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.LocalMachine, _316_WINLOGON_PATH, _316_FOREGROUND_POLICY, ENABLED_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.LocalMachine, _316_WINLOGON_PATH, _316_FOREGROUND_POLICY);
        }

        public static void _317(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        _317_CURRENT_VERSION_WINDOWS_PATH,
                                                                            _317_PRINTER_LEGACY_MODE,
                                                                                IsActive ? _317_ENABLED_VALUE
                                                                                         : _317_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _319(bool IsActive) => DismHelper.EnableFeature(_319_LEGACY_COMPONENTS_FEATURE, IsActive);

        public static void _320(bool IsActive) => DismHelper.EnableFeature(_320_POWERSHELL_V2_FEATURE, IsActive);

        public static void _321(bool IsActive) => DismHelper.EnableFeature(_321_POWERSHELL_V2_ROOT_FEATURE, IsActive);

        public static void _322(bool IsActive) => DismHelper.EnableFeature(_322_XPS_SERVICES_FEATURE, IsActive);

        public static void _323(bool IsActive) => DismHelper.EnableFeature(_323_WORK_FOLDERS_FEATURE, IsActive);

        public static void _324(bool IsActive) => DismHelper.EnableFeature(_324_MEDIA_PLAYBACK_FEATURE, IsActive);

        public static void _800(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _800_MSI_EXTRACT_COM_PATH, string.Empty, _800_MSI_EXTRACT_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _800_MSI_EXTRACT_PATH, MUIVERB, _800_MSI_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _800_MSI_EXTRACT_PATH, _800_MSI_ICON, _800_MSI_ICON_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _800_MSI_EXTRACT_PATH);
        }

        public static void _801(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _801_CAB_COM_PATH, string.Empty, _801_CAB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _801_CAB_RUNAS_PATH, MUIVERB, _801_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _801_CAB_RUNAS_PATH, _801_CAB_LUA_SHIELD, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _801_CAB_RUNAS_PATH);
        }

        public static void _802(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _802_RUNAS_USER_PATH, _802_EXTENDED);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _802_RUNAS_USER_PATH, _802_EXTENDED, string.Empty);
        }

        public static void _803(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _803_CAST_TO_DEV_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _803_CAST_TO_DEV_GUID, _803_CAST_TO_DEV_VALUE);
        }

        public static void _804(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _804_SHARE_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _804_SHARE_GUID, string.Empty);
        }

        public static void _806(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _806_BMP_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _806_BMP_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _807(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _807_GIF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _807_GIF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _808(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _808_JPE_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _808_JPE_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _809(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _809_JPEG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _809_JPEG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _810(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _810_JPG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _810_JPG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _811(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _811_PNG_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _811_PNG_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _812(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _812_TIF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _812_TIF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _813(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _813_TIFF_EXT, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _813_TIFF_EXT, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _814(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _814_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _814_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _815(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _815_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _815_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _816(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _816_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _816_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _817(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _817_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _817_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _817_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
            RegHelper.SetValue(RegistryHive.ClassesRoot, _817_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _818(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, _818_LIB_LOCATION_PATH, string.Empty,
                                                                                IsActive ? _818_SHOW_VALUE : _818_HIDE_VALUE);

        public static void _819(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, _819_SEND_TO_PATH, string.Empty,
                                                                                IsActive ? _819_SHOW_VALUE : _819_HIDE_VALUE);

        public static void _820(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, _820_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, _820_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _821(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_ITEM_NAME, _821_BMP_ITEM_VALUE, RegistryValueKind.ExpandString);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_NULL_FILE, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW);
        }

        public static void _822(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, DATA, _822_DATA_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, ITEM_NAME, _822_ITEM_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW);
        }

        public static void _823(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, _823_ZIP_SHELLNEW_PATH, DATA, _823_ZIP_DATA, RegistryValueKind.Binary);
                RegHelper.SetValue(RegistryHive.ClassesRoot, _823_ZIP_SHELLNEW_PATH, ITEM_NAME, _823_ITEM_DATA, RegistryValueKind.ExpandString);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, _823_ZIP_SHELLNEW_PATH);
        }

        public static void _824(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _824_PROMPT_NAME, _824_PROMPT_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _824_PROMPT_NAME);
        }

        public static void _825(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _825_NO_USE_NAME);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _825_NO_USE_NAME, _825_NO_USE_VALUE, RegistryValueKind.DWord);
        }
    }
}