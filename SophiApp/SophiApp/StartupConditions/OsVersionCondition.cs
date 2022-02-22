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
            var hasProblem = build >= OsHelper.WIN11_MIN_SUPPORTED_BUILD || (build >= OsHelper.WIN10_MIN_SUPPORTED_BUILD & build <= OsHelper.WIN10_MAX_SUPPORTED_BUILD);
            return HasProblem = hasProblem.Invert();
        }
    }
}