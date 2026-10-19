// <copyright file="IGroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using Microsoft.Win32;
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with group policy API.
    /// </summary>
    public interface IGroupPolicyService
    {
        /// <summary>
        /// Delete "%TEMP\LGPO.txt" config file if exist.
        /// </summary>
        void DeleteLGPOConfig();

        /// <summary>
        /// Delete registry value to make changes visible in group policy UI.
        /// </summary>
        /// <param name="registryKey">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Registry subkey path to open.</param>
        /// <param name="value">The name of the value to delete.</param>
        void DeleteRegistryValue(RegistryKey registryKey, string subKey, string value);

        /// <summary>
        /// Delete registry value to make changes visible in group policy UI.
        /// </summary>
        /// <param name="registryKey">Represents a key-level node in the Windows registry.</param>
        /// <param name="subKey">Registry subkey path to open.</param>
        /// <param name="values">The name of the values to delete.</param>
        void DeleteRegistryValue(RegistryKey registryKey, string subKey, params string[] values);

        /// <summary>
        /// Delete registry value to make changes visible in group policy UI.
        /// </summary>
        /// <param name="subKey">Registry subkey path to open.</param>
        /// <param name="value">The name of the values to delete.</param>
        /// <param name="registryKeys">Represents a key-level nodes in the Windows registry.</param>
        void DeleteRegistryValue(string subKey, string value, params RegistryKey[] registryKeys);

        /// <summary>
        /// Set registry value to clear group policy cache and make changes visible in UI.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="value"/>.</typeparam>
        /// <param name="registryKey">Represents a key-level nodes in the Windows registry.</param>
        /// <param name="subKey">Registry subkey path to open.</param>
        /// <param name="name">The name of the value to be stored.</param>
        /// <param name="value">The data to be stored.</param>
        /// <param name="kind">Specifies the data types to use when storing values in the registry.</param>
        public void SetRegistryValue<T>(RegistryKey registryKey, string subKey, string name, T value, RegistryValueKind kind)
            where T : struct;

        /// <summary>
        /// Call LGPO.exe to make changes in "C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database.
        /// </summary>
        /// <param name="scope">Policy scope.</param>
        /// <param name="path">Policy path.</param>
        /// <param name="name">Policy value name.</param>
        /// <param name="type">Policy value type.</param>
        /// <param name="value">Policy value.</param>
        void ClearPolicyCache(LGPOScope scope, string path, string name, string type = "", string value = "");

        /// <summary>
        /// Call LGPO.exe to make changes in "C:\Windows\System32\GroupPolicy\Machine\Registry.pol" or "C:\Windows\System32\GroupPolicy\User\Registry.pol" database.
        /// </summary>
        /// <param name="path">Policy path.</param>
        /// <param name="name">Policy value name.</param>
        /// <param name="scopes">Policy scopes.</param>
        void ClearPolicyCache(string path, string name, params LGPOScope[] scopes);

        /// <summary>
        /// Update group policy cache using by LGPO.txt file.
        /// </summary>
        /// <param name="deleteConfig">Delete LGPO.txt file after update.</param>
        void UpdatePolicy(bool deleteConfig = true);
    }
}
