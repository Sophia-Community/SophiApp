using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.ServiceProcess;

namespace SophiApp.Actions
{
    internal class SystemStateAction
    {
        //TODO: Implement system state methods

        public static void _100()
        {
            var serviceName = "DiagTrack";
            var service = ServiceManager.GetService(serviceName);

            switch (service.Status)
            {
                case ServiceControllerStatus.Running:
                    service.Stop();
                    break;

                case ServiceControllerStatus.Stopped:
                    service.Start();
                    break;

                default:
                    throw new Exception($"Set {serviceName} service state result is {service.Status}");
            }
        }

        public static void _102()
        {
            const string ENTERPRISE_EDITION = "Enterprise";
            const string EDUCATION_EDITION = "Education";

            var osEdition = OsManager.GetEditionId();

            if (osEdition.Contains(ENTERPRISE_EDITION) || osEdition == EDUCATION_EDITION)
            {
                Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION, writable: true).SetValue(RegistryPathManager.ALLOW_TELEMETRY, 0);
                return;
            }

            Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION, writable: true).SetValue(RegistryPathManager.ALLOW_TELEMETRY, 1);
        }

        public static void _103() => Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION, writable: true).SetValue(RegistryPathManager.ALLOW_TELEMETRY, 3);

        public static void FOR_DEBUG_ONLY()
        {
        }
    }
}