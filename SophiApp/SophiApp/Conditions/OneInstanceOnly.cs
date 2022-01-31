using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class OneInstanceOnly : ICondition
    {
        private readonly string APP_PROCESS_NAME = AppHelper.AppName;
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionOneInstanceOnly;

        public bool Invoke() => Result = ProcessHelper.GetProcessIdentity(APP_PROCESS_NAME).Count() == 1;
    }
}