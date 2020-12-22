using SophiAppCE.Helpers;
using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Actions.Privacy
{
    public class _100 : IAction
    {
        public Action Run => Action;

        public static void Action()
        {
            WinService.ReverseState("DiagTrack");
        }
    }
}
