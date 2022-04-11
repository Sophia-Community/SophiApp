using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class DefenderWarningCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.DefenderWarning;

        public bool Invoke()
        {
            if (WindowsDefenderHelper.DisabledByGroupPolicy().Invert())
            {
                if (WmiHelper.HasExternalAntiVirus())
                {
                    var antivirusName = WmiHelper.GetAntiVirusInfo<string>("displayName");
                    DebugHelper.FoundExternalAntiVirus(antivirusName);
                }
                else
                {
                    HasProblem = WmiHelper.DefenderProtectionIsDisabled();
                }
            }

            return HasProblem;
        }
    }
}