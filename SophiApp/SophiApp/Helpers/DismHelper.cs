using Microsoft.Dism;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SophiApp.Helpers
{
    internal class DismHelper
    {
        private static readonly object locked = new object();
        private static List<DismCapability> Capabilites;
        private static List<DismFeature> Features;
        private static DismHelper instance;

        private DismHelper()
        {
            GetInstalledComponents();
        }

        internal static bool CapabilityIsInstalled(string name)
        {
            var index = Capabilites.FindIndex(c => c.Name == name);
            return index != -1 && Capabilites[index].State == DismPackageFeatureState.Installed;
        }

        internal static bool FeatureIsInstalled(string name)
        {
            var index = Features.FindIndex(f => f.FeatureName == name);
            return index != -1 && Features[index].State == DismPackageFeatureState.Installed;
        }

        internal static DismHelper GetInstance()
        {
            if (instance == null)
            {
                lock (locked)
                {
                    if (instance == null)
                        instance = new DismHelper();
                }
            }
            return instance;
        }

        internal static async Task<DismHelper> GetInstanceAsync() => await Task.Run(() => GetInstance());

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

        internal void GetInstalledComponents()
        {
            DebugHelper.StartInitDismInstalledComponents();
            var stopwatch = Stopwatch.StartNew();

            DismApi.Initialize(DismLogLevel.LogErrors);
            var session = DismApi.OpenOnlineSession();
            Capabilites = DismApi.GetCapabilities(session).ToList();
            Features = DismApi.GetFeatures(session).ToList();
            session.Close();
            DismApi.Shutdown();

            stopwatch.Stop();
            DebugHelper.StopInitDismInstalledComponents(stopwatch.Elapsed.TotalSeconds);
        }
    }
}