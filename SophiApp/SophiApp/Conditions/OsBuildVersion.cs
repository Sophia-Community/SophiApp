using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class OsBuildVersion : ICondition
    {
        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionOsBuildVersion;

        public bool Invoke()
        {
            var build = OsHelper.GetBuild();
            return Result = build == OsHelper.WIN11_SUPPORT_BUILD || build >= OsHelper.WIN10_MIN_SUPPORT_BUILD;
        }
    }
}