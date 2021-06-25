using Microsoft.Win32;
using SophiApp.Helpers;

namespace SophiApp.Actions
{
    //TODO: Implement method selection by ID

    public class CurrentStateAction
    {
        public static bool _265() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._265_EXTRACT_PATH) is null);

        public static bool _267() => RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._267_RUNASUSER_PATH).GetValue(RegPaths._267_EXTENDED_NAME) is null;

        public static bool _268() => RegHelper.GetRegistryKey(RegistryHive.LocalMachine, RegPaths._268_CAST_TO_DEVICE_PATH)
                                              .GetValue(RegPaths._268_CAST_TO_DEVICE_NAME) as string == RegPaths._268_CAST_TO_DEVICE_VALUE;

        public static bool _269() => RegHelper.GetRegistryKey(RegistryHive.LocalMachine, RegPaths._269_SHARE_CONTENT_PATH)
                                              .GetValue(RegPaths._269_SHARE_CONTENT_NAME) is string;

        public static bool _271() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._271_3D_EDIT_BMP_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _272() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._272_3D_EDIT_GIF_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _273() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._273_3D_EDIT_JPE_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _274() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._274_3D_EDIT_JPEG_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _275() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._275_3D_EDIT_JPG_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _276() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._276_3D_EDIT_PNG_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _277() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._277_3D_EDIT_TIF_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _278() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._278_3D_EDIT_TIFF_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _279() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._279_SHELL_EDIT_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _280() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._280_SHELL_CREATE_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _281() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._281_SHELL_EDIT_PATH)
                                                .GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        //TODO: Add Get-AppxPackage -Name Microsoft.Windows.Photos check !
        //TODO: Add Get-AppxPackage -Name Microsoft.Windows.Photos check !
        public static bool _282()
        {
            var batFile = !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_BAT_PRINT_PATH).GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);
            var cmdFile = !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._282_CMD_PRINT_PATH).GetValue(RegPaths.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);
            return batFile && cmdFile;
        }

        public static bool _283() => RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, RegPaths._283_LIBRARY_LOCATION_PATH)
                                              .GetValue(string.Empty) as string == RegPaths._283_LIBRARY_LOCATION_VALUE;

        public static bool FOR_DEBUG_ONLY() => false; //TODO: CurrentStateAction - This method for debug only.
    }
}