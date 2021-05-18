using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class OsManager
    {
        static internal string GetProductName() => Registry.LocalMachine.OpenSubKey(RegistryPathManager.CURRENT_VERSION)
                                                                        .GetValue(RegistryPathManager.PRODUCT_NAME) as string;

        static internal string GetEditionId() => Registry.LocalMachine.OpenSubKey(RegistryPathManager.CURRENT_VERSION)
                                                                      .GetValue(RegistryPathManager.EDITION_ID) as string;

    }
}
