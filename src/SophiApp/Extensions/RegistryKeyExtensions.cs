// <copyright file="RegistryKeyExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using Microsoft.Win32;

    /// <summary>
    /// Implements <see cref="RegistryKey"/> extensions.
    /// </summary>
    public static class RegistryKeyExtensions
    {
        /// <summary>
        /// Retrieves or create, if missing, a specified subkey.
        /// </summary>
        /// <param name="key">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Path of the subkey to open or create.</param>
        public static RegistryKey OpenOrCreateSubKey(this RegistryKey key, string subKey)
        {
            return key.OpenSubKey(subKey, true) ?? key.CreateSubKey(subKey, true);
        }
    }
}
