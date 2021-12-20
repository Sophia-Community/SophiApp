using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SophiApp.Dto;
using SophiApp.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Principal;
using System.ServiceProcess;
using static SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public static class CustomisationStatus
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

        public static bool _210() => UwpHelper.PackageExist(UWP_MS_CORTANA)
                                              ? RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _210_CORTANA_BUTTON)
                                                         .HasNullOrValue(ENABLED_VALUE)
                                              : throw new UwpAppNotFoundException(UWP_MS_CORTANA);

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

        public static bool _243() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, WINLOGON_PATH, _243_FIRST_LOGON_ANIMATION)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _245() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTROL_PANEL_DESKTOP_PATH, JPEG_QUALITY)
                                              .HasValue(_245_JPEG_MAX_QUALITY);

        public static bool _246() => _245().Invert();

        public static bool _247() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _247_WINDOWS_UPDATE_SETTINGS_PATH, _247_RESTART_NOTIFICATIONS)
                                              .HasNullOrValue(_247_HIDE_VALUE).Invert();

        public static bool _248()
        {
            RegHelper.TryDeleteValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _248_LINK);
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

        public static bool _316() => DomainHelper.PcInDomain() ? RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _316_WINLOGON_PATH, _316_FOREGROUND_POLICY)
                                                                          .HasNullOrValue(DISABLED_VALUE).Invert()
                                                               : throw new PcNotJoinedToDomainException();

        public static bool _317() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _317_CURRENT_VERSION_WINDOWS_PATH, _317_PRINTER_LEGACY_MODE)
                                              .HasNullOrValue(_317_ENABLED_VALUE);

        public static bool _319() => DismHelper.FeatureIsInstalled(_319_LEGACY_COMPONENTS_FEATURE);

        public static bool _320() => DismHelper.FeatureIsInstalled(_320_POWERSHELL_V2_FEATURE);

        public static bool _321() => DismHelper.FeatureIsInstalled(_321_POWERSHELL_V2_ROOT_FEATURE);

        public static bool _322() => DismHelper.FeatureIsInstalled(_322_XPS_SERVICES_FEATURE);

        public static bool _323() => DismHelper.FeatureIsInstalled(_323_WORK_FOLDERS_FEATURE);

        public static bool _324() => DismHelper.FeatureIsInstalled(_324_MEDIA_PLAYBACK_FEATURE);

        public static bool _326() => DismHelper.CapabilityIsInstalled(_326_STEPS_RECORDER_CAPABILITY);

        public static bool _327() => DismHelper.CapabilityIsInstalled(_327_QUICK_SUPPORT_CAPABILITY);

        public static bool _328() => DismHelper.CapabilityIsInstalled(_328_MS_PAINT_CAPABILITY);

        public static bool _329() => DismHelper.CapabilityIsInstalled(_329_MS_WORDPAD_CAPABILITY);

        public static bool _330() => DismHelper.CapabilityIsInstalled(_330_INTERNET_EXPLORER_CAPABILITY);

        public static bool _331() => DismHelper.CapabilityIsInstalled(_331_MATH_RECOGNIZER_CAPABILITY);

        public static bool _332() => DismHelper.CapabilityIsInstalled(_332_MEDIA_PLAYER_CAPABILITY);

        public static bool _333() => DismHelper.CapabilityIsInstalled(_333_OPENSSH_CLIENT_CAPABILITY);

        public static bool _334()
        {
            var result = false;
            var updateManager = ComObjectHelper.CreateFromProgID(_334_UPDATE_SERVICE_MANAGER);

            foreach (var service in updateManager.Services)
            {
                if (service.ServiceID == _334_SERVICE_MANAGER_GUID)
                {
                    result = service.IsDefaultAUService;
                    break;
                }
            }

            return result;
        }

        public static bool _336() => WmiHelper.GetActivePowerPlanId() == _336_HIGH_POWER_GUID;

        public static bool _337() => _336().Invert();

        public static bool _338() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _338_NET_FRAMEWORK64_PATH, _338_USE_LATEST_CLR) == ENABLED_VALUE
                                     || RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _338_NET_FRAMEWORK32_PATH, _338_USE_LATEST_CLR) == ENABLED_VALUE;

        public static bool _339() => WmiHelper.HasNetworkAdaptersPowerSave();

        public static bool _340() => PowerShell.Create().AddScript(_340_GET_IPV6_PS).Invoke().Count > 0;

        public static bool _342() => _343().Invert();

        public static bool _343() => RegHelper.GetStringValue(RegistryHive.CurrentUser, CONTROL_PANEL_USER_PROFILE_PATH, INPUT_METHOD_OVERRIDE) == INPUT_ENG_VALUE;

        public static bool _344() => OneDriveHelper.IsInstalled() ? throw new OneDriveIsInstalledException() : true;

        public static bool _345() => RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, IMAGES_FOLDER)
                                        == RegHelper.GetStringValue(RegistryHive.CurrentUser, USER_SHELL_FOLDERS_PATH, _345_DESKTOP_FOLDER);

        public static bool _346() => _345().Invert();

        public static bool _348() => _349().Invert();

        public static bool _349() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, WINDOWS_MITIGATION_PATH, MITIGATION_USER_PREFERENCE)
                                              .HasNullOrValue(_349_DEFAULT_VALUE);

        public static bool _350() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, ADVANCED_EXPLORER_PATH, _350_SEPARATE_PROCESS)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _351() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _351_RESERVE_MANAGER_PATH, _351_SHIPPED_RESERVES)
                                              .HasNullOrValue(ENABLED_VALUE);

        public static bool _352() => RegHelper.SubKeyExist(RegistryHive.CurrentUser, _352_TYPELIB_PATH).Invert();

        public static bool _353() => RegHelper.GetStringValue(RegistryHive.Users, _353_DEFAULT_KEYBOARD_PATH, _353_INITIAL_INDICATORS) == _353_ENABLED_VALUE;

        public static bool _354() => RegHelper.KeyExist(RegistryHive.LocalMachine, _354_KEYBOARD_LAYOUT_PATH, _354_SCAN_CODE).Invert();

        public static bool _355() => RegHelper.GetStringValue(RegistryHive.CurrentUser, _355_STICKY_KEYS_PATH, _355_FLAGS)
                                              .HasNullOrValue(_355_ENABLED_VALUE);

        public static bool _356() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, _356_AUTOPLAY_HANDLERS_PATH, _356_AUTOPLAY)
                                              .HasNullOrValue(_356_ENABLED_VALUE);

        public static bool _357() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, _357_THUMBNAIL_CACHE_PATH, _357_AUTOPLAY)
                                              .HasNullOrValue(_357_ENABLED_VALUE);

        public static bool _358() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, WINLOGON_PATH, _358_RESTART_APPS)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _359() => DomainHelper.PcInDomain().Invert()
                                     ? FirewallHelper.IsRuleGroupEnabled(_359_FILE_PRINTER_SHARING_GROUP) && FirewallHelper.IsRuleGroupEnabled(_359_NETWORK_DISCOVERY_GROUP)
                                     : throw new PcJoinedToDomainException();

        public static bool _360() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, UPDATE_UX_SETTINGS_PATH, _360_ACTIVE_HOURS)
                                              .HasNullOrValue(_360_AUTO_STATE);

        public static bool _361() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, UPDATE_UX_SETTINGS_PATH, _361_IS_EXPEDITED)
                                              .HasNullOrValue(DISABLED_VALUE).Invert();

        public static bool _362() => MsiHelper.GetProperties(Directory.GetFiles(_362_INSTALLER_PATH, _362_MSI_MASK))
                                              .FirstOrDefault(property => property[_362_PRODUCT_NAME] == _362_PC_HEALTH_CHECK) != null
                                                                                                                                ? false
                                                                                                                                : throw new UpdateNotInstalledException(KB5005463_UPD);

        public static bool _363()
        {
            var vcVersions = WebHelper.GetJsonResponse(_363_VC_VERSION_URL, new VCRedistrDto());
            var latestVersion = vcVersions.Supported.Where(item => item.Name == _363_VC_REDISTR_FOR_VS_2022 && item.Architecture == X64).Select(item => item.Version).First();
            var registryVersionPath = $@"Installer\Dependencies\VC,redist.x64,amd64,{latestVersion.Major}.{latestVersion.Minor},bundle";
            return RegHelper.GetStringValue(RegistryHive.ClassesRoot, registryVersionPath, "Version") != null;
        }

        public static bool _365() => OneDriveHelper.IsInstalled() ? throw new OneDriveIsInstalledException() : false;

        public static bool _366() => OneDriveHelper.IsInstalled() ? false : throw new OneDriveNotInstalledException();

        public static bool _400() => RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _400_HIDE_ADDED_APPS) != _400_DISABLED_VALUE;

        public static bool _401() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CONTENT_DELIVERY_MANAGER_PATH, _401_APP_SUGGESTIONS) == ENABLED_VALUE;

        public static bool _402() => (File.ReadAllBytes(_402_POWERSHELL_LNK)[0x15] == 2).Invert();

        public static bool _500() => UwpHelper.PackageExist(_500_UWP_HEVC_VIDEO).Invert() && UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? false
                                     : throw new UwpAppFoundException(_500_UWP_HEVC_VIDEO);

        public static bool _501() => UwpHelper.PackageExist(UWP_MS_CORTANA)
                                     ? RegHelper.GetByteValue(RegistryHive.ClassesRoot, _501_CORTANA_STARTUP_PATH, _501_CORTANA_STATE) == _501_ENABLED_VALUE
                                     : throw new UwpAppNotFoundException(UWP_MS_CORTANA);

        public static bool _600() => RegHelper.GetByteValue(RegistryHive.CurrentUser, _600_GAME_DVR_PATH, _600_APP_CAPTURE) == ENABLED_VALUE
                                     && RegHelper.GetByteValue(RegistryHive.CurrentUser, _600_GAME_CONFIG_PATH, _600_GAME_DVR) == ENABLED_VALUE;

        public static bool _601() => UwpHelper.PackageExist(XBOX_GAMING_OVERLAY_UWP) || UwpHelper.PackageExist(GAMING_APP_UWP)
                                     ? RegHelper.GetByteValue(RegistryHive.CurrentUser, _601_GAME_BAR_PATH, _601_SHOW_PANEL) == ENABLED_VALUE
                                     : throw new UwpAppNotFoundException($"{XBOX_GAMING_OVERLAY_UWP} or {GAMING_APP_UWP}");

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

        public static bool _700() => ScheduledTaskHelper.Exist(taskPath: SOPHIA_SCRIPT_SCHEDULED_PATH, taskName: _700_SOPHIA_CLEANUP_TASK)
                                        || ScheduledTaskHelper.Exist(taskPath: SOPHIA_APP_SCHEDULED_PATH, taskName: _700_SOPHIA_CLEANUP_TASK);

        public static bool _900() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _900_MSI_EXTRACT_PATH);

        public static bool _901() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _901_CAB_COM_PATH);

        public static bool _902() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _902_RUNAS_USER_PATH, _902_EXTENDED).Invert();

        public static bool _903() => (RegHelper.GetStringValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _903_CAST_TO_DEV_GUID) == _903_CAST_TO_DEV_VALUE)
                                               .Invert();

        public static bool _904() => RegHelper.KeyExist(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED_PATH, _904_SHARE_GUID).Invert();

        public static bool _905() => UwpHelper.PackageExist(_905_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(_905_MS_PAINT_3D);

        public static bool _906() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _906_BMP_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _907() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _907_GIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _908() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _908_JPE_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _909() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _909_JPEG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _910() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _910_JPG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _911() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _911_PNG_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _912() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _912_TIF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _913() => RegHelper.KeyExist(RegistryHive.ClassesRoot, _913_TIFF_EXT, PROGRAM_ACCESS_ONLY).Invert();

        public static bool _914() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _914_PHOTOS_SHELL_EDIT_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _915() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _915_PHOTOS_SHELL_VIDEO_PATH, PROGRAM_ACCESS_ONLY).Invert()
                                     : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _916() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
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

        public static bool _921() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
                                                                                                                                                                                                             ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _921_BMP_SHELL_NEW, _921_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _922() => DismHelper.CapabilityIsInstalled(_922_MS_WORD_PAD)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, _922_RTF_SHELL_NEW, DATA)
                                     : throw new WindowsCapabilityNotInstalledException(_922_MS_WORD_PAD);

        public static bool _923() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _923_ZIP_SHELLNEW_PATH);

        public static bool _924() => RegHelper.GetNullableIntValue(RegistryHive.CurrentUser, CURRENT_EXPLORER_PATH, _924_PROMPT_NAME)
                                              .HasNullOrValue(_924_PROMPT_VALUE);

        public static bool _925() => RegHelper.KeyExist(RegistryHive.LocalMachine, POLICIES_EXPLORER_PATH, _925_NO_USE_NAME).Invert();

        /// <summary>
        /// There must be a little magic in every app
        /// </summary>

        public static bool ItsMagic() => true;
    }
}