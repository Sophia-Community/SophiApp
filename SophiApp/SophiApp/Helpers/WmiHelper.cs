using System;
using System.Linq;
using System.Management;

namespace SophiApp.Helpers
{
    internal class WmiHelper
    {
        private const string PROTECTION_STATUS = "ProtectionStatus";

        private static ManagementObjectSearcher GetManagementObjectSearcher(string scope, string query) => new ManagementObjectSearcher(scope, query);

        internal static string GetActivePowerPlanId()
        {
            var searcher = GetManagementObjectSearcher(@"Root\cimv2\power", "SELECT * FROM Win32_PowerPlan WHERE IsActive = True");
            var activePlan = searcher.Get().Cast<ManagementBaseObject>().First();
            return (activePlan.GetPropertyValue("InstanceId") as string).Split('\\')[1];
        }

        internal static int GetBitLockerVolumeProtectionStatus()
        {
            var searcher = GetManagementObjectSearcher(@"Root\cimv2\security\MicrosoftVolumeEncryption", "SELECT * FROM Win32_Encryptablevolume");
            var status = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault().Properties[PROTECTION_STATUS].Value;
            return Convert.ToInt32(status);
        }

        internal static bool HasNetworkAdaptersPowerSave()
        {
            // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh872363(v=vs.85)
            // Unsupported (0)
            // Disabled(1)
            // Enabled(2)

            var result = false;
            var scope = @"Root\StandardCimv2";
            var queryEnergySavingAdapters = "SELECT AllowComputerToTurnOffDevice FROM MSFT_NetAdapterPowerManagementSettingData WHERE AllowComputerToTurnOffDevice != \"0\"";
            var energySavingAdapters = GetManagementObjectSearcher(scope, queryEnergySavingAdapters).Get();

            if (energySavingAdapters.Count == 0)
            {
                throw new NetworkAdapterNotEnergySavingException();
            }

            foreach (var adapter in energySavingAdapters)
            {
                if (adapter.Properties["AllowComputerToTurnOffDevice"].Value.ToUshort() == 2)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        internal static void SetNetworkAdaptersPowerSave(bool enablePowerSave)
        {
            // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh872363(v=vs.85)
            // Unsupported (0)
            // Disabled(1)
            // Enabled(2)

            var value = enablePowerSave == true ? 2 : 1;
            var scope = @"Root\StandardCimv2";
            var queryEnergySavingAdapters = "SELECT * FROM MSFT_NetAdapterPowerManagementSettingData WHERE AllowComputerToTurnOffDevice != \"0\"";
            var energySavingAdapters = GetManagementObjectSearcher(scope, queryEnergySavingAdapters).Get();

            foreach (ManagementObject adapter in energySavingAdapters)
            {
                adapter.SetPropertyValue("AllowComputerToTurnOffDevice", value);
                _ = adapter.Put();
            }
        }
    }
}