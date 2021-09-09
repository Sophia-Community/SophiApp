using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Linq;
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

        public static bool _102()
        {
            var allowTelemetry = RegHelper.GetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY) as int?;
            var maxTelemetry = RegHelper.GetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED) as int?;
            var showedToast = RegHelper.GetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL) as int?;
            return (allowTelemetry is Const.DEFAULT_TELEMETRY_VALUE &&
                        maxTelemetry is Const.DEFAULT_TELEMETRY_VALUE &&
                            showedToast is Const.DEFAULT_TELEMETRY_VALUE) ||
                                (allowTelemetry is null ||
                                    maxTelemetry is null ||
                                        showedToast is null);
        }

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
            return queueTask.State.Equals(Const._104_TASK_STATE_READY) &&
                    wer != Const._104_DISABLED_DEFAULT_VALUE &&
                        werService.StartType.Equals(Const._104_WER_SERVICE_MANUAL);
        }

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
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == Const._820_BITLOCKER_PROTECTION_OFF
                                                ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY)
                                                : throw new BitlockerEnabledException()
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

        public static bool _825() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Const._825_SOFTWARE_EXPLORER_PATH, Const._825_NO_USE_NAME);

        /// <summary>
        /// This is a magic method
        /// </summary>
        /// <returns>
        /// A little magic
        /// </returns>
        public static bool ItsMagic() => true;
    }
}