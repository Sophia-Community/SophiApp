using Microsoft.Dism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
        internal static bool CapabilityExist(string name)
        {            
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();
            var capabilities = DismApi.GetCapabilities(session).ToList();
            session.Close();
            DismApi.Shutdown();
            return capabilities.Any(c => c.Name.Contains(name) && c.State == DismPackageFeatureState.Installed);
        }
    }
}
