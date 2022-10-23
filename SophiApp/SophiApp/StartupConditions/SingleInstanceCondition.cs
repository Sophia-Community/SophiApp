using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class SingleInstanceCondition : IStartupCondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.SingleInstance;

        public bool Invoke() => HasProblem = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME).Count() > 1;
    }
}