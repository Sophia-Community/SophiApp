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
            return capability.State == DismPackageFeatureState.Installed
                    || capability.State == DismPackageFeatureState.InstallPending;
        }

        internal static bool FeatureIsInstalled(string name)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();
            var featureInfo = DismApi.GetFeatureInfo(session, name);
            session.Close();
            DismApi.Shutdown();
            return featureInfo.FeatureState == DismPackageFeatureState.Installed
                    || featureInfo.FeatureState == DismPackageFeatureState.InstallPending;
        }

        internal static void SetCapabilityState(string name, bool enable)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();

            try
            {
                if (enable)
                {
                    DismApi.AddCapability(session, name, false, null);
                }
                else
                {
                    DismApi.RemoveCapability(session, name);
                }
            }
            catch (DismRebootRequiredException)
            {
            }
            finally
            {
                session.Close();
                DismApi.Shutdown();
            }
        }

        internal static void SetFeatureState(string name, bool enable)
        {
            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();

            try
            {
                if (enable)
                {
                    DismApi.EnableFeatureByPackageName(session, name, null, false, true);
                }
                else
                {
                    DismApi.DisableFeature(session, name, null, false);
                }
            }
            catch (DismRebootRequiredException)
            {
            }
            finally
            {
                session.Close();
                DismApi.Shutdown();
            }
        }
    }
}