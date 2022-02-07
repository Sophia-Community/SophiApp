using Microsoft.Win32;
using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;

namespace SophiApp.Conditions
{
    internal class NoRebootRequired : ICondition
    {
        private const string REBOOT_PENDING = "RebootPending";
        private const string REBOOT_REQUIRED = "RebootRequired";
        private const string SYSTEM_REBOOT_PENDING = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing";
        private const string UPDATE_REBOOT_REQUIRED = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update";

        public bool Result { get; set; }
        public string Tag { get; set; } = Tags.ConditionNoRebootRequired;

        public bool Invoke() => Result = RegHelper.KeyExist(RegistryHive.LocalMachine, SYSTEM_REBOOT_PENDING, REBOOT_PENDING).Invert()
                                            || RegHelper.KeyExist(RegistryHive.LocalMachine, UPDATE_REBOOT_REQUIRED, REBOOT_REQUIRED).Invert();
    }
}