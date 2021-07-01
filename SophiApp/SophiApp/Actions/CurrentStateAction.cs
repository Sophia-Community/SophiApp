using Microsoft.Win32;
using NetFwTypeLib;
using SophiApp.Helpers;
using System;
using System.Linq;
using System.ServiceProcess;

namespace SophiApp.Actions
{
    public class CurrentStateAction
    {
        public static bool _100()
        {
            var diagTrack = ServiceHelper.GetService(ActionsData._100_DIAGTRACK_NAME).StartType == ServiceStartMode.Automatic;
            var firewallRule = FirewallHelper.GetGroupRule(ActionsData._100_DIAGTRACK_NAME).FirstOrDefault();
            return diagTrack && firewallRule.Enabled && firewallRule.Action.Equals(NET_FW_ACTION_.NET_FW_ACTION_ALLOW);
        }

        public static bool _265() => RegHelper.GetSubKey(hive: RegistryHive.ClassesRoot,
                                                         path: ActionsData._265_EXTRACT_PATH) != null;

        public static bool _267() => !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                                         path: ActionsData._267_RUNASUSER_PATH,
                                                         name: ActionsData._267_EXTENDED_NAME);

        public static bool _268() => RegHelper.GetValue(hive: RegistryHive.LocalMachine,
                                                           path: ActionsData._268_CAST_TO_DEVICE_PATH,
                                                           name: ActionsData._268_CAST_TO_DEVICE_NAME) != ActionsData._268_CAST_TO_DEVICE_VALUE;

        public static bool _269() => !RegHelper.KeyExist(hive: RegistryHive.LocalMachine,
                                                         path: ActionsData._269_SHARE_CONTENT_PATH,
                                                         name: ActionsData._269_SHARE_CONTENT_NAME);

        public static bool _271()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._271_3D_EDIT_BMP_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _272()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._272_3D_EDIT_GIF_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _273()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._273_3D_EDIT_JPE_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _274()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._274_3D_EDIT_JPEG_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _275()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._275_3D_EDIT_JPG_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _276()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._276_3D_EDIT_PNG_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _277()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._277_3D_EDIT_TIF_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _278()
        {
            if (UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._278_3D_EDIT_TIFF_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _279()
        {
            if (UwpHelper.PackageExist(ActionsData._279_UWP_PHOTOS_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._279_SHELL_EDIT_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData._279_UWP_PHOTOS_NAME}{ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _280()
        {
            if (UwpHelper.PackageExist(ActionsData._279_UWP_PHOTOS_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._280_SHELL_CREATE_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData._279_UWP_PHOTOS_NAME}{ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _281()
        {
            if (DismHelper.CapabilityExist(ActionsData._281_CAPABILITY_PAINT_NAME))
            {
                return !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                           path: ActionsData._281_SHELL_EDIT_PATH,
                                           name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            }

            throw new Exception($"{ActionsData._281_CAPABILITY_PAINT_NAME}{ActionsData.EXCEPTION_CAPABILITY_NOT_FOUND}");
        }

        public static bool _282()
        {
            var batFile = RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                             path: ActionsData._282_BAT_PRINT_PATH,
                                             name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);

            var cmdFile = RegHelper.KeyExist(hive: RegistryHive.ClassesRoot,
                                             path: ActionsData._282_CMD_PRINT_PATH,
                                             name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);

            return !batFile && !cmdFile;
        }

        public static bool _283() => RegHelper.GetValue(hive: RegistryHive.ClassesRoot,
                                                        path: ActionsData._283_LIBRARY_LOCATION_PATH,
                                                        name: string.Empty) == ActionsData._283_LIBRARY_LOCATION_VALUE;

        public static bool _284() => RegHelper.GetValue(hive: RegistryHive.ClassesRoot,
                                                         path: ActionsData._284_SEND_TO_PATH,
                                                         name: string.Empty) == ActionsData._284_SEND_TO_VALUE;

        // TODO: 285 not implemented.

        public static bool _286()
        {
            return true;
        }

        //TODO: Implement method selection by ID
        public static bool FOR_DEBUG_ONLY() => false; //TODO: CurrentStateAction - This method for debug only.
    }
}