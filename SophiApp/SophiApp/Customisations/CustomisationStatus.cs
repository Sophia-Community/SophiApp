using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SophiApp.Helpers;
using System;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public sealed class CustomisationStatus
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.Get(_100_DIAG_TRACK).StartType;
            var firewallRule = FirewallHelper.GetGroupRule(_100_DIAG_TRACK).FirstOrDefault();
            return diagTrack is ServiceStartMode.Automatic && firewallRule.Enabled && firewallRule.Action is NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        }

        public static bool _102() => _103().Invert();

        public static bool _103()
        {
            var allowTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, ALLOW_TELEMETRY);
            var maxTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, MAX_TELEMETRY_ALLOWED);
            var showedToast = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL);
            return (allowTelemetry is MIN_ENT_TELEMETRY_VALUE &&
                        maxTelemetry is MIN_ENT_TELEMETRY_VALUE &&
                            showedToast is MIN_ENT_TELEMETRY_VALUE) ||
                            (allowTelemetry is MIN_TELEMETRY_VALUE &&
                                maxTelemetry is MIN_TELEMETRY_VALUE &&
                                    showedToast is MIN_TELEMETRY_VALUE);
        }

        public static bool _104()
        {
            var taskState = ScheduledTaskHelper.GetTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK);
            var reportingValue = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _104_WER_PATH, _104_DISABLED);
            var reportingService = ServiceHelper.Get(_104_WER_SERVICE);
            return taskState == TaskState.Ready &&
                    reportingValue != _104_DISABLED_DEFAULT_VALUE &&
                        reportingService.StartType == ServiceStartMode.Manual;
        }

        public static bool _106() => _107().Invert();

        public static bool _107() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, SIUF_PATH, SIUF_PERIOD) == DISABLED_VALUE;

        public static bool _109() => ScheduledTaskHelper.GetTaskState(_109_DATA_UPDATER_TASK_PATH, _109_DATA_UPDATER_TASK) is TaskState.Ready;

        public static bool _110() => ScheduledTaskHelper.GetTaskState(_110_PROXY_TASK_PATH, _110_PROXY_TASK) is TaskState.Ready;

        public static bool _111() => ScheduledTaskHelper.GetTaskState(CEIP_TASK_PATH, _111_CONS_TASK) is TaskState.Ready;

        public static bool _112() => ScheduledTaskHelper.GetTaskState(CEIP_TASK_PATH, _112_USB_CEIP_TASK) is TaskState.Ready;

        public static bool _113() => ScheduledTaskHelper.GetTaskState(_113_DISK_DATA_TASK_PATH, _113_DISK_DATA_TASK) is TaskState.Ready;

        public static bool _114() => ScheduledTaskHelper.GetTaskState(MAPS_TASK_PATH, _114_MAPS_TOAST_TASK) is TaskState.Ready;

        public static bool _115() => ScheduledTaskHelper.GetTaskState(MAPS_TASK_PATH, _115_MAPS_UPDATE) is TaskState.Ready;

        public static bool _116() => ScheduledTaskHelper.GetTaskState(_116_FAMILY_MONITOR_TASK_PATH, _116_FAMILY_MONITOR_TASK) is TaskState.Ready;

        public static bool _117() => ScheduledTaskHelper.GetTaskState(_117_XBOX_SAVE_TASK_PATH, _117_XBOX_SAVE_TASK) is TaskState.Ready;

        public static bool _118() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine,
                                                                    $"{_118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}",
                                                                        _118_OPT_OUT) != _118_OPT_OUT_DEFAULT_VALUE;

        public static bool _119() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _119_USER_PROFILE_PATH, _119_HTTP_ACCEPT) != _119_HTTP_ACCEPT_DEFAULT_VALUE;

        public static bool _120() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _120_ADVERT_INFO_PATH, _120_ADVERT_ENABLED)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _121() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _121_SUB_CONTENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _122() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _122_SUB_CONTENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _123()
        {
            var content93 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_93);
            var content94 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_94);
            var content96 = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _123_SUB_CONTENT_96);
            return content93.HasNullOrValue(ENABLED_VALUE) && content94.HasNullOrValue(ENABLED_VALUE) && content96.HasNullOrValue(ENABLED_VALUE);
        }

        public static bool _124() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _124_SILENT_APP_INSTALL)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _125() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _125_PROFILE_ENGAGE_PATH, _125_SETTING_ENABLED)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _126() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _126_PRIVACY_PATH, _126_TAILORED_DATA)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _127() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, POLICIES_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS)
                                              .HasNullOrValue(DISABLED_VALUE);

        public static bool _201() => _202().Invert();

        public static bool _202() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _203() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _203_AUTO_CHECK_SELECT)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _204() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _204_HIDDEN)
                                              .HasNullOrValue(_204_DISABLED_VALUE)
                                              .Invert();

        public static bool _205() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _205_HIDE_FILE_EXT)
                                              .HasNullOrValue(_205_HIDE_VALUE)
                                              .Invert();

        public static bool _206() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _206_HIDE_MERGE_CONF)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _208() => _209().Invert();

        public static bool _209() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, LAUNCH_TO)
                                              .HasNullOrValue(LAUNCH_QA_VALUE);

        public static bool _210() => UwpHelper.PackageExist(_210_CORTANA)
                                              ? RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _210_CORTANA_BUTTON)
                                                         .HasNullOrValue(ENABLED_VALUE)
                                              : throw new UwpAppNotFoundException(_210_CORTANA);

        public static bool _211() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _211_SHOW_SYNC_PROVIDER)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _212() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _212_SNAP_ASSIST)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _214() => _215().Invert();

        public static bool _215() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STATUS_MANAGER_PATH, ENTHUSIAST_MODE)
                                              .HasNullOrValue(DIALOG_COMPACT_VALUE);

        public static bool _216() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _216_RIBBON_EXPLORER_PATH, _216_TABLET_MODE_OFF)
                                              .HasNullOrValue(_216_MINIMIZED_VALUE);

        public static bool _217() => RegHelper.KeyExist(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _217_SHELL_STATE)[4] == _217_SHELL_ENABLED_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{CURRENT_EXPLORER_PATH}\{_217_SHELL_STATE}");

        public static bool _218() => RegHelper.GetStringValue(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY) == null
                                     || RegHelper.GetStringValue(RegistryHive.LocalMachine, _218_3D_OBJECT_PROPERTY_PATH, _218_PC_POLICY) != _218_3D_OBJECT_HIDE_VALUE;

        public static bool _219() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _219_SHOW_RECENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _220() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _220_SHOW_FREQUENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _221() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _221_SHOW_TASK_VIEW)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _222() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _222_PEOPLE_EXPLORER_PATH, _222_PEOPLE_BAND)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _223() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _223_SHOW_SECONDS)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _225() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE) == TASKBAR_SEARCH_HIDE_VALUE;

        public static bool _226() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE) == TASKBAR_SEARCH_ICON_VALUE;

        public static bool _227() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE)
                                              .HasNullOrValue(TASKBAR_SEARCH_BOX_VALUE);

        public static bool _228() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _228_PEN_WORKSPACE_PATH, _228_PEN_WORKSPACE_VISIBILITY)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _229() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _229_AUTO_TRAY)
                                              .HasNullOrValue(_229_AUTO_TRAY_HIDE_VALUE)
                                              .Invert();

        public static bool _230() => RegHelper.KeyExist(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, _230_STUCK_RECTS3_PATH, _230_STUCK_RECTS3_SETTINGS)[9] == _230_STUCK_RECTS3_SHOW_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{_230_STUCK_RECTS3_PATH}\{_230_STUCK_RECTS3_SETTINGS}");

        public static bool _231() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _213_FEEDS_PATH, _231_SHELL_FEEDS)
                                              .HasNullOrValue(_231_SHELL_FEEDS_ENABLED_VALUE);

        public static bool _233() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasValue(ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasValue(STARTUP_PAGE_ICON_VALUE);

        public static bool _234() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasValue(ALL_ITEMS_ICON_SMALL_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasValue(STARTUP_PAGE_ICON_VALUE);

        public static bool _235() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasNullOrValue(ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasNullOrValue(STARTUP_PAGE_CATEGORY_VALUE);

        public static bool _237() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME)
                                              .HasValue(LIGHT_THEME_VALUE);

        public static bool _238() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME)
                                              .HasNullOrValue(DARK_THEME_VALUE);

        public static bool _240() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME)
                                              .HasValue(LIGHT_THEME_VALUE);

        public static bool _241() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME)
                                              .HasNullOrValue(DARK_THEME_VALUE);

        public static bool _242() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _242_NO_NEW_APP_ALERT)
                                              .HasNullOrValue(_242_SHOW_ALERT_VALUE);

        public static bool _243() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _243_WINLOGON_PATH, _243_FIRST_LOGON_ANIMATION)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _245() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_DESKTOP_PATH, JPEG_QUALITY)
                                              .HasValue(_245_JPEG_MAX_QUALITY);

        public static bool _246() => _245().Invert();

        public static bool _247() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _247_WINDOWS_UPDATE_SETTINGS_PATH, _247_RESTART_NOTIFICATIONS)
                                              .HasNullOrValue(_247_HIDE_VALUE).Invert();

        public static bool _248()
        {
            RegHelper.TryDeleteKey(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _248_LINK);
            return RegHelper.GetStringValue(RegistryHive.CurrentUser, _248_EXPLORER_NAMING_PATH, _248_SHORTCUT) is null;
        }

        public static bool _249() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _249_CONTROL_PANEL_KEYBOARD_PATH, _249_PRINT_SCREEN_SNIPPING)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _250() => SystemParametersHelper.GetInputSettings();

        public static bool _251() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _251_DISALLOW_WINDOWS_SHAKE)
                                              .HasNullOrValue(_251_ENABLED_VALUE);

        public static bool _300() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _301() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01) == ENABLED_VALUE
                                              ? true
                                              : throw new RegistryKeyUnexpectedValue($@"{RegistryHive.CurrentUser}\{STORAGE_POLICY_PATH}\{STORAGE_POLICY_01}");

        public static bool _302() => _303().Invert();

        public static bool _303() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048)
                                              .HasNullOrValue(DISABLED_VALUE);

        public static bool _304() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01) == ENABLED_VALUE
                                              ? RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, _304_STORAGE_POLICY_04).HasNullOrValue(DISABLED_VALUE).Invert()
                                              : throw new RegistryKeyUnexpectedValue($@"{RegistryHive.CurrentUser}\{STORAGE_POLICY_PATH}\{STORAGE_POLICY_01}");

        public static bool _305() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _305_POWER_CONTROL_PATH, _305_HIBERNATE) == _305_ENABLED_VALUE;

        public static bool _307() => Environment.ExpandEnvironmentVariables(ENVIRONMENT_TEMP)
                                        == Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_DRIVE}\\{TEMP_FOLDER}");

        public static bool _308() => _307().Invert();

        public static bool _309() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _309_CONTROL_FILE_SYSTEM_PATH, _309_LONG_PATH)
                                              .HasNullOrValue(_309_ENABLED_VALUE);

        public static bool _310() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _310_SYSTEM_CRASH_CONTROL_PATH, _310_DISPLAY_PARAMS)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _312() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, ADMIN_PROMPT)
                                              .HasNullOrValue(ADMIN_PROMPT_DEFAULT_VALUE);

        public static bool _313() => _312().Invert();

        public static bool _314() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICIES_SYSTEM_PATH, _314_ENABLE_LINKED) == _314_ENABLE_LINKED_VALUE;

        public static bool _315() => RegHelper.GetNullableIntValue(RegistryHive.Users, _315_DELIVERY_SETTINGS_PATH, _315_DOWNLOAD_MODE)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _800_MSI_EXTRACT_PATH);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _801_CAB_COM_PATH);

        public static bool _802() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _802_RUNAS_USER_PATH, _802_EXTENDED).Invert();

        public static bool _803() => (RegHelper.GetStringValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _803_CAST_TO_DEV_GUID) == _803_CAST_TO_DEV_VALUE)
                                               .Invert();

        public static bool _804() => RegHelper.KeyExist(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _804_SHARE_GUID).Invert();

        public static bool _805() => UwpHelper.PackageExist(_805_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(_805_MS_PAINT_3D);

        public static bool _806() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _806_BMP_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _807() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _807_GIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _808() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _808_JPE_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _809() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _809_JPEG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _810() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _810_JPG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _811() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _811_PNG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _812() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _812_TIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _813() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _813_TIFF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _814() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _814_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _815_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _816_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _817() => (RegHelper.KeyExist(RegistryHive.ClassesRoot, _817_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, _817_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY))
                                                   .Invert();

        public static bool _818() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, _818_LIB_LOCATION_PATH, string.Empty) == _818_SHOW_VALUE;

        public static bool _819() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, _819_SEND_TO_PATH, string.Empty) == _819_SHOW_VALUE;

        public static bool _820() => OsHelper.IsEdition(WIN_VER_PRO) || OsHelper.IsEdition(WIN_VER_ENT)
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == DISABLED_VALUE
                                                ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _820_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                                : throw new BitlockerIsEnabledException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _821() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _822() => DismHelper.CapabilityIsInstalled(_822_MS_WORD_PAD)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, DATA)
                                     : throw new WindowsCapabilityNotInstalledException(_822_MS_WORD_PAD);

        public static bool _823() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _823_ZIP_SHELLNEW_PATH);

        public static bool _824() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _824_PROMPT_NAME)
                                              .HasNullOrValue(_824_PROMPT_VALUE);

        public static bool _825() => RegHelper.KeyExist(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _825_NO_USE_NAME).Invert();

        /// <summary>
        /// There must be a little magic in every app
        /// </summary>

        public static bool ItsMagic() => true;
    }
}