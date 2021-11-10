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

        public bool Invoke() => Result = ProcessHelper.GetProcessUser(EXPLORER_PROCESS_NAME).Name == ProcessHelper.GetProcessUser(APP_PROCESS_NAME).Name;
    }
}