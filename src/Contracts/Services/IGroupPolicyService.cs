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
        /// Gets a value indicating whether Windows has the Local Group Policy Editor snap-in (gpedit.msc). Windows Home edition does not.
        /// </summary>
        bool EditorExists { get; }

        /// <summary>
        /// Forget the policy changes that were not written yet, and delete the "%TEMP%\LGPO.txt" file left by the previous versions of the app.
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
        /// Set the policy value, or remove the policy if the type and the value are not specified. The changes are written to the Registry.pol file by <see cref="UpdatePolicy"/>.
        /// </summary>
        /// <param name="scope">Policy scope.</param>
        /// <param name="path">Policy path.</param>
        /// <param name="name">Policy value name.</param>
        /// <param name="type">Policy value type.</param>
        /// <param name="value">Policy value.</param>
        void ClearPolicyCache(LGPOScope scope, string path, string name, string type = "", string value = "");

        /// <summary>
        /// Remove the policy in the specified scopes. The changes are written to the Registry.pol files by <see cref="UpdatePolicy"/>.
        /// </summary>
        /// <param name="path">Policy path.</param>
        /// <param name="name">Policy value name.</param>
        /// <param name="scopes">Policy scopes.</param>
        void ClearPolicyCache(string path, string name, params LGPOScope[] scopes);

        /// <summary>
        /// Remove several policy values of one key. The changes are written to the Registry.pol file by <see cref="UpdatePolicy"/>.
        /// </summary>
        /// <param name="scope">Policy scope.</param>
        /// <param name="path">Policy path.</param>
        /// <param name="names">Policy value names.</param>
        void RemovePolicies(LGPOScope scope, string path, params string[] names);

        /// <summary>
        /// Find the registry values that belong to policies, even set by hand, and add them to the policies,
        /// so the Local Group Policy Editor shows them. The changes are written to the Registry.pol files by <see cref="UpdatePolicy"/>.
        /// </summary>
        void AddRegistryPolicies();

        /// <summary>
        /// Write the changed policies to "%SystemRoot%\System32\GroupPolicy\Machine\Registry.pol" and "%SystemRoot%\System32\GroupPolicy\User\Registry.pol" files.
        /// </summary>
        void UpdatePolicy();
    }
}
