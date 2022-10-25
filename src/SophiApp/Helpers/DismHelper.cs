using Microsoft.Dism;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
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