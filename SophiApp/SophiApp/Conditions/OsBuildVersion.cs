using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Conditions
{
    internal class OsBuildVersion : ICondition
    {
        private const uint MIN_SUPPORT_BUILD = 19041;
        private const uint MAX_SUPPORT_BUILD = 19044;

        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionOsBuildVersion;

        public bool Invoke()
        {
            var build = OsHelper.GetBuild();
            return Result = build >= MIN_SUPPORT_BUILD && build <= MAX_SUPPORT_BUILD;
        }
    }
}
