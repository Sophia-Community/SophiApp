using Microsoft.Win32;
using System;

namespace SophiApp.Helpers
{
    internal class OsManager
    {
        internal static string GetProductName()
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey($"{RegPaths.CURRENT_VERSION}").GetValue(RegPaths.PRODUCT_NAME) as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}