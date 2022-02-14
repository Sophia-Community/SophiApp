using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class SingleAdminSessionCondition : IStartupCondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        private readonly string EXPLORER_PROCESS_NAME = "explorer";
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.SingleAdminSession;

        public bool Invoke()
        {
            var explorerIdentity = ProcessHelper.GetProcessIdentity(EXPLORER_PROCESS_NAME);
            var appIdentity = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME).First();
            var isSingleSession = explorerIdentity.All(id => id.Name == appIdentity.Name);
            return HasProblem = isSingleSession.Invert();
        }
    }
}