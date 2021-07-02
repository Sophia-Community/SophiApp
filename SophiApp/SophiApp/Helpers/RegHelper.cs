using Microsoft.Win32;
using System;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class RegHelper
    {
        private static RegistryKey GetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).OpenSubKey(keyPath);

        private static RegistryKey SetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).CreateSubKey(keyPath, true);

        internal static void DeleteKey(RegistryHive hive, string path, string name) => SetKey(hive, path).DeleteValue(name);

        internal static void DeleteSubKeyTree(RegistryHive hive, string subKey) => RegistryKey.OpenBaseKey(hive, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32).DeleteSubKeyTree(subKey);

        internal static RegistryKey GetSubKey(RegistryHive hive, string path) => GetKey(hive, path);

        internal static string GetValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as string;

        internal static bool KeyExist(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValueNames().Contains(name) ?? false;

        internal static RegistryKey SetSubKey(RegistryHive hive, string path) => SetKey(hive, path);

        internal static void SetValue(RegistryHive hive, string path, string name, string value) => SetKey(hive, path).SetValue(name, value);

        internal static void SetValue(RegistryHive hive, string path, string name, string value, RegistryValueKind type) => SetKey(hive, path).SetValue(name, value, type);
    }
}