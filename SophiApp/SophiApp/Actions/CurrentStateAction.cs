using Microsoft.Win32;
using SophiApp.Helpers;
using System;
using System.Threading;

namespace SophiApp.Actions
{
    public class CurrentStateAction
    {
        //TODO: Implement method selection by ID
        public static bool _102()
        {
            var telemetryLevel = Convert.ToByte(Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION).GetValue(RegistryPathManager.ALLOW_TELEMETRY));
            return telemetryLevel == 0 || telemetryLevel == 1;
        }

        public static bool _103()
        {
            var telemetryLevel = Convert.ToByte(Registry.LocalMachine.OpenSubKey(RegistryPathManager.DATA_COLLECTION).GetValue(RegistryPathManager.ALLOW_TELEMETRY));
            return telemetryLevel == 3;
        }



        public static bool FOR_DEBUG_ONLY()
        {
            Thread.Sleep(700); //TODO: For debug only !!!
            return new Random().Next(0, 2) == 1;
        }
    }
}