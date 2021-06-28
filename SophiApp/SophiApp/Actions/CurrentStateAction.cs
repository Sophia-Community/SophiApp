using Microsoft.Win32;
using NetFwTypeLib;
using SophiApp.Helpers;
using System;
using System.Linq;
using System.ServiceProcess;

namespace SophiApp.Actions
{
    //TODO: Implement method selection by ID

    public class CurrentStateAction
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.GetService(ActionsData._100_DIAGTRACK_NAME).StartType == ServiceStartMode.Automatic;
            var firewallRule = FirewallHelper.GetGroupRule(ActionsData._100_DIAGTRACK_NAME).FirstOrDefault();
            return diagTrack && firewallRule.Enabled && firewallRule.Action.Equals(NET_FW_ACTION_.NET_FW_ACTION_ALLOW);
        }

        public static bool _265() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._265_EXTRACT_PATH) is null);

        public static bool _267() => RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._267_RUNASUSER_PATH)
                                              .GetValue(ActionsData._267_EXTENDED_NAME) is null;

        public static bool _268() => RegHelper.GetRegistryKey(RegistryHive.LocalMachine, ActionsData._268_CAST_TO_DEVICE_PATH)
                                              .GetValue(ActionsData._268_CAST_TO_DEVICE_NAME) as string != ActionsData._268_CAST_TO_DEVICE_VALUE;

        public static bool _269() => Array.IndexOf(RegHelper.GetRegistryKey(RegistryHive.LocalMachine, ActionsData._269_SHARE_CONTENT_PATH).GetValueNames(),
                                                   ActionsData._269_SHARE_CONTENT_NAME) != -1;

        public static bool _271() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._271_3D_EDIT_BMP_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _272() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._272_3D_EDIT_GIF_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _273() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._273_3D_EDIT_JPE_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _274() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._274_3D_EDIT_JPEG_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _275() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._275_3D_EDIT_JPG_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _276() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._276_3D_EDIT_PNG_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _277() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._277_3D_EDIT_TIF_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _278() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._278_3D_EDIT_TIFF_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _279() => Array.IndexOf(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._279_SHELL_EDIT_PATH).GetValueNames(),
                                                   ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) != -1;

        public static bool _280() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._280_SHELL_CREATE_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        public static bool _281() => !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._281_SHELL_EDIT_PATH)
                                                .GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);

        //TODO: Add Get-AppxPackage -Name Microsoft.Windows.Photos check !
        //TODO: Add Get-AppxPackage -Name Microsoft.Windows.Photos check !
        public static bool _282()
        {
            var batFile = !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_BAT_PRINT_PATH).GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);
            var cmdFile = !(RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_CMD_PRINT_PATH).GetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME) is string);
            return batFile && cmdFile;
        }

        public static bool _283() => RegHelper.GetRegistryKey(RegistryHive.ClassesRoot, ActionsData._283_LIBRARY_LOCATION_PATH)
                                              .GetValue(string.Empty) as string == ActionsData._283_LIBRARY_LOCATION_VALUE;

        public static bool FOR_DEBUG_ONLY() => false; //TODO: CurrentStateAction - This method for debug only.
    }
}