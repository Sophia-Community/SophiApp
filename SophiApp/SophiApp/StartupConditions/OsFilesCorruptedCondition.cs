using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.StartupConditions
{
    internal class OsFilesCorruptedCondition : IStartupCondition
    {
        private const string WINDOWS_UPDATE_SERVICE = "wuauserv";

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.OsFilesCorrupted;

        public bool Invoke()
        {
            return HasProblem = !WindowsDefenderHelper.AllServicesExist()
                    || !ServiceHelper.ServiceExist(WINDOWS_UPDATE_SERVICE);
        }
    }
}