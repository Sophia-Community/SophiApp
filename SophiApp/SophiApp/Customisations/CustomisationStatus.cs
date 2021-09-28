using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SophiApp.Helpers;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Const = SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public sealed class CustomisationStatus
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.GetService(Const._100_DIAG_TRACK).StartType;
            var firewallRule = FirewallHelper.GetGroupRule(Const._100_DIAG_TRACK).FirstOrDefault();
            return diagTrack is ServiceStartMode.Automatic && firewallRule.Enabled && firewallRule.Action is NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        }

        public static bool _102() => _103().Invert();

        public static bool _103()
        {
            var allowTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY);
            var maxTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED);
            var showedToast = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL);
            return (allowTelemetry is Const.MIN_ENT_TELEMETRY_VALUE &&
                        maxTelemetry is Const.MIN_ENT_TELEMETRY_VALUE &&
                            showedToast is Const.MIN_ENT_TELEMETRY_VALUE) ||
                            (allowTelemetry is Const.MIN_TELEMETRY_VALUE &&
                                maxTelemetry is Const.MIN_TELEMETRY_VALUE &&
                                    showedToast is Const.MIN_TELEMETRY_VALUE);
        }

        public static bool _104()
        {
            var taskState = ScheduledTaskHelper.GetTaskState(Const._104_QUEUE_TASK_PATH, Const._104_QUEUE_TASK);
            var reportingValue = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED);
            var reportingService = ServiceHelper.GetService(Const._104_WER_SERVICE);
            return taskState == TaskState.Ready &&
                    reportingValue != Const._104_DISABLED_DEFAULT_VALUE &&
                        reportingService.StartType == ServiceStartMode.Manual;
        }

        public static bool _106() => _107().Invert();

        public static bool _107() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.SIUF_PATH, Const.SIUF_PERIOD) == Const.DISABLED_VALUE;

        public static bool _109() => ScheduledTaskHelper.GetTaskState(Const._109_DATA_UPDATER_TASK_PATH, Const._109_DATA_UPDATER_TASK) is TaskState.Ready;

        public static bool _110() => ScheduledTaskHelper.GetTaskState(Const._110_PROXY_TASK_PATH, Const._110_PROXY_TASK) is TaskState.Ready;

        public static bool _111() => ScheduledTaskHelper.GetTaskState(Const.CEIP_TASK_PATH, Const._111_CONS_TASK) is TaskState.Ready;

        public static bool _112() => ScheduledTaskHelper.GetTaskState(Const.CEIP_TASK_PATH, Const._112_USB_CEIP_TASK) is TaskState.Ready;

        public static bool _113() => ScheduledTaskHelper.GetTaskState(Const._113_DISK_DATA_TASK_PATH, Const._113_DISK_DATA_TASK) is TaskState.Ready;

        public static bool _114() => ScheduledTaskHelper.GetTaskState(Const.MAPS_TASK_PATH, Const._114_MAPS_TOAST_TASK) is TaskState.Ready;

        public static bool _115() => ScheduledTaskHelper.GetTaskState(Const.MAPS_TASK_PATH, Const._115_MAPS_UPDATE) is TaskState.Ready;

        public static bool _116() => ScheduledTaskHelper.GetTaskState(Const._116_FAMILY_MONITOR_TASK_PATH, Const._116_FAMILY_MONITOR_TASK) is TaskState.Ready;

        public static bool _117() => ScheduledTaskHelper.GetTaskState(Const._117_XBOX_SAVE_TASK_PATH, Const._117_XBOX_SAVE_TASK) is TaskState.Ready;

        public static bool _118() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine,
                                                                    $"{Const._118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}",
                                                                        Const._118_OPT_OUT) != Const._118_OPT_OUT_DEFAULT_VALUE;

        public static bool _119() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._119_USER_PROFILE_PATH, Const._119_HTTP_ACCEPT) != Const._119_HTTP_ACCEPT_DEFAULT_VALUE;

        public static bool _120() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._120_ADVERT_INFO_PATH, Const._120_ADVERT_ENABLED)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _121() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._121_SUB_CONTENT)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _122() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._122_SUB_CONTENT)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _123()
        {
            var content93 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_93);
            var content94 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_94);
            var content96 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_96);
            return content93.HasNullOrValue(Const.ENABLED_VALUE) && content94.HasNullOrValue(Const.ENABLED_VALUE) && content96.HasNullOrValue(Const.ENABLED_VALUE);
        }

        public static bool _124() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._124_SILENT_APP_INSTALL)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _125() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._125_PROFILE_ENGAGE_PATH, Const._125_SETTING_ENABLED)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _126() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._126_PRIVACY_PATH, Const._126_TAILORED_DATA)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _127() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.POLICIES_EXPLORER_PATH, Const._127_DISABLE_SEARCH_SUGGESTIONS)
                                              .HasNullOrValue(Const.DISABLED_VALUE);

        public static bool _201() => _202().Invert();

        public static bool _202() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.START_PANEL_EXPLORER_PATH, Const.DESKTOP_ICON_THIS_COMPUTER)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _203() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._203_AUTO_CHECK_SELECT)
                                              .HasNullOrValue(Const.DISABLED_VALUE)
                                              .Invert();

        public static bool _204() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._204_HIDDEN)
                                              .HasNullOrValue(Const._204_DISABLED_VALUE)
                                              .Invert();

        public static bool _205() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._205_HIDE_FILE_EXT)
                                              .HasNullOrValue(Const._205_HIDE_VALUE)
                                              .Invert();

        public static bool _206() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._206_HIDE_MERGE_CONF)
                                              .HasNullOrValue(Const.DISABLED_VALUE)
                                              .Invert();

        public static bool _208() => _209().Invert();

        public static bool _209() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const.LAUNCH_TO)
                                              .HasNullOrValue(Const.LAUNCH_QA_VALUE);

        public static bool _210() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._210_CORTANA_BUTTON)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _211() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._211_SHOW_SYNC_PROVIDER)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _212() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._212_SNAP_ASSIST)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _214() => _215().Invert();

        public static bool _215() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.STATUS_MANAGER_PATH, Const.ENTHUSIAST_MODE)
                                              .HasNullOrValue(Const.DIALOG_COMPACT_VALUE);

        public static bool _216() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._216_RIBBON_EXPLORER_PATH, Const._216_TABLET_MODE_OFF)
                                              .HasNullOrValue(Const._216_MINIMIZED_VALUE).Invert();

        public static bool _217() => RegHelper.KeyExist(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._217_SHELL_STATE)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._217_SHELL_STATE)[4] == Const._217_SHELL_ENABLED_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{Const.CURRENT_EXPLORER_PATH}\{Const._217_SHELL_STATE}");

        public static bool _218() => RegHelper.GetStringValue(RegistryHive.LocalMachine, Const._218_3D_OBJECT_PROPERTY_PATH, Const._218_PC_POLICY) == null
                                     || RegHelper.GetStringValue(RegistryHive.LocalMachine, Const._218_3D_OBJECT_PROPERTY_PATH, Const._218_PC_POLICY) != Const._218_3D_OBJECT_HIDE_VALUE;

        public static bool _219() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._219_SHOW_RECENT)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _220() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._220_SHOW_FREQUENT)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _221() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._221_SHOW_TASK_VIEW)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _222() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._222_PEOPLE_EXPLORER_PATH, Const._222_PEOPLE_BAND)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _223() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._223_SHOW_SECONDS)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _225() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.TASKBAR_SEARCH_PATH, Const.TASKBAR_SEARCH_MODE) == Const.TASKBAR_SEARCH_HIDE_VALUE;

        public static bool _226() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.TASKBAR_SEARCH_PATH, Const.TASKBAR_SEARCH_MODE) == Const.TASKBAR_SEARCH_ICON_VALUE;

        public static bool _227() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.TASKBAR_SEARCH_PATH, Const.TASKBAR_SEARCH_MODE)
                                              .HasNullOrValue(Const.TASKBAR_SEARCH_BOX_VALUE);

        public static bool _228() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._228_PEN_WORKSPACE_PATH, Const._228_PEN_WORKSPACE_VISIBILITY)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _229() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._229_AUTO_TRAY)
                                              .HasNullOrValue(Const._229_AUTO_TRAY_HIDE_VALUE)
                                              .Invert();

        public static bool _230() => RegHelper.KeyExist(RegistryHive.CurrentUser, Const._230_STUCK_RECTS3_PATH, Const._230_STUCK_RECTS3_SETTINGS)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, Const._230_STUCK_RECTS3_PATH, Const._230_STUCK_RECTS3_SETTINGS)[9] == Const._230_STUCK_RECTS3_SHOW_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{Const._230_STUCK_RECTS3_PATH}\{Const._230_STUCK_RECTS3_SETTINGS}");

        public static bool _231() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._213_FEEDS_PATH, Const._231_SHELL_FEEDS)
                                              .HasNullOrValue(Const._231_SHELL_FEEDS_ENABLED_VALUE);

        public static bool _233() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW).HasValue(Const.ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE).HasValue(Const.STARTUP_PAGE_ICON_VALUE);

        public static bool _234() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW).HasValue(Const.ALL_ITEMS_ICON_SMALL_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE).HasValue(Const.STARTUP_PAGE_ICON_VALUE);

        public static bool _235() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.ALL_ITEMS_ICON_VIEW).HasNullOrValue(Const.ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_EXPLORER_PATH, Const.STARTUP_PAGE).HasNullOrValue(Const.STARTUP_PAGE_CATEGORY_VALUE);

        public static bool _237() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.SYSTEM_USES_THEME)
                                              .HasValue(Const.LIGHT_THEME_VALUE);

        public static bool _238() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.SYSTEM_USES_THEME)
                                              .HasNullOrValue(Const.DARK_THEME_VALUE);

        public static bool _240() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.APPS_USES_THEME)
                                              .HasValue(Const.LIGHT_THEME_VALUE);

        public static bool _241() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.PERSONALIZE_PATH, Const.APPS_USES_THEME)
                                              .HasNullOrValue(Const.DARK_THEME_VALUE);

        public static bool _242() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._242_NO_NEW_APP_ALERT)
                                              .HasNullOrValue(Const._242_SHOW_ALERT_VALUE);

        public static bool _243() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, Const._243_WINLOGON_PATH, Const._243_FIRST_LOGON_ANIMATION)
                                              .HasNullOrValue(Const.ENABLED_VALUE);

        public static bool _245() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CONTROL_PANEL_DESKTOP_PATH, Const.JPEG_QUALITY)
                                              .HasValue(Const._245_JPEG_MAX_QUALITY);

        public static bool _246() => _245().Invert();

        public static bool _247() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, Const._247_WINDOWS_UPDATE_SETTINGS_PATH, Const._247_RESTART_NOTIFICATIONS)
                                              .HasNullOrValue(Const._247_HIDE_VALUE).Invert();

        public static bool _248()
        {
            var shortcut = RegHelper.GetStringValue(RegistryHive.CurrentUser, Const._248_EXPLORER_NAMING_PATH, Const._248_SHORTCUT);
            var link = RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._248_LINK);
            RegHelper.TryDeleteKey(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._248_LINK);
            return shortcut == null || link == null;
        }

        public static bool _249() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const._249_CONTROL_PANEL_KEYBOARD_PATH, Const._249_PRINT_SCREEN_SNIPPING)
                                              .HasNullOrValue(Const.DISABLED_VALUE).Invert();

        public static bool _250() => SystemParametersHelper.GetInputSettings();

        public static bool _251() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.ADVANCED_EXPLORER_PATH, Const._251_DISALLOW_WINDOWS_SHAKE)
                                              .HasNullOrValue(Const._251_ENABLED_VALUE);

        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._801_CAB_COM_PATH);

        public static bool _802() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED).Invert();

        public static bool _803() => (RegHelper.GetStringValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID) == Const._803_CAST_TO_DEV_VALUE)
                                               .Invert();

        public static bool _804() => RegHelper.KeyExist(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID).Invert();

        public static bool _805() => UwpHelper.PackageExist(Const._805_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(Const._805_MS_PAINT_3D);

        public static bool _806() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _807() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _808() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _809() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _810() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _811() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _812() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _813() => RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY).Invert();

        public static bool _814() => UwpHelper.PackageExist(Const.UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(Const.UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(Const.CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new WindowsCapabilityNotInstalledException(Const.CAPABILITY_MS_PAINT);

        public static bool _817() => (RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY))
                                                   .Invert();

        public static bool _818() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, Const._818_LIB_LOCATION_PATH, string.Empty) == Const._818_SHOW_VALUE;

        public static bool _819() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, Const._819_SEND_TO_PATH, string.Empty) == Const._819_SHOW_VALUE;

        public static bool _820() => OsHelper.IsEdition(Const.WIN_VER_PRO) || OsHelper.IsEdition(Const.WIN_VER_ENT)
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == Const.DISABLED_VALUE
                                                ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY).Invert()
                                                : throw new BitlockerIsEnabledException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _821() => DismHelper.CapabilityIsInstalled(Const.CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(Const.CAPABILITY_MS_PAINT);

        public static bool _822() => DismHelper.CapabilityIsInstalled(Const._822_MS_WORD_PAD)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.DATA)
                                     : throw new WindowsCapabilityNotInstalledException(Const._822_MS_WORD_PAD);

        public static bool _823() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH);

        public static bool _824() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, Const.CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME)
                                              .HasNullOrValue(Const._824_PROMPT_VALUE);

        public static bool _825() => RegHelper.KeyExist(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._825_NO_USE_NAME).Invert();

        /// <summary>
        /// There must be a little magic in every app
        /// </summary>

        public static bool ItsMagic() => true;
    }
}