using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using Newtonsoft.Json;
using SophiApp.Dto;
using SophiApp.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public static class CustomisationStatus
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.Get(_100_DIAG_TRACK).StartType;
            var firewallRule = FirewallHelper.GetGroupRule(_100_DIAG_TRACK).First();
            return diagTrack is ServiceStartMode.Automatic && firewallRule.Enabled && firewallRule.Action is NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        }

        public static bool _102() => _103().Invert();

        public static bool _103()
        {
            var allowTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH, ALLOW_TELEMETRY) == ENABLED_VALUE;
            var maxTelemetry = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DATA_COLLECTION_PATH_ALLOWED, MAX_TELEMETRY_ALLOWED) == ENABLED_VALUE;
            var showedToast = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, DIAG_TRACK_PATH, SHOWED_TOAST_LEVEL) == ENABLED_VALUE;
            return allowTelemetry && maxTelemetry && showedToast;
        }

        public static bool _104()
        {
            var taskState = ScheduledTaskHelper.GetTaskState(_104_QUEUE_TASK_PATH, _104_QUEUE_TASK);
            var reportingValue = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _104_WER_PATH, DISABLED);
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

        public static bool _120() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _120_ADVERT_INFO_PATH, ENABLED)
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

        public static bool _127() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _127_DISABLE_SEARCH_SUGGESTIONS)
                                              .HasNullOrValue(DISABLED_VALUE);

        public static bool _201() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_THIS_COMPUTER) == DISABLED_VALUE;

        public static bool _202() => _201().Invert();

        public static bool _204() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _204_AUTO_CHECK_SELECT)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _205() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _205_HIDDEN)
                                              .HasNullOrValue(_205_DISABLED_VALUE)
                                              .Invert();

        public static bool _206() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _206_HIDE_FILE_EXT)
                                              .HasNullOrValue(_206_HIDE_VALUE)
                                              .Invert();

        public static bool _207() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _207_HIDE_MERGE_CONF)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _209() => _210().Invert();

        public static bool _210() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, LAUNCH_TO)
                                              .HasNullOrValue(LAUNCH_QA_VALUE);

        public static bool _211() => UwpHelper.PackageExist(UWP_MS_CORTANA)
                                              ? RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _211_CORTANA_BUTTON)
                                                         .HasNullOrValue(ENABLED_VALUE)
                                              : throw new UwpAppNotFoundException(UWP_MS_CORTANA);

        public static bool _212() => RegHelper.GetByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _212_EXPLORER_COMPACT_MODE) == ENABLED_VALUE;

        public static bool _213() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _213_SHOW_SYNC_PROVIDER)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _214() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _214_SNAP_ASSIST)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _215() => (RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _215_SNAP_ASSIST_FLYOUT) == DISABLED_VALUE).Invert();

        public static bool _217() => _218().Invert();

        public static bool _218() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STATUS_MANAGER_PATH, ENTHUSIAST_MODE)
                                              .HasNullOrValue(DIALOG_COMPACT_VALUE);

        public static bool _219() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _219_RIBBON_EXPLORER_PATH, _219_TABLET_MODE_OFF)
                                              .HasNullOrValue(_219_MINIMIZED_VALUE);

        public static bool _220() => RegHelper.KeyExist(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _220_SHELL_STATE)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _220_SHELL_STATE)[4] == _220_SHELL_ENABLED_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{CURRENT_VERSION_EXPLORER_PATH}\{_220_SHELL_STATE}");

        public static bool _221() => RegHelper.GetStringValue(RegistryHive.LocalMachine, _221_3D_OBJECT_PROPERTY_PATH, _221_PC_POLICY) == null
                                     || RegHelper.GetStringValue(RegistryHive.LocalMachine, _221_3D_OBJECT_PROPERTY_PATH, _221_PC_POLICY) != _221_3D_OBJECT_HIDE_VALUE;

        public static bool _222() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _222_SHOW_RECENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _223() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _223_SHOW_FREQUENT)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _225() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, EXPLORER_TASKBAR_ALIGNMENT) == _225_TASKBAR_ALIGNMENT_LEFT;

        public static bool _226() => _225().Invert();

        public static bool _227() => RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE) != DISABLED_VALUE;

        public static bool _228() => RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, SHOW_TASKVIEW_BUTTON) != DISABLED_VALUE;

        public static bool _229() => RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, SHOW_TASKVIEW_BUTTON) != DISABLED_VALUE;

        public static bool _230() => UwpHelper.PackageExist(_230_UWP_WEB_EXPERIENCE)
                                     ? RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _230_WIDGETS_IN_TASKBAR) != DISABLED_VALUE
                                     : throw new UwpAppNotFoundException(_230_UWP_WEB_EXPERIENCE);

        public static bool _231() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _231_PEOPLE_EXPLORER_PATH, _231_PEOPLE_BAND) == ENABLED_VALUE;

        public static bool _232() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _232_SHOW_SECONDS) == ENABLED_VALUE;

        public static bool _234() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE) == TASKBAR_SEARCH_HIDE_VALUE;

        public static bool _235() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE) == TASKBAR_SEARCH_ICON_VALUE;

        public static bool _236() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, TASKBAR_SEARCH_PATH, TASKBAR_SEARCH_MODE)
                                              .HasNullOrValue(TASKBAR_SEARCH_BOX_VALUE);

        public static bool _237() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _237_PEN_WORKSPACE_PATH, _237_PEN_WORKSPACE_VISIBILITY) == ENABLED_VALUE;

        public static bool _238() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _238_AUTO_TRAY)
                                              .HasNullOrValue(_238_AUTO_TRAY_HIDE_VALUE)
                                              .Invert();

        public static bool _239() => RegHelper.KeyExist(RegistryHive.CurrentUser, _239_STUCK_RECTS3_PATH, _239_STUCK_RECTS3_SETTINGS)
                                     ? RegHelper.GetByteArrayValue(RegistryHive.CurrentUser, _239_STUCK_RECTS3_PATH, _239_STUCK_RECTS3_SETTINGS)[9] == _239_STUCK_RECTS3_SHOW_VALUE
                                     : throw new RegistryKeyNotFoundException($@"{RegistryHive.CurrentUser}\{_239_STUCK_RECTS3_PATH}\{_239_STUCK_RECTS3_SETTINGS}");

        public static bool _240() => (RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _240_FEEDS_PATH, _240_SHELL_FEEDS_MODE) == _240_SHELL_FEEDS_DISABLED_VALUE
                                        || RegHelper.GetNullableByteValue(RegistryHive.LocalMachine, _240_FEEDS_POLICY_PATH, _240_ENABLE_FEEDS) == _240_SHELL_FEEDS_ENABLED_VALUE).Invert();

        public static bool _241() => RegHelper.GetByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _241_TASKBAR_TEAMS_ICON) == ENABLED_VALUE;

        public static bool _243() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasValue(ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasValue(STARTUP_PAGE_ICON_VALUE);

        public static bool _244() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasValue(ALL_ITEMS_ICON_SMALL_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasValue(STARTUP_PAGE_ICON_VALUE);

        public static bool _245() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, ALL_ITEMS_ICON_VIEW).HasNullOrValue(ALL_ITEMS_ICON_CATEGORY_VALUE)
                                     && RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_EXPLORER_PATH, STARTUP_PAGE).HasNullOrValue(STARTUP_PAGE_CATEGORY_VALUE);

        public static bool _247() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME)
                                              .HasValue(LIGHT_THEME_VALUE);

        public static bool _248() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, SYSTEM_USES_THEME)
                                              .HasNullOrValue(DARK_THEME_VALUE);

        public static bool _250() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME)
                                              .HasValue(LIGHT_THEME_VALUE);

        public static bool _251() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, PERSONALIZE_PATH, APPS_USES_THEME)
                                              .HasNullOrValue(DARK_THEME_VALUE);

        public static bool _252() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _252_NO_NEW_APP_ALERT)
                                              .HasNullOrValue(_252_SHOW_ALERT_VALUE);

        public static bool _253() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, WINLOGON_PATH, _253_FIRST_LOGON_ANIMATION)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _255() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_DESKTOP_PATH, JPEG_QUALITY)
                                              .HasValue(_255_JPEG_MAX_QUALITY);

        public static bool _256() => _255().Invert();

        public static bool _257() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _257_WINDOWS_UPDATE_SETTINGS_PATH, _257_RESTART_NOTIFICATIONS)
                                              .HasNullOrValue(_257_HIDE_VALUE)
                                              .Invert();

        public static bool _258()
        {
            RegHelper.TryDeleteValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _258_LINK);
            return RegHelper.GetStringValue(RegistryHive.CurrentUser, _258_EXPLORER_NAMING_PATH, _258_SHORTCUT) is null;
        }

        public static bool _259() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _259_CONTROL_PANEL_KEYBOARD_PATH, _259_PRINT_SCREEN_SNIPPING)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _260() => SystemParametersHelper.GetInputSettings();

        public static bool _261() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _261_DISALLOW_WINDOWS_SHAKE) != ENABLED_VALUE;

        public static bool _263() => OsHelper.GetVersion().Build >= _263_MIN_SUPPORTED_VERSION
                                     ? RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT) == START_LAYOUT_DEFAULT_VALUE || RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT) == null
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _264() => RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT) == START_LAYOUT_PINS_VALUE;

        public static bool _265() => RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, START_LAYOUT) == START_LAYOUT_RECOMMENDATIONS_VALUE;

        public static bool _266() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICY_EXPLORER_PATH, _266_HIDE_ADDED_APPS) != _266_DISABLED_VALUE;

        public static bool _267() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _267_APP_SUGGESTIONS) == ENABLED_VALUE;

        public static bool _268() => (File.ReadAllBytes(_268_POWERSHELL_LNK)[0x15] == 2).Invert();

        public static bool _269()
        {
            var showDynamicContent = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _269_FEEDS_DSB_PATH, _269_SHOW_DYNAMIC_CONTENT) != DISABLED_VALUE;
            var dynamicSearchBox = RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _269_SEARCH_SETTINGS_PATH, _269_DYNAMIC_SEARCH_BOX) != DISABLED_VALUE;
            return showDynamicContent && dynamicSearchBox;
        }

        public static bool _270() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, START_PANEL_EXPLORER_PATH, DESKTOP_ICON_LEARN_ABOUT_THIS_PICTURE) != ENABLED_VALUE;

        public static bool _300() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_01)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _302() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, STORAGE_POLICY_2048) == _302_STORAGE_POLICY_MONTH_VALUE;

        public static bool _303() => _302().Invert();

        public static bool _304() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, STORAGE_POLICY_PATH, _304_STORAGE_POLICY_04).HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _305() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _305_POWER_CONTROL_PATH, _305_HIBERNATE) == _305_ENABLED_VALUE;

        public static bool _307() => Environment.ExpandEnvironmentVariables(ENVIRONMENT_TEMP)
                                        == Environment.ExpandEnvironmentVariables($"{ENVIRONMENT_SYSTEM_DRIVE}\\{TEMP_FOLDER}");

        public static bool _308() => _307().Invert();

        public static bool _309() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _309_CONTROL_FILE_SYSTEM_PATH, _309_LONG_PATH)
                                              .HasNullOrValue(_309_ENABLED_VALUE);

        public static bool _310() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _310_SYSTEM_CRASH_CONTROL_PATH, _310_DISPLAY_PARAMS)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _311() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, _311_ENABLE_LINKED) == _311_ENABLE_LINKED_VALUE;

        public static bool _312() => RegHelper.GetNullableIntValue(RegistryHive.Users, _312_DELIVERY_SETTINGS_PATH, _312_DOWNLOAD_MODE)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _313() => DomainHelper.PcInDomain
                                    ? RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _313_WINLOGON_PATH, _313_FOREGROUND_POLICY)
                                               .HasNullOrValue(DISABLED_VALUE)
                                               .Invert()
                                    : throw new PcNotJoinedToDomainException();

        public static bool _314() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _314_CURRENT_VERSION_WINDOWS_PATH, _314_PRINTER_LEGACY_MODE)
                                              .HasNullOrValue(_314_ENABLED_VALUE);

        public static bool _315()
        {
            var result = false;
            var updateManager = ComObjectHelper.CreateFromProgID(_315_UPDATE_SERVICE_MANAGER);

            foreach (var service in updateManager.Services)
            {
                if (service.ServiceID == _315_SERVICE_MANAGER_GUID)
                {
                    result = service.IsDefaultAUService;
                    break;
                }
            }

            return result;
        }

        public static bool _317() => WmiHelper.GetActivePowerPlanId() == _317_HIGH_POWER_GUID;

        public static bool _318() => _317().Invert();

        public static bool _319() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _319_NET_FRAMEWORK64_PATH, _319_USE_LATEST_CLR) == ENABLED_VALUE
                                     || RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _319_NET_FRAMEWORK32_PATH, _319_USE_LATEST_CLR) == ENABLED_VALUE;

        public static bool _320() => WmiHelper.HasNetworkAdaptersPowerSave();

        public static bool _322() => _323().Invert();

        public static bool _323() => RegHelper.GetStringValue(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE) == INPUT_ENG_VALUE;

        public static bool _325() => RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, IMAGES_FOLDER)
                                        == RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, _325_DESKTOP_FOLDER);

        public static bool _326() => _325().Invert();

        public static bool _328() => _329().Invert();

        public static bool _329() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, WINDOWS_MITIGATION_PATH, MITIGATION_USER_PREFERENCE)
                                              .HasNullOrValue(_329_DEFAULT_VALUE);

        public static bool _330() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _330_SEPARATE_PROCESS)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _331() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _331_RESERVE_MANAGER_PATH, _331_SHIPPED_RESERVES)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _332() => RegHelper.SubKeyExist(RegistryHive.CurrentUser, _332_TYPELIB_PATH)
                                              .Invert();

        public static bool _333() => RegHelper.GetStringValue(RegistryHive.Users, _333_DEFAULT_KEYBOARD_PATH, _333_INITIAL_INDICATORS) == _333_ENABLED_VALUE;

        public static bool _334() => RegHelper.KeyExist(RegistryHive.LocalMachine, _334_KEYBOARD_LAYOUT_PATH, _334_SCAN_CODE)
                                              .Invert();

        public static bool _335() => RegHelper.GetStringValue(RegistryHive.CurrentUser, _335_STICKY_KEYS_PATH, _335_FLAGS)
                                              .HasNullOrValue(_335_ENABLED_VALUE);

        public static bool _336() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _336_AUTOPLAY_HANDLERS_PATH, _336_AUTOPLAY)
                                              .HasNullOrValue(_336_ENABLED_VALUE);

        public static bool _337() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _337_THUMBNAIL_CACHE_PATH, _337_AUTORUN)
                                              .HasNullOrValue(_337_ENABLED_VALUE);

        public static bool _338() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, WINLOGON_PATH, _338_RESTART_APPS)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _339() => DomainHelper.PcInDomain.Invert()
                                     ? FirewallHelper.IsRuleGroupEnabled(_339_FILE_PRINTER_SHARING_GROUP) && FirewallHelper.IsRuleGroupEnabled(_339_NETWORK_DISCOVERY_GROUP)
                                     : throw new PcJoinedToDomainException();

        public static bool _340() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, UPDATE_UX_SETTINGS_PATH, _340_ACTIVE_HOURS)
                                              .HasNullOrValue(_340_AUTO_STATE);

        public static bool _341() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, UPDATE_UX_SETTINGS_PATH, _341_IS_EXPEDITED)
                                              .HasNullOrValue(DISABLED_VALUE)
                                              .Invert();

        public static bool _343()
        {
            if (UwpHelper.PackageExist(UWP_WINDOWS_TERMINAL))
            {
                var terminal = UwpHelper.GetPackage(UWP_WINDOWS_TERMINAL);

                if (terminal.Id.Version.Major == MIN_TERMINAL_SUPPORT_VERSION.Major
                    && terminal.Id.Version.Minor >= MIN_TERMINAL_SUPPORT_VERSION.Minor)
                {
                    var terminalClassRegistryPath = $@"SOFTWARE\Classes\PackagedCom\Package\{terminal.Id.FullName}\Class";

                    var consoleGuid = RegHelper.GetSubKeyNames(RegistryHive.LocalMachine, terminalClassRegistryPath)
                                               .Where(key => RegHelper.GetByteValue(RegistryHive.LocalMachine, key, TERMINAL_REGISTRY_SERVER_ID) == DISABLED_VALUE)
                                               .First()
                                               .Split('\\')
                                               .LastOrDefault();

                    var terminalGuid = RegHelper.GetSubKeyNames(RegistryHive.LocalMachine, terminalClassRegistryPath)
                                                .Where(key => RegHelper.GetByteValue(RegistryHive.LocalMachine, key, TERMINAL_REGISTRY_SERVER_ID) == ENABLED_VALUE)
                                                .First()
                                                .Split('\\')
                                                .LastOrDefault();

                    var consoleDelegation = RegHelper.GetStringValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_CONSOLE);
                    var terminalDelegation = RegHelper.GetStringValue(RegistryHive.CurrentUser, CONSOLE_STARTUP_PATH, DELEGATION_TERMINAL);

                    return consoleDelegation == consoleGuid && terminalDelegation == terminalGuid;
                }

                throw new UwpNotSupportedVersion(terminal.Id.FullName);
            }

            throw new UwpAppNotFoundException(UWP_WINDOWS_TERMINAL);
        }

        public static bool _344() => _343().Invert();

        public static bool _346() => MsiHelper.GetProperties(Directory.GetFiles(_346_INSTALLER_PATH, _346_MSI_MASK))
                                              .FirstOrDefault(property => property[_346_PRODUCT_NAME] == _346_PCHC) == null
                                     ? throw new UpdateNotInstalledException(KB5005463_UPD)
                                     : false;

        public static bool _348() => OneDriveHelper.IsInstalled()
                                     ? throw new OneDriveIsInstalledException()
                                     : OneDriveHelper.HasSetupExe() || HttpHelper.IsOnline
                                        ? false
                                        : throw new OneDriveIsInstalledException();

        public static bool _349() => OneDriveHelper.IsInstalled() ? false : throw new OneDriveNotInstalledException();

        public static bool _351() => HttpHelper.IsOnline
                                     ? VisualRedistrLibsHelper.X64IsInstalled()
                                        ? VisualRedistrLibsHelper.GetX64CloudLatestVersion() > VisualRedistrLibsHelper.GetX64InstalledVersion()
                                            ? false
                                            : throw new VisualRedistrLibsLastVersionException()
                                        : false
                                     : throw new NoInternetConnectionException();

        public static bool _352() => VisualRedistrLibsHelper.X64IsInstalled()
                                     ? false
                                     : throw new VisualRedistrLibsNotInstalled();

        public static bool _354()
        {
            if (HttpHelper.IsOnline)
            {
                var cloudNetVersion = WebHelper.GetJsonResponse<MsNetDto>(@"https://builds.dotnet.microsoft.com/dotnet/release-metadata/6.0/releases.json");
                return DotNetHelper.IsInstalled(cloudNetVersion.LatestRelease, DotNetRid.Win_x86)
                        ? throw new DotNetInstalledException(cloudNetVersion.LatestRelease)
                        : false;
            }

            throw new NoInternetConnectionException();
        }

        public static bool _355() => DotNetHelper.IsInstalled("windowsdesktop-runtime-6.*-win-x86.exe", DotNetRid.Win_x86)
                                        ? false
                                        : throw new FileNotExistException("windowsdesktop-runtime-6.*-win-x86.exe");

        public static bool _357()
        {
            if (HttpHelper.IsOnline)
            {
                var cloudNetVersion = WebHelper.GetJsonResponse<MsNetDto>(@"https://builds.dotnet.microsoft.com/dotnet/release-metadata/6.0/releases.json");
                return DotNetHelper.IsInstalled(cloudNetVersion.LatestRelease, DotNetRid.Win_x64)
                        ? throw new DotNetInstalledException(cloudNetVersion.LatestRelease)
                        : false;
            }

            throw new NoInternetConnectionException();
        }

        public static bool _358() => DotNetHelper.IsInstalled("windowsdesktop-runtime-6.*-win-x64.exe", DotNetRid.Win_x86)
                                        ? false
                                        : throw new FileNotExistException("windowsdesktop-runtime-6.*-win-x64.exe");

        public static bool _359()
        {
            var geoId = new RegionInfo(Thread.CurrentThread.CurrentUICulture.Name).GeoId;
            var hasAntizapretPac = RegHelper.GetStringValue(RegistryHive.CurrentUser, _359_INTERNET_SETTINGS_PATH, _359_AUTO_CONFIG_URL) == _359_ANTIZAPRET_PROXY_LINK;

            if (geoId != _359_RUSSIA_GEOID)
                throw new WrongGeoIdException(_359_RUSSIA_GEOID, geoId);

            return hasAntizapretPac;
        }

        public static bool _361() => HttpHelper.IsOnline
                                     ? VisualRedistrLibsHelper.X86IsInstalled()
                                        ? VisualRedistrLibsHelper.GetX86CloudLatestVersion() > VisualRedistrLibsHelper.GetX86InstalledVersion()
                                            ? false
                                            : throw new VisualRedistrLibsLastVersionException()
                                        : false
                                     : throw new NoInternetConnectionException();

        public static bool _362() => VisualRedistrLibsHelper.X86IsInstalled()
                                     ? false
                                     : throw new VisualRedistrLibsNotInstalled();

        public static bool _500() => HttpHelper.IsOnline
                                        ? UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                            ? UwpHelper.PackageExist(_500_UWP_HEVC_VIDEO)
                                            : throw new UwpAppFoundException(UWP_MS_WIN_PHOTOS)
                                     : throw new NoInternetConnectionException();

        public static bool _501() => UwpHelper.PackageExist(UWP_MS_CORTANA)
                                     ? RegHelper.GetNullableByteValue(RegistryHive.ClassesRoot, _501_CORTANA_STARTUP_PATH, _501_CORTANA_STATE) == _501_ENABLED_VALUE
                                     : throw new UwpAppNotFoundException(UWP_MS_CORTANA);

        public static bool _502() => UwpHelper.PackageExist(_502_UWP_MICROSOFT_TEAMS)
                                     ? RegHelper.GetByteValue(RegistryHive.CurrentUser, _502_TEAMS_STARTUP_PATH, STATE) == _502_TEAMS_ENABLED_VALUE
                                     : throw new UwpAppNotFoundException(_502_UWP_MICROSOFT_TEAMS);

        public static bool _504() => HttpHelper.IsOnline
                                     ? File.Exists(_504_MS_STORE_RESET_EXE)
                                        ? false
                                        : throw new FileNotExistException(_504_MS_STORE_RESET_EXE)
                                     : throw new NoInternetConnectionException();

        public static bool _600() => (RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, _600_GAME_DVR_PATH, _600_APP_CAPTURE) == DISABLED_VALUE
                                        && RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, _600_GAME_CONFIG_PATH, _600_GAME_DVR) == DISABLED_VALUE).Invert();

        public static bool _601() => UwpHelper.PackageExist(_601_UWP_XBOX_GAMING_OVERLAY) || UwpHelper.PackageExist(_601_UWP_GAMING_APP)
                                     ? (RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, _601_GAME_BAR_PATH, _601_SHOW_PANEL) == DISABLED_VALUE)
                                                 .Invert()
                                     : throw new UwpAppNotFoundException($"{_601_UWP_XBOX_GAMING_OVERLAY} or {_601_UWP_GAMING_APP}");

        public static bool _602()
        {
            var adapterDAC = WmiHelper.GetVideoControllerDacType();
            var pcIsVM = WmiHelper.IsVirtualMachine();
            var wddmVersion = RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _602_FEATURE_SET_PATH, _602_WDDM_VERSION);

            return adapterDAC != _602_INTERNAL_DAC_TYPE && adapterDAC != null
                    ? pcIsVM != true
                        ? wddmVersion >= _602_WDDM_VERSION_MIN
                            ? RegHelper.GetByteValue(RegistryHive.LocalMachine, _602_GRAPHICS_DRIVERS_PATH, _602_HWSCH_MODE) == _602_ENABLED_VALUE
                            : throw new WddmMinimalVersionException($"{_602_WDDM_VERSION_MIN}", $"{wddmVersion}")
                        : throw new PcIsVirtualMachineException()
                    : throw new AdapterTypeInternalOrNullException($"{adapterDAC}");
        }

        public static bool _700() => ScheduledTaskHelper.Exist(taskPath: SOPHIA_SCHEDULED_PATH, taskName: _700_SOPHIA_CLEANUP_TASK)
                                        && ScheduledTaskHelper.Exist(taskPath: SOPHIA_SCHEDULED_PATH, taskName: _700_SOPHIA_CLEANUP_NOTIFICATION_TASK);

        public static bool _701() => ScheduledTaskHelper.Exist(taskPath: SOPHIA_SCHEDULED_PATH, taskName: _701_SOPHIA_SOFTWARE_DISTRIBUTION_TASK);

        public static bool _702() => ScheduledTaskHelper.Exist(taskPath: SOPHIA_SCHEDULED_PATH, taskName: _702_SOPHIA_CLEAR_TEMP_TASK);

        public static bool _800()
        {
            return WmiHelper.AntiSpywareEnabled()
                   ? RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _800_DEFENDER_NETWORK_PROTECTION_POLICIES_PATH, _800_ENABLE_NETWORK_PROTECTION) == ENABLED_VALUE
                        || RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _800_DEFENDER_NETWORK_PROTECTION_PATH, _800_ENABLE_NETWORK_PROTECTION) == ENABLED_VALUE
                   : throw new MicrosoftDefenderNotRunning();
        }

        public static bool _801()
        {
            return WmiHelper.AntiSpywareEnabled()
                   ? RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _801_WINDOWS_DEFENDER_PATH, _801_PUA_PROTECTION) == ENABLED_VALUE
                   : throw new MicrosoftDefenderNotRunning();
        }

        public static bool _802()
        {
            return WmiHelper.AntiSpywareEnabled() ? ProcessHelper.ProcessExist(_802_DEFENDER_SANDBOX_PROCESS)
                                                || Environment.GetEnvironmentVariable(_802_FORCE_USE_SANDBOX, EnvironmentVariableTarget.Machine) == _802_SANDBOX_ENABLED_VALUE
                                             : throw new MicrosoftDefenderNotRunning();
        }

        public static bool _803() => PowerShellHelper.GetScriptResult<bool>(_803_PROGRAM_AUDIT_ENABLED_PS);

        public static bool _804() => PowerShellHelper.GetScriptResult<bool>(_804_COMMAND_AUDIT_ENABLED_PS);

        public static bool _805() => PowerShellHelper.GetScriptResult<bool>(_805_EVENT_VIEWER_IS_CUSTOM_VIEW_PS);

        public static bool _806() => RegHelper.GetNullableByteValue(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_LOGGING_PATH, _806_ENABLE_MODULE_LOGGING) == ENABLED_VALUE
                                     && RegHelper.GetStringValue(RegistryHive.LocalMachine, _806_POWERSHELL_MODULE_NAMES_PATH, _806_ALL_MODULE) == _806_ALL_MODULE;

        public static bool _807() => RegHelper.GetNullableByteValue(RegistryHive.LocalMachine, _807_POWERSHELL_SCRIPT_BLOCK_LOGGING_PATH, _807_ENABLE_SCRIPT_BLOCK_LOGGING) == ENABLED_VALUE;

        public static bool _808()
        {
            return WmiHelper.AntiSpywareEnabled() ? (RegHelper.GetStringValue(RegistryHive.LocalMachine, CURRENT_VERSION_EXPLORER_PATH, _808_SMART_SCREEN_ENABLED) == _808_SMART_SCREEN_DISABLED_VALUE).Invert()
                                             : throw new MicrosoftDefenderNotRunning();
        }

        public static bool _809() => (RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, _809_CURRENT_POLICIES_ATTACHMENTS_PATH, _809_SAFE_ZONE_INFO) == _809_DISABLED_VALUE).Invert();

        public static bool _810() => (RegHelper.GetNullableByteValue(RegistryHive.CurrentUser, _810_WSH_SETTINGS_PATH, ENABLED) == DISABLED_VALUE).Invert();

        public static bool _811() => OsHelper.IsEdition(WIN_VER_PRO) || OsHelper.IsEdition(WIN_VER_ENT)
                                     ? WmiHelper.ProcessorVirtualizationIsEnabled() || WmiHelper.GetComputerSystemInfo<bool>(_811_HYPERVISOR_PRESENT)
                                        ? File.Exists(_811_WINDOWS_SANDBOX_EXE)
                                        : throw new VitualizationNotSupportedException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _813() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICY_SYSTEM_PATH, ADMIN_PROMPT)
                                              .HasNullOrValue(ADMIN_PROMPT_DEFAULT_VALUE);

        public static bool _814() => _813().Invert();

        public static bool _900() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_PATH);

        public static bool _901() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _901_CAB_COM_PATH);

        public static bool _902() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _902_RUNAS_USER_PATH, _902_EXTENDED).Invert();

        public static bool _903() => (RegHelper.GetStringValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _903_CAST_TO_DEV_GUID) == _903_CAST_TO_DEV_VALUE).Invert();

        public static bool _904() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, _904_CONTEXT_MENU_MODERN_SHARE_PATH, string.Empty) == CONTEXT_MENU_SHARE_GUID;

        public static bool _905() => UwpHelper.PackageExist(_905_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(_905_MS_PAINT_3D);

        public static bool _906() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _906_BMP_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _907() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _907_GIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _908() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _908_JPE_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _909() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _909_JPEG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _910() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _910_JPG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _911() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _911_PNG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _912() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _912_TIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _913() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _913_TIFF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _914() => OsHelper.GetBuild() == WIN_BUILD_19045 && UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _914_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _915() => OsHelper.GetBuild() == WIN_BUILD_19045 && UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _915_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _916() => File.Exists(MS_PAINT_EXE)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _916_IMG_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _917() => (RegHelper.KeyExist(RegistryHive.ClassesRoot, _917_BAT_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, _917_CMD_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY))
                                                   .Invert();

        public static bool _918() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, _918_LIB_LOCATION_PATH, string.Empty) == _918_SHOW_VALUE;

        public static bool _919() => RegHelper.GetStringValue(RegistryHive.ClassesRoot, _919_SEND_TO_PATH, string.Empty) == _919_SHOW_VALUE;

        public static bool _920() => OsHelper.IsEdition(WIN_VER_PRO) || OsHelper.IsEdition(WIN_VER_ENT)
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == DISABLED_VALUE
                                                ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _920_BITLOCKER_BDELEV_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                                : throw new BitlockerIsEnabledException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _921() => File.Exists(MS_PAINT_EXE)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _922() => File.Exists(_922_MS_WORDPAD_EXE)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, DATA)
                                     : throw new WindowsCapabilityNotInstalledException(_922_MS_WORD_PAD);

        public static bool _923() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _923_ZIP_SHELLNEW_PATH);

        public static bool _924() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_VERSION_EXPLORER_PATH, _924_PROMPT_NAME) == _924_PROMPT_VALUE;

        public static bool _925() => RegHelper.KeyExist(RegistryHive.CurrentUser, POLICY_EXPLORER_PATH, _925_NO_USE_NAME).Invert();

        public static bool _926() => UwpHelper.PackageExist(UWP_WINDOWS_TERMINAL)
                                     ? RegHelper.KeyExist(RegistryHive.LocalMachine, POLICY_BLOCKED_PATH, WIN_TERMINAL_ID).Invert()
                                     : throw new UwpAppNotFoundException(UWP_WINDOWS_TERMINAL);

        public static bool _927() => UwpHelper.PackageExist(UWP_WINDOWS_TERMINAL)
                                     ? RegHelper.KeyExist(RegistryHive.LocalMachine, POLICY_BLOCKED_PATH, WIN_TERMINAL_ID)
                                        ? throw new ApplicationBlockedByPolicyException(UWP_WINDOWS_TERMINAL)
                                        : UwpHelper.GetVersion(UWP_WINDOWS_TERMINAL) >= MIN_TERMINAL_SUPPORT_VERSION
                                            && UwpHelper.GetVersion(UWP_WINDOWS_TERMINAL) >= MIN_TERMINAL_SUPPORT_VERSION
                                                ? JsonConvert.DeserializeObject<WinTerminalSettingsDto>(File.ReadAllText(TERMINAL_SETTINGS_JSON_PATH)).Profiles.Defaults.Elevate
                                                : throw new WrongApplicationVersionException(UWP_WINDOWS_TERMINAL, UwpHelper.GetVersion(UWP_WINDOWS_TERMINAL), MIN_TERMINAL_SUPPORT_VERSION)
                                     : throw new UwpAppNotFoundException(UWP_WINDOWS_TERMINAL);

        public static bool _928() => RegHelper.GetStringValue(RegistryHive.CurrentUser, _928_WIN10_CONTEXT_MENU_PATH, null) == string.Empty;

        public static bool _929() => RegHelper.KeyExist(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, CONTEXT_MENU_SHARE_GUID).Invert();

        /// <summary>
        /// There must be a little magic in every app
        /// </summary>

        public static bool ItsMagic() => true;
    }
}
