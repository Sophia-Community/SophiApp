// <copyright file="GroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using RegistryKey = Microsoft.Win32.RegistryKey;

    /// <inheritdoc/>
    public class GroupPolicyService : IGroupPolicyService
    {
        // The policies are shared by all instances of the service and are written to Registry.pol once, after all functions were applied
        private static readonly Lock PoliciesLock = new();
        private static readonly bool GpeditExists = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "gpedit.msc"));
        private static readonly string MachinePolicyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "GroupPolicy\\Machine\\Registry.pol");
        private static readonly string UserPolicyFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "GroupPolicy\\User\\Registry.pol");
        private static readonly string GptIniFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "GroupPolicy\\gpt.ini");

        // A leftover of the previous versions of the app
        private static readonly string LegacyLgpoSettings = Path.Combine(Path.GetTempPath(), "LGPO.txt");
        private static Dictionary<LGPOScope, List<RegistryPolicy.Entry>>? policies;

        /// <inheritdoc/>
        public void DeleteLGPOConfig()
        {
            // Forget the changes that were not written. The policies will be read from Registry.pol again
            ResetPolicies();
            LegacyLgpoSettings.TryDelete();
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(RegistryKey registryKey, string subKey, string value)
        {
            registryKey.DeleteSubKeyValue(subKey, value);
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(RegistryKey registryKey, string subKey, params string[] values)
        {
            using var regKey = registryKey.OpenSubKey(subKey, true);

            foreach (var value in values)
            {
                regKey?.DeleteValue(value, false);
            }
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(string subKey, string value, params RegistryKey[] registryKeys)
        {
            foreach (var key in registryKeys)
            {
                key.DeleteSubKeyValue(subKey, value);
            }
        }

        /// <inheritdoc/>
        public void SetRegistryValue<T>(RegistryKey registryKey, string subKey, string name, T value, RegistryValueKind kind)
            where T : struct
        {
            registryKey.SetValueIfKeyExists(subKey, name, value, kind);
        }

        /// <inheritdoc/>
        public void ClearPolicyCache(LGPOScope scope, string path, string name, string type = "", string value = "")
        {
            lock (PoliciesLock)
            {
                if (!TryGetPolicies(scope, out var entries))
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(value))
                {
                    // No type and value: remove the policy
                    RegistryPolicy.Remove(entries, path, name);
                    return;
                }

                // Otherwise set the policy value
                RegistryPolicy.Set(entries, path, name, type, value);
            }
        }

        /// <inheritdoc/>
        public void ClearPolicyCache(string path, string name, params LGPOScope[] scopes)
        {
            foreach (var scope in scopes)
            {
                ClearPolicyCache(scope, path, name);
            }
        }

        /// <inheritdoc/>
        public void UpdatePolicy()
        {
            lock (PoliciesLock)
            {
                if (policies is null || policies.Count == 0)
                {
                    return;
                }

                try
                {
                    // Registry.pol is recreated from scratch, so it has the policies from the list only. If the list is empty, the file is removed
                    // Write returns true if the file was changed
                    var machineChanged = RegistryPolicy.Write(MachinePolicyFile, policies[LGPOScope.Computer]);
                    var userChanged = RegistryPolicy.Write(UserPolicyFile, policies[LGPOScope.User]);

                    // Windows applies the policies only if the gpt.ini version was increased
                    if (machineChanged || userChanged)
                    {
                        RegistryPolicy.UpdateGptIni(GptIniFile, machineChanged, userChanged);
                    }
                }
                catch (Exception ex)
                {
                    // The policies are still set in the registry. Only the policy editor will not show them
                    App.Logger.LogPolicyUpdateException(ex);
                }
                finally
                {
                    // Policies will be read again if any function is applied after
                    ResetPolicies();
                }
            }
        }

        private static void ResetPolicies()
        {
            lock (PoliciesLock)
            {
                policies = null;
            }
        }

        /// <summary>
        /// Gets the policies of the scope. All current policies are read from Registry.pol files once.
        /// </summary>
        /// <param name="scope">Policies scope.</param>
        /// <param name="entries">Policies of the scope.</param>
        private static bool TryGetPolicies(LGPOScope scope, out List<RegistryPolicy.Entry> entries)
        {
            entries = [];

            // Windows Home edition doesn't have the Local Group Policy Editor snap-in (gpedit.msc)
            if (!GpeditExists)
            {
                return false;
            }

            if (policies is null)
            {
                try
                {
                    policies = new Dictionary<LGPOScope, List<RegistryPolicy.Entry>>
                    {
                        [LGPOScope.Computer] = RegistryPolicy.Read(MachinePolicyFile),
                        [LGPOScope.User] = RegistryPolicy.Read(UserPolicyFile),
                    };
                }
                catch (Exception)
                {
                    // Registry.pol cannot be read, so it is kept as is. An empty table means that there is nothing to change and to write back
                    policies = [];
                }
            }

            return policies.TryGetValue(scope, out entries!);
        }
    }
}
