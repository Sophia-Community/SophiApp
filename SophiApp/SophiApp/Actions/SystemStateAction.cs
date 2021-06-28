using Microsoft.Win32;
using SophiApp.Helpers;
using System.Linq;
using System.ServiceProcess;

namespace SophiApp.Actions
{
    //TODO: Implement system state methods

    public class SystemStateAction
    {
        public static void _100(bool IsActive)
        {
            var diagTrack = ServiceHelper.GetService(ActionsData._100_DIAGTRACK_NAME);
            var firewallRule = FirewallHelper.GetGroupRule(ActionsData._100_DIAGTRACK_NAME).FirstOrDefault();

            if (IsActive)
            {
                ServiceHelper.ChangeStartMode(diagTrack, ServiceStartMode.Automatic);
                diagTrack.Start();
                // Allow connection for the Unified Telemetry Client Outbound Traffic
                firewallRule.Enabled = true;
                firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                return;
            }

            diagTrack.Stop();
            ServiceHelper.ChangeStartMode(diagTrack, ServiceStartMode.Disabled);
            // Block connection for the Unified Telemetry Client Outbound Traffic
            firewallRule.Enabled = false;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

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
                string COMMAND_PATH = $"{ActionsData._265_EXTRACT_PATH}\\{COMMAND_NAME}";

                Registry.ClassesRoot.CreateSubKey(ActionsData._265_EXTRACT_PATH, true).CreateSubKey(COMMAND_NAME);
                Registry.ClassesRoot.OpenSubKey(COMMAND_PATH, true).SetValue(string.Empty, EXTRACT_VALUE);
                Registry.ClassesRoot.OpenSubKey(ActionsData._265_EXTRACT_PATH, true).SetValue(MUIVERB_NAME, MUIVERB_VALUE);
                Registry.ClassesRoot.OpenSubKey(ActionsData._265_EXTRACT_PATH, true).SetValue(ICON_NAME, ICON_VALUE);
                return;
            }

            Registry.ClassesRoot.DeleteSubKeyTree(ActionsData._265_EXTRACT_PATH);
        }

        public static void _267(bool IsActive)
        {
            if (IsActive)
            {
                var key = Registry.ClassesRoot.OpenSubKey(ActionsData._267_RUNASUSER_PATH, true);
                key.DeleteValue(ActionsData._267_EXTENDED_NAME);
                return;
            }

            Registry.ClassesRoot.CreateSubKey(ActionsData._267_RUNASUSER_PATH, true).SetValue(ActionsData._267_EXTENDED_NAME, string.Empty);
        }

        public static void _268(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.LocalMachine, ActionsData._268_CAST_TO_DEVICE_PATH).DeleteValue(ActionsData._268_CAST_TO_DEVICE_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.LocalMachine, ActionsData._268_CAST_TO_DEVICE_PATH).SetValue(ActionsData._268_CAST_TO_DEVICE_NAME, ActionsData._268_CAST_TO_DEVICE_VALUE);
        }

        public static void _269(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.LocalMachine, ActionsData._269_SHARE_CONTENT_PATH).SetValue(ActionsData._269_SHARE_CONTENT_NAME, string.Empty);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.LocalMachine, ActionsData._269_SHARE_CONTENT_PATH).DeleteValue(ActionsData._269_SHARE_CONTENT_NAME);
        }

        public static void _271(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._271_3D_EDIT_BMP_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._271_3D_EDIT_BMP_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _272(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._272_3D_EDIT_GIF_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._272_3D_EDIT_GIF_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _273(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._273_3D_EDIT_JPE_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._273_3D_EDIT_JPE_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _274(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._274_3D_EDIT_JPEG_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._274_3D_EDIT_JPEG_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _275(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._275_3D_EDIT_JPG_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._275_3D_EDIT_JPG_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _276(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._276_3D_EDIT_PNG_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._276_3D_EDIT_PNG_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _277(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._277_3D_EDIT_TIF_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._277_3D_EDIT_TIF_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _278(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._278_3D_EDIT_TIFF_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._278_3D_EDIT_TIFF_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _279(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._279_SHELL_EDIT_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._279_SHELL_EDIT_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _280(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._280_SHELL_CREATE_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._280_SHELL_CREATE_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _281(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._281_SHELL_EDIT_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._281_SHELL_EDIT_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _282(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_BAT_PRINT_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_CMD_PRINT_PATH).DeleteValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_BAT_PRINT_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._282_CMD_PRINT_PATH).SetValue(ActionsData.PROGRAMMATIC_ACCESS_ONLY_NAME, string.Empty);
        }

        public static void _283(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._283_LIBRARY_LOCATION_PATH).SetValue(string.Empty, ActionsData._283_LIBRARY_LOCATION_VALUE);
                return;
            }

            RegHelper.SetRegistryKey(RegistryHive.ClassesRoot, ActionsData._283_LIBRARY_LOCATION_PATH).SetValue(string.Empty, $"-{ActionsData._283_LIBRARY_LOCATION_VALUE}");
        }

        public static void FOR_DEBUG_ONLY(bool state)
        {
        }
    }
}