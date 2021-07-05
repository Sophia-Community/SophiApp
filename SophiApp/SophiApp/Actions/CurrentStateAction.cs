using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
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

        public static bool _102()
        {
            var allowTelemetry = RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData._101_TELEMETRY_PATH, name: ActionsData._101_TELEMETRY_NAME) as byte?;
            var maxTelemetry = RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData._101_MAX_TELEMETRY_PATH, name: ActionsData._101_MAX_TELEMETRY_NAME) as byte?;
            var toastLevel = RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData._101_TOAST_LEVEL_PATH, name: ActionsData._101_TOAST_LEVEL_NAME) as byte?;

            if ((allowTelemetry == ActionsData._101_TELEMETRY_VALUE && maxTelemetry == ActionsData._101_TELEMETRY_VALUE && toastLevel == ActionsData._101_TELEMETRY_VALUE)
                || (allowTelemetry is null && maxTelemetry is null && toastLevel is null))
            {
                return true;
            }

            return false;
        }

        public static bool _104()
        {
            var queueTaskState = TaskService.Instance.RootFolder.EnumerateTasks(task => task.Name == ActionsData._104_QUEUE_REPORTING_NAME, true).FirstOrDefault().State == TaskState.Ready;
            var wer = RegHelper.GetValue(hive: RegistryHive.CurrentUser, path: ActionsData._104_WER_PATH, name: ActionsData._104_WER_NAME);
            var werSvc = ServiceHelper.GetService(ActionsData._104_WER_SVC_NAME).StartType == ServiceStartMode.Manual;

            if ((queueTaskState && Convert.ToByte(wer) != 1) || (wer is null && werSvc))
            {
                return true;
            }

            return false;
        }

        public static bool _265() => RegHelper.GetSubKey(hive: RegistryHive.ClassesRoot, path: ActionsData._265_EXTRACT_PATH) != null;

        public static bool _267() => !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._267_RUNASUSER_PATH, name: ActionsData._267_EXTENDED_NAME);

        public static bool _268() => RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData._268_CAST_TO_DEVICE_PATH, name: ActionsData._268_CAST_TO_DEVICE_NAME) as string != ActionsData._268_CAST_TO_DEVICE_VALUE;

        public static bool _269() => !RegHelper.KeyExist(hive: RegistryHive.LocalMachine, path: ActionsData._269_SHARE_CONTENT_PATH, name: ActionsData._269_SHARE_CONTENT_NAME);

        public static bool _271()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._271_3D_EDIT_BMP_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _272()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._272_3D_EDIT_GIF_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _273()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._273_3D_EDIT_JPE_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _274()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._274_3D_EDIT_JPEG_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _275()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._275_3D_EDIT_JPG_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _276()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._276_3D_EDIT_PNG_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _277()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._277_3D_EDIT_TIF_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _278()
        {
            return UwpHelper.PackageExist(ActionsData.UWP_3D_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._278_3D_EDIT_TIFF_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.UWP_3D_PAINT_NAME} {ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _279()
        {
            return UwpHelper.PackageExist(ActionsData._279_UWP_PHOTOS_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._279_SHELL_EDIT_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData._279_UWP_PHOTOS_NAME}{ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _280()
        {
            return UwpHelper.PackageExist(ActionsData._279_UWP_PHOTOS_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._280_SHELL_CREATE_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData._279_UWP_PHOTOS_NAME}{ActionsData.EXCEPTION_UWP_NOT_FOUND}");
        }

        public static bool _281()
        {
            return DismHelper.CapabilityExist(ActionsData.CAPABILITY_PAINT_NAME)
                   ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._281_SHELL_EDIT_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                   : throw new Exception($"{ActionsData.CAPABILITY_PAINT_NAME} {ActionsData.EXCEPTION_CAPABILITY_NOT_FOUND}");
        }

        public static bool _282()
        {
            var batFile = RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._282_BAT_PRINT_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            var cmdFile = RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._282_CMD_PRINT_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
            return !batFile && !cmdFile;
        }

        public static bool _283() => RegHelper.GetValue(hive: RegistryHive.ClassesRoot, path: ActionsData._283_LIBRARY_LOCATION_PATH, name: string.Empty) as string == ActionsData._283_LIBRARY_LOCATION_VALUE;

        public static bool _284() => RegHelper.GetValue(hive: RegistryHive.ClassesRoot, path: ActionsData._284_SEND_TO_PATH, name: string.Empty) as string == ActionsData._284_SEND_TO_VALUE;

        public static bool _285()
        {
            if (OsHelper.IsEdition(ActionsData._285_PRO_EDITION) || OsHelper.IsEdition(ActionsData._285_ENT_EDITION))
            {
                var status = WmiHelper.GetBitLockerVolumeProtectionStatus();
                return status == 0
                               ? !RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._285_ENCRYPT_BDE_ELEV_PATH, name: ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME)
                               : throw new Exception($"{ActionsData.EXCEPTION_BITLOCKER_IS_ENABLED}");
            }

            throw new Exception($"{ActionsData.EXCEPTION_NOT_SUPPORTED_OS_VERSION}");
        }

        public static bool _286()
        {
            return DismHelper.CapabilityExist(ActionsData.CAPABILITY_PAINT_NAME)
                   ? RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._286_SHELL_NEW_PATH, name: ActionsData._286_ITEM_NAME) &&
                     RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._286_SHELL_NEW_PATH, name: ActionsData._286_NULL_FILE_NAME)
                   : throw new Exception($"{ActionsData.CAPABILITY_PAINT_NAME} {ActionsData.EXCEPTION_CAPABILITY_NOT_FOUND}");
        }

        public static bool _287()
        {
            return DismHelper.CapabilityExist(ActionsData._287_CAPABILITY_WORDPAD_NAME)
                   ? RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._287_SHELL_NEW_PATH, name: ActionsData._287_DATA_NAME) &&
                     RegHelper.KeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._287_SHELL_NEW_PATH, name: ActionsData._287_ITEM_NAME)
                   : throw new Exception($"{ActionsData._287_CAPABILITY_WORDPAD_NAME} {ActionsData.EXCEPTION_CAPABILITY_NOT_FOUND}");
        }

        public static bool _288() => RegHelper.SubKeyExist(hive: RegistryHive.ClassesRoot, path: ActionsData._288_SHELL_NEW_PATH);

        public static bool _289() => RegHelper.KeyExist(hive: RegistryHive.CurrentUser, path: ActionsData._289_EXPLORER_PATH, name: ActionsData._289_PROMPT_MIN_NAME);

        public static bool _290() => Convert.ToByte(RegHelper.GetValue(hive: RegistryHive.LocalMachine, path: ActionsData._290_EXPLORER_PATH, name: ActionsData._290_OPEN_WITH_NAME)) == ActionsData._290_OPEN_WITH_VALUE;

        public static bool FOR_DEBUG_ONLY() => false; //TODO: CurrentStateAction - Method placeholder.
    }
}