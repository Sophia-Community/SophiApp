using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class IsSingleSession : ICondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        private readonly string EXPLORER_PROCESS_NAME = "explorer";
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionIsSingleSession;

        public bool Invoke()
        {
            var explorerIdentity = ProcessHelper.GetProcessIdentity(EXPLORER_PROCESS_NAME);
            var appIdentity = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME).First();
            return Result = explorerIdentity.All(id => id.Name == appIdentity.Name);
        }
    }
}