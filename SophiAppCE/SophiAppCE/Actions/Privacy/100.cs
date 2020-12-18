using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;

namespace SophiAppCE.Actions.Privacy
{
    class _100 : IApplicable
    {
        ServiceController diagTrack = new ServiceController("DiagTrack");
        public void Execute()
        {
            throw new NotImplementedException();
        }

        public bool State()
        {
            diagTrack.Refresh();
            return diagTrack.Status == ServiceControllerStatus.Running ? true : false;
        }
    }
}
