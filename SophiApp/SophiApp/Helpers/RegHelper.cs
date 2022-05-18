using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace SophiApp.Helpers
{
    internal class RegHelper
    {
        private static RegistryKey GetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, RegistryView.Registry64)?.OpenSubKey(keyPath);

        private static RegistryKey SetKey(RegistryHive hive, string keyPath) => RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(keyPath, true) ?? RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).CreateSubKey(keyPath, true);

        internal static void DeleteKey(RegistryHive hive, string path, string name) => SetKey(hive, path).DeleteValue(name, false);

        internal static void DeleteKey(RegistryHive hive, string path, string name, bool throwOnMissingValue) => SetKey(hive, path).DeleteValue(name, throwOnMissingValue);

        internal static void DeleteSubKeyTree(RegistryHive hive, string subKey) => RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).DeleteSubKeyTree(subKey, true);

        internal static byte[] GetByteArrayValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as byte[];

        internal static byte GetByteValue(RegistryHive hive, string path, string name) => Convert.ToByte(GetKey(hive, path).GetValue(name));

        internal static byte? GetNullableByteValue(RegistryHive hive, string path, string name) => (GetKey(hive, path)?.GetValue(name)) is null ? null as byte? : Convert.ToByte(GetKey(hive, path).GetValue(name));

        internal static int? GetNullableIntValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as int?;

        internal static string GetStringValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name) as string;

        internal static IEnumerable<string> GetSubKeyNames(RegistryHive hive, string path)
        {
            foreach (var name in GetKey(hive, path).GetSubKeyNames())
            {
                yield return $@"{path}\{name}";
            }
        }

        internal static object GetValue(RegistryHive hive, string path, string name) => GetKey(hive, path)?.GetValue(name);

        internal static bool KeyExist(RegistryHive hive, string path, string name) => (GetKey(hive, path)?.GetValue(name) is null).Invert();

        internal static void SetValue(RegistryHive hive, string path, string name, object value) => SetKey(hive, path).SetValue(name, value);

        internal static void SetValue(RegistryHive hive, string path, IEnumerable<string> subKeyNames, string keyName, object value, RegistryValueKind type)
        {
            foreach (var name in subKeyNames)
                SetValue(hive, $@"{path}\{name}", keyName, value, type);
        }

        internal static void SetValue(RegistryHive hive, string path, string name, object value, RegistryValueKind type) => SetKey(hive, path).SetValue(name, value, type);

        internal static bool SubKeyExist(RegistryHive hive, string path) => (GetKey(hive, path) is null).Invert();

        internal static void TryDeleteKey(RegistryHive hive, IEnumerable<string> paths, string name)
        {
            foreach (var path in paths)
                DeleteKey(hive, path, name, false);
        }

        internal static void TryDeleteKey(RegistryHive hive, string path, params string[] names)
        {
            foreach (var name in names)
                DeleteKey(hive, path, name, false);
        }

        internal static void TryDeleteSubKeyTree(RegistryHive hive, string subKey) => RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).DeleteSubKeyTree(subKey, false);

        internal static void TryDeleteValue(RegistryHive hive, string path, string name)
        {
            if (KeyExist(hive, path, name))
                SetKey(hive, path).DeleteValue(name);
        }
    }
}