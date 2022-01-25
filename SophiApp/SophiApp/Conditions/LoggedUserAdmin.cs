using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class LoggedUserAdmin : ICondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        private readonly string EXPLORER_PROCESS_NAME = "explorer";
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionLoggedUserAdmin;

        public bool Invoke()
        {
            var explorerIdentity = ProcessHelper.GetProcessIdentity(EXPLORER_PROCESS_NAME);
            var appIdentity = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME);
            return Result = explorerIdentity.TrueForAll(id => id.Name == appIdentity[0].Name) && appIdentity.Count == 1;
        }
    }
}