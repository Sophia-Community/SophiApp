using Microsoft.Win32;

namespace SophiApp.Helpers
{
    internal class WindowsDefenderHelper
    {
        internal static bool DisabledByGpo()
        {
            const string DISABLE_RTM_MONITORING = "DisableRealtimeMonitoring";
            const string DISABLE_ANTI_SPYWARE = "DisableAntiSpyware";
            const string DEFENDER_REAL_TIME_PATH = @"SOFTWARE\Policies\Microsoft\Windows Defender\Real-Time";
            const string DEFENDER_PATH = @"SOFTWARE\Policies\Microsoft\Windows Defender";
            const int DISABLED_VALUE = 1;

            return RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DEFENDER_PATH, DISABLE_ANTI_SPYWARE) == DISABLED_VALUE
                    || RegHelper.GetNullableIntValue(RegistryHive.LocalMachine, DEFENDER_REAL_TIME_PATH, DISABLE_RTM_MONITORING) == DISABLED_VALUE;
        }
    }
}