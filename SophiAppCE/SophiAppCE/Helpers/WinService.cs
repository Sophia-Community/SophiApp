using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

        internal static StartupType GetStartupState(string serviceName) => (StartupType)Registry.LocalMachine
                                                                                                .OpenSubKey($"SYSTEM\\CurrentControlSet\\Services\\{serviceName}")
                                                                                                .GetValue("Start");
                                                                                         
    }
}
