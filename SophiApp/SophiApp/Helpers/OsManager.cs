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
                return Registry.LocalMachine.OpenSubKey($"{RegPath.CURRENT_VERSION}").GetValue(RegPath.PRODUCT_NAME) as string;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}