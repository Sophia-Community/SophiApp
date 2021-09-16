using Microsoft.Win32;
using SophiApp.Extensions;
using SophiApp.Helpers;
using System;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using Const = SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public class CustomisationStatus
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.GetService(Const._100_DIAG_TRACK).StartType;
            var firewallRule = FirewallHelper.GetGroupRule(Const._100_DIAG_TRACK).FirstOrDefault();
            return diagTrack is ServiceStartMode.Automatic && firewallRule.Enabled && firewallRule.Action is NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
        }

        public static bool _102() => !_103();

        public static bool _103()
        {
            var allowTelemetry = Convert.ToByte(RegHelper.GetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY));
            var maxTelemetry = Convert.ToByte(RegHelper.GetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED));
            var showedToast = Convert.ToByte(RegHelper.GetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL));
            return (allowTelemetry is Const.MIN_ENT_TELEMETRY_VALUE &&
                        maxTelemetry is Const.MIN_ENT_TELEMETRY_VALUE &&
                            showedToast is Const.MIN_ENT_TELEMETRY_VALUE) ||
                            (allowTelemetry is Const.MIN_TELEMETRY_VALUE &&
                                maxTelemetry is Const.MIN_TELEMETRY_VALUE &&
                                    showedToast is Const.MIN_TELEMETRY_VALUE);
        }

        public static bool _104()
        {
            var queueTask = ScheduledTaskHelper.GetTask(Const._104_QUEUE_TASK);
            var wer = Convert.ToByte(RegHelper.GetValue(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED));
            var werService = ServiceHelper.GetService(Const._104_WER_SERVICE);
            return queueTask.State.Equals(Const.TASK_STATE_READY) &&
                    wer != Const._104_DISABLED_DEFAULT_VALUE &&
                        werService.StartType.Equals(Const._104_WER_SERVICE_MANUAL);
        }

        public static bool _106() => !_107();

        public static bool _107() => RegHelper.GetValue(RegistryHive.CurrentUser, Const.SIUF_PATH, Const.SIUF_PERIOD) as int? == Const.DISABLED_VALUE;

        public static bool _109() => ScheduledTaskHelper.GetTask(Const._109_PRO_DATA_UPD).State == Const.TASK_STATE_READY;

        public static bool _110() => ScheduledTaskHelper.GetTask(Const._110_PROXY).State == Const.TASK_STATE_READY;

        public static bool _111() => ScheduledTaskHelper.GetTask(Const._111_CONS).State == Const.TASK_STATE_READY;

        public static bool _112() => ScheduledTaskHelper.GetTask(Const._112_USB_CEIP).State == Const.TASK_STATE_READY;

        public static bool _113() => ScheduledTaskHelper.GetTask(Const._113_DISK_DATA_COLLECTOR).State == Const.TASK_STATE_READY;

        public static bool _114() => ScheduledTaskHelper.GetTask(Const._114_MAPS_TOAST).State == Const.TASK_STATE_READY;

        public static bool _115() => ScheduledTaskHelper.GetTask(Const._115_MAPS_UPDATE).State == Const.TASK_STATE_READY;

        public static bool _116() => ScheduledTaskHelper.GetTask(Const._116_FAMILY_MONITOR).State == Const.TASK_STATE_READY;

        public static bool _117() => ScheduledTaskHelper.GetTask(Const._117_XBOX_SAVE).State == Const.TASK_STATE_READY;

        public static bool _118() => RegHelper.GetValue(RegistryHive.LocalMachine,
                                                        $"{Const._118_USER_ARSO_PATH}\\{WindowsIdentity.GetCurrent().User.Value}",
                                                        Const._118_OPT_OUT) as int? != Const._118_OPT_OUT_DEFAULT_VALUE;

        public static bool _119() => RegHelper.GetValue(RegistryHive.CurrentUser, Const._119_USER_PROFILE_PATH, Const._119_HTTP_ACCEPT) as int? != Const._119_HTTP_ACCEPT_DEFAULT_VALUE;

        public static bool _120() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const._120_ADVERT_INFO_PATH, Const._120_ADVERT_ENABLED) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _121() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._121_SUB_CONTENT) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _122() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._122_SUB_CONTENT) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _123()
        {
            var content93 = RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_93) as int?;
            var content94 = RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_94) as int?;
            var content96 = RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._123_SUB_CONTENT_96) as int?;
            return content93.HasValueOrNull(Const.ENABLED_VALUE) && content94.HasValueOrNull(Const.ENABLED_VALUE) && content96.HasValueOrNull(Const.ENABLED_VALUE);                                
        }

        public static bool _124() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const.CONTENT_DELIVERY_MANAGER_PATH, Const._124_SILENT_APP_INSTALL) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _125() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const._125_PROFILE_ENGAGE_PATH, Const._125_SETTING_ENABLED) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _126() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const._126_PRIVACY_PATH, Const._126_TAILORED_DATA) as int?)
                                               .HasValueOrNull(Const.ENABLED_VALUE);

        public static bool _127() => (RegHelper.GetValue(RegistryHive.CurrentUser, Const.POLICIES_EXPLORER_PATH, Const._127_DISABLE_SEARCH_SUGGESTIONS) as int?)
                                               .HasValueOrNull(Const.DISABLED_VALUE);

        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._801_CAB_COM_PATH);

        public static bool _802() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED);

        public static bool _803() => !(RegHelper.GetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID) as string == Const._803_CAST_TO_DEV_VALUE);

        public static bool _804() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID);

        public static bool _805() => UwpHelper.PackageExist(Const._805_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(Const._805_MS_PAINT_3D);

        public static bool _806() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _807() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _808() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _809() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _810() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _811() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _812() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _813() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY);

        public static bool _814() => UwpHelper.PackageExist(Const.UWP_MS_WIN_PHOTOS)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(Const.UWP_MS_WIN_PHOTOS)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(Const.CAPABILITY_MS_PAINT)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new WindowsCapabilityNotInstalledException(Const.CAPABILITY_MS_PAINT);

        public static bool _817() => !(RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY));

        public static bool _818() => RegHelper.GetValue(RegistryHive.ClassesRoot, Const._818_LIB_LOCATION_PATH, string.Empty) as string == Const._818_SHOW_VALUE;

        public static bool _819() => RegHelper.GetValue(RegistryHive.ClassesRoot, Const._819_SEND_TO_PATH, string.Empty) as string == Const._819_SHOW_VALUE;

        public static bool _820() => OsHelper.IsEdition(Const.WIN_VER_PRO) || OsHelper.IsEdition(Const.WIN_VER_ENT)
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == Const.DISABLED_VALUE
                                                ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY)
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

        public static bool _824() => Convert.ToInt32(RegHelper.GetValue(RegistryHive.CurrentUser, Const._824_CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME)) == Const._824_PROMPT_VALUE;

        public static bool _825() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Const.POLICIES_EXPLORER_PATH, Const._825_NO_USE_NAME);

        /// <summary>
        /// A bit of magic
        /// </summary>

        public static bool ItsMagic() => false;
    }
}