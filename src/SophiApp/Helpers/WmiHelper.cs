using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace SophiApp.Helpers
{
    internal class WmiHelper
    {
        private const string AM_ENGINE_VERSION = "AMEngineVersion";
        private const string ANTISPYWARE_ENABLED = "AntispywareEnabled";
        private const string DEFENDER_GUID = "{D68DDC3A-831F-4fae-9E44-DA132C1ACF46}";
        private const string DEFENDER_INSTANCE_GUID = "instanceGuid";
        private const string DEFENDER_ROOT = @"Root/Microsoft/Windows/Defender";
        private const string PRODUCT_STATE = "productState";
        private const string PROTECTION_STATUS = "ProtectionStatus";
        private const string SECURITY_CENTER2_ROOT = @"Root/SecurityCenter2";
        private const string SELECT_ALL_FROM_ANTIVIRUS_PRODUCT = "SELECT * FROM AntivirusProduct";
        private const string SELECT_ALL_FROM_MSF_MPCOMPUTER_STATUS = "SELECT * FROM MSFT_MpComputerStatus";

        private static ManagementObjectSearcher GetManagementObjectSearcher(string scope, string query) => new ManagementObjectSearcher(scope, query);

        internal static bool AntiSpywareEnabled()
        {
            var scope = @"Root/Microsoft/Windows/Defender";
            var query = "SELECT * FROM MSFT_MpComputerStatus";
            var status = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First().Properties[ANTISPYWARE_ENABLED].Value;
            return (bool)status;
        }

        internal static bool DefenderProtectionDisabled()
        {
            var defender = GetAntiVirusProduct().Where(product => product.GetPropertyValue(DEFENDER_INSTANCE_GUID) as string == DEFENDER_GUID).First();
            var defenderState = string.Format("0x{0:x}", defender.GetPropertyValue(PRODUCT_STATE)).Substring(3, 2);
            return defenderState == "00" || defenderState == "01";
        }

        internal static bool DefenderWmiCacheIsValid()
        {
            try
            {
                var scope = @"Root/Microsoft/Windows/Defender";
                var query = "SELECT * FROM MSFT_MpComputerStatus";
                _ = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static string GetActivePowerPlanId()
        {
            var searcher = GetManagementObjectSearcher(@"Root\Cimv2\power", "SELECT * FROM Win32_PowerPlan WHERE IsActive = True");
            var activePlan = searcher.Get().Cast<ManagementBaseObject>().First();
            return (activePlan.GetPropertyValue("InstanceId") as string).Split('\\')[1];
        }

        internal static ManagementBaseObject GetAntiVirusInfo() => GetAntiVirusProduct().First(av => av.GetPropertyValue(DEFENDER_INSTANCE_GUID) as string != DEFENDER_GUID);

        internal static T GetAntiVirusInfo<T>(string property) => (T)GetAntiVirusInfo().GetPropertyValue(property);

        internal static IEnumerable<ManagementBaseObject> GetAntiVirusProduct() => GetManagementObjectSearcher(SECURITY_CENTER2_ROOT, SELECT_ALL_FROM_ANTIVIRUS_PRODUCT).Get().Cast<ManagementBaseObject>();

        internal static int GetBitLockerVolumeProtectionStatus()
        {
            var searcher = GetManagementObjectSearcher(@"Root\Cimv2\security\MicrosoftVolumeEncryption", "SELECT * FROM Win32_Encryptablevolume");
            var status = searcher.Get().Cast<ManagementBaseObject>().First().Properties[PROTECTION_STATUS].Value;
            return Convert.ToInt32(status);
        }

        internal static T GetComputerSystemInfo<T>(string propertyName)
        {
            var scope = @"Root\Cimv2";
            var query = "SELECT * FROM CIM_ComputerSystem";
            var info = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First();
            return (T)info.Properties[propertyName].Value;
        }

        internal static string GetDefenderAMEngineVersion() => GetMsftMpComputerStatus().GetPropertyValue(AM_ENGINE_VERSION) as string;

        internal static ManagementBaseObject GetMsftMpComputerStatus() => GetManagementObjectSearcher(DEFENDER_ROOT, SELECT_ALL_FROM_MSF_MPCOMPUTER_STATUS).Get().Cast<ManagementBaseObject>().First();

        internal static string GetVideoControllerDacType()
        {
            var scope = @"Root\Cimv2";
            var query = "SELECT * FROM CIM_VideoController";
            var adapter = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First();
            return adapter.Properties["AdapterDACType"].Value as string;
        }

        internal static bool HasExternalAntiVirus() => GetAntiVirusProduct().Count() > 1;

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

        internal static bool ProcessorVirtualizationIsEnabled()
        {
            var scope = @"Root\Cimv2";
            var query = "SELECT * FROM CIM_Processor";
            var processor = GetManagementObjectSearcher(scope, query).Get().Cast<ManagementBaseObject>().First();
            return (bool)processor.Properties["VirtualizationFirmwareEnabled"].Value;
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
    }
}