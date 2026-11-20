// <copyright file="WslService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class WslService : IWslService
    {
        // https://github.com/microsoft/WSL/blob/master/distributions/DistributionInfo.json
        private const string DistributionsUrl = "https://raw.githubusercontent.com/microsoft/WSL/refs/heads/master/distributions/DistributionInfo.json";
        private const string InstalledDistributionsKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Lxss";
        private const double TimeoutSeconds = 5;

        private readonly IHttpService httpService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WslService"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        public WslService(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        /// <inheritdoc/>
        public IReadOnlyList<(string Family, string Name, string FriendlyName)> GetDistributions()
        {
            JsonObject info;

            try
            {
                info = httpService.GetFromJson<JsonObject>(DistributionsUrl, TimeoutSeconds);
            }
            catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or IOException or JsonException)
            {
                throw new CustomizationException(exceptionMessage: $"Url {DistributionsUrl} is not available", resourceKey: "UIModel_Exception_InternetConnectionRequired") { Arguments = [DistributionsUrl] };
            }

            var distributions = new List<(string Family, string Name, string FriendlyName)>();

            // The distributions are grouped by families: "Ubuntu" has several versions, and so on
            foreach (var family in info["ModernDistributions"]?.AsObject() ?? [])
            {
                foreach (var distribution in family.Value?.AsArray() ?? [])
                {
                    var name = (string?)distribution?["Name"];

                    if (!string.IsNullOrEmpty(name))
                    {
                        distributions.Add((family.Key, name, (string?)distribution?["FriendlyName"] ?? name));
                    }
                }
            }

            return distributions;
        }

        /// <inheritdoc/>
        public IReadOnlyList<string> GetInstalledDistributions()
        {
            using var key = Registry.CurrentUser.OpenSubKey(InstalledDistributionsKey);

            if (key is null)
            {
                return [];
            }

            var names = new List<string>();

            foreach (var subKeyName in key.GetSubKeyNames())
            {
                using var subKey = key.OpenSubKey(subKeyName);

                if (subKey?.GetValue("DistributionName") is string name)
                {
                    names.Add(name);
                }
            }

            return names;
        }

        /// <inheritdoc/>
        public void Install(string distribution)
        {
            // The installer works in its own console window: it shows the progress and asks for the name of the Linux user
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "wsl.exe"),
                ArgumentList = { "--install", "--distribution", distribution },
                UseShellExecute = true,
            });

            process?.WaitForExit();
        }
    }
}
