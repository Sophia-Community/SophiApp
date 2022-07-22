using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.IO;

namespace SophiApp.StartupConditions
{
    internal class OsFilesCorruptedCondition : IStartupCondition
    {
        private const string WINDOWS_UPDATE_SERVICE = "wuauserv";
        private static readonly string SMART_SCREEN_PATH = $@"{Environment.GetFolderPath(Environment.SpecialFolder.System)}\smartscreen.exe";

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.OsFilesCorrupted;

        public bool Invoke()
        {
            return HasProblem = !ServiceHelper.ServiceExist(WINDOWS_UPDATE_SERVICE) && !File.Exists(SMART_SCREEN_PATH);
        }
    }
}