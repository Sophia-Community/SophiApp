using Microsoft.Win32;
using SophiApp.Helpers;

namespace SophiApp.Actions
{
    //TODO: Implement method selection by ID

    public class CurrentStateAction
    {
        public static bool FOR_DEBUG_ONLY() => false; //TODO: CurrentStateAction - This method for debug only.

        public static bool _265() => !(Registry.ClassesRoot.OpenSubKey(RegPaths._265_EXTRACT_PATH) is null);
    }
}