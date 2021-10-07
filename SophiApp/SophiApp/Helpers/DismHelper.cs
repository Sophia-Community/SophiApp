using Microsoft.Dism;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
        internal static bool CapabilityIsInstalled(string name)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();
            var capability = DismApi.GetCapabilityInfo(session, name);
            session.Close();
            DismApi.Shutdown();
            return capability.State == DismPackageFeatureState.Installed;
        }
    }
}