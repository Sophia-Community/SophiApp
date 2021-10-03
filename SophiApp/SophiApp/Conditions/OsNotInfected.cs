using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class OsNotInfected : ICondition
    {
        private readonly string W10T_REGISTRY_PATH = @"Software\Win 10 Tweaker";

        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionOsNotInfected;

        public bool Invoke() => Result = RegHelper.SubKeyExist(Microsoft.Win32.RegistryHive.CurrentUser, W10T_REGISTRY_PATH).Invert();
    }
}