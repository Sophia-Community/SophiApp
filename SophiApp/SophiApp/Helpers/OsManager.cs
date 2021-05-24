using Microsoft.Win32;

namespace SophiApp.Helpers
{
    internal class OsManager
    {
        internal static string GetEdition() => Registry.LocalMachine.OpenSubKey(RegPath.CURRENT_VERSION).GetValue(RegPath.EDITION_ID) as string;

        internal static string GetProductName() => Registry.LocalMachine.OpenSubKey(RegPath.CURRENT_VERSION).GetValue(RegPath.PRODUCT_NAME) as string;
    }
}