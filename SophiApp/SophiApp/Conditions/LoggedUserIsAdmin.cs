using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SophiApp.Conditions
{
    internal class LoggedUserIsAdmin : ICondition
    {
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionLoggedUserIsAdmin;

        public bool Invoke()
        {
         // https://stackoverflow.com/questions/777548/how-do-i-determine-the-owner-of-a-process-in-c
        }
    }
}
