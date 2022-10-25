using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class OsVersionCondition : IStartupCondition
    {
        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.OsVersion;

        public bool Invoke()
        {
            var build = OsHelper.GetBuild();
            return HasProblem = OsHelper.IsWindows11() ? build < OsHelper.WIN11_MIN_SUPPORTED_BUILD : build < OsHelper.WIN10_MIN_SUPPORTED_BUILD;
        }
    }
}