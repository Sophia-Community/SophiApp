// <copyright file="GroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Globalization;
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

        private static readonly string PolicyTemplatesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "PolicyDefinitions");

        // A leftover of the previous versions of the app
        private static readonly string LegacyLgpoSettings = Path.Combine(Path.GetTempPath(), "LGPO.txt");

        // The policy roots, and an ordinary key that has a setting which is also set as a policy (long paths).
        // Other ordinary settings from the templates exist by default, and they would turn into policies
        private static readonly (RegistryKey Hive, LGPOScope Scope, string Path)[] PolicyRoots =
        [
            (Registry.LocalMachine, LGPOScope.Computer, "SOFTWARE\\Policies"),
            (Registry.LocalMachine, LGPOScope.Computer, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies"),
            (Registry.LocalMachine, LGPOScope.Computer, "SYSTEM\\CurrentControlSet\\Policies"),
            (Registry.LocalMachine, LGPOScope.Computer, "SYSTEM\\CurrentControlSet\\Control\\FileSystem"),
            (Registry.CurrentUser, LGPOScope.User, "Software\\Policies"),
            (Registry.CurrentUser, LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies"),
        ];

        // The values written by Windows or apps, not by policies
        private static readonly (LGPOScope Scope, string Path, string Name, string Value)[] NotPolicies =
        [
            // Windows default
            (LGPOScope.User, "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer", "NoDriveTypeAutoRun", "145"),

            // Windows default
            (LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\Windows\\NetworkConnectivityStatusIndicator", "UseGlobalDNS", "1"),

            // Written by Windows during TPM provisioning
            (LGPOScope.Computer, "SOFTWARE\\Policies\\Microsoft\\TPM", "OSManagedAuthLevel", "5"),

            // Microsoft Edge "IE to Edge" browser helper object
            (LGPOScope.Computer, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Ext\\CLSID", "{1FD49718-1D00-4B19-AF5F-070AF6D5D54C}", "1"),

            // LongPathsEnabled exists by default, and only the turned on policy is taken
            (LGPOScope.Computer, "SYSTEM\\CurrentControlSet\\Control\\FileSystem", "LongPathsEnabled", "0"),
        ];

        private static Dictionary<LGPOScope, List<RegistryPolicy.Entry>>? policies;

        /// <inheritdoc/>
        public bool EditorExists => GpeditExists;

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
        public void RemovePolicies(LGPOScope scope, string path, params string[] names)
        {
            foreach (var name in names)
            {
                ClearPolicyCache(scope, path, name);
            }
        }

        /// <inheritdoc/>
        public void AddRegistryPolicies()
        {
            if (!GpeditExists)
            {
                return;
            }

            var templates = AdmxPolicies.Read(PolicyTemplatesFolder);

            foreach (var (hive, scope, path) in PolicyRoots)
            {
                AddKeyPolicies(hive, scope, path, templates);
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

        // Registry.pol has only these types
        private static string? GetPolicyType(RegistryValueKind kind) => kind switch
        {
            RegistryValueKind.DWord => "DWORD",
            RegistryValueKind.ExpandString => "EXSZ",
            RegistryValueKind.String => "SZ",
            _ => null,
        };

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
                catch (Exception ex)
                {
                    // Registry.pol cannot be read, so it is kept as is. An empty table means that there is nothing to change and to write back
                    App.Logger.LogSkippedException("Reading the Registry.pol files", ex);
                    policies = [];
                }
            }

            return policies.TryGetValue(scope, out entries!);
        }

        /// <summary>
        /// Adds the values of a registry key and of all its subkeys that belong to policies.
        /// </summary>
        private void AddKeyPolicies(RegistryKey hive, LGPOScope scope, string path, AdmxPolicies templates)
        {
            RegistryKey? key;

            try
            {
                key = hive.OpenSubKey(path);
            }
            catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException)
            {
                // A key that cannot be opened is skipped
                App.Logger.LogSkippedException($"Opening the registry key {hive.Name}\\{path}", ex);
                return;
            }

            if (key is null)
            {
                return;
            }

            using (key)
            {
                if (templates.HasKey(scope, path))
                {
                    foreach (var name in key.GetValueNames())
                    {
                        AddValuePolicy(key, scope, path, name, templates);
                    }
                }

                foreach (var subKey in key.GetSubKeyNames())
                {
                    AddKeyPolicies(hive, scope, $"{path}\\{subKey}", templates);
                }
            }
        }

        private void AddValuePolicy(RegistryKey key, LGPOScope scope, string path, string name, AdmxPolicies templates)
        {
            // The default value of a key has no name
            if (name.Length == 0 || !templates.Contains(scope, path, name))
            {
                return;
            }

            // The value is read as is, without expanding the environment variables in it
            var data = key.GetValue(name, null, RegistryValueOptions.DoNotExpandEnvironmentNames);

            if (data is null || GetPolicyType(key.GetValueKind(name)) is not string type)
            {
                return;
            }

            // A DWORD higher than 0x7FFFFFFF is read as a negative number
            var value = data is int number ? unchecked((uint)number).ToString(CultureInfo.InvariantCulture) : data.ToString() ?? string.Empty;

            var isNotPolicy = NotPolicies.Any(item => item.Scope == scope
                && item.Path.Equals(path, StringComparison.OrdinalIgnoreCase)
                && item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                && item.Value.Equals(value, StringComparison.OrdinalIgnoreCase));

            if (isNotPolicy)
            {
                return;
            }

            App.Logger.LogRegistryPolicyFound(key.Name, name, value);
            ClearPolicyCache(scope, path, name, type, value);
        }
    }
}
