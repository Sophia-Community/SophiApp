using Microsoft.Win32;
using System;

namespace SophiApp.Helpers
{
    internal class RegHelper
    {
        internal static RegistryKey GetRegistryKey(RegistryHive hive, string keyPath)
        {
            RegistryKey regHive = RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            return string.IsNullOrEmpty(keyPath) ? null : regHive.OpenSubKey(keyPath);
        }

        internal static RegistryKey SetRegistryKey(RegistryHive hive, string keyPath)
        {
            RegistryKey regHive = RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);
            return string.IsNullOrEmpty(keyPath) ? null : regHive.OpenSubKey(keyPath, true);
        }
    }
}