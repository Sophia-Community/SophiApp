using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.StartupConditions
{
    internal class SpoolerNotCorruptedCondition : IStartupCondition
    {
        public bool HasProblem { get; set; } = true;
        public ConditionsTag Tag { get; set; } = ConditionsTag.SpoolerCorrupted;

        public bool Invoke()
        {
            const string SERVICE_SPOOLER = "Spooler";

            if (ServiceHelper.ServiceExist(SERVICE_SPOOLER))
            {
                return HasProblem = ServiceHelper.Get(SERVICE_SPOOLER).StartType == ServiceStartMode.Disabled;
            }

            return HasProblem;           
        }
    }
}
