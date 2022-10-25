using Microsoft.Win32;
using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Conditions
{
    internal class RebootRequiredCondition : IStartupCondition
    {
        private const string CBS_PACKAGES_PENDING = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\PackagesPending";
        private const string CBS_REBOOT_IN_PROGRESS = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootInProgress";
        private const string CBS_REBOOT_PENDING = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending";
        private const string UPDATE_POST_REBOOT = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\PostRebootReporting";
        private const string UPDATE_REBOOT_REQUIRED = @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired";

        public bool HasProblem { get; set; }
        public ConditionsTag Tag { get; set; } = ConditionsTag.RebootRequired;

        public bool Invoke()
        {
            var registryRebootRequired = new List<bool>
            {
                RegHelper.SubKeyExist(RegistryHive.LocalMachine, CBS_PACKAGES_PENDING), RegHelper.SubKeyExist(RegistryHive.LocalMachine, CBS_REBOOT_PENDING),
                RegHelper.SubKeyExist(RegistryHive.LocalMachine, CBS_REBOOT_IN_PROGRESS), RegHelper.SubKeyExist(RegistryHive.LocalMachine, UPDATE_POST_REBOOT),
                RegHelper.SubKeyExist(RegistryHive.LocalMachine, UPDATE_REBOOT_REQUIRED)
            };

            return HasProblem = registryRebootRequired.Any(key => key == true);
        }
    }
}