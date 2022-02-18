using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class OsBuildVersionCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.OsBuildVersion;

        public bool Invoke()
        {
            var ubr = OsHelper.GetUpdateBuildRevision();
            HasProblem = OsHelper.IsWindows11() ? ubr >= OsHelper.WIN11_MIN_SUPPORTED_UBR : ubr >= OsHelper.WIN10_MIN_SUPPORTED_UBR;
            return HasProblem.Invert();
        }
    }
}