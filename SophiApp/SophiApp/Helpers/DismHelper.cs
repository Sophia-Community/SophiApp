using Microsoft.Dism;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
        internal static bool CapabilityIsInstalled(string name)
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