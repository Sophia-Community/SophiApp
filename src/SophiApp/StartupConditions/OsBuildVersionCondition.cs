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
            var buildRevision = OsHelper.GetUpdateBuildRevision();
            var buildVersion = OsHelper.GetBuild();
            return HasProblem = OsHelper.IsWindows11() ? CheckWindows11(buildVersion, buildRevision) : CheckWindows10(buildRevision);
        }

        private bool CheckWindows11(ushort buildVersion, ushort buildRevision)
        {
            if (buildVersion == OsHelper.WIN11_INSIDER_BUILD)
                return buildRevision < 1992;

            return buildVersion < 22621;
        }

        private bool CheckWindows10(ushort buildRevision)
        {
            return buildRevision < 3208;
        }
    }
}