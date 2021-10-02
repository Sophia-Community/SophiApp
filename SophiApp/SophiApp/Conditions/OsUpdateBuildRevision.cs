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
    internal class OsUpdateBuildRevision : ICondition
    {
        private const ushort MIN_SUPPORT_UBR = 1151;

        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionUpdateBuildRevision;

        public bool Invoke() => Result = OsHelper.GetUpdateBuildRevision() >= MIN_SUPPORT_UBR;

    }
}
