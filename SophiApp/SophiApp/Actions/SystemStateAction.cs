using Microsoft.Win32;
using SophiApp.Helpers;

namespace SophiApp.Actions
{
    //TODO: Implement system state methods

    public class SystemStateAction
    {
        public static void _265(bool IsActive)
        {
            if (IsActive)
            {
                string MUIVERB_NAME = "MUIVerb";
                string MUIVERB_VALUE = "@shell32.dll,-37514";
                string ICON_NAME = "Icon";
                string ICON_VALUE = "shell32.dll,-16817";
                string COMMAND_NAME = "Command";
                string EXTRACT_VALUE = "msiexec.exe /a \"%1\" /qb TARGETDIR=\"%1 extracted\"";
                string COMMAND_PATH = $"{RegPaths._265_EXTRACT_PATH}\\{COMMAND_NAME}";

                Registry.ClassesRoot.CreateSubKey(RegPaths._265_EXTRACT_PATH, true).CreateSubKey(COMMAND_NAME);
                Registry.ClassesRoot.OpenSubKey(COMMAND_PATH, true).SetValue(string.Empty, EXTRACT_VALUE);
                Registry.ClassesRoot.OpenSubKey(RegPaths._265_EXTRACT_PATH, true).SetValue(MUIVERB_NAME, MUIVERB_VALUE);
                Registry.ClassesRoot.OpenSubKey(RegPaths._265_EXTRACT_PATH, true).SetValue(ICON_NAME, ICON_VALUE);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(RegPaths._265_EXTRACT_PATH);
        }

        public static void _267(bool IsActive)
        {
            if (IsActive)
            {
                var key = Registry.ClassesRoot.OpenSubKey(RegPaths._267_RUNASUSER_PATH, true);
                key.DeleteValue(RegPaths._267_EXTENDED_NAME);
                return;
            }

            Registry.ClassesRoot.CreateSubKey(RegPaths._267_RUNASUSER_PATH, true).SetValue(RegPaths._267_EXTENDED_NAME, string.Empty);
        }

        public static void _268(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.LocalMachine, RegPaths._268_CAST_TO_DEVICE_PATH).SetValue(RegPaths._268_CAST_TO_DEVICE_NAME, RegPaths._268_CAST_TO_DEVICE_VALUE);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.LocalMachine, RegPaths._268_CAST_TO_DEVICE_PATH).DeleteValue(RegPaths._268_CAST_TO_DEVICE_NAME);
        }

        public static void _269(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.LocalMachine, RegPaths._269_SHARE_CONTENT_PATH).SetValue(RegPaths._269_SHARE_CONTENT_NAME, string.Empty);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.LocalMachine, RegPaths._269_SHARE_CONTENT_PATH).DeleteValue(RegPaths._269_SHARE_CONTENT_NAME);
        }

        public static void _271(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._271_3D_EDIT_BMP_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._271_3D_EDIT_BMP_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _272(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._272_3D_EDIT_GIF_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._272_3D_EDIT_GIF_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _273(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._273_3D_EDIT_JPE_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._273_3D_EDIT_JPE_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _274(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._274_3D_EDIT_JPEG_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._274_3D_EDIT_JPEG_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _275(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._275_3D_EDIT_JPG_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._275_3D_EDIT_JPG_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _276(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._276_3D_EDIT_PNG_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._276_3D_EDIT_PNG_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _277(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._277_3D_EDIT_TIF_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._277_3D_EDIT_TIF_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _278(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._278_3D_EDIT_TIFF_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._278_3D_EDIT_TIFF_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _279(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._279_SHELL_EDIT_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._279_SHELL_EDIT_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _280(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._280_SHELL_CREATE_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._280_SHELL_CREATE_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _281(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._281_SHELL_EDIT_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._281_SHELL_EDIT_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _282(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_BAT_PRINT_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_CMD_PRINT_PATH).DeleteValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_BAT_PRINT_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_CMD_PRINT_PATH).SetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _283(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._283_LIBRARY_LOCATION_PATH).SetValue(string.Empty, RegPaths._283_LIBRARY_LOCATION_VALUE);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, RegPaths._283_LIBRARY_LOCATION_PATH).SetValue(string.Empty, $"-{RegPaths._283_LIBRARY_LOCATION_VALUE}");
        }

        public static void FOR_DEBUG_ONLY(bool state)
        {
        }
    }
}