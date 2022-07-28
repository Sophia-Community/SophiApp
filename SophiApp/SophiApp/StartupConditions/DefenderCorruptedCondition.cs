using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class DefenderCorruptedCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.DefenderCorrupted;

        public bool Invoke()
        {
            if (WmiHelper.HasExternalAntiVirus())
            {
                var antivirusName = WmiHelper.GetAntiVirusInfo<string>("displayName");
                DebugHelper.FoundExternalAntiVirus(antivirusName);
            }

            return HasProblem;
        }
    }
}