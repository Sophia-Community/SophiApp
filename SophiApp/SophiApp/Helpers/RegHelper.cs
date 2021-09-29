using Microsoft.Win32;
using System;

namespace SophiApp.Helpers
{
    internal class RegHelper
    {
        private static RegistryKey GetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, GetRegistryView()).OpenSubKey(keyPath);

        private static RegistryView GetRegistryView() => Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

        private static RegistryKey SetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, GetRegistryView()).OpenSubKey(keyPath, true) ?? RegistryKey.OpenBaseKey(hive, GetRegistryView()).CreateSubKey(keyPath, true);

        internal static void DeleteKey(RegistryHive hive, string path, string name) => SetKey(hive, path).DeleteValue(name);

        internal static void DeleteSubKeyTree(RegistryHive hive, string subKey) => RegistryKey.OpenBaseKey(hive, GetRegistryView()).DeleteSubKeyTree(subKey, true);

        internal static byte[] GetByteArrayValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as byte[];

        internal static int? GetNullableIntValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as int?;

        internal static string GetStringValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as string;

        internal static object GetValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name);

        internal static bool KeyExist(RegistryHive hive, string path, string name) => (GetKey(hive, path)?.GetValue(name) is null).Invert();

        internal static void SetValue(RegistryHive hive, string path, string name, object value) => SetKey(hive, path).SetValue(name, value);

        internal static void SetValue(RegistryHive hive, string path, string name, object value, RegistryValueKind type) => SetKey(hive, path).SetValue(name, value, type);

        internal static bool SubKeyExist(RegistryHive hive, string path) => (GetKey(hive, path) is null).Invert();

        internal static void TryDeleteKey(RegistryHive hive, string path, string name)
        {
            if (KeyExist(hive, path, name))
                SetKey(hive, path).DeleteValue(name);
        }
    }
}