using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class Win10TweakerCondition : IStartupCondition
    {
        private readonly string W10T_REGISTRY_PATH = @"Software\Win 10 Tweaker";

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.Win10Tweaker;

        public bool Invoke()
        {
            try
            {
                HasProblem = RegHelper.SubKeyExist(Microsoft.Win32.RegistryHive.CurrentUser, W10T_REGISTRY_PATH);
            }
            catch (System.Security.SecurityException)
            {
                HasProblem = true;
            }

            return HasProblem;
        }
    }
}