using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class DefenderWarningCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.DefenderWarning;

        public bool Invoke() => HasProblem = WmiHelper.AntiVirusProtectionDisabled();
    }
}