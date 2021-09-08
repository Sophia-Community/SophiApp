using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using Const = SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public class CustomisationStatus
    {
        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._801_CAB_COMMAND);

        public static bool _802() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._802_RUNAS_USER, Const._802_EXTENDED);

        public static bool _803() => !(RegHelper.GetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED, Const._803_CAST_TO_DEV_GUID) as string == Const._803_CAST_TO_DEV_VALUE);

        public static bool _804() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED, Const._804_SHARE_GUID);

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
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(Const.UWP_MS_WIN_PHOTOS)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Const.UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(Const.CAPABILITY_MS_PAINT)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT, Const.PROGRAM_ACCESS_ONLY)
                                     : throw new WindowsCapabilityNotInstalledException(Const.CAPABILITY_MS_PAINT);

        public static bool _817() => !(RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT, Const.PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT, Const.PROGRAM_ACCESS_ONLY));

        public static bool _818() => RegHelper.GetValue(RegistryHive.ClassesRoot, Const._818_LIB_LOCATION, string.Empty) as string == Const._818_SHOW_VALUE;

        public static bool _819() => RegHelper.GetValue(RegistryHive.ClassesRoot, Const._819_SEND_TO, string.Empty) as string == Const._819_SHOW_VALUE;

        public static bool _820() => OsHelper.IsEdition(Const._820_WIN_VER_PRO) || OsHelper.IsEdition(Const._820_WIN_VER_ENT)
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == 0
                                                ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDE_ELEV, Const.PROGRAM_ACCESS_ONLY)
                                                : throw new BitlockerEnabledException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _821() => DismHelper.CapabilityIsInstalled(Const.CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(Const.CAPABILITY_MS_PAINT);

        public static bool _822() => DismHelper.CapabilityIsInstalled(Const._822_MS_WORD_PAD)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.DATA_NAME)
                                     : throw new WindowsCapabilityNotInstalledException(Const._822_MS_WORD_PAD);

        public static bool _823() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Const._823_ZIP_SHELL_NEW);

        public static bool _824() => Convert.ToInt32(RegHelper.GetValue(RegistryHive.CurrentUser, Const._824_CURRENT_EXPLORER, Const._824_PROMPT_NAME)) == Const._824_PROMPT_VALUE;

        public static bool _825() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Const._825_SOFTWARE_EXPLORER, Const._825_NO_USE_NAME);

        //TODO: CustomisationState - Method placeholder.
        public static bool FOR_DEBUG_ONLY() => new Random().Next(101) <= 50;
    }
}