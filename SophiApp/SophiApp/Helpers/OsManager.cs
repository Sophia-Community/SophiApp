using Microsoft.Win32;

namespace SophiApp.Helpers
{
    internal class OsManager
    {
        static internal string GetEditionId() => Registry.LocalMachine.OpenSubKey(RegistryPathManager.CURRENT_VERSION)
                                                                      .GetValue(RegistryPathManager.EDITION_ID) as string;

        static internal string GetProductName() => Registry.LocalMachine.OpenSubKey(RegistryPathManager.CURRENT_VERSION)
                                                                                .GetValue(RegistryPathManager.PRODUCT_NAME) as string;
    }
}