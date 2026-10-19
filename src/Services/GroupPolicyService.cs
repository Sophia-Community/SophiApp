// <copyright file="GroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class GroupPolicyService : IGroupPolicyService
    {
        private readonly IProcessService processService;
        private readonly bool gpeditExists = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "gpedit.msc"));
        private readonly string lgpoExe = Path.Combine(AppContext.BaseDirectory, "Binaries", "LGPO.exe");
        private readonly string lgpoSettings = Environment.ExpandEnvironmentVariables("%TEMP%\\LGPO.txt");

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupPolicyService"/> class.
        /// </summary>
        /// <param name="processService">A service for working with Windows process./> API.</param>
        public GroupPolicyService(IProcessService processService)
        {
            this.processService = processService;
        }

        /// <inheritdoc/>
        public void DeleteLGPOConfig()
        {
            if (File.Exists(lgpoSettings))
            {
                File.Delete(lgpoSettings);
            }
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(RegistryKey registryKey, string subKey, string value)
        {
            registryKey.OpenSubKey(subKey, true)?.DeleteValue(value, false);
            registryKey.Dispose();
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(RegistryKey registryKey, string subKey, params string[] values)
        {
            var regKey = registryKey.OpenSubKey(subKey, true);

            foreach (var value in values)
            {
                regKey?.DeleteValue(value, false);
            }

            regKey?.Dispose();
            registryKey.Dispose();
        }

        /// <inheritdoc/>
        public void DeleteRegistryValue(string subKey, string value, params RegistryKey[] registryKeys)
        {
            foreach (var key in registryKeys)
            {
                key.OpenSubKey(subKey, true)?.DeleteValue(value, false);
                key.Dispose();
            }
        }

        /// <inheritdoc/>
        public void SetRegistryValue<T>(RegistryKey registryKey, string subKey, string name, T value, RegistryValueKind kind)
            where T : struct
        {
            registryKey.OpenSubKey(subKey, true)?.SetValue(name, value, kind);
            registryKey.Dispose();
        }

        /// <inheritdoc/>
        public void ClearPolicyCache(LGPOScope scope, string path, string name, string type = "", string value = "")
        {
            if (gpeditExists)
            {
                var settingValues = string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(value)
                    ? [scope.ToString(), path, name, "CLEAR", string.Empty]
                    : new string[5] { scope.ToString(), path, name, $"{type}:{value}", string.Empty };
                File.AppendAllLines(lgpoSettings, settingValues, System.Text.Encoding.UTF8);
            }
        }

        /// <inheritdoc/>
        public void ClearPolicyCache(string path, string name, params LGPOScope[] scopes)
        {
            if (gpeditExists)
            {
                var scopeValues = new List<string>();

                foreach (var scope in scopes)
                {
                    scopeValues.AddRange([scope.ToString(), path, name, "CLEAR", string.Empty]);
                }

                File.AppendAllLines(lgpoSettings, scopeValues, System.Text.Encoding.UTF8);
            }
        }

        /// <inheritdoc/>
        public void UpdatePolicy(bool deleteConfig = true)
        {
            if (File.Exists(lgpoSettings) && File.Exists(lgpoExe))
            {
                _ = processService.WaitForExit(name: lgpoExe, arguments: $"/t {lgpoSettings}");

                if (deleteConfig)
                {
                    File.Delete(lgpoSettings);
                }
            }
        }
    }
}
