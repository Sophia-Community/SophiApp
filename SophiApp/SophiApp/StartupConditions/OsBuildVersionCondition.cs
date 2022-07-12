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
            var build = OsHelper.GetBuild();
            var hasProblem = OsHelper.IsWindows11()
                             ? build >= OsHelper.WIN11_MIN_SUPPORTED_INSIDER_BUILD || build == OsHelper.WIN11_MIN_SUPPORTED_BUILD
                             : ubr >= OsHelper.WIN10_MIN_SUPPORTED_UBR;

            return HasProblem = hasProblem.Invert();
        }
    }
}