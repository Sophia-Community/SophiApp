// <copyright file="NetworkAdapterService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Diagnostics;
    using System.Globalization;
    using System.Management;
    using System.Management.Automation;
    using System.Text;
    using System.Xml.Linq;

    /// <inheritdoc/>
    public class NetworkAdapterService : INetworkAdapterService
    {
        // The cmdlets return CIM objects with numeric property values
        // InterfaceOperationalStatus of MSFT_NetAdapter: the adapter status is "Up"
        private const uint OperationalStatusUp = 1;

        // NdisPhysicalMedium of MSFT_NetAdapter: Native 802.11
        // https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/hh968170(v=vs.85)
        private const uint WiFiPhysicalMedium = 9;

        // IPv4Connectivity and IPv6Connectivity of MSFT_NetConnectionProfile: Internet
        private const uint InternetConnectivity = 4;

        // AllowComputerToTurnOffDevice of MSFT_NetAdapterPowerManagementSettingData
        private const uint TurnOffDeviceUnsupported = 0;
        private const uint TurnOffDeviceEnabled = 2;

        private readonly IPowerShellService powerShellService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkAdapterService"/> class.
        /// </summary>
        /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
        public NetworkAdapterService(IPowerShellService powerShellService)
        {
            this.powerShellService = powerShellService;
        }

        /// <inheritdoc/>
        public bool? GetTurnOffDevice()
        {
            // WMI answers ten times faster than the cmdlets, which have to load their module first
            if (GetTurnOffDeviceFromWmi() is { } stateFromWmi)
            {
                return stateFromWmi;
            }

            // Check whether adapters have AllowComputerToTurnOffDevice property to manage
            bool? allowTurnOff = null;

            foreach (var (_, state) in GetPowerManagement(GetPhysicalUpAdapters()))
            {
                allowTurnOff = state == TurnOffDeviceEnabled;
            }

            return allowTurnOff;
        }

        /// <inheritdoc/>
        public void SetTurnOffDevice(bool enable)
        {
            // PowerShell is used only to call the cmdlets
            var physicalAdapters = GetPhysicalUpAdapters();
            var adapterNames = GetPowerManagement(physicalAdapters).Select(settings => settings.Name).ToList();

            // Check whether adapters have AllowComputerToTurnOffDevice property to manage
            if (adapterNames.Count == 0)
            {
                return;
            }

            string? ssid = null;
            string? profileName = null;

            // Wi-Fi adapters whose power management will be changed
            var wiFiAdapters = physicalAdapters.Where(adapter => adapter.IsWiFi && adapterNames.Contains(adapter.Name)).Select(adapter => adapter.Name).ToList();

            // Check whether PC is currently connected to a Wi-Fi network
            if (wiFiAdapters.Count > 0)
            {
                // Get currently connected Wi-Fi network SSID
                ssid = GetConnectedSsid();

                if (ssid is null)
                {
                    // Network shell commands need location permission to access WLAN information
                    ExplorerHelper.Open("ms-settings:privacy-location");
                    return;
                }

                // Get the WLAN profile name by SSID, since they may differ. If the profile was not found, SSID is used
                profileName = GetWlanProfileName(ssid) ?? ssid;
            }

            // Check whether PC has Internet access before adapters restart
            // If not (no Internet or NCSI active probing is disabled), we will wait only until adapters are up
            var hadInternet = HasInternetAccess();
            var state = enable ? "Enabled" : "Disabled";

            foreach (var adapterName in adapterNames)
            {
                // Set-NetAdapterPowerManagement has no parameter for this property, so it is changed in the object that is passed to the cmdlet
                _ = powerShellService.Invoke($"Get-NetAdapterPowerManagement -Name {ToArgument(adapterName)} | ForEach-Object -Process {{$_.AllowComputerToTurnOffDevice = '{state}'; $_ | Set-NetAdapterPowerManagement}}");
            }

            WaitForRestart(adapterNames);
            WaitForConnection(adapterNames, wiFiAdapters, hadInternet, ssid, profileName);
        }

        /// <summary>
        /// Converts the adapter name to a cmdlet argument: wildcard characters are escaped, and the name is quoted.
        /// </summary>
        /// <param name="adapterName">The adapter name.</param>
        private static string ToArgument(string adapterName) => $"'{WildcardPattern.Escape(adapterName).Replace("'", "''")}'";

        /// <summary>
        /// Reads the AllowComputerToTurnOffDevice state of the connected physical adapters from the WMI classes the cmdlets use.
        /// </summary>
        /// <returns>The state, or null if it cannot be read this way.</returns>
        private static bool? GetTurnOffDeviceFromWmi()
        {
            try
            {
                bool? allowTurnOff = null;
                using var adapters = new ManagementObjectSearcher("root/StandardCimv2", "SELECT Name, InterfaceOperationalStatus FROM MSFT_NetAdapter WHERE ConnectorPresent = TRUE AND Hidden = FALSE");

                foreach (var adapter in adapters.Get().Cast<ManagementBaseObject>())
                {
                    if (Convert.ToUInt32(adapter["InterfaceOperationalStatus"] ?? 0u) != OperationalStatusUp || adapter["Name"] is not string name)
                    {
                        continue;
                    }

                    var adapterName = name.Replace("\\", "\\\\").Replace("'", "\\'");
                    using var settings = new ManagementObjectSearcher("root/StandardCimv2", $"SELECT AllowComputerToTurnOffDevice FROM MSFT_NetAdapterPowerManagementSettingData WHERE Name = '{adapterName}'");

                    foreach (var setting in settings.Get().Cast<ManagementBaseObject>())
                    {
                        var state = Convert.ToUInt32(setting["AllowComputerToTurnOffDevice"] ?? TurnOffDeviceUnsupported);

                        if (state != TurnOffDeviceUnsupported)
                        {
                            allowTurnOff = state == TurnOffDeviceEnabled;
                        }
                    }
                }

                return allowTurnOff;
            }
            catch (Exception ex) when (ex is ManagementException || ex is System.Runtime.InteropServices.COMException || ex is UnauthorizedAccessException)
            {
                // The cmdlets will be asked
                return null;
            }
        }

        private static string? GetConnectedSsid()
        {
            // The "SSID" label is not localized. "BSSID" lines are skipped, because they do not start with "SSID"
            var line = RunNetsh("wlan show interfaces")
                .Split('\n')
                .Select(text => text.Trim())
                .FirstOrDefault(text => text.StartsWith("SSID", StringComparison.Ordinal) && text.Contains(':'));

            return line?.Split(':', 2)[1].Trim();
        }

        private static string? GetWlanProfileName(string ssid)
        {
            try
            {
                var profilesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Microsoft\\Wlansvc\\Profiles\\Interfaces");

                foreach (var file in Directory.EnumerateFiles(profilesFolder, "*.xml", SearchOption.AllDirectories))
                {
                    var profile = XDocument.Load(file).Root;
                    var profileSsid = profile?.Descendants().FirstOrDefault(element => element.Name.LocalName == "SSID")?.Elements().FirstOrDefault(element => element.Name.LocalName == "name")?.Value;

                    if (string.Equals(profileSsid, ssid, StringComparison.OrdinalIgnoreCase))
                    {
                        return profile!.Elements().FirstOrDefault(element => element.Name.LocalName == "name")?.Value;
                    }
                }
            }
            catch (Exception)
            {
                // WLAN profiles cannot be read
            }

            return null;
        }

        private static string RunNetsh(string arguments)
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "netsh.exe"), arguments)
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                StandardOutputEncoding = GetOemEncoding(),
            };
            _ = process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private static Encoding GetOemEncoding()
        {
            try
            {
                // Console programs write their output in the OEM code page
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                return Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
            }
            catch (Exception)
            {
                return Encoding.Default;
            }
        }

        /// <summary>
        /// Gets physical network adapters whose status is "Up".
        /// </summary>
        private List<(string Name, bool IsWiFi)> GetPhysicalUpAdapters()
        {
            return powerShellService.Invoke("Get-NetAdapter -Physical -ErrorAction Ignore")
                .Where(adapter => adapter is not null && adapter.GetPropertyValue<uint>("InterfaceOperationalStatus") == OperationalStatusUp)
                .Select(adapter => (Name: adapter.GetPropertyValue<string>("Name") ?? string.Empty, IsWiFi: adapter.GetPropertyValue<uint>("NdisPhysicalMedium") == WiFiPhysicalMedium))
                .Where(adapter => adapter.Name.Length > 0)
                .ToList();
        }

        /// <summary>
        /// Gets names and the AllowComputerToTurnOffDevice property state of the adapters that have this property to manage.
        /// </summary>
        /// <param name="adapters">Network adapters.</param>
        private List<(string Name, uint State)> GetPowerManagement(IEnumerable<(string Name, bool IsWiFi)> adapters)
        {
            return adapters
                .SelectMany(adapter => powerShellService.Invoke($"Get-NetAdapterPowerManagement -Name {ToArgument(adapter.Name)} -ErrorAction Ignore")
                    .Where(powerManagement => powerManagement is not null)
                    .Select(powerManagement => (adapter.Name, State: powerManagement.GetPropertyValue<uint>("AllowComputerToTurnOffDevice"))))
                .Where(settings => settings.State != TurnOffDeviceUnsupported)
                .ToList();
        }

        private int CountUpAdapters(List<string> adapterNames)
        {
            try
            {
                return GetPhysicalUpAdapters().Count(adapter => adapterNames.Contains(adapter.Name));
            }
            catch (Exception)
            {
                // The adapters cannot be enumerated while their drivers are reloading
                return 0;
            }
        }

        private bool HasInternetAccess()
        {
            try
            {
                return powerShellService.Invoke("Get-NetConnectionProfile -ErrorAction Ignore")
                    .Any(profile => profile is not null && (profile.GetPropertyValue<uint>("IPv4Connectivity") == InternetConnectivity || profile.GetPropertyValue<uint>("IPv6Connectivity") == InternetConnectivity));
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Waits until at least one adapter goes down: adapters restart asynchronously and stay "Up" for a few seconds after their settings were changed.
        /// </summary>
        /// <param name="adapterNames">Names of the adapters whose settings were changed.</param>
        private void WaitForRestart(List<string> adapterNames)
        {
            // Some drivers don't restart the adapter at all, so wait 10 seconds at most
            var timer = Stopwatch.StartNew();
            int upAdapters;

            do
            {
                Thread.Sleep(500);

                // An adapter may disappear while its driver is reloading, so count only adapters that are up
                upAdapters = CountUpAdapters(adapterNames);
            }
            while (upAdapters == adapterNames.Count && timer.Elapsed.TotalSeconds < 10);
        }

        /// <summary>
        /// Waits until Internet access is restored, or until adapters are up if there was no Internet access before. 60 seconds at most.
        /// </summary>
        /// <param name="adapterNames">Names of the adapters whose settings were changed.</param>
        /// <param name="wiFiAdapters">Names of the Wi-Fi adapters among them.</param>
        /// <param name="hadInternet">Whether PC had Internet access before the adapters restart.</param>
        /// <param name="ssid">SSID of the Wi-Fi network that was used, if any.</param>
        /// <param name="profileName">WLAN profile name of that network.</param>
        private void WaitForConnection(List<string> adapterNames, List<string> wiFiAdapters, bool hadInternet, string? ssid, string? profileName)
        {
            var timer = Stopwatch.StartNew();
            var attempt = 0;

            while (timer.Elapsed.TotalSeconds < 60)
            {
                var isReady = hadInternet ? HasInternetAccess() : CountUpAdapters(adapterNames) == adapterNames.Count;

                if (isReady)
                {
                    break;
                }

                // If Wi-Fi network was used and Wi-Fi adapter is not connected yet, connect to it again
                // Retry every 10 seconds in case the adapter was not ready
                if (ssid is not null && attempt % 5 == 0 && CountUpAdapters(wiFiAdapters) == 0)
                {
                    _ = RunNetsh($"wlan connect name=\"{profileName}\" ssid=\"{ssid}\"");
                }

                Thread.Sleep(2000);
                attempt++;
            }
        }
    }
}
