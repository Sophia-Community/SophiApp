using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Threading;

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

            if (hasProblem.Invert())
            {
                try
                {
                    ComObjectHelper.EnableUpdateForOtherProducts();
                    Thread.Sleep(1000);
                    PowerShellHelper.GetUwpAppsUpdates();
                    Thread.Sleep(1000);
                    ComObjectHelper.SetWindowsUpdateDetectNow();
                }
                catch (Exception)
                {
                }
            }

            return HasProblem = hasProblem.Invert();
        }
    }
}