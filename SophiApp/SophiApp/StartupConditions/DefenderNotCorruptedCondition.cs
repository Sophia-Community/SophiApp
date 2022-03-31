using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class DefenderNotCorruptedCondition : IStartupCondition
    {
        private const string AME_WRONG_VERSION = "0.0.0.0";

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.DefenderCorrupted;

        public bool Invoke() => WmiHelper.HasExternalAntiVirus() ? HasProblem = false : WmiHelper.GetDefenderAMEngineVersion() == AME_WRONG_VERSION;
    }
}