using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    class WinService
    {
        internal enum StartupType
        {
            Boot = 0,
            System = 1,
            Automatic = 2,
            Manual = 3,
            Disabled = 4
        }

        internal enum StateType
        {
            ContinuePending = 5,
            Paused = 7,
            PausePending = 6,
            Running = 4,
            StartPending = 2,
            Stopped = 1,
            StopPending = 3
        }

        internal static StartupType GetStartupState(string serviceName)
        {
            return (StartupType)Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{serviceName}").GetValue("Start");
        }

        internal static void SetStartupState(string serviceName, StartupType startupType)
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{serviceName}", true))
            {
                registryKey.SetValue("Start", startupType, RegistryValueKind.DWord);
            }
        }

         internal static void ReverseState(string serviceName)
        {
            if (GetStartupState(serviceName) == StartupType.Disabled)
                SetStartupState(serviceName, StartupType.Automatic);
            else
                SetStartupState(serviceName, StartupType.Disabled);

            ServiceController service = new ServiceController(serviceName);

            if (service.Status == ServiceControllerStatus.Running)
                service.Stop();

            if (service.Status == ServiceControllerStatus.Stopped)
                service.Start();
        }
    }
}
