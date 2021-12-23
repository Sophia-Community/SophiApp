using System;
using System.Linq;
using System.Management;

namespace SophiApp.Helpers
{
    internal class WmiHelper
    {
        private const string PROTECTION_STATUS = "ProtectionStatus";

        internal static string GetActivePowerPlanId()
        {
            var searcher = GetManagementObjectSearcher(@"Root\Cimv2\power", "SELECT * FROM Win32_PowerPlan WHERE IsActive = True");
            var activePlan = searcher.Get().Cast<ManagementBaseObject>().First();
            return (activePlan.GetPropertyValue("InstanceId") as string).Split('\\')[1];
        }

        internal static int GetBitLockerVolumeProtectionStatus()
        {
            var searcher = GetManagementObjectSearcher(@"Root\Cimv2\security\MicrosoftVolumeEncryption", "SELECT * FROM Win32_Encryptablevolume");
            var status = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault().Properties[PROTECTION_STATUS].Value;
            return Convert.ToInt32(status);
        }

        internal static string GetVideoControllerDacType()
        {
            var scope = @"Root\Cimv2";
            var query = "SELECT * FROM CIM_VideoController";
            var adapter = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First();
            return adapter.Properties["AdapterDACType"].Value as string;
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
            var adapters = GetManagementObjectSearcher(scope, queryEnergySavingAdapters).Get();

            if (adapters.Count == 0)
                throw new NetworkAdapterNotEnergySavingException();

            foreach (var adapter in adapters)
            {
                if (adapter.Properties["AllowComputerToTurnOffDevice"].Value.ToUshort() == 2)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        internal static bool IsVirtualMachine()
        {
            var scope = @"Root\Cimv2";
            var query = "SELECT * FROM CIM_ComputerSystem";
            var computer = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First();
            var model = computer.Properties["Model"].Value as string;
            return model.Contains("Virtual");
        }

        internal static void SetNetworkAdaptersPowerSave(bool enablePowerSave)
        {
            // https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh872363(v=vs.85)
            // Unsupported (0)
            // Disabled(1)
            // Enabled(2)

            var scope = @"Root\StandardCimv2";
            var query = "SELECT * FROM MSFT_NetAdapterPowerManagementSettingData WHERE AllowComputerToTurnOffDevice != \"0\"";
            var adapters = GetManagementObjectSearcher(scope, query).Get();

            foreach (ManagementObject adapter in adapters)
            {
                adapter.SetPropertyValue("AllowComputerToTurnOffDevice", enablePowerSave == true ? 2 : 1);
                _ = adapter.Put();
            }
        }

        private static ManagementObjectSearcher GetManagementObjectSearcher(string scope, string query) => new ManagementObjectSearcher(scope, query);
    }
}