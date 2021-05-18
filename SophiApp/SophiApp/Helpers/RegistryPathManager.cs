using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal static class RegistryPathManager
    {
        internal const string CURRENT_VERSION = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        internal const string DATA_COLLECTION = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";

        internal const string ALLOW_TELEMETRY = "AllowTelemetry";
        internal const string PRODUCT_NAME = "ProductName";
        internal const string EDITION_ID = "EditionID";
    }
}
