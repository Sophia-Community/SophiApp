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

        internal static void EnableFeature(string name, bool enable)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();

            if (enable)
            {
                DismApi.EnableFeatureByPackageName(session, name, null, false, true);
            }
            else
            {
                DismApi.DisableFeature(session, name, null, false);
            }

            session.Close();
            DismApi.Shutdown();
        }

        internal static DismFeatureInfo GetFeatureInfo(string name)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();
            var featureInfo = DismApi.GetFeatureInfo(session, name);
            session.Close();
            DismApi.Shutdown();
            return featureInfo;
        }
    }
}