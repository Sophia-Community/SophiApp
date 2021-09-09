using Microsoft.Win32;
using SophiApp.Helpers;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Const = SophiApp.Customisations.CustomisationConstants;

namespace SophiApp.Customisations
{
    public class CustomisationOs
    {
        public static void _100(bool IsActive)
        {
            var diagTrack = ServiceHelper.GetService(Const._100_DIAG_TRACK);
            var firewallRule = FirewallHelper.GetGroupRule(Const._100_DIAG_TRACK).FirstOrDefault();

            if (IsActive)
            {
                ServiceHelper.SetStartMode(diagTrack, ServiceStartMode.Automatic);
                diagTrack.Start();                
                firewallRule.Enabled = true;
                firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                return;
            }

            diagTrack.Stop();
            ServiceHelper.SetStartMode(diagTrack, ServiceStartMode.Disabled);            
            firewallRule.Enabled = false;
            firewallRule.Action = NetFwTypeLib.NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
        }

        public static void _102(bool IsActive)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL, Const.DEFAULT_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _103(bool IsActive)
        {
            RegHelper.SetValue(RegistryHive.LocalMachine,
                                Const.DATA_COLLECTION_PATH, Const.ALLOW_TELEMETRY,
                                    OsHelper.IsEdition(Const.WIN_VER_ENT) || OsHelper.IsEdition(Const.WIN_VER_EDU)
                                        ? Const.MIN_ENT_TELEMETRY_VALUE : Const.MIN_TELEMETRY_VALUE,
                                            RegistryValueKind.DWord);

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.DATA_COLLECTION_PATH, Const.MAX_TELEMETRY_ALLOWED, Const.MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
            RegHelper.SetValue(RegistryHive.CurrentUser, Const.DIAG_TRACK_PATH, Const.SHOWED_TOAST_LEVEL, Const.MIN_TELEMETRY_VALUE, RegistryValueKind.DWord);
        }

        public static void _104(bool IsActive)
        {
            var werService = ServiceHelper.GetService(Const._104_WER_SERVICE);

            if (IsActive)
            {
                ScheduledTaskHelper.EnableTask(Const._104_QUEUE_TASK);
                RegHelper.DeleteKey(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED);
                ServiceHelper.SetStartMode(werService, ServiceStartMode.Manual);
                werService.Start();
                return;
            }

            if (!OsHelper.IsEdition(Const._104_CORE))
            {
                ScheduledTaskHelper.DisableTask(Const._104_QUEUE_TASK);
                RegHelper.SetValue(RegistryHive.CurrentUser, Const._104_WER_PATH, Const._104_DISABLED, Const._104_DISABLED_DEFAULT_VALUE, RegistryValueKind.DWord);                
            }

            werService.Stop();
            ServiceHelper.SetStartMode(werService, ServiceStartMode.Disabled);
        }


        public static void _800(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_COM_PATH, string.Empty, Const._800_MSI_EXTRACT_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH, Const.MUIVERB, Const._800_MSI_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH, Const._800_MSI_ICON, Const._800_MSI_ICON_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._800_MSI_EXTRACT_PATH);
        }

        public static void _801(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_COM_PATH, string.Empty, Const._801_CAB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH, Const.MUIVERB, Const._801_MUIVERB_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH, Const._801_CAB_LUA_SHIELD, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._801_CAB_RUNAS_PATH);
        }

        public static void _802(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._802_RUNAS_USER_PATH, Const._802_EXTENDED, string.Empty);
        }

        public static void _803(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._803_CAST_TO_DEV_GUID, Const._803_CAST_TO_DEV_VALUE);
        }

        public static void _804(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const.SHELL_EXT_BLOCKED_PATH, Const._804_SHARE_GUID, string.Empty);
        }

        public static void _806(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._806_BMP_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _807(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._807_GIF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _808(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._808_JPE_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _809(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._809_JPEG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _810(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._810_JPG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _811(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._811_PNG_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _812(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._812_TIF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _813(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._813_TIFF_EXT, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _814(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._814_PHOTOS_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _815(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._815_PHOTOS_SHELL_VIDEO_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _816(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._816_IMG_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _817(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._817_BAT_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._817_CMD_SHELL_EDIT_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _818(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, Const._818_LIB_LOCATION_PATH, string.Empty,
                                                                                IsActive ? Const._818_SHOW_VALUE : Const._818_HIDE_VALUE);

        public static void _819(bool IsActive) => RegHelper.SetValue(RegistryHive.ClassesRoot, Const._819_SEND_TO_PATH, string.Empty,
                                                                                IsActive ? Const._819_SHOW_VALUE : Const._819_HIDE_VALUE);

        public static void _820(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY);
                return;
            }

            RegHelper.SetValue(RegistryHive.ClassesRoot, Const._820_BITLOCKER_BDELEV_PATH, Const.PROGRAM_ACCESS_ONLY, string.Empty);
        }

        public static void _821(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_ITEM_NAME, Const._821_BMP_ITEM_VALUE, RegistryValueKind.ExpandString);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW, Const._821_BMP_NULL_FILE, string.Empty);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._821_BMP_SHELL_NEW);
        }

        public static void _822(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.DATA, Const._822_DATA_VALUE);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW, Const.ITEM_NAME, Const._822_ITEM_VALUE);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._822_RTF_SHELL_NEW);
        }

        public static void _823(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH, Const.DATA, Const._823_ZIP_DATA, RegistryValueKind.Binary);
                RegHelper.SetValue(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH, Const.ITEM_NAME, Const._823_ITEM_DATA, RegistryValueKind.ExpandString);
                return;
            }

            RegHelper.DeleteSubKeyTree(RegistryHive.ClassesRoot, Const._823_ZIP_SHELLNEW_PATH);
        }

        public static void _824(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.SetValue(RegistryHive.CurrentUser, Const._824_CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME, Const._824_PROMPT_VALUE, RegistryValueKind.DWord);
                return;
            }

            RegHelper.DeleteKey(RegistryHive.CurrentUser, Const._824_CURRENT_EXPLORER_PATH, Const._824_PROMPT_NAME);
        }

        public static void _825(bool IsActive)
        {
            if (IsActive)
            {
                RegHelper.DeleteKey(RegistryHive.LocalMachine, Const._825_SOFTWARE_EXPLORER_PATH, Const._825_NO_USE_NAME);
                return;
            }

            RegHelper.SetValue(RegistryHive.LocalMachine, Const._825_SOFTWARE_EXPLORER_PATH, Const._825_NO_USE_NAME, Const._825_NO_USE_VALUE, RegistryValueKind.DWord);
        }

        //TODO: CustomisationOs - Method placeholder.
        public static void FOR_DEBUG_ONLY(bool IsActive) => Thread.Sleep(1000);
    }
}