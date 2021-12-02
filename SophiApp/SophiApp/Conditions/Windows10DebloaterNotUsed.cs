using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.IO;

namespace SophiApp.Conditions
{
    internal class Windows10DebloaterNotUsed : ICondition
    {
        private readonly string win10DebloaterDirectory = $@"{Environment.GetEnvironmentVariable("SystemDrive")}\Temp\Windows10Debloater";

        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionWindows10DebloaterNotUsed;

        public bool Invoke() => Result = Directory.Exists(win10DebloaterDirectory).Invert();
    }
}