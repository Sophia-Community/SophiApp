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
            if (OsHelper.IsEnterpriseG)
            {
                return HasProblem;
            }

            if (WmiHelper.HasExternalAntiVirus())
            {
                var antivirusName = WmiHelper.GetAntiVirusInfo<string>("displayName");
                DebugHelper.FoundExternalAntiVirus(antivirusName);
                return HasProblem;
            }

            return HasProblem = WindowsDefenderHelper.IsCorrupted();
        }
    }
}