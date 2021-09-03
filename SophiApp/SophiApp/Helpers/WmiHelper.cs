using System;
using System.Linq;
using System.Management;

namespace SophiApp.Helpers
{
    internal class WmiHelper
    {
        private const string PROTECTION_STATUS = "ProtectionStatus";

        internal static int GetBitLockerVolumeProtectionStatus()
        {
            var searcher = new ManagementObjectSearcher(@"Root\cimv2\security\MicrosoftVolumeEncryption", "SELECT * FROM Win32_Encryptablevolume");
            var status = searcher.Get().Cast<ManagementBaseObject>().FirstOrDefault().Properties[PROTECTION_STATUS].Value;
            return Convert.ToInt32(status);
        }
    }
}