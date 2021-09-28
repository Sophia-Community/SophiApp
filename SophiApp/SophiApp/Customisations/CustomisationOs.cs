using Microsoft.Win32;
using SophiApp.Helpers;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Const = SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public sealed class CustomisationOs
    {
        public static void _100(bool IsActive)
        {
            var diagTrack = ServiceHelper.GetService(Const._100_DIAG_TRACK);
            var firewallRule = FirewallHelper.GetGroupRule(Const._100_DIAG_TRACK).FirstOrDefault();

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
            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _103(bool _)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine,
                                Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY,
                                    OsHelper.IsEdition(Const.WIN_VER_ENT) || OsHelper.IsEdition(Const.WIN_VER_EDU)
                                        ? Const.MIN_ENT_TELEMETRY_VALUE : Const.MIN_TELEMETRY_VALUE,
                                            RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED, Const.MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL, Const.MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _104(bool IsActive)
        {
            var werService = ServiceHelper.GetService(Const._104_WER_SERVICE);

            if (IsActive)
            {
                ScheduledTaskHelper.EnableTask(Const._104_QUEUE_TASK_PATH, Const._104_QUEUE_TASK);
                RegHelper.DeleteKey(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED);
                ServiceHelper.SetStartMode(werService, ServiceStartMode.Manual);
                werService.Start();
                return;
            }

            if (OsHelper.IsEdition(Const._104_CORE).Invert())
            {
                ScheduledTaskHelper.DisableTask(Const._104_QUEUE_TASK_PATH, Const._104_QUEUE_TASK);
                RegHelper.SetValue(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED, Const._104_DISABLED_DEFAULT_VALUE, RegistryValueKind.DWord);
            }

            werService.Stop();
            ServiceHelper.SetStartMode(werService, ServiceStartMode.Disabled);
        }

        public static void _106(bool _) => RegHelper.DeleteSubKeyTree(RegistryHive.CurrentUser, Const.SIUF_PATH);

        public static void _107(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.SIUF_PATH, Const.SIUF_PERIOD, Const.DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _109(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const._109_DATA_UPDATER_TASK_PATH, Const._109_DATA_UPDATER_TASK, IsActive);

        public static void _110(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const._110_PROXY_TASK_PATH, Const._110_PROXY_TASK, IsActive);

        public static void _111(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const.CEIP_TASK_PATH, Const._111_CONS_TASK, IsActive);

        public static void _112(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const.CEIP_TASK_PATH, Const._112_USB_CEIP_TASK, IsActive);

        public static void _113(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const._113_DISK_DATA_TASK_PATH, Const._113_DISK_DATA_TASK, IsActive);

        public static void _114(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const.MAPS_TASK_PATH, Const._114_MAPS_TOAST_TASK, IsActive);

        public static void _115(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const.MAPS_TASK_PATH, Const._115_MAPS_UPDATE, IsActive);

        public static void _116(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const._116_FAMILY_MONITOR_TASK_PATH, Const._116_FAMILY_MONITOR_TASK, IsActive);

        public static void _117(bool IsActive) => ScheduledTaskHelper.ChangeTaskState(Const._117_XBOX_SAVE_TASK_PATH, Const._117_XBOX_SAVE_TASK, IsActive);

        public static void _118(bool IsActive)
        {
            var userArso = $"{Const._118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}";

            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, userArso, Const._118_OPT_OUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, userArso, Const._118_OPT_OUT, Const._118_OPT_OUT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _119(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, Const._119_USER_PROFILE_PATH, Const._119_HTTP_ACCEPT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, Const._119_USER_PROFILE_PATH, Const._119_HTTP_ACCEPT, Const._119_HTTP_ACCEPT_DEFAULT_VALUE, RegistryValueKind.DWord);
        }

        public static void _120(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._120_ADVERT_INFO_PATH,
                                                                            Const._120_ADVERT_ENABLED,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _121(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CONTENT_DELIVERY_MANAGER_PATH,
                                                                            Const._121_SUB_CONTENT,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _122(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CONTENT_DELIVERY_MANAGER_PATH,
                                                                            Const._122_SUB_CONTENT,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _123(bool IsActive)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_93,
                                IsActive ? Const.ENABLED_VALUE : Const.DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_94,
                                IsActive ? Const.ENABLED_VALUE : Const.DISABLED_VALUE, RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_96,
                                IsActive ? Const.ENABLED_VALUE : Const.DISABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _124(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CONTENT_DELIVERY_MANAGER_PATH,
                                                                            Const._124_SILENT_APP_INSTALL,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _125(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._125_PROFILE_ENGAGE_PATH,
                                                                            Const._125_SETTING_ENABLED,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _126(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._126_PRIVACY_PATH,
                                                                            Const._126_TAILORED_DATA,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _127(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, Const.POLICIES_EXPLORER_PATH, Const._127_DISABLE_SEARCH_SUGGESTIONS);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, Const.POLICIES_EXPLORER_PATH, Const._127_DISABLE_SEARCH_SUGGESTIONS, Const.ENABLED_VALUE, RegistryValueKind.DWord);
        }

        public static void _201(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.START_PANEL_EXPLORER_PATH, Const.DESKTOP_ICON_THIS_COMPUTER, Const.DISABLED_VALUE, RegistryValueKind.DWord);

        public static void _202(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser, Const.START_PANEL_EXPLORER_PATH, Const.DESKTOP_ICON_THIS_COMPUTER);

        public static void _203(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._203_AUTO_CHECK_SELECT,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _204(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._204_HIDDEN,
                                                                                IsActive ? Const._204_ENABLED_VALUE
                                                                                         : Const._204_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _205(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._205_HIDE_FILE_EXT,
                                                                                IsActive ? Const._205_SHOW_VALUE
                                                                                         : Const._205_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _206(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._206_HIDE_MERGE_CONF,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _208(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.ADVANCED_EXPLORER_PATH,
                                                                    Const.LAUNCH_TO,
                                                                        Const.LAUNCH_PC_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _209(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.ADVANCED_EXPLORER_PATH,
                                                                    Const.LAUNCH_TO,
                                                                        Const.LAUNCH_QA_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _210(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._210_CORTANA_BUTTON,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _211(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._211_SHOW_SYNC_PROVIDER,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _212(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._212_SNAP_ASSIST,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _214(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.STATUS_MANAGER_PATH,
                                                                    Const.ENTHUSIAST_MODE,
                                                                        Const.DIALOG_DETAILED_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _215(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.STATUS_MANAGER_PATH,
                                                                    Const.ENTHUSIAST_MODE,
                                                                        Const.DIALOG_COMPACT_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _216(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._216_RIBBON_EXPLORER_PATH,
                                                                            Const._216_TABLET_MODE_OFF,
                                                                                IsActive ? Const._216_MINIMIZED_VALUE
                                                                                         : Const._216_EXPANDED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _217(bool IsActive)
        {
            var shellState = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._217_SHELL_STATE);
            shellState[4] = IsActive ? Const._217_SHELL_ENABLED_VALUE : Const._217_SHELL_DISABLED_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._217_SHELL_STATE, shellState, RegistryValueKind.Binary);
        }

        public static void _218(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const._218_3D_OBJECT_PROPERTY_PATH, Const._218_PC_POLICY);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const._218_3D_OBJECT_PROPERTY_PATH, Const._218_PC_POLICY, Const._218_3D_OBJECT_HIDE_VALUE);
        }

        public static void _219(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CURRENT_EXPLORER_PATH,
                                                                            Const._219_SHOW_RECENT,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _220(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CURRENT_EXPLORER_PATH,
                                                                            Const._220_SHOW_FREQUENT,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _221(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._221_SHOW_TASK_VIEW,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _222(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._222_PEOPLE_EXPLORER_PATH,
                                                                            Const._222_PEOPLE_BAND,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _223(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._223_SHOW_SECONDS,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _225(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.TASKBAR_SEARCH_PATH,
                                                                    Const.TASKBAR_SEARCH_MODE,
                                                                        Const.TASKBAR_SEARCH_HIDE_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _226(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.TASKBAR_SEARCH_PATH,
                                                                    Const.TASKBAR_SEARCH_MODE,
                                                                        Const.TASKBAR_SEARCH_ICON_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _227(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.TASKBAR_SEARCH_PATH,
                                                                    Const.TASKBAR_SEARCH_MODE,
                                                                        Const.TASKBAR_SEARCH_BOX_VALUE,
                                                                            RegistryValueKind.DWord);

        public static void _228(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._228_PEN_WORKSPACE_PATH,
                                                                            Const._228_PEN_WORKSPACE_VISIBILITY,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _229(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.CURRENT_EXPLORER_PATH,
                                                                            Const._229_AUTO_TRAY,
                                                                                IsActive ? Const._229_AUTO_TRAY_SHOW_VALUE
                                                                                         : Const._229_AUTO_TRAY_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _230(bool IsActive)
        {
            var settings = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, Const._230_STUCK_RECTS3_PATH, Const._230_STUCK_RECTS3_SETTINGS);
            settings[9] = IsActive ? Const._230_STUCK_RECTS3_SHOW_VALUE : Const._230_STUCK_RECTS3_HIDE_VALUE;
            RegHelper.SetValue(RegistryHive.CurrentUser, Const._230_STUCK_RECTS3_PATH, Const._230_STUCK_RECTS3_SETTINGS, settings, RegistryValueKind.Binary);
        }

        public static void _231(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._213_FEEDS_PATH,
                                                                            Const._231_SHELL_FEEDS,
                                                                                IsActive ? Const._231_SHELL_FEEDS_ENABLED_VALUE
                                                                                         : Const._231_SHELL_FEEDS_DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _233(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW, Const.ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE, Const.STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _234(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW, Const.ALL_ITEMS_ICON_SMALL_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE, Const.STARTUP_PAGE_ICON_VALUE, RegistryValueKind.DWord);
        }

        public static void _235(bool _)
        {
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW, Const.ALL_ITEMS_ICON_CATEGORY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE, Const.STARTUP_PAGE_CATEGORY_VALUE, RegistryValueKind.DWord);
        }

        public static void _237(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.SYSTEM_USES_THEME, Const.LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _238(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.SYSTEM_USES_THEME, Const.DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _240(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.APPS_USES_THEME, Const.LIGHT_THEME_VALUE, RegistryValueKind.DWord);

        public static void _241(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.APPS_USES_THEME, Const.DARK_THEME_VALUE, RegistryValueKind.DWord);

        public static void _242(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._242_NO_NEW_APP_ALERT);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._242_NO_NEW_APP_ALERT, Const._242_HIDE_ALERT_VALUE, RegistryValueKind.DWord);
        }

        public static void _243(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        Const._243_WINLOGON_PATH,
                                                                            Const._243_FIRST_LOGON_ANIMATION,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _245(bool _) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                Const.CONTROL_PANEL_DESKTOP_PATH,
                                                                    Const.JPEG_QUALITY,
                                                                        Const._245_JPEG_MAX_QUALITY,
                                                                            RegistryValueKind.DWord);

        public static void _246(bool _) => RegHelper.DeleteKey(RegistryHive.CurrentUser,
                                                                Const.CONTROL_PANEL_DESKTOP_PATH,
                                                                    Const.JPEG_QUALITY);

        public static void _247(bool IsActive) => RegHelper.SetValue(RegistryHive.LocalMachine,
                                                                        Const._247_WINDOWS_UPDATE_SETTINGS_PATH,
                                                                            Const._247_RESTART_NOTIFICATIONS,
                                                                                IsActive ? Const._247_SHOW_VALUE
                                                                                         : Const._247_HIDE_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _248(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.CurrentUser, Const._248_EXPLORER_NAMING_PATH, Const._248_SHORTCUT);
                return;
            }

            RegHelper.SetValue(RegistryHive.CurrentUser, Const._248_EXPLORER_NAMING_PATH, Const._248_SHORTCUT, Const._248_DISABLE_VALUE);
        }

        public static void _249(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const._249_CONTROL_PANEL_KEYBOARD_PATH,
                                                                            Const._249_PRINT_SCREEN_SNIPPING,
                                                                                IsActive ? Const.ENABLED_VALUE
                                                                                         : Const.DISABLED_VALUE,
                                                                                    RegistryValueKind.DWord);

        public static void _250(bool IsActive) => SystemParametersHelper.SetInputSettings(IsActive);

        public static void _251(bool IsActive) => RegHelper.SetValue(RegistryHive.CurrentUser,
                                                                        Const.ADVANCED_EXPLORER_PATH,
                                                                            Const._251_DISALLOW_WINDOWS_SHAKE,
                                                                                IsActive ? Const._251_ENABLED_VALUE
                                                                                         : Const._251_DISABLED_VALUE,
                                                                                            RegistryValueKind.DWord);

        public static void _800(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_COM_PATH, string.Empty, Const._800_MSI_EXTRACT_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH, Const.MUIVERB, Const._800_MSI_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH, Const._800_MSI_ICON, Const._800_MSI_ICON_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH);
        }

        public static void _801(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_COM_PATH, string.Empty, Const._801_CAB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH, Const.MUIVERB, Const._801_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH, Const._801_CAB_LUA_SHIELD, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH);
        }

        public static void _802(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED, string.Empty);
        }

        public static void _803(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID, Const._803_CAST_TO_DEV_VALUE);
        }

        public static void _804(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID, string.Empty);
        }

        public static void _806(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _807(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _808(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _809(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _810(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _811(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _812(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _813(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _814(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _815(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _816(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _817(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _818(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, Const._818_LIB_LOCATION_PATH, string.Empty,
                                                                                IsActive ? Const._818_SHOW_VALUE : Const._818_HIDE_VALUE);

        public static void _819(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, Const._819_SEND_TO_PATH, string.Empty,
                                                                                IsActive ? Const._819_SHOW_VALUE : Const._819_HIDE_VALUE);

        public static void _820(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _821(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_ITEM_NAME, Const._821_BMP_ITEM_VALUE, RegistryValueKind.ExpandString);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_NULL_FILE, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW);
        }

        public static void _822(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.DATA, Const._822_DATA_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.ITEM_NAME, Const._822_ITEM_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW);
        }

        public static void _823(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH, Const.DATA, Const._823_ZIP_DATA, RegistryValueKind.Binary);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH, Const.ITEM_NAME, Const._823_ITEM_DATA, RegistryValueKind.ExpandString);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH);
        }

        public static void _824(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME, Const._824_PROMPT_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME);
        }

        public static void _825(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._825_NO_USE_NAME);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._825_NO_USE_NAME, Const._825_NO_USE_VALUE, RegistryValueKind.DWord);
        }
    }
}