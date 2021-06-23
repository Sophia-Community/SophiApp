using Microsoft.Win32;
using SophiApp.Commons;
using SophiApp.Helpers;

namespace SophiApp.Actions
{
    //TODO: Implement system state methods

    public class SystemStateAction
    {
        public static void FOR_DEBUG_ONLY(bool state)
        {
        }

        public static void _265(bool state)
        {

            if (state)
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
    }
}