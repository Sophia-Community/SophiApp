using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class OnlyOneRun : ICondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionOnlyOneRun;

        public bool Invoke() => Result = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME).Count() == 1;
    }
}