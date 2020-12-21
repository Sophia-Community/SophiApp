using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using Microsoft.Win32;

namespace SophiAppCE.Actions
{
    internal class WindowsServices
    {
        internal enum StartupType
        {
            Boot = 0,
            System = 1,
            Automatic = 2,
            Manual = 3,
            Disabled = 4
        }

        public static bool GetStartupState(string serviceName) => (StartupType)Registry.LocalMachine
                                                                                         .OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{serviceName}")
                                                                                         .GetValue("Start") != StartupType.Disabled ? true : false;        
    }
}
