using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class LoggedUserIsAdmin : ICondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        private readonly string EXPLORER_PROCESS_NAME = "explorer";
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionLoggedUserIsAdmin;

        public bool Invoke() => Result = ProcessHelper.GetProcessUser(EXPLORER_PROCESS_NAME).Name == ProcessHelper.GetProcessUser(APP_PROCESS_NAME).Name;
    }
}