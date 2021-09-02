using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using Var = SophiApp.Customisations.CustomisationVars;

namespace SophiApp.Customisations
{
    public class CustomisationStatus
    {
        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Var._800_MSI_EXTRACT);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Var._801_CAB_COMMAND);

        public static bool _802() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._802_RUNAS_USER, Var._802_EXTENDED);

        public static bool _803() => !(RegHelper.GetValue(RegistryHive.LocalMachine, Var.SHELL_EXT_BLOCKED, Var._803_CAST_TO_DEV_GUID) as string == Var._803_CAST_TO_DEV_VALUE);

        public static bool _804() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Var.SHELL_EXT_BLOCKED, Var._804_SHARE_GUID);

        public static bool _805() => UwpHelper.PackageExist(Var._805_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(Var._805_MS_PAINT_3D);

        public static bool _806() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._806_BMP_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _807() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._807_GIF_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _808() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._808_JPE_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _809() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._809_JPEG_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _810() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._810_JPG_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _811() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._811_PNG_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _812() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._812_TIF_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _813() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._813_TIFF_EXT, Var.PROGRAM_ACCESS_ONLY);

        public static bool _814() => UwpHelper.PackageExist(Var.UWP_MS_WIN_PHOTOS)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._814_PHOTOS_SHELL_EDIT, Var.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Var.UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(Var.UWP_MS_WIN_PHOTOS)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._815_PHOTOS_SHELL_VIDEO, Var.PROGRAM_ACCESS_ONLY)
                                     : throw new UwpAppNotFoundException(Var.UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(Var.CAPABILITY_MS_PAINT)
                                     ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._816_IMG_SHELL_EDIT, Var.PROGRAM_ACCESS_ONLY)
                                     : throw new WindowsCapabilityNotInstalledException(Var.CAPABILITY_MS_PAINT);

        public static bool _817() => !(RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._817_BAT_SHELL_EDIT, Var.PROGRAM_ACCESS_ONLY)
                                     || RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._817_CMD_SHELL_EDIT, Var.PROGRAM_ACCESS_ONLY));

        public static bool _818() => RegHelper.GetValue(RegistryHive.ClassesRoot, Var._818_LIB_LOCATION, string.Empty) as string == Var._818_SHOW_VALUE;

        public static bool _819() => RegHelper.GetValue(RegistryHive.ClassesRoot, Var._819_SEND_TO, string.Empty) as string == Var._819_SHOW_VALUE;

        public static bool _820() => OsHelper.IsEdition(Var._820_WIN_VER_PRO) || OsHelper.IsEdition(Var._820_WIN_VER_ENT)   
                                     ? WmiHelper.GetBitLockerVolumeProtectionStatus() == 0
                                                ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._820_BITLOCKER_BDE_ELEV, Var.PROGRAM_ACCESS_ONLY)
                                                : throw new BitlockerEnabledException()
                                     : throw new WindowsEditionNotSupportedException();

        public static bool _821() => DismHelper.CapabilityIsInstalled(Var.CAPABILITY_MS_PAINT)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._821_BMP_SHELL_NEW, Var._821_BMP_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._821_BMP_SHELL_NEW, Var._821_BMP_NULL_FILE)
                                     : throw new WindowsCapabilityNotInstalledException(Var.CAPABILITY_MS_PAINT);

        public static bool _822() => DismHelper.CapabilityIsInstalled(Var._822_MS_WORD_PAD)
                                     ? RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._822_RTF_SHELL_NEW, Var._822_RTF_ITEM_NAME)
                                        && RegHelper.KeyExist(RegistryHive.ClassesRoot, Var._822_RTF_SHELL_NEW, Var._822_RTF_DATA)
                                     : throw new WindowsCapabilityNotInstalledException(Var._822_MS_WORD_PAD);

        public static bool _823() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, Var._823_ZIP_SHELL_NEW);

        public static bool _824() => Convert.ToInt32(RegHelper.GetValue(RegistryHive.CurrentUser, Var._824_CURRENT_EXPLORER, Var._824_PROMPT_MIN)) == Var._824_PROMPT_MIN_VALUE;

        public static bool _825() => !RegHelper.KeyExist(RegistryHive.LocalMachine, Var._825_SOFTWARE_EXPLORER, Var._825_NO_USE_STORE);

        //TODO: CustomisationState - Method placeholder.
        public static bool FOR_DEBUG_ONLY() => new Random().Next(101) <= 50;
    }
}