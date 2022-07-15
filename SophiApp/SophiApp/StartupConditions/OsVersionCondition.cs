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
            var ubr = OsHelper.GetUpdateBuildRevision();
            var hasProblem = OsHelper.IsWindows11()
                           ? CheckWin11Version(build, ubr)
                           : CheckWin10Version(build);

            return HasProblem = hasProblem.Invert();
        }

        private bool CheckWin10Version(ushort build) => build >= OsHelper.WIN10_MIN_SUPPORTED_BUILD & build <= OsHelper.WIN10_MAX_SUPPORTED_BUILD;

        private bool CheckWin11Version(ushort build, ushort ubr)
        {
            const uint win11SupportedVersion = 22000;
            const uint win11SupportedUbr = 739;

            return build < win11SupportedVersion
                  ? false
                  : build == win11SupportedVersion
                          ? ubr >= win11SupportedUbr
                          : true;
        }
    }
}