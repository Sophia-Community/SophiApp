using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.ServiceProcess;

namespace SophiApp.Actions
{
    public class CurrentStateAction
    {
        //TODO: Implement method selection by ID

        public static bool _100()
        {
            var serviceName = "DiagTrack";
            var serviceState = ServiceManager.GetState(serviceName);

            if (serviceState == ServiceControllerStatus.Running ||
                serviceState == ServiceControllerStatus.StartPending ||
                serviceState == ServiceControllerStatus.Stopped ||
                serviceState == ServiceControllerStatus.StopPending)
            {
                return serviceState == ServiceControllerStatus.Running || serviceState == ServiceControllerStatus.StartPending;
            }

            throw new Exception($"Get {serviceName} service state result is {serviceState}");
        }

        public static bool _102()
        {
            var telemetryLevel = Convert.ToByte(Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION).GetValue(RegistryPathManager.ALLOW_TELEMETRY));
            return telemetryLevel == 0 || telemetryLevel == 1;
        }

        public static bool _103()
        {
            var telemetryLevel = Convert.ToByte(Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION).GetValue(RegistryPathManager.ALLOW_TELEMETRY));
            return telemetryLevel == 3;
        }

        public static bool FOR_DEBUG_ONLY() => false;
    }
}