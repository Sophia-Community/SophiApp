using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class OsBuildVersionCondition : IStartupCondition
    {
        private const ushort MIN_SUPPORT_UBR = 1151;

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.OsBuildVersion;

        public bool Invoke() => HasProblem = (OsHelper.GetUpdateBuildRevision() >= MIN_SUPPORT_UBR).Invert();
    }
}