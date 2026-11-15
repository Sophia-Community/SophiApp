// <copyright file="RegistryKey.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    /// <summary>
    /// Implements <see cref="Microsoft.Win32.RegistryKey"/> extensions.
    /// </summary>
    public static class RegistryKey
    {
        /// <summary>
        /// Sets a value in an existing subkey and closes the subkey. Does nothing if the subkey does not exist.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="kind">The registry data type.</param>
        public static void SetValueIfKeyExists(this Microsoft.Win32.RegistryKey key, string subKey, string name, object value, Microsoft.Win32.RegistryValueKind kind)
        {
            using var registryKey = key.OpenSubKey(subKey, true);
            registryKey?.SetValue(name, value, kind);
        }

        /// <summary>
        /// Sets a value in a subkey and closes the subkey. The subkey is created if it does not exist.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="kind">The registry data type.</param>
        public static void CreateKeyAndSetValue(this Microsoft.Win32.RegistryKey key, string subKey, string name, object value, Microsoft.Win32.RegistryValueKind kind)
        {
            using var registryKey = key.OpenSubKey(subKey, true) ?? key.CreateSubKey(subKey, true);
            registryKey.SetValue(name, value, kind);
        }

        /// <summary>
        /// Deletes a value from a subkey and closes the subkey. Does nothing if the subkey or the value does not exist.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        /// <param name="name">The name of the value.</param>
        public static void DeleteSubKeyValue(this Microsoft.Win32.RegistryKey key, string subKey, string name)
        {
            using var registryKey = key.OpenSubKey(subKey, true);
            registryKey?.DeleteValue(name, false);
        }

        /// <summary>
        /// Reads a value from a subkey and closes the subkey.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        /// <param name="name">The name of the value.</param>
        /// <returns>The value or null if the subkey or the value does not exist.</returns>
        public static object? GetSubKeyValue(this Microsoft.Win32.RegistryKey key, string subKey, string name)
        {
            using var registryKey = key.OpenSubKey(subKey);
            return registryKey?.GetValue(name);
        }

        /// <summary>
        /// Determines whether a subkey exists.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        public static bool SubKeyExists(this Microsoft.Win32.RegistryKey key, string subKey)
        {
            using var registryKey = key.OpenSubKey(subKey);
            return registryKey is not null;
        }

        /// <summary>
        /// Reads the names of all values of a subkey and closes the subkey.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey.</param>
        /// <returns>The value names or an empty array if the subkey does not exist.</returns>
        public static string[] GetSubKeyValueNames(this Microsoft.Win32.RegistryKey key, string subKey)
        {
            using var registryKey = key.OpenSubKey(subKey);
            return registryKey?.GetValueNames() ?? [];
        }
    }
}
