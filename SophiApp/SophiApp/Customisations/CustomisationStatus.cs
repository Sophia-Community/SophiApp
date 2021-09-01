using Microsoft.Win32;
using SophiApp.Helpers;
using System;

namespace SophiApp.Customisations
{
    public class CustomisationStatus
    {
        private const string _800_MSI_EXTRACT = @"Msi.Package\shell\Extract";
        private const string _801_CAB_INSTALL = @"CABFolder\Shell\RunAs\Command";
        private const string _802_EXTENDED = @"Extended";
        private const string _802_RUNAS_USER = @"exefile\shell\runasuser";
        private const string _803_CAST_TO_DEV_GUID = @"{7AD84985-87B4-4a16-BE58-8B72A5B390F7}";
        private const string _803_CAST_TO_DEV_VALUE = "Play to menu";
        private const string _804_SHARE_GUID = @"{E2BF9676-5F8F-435C-97EB-11607A5BEDF7}";
        private const string _805_MS_PAINT_3D = "Microsoft.MSPaint";
        private const string _806_BMP_EXT = @"SystemFileAssociations\.bmp\Shell\3D Edit";
        private const string _807_GIF_EXT = @"SystemFileAssociations\.gif\Shell\3D Edit";
        private const string _808_JPE_EXT = @"SystemFileAssociations\.jpe\Shell\3D Edit";
        private const string _809_JPEG_EXT = @"SystemFileAssociations\.jpeg\Shell\3D Edit";
        private const string _810_JPG_EXT = @"SystemFileAssociations\.jpg\Shell\3D Edit";
        private const string _811_PNG_EXT = @"SystemFileAssociations\.png\Shell\3D Edit";
        private const string _812_TIF_EXT = @"SystemFileAssociations\.tif\Shell\3D Edit";
        private const string _813_TIFF_EXT = @"SystemFileAssociations\.tiff\Shell\3D Edit";
        private const string _814_PHOTOS_SHELL_EDIT = @"AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\Shell\ShellEdit";
        private const string _815_PHOTOS_SHELL_VIDEO = @"AppX43hnxtbyyps62jhe9sqpdzxn1790zetc\Shell\ShellCreateVideo";
        private const string _816_IMG_SHELL_EDIT = @"SystemFileAssociations\image\shell\edit";
        private const string _817_BAT_SHELL_EDIT = @"batfile\shell\print";
        private const string _817_CMD_SHELL_EDIT = @"cmdfile\shell\print";
        private const string _818_DEFAULT_VALUE = "{3dad6c5d-2167-4cae-9914-f99e41c12cfa}";
        private const string _818_LIB_LOCATION = @"Folder\ShellEx\ContextMenuHandlers\Library Location";
        private const string _819_DEFAULT_VALUE = "{7BA4C740-9E81-11CF-99D3-00AA004AE837}";
        private const string _819_SEND_TO = @"AllFilesystemObjects\shellex\ContextMenuHandlers\SendTo";
        private const string _820_BITLOCKER_BDE_ELEV = @"Drive\shell\encrypt-bde-elev";
        private const string _820_WIN_VER_ENT = "Enterprise";
        private const string _820_WIN_VER_PRO = "Professional";
        private const string _821_BMP_ITEM_NAME = "ItemName";
        private const string _821_BMP_NULL_FILE = "NullFile";
        private const string _821_BMP_SHELL_NEW = @".bmp\ShellNew";
        private const string _822_MS_WORD_PAD = "Microsoft.Windows.WordPad";
        private const string _822_RTF_DATA = "Data";
        private const string _822_RTF_ITEM_NAME = "ItemName";
        private const string _822_RTF_SHELL_NEW = @".rtf\ShellNew";
        private const string _823_ZIP_SHELL_NEW = @".zip\CompressedFolder\ShellNew";
        private const string _824_CURRENT_EXPLORER = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
        private const string _824_PROMPT_MIN = "MultipleInvokePromptMinimum";
        private const int _824_PROMPT_MIN_VALUE = 300;
        private const string _825_NO_USE_STORE = "NoUseStoreOpenWith";
        private const string _825_SOFTWARE_EXPLORER = @"SOFTWARE\Policies\Microsoft\Windows\Explorer";
        private const string CAPABILITY_MS_PAINT = "Microsoft.Windows.MSPaint";
        private const string PROGRAM_ACCESS_ONLY = "ProgrammaticAccessOnly";
        private const string SHELL_EXT_BLOCKED = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        private const string UWP_MS_WIN_PHOTOS = "Microsoft.Windows.Photos";

        public static bool _800() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _800_MSI_EXTRACT);

        public static bool _801() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _801_CAB_INSTALL);

        public static bool _802() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _802_RUNAS_USER, _802_EXTENDED);

        public static bool _803() => !(RegHelper.GetValue(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED, _803_CAST_TO_DEV_GUID) as string == _803_CAST_TO_DEV_VALUE);

        public static bool _804() => !RegHelper.KeyExist(RegistryHive.LocalMachine, SHELL_EXT_BLOCKED, _804_SHARE_GUID);

        public static bool _805() => UwpHelper.PackageExist(_805_MS_PAINT_3D) ? true : throw new UwpAppNotFoundException(_805_MS_PAINT_3D);

        public static bool _806() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _806_BMP_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _807() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _807_GIF_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _808() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _808_JPE_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _809() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _809_JPEG_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _810() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _810_JPG_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _811() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _811_PNG_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _812() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _812_TIF_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _813() => !RegHelper.KeyExist(RegistryHive.ClassesRoot, _813_TIFF_EXT, PROGRAM_ACCESS_ONLY);

        public static bool _814() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                             ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, _814_PHOTOS_SHELL_EDIT, PROGRAM_ACCESS_ONLY)
                                             : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _815() => UwpHelper.PackageExist(UWP_MS_WIN_PHOTOS)
                                             ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, _815_PHOTOS_SHELL_VIDEO, PROGRAM_ACCESS_ONLY)
                                             : throw new UwpAppNotFoundException(UWP_MS_WIN_PHOTOS);

        public static bool _816() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
                                             ? !RegHelper.KeyExist(RegistryHive.ClassesRoot, _816_IMG_SHELL_EDIT, PROGRAM_ACCESS_ONLY)
                                             : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _817() => !(RegHelper.KeyExist(RegistryHive.ClassesRoot, _817_BAT_SHELL_EDIT, PROGRAM_ACCESS_ONLY)
                                                || RegHelper.KeyExist(RegistryHive.ClassesRoot, _817_CMD_SHELL_EDIT, PROGRAM_ACCESS_ONLY));

        public static bool _818() => RegHelper.GetValue(RegistryHive.ClassesRoot, _818_LIB_LOCATION, string.Empty) as string == _818_DEFAULT_VALUE;

        public static bool _819() => RegHelper.GetValue(RegistryHive.ClassesRoot, _819_SEND_TO, string.Empty) as string == _819_DEFAULT_VALUE;

        public static bool _820() => OsHelper.IsEdition(_820_WIN_VER_PRO) || OsHelper.IsEdition(_820_WIN_VER_ENT)
                                             ? WmiHelper.GetBitLockerVolumeProtectionStatus() == 0
                                                        ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, _820_BITLOCKER_BDE_ELEV, PROGRAM_ACCESS_ONLY)
                                                        : throw new BitlockerEnabledException()
                                             : throw new WindowsEditionNotSupportedException();

        public static bool _821() => DismHelper.CapabilityIsInstalled(CAPABILITY_MS_PAINT)
                                             ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_ITEM_NAME)
                                                && RegHelper.KeyExist(RegistryHive.ClassesRoot, _821_BMP_SHELL_NEW, _821_BMP_NULL_FILE)
                                             : throw new WindowsCapabilityNotInstalledException(CAPABILITY_MS_PAINT);

        public static bool _822() => DismHelper.CapabilityIsInstalled(_822_MS_WORD_PAD)
                                             ? RegHelper.KeyExist(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, _822_RTF_ITEM_NAME)
                                                && RegHelper.KeyExist(RegistryHive.ClassesRoot, _822_RTF_SHELL_NEW, _822_RTF_DATA)
                                             : throw new WindowsCapabilityNotInstalledException(_822_MS_WORD_PAD);

        public static bool _823() => RegHelper.SubKeyExist(RegistryHive.ClassesRoot, _823_ZIP_SHELL_NEW);

        public static bool _824() => !(Convert.ToInt32(RegHelper.GetValue(RegistryHive.CurrentUser, _824_CURRENT_EXPLORER, _824_PROMPT_MIN)) == _824_PROMPT_MIN_VALUE);

        public static bool _825() => !RegHelper.KeyExist(RegistryHive.LocalMachine, _825_SOFTWARE_EXPLORER, _825_NO_USE_STORE);

        //TODO: CustomisationState - Method placeholder.
        public static bool FOR_DEBUG_ONLY() => new Random().Next(101) <= 50;
    }
}