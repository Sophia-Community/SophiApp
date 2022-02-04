using Microsoft.Dism;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
        internal static bool CapabilityIsInstalled(string name)
        {
            var result = default(bool);

            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();

            try
            {
                var capability = DismApi.GetCapabilityInfo(session, name);
                result = capability.State == DismPackageFeatureState.Installed || capability.State == DismPackageFeatureState.InstallPending;
            }
            catch (DismRebootRequiredException)
            {
            }
            finally
            {
                session.Close();
                DismApi.Shutdown();
            }

            return result;
        }

        internal static bool FeatureIsInstalled(string name)
        {
            var result = default(bool);

            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();

            try
            {
                var feature = DismApi.GetFeatureInfo(session, name);
                result = feature.FeatureState == DismPackageFeatureState.Installed || feature.FeatureState == DismPackageFeatureState.InstallPending;
            }
            catch (DismRebootRequiredException)
            {
            }
            finally
            {
                session.Close();
                DismApi.Shutdown();
            }

            return result;
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