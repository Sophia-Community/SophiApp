using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class DefenderIsBrokenCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.DefenderIsBroken;

        public bool Invoke() => HasProblem = WmiHelper.AntiVirusProtectionDisabled();
    }
}